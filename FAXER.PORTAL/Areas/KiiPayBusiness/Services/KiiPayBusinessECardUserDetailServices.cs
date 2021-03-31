
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Services
{
    public class KiiPayBusinessECardUserDetailServices
    {

        DB.FAXEREntities dbContext = null;

        public KiiPayBusinessECardUserDetailServices()
        {
            dbContext = new DB.FAXEREntities();
        }
        public List<WalletDropDownVM> GetWalletInformation(int kiiPayBusinessInfoId)
        {
            var data = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == kiiPayBusinessInfoId).ToList();
            var result = (from c in data
                          select new WalletDropDownVM()
                          {
                              Id = c.Id,
                              UserName = c.FirstName + " " + c.MiddleName + " " + c.LastName
                          }).ToList();
            return result;

        }



        public KiiPayECardUserDetailsVm GetEcardUserDetails(int WalletId)
        {

            var data = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == WalletId).ToList();

            var result = (from c in data
                          select new KiiPayECardUserDetailsVm()
                          {
                              WalletId = c.Id,
                              Name = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                              City = c.City,
                              Country = Common.Common.GetCountryName(c.Country),
                              Address = c.AddressLine1,
                              DateOfBirth = c.DOB.ToString("dd/MM/yyyy"),
                              EmailAddress = c.Email,
                              PhoneNo = c.PhoneNumber,
                              MobileNo = c.KiiPayBusinessInformation.BusinessMobileNo,
                              IDCardNumber = c.IdCardNumber,
                              ExpiringDateDay = c.IdExpiryDate.Day,
                              ExpiringDateMonth = Enum.GetName(typeof(Month), (Month)c.IdExpiryDate.Month) ,
                              ExpiringDateYear = c.IdExpiryDate.Year,
                              IDCardType = c.IdCardType,
                              IDIssuingCountry = Common.Common.GetCountryName(c.IdIssuingCountry),

                          }).FirstOrDefault();
            return result;

        }

        public void UpdateEcardUserInfo(KiiPayECardUserDetailsVm vm)
        {
            var data = dbContext.KiiPayBusinessECardInfo.Where(x => x.Id == vm.WalletId).FirstOrDefault();
            if (data != null)
            {
                data.City = vm.City;
                data.AddressLine1 = vm.Address;
                data.MobileNo = vm.MobileNo;
                data.Email = vm.EmailAddress;

                dbContext.Entry(data).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
        }
    }
}