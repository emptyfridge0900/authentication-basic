using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Basics.AuthorizationRequirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Basics
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //object를 생성해서 UseAuthorization에 inject 해준다. service provider역할
            //HttpContext.User가 cookie로부터 생성되고 UseAuthentication()에 inject시킴
            //UseAuthentication은 cookie를 가지고 있다가 UseAuthorization에서 cookie를 검사한다
            services.AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth", config =>
                 {
                     config.Cookie.Name = "Granmas.Cookie";
                     
                    /*
                    쿠키가 없을때 로그인 페이지로 redirecr해주는데 만약 LoginPath를 설정해주지 않으면
                    기본 path는 /Account/Login?ReturnUrl=/Home/Secret가 된다
                     */
                     config.LoginPath = "/Home/Authenticate";
                 });

            services.AddAuthorization(config =>
            {
                /*
                 * 이것은 dafault policy 다. 만약 아무것도 설정하지 않으면 아래와 같이 설정이 된다
                 * 먼저 policy builder를 만들고 policy builder로 AuthorizationPolicy를 만들었다
                 * 우리는 AuthorizationPolicy를 직접 만들어 볼거다. 
                 * 만약 custom requirement랑 custom hanlder가 필요없다면 그냥 defaultPolicy를 사용하면 된다
                */
                /*var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                var defaultAuthPolicy = defaultAuthBuilder
                        .RequireAuthenticatedUser()
                        .RequireClaim(ClaimTypes.DateOfBirth)// 이걸 추가하면 data of birth를 요구하게 된다
                        .Build();
                config.DefaultPolicy = defaultAuthPolicy;*/


                /*
                 * AuthorizationPolicy(IEnumerable<IAuthorizationRequirement> requirements, IEnumerable<string> authenticationSchemes);
                 * AuthorizationPolicy는 IAuthorizationRequirement 타입의 collection과 또한 string 타입의 authenticationScheme 의 collection이 필요하다
                 * authenticationScheme은 앞에서 AddAuthentication("Cookie")한 것이 바로 Scheme이다 
                 * 
                 * 이것은 이미 .net에 있는 claimtype을 쓸 때 사용할 수 있는 방법
                */
                /*config.AddPolicy("Claim.DoB", policyBuilder =>
                 {
                     policyBuilder.RequireClaim(ClaimTypes.DateOfBirth);
                 });*/

                //이 두개는 같은 것이다.
                /*config.AddPolicy("Admin", policyBuilder => policyBuilder.RequireRole("Admin"));
                config.AddPolicy("Admin", policyBuilder => policyBuilder.RequireClaim(ClaimTypes.Role, "Admin"));*/

                /*
                 * 이것은 우리가 custom claim을 만들때 사용하는 방법
                 */
                config.AddPolicy("Claim.DoB", policyBuilder =>
                {
                    policyBuilder.AddRequirements(new CustomRequireClaim(ClaimTypes.DateOfBirth));
                });




            });

            services.AddScoped<IAuthorizationHandler,CustomRequireClaimHanlder>();

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            // 위에서 제공해준 service를 사용하는 역할
            app.UseAuthentication();

            //우리가 secret에 [Authorize]를 썼으니까 미들웨어를 추가해주어야한다. 추가 안해주면 invalidOperationException뜸
            //user context를 찾는 함수를 실행시킨다. user context를 찾는 함수가 바로 서비스에서 주입해준 handler이다
            //services.AddAuthorization에서 설정한 모든 policy를 가져다가 policy 안에 있는 requirement를 끄집어내고
            //찾아낸 requirement를 처리해줄 service를 찾는다. requirement를 처리해주는 service는 위에서 설정한 
            //service.AddScoped<IAuthorizationHandler,CustomRequireClaimHanlder> 이다.
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
