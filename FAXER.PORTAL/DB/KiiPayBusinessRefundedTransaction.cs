using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    /// <summary>
    /// KiiPay Business refunded transaction 
    /// </summary>
    public class KiiPayBusinessRefundedTransaction
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public decimal AmountRefunded { get; set; }
        public TypeOfTransaction TypeOfTransaction { get; set; }
        public DateTime RefundedDate { get; set; }



    }
    public enum TypeOfTransaction {

        BusinessNationalPayment ,
        BusinessInternationalPayment,
        KiiPayPersonalPayment
    }
}