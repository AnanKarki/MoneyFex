using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using FAXER.PORTAL.SignalR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Services
{
    public class KiiPayPersonalCommonServices
    {
        FAXEREntities dbContext = null;
        public KiiPayPersonalCommonServices()
        {
            dbContext = new FAXEREntities();
        }

        public List<Admin.Services.DropDownViewModel> GetCountries()
        {
            var result = (from c in dbContext.Country
                          select new Admin.Services.DropDownViewModel()
                          {
                              Id = c.Id,
                              Code = c.CountryCode,
                              Name = c.CountryName
                          }
                      ).ToList();

            return result;
        }

        public List<Admin.Services.DropDownViewModel> GetBanks() {
            var result = (from c in dbContext.SavedBank
                          select new Admin.Services.DropDownViewModel()
                          {
                              Id = c.Id,
                              Code = c.BankName,
                              Name = c.BankName
                          }).GroupBy(x => x.Code).Select(x => x.FirstOrDefault()).ToList();
            return result;
        }

        public string getCountryPhoneCode(string countryCode)
        {
            if (!string.IsNullOrEmpty(countryCode))
            {
                var countryData = dbContext.Country.Where(x => x.CountryCode.ToLower().Trim() == countryCode.ToLower().Trim()).FirstOrDefault();
                if (countryData != null)
                {
                    return countryData.CountryPhoneCode;
                }
            }
            return "";
        }

        public DB.KiiPayBusinessWalletInformation GetKiipayBusinessWalletInfoByKiiPayBusinessId(int Id)
        {
            var data = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == Id).FirstOrDefault();
            return data == null ? null : data;

        }
        internal string GetReceiptNoForKiiPayPersonalTransaction()
        {
            return "";
        }

        internal string GetReceiptNoForKiiPayPersonalInternationalTransaction()
        {



            return "";
        }
        public List<RecenltyPaidKiiPayBusinessVM> GetRecentlyPaidInternationalKiiPayBusiness(int KiiPayPersonalWalletId)
        {

            var result = (from c in dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.Where(x => x.PayedFromKiiPayPersonalWalletId == KiiPayPersonalWalletId)
                          select new RecenltyPaidKiiPayBusinessVM()
                          {
                              BusinessId = c.PayedToKiiPayBusinessWalletId,
                              MobileNo = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo,
                              Country = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessCountry
                          }).GroupBy(x => x.MobileNo).Select(x => x.FirstOrDefault()).ToList();

            return result;
        }
        public List<RecenltyPaidKiiPayBusinessVM> GetRecentlyPaidNationalKiiPayBusiness(int KiiPayPersonalWalletId)
        {

            var result = (from c in dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Where(x => x.KiiPayPersonalWalletInformationId == KiiPayPersonalWalletId)
                          select new RecenltyPaidKiiPayBusinessVM()
                          {
                              BusinessId = c.KiiPayBusinessWalletInformationId,
                              MobileNo = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo,
                              Country = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessCountry
                          }).GroupBy(x => x.MobileNo).Select(x => x.FirstOrDefault()).ToList();

            return result;
        }
        public List<RecenltyPaidKiiPayBusinessVM> GetAllRecenltyPaidKiiPayPersonal(int KiiPayPersonalWalletId)
        {

            var result = GetRecentlyPaidNationalKiiPayBusiness(KiiPayPersonalWalletId).
                Concat(GetRecentlyPaidInternationalKiiPayBusiness(KiiPayPersonalWalletId)).
                GroupBy(x => x.MobileNo).Select(x => x.FirstOrDefault()).ToList();

            return result;
        }


        public bool DoesAccountHaveEnoughBal(int SenderWalletId, decimal SendingAmount)
        {

            var Curbal = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == SenderWalletId).FirstOrDefault().CurrentBalance;
            if (SendingAmount > Curbal)
            {
                return false;
            }
            return true;
        }
        public List<RecentlyPaidKiiPayPersonalVM> GetAllRecenltyPaidKiiPayPersonalWalletInfo(int KiiPayPersonalWalletId)
        {

            var result = (from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.Where(x => x.SenderWalletId == KiiPayPersonalWalletId)
                          select new RecentlyPaidKiiPayPersonalVM()
                          {
                              WalletId = c.KiiPayPersonalWalletInformation.Id,
                              MobileNo = c.KiiPayPersonalWalletInformation.MobileNo,
                              ReceiverIsLocal = c.PaymentType == PaymentType.Local ? true : false,
                              Country = c.KiiPayPersonalWalletInformation.CardUserCountry
                          }).GroupBy(x => x.MobileNo).Select(x => x.FirstOrDefault()).ToList();
            return result;

        }

        public KiiPayPersonalUserInformation kiiPayPersonalUserInformation()
        {
            if (Common.CardUserSession.LoggedCardUserViewModel != null)
            {
                if (Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId != 0)
                {
                    var data = dbContext.KiiPayPersonalUserInformation.Where(x => x.Id == Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId).FirstOrDefault();
                    return data;
                }
            }
            return null;
        }

        public decimal calculateExchangeRate(string fromCountryCode, string toCountryCode)
        {
            if (!string.IsNullOrEmpty(fromCountryCode) && !string.IsNullOrEmpty(toCountryCode))
            {
                if (fromCountryCode.Trim() == toCountryCode.Trim())
                {
                    return 1;
                }
                var exchangeRateData = dbContext.ExchangeRate.Where(x => x.CountryCode1.Trim() == fromCountryCode.Trim() && x.CountryCode2.Trim() == toCountryCode.Trim()).FirstOrDefault();
                if (exchangeRateData != null)
                {
                    return exchangeRateData.CountryRate1;
                }
                else
                {
                    exchangeRateData = dbContext.ExchangeRate.Where(x => x.CountryCode1.Trim() == toCountryCode.Trim() && x.CountryCode2.Trim() == fromCountryCode.Trim()).FirstOrDefault();
                    if (exchangeRateData != null)
                    {
                        decimal rate = 1 / exchangeRateData.CountryRate1;
                        return rate;
                    }
                }
            }
            return 0;
        }
        public void BalanceIn(int KiiPayPersonalWalletId, decimal Amount)
        {

            var data = GetKiipayPersonalWalletInfo(KiiPayPersonalWalletId);
            data.CurrentBalance += Amount;
            dbContext.Entry<DB.KiiPayPersonalWalletInformation>(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }

        public void BalanceOut(int KiiPayPersonalWalletId, decimal Amount)
        {

            var data = GetKiipayPersonalWalletInfo(KiiPayPersonalWalletId);
            data.CurrentBalance = data.CurrentBalance - Amount;
            dbContext.Entry<DB.KiiPayPersonalWalletInformation>(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }
        public string generateTransactionReceipt(KiiPayTransactionType transactionType)
        {
            return "";
        }

        public DB.KiiPayPersonalWalletInformation GetKiipayPersonalWalletInfo(int Id)
        {
            var data = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == Id).FirstOrDefault();
            return data;

        }

        public PayForServicesSummaryViewModel CalculateKiiPayPersonalToKiiPayBusinessPaymentSummary(bool includingFaxingFee, bool isAmountToBeReceived)
        {
            var receiverInformationData = dbContext.KiiPayBusinessInformation.Where(x => x.BusinessMobileNo.Trim() == Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingPhoneNumber.Trim()).FirstOrDefault();
            var senderInformationData = dbContext.KiiPayPersonalUserInformation.Where(x => x.MobileNo.Trim() == Common.CardUserSession.LoggedCardUserViewModel.MobileNumber.Trim()).FirstOrDefault();
            var senderWalletData = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo.Trim() == Common.CardUserSession.LoggedCardUserViewModel.MobileNumber.Trim()).FirstOrDefault();
            if (receiverInformationData != null && senderInformationData != null && senderWalletData != null)
            {
                var receiverWalletData = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == receiverInformationData.Id && x.CardStatus == CardStatus.Active).FirstOrDefault();
                if (receiverWalletData != null)
                {
                    decimal amount = Common.CardUserSession.PayIntoAnotherWalletSession.PayingAmount;
                    decimal exchangeRate = 1;
                    decimal smsFee = 0;
                    if (receiverInformationData.BusinessCountry.Trim() == senderInformationData.Country.Trim())
                    {
                        exchangeRate = 1;
                        if (Common.CardUserSession.PayIntoAnotherWalletSession.SendSMSNotification == true)
                        {
                            smsFee = Common.Common.GetSmsFee(senderInformationData.Country);
                        }
                    }
                    else
                    {
                        var exchangeData = dbContext.ExchangeRate.Where(x => x.CountryCode1.Trim() == senderInformationData.Country.Trim() && x.CountryCode2.Trim() == receiverInformationData.BusinessCountry.Trim()).FirstOrDefault();
                        if (exchangeData != null)
                        {
                            exchangeRate = exchangeData.CountryRate1;
                        }
                        else
                        {
                            exchangeData = dbContext.ExchangeRate.Where(x => x.CountryCode1.Trim() == receiverInformationData.BusinessCountry.Trim() && x.CountryCode2.Trim() == senderInformationData.Country.Trim()).FirstOrDefault();
                            if (exchangeData != null)
                            {
                                exchangeRate = 1 / exchangeData.CountryRate1;
                            }
                        }
                    }
                    decimal faxingFeeRate = SEstimateFee.GetFaxingCommision(senderInformationData.Country);

                    var paymentData = SEstimateFee.CalculateFaxingFee(amount, includingFaxingFee, isAmountToBeReceived, exchangeRate, faxingFeeRate);
                    if (paymentData != null)
                    {
                        var result = new PayForServicesSummaryViewModel()
                        {
                            Id = 0,
                            Amount = paymentData.FaxingAmount,
                            Fee = paymentData.FaxingFee,
                            LocalSMSMessage = smsFee,
                            PayingAmout = paymentData.TotalAmount,
                            ReceivingAmount = paymentData.ReceivingAmount,
                            PaymentReference = Common.CardUserSession.PayIntoAnotherWalletSession.PaymentReference,
                            SendingCurrencySymbol = Common.Common.GetCurrencySymbol(senderInformationData.Country),
                            SendingCurrencyCode = Common.Common.GetCountryCurrency(senderInformationData.Country),
                            ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(receiverInformationData.BusinessCountry),
                            ReceivingCurrencyCode = Common.Common.GetCountryCurrency(receiverInformationData.BusinessCountry),
                            ExchangeRate = exchangeRate
                        };
                        return result;
                    }
                }
            }

            return null;
        }

        public AccountPaymentSummaryViewModel CalculateKiiPayPersonalPaymentSummary(decimal amount, string sendingMobileNumber, string receivingMobileNumber, bool IncludeFaxingFee, string reference)
        {
            if (amount != 0 && !string.IsNullOrEmpty(sendingMobileNumber) && !string.IsNullOrEmpty(receivingMobileNumber))
            {
                var ReceiversUserData = dbContext.KiiPayPersonalUserInformation.Where(x => x.MobileNo.Trim() == receivingMobileNumber.Trim()).FirstOrDefault();
                var ReceiversWalletData = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo.Trim() == receivingMobileNumber.Trim()).FirstOrDefault();
                var SendersUserData = dbContext.KiiPayPersonalUserInformation.Where(x => x.MobileNo.Trim() == sendingMobileNumber.Trim()).FirstOrDefault();
                var SendersWalletData = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo.Trim() == sendingMobileNumber.Trim()).FirstOrDefault();
                if (ReceiversUserData != null && ReceiversWalletData != null && SendersUserData != null && SendersWalletData != null)
                {
                    string sendingCountryCode = SendersWalletData.CardUserCountry.Trim();
                    string receivingCountryCode = ReceiversWalletData.CardUserCountry.Trim();
                    decimal exchangeRate = 0;
                    decimal smsFee = 0;

                    if (sendingCountryCode == receivingCountryCode)
                    {
                        exchangeRate = 1;
                        smsFee = Common.Common.GetSmsFee(sendingCountryCode);
                    }
                    else
                    {
                        var exchangeRateData = dbContext.ExchangeRate.Where(x => x.CountryCode1.Trim() == sendingCountryCode && x.CountryCode2.Trim() == receivingCountryCode).FirstOrDefault();
                        if (exchangeRateData != null)
                        {
                            exchangeRate = exchangeRateData.CountryRate1;
                        }
                    }
                    if (Common.CardUserSession.PayIntoAnotherWalletSession.SendSMSNotification == false)
                    {
                        smsFee = 0;
                    }
                    var paymentData = SEstimateFee.CalculateFaxingFee(amount, IncludeFaxingFee, false, exchangeRate, SEstimateFee.GetFaxingCommision(sendingCountryCode));
                    if (paymentData != null)
                    {
                        var result = new AccountPaymentSummaryViewModel()
                        {
                            Id = 0,
                            SendingCurrencySymbol = Common.Common.GetCurrencySymbol(SendersWalletData.CardUserCountry),
                            ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(ReceiversWalletData.CardUserCountry),
                            SendingCurrencyCode = Common.Common.GetCountryCurrency(SendersWalletData.CardUserCountry),
                            ReceivingCurrencyCode = Common.Common.GetCountryCurrency(ReceiversWalletData.CardUserCountry),
                            AvailableBalance = SendersWalletData.CurrentBalance,
                            Amount = amount,
                            Fee = paymentData.FaxingFee,
                            LocalSMSMessage = smsFee,
                            PayingAmount = paymentData.FaxingAmount + smsFee,
                            ReceivingAmount = paymentData.ReceivingAmount,
                            PaymentReference = reference,
                            ExchangeRate = exchangeRate
                        };
                        return result;
                    }
                }

            }
            return null;
        }

        public string getKiiPayUsersNameFromMobile(string mobileNumber)
        {
            if (!string.IsNullOrEmpty(mobileNumber))
            {
                var data = dbContext.KiiPayPersonalUserInformation.Where(x => x.MobileNo.Trim() == mobileNumber.Trim()).FirstOrDefault();
                if (data != null)
                {
                    return data.FirstName + " " + data.Lastname;
                }
            }
            return "";
        }

        public string getKiiPayPersonalUserNameFromWalletId(int walletId)
        {
            if (walletId != 0)
            {
                var data = dbContext.KiiPayPersonalWalletInformation.Find(walletId);
                if (data != null)
                {
                    return data.FirstName + " " + data.MiddleName + " " + data.LastName;
                }
            }
            return "";
        }

        public string getKiiPayPersonalWalletNumberFromWalletId(int walletId)
        {
            if (walletId != 0)
            {
                var data = dbContext.KiiPayPersonalWalletInformation.Find(walletId);
                if (data != null)
                {
                    return data.MobileNo;
                }
            }
            return "";
        }

        internal DB.KiiPayBusinessInformation GetKiipayBusinessInfo(int KiiPayBusinessId)
        {
            var result = dbContext.KiiPayBusinessInformation.Where(x => x.Id == KiiPayBusinessId).FirstOrDefault();
            return result;
        }

        public string getKiiPayBusinessName(string mobileNumber)
        {
            if (!string.IsNullOrEmpty(mobileNumber))
            {
                var data = dbContext.KiiPayBusinessInformation.Where(x => x.BusinessMobileNo.Trim() == mobileNumber.Trim()).FirstOrDefault();
                if (data != null)
                {
                    return data.BusinessName;
                }
            }
            return "";
        }
        public string KiiPayPersonalWalletUserCurrencySymbolFromWalletId(int walletId)
        {
            if (walletId != 0)
            {
                var data = dbContext.KiiPayPersonalWalletInformation.Find(walletId);
                if (data != null)
                {
                    string countryCode = data.CardUserCountry;
                    string currencySymbol = Common.Common.GetCurrencySymbol(countryCode);
                    return currencySymbol;
                }
            }
            return "";
        }

        public string KiiPayBusinessWalletUserCurrencySymbolFromWalletId(int walletId)
        {
            if (walletId != 0)
            {
                var data = dbContext.KiiPayBusinessWalletInformation.Find(walletId);
                if (data != null)
                {
                    string countryCode = data.Country;
                    string currencySymbol = Common.Common.GetCurrencySymbol(countryCode);
                    return currencySymbol;
                }
            }
            return "";
        }
        public bool sendMoneyKiiPayPersonalToKiiPayBusiness(PaymentType paymentType)
        {
            if (Common.CardUserSession.PayIntoAnotherWalletSession != null)
            {
                var paymentData = Common.CardUserSession.PayIntoAnotherWalletSession;
                var receiverInformationData = dbContext.KiiPayBusinessInformation.Where(x => x.BusinessMobileNo.Trim() == Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingPhoneNumber.Trim()).FirstOrDefault();
                var senderInformationData = dbContext.KiiPayPersonalUserInformation.Where(x => x.MobileNo.Trim() == Common.CardUserSession.LoggedCardUserViewModel.MobileNumber.Trim()).FirstOrDefault();
                var senderWalletData = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo.Trim() == Common.CardUserSession.LoggedCardUserViewModel.MobileNumber.Trim()).FirstOrDefault();
                if (receiverInformationData != null && senderInformationData != null && senderWalletData != null)
                {
                    var receiverWalletData = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == receiverInformationData.Id && x.CardStatus == CardStatus.Active).FirstOrDefault();
                    if (receiverWalletData != null)
                    {
                        //deducting amount from sender's wallet
                        senderWalletData.CurrentBalance = senderWalletData.CurrentBalance - paymentData.PayingAmount;
                        dbContext.Entry(senderWalletData).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();


                        //adding amount to receiver's wallet
                        receiverWalletData.CurrentBalance = receiverWalletData.CurrentBalance + paymentData.ReceivingAmount;
                        dbContext.Entry(receiverWalletData).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                        //inserting transaction data



                        if (paymentType == PaymentType.Local)
                        {
                            var data = new DB.KiiPayPersonalNationalKiiPayBusinessPayment()
                            {
                                KiiPayPersonalWalletInformationId = senderWalletData.Id,
                                KiiPayBusinessWalletInformationId = receiverWalletData.Id,
                                AmountSent = paymentData.PayingAmount,
                                PaymentReference = Common.CardUserSession.PayIntoAnotherWalletSession.PaymentReference,
                                TransactionDate = DateTime.Now
                            };
                            dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Add(data);
                            dbContext.SaveChanges();

                            #region Notification Section 
                            DB.Notification notification = new DB.Notification()
                            {
                                SenderId = data.KiiPayPersonalWalletInformationId,
                                ReceiverId = data.KiiPayBusinessWalletInformationId,
                                Amount = Common.Common.GetCurrencySymbol(receiverWalletData.Country) + " " + data.AmountSent,
                                CreationDate = DateTime.Now,
                                Title = DB.Title.KiiPayPersonalLocalPayment,
                                Message = "Mobile No :" + senderWalletData.MobileNo,
                                NotificationReceiver = DB.NotificationFor.KiiPayBusiness,
                                NotificationSender = DB.NotificationFor.kiiPayPersonal,
                                Name = Common.CardUserSession.LoggedCardUserViewModel.FullName,
                            };

                            SendNotification(notification);
                            #endregion

                            #region Local Wallet Statement
                            KiiPayPersonalWalletStatementServices _kiiPayPersonalWalletStatementServices = new KiiPayPersonalWalletStatementServices();
                            KiiPayPersonalWalletStatementVM KiiPayPersonalWalletStatement = new KiiPayPersonalWalletStatementVM()
                            {
                                SendingAmount = data.AmountSent,
                                Fee = 0,
                                ReceivingAmount = data.AmountSent,
                                SenderCurBal = GetKiipayPersonalWalletInfo(data.KiiPayPersonalWalletInformationId).CurrentBalance,
                                SenderCountry = Common.CardUserSession.LoggedCardUserViewModel.CountryCode,
                                TransactionDate = data.TransactionDate,
                                TransactionId = data.Id,
                                ReceiverCurBal = data.KiiPayBusinessWalletInformation.CurrentBalance,
                                ReceiverCountry = data.KiiPayBusinessWalletInformation.Country,
                                WalletStatmentType = DB.WalletStatmentType.KiiPayPersoanlPayment,
                            };

                            _kiiPayPersonalWalletStatementServices.AddkiiPayPersonalWalletStatement(KiiPayPersonalWalletStatement);
                            #endregion

                        }

                        else if (paymentType == PaymentType.International)
                        {
                            var data = new DB.KiiPayPersonalInternationalKiiPayBusinessPayment()
                            {
                                PayedFromKiiPayPersonalWalletId = senderWalletData.Id,
                                PayedToKiiPayBusinessInformationId = receiverInformationData.Id,
                                PayedToKiiPayBusinessWalletId = receiverWalletData.Id,
                                ReceivingAmount = Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingAmount,
                                FaxingAmount = Common.CardUserSession.PayIntoAnotherWalletSession.Amount,
                                FaxingFee = Common.CardUserSession.PayIntoAnotherWalletSession.Fee,
                                ExchangeRate = Common.CardUserSession.PayIntoAnotherWalletSession.ExchangeRate,
                                TotalAmount = Common.CardUserSession.PayIntoAnotherWalletSession.PayingAmount,
                                ReceiptNumber = generateTransactionReceipt(KiiPayTransactionType.PersonalToBusinessInternational),
                                PaymentReference = Common.CardUserSession.PayIntoAnotherWalletSession.PaymentReference,
                                TransactionDate = DateTime.Now
                            };
                            dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.Add(data);
                            dbContext.SaveChanges();



                            #region Notification Section 

                            DB.Notification notification = new DB.Notification()
                            {
                                SenderId = data.PayedFromKiiPayPersonalWalletId,
                                ReceiverId = data.PayedToKiiPayBusinessInformationId,
                                Amount = Common.Common.GetCurrencySymbol(receiverInformationData.BusinessCountry) + " " + data.ReceivingAmount,
                                CreationDate = DateTime.Now,
                                Title = DB.Title.KiiPayPersonalInternationalPayment,
                                Message = "Mobile No :" + senderWalletData.MobileNo,
                                NotificationReceiver = DB.NotificationFor.KiiPayBusiness,
                                NotificationSender = DB.NotificationFor.kiiPayPersonal,
                                Name = Common.CardUserSession.LoggedCardUserViewModel.FullName,
                            };

                            SendNotification(notification);
                            #endregion

                            #region International Wallet Statement
                            KiiPayPersonalWalletStatementServices _kiiPayPersonalWalletStatementServices = new KiiPayPersonalWalletStatementServices();
                            KiiPayPersonalWalletStatementVM KiiPayPersonalWalletStatement = new KiiPayPersonalWalletStatementVM()
                            {
                                SendingAmount = data.ReceivingAmount,
                                Fee = data.FaxingFee,
                                ReceivingAmount = data.ReceivingAmount,
                                SenderCurBal = GetKiipayPersonalWalletInfo(data.PayedFromKiiPayPersonalWalletId).CurrentBalance,
                                SenderCountry = Common.CardUserSession.LoggedCardUserViewModel.CountryCode,
                                TransactionDate = data.TransactionDate,
                                TransactionId = data.Id,
                                ReceiverCurBal = data.KiiPayBusinessWalletInformation.CurrentBalance,
                                ReceiverCountry = data.KiiPayBusinessWalletInformation.Country,
                                WalletStatmentType = DB.WalletStatmentType.KiiPayPersoanlPayment,
                            };

                            _kiiPayPersonalWalletStatementServices.AddkiiPayPersonalWalletStatement(KiiPayPersonalWalletStatement);
                            #endregion
                        }

                        if (Common.CardUserSession.PayIntoAnotherWalletSession.SendSMSNotification == true)
                        {
                            SmsApi smsApiServices = new SmsApi();
                            string senderName = senderInformationData.FirstName + " " + senderInformationData.Lastname;
                            string message = smsApiServices.GetVirtualAccountDepositMessage(senderName, receiverInformationData.BusinessMobileNo, paymentData.Amount.ToString(), paymentData.ReceivingAmount.ToString());
                            string mobileNumber = Common.Common.GetCountryPhoneCode(receiverInformationData.BusinessCountry) + receiverInformationData.BusinessMobileNo;
                            smsApiServices.SendSMS(mobileNumber, message);
                        }



                        return true;
                    }
                }
            }
            return false;
        }
        public bool sendMoneyKiiPayPersonal()
        {
            if (Common.CardUserSession.PayIntoAnotherWalletSession != null)
            {
                var paymentSessionData = Common.CardUserSession.PayIntoAnotherWalletSession;
                var receiverUserData = dbContext.KiiPayPersonalUserInformation.Where(x => x.MobileNo.Trim() == Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingPhoneNumber.Trim()).FirstOrDefault();
                var senderUserData = dbContext.KiiPayPersonalUserInformation.Where(x => x.MobileNo.Trim() == Common.CardUserSession.PayIntoAnotherWalletSession.SendingPhoneNumber.Trim()).FirstOrDefault();
                var receiverWalletData = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo.Trim() == Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingPhoneNumber.Trim()).FirstOrDefault();
                var senderWalletData = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo.Trim() == Common.CardUserSession.PayIntoAnotherWalletSession.SendingPhoneNumber.Trim()).FirstOrDefault();
                if (receiverUserData != null && senderUserData != null && receiverWalletData != null && senderWalletData != null)
                {

                    //deducting amount from sender's wallet
                    senderWalletData.CurrentBalance = senderWalletData.CurrentBalance - paymentSessionData.PayingAmount;
                    dbContext.Entry(senderWalletData).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    //adding amount to receiver's wallet
                    receiverWalletData.CurrentBalance = receiverWalletData.CurrentBalance + paymentSessionData.ReceivingAmount;
                    dbContext.Entry(receiverWalletData).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();

                    //main transaction data
                    var data = new DB.KiiPayPersonalWalletPaymentByKiiPayPersonal()
                    {
                        SenderId = senderUserData.Id,
                        SenderWalletId = senderWalletData.Id,
                        ReceiverWalletId = receiverWalletData.Id,
                        ReceivingMobileNumber = receiverWalletData.MobileNo,
                        RecievingAmount = paymentSessionData.ReceivingAmount,
                        FaxingAmount = paymentSessionData.Amount,
                        FaxingFee = paymentSessionData.Fee,
                        ExchangeRate = paymentSessionData.ExchangeRate,
                        TotalAmount = paymentSessionData.PayingAmount,
                        PaymentReference = paymentSessionData.PaymentReference,
                        PaymentType = paymentSessionData.PaymentType,
                        ReceiptNumber = "",
                        TransactionDate = DateTime.Now
                    };
                    dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.Add(data);
                    dbContext.SaveChanges();
                    if (Common.CardUserSession.PayIntoAnotherWalletSession.SendSMSNotification == true)
                    {
                        SmsApi smsApiServices = new SmsApi();
                        string senderName = senderUserData.FirstName + " " + senderUserData.Lastname;
                        string message = smsApiServices.GetVirtualAccountDepositMessage(senderName, receiverWalletData.MobileNo, paymentSessionData.PayingAmount.ToString(), paymentSessionData.ReceivingAmount.ToString());
                        string mobileNumber = Common.Common.GetCountryPhoneCode(receiverUserData.Country) + receiverWalletData.MobileNo;
                        smsApiServices.SendSMS(mobileNumber, message);
                    };
                    Common.CardUserSession.LoggedCardUserViewModel.BalanceOnCard = getAvailableBalance();

                    #region Notification Section 

                    DB.Notification notification = new DB.Notification()
                    {
                        SenderId = data.SenderWalletId,
                        ReceiverId = data.ReceiverWalletId,
                        Amount = Common.Common.GetCurrencySymbol(receiverUserData.Country) + " " + data.RecievingAmount,
                        CreationDate = DateTime.Now,
                        Title = data.PaymentType == PaymentType.International ? DB.Title.KiiPayPersonalInternationalPayment : DB.Title.KiiPayPersonalLocalPayment,
                        Message = "Mobile No :" + senderUserData.MobileNo,
                        NotificationReceiver = DB.NotificationFor.kiiPayPersonal,
                        NotificationSender = DB.NotificationFor.kiiPayPersonal,
                        Name = Common.CardUserSession.LoggedCardUserViewModel.FullName,
                    };

                    SendNotification(notification);
                    #endregion

                    #region Wallet Statement Section of both Local And International 

                    KiiPayPersonalWalletStatementServices _kiiPayPersonalWalletStatementServices = new KiiPayPersonalWalletStatementServices();
                    KiiPayPersonalWalletStatementVM KiiPayPersonalWalletStatement = new KiiPayPersonalWalletStatementVM()
                    {
                        SendingAmount = data.RecievingAmount,
                        Fee = data.FaxingFee,
                        ReceivingAmount = data.RecievingAmount,
                        SenderCurBal = GetKiipayPersonalWalletInfo(data.SenderWalletId).CurrentBalance,
                        SenderCountry = Common.CardUserSession.LoggedCardUserViewModel.CountryCode,
                        TransactionDate = data.TransactionDate,
                        TransactionId = data.Id,
                        ReceiverCurBal = GetKiipayPersonalWalletInfo(data.ReceiverWalletId).CurrentBalance,
                        ReceiverCountry = GetKiipayPersonalWalletInfo(data.ReceiverWalletId).CardUserCountry,
                        WalletStatmentType = DB.WalletStatmentType.KiiPayPersoanlPayment,
                    };

                    _kiiPayPersonalWalletStatementServices.AddkiiPayPersonalWalletStatement(KiiPayPersonalWalletStatement);
                    #endregion
                    return true;
                }
            }
            return false;
        }


        public void SendNotification(DB.Notification notification)
        {

            SNotification _notificationServices = new SNotification();
            var result = _notificationServices.SaveNotification(notification);

            string hourago = result.CreationDate.CalulateTimeAgo();
            //HubController.SendToDashBoard(result.ReceiverId.ToString(), result.Name ,result.Message ,  result.Amount , result.CreationDate.ToString() , hourago , null);
            HubController.SendToKiiPayBusiness(result.ReceiverId.ToString(), result.Id.ToString(), result.Name, result.Message, result.Amount, result.CreationDate.ToString(), hourago, null);
        }
        public decimal getAvailableBalance()
        {
            var userData = dbContext.KiiPayPersonalUserInformation.Where(x => x.Id == Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId).FirstOrDefault();
            if (userData != null)
            {
                decimal balance = userData.KiiPayPersonalWalletInformation.CurrentBalance;
                return balance;
            }
            return 0;
        }


    }

    public enum KiiPayTransactionType
    {
        PersonalToPersonalNational,
        PersonalToPersonalInternational,
        PersonalToBusinessNational,
        PersonalToBusinessInternational
    }

}
