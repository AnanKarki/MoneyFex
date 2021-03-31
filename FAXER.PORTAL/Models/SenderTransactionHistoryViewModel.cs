using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderTransactionHistoryViewModel
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public Month Month { get; set; }
        public TransactionServiceType FilterKey { get; set; }
        public RefundTransactionViewModel RefundTransactionViewModel { get; set; }

        public List<SenderTransactionHistoryList> TransactionHistoryList { get; set; }
        public SenderTransactionHistoryList TransactionDetails { get; set; }
    }
    public class SenderTransactionHistoryList
    {
        public int Id { get; set; }
        public string ReceiverName { get; set; }
        public string SenderName { get; set; }

        public string SenderTelephoneNo { get; set; }
        public string FaxerAccountNo { get; set; }
        public string SenderEmail { get; set; }
        public string SenderCountryName { get; set; }
        public string ReceivingCountryName { get; set; }
        public string SenderCountryCode { get; set; }
        public string ReceivingCountryCode { get; set; }
        public string FaxerCountry { get; set; }
        public string FaxerAddress { get; set; }
        public string Date { get; set; }
        public string WalletName { get; set; }
        public DateTime TransactionDate { get; set; }

        /// <summary>
        /// If CashPickup is MFCN / If wallet payment Mobile NO
        /// </summary>
        public string AccountNumber { get; set; }
        public string TransactionIdentifier { get; set; }
        int _bankId;
        public int BankId
        {
            get { return _bankId; }
            set
            {
                _bankId = value.ToInt();
            }
        }
        private int _WalletId;

        public int WalletId
        {
            get { return _WalletId; }
            set { _WalletId = value.ToInt(); }
        }


        public int RecipientId { get; set; }
        public string MobileNo { get; set; }
        public string TransactionType { get; set; }
        public string Reference { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal Fee { get; set; }

        public decimal ReceivingAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ExchangeRate { get; set; }

        public FaxingStatus Status { get; set; }
        public int _status { get { return (int)Status; } set { Status = (FaxingStatus)value; } }
        public MobileMoneyTransferStatus statusOfMobileWallet { get; set; }
        public int _statusOfMobileWallet { get { return (int)statusOfMobileWallet; } set { statusOfMobileWallet = (MobileMoneyTransferStatus)value; } }
        public BankDepositStatus StatusOfBankDepoist { get; set; }
        public int _StatusOfBankDepoist { get { return (int)StatusOfBankDepoist; } set { StatusOfBankDepoist = (BankDepositStatus)value; } }
        public MobileMoneyTransferStatus StatusofMobileTransfer { get; set; }
        public int _StatusofMobileTransfer { get { return (int)StatusofMobileTransfer; } set { StatusofMobileTransfer = (MobileMoneyTransferStatus)value; } }

        private int _RecipientIdenityCardId;

        public int RecipientIdenityCardId
        {
            get { return _RecipientIdenityCardId; }
            set { _RecipientIdenityCardId = value.ToInt(); }
        }



        public string RecipientIdentityCardNumber { get; set; }

        public string StatusName { get; set; }
        public string TransferReference { get; set; }
        public string ApiService { get; set; }



        public string SendingCurrency { get; set; }
        public string ReceivingCurrrency { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public string ReceivingCurrencySymbol { get; set; }

        #region Bill Payment Details 

        public string BillNo { get; set; }
        public string BillReferenceNo { get; set; }

        #endregion

        public string PaymentMethod
        {
            get { return GetPaymentMethod(); }
        }

        public string ReceiverCity { get; set; }
        public string ReceiverCountry { get; set; }
        public string ReceiverStreet { get; set; }
        public string ReceiverPostalCode { get; set; }
        public string ReceiverEmail { get; set; }


        public TransactionServiceType TransactionServiceType { get; set; }
        public string BankName { get; set; }
        public bool IsManualBankDeposit { get; set; }
        public bool IsEuropeTransfer { get; set; }

        public bool IsRetryAbleCountry { get; set; }
        public bool IsBusiness { get; set; }
        public string BankCode { get; set; }

        public string PaymentReference { get; set; }

        public bool IsManualApprovalNeeded { get; set; }

        #region for bill payment
        public string BillpaymentCode { get; set; }
        public int senderId { get; set; }
        public bool IsAbnormalTransaction { get; internal set; }

        #endregion



        #region Re actionable transaction property 


        public bool IsAwaitForConfirmation { get; set; }
        public bool IsAwaitForApproval { get; set; }

        #endregion

        public string TransactionStatusForAdmin
        {
            get
            {
                switch (TransactionServiceType)
                {
                    case TransactionServiceType.All:
                        break;
                    case TransactionServiceType.MobileWallet:
                        return GetMobilePaymentStatusName(this.statusOfMobileWallet);
                        break;
                    case TransactionServiceType.KiiPayWallet:
                        break;
                    case TransactionServiceType.BillPayment:
                        break;
                    case TransactionServiceType.ServicePayment:
                        break;
                    case TransactionServiceType.CashPickUp:
                        SSenderTransactionHistory sSenderTransactionHistory = new SSenderTransactionHistory();
                        return sSenderTransactionHistory.GetCashPickUpStatusName(this.Status);
                        break;
                    case TransactionServiceType.BankDeposit:
                        return GetBankDepositStatusName(this.StatusOfBankDepoist);
                        break;
                    default:
                        break;
                }
                return "";
            }
        }


        public SenderPaymentMode SenderPaymentMode { get; set; }

        public string CardNumber { get; set; }
        public string GetPaymentMethod()
        {
            try
            {

                return Common.Common.GetEnumDescription(SenderPaymentMode) + " " + CardNumber.Right(9);
            }
            catch (Exception)
            {

                return Common.Common.GetEnumDescription(SenderPaymentMode);
            }


        }
        public Module PaidFromModule { get; set; }

        public string GetBankDepositStatusName(BankDepositStatus depositStatus)
        {

            string status = "";
            switch (depositStatus)
            {
                case BankDepositStatus.Held:
                    status = "On Hold";
                    break;
                case BankDepositStatus.UnHold:
                    status = "Un Hold";
                    break;
                case BankDepositStatus.Cancel:
                    status = "Cancelled";
                    break;
                case BankDepositStatus.Confirm:
                    status = "Paid";
                    break;
                case BankDepositStatus.Incomplete:
                    status = "In progress";
                    break;
                case BankDepositStatus.Failed:
                    status = "Failed";
                    break;
                case BankDepositStatus.PaymentPending:
                    status = "Payment Pending";
                    break;
                case BankDepositStatus.IdCheckInProgress:
                    status = "In progress (ID Check)";
                    break;
                case BankDepositStatus.PendingBankdepositConfirmtaion:
                    status = "In progress (MoneyFex Bank Deposit)";
                    break;
                case BankDepositStatus.ReInitialise:
                    status = "Abnormal";
                    break;
                case BankDepositStatus.Abnormal:
                    status = "Abnormal";
                    break;
                case BankDepositStatus.FullRefund:
                    status = "Refunded";
                    break;
                case BankDepositStatus.PartailRefund:
                    status = "Partial Refund";
                    break;
                default:
                    break;
            }
            return status;

        }

        public string GetMobilePaymentStatusName(MobileMoneyTransferStatus moneyTransferStatus)
        {


            string status = "";
            switch (moneyTransferStatus)
            {
                case MobileMoneyTransferStatus.Failed:
                    status = "Failed";
                    break;
                case MobileMoneyTransferStatus.InProgress:
                    status = "In Progress";
                    break;
                case MobileMoneyTransferStatus.Paid:
                    status = "Paid";
                    break;
                case MobileMoneyTransferStatus.Cancel:
                    status = "Cancelled";
                    break;
                case MobileMoneyTransferStatus.PaymentPending:
                    status = "Payment Pending";
                    break;
                case MobileMoneyTransferStatus.IdCheckInProgress:
                    status = "In Progress (ID Check)";
                    break;
                case MobileMoneyTransferStatus.PendingBankdepositConfirmtaion:
                    status = "PendingBankdepositConfirmtaion";
                    break;
                case MobileMoneyTransferStatus.Abnormal:
                    status = "Abnormal";
                    break;
                case MobileMoneyTransferStatus.Held:
                    status = "On Hold";
                    break;
                case MobileMoneyTransferStatus.PartailRefund:
                    status = "Refunded";
                    break;
                case MobileMoneyTransferStatus.FullRefund:
                    status = "Partial Refund";
                    break;
                default:
                    break;
            }
            return status;

        }


        private string _transactionPerformedBy;

        public string TransactionPerformedBy
        {
            get
            {
                switch (PaidFromModule)
                {
                    case Module.Faxer:
                        return "Sender";
                        break;
                    case Module.CardUser:

                        break;
                    case Module.BusinessMerchant:
                        break;
                    case Module.Agent:

                        return "Agent";
                        break;
                    case Module.Staff:

                        return "Admin Staff";
                        break;
                    case Module.KiiPayBusiness:
                        break;
                    case Module.KiiPayPersonal:
                        break;
                    default:
                        break;
                }
                return "";
            }
        }
        public int? AgentStaffId { get; set; }

        public bool IsDuplicatedTransaction { get; set; }
        public string DuplicatedTransactionReceiptNo { get; set; }
        public bool IsReInitializedTransaction { get; set; }

        public string ReInitializedReceiptNo { get; set; }
        public string ReInitializeStaffName { get; set; }
        public string ReInitializedDateTime { get; set; }
        public int TotalCount { get; set; }
        public bool HasFee { get; set; }
    }


    public enum TransactionHistoryStatus
    {

        [Display(Name = "Received", Description = "Received")]
        Received,
        [Display(Name = "Received", Description = "Received")]
        Completed,
        [Display(Name = "Not Received", Description = "Not Received")]
        NotRecived,
        [Display(Name = "Cancelled", Description = "Cancelled")]
        Cancelled,
        [Display(Name = "Held", Description = "Held")]
        OnHold

    }
    public enum TransactionServiceType
    {

        All,
        [Display(Name = "Mobile Wallet", Description = "MobileWallet")]
        [Description("Mobile Wallet")]
        MobileWallet,
        [Display(Name = "KiiPay Wallet", Description = "KiiPayWallet")]
        [Description("KiiPay Wallet")]
        KiiPayWallet,
        [Display(Name = "Bill Payment", Description = "BillWallet")]
        [Description("Bill Payment")]
        BillPayment,
        [Display(Name = "Service Payment", Description = "ServicePayment")]
        [Description("Service Payment")]
        ServicePayment,
        [Display(Name = "Cash Pickup", Description = "CashPickup")]
        [Description("Cash Pickup")]
        CashPickUp,
        [Display(Name = "Bank Deposit", Description = "BankDeposit")]
        [Description("Bank Deposit")]
        BankDeposit
    }

}