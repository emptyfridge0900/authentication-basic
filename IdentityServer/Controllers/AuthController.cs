using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer.Controllers
{
    public class AuthController: Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            //check if the model is valid
            var result= await _signInManager.PasswordSignInAsync(vm.Username, vm.Password,false,false);
            if(result.Succeeded)
            {
                return Redirect(vm.ReturnUrl);
            }
            
            return View();
        }
        [HttpGet]
        public IActionResult Register(string returnUrl)
        {
            return View(new RegisterViewModel { ReturnUrl = returnUrl });
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            //RegisterViewModel에서 [require][compare]같은 속성을 사용했기 때문에 여기서 ModelState.IsValid사용
            if(!ModelState.IsValid)
            {
                return View(vm);
            }
            var user = new IdentityUser(vm.Username);
            var result=await _userManager.CreateAsync(user, vm.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user,false);
                return Redirect(vm.ReturnUrl);
            }
            
            return View();
        }

    }
}
