using IdentityExample.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private SignInManager<IdentityUser> _signinManager;

        public HomeController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signinManager = signInManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        public IActionResult Login()
        {
            
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(string username,string password)
        {

            var user = await _userManager.FindByNameAsync(username);
            if(user!=null)
            {
                //3번째 parameter는 만료되지 않는 cookie를 생성하겠냐
                var signinResult = await _signinManager.PasswordSignInAsync(user, password,false,false);
                if(signinResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }
        public IActionResult Register()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = new IdentityUser
            {
                UserName = username,
                Email = ""
            };

            var result= await _userManager.CreateAsync(user, password);

            if(result.Succeeded)
            {
                var signinResult = await _signinManager.PasswordSignInAsync(user, password,false,false);
                if(signinResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }

            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> LogOut()
        {
            await _signinManager.SignOutAsync();

            return RedirectToAction("Index"); 
        }
    }
}
