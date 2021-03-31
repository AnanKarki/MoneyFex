using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class KiiPayEnterAmountViewModel
    {
        public const string BindProperty = "Id ,Name ,CurrencySymbol ,CurrencyCode , Amount, SendSMSNotification, PaymentReference , PhotoUrl";

        public int Id { get; set; }
        public string Name { get; set; }
        public string CurrencySymbol { get; set; }
        public string CurrencyCode { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public bool SendSMSNotification { get; set; }
        [Required]
        public string PaymentReference { get; set; }
        public string PhotoUrl { get; set; }
    }
}