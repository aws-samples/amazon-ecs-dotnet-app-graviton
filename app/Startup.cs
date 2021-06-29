using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MvcMovie.Data;
using MvcMovie.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MySql.EntityFrameworkCore;

namespace MvcMovie
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Environment = env;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<MvcMovieContext>(options =>
            {
                var connectionString = Configuration.GetConnectionString("MvcMovieContext");
                
                var dbHost = Configuration["DBHOST"];
                var dbUser = Configuration["DBUSER"];
                var dbPass = Configuration["DBPASS"];
                var dbName = Configuration["DBNAME"];
                
                if(!string.IsNullOrWhiteSpace(dbHost) &&
                   !string.IsNullOrWhiteSpace(dbUser) && 
                   !string.IsNullOrWhiteSpace(dbPass) &&
                   !string.IsNullOrWhiteSpace(dbName))
                {
                    var dbPort = Configuration["DBPORT"] ?? "3306";
                    var dbSslMode = Configuration["DBSSLMODE"];
                    
                    connectionString = $"server={dbHost};port={dbPort};database={dbName};uid={dbUser};password={dbPass};";
                    if(!string.IsNullOrWhiteSpace(dbSslMode))
                        connectionString += $"SslMode={dbSslMode};";
                }

                options.UseMySQL(connectionString);
            });

            // Add identity types
            services.AddIdentity<AppUser, AppRole>().AddDefaultTokenProviders();

            // Identity Services
            services.AddTransient<IUserStore<AppUser>, AppUserStore>();
            services.AddTransient<IRoleStore<AppRole>, AppRoleStore>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
