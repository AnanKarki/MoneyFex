using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessNationalMobileAccountPaymentSummaryVM
    {
        public const string BindProperty = "Id ,AvailableBalanceDollar ,AvailableBalanceCent ,SendingAmount , ReceivingAmount ,TransferFee , SmsFee ,TotalAmount," +
            " PaymentReference ,CurrencySymbol ,Receiver ";
        public int Id { get; set; }
        public decimal AvailableBalanceDollar { get; set; }
        public decimal AvailableBalanceCent { get; set; }
        public decimal SendingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public decimal TransferFee { get; set; }
        public decimal SmsFee { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentReference { get; set; }
        public string CurrencySymbol { get; set; }
        public string Receiver { get; set; }
    }
}