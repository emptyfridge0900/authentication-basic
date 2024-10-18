using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.AuthRequirement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication();
            services.AddAuthorization(config =>
            {
                /*
                 * 이것은 dafault policy 다. 만약 아무것도 설정하지 않으면 아래와 같이 설정이 된다
                 * 먼저 policy builder를 만들고 policy builder로 AuthorizationPolicy를 만들었다
                 * 우리는 AuthorizationPolicy를 직접 만들어 볼거다. 
                 * 만약 custom requirement랑 custom hanlder가 필요없다면 그냥 defaultPolicy를 사용하면 된다
                */
                var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                var defaultAuthPolicy = defaultAuthBuilder
                          .AddRequirements(new JwtRequirement())
                          .Build();
                config.DefaultPolicy = defaultAuthPolicy;
            });

            services.AddScoped<IAuthorizationHandler, JwtRequirementHandler>();

            services.AddHttpClient()//IHttpClientFactory를 inject
                .AddHttpContextAccessor();//HttpContext에 접근하게 만들어줌
            services.AddControllers();
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
