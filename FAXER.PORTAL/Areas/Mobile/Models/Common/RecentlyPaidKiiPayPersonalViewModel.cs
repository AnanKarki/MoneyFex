using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.Common
{
    public class RecentlyPaidKiiPayPersonalViewModel
    {
        public int WalletId { get; set; }
        public string MobileNo { get; set; }
        public string Country { get; set; }
        public bool ReceiverIsLocal { get; set; }
        public string FullName { get; set; }
    }
}