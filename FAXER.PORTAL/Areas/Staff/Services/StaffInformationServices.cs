using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Staff.Services
{
    public class StaffInformationServices
    {
        FAXER.PORTAL.DB.FAXEREntities dbContext = new DB.FAXEREntities();


        public DB.StaffInformation GetStaffInformation(int id) {
            var result = dbContext.StaffInformation.Where(x => x.Id == id).FirstOrDefault();
            return result;
        }
        public DB.StaffInformation UpdateStaffAddress(string Address,int id)
        {
            string[] tokens = Address.Split(',');
            var data = dbContext.StaffInformation.Where(x => x.Id == id).FirstOrDefault();
            data.Address1 = tokens[0];
            data.City = tokens[1];
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return data;

        }
        public DB.StaffInformation UpdateStaffTelephone(string Telephone, int id)
        {

            var data = dbContext.StaffInformation.Where(x => x.Id == id).FirstOrDefault();
            data.PhoneNumber = Telephone;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return data;
        }
        public DB.StaffInformation UpdateStaffEmailAddress(string EmailAddress, int id)
        {
            var email = dbContext.StaffInformation.Where(x => x.EmailAddress == EmailAddress).FirstOrDefault();
            if (email == null)
            {
                var data = dbContext.StaffInformation.Where(x => x.Id == id).FirstOrDefault();
                data.EmailAddress = EmailAddress;
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                int result = dbContext.SaveChanges();
                if (result > 0)
                {
                    var data1 = dbContext.StaffLogin.Where(x => x.StaffId == id).FirstOrDefault();
                    data1.UserName = EmailAddress;
                    dbContext.Entry(data1).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return data;
                }
                return data;
            }
            return null;
        }
        public DB.StaffInformation UpdateKinTelephone(string Telephone, int id)
        {

            var data = dbContext.StaffInformation.Where(x => x.Id == id).FirstOrDefault();
            data.NOKPhoneNumber = Telephone;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return data;
        }
        public DB.StaffInformation UpdateKinAddress(string Address, int id)
        {
            string[] tokens = Address.Split(',');
            var data = dbContext.StaffInformation.Where(x => x.Id == id).FirstOrDefault();
            data.NOKAddress1 = tokens[0];
            data.NOKCity = tokens[1];
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return data;
        }
        public DB.StaffInformation UpdateKinFullName(string FullName, int id)
        {
            string[] tokens = FullName.Split(' ');
            var data = dbContext.StaffInformation.Where(x => x.Id == id).FirstOrDefault();
            //data.FirstName = "";
            //data.LastName = "";
            if (tokens.Length > 2)
            {
                data.NOKFirstName = tokens[0];
                data.NOKMiddleName = tokens[1];
                data.NOKLastName = tokens[2];
            }
            else
            {
                data.NOKFirstName = tokens[0];
                data.NOKLastName = tokens[1];
            }
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return data;
        }

        public DB.StaffInformation UpdateResidenePermit(string ResdiencePermitURL , int id) {
            var data = dbContext.StaffInformation.Where(x => x.Id == id).FirstOrDefault();
            data.ResidencePermitUrl = ResdiencePermitURL;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return data;
        }
        public DB.StaffInformation UpdatePassportSide1(string Passport1URL, int id)
        {

            var data = dbContext.StaffInformation.Where(x => x.Id == id).FirstOrDefault();
            data.PassportSide1Url = Passport1URL;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return data;
        }
        public DB.StaffInformation UpdatePassportSide2(string Passport2URL, int id)
        {

            var data = dbContext.StaffInformation.Where(x => x.Id == id).FirstOrDefault();
            data.PassportSide2Url = Passport2URL;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return data;
        }
        public DB.StaffInformation UpdateUtilityBill(string UtilityBillURL, int id)
        {

            var data = dbContext.StaffInformation.Where(x => x.Id == id).FirstOrDefault();
            data.UtilityBillUrl = UtilityBillURL;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return data;
        }
        public DB.StaffInformation UpdateCV(string CVURL, int id)
        {

            var data = dbContext.StaffInformation.Where(x => x.Id == id).FirstOrDefault();
            data.CVUrl = CVURL;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return data;
        }

        public DB.StaffInformation GetImageURL( int id)
        {
            var result = dbContext.StaffInformation.Where(x => x.Id == id).FirstOrDefault();
            return result;
        }
        
    }
}