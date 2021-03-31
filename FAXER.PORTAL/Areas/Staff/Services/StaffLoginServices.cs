using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;


namespace FAXER.PORTAL.Areas.Staff.Services
{
    public class StaffLoginServices
    {
        FAXER.PORTAL.DB.FAXEREntities dbContext = null;
        public StaffLoginServices()
        {
            dbContext = new DB.FAXEREntities();
        }
        public DB.StaffLogin GetLogin(ViewModels.StaffLoginViewModel login)
        {

            var password = Common.Common.Encrypt(login.StaffPassword);
            var data = dbContext.StaffLogin.Where(x => x.UserName == login.StaffEmail && x.IsDeleted == false).FirstOrDefault();

            return data;
        }
        public DB.StaffLogin UpdateLoginFailureCount(string email, int count, bool isActive)
        {

            var data = dbContext.StaffLogin.Where(x => x.UserName == email).FirstOrDefault();

            data.LoginFailedCount = count;
            data.IsActive = isActive;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return data;
        }

        public DB.StaffLogin PasswordReset(string email, ViewModels.ConfirmPasswordResetViewModel model)
        {
            var result = dbContext.StaffLogin.Where(x => x.UserName == email).FirstOrDefault();
            result.Password = Common.Common.Encrypt(model.NewPassword);
            result.IsActive = false;
            dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return result;

        }
        public bool SaveLoginHistory(int staffId)
        {

            DB.StaffLoginHistory loginHistory = new DB.StaffLoginHistory()
            {
                StaffInformationId = staffId,
                LoginDate = DateTime.Now.Date,
                LoginTime = TimeSpan.Zero,
            };
            return true;
        }

        public string CheckStaffName(string staffName, string mfsCode)
        {
            if (StaffSession.LoggedStaff == null )
            {
                return "noStaff";
            }
            //name and mfsCode validation
            var data = dbContext.StaffInformation.Where(x => x.StaffMFSCode == mfsCode).FirstOrDefault();

            if (data == null)
            {
                return "mfsCode";
            }

            if (data != null)
            {
                if (data.Id != StaffSession.LoggedStaff.StaffId)
                {
                    return "invalidUser";
                }

                string name = data.FirstName.Trim() + " " + data.LastName.Trim();
                if (name.ToLower() != staffName.ToLower())
                {
                    return "nameMismatch";
                }
            }



            //day and time validation
            var data1 = dbContext.StaffLogin.Where(x => x.Staff.StaffMFSCode == mfsCode).FirstOrDefault();
            if (data1 == null)
            {
                return "noData";
            }
            if (data1.LoginEndDay == null || data1.LoginStartDay == null)
            {
                return "noData";
            }

            if (data1 != null)
            {
                DayOfWeek today = DateTime.Today.DayOfWeek;
                int today1 = (int)today;
                var startDay = (int)data1.LoginStartDay;
                var endDay = (int)data1.LoginEndDay;
                if (startDay < endDay)
                {
                    if (!(today1 >= startDay && today1 <= endDay))
                    {

                        return "dayMismatch";
                    }
                }
                else if (endDay < startDay)
                {
                    if (!((today1 >= startDay && today1 <= 6) || (today1 >= 0 && today1 <= endDay)))
                    {
                        return "dayMismatch";
                    }
                }

                //time validation
                TimeSpan diff = new TimeSpan();
                var ts =- new TimeSpan(0,StaffSession.StaffTimeZone.ToInt(),0);
                TimeSpan serverTime = getServerTimeByTimeZone();
                if (serverTime < ts)
                {

                    diff = (ts - serverTime);

                }
                else
                {
                    diff = ts - serverTime;
                }

              TimeSpan totalTime =System.DateTime.Now.TimeOfDay.Add(diff);
                if  (!((totalTime>= data1.LoginStartTime.ToTimeSpan()) && (totalTime <= data1.LoginEndTIme.ToTimeSpan())))
                {
                    return "logInTime";
                }
            }

            
            return "success";
        }

        private TimeSpan getServerTimeByTimeZone()
        {

            return TimeZoneInfo.Local.BaseUtcOffset;
        }

        public static int GetTimeZoneOffset(HttpRequest Request)
        {
            TimeZone tz = TimeZone.CurrentTimeZone;
            TimeSpan ts = tz.GetUtcOffset(DateTime.Now);
            int result = (int)ts.TotalMinutes;
            HttpCookie cookie = Request.Cookies["ClientTimeZone"];
            if (cookie != null)
                Int32.TryParse(cookie.Value, out result);
            return result;
        }

        public SystemLoginLevel GetLoginLevel(string mfsCode)
        {
            var result = dbContext.StaffInformation.Where(x => x.StaffMFSCode == mfsCode).FirstOrDefault().SytemLoginLevelOfStaff;
            return (SystemLoginLevel)result;
        }

        public void SaveStaffLoginHistory()
        {
            DB.StaffLoginHistory data = new DB.StaffLoginHistory()
            {
                LoginDate = DateTime.Now.Date,
                LoginTime = DateTime.Now.TimeOfDay,
                LoginLocation = StaffSession.LoggedStaff.StaffCurrentLocation,
                StaffInformationId = StaffSession.LoggedStaff.StaffId,
                CurrentLoginStatus = true


            };
            var loginHistory = dbContext.StaffLoginHistory.Add(data);
            dbContext.SaveChanges();
            AdminSession.StaffLoginHistoryId = loginHistory.Id;

        }
      
        

       
        
        }
    }
