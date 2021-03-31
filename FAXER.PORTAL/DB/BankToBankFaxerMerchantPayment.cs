using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class BankToBankFaxerMerchantPayment
    {
        public int Id { get; set; }

        public int FaxerId { get; set; }

        public int KiiPayBusinessInformationId { get; set; }

        public string BusinessMerchantAccountNo { get; set; }
        public decimal AmountPaid { get; set; }

        public decimal FaxingFee { get; set; }
        public decimal ReceivingAmount { get; set; }

        public decimal TotalAmountImcludingFee { get; set; }

        public decimal ExchangeRate { get; set; }

        public string PaymentRefernce { get; set; }

        public DateTime TransactionDate { get; set; }

        public virtual FaxerInformation Faxer { get; set; }
        public virtual KiiPayBusinessInformation Business { get; set; }
    }
}