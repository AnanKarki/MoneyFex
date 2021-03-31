using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{

    public class ViewRegisteredMFBCCardsServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();
        public ViewRegisteredMFBCCardsServices()
        {
        }

        public List<ViewRegisteredMFBCCardsViewModel> getMFBCCardsInfo(string CountryCode = "", string City = "")
        {
            var data = new List<DB.KiiPayBusinessWalletInformation>();

            if (!string.IsNullOrEmpty(CountryCode) && string.IsNullOrEmpty(City))
            {
                data = dbContext.KiiPayBusinessWalletInformation.Where(x => (x.CardStatus != CardStatus.IsDeleted) && (x.CardStatus != CardStatus.IsRefunded) && (x.Country == CountryCode)).ToList();
            }
            else if (string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = dbContext.KiiPayBusinessWalletInformation.Where(x => (x.CardStatus != CardStatus.IsDeleted) && (x.CardStatus != CardStatus.IsRefunded) && (x.City.ToLower() == City.ToLower())).ToList();
            }
            else if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = dbContext.KiiPayBusinessWalletInformation.Where(x => (x.CardStatus != CardStatus.IsDeleted) && (x.CardStatus != CardStatus.IsRefunded) && (x.City.ToLower() == City.ToLower()) && (x.Country == CountryCode)).ToList();
            }



            var result = (from c in data
                          select new ViewRegisteredMFBCCardsViewModel()
                          {
                              Id = c.Id,
                              KiiPayBusinessInformationId = c.KiiPayBusinessInformationId,
                              BusinessName = c.KiiPayBusinessInformation.BusinessName,
                              BusinessRegNumber = c.KiiPayBusinessInformation.RegistrationNumBer,
                              Address = c.KiiPayBusinessInformation.BusinessOperationAddress1,
                              Country = CommonService.getCountryNameFromCode(c.KiiPayBusinessInformation.BusinessOperationCountryCode),
                              PhoneNumber = CommonService.getPhoneCodeFromCountry(c.KiiPayBusinessInformation.BusinessOperationCountryCode) + c.KiiPayBusinessInformation.PhoneNumber,
                              Email = c.KiiPayBusinessInformation.Email,
                              MFBCCardNumber = c.MobileNo.Decrypt(),
                              TempSMSMFBC = c.TempSMS ? "Yes" : "No",
                              CreditOnCard = c.CurrentBalance,
                              Currency = CommonService.getCurrencyCodeFromCountry(c.Country),
                              NumberOfRegMFTCCard = 0,
                              CardUserFulllName = c.FirstName + " " + c.MiddleName + " " +c.LastName,
                              CardUserDoB = c.DOB,
                              CardUserGender = c.Gender == 0 ? "Male" : "Female",
                              CardUserFullAddress = c.AddressLine1,
                              CardUserState = c.State,
                              CardUserTelephone = c.PhoneNumber,
                              CarduserEmail = c.Email,
                              CardUserPhoto = c.KiiPayUserPhoto,
                              CardUsageStatus = Enum.GetName(typeof(CardStatus), c.CardStatus) 
                          }).ToList();
            return result;
        }

        public ViewRegisteredMFBCCardsViewModel getMFBCCardsInfoMore(int id)
        {
            var result = (from c in dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == id).ToList()
                          join d in dbContext.KiiPayBusinessInformation on c.KiiPayBusinessInformationId equals d.Id
                          select new ViewRegisteredMFBCCardsViewModel()
                          {
                              Id = c.Id,
                              KiiPayBusinessInformationId = c.KiiPayBusinessInformationId,
                              BusinessName = d.BusinessName,
                              BusinessRegNumber = d.RegistrationNumBer,
                              Address = d.BusinessOperationAddress1,
                              PhoneNumber = d.PhoneNumber,
                              CountryPhoneCode = CommonService.getPhoneCodeFromCountry(c.Country),
                              Email = d.Email,
                              MFBCCardNumber = c.MobileNo,
                              TempSMSMFBC = c.TempSMS ? "Yes" : "No",
                              CreditOnCard = c.CurrentBalance,
                              NumberOfRegMFTCCard = 0,
                              CardUserFulllName = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                              CardUserDoB = c.DOB,
                              CardUserGender = c.Gender == 0 ? "Male" : "Female",
                              CardUserFullAddress = c.AddressLine1,
                              CardUserState = c.State,
                              CardUserTelephone = c.PhoneNumber,
                              CarduserEmail = c.Email,
                              CardUserPhoto = c.KiiPayUserPhoto,
                              CardPhoto = c.MFBCardPhoto,
                              CardUsageStatus = Enum.GetName(typeof(CardStatus), c.CardStatus )
                          }).FirstOrDefault();

            return result;

        }


        public bool UpdateCardStatus(int id)
        {
            var data = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == id).FirstOrDefault();
            if (data != null)
            {
                if (data.CardStatus == CardStatus.Active)
                {
                    data.CardStatus = CardStatus.InActive;
                }
                else if (data.CardStatus == CardStatus.InActive)
                {
                    data.CardStatus = CardStatus.Active;
                }
                dbContext.Entry(data).State = EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }


        public bool DeleteCard(int id)
        {
            var data = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == id).FirstOrDefault();
            if (data != null)
            {
                data.CardStatus = CardStatus.IsDeleted;
                dbContext.Entry(data).State = EntityState.Modified;
                dbContext.SaveChanges();

                MailCommon mail = new MailCommon();
                var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

                string body = "";
                string RegisterBusinessCard = baseUrl + "/Businesses/RegisterMFBCCard/RegisterBusinessCard";
                var BusinessDetails = dbContext.KiiPayBusinessInformation.Where(x => x.Id == data.KiiPayBusinessInformationId).FirstOrDefault();
                string CardUserCountry = CommonService.getCountryNameFromCode(data.Country);
                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFBCCardDeletionEmail?NameOfBusinessContactPerson=" + BusinessDetails.ContactPerson 
                    + "&MFBCCardNumber=" + data.MobileNo.Decrypt()
                    + "&CardUserName=" + data.FirstName + " " + data.MiddleName + " " + data.LastName +
                       "&CardUserDOB=" + data.DOB.ToString("dd/MM/yyyy") + "&BusinessPhoneNumber=" + data.PhoneNumber
                       + "&CardUserEmail=" + data.Email +
                       "&CardUserCountry=" + CardUserCountry + "&CardUserCity=" + data.City + "&RegisterBusinessCard=" + RegisterBusinessCard);
                mail.SendMail(BusinessDetails.Email, "MoneyFex Business Card - Deletion", body);

                //sending mail to mfbcdeletion@moneyfex.com on mfbccarddeletion
                string subject = "Alert - MFB Card " + data.MobileNo.Decrypt().FormatMFBCCard() + " DELETED";
                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFBCCardDeletion?NameOfDeleter=" + CommonService.getStaffName(Common.StaffSession.LoggedStaff.StaffId)
                    + "&MFBCNo=" + data.MobileNo.Decrypt().FormatMFBCCard() + "&NameOnMFBCard=" + data.FirstName + " " + data.MiddleName + " " + data.LastName
                    + "&MerchantName=" + data.KiiPayBusinessInformation.BusinessName + "&MerchantAccNo=" + data.KiiPayBusinessInformation.BusinessMobileNo + "&DOBOfCardUser=" + data.DOB.ToFormatedString()
                    + "&CardUserHistory=" + CommonService.getCountryNameFromCode(data.Country) + "&CardUserCity=" + data.City + "&AmountOnCard=" + data.CurrentBalance.ToString());
                mail.SendMail("mfbcdeletion@moneyfex.com", subject , body);


                //inserting into deletedmfbccards table
                DeletedMFBCCards data1 = new DeletedMFBCCards()
                {
                    MFBCCardNumber = data.MobileNo,
                    KiiPayBusinessInformationId = data.KiiPayBusinessInformationId,
                    DeletedBy = Common.StaffSession.LoggedStaff.StaffId,
                    Date = DateTime.Now.Date,
                    Time = DateTime.Now.TimeOfDay
                };
                dbContext.DeletedMFBCCards.Add(data1);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool cardExist(string cardNum)
        {
            cardNum = cardNum.Encrypt();
            var data = dbContext.KiiPayBusinessWalletInformation.Where(x => x.MobileNo == cardNum).FirstOrDefault();
            if (data == null)
            {
                return false;
            }
            return true;
        }

        public bool cardDeletedOrNot(string cardNum)
        {
            cardNum = cardNum.Encrypt();
            var data = dbContext.KiiPayBusinessWalletInformation.Where(x => x.MobileNo == cardNum).FirstOrDefault();
            if (data.CardStatus == CardStatus.IsDeleted || data.CardStatus == CardStatus.IsRefunded)
            {
                return false;
            }
            return true;
        }

        public UploadMFBCardPhotoViewModel getMFBCardInfomn(string cardNum)
        {
            cardNum = cardNum.Encrypt();
            var result = (from c in dbContext.KiiPayBusinessWalletInformation.Where(x => x.MobileNo == cardNum).ToList()
                          select new UploadMFBCardPhotoViewModel()
                          {
                              Id = c.Id,
                              Name = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                              CardNum = c.MobileNo.Decrypt().FormatMFBCCard(),
                              BusinessName = c.KiiPayBusinessInformation.BusinessName

                          }).FirstOrDefault();
            return result;
        }

        public bool saveCardPhoto (UploadMFBCardPhotoViewModel model)
        {
            if (model != null)
            {
                var data = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == model.Id).FirstOrDefault();
                if(data != null)
                {
                    data.MFBCardPhoto = model.PhotoURL;
                    data.KiiPayUserPhoto = model.PhotoURL;
                    dbContext.Entry(data).State = EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }

            }
            return false;

        }


    }





}