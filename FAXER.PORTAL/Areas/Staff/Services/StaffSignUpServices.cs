using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Staff.Services
{
    public class StaffSignUpServices
    {
        DB.FAXEREntities dbContext = null;
        public StaffSignUpServices() {
            dbContext = new DB.FAXEREntities();
        }

        public DB.StaffInformation SignUp(DB.StaffInformation staff) {
            dbContext.StaffInformation.Add(staff);
            dbContext.SaveChanges();
            SCity.Save(staff.City, staff.Country, DB.Module.Staff);

            return staff;
        }
        public DB.StaffLogin AddStaffLogin(DB.StaffLogin staffLogin) {

            dbContext.StaffLogin.Add(staffLogin);
            dbContext.SaveChanges();
            return staffLogin;
        }
        public DB.StaffInformation GetStaffByEmail(string Email) {

            var data = dbContext.StaffInformation.Where(x => x.EmailAddress == Email).FirstOrDefault();
            return data;
        }
        public DB.StaffLogin GetStaffLoginDetails(string ActivationCode) {

            var data = dbContext.StaffLogin.Where(x => x.ActivationCode == ActivationCode).FirstOrDefault();

            return data;
        }
        public DB.StaffLogin UpdateStaffLogin(ViewModels.StaffLoginViewModel staffLogin) {
            var data = dbContext.StaffLogin.Where(x => (x.UserName == staffLogin.StaffEmail && x.LoginCode == staffLogin.LoginCode)).FirstOrDefault();
            if (data != null)
            {
                data.Password = Common.Common.Encrypt(staffLogin.StaffPassword);
                data.IsActive = true;
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
            return data;

        }
        public DB.StaffAddresses AddStaffAddresses(DB.StaffAddresses staffAddresses) {
            dbContext.StaffAddresses.Add(staffAddresses);
            dbContext.SaveChanges();
            return staffAddresses;
        }
        public string GetNewAccount(int length)
        {
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < length; i++)
                s = String.Concat(s, random.Next(10).ToString());
            //return s;
            while (dbContext.StaffInformation.Where(x => x.StaffMFSCode == s).Count() > 0)
            {
                for (int i = 0; i < length; i++)
                    s = String.Concat(s, random.Next(10).ToString());
            }
            return s;
        }
        public string GetNewLoginCode(int length)
        {
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < length; i++)
                s = String.Concat(s, random.Next(5).ToString());
            //return s;
            while (dbContext.StaffLogin.Where(x => x.LoginCode == s).Count() > 0)
            {
                for (int i = 0; i < length; i++)
                    s = String.Concat(s, random.Next(5).ToString());
            }
            return s;
        }
    }
}