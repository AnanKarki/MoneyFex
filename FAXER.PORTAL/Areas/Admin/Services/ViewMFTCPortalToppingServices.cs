using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Controllers;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System.Data.Entity;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewMFTCPortalToppingServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();
        public ViewMFTCPortalToppingViewModel getFaxerAndCardUserInfo(string MFTCCard)
        {

            string[] CardNumber = MFTCCard.Split('-');
            if (CardNumber.Length < 2)
            {
                var result = dbContext.KiiPayPersonalWalletInformation.Where(x => x.IsDeleted == false).ToList();

                for (int i = 0; i < result.Count; i++)
                {

                    string[] tokens = result[i].MobileNo.Decrypt().Split('-');
                    if (tokens[1] == MFTCCard)
                    {

                        var Card = result[i].MobileNo;
                        var model = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == Card && x.CardStatus == DB.CardStatus.Active).FirstOrDefault();
                        if (model == null)
                        {

                            MFTCCard = "";
                        }
                        MFTCCard = model.MobileNo.Decrypt();
                    }

                }
            }

            if (string.IsNullOrEmpty(MFTCCard))
                return null;

            ViewMFTCPortalToppingViewModel vm = new ViewMFTCPortalToppingViewModel();

            string card = MFTCCard.Encrypt();
            //vm.ViewMFTCPortalFaxer = (from c in dbContext.MFTCCardInformation.Where(x => x.MobileNo == card).ToList()
            //                          join d in dbContext.SenderKiiPayPersonalAccount on c.Id equals d.KiiPayPersonalWalletId
            //                          select new ViewMFTCPortalToppingFaxerDetailViewModel()
            //                          {
            //                              FaxerId = d == null ==  c.FaxerId,
            //                              FaxerFirstName = c.FaxerInformation.FirstName,
            //                              FaxerMiddleName = c.FaxerInformation.MiddleName,
            //                              FaxerLastName = c.FaxerInformation.LastName,
            //                              FaxerIDCardNumber = c.FaxerInformation.IdCardNumber,
            //                              FaxerIDCardType = c.FaxerInformation.IdCardType,
            //                              FaxerIDCardExpDate = c.FaxerInformation.IdCardExpiringDate.ToString("dd-MM-yyyy"),
            //                              FaxingIDCardIssuingCountry = c.FaxerInformation.IssuingCountry,
            //                              FaxerAddress = c.FaxerInformation.Address1,
            //                              FaxerCity = c.FaxerInformation.City,
            //                              FaxerPostalCode = c.FaxerInformation.PostalCode,
            //                              FaxerCountry = CommonService.getCountryNameFromCode(c.FaxerInformation.Country),
            //                              FaxerTelephone = c.FaxerInformation.PhoneNumber,
            //                              FaxerEmailAddress = c.FaxerInformation.Email,
            //                              FaxerCountryCode = c.FaxerInformation.Country
            //                          }).FirstOrDefault();

            vm.ViewMFTCPortalCardUser = (from c in dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == card).ToList()
                                         join d in dbContext.Country on c.CardUserCountry equals d.CountryCode
                                         select new ViewMFTCPortalToppingCardUserDetailViewModel()
                                         {
                                             CardUserId = c.Id,
                                             CardUserFirstName = c.FirstName,
                                             CardUserMiddleName = c.MiddleName,
                                             CardUserLastName = c.LastName,
                                             CardUserAddress = c.Address1,
                                             CardUserCity = c.CardUserCity,
                                             CardUserCountry = d.CountryName,
                                             CardUserCountryCode = c.CardUserCountry,
                                             CardUserTelephone = c.CardUserTel,
                                             CardUserPhoto = c.UserPhoto
                                         }).FirstOrDefault();
            //  vm.isCardAvailable = true;
            vm.CalculateFaxingFee.Currency = CommonService.getCurrency(vm.ViewMFTCPortalFaxer.FaxerCountryCode);
            vm.CalculateFaxingFee.CurrencySymbol = CommonService.getCurrencySymbol(vm.ViewMFTCPortalFaxer.FaxerCountryCode);

            vm.CalculateFaxingFee.ReceivingCurrency = CommonService.getCurrency(vm.ViewMFTCPortalCardUser.CardUserCountryCode);
            vm.CalculateFaxingFee.ReceivingCurrencySymbol = CommonService.getCurrencySymbol(vm.ViewMFTCPortalCardUser.CardUserCountryCode);

            vm.AdminName = CommonService.getStaffName(Common.StaffSession.LoggedStaff.StaffId);




            return vm;
        }

        public bool checkMFTCCard(string card)
        {

            string[] CardNumber = card.Split('-');
            if (CardNumber.Length < 2)
            {
                var result = dbContext.KiiPayPersonalWalletInformation.Where(x => x.IsDeleted == false).ToList();

                for (int i = 0; i < result.Count; i++)
                {

                    string[] tokens = result[i].MobileNo.Decrypt().Split('-');
                    if (tokens[1] == card)
                    {

                        var MFTCCard = result[i].MobileNo;
                        var model = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == MFTCCard && x.CardStatus == DB.CardStatus.Active).FirstOrDefault();
                        if (model == null) {

                            return false;
                        }
                        return true;
                    }

                }
            }
            card = card.Encrypt();
            var data = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == card).FirstOrDefault();
            if (data == null)
            {
                return false;
            }
            return true;
        }



        public CalculateFaxingFeeVm getCalculateSummary(decimal topUpAmount, string FaxingCountryCode, int mFTCCardInformationId)
        {
            CalculateFaxingFeeVm vm = new CalculateFaxingFeeVm();
            var reciver = getRecvingFaxer(mFTCCardInformationId);
            string ReceivingCountryCode = reciver.CardUserCountry;
            var feeSummary = new EstimateFaxingFeeSummary();
            decimal exchangeRate = 0, faxingFee = 0;
            var exchangeRateObj = dbContext.ExchangeRate.Where(x => x.CountryCode1 == FaxingCountryCode && x.CountryCode2 == ReceivingCountryCode).FirstOrDefault();
            if (exchangeRateObj == null)
            {
                var exchangeRateobj2 = dbContext.ExchangeRate.Where(x => x.CountryCode1 == ReceivingCountryCode && x.CountryCode2 == FaxingCountryCode).FirstOrDefault();
                if (exchangeRateobj2 != null)
                {
                    exchangeRateObj = exchangeRateobj2;
                    exchangeRateObj.CountryRate1 = Math.Round(1 / exchangeRateobj2.CountryRate1, 6, MidpointRounding.AwayFromZero);
                }
                else if (FaxingCountryCode.ToLower() == ReceivingCountryCode.ToLower())
                {

                    exchangeRate = 1m;
                }
                else 
                {

                    return null;
                }
            }
            else
            {

                exchangeRate = exchangeRateObj.CountryRate1;

            }
          

            feeSummary = SEstimateFee.CalculateFaxingFee(topUpAmount, true, false, exchangeRate, SEstimateFee.GetFaxingCommision(FaxingCountryCode));
            Common.FaxerSession.FaxingAmountSummary = feeSummary;
            vm.ExchangeRate = feeSummary.ExchangeRate;
            vm.TopUpFees = feeSummary.FaxingFee;
            vm.TopUpAmount = feeSummary.FaxingAmount;
            vm.ReceivingAmount = feeSummary.ReceivingAmount;
            vm.TotalAmountIncludingFees = feeSummary.FaxingFee + feeSummary.FaxingAmount;
            vm.ReceivingCurrency = CommonService.getCurrency(ReceivingCountryCode);
            vm.ReceivingCurrencySymbol = CommonService.getCurrencySymbol(ReceivingCountryCode);
            return vm;
        }

        internal Country GetFaxerCountry(string faxerCountryCode)
        {
            var country = dbContext.Country.Where(x => x.CountryCode == faxerCountryCode).FirstOrDefault();
            if (country != null)
            {
                return country;
            }
            else
            {
                return null;
            }
        }

        public List<DropDownMFTCCardsViewModel> getMFTCCardNumbersWithFormating(int faxerId)
        {
            var result = (from c in dbContext.KiiPayPersonalWalletInformation.ToList()
                          select new DropDownMFTCCardsViewModel()
                          {
                              Id = c.Id,
                              MFTCCardNumber = c.MobileNo.Decrypt().FormatMFTCCard()
                          }).ToList();
            return result;
        }
        public List<DropDownMFTCCardsViewModel> getMFTCCardNumbersWithoutFormating(int faxerId)
        {
            var result = (from c in dbContext.KiiPayPersonalWalletInformation.ToList()
                          select new DropDownMFTCCardsViewModel()
                          {
                              Id = c.Id,
                              MFTCCardNumber = c.MobileNo
                          }).ToList();
            return result;
        }

        public string MFTC(int Id)
        {
            var card = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == Id).FirstOrDefault();
            if (card != null)
            {
                return card.MobileNo.Decrypt();
            }
            else
                return "";
        }

        public KiiPayPersonalWalletInformation getRecvingFaxer(int Id)
        {
            return dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == Id).FirstOrDefault();
        }

        public bool saveTopUpData(ViewMFTCPortalToppingViewModel model, int MFTCCardInformationId)
        {

            if (model != null)
            {
                string ReceiptNo = GetNewAdminMFTCTopUpReceiptNumber();
                var data = new SenderKiiPayPersonalWalletPayment()
                {
                    FaxerId = model.ViewMFTCPortalFaxer.FaxerId,
                    RecievingAmount = model.CalculateFaxingFee.ReceivingAmount,
                    TransactionDate = DateTime.Now,
                    FaxingAmount = model.CalculateFaxingFee.TopUpAmount,
                    FaxingFee = model.CalculateFaxingFee.TopUpFees,
                    ExchangeRate = model.CalculateFaxingFee.ExchangeRate,
                    KiiPayPersonalWalletInformationId = MFTCCardInformationId,
                    OperatingUserType = OperatingUserType.Admin,
                    OperatingStaffId = Common.StaffSession.LoggedStaff.StaffId,
                    OperatingUserName = CommonService.getStaffName(Common.StaffSession.LoggedStaff.StaffId),
                    ReceiptNumber = ReceiptNo,
                    PaymentMethod = model.isCardAvailable == true ? "PM001" : "PM003"
                    //FaxingStatus = 
                };

                var save = dbContext.FaxingCardTransaction.Add(data);
                dbContext.SaveChanges();
                if (model.ToppingPayment.CardNumber != null)
                {
                    string cardNum = model.ToppingPayment.CardNumber.Encrypt();

                    var savedCard = dbContext.SavedCard.Where(x => x.Num == cardNum).FirstOrDefault();

                    if (model.isCardAvailable == true)
                    {
                        var cardData = new CardTopUpCreditDebitInformation()
                        {
                            CardTransactionId = save.Id,
                            NameOnCard = model.ToppingPayment.NameOnCard,
                            CardNumber = "xxxx-xxxx-xxxx-" + model.ToppingPayment.CardNumber.Right(4),
                            ExpiryDate = model.ToppingPayment.EndMonth + "/" + model.ToppingPayment.EndYear,
                            IsSavedCard = savedCard == null ? false : true
                        };
                        dbContext.CardTopUpCreditDebitInformation.Add(cardData);
                        dbContext.SaveChanges();
                    }
                }


                var MFTCCardInfo = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardInformationId).FirstOrDefault();
                if (MFTCCardInfo != null)
                {
                    decimal currentBalance = MFTCCardInfo.CurrentBalance;
                    currentBalance = currentBalance + model.CalculateFaxingFee.ReceivingAmount;
                    MFTCCardInfo.CurrentBalance = currentBalance;
                    dbContext.Entry(MFTCCardInfo).State = EntityState.Modified;
                    dbContext.SaveChanges();

                    // Send Confirmation of Top Up
                    MailCommon mail = new MailCommon();
                    var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                    string FaxerName = model.ViewMFTCPortalFaxer.FaxerFirstName + " " + model.ViewMFTCPortalFaxer.FaxerMiddleName + " " + model.ViewMFTCPortalFaxer.FaxerLastName;
                    string FaxerEmail = model.ViewMFTCPortalFaxer.FaxerEmailAddress;
                    string body = "";

                    string CardUserCountry = MFTCCardInfo.CardUserCountry;
                    string TopUpMoneyfaxCard = baseUrl + "/StartFaxingMoney/Index?mFTCCardInformationId=" + MFTCCardInformationId;
                    string SetAutoTopUp = baseUrl + "/FaxerAutoPayments/MoneyFaxCardAutoTopUp?mftcCardInformationId=" + MFTCCardInformationId;
                    body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFTCCardTopUp?FaxerName=" + FaxerName +
                        "&CardUserCountry=" + model.ViewMFTCPortalCardUser.CardUserCountry + "&TopUpMoneyfaxCard=" + TopUpMoneyfaxCard + "&SetAutoTopUp=" + SetAutoTopUp);


                    string FaxerCurrency = Common.Common.GetCountryCurrency(model.ViewMFTCPortalFaxer.FaxerCountryCode);
                    var CardUserCurrency = Common.Common.GetCountryCurrency(MFTCCardInfo.CardUserCountry);
                    string ReceiptURL = baseUrl + "/EmailTemplate/AdminMFTCCardTop?ReceiptNumber=" + data.ReceiptNumber + "&Date=" +
                        data.TransactionDate.ToString("dd/MM/yyyy") + "&Time=" + data.TransactionDate.ToString("HH:mm")
                        + "&FaxerFullName=" + FaxerName + "&MFTCCardNumber=" + MFTCCardInfo.MobileNo.Decrypt()
                        + "&CardUserFullName=" + MFTCCardInfo.FirstName + " " + MFTCCardInfo.MiddleName + " " + MFTCCardInfo.LastName
                        + "&AmountTopUp=" + model.CalculateFaxingFee.TopUpAmount +  "&ExchangeRate=" + model.CalculateFaxingFee.ExchangeRate +
                        "&AmountInLocalCurrency=" + model.CalculateFaxingFee.ReceivingAmount + "&Fee=" + model.CalculateFaxingFee.TopUpFees + " "
                         + "&BalanceOnCard=" + MFTCCardInfo.CurrentBalance +
                        "&StaffName=" + Common.StaffSession.LoggedStaff.FirstName + " " + Common.StaffSession.LoggedStaff.MiddleName + " " + Common.StaffSession.LoggedStaff.LastName
                        + "&StaffCode=" + Common.StaffSession.LoggedStaff.StaffMFSCode + "&SendingCurrency=" + Common.Common.GetCountryCurrency(model.ViewMFTCPortalFaxer.FaxerCountryCode) + "&ReceivingCurrency=" + Common.Common.GetCountryCurrency(model.ViewMFTCPortalCardUser.CardUserCountryCode) +
                         "&DepositType=" + Common.StaffSession.LoggedStaff.LoginCode + 
                         "&FaxerCountry=" + Common.Common.GetCountryName( model.ViewMFTCPortalFaxer.FaxerCountryCode)
                          + "&CardUserCountry=" + Common.Common.GetCountryName(model.ViewMFTCPortalCardUser.CardUserCountryCode) + 
                          "&CardUserCity=" + model.ViewMFTCPortalCardUser.CardUserCity + "&SendingCurrency="  + FaxerCurrency + "&ReceivingCurrency=" + CardUserCurrency;
                    
                    var ReceiptPDF = Common.Common.GetPdf(ReceiptURL);

                    mail.SendMail(FaxerEmail, "Confirmation of Virtual Account Payment", body, ReceiptPDF);
                    //mail.SendMail("anankarki97@gmail.com", "Confirmation of Virtual Account Payment", body, ReceiptPDF);

                    // Sms Function  
                    SmsApi smsApiServices = new SmsApi();
                    string senderName = model.ViewMFTCPortalFaxer.FaxerFirstName + " " + model.ViewMFTCPortalFaxer.FaxerMiddleName + " " + model.ViewMFTCPortalFaxer.FaxerLastName;
                    string virtualAccounntNo = MFTCCardInfo.MobileNo.Decrypt().GetVirtualAccountNo();
                
                    string amount = CommonService.getCurrencySymbol(model.ViewMFTCPortalFaxer.FaxerCountryCode) + model.ToppingPayment.FaxingAmount.ToString();
                    string receivingAmount = CommonService.getCurrencySymbol(model.ViewMFTCPortalCardUser.CardUserCountryCode)
                                                   + model.CalculateFaxingFee.ReceivingAmount;
                    string message = smsApiServices.GetVirtualAccountDepositMessage(senderName,virtualAccounntNo , amount , receivingAmount);
                    string phoneNumber = CommonService.getPhoneCodeFromCountry(model.ViewMFTCPortalFaxer.FaxerCountryCode) + model.ViewMFTCPortalFaxer.FaxerTelephone;
                    smsApiServices.SendSMS(phoneNumber, message);

                  

                    string receiverPhoneNo = Common.Common.GetCountryPhoneCode(MFTCCardInfo.CardUserCountry) + "" + MFTCCardInfo.CardUserTel;
                    smsApiServices.SendSMS(receiverPhoneNo, message);

                }
                else
                {
                    return false;
                }
                return true;
            }
            return false;
        }
        internal string GetNewAdminMFTCTopUpReceiptNumber()
        {

            var val = "Ad-Ctu-MF" + Common.Common.GenerateRandomDigit(5);

            while (dbContext.FaxingCardTransaction.Where(x => x.ReceiptNumber == val).Count() > 0)
            {

                val = "Ad-Ctu-MF" + Common.Common.GenerateRandomDigit(5);

            }
            return val;
        }
    }

    public class DropDownMFTCCardsViewModel
    {
        public int Id { get; set; }
        public string MFTCCardNumber { get; set; }
    }





}

