using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class KiiPayPersonalRefundedTransaction
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public decimal AmountRefunded { get; set; }
        public TypeOfKiiPayPersonalTransaction TypeOfTransaction { get; set; }
        public DateTime RefundedDate { get; set; }
    }

    public enum TypeOfKiiPayPersonalTransaction
    {

        BusinessToPersonalPayment,
        KiiPayPersonalPayment
    }
}