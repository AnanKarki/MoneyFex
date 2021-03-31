using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class AddMoneyToWalletEnterAmountVM
    {

        public const string BindProperty = "Amount, CurrencyCode , CurrencySymbol";
        [Range(1, int.MaxValue, ErrorMessage = "Please enter the amount greater than {0}")]
        public decimal Amount { get; set; }
        [Required]
        public string CurrencyCode { get; set; }
        [Required]
        public string CurrencySymbol { get; set; }
    }
}