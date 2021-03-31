using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AgentInformtionViewModel
    {
        public const string BindProperty = "Id , AgentName ,BusinessRegistrationNumber,BusinessType , AddressLine1 ,AddressLine2,City , State ," +
         "ZipCode,CountryCode , PhoneCode,MobileNumber,OfficeNumber , ContactPersonName, ContactNumber";

        public int Id { get; set; }

        [Required(ErrorMessage ="Enter Agent Name"), DisplayName("Agent Name")]
        public string AgentName { get; set; }

        [Required(ErrorMessage ="Enter Bussiness Registration Number"), DisplayName("Business Registration Number")]
        public string BusinessRegistrationNumber { get; set; }

        [Required(ErrorMessage ="Enter Business Type"), DisplayName("Business Type")]
        public BusinessType BusinessType { get; set; }

        [Required(ErrorMessage ="Enter Address"), DisplayName("Address")]
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        [Required(ErrorMessage ="Enter City")]
        public string City { get; set; }

        [Required(ErrorMessage ="Enter State")]
        public string State { get; set; }

        [Required(ErrorMessage ="Enter Zip Code"), DisplayName("Zip Code")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage ="Enter Country Code")]
        public string CountryCode { get; set; }

        public string PhoneCode { get; set; }

        public string MobileNumber { get; set; }
        public string OfficeNumber { get; set; }

        [Required(ErrorMessage="Enter Contact Person Name"), DisplayName("Contact Person Name")]
        public string ContactPersonName { get; set; }

        [Required(ErrorMessage ="Enter Contact Number"), DisplayName("Contact Number")]
        public string ContactNumber { get; set; }
    }
}