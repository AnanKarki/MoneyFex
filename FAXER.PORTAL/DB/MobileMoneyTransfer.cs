using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class MobileMoneyTransfer
    {

        public int Id { get; set; }
        public string PaidToMobileNo { get; set; }
        public Module PaidFromModule { get; set; }

        public int SenderId { get; set; }

        public string SendingCountry { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceiverCity { get; set; }
        public string ReceivingCountry { get; set; }
        public string ReceivingCurrency { get; set; }

        #region Transaction Info

        public decimal SendingAmount { get; set; }
        public decimal Fee { get; set; }
        public decimal ReceivingAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ExchangeRate { get; set; }

        public string PaymentReference { get; set; }


        #endregion
        public PaymentType PaymentType { get; set; }
        public DateTime TransactionDate { get; set; }

        public SenderPaymentMode SenderPaymentMode { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiptNo { get; set; }
        public int? PayingStaffId { get; set; }
        public string PayingStaffName { get; set; }

        public decimal AgentCommission { get; set; }

        public int WalletOperatorId { get; set; }
        public int RecipientId { get; set; }
        public MobileMoneyTransferStatus Status { get; set; }
        public Apiservice? Apiservice { get; set; }
        public string TransferZeroSenderId { get; internal set; }

        public decimal ExtraFee { get; set; }

        public bool IsComplianceNeededForTrans { get; set; }

        public bool IsComplianceApproved { get; set; }

        public int ComplianceApprovedBy { get; set; }
        public DateTime? ComplianceApprovedDate { get; set; }
        public string TransferReference { get; set; }

        public decimal Margin { get; set; }
        public decimal MFRate { get; set; }
        public ReasonForTransfer ReasonForTransfer { get; set; }
        public CardProcessorApi? CardProcessorApi { get; set; }
    }

    public class MobileWalletOperator
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }

        public string MobileNetworkCode { get; set; }
        public int PayoutProviderId { get; set; }
    }
    public enum MobileMoneyTransferStatus
    {
        [Description("Failed")]
        Failed,
        [Description("In progress")]
        InProgress,
        [Description("Paid")]
        Paid,
        [Description("Cancelled")]
        Cancel,
        [Description("Payment Pending")]
        PaymentPending,
        [Description("In Progress (ID Check)")]
        IdCheckInProgress,
        [Description("In progress")]
        PendingBankdepositConfirmtaion,
        [Description("In progress ")]
        Abnormal,

        [Description("In progress")]
        Held,
        [Description("Refund")]
        FullRefund,
        [Description("Refund")]
        PartailRefund,
        [Description("In progress")]
        Paused

    }
}