using System.Threading;
using HomeCenter.Mqtt.Server;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView; // mark
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using SkiaSharp; // mark

namespace MauiSample;

public partial class App : Application
{
    private readonly MqttFactory _mqttFactory;
    private readonly IMqttClient _mqttClient;
    private readonly MqttClientOptions _mqttClientOptions;

    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();

        _mqttFactory = new MqttFactory();

        _mqttClient = _mqttFactory.CreateMqttClient();
        _mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("lvmini").Build();

        // Setup message handling before connecting so that queued messages
        // are also handled properly. When there is no event handler attached all
        // received messages get lost.
        _mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            var s = e.ApplicationMessage.ConvertPayloadToString();
            System.Diagnostics.Debug.WriteLine(s);
            try
            {
                var jts = System.Text.Json.JsonSerializer.Deserialize<JTSRoot>(s);
            }
            catch
            {
            }
            return Task.CompletedTask;
        };
    }

    protected async override void OnStart()
    {
        UserAppTheme = AppTheme.Light;
        base.OnStart();

        LiveCharts.Configure(config => // mark
            config // mark
                   // you can override the theme 
                   //.AddDarkTheme() // mark 

                // In case you need a non-Latin based font, you must register a typeface for SkiaSharp
                //.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('汉')) // <- Chinese // mark
                //.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('あ')) // <- Japanese // mark
                //.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('헬')) // <- Korean // mark
                //.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('Ж'))  // <- Russian // mark

                //.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('أ'))  // <- Arabic // mark
                //.UseRightToLeftSettings() // Enables right to left tooltips // mark

                // finally register your own mappers
                // you can learn more about mappers at:
                // https://livecharts.dev/docs/{{ platform }}/{{ version }}/Overview.Mappers

                // here we use the index as X, and the population as Y // mark
                .HasMap<City>((city, index) => new(index, city.Population)) // mark
                                                                            // .HasMap<Foo>( .... ) // mark
                                                                            // .HasMap<Bar>( .... ) // mark
        ); // mark

        _ = await _mqttClient.ConnectAsync(_mqttClientOptions, CancellationToken.None);

        var mqttSubscribeOptions = _mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter("SensorGrid")
        .Build();

        _ =
            _mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
    }

    public record City(string Name, double Population);
}
