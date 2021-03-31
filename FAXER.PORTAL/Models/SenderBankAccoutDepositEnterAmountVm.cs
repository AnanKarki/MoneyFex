using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderBankAccoutDepositEnterAmountVm
    {

        public const string BindProperty = "Id , Image ,ReceiverName,ReceiverId, SendingAmount,SendingCurrencySymbol," +
            "SendingCurrencyCode  ,ReceivingCurrencyCode ,ReceivingCurrencySymbol,SendingCurrency," +
            "ReceivingAmount,ReceivingCurrency,SendSms,SmsCharge,Fee,TotalAmount,ExchangeRate" +
            "SendingCountryCode,ReceivingCountryCode ";

        [Range(0 , int.MaxValue)]
        public int Id { get; set; }

        [StringLength(500)]
        public string Image { get; set; }

        [StringLength(200)]
        public string ReceiverName { get; set; }
        [Range(0 , int.MaxValue)]
        public int ReceiverId { get; set; }
        [Range(0.0, double.MaxValue, ErrorMessage = "Enter amount")]
        public decimal SendingAmount { get; set; }

        [StringLength(10)]
        public string SendingCurrencySymbol { get; set; }

        [StringLength(10)]
        public string SendingCurrencyCode { get; set; }

        [StringLength(10)]
        public string ReceivingCurrencyCode { get; set; }

        [StringLength(10)]
        public string ReceivingCurrencySymbol { get; set; }

        [StringLength(10)]
        public string SendingCurrency { get; set; }

        [Range(0.0, double.MaxValue, ErrorMessage = "Enter amount")]
        public decimal ReceivingAmount { get; set; }

        [StringLength(10)]
        public string ReceivingCurrency { get; set; }
        public bool SendSms { get; set; }

        [Range(0.0, double.MaxValue)]
        public decimal SmsCharge { get; set; }


        [Range(0.0, double.MaxValue)]
        public decimal Fee { get; set; }

        [Range(0.0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Range(0.0, double.MaxValue)]
        public decimal ExchangeRate { get; set; }
        [StringLength(10)]
        public string SendingCountryCode { get; set; }
        [StringLength(10)]
        public string ReceivingCountryCode { get; set; }

      

    }
}