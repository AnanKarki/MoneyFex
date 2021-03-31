using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AlertsViewModel
    {
        public int Id { get; set; }
        public  string  AlertHeading { get; set; }

        public string AlertFullMessage { get; set; }

        public string AlertPhoto { get; set; }

        public string Date { get; set; }
        public string Time { get; set; }

       
    }
}