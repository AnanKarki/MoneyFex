using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.Services
{
    public class CardUserCommonServices
    {
        DB.FAXEREntities dbContext = null;
        PayGoodsAndServicesAbroadServices payGoodsAndServicesAbroadServices = null;

        int MFTCCardId = Common.CardUserSession.LoggedCardUserViewModel == null ? 0 : Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId;
        public CardUserCommonServices()
        {
            dbContext = new DB.FAXEREntities();
            payGoodsAndServicesAbroadServices = new PayGoodsAndServicesAbroadServices();
        }

        public decimal getCurrentBalanceOnCard(int MFTCCardId)
        {

            var result = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardId).FirstOrDefault();
            return result.CurrentBalance;

        }
        public DB.KiiPayBusinessInformation GetBusinessInformation(string AccountNo = "")
        {

            var result = dbContext.KiiPayBusinessInformation.Where(x => x.BusinessMobileNo == AccountNo).FirstOrDefault();
            return result;
        }

        public DB.KiiPayBusinessWalletInformation GetMFBCCardInformation(int KiiPayBusinessInformationId)
        {

            var result = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId && (x.CardStatus != DB.CardStatus.IsDeleted && x.CardStatus != DB.CardStatus.IsRefunded)).FirstOrDefault();
            return result;
        }

        public DB.KiiPayBusinessWalletInformation GetMFBCCardInformationByMFBCID(int MFTCCardId) {


            var result = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == MFTCCardId).FirstOrDefault();
            return result;
        }

        public DB.KiiPayBusinessWalletInformation GetMFTCCardInformationByCardid(int CardId)
        {

            var result = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == CardId).FirstOrDefault();
            return result;
        }

        public List<CountryDropDownviewModel> getCountries()
        {

            var result = (from c in dbContext.Country
                          select new CountryDropDownviewModel()
                          {
                              Code = c.CountryCode,
                              Name = c.CountryName
                          }).ToList();
            return result;
        }

        public DB.KiiPayPersonalWalletInformation GetMFTCCardUserInformation(int MFTCCardID)
        {

            var result = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardID).FirstOrDefault();
            return result;
        }


       

     


        public List<PreviousPayeeDropDown> getPreviousPayees()
        {
            int MFTCCardId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId;
            string CardUserCountry = Common.CardUserSession.LoggedCardUserViewModel.Country;
            var result = (from c in dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Where(x => x.KiiPayPersonalWalletInformationId == MFTCCardId && x.KiiPayPersonalWalletInformation.CardUserCountry == CardUserCountry )
                          select new PreviousPayeeDropDown()
                          {

                              BusinessMFCode = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo,
                              Name = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessName
                          }
                          ).Distinct().ToList();
            return result;



        }
        public List<PreviousPayeeDropDown> getInternationalPreviousPayees()
        {
            int MFTCCardId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId;
            string CardUserCountry = Common.CardUserSession.LoggedCardUserViewModel.Country;
            var result = (from c in dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.Where(x => x.PayedFromKiiPayPersonalWalletId == MFTCCardId)
                          select new PreviousPayeeDropDown()
                          {
                              BusinessMFCode = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo,
                              Name = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessName
                          }
                          ).Distinct().ToList();
            return result;



        }



        internal string getMFCN()
        {
            //this code should be unique and random with 8 digit length
            var val = Common.Common.GenerateRandomDigit(8);

            while (dbContext.CardUserNonCardTransaction.Where(x => x.MFCN == val).Count() > 0)
            {
                val = Common.Common.GenerateRandomDigit(8);
            }
            // 21 is code for cash to cash payment made by virtual account User
            return val + "-21";
        }
        internal string GetNewReceiptNumberToSave()
        {
            //this code should be unique and random with 8 digit length
            var val = "Os-Np-MF" + Common.Common.GenerateRandomDigit(5);
            while (dbContext.CardUserNonCardTransaction.Where(x => x.ReceiptNumber == val).Count() > 0)
            {
                val = "Os-Np-MF" + Common.Common.GenerateRandomDigit(5);
            }
            return val;
        }
        internal string ReceiptNoForMerchantInternationalPayment()
        {
            //this code should be unique and random with 8 digit length
            var val = "Os-MP-MF-" + Common.Common.GenerateRandomDigit(5);
            while (dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.Where(x => x.ReceiptNumber == val).Count() > 0)
            {
                val = "Os-Mp-MF-" + Common.Common.GenerateRandomDigit(5);
            }
            return val;
        }
        //internal string ReceiptNoForMerchantNationalPayment()
        //{
        //    //this code should be unique and random with 8 digit length
        //    var val = "Os-MP-MF-" + Common.Common.GenerateRandomDigit(5);
        //    while (dbContext.MFTCCardToMFBCCardTransaction.Where(x => x. == val).Count() > 0)
        //    {
        //        val = "Os-Mp-MF-" + Common.Common.GenerateRandomDigit(5);
        //    }
        //    return val;
        //}
        internal string ReceiptNoForMFTCPayment()
        {
            //this code should be unique and random with 8 digit length
            var val = "Os-Ctu-MF-" + Common.Common.GenerateRandomDigit(5);
            while (dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.Where(x => x.ReceiptNumber == val).Count() > 0)
            {
                val = "Os-Ctu-MF-" + Common.Common.GenerateRandomDigit(5);
            }
            return val;
        }

        public int TotalNonCardTransfer()
        {

            int result = dbContext.CardUserNonCardTransaction.Where(x => x.MFTCCardId == MFTCCardId && x.FaxingStatus == DB.FaxingStatus.NotReceived ||  x.FaxingStatus == DB.FaxingStatus.Hold).Count();

            return result;

        }

        public DB.CardUserReceiverDetails GetReceiverDetails(int Id)
        {

            var result = dbContext.CardUserReceiverDetails.Where(x => x.Id == Id).FirstOrDefault();
            return result;
        }

        public decimal TotalTransferAmountByCardUser(DateTime StartTransactionDate, int CardId)
        {

            var MFTCCardTopUP = dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.Where(x => x.SenderWalletId == CardId && x.TransactionDate >= StartTransactionDate).Sum(x => (Decimal?)x.TotalAmount) ?? 0;


            var NonCardTransfer = dbContext.CardUserNonCardTransaction.Where(x => x.CardUserReceiverDetails.Id == CardId && x.TransactionDate >= StartTransactionDate).Sum(x => (Decimal?)x.TotalAmount) ?? 0;


            return MFTCCardTopUP + NonCardTransfer;

        }

        public bool ValidAmountAccordingToPurchaseLimit(int CardID, decimal Amount)
        {


            var CardDetails = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == CardID).FirstOrDefault();
            Decimal TotaAmountPurchase = 0;



            DateTime currentDate = DateTime.Now.Date;

            if (CardDetails.GoodsLimitType == DB.AutoPaymentFrequency.NoLimitSet)
            {
                return true;

            }
            else
            {

                DateTime StartedTransactionDate = new System.DateTime();

                var Day = 0;
                var gannuParneDays = 0.0;
                if (gannuParneDays == 0 && CardDetails.GoodsLimitType != DB.AutoPaymentFrequency.NoLimitSet)
                {
                    switch (CardDetails.GoodsLimitType)
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

                ///TODO: put this on services
                //TotaAmountWithDrawl = dbContext.UserCardWithdrawl.Where(x => x.TransactionDate >= StartedTransactionDate).Sum(x => x.TransactionAmount);
                TotaAmountPurchase = payGoodsAndServicesAbroadServices.TotalGoodsPurchaseAmount(StartedTransactionDate, CardDetails.Id);
                if (TotaAmountPurchase + Convert.ToDecimal(Amount) > Convert.ToDecimal(CardDetails.GoodsPurchaseLimit))
                {

                    return false;
                    //ModelState.AddModelError("PayingAmount", "Sorry, You have exceeded your withdrawl limit");
                    //return View(model);
                }


                return true;




            }
        }


        public bool ValidAmountAccordingToWithdrawalLimit(int CardID, decimal Amount)
        {


            var CardDetails = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == CardID).FirstOrDefault();
            Decimal TotaAmountTransfered = 0;



            DateTime currentDate = DateTime.Now.Date;

            if (CardDetails.CashLimitType == DB.CardLimitType.NoLimitSet)
            {
                return true;

            }
            if (CardDetails.CashLimitType == DB.CardLimitType.Daily)
            {

                var MFTCCardTopUP = dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.Where(x => x.SenderWalletId == CardDetails.Id && DbFunctions.TruncateTime(x.TransactionDate) == DateTime.Now.Date).Sum(x => (Decimal?)x.TotalAmount) ?? 0;


                var NonCardTransfer = dbContext.CardUserNonCardTransaction.Where(x => x.CardUserReceiverDetails.Id == CardDetails.Id && DbFunctions.TruncateTime(x.TransactionDate) == DateTime.Now.Date).Sum(x => (Decimal?)x.TotalAmount) ?? 0;

                if (MFTCCardTopUP + NonCardTransfer + Convert.ToDecimal(Amount) > Convert.ToDecimal(CardDetails.CashWithdrawalLimit))
                {

                    return false;
                }
                else
                {

                    return true;
                }

            }
            else
            {

                DateTime StartedTransactionDate = new System.DateTime();

                var Day = 0;
                var gannuParneDays = 0.0;
                if (gannuParneDays == 0 && CardDetails.CashLimitType != DB.CardLimitType.NoLimitSet)
                {
                    switch (CardDetails.CashLimitType)
                    {
                        case DB.CardLimitType.Weekly:
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

                        case DB.CardLimitType.Monthly:
                            Day = currentDate.Day;
                            gannuParneDays = Day;

                            break;


                        default:
                            break;
                    }
                }
                StartedTransactionDate = currentDate.AddDays(-(gannuParneDays));

                ///TODO: put this on services
                //TotaAmountWithDrawl = dbContext.UserCardWithdrawl.Where(x => x.TransactionDate >= StartedTransactionDate).Sum(x => x.TransactionAmount);
                TotaAmountTransfered = TotalTransferAmountByCardUser(StartedTransactionDate, CardDetails.Id);
                if (TotaAmountTransfered + Convert.ToDecimal(Amount) > Convert.ToDecimal(CardDetails.CashWithdrawalLimit))
                {

                    return false;
                    //ModelState.AddModelError("PayingAmount", "Sorry, You have exceeded your withdrawl limit");
                    //return View(model);
                }


                return true;




            }
        }


        public void SendMailWhenBalanceISZero(int MFTCCardId) {

            MailCommon mailCommon = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string EmailToSetAutoTopup_body = "";
            var MFTCCardDetails = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardId).FirstOrDefault();
            string SetAutoTopUp = baseUrl + "/FaxerAutoPayments/MoneyFaxCardAutoTopUp?mftcCardInformationId=" + MFTCCardId;

            string TopUpMoneyfaxCard = baseUrl + "/StartFaxingMoney/Index?mFTCCardInformationId=" + MFTCCardId;
            decimal exchangeRate = 0;

            var SenderInformation = GetSenderInformation(MFTCCardId);
            string FaxerCountry = SenderInformation.Country;
            string ReceiverCountry = MFTCCardDetails.CardUserCountry;
            string FaxerCurrency = Common.Common.GetCountryCurrency(FaxerCountry);
            string CardUserCurrency = Common.Common.GetCountryCurrency(ReceiverCountry);
            string CardUserName = MFTCCardDetails.FirstName + " " + MFTCCardDetails.MiddleName + " " + MFTCCardDetails.LastName; 
            string FaxerName = SenderInformation.FirstName +  " " + SenderInformation.MiddleName + " " + SenderInformation.LastName;
            // Calculate exchange Rate 
            var exchangeRateObj = dbContext.ExchangeRate.Where(x => x.CountryCode1 == SenderInformation.Country && x.CountryCode2 == MFTCCardDetails.CardUserCountry).FirstOrDefault();
            if (exchangeRateObj == null)
            {
                var exchangeRateobj2 = dbContext.ExchangeRate.Where(x => x.CountryCode1 == MFTCCardDetails.CardUserCountry && x.CountryCode2 == SenderInformation.Country).FirstOrDefault();
                if (exchangeRateobj2 != null)
                {
                    exchangeRateObj = exchangeRateobj2;
                }

            }
            else
            {
                exchangeRate = exchangeRateObj.CountryRate1;
            }
            if (FaxerCountry == ReceiverCountry)
            {

                exchangeRate = 1m;

            }
            EmailToSetAutoTopup_body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/BalanceonMFTCCardZeroEmail?FaxerName=" + FaxerName +
                "&MFTCCardNumber=" + MFTCCardDetails.MobileNo.Decrypt().GetVirtualAccountNo() +
                "&CardUserName" + CardUserName + "&SenderCountryCode=" + FaxerCountry + "&SenderCurrency=" + FaxerCurrency
                + "&CardUserCountryCode=" + MFTCCardDetails.CardUserCountry +
                "&CardUserCurrency=" + CardUserCurrency + "&TopUpMoneyfaxCard=" + TopUpMoneyfaxCard +
                "&SetAutoTopUp=" + SetAutoTopUp + "&ExchangeRate=" + exchangeRate);

            mailCommon.SendMail(SenderInformation.Email, "Balance on virtual account " + MFTCCardDetails.MobileNo.Decrypt().GetVirtualAccountNo() + "is Zero (0)", EmailToSetAutoTopup_body);



            //mail.SendMail(MFTCCardDetails.CardUserEmail, "Balance on MoneyFex Top-Up Card " + MFTCCardDetails.MFTCCardNumber.Decrypt() + "is Zero (0)", EmailToSetAutoTopup_body);


            //Balance Zero SMS 

            SmsApi smsApiServices = new SmsApi();

           string message =  smsApiServices.GetVirtualAccountBalanceZeroMessage(FaxerName , MFTCCardDetails.MobileNo.Decrypt().GetVirtualAccountNo() ,
                                                            CardUserName , Common.Common.GetCountryName(MFTCCardDetails.CardUserCountry));

            string PhoneNo = Common.Common.GetCountryPhoneCode(SenderInformation.Country) + "" + SenderInformation.PhoneNumber;
            smsApiServices.SendSMS(PhoneNo, message);


        }

        public DB.FaxerInformation GetSenderInformation(int mFTCCardId)
        {
            var result = (from c in dbContext.SenderKiiPayPersonalAccount.Where(x => x.KiiPayPersonalWalletId == mFTCCardId)
                          join d in dbContext.FaxerInformation on c.SenderId equals d.Id
                          select d).FirstOrDefault();
            return result;

        }

        public void AutoTopUp(int MFTCCardId)
        {

            var result = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardId).FirstOrDefault();
            if (result.AutoTopUp == true) {

                // Auto Topup MFTC Card Has been used of Agent services name (MFTCAutoTopUpServices)
                Agent.AgentServices.MFTCAutoTopUpServices mFTCAutoTopUpServices = new Agent.AgentServices.MFTCAutoTopUpServices();
                mFTCAutoTopUpServices.AutoTopUp(MFTCCardId);
            }
        }

        public bool CheckBalanceForMessage(decimal transactionAmount)
        {
            var senderDetails = GetMFTCCardUserInformation(Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId);
            decimal smsFee = dbContext.SmsFee.Where(x => x.CountryCode == senderDetails.CardUserCountry).Select(x => x.SmsFee).FirstOrDefault();
            decimal accountbalance = senderDetails.CurrentBalance;
            if(accountbalance < smsFee + transactionAmount)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }


    public class PreviousPayeeDropDown
    {


        public string Name { get; set; }
        public string BusinessMFCode { get; set; }


    }

    public class CountryDropDownviewModel
    {

        public string Code { get; set; }

        public string Name { get; set; }
    }



}