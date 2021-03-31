using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AdminRegisteredAgentStaffInfoViewModel
    {
        public const string BindProperty = "Id ,Country,CountryFlag ,AgentName , StaffName, AgentCode,StaffCode ,Status , AccountNumber, PhoneNumber,DateOfBirth , EmailAddress" +
            ",Branch , Address ,IdCardType , IdCardNumber ,Day , Month ,Year ,IdIssuingCountry,AgentStaffId,AgentId";
        public int Id { get; set; }
        public string Country { get; set; }
        public string CountryFlag { get; set; }
        [Required(ErrorMessage = "Enter Name")]
        public string AgentName { get; set; }
        public string StaffName { get; set; }
        public string AgentCode { get; set; }
        public string StaffCode { get; set; }
        public string Status { get; set; }
        public string AccountNumber { get; set; }
        [Required(ErrorMessage = "Enter Phone Number")]
        public string PhoneNumber { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Enter Date of Birth")]
        public DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = "Enter Email Address")]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "Enter Branch")]
        public string Branch { get; set; }
        [Required(ErrorMessage = "Enter Address")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Select IdCardType")]
        public string IdCardType { get; set; }
        [Required(ErrorMessage = "Enter IdCardNumber")]
        public string IdCardNumber { get; set; }

        [Range(1, 32, ErrorMessage = "Enter a valid day")]
        [Required(ErrorMessage = "Enter Day")]
        public int Day { get; set; }

        [Required(ErrorMessage = "Enter Month")]
        public Month Month { get; set; }

        [Range(1000, int.MaxValue, ErrorMessage = "Enter a valid year")]
        [Required(ErrorMessage = "Enter Year")]
        public int Year { get; set; }
        [Required(ErrorMessage = "select IdIssuingCountry")]
        public string IdIssuingCountry { get; set; }
        public DateTime IdExpiryDate { get; set; }
        public int AgentStaffId { get; set; }
        public int AgentId { get; set; }
        public string AgentType { get; set; }

    }
}