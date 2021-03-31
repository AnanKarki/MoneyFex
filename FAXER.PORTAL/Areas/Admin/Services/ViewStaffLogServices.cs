using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FAXER.PORTAL.Common;
namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewStaffLogServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices ComonnService = new CommonServices();

        public List<ViewStaffLogsViewModel> getStaffLogList(string CountryCode = "", string City = "")
        {
            var data = new List<DB.StaffLoginHistory>();

            if (!string.IsNullOrEmpty(CountryCode) && string.IsNullOrEmpty(City))
            {
                data = dbContext.StaffLoginHistory.Where(x => x.StaffInformation.Country == CountryCode && x.IsDeleted == false).ToList();
            }
            else if (string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = dbContext.StaffLoginHistory.Where(x => x.StaffInformation.City.ToLower() == City.ToLower() && x.IsDeleted == false).ToList();
            }
            else if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = dbContext.StaffLoginHistory.Where(x => (x.StaffInformation.City.ToLower() == City.ToLower()) && (x.StaffInformation.Country == CountryCode) && (x.IsDeleted == false)).ToList();
            }

            var result = (from c in data.OrderByDescending(x=>x.LoginDate)
                          select new ViewStaffLogsViewModel()
                          {
                              StaffId = c.StaffInformationId,
                              StaffFirstName = c.StaffInformation.FirstName,
                              StaffMiddleName = c.StaffInformation.MiddleName,
                              StaffLastName = c.StaffInformation.LastName,
                              StaffCountry = ComonnService.getCountryNameFromCode(c.StaffInformation.Country),
                              StaffCity = c.StaffInformation.City,
                              StaffLoginLevel = c.StaffInformation.SytemLoginLevelOfStaff,
                              LoginDate = c.LoginDate.ToFormatedString(),
                              LoginTime = c.LoginTime.ToString(@"hh\:mm"),
                              LoginLocation = c.LoginLocation,
                              LogoutDate = getLogoutDate(c),
                              LogoutTime =manageLogOutTime(c),
                              LogoutLocation = c.LogoutLocation,
                              TimeLoginToSystem = (c.LoginTime - getLogoutTime(c)).ToString(@"hh\:mm"), 
                              CurrentLoginStatus = c.CurrentLoginStatus ? "Online" : "Offline"
                          }).ToList();
            return result;
        }

        
        
        public List<ViewLogsViewModel> getStaffLog(int id)
        {
            var result = (from c in dbContext.StaffLoginHistory.Where(x => x.StaffInformationId == id && x.IsDeleted == false).OrderByDescending(x=>x.LoginDate).ToList()
                          select new ViewLogsViewModel()
                          {
                              Id = c.Id,
                              StaffId = c.StaffInformationId,
                              StaffName = ComonnService.getStaffName(id),
                              Country = ComonnService.getCountryNameFromCode(c.StaffInformation.Country),
                              City = c.StaffInformation.City,
                              LoginLevel = c.StaffInformation.SytemLoginLevelOfStaff,
                              LoginDate = c.LoginDate.ToFormatedString(),
                              LoginTime = c.LoginTime.ToString(@"hh\:mm"),
                              LoginLocation = c.LoginLocation,
                              LogoutDate = getLogoutDate(c),
                              LogoutTime = manageLogOutTime(c),
                              LogoutLocation = c.LogoutLocation,
                              TDESystem = (c.LoginTime - getLogoutTime(c)).ToString(@"hh\:mm"),
                              CurrentLoginStatus = c.CurrentLoginStatus ? "Online" : "Offline"
                          }).ToList();
            return result;
        }

        public int deleteStaffLog(int id)
        {
            var data = dbContext.StaffLoginHistory.Where(x => x.Id == id).FirstOrDefault();
            if (data != null)
            {
                data.IsDeleted = true;
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                int result = data.StaffInformationId;
                return result;
            }
            return 0;
        }

        private string manageLogOutTime(StaffLoginHistory c)
        {
            if (c.LogoutTime != null)
            {
                return c.LogoutTime.ToString(@"hh\:mm");
            }
            else
            {
                return c.LoginTime.Add(new TimeSpan(0, 20, 0)).ToString(@"hh\:mm");
            }
        }

        private TimeSpan getLogoutTime(StaffLoginHistory c)
        {
            if (c.LogoutTime != null)
            {
                return c.LogoutTime.Value;
            }
            else
            {
                return c.LoginTime.Add(new TimeSpan(0, 20, 0));
            }
        }

        private string getLogoutDate(StaffLoginHistory c)
        {
            if (c.LogoutDate != null)
            {
                return c.LogoutDate.ToFormatedString();
            }
            else
            {
                if (c.LoginTime > new TimeSpan(23, 40, 0))
                {
                    return c.LoginDate.AddDays(1).ToFormatedString();
                }
                return c.LoginDate.ToFormatedString();
            }
        }


    }
}