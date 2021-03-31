using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.MoneyFex
{
    public class YearlyTransactionDataListvm
    {
        public int SenderId { get; set; }
        public Month Month { get; set; }
        public decimal LimitAmount { get; set; }
        public decimal TransactionAmount { get; set; }
        public string LimitTitle { get; set; }
        
    }
}