using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class PrefundingStatementViewModel
    {
        public int Id{ get; set; }
        public string Date{ get; set; }
        public string StaffPayer{ get; set; }
        public string Type{ get; set; }

        public string CurrencyIn { get; set; }
        public string CurrencyOut { get; set; }
        public decimal MoneyIn { get; set; }
        public decimal MoneyOut { get; set; }
        
        public decimal Balance { get; set; }
    }
}