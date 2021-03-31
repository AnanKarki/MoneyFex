using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FAXER.PORTAL.Common;
namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class LoginServices
    {
        FAXEREntities dbContext = new FAXEREntities();

        public bool SaveStaffLoginHistory ()
        {
            var data = dbContext.StaffLoginHistory.Where(x => x.Id == Common.AdminSession.StaffLoginHistoryId).FirstOrDefault();
            data.LogoutDate = DateTime.Now.Date;
            data.LogoutTime = DateTime.Now.TimeOfDay;
            data.LogoutLocation = StaffSession.LoggedStaff.StaffCurrentLocation;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

            var data1 = dbContext.StaffLoginHistory.Where(x => x.Id == Common.AdminSession.StaffLoginHistoryId).FirstOrDefault();

            TimeSpan time = data1.LogoutTime.ToString().ToTimeSpan() - data1.LoginTime.ToString().ToTimeSpan();
            data1.TimeLoginToSystem = time;
            data1.CurrentLoginStatus = false;
            dbContext.Entry(data1).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

            return true;



        }

    }
}