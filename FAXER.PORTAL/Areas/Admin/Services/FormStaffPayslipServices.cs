using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class FormStaffPayslipServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();

        public bool savePayslipInfo(FormStaffPayslipViewModel model)
        {
            if (model != null)
            {
                var data = new StaffPayslip()
                {
                    Year = model.Year,
                    Month = model.Month,
                    StaffId = model.StaffId,
                    PayslipURL = model.PayslipURL,
                    CreatedDateTime = DateTime.Now,
                    CreatedBy = Common.StaffSession.LoggedStaff.StaffId
                };
                dbContext.StaffPayslip.Add(data);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public List<DropDownStaffViewModel> GetEmptyStaffList()
        {
            var result = (from c in dbContext.StaffInformation.Where(x => x.Id == 0)
                          select new DropDownStaffViewModel()
                          {
                              staffId = c.Id,
                              staffName = c.FirstName + " " + c.MiddleName + " " + c.LastName
                          }).ToList();
            return result;
        }

        public List<DropDownStaffViewModel> GetFilteredStaffList(string country, string city)
        {
            var result = (from c in dbContext.StaffInformation.Where(x => (x.Country == country) && ((x.City.Trim()).ToLower() == city))
                          select new DropDownStaffViewModel()
                          {
                              staffId = c.Id,
                              staffName = c.FirstName + " " + c.MiddleName + " " + c.LastName
                          }).ToList();
            return result;
        }
    }
}