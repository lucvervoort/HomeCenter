// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text.Json;
using MQTTnet;
using MQTTnet.Packets;
using MQTTnet.Diagnostics;

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

        var mqttFactory = new MqttFactory();
        //mqttFactory.UseWebSocket4Net();

        // Due to security reasons the "default" endpoint (which is unencrypted) is not enabled by default!
        var mqttServerOptions = mqttFactory.CreateServerOptionsBuilder().WithDefaultEndpoint().Build();

        //var certificate = CreateSelfSignedCertificate("1.3.6.1.5.5.7.3.1");
        //var mqttServerOptions = new MqttServerOptionsBuilder().WithEncryptionCertificate(certificate).WithEncryptedEndpoint().Build();

        _logger.LogMessagePublished += Logger_LogMessagePublished;

        using (var server = mqttFactory.CreateMqttServer(mqttServerOptions, _logger))
        {
            // Attach the event handler.
            server.ClientAcknowledgedPublishPacketAsync += e =>
            {
                _logger.Publish(MqttNetLogLevel.Verbose, "MqttServer", $"Client '{e.ClientId}' acknowledged packet {e.PublishPacket.PacketIdentifier} with topic '{e.PublishPacket.Topic}'", null, null);

                // It is also possible to read additional data from the client response. This requires casting the response packet.
                var qos1AcknowledgePacket = e.AcknowledgePacket as MqttPubAckPacket;
                _logger.Publish(MqttNetLogLevel.Verbose, "MqttServer", $"QoS 1 reason code: {qos1AcknowledgePacket?.ReasonCode}", null, null);

                var qos2AcknowledgePacket = e.AcknowledgePacket as MqttPubCompPacket;
                _logger.Publish(MqttNetLogLevel.Verbose, "MqttServer", $"QoS 2 reason code: {qos1AcknowledgePacket?.ReasonCode}", null, null);
                return MQTTnet.Internal.CompletedTask.Instance;
            };

            // Make sure that the server will load the retained messages.
            server.LoadingRetainedMessageAsync += async eventArgs =>
            {
                try
                {
                    var models = await JsonSerializer.DeserializeAsync<List<MqttRetainedMessageModel>>(File.OpenRead(storePath)) ?? [];
                    var retainedMessages = models.Select(m => m.ToApplicationMessage()).ToList();

                    eventArgs.LoadedRetainedMessages = retainedMessages;
                    _logger.Publish(MqttNetLogLevel.Verbose, "MqttServer", "Retained messages loaded.", null, null);
                }
                catch (FileNotFoundException)
                {
                    // Ignore because nothing is stored yet.
                    _logger.Publish(MqttNetLogLevel.Verbose, "MqttServer", "No retained messages stored yet.", null, null);
                }
                catch (Exception exception)
                {
                    _logger.Publish(MqttNetLogLevel.Verbose, "MqttServer", exception.Message, null, exception);
                }
            };

            // Make sure to persist the changed retained messages.
            server.RetainedMessageChangedAsync += async eventArgs =>
            {
                try
                {
                    // This sample uses the property _StoredRetainedMessages_ which will contain all(!) retained messages.
                    // The event args also contain the affected retained message (property ChangedRetainedMessage). This can be
                    // used to write all retained messages to dedicated files etc. Then all files must be loaded and a full list
                    // of retained messages must be provided in the loaded event.

                    var models = eventArgs.StoredRetainedMessages.Select(MqttRetainedMessageModel.Create);

                    var buffer = JsonSerializer.SerializeToUtf8Bytes(models);
                    await File.WriteAllBytesAsync(storePath, buffer);
                    _logger.Publish(MqttNetLogLevel.Verbose, "MqttServer", "Retained messages saved.", null, null);
                }
                catch (Exception exception)
                {
                    _logger.Publish(MqttNetLogLevel.Verbose, "MqttServer", exception.Message, null, exception);
                }
            };

            // Make sure to clear the retained messages when they are all deleted via API.
            server.RetainedMessagesClearedAsync += _ =>
            {
                File.Delete(storePath);
                return Task.CompletedTask;
            };

            await server.StartAsync();

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
        }
    }
}