using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Server.Controllers
{
    public class OAuthController:Controller
    {
        //여기는 client가 authorize에 걸렸을때 authorize를 받으려고 server에 auth요청 보내는 endpoint
        //config.AuthorizationEndpoint = "https://localhost:44357/oauth/authorize"
        public IActionResult Authorize(
            string response_type,//authorization flow type 여기서는 code
            string client_id,
            string redirect_uri,
            string scope,// 전번, 이메일 등의 리소스
            string state)//클라이언트가 식별자로 랜덤하게 생성한 문자
        {

            var query = new QueryBuilder();
            query.Add("redirect_uri", redirect_uri);//여기서 redirect_uri는 authorize가 받을 변수 이름이랑 매치시켜주면 된다
            query.Add("state", state);

            // 로그인/인증페이지로 이동한다
            return View(model:query.ToString());
        }

        //로그인/인증페이지에서 submit버튼을 누르면 여기로 오게된다
        [HttpPost]
        public IActionResult Authorize(
            string username,
            string redirect_uri,
            string state)
        {
            const string code = "client_will_use_this_to_ask_access_token";

            var query = new QueryBuilder();
            query.Add("code", code);
            query.Add("state", state);


            //client의 callback endpoint로 정보(code)를 보낸다.code는 access token이 아니다. access token을 받기위한 코드이다
            return Redirect($"{redirect_uri}{query.ToString()}");
        }

        //client는 서버로부터 받은 code를 다시 서버의 token endpoint로 보낸다
        //server는 client에게 access token을 발급해준다
        public async Task<IActionResult> Token(
            string grant_type,//access token request flow 
            string code,
            string redirect_uri,
            string client_id)
        {
            //데이터베이스에서 유저 정보를 자져다가 비교해본다.. 일단 맞다 치고 넘어가자
            /*{  }*/

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,"some_id"),
                new Claim("granny","cookie")
            };
            var secretBytes = Encoding.UTF8.GetBytes(Constants.Secret);
            var key = new SymmetricSecurityKey(secretBytes);
            var algorithm = SecurityAlgorithms.HmacSha256;

            var signingCredentials = new SigningCredentials(key, algorithm);

            var token = new JwtSecurityToken(
                  Constants.Issuer,
                  Constants.Audiance,
                  claims,
                  notBefore: DateTime.Now,
                  expires: DateTime.Now.AddHours(1),
                  signingCredentials);

            var access_token = new JwtSecurityTokenHandler().WriteToken(token);

            var responseObject = new
            {
                access_token,//access_token=access_token
                token_type = "Bearer",
                raw_claim = "oauthTutorial"
            };

            var responseJson = JsonConvert.SerializeObject(responseObject);
            var responeBytes = Encoding.UTF8.GetBytes(responseJson);

            await Response.Body.WriteAsync(responeBytes, 0, responeBytes.Length);


            return Redirect(redirect_uri);
        }
        [Authorize]//이건 붙여도 되고 안붙여도 되고..
        public IActionResult Validate()
        {
            if(HttpContext.Request.Query.TryGetValue("access_token",out var accessToken))
            {
                return Ok();
            }
            
            return BadRequest();
        }
    }
}
