using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Services
{
    public class KiiPayBusinessRegisterECardUserService
    {
        DB.FAXEREntities dbContext = null;
        public KiiPayBusinessRegisterECardUserService()
        {
            dbContext = new DB.FAXEREntities();
        }
        public KiiPayBusinessRegisterECardUserPersonalDetailVM GetKiiPayBusinessPersonalDetail()
        {
            KiiPayBusinessRegisterECardUserPersonalDetailVM vm = new KiiPayBusinessRegisterECardUserPersonalDetailVM();
            // if user try to go back get the session value of business PersonalDetail  
            if (Common.BusinessSession.KiiPayBusinessRegisterECardUserPersonalDetail != null)
            {
                vm = Common.BusinessSession.KiiPayBusinessRegisterECardUserPersonalDetail;
            }

            return vm;

        }
        public void SetKiiPayBusinessPersonalDetail(KiiPayBusinessRegisterECardUserPersonalDetailVM vm)
        {

            Common.BusinessSession.KiiPayBusinessRegisterECardUserPersonalDetail = vm;
        }

        public KiiPayBusinessRegisterECardUserAddressInformationVM GetKiiPayBusinessAddressInformation()
        {
            KiiPayBusinessRegisterECardUserAddressInformationVM vm = new KiiPayBusinessRegisterECardUserAddressInformationVM();
            // if user try to go back get the session value of business AddressInformation
            if (Common.BusinessSession.KiiPayBusinessRegisterECardUserAddressInformation != null)
            {
                vm = Common.BusinessSession.KiiPayBusinessRegisterECardUserAddressInformation;
            }

            return vm;

        }
        public void SetKiiPayBusinessAddressInformation(KiiPayBusinessRegisterECardUserAddressInformationVM vm)
        {

            Common.BusinessSession.KiiPayBusinessRegisterECardUserAddressInformation = vm;
        }
        public KiiPayBusinessRegisterECardUserIdentificationInformationVM GetKiiPayBusinessIdentificationInformation()
        {
            KiiPayBusinessRegisterECardUserIdentificationInformationVM vm = new KiiPayBusinessRegisterECardUserIdentificationInformationVM();
            // if user try to go back get the session value of business AddressInformation
            if (Common.BusinessSession.KiiPayBusinessRegisterECardUserIdentificationInformation != null)
            {
                vm = Common.BusinessSession.KiiPayBusinessRegisterECardUserIdentificationInformation;
            }

            return vm;

        }
        public void SetKiiPayBusinessIdentificationInformation(KiiPayBusinessRegisterECardUserIdentificationInformationVM vm)
        {

            Common.BusinessSession.KiiPayBusinessRegisterECardUserIdentificationInformation = vm;
        }


        public bool CompleteKiiPayBusinessRegisterECardUser(KiiPayBusinessRegisterECardUserPhotoInformationVM vm)
        {


            var personalDetail = Common.BusinessSession.KiiPayBusinessRegisterECardUserPersonalDetail;
            var addressInformation = Common.BusinessSession.KiiPayBusinessRegisterECardUserAddressInformation;
            var identificationInformation = Common.BusinessSession.KiiPayBusinessRegisterECardUserIdentificationInformation;
            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId;

            
            
            DB.KiiPayBusinessECardInfo kiiPayBusinessECardInfo = new DB.KiiPayBusinessECardInfo()
            {
                AddressLine1 = addressInformation.AddressLine1 ,
                AddressLine2 = addressInformation.AddressLine2,
                AutoTopUp = false,
                CardStatus = DB.CardStatus.InActive,
                 CashLimitType = "",
                 CashWithdrawalLimit = 0,
                 City = addressInformation.City,
                 Country = addressInformation.Country,
                 CurrentBalance = 0,
                 DOB = new DateTime(personalDetail.BirthDateYear, (int)personalDetail.BirthDateMonth, personalDetail.BirthDateDay) ,
                 Email = "",
                 FirstName = personalDetail.FirstName ,
                 Gender = personalDetail.Gender,
                 GoodsLimitType = "",
                 GoodsPurchaseLimit = 0,
                 IdCardNumber = identificationInformation.IDCardNumber,
                 IdCardType = identificationInformation.IDCardType,
                 IdExpiryDate = new DateTime(identificationInformation.ExpiringDateYear, (int)identificationInformation.ExpiringDateMonth, identificationInformation.ExpiringDateDay) ,
                 IdIssuingCountry = identificationInformation.IDIssueCountry,
                 KiiPayBusinessInformationId = KiiPayBusinessInformationId,
                 KiiPayUserPhoto = vm.PhotoUrl,
                 LastName = personalDetail.LastName,
                 MFBCardPhoto = "",
                 MiddleName =personalDetail.MiddleName ,
                 MobileNo = "",
                 PhoneNumber = "",
                  PostalCode = addressInformation.PostCodeZipCode,
                   State = "",
                    TempSMS = false,

            };

            var kiiPayBusinessInformation_result = SaveKiiPayBusinessECardInfo(kiiPayBusinessECardInfo);
            return true;
        }

        public DB.KiiPayBusinessECardInfo SaveKiiPayBusinessECardInfo(DB.KiiPayBusinessECardInfo model)
        {
            dbContext.KiiPayBusinessECardInfo.Add(model);
            dbContext.SaveChanges();
            return model;
        }
    }
}