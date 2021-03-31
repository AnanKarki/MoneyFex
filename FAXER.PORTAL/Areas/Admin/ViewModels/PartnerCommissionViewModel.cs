using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class PartnerCommissionViewModel
    {
        public int Id{ get; set; }
        public string PartnerName{ get; set; }
        public string Country{ get; set; }
        public string City{ get; set; }
        public string AgentName{ get; set; }
        public string Service{ get; set; }
        public string Type{ get; set; }
        public string SendingCurrencySymbol { get; set; }
        public string SendingAmount{ get; set; }
        public string RecevingCurrencySymbol { get; set; }
        public string RecevingAmount{ get; set; }
        public string CreationDate { get; set; }
    }
}