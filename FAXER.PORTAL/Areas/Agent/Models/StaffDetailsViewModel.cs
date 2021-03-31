using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class StaffDetailsViewModel
    {
        public const string BindProperty = "Id , FirstName ,MiddleName,LastName , Day ,Month,Year , BirthCountry ,Gender";

        public int Id { get; set; }

        [Required(ErrorMessage ="Enter First Name"), DisplayName("First Name")]        
        public string FirstName { get; set; }
        public string MiddleName { get; set; }

        [Required(ErrorMessage ="Enter Last Name"), DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage ="Enter Day"), DisplayName("Day")]
        public string Day { get; set; }
        [Required(ErrorMessage ="Enter Month")]
        public Month Month { get; set; }
        [Required(ErrorMessage ="Enter Year")  ]
        public int Year { get; set; }
        [Required(ErrorMessage ="Select Counttry"), DisplayName("Country")]
        public string BirthCountry { get; set; }
        [Required(ErrorMessage ="Select Gender")]
        public Gender Gender { get; set; }

    }
}