using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessNationalMobileMoneySendingVM
    {
        public const string BindProperty = "Id ,SendingAmount ,Currency ,AvailableBalanceDollar , AvailableBalanceCent ,SendSms , Sender ,SenderImageUrl";

        public int Id { get; set; }

        [Required]
        public decimal SendingAmount { get; set; }
        public string Currency { get; set; }
        public decimal AvailableBalanceDollar { get; set; }
        public decimal AvailableBalanceCent { get; set; }
        public bool SendSms { get; set; }
        public string Sender { get; set; }
        public string SenderImageUrl { get; set; }
    }
}