using AspNet.Security.OpenIdConnect.Primitives;
using CorsairWebSolutions.IdentityServer.Data;
using CorsairWebSolutions.IdentityServer.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using System;
using System.Security.Cryptography;

namespace CorsairWebSolutions.IdentityServer
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
            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddIdentity<ApplicationUser, IdentityRole>()
       .AddEntityFrameworkStores<ApplicationDbContext>()
       .AddDefaultTokenProviders();

            services.AddOpenIddict()
        .AddCore(options =>
        {
            // Configure OpenIddict to use the Entity Framework Core stores and entities.
            options.UseEntityFrameworkCore()
                   .UseDbContext<ApplicationDbContext>()
                   .ReplaceDefaultEntities<Guid>();
        })

        .AddServer(options =>
        {
            // Register the ASP.NET Core MVC binder used by OpenIddict.
            // Note: if you don't call this method, you won't be able to
            // bind OpenIdConnectRequest or OpenIdConnectResponse parameters.
            options.UseMvc();

            // Enable the token endpoint (required to use the password flow).
            options.EnableAuthorizationEndpoint("/connect/authorize")
                  .EnableTokenEndpoint("/connect/token");

            // Allow client applications to use the grant_type=password flow.
            options.AllowPasswordFlow();
            options.AllowImplicitFlow();
            options.AllowAuthorizationCodeFlow();
            // During development, you can disable the HTTPS requirement.
            options.DisableHttpsRequirement();

            // Accept token requests tha9t don't specify a client_id.

            options.AddEphemeralSigningKey();
            options.UseJsonWebTokens();

            options.RegisterScopes(
        OpenIdConnectConstants.Scopes.Profile,
        OpenIdConnectConstants.Scopes.Email,
        OpenIdConnectConstants.Scopes.OpenId,
        "corsairwebsolution_client");
        })

        .AddValidation();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                // Configure the context to use Microsoft SQL Server.
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

                // Register the entity sets needed by OpenIddict.
                // Note: use the generic overload if you need
                // to replace the default OpenIddict entities.
                options.UseOpenIddict<Guid>();
            });



            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(c =>
           c.AllowAnyHeader()
           .AllowAnyMethod()
           .AllowAnyOrigin());

            app.UseAuthentication();

           

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

           // app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

            

            // Create a new service scope to ensure the database context is correctly disposed when this methods returns.
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureCreated();

                // Note: when using a custom entity or a custom key type, replace OpenIddictApplication by the appropriate type.
                var manager = scope.ServiceProvider.GetRequiredService<OpenIddictApplicationManager<OpenIddictApplication<Guid>>>();

                if (manager.FindByClientIdAsync("corsairwebsolutions").GetAwaiter().GetResult() == null)
                {
                    var descriptor = new OpenIddictApplicationDescriptor
                    {
                        ClientId = "corsairwebsolutions",
                        RedirectUris = { new Uri("http://localhost:21055/auth-callback") },
                        DisplayName = "Corsair Web Sollutions",
                        Permissions =
                            {
                                OpenIddictConstants.Permissions.Endpoints.Authorization,
                                OpenIddictConstants.Permissions.Endpoints.Logout,
                                OpenIddictConstants.Permissions.GrantTypes.Implicit,
                                OpenIddictConstants.Permissions.Scopes.Email,
                                OpenIddictConstants.Permissions.Scopes.Profile,
                                OpenIddictConstants.Permissions.Scopes.Roles,
                                "scp:corsairwebsolution_client"

                            }

                    };

                    manager.CreateAsync(descriptor).GetAwaiter().GetResult();
                }
            }
        }
    }
}
