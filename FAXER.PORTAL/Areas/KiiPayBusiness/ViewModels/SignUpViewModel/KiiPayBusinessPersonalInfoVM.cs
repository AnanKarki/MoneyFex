using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels.SignUpViewModel
{
    public class KiiPayBusinessPersonalInfoVM
    {
        public const string BindProperty = " FirstName,MiddleName ,LastName , BirthDate, Gender";
        [Required(ErrorMessage ="Please enter your first name..")]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Please enter your last name..")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter your birth date..")]
        public DateTime? BirthDate { get; set; }


        public Gender Gender { get; set; }

    }
}