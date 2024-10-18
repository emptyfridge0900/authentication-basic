using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client.Controllers
{
    public class HomeController:Controller
    {
        private readonly HttpClient _client;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
        }
        public IActionResult Index()
        {
            return View();
        }

        //이번에도 이놈이 목표
        //secret에 접근하려고 하면 접근이 안됨 그러면 자동으로 oauth server에 요청이 날아감
        [Authorize]
        public async Task<IActionResult> Secret()
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            //여기서 서버에게 call을 하려고 한다.. 서버는 JwtBearer를 사용하고 있기때문에 토큰이 없으면 접근 못한다.
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            var serverResponse = await _client.GetAsync("https://localhost:44357/secret/index");
            var apiResponse = await _client.GetAsync("https://localhost:44378/secret/index");
           
            return View();
        }
    }
}
