using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MvcClient.Controllers
{
    public class HomeController:Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            return View();

        }
        [Authorize]
        public async Task<IActionResult> Secret()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var idToken = await HttpContext.GetTokenAsync("id_token");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            var claims = User.Claims.ToList();
            //client는 서버의 token endpoint에다가 authorize code를 요청하게 되는데
            //그러면 서버에서 response body 에 id token이랑 access token을 보내준다 (refresh token 은 optional)
            var _accessToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var _idToken = new JwtSecurityTokenHandler().ReadJwtToken(idToken);

            var result = await GetSecret(accessToken);

            return View();
        }
        public async Task<string> GetSecret(string accessToken)
        {
            var apiClient = _httpClientFactory.CreateClient();
            apiClient.SetBearerToken(accessToken);
            var apiResponse = await apiClient.GetAsync("https://localhost:44300/");//apione의 주소
            var content = await apiResponse.Content.ReadAsStringAsync();

            return content;
        }
    }
}
