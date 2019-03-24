using DMS.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace DMS
{
    public class Program
    {

        public static void Main( string[] args )
        {
            //BuildWebHost(args).Run();                     // ASP.NET CORE 2.0

            // Use the settings below to deploy to a custom environment (Linux deploy, etc).
            //var hostUrl = "http://0.0.0.0:51874";

            var host = WebHost
                .CreateDefaultBuilder(args) // <-- Automatically adds User Secrets
                .ConfigureAppConfiguration((ctx, builder) =>    // CosmosDB
                {
                    // This is set in the Azure Application Settings > Application Settings Section on the website
                    var keyVaultEndpoint = Environment.GetEnvironmentVariable( "KEYVAULT_ENDPOINT" );
                    if (!string.IsNullOrEmpty(keyVaultEndpoint))
                    {
                        var azureServiceTokenProvider = new AzureServiceTokenProvider();
                        var keyVaultClient = new KeyVaultClient(
                            new KeyVaultClient.AuthenticationCallback(
                                azureServiceTokenProvider.KeyVaultTokenCallback));
                        builder.AddAzureKeyVault(
                                keyVaultEndpoint, keyVaultClient, new DefaultKeyVaultSecretManager());
                    }
                })
                .UseKestrel()
                //.UseUrls(hostUrl)   // Override http://localhost:5000 to allow from outside machines
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                //.UseApplicationInsights()
                .UseStartup<Startup>()
                .Build();

            using( var scope = host.Services.CreateScope() )
            {
                //var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
                //db.Database.EnsureCreated();//.Migrate();//

                // Moved to Startup, not using EF Core due to errors
                //var roleManager = scope.ServiceProvider.GetService<RoleManager<ApplicationRole>>();
                //new DatabaseSeeder( db, roleManager ).SeedRoles();

            } // using

            host.Run();

        } // Main


        // Added for CosmosDB:
        private static string GetKeyVaultEndpoint() => Environment.GetEnvironmentVariable( "KEYVAULT_ENDPOINT" );


        public static IWebHost BuildWebHost( string[] args ) =>
            WebHost.CreateDefaultBuilder( args )
                .UseStartup<Startup>()
                .Build();
    }
}
