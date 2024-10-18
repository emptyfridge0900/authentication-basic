using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityExample.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityExample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //AdDbContext requires entityframework
            services.AddDbContext<AppDbContext>(config =>//config in-memory database
            {
                config.UseInMemoryDatabase("Memory");
            });
            // register repositories(collection of fuctions, interfaces that abstracts your calls to the database)
            // 누겟 패키지 Microsoft.AspNetCore.Identity.EntityFrameworkCore 를 추가해주면 AddEntityFrameworkStores를 추가할 수 있다
            services.AddIdentity<IdentityUser, IdentityRole>(config=> 
            {
                config.Password.RequiredLength = 4;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
                
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            //services.AddAuthentication.AddCookie는 authentication할 때 쿠키 사용하는 방식이고
            //identity에서 쿠키를 사용하기위해서는 configureApplicationCookie를 써야한다
            services.ConfigureApplicationCookie(config =>
            {
                    config.Cookie.Name = "Identity.Cookie";
                    config.LoginPath = "/Home/Login";//AddCookie에서는 Authenticate로 가서 클레임을 define했지만 여기서는 Login으로 가서 

            });

           /*services.AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", config =>
                {
                    config.Cookie.Name = "Granmas.Cookie";
                    config.LoginPath = "/Home/Authenticate";
                });*/

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
