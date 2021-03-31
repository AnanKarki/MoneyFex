using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class SendRequesEnterAmountAbroadViewModel
    {
        public const string BindProperty = "Id ,PhotoUrl ,ReceiversName , LocalAmount, ForeignAmount, ExchangeRate ,LocalCurrency , ForeignCurrency," +
            "LocalCurrencySymbol,ForeignCurrencySymbol,Note";
        public int Id { get; set; }
        public string PhotoUrl { get; set; }
        public string ReceiversName { get; set; }
        [Required]
        public decimal LocalAmount { get; set; }
        [Required]
        public decimal ForeignAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public string LocalCurrency { get; set; }
        public string ForeignCurrency { get; set; }
        public string LocalCurrencySymbol { get; set; }
        public string ForeignCurrencySymbol { get; set; }
        public string Note { get; set; }
    }
}