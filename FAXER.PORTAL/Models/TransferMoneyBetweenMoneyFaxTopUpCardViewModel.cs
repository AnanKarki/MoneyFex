using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class TransferMoneyBetweenMoneyFaxTopUpCardViewModel
    {
        public const string BindProperty = "Id,TransferringTopUpCardRegisteredCountryCode,TransferringTopUpCardRegisteredCountry,TransferringTopUpCardAmount,TopUpCard" +
            ",CountryCurrency,CountryCurrencySymbol,AmountToBeTransferred";
        public int Id { get; set; }
        public string TransferringTopUpCardRegisteredCountryCode { get; set; }
        public string TransferringTopUpCardRegisteredCountry { get; set; }
        public double TransferringTopUpCardAmount { get; set; }
        public string TopUpCard { get; set; }
        public string CountryCurrency { get; set; }
        public string CountryCurrencySymbol { get; set; }
        public decimal AmountToBeTransferred { get; set; }

    }
    public class ReceivingMoneyBetweenMoneyFaxTopUpCardViewModel
    {

        public const string BindProperty = "Id,TopUpCard,ReceivingTopUpCardRegisteredCountry,ReceivingTopUpCardAmount,CountryCurrency" +
            ",CountryCurrencySymbol,Confirm";
        public int Id { get; set; }
        public string TopUpCard { get; set; }
        public string ReceivingTopUpCardRegisteredCountry { get; set; }
        public decimal ReceivingTopUpCardAmount { get; set; }

        public string CountryCurrency { get; set; }
        public string CountryCurrencySymbol { get; set; }
        public bool Confirm { get; set; }
    }
}