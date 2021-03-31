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
    public class FormFaxerMerchantPaymentsServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();

        public FormFaxerMerchantPaymentsViewModel getDetails(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber))
            {
                return null;
            }
            FormFaxerMerchantPaymentsViewModel vm = new FormFaxerMerchantPaymentsViewModel();
            vm.FormFaxerDetails = (from c in dbContext.FaxerInformation.Where(x => x.AccountNo == accountNumber).ToList()
                                   select new FormFaxerDetailsViewModel()
                                   {
                                       FaxerId = c.Id,
                                       FaxerFirstName = c.FirstName,
                                       FaxerMiddleName = c.MiddleName,
                                       FaxerLastName = c.LastName,
                                       FaxerIDCardNo = c.IdCardNumber,
                                       FaxerIDCardType = c.IdCardType,
                                       FaxerIDCardExpDate = c.IdCardExpiringDate.ToFormatedString(),
                                       FaxerIDCardIssuingCountry = CommonService.getCountryNameFromCode(c.IssuingCountry),
                                       FaxerAddress = c.Address1,
                                       FaxerCity = c.City,
                                       FaxerCountry = CommonService.getCountryNameFromCode(c.Country),
                                       FaxerCountryCode = c.Country,
                                       FaxerTelephone = c.PhoneNumber,
                                       FaxerEmail = c.Email
                                   }).FirstOrDefault();
            if (vm.FormFaxerDetails != null)
            {
                vm.FaxerAccountNo = accountNumber;
                vm.FormPaymentDetails.SenderCurrency = CommonService.getCurrency(vm.FormFaxerDetails.FaxerCountryCode);
                vm.FormPaymentDetails.SenderCurrencySymbol = CommonService.getCurrencySymbol(vm.FormFaxerDetails.FaxerCountryCode);

                vm.isCardAvailable = true;
                vm.FormPaymentDetails.PaymentIncludeFee = true;
                vm.BankPayment = false;
            }
            return vm;

        }

        public FormBusinessDetailsViewModel getBusinessDetails(string MFSCode)
        {
            if (string.IsNullOrEmpty(MFSCode))
            {
                return null;
            }
            FormBusinessDetailsViewModel businessDetails = new FormBusinessDetailsViewModel();
            businessDetails = (from c in dbContext.KiiPayBusinessInformation.Where(x => x.BusinessMobileNo == MFSCode).ToList()
                               select new FormBusinessDetailsViewModel()
                               {
                                   KiiPayBusinessInformationId = c.Id,
                                   BusinessMobileNo = c.BusinessMobileNo,
                                   BusinessMerchantName = c.BusinessName,
                                   BusinessAccountNo = c.BusinessMobileNo,
                                   BusinessCity = c.BusinessOperationCity,
                                   BusinessCountry = CommonService.getCountryNameFromCode(c.BusinessOperationCountryCode),
                                   BusinessCC = c.BusinessOperationCountryCode
                               }).FirstOrDefault();
            return businessDetails;
        }

        public CardStatus? GetMerchantcardStatus(int KiiPayBusinessInformationId)
        {

            var MerchantInformation = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId).ToList().LastOrDefault();

            return MerchantInformation.CardStatus;

        }
        public DropDownSavedCardsViewModel getCardDetails(int id)
        {
            var result = (from c in dbContext.SavedCard.Where(x => x.Id == id).ToList()
                          select new DropDownSavedCardsViewModel()
                          {
                              CardNameOnCard = c.CardName.Decrypt(),
                              CardNumber = c.Num.Decrypt(),
                              CardEndMonth = Convert.ToInt32(c.EMonth.Decrypt()),
                              CardEndYear = Convert.ToInt32(c.EYear.Decrypt())
                          }).FirstOrDefault();
            return result;



        }


        public List<DropDownSavedCardsViewModel> getSavedCards(int faxerId)
        {
            var result = (from c in dbContext.SavedCard.Where(x => x.UserId == faxerId).ToList()
                          select new DropDownSavedCardsViewModel()
                          {
                              Id = c.Id,
                              CardNameOnCard = c.CardName.Decrypt(),
                              CardNumber = c.Num.Decrypt()
                          }).ToList();
            return result;
        }

        public CalculateFaxingFeeVm getFaxingCalculations(decimal faxingAmount, string faxingCountryCode, string receivingCountryCode, decimal receivingAmount, bool isFaxingFeeIncluded)
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

                    exchangeRateObj = new ExchangeRate();
                    exchangeRateObj.CountryRate1 = Math.Round(1 / exchangeRateObj2.CountryRate1, 6, MidpointRounding.AwayFromZero);
                }
            }
            if (faxingCountryCode == receivingCountryCode)
            {
                exchangeRate = 1;

            }
            else if (exchangeRateObj == null)
            {

                exchangeRate = 0;
            }
            else
            {
                exchangeRate = exchangeRateObj.CountryRate1;
            }
            feeSummary = SEstimateFee.CalculateFaxingFee(((receivingAmount > 0) ? receivingAmount : faxingAmount), isFaxingFeeIncluded, receivingAmount > 0, exchangeRate, SEstimateFee.GetFaxingCommision(faxingCountryCode));
            fvm.ExchangeRate = feeSummary.ExchangeRate;
            fvm.TopUpFees = feeSummary.FaxingFee;
            fvm.TopUpAmount = feeSummary.FaxingAmount;
            fvm.ReceivingAmount = feeSummary.ReceivingAmount;
            fvm.TotalAmountIncludingFees = feeSummary.FaxingFee + feeSummary.FaxingAmount;
            return fvm;

        }

        public bool SaveFaxerMerchantPayments(FormFaxerMerchantPaymentsViewModel model)
        {
            if (model.FormFaxerDetails == null || model.FormBusinessDetails == null || model.FormPaymentDetails == null)
            {
                return false;
            }

            //inserting data into FaxerMerchantPaymentTransaction table
            string paymentMethod = "";
            if (model.BankPayment == true)
            {
                paymentMethod = "PM003";
            }
            else
            {
                paymentMethod = "PM001";
            }
            var paymentInfoData = new SenderKiiPayBusinessPaymentInformation()
            {
                KiiPayBusinessInformationId = model.FormBusinessDetails.KiiPayBusinessInformationId,
                PaymentRefrence = model.PaymentReference,
                SenderInformationId = model.FormFaxerDetails.FaxerId
            };
            int paymentInfoId = 0;
            var checkPaymentInfo = dbContext.FaxerMerchantPaymentInformation.Where(x => x.KiiPayBusinessInformationId == model.FormBusinessDetails.KiiPayBusinessInformationId).FirstOrDefault();
            if (checkPaymentInfo == null)
            {
                var savePaymentInfo = dbContext.FaxerMerchantPaymentInformation.Add(paymentInfoData);
                dbContext.SaveChanges();
                paymentInfoId = savePaymentInfo.Id;
            }
            else if (checkPaymentInfo != null)
            {
                paymentInfoId = checkPaymentInfo.Id;
            }
            string ReceiptNumber = GetNewAdminMerhcantPaymentReceiptNumber();
            var paymentTransactionData = new SenderKiiPayBusinessPaymentTransaction()
            {
                SenderKiiPayBusinessPaymentInformationId = paymentInfoId,
                StaffId = Common.StaffSession.LoggedStaff.StaffId,
                FaxingFee = model.FormPaymentDetails.PaymentFaxingFee,
                ExchangeRate = model.FormPaymentDetails.PaymentExchangeRate,
                ReceivingAmount = model.FormPaymentDetails.PaymentAmountToBeReceived,
                PaymentMethod = paymentMethod,
                PaymentDate = DateTime.Now,
                PaymentAmount = model.FormPaymentDetails.PaymentAmountIncludingFee,
                PaymentReference = model.PaymentReference,
                ReceiptNumber = ReceiptNumber
            };


            var save = dbContext.FaxerMerchantPaymentTransaction.Add(paymentTransactionData);
            dbContext.SaveChanges();

            // Send Email for Confirmation Of Merchant Payment 
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            MailCommon mail = new MailCommon();
            string body = "";

            string FaxerName = model.FormFaxerDetails.FaxerFirstName + " " + model.FormFaxerDetails.FaxerMiddleName + " " + model.FormFaxerDetails.FaxerLastName;
            string FaxerEmail = model.FormFaxerDetails.FaxerEmail;
            //var BusinessMerchantDetails = service.GetBusinessMerchantDetials(obj.BusinessInformationId);

            string PayForGoodsAbroad = baseUrl + "/PayForGoodsAndServicesAbroad/MerchantDetails?MerchantACNumber=" + model.FormBusinessDetails.BusinessMobileNo;
            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfMerchantPaymentWithReceiptFaxer?FaxerName="
                + FaxerName + "&MerchantBusinessName=" + model.FormBusinessDetails.BusinessMerchantName + "&PayForGoodsAbroad=" + PayForGoodsAbroad);

            // Generate a receipt PDF

            string fee = model.FormPaymentDetails.PaymentFaxingFee.ToString();
            string AmountPaid = model.FormPaymentDetails.PaymentTopUpAmount.ToString();
            string AmountReceived = model.FormPaymentDetails.PaymentAmountToBeReceived.ToString();
            string exchangeRate = model.FormPaymentDetails.PaymentExchangeRate.ToString();
            string transactionDate = save.PaymentDate.ToString("dd/MM/yyyy");
            string transactionTime = save.PaymentDate.ToString("HH:mm");
            string FaxerCountryCode = dbContext.FaxerInformation.Where(x => x.Id == model.FormFaxerDetails.FaxerId).Select(x => x.Country).FirstOrDefault();
            string FaxerCurrency = Common.Common.GetCountryCurrency(FaxerCountryCode);
            var businessMerchantDetails = dbContext.KiiPayBusinessInformation.Where(x => x.Id == model.FormBusinessDetails.KiiPayBusinessInformationId).FirstOrDefault();
            string ReceiverCurremcy = Common.Common.GetCountryCurrency(businessMerchantDetails.BusinessOperationCountryCode);
            string BusinessEmail = businessMerchantDetails.Email;
            //string ReceiverPhonecode = Common.Common.GetCountryPhoneCode(BusinessMerchantDetails.CountryCode);
            var ReceiptURL = baseUrl + "/EmailTemplate/AdminPayGoodsAndServicesReceipt?ReceiptNumber=" + save.ReceiptNumber +
                 "&Date=" + transactionDate + "&Time=" + transactionTime + "&FaxerFullName=" + FaxerName + "&BusinessMerchantName=" + model.FormBusinessDetails.BusinessMerchantName
                 + "&BusinessMFCode=" + model.FormBusinessDetails.BusinessMobileNo + "&AmountPaid=" + AmountPaid + "&ExchangeRate=" + exchangeRate +
                 "&AmountInLocalCurrency=" + AmountReceived + "&Fee=" + fee +
                 "&StaffName=" + Common.StaffSession.LoggedStaff.FirstName + " " + Common.StaffSession.LoggedStaff.MiddleName + " " + Common.StaffSession.LoggedStaff.LastName + "&StaffCode=" + Common.StaffSession.LoggedStaff.StaffMFSCode
                 + "&SendingCurrency=" + FaxerCurrency + "&ReceivingCurrency=" + ReceiverCurremcy +
                 "&Faxercountry="  + Common.Common.GetCountryName( FaxerCountryCode) 
                 +  "&BusinessCountry=" + Common.Common.GetCountryName(businessMerchantDetails.BusinessOperationCountryCode) 
                 + "&BusinessCity=" +  businessMerchantDetails.BusinessOperationCity + "&DepositType=" + Common.StaffSession.LoggedStaff.LoginCode;

            var receipt = Common.Common.GetPdf(ReceiptURL);

            // receipt end 
            // send Email With receipt attachment 

            mail.SendMail(FaxerEmail, "Confirmation of Payment to a Merchant", body, receipt);

            // End 
            string body2 = "";
            //Send Email for Confirmation of Payment to a Merchant 
            body2 = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfMerchantPaymentMerchant?BusinessMerchantName="
                + model.FormBusinessDetails.BusinessMerchantName + "&FaxerName=" + FaxerName);
            mail.SendMail(BusinessEmail, "Confirmation of Payment to a Merchant", body2);

            // End

            SmsApi smsApiServices = new SmsApi();
            string senderName = model.FormFaxerDetails.FaxerFirstName + " " + model.FormFaxerDetails.FaxerMiddleName + " " + model.FormFaxerDetails.FaxerLastName;
            string businessAccounntNo = model.FormBusinessDetails.BusinessAccountNo;
            string businessName = model.FormBusinessDetails.BusinessMerchantName;
            string amount = CommonService.getCurrencySymbol(model.FormFaxerDetails.FaxerCountryCode) + model.FormPaymentDetails.PaymentTopUpAmount.ToString();
            string paymentReference = model.PaymentReference;
            string receivingAmount = CommonService.getCurrencySymbol(model.FormBusinessDetails.BusinessCountry)
                                      + model.FormPaymentDetails.PaymentAmountToBeReceived;
            string message = smsApiServices.GetBusinessPaymentMessage(senderName, businessAccounntNo, businessName, amount, paymentReference , receivingAmount);
            string phoneNumber = CommonService.getPhoneCodeFromCountry(model.FormFaxerDetails.FaxerCountryCode) + model.FormFaxerDetails.FaxerTelephone;
            smsApiServices.SendSMS(phoneNumber, message);


            string receiverPhoneNo = Common.Common.GetCountryPhoneCode(model.FormBusinessDetails.BusinessCountry) + "" + businessMerchantDetails.PhoneNumber;
            smsApiServices.SendSMS(receiverPhoneNo, message);

            //inserting data into FaxerMerchantPaymentByStaffCardInfo table
            if (model.BankPayment == false)
            {
                var data1 = new FaxerMerchantPaymentByStaffCardInfo()
                {
                    TransactionId = save.Id,
                    FaxerId = model.FormFaxerDetails.FaxerId,
                    NameOnCard = model.FormCreditCardDetails.CardNameOnCard,
                    CardNumber = "xxxx-xxxx-xxxx-" + model.FormCreditCardDetails.CardNumber.Right(4),
                    ExpiryDate = model.FormCreditCardDetails.CardEndMonth + "/" + model.FormCreditCardDetails.CardEndYear
                };
                dbContext.FaxerMerchantPaymentByStaffCardInfo.Add(data1);
                dbContext.SaveChanges();
            }
            return true;
        }

        public string getCurrency(string countryCode)
        {
            var result = dbContext.Country.Where(x => x.CountryCode == countryCode).FirstOrDefault().Currency;
            return result;
        }

        internal string GetNewAdminMerhcantPaymentReceiptNumber()
        {
            //this code should be unique and random with 8 digit length
            var val = "Ad-Mp-MF" + Common.Common.GenerateRandomDigit(5);

            while (dbContext.UserCardWithdrawl.Where(x => x.ReceiptNumber == val).Count() > 0)
            {
                val = "Ad-Mp-MF" + Common.Common.GenerateRandomDigit(5);
            }
            return val;
        }

    }
    public class DropDownSavedCardsViewModel
    {
        public int Id { get; set; }
        public string CardNameOnCard { get; set; }
        public string CardNumber { get; set; }
        public int CardEndYear { get; set; }
        public int CardEndMonth { get; set; }
    }


}