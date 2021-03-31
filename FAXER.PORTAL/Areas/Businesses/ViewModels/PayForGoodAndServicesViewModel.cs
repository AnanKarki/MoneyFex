using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class PayForGoodAndServicesViewModel
    {
        public const string BindProperty = "CardId ,ReceiverMFBCCardId , ReceiverMFBCCardNumber ,  ReceiverBusinessCardUserName , ReceiverCardUserAccountNo  ,  PaymentReference" +
            ", PayingAmount ,  AmountOnCard , IsConfirmed , InvalidCard , InvalidCountry ,Currency  , CurrencySymbol";
        public int CardId { get; set; }

        public int ReceiverMFBCCardId { get; set; }
        [Required]
        public string ReceiverMFBCCardNumber { get; set; }

        [Required]
        public string ReceiverBusinessCardUserName { get; set; }

        [Required]
        public string ReceiverCardUserAccountNo { get; set; }

        [Required]
        public string PaymentReference { get; set; }
        public decimal PayingAmount { get; set; }

        public decimal AmountOnCard { get; set; }

        public bool IsConfirmed { get; set; }

        public bool InvalidCard { get; set; }
        public bool InvalidCountry { get; set; }
        public string Currency { get; set; }
        public string CurrencySymbol { get; set; }

    }
}