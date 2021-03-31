using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class KiiPayBusinessWalletWithdrawlFromAgent
    {
        public int Id { get; set; }
        [ForeignKey("KiiPayBusinessWalletInformation")]
        public int KiiPayBusinessWalletInformationId { get; set; }
        public decimal TransactionAmount { get; set; }
        public int TransactionType { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public int AgentInformationId { get; set; }
        public int IdentificationTypeId { get; set; }
        [Required]
        public string IdNumber { get; set; }
        public DateTime IdCardExpiringDate { get; set; }
        [Required]
        public string IssuingCountryCode { get; set; }
        public string ReceiptNumber { get; set; }
        public string PayingAgentName { get; set; }

        public int PayingAgentStaffId { get; set; }

        public decimal AgentCommission { get; set; }

        public virtual AgentInformation AgentInformation { get; set; }
        public virtual KiiPayBusinessWalletInformation KiiPayBusinessWalletInformation { get; set; }
    }
}