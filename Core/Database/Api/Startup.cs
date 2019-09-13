// <copyright file="Startup.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Api
{
    using System.Text;
    using Allors.Adapters.SqlClient;
    using Allors.Domain;
    using Allors.Meta;
    using Allors.Services;
    using Identity;
    using JSNLog;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Cors.Internal;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using Newtonsoft.Json;
    using ObjectFactory = Allors.ObjectFactory;

    public class Startup
    {
        public Startup(IConfiguration configuration) => this.Configuration = configuration;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(this.Configuration);

            // Allors
            services.AddAllors();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IPolicyService, PolicyService>();
            services.AddSingleton<IExtentService, ExtentService>();

            //// Use this derivation log to capture and analyze derivations
            //// Uncomment the following two lines to use a custom list derivation
            // var listDerivationService = new DerivationService(new DerivationConfig { DerivationLogFunc = () => new CustomListDerivationLog() });
            // services.AddSingleton<IDerivationService>(listDerivationService);  // Set object ID's for breakpoints in CustomListDerivationLog above

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Identity
            services.AddDefaultIdentity<IdentityUser>()
                .AddAllorsStores();

            // Enable Dual Authentication
            services.AddAuthentication(option =>
                    {
                        option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                .AddCookie(cfg => cfg.SlidingExpiration = true)
                .AddJwtBearer(cfg =>
                    {
                        cfg.RequireHttpsMetadata = false;
                        cfg.SaveToken = true;

                        cfg.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidIssuer = this.Configuration["Tokens:Issuer"],
                            ValidAudience = this.Configuration["Tokens:Issuer"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["Tokens:Key"])),
                        };
                    });

            services.ConfigureApplicationCookie(options =>
                {
                    options.Events.OnRedirectToLogin = context =>
                        {
                            context.Response.StatusCode = 401;
                            return System.Threading.Tasks.Task.CompletedTask;
                        };
                });

            // For IIS Authentication
            // services.AddAuthentication(IISDefaults.AuthenticationScheme);

            // Add framework services.
            services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllorsSpa",
                    builder =>
                    {
                        builder
                            .WithOrigins("http://localhost:4000", "http://localhost:4200", "http://localhost:9876")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
            });

            services.AddResponseCaching();
            services.AddMvc();

            services.Configure<MvcOptions>(options =>
                {
                    options.Filters.Add(new CorsAuthorizationFilterFactory("AllorsSpa"));
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Allors
            var database = new Database(
                app.ApplicationServices,
                new Configuration
                {
                    ObjectFactory = new ObjectFactory(MetaPopulation.Instance, typeof(User)),
                    ConnectionString = this.Configuration.GetConnectionString("DefaultConnection"),
                    CommandTimeout = 600,
                });

            var databaseService = app.ApplicationServices.GetRequiredService<IDatabaseService>();
            databaseService.Database = database;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            var jsnlogConfiguration = new JsnlogConfiguration
            {
                corsAllowedOriginsRegex = ".*",
                defaultAjaxUrl = "logging",
                serverSideMessageFormat = env.IsDevelopment() ?
                                            "%requestId | %url | %message" :
                                            "%requestId | %url | %userHostAddress | %userAgent | %message",
            };

            app.UseJSNLog(new LoggingAdapter(loggerFactory), jsnlogConfiguration);

            // app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseCors("AllorsSpa");

            app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Use(async (context, next) =>
                        {
                            var error = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;
                            var message = error?.Error.GetType().ToString();

                            if (error?.Error is SecurityTokenExpiredException)
                            {
                                context.Response.StatusCode = 401;
                                context.Response.ContentType = "application/json";

                                await context.Response.WriteAsync(JsonConvert.SerializeObject(message));
                            }
                            else if (error?.Error != null)
                            {
                                context.Response.StatusCode = 500;
                                context.Response.ContentType = "application/json";
                                await context.Response.WriteAsync(JsonConvert.SerializeObject(message));
                            }
                            else
                                await next();
                        });
                });

            app.UseResponseCaching();

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private class CustomListDerivationLog : Allors.Domain.Logging.ListDerivationLog
        {
            public Allors.Domain.Logging.Derivation Derivation { get; set; }

            public override void AddedDerivable(Object derivable)
            {
                base.AddedDerivable(derivable);

                if (derivable.Id == 787812)
                {
                    var setBreakpointHere = derivable;  // Set a breakpoint here to debug why derivable was added
                }
            }

            public override void AddedDependency(Object dependent, Object dependee)
            {
                base.AddedDependency(dependent, dependee);

                if (dependent.Id == 787812 || dependee.Id == 787812)
                {
                    var setBreakpointHere = (dependent.Id == 787812) ? dependent : dependee;  // Set breakpoint here to debug why dependency was added
                }
            }
        }
    }
}