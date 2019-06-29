using DMS.Data;
using DMS.Models;
using DMS.Services;
using DMS.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Threading.Tasks;
//using Amazon.DynamoDBv2;

namespace DMS
{
    public class Startup
    {
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
            IConfigurationSection configSection = Configuration.GetSection(DbInfo.KEY_PRIMARY_DB_SECTION);

            // The "AuthKey" is retrieved from the Azure Key Manager, using the connection string retrieved 
            //      in Program.cs. If running locally, it will use secrets.json which is set with :
            // dotnet user-secrets -p ./DMS/DMS.csproj set AuditCosmosSettings:AuditAuthKey "..."
            //
            services.AddDbContextPool<ApplicationDbContext>( options =>
              options.UseCosmos(
                  configSection[ DbInfo.KEY_SERVICE_ENDPOINT ],
                  configSection[ DbInfo.KEY_AUTH_KEY ],               //Saved in Azure Key Vault
                  configSection[ DbInfo.KEY_DB_NAME ]
                  ) );
            //.UseMySql( Configuration.GetConnectionString("AzureStorageConnectionString-1") ) );

            // https://docs.microsoft.com/en-us/azure/cosmos-db/sql-api-dotnetcore-get-started
            try
            {
                DocumentClient client = new DocumentClient( new Uri( configSection[ DbInfo.KEY_SERVICE_ENDPOINT ] ), 
                    configSection[ DbInfo.KEY_AUTH_KEY ] );
                CreateCosmosCollection( client, configSection[ DbInfo.KEY_DB_NAME ], DbInfo.COLLECTION_NAME ).Wait();
                SeedRoles( client ).Wait();
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


            // Create the connection to the Audit database (if auditing enabled):
            if( Config.AuditingOn )
            {
                IConfigurationSection auditConfigSection = Configuration.GetSection(DbInfo.KEY_AUDIT_DB_SECTION);

                services.AddDbContextPool<AuditDbContext>( options =>
                  options.UseCosmos(
                      auditConfigSection[ DbInfo.KEY_SERVICE_ENDPOINT ],
                      auditConfigSection[ DbInfo.KEY_AUDIT_AUTH_KEY ],      //Saved in Azure Key Vault
                      auditConfigSection[ DbInfo.KEY_DB_NAME ]
                      ) );

                try
                {
                    DocumentClient auditClient = new DocumentClient(
                        new Uri( auditConfigSection[ DbInfo.KEY_SERVICE_ENDPOINT ] ),
                        auditConfigSection[ DbInfo.KEY_AUDIT_AUTH_KEY ]
                        );
                    CreateCosmosCollection(
                        auditClient,
                        auditConfigSection[ DbInfo.KEY_DB_NAME ],
                        DbInfo.AUDIT_COLLECTION_NAME
                        ).Wait();
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

            } // if Auditing On


            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();


            // Adding scoped services to provide DB Repositories:
            services.AddScoped<IApplicationUserRepository, DbApplicationUserRepository>();
            services.AddScoped<IExerciseEntryRepository, DbExerciseEntryRepository>();
            services.AddScoped<IGlucoseEntryRepository, DbGlucoseEntriesRepository>();
            services.AddScoped<IMealEntryRepository, DbMealEntryRepository>();
            services.AddScoped<IMealItemRepository, DbMealItemRepository>();
            services.AddScoped<IPatientRepository, DbPatientRepository>();
            services.AddScoped<IDoctorRepository, DbDoctorRepository>();
            services.AddScoped<IDeveloperRepository, DbDeveloperRepository>();
            if( Config.AuditingOn )
                services.AddScoped<IAuditRepository, DbAuditRepository>();
            else
                services.AddScoped<IAuditRepository, DummyAuditRepository>();
            services.AddScoped<IHIPAANoticeRepository, DbHIPAANoticeRepository>();
            services.AddScoped<IPatientSignedHIPAANoticeRepository, DbPatientSignedHIPAANoticeRepository>();


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


        /// This method gets called by the runtime. Use this to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IHostingEnvironment env )
        {
            if( env.IsDevelopment() )
            {
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
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


        /// <summary>
        /// Creates CosmosDB collections as needed by the Application.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="dbName"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        private async Task CreateCosmosCollection( DocumentClient client, string dbName, string collectionName )
        {
            await client.CreateDatabaseIfNotExistsAsync( new Database { Id = dbName } );     // Create DB

            RequestOptions options = new RequestOptions() { PartitionKey = new PartitionKey("Id") };
            await client.CreateDocumentCollectionIfNotExistsAsync(                           // Create "table"
                UriFactory.CreateDatabaseUri( dbName ),
                new DocumentCollection { Id = collectionName },
                options
                );

        } // StartCosmosConnection


        /// <summary>
        /// Creates all of the needed roles in the system on startup of the Application.
        /// </summary>
        /// <returns></returns>
        private async Task SeedRoles( DocumentClient client )
        {
            //var testUser = new ApplicationUser { FirstName = "Bob", LastName = "TestUser" };      // Test data
            var doctorRole = new ApplicationRole
            {
                //Id = new Guid().ToString(),
                Name = Roles.DOCTOR,
                NormalizedName = Roles.DOCTOR.ToUpper(),
                CreatedDate = DateTime.Now,
                Discriminator = nameof( ApplicationRole )
            };
            await CreateRoleIfNotExists( DbInfo.PRIMARY_DB_NAME, DbInfo.COLLECTION_NAME, doctorRole, client );

            var patientRole = new ApplicationRole
            {
                //Id = new Guid().ToString(),
                Name = Roles.PATIENT,
                NormalizedName = Roles.PATIENT.ToUpper(),
                CreatedDate = DateTime.Now,
                Discriminator = nameof( ApplicationRole )
            };
            await CreateRoleIfNotExists( DbInfo.PRIMARY_DB_NAME, DbInfo.COLLECTION_NAME, patientRole, client );

            var developerRole = new ApplicationRole
            {
                //Id = new Guid().ToString(),
                Name = Roles.DEVELOPER,
                NormalizedName = Roles.DEVELOPER.ToUpper(),
                CreatedDate = DateTime.Now,
                Discriminator = nameof( ApplicationRole )
            };
            await CreateRoleIfNotExists( DbInfo.PRIMARY_DB_NAME, DbInfo.COLLECTION_NAME, developerRole, client );

        } // SeedRoles


        private async Task CreateRoleIfNotExists( string databaseName, string collectionName, ApplicationRole role, DocumentClient client )
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


        private async Task CreateDeveloperIfNotExists(string databaseName, string collectionName, Developer developer, DocumentClient client)
        {
            // Set some common query options
            var queryOptions = new FeedOptions { MaxItemCount = -1 };
            IQueryable<Developer> devQuery = client.CreateDocumentQuery<Developer>(
                UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), queryOptions)
                .Where(n => n.UserName == developer.UserName);

            if( devQuery.ToList().Count < 1 )
                await client.CreateDocumentAsync(
                        UriFactory.CreateDocumentCollectionUri( databaseName, collectionName ), developer
                    );

        } // CreateUserIfNotExists

    } // class

} // namespace
