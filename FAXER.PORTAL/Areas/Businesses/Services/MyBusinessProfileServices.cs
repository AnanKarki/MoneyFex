using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.Services
{
    public class MyBusinessProfileServices
    {
        DB.FAXEREntities dbContext = null;
        public MyBusinessProfileServices()
        {

            dbContext = new DB.FAXEREntities();
        }

        internal bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        public ViewModels.MyBusinessProfileViewModel GetBusinessInformation() {

            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
            var result = (from c in dbContext.KiiPayBusinessInformation.Where(x => x.Id == KiiPayBusinessInformationId)
                          join d in dbContext.Country on c.BusinessOperationCountryCode equals d.CountryCode
                          select new ViewModels.MyBusinessProfileViewModel()
                          {
                              Id = c.Id,
                              BusinessName = c.BusinessName,
                              BusinessLicenseNumber = c.BusinessLicenseNumber,
                              Address1 = c.BusinessOperationAddress1 ,
                              Address2 = c.BusinessOperationAddress2,
                              City = c.BusinessOperationCity,
                              PostalCode = c.BusinessOperationPostalCode,
                              State = c.BusinessOperationState,
                              Country = c.BusinessOperationCountryCode,
                              EmailAddress =c.Email,
                              PhoneNumber = c.PhoneNumber,
                              FaxNo = c.FaxNumber,
                              Website = c.Website ,
                              NameOfContactPerson = c.ContactPerson
                              
                          }).FirstOrDefault();
            return result;

        }
        public bool UpdateBusinessAddress(string Address, int id)
        {
            string[] tokens = Address.Split(',');
            var data = dbContext.KiiPayBusinessInformation.Where(x => x.Id == id).FirstOrDefault();
            
            data.BusinessOperationAddress1 = tokens[0];
            data.BusinessOperationCity = tokens[1];
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;
        }
        public bool UpdatePhone(string phone, int id)
        {
            var data = dbContext.KiiPayBusinessInformation.Where(x => x.Id == id).FirstOrDefault();
            data.PhoneNumber = phone;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;
        }
        public bool UpdateFaxNo(string FaxNo, int id)
        {
            var data = dbContext.KiiPayBusinessInformation.Where(x => x.Id == id).FirstOrDefault();
            data.FaxNumber = FaxNo;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;
        }
        public bool UpdateEmailAddress(string Email, int id)
        {
            if (!string.IsNullOrEmpty(Email)) {
                var emailList = dbContext.KiiPayBusinessInformation.Where(x => x.Email == Email).FirstOrDefault();
                if (emailList == null)
                {
                    var data = dbContext.KiiPayBusinessInformation.Where(x => x.Id == id).FirstOrDefault();
                    data.Email = Email;
                    dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    int result = dbContext.SaveChanges();
                    if (result > 0)
                    {
                        var data1 = dbContext.KiiPayBusinessLogin.Where(x => x.KiiPayBusinessInformationId == id).FirstOrDefault();
                        data1.Username = Email;
                        dbContext.Entry(data1).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        var data2 = dbContext.BecomeAMerchant.Where(x => x.RegistrationNumber == data.RegistrationNumBer).FirstOrDefault();
                        if (data2 != null)
                        {
                            data2.BusinessEmailAddress = Email;
                            dbContext.Entry(data2).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                        return true;
                    }
                }
            }
            return false;
        }
        public bool UpdateWebsite(string website, int id)
        {
            var data = dbContext.KiiPayBusinessInformation.Where(x => x.Id == id).FirstOrDefault();
            data.Website = website;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;
        }
        public bool UpdateContactPerson(string ContactPerson, int id)
        {
            var data = dbContext.KiiPayBusinessInformation.Where(x => x.Id == id).FirstOrDefault();
            data.ContactPerson = ContactPerson;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;
        }
    }
}