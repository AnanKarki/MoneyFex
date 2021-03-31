using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Staff.ViewModels
{
    public class StaffPayslipsViewModel
    {

        public int PaySlipsId { get; set; }
        public string Date { get; set; }

        public string PayslipUrl { get; set; }
    }
}