using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Controllers;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class FormPortalNonCardFaxingServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();

        public FormPortalNonCardFaxingViewModel getFaxerInfo(string accountNo)
        {
            AdminResult adminResult = new AdminResult();
            if (string.IsNullOrEmpty(accountNo))
            {
                return null;
            }
            FormPortalNonCardFaxingViewModel vm = new FormPortalNonCardFaxingViewModel();
            vm.PortalNonCardFaxerDetails = (from c in dbContext.FaxerInformation.Where(x => x.AccountNo == accountNo).ToList()
                                            join d in dbContext.Country on c.Country equals d.CountryCode
                                            select new FormPortalNonCardFaxerDetailsViewModel()
                                            {
                                                faxerId = c.Id,
                                                faxerFirstName = c.FirstName,
                                                faxerMiddleName = c.MiddleName,
                                                faxerLastName = c.LastName,
                                                faxerIDCardNumber = c.IdCardNumber,
                                                faxerIDCardType = c.IdCardType,
                                                faxerIDCardExpDate = c.IdCardExpiringDate.ToString("dd-MM-yyyy"),
                                                faxerIDCardIssuingCountry = c.IssuingCountry,
                                                faxerAddress = c.Address1,
                                                faxerCity = c.City,
                                                faxerCountry = d.CountryName,
                                                faxerCountryCode = c.Country,
                                                faxerPostalCode = c.PostalCode,
                                                faxerTelephone = c.PhoneNumber,
                                                faxerEmailAddress = c.Email,

                                            }).FirstOrDefault();

            if (vm.PortalNonCardFaxerDetails == null) {

                return null;

            }
            vm.isCardAvailable = true;
            vm.faxerAccountNo = accountNo;
            vm.SendingCurrency = CommonService.getCurrency(vm.PortalNonCardFaxerDetails.faxerCountryCode);
            vm.SendingCurSymbol = CommonService.getCurrencySymbol(vm.PortalNonCardFaxerDetails.faxerCountryCode);
            vm.PortalNonCardFaxingCalculations.sendingCurrencySymbol = CommonService.getCurrencySymbol(vm.PortalNonCardFaxerDetails.faxerCountryCode);
            vm.PortalNonCardFaxingCalculations.sendingCurrency = CommonService.getCurrency(vm.PortalNonCardFaxerDetails.faxerCountryCode);

            return vm;
        }

        public List<ReceiversDropDownViewModel> getReceiversList(string accountNo)
        {
            var result = (from c in dbContext.FaxerInformation.Where(x => x.AccountNo == accountNo).ToList()
                          join d in dbContext.ReceiversDetails on c.Id equals d.FaxerID
                          select new ReceiversDropDownViewModel()
                          {
                              ReceiverId = d.Id,
                              ReceiverName = d.FirstName + " " + d.MiddleName + " " + d.LastName
                          }).ToList();
            return result;
        }
        public CalculateFaxingFeeVm getFaxingCalculations(decimal faxingAmount, string faxingCountryCode, string receivingCountryCode, decimal receivingAmount)
        {
            CalculateFaxingFeeVm fvm = new CalculateFaxingFeeVm();
            var feeSummary = new EstimateFaxingFeeSummary();
            decimal exchangeRate = 0;
            var exchangeRateObj = dbContext.ExchangeRate.Where(x => x.CountryCode1 == faxingCountryCode && x.CountryCode2 == receivingCountryCode).FirstOrDefault();
            if (exchangeRateObj == null)
            {
                var exchangeRateObj2 = dbContext.ExchangeRate.Where(x => x.CountryCode1 == receivingCountryCode && x.CountryCode2 == faxingCountryCode).FirstOrDefault();
                if (exchangeRateObj2 != null)
                {
                    exchangeRateObj = exchangeRateObj2;
                    exchangeRateObj.CountryRate1 = Math.Round(1 / exchangeRateObj2.CountryRate1, 6, MidpointRounding.AwayFromZero);
                }
            }
            exchangeRate = exchangeRateObj.CountryRate1;
            feeSummary = SEstimateFee.CalculateFaxingFee(((receivingAmount > 0) ? receivingAmount : faxingAmount), true, receivingAmount > 0, exchangeRate, SEstimateFee.GetFaxingCommision(faxingCountryCode) ); //+ 0.01m
            fvm.ExchangeRate = feeSummary.ExchangeRate;
            fvm.TopUpFees = feeSummary.FaxingFee;
            fvm.TopUpAmount = feeSummary.FaxingAmount;
            fvm.ReceivingAmount = feeSummary.ReceivingAmount;
            fvm.TotalAmountIncludingFees = feeSummary.FaxingFee + feeSummary.FaxingAmount;
            fvm.ReceivingCurrency = CommonService.getCurrency(receivingCountryCode);
            fvm.ReceivingCurrencySymbol = CommonService.getCurrencySymbol(receivingCountryCode);
            fvm.SendingCurrency = CommonService.getCurrency(faxingCountryCode);
            fvm.SendingCurrencySymbol = CommonService.getCurrencySymbol(faxingCountryCode);
            return fvm;

        }

        public string[] exchangeRateDetails(string sendingCC, string receivingCC)
        {
            decimal exchangeRate = 0;
            string[] hello = new string[3];
            var data = dbContext.ExchangeRate.Where(x => x.CountryCode1 == sendingCC && x.CountryCode2 == receivingCC).FirstOrDefault();
            if (data != null)
            {
                exchangeRate = data.CountryRate1;
            }
            else if (data == null)
            {
                data = dbContext.ExchangeRate.Where(x => x.CountryCode1 == receivingCC && x.CountryCode2 == sendingCC).FirstOrDefault();
                if (data != null)
                {
                    exchangeRate = data.CountryRate1;
                }
                else if (sendingCC == receivingCC)
                {

                    exchangeRate = 1;
                }
                else {

                    exchangeRate = 0;
                }
            }
           

            else
            {
                return null;
            }

            string recCountryCurrency = CommonService.getCurrencyCodeFromCountry(receivingCC);
            string recCountryCurSymbol = CommonService.getCurrencySymbol(receivingCC);

            hello[0] = exchangeRate.ToString();
            hello[1] = recCountryCurrency;
            hello[2] = recCountryCurSymbol;


            return hello;
        }
        public FormPortalNonCardReceiverDetailsViewModel getReceiverDetails(int recId)
        {
            if (recId == 0)
            {
                return null;
            }
            FormPortalNonCardReceiverDetailsViewModel details = new FormPortalNonCardReceiverDetailsViewModel();
            details = (from c in dbContext.ReceiversDetails.Where(x => x.Id == recId).ToList()
                       select new FormPortalNonCardReceiverDetailsViewModel
                       {
                           receiverId = c.Id,
                           receiverFirstName = c.FirstName,
                           receiverMiddleName = c.MiddleName,
                           receiverLastName = c.LastName,
                           receiverCity = c.City,
                           receiverCountry = c.Country,
                           receiverTelephone = c.PhoneNumber,
                           receiverEmailAddress = c.EmailAddress,
                           receiversCurrency = CommonService.getCurrency(c.Country),
                           receiversCurSymbol = CommonService.getCurrencySymbol(c.Country)
                       }).FirstOrDefault();
            return details;
        }

        public bool saveNonCardFaxingDetails(FormPortalNonCardFaxingViewModel model)
        {
            if (model.PortalNonCardFaxerDetails != null && model.PortalNonCardFaxingCalculations != null && model.PortalNonCardReceiverDetails != null)
            {
                var check = dbContext.ReceiversDetails.Where(x => (x.Id == model.PortalNonCardReceiverDetails.receiverId || x.EmailAddress == model.PortalNonCardReceiverDetails.receiverEmailAddress) && x.FaxerID == model.PortalNonCardFaxerDetails.faxerId).FirstOrDefault();
                if (check == null)
                {
                    //inserting data into ReceiversDetails table
                    var data = new ReceiversDetails()
                    {
                        FaxerID = model.PortalNonCardFaxerDetails.faxerId,
                        FirstName = model.PortalNonCardReceiverDetails.receiverFirstName,
                        MiddleName = model.PortalNonCardReceiverDetails.receiverMiddleName,
                        LastName = model.PortalNonCardReceiverDetails.receiverLastName,
                        City = model.PortalNonCardReceiverDetails.receiverCity,
                        Country = model.PortalNonCardReceiverDetails.receiverCountry,
                        PhoneNumber = model.PortalNonCardReceiverDetails.receiverTelephone,
                        EmailAddress = model.PortalNonCardReceiverDetails.receiverEmailAddress,
                        CreatedDate = DateTime.Now,
                    };
                    dbContext.ReceiversDetails.Add(data);
                    dbContext.SaveChanges();
                }


                //inserting data into FaxingNonCardTransaction
                var receiverInfo = dbContext.ReceiversDetails.Where(x => x.EmailAddress == model.PortalNonCardReceiverDetails.receiverEmailAddress).FirstOrDefault();
                FAXER.PORTAL.Services.SFaxingNonCardTransaction getMFCN = new SFaxingNonCardTransaction();
                var MFCN = getMFCN.GetNewMFCNToSave();
                var receiptNumber = GetNewAdminNonCardTransferReceiptNumber();

                var data1 = new FaxingNonCardTransaction()
                {
                    NonCardRecieverId = receiverInfo.Id,
                    ReceivingAmount = model.PortalNonCardFaxingCalculations.receivingAmount,
                    TransactionDate = DateTime.Now,
                    FaxingAmount = model.PortalNonCardFaxingCalculations.faxingAmount,
                    FaxingFee = model.PortalNonCardFaxingCalculations.faxingFee,
                    ExchangeRate = model.PortalNonCardFaxingCalculations.exchangeRate,
                    FaxingMethod = model.isCardAvailable == true ? "Card" : "BankToBank",
                    MFCN = MFCN,
                    ReceiptNumber = receiptNumber,
                    OperatingUserType = OperatingUserType.Admin,
                    UserId = Common.StaffSession.LoggedStaff.StaffId,
                    PaymentMethod = model.isCardAvailable == true ? "PM001" : "PM003",
                    stripe_ChargeId = model.StripeChargeId
                };
                var save = dbContext.FaxingNonCardTransaction.Add(data1);
                dbContext.SaveChanges();

                // Send Email to Faxer
                MailCommon mail = new MailCommon();

                var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                string FaxerName = model.PortalNonCardFaxerDetails.faxerFirstName + " " + model.PortalNonCardFaxerDetails.faxerMiddleName
                    + " " + model.PortalNonCardFaxerDetails.faxerLastName;
                string FaxerEmail = model.PortalNonCardFaxerDetails.faxerEmailAddress;
                string FaxerCountry = model.PortalNonCardFaxerDetails.faxerCountry; // CommonService.getCountryNameFromCode(model.PortalNonCardFaxerDetails.faxerCountry);
                string body = "";
                string ReceiverName = model.PortalNonCardReceiverDetails.receiverFirstName + " " + model.PortalNonCardReceiverDetails.receiverMiddleName +
                    " " + model.PortalNonCardReceiverDetails.receiverLastName;
                string ReceiverCity = model.PortalNonCardReceiverDetails.receiverCity;
                string RegisterMFTCLink = baseUrl + "/TopUpRegestration/UserContactDetails";
                string TotalFaxedAmount = (model.PortalNonCardFaxingCalculations.faxingAmount + model.PortalNonCardFaxingCalculations.faxingFee).ToString();
                string FaxerCurrency = Common.Common.GetCountryCurrency(model.PortalNonCardFaxerDetails.faxerCountryCode);
                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfMoneyFaxedWithReceipt?FaxerName=" + FaxerName +
                    "&ReceiverName=" + ReceiverName + "&ReceiverCity=" + ReceiverCity
                    + "&MFCN=" + MFCN + "&FaxAmount=" + TotalFaxedAmount  + " " + FaxerCurrency + "&RegisterMFTC=" + RegisterMFTCLink + "&FaxerCountry=" + FaxerCountry );

                //mail.SendMail(FaxerEmail, "Confirmation of Money Faxed with MFCN", body);
                string CountryCode = "+" + Common.Common.GetCountryPhoneCode(model.PortalNonCardReceiverDetails.receiverCountry);


                
                string ReceiverCurrency = Common.Common.GetCountryCurrency(model.PortalNonCardReceiverDetails.receiverCountry);
                string URL = baseUrl + "/EmailTemplate/AdminNonCardMoneyTransfer?MFReceiptNumber=" + save.ReceiptNumber +
                    "&TransactionDate=" + save.TransactionDate.ToString("dd/MM/yyy") + "&TransactionTime=" + save.TransactionDate.ToString("HH:mm")
                      + "&FaxerFullName=" + FaxerName +
                    "&MFCN=" + save.MFCN + "&ReceiverFullName=" + ReceiverName
                    + "&Telephone=" + CountryCode + " " + model.PortalNonCardReceiverDetails.receiverTelephone +
                    "&StaffName=" + Common.StaffSession.LoggedStaff.FirstName + " " + Common.StaffSession.LoggedStaff.MiddleName + " " + Common.StaffSession.LoggedStaff.LastName
                    + "&StaffCode=" + Common.StaffSession.LoggedStaff.StaffMFSCode + "&AmountSent=" + model.PortalNonCardFaxingCalculations.faxingAmount 
                    + "&ExchangeRate=" + model.PortalNonCardFaxingCalculations.exchangeRate + "&Fee=" + model.PortalNonCardFaxingCalculations.faxingFee + 
                    "&AmountReceived=" + model.PortalNonCardFaxingCalculations.receivingAmount  + "&TotalAmountSentAndFee=" 
                    + TotalFaxedAmount + "&SendingCurrency=" + FaxerCurrency + "&ReceivingCurrency=" + ReceiverCurrency 
                    + "&PaymentType=" + Common.StaffSession.LoggedStaff.LoginCode 
                    + "&SenderPhoneNo=" + Common.Common.GetCountryPhoneCode(model.PortalNonCardFaxerDetails.faxerCountryCode) + " " +  model.PortalNonCardFaxerDetails.faxerTelephone;

                var output = Common.Common.GetPdf(URL);

                mail.SendMail(FaxerEmail, "Confirmation of Money Faxed with MFCN", body, output);
                //mail.SendMail("anankarki97@gmail.com", "Confirmation of Money Faxed with MFCN", body, output);

                // Sms Function  
                SmsApi smsApiServices = new SmsApi();
                string senderName = model.PortalNonCardFaxerDetails.faxerFirstName + " " + model.PortalNonCardFaxerDetails.faxerMiddleName + " " + model.PortalNonCardFaxerDetails.faxerLastName;
                string virtualAccounntNo = model.faxerAccountNo;

                string Fee = Common.Common.GetCurrencySymbol(Common.FaxerSession.FaxerCountry) + model.PortalNonCardFaxingCalculations.faxingFee;
                string amount = Common.Common.GetCurrencySymbol(model.PortalNonCardFaxerDetails.faxerCountryCode) + data1.FaxingAmount.ToString();
                string receivingAmount = Common.Common.GetCurrencySymbol(model.PortalNonCardReceiverDetails.receiverCountry)
                                             + model.PortalNonCardFaxingCalculations.receivingAmount;
                string message = smsApiServices.GetCashToCashTransferMessage(senderName, data1.MFCN, amount , Fee ,receivingAmount);
                        //string message = smsApiServices.GetVirtualAccountDepositMessage(senderName, virtualAccounntNo, amount);
                        string phoneNumber = Common.Common.GetCountryPhoneCode(model.PortalNonCardFaxerDetails.faxerCountryCode) + model.PortalNonCardFaxerDetails.faxerTelephone;
                smsApiServices.SendSMS(phoneNumber, message);

                string receiverPhoneNo = Common.Common.GetCountryPhoneCode(model.PortalNonCardReceiverDetails.receiverCountry) + model.PortalNonCardReceiverDetails.receiverTelephone;

                smsApiServices.SendSMS(receiverPhoneNo, message);

                if (model.isCardAvailable == true)
                {
                    string cardNum = model.PortalNonCardFaxingPaymentDetails.cardNumber.Encrypt();
                    var savedCard = dbContext.SavedCard.Where(x => x.Num == cardNum).FirstOrDefault();

                    var cardData = new CardTopUpCreditDebitInformation()
                    {
                        NonCardTransactionId = save.Id,
                        NameOnCard = model.PortalNonCardFaxingPaymentDetails.nameOnCard,
                        CardNumber = "xxxx-xxxx-xxxx-" + model.PortalNonCardFaxingPaymentDetails.cardNumber.Right(4),
                        ExpiryDate = model.PortalNonCardFaxingPaymentDetails.endMonth + "/" + model.PortalNonCardFaxingPaymentDetails.endYear,
                        IsSavedCard = savedCard == null ? false : true
                    };
                    dbContext.CardTopUpCreditDebitInformation.Add(cardData);
                    dbContext.SaveChanges();
                }

                return true;

            }
            return false;
        }

        internal string GetNewAdminNonCardTransferReceiptNumber()
        {
            //this code should be unique and random with 8 digit length
            var val = "Ad-Np-MF" + Common.Common.GenerateRandomDigit(5);

            while (dbContext.UserCardWithdrawl.Where(x => x.ReceiptNumber == val).Count() > 0)
            {
                val = "Ad-Np-MF" + Common.Common.GenerateRandomDigit(5);
            }
            return val;
        }
    }
    public class ReceiversDropDownViewModel
    {
        public int ReceiverId { get; set; }
        public string ReceiverName { get; set; }
    }
}