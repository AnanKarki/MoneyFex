using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class PayForServicesEnterAmountViewModel
    {
        public const string BindProperty = "Id ,PhotoUrl ,BusinessName , CurrencySymbol, CurrencyCode, Amount,  PaymentReference ,SendSMSNotification  ";

        public int Id { get; set; }
        public string PhotoUrl { get; set; }
        public string BusinessName { get; set; }
        public string CurrencySymbol { get; set; }
        public string CurrencyCode { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string PaymentReference { get; set; }
        public bool SendSMSNotification { get; set; }
    }
}