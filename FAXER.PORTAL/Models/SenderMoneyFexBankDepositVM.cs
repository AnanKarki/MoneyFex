using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderMoneyFexBankDepositVM
    {


        public const string BindProperty = "Id , SendingCurrencySymbol , SendingCurrencyCode  , AvailableBalance," +
            "Amount, AccountNumber,  ShortCode,LabelName, PaymentReference , HasMadePaymentToBankAccount,BankFee";

        public SenderMoneyFexBankDepositVM()
        {
            if(Common.FaxerSession.LoggedUser!=null)
            SetBankFee(Common.FaxerSession.LoggedUser.CountryCode);

        }
        public SenderMoneyFexBankDepositVM(string CountryCode)
        {
            SetBankFee(CountryCode);
        }
        [Range(0, int.MaxValue)]
        public int Id { get; set; }
        [MaxLength(200)]
        public string SendingCurrencySymbol { get; set; }
        [MaxLength(200)]
        public string SendingCurrencyCode { get; set; }
        [Range(0.0, double.MaxValue)]
        public decimal AvailableBalance { get; set; }
        [Range(0.0, double.MaxValue)]
        public decimal Amount { get; set; }
        [MaxLength(200)]
        public string AccountNumber { get; set; }
        [MaxLength(200)]
        public string ShortCode { get; set; }
        public string LabelName { get; set; }
        [MaxLength(200)]
        public string PaymentReference { get; set; }
        public bool HasMadePaymentToBankAccount { get; set; }
        public decimal BankFee { get; set; }

        public void SetBankFee(string countryCode)
        {

            // Need be changes used as static for right now

            var CustomerPaymentFee = Common.Common.CustomerPaymentFee(countryCode);

            if (CustomerPaymentFee != null)
            {
                //this.BankFee = 0.79M;
                this.BankFee = CustomerPaymentFee.BankTransfer;
            }
            else
            {
                this.BankFee = 0.79M;
            }
        }

        public int TransactionSummaryId { get; set; }


    }
}