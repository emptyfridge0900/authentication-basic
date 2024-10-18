using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Client
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(config=> 
            {
                //we check the cookies to confirm that we are authenticated
                config.DefaultAuthenticateScheme = "ClientCookie";
                //when we sign in we will deal out a cookie
                config.DefaultSignInScheme = "ClientCookie";
                // use this to check  if we are allowed to do somehting
                //authorize endpoint에 접근했을때 lognin page로 redirect하는 대신 AddOAuth()에 설정한 대로 한다
                config.DefaultChallengeScheme = "OurServer";
            })
                .AddCookie("ClientCookie")
                .AddOAuth("OurServer",config=> 
                {
                    config.ClientId = "client_id";
                    config.ClientSecret = "client_secret";

                    //서버 주소/authorizeendpoint
                    //authorize하라고 유저를 날려 보낼 목적지. 예를 들어 google의 수락페이지
                    //콜백주소, clientid, clientsecret, 등등을 보낸다
                    //response_type, client_id, redirect_uri, scope, state
                    config.AuthorizationEndpoint = "https://localhost:44357/oauth/authorize";
                    
                    //Identity server가 유저에게 code를 손에 들려서 보내줄 주소
                    config.CallbackPath = "/oauth/callback1";
                    //client가 code를 받은 후 code를 identity server에게 보낼때 사용하는 identity server의 endpoint
                    config.TokenEndpoint = "https://localhost:44357/oauth/token";

                    config.SaveTokens = true;

                    config.Events = new OAuthEvents()
                    {
                        OnCreatingTicket = context =>
                        {
                            var accessToken = context.AccessToken;
                            var base64Payload = accessToken.Split('.')[1];
                            var bytes = Convert.FromBase64String(base64Payload);
                            var jsonPayload = Encoding.UTF8.GetString(bytes);
                            var claims = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonPayload);
                            
                            foreach(var claim in claims)
                            {
                                context.Identity.AddClaim(new Claim(claim.Key, claim.Value));
                            }
                            


                            return Task.CompletedTask;
                        }
                    };

                });

            services.AddHttpClient();//IHttpClientFactory를 DI해준다. IhttpClientFactory는 client ojbect를 만드는데 쓰임
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();//Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation 추가
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
