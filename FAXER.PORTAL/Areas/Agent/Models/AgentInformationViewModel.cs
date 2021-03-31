using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AgentsDetailsViewModel
    {
        public const string BindProperty = "AgentName , RegistrationNo , BusinessLicenseNo, EmailAddress , Password ,ConfirmPassword,ContactPerson";

        [Required(ErrorMessage ="Enter Name"),DisplayName("Name of Agent")]
        public string AgentName { get; set; }
        [Required(ErrorMessage ="Enter Registration Number"), DisplayName("Agent Business License/Registration Number")]
        public string RegistrationNo { get; set; }
        public string BusinessLicenseNo { get; set; }
        [Required(ErrorMessage ="Enter Email Address"), DisplayName("Email Address")]
        [EmailAddress]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage ="Enter Password"), DisplayName("Password")]
        public string Password { get; set; }
        [Required, DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage ="Enter Contact Person"), DisplayName("Name of Contact Person at the agency")]
        public string ContactPerson { get; set; }
    }
    public class AgentContactDetailsViewModel
    {
        public const string BindProperty = "Address1 , Address2 , City, StateProvinceRegion , ZipPostalCode ,Country,PhoneNumber, FaxNo , Website ,Accept";

        [Required(ErrorMessage ="Enter Address"),DisplayName("Address 1")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Required(ErrorMessage="Enter City"), DisplayName("City")]
        public string City { get; set; }
        [Required(ErrorMessage ="Enter State/Province/Region"), DisplayName("State/Province/Region")]
        public string StateProvinceRegion { get; set; }
        [Required(ErrorMessage ="Enter Zip/Postal Code"), DisplayName("Zip/Postal Code")]
        public string ZipPostalCode { get; set; }
        [Required(ErrorMessage ="Select Country"), DisplayName("Country ")]
        public string Country { get; set; }
        [Required(ErrorMessage ="Enter Phone Number"), DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }
        public string FaxNo { get; set; }
        public string Website { get; set; }
        public bool Accept { get; set; }
    }
}