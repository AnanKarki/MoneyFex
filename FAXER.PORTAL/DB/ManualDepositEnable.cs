using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class ManualDepositEnable
    {
        public int Id { get; set; }
        public string PayingCountry { get; set; }
      
        public bool IsEnabled { get; set; }
        public int CreatedById{ get; set; }
        public DateTime CreatedDate{ get; set; }
        public string Agent { get; set; }
        public string AgentAccountNo { get; set; }
        public string AgentAddress  { get; set; }
        public string MobileNo  { get; set; }
    }
}