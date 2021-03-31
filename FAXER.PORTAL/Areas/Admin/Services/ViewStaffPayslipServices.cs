using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewStaffPayslipServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();

        public List<ViewStaffPayslipViewModel> getList(string CountryCode = "", string City = "", int Year = 0, int month = 0)
        {

            var data = dbContext.StaffPayslip.Where(x => x.IsDeleted == false).ToList();
            //if (CountryCode != "" && City != "" && Year != 0 && month != 0)
            //{
            //    data = dbContext.StaffPayslip.Where(x => x.IsDeleted == false && x.Staff.Country == CountryCode && x.Staff.City.ToLower() == City.ToLower() && x.Year == Year && x.Month == (Month)month).ToList();
            //}
            if (!string.IsNullOrEmpty(CountryCode))
            {
                data = data.Where(x => x.Staff.Country == CountryCode).ToList();

            }
            if (!string.IsNullOrEmpty(City))
            {
                data = data.Where(x => x.Staff.City == City).ToList();

            }
            if (Year != 0)
            {
                data = data.Where(x => x.Year == Year).ToList();

            }
            if (month != 0)
            {
                data = data.Where(x => x.Month == (Month)month).ToList();

            }
            var result = (from c in data
                          join d in dbContext.StaffLogin on c.StaffId equals d.StaffId
                          select new ViewStaffPayslipViewModel()
                          {
                              Id = c.Id,
                              StaffId = c.StaffId,
                              StaffName = CommonService.getStaffName(c.StaffId),
                              StaffMFSCode = CommonService.getStaffMFSCode(c.StaffId),
                              StaffCountry = CommonService.getCountryNameFromCode(c.Staff.Country),
                              StaffCity = c.Staff.City,
                              PayslipMonth = Enum.GetName(typeof(Month), c.Month),
                              PayslipYear = c.Year,
                              StaffStatus = d.IsActive ? "Active" : "Inactive",
                              PayslipPDF = c.PayslipURL,
                              StaffCountryFlag = c.Staff.Country.ToLower()

                          }).ToList();
            return result;
        }
        public List<ViewStaffPayslipViewModel> getStaffPaySlipList()
        {


            var data = dbContext.StaffPayslip.Where(x => x.IsDeleted == false).ToList();

            var result = (from c in data
                          join d in dbContext.StaffLogin on c.StaffId equals d.StaffId
                          select new ViewStaffPayslipViewModel()
                          {
                              Id = c.Id,
                              StaffId = c.StaffId,
                              StaffName = CommonService.getStaffName(c.StaffId),
                              StaffMFSCode = CommonService.getStaffMFSCode(c.StaffId),
                              StaffCountry = CommonService.getCountryNameFromCode(c.Staff.Country),
                              StaffCity = c.Staff.City,
                              PayslipMonth = Enum.GetName(typeof(Month), c.Month),
                              PayslipYear = c.Year,
                              StaffStatus = d.IsActive ? "Active" : "Inactive",
                              PayslipPDF = c.PayslipURL,
                              StaffCountryFlag = c.Staff.Country.ToLower()

                          }).ToList();
            return result;
        }

        public bool deletePayslip(int id)
        {
            if (id != 0)
            {
                var data = dbContext.StaffPayslip.Where(x => x.Id == id).FirstOrDefault();
                if (data != null)
                {
                    data.IsDeleted = true;
                    data.DeletedBy = Common.StaffSession.LoggedStaff.StaffId;
                    data.DeletedDate = DateTime.Now;
                    dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public ViewStaffPayslipViewModel GetPaySlipById(int id)
        {
            var result = (from c in dbContext.StaffPayslip.Where(x => (x.Id == id)).ToList()
                          select new ViewStaffPayslipViewModel()
                          {
                              StaffCountry = c.Staff.Country,
                              StaffCity = c.Staff.City,
                              StaffId = c.StaffId,
                              StaffMFSCode = CommonService.getStaffMFSCode(c.StaffId),
                              Month = c.Month,
                              PayslipYear = c.Year,
                              PayslipPDF = c.PayslipURL
                          }).FirstOrDefault();
            return result;
        }

        public bool EditPaySpil(ViewStaffPayslipViewModel model)
        {
            if (model != null)
            {
                var data = dbContext.StaffPayslip.Find(model.Id);
                if (data != null)
                {
                    data.Year = model.PayslipYear;
                    data.Month = model.Month;
                    data.StaffId = model.StaffId;
                    data.PayslipURL = model.PayslipPDF;
                    data.ModifiedBy = Common.StaffSession.LoggedStaff.StaffId;
                    dbContext.Entry(data).State = EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
            }
            return false;
        }
        public bool SavePaySpil(ViewStaffPayslipViewModel model)
        {
            if (model != null)
            {
                var data = new StaffPayslip()
                {
                    Month = model.Month,
                    CreatedBy = Common.StaffSession.LoggedStaff.StaffId,
                    CreatedDateTime = DateTime.Now.Date,
                    PayslipURL = model.PayslipPDF,
                    StaffId = model.StaffId,
                    Year = model.PayslipYear,

                };
                dbContext.StaffPayslip.Add(data);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }


    }
}