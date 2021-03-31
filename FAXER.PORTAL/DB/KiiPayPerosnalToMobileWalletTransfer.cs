
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class KiiPayPerosnalToMobileWalletTransfer
    {
        public int Id { get; set; }
        public int KiiPayPersonalWalletInformationId { get; set; }
        public string ReceiverMobileNumber { get; set; }
        public string ReceiverCountry { get; set; }
        public decimal SendingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal Fee { get; set; }
        public string PaymentReference { get; set; }
        public PaymentType PaymentType { get; set; }
        public DateTime TransactionDate { get; set; }
        public virtual KiiPayPersonalWalletInformation KiiPayPersonalWalletInformation { get; set; }


    }
}