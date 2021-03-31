using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class PaymentSelectionViewModel
    {
        public const string BindProperty = " Id,SendingCountry , ReceivingCountry, PaymentMethod,PaymentMethodName,SendingCountryFlag,ReceivingCountryFlag";

        public int Id { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string SendingCountry { get; set; }

        [Required(ErrorMessage = "Select Country")]
        public string ReceivingCountry { get; set; }
        public SenderPaymentMode PaymentMethod { get; set; }
        public string PaymentMethodName { get; set; }
        public string SendingCountryFlag { get; set; }
        public string ReceivingCountryFlag { get; set; }
    }
}