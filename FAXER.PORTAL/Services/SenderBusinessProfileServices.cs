using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;

namespace FAXER.PORTAL.Services
{
    public class SenderBusinessProfileServices
    {
        DB.FAXEREntities db = null;
        public SenderBusinessProfileServices()
        {
            db = new DB.FAXEREntities();
        }

        public SenderBusinessprofileViewModel GetSenderBusinessProfileData(int FaxerId)
        {

            var FaxerInfo = db.FaxerInformation.Where(x => x.Id == FaxerId).FirstOrDefault();
            var businessInfo = db.BusinessRelatedInformation.Where(x => x.FaxerId == FaxerId).FirstOrDefault();

            SenderBusinessprofileViewModel vm = new SenderBusinessprofileViewModel()
            {
                AccountNo = FaxerInfo.AccountNo,
                Address1 = FaxerInfo.Address1,
                Address2 = FaxerInfo.Address2,
                Country = Common.Common.GetCountryName(FaxerInfo.Country),
                Email = FaxerInfo.Email,
                City = FaxerInfo.City,
                RegistrationNumber = businessInfo.RegistrationNo,
                BusinessAddress1 = businessInfo.OperatingAddressLine1,
                BusinessAddress2 = businessInfo.OperatingAddressLine2,
                BusinessCity = businessInfo.OperatingCity,
                BusinessName = businessInfo.BusinessName,
                ContactName = FaxerInfo.FirstName + " " + FaxerInfo.MiddleName + " " + FaxerInfo.LastName,
                PhoneCode = Common.Common.GetCountryPhoneCode(FaxerInfo.Country),
                PhoneNumber = FaxerInfo.PhoneNumber,
                Postal = FaxerInfo.PostalCode,
                BusinessCountry = Common.Common.GetCountryName(businessInfo.Country)


            };
            return vm;

        }
        public bool Update(SenderBusinessprofileViewModel model, int FaxerId)
        {

            var faxer = db.FaxerInformation.Where(x => x.Id == FaxerId).FirstOrDefault();
            faxer.PhoneNumber = model.PhoneNumber;
            faxer.Email = model.Email;
            faxer.Address1 = model.Address1;
            faxer.Address2 = model.Address2;
            faxer.City = model.City;
            faxer.PostalCode = model.Postal;
            db.Entry<FaxerInformation>(faxer).State = EntityState.Modified;
            db.SaveChanges();
            return true;

        }


        public string GetMobilePin(string PhoneNo = "")
        {
            SSenderProfileService _senderProfileService = new SSenderProfileService();
            //if session null generate code and return  else return value in session
            string code = "";
            if (Common.FaxerSession.SentMobilePinCode == null || Common.FaxerSession.SentMobilePinCode == "")
            {
                code = Common.Common.GenerateRandomDigit(6);
                _senderProfileService.SetMobilePinCode(code);

                SmsApi smsApi = new SmsApi();
                var msg = smsApi.GetAddressUpdateMessage(code);
                string UserPhoneNo = Common.FaxerSession.LoggedUser.CountryPhoneCode + Common.FaxerSession.LoggedUser.PhoneNo;
                smsApi.SendSMS(UserPhoneNo, msg);
            }
            else
            {
                code = Common.FaxerSession.SentMobilePinCode;
            }
            string mobilePinCode = code;
            return mobilePinCode;
        }

        public FaxerInformation GetFaxerInfo(int faxerId)
        {
            var faxer = db.FaxerInformation.Where(x => x.Id == faxerId).FirstOrDefault();
            return faxer;
        }
    }
}