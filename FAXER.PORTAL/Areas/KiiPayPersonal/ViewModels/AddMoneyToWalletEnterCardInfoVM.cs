using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class AddMoneyToWalletEnterCardInfoVM
    {
        public const string BindProperty = "CardId ,CardType , CardNumber " +
            ",ExpiringDateMonth ,ExpiringDateYear, SecurityCode , SaveCard , Address , DepositingAmount";
        public int CardId { get; set; }
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
        public bool SaveCard { get; set; }
        public string Address { get; set; }
   
        public decimal DepositingAmount { get; set; }
    }
}