using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TaskRecorder.ActivityTracker;
using TaskRecorder.Data;

namespace TaskRecorder
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ConnectionDBContext>(optioms =>
                 optioms.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("MyConnection")));
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "User_Schema";
            })
                .AddCookie("User_Schema", options =>
                  {
                      options.LoginPath = "/user/login";
                      options.LogoutPath = "/User/UserLogin/SignOut";
                      options.AccessDeniedPath = "/User/UserLogin/AccessDenied";
                  })
                  .AddCookie("Admin_Schema", options =>
                  {
                      options.LoginPath = "/admin/login";
                      options.LogoutPath = "/Administration/AdminLogin/SignOut";
                      options.AccessDeniedPath = "/Administration/AdminLogin/AccessDenied";
                  });
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddControllersWithViews( options => 
            options.Filters.Add(typeof(ActivityRecoder)));
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.Use(async (context, next) =>
            {
                var principal = new ClaimsPrincipal();
                var result1 = await context.AuthenticateAsync("User_Schema");
                if (result1?.Principal != null)
                {
                    principal.AddIdentities(result1.Principal.Identities);
                }
                var result2 = await context.AuthenticateAsync("Admin_Schema");
                if (result2?.Principal != null)
                {
                    principal.AddIdentities(result2.Principal.Identities);
                }
                context.User = principal;
                await next();
            });
            app.UseAuthentication();
            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                     name: "areas",
                     pattern: "{area:exists=Home}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
