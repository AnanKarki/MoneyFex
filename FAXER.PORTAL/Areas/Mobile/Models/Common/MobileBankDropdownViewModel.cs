using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.Common
{
    public class MobileBankDropdownViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string CountryCode { get; set; }
    }
    public class MobileWalletDropDownVm
    {
        public int WalletId { get; set; }
        public string WalletName { get; set; }
        public string CountryCode { get; set; }
    }
}