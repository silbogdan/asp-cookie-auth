using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Authentication
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
            services.AddCors(options =>
              options.AddPolicy("Dev", builder =>
              {
                  // Allow multiple methods
                  builder.WithMethods("GET", "POST", "PATCH", "DELETE", "OPTIONS")
                    .WithHeaders(
                      HeaderNames.Accept,
                      HeaderNames.ContentType,
                      HeaderNames.Authorization)
                    .AllowCredentials()
                    .SetIsOriginAllowed(origin =>
                    {
                        if (string.IsNullOrWhiteSpace(origin)) return false;
                      // Only add this to allow testing with localhost, remove this line in production!
                      if (origin.ToLower().StartsWith("http://localhost")) return true;
                        // Insert your production domain here.
                        //if (origin.ToLower().StartsWith("https://dev.mydomain.com")) return true;
                        return false;
                    });
              })
            );
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
              .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
              {
                  options.Cookie.Name = "UserLoginCookie";
                  options.SlidingExpiration = true;
                  options.ExpireTimeSpan = new TimeSpan(0, 1, 0); // Expires in 1 minute
                  options.Events.OnRedirectToLogin = (context) =>
                  {
                      context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                      return Task.CompletedTask;
                  };

                  options.Cookie.HttpOnly = true;
                  // Only use this when the sites are on different domains
                  options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
              });
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("Dev");

            app.UseCookiePolicy(
            new CookiePolicyOptions
            {
                Secure = CookieSecurePolicy.Always
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
