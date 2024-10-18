using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Basics.Controllers
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

        //쿠키에 policy에 Claim.DoB가 있어야지 접근할 수 있음
        [Authorize(Policy ="Claim.DoB")]
        public IActionResult SecretPolicy()
        {

            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult SecretRole()
        {

            return View();
        }

        /*
         * 쿠키가 없을때 여기로 보내진다
         * 여기서는 user(identity)를 생성해낸다
         */
        public IActionResult Authenticate()
        {
            var granmaClaims = new List<Claim>()
            { 
                new Claim(ClaimTypes.Name,"Doosan"),
                new Claim(ClaimTypes.Email,"bds0900@gmail.com"),
                new Claim(ClaimTypes.DateOfBirth,"11/11/2000"),
                new Claim(ClaimTypes.Role,"Admin"),
                new Claim("Granma.Says","good boy"),
            
            };
            var licenseClaims = new List<Claim>()
            { 
                new Claim(ClaimTypes.Name,"Doosan Beak"),
                new Claim("DrivingLicense","G"),
            };

            var granmaIdentity = new ClaimsIdentity(granmaClaims, "Grandma Identidy");
            var licenseIdentity = new ClaimsIdentity(licenseClaims, "Government");

            var userPrincipal = new ClaimsPrincipal(new[] { granmaIdentity, licenseIdentity });

           
            //이것이 없으면 cookie는 발급 안된다. 사실상 이것이 로그인
            HttpContext.SignInAsync(userPrincipal);

            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}
