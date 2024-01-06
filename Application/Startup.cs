using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RIAPP.DataService.Core.CodeGen;
using RIAPP.DataService.Utils;
using RIAppDemo.BLL.DataServices.Config;
using RIAppDemo.BLL.Utils;
using RIAppDemo.Services;
using RIAppDemo.Utils;
using System;
using System.Security.Claims;

namespace AngularTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/my-workspace/dist/ClientApp";
            });

            services.AddHttpContextAccessor();

            services.AddScoped<IHostAddrService, HostAddrService>();

            services.AddSingleton<IPathService, PathService>();

            services.AddResponseCaching();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireUpdateRights", policy => policy.RequireClaim("Permission", "CanUpdate"));
            });

            #region Local Functions
            ClaimsPrincipal getCurrentUser(IServiceProvider sp)
            {
                ClaimsPrincipal basicPrincipal = new ClaimsPrincipal(
                 new ClaimsIdentity(
                     new Claim[] {
                        new Claim("Permission", "CanUpdate"),
                        new Claim(ClaimTypes.Role, "Admins"),
                        new Claim(ClaimTypes.Role,  "Users"),
                        new Claim(ClaimTypes.Name, "DUMMY_USER"),
                        new Claim(ClaimTypes.NameIdentifier, "DUMMY_USER Basic")
                   },
                         "Basic"));

                ClaimsPrincipal validUser = basicPrincipal;

                ClaimsIdentity bearerIdentity = new ClaimsIdentity(
                        new Claim[] {
                        new Claim("Permission", "CupBearer"),
                        new Claim(ClaimTypes.Role, "Token"),
                        new Claim(ClaimTypes.Name, "DUMMY_USER"),
                        new Claim(ClaimTypes.NameIdentifier, "DUMMY_USER Bear")},
                            "Bearer");

                validUser.AddIdentity(bearerIdentity);

                return validUser;
            };
            #endregion

            services.AddSingleton<ICodeGenConfig, CodeGenConfig>();
            services.AddSingleton<ISerializer, Serializer>();

            services.AddFolderBrowser((options) =>
            {
                options.GetUser = getCurrentUser;
            });

            services.AddRIAppDemoService((options) =>
            {
                options.GetUser = getCurrentUser;
                options.ConnectionString = Configuration[$"ConnectionStrings:DBConnectionStringADW"];
            });

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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = (context) =>
                {
                    context.Context.Response.Headers["Cache-Control"] = Configuration["StaticFiles:Headers:Cache-Control"];
                }
            });

            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp/my-workspace";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
