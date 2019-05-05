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
            await CreateRoleIfNotExists(DbInfo.PRIMARY_DB_NAME, DbInfo.COLLECTION_NAME, client, Roles.DOCTOR);
            await CreateRoleIfNotExists(DbInfo.PRIMARY_DB_NAME, DbInfo.COLLECTION_NAME, client, Roles.PATIENT);
            await CreateRoleIfNotExists(DbInfo.PRIMARY_DB_NAME, DbInfo.COLLECTION_NAME, client, Roles.DEVELOPER);

        } // SeedRoles


        private async Task CreateRoleIfNotExists( string databaseName, string collectionName, DocumentClient client, string roleName )
        {
            var role = new ApplicationRole
            {
                //Id = new Guid().ToString(),
                Name = roleName,
                NormalizedName = roleName.ToUpper(),
                CreatedDate = DateTime.Now,
                Discriminator = nameof(ApplicationRole)
            };

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
