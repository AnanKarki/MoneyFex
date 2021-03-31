using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class FaxerDetails
    {
        [Required(ErrorMessage ="Enter First Name")]
        [Display(Name = "First Name")]
        [RegularExpression(@"[a-zA-Z]+$", ErrorMessage = "Please, use letters only")]
        public string FirstName { get; set; }

       
        [Display(Name = "Middle Name")]
        [RegularExpression(@"[a-zA-Z]+$", ErrorMessage = "Please, use letters only")]
        public string MiddleName { get; set; }


        [Required(ErrorMessage ="Enter Last Name")]
        [Display(Name = "Last Name")]
        [RegularExpression(@"[a-zA-Z]+$", ErrorMessage = "Please, use letters only")]
        public string LastName { get; set; }

        [Required(ErrorMessage ="Enter Date of Birth")]
        [Display(Name = "Date Of Birth")]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage ="Select Gender")]
        [Display(Name = "Gender")]
        public DateTime Gender { get; set; }



    }
}