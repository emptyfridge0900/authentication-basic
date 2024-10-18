using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ApiTwo
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer",config =>
                {
                    config.Authority = "https://localhost:44368/";//IdentityServer ip
                    config.Audience = "ApiTwo";//authenticate 하려는 ApiSesource가 무엇? ApiScope아님
                });//this design to work with OpenId Connect

            //JWT 설정은 apione이랑 같지만 apione설정에는 addhttpclient는 없음..
            //왜냐면 apitwo는 client역할을 하기 때문에 servcer랑 apione에게 request를 날려야되서 httpclient가 필요   
            services.AddHttpClient();
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
                endpoints.MapControllers();
            });
        }
    }
}
