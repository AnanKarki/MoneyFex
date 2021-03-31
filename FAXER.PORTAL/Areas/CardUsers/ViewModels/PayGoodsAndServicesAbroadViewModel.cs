using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.ViewModels
{
    public class PayGoodsAndServicesAbroadViewModel
    {
        public const string BindProperty = " CardId,ReceiverMFBCCardId , ReceiverMFBCCardNumber , ReceiverBusinessCardUserName , ReceiverCardUserAccountNo ,PaymentReference " +
            " ,PayingAmount ,GoodsPurchaseLimit ,GoodsPurchaseLimitAmount , AmountOnCard, IsConfirmed, InvalidCard, InvalidCountry,Currency , CurrencySymbol";
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

        public DB.AutoPaymentFrequency GoodsPurchaseLimit { get; set; }

        public decimal GoodsPurchaseLimitAmount { get; set; }

        public decimal AmountOnCard { get; set; }

        public bool IsConfirmed { get; set; }

        public bool InvalidCard { get; set; }
        public bool InvalidCountry { get; set; }
        public string Currency { get; set; }
        public string CurrencySymbol { get; set; }
    }
}