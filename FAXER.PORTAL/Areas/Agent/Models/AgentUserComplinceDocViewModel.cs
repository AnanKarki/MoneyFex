using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AgentUserComplinceDocViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Enter Id Card Type"), DisplayName("Id Card Type")]
        public string IdCaardType { get; set; }

        [Required(ErrorMessage ="Enter Id Card Numnber"), DisplayName("Id Card Number")]
        public string IdCardNumber { get; set; }

        [Required( ErrorMessage ="Enter Expiry Date"), DisplayName("Expiry Day")]
        public int ExpiryDay { get; set; }

        [Required(ErrorMessage ="Enter Exipry Monty"), DisplayName("Expiry Month")]
        public Month ExpiryMonth { get; set; }

        [Required(ErrorMessage ="Enter Expiry Year"), DisplayName("Expiry Year")]
        public Month ExpiryYear { get; set; }

        [Required(ErrorMessage ="Enter Issuing Country"), DisplayName("Issuing Country")]
        public string IssuingCountry { get; set; }

        [Required(ErrorMessage ="Choose government issued identification document"), DisplayName(" Government issued Identification Document.")]
        public string IdentificationDoc { get; set; }

        [Required(ErrorMessage ="Select International passport"), DisplayName("Does the staff have an international passport?")]
        public bool HaveInternationalPassport { get; set; }
        public string PassportSide1 { get; set; }
        public string PassportSide2 { get; set; }

    }
}