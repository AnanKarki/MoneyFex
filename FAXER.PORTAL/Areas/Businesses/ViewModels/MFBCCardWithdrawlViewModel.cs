using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class MFBCCardWithdrawlViewModel
    {

        public string PayinAgentName { get; set; }
        public string city { get; set; }

        public string withdrawlAmount { get; set; }

        public string AgentMFSCode { get; set; }
            
        public string Date { get; set; }

        public string TIme { get; set; }
    }
}