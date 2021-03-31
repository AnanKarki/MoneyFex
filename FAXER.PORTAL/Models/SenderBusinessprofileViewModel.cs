using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderBusinessprofileViewModel
    {
        public const string BindProperty = "AccountNo,BusinessName,RegistrationNumber,Address1,Address2,City,Country,Postal," +
                        "PhoneCode,PhoneNumber,Email,ContactName,BusinessAddress1,BusinessAddress2,BusinessCity,BusinessCountry , BusinessType ,DOB " +
            ", Status , Id , CountryCurrency,CountryFlag ";

        public int Id { get; set; }
        public string AccountNo { get; set; }
        public string BusinessName { get; set; }
        public string RegistrationNumber { get; set; }
        [Required(ErrorMessage = "Enter Address")]

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Required(ErrorMessage = "Enter Address")]
        public string City { get; set; }
        public string Country { get; set; }
        public string Postal { get; set; }
        public string PhoneCode { get; set; }
        [Required(ErrorMessage = "Enter Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Enter Email")]
        public string Email { get; set; }
        public string ContactName { get; set; }
        public string BusinessAddress1 { get; set; }
        public string BusinessAddress2 { get; set; }
        public string BusinessCity { get; set; }
        public string BusinessCountry { get; set; }
        public string BusinessType { get; set; }
        public string DOB { get; set; }
        public string Status{ get; set; }
        public string CountryCurrency { get; set; }
        public string CountryFlag{ get; set; }

    }
}