using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.ViewModels
{
    public class MyMoneyFaxCardLimitViewModel
    {
        public string AmountOnCard { get; set; }
        public string CardWithdDrawlLimitAmount { get; set; }
        public string CardWithDrawlLimitType { get; set; }

        public string CardPurchaseLimitAmount { get; set; }
        public string CardPurchaseLimitType { get; set; }
    }
}