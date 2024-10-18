using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Controllers
{
    public class RegisterViewModel
    {
        
        public string ReturnUrl { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]//password property
        public string ConfirmPassword { get; set; }
    }
}