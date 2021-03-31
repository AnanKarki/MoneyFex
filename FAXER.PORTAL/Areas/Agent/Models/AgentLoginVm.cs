using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AgentLoginVm
    {
        public int Id { get; set; }
        public string Email { get; set; }

        [Required(ErrorMessage ="EnterAgent Code") ]
        public string AgentCode { get; set; }

        [Required(ErrorMessage="Enter Staff Code")]
        public string StaffCode { get; set; }

        [Required(ErrorMessage="Enter Password")]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class AgentLoginConfirmVm
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Enter Agent Code")]
        public string AgentCode { get; set; }

        [Required(ErrorMessage ="Enter Staff Code")]
        public string StaffCode { get; set; }

        [Required(ErrorMessage ="Enter Password") ]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, DisplayName("Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}