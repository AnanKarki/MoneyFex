using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class CustomerPaymentFeeViewModel
    {
        public const string BindProperty = " Id,Country,CountryFlag ,BankTransfer ,DebitCard ,CreditCard ,KiiPayWallet ";
        public int? Id { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string Country { get; set; }
        public string CountryFlag { get; set; }
        public decimal BankTransfer { get; set; }
        public decimal DebitCard { get; set; }
        public decimal CreditCard { get; set; }
        public decimal KiiPayWallet { get; set; }
    }
}