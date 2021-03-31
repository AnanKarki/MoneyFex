using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class PerformanceReportViewModel
    {
        public int Id{ get; set; }
        public string Date{ get; set; }
        public string Platform{ get; set; }
        public string Type{ get; set; }
        public string NoOfErrors{ get; set; }
        public string Status{ get; set; }
    }
}