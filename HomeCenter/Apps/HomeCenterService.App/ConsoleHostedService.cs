// using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Diagnostics;
using MQTTnet.Extensions.ManagedClient;
//using MQTTnet.Extensions.WebSocket4Net;

using HueApi;
// using HueApi.Models;
using HueApi.Models.Responses;
using System.Text.Json.Nodes;
using HueApi.Models;
using HomeCenter.Mqtt.Server;
using System.Data;

// MOVEMENT ANALYSIS STUFF:
// Movement analysis: https://github.com/mie-lab/trackintel
// https://github.com/sandialabs/tracktable
// https://github.com/MobilityDB/MobilityDB
// https://github.com/movetk/movetk


internal partial class Program
{
    internal sealed class ConsoleHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        private int? _exitCode;
        // private readonly IConfiguration _configuration;

        private readonly Dictionary<string, string> _ipNames = new() { { "", "unknown" }, { "192.168.0.179", "downstairs" }, { "192.168.1.15", "upstairs" } };

        private const string ipDownstairs = "192.168.0.179";
        private const string keyDownstairs = "Xj9OWvQTPvvQLKkGm2uRX9t8-cMHseznTkpYEztA";
        private const string ipUpstairs = "192.168.1.15";
        private const string keyUpstairs = "w7G-n8c5cWdwXSMzn2C0X1fyJ0CyAGmwcV8s-dCz";

        private IManagedMqttClient _managedMqttClient;
        private readonly Dictionary<string, string> _deviceNames = [];

        public ConsoleHostedService(
            ILogger<ConsoleHostedService> logger,
            /*IConfiguration configuration,*/
            IHostApplicationLifetime appLifetime)
        {
            _logger = logger;
            // _configuration = configuration;
            _appLifetime = appLifetime;
        }

        private Task SubscriptionsResultAsync(SubscriptionsChangedEventArgs arg, ref bool subscribed)
        {
            foreach (var mqttClientSubscribeResult in arg.SubscribeResult)
            {
                _logger.LogDebug($"Subscription reason {mqttClientSubscribeResult.ReasonString}");
                foreach (var item in mqttClientSubscribeResult.Items)
                {
                    _logger.LogDebug($"For topic filter {item.TopicFilter}, result code: {item.ResultCode}");

                    if (item.TopicFilter.Topic == "Topic" && item.ResultCode == MqttClientSubscribeResultCode.GrantedQoS0 && !subscribed)
                    {
                        subscribed = true;
                    }
                }
            }

            foreach (var mqttClientUnsubscribeResult in arg.UnsubscribeResult)
            {
                _logger.LogDebug($"Unsubscription reason {mqttClientUnsubscribeResult.ReasonString}");
                foreach (var item in mqttClientUnsubscribeResult.Items)
                {
                    _logger.LogDebug($"For topic filter {item.TopicFilter}, result code: {item.ResultCode}");
                }
            }

            return Task.CompletedTask;
        }

