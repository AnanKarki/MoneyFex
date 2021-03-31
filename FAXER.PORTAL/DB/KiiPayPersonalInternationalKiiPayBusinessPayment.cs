using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class KiiPayPersonalInternationalKiiPayBusinessPayment
    {

        public int Id { get; set; }

        #region Sender Details (KiiPay Personal User  ) holds  KiiPay Personal Wallet 


        [ForeignKey("KiiPayPersonalWalletInformation")]
        public int PayedFromKiiPayPersonalWalletId { get; set; }
        #endregion


        #region Receiver Details (kiiPay Business User ) holds KiiPay Business Wallet

        public int PayedToKiiPayBusinessInformationId { get; set; }
        [ForeignKey("KiiPayBusinessWalletInformation")]
        public int PayedToKiiPayBusinessWalletId { get; set; }

        #endregion


        #region Transaction Details 
        public decimal ReceivingAmount { get; set; }
        public decimal FaxingAmount { get; set; }
        public decimal FaxingFee { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal TotalAmount { get; set; }
        public string ReceiptNumber { get; set; }

        public string PaymentReference { get; set; }

        public System.DateTime TransactionDate { get; set; }

        #endregion

        public bool IsAutoPayment { get; set; }
        public bool IsRefunded { get; set; }
        public virtual KiiPayPersonalWalletInformation KiiPayPersonalWalletInformation { get; set; }

        public virtual KiiPayBusinessWalletInformation KiiPayBusinessWalletInformation { get; set; }
    }
}