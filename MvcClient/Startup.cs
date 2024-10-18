using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MvcClient
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(config=> 
            {
                config.DefaultScheme = "Cookie";
                config.DefaultChallengeScheme = "oidc";
            })
                .AddCookie("Cookie")
                .AddOpenIdConnect("oidc",config=> 
                {
                    config.Authority = "https://localhost:44368/";//identityServer 의 주소
                    config.ClientId = "client_id_mvc";
                    config.ClientSecret = "client_secret_mvc";
                    config.SaveTokens = true;

                    config.ResponseType = "code";

                    //id token을 얻은 후에 또다시 claim을 요구해서 가져온다
                    // one trip or two trips 
                    //한번에 id token안에 claim을 넣어서 받던지 아니면 id token 받고나서 다시 한번 claim을 받던지
                    //name, given name, family name, amr, idp s_hash 등등의 추가 정보를 UserInfo endpoint를 통해서 가져옴. 
                    config.GetClaimsFromUserInfoEndpoint = true;


                    //configure cookie claim mapping
                    config.ClaimActions.DeleteClaim("amr");
                    config.ClaimActions.MapUniqueJsonKey("RawCoding.granma","rc.granma");

                    //config.GetClaimsFromUserInfoEndpoint = true;가 되있을때 사용
                    //scope을 추가해보자
                    config.Scope.Clear();
                    config.Scope.Add("openid");
                    config.Scope.Add("rc.scope");
                    config.Scope.Add("ApiOne");//ApiOne에 접근하고 싶당...
                    //config.Scope.Add("Profile");//Profile scope에 있는 정보를 추가로 요구

                });//Microsoft.AspNetCore.Authentication.OpenIdConnct 누겟 패키지 설치

            services.AddHttpClient();

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
