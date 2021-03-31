using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class KiiPayBusinessWalletStatement
    {

        public int Id { get; set; }
        public int TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        /// <summary>
        /// Is outbound amount = sending Amount 
        /// Is Inbound amount = receiveing amount 
        /// Fee is always in sending currency
        /// </summary>
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public string SenderCountry { get; set; }
        public string ReceivingCountry { get; set; }
        public decimal CurBal { get; set; }
        public WalletStatmentType WalletStatmentType { get; set; }
        public WalletStatmentStatus WalletStatmentStatus { get; set; }



    }
    public class KiiPayPersonalWalletStatement
    {

        public int Id { get; set; }

        public int WalletId { get; set; }

        public int TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        /// <summary>
        /// Is outbound amount = sending Amount 
        /// Is Inbound amount = receiveing amount 
        /// Fee is always in sending currency
        /// </summary>
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }

        public decimal TotalAmount { get; set; }

        public string SenderCountry { get; set; }
        public string ReceivingCountry { get; set; }

        public decimal CurBal { get; set; }

        public string ReceiverName { get; set; }

        public string AccountNumber { get; set; }

        public string PaymentReference { get; set; }


        public WalletStatmentType WalletStatmentType { get; set; }
        public WalletStatmentStatus WalletStatmentStatus { get; set; }


    }

    public enum WalletStatmentType
    {

        KiiPayPersoanlPayment,
        BusinessNationalPayment,
        BusinessInternationalPayment,
        KiiPayPersonalToBusinessInternationalPayment,
        CreditDebitCard,
        Invoice
    }

    public enum WalletStatmentStatus
    {

        InBound,
        OutBound
    }
}