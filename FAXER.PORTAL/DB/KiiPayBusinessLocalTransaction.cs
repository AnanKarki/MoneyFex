using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class KiiPayBusinessLocalTransaction
    {

        public int Id { get; set; }
        
        public int PayedFromKiiPayBusinessInformationId { get; set; }
        
        [ForeignKey("PayedFromKiiPayBusinessWalletInformation")]
        public int? PayedFromKiiPayBusinessWalletInformationId { get; set; }    

        [ForeignKey("PayedToKiiPayBusinessWalletInformation")]
        public int PayedToKiiPayBusinessWalletInformationId { get; set; }

        public int PayedToKiiPayBusinessInformationId { get; set; }

        public DateTime TransactionDate { get; set; }
        public string PaymentReference { get; set; }
        public decimal AmountSent { get; set; }

        public bool IsAutoPayment { get; set; }

        public bool IsRefunded { get; set; }

        public virtual KiiPayBusinessWalletInformation PayedToKiiPayBusinessWalletInformation { get; set; }

        public virtual KiiPayBusinessWalletInformation PayedFromKiiPayBusinessWalletInformation { get; set; }



    }
}