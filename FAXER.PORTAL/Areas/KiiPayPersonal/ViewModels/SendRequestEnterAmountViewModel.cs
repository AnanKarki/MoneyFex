using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class SendRequestEnterAmountViewModel
    {
        public const string BindProperty = "Id ,PhotoUrl ,ReceiverName , CurrencySymbol, CurrencyCode, Amount ,Note";

        public int Id { get; set; }
        public string PhotoUrl { get; set; }
        public string ReceiverName { get; set; }
        public string CurrencySymbol { get; set; }
        public string CurrencyCode { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public string Note { get; set; }
    }
}