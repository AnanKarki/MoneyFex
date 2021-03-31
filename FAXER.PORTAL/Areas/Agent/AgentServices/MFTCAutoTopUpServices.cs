using FAXER.PORTAL.Common;
using FAXER.PORTAL.Services;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class MFTCAutoTopUpServices
    {
        DB.FAXEREntities dbContext = null;

        public MFTCAutoTopUpServices()
        {

            dbContext = new DB.FAXEREntities();
        }

        private int getSenderId(int KiiPayWalletId) {


            int Id = dbContext.SenderKiiPayPersonalAccount.Where(x => x.KiiPayPersonalWalletId == KiiPayWalletId).Select(x => x.SenderId).FirstOrDefault();
            return Id; 

        }
        public bool AutoTopUp(int MFTCCardId)
        {

            var MFTCCardDetails = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardId).FirstOrDefault();


            //int FaxerID = MFTCCardDetails.FaxerId;

            int FaxerID = getSenderId(MFTCCardId);
            var FaxerDetails = dbContext.FaxerInformation.Where(x => x.Id == FaxerID).FirstOrDefault();

            decimal exchangeRate = 0, faxingFee = 0;
            var exchangeRateObj = dbContext.ExchangeRate.Where(x => x.CountryCode1 == FaxerDetails.Country && x.CountryCode2 == MFTCCardDetails.CardUserCountry).FirstOrDefault();
            if (exchangeRateObj == null)
            {
                var exchangeRateobj2 = dbContext.ExchangeRate.Where(x => x.CountryCode1 == MFTCCardDetails.CardUserCountry && x.CountryCode2 == FaxerDetails.Country).FirstOrDefault();
                if (exchangeRateobj2 != null)
                {
                    exchangeRateObj = exchangeRateobj2;
                    exchangeRateObj.CountryRate1 = Math.Round(1 / exchangeRateobj2.CountryRate1 / 6, MidpointRounding.AwayFromZero);
                }
            }
            exchangeRate = exchangeRateObj.CountryRate1;
            var feeSummary = Services.SEstimateFee.CalculateFaxingFee(MFTCCardDetails.AutoTopUpAmount, true, false, exchangeRateObj.CountryRate1, Services.SEstimateFee.GetFaxingCommision(FaxerDetails.Country));


            var FaxerCurrrency = Common.Common.GetCountryCurrency(FaxerDetails.Country);
            
            var SavedCardDetials = dbContext.SavedCard.Where(x => x.UserId == FaxerID).FirstOrDefault();
            #region  Strip portion
            StripeConfiguration.SetApiKey("sk_test_OJiTq50I3SNoO3tL3bOJBhTy");

            //Sample  Credit card

            var stripeTokenCreateOptions = new StripeTokenCreateOptions
            {
                Card = new StripeCreditCardOptions
                {
                    Number = SavedCardDetials.Num.Decrypt(),
                    ExpirationMonth = int.Parse(SavedCardDetials.EMonth.Decrypt()),
                    ExpirationYear = int.Parse(SavedCardDetials.EYear.Decrypt()),
                    Cvc = SavedCardDetials.ClientCode.Decrypt(),
                    Name = SavedCardDetials.CardName.Decrypt()
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

                Log.Write("MFTC Card Auto To up  Exception :" + ex.Message);
            }


            if (!string.IsNullOrEmpty(token))
            {
                var chargeOptions = new StripeChargeCreateOptions()
                {
                    Amount = (Int32)feeSummary.TotalAmount * 100,
                    Currency = FaxerCurrrency,
                    Description = "Charge for " + SavedCardDetials.CardName.Decrypt(),
                    //SourceTokenOrExistingSourceId = "tok_mastercard",// obtained with Stripe.js
                    SourceTokenOrExistingSourceId = token
                };
                var chargeService = new StripeChargeService();
                StripeCharge charge = chargeService.Create(chargeOptions);

                #endregion
                SFaxingTopUpCardTransaction service = new SFaxingTopUpCardTransaction();
                string ReceiptNumber = service.GetNewMFTCCardTopUpReceipt();
                //transaction history object
                DB.SenderKiiPayPersonalWalletPayment obj = new DB.SenderKiiPayPersonalWalletPayment()
                {
                    KiiPayPersonalWalletInformationId = MFTCCardDetails.Id,
                    FaxerId = FaxerID,
                    FaxingAmount = feeSummary.FaxingAmount,
                    RecievingAmount = feeSummary.ReceivingAmount,
                    ExchangeRate = feeSummary.ExchangeRate,
                    FaxingFee = feeSummary.FaxingFee,
                    ReceiptNumber = ReceiptNumber,
                    TransactionDate = System.DateTime.Now,
                };
                obj = service.SaveTransaction(obj);

                DB.MFTCAutoTopUpTransactionLog transactionLog = new DB.MFTCAutoTopUpTransactionLog()
                {

                    FaxerID = obj.FaxerId,
                    MFTCCardID = obj.KiiPayPersonalWalletInformationId,
                    AutoTopUpAmount = MFTCCardDetails.AutoTopUpAmount,
                    TransactionDate = obj.TransactionDate
                };

                dbContext.MFTCAutoTopUpTransactionLog.Add(transactionLog);
                dbContext.SaveChanges();

                //MFTCCardDetails.CurrentBalance += MFTCCardDetails.AutoTopUpAmount;
                //dbContext.SaveChanges();

                MailCommon mail = new MailCommon();

                var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

                string body = "";


                var FaxingCurrency = Common.Common.GetCountryCurrency(FaxerDetails.Country);
                var CardUserCurrency = Common.Common.GetCountryCurrency(MFTCCardDetails.CardUserCountry);
                var CardUserCountry = Common.Common.GetCountryName(MFTCCardDetails.CardUserCountry);

                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFTCAutoTopUpTransaction?FaxerName=" + FaxerDetails.FirstName + " " + FaxerDetails.MiddleName + " " + FaxerDetails.LastName
                    + "&CardUserCountry=" + CardUserCountry
                    + "&CardUserName=" + MFTCCardDetails.FirstName + " " + MFTCCardDetails.MiddleName + " " + MFTCCardDetails.LastName
                    + "&CardNumber=" + MFTCCardDetails.MobileNo.Decrypt() + "&CardUserCity=" + MFTCCardDetails.CardUserCity + "&TopUpAmount=" + obj.FaxingAmount + " " + FaxingCurrency);


                string ReceiptURL = baseUrl + "/EmailTemplate/MFTCCardTopUpReceipt?ReceiptNumber=" + obj.ReceiptNumber + "&Date=" +
                  obj.TransactionDate.ToString("dd/MM/yyyy") + "&Time=" + obj.TransactionDate.ToString("HH:mm")
                  + "&FaxerFullName=" + FaxerDetails.FirstName + " " + FaxerDetails.MiddleName + " " + FaxerDetails.LastName
                  + "&MFTCCardNumber=" + MFTCCardDetails.MobileNo.Decrypt()
                  + "&CardUserFullName=" + MFTCCardDetails.FirstName + " " + MFTCCardDetails.MiddleName + " " + MFTCCardDetails.LastName
                  + "&AmountTopUp=" + obj.FaxingAmount + " " + FaxingCurrency + "&ExchangeRate=" + obj.ExchangeRate +
                  "&AmountInLocalCurrency=" + obj.RecievingAmount + " " + CardUserCurrency + "&Fee=" + obj.FaxingFee + " " + FaxingCurrency + "&BalanceOnCard=" + obj.KiiPayPersonalWalletInformation.CurrentBalance + " " + CardUserCurrency;
                var ReceiptPDF = Common.Common.GetPdf(ReceiptURL);

                mail.SendMail(FaxerDetails.Email, "Confirmation of Moneyfex card Top-up ", body, ReceiptPDF);

                //Virtual Account Deposit SMS 
                
                SmsApi smsApiServices = new SmsApi();

                string SenderName = FaxerDetails.FirstName + " " + FaxerDetails.MiddleName + " " + FaxerDetails.LastName;
                string message = smsApiServices.GetVirtualAccountDepositMessage(SenderName, MFTCCardDetails.MobileNo.Decrypt().GetVirtualAccountNo(),
                                                                  Common.Common.GetCurrencySymbol(FaxerDetails.Country) + obj.FaxingAmount );

                string PhoneNo = Common.Common.GetCountryPhoneCode(FaxerDetails.Country) + "" + FaxerDetails.PhoneNumber;
                smsApiServices.SendSMS(PhoneNo, message);


                string receiverPhoneNo = Common.Common.GetCountryPhoneCode(MFTCCardDetails.CardUserCountry) + "" + MFTCCardDetails.CardUserTel;
                smsApiServices.SendSMS(receiverPhoneNo, message);


                return true;
            }
            return false;
        }

    }
}