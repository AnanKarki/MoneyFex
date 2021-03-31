using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AgentBusinessTransferMoneyViewModel
    {
        public const string BindProperty = "Id , SendersAccountNo ,SenderFirstName,SenderMiddleName , SenderLastName ,SenderDOB,SenderGender , SenderAddress ," +
         "SenderCity ,SenderCountry  , SenderCountryPhoneCode ,SenderTelephone,SenderEmail , SenderDate, SenderTime ,SenderIDType ,SenderIDNumber ,SenderIDExpiringDate ,SenderIDIssuingCountry" +
            " ,BusinessAccountNo ,BusinessName ,BusinessLicenseNumber , BusinessAddress, BusinessCity, BusinessCountry , BusinessCountryPhoneCode ,BusinessTelephoneNumber , BusinessAccountStatus" +
            " ,DepositAmount ,DepositFees,TotalAmountIncludingFee ,CurrentExchangeRate ,ReceivingAmount ,PaymentReference , NameOfAgency, AgentAccountNo ,NameOfPayingStaff ,VerificationConfirm" +
            " ,SenderFaxerId ,ReceiverKiiPayBusinessInformationId , SenderCurrency , SenderCurrencySymbol , ReceiverCurrency, ReceiverCurrenySymbol , ReceiptNumber ";

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

        public string SenderDate { get; set; }
        public string SenderTime { get; set; }
        #endregion
        #region Sender's Identification
        public int SenderIDType { get; set; }
        public string SenderIDNumber { get; set; }
        public DateTime SenderIDExpiringDate { get; set; }
        public string SenderIDIssuingCountry { get; set; }
        #endregion
        #region Business Account Details
        public string BusinessAccountNo { get; set; }
        public string BusinessName { get; set; }
        public string BusinessLicenseNumber { get; set; }
        public string BusinessAddress { get; set; }
        public string BusinessCity { get; set; }
        public string BusinessCountry { get; set; }
        public string BusinessCountryPhoneCode { get; set; }
        public string BusinessTelephoneNumber { get; set; }
        public string BusinessAccountStatus { get; set; }
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
        public int ReceiverKiiPayBusinessInformationId { get; set; }
        public string SenderCurrency { get; set; }
        public string SenderCurrencySymbol { get; set; }
        public string ReceiverCurrency { get; set; }
        public string ReceiverCurrenySymbol { get; set; }
        public string ReceiptNumber { get; set; }
    }
}