using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class TopUpSomeoneElseCardTransaction
    {

        public int Id { get; set; }

        public int FaxerId { get; set; }

        [ForeignKey("KiiPayPersonalWalletInformation")]
        public int KiiPayPersonalWalletId { get; set; }
        public string TransferToMobileNo { get; set; }


        public decimal RecievingAmount { get; set; }
        public decimal FaxingAmount { get; set; }
        public decimal FaxingFee { get; set; }
        public decimal ExchangeRate { get; set; }

        public decimal TotalAmount { get; set; }

        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }
        public string ReceiptNumber { get; set; }

        public PayedBy PayedBy { get; set; }

        public string PayingStaffName { get; set; }

        public int? PayingStaffId { get; set; }

        public string PaymentMethod { get; set; }
        public string TopUpReference { get; set; }

        public bool IsAutoPaymentTransaction { get; set; }

        public System.DateTime TransactionDate { get; set; }

        public PaymentType PaymentType { get; set; }

        public SenderPaymentMode SenderPaymentMode { get; set; }

        public decimal AgentCommission { get; set; }
        public string StripeTokenId { get; set; }
        public decimal Margin { get; set; }
        public decimal MFRate { get; set; }
        public int RecipientId { get; set; }

        public virtual KiiPayPersonalWalletInformation KiiPayPersonalWalletInformation { get; set; }
       


    }

    public enum PayedBy {

        Sender ,
        Admin,
        Agent
    }
}