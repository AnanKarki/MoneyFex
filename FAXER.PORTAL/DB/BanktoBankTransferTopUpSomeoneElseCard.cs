using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class BanktoBankTransferTopUpSomeoneElseCard
    {
        public int Id { get; set; }

        public int FaxerId { get; set; }

        public int MFTCCardId { get; set; }

        public string MFTCCardNumber { get; set; }

        public decimal TopUpAmount { get; set; }

        public decimal FaxingFee { get; set; }
        public decimal ReceivingAmount { get; set; }

        public decimal TotalAmountImcludingFee { get; set; }

        public decimal ExchangeRate { get; set; }


        public string TopUpReference { get; set; }
        public string PaymentReference { get; set; }

        public DateTime TransactionDate { get; set; }

        public virtual FaxerInformation Faxer { get; set; }
    }
}