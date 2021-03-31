using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AgentStaffInformationViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }
        public string Address1 { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }

        public string IdCardType { get; set; }
        public string IdCardNumber { get; set; }
        public DateTime IdCardExpiryDate { get; set; }
        public string IssuingCountry { get; set; }
        public string Id1 { get; set; }
        public string Id2 { get; set; }
        public string Id3 { get; set; }
        public bool IsActive { get; set; }
        public string StaffId{ get; set; }
        public string StaffLoginCode{ get; set; }

        [Required(ErrorMessage = "Enter Expiry Day"), DisplayName("Expiry Day")]
        public int ExpiryDay { get; set; }

        [Required(ErrorMessage = "Enter Expiry Month"), DisplayName("Expiry Month")]
        public Month ExpiryMonth { get; set; }

        [Required(ErrorMessage = "Enter Expiry Year"), DisplayName("Expiry Year")]
        public int ExpiryYear { get; set; }
        public string CountryCode { get;  set; }
        public string IssuingCountryCode { get;  set; }
    }

}