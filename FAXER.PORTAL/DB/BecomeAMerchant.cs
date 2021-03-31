using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class BecomeAMerchant
    {
        public int Id { get; set; }
        public string RegistrationNumber { get; set; }
        [Required]
        [RegularExpression(@"^[a-z||A-Z]+[a-zA-Z""'\s-]*$", ErrorMessage = "Only Aphabetic Character is allowed")]
        public string FirstName { get; set; }
        [Required]
        [RegularExpression(@"^[a-z||A-Z]+[a-zA-Z""'\s-]*$", ErrorMessage = "Only Aphabetic Character is allowed")]
        public string LastName { get; set; }
        [Required]
        public string CompanyBusinessName { get; set; }

        public BusinessType BusinessType { get; set; }
        [Required]
        public string BusinessLicenseRegistrationNumber { get; set; }
        [Required]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        
        public string Street { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string StateProvince { get; set; }
        [Required]
        public string PostZipCode { get; set; }

        public string CountryCode { get; set; }

        [Required]
        //[RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter valid email")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter valid email")]
        public string BusinessEmailAddress { get; set; }
        [Required]
        
        public string ContactPhone { get; set; }
        public string FaxNo { get; set; }

        //[RegularExpression(@"(http(s)?://)?([\w-]+\.)+[\w-]+(/[\w- ;,./?%&=]*)?", ErrorMessage = "Please enter valid URL")]
        public string Website { get; set; }
        public string ActivationCode { get; set; }

    

    }
}