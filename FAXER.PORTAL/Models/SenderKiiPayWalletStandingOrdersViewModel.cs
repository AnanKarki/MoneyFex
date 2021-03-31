using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderKiiPayWalletStandingOrdersViewModel
    {
        public string MobileNo { get; set; }

        public string WalletId { get; set; }
        public int Id { get; set; }
        public string Country { get; set; }
        public string WalletName { get; set; }

        public string City { get; set; }

        public decimal AutoAmount { get; set; }
        public string FrequencyDetails { get; set; }
        public string AutoTopUp { get; set; }
        public AutoPaymentFrequency Frequency { get; set; }
    }
        
}
