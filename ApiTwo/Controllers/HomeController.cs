using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiTwo.Controllers
{
    public class HomeController:Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [Route("/")]//루트를 지정해주어서 api가 시작되자마자 바로 index로 오게 하자
        public async Task<IActionResult> Index()
        {

            //retrieve access token
            var serverClient = _httpClientFactory.CreateClient();

            //GetDiscoveryDocumentAsync사용하려면 identity model패키지 설치
            var discoveryDocument = await serverClient.GetDiscoveryDocumentAsync("https://localhost:44368/");

            /*
             * tokenResponse는 oauth서버에서 보내는 아래와 같은 형태의 json object
             var responseObject = new
            {
                access_token,//access_token=access_token
                token_type = "Bearer",
                raw_claim = "oauthTutorial"
            };
             */
            var tokenResponse=await serverClient.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address=discoveryDocument.TokenEndpoint,
                    ClientId="client_id",
                    ClientSecret="client_secret",
                    Scope="ApiOne"//여기 Scop는 ApiScope를 말하는 것이다. ApiSource를 넣으면 안됨
                });
            //retrive protected data
            var apiClient = _httpClientFactory.CreateClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);
            var apiResponse= await apiClient.GetAsync("https://localhost:44300/secret");//apione의 주소
            var content = await apiResponse.Content.ReadAsStringAsync();

            return Ok(new
            {
                access_token=tokenResponse.AccessToken,
                message=content
            });
        }

    }
}
