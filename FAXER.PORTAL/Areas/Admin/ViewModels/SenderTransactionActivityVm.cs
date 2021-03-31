using FAXER.PORTAL.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class SenderTransactionActivityVm
    {
        public int SenderId { get; set; }
        public string SenderPhoneNumber { get; set; }
        public int TransactionId { get; set; }
        public TransactionServiceType TransactionServiceType { get; set; }

        public string TransferMethod { get; set; }
        public string TransferType { get; set; }
        public string Amount { get; set; }
        public string SendingAmount { get; set; }
        public string ReceivingAmount { get; set; }
        public string Fee { get; set; }
        public string identifier { get; set; }
        public string DateTime { get; set; }
        public string TransactionTime { get; set; }
        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string Status { get; set; }

        public int BankId { get; set; }
        public string BankAccount { get; set; }
        public string BranchCode { get; set; }

        public bool IsAbnormalTrans { get; set; }

        public string Reference { get; set; }

        #region Is Re Actionable transaction property 

        public bool IsAwaitForConfirmation { get; set; }
        public bool IsAwaitForApproval { get; set; }
        public bool IsManualApprovalNeeded { get; set; }

        #endregion

        public string TransactionStatusForAdmin { get; set; }

        public string TransactionPerformedBy { get; set; }

        public int? AgentStaffId { get; set; }
        public int NoteCount { get; set; }

        public bool IsDuplicatedTransaction { get; set; }
        public string DuplicatedTransactionReceiptNo { get; set; }

        public bool IsReInitializedTransaction { get; set; }

        public string ReInitializedReceiptNo { get; set; }
        public string ReInitializedDateTime { get; set; }

        public string ReInitializeStaffName { get; set; }
        public bool IsSenderActive { get; set; }

        public int RecipentId { get; set; }
        public string SenderEmail { get; set; }
        public int TotalCount { get; set; }
    }

    public class SenderTransactionActivityWithSenderDetails
    {
        public TransactionStatementNoteViewModel TransactionStatementNote { get; set; }
        public List<SenderTransactionActivityVm> SenderTransactionStatement { get; set; }
        public List<TransactionStatementNoteViewModel> TransactionStatementNoteList { get; set; }
        public string TotalAmountWithCurrency { get; set; }
        public int TotalNumberOfTransaction { get; set; }
        public string TotalFeePaidwithCurrency { get; set; }
    }
}