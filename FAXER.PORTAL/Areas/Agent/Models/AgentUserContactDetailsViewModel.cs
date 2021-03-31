using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AgentUserContactDetailsViewModel
    {
        public const string BindProperty = "Id , Address1 ,Address2,City , State ,ZipCode,CountryCode , PhoneCode ," +
                 "PhoneNumber,EmailAddress, StaffType, Password, ConfirmPassword, AgentStaffId";
        public int Id { get; set; }

        [Required(ErrorMessage ="Enter Address")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Required(ErrorMessage ="Enter City") ]
        public string City { get; set; }
        [Required(ErrorMessage ="Enter State")]
        public string State { get; set; }

        [Required(ErrorMessage ="Enter Zip Code "), DisplayName("Zip Code")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage ="Select Country"), DisplayName("Country")]
        public string CountryCode { get; set; }


        public string PhoneCode { get; set; }

        [Required(ErrorMessage ="Enter Phone Number"), DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }
        
        public string EmailAddress { get; set; }

        public StaffType StaffType { get; set; }


       
        public string Password { get; set; }

 
        public string ConfirmPassword { get; set; }

        public int AgentStaffId { get; set; }



    }
}