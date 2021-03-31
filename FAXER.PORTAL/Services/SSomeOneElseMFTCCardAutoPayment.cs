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
    public class SSomeOneElseMFTCCardAutoPayment
    {
        DB.FAXEREntities dbContext = null;
        public SSomeOneElseMFTCCardAutoPayment()
        {
            dbContext = new DB.FAXEREntities();
        }

        public void MFTCCardAutoPayment()
        {


            var data = dbContext.OtherMFTCCardAutoTopUpInformation.Where(x => x.EnableAutoPayment == true).ToList();
            foreach (var AutoPaymentInformation in data)
            {

                var IsValidPayment = IsValidAutoPayment(AutoPaymentInformation);
                if (IsValidPayment)
                {
                    var ValidDayOfAutoPayment = IsValidDayOfAutoPayment(AutoPaymentInformation);

                    if (ValidDayOfAutoPayment)
                    {

                        //Calculate Fees 
                        var FaxerInformation = dbContext.FaxerInformation.Where(x => x.Id == AutoPaymentInformation.FaxerId).FirstOrDefault();
                        string FaxingCountryCode = FaxerInformation.Country;
                        string ReceivingCountryCode = AutoPaymentInformation.MFTCCard.CardUserCountry;

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
                            var feeSummary = Services.SEstimateFee.CalculateFaxingFee(AutoPaymentInformation.AutoPaymentAmount, true, false, exchangeRate, Services.SEstimateFee.GetFaxingCommision(FaxingCountryCode));

                            string PaymentReference = AutoPaymentInformation.TopUpReference;


                            var savedCreditDebitCardDetails = dbContext.SavedCard.Where(x => x.UserId == AutoPaymentInformation.FaxerId).FirstOrDefault();
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

                            var stripeTokenCreateOptions = new StripeTokenCreateOptions
                            {
                                Card = new StripeCreditCardOptions
                                {
                                    Number = "4242424242424242",
                                    ExpirationMonth = 12,
                                    ExpirationYear = 20,
                                    Cvc = "123",
                                    Name = "John Appleseed"
                                }
                            };

                            //var stripeTokenCreateOptions = new StripeTokenCreateOptions
                            //{
                            //    Card = new StripeCreditCardOptions
                            //    {
                            //        Number = savedCreditDebitCardDetails.Num.Decrypt(),
                            //        ExpirationMonth = int.Parse(savedCreditDebitCardDetails.EYear.Decrypt()),
                            //        ExpirationYear = int.Parse(savedCreditDebitCardDetails.EMonth.Decrypt()),
                            //        Cvc = savedCreditDebitCardDetails.ClientCode.Decrypt(),
                            //        Name = savedCreditDebitCardDetails.CardName.Decrypt()
                            //    }
                            //};

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

                                Log.Write("MFTC Card Auto Payment Exception :" + ex.Message);
                            }
                            if (!string.IsNullOrEmpty(token))
                            {
                                var chargeOptions = new StripeChargeCreateOptions()
                                {
                                    Amount = (Int32)feeSummary.TotalAmount * 100,
                                    Currency = Common.Common.GetCountryCurrency(FaxingCountryCode),
                                    Description = "Charge for " + savedCreditDebitCardDetails.CardName.Decrypt(),
                                    //SourceTokenOrExistingSourceId = "tok_mastercard",// obtained with Stripe.js
                                    SourceTokenOrExistingSourceId = token
                                };
                                var chargeService = new StripeChargeService();
                                StripeCharge charge = chargeService.Create(chargeOptions);
                                #endregion
                                SFaxingTopUpCardTransaction service = new SFaxingTopUpCardTransaction();

                                STopUpSomeoneElseCard TopUpSomeoneElseServices = new STopUpSomeoneElseCard();
                                string ReceiptNumber = service.GetNewMFTCCardTopUpReceipt();
                                //transaction history object
                                DB.TopUpSomeoneElseCardTransaction obj = new DB.TopUpSomeoneElseCardTransaction()
                                {
                                    KiiPayPersonalWalletId = AutoPaymentInformation.MFTCCardId,
                                    FaxerId = AutoPaymentInformation.FaxerId,
                                    FaxingAmount = feeSummary.FaxingAmount,
                                    RecievingAmount = feeSummary.ReceivingAmount,
                                    ExchangeRate = feeSummary.ExchangeRate,
                                    FaxingFee = feeSummary.FaxingFee,
                                    ReceiptNumber = ReceiptNumber,
                                    TransactionDate = System.DateTime.Now,
                                    TopUpReference = AutoPaymentInformation.TopUpReference,
                                    PaymentMethod = "PM002",
                                    IsAutoPaymentTransaction = true,
                                };
                                obj = TopUpSomeoneElseServices.SaveTransaction(obj);


                                // MFTC Auto Payment Transaction Log 
                                MFTCAutoTopUpTransactionLog mFTCAutoTopUpTransactionLog = new MFTCAutoTopUpTransactionLog()
                                {
                                    AutoTopUpAmount = AutoPaymentInformation.AutoPaymentAmount,
                                    FaxerID = AutoPaymentInformation.FaxerId,
                                    MFTCAutoTopUpType = MFTCAutoTopUpType.SomeoneElseCard,
                                    MFTCCardID = AutoPaymentInformation.MFTCCardId,
                                    TransactionDate = DateTime.Now
                                };

                                dbContext.MFTCAutoTopUpTransactionLog.Add(mFTCAutoTopUpTransactionLog);
                                dbContext.SaveChanges();


                                // Increse the balance on MFTC Card 

                                var MFTCCardDetails = AutoPaymentInformation.MFTCCard;
                                MFTCCardDetails.CurrentBalance += obj.RecievingAmount;
                                dbContext.Entry(MFTCCardDetails).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();


                                // Send Email 
                                MailCommon mail = new MailCommon();
                                var baseUrl = "http://moneyfex.com/";
                                string FaxerName = FaxerInformation.FirstName + " " + FaxerInformation.MiddleName + " " + FaxerInformation.LastName;
                                string FaxerEmail = FaxerInformation.Email;
                                string body = "";

                                string CardUserCountry = Common.Common.GetCountryName(ReceivingCountryCode);

                                string SetAutoTopUp = baseUrl + "/FaxerAutoPayments/AddSomeoneElseMoneyFaxCardAutoTopUp?MFTCCardid=" + obj.KiiPayPersonalWalletId;
                                string TopUpMoneyfaxCard = baseUrl + "/TopUpSomeoneElseMFTCCard/MFTCCardAccountNo?MFTCCardNO=" + MFTCCardDetails.MobileNo.Decrypt();
                                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFTCCardTopUp?FaxerName=" + FaxerName +
                                    "&CardUserCountry=" + CardUserCountry + "&TopUpMoneyfaxCard=" + TopUpMoneyfaxCard + "&SetAutoTopUp=" + SetAutoTopUp);


                                string FaxerCurrency = Common.Common.GetCountryCurrency(FaxingCountryCode);
                                string CardUserCurrency = Common.Common.GetCountryCurrency(ReceivingCountryCode);
                                string ReceiptURL = baseUrl + "/EmailTemplate/MFTCCardTopUpReceipt?ReceiptNumber=" + obj.ReceiptNumber + "&Date=" +
                                    obj.TransactionDate.ToString("dd/MM/yyyy") + "&Time=" + obj.TransactionDate.ToString("HH:mm")
                                    + "&FaxerFullName=" + FaxerName + "&MFTCCardNumber=" + MFTCCardDetails.MobileNo.Decrypt()
                                    + "&CardUserFullName=" + MFTCCardDetails.FirstName + " " + MFTCCardDetails.MiddleName + " " + MFTCCardDetails.LastName
                                    + "&AmountTopUp=" + obj.FaxingAmount + " " + FaxerCurrency + "&ExchangeRate=" + obj.ExchangeRate +
                                    "&AmountInLocalCurrency=" + obj.RecievingAmount + " " + CardUserCurrency + "&Fee=" + obj.FaxingFee + " " + FaxerCurrency + "&BalanceOnCard=" + MFTCCardDetails.CurrentBalance + " " + CardUserCurrency + "&TopupReference=" + AutoPaymentInformation.TopUpReference;
                                var ReceiptPDF = Common.Common.GetPdf(ReceiptURL);

                                mail.SendMail(FaxerEmail, "Confirmation of virtual account deposit", body, ReceiptPDF);
                                // End 

                                #region Virtul account deposit SMS

                                SmsApi smsApiServices = new SmsApi();
                                
                                string message = smsApiServices.GetVirtualAccountDepositMessage(FaxerName, MFTCCardDetails.MobileNo.Decrypt().GetVirtualAccountNo(),
                                                                                  Common.Common.GetCurrencySymbol(FaxerInformation.Country) + obj.FaxingAmount);

                                string PhoneNo = Common.Common.GetCountryPhoneCode(FaxerInformation.Country) + "" + FaxerInformation.PhoneNumber;
                                smsApiServices.SendSMS(PhoneNo, message);


                                string receiverPhoneNo = Common.Common.GetCountryPhoneCode(MFTCCardDetails.CardUserCountry) + "" + MFTCCardDetails.CardUserTel;
                                smsApiServices.SendSMS(receiverPhoneNo, message);
                                #endregion

                            }
                        }
                    }
                }

            }

        }

        public bool IsValidDayOfAutoPayment(DB.OtherMFTCCardAutoTopUpInformation model)
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



        public bool IsValidAutoPayment(DB.OtherMFTCCardAutoTopUpInformation model)
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

                        gannuParneDays = Day ;
                        
                        break;
                    case DB.AutoPaymentFrequency.Yearly:
                        Day = currentDate.DayOfYear;
                        gannuParneDays = Day;

                        break;
                    default:
                        break;
                }
            }
            StartedTransactionDate = currentDate.AddDays(-(gannuParneDays)).Date;

            var result = dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.FaxerId == model.FaxerId && x.KiiPayPersonalWalletId == model.MFTCCardId && DbFunctions.TruncateTime( x.TransactionDate) >= DbFunctions.TruncateTime(StartedTransactionDate)
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