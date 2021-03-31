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
    public class ViewRegisteredMFTCServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();


        public List<ViewRegisteredMFTCViewModel> getMFTCList(int id)
        {
            var result = (from c in dbContext.KiiPayPersonalWalletInformation.Where(x => x.IsDeleted == false).ToList()
                          join d in dbContext.SenderKiiPayPersonalAccount.Where(x => x.SenderId == id) on c.Id equals d.KiiPayPersonalWalletId 
                          select new ViewRegisteredMFTCViewModel()
                          {
                              Id = c.Id,
                              FaxerId = d.SenderId,
                              MFTCCardNumber = c.MobileNo.Decrypt(),
                              CardUserFirstName = c.FirstName,
                              CardUserMiddleName = c.MiddleName,
                              CardUserLastName = c.LastName,
                              CardUserDOB = c.CardUserDOB,
                              CardUserAddress1 = c.Address1,
                              CardUserAddress2 = c.Address2,
                              CardUserCountry = CommonService.getCountryNameFromCode(c.CardUserCountry),
                              CardUserCity = c.CardUserCity,
                              CardUserTelephone = CommonService.getPhoneCodeFromCountry(c.CardUserCountry) + c.CardUserTel,
                              CardUserEmail = c.CardUserEmail,
                              CardUserPhoto = c.UserPhoto,
                              CardPhoto = c.MFTCardPhoto,
                              CardUsageStatus = Enum.GetName(typeof(CardStatus), c.CardStatus),
                              TempSMS = false,
                              AmountOnCard = c.CurrentBalance,
                              Currency = CommonService.getCurrencyCodeFromCountry(c.CardUserCountry),
                              AutoTopUp = c.AutoTopUp,
                              CashWithDrawalLimit = c.CashWithdrawalLimit,
                              CashWithDrawalLimitType = Enum.GetName(typeof(DB.CardLimitType), c.CashLimitType),
                              GoodsPurchaseLimit = c.GoodsPurchaseLimit,
                              GoodsPurchaseLimitType = Enum.GetName(typeof(AutoPaymentFrequency), c.GoodsLimitType)
                          }).ToList();
            return result;
        }

        public List<ViewRegisteredMFTCFaxerViewModel> getMFTCFaxerList()
        {
            var result = (from c in dbContext.FaxerInformation.Where(x => x.IsDeleted == false)
                              //join d in dbContext.MFTCCardInformation on c.Id equals d.FaxerId
                          select new ViewRegisteredMFTCFaxerViewModel()
                          {
                              Id = c.Id,
                              FaxerFirstName = c.FirstName,
                              FaxerMiddleName = c.MiddleName,
                              FaxerLastName = c.LastName,
                              FaxerAddress = c.Address1,
                              FaxerCountry = c.Country,
                              FaxerCity = c.City,
                              FaxerTelephone = c.PhoneNumber,
                              FaxerEmail = c.Email

                          }).ToList();
            return result;
        }


        public bool ActivateCard(int id)
        {
            if (id != 0)
            {
                var data = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == id).FirstOrDefault();
                if (data != null)
                {

                    var CardUserLoginInformation = (from c in dbContext.KiiPayPersonalUserLogin
                                                    join d in dbContext.KiiPayPersonalUserInformation.Where(x => x.KiiPayPersonalWalletInformationId == data.Id) on c.KiiPayPersonalUserInformationId equals d.Id
                                                    select c).FirstOrDefault();


                    if (data.CardStatus == CardStatus.Active)
                    {
                        data.CardStatus = CardStatus.InActive;
                        dbContext.Entry(data).State = EntityState.Modified;

                        if (CardUserLoginInformation != null)
                        {

                            CardUserLoginInformation.IsActive = false;
                            dbContext.Entry(CardUserLoginInformation).State = EntityState.Modified;
                           
                        }
                        dbContext.SaveChanges();
                        return true;
                    }
                    else if (data.CardStatus == CardStatus.InActive)
                    {
                        data.CardStatus = CardStatus.Active;
                        dbContext.Entry(data).State = EntityState.Modified;

                        if (CardUserLoginInformation != null)
                        {
                            CardUserLoginInformation.IsActive = true;
                            dbContext.Entry(CardUserLoginInformation).State = EntityState.Modified;
                          
                        }
                        dbContext.SaveChanges();

                        return true;
                    }
                    else if (data.CardStatus == CardStatus.IsDeleted)
                    {
                        if (CardUserLoginInformation != null)
                        {


                            CardUserLoginInformation.IsActive = false;
                            dbContext.Entry(CardUserLoginInformation).State = EntityState.Modified;
                            dbContext.SaveChanges();
                        }

                        return false;
                    }

                }
                return false;
            }
            return false;
        }


        public bool DeleteMFTCCard(int id)
        {
            if (id != 0)
            {
                var data = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == id).FirstOrDefault();
                if (data != null)
                {

                    var CardUserLoginInformation = (from c in dbContext.KiiPayPersonalUserLogin
                                                    join d in dbContext.KiiPayPersonalUserInformation.Where(x => x.KiiPayPersonalWalletInformationId == data.Id) on c.KiiPayPersonalUserInformationId equals d.Id
                                                    select c).FirstOrDefault();

                    data.IsDeleted = true;
                    data.CardStatus = CardStatus.IsDeleted;
                    dbContext.Entry(data).State = EntityState.Modified;
                    if (CardUserLoginInformation != null)
                    {
                        CardUserLoginInformation.IsActive = false;
                        dbContext.Entry(CardUserLoginInformation).State = EntityState.Modified;
                        dbContext.SaveChanges();
                    }

                    MailCommon mail = new MailCommon();
                    var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

                    string body = "";

                    string RegisterMFTCLink = baseUrl + "/TopUpRegestration/UserContactDetails";
                    //var FaxerDetails = dbContext.FaxerInformation.Where(x => x.Id == data.FaxerId).FirstOrDefault();
                    string CardUserCountry = CommonService.getCountryNameFromCode(data.CardUserCountry);

                    //body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFTCCardDeletionEmail?FaxerName=" + FaxerDetails.FirstName + " "
                    //    + FaxerDetails.MiddleName + " " + FaxerDetails.LastName + "&MFTCCardNumber=" + data.MobileNo.Decrypt()
                    //    + "&CardUserName=" + data.FirstName + " " + data.MiddleName + " " + data.LastName +
                    //       "&CardUserDOB=" + data.CardUserDOB.ToString("dd/MM/yyyy") + "&CardUserPhoneNumber=" + data.CardUserTel
                    //       + "&CardUserEmailAddress=" + data.CardUserEmail +
                    //       "&CardUserCountry=" + CardUserCountry + "&CardUserCity=" + data.CardUserCity + "&RegisterMFTC=" + RegisterMFTCLink);
                    //mail.SendMail(FaxerDetails.Email, "MoneyFex Top-Up Card - Deletion", body);


                    //mail for deleted card
                    string subject = "Alert - MFT Card " + data.MobileNo.Decrypt().FormatMFTCCard() + " DELETED";
                    body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFTCCardDeletion?NameOfDeleter=" + CommonService.getStaffName(Common.StaffSession.LoggedStaff.StaffId)
                        + "&MFTCNo=" + data.MobileNo.Decrypt().FormatMFTCCard() + "&NameOnMFTCard=" + data.FirstName + " " + data.MiddleName + " " + data.LastName + "&DOBOfCardUser="
                        + data.CardUserDOB.ToFormatedString() + "&CardUserHistory=" + CommonService.getCountryNameFromCode(data.CardUserCountry) + "&CardUserCity="
                        + data.CardUserCity + "&AmountOnCard=" + data.CurrentBalance.ToString());
                    mail.SendMail("mftcdeletion@moneyfex.com", subject, body);

                    //inserting in deletedmftccards table

                    DeletedMFTCCards data1 = new DeletedMFTCCards()
                    {
                        MoblieNumber = data.MobileNo,
                        FaxerId = 0,
                        DeletedBy = Common.StaffSession.LoggedStaff.StaffId,
                        Date = DateTime.Now.Date,
                        Time = DateTime.Now.TimeOfDay
                    };
                    dbContext.DeletedMFTCCards.Add(data1);
                    dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        public List<ViewRegisteredMFTCFaxerViewModel> getFilterMFTCList(string CountryCode, string City)
        {
            var data = new List<DB.FaxerInformation>();
            if (!string.IsNullOrEmpty(CountryCode) && string.IsNullOrEmpty(City))
            {
                data = dbContext.FaxerInformation.Where(x => x.Country == CountryCode && x.IsDeleted == false).ToList();
            }
            else if (string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {

                data = dbContext.FaxerInformation.Where(x => x.City.ToLower() == City.ToLower() && x.IsDeleted == false).ToList();
            }
            else
            {

                data = dbContext.FaxerInformation.Where(x => x.City.ToLower() == City.ToLower() && x.Country == CountryCode && x.IsDeleted == false).ToList();
            }

            var result = (from c in data
                              //join d in dbContext.MFTCCardInformation on c.Id equals d.FaxerId
                          select new ViewRegisteredMFTCFaxerViewModel()
                          {
                              Id = c.Id,
                              FaxerFirstName = c.FirstName,
                              FaxerMiddleName = c.MiddleName,
                              FaxerLastName = c.LastName,
                              FaxerAddress = c.Address1,
                              FaxerCountry = CommonService.getCountryNameFromCode(c.Country),
                              FaxerCity = c.City,
                              FaxerTelephone = CommonService.getPhoneCodeFromCountry(c.Country) + c.PhoneNumber,
                              FaxerEmail = c.Email

                          }).ToList();
            return result;
        }

        public bool cardExist(string cardNum)
        {
            cardNum = cardNum.Encrypt();
            var data = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == cardNum).FirstOrDefault();
            if (data == null)
            {
                return false;
            }
            return true;
        }

        public bool deletedOrNot(string cardNum)
        {
            cardNum = cardNum.Encrypt();
            var data = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == cardNum).FirstOrDefault();
            if (data.IsDeleted == true)
            {
                return false;
            }
            return true;

        }

        public UploadMFTCardPhotoViewModel getMFTCardInfomn(string cardNum)
        {
            cardNum = cardNum.Encrypt();
            var result = (from c in dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == cardNum).ToList()
                          join d in dbContext.SenderKiiPayPersonalAccount on c.Id equals d.KiiPayPersonalWalletId into SenderKiiPay
                          from d in SenderKiiPay.DefaultIfEmpty()
                          select new UploadMFTCardPhotoViewModel()
                          {
                              Id = c.Id,
                              Name = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                              CardNum = c.MobileNo.Decrypt().FormatMFTCCard(),
                              FaxerName = d == null ? "" :  d.SenderInformation.FirstName + " " + d.SenderInformation.MiddleName + " " + d.SenderInformation.LastName

                          }).FirstOrDefault();
            return result;
        }

        public bool saveCardPhoto(UploadMFTCardPhotoViewModel model)
        {
            if (model != null)
            {
                var data = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == model.Id).FirstOrDefault();
                if (data != null)
                {
                    data.MFTCardPhoto = model.PhotoURL;
                    dbContext.Entry(data).State = EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }

            }
            return false;

        }

    }
}