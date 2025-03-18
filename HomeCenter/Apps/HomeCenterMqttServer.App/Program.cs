// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using MQTTnet.Diagnostics.Logger;
// using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using MQTTnet.AspNetCore;
using Microsoft.AspNetCore.Hosting.Server;

namespace HomeCenter.Mqtt.Server;

internal partial class Program
{
    private static readonly MqttNetEventLogger _logger = new();

    static X509Certificate2 CreateSelfSignedCertificate(string oid)
    {
        var sanBuilder = new SubjectAlternativeNameBuilder();
        sanBuilder.AddIpAddress(IPAddress.Loopback);
        sanBuilder.AddIpAddress(IPAddress.IPv6Loopback);
        sanBuilder.AddDnsName("localhost");

        using (var rsa = RSA.Create())
        {
            var certRequest = new CertificateRequest("CN=localhost", rsa, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);

            certRequest.CertificateExtensions.Add(
                new X509KeyUsageExtension(X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature, false));

            certRequest.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection { new(oid) }, false));

            certRequest.CertificateExtensions.Add(sanBuilder.Build());

            using (var certificate = certRequest.CreateSelfSigned(DateTimeOffset.Now.AddMinutes(-10), DateTimeOffset.Now.AddMinutes(10)))
            {
                var pfxCertificate = new X509Certificate2(
                    certificate.Export(X509ContentType.Pfx),
                    (string)null!,
                    X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);

                return pfxCertificate;
            }
        }
    }

    private static void Logger_LogMessagePublished(object? sender, MqttNetLogMessagePublishedEventArgs e)
    {
        // System.Console.WriteLine(e.LogMessage);
        System.Diagnostics.Debug.WriteLine(e.LogMessage);
    }

    private static async Task Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseKestrel(
                    o =>
                    {
                        o.ListenAnyIP(1883, l => l.UseMqtt()); // MQTT pipeline
                        o.ListenAnyIP(5000); // Default HTTP pipeline
                    });

                webBuilder.UseStartup<Startup>();
            }).Build().RunAsync();

        _logger.LogMessagePublished += Logger_LogMessagePublished;

        // Due to security reasons the "default" endpoint (which is unencrypted) is not enabled by default! Use .WithDefaultEndpoint()

        //var certificate = CreateSelfSignedCertificate("1.3.6.1.5.5.7.3.1");
        //var mqttServerOptions = new MqttServerOptionsBuilder().WithEncryptionCertificate(certificate).WithEncryptedEndpoint().Build();

        Console.WriteLine("Press Enter to exit.");
        Console.ReadLine();
    }
}