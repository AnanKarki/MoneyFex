using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class KiiPayPersonalNationalKiiPayBusinessPayment
    {
        public int Id { get; set; }

        #region Sender Details  (KiiPay Personal User) holds KiiPay Personal Wallet
        [ForeignKey("KiiPayPersonalWalletInformation")]
        public int KiiPayPersonalWalletInformationId { get; set; }

        #endregion



        #region Receiver Details  ( KiiPay Business User  ) holds  KiiPay Business Wallet

        [ForeignKey("KiiPayBusinessWalletInformation")]
        public int KiiPayBusinessWalletInformationId { get; set; }

        #endregion

        #region Transaction Details 

        public decimal AmountSent { get; set; }
        public string PaymentReference { get; set; }
        public DateTime TransactionDate { get; set; }

        #endregion

        public bool IsAutoPayment { get; set; }
        public bool IsRefunded { get; set; }
        public virtual KiiPayBusinessWalletInformation KiiPayBusinessWalletInformation { get; set; }
        public virtual KiiPayPersonalWalletInformation KiiPayPersonalWalletInformation { get; set; }
        
    }
}