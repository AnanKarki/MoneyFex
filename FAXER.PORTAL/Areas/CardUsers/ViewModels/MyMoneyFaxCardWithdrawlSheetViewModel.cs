using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.ViewModels
{
    public class MyMoneyFaxCardWithdrawlSheetViewModel
    {
        public int TransactionId { get; set; }
        public string NameOfAgency { get; set; }
        public string AgencyMFSCode { get; set; }
        public string WithdrwalAmount { get; set; }

        public string Date { get; set; }
        public string Time { get; set; }
        

    }
}