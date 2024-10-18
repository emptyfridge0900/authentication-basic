using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Api.AuthRequirement
{
    //그냥 비워두자. 이건 default로 쓸거니까
    public class JwtRequirement:IAuthorizationRequirement
    {
        
    }

    //여기서 identity server에게 token을 보낸다
    public class JwtRequirementHandler : AuthorizationHandler<JwtRequirement>
    {
        private readonly HttpClient _client;//Server에게 token을 보낸다
        private readonly HttpContext _httpContext;//token을 여기서 꺼낸다

        public JwtRequirementHandler(
            IHttpClientFactory httpClientFactory, 
            IHttpContextAccessor httpContextAccessor)
        {
            _client = httpClientFactory.CreateClient();
            _httpContext = httpContextAccessor.HttpContext;
        }


        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            JwtRequirement requirement)
        {
          
            if(_httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var accessToken = authHeader.ToString().Split(' ')[1];
                var serverResponse = await _client
                    .GetAsync($"https://localhost:44357/oauth/validate?access_token={accessToken}");
                if(serverResponse.StatusCode==System.Net.HttpStatusCode.OK)
                {
                    context.Succeed(requirement);
                }
            }
        }

    }

}
