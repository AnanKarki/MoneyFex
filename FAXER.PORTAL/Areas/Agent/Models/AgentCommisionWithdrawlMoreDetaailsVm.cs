using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AgentCommisionWithdrawlMoreDetaailsVm
    {
        public int Id { get; set; }
        public string IdType { get; set; }
        public string IdNo { get; set; }
        public string ExpiringDate { get; set; }
        public string IssuingCountry { get; set; }
    }
}