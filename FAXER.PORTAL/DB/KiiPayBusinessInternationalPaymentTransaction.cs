using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class KiiPayBusinessInternationalPaymentTransaction
    {

        public int Id { get; set; }

        public int PayedFromKiiPayBusinessInformationId { get; set; }

        public int PayedFromKiiPayBusinessWalletId { get; set; }

        public int PayedToKiiPayBusinessInformationId { get; set; }

        [ForeignKey("PayedToKiiPayBusinessWallet")]
        public int PayedToKiiPayBusinessWalletId { get; set; }

        public decimal RecievingAmount { get; set; }
        public decimal FaxingAmount { get; set; }
        public decimal FaxingFee { get; set; }
        public decimal ExchangeRate { get; set; }

        public decimal TotalAmount { get; set; }
     
        public string ReceiptNumber { get; set; }

        public string PaymentReference { get; set; }


        public bool IsAutoPayment { get; set; }

        public bool IsRefunded { get; set; }


        public System.DateTime TransactionDate { get; set; }

        public virtual KiiPayBusinessWalletInformation PayedToKiiPayBusinessWallet { get; set; }


    }
}