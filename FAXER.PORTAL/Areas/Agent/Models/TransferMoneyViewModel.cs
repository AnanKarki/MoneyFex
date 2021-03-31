using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class TransferMoneyViewModel
    {
        public const string BindProperty = "Id , SendersAccountNo ,SenderFirstName,SenderMiddleName , SenderLastName ,SenderDOB,SenderGender , SenderAddress ," +
            "SenderCity ,SenderCountry  , SenderCountryPhoneCode ,SenderTelephone,SenderEmail , SenderAccountStatus ,SenderDate, SenderTime ,SenderIDType ,SenderIDNumber ,SenderIDExpiringDate ,SenderIDIssuingCountry" +
               " ,AccountDetailsVirtualAccountNo ,AccountDetailsName ,AccountDetailsNumber , AccountDetailsBalance, AccountDetailsWithdrawalLimit, AccountDetailsLimitType" +
            " , AccountUserDetailsFirstName ,AccountUserDetailsMiddleName , AccountUserDetailsLastName ,AccountUserDetailsAddress ,AccountUserDetailsCity , AccountUserDetailsCountry ,AccountUserDetailsTelephone" +
            ",AccountUserDetailsPhoto ,DepositAmount ,DepositFees,TotalAmountIncludingFee ,CurrentExchangeRate ,ReceivingAmount ,PaymentReference , NameOfAgency, AgentAccountNo ,NameOfPayingStaff ,VerificationConfirm" +
               " ,SenderFaxerId ,ReceiverMFTCId , SenderCurrency , SenderCurrencySymbol , ReceiverCurrency, ReceiverCurrenySymbol , ReceiptNumber ";

        public int Id { get; set; }
        #region sender's Details

        public string SendersAccountNo { get; set; }
        public string SenderFirstName { get; set; }
        public string SenderMiddleName { get; set; }
        public string SenderLastName { get; set; }
        public string SenderDOB { get; set; }
        public string SenderGender { get; set; }
        public string SenderAddress { get; set; }
        public string SenderCity { get; set; }
        public string SenderCountry { get; set; }
        public string SenderCountryPhoneCode { get; set; }
        public string SenderTelephone { get; set; }
        public string SenderEmail { get; set; }
        public string SenderAccountStatus { get; set; }
        public string SenderDate { get; set; }
        public string SenderTime { get; set; }
        #endregion
        #region Sender's Identification
        public int SenderIDType { get; set; }
        public string SenderIDNumber { get; set; }
        public DateTime SenderIDExpiringDate { get; set; }
        public string SenderIDIssuingCountry { get; set; }
        #endregion
        #region Account Details
        public string AccountDetailsVirtualAccountNo { get; set; }
        public string AccountDetailsName { get; set; }
        public string AccountDetailsNumber { get; set; }
        public string AccountDetailsBalance { get; set; }
        public decimal AccountDetailsWithdrawalLimit { get; set; }
        public string AccountDetailsLimitType { get; set; }
        #endregion
        #region Account User Details
        public string AccountUserDetailsFirstName { get; set; }
        public string AccountUserDetailsMiddleName { get; set; }
        public string AccountUserDetailsLastName { get; set; }
        public string AccountUserDetailsAddress { get; set; }
        public string AccountUserDetailsCity { get; set; }
        public string AccountUserDetailsCountry { get; set; }
        public string AccountUserDetailsTelephone { get; set; }
        public string AccountUserDetailsPhoto { get; set; }
        #endregion
        #region Deposit Details
        public decimal DepositAmount { get; set; }
        public decimal DepositFees { get; set; }
        public decimal TotalAmountIncludingFee { get; set; }
        public decimal CurrentExchangeRate { get; set; }
        public decimal ReceivingAmount { get; set; }
        public string PaymentReference { get; set; }
        #endregion
        #region Official Use Only
        public string NameOfAgency { get; set; }
        public string AgentAccountNo { get; set; }
        public string NameOfPayingStaff { get; set; }
        #endregion

        #region Information Verification
        public bool VerificationConfirm { get; set; }
        #endregion

        public int SenderFaxerId { get; set; }
        public int ReceiverMFTCId { get; set; }
        public string SenderCurrency { get; set; }
        public string SenderCurrencySymbol { get; set; }
        public string ReceiverCurrency { get; set; }
        public string ReceiverCurrenySymbol { get; set; }
        public string ReceiptNumber { get; set; }
    }

    public class VirtualAccountDetailsVm
    {
        public int ReceiverMFTCId { get; set; }
        public string AccountDetailsVirtualAccountNo { get; set; }
        public string AccountDetailsName { get; set; }
        public string AccountDetailsNumber { get; set; }
        public string AccountDetailsBalance { get; set; }
        public decimal AccountDetailsWithdrawalLimit { get; set; }
        public string AccountDetailsLimitType { get; set; }
        public string AccountUserDetailsFirstName { get; set; }
        public string AccountUserDetailsMiddleName { get; set; }
        public string AccountUserDetailsLastName { get; set; }
        public string AccountUserDetailsAddress { get; set; }
        public string AccountUserDetailsCity { get; set; }
        public string AccountUserDetailsCountry { get; set; }
        public string AccountUserDetailsTelephone { get; set; }
        public string ReceiverCurrency { get; set; }
        public string ReceiverCurrenySymbol { get; set; }
        public string SenderAccountStatus { get; set; }
        public string AccountUserDetailsPhoto { get; set; }
    }
}