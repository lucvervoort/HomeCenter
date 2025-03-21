﻿// using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using HueApi;
using HueApi.Models.Responses;
using HueApi.Models;
using HomeCenter.Mqtt.Server;
using System.Data;
using MQTTnet.Diagnostics.Logger;
using MQTTnet.Rx.Client;
using System.Reactive.Linq;
using MQTTnet;

// MOVEMENT ANALYSIS STUFF:
// Movement analysis: https://github.com/mie-lab/trackintel
// https://github.com/sandialabs/tracktable
// https://github.com/MobilityDB/MobilityDB
// https://github.com/movetk/movetk


internal partial class Program
{
    internal static void RecurringTask(Action action, int seconds, CancellationToken token)
    {
        if (action == null)
            return;
        Task.Run(async () => {
            while (!token.IsCancellationRequested)
            {
                action();
                await Task.Delay(TimeSpan.FromSeconds(seconds), token);
            }
        }, token);
    }

    internal sealed class ConsoleHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        private int? _exitCode;
        // private readonly IConfiguration _configuration;

        private readonly Dictionary<string, string> _ipNames = new() { { "", "unknown" }, { "192.168.0.179", "downstairs" }, { "192.168.1.30", "upstairs" } };

        private IDisposable? _disposable = default;
        private IResilientMqttClient _mqttClient;
        private string _serverIp = "localhost";
        private int _serverPort = 1883;

        private const string ipDownstairs = "192.168.0.179";
        private const string keyDownstairs = "Xj9OWvQTPvvQLKkGm2uRX9t8-cMHseznTkpYEztA";
        private const string ipUpstairs = "192.168.1.30"; // was .15
        private const string keyUpstairs = "w7G-n8c5cWdwXSMzn2C0X1fyJ0CyAGmwcV8s-dCz";

        private readonly Dictionary<string, string> _deviceNames = [];

        private readonly Dictionary<string, decimal> _currentDeviceLightLevel = [];
        private readonly Dictionary<string, decimal> _currentDeviceTemperature = [];

        public ConsoleHostedService(
            ILogger<ConsoleHostedService> logger,
            /*IConfiguration configuration,*/
            IHostApplicationLifetime appLifetime)
        {
            _logger = logger;
            // _configuration = configuration;
            _appLifetime = appLifetime;
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
                                    var lightLevelAsJTSTimeSeries = ToJTSTimeSeries("LightLevel", l, loc, data.IdV1, data.CreationTime, dn);
                                    _currentDeviceLightLevel[dn] = l;
                                    _logger.LogDebug($"{loc}: light level {l} on /{loc}{data.IdV1} ({dn})");
                                    _mqttClient.EnqueueAsync(new MqttApplicationMessage() { Topic = "SensorGrid", PayloadSegment = System.Text.Encoding.ASCII.GetBytes($"{lightLevelAsJTSTimeSeries}"), ContentType = "string", QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce, Retain = false });
                                    break;
                                case "light":
                                    var lightAsJTSTimeSeries = ToJTSTimeSeries("Light", 1, loc, data.IdV1, data.CreationTime, dn);
                                    _logger.LogDebug($"{_ipNames[bridgeIp]}: light on /{loc}{data.IdV1} ({dn})");
                                    _mqttClient.EnqueueAsync(new MqttApplicationMessage() { Topic = "SensorGrid", PayloadSegment = System.Text.Encoding.ASCII.GetBytes($"{lightAsJTSTimeSeries}"), ContentType = "string", QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce, Retain = false }/*"SensorGrid", $"{lightAsJTSTimeSeries}", qualityOfServiceLevel: MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce, retain: false*/);
                                    break;
                                case "temperature":
                                    data.ExtensionData["temperature"].GetProperty("temperature").TryGetDecimal(out decimal t);
                                    var temperatureAsJTSTimeSeries = ToJTSTimeSeries("Temperature", t, loc, data.IdV1, data.CreationTime, dn);
                                    _currentDeviceTemperature[dn] = t;
                                    _logger.LogDebug($"{temperatureAsJTSTimeSeries}");
                                    _mqttClient.EnqueueAsync(new MqttApplicationMessage() { Topic = "SensorGrid", PayloadSegment = System.Text.Encoding.ASCII.GetBytes($"{temperatureAsJTSTimeSeries}"), ContentType = "string", QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce, Retain = false }/*"SensorGrid", $"{temperatureAsJTSTimeSeries}", qualityOfServiceLevel: MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce, retain: false*/);
                                    break;
                                case "motion":
                                    var motionAsJTSTimeSeries = ToJTSTimeSeries("Motion", 1, loc, data.IdV1, data.CreationTime, dn);
                                    _logger.LogDebug($"{loc}: motion on /{loc}{data.IdV1} ({dn})");
                                    _mqttClient.EnqueueAsync(new MqttApplicationMessage() { Topic = "SensorGrid", PayloadSegment = System.Text.Encoding.ASCII.GetBytes($"{motionAsJTSTimeSeries}"), ContentType = "string", QualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce, Retain = false }/*"SensorGrid", $"{motionAsJTSTimeSeries}", qualityOfServiceLevel: MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce, retain: false*/);
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
                case "Light":
                    jts.Header.Columns.H0.Name = "Light";
                    jts.Header.Columns.H0.DataType = "NUMBER";
                    jts.Header.Columns.H0.RenderType = "VALUE";
                    jts.Header.Columns.H0.Format = "0.###";
                    jts.Header.Columns.H0.Aggregate = "NONE";
                    var l = new JTSData() { Ts = DateTime.Now };
                    l.F.F0.Value = (double)v;
                    l.F.F0.Quality = 100;
                    l.F.F0.Annotation = dn;
                    jts.Data.Add(l);
                    break;
                case "Motion":
                    jts.Header.Columns.H0.Name = "Motion";
                    jts.Header.Columns.H0.DataType = "NUMBER";
                    jts.Header.Columns.H0.RenderType = "VALUE";
                    jts.Header.Columns.H0.Format = "0.###";
                    jts.Header.Columns.H0.Aggregate = "NONE";
                    var m = new JTSData() { Ts = DateTime.Now };
                    m.F.F0.Value = (double)v;
                    m.F.F0.Quality = 100;
                    m.F.F0.Annotation = dn;
                    jts.Data.Add(m);
                    break;
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
                case "LightLevel":
                    jts.Header.Columns.H0.Name = "LightLevel";
                    jts.Header.Columns.H0.DataType = "NUMBER";
                    jts.Header.Columns.H0.RenderType = "VALUE";
                    jts.Header.Columns.H0.Format = "0.###";
                    jts.Header.Columns.H0.Aggregate = "NONE";
                    var ll = new JTSData() { Ts = DateTime.Now };
                    ll.F.F0.Value = (double)v;
                    ll.F.F0.Quality = 100;
                    ll.F.F0.Annotation = dn;
                    jts.Data.Add(ll);
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
                Task.Run(() =>
                {
                    try
                    {
                        _disposable = Create.ResilientMqttClient()
                            .WithResilientClientOptions(resilientClientOptions =>
                                resilientClientOptions.WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                            .WithClientOptions(clientOptions =>
                            {
                                clientOptions.WithTcpServer(_serverIp, _serverPort);
                                clientOptions.WithCredentials("sidlvet", "KrommeBeet55");
                            }))                    
                        .Subscribe(
                        i =>
                        {
                            _mqttClient = i;
                            i.Connected.Subscribe((_) =>
                                Console.WriteLine($"{DateTime.Now}\t CLIENT: Connected with server."));
                            i.Disconnected.Subscribe((_) =>
                                Console.WriteLine($"{DateTime.Now}\t CLIENT: Disconnected with server."));
                        });

                        var subscribed = false;

                        var logger = new MqttNetEventLogger();
                        logger.LogMessagePublished += Logger_LogMessagePublished;

                        _logger.LogDebug($"Connecting to {ipDownstairs} with key: {keyDownstairs}");
                        var localHueClientDownstairs = new LocalHueApi(ipDownstairs, keyDownstairs);

                        _logger.LogDebug($"Connecting to {ipUpstairs} with key: {keyUpstairs}");
                        var localHueClientUpstairs = new LocalHueApi(ipUpstairs, keyUpstairs);

                        // _logger.LogDebug("Getting all resources...");

                        var resourcesDownstairs = localHueClientDownstairs.GetResourcesAsync().Result;
                        //var txt = System.Text.Json.JsonSerializer.Serialize(resourcesDownstairs);
                        //System.IO.File.WriteAllText("DownstairsInfo.txt", txt);

                        // var rootsDownstairs = resourcesDownstairs.Data.Where(x => x.Owner == null);

                        var resourcesUpstairs = localHueClientUpstairs.GetResourcesAsync().Result;
                        //txt = System.Text.Json.JsonSerializer.Serialize(resourcesUpstairs);
                        //System.IO.File.WriteAllText("UpInfo.txt", txt);

                        var downstairs = localHueClientDownstairs.GetDevicesAsync().Result;
                        foreach (var device in downstairs.Data)
                        {
                            if (!string.IsNullOrEmpty(device.IdV1) && device.Metadata != null)
                                _deviceNames.Add("/" + _ipNames[ipDownstairs] + device.IdV1, device.Metadata.Name);
                        }

                        var devicesUpstairs = localHueClientUpstairs.GetDevicesAsync().Result;
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

                        var broadcastCancellationToken = new CancellationToken();
                        RecurringTask(() => BroadcastCurrentMeasurements(), 60, broadcastCancellationToken);

                        _logger.LogInformation("Waiting for Hue Bridge events (press any key to stop)...");
                        //await Task.Delay(TimeSpan.FromHours(1));
                        Console.ReadLine();

                        localHueClientDownstairs.StopEventStream();
                        localHueClientUpstairs.StopEventStream();

                        _logger.LogInformation("Stopped listening for Hue Bridge events...");
                        _exitCode = 0;
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

        internal void BroadcastCurrentMeasurements()
        {
            System.Diagnostics.Debug.WriteLine("Temporary dump: ");
            foreach (var cll in _currentDeviceLightLevel)
            {
                System.Diagnostics.Debug.WriteLine(cll);
            }
            foreach(var ct in _currentDeviceTemperature)
            {
                System.Diagnostics.Debug.WriteLine(ct);
            }
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
            //System.Console.WriteLine(e.LogMessage);
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