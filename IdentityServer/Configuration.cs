using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer
{
    public static class Configuration
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() =>
            new List<IdentityResource> {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource
                { 
                    //this is possible scope that requested with these claims
                    //이것은 rc.granma라는 클레임을 가지고 있고, 이름은 rc.scope인 scope이다
                    //누가 이것을 사용할 수 있는지 아직 모르지만 어쨌든 이런 스코프가 있다는걸 알려줌
                    Name="rc.scope",
                    UserClaims =//rc.scope와 함께 오는 claim은 무엇인가?
                    {
                        "rc.granma"
                    }
                }

            };
        public static IEnumerable<ApiResource> GetApis() =>
            new List<ApiResource> {
                new ApiResource("ApiOne"),
                new ApiResource("ApiTwo",new string[]{ "rc.api.granma"}),
                new ApiResource("TestApi")
                {
                    Scopes={ "ApiOne","ApiTwo"},
                    UserClaims=new[] { "user_level" }//new string[]{ "user_level"}과 동일
                }
            };
        public static IEnumerable<ApiScope> GetApiScopes() =>
            new ApiScope[]
            {
                new ApiScope("ApiOne"),
                new ApiScope("ApiTwo")
            };
        public static IEnumerable<Client> GetClients() =>
            new List<Client> { 
                /*
                 * 이 클라이언트는
                 * client_id 가 id이고
                 * client_secret가 secrets이고
                 * clientCredentials를 granttypes으로 가지며
                 * 허락받은 scopes은 ApiOne 뿐이다
                 * 
                 * 위의 조건을 만족하는 client
                 */
                new Client
                {
                    ClientId="client_id",
                    ClientSecrets={new Secret("client_secret".ToSha256()) },
                    AllowedGrantTypes=GrantTypes.ClientCredentials ,//clientCredential은 machine to machine communication이라서 concent page필요없다
                    AllowedScopes={"ApiOne"}
                },
                new Client
                {
                    ClientId="client_id_mvc",
                    ClientSecrets={new Secret("client_secret_mvc".ToSha256()) },
                    AllowedGrantTypes=GrantTypes.Code,
                    //위에서 Identity resource에서openid, profile를 추가했으니까 scope에 추가해주자
                    AllowedScopes=//여기에 나열된 scope들은 이 클라이언트가 사용할 수 있도록 허람됨, Identity Resource나 Api Resource에 등록된 scope만 사용가능
                    {
                        "ApiOne",//api resource
                        "ApiTwo",//api resource
                        IdentityServerConstants.StandardScopes.OpenId,//identity resource
                        IdentityServerConstants.StandardScopes.Profile,//identity resource
                        "rc.scope",//identity resource
                    },
                    //openid connect middleware의 기본 endpoint는 singin-oidc이다
                    //mvc_client의 주소/signin-oidc
                    //RedirectUris={"https://localhost:44349/signin-oidc" },
                    RedirectUris={"https://localhost:44316/signin-oidc" },
                    RequireConsent=false,//consent page가 안뜨게 한다. 이러이러한 권한을 수락하시겠습니까? 하고 물어보는 페이지

                    //IdentityServer가 제공하는 옵션. client로 부터 authorization code를 돌려받고나서 id token이랑 access token으로 바꿔준다
                    //user로부터 유저 정보는 얻는 방법은 user한테 user information을 달라고 요청을 보낸다. 이것은 옵션을 true로 해줌으로 자동으로 이루어진다
                    //id token에다 모든 claim을 넣어준다. load all the claim to the id token
                    //AlwaysIncludeUserClaimsInIdToken=true,
                }
            };
    }
}
