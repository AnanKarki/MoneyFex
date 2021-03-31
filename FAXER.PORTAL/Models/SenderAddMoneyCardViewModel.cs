using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderAddMoneyCardViewModel
    {
        public const string BindProperty = "CardHolderName,SendingCurrency,SendingBalance,SelectCard,CardNumber,ExpiringDateMonth,ExpiringDateYear,SecurityCode" +
         ",Address ,IsSaveCard ";

        public string CardHolderName { get; set; }
        public string SendingCurrency { get; set; }
        [Range(0.0, Double.MaxValue)]

        public decimal SendingBalance { get; set; }
        public CreditDebitCardType SelectCard { get; set; }

        [Required(ErrorMessage = "Enter Card Number")]

        public string CardNumber { get; set; }

        public string ExpiringDateMonth { get; set; }

        public string ExpiringDateYear { get; set; }

        [Required(ErrorMessage = "Enter Security Code")]

        public string SecurityCode { get; set; }
        public string Address { get; set; }

        public bool IsSaveCard { get; set; }
    }
    public class AddressVm
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class BankVM
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}