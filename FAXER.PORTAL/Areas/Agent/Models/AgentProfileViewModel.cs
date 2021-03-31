using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AgentProfileViewModel
    {
        public const string BindProperty = "Id , AgentStaffId ,FullName,Address1 , Address2 ,City,State , PostalCode ," +
          "Country,CountryCode , CountryPhoneCode,PhoneNumber,Email , PhotoIdUrl ,Address ," +
            "Address1Hidden, CityHidden,PostalCodeHidden,PhoneNubmerHidden , EmailHidden, AccountNo , AgentName, RegistrationNumber ";

        public int Id { get; set; }
        public int AgentStaffId { get; set; }
        public string FullName { get; set; }

        [Required(ErrorMessage = "Enter Address 1")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }

        [Required(ErrorMessage = "Enter City")]
        public string City { get; set; }
        public string State { get; set; }
        [Required(ErrorMessage = "Enter Postal Code")]
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string CountryPhoneCode { get; set; }
        [Required(ErrorMessage ="Enter Phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Enter Email ")]
        public string Email { get; set; }
        public string PhotoIdUrl { get; set; }
        public string Address { get; set; }
        public string AccountNo { get; set; }
        public string AgentName { get; set; }
        public string RegistrationNumber { get; set; }

        //Hidden Strings
        public string Address1Hidden { get; set; }
        public string CityHidden { get; set; }
        public string PostalCodeHidden { get; set; }
        public string PhoneNubmerHidden { get; set; }
        public string EmailHidden { get; set; }

    }
}