using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class PayingToKiiPayBusinessInfoVM
    {
        public int BusinessId { get; set; }
        public int WalletId { get; set; }

        public string BusinessName { get; set; }

    }
}