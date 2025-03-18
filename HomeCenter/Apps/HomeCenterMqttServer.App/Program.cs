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
using MQTTnet.Rx.Server;
using Microsoft.AspNetCore.SignalR;

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
        /*
         * This server starts a simple MQTT server which will store all retained messages in a file.
         */

        var storePath = Path.Combine(Path.GetTempPath(), "RetainedMessages.json");

        // var mqttFactory = new MqttFactory();
        // Due to security reasons the "default" endpoint (which is unencrypted) is not enabled by default!
        // var mqttServerOptions = mqttFactory.CreateServerOptionsBuilder().WithDefaultEndpoint().Build();

        //var certificate = CreateSelfSignedCertificate("1.3.6.1.5.5.7.3.1");
        //var mqttServerOptions = new MqttServerOptionsBuilder().WithEncryptionCertificate(certificate).WithEncryptedEndpoint().Build();

        _logger.LogMessagePublished += Logger_LogMessagePublished;

        var server = Create.MqttServer(builder => builder
            .WithDefaultEndpointPort(2883)
            .WithDefaultEndpoint()
            .WithKeepAlive()
            .Build())            
            .Subscribe(/*async*/ subscription => {
                subscription.Disposable.Add(subscription.Server.ClientConnected().Subscribe(args => Console.WriteLine($"SERVER: ClientConnectedAsync => clientId:{args.ClientId}")));
                subscription.Disposable.Add(subscription.Server.ClientDisconnected().Subscribe(args => Console.WriteLine($"SERVER: ClientDisconnectedAsync => clientId:{args.ClientId}")));
                System.Diagnostics.Debug.WriteLine("Subscribed.");
            });
                
        Console.WriteLine("Press Enter to exit.");
        Console.ReadLine();
        server.Dispose();
    }
}