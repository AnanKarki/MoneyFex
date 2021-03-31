using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class KiiPayPersonalWalletPaymentByKiiPayBusiness
    {
        
        public int Id { get; set; }

        #region Sender Details  (KiiPay Business wallet Info) 
        public int KiiPayBusinessInformationId { get; set; }
        public int KiiPayBusinessWalletInformationId { get; set; }
        #endregion



        #region Receiver Details (KiiPay Personal Wallet Info)
        [ForeignKey("KiiPayPersonalWalletInformation")]
        public int KiiPayPersonalWalletInformationId { get; set; }

        #endregion


        #region Transaction Details 
        public decimal RecievingAmount { get; set; }
        public decimal PayingAmount { get; set; }
        public decimal Fee { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal TotalAmount { get; set; }
        public string ReceiptNumber { get; set; }
        public string PaymentReference { get; set; }
        public PaymentType PaymentType { get; set; }
        public bool IsRefunded { get; set; }
        public int? KiiPayPersonalRefundedTransactionId { get; set; }

        public System.DateTime TransactionDate { get; set; }

        /// <summary>
        /// Transaction is Auto Top up 
        /// </summary>
        public bool IsAutoPayment { get; set; }

        #endregion  


        public virtual KiiPayPersonalWalletInformation KiiPayPersonalWalletInformation { get; set; }
        public virtual KiiPayBusinessWalletInformation KiiPayBusinessWalletInformation { get; set; }
    }

    public enum PaymentType {

        International,
        Local
    }
}