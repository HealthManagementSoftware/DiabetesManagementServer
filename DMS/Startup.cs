using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DMS.Data;
using DMS.Models;
using DMS.Services;
using DMS.Services.Interfaces;
using Newtonsoft.Json;

namespace DMS
{
    public class Startup
    {
        public Startup( IConfiguration configuration )
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices( IServiceCollection services )
        {
            //services.AddDbContext<ApplicationDbContext>( options =>
            //     options.UseSqlServer( Configuration.GetConnectionString( "DefaultConnection" ) ) );

            services.AddScoped<DatabaseSeeder>();

            services.AddDbContextPool<ApplicationDbContext>( options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                //options.CheckConsentNeeded = context => true;
                //options.MinimumSameSitePolicy = SameSiteMode.None;
                options.UseMySql(Configuration.GetConnectionString("MySqlConnection") );
            } );//,
                    //mysqlOptions =>
                    //{
                    //    mysqlOptions.ServerVersion( new Version( 5, 5, 60 ), ServerType.MySql );
                    //} ) );

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

            //services.AddIdentityCore<RoleUser>(options => { });

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IHostingEnvironment env )
        {
            if( env.IsDevelopment() )
            {
                app.UseBrowserLink();
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
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
            } );
        }
    }
}
