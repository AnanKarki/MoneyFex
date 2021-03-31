using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Staff.Services
{
    public class StaffPaysilpServices
    {
        DB.FAXEREntities dbContext = null;

        public StaffPaysilpServices()
        {
            dbContext = new DB.FAXEREntities();

        }

        public List<ViewModels.StaffPayslipsViewModel> GetStaffPayslipsList(Month month = 0)
        {
            int staffId = Common.StaffSession.LoggedStaff.StaffId;
            var data = dbContext.StaffPayslip.Where(x => x.StaffId == staffId).ToList();
            if (month != 0)
            {
                data = dbContext.StaffPayslip.Where(x => x.StaffId == staffId && x.Month == month).ToList();
            }
            var reuslt = (from c in data
                          select new ViewModels.StaffPayslipsViewModel()
                          {
                              Date = c.Month + " " + c.Year,
                              PaySlipsId = c.Id,
                              PayslipUrl = c.PayslipURL

                          }).ToList();

            return reuslt;

        }
    }

}