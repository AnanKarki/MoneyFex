using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class WalletStatementVM
    {

        public int TransactionId { get; set; }

        public string TransactionDate { get; set; }

        public DateTime TransactionDateTime { get; set; }
        public string ReceiverORSenderName { get; set; }
        public string PaymentType { get; set; }
        public string MobileNo { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }

        public decimal Amount { get; set; }

        public decimal Fee { get; set; }

        public decimal TotalAmount { get; set; }

        public string PaymentReference { get; set; }

        public decimal Balance { get; set; }


        public TrasactionStatus TrasactionStatus { get; set; }
        public WalletStatementType WalletStatementType { get; set; }



    }

    public class KiiPayBusinessMobileWalletStatementvm
    {

        public string InvoiceDate { get; set; }
        public List<WalletStatementVM> WalletStatementListvm { get; set; }
    }
    public enum TrasactionStatus
    {

        IN,
        Out
    }
    public enum WalletStatementType {

        KiiPayPersonalIn,
        KiiPayPersonalOut,
        BusinessPaymentNationalIn,
        BusinessPaymentNationalOut,
        BusinessPaymentInternationalIn,
        BusinessPaymentInternationalOut,


    }
}