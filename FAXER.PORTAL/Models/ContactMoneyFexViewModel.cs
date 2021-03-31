using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class ContactMoneyFexViewModel
    {
        public const string BindProperty = "Subject,PaymentRefernce,ReceiverEmail," +
            "MoneyFexEmail,MFTCCardNumber,BusinessMerchantAccountNumber";
        [MaxLength(200)]
        public string Subject { get; set; }
        [MaxLength(200)]
        public string PaymentRefernce { get; set; }
        [MaxLength(200)]
        public string ReceiverEmail { get; set; }
        [MaxLength(200)]
        public string MoneyFexEmail { get; set; }
        [MaxLength(200)]
        public string MFTCCardNumber { get; set; }
        [MaxLength(200)]
        public string BusinessMerchantAccountNumber { get; set; }
    }
}