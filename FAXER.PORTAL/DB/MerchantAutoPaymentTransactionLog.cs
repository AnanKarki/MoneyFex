using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class MerchantAutoPaymentTransactionLog
    {

        public int Id { get; set; }

        public int FaxerId { get; set; }

        public int KiiPayBusinessInformationId { get; set; }

        public decimal PaymentAmount { get; set; }

        public DateTime TransactionDate { get; set; }


    }
}