using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class SendMoneyToMobileViewModel
    {
        public const string BindProperty = "Id ,PhotoUrl ,ReceiverName ,CurrencySymbol , Amount , CurrencyCode ,SendSMSNotification ";

        public int Id { get; set; }
        public string PhotoUrl { get; set; }
        public string ReceiverName { get; set; }
        public string CurrencySymbol { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }
        public bool SendSMSNotification { get; set; }
    }
}