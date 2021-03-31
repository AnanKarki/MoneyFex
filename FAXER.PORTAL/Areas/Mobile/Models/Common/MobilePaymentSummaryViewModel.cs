using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.Common
{
    public class MobilePaymentSummaryViewModel
    {
        public decimal ExchangeRate { get; set; }
        public decimal Fee { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ReceivingAmount { get; set; }



        public decimal SendingAmount { get; set; }

    }
}