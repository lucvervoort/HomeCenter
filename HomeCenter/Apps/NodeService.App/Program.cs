using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using static Program;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry;

/*
https://dev.to/asimmon/net-aspire-dashboard-is-the-best-tool-to-visualize-your-opentelemetry-data-during-local-development-9dl

docker run --rm -it -p 18888:18888 -p 4317:18889 -d --name aspire-dashboard -e DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS='true' mcr.microsoft.com/dotnet/nightly/aspire-dashboard:8.0.0-preview.6

login: see output of docker container!
 */

namespace HomeCenterService
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            bool useOtlpExporter = false;
            var dirName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // We create the generic host
            await Host.CreateDefaultBuilder(args)
                .UseContentRoot(dirName)
                .ConfigureLogging(loggingBuilder =>
                {
                    var configuration = new ConfigurationBuilder()
                                        .AddJsonFile("appsettings.json")
                                        .Build();

                    //var logger = new LoggerConfiguration()
                    //    .ReadFrom.Configuration(configuration)
                    //    .CreateLogger();                  
                    //loggingBuilder.AddSerilog(logger, dispose: true);

                    loggingBuilder.AddOpenTelemetry(logging =>
                     {
                         logging.IncludeFormattedMessage = true;
                         logging.IncludeScopes = true;                   
                     });

                    useOtlpExporter = !string.IsNullOrWhiteSpace(configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging(c => { c.AddConsole(); c.AddDebug(); });

                    services.AddOpenTelemetry()
                        .WithMetrics(metrics =>
                        {
                            metrics.AddRuntimeInstrumentation()
                                .AddMeter("LvMeter");
                        })
                        .WithTracing(tracing =>
                        {
                            tracing.AddAspNetCoreInstrumentation()
                                .AddHttpClientInstrumentation();
                        });
                    if (useOtlpExporter)
                    {
                        services.AddOpenTelemetry().UseOtlpExporter();
                    }
                    services.AddHostedService<ConsoleHostedService>(); 
                })
                .RunConsoleAsync();
        }
    }
}