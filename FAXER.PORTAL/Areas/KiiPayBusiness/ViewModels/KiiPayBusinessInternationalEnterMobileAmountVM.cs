using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessInternationalEnterMobileAmountVM
    {
        public const string BindProperty = "Id ,Sender ,SenderImageUrl ,SenderCurrencySymbol , SenderCurrencyCode ,ReceiverCurrencySymbol , ReceiverCurrencyCode" +
            " ,AvailableBalanceDollar , AvailableBalanceCent,SendingAmount , TransferFee, TotalAmount, ReceivingAmount, ExchangeRate";
        public int Id { get; set; }
        public string Sender { get; set; }
        public string SenderImageUrl { get; set; }
        public string SenderCurrencySymbol { get; set; }
        public string SenderCurrencyCode { get; set; }
        public string ReceiverCurrencySymbol { get; set; }
        public string ReceiverCurrencyCode { get; set; }
        public decimal AvailableBalanceDollar { get; set; }
        public decimal AvailableBalanceCent { get; set; }
        [Required]
        public decimal SendingAmount { get; set; }
        public decimal TransferFee { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public decimal ExchangeRate { get; set; }
    }
}