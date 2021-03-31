using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessInternationalMobileTransferSummaryVM
    {
        public const string BindProperty = "Id ,CurrencySymbol ,CurrencyCode ,AvailableBalanceDollar , AvailableBalanceCent ,TransferAmount , TransferFee" +
           " ,TotalAmount , ReceivingAmount,Receiver";

        public int Id { get; set; }
        public string CurrencySymbol { get; set; }
        public string CurrencyCode { get; set; }
        public decimal AvailableBalanceDollar { get; set; }
        public decimal AvailableBalanceCent { get; set; }

        public decimal TransferAmount { get; set; }
        public decimal TransferFee { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public string Receiver { get; set; }
    }
}