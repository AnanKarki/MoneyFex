using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class AutoPaymentTopUpViewModel
    {
        public const string BindProperty = "TopUpAmount";
        public decimal TopUpAmount { get; set; }
    }
}