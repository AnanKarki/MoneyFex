using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class KiiPayPersonalWalletPaymentByKiiPayPersonal
    {

        public int Id { get; set; }
        public int SenderId { get; set; }
        public int SenderWalletId { get; set; }

        [ForeignKey("KiiPayPersonalWalletInformation")]
        public int ReceiverWalletId { get; set; }

        public string ReceivingMobileNumber { get; set; }
        public decimal RecievingAmount { get; set; }
        public decimal FaxingAmount { get; set; }
        public decimal FaxingFee { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal TotalAmount { get; set; }

        public string PaymentReference { get; set; }
        public PaymentType PaymentType { get; set; }

        public string ReceiptNumber { get; set; }


        public System.DateTime TransactionDate { get; set; }

        public bool IsRefunded { get; set; }
        public int? KiiPayPersonalRefundedTransactionId { get; set; }
        public bool IsAutoPayment { get; set; }

        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }


        public TransactionFrom TransactionFromPortal { get; set; }
    
        public virtual KiiPayPersonalWalletInformation KiiPayPersonalWalletInformation { get; set; }

    }


    public enum TransactionFrom {

        KiiPayPortal,
        SenderPortal,
        AdminPortal
        
    }

}