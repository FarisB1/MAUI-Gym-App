using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PTFGym.Authentication;
using PTFGym.Models;
using PTFGym.Services;
using PTFGym.Views;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PTFGym
{
    public partial class App : Application
    {
        public readonly AuthenticationService authenticationService;
        public static IServiceProvider Services { get; private set; }

        public App()
        {
            InitializeComponent();
            ConfigureServices();

            MainPage = new AppShell();
        }
        protected override async void OnStart()
        {
            base.OnStart();
            var logger = Services.GetRequiredService<ILogger<App>>();
            logger.LogInformation("Application starting...");

            try
            {
                var dbService = Services.GetRequiredService<LocalDbService>();
                await dbService.InitAsync();
                logger.LogInformation("Database initialized successfully.");

                // Display data from a specific table
                await DisplayTableData<Clan>(dbService);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database initialization failed.");
                Debug.WriteLine($"Database initialization failed: {ex.Message}");
            }
        }

        private void ConfigureServices()
        {
            var services = new ServiceCollection();

            // Register services here
            services.AddSingleton<LocalDbService>();
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            Services = services.BuildServiceProvider();
        }

        private async Task DisplayTableData<T>(LocalDbService dbService) where T : new()
        {
            try
            {
                var tableData = await dbService.GetItemsAsync<T>();
                Debug.WriteLine($"Table: {typeof(T).Name}");

                foreach (var item in tableData)
                {
                    Debug.WriteLine($"Item: {item}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error displaying data from table '{typeof(T).Name}': {ex.Message}");
            }
        }


    }
}
