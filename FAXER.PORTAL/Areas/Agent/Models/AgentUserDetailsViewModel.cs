using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AgentUserDetailsViewModel
    {
        public const string BindProperty = "Id , FirstName ,MiddleName,LastName , Day ,Month,Year , CountryCode ," +
               "Gender,AgentStaffId ";
        public int Id { get; set; }

        [Required(ErrorMessage ="Enter First Name"), DisplayName("First Name")]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }

        [Required(ErrorMessage ="Enter Last Name") ,DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage="Enter Day"), DisplayName("Day")]
        public string Day { get; set; }

        [Required(ErrorMessage ="Enter Month"), DisplayName("Month")]
        public Months Month { get; set; }

        [Required(ErrorMessage ="Enter year"), DisplayName("Year")]
        public string Year { get; set; }

        [Required(ErrorMessage ="Select Country"), DisplayName("Country")]
        public string CountryCode { get; set; }

        [Required(ErrorMessage="Select Gender"), DisplayName("Gender")]
        public Gender Gender { get; set; }

        public int AgentStaffId { get; set; }

    }
}