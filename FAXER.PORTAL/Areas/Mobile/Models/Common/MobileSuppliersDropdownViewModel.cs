using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.Common
{
    public class MobileSuppliersDropdownViewModel
    {
        public int Id { get; set; }
        public string RefCode { get; set; }
        public string CountryCode { get; set; }
        public int KiiPayBusinessId { get; set; }
        public string KiiPayBusinessName { get; set; }
    }
}