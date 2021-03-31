using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessAddMoneyToWalletVM
    {
    }

    public class AddMoneyToWalletEnterAmountVm
    {
        public const string BindProperty = "Amount ,CurrencyCode , CurrencySymbol ";
        [Range(1, int.MaxValue, ErrorMessage = "Please enter the amount greater than {0}")]
        public decimal Amount{ get; set; }
        public string CurrencyCode{ get; set; }
        public string CurrencySymbol{ get; set; }
    }


    public class AddMoneyToWalletEnterCardInfoVm
    {
        public const string BindProperty = "CardId ,CardType , CardNumber ,ExpiringDateMonth ,ExpiringDateYear , SecurityCode, SaveCard ,Address";

        public int CardId{ get; set; }
        [Required(ErrorMessage = "Please enter the Card Number.")]
        public CreditDebitCardType CardType { get; set; }
        
        [Required(ErrorMessage = "Please enter the Card Number.")]
        public string CardNumber { get; set; }
        [Required(ErrorMessage = "Please enter the Expiring Month.")]
        public string ExpiringDateMonth { get; set; }
        [Required(ErrorMessage = "Please enter the Expiring Year.")]
        public string ExpiringDateYear { get; set; }
        [Required(ErrorMessage = "Please enter the SecurityCode.")]
        public string SecurityCode { get; set; }
        public bool SaveCard{ get; set; }
        public string Address { get; set; }

    }
    public enum CardType {
        VisaCard,
        MasterCard,
        AmericanExpress,
        Maestro
    }


}