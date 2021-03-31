using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class MobileTransferPaymentSummaryViewModel
    {
        public const string BindProperty = "Id ,ReceivingCurrencyCode ,ReceivingCurrnecySymbol ,SendingCurrencyCode , SendingCurrencySymbol , Amount ,Fee  ," +
            " LocalSmsFee, TotalAmount, ReceivingAmount,PaymentReference , ReceiversName ";
        public int Id { get; set; }
        public string ReceivingCurrencyCode { get; set; }
        public string ReceivingCurrnecySymbol { get; set; }
        public string SendingCurrencyCode { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public decimal LocalSmsFee { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public string PaymentReference { get; set; }
        public string ReceiversName { get; set; }



    }
}