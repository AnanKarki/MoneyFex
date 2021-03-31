using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class SessionTransactionSummary
    {
        public int Id { get; set; }
        public string SessionId { get; set; }
        public string ReceiptNo { get; set; }
        public TransferType TransferType { get; set; }
        public bool IsLocalPayment { get; set; }
        public bool IsIdCheckInProgress { get; set; }
        public CardProcessorApi CardProcessorApi { get; set; }

    }


    public class SessionSenderAndReceiverDetial
    {
        public int Id { get; set; }

        public int TransactionSummaryId { get; set; }
        public int WalletOperatorId { get; set; }

        public int SenderId { get; set; }

        public string SenderCountry { get; set; }

        /// <summary>
        /// Receiver Id Can be Kiipay Wallet Id OR Business Wallet Id 
        /// </summary>

        public int ReceiverId { get; set; }

        public string ReceiverCountry { get; set; }

        public string ReceiverMobileNo { get; set; }
        public int SenderWalletId { get; set; }
        public string SenderPhoneNo { get; set; }

        public virtual SessionTransactionSummary TransactionSummary { get; set; }
    }

    public class SessionKiiPayTransferPaymentSummary
    {
        public int Id { get; set; }
        public int TransactionSummaryId { get; set; }
        public string ReceiverName { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }

        public string SendingCurrencySymbol { get; set; }

        public string ReceivingCurrencySymbol { get; set; }


        public decimal SendingAmount { get; set; }
        public string ReceiverCity { get; set; }

        public decimal ReceivingAmount { get; set; }


        public decimal Fee { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal ExchangeRate { get; set; }
        public string PaymentReference { get; set; }
        public bool SendSMS { get; set; }

        public decimal SMSFee { get; set; }

        public virtual SessionTransactionSummary TransactionSummary { get; set; }

    }


    public class SessionPaymentMethod
    {

        public int Id { get; set; }
        public int TransactionSummaryId { get; set; }
        public string PaymentMethod { get; set; }
        public decimal TotalAmount { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public SenderPaymentMode SenderPaymentMode { get; set; }
        public bool EnableAutoPayment { get; set; }
        public AutoPaymentFrequency AutopaymentFrequency { get; set; }
        public decimal AutoPaymentAmount { get; set; }
        public string PaymentDay { get; set; }
        public decimal KiipayWalletBalance { get; set; }
        public bool HasKiiPayWallet { get; set; }
        public bool HasEnableMoneyFexBankAccount { get; set; }

        public virtual SessionTransactionSummary TransactionSummary { get; set; }
    }


    public class SessionCreditDebitCardViewModel
    {
        public int Id { get; set; }
        public int TransactionSummaryId { get; set; }
        public decimal FaxingAmount { get; set; }
        public string NameOnCard { get; set; }
        public string ReceiverName { get; set; }
        public string CardNumber { get; set; }
        public string EndMM { get; set; }
        public string EndYY { get; set; }
        public string SecurityCode { get; set; }
        public string AddressLineOne { get; set; }
        public string AddressLineTwo { get; set; }
        public string CityName { get; set; }
        public string ZipCode { get; set; }
        public bool SaveCard { get; set; }
        public bool AutoTopUp { get; set; }
        public decimal AutoTopUpAmount { get; set; }
        public DB.AutoPaymentFrequency PaymentFrequency { get; set; }
        public string CountyName { get; set; }
        public string FaxingCurrency { get; set; }
        public string FaxingCurrencySymbol { get; set; }
        public string PaymentDay { get; set; }
        public bool Confirm { get; set; }
        public string StripeTokenID { get; set; }
        public CreditDebitCardType CreditDebitCardType { get; set; }
        public string UserImage { get; set; }
        public string ExpiryDate { get; set; }
        public bool ThreeDEnrolled { get; set; }
        public decimal CreditDebitCardFee { get; set; }
        public string CardUsageMsg { get; set; }
        public bool IsCardUsageMsg { get; set; }
        public string ErrorMsg { get; set; }

        public virtual SessionTransactionSummary TransactionSummary { get; set; }
    }

    public class SessionSenderMoneyFexBankDeposit
    {
        public int Id { get; set; }
        public int TransactionSummaryId { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public string SendingCurrencyCode { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal Amount { get; set; }
        public string AccountNumber { get; set; }
        public string ShortCode { get; set; }
        public string LabelName { get; set; }
        public string PaymentReference { get; set; }
        public bool HasMadePaymentToBankAccount { get; set; }
        public decimal BankFee { get; set; }

        public virtual SessionTransactionSummary TransactionSummary { get; set; }

    }


    public class SessionSenderCashPickUp
    {
        public int Id { get; set; }
        public int TransactionSummaryId { get; set; }
        public int? RecentReceiverId { get; set; }
        public string FullName { get; set; }
        public string CountryCode { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public ReasonForTransfer Reason { get; set; }
        public int IdenityCardId { get; set; }
        public string IdentityCardNumber { get; set; }

        public virtual SessionTransactionSummary TransactionSummary { get; set; }
    }

    public class SessionCashPickUpReceiverDetailsInformation
    {
        public int Id { get; set; }

        public int TransactionSummaryId { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string PreviousReceiver { get; set; }
        public string MobileNo { get; set; }
        public string ReceiverFullName { get; set; }
        public string MobileCode { get; set; }
        public string Email { get; set; }
        public ReasonForTransfer ReasonForTransfer { get; set; }
        public bool Searched { get; set; }

        public virtual SessionTransactionSummary TransactionSummary { get; set; }
    }


    public class SessionSenderMobileMoneyTransfer
    {
        public int Id { get; set; }

        public int TransactionSummaryId { get; set; }
        public string CountryCode { get; set; }
        public string CountryPhoneCode { get; set; }
        public int WalletId { get; set; }
        public string MobileNumber { get; set; }
        public string RecentlyPaidMobile { get; set; }
        public string ReceiverName { get; set; }

        public virtual SessionTransactionSummary TransactionSummary { get; set; }
    }

    public class SessionReceiverDetailsInformation
    {
        public int Id { get; set; }

        public int TransactionSummaryId { get; set; }
        public string Country { get; set; }
        public int MobileWalletProvider { get; set; }
        public string PreviousMobileNumber { get; set; }
        public string MobileNumber { get; set; }
        public string ReceiverName { get; set; }
        public ReasonForTransfer ReasonForTransfer { get; set; }
        public string MobileCode { get; set; }

        public virtual SessionTransactionSummary TransactionSummary { get; set; }
    }


    public class SessionSenderBankAccountDeposit
    {

        public int Id { get; set; }

        public int TransactionSummaryId { get; set; }
        public int walletId { get; set; }
        public int ReceipientId { get; set; }
        public string CountryCode { get; set; }
        public string RecentAccountNumber { get; set; }

        public string AccountOwnerName { get; set; }

        public string CountryPhoneCode { get; set; }

        public string MobileNumber { get; set; }
        public string AccountNumber { get; set; }
        public int BankId { get; set; }
        public int? BranchId { get; set; }
        public string BranchCode { get; set; }
        public bool IsManualDeposit { get; set; }
        public bool IsBusiness { get; set; }
        public ReasonForTransfer ReasonForTransfer { get; set; }
        public bool IsEuropeTransfer { get; set; }
        public string BankName { get; set; }


        public bool IsSouthAfricaTransfer { get; set; }
        public bool IsWestAfricaTransfer { get; set; }
        public string ReceiverStreet { get; set; }
        public string ReceiverPostalCode { get; set; }
        public string ReceiverEmail { get; set; }
        public string ReceiverCity { get; set; }

        public virtual SessionTransactionSummary TransactionSummary { get; set; }
    }

}