using CorsairWebSolutions.IdentityServer.Data;
using CorsairWebSolutions.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace CorsairWebSolutions.IdentityManagement.Test
{
    [TestClass]
    public class UnitTest1
    {
        ApplicationDbContext context = null;
        public IConfiguration Configuration = null;
        ServiceProvider provider = null;
        [TestInitialize]
        public void init()
        {
            if (provider == null)
            {
                IServiceCollection services = new ServiceCollection();

                Configuration = new ConfigurationBuilder()
                    .SetBasePath(Environment.CurrentDirectory)
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build();
                services.AddScoped(c => Configuration);

                services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Users")));
                services.AddScoped<RoleManager<IdentityRole>>();

                services.AddIdentity<ApplicationUser, IdentityRole>()
          .AddEntityFrameworkStores<ApplicationDbContext>()
          .AddDefaultTokenProviders();

                provider = services.BuildServiceProvider();
            }

        }

        [TestMethod]
        public async Task TestMethod1()
        {

            var p = provider.GetService<UserManager<ApplicationUser>>();
            var r = provider.GetService<RoleManager<IdentityRole>>();
            var context3 = provider.GetService<ApplicationDbContext>();
            Assert.IsNotNull(context3);



            await r.CreateAsync(new IdentityRole("CWS_Admin"));

            var user = new ApplicationUser()
            {

                Email = "corsairwebsolutions@gmail.com",
                FirstName = "Dalal",
                LastName = "Pound",
                TenantId = "corsairwebsolutions",
                Company = "Corsair Web Solutions",
                UserName = "corsairwebsolutions@gmail.com"
            };
            var result = await p.CreateAsync(user);

            await p.AddClaimAsync(user, new System.Security.Claims.Claim("sub", Guid.NewGuid().ToString("N")));
            await p.AddPasswordAsync(user, "Noah2017!!");

            await p.AddToRoleAsync(user, "CWS_Admin");
            


        }



    }
}