        private void EventStreamMessage(string bridgeIp, List<EventStreamResponse> events)
        {
            try
            {
                var loc = _ipNames[bridgeIp];

                foreach (var hueEvent in events)
                {
                    if (hueEvent.Data.Count <= 0)
                    {
                        _logger.LogDebug("Data?");
                    }
                    else
                    {
                        foreach (var data in hueEvent.Data)
                        {
                            var dn = "?";
                            if (!string.IsNullOrEmpty(data.IdV1) && _deviceNames != null && _deviceNames.TryGetValue("/" + loc + data.IdV1, out string? value))
                                dn = value;
                            switch (data.Type)
                            {
                                case "light_level":
                                    data.ExtensionData["light"].GetProperty("light_level").TryGetDecimal(out decimal l);
                                    _logger.LogDebug($"{loc}: light level {l} on /{loc}{data.IdV1} ({dn})");
                                    _managedMqttClient.EnqueueAsync("SensorGrid", $"{loc}: light level {l} on /{loc}{data.IdV1} ({dn})", qualityOfServiceLevel: MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce, retain: false);
                                    break;
                                case "light":
                                    _logger.LogDebug($"{_ipNames[bridgeIp]}: light on /{loc}{data.IdV1} ({dn})");
                                    _managedMqttClient.EnqueueAsync("SensorGrid", $"{_ipNames[bridgeIp]}: light on /{loc}{data.IdV1} ({dn})", qualityOfServiceLevel: MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce, retain: false);
                                    break;
                                case "temperature":
                                    data.ExtensionData["temperature"].GetProperty("temperature").TryGetDecimal(out decimal t);

                                    var temperatureAsJTSTimeSeries = ToJTSTimeSeries("Temperature", t, loc, data.IdV1, data.CreationTime, dn);

                                    _logger.LogDebug($"{temperatureAsJTSTimeSeries}");
                                    _managedMqttClient.EnqueueAsync("SensorGrid", $"{temperatureAsJTSTimeSeries}", qualityOfServiceLevel: MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce, retain: false);
                                    break;
                                case "motion":
                                    _logger.LogDebug($"{loc}: motion on /{loc}{data.IdV1} ({dn})");
                                    _managedMqttClient.EnqueueAsync("SensorGrid", $"{loc}: motion on /{loc}{data.IdV1} ({dn})", qualityOfServiceLevel: MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce, retain: false);
                                    break;
                                case "grouped_motion":
                                    // _logger.LogDebug($"{_ipNames[bridgeIp]}: grouped motion");
                                    break;
                                default:
                                    _logger.LogDebug($"{loc}: unprocessed type {data.Type} ({dn})");
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private string ToJTSTimeSeries(string valueType, decimal v, string loc /* upstairs or downstairs */, string? idV1, DateTimeOffset? creationTime, string dn /* sensor name */)
        {
            var jts = new JTSRoot();
            jts.Header.RecordCount = 1;
            jts.Header.Columns.H0.Id = Guid.NewGuid().ToString();
            switch (valueType)
            {
                case "Temperature":
                    jts.Header.Columns.H0.Name = "Temperature";
                    jts.Header.Columns.H0.DataType = "NUMBER";
                    jts.Header.Columns.H0.RenderType = "VALUE";
                    jts.Header.Columns.H0.Format = "0.###";
                    jts.Header.Columns.H0.Aggregate = "NONE";
                    var d = new JTSData() { Ts = DateTime.Now };
                    d.F.F0.Value = (double)v;
                    d.F.F0.Quality = 100;
                    d.F.F0.Annotation = dn;
                    jts.Data.Add(d);
                    break;
            }
            var json = System.Text.Json.JsonSerializer.Serialize(jts);
            return json;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Starting with arguments: {string.Join(" ", Environment.GetCommandLineArgs())}");

            _appLifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        /*
                        while (true) {
                            _logger.LogDebug("Trying");
                            Thread.Sleep(1000);
                        }
                        */
                        var mqttFactory = new MqttFactory();
                        //mqttFactory.UseWebSocket4Net();

                        var subscribed = false;

                        var logger = new MqttNetEventLogger();
                        logger.LogMessagePublished += Logger_LogMessagePublished;

                        using (_managedMqttClient = mqttFactory.CreateManagedMqttClient(logger))
                        {
                            var mqttClientOptions = new MqttClientOptionsBuilder()
                                .WithTcpServer("lvmini"/*, 8883*/) // default 1883, encrypted 8883
                                .Build();

                            var managedMqttClientOptions = new ManagedMqttClientOptionsBuilder()
                                .WithClientOptions(mqttClientOptions)
                                .Build();

                            await _managedMqttClient.StartAsync(managedMqttClientOptions);

                            // The application message is not sent. It is stored in an internal queue and
                            // will be sent when the client is connected.
                            // await _managedMqttClient.EnqueueAsync("Topic", "Payload");

                            _logger.LogDebug("The managed MQTT client is connected.");

                            // Wait until the queue is fully processed.
                            SpinWait.SpinUntil(() => _managedMqttClient.PendingApplicationMessagesCount == 0, 10000);

                            _logger.LogDebug($"Pending messages: {_managedMqttClient.PendingApplicationMessagesCount}");

                            _managedMqttClient.SubscriptionsChangedAsync += args => SubscriptionsResultAsync(args, ref subscribed);
                            await _managedMqttClient.SubscribeAsync("Topic").ConfigureAwait(false);

                            SpinWait.SpinUntil(() => subscribed, 1000);
                            _logger.LogDebug($"Subscription (timeout 1s): {subscribed}");

                            _logger.LogDebug($"Connecting to {ipDownstairs} with key: {keyDownstairs}");
                            var localHueClientDownstairs = new LocalHueApi(ipDownstairs, keyDownstairs);

                            _logger.LogDebug($"Connecting to {ipUpstairs} with key: {keyUpstairs}");
                            var localHueClientUpstairs = new LocalHueApi(ipUpstairs, keyUpstairs);

                            // _logger.LogDebug("Getting all resources...");

                            var resourcesDownstairs = await localHueClientDownstairs.GetResourcesAsync();
                            //var txt = System.Text.Json.JsonSerializer.Serialize(resourcesDownstairs);
                            //System.IO.File.WriteAllText("DownstairsInfo.txt", txt);

                            // var rootsDownstairs = resourcesDownstairs.Data.Where(x => x.Owner == null);

                            var resourcesUpstairs = await localHueClientUpstairs.GetResourcesAsync();
                            //txt = System.Text.Json.JsonSerializer.Serialize(resourcesUpstairs);
                            //System.IO.File.WriteAllText("UpInfo.txt", txt);

                            var downstairs = await localHueClientDownstairs.GetDevicesAsync();
                            foreach (var device in downstairs.Data)
                            {
                                if (!string.IsNullOrEmpty(device.IdV1) && device.Metadata != null)
                                    _deviceNames.Add("/" + _ipNames[ipDownstairs] + device.IdV1, device.Metadata.Name);
                            }

                            var devicesUpstairs = await localHueClientUpstairs.GetDevicesAsync();
                            foreach (var device in devicesUpstairs.Data)
                            {
                                if (!string.IsNullOrEmpty(device.IdV1) && device.Metadata != null)
                                    _deviceNames.Add("/" + _ipNames[ipUpstairs] + device.IdV1, device.Metadata.Name);
                            }

                            LinkDependentDevices(resourcesDownstairs);
                            LinkDependentDevices(resourcesUpstairs);

                            localHueClientDownstairs.OnEventStreamMessage += EventStreamMessage;
                            localHueClientDownstairs.StartEventStream();

                            localHueClientUpstairs.OnEventStreamMessage += EventStreamMessage;
                            localHueClientUpstairs.StartEventStream();

                            _logger.LogInformation("Waiting for Hue Bridge events (press any key to stop)...");
                            //await Task.Delay(TimeSpan.FromHours(1));
                            Console.ReadLine();

                            localHueClientDownstairs.StopEventStream();
                            localHueClientUpstairs.StopEventStream();

                            _logger.LogInformation("Stopped listening for Hue Bridge events...");
                            _exitCode = 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unhandled exception!");
                        _exitCode = 1;
                    }
                    finally
                    {
                        // Stop the application once the work is done
                        _appLifetime.StopApplication();
                    }
                });
            });

            return Task.CompletedTask;
        }

        private void LinkDependentDevices(HueResponse<HueResource> resources)
        {
            var devicesDownstairs = resources.Data.Where(d => d.Type == "device" && !string.IsNullOrEmpty(d.IdV1) && d.IdV1.Contains("/sensors/"));
            foreach (var sensor in devicesDownstairs)
            {
                var t = sensor.Services?.Where(s => s.Rtype == "temperature")?.FirstOrDefault()?.Rid;
                if (t != null)
                {
                    var ts = resources.Data.Where(d => d.Id == t).FirstOrDefault();
                    if (ts != null)
                    {
                        if (!string.IsNullOrEmpty(sensor.IdV1) && sensor.Metadata != null)
                            _deviceNames.Add("/" + _ipNames[ipDownstairs] + ts.IdV1, sensor.Metadata.Name);
                    }
                }
                var l = sensor.Services?.Where(s => s.Rtype == "light_level")?.FirstOrDefault()?.Rid;
                if (l != null)
                {
                    var ls = resources.Data.Where(d => d.Id == l).FirstOrDefault();
                    if (ls != null)
                    {
                        if (!string.IsNullOrEmpty(sensor.IdV1) && sensor.Metadata != null)
                            _deviceNames.Add("/" + _ipNames[ipDownstairs] + ls.IdV1, sensor.Metadata.Name);
                    }
                }
            }
        }

        private void Logger_LogMessagePublished(object? sender, MqttNetLogMessagePublishedEventArgs e)
        {
            // System.Console.WriteLine(e.LogMessage);
            System.Diagnostics.Debug.WriteLine(e.LogMessage);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Exiting with return code: {_exitCode}");

            // Exit code may be null if the user cancelled via Ctrl+C/SIGTERM
            Environment.ExitCode = _exitCode.GetValueOrDefault(-1);
            return Task.CompletedTask;
        }
    }
}