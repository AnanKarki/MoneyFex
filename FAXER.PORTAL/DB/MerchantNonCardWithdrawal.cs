using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class MerchantNonCardWithdrawal
    {

        [Key]
        public int Id { get; set; }
        [ForeignKey("ReceiversDetails")]
        public int ReceiverId { get; set; }
        [ForeignKey("Agent")]
        public int AgentId { get; set; }
        //[Required, StringLength(8)]
        public string MFCN { get; set; }
        public decimal TransactionAmount { get; set; }

        public decimal ReceivingAmount { get; set; }
        public int TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public int IdentificationTypeId { get; set; }
        [Required, StringLength(50)]
        public string IdNumber { get; set; }
        public DateTime IdCardExpiringDate { get; set; }
        //[Required, StringLength(3)]
        public string IssuingCountryCode { get; set; }
        public string ReceiptNumber { get; set; }
        public string PayingAgentName { get; set; }

        public int PyingAgentStaffId { get; set; }
        public virtual MerchantNonCardReceiverDetails ReceiversDetails { get; set; }
        public virtual AgentInformation Agent { get; set; }
    }
}