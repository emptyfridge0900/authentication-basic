using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basics.AuthorizationRequirements
{
    public class CustomRequireClaim: IAuthorizationRequirement
    {
        public CustomRequireClaim(string claimType)
        {
            ClaimType = claimType;
        }

        public string ClaimType { get; }
    }
    public class CustomRequireClaimHanlder : AuthorizationHandler<CustomRequireClaim>
    {
        //access to the dependency injection container
        //만약 데이터베이스에 접속하고 싶다던가 다른 서비스가 필요하다면  constructor에서 가져다가 쓰자
        //이런 장점이 있기 때문에 custorm handler를 만들어서 쓰는 것
        public CustomRequireClaimHanlder()
        {

        }
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            CustomRequireClaim requirement)
        {
            //User is ClaimPrincipal that created in HomeController
            //requirment에 일치하는 claim이 있는지 확인
            var hasClaim=context.User.Claims.Any(x => x.Type == requirement.ClaimType);

            //있으면 이 handler가 성공적으로 handle했다고 표시해준다
            if(hasClaim)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }

    }
}
