using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Server.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        //cookie만 있으면 접근 할 수 있음
        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }



        /*
         * 쿠키가 없을때 여기로 보내진다
         * 여기서는 user(identity)를 생성해낸다
         */
        public IActionResult Authenticate()
        {
            var claims=new []
            {
                new Claim(JwtRegisteredClaimNames.Sub,"some_id"),
                new Claim("granny","cookie")
            };
            /*
             * JwtSecurityToken(
             *     string issuer = null, 
             *     string audience = null, 
             *     IEnumerable<Claim> claims = null, 
             *     DateTime? notBefore = null, DateTime? 
             *     expires = null, 
             *     SigningCredentials signingCredentials = null);
            */
            var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
            var key = new SymmetricSecurityKey(secretBytes);
            var algorithm = SecurityAlgorithms.HmacSha256;

            var signingCredentials = new SigningCredentials(key,algorithm);

            var token = new JwtSecurityToken(
                  Constants.Issuer,
                  Constants.Audiance,
                  claims,
                  notBefore: DateTime.Now,
                  expires: DateTime.Now.AddHours(1),
                  signingCredentials);

            var tokenJson = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new { access_token=tokenJson});
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
