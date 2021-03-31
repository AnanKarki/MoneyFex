using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderAddMoneyToWalletViewModel
    {

        public const string BindProperty = "AvailableCurrency,CurrencyCode,CurrencySymbol, Amount , AvailableBalance";
        [MaxLength(200)]
        public string  AvailableCurrency { get; set; }
        [MaxLength(200)]
        public string CurrencyCode { get; set; }
        [MaxLength(200)]
        public string CurrencySymbol { get; set; }
        
        [Range(0.0, Double.MaxValue , ErrorMessage = "Enter amount")]

        public decimal Amount { get; set; }
        [Range(0.0, Double.MaxValue)]

        public decimal AvailableBalance { get; set; }
       
    }
}