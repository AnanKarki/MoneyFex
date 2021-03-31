using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class PayBill
    {

        public int Id { get; set; }
        public int PayerId { get; set; }

        public Module Module { get; set; }
        public  string RefCode { get; set; }

        public int SupplierId { get; set; }

        #region Transaction Info

        public  string BillNo { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal SendingAmount { get; set; }
        /// <summary>
        /// Receiving Amount
        /// </summary>
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public decimal Total { get; set; }
        #endregion

        public string PayerCountry { get; set; }
        public string SupplierCountry { get; set; }

        public DateTime PaymentDate { get; set; }

        public PaymentType PaymentType { get; set; }
            
        public string PayingStaffName { get; set; }
        public string ReceiptNo { get; set; }
        public decimal AgentCommission { get; set; }
        public int PayingStaffId { get; set; }
        public SenderPaymentMode SenderPaymentMode { get; set; }

        public virtual Suppliers Supplier { get; set; }
        //public virtual FaxerInformation Payer { get; set; }

    }
}