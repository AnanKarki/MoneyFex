using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FAXER.PORTAL.Models
{
    public class ExternalLoginConfirmationViewModel
    {

        public const string BindProperty = "Email";


        [Required(ErrorMessage ="Enter Email Address")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public const string BindProperty = "SelectedProvider,Providers,ReturnUrl,RememberMe";

        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {

        public const string BindProperty = "Provider,Code,ReturnUrl,RememberBrowser,RememberMe ";
        [Required(ErrorMessage ="Enter Provider")]
        public string Provider { get; set; }

        [Required(ErrorMessage ="Enter Code")]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required(ErrorMessage = "Enter Email Address")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {

        public const string BindProperty = "Email,Password,RememberMe,EndDate,Count,isBusiness";

        [Required(ErrorMessage ="Enter Email Address")]
        [Display(Name = "Email")]
        
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me on this device (Not recommended on public shared devices)")]
        public bool RememberMe { get; set; }
  
        [Display(Name = "End Date)")]
        public DateTime EndDate { get; set; }
        public int Count { get; set; }
        public bool isBusiness { get; set; }


    }

    public class RegisterViewModel
    {
        public const string BindProperty = "Email,Password,ConfirmPassword,FirstName,MiddleName,LastName,DateOfBirth,AccountNo,GGender";
        [Required(ErrorMessage ="Enter Email Address")]
        [EmailAddress]
        [Display(Name = "Username(email address)")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter valid email")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Enter Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }


        [Required(ErrorMessage ="Enter First Name")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Invalid First Name")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }


        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Invalid Middle Name")]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage="Enter Last Name")]
        [RegularExpression("^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Invalid Last Name")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage ="Enter Date of Birth") ]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [StringLength(8)]
        public string AccountNo { get; set; }

        [Display(Name = "Gender")]
        [Required(ErrorMessage = "Select Gender.")]
        public Gender? GGender
        { get; set; } //= null;



    }
    public enum Gender
    {
        [Display(Name = "Male", Order = 0)]
        Male = 0,

        [Display(Name = "Female", Order = 1)]
        Female = 1,

    }
    public class ResetPasswordViewModel
    {

        public const string BindProperty = "Email,Password,ConfirmPassword,Code";


        [Required(ErrorMessage ="Enter Email Address")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage ="Enter Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        public const string BindProperty = "Email";

        [Required(ErrorMessage ="Enter Email Address")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public enum TransferType
    {


        KiiPayWallet,
        PayForServices,
        CashPickup,
        MobileTransfer,
        BankDeposit,
        PayARequest,
        BillPayment
    }


}
