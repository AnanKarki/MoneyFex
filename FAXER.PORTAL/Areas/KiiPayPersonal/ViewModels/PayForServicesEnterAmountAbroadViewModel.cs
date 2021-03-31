using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class PayForServicesEnterAmountAbroadViewModel
    {
        public const string BindProperty = "Id ,PhotoUrl ,ReceiversName , SendingAmount, ReceivingAmount, SendingCurrencyCode,ReceivingCurrencyCode ,SendingCurrencySymbol" +
            " ,ReceivingCurrencySymbol ,Fee ,PayingAmount , ExchangeRate , PaymentReference  ";

        public int Id { get; set; }
        public string PhotoUrl { get; set; }
        public string ReceiversName { get; set; }
        [Required]
        public decimal SendingAmount { get; set; }
        [Required]
        public decimal ReceivingAmount { get; set; }
        public string SendingCurrencyCode { get; set; }
        public string ReceivingCurrencyCode { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
        public decimal Fee { get; set; }
        public decimal PayingAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        [Required]
        public string PaymentReference { get; set; }
    }
}