using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using Stripe;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SMerchantAutoPayment
    {

        FAXER.PORTAL.DB.FAXEREntities dbContext = null;
        public SMerchantAutoPayment()
        {

            dbContext = new DB.FAXEREntities();
        }

        public DB.KiiPayBusinessLocalTransaction MerchantAutoPayment()
        {


            var data = dbContext.FaxerMerchantPaymentInformation.Where(x => x.EnableAutoPayment == true).ToList();

            foreach (var AutoPaymentInformation in data)
            {

                var IsVaildAutoPayment = IsValidAutoPayment(AutoPaymentInformation);

                var currentdate = DateTime.Now.Date;
                if (IsVaildAutoPayment)
                {

                    var ValidDayOfAutoPayment = IsValidDayOfAutoPayment(AutoPaymentInformation);

                    if (ValidDayOfAutoPayment)
                    {

                        //Calculate Fees 

                        string FaxingCountryCode = AutoPaymentInformation.SenderInformation.Country;
                        string ReceivingCountryCode = AutoPaymentInformation.KiiPayBusinessInformation.BusinessOperationCountryCode;

                        decimal exchangeRate = 0, faxingFee = 0;
                        var exchangeRateObj = dbContext.ExchangeRate.Where(x => x.CountryCode1 == FaxingCountryCode && x.CountryCode2 == ReceivingCountryCode).FirstOrDefault();
                        if (exchangeRateObj == null)
                        {
                            var exchangeRateobj2 = dbContext.ExchangeRate.Where(x => x.CountryCode1 == ReceivingCountryCode && x.CountryCode2 == FaxingCountryCode).FirstOrDefault();
                            if (exchangeRateobj2 != null)
                            {
                                exchangeRateObj = exchangeRateobj2;
                                exchangeRate = Math.Round(1 / exchangeRateObj.CountryRate1, 6, MidpointRounding.AwayFromZero);
                            }

                        }
                        else
                        {
                            exchangeRate = exchangeRateObj.CountryRate1;
                        }
                        if (ReceivingCountryCode == FaxingCountryCode)
                        {

                            exchangeRate = 1m;

                        }

                        if (exchangeRate > 0)
                        {
                            var feeSummary = Services.SEstimateFee.CalculateFaxingFee(AutoPaymentInformation.PaymentAmount, true, false, exchangeRate, Services.SEstimateFee.GetFaxingCommision(FaxingCountryCode));

                            string PaymentReference = AutoPaymentInformation.PaymentRefrence;


                            var savedCreditDebitCardDetails = dbContext.SavedCard.Where(x => x.UserId == AutoPaymentInformation.SenderInformationId).FirstOrDefault();
                            #region  Strip portion
                            StripeConfiguration.SetApiKey("sk_test_OJiTq50I3SNoO3tL3bOJBhTy");
                            //Stripe.SourceCard card = new SourceCard()
                            //{
                            //    //Number = savedCreditDebitCardDetails.Num.Decrypt(),
                            //    Number = "5555555555554444",
                            //    ExpirationYear = int.Parse(savedCreditDebitCardDetails.EYear.Decrypt()),
                            //    ExpirationMonth = int.Parse(savedCreditDebitCardDetails.EMonth.Decrypt()),
                            //    Cvc = savedCreditDebitCardDetails.ClientCode.Decrypt(),
                            //    Name = savedCreditDebitCardDetails.CardName.Decrypt(),
                            //    AddressCity = AutoPaymentInformation.FaxerInformation.City,
                            //    AddressCountry = AutoPaymentInformation.FaxerInformation.Country,
                            //    AddressLine1 = AutoPaymentInformation.FaxerInformation.Address1,
                            //    AddressState = AutoPaymentInformation.FaxerInformation.State,
                            //    AddressZip = AutoPaymentInformation.FaxerInformation.PostalCode,
                            //};

                            //Sample  Credit card

                            //var stripeTokenCreateOptions = new StripeTokenCreateOptions
                            //{
                            //    Card = new StripeCreditCardOptions
                            //    {
                            //        Number = "4242424242424242",
                            //        ExpirationMonth = 12,
                            //        ExpirationYear = 20,
                            //        Cvc = "123",
                            //        Name = "John Appleseed"
                            //    }
                            //};

                            var stripeTokenCreateOptions = new StripeTokenCreateOptions
                            {
                                Card = new StripeCreditCardOptions
                                {
                                    Number = savedCreditDebitCardDetails.Num.Decrypt(),
                                    ExpirationMonth = int.Parse(savedCreditDebitCardDetails.EYear.Decrypt()),
                                    ExpirationYear = int.Parse(savedCreditDebitCardDetails.EMonth.Decrypt()),
                                    Cvc = savedCreditDebitCardDetails.ClientCode.Decrypt(),
                                    Name = savedCreditDebitCardDetails.CardName.Decrypt()
                                }
                            };

                            string token = "";
                            var tokenService = new StripeTokenService();

                            StripeResponse stripeResponse = new StripeResponse();
                            try
                            {
                                var stripeToken = tokenService.Create(stripeTokenCreateOptions);

                                token = stripeToken.Id;
                            }
                            catch (Exception ex)
                            {
                                Log.Write("Merchant Auto Payment Exception : " + ex.Message);
                            }
                            if (string.IsNullOrEmpty(token))
                            {
                                var chargeOptions = new StripeChargeCreateOptions()
                                {
                                    Amount = (Int32)feeSummary.TotalAmount * 100,
                                    Currency = Common.Common.GetCountryCurrency(AutoPaymentInformation.SenderInformation.Country),
                                    Description = "Charge for " + savedCreditDebitCardDetails.CardName.Decrypt(),
                                    //SourceTokenOrExistingSourceId = "tok_mastercard",// obtained with Stripe.js
                                    SourceTokenOrExistingSourceId = token
                                };
                                var chargeService = new StripeChargeService();
                                StripeCharge charge = chargeService.Create(chargeOptions);
                                #endregion
                                SFaxerMerchantPaymentInformation merchantPaymentInformation = new SFaxerMerchantPaymentInformation();
                                string ReceiptNumber = merchantPaymentInformation.GetNewPayGoodsandServicesReceipt();
                                DB.SenderKiiPayBusinessPaymentTransaction faxerMerchantPaymentTransaction = new DB.SenderKiiPayBusinessPaymentTransaction()
                                {
                                    SenderKiiPayBusinessPaymentInformationId = AutoPaymentInformation.Id,
                                    ExchangeRate = feeSummary.ExchangeRate,
                                    FaxingFee = feeSummary.FaxingFee,
                                    ReceivingAmount = feeSummary.ReceivingAmount,
                                    PaymentAmount = feeSummary.TotalAmount,
                                    IsAutoPaymentTransaction = true,
                                    PaymentDate = DateTime.Now,
                                    PaymentMethod = "PM002",
                                    PaymentReference = PaymentReference,
                                    ReceiptNumber = ReceiptNumber,
                                };

                                var result = dbContext.FaxerMerchantPaymentTransaction.Add(faxerMerchantPaymentTransaction);
                                dbContext.SaveChanges();


                                // Merchant Auto Payment Transaction Log 

                                MerchantAutoPaymentTransactionLog merchantAutoPaymentTransactionLog = new MerchantAutoPaymentTransactionLog()
                                {
                                    KiiPayBusinessInformationId = AutoPaymentInformation.KiiPayBusinessInformation.Id,
                                    FaxerId = AutoPaymentInformation.SenderInformation.Id,
                                    PaymentAmount = feeSummary.TotalAmount,
                                    TransactionDate = DateTime.Now

                                };

                                dbContext.MerchantAutoPaymentTransactionLog.Add(merchantAutoPaymentTransactionLog);
                                dbContext.SaveChanges();
                                //End


                                // Increase the MFBC card Balance of Merchant 
                                var MFBCCardDetails = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == AutoPaymentInformation.KiiPayBusinessInformationId
                                                                                           && x.CardStatus != CardStatus.IsDeleted && x.CardStatus != CardStatus.IsRefunded).FirstOrDefault();

                                MFBCCardDetails.CurrentBalance += feeSummary.ReceivingAmount;
                                // End 

                                // Send Email for Confirmation Of Merchant Payment 
                                var baseUrl = "http://moneyfex.com/";
                                MailCommon mail = new MailCommon();
                                string body = "";

                                string FaxerName = AutoPaymentInformation.SenderInformation.FirstName + " " + AutoPaymentInformation.SenderInformation.MiddleName + "" + AutoPaymentInformation.SenderInformation.LastName;
                                string FaxerEmail = AutoPaymentInformation.SenderInformation.Email;
                                var BusinessMerchantDetails = AutoPaymentInformation.KiiPayBusinessInformation;
                                string BusinessEmail = AutoPaymentInformation.KiiPayBusinessInformation.Email;
                                string PayForGoodsAbroad = baseUrl + "/PayForGoodsAndServicesAbroad/MerchantDetails?MerchantACNumber=" + BusinessMerchantDetails.BusinessMobileNo;
                                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfMerchantPaymentWithReceiptFaxer?FaxerName="
                                    + FaxerName + "&MerchantBusinessName=" + BusinessMerchantDetails.BusinessName + "&PayForGoodsAbroad=" + PayForGoodsAbroad);

                                // Generate a receipt PDF

                                string fee = feeSummary.FaxingFee.ToString();
                                string AmountPaid = feeSummary.TotalAmount.ToString();
                                string AmountReceived = feeSummary.ReceivingAmount.ToString();
                                string transactionDate = result.PaymentDate.ToString("dd/MM/yyyy");
                                string transactionTime = result.PaymentDate.ToString("HH:mm");
                                string FaxerCurrency = Common.Common.GetCountryCurrency(AutoPaymentInformation.SenderInformation.Country);
                                string ReceiverCurremcy = Common.Common.GetCountryCurrency(BusinessMerchantDetails.BusinessOperationCountryCode);
                                string ReceiverPhonecode = Common.Common.GetCountryPhoneCode(BusinessMerchantDetails.BusinessOperationCountryCode);

                                var ReceiptURL = baseUrl + "/EmailTemplate/PayGoodsAndServicesFaxerReceipt?ReceiptNumber=" + ReceiptNumber +
                                     "&Date=" + transactionDate + "&Time=" + transactionTime + "&FaxerFullName=" + FaxerName + "&BusinessMerchantName=" + BusinessMerchantDetails.BusinessName
                                     + "&BusinessMFCode=" + BusinessMerchantDetails.BusinessMobileNo + "&AmountPaid=" + AmountPaid + " " + FaxerCurrency + "&ExchangeRate=" + exchangeRate +
                                     "&AmountInLocalCurrency=" + AmountReceived + " " + ReceiverCurremcy + "&Fee=" + fee + " " + FaxerCurrency;

                                var receipt = Common.Common.GetPdf(ReceiptURL);

                                // receipt end 
                                // send Email With receipt attachment 

                                mail.SendMail(FaxerEmail, "Confirmation of Payment to a Merchant", body, receipt);

                                // End 


                                #region Send  Business Payment SMS 

                                SmsApi smsApiServices = new SmsApi();

                                string FaxingCurrencySymbol = Common.Common.GetCurrencySymbol(AutoPaymentInformation.SenderInformation.Country);
                                string message = smsApiServices.GetBusinessPaymentMessage(FaxerName, BusinessMerchantDetails.BusinessMobileNo, BusinessMerchantDetails.BusinessName,
                                                                                   FaxingCurrencySymbol + AmountPaid, faxerMerchantPaymentTransaction.PaymentReference);

                                string PhoneNo = Common.Common.GetCountryPhoneCode(AutoPaymentInformation.SenderInformation.Country) + "" + AutoPaymentInformation.SenderInformation.PhoneNumber;
                                smsApiServices.SendSMS(PhoneNo, message);

                                string ReceiverPhoneNo = Common.Common.GetCountryPhoneCode(AutoPaymentInformation.KiiPayBusinessInformation.BusinessOperationCountryCode) + "" + AutoPaymentInformation.KiiPayBusinessInformation.PhoneNumber;
                                smsApiServices.SendSMS(ReceiverPhoneNo, message);
                                 
                                #endregion

                                string body2 = "";
                                //Send Email for Confirmation of Payment to a Merchant 
                                body2 = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfMerchantPaymentMerchant?BusinessMerchantName="
                                    + BusinessMerchantDetails.BusinessName + "&FaxerName=" + FaxerName);
                                mail.SendMail(BusinessEmail, "Confirmation of Payment to a Merchant", body2);

                                // End 
                            }
                        }
                    }

                }

            }

            return null;

        }

        public bool IsValidDayOfAutoPayment(DB.SenderKiiPayBusinessPaymentInformation model)
        {


            var currentDate = DateTime.Now.Date;
            if (model.AutoPaymentFrequency == DB.AutoPaymentFrequency.Weekly)
            {

                // Find the day of the week for  which the day value has been stored as a frequency detials 
                // i.e 0 = Sunday and 6 = Saturday  // Single Digit is stored for week
                var frequencyDetails = int.Parse(model.FrequencyDetails);
                var day = (DayOfWeek)frequencyDetails;
                if (day == currentDate.DayOfWeek)
                {

                    return true;
                }
                else
                {

                    return false;
                }

            }

            if (model.AutoPaymentFrequency == DB.AutoPaymentFrequency.Monthly)
            {
                // Find the day of the week for  which the day value has been stored as a frequency detials 
                // i.e 01 = firstday of the month //Double Digit is stored for month
                var day = int.Parse(model.FrequencyDetails);

                if (day == currentDate.Day)
                {

                    return true;
                }
                else
                {

                    return false;
                }
            }
            if (model.AutoPaymentFrequency == DB.AutoPaymentFrequency.Yearly)
            {
                // Find the day of the week for  which the day value has been stored as a frequency detials 
                // i.e 0102  : 01= January , 02 = Second Day //Four Digit is stored for year
                int Day = 0;
                int Month = 0;
                var frequencyDetails = model.FrequencyDetails;
                //First Two digit is of Month
                Month = int.Parse(frequencyDetails.Substring(0, 2));
                // Last Two digit is of Day
                Day = int.Parse(frequencyDetails.Substring(2, 2));

                if (currentDate.Month == Month && currentDate.Day == Day)
                {

                    return true;
                }
                else
                {

                    return false;
                }

            }
            return false;
        }



        public bool IsValidAutoPayment(DB.SenderKiiPayBusinessPaymentInformation model)
        {

            DateTime currentDate = DateTime.Now.Date;
            DateTime StartedTransactionDate = new System.DateTime();

            var Day = 0;
            var gannuParneDays = 0.0;
            if (gannuParneDays == 0 && model.AutoPaymentFrequency != DB.AutoPaymentFrequency.NoLimitSet)
            {
                switch (model.AutoPaymentFrequency)
                {
                    case DB.AutoPaymentFrequency.Weekly:
                        Day = (int)currentDate.DayOfWeek;//2
                                                         //hamile monday lai firstday manchau
                                                         //day starts =1=monday
                                                         //enum starts=0=sunday;
                        gannuParneDays = Day;
                        if (Day == 0)
                        {
                            gannuParneDays = 7;
                        }


                        break;

                    case DB.AutoPaymentFrequency.Monthly:
                        Day = currentDate.Day;
                        gannuParneDays = Day;

                        break;
                    case DB.AutoPaymentFrequency.Yearly:
                        Day = currentDate.DayOfYear;
                        gannuParneDays = Day;

                        break;
                    default:
                        break;
                }
            }
            StartedTransactionDate = currentDate.AddDays(-(gannuParneDays));

            var result = dbContext.FaxerMerchantPaymentTransaction.Where(x => x.SenderKiiPayBusinessPaymentInformationId == model.Id && DbFunctions.TruncateTime(x.PaymentDate) >= DbFunctions.TruncateTime(StartedTransactionDate)
                                                                         && x.IsAutoPaymentTransaction == true).Count();
            if (result > 0)
            {

                return false;
            }
            else
            {

                return true;
            }

        }

    }
}