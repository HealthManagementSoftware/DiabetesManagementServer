using DMS.Data;
using DMS.Models;
using DMS.Services;
using DMS.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
//using Amazon.DynamoDBv2;

namespace DMS
{
    public class Startup
    {
        private DocumentClient client;

        public IConfiguration Configuration { get; }


        public Startup( IConfiguration configuration )
        {
            Configuration = configuration;

        } // constructor


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices( IServiceCollection services )
        {
            services.AddScoped<DatabaseSeeder>();

            //
            // Change DB Connection here:
            //
            IConfigurationSection configSection = Configuration.GetSection("CosmosSettings");
            //IConfigurationSection configSection = Configuration.GetSection("CosmosSettings");

            // The "AuthKey" is retrieved from the Azure Key Manager, using the connection string retrieved 
            //      in Program.cs. If running locally, it will use secrets.json which is set with :
            //
            services.AddDbContextPool<ApplicationDbContext>( options =>
              options.UseCosmos( 
                  configSection[ "ServiceEndpoint" ], 
                  configSection[ "AuthKey" ], 
                  configSection[ "DatabaseName" ] 
                  ) );
            //.UseMySql( Configuration.GetConnectionString("AzureStorageConnectionString-1") ) );

            // https://docs.microsoft.com/en-us/azure/cosmos-db/sql-api-dotnetcore-get-started
            client = new DocumentClient( new Uri( configSection[ "ServiceEndpoint" ] ), configSection[ "AuthKey" ] );
            try
            {
                StartCosmosConnection( configSection[ "ServiceEndpoint" ], configSection[ "AuthKey" ] ).Wait();
            }
            catch( DocumentClientException de )
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine( "{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message );
            }
            catch( Exception e )
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine( "Error: {0}, Message: {1}", e.Message, baseException.Message );
            }

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            // Adding scoped services to provide DB Repositories:
            services.AddScoped<IApplicationUserRepository, EFApplicationUserRepository>();
            services.AddScoped<IExerciseEntryRepository, DbExerciseEntryRepository>();
            services.AddScoped<IGlucoseEntryRepository, DbGlucoseEntriesRepository>();
            services.AddScoped<IMealEntryRepository, DbMealEntryRepository>();
            services.AddScoped<IMealItemRepository, DbMealItemRepository>();
            services.AddScoped<IPatientRepository, DbPatientRepository>();
            services.AddScoped<IDoctorRepository, DbDoctorRepository>();

            //services.AddIdentityCore<RoleUser>(options => { });
            //services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            //services.AddAWSService<IAmazonDynamoDB>();
            //services.AddAWSService<IAmazonS3>();

            services.AddMvc()
            .AddJsonOptions( options =>
            {
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            } );

            // Added the following service to use the ApplicationUser, 
            // ApplicationRole, and ApplicationDbContext classes 
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

        } // ConfigureServices


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IHostingEnvironment env )
        {
            if( env.IsDevelopment() )
            {
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler( "/Home/Error" );
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc( routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}" );

                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller=AccountApi}/{action=Index}/{id?}"
                );
            } );

        } // Configure


        private async Task StartCosmosConnection( string EndpointUri, string PrimaryKey )
        {
            await client.CreateDatabaseIfNotExistsAsync( new Database { Id = DbInfo.DBNAME } );    // Create DB

            await client.CreateDocumentCollectionIfNotExistsAsync(                              // Create "table"
                UriFactory.CreateDatabaseUri( DbInfo.DBNAME ),
                new DocumentCollection { Id = DbInfo.COLLECTIONNAME }
                );

            //var testUser = new ApplicationUser { FirstName = "Bob", LastName = "TestUser" };    // Test data
            var doctorRole = new ApplicationRole
            {
                //Id = new Guid().ToString(),
                Name = Roles.DOCTOR,
                NormalizedName = "DOCTOR",
                CreatedDate = DateTime.Now,
                Discriminator = nameof( ApplicationRole )
            };
            await CreateRoleIfNotExists( DbInfo.DBNAME, DbInfo.COLLECTIONNAME, doctorRole );

            var patientRole = new ApplicationRole
            {
                //Id = new Guid().ToString(),
                Name = Roles.PATIENT,
                NormalizedName = "PATIENT",
                CreatedDate = DateTime.Now,
                Discriminator = nameof( ApplicationRole )
            };
            await CreateRoleIfNotExists( DbInfo.DBNAME, DbInfo.COLLECTIONNAME, patientRole );

        } // StartCosmosConnection


        private async Task CreateRoleIfNotExists( string databaseName, string collectionName, ApplicationRole role )
        {
            // Set some common query options
            var queryOptions = new FeedOptions { MaxItemCount = -1 };
            IQueryable<ApplicationRole> roleQuery = client.CreateDocumentQuery<ApplicationRole>(
                UriFactory.CreateDocumentCollectionUri( databaseName, collectionName ), queryOptions )
                .Where( n => n.Name == role.Name );

            if( roleQuery.ToList().Count < 1 )
                await client.CreateDocumentAsync(
                        UriFactory.CreateDocumentCollectionUri( databaseName, collectionName ), role
                        );

        } // CreateUserDocumentIfNotExists

    } // class

} // namespace
