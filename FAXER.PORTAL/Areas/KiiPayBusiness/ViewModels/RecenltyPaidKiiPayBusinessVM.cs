using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class RecenltyPaidKiiPayBusinessVM
    {

        public int BusinessId { get; set; }
        public string MobileNo { get; set; }
        public string Country { get; internal set; }
    }
}