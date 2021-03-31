using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class KiiPayBusinessInternationalTransactionWithSuppliers
    {


        public int Id { get; set; }

        /// <summary>
        /// Sender 
        /// </summary>
        public int BusinessId { get; set; }

        /// <summary>
        /// Receiver
        /// </summary>
        public int SuppliersId { get; set; }
        public decimal RecievingAmount { get; set; }
        public decimal FaxingAmount { get; set; }
        public decimal FaxingFee { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal TotalAmount { get; set; }
        public string ReferenceNo { get; set; }
        public bool IsAutoPayment { get; set; }
        public bool IsRefunded { get; set; }
        public System.DateTime TransactionDate { get; set; }

    }
}