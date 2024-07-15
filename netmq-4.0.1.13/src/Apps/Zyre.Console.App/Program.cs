//using Assignment.Repository.Context;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Assignment.Console.App
{
    internal class Program
    {
        private static IHost? _host;

        static async Task Main(string[] args)
        {
            var hostBuilder = new HostBuilder()

                .ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    configurationBuilder.SetBasePath(context.HostingEnvironment.ContentRootPath);
                    configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    // Niet optioneel: config bestand moet er staan!
                    // reload on change: zonder de app te herstarten is een aangepast configuratiebestand opgeladen
                    configurationBuilder.AddEnvironmentVariables(); // bijvoorbeeld SET X=Y
                })

                .ConfigureServices((context, services) =>
                {
                    var cs = context.Configuration.GetConnectionString("DefaultConnectionString");
                 // TODO - Registreer hier je DbContext klasse bij DI ...
                 // ============================================================================
                 // services.AddDbContext<HotelManagementContext>(options =>
                 //    options.UseSqlServer(cs), contextLifetime: ServiceLifetime.Scoped
                 //    , optionsLifetime: ServiceLifetime.Singleton);
                 // ============================================================================
                    services.AddSingleton<IAssignment, Assignment>();
                });

            _host = hostBuilder.Build(); // Nodig om configuratie te initialiseren      

            await _host.StartAsync();

            var logger = _host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Host lifecycle coupled");

            var assignment = _host.Services.GetRequiredService<IAssignment>();
            assignment.Run();

            await _host.StopAsync(TimeSpan.FromSeconds(5)); // we gunnen de host 5 seconden alvorens geforceerd te stoppen
        }
    }
}
