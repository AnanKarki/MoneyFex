using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class LocationViewModel
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public AgentType AgentType { get; set; }
        public string AgentName { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public int AgentId { get;  set; }
    }
}