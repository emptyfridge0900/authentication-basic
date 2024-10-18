using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            //grab the services in order to create user
            //scope is dependency injection container
            using (var scope=host.Services.CreateScope())
            {
                //UserManager<IdentityUser> is added in service.AddIdentity
                var userManager = scope.ServiceProvider
                    .GetRequiredService<UserManager<IdentityUser>>();

                var user = new IdentityUser("bob");
                userManager.CreateAsync(user, "password").GetAwaiter().GetResult();
                //add claim to user(bob in here)
                userManager.AddClaimAsync(user, new Claim("rc.granma", "big.cookie"))
                    .GetAwaiter().GetResult();
                userManager.AddClaimAsync(user, new Claim("rc.api.granma", "big.api.cookie"))
                    .GetAwaiter().GetResult();
            }

                host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
