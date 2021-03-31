using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.ViewModels
{
    public class MFTCCardLocalTopupByCardUserViewModel
    {
        public const string BindProperty = "FaxingAmount ,PaymentReference ";
        public decimal FaxingAmount { get; set; }
        public string PaymentReference { get; set; }
    }
}