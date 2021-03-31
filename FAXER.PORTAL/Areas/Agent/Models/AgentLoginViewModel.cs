using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AgentLoginViewModel
    {
        public const string BindProperty = "IsFirstLogin , Email ,AgentCode ,StaffCode ,Password ,NewPassword ,ConfirmPassword ,TimeZone";
        public int IsFirstLogin { get; set; }
        [Required(ErrorMessage ="Enter Email Address")]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage ="Enter Agent Code")]
        [Display(Name = "Agent Code")]
        public string AgentCode { get; set; }
        [Required(ErrorMessage ="Enter Staff Code")]
        [Display(Name = "Staff Code")]
        public string StaffCode { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string TimeZone { get; set; }
    }

    public class AgentConfirmResetPasswordViewModel
    {

        public const string BindProperty = "NewPassword , ConfirmPassword ";

        [Required(ErrorMessage = "Enter New Password"), DisplayName("New Password")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage ="Enter Confirm Password"),DisplayName("Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }
    }
}