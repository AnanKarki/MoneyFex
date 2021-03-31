using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class VirtualAccountDepositByAgent
    {
        public int Id { get; set; }
        public int FaxerId { get; set; }
        public int CardUserId { get; set; }
        public int AgentId { get; set; }
        public int IDCardType { get; set; }
        public string IDCardNumber { get; set; }
        public DateTime IDCardExpiryDate { get; set; }
        public string IDCardIssuingCountry { get; set; }
        public decimal DepositAmount { get; set; }
        public decimal DepositFees { get; set; }
        public decimal ExchangeRate { get; set; }

        public decimal ReceivingAmount { get; set; }
        public string TotalAmount { get; set; }
        public string PaymentReference { get; set; }
        public string PayingStaffName { get; set; }
        public DateTime TransactionDate { get; set; }
        public string ReceiptNumber { get; set; }
        public int PayingAgentStaffId { get; set; }
        public virtual FaxerInformation Faxer { get; set; }
        
        public virtual AgentInformation Agent { get; set; }
    }
}