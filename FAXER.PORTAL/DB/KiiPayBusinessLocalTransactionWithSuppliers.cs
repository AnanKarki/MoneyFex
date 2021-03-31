using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class KiiPayBusinessLocalTransactionWithSuppliers
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

        public DateTime TransactionDate { get; set; }
        public string ReferenceNo { get; set; }
        public decimal AmountSent { get; set; }

        public bool IsAutoPayment { get; set; }

        public bool IsRefunded { get; set; }


    }
}