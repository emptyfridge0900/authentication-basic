using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //우리가 여기서 config하고자 하는것은 Basic의 Authenticate method에서 했던거다
            // claim만들고 그것으로 유저를 만들었다
            //하지만 token manner로 하는 것 뿐
            services.AddAuthentication("OAuth")
                .AddJwtBearer("OAuth",config =>//누겟에서 Microsoft.AspNetCore.Authentication.JwtBearer 
                {
                    // provide token validation parameters
                    var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
                    var key = new SymmetricSecurityKey(secretBytes);

                    //token sent with parameter
                    config.Events = new JwtBearerEvents()
                    {
                        OnMessageReceived = context =>
                        {
                            if(context.Request.Query.ContainsKey("access_token"))
                            {
                                context.Token = context.Request.Query["access_token"];
                            }
                            return Task.CompletedTask;
                        }
                    };

                    //나는 ValidIssuer, ValidAudience, IssuersigningKey가 일치하는 접속자만 들여보내겠다
                    //accept the token that sent in header
                    config.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = Constants.Issuer,//옵션이 아닌 필수
                        ValidAudience = Constants.Audiance,//옵션이 아닌 필수

                        IssuerSigningKey = key
                    };
                });

            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();//Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation 추가
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
