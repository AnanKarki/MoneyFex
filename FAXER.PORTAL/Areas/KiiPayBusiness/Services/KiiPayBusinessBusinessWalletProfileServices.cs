using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Services
{
    public class KiiPayBusinessBusinessWalletProfileServices
    {
        DB.FAXEREntities dbContext = null;
        public KiiPayBusinessBusinessWalletProfileServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public KiiPayBusinessBusinessWalletProfileVM GetBusinessWalletProfile(int Id)
        {
            var data = dbContext.KiiPayBusinessUserPersonalInfo.Where(x => x.KiiPayBusinessInformationId == Id).ToList();
            var result = (from c in data.ToList()
                          select new KiiPayBusinessBusinessWalletProfileVM()
                          {

                              Id = c.Id,
                              Address = c.KiiPayBusinessInformation.BusinessOperationAddress1,
                              BillIssuingCompany = c.KiiPayBusinessInformation.BillIsIssuedToCustomer,
                              BusinessLicense = c.KiiPayBusinessInformation.BusinessLicenseNumber,
                              Country = c.KiiPayBusinessInformation.BusinessCountry,
                              TelephoneWalletNumber = c.KiiPayBusinessInformation.BusinessMobileNo,
                              Email = c.KiiPayBusinessInformation.Email,
                              NameofBusiness = c.KiiPayBusinessInformation.BusinessName,
                              ContactPersonName = c.KiiPayBusinessInformation.ContactPerson,
                              ContactPersonAddress = c.AddressLine1,
                              ContactPersonCountry = c.Country,
                          }).FirstOrDefault();
            return result;
        }


        public void UpdateBusinessWalletProfile(KiiPayBusinessBusinessWalletProfileVM vm)
        {
            var data = dbContext.KiiPayBusinessInformation.Where(x => x.Id == Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId).FirstOrDefault();
            if (data != null)
            {
                data.BusinessOperationAddress1 = vm.Address;
                data.BusinessMobileNo = vm.TelephoneWalletNumber;
                data.Email = vm.Email;
                data.BillIsIssuedToCustomer = vm.BillIssuingCompany;
                dbContext.Entry(data).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }
    }
}