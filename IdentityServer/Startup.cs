using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using IdentityServer.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;

namespace IdentityServer
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            //AdDbContext requires entityframework
            services.AddDbContext<AppDbContext>(config =>//config in-memory database
            {
                //config.UseSqlServer(connectionString);
                config.UseInMemoryDatabase("Memory");
            });
            // register repositories(collection of fuctions, interfaces that abstracts your calls to the database)
            // 누겟 패키지 Microsoft.AspNetCore.Identity.EntityFrameworkCore 를 추가해주면 AddEntityFrameworkStores를 추가할 수 있다
            // AddAuthentication is called internally
            services.AddIdentity<IdentityUser, IdentityRole>(config =>
            {
                config.Password.RequiredLength = 4;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;

            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            //services.AddAuthentication.AddCookie는 identity 없이 authentication할 때 쿠키 사용하는 방식이고
            //identity에서 쿠키를 사용하기위해서는 configureApplicationCookie를 써야한다
            //https://learn.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-7.0 참고
            //https://learn.microsoft.com/en-us/aspnet/core/security/cookie-sharing?view=aspnetcore-7.0 이건 여러 어플에서 동일한 쿠키쓰기
            services.ConfigureApplicationCookie(config =>
            {
                config.Cookie.Name = "IdentityServer.Cookie";
                config.LoginPath = "/Auth/Login";//AddCookie에서는 Authenticate로 가서 클레임을 define했지만 여기서는 Login으로 가서 

            });

            services.AddIdentityServer()
                
                //identityserver와 identity를 결합하는 역할을 한다. IdentityServer.AspNetIdentity 누겟패키지 설치필요
                //이것이 없으면 sign in 할때 user object를 찾지 못함
                .AddAspNetIdentity<IdentityUser>()
                /*.AddConfigurationStore(options =>//configuration in Configuration.cs file client, api, identity etc
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>//this is mainly about operational like keep track of authorization code
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })*/
                .AddInMemoryApiResources(Configuration.GetApis())
                .AddInMemoryApiScopes(Configuration.GetApiScopes())
                .AddInMemoryIdentityResources(Configuration.GetIdentityResources())
                .AddInMemoryClients(Configuration.GetClients())   //사실상AddOAuth()를 대신하는 역할을 한다
                .AddDeveloperSigningCredential();

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //InitializeDatabase(app);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in Configuration.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Configuration.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Configuration.GetApis())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
