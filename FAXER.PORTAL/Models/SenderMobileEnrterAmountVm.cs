using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderMobileEnrterAmountVm
    {

        public const string BindProperty = "Id, ImageUrl , ReceiverName , ReceiverId , SendingCurrencySymbol " +
            ", SendingCurrencyCode ,SendingAmount,ReceivingCurrencySymbol,ReceivingCurrencyCode,ReceivingAmount,Fee," +
            " TotalAmount,ExchangeRate,PaymentReference , SendSms ,SmsCharge , SendingCountryCode, ReceivingCountryCode  ";


        [Range(0, int.MaxValue)]
        public int Id { get; set; }

        [MaxLength(200)]
        public string ImageUrl { get; set; }


        [MaxLength(200)]
        public string ReceiverName { get; set; }

        [Range(0, int.MaxValue)]
        public int ReceiverId { get; set; }

        [Required]
        public string SendingCurrencySymbol { get; set; }

        [Required]
        public string SendingCurrencyCode { get; set; }

        [Range(1.0, double.MaxValue, ErrorMessage = "Enter amount")]
        public decimal SendingAmount { get; set; }

        [MaxLength(200)]
        public string ReceivingCurrencySymbol { get; set; }


        [MaxLength(200)]
        public string ReceivingCurrencyCode { get; set; }
        [Range(1.0, double.MaxValue, ErrorMessage = "Enter amount")]
        public decimal ReceivingAmount { get; set; }

        [Range(0.0, double.MaxValue)]
        public decimal Fee { get; set; }

        [Range(0.0, double.MaxValue)]
        public decimal TotalAmount { get; set; }


        [Range(0.0, double.MaxValue)]
        public decimal ExchangeRate { get; set; }



        [MaxLength(200)]
        public string PaymentReference { get; set; }

        public bool SendSms { get; set; }



        [Range(0.0, double.MaxValue)]
        public decimal SmsCharge { get; set; }
        [MaxLength(200)]
        public string SendingCountryCode { get; set; }
        [MaxLength(200)]
        public string ReceivingCountryCode { get; set; }



    }
}