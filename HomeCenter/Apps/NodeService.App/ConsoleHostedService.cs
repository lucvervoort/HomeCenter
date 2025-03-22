// using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet.Diagnostics.Logger;
using MQTTnet.Rx.Client;
using System.Reactive.Linq;

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

        private IDisposable? _disposable = default;
        private IResilientMqttClient _mqttClient;
        private string _serverIp = "localhost";
        private int _serverPort = 1883;

        public ConsoleHostedService(
            ILogger<ConsoleHostedService> logger,
            /*IConfiguration configuration,*/
            IHostApplicationLifetime appLifetime)
        {
            _logger = logger;
            // _configuration = configuration;
            _appLifetime = appLifetime;
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

                        _logger.LogInformation("Waiting for events (press any key to stop)...");
                        //await Task.Delay(TimeSpan.FromHours(1));
                        Console.ReadLine();

                        _logger.LogInformation("Stopped listening for events...");
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