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
    public class StaffComplianceDocViewModel
    {
        public const string BindProperty = "Id , IdCaardType ,IdCardNumber,ExpiryDay , ExpiryMonth ,ExpiryYear,IssuingCountry , IdentificationDoc ," +
           "HaveInternationalPassport,PassportSide1 , PassportSide2,AgentStaffId";

        public int Id { get; set; }

        [Required(ErrorMessage ="Enter Id Type"), DisplayName("Id Type")]
        public string IdCaardType { get; set; }

        [Required(ErrorMessage ="Enter Id Number"), DisplayName("Id Number")]
        public string IdCardNumber { get; set; }

        [Required(ErrorMessage ="Enter Expiry Day"), DisplayName("Expiry Day")]
        public int ExpiryDay { get; set; }

        [Required(ErrorMessage = "Enter Expiry Month"), DisplayName("Expiry Month")]
        public Month ExpiryMonth { get; set; }

        [Required(ErrorMessage ="Enter Expiry Year"), DisplayName("Expiry Year")]
        public int ExpiryYear { get; set; }

        [Required(ErrorMessage ="Select Country"), DisplayName("Issuing Country")]
        public string IssuingCountry { get; set; }

        [DisplayName(" Government issued Identification Document.")]
        public string IdentificationDoc { get; set; }
        
        public bool HaveInternationalPassport { get; set; }
        public string PassportSide1 { get; set; }
        public string PassportSide2 { get; set; }

        public int AgentStaffId { get; set; }
     
    }
}