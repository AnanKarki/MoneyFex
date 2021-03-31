using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class AgentLocations
    {
        public int Id { get; set; }
        public string CountryCode { get; set; }
        public string City { get; set; }
        public AgentType AgentType { get; set; }
        public int AgentId { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }

        public int ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}