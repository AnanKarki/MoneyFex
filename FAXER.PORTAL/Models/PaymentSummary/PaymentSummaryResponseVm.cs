using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models.PaymentSummary
{
    public class PaymentSummaryResponseVm
    {
        private string _exchangeRatedesc;
        public decimal SendingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public decimal Fee { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }
        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
        public bool IsIntroductoryRate { get; set; }

        public bool IsIntroductoryFee { get; set; }
        public decimal ActualFee { get; set; }
        public string ExchangeRateText
        {
            get
            {

                return _exchangeRatedesc;
            }
            set
            {

                _exchangeRatedesc = "1 " + SendingCurrency + " = " + ExchangeRate + " " + ReceivingCurrency;

            }
        }
        public ServiceResult<bool> IsValid { get; set; }


    }
}