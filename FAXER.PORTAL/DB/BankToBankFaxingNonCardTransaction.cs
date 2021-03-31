using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class BankToBankFaxingNonCardTransaction
    {

        public int Id { get; set; }

        public int FaxerId { get; set; }

        public int ReceiverId { get; set; }

        public decimal AmountSent { get; set; }
        public decimal FaxingFee { get; set; }
        public decimal ReceivingAmount { get; set; }

        public decimal TotalAmountImcludingFee { get; set; }

        public decimal ExchangeRate { get; set; }

        public string PaymentReference { get; set; }

        public DateTime TransactionDate { get; set; }

        public virtual FaxerInformation Faxer { get; set; }
    }
}