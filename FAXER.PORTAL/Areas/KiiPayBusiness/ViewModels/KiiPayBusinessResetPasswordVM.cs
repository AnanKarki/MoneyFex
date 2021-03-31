using System.ComponentModel.DataAnnotations;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessResetPasswordVM
    {
        public const string BindProperty = "Email , Password,ConfirmPassword ";
        public string Email { get; set; }

        //[Required(ErrorMessage = "Please enter the password")
        //PasswordPolicy(ErrorMessage = "password policy did not match !")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Please re-enter your password!")]
        [Compare("Password", ErrorMessage = "the password did not match!")]
        public string ConfirmPassword { get; set; }

    }
}