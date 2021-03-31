using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class TopUpToSupplier
    {
        public int Id { get; set; }
        public int SuplierId { get; set; }

        //FaxerInformationId
        public int PayerId { get; set; }
        public string SupplierAccountNo { get; set; }
        public string PayingCountry{ get; set; }
        public string SupplierCountry{ get; set; }
        public decimal SendingAmount{ get; set; }
        public decimal Fee{ get; set; }
        public decimal ReceivingAmount{ get; set; }
        public decimal TotalAmount{ get; set; }
        public decimal EcxhangeRate{ get; set; }
        public DateTime PaymentDate{ get; set; }
        public Module PaymentModule{ get; set; }
        public SenderPaymentMode SenderPaymentMode { get; set; }

        public PaymentType PaymentType{ get; set; }
        public string WalletNo{ get; set; }
        public decimal AgentCommission { get; set; }
        public string PayingStaffName { get; set; }
        public string ReceiptNo { get; set; }
        public int PayingStaffId { get; set; }

    }
}