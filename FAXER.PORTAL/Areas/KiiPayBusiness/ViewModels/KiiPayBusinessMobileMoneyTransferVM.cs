using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessMobileMoneyTransferVM
    {
        public const string BindProperty= "Id ,AvailableBalanceDollar ,AvailableBalanceCent ,RecentMobileNumber , NewMobileNumber ,CurrencySymbol";
        public int Id { get; set; }
        public decimal AvailableBalanceDollar { get; set; }
        public decimal AvailableBalanceCent { get; set; }
        public string RecentMobileNumber { get; set; }
        public string NewMobileNumber { get; set; }
        public string CurrencySymbol { get; set; }
    }
}