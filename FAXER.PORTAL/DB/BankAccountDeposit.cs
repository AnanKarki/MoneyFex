using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class BankAccountDeposit
    {

        public int Id { get; set; }
        /// <summary>
        /// Added Id In table By Umesh Due to conflit was in update  
        /// </summary>


        public int TransactionId { get; set; }

        public Module PaidFromModule { get; set; }

        public int BankId { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string ReceiverAccountNo { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverCity { get; set; }
        public string ReceiverCountry { get; set; }

        public string ReceiverMobileNo { get; set; }

        public int SenderId { get; set; }
        public int RecipientId { get; set; }

        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }

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

        [Index("IX_ReceiptNo")]
        [StringLength(50)]
        public string ReceiptNo { get; set; }

        public int? PayingStaffId { get; set; }

        public string PayingStaffName { get; set; }

        public decimal AgentCommission { get; set; }

        public bool IsManualDeposit { get; set; }
        public bool IsBusiness { get; set; }

        public BankDepositStatus Status { get; set; }

        public Apiservice? Apiservice { get; set; }
        public string TransferZeroSenderId { get; internal set; }

        public string TransferReference { get; set; }

        /// <summary>
        /// If SenderPaymentMode is MoneyFex Bank Account then 
        /// only this property is applicable
        /// </summary>
        public bool HasMadePaymentToBankAccount { get; set; }

        public decimal ExtraFee { get; set; }

        public bool IsEuropeTransfer { get; set; }

        public ReasonForTransfer ReasonForTransfer { get; set; }
        public bool IsComplianceNeededForTrans { get; set; }
        public bool IsComplianceApproved { get; set; }
        public bool IsManualApproveNeeded { get; set; }
        public bool ManuallyApproved { get; set; }
        public int ComplianceApprovedBy { get; set; }
        public DateTime? ComplianceApprovedDate { get; set; }

        public decimal Margin { get; set; }
        public decimal MFRate { get; set; }

        public bool IsTransactionDuplicated { get; set; }
        public string DuplicateTransactionReceiptNo { get; set; }

        public string TransactionDescription { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }

        public CardProcessorApi? CardProcessorApi { get; set; }


    }

    public class ReinitializeTransaction
    {

        public int Id { get; set; }
        public string ReceiptNo { get; set; }
        public string NewReceiptNo { get; set; }
        public DateTime Date { get; set; }
        public int? CreatedById { get; set; }
        public string CreatedByName { get; set; }


    }
    public enum BankDepositStatus
    {
        [Description("In Progress")]
        Held,
        [Description("In Progress")]
        UnHold,
        [Description("Cancelled")]
        Cancel,
        [Description("Paid")]
        Confirm,
        [Description("In progress")]
        Incomplete,
        Failed,
        [Description("Payment Pending")]
        PaymentPending,
        [Description("In progress (ID Check)")]
        IdCheckInProgress,
        [Description("In progress (MoneyFex Bank Deposit) ")]
        PendingBankdepositConfirmtaion,
        [Description("In progress")]
        ReInitialise,
        Abnormal,
        [Description("Full Refund")]
        FullRefund,
        [Description("Partial Refund")]
        PartailRefund,
        [Description("In Progress")]
        Paused,
        [Description("In Progress")]
        InProgressFC


    }

}