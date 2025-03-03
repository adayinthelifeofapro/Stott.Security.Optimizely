﻿namespace OptimizelyTwelveTest
{
    using EPiServer.Cms.UI.AspNetIdentity;
    using EPiServer.Scheduler;
    using EPiServer.Web.Routing;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Cors.Infrastructure;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    using OptimizelyTwelveTest.Features.Common;
    using OptimizelyTwelveTest.Features.Home;

    using ServiceExtensions;

    using Stott.Optimizely.RobotsHandler.Configuration;
    using Stott.Security.Optimizely.Common;
    using Stott.Security.Optimizely.Features.Configuration;

    public class Startup
    {
        private readonly IWebHostEnvironment _webHostingEnvironment;

        public Startup(IWebHostEnvironment webHostingEnvironment)
        {
            _webHostingEnvironment = webHostingEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            if (_webHostingEnvironment.IsDevelopment())
            {
                services.Configure<SchedulerOptions>(o =>
                {
                    o.Enabled = false;
                });
            }

            services.AddRazorPages();
            services.AddCmsAspNetIdentity<ApplicationUser>();

            // Various serialization formats.
            //// services.AddMvc().AddNewtonsoftJson();
            services.AddMvc().AddJsonOptions(config =>
            {
                config.JsonSerializerOptions.PropertyNamingPolicy = new UpperCaseNamingPolicy();
            });

            services.AddCms();
            services.AddFind();
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblyContaining(typeof(HomePage));
            });
            services.AddCustomDependencies();
            services.AddRobotsHandler();
            services.AddSwaggerGen();

            // Configuration App Settings (Simple)
            //// services.AddStottSecurity();

            // Configuration App Settings (Full)
            services.AddStottSecurity(cspSetupOptions =>
            {
                cspSetupOptions.ConnectionStringName = "EPiServerDB";
            },
            authorizationOptions => 
            {
                authorizationOptions.AddPolicy(CspConstants.AuthorizationPolicy, policy =>
                {
                    policy.RequireRole("WebAdmins", "Everyone");
                });
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/util/Login";
            });

            services.AddCors(x =>
            {
                x.AddPolicy("TEST-POLICY", x =>
                {
                    x.AllowAnyMethod();
                    x.AllowAnyOrigin();
                    x.AllowAnyHeader();
                });
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
                app.UseExceptionHandler("/server-error.html");
            }

            app.UseStatusCodePagesWithReExecute("/error", "?statusCode={0}");

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseStottSecurity();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapContent();
                endpoints.MapRazorPages();
            });
        }
    }
}