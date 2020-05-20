using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebApplicationFacebookMessengerBotDemo.Helpers;

namespace WebApplicationFacebookMessengerBotDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        { 
            // services.AddAuthentication(IISDefaults.AuthenticationScheme);
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //services.AddScoped<ILogger, Logger>();
            services.Configure<PageAccessTokenVerifyToken>(Configuration.GetSection("PageAccessTokenVerifyToken"));

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "bearer"; // this is the default scheme to be used
                options.DefaultChallengeScheme = "bearer";
            })
            // we configure JWT bearer authentication here
            .AddJwtBearer("bearer", options =>
            {
                //options.Authority = "https://localhost:5023"; // URL of Identity Server; use IConfiguration instead of hardcoding
                //options.Audience = "localhost:5023"; // ID of the client application; either hardcoded or configureable via IConfiguration if needed 

                options.Authority = "https://www.chatbotapp.website"; // URL of Identity Server; use IConfiguration instead of hardcoding
                options.Audience = "chatbotapp.website"; // ID of the client application; either hardcoded or configureable via IConfiguration if needed 

                //options.Authority = "https://raulronaldo-001-site1.itempurl.com"; // URL of Identity Server; use IConfiguration instead of hardcoding
                //options.Audience = "raulronaldo-001-site1.itempurl.com"; // ID of the client application; either hardcoded or configureable via IConfiguration if needed 
                options.RequireHttpsMetadata = true; // require HTTPS (may be disabled in development, but advice against it)
                options.SaveToken = true; // cache the token for faster authentication
                options.IncludeErrorDetails = true; // get more details on errors; may be disabled in production 
            });
            /*
            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
                options.HttpsPort = 443;
            });
            */
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
