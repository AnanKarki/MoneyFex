using FAXER.PORTAL.Areas.Staff.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class StaffDashboardViewModel
    {

        public List<StaffHolidaysViewModel> HoliDay { get; set; }
        public List<ViewStaffTrainingViewModel> Traning { get; set; }
        public List<BusinessDocumentationViewModel> Documentation { get; set; }
        public List<ViewStaffPayslipViewModel> Payslip { get; set; }
        public StaffInformationViewModel Staff { get; set; }
        public ViewPreviousAddressViewModel PreviousAddress { get; set; }
        public ViewRegisteredStaffMoreViewModel Employment { get; set; }
      
    }


}