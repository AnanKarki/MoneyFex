using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class MerchantNonCardTransaction
    {
        public int Id { get; set; }
        public int KiiPayBusinessInformationId { get; set; }
        public int MFBCCardID { get; set; }

        [ForeignKey("NonCardReciever")]
        public int NonCardRecieverId { get; set; }


        public decimal FaxingAmount { get; set; }
        public decimal FaxingFee { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal ReceivingAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public string MFCN { get; set; }
        //TODO: Add Non Card Receipt Number

        public string ReceiptNumber { get; set; }
        public FaxingStatus FaxingStatus { get; set; }

        public System.DateTime TransactionDate { get; set; }
        public DateTime? StatusChangedDate { get; set; }


        // National And International national doesnot include fees , Exchange rates
        public TopUpType PaymentType { get; set; }
        public virtual MerchantNonCardReceiverDetails NonCardReciever { get; set; }

      
    }


}

public enum TopUpType
{

    International,
    National
}