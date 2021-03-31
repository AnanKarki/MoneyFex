using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Services
{
    public class KiiPayBusinessPayForServicesServices
    {
        DB.FAXEREntities dbContext = null;
        Services.KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = null;
        
        public KiiPayBusinessPayForServicesServices()
        {
            dbContext = new DB.FAXEREntities();
            _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();
            
        }
        #region Local Transaction 
        
        public void KiiPayBusinessLocalSearchBusinessProviderSuccessFul(KiiPayBusinessSearchBusinessProviderVM vm)
        {

            var data = GetKiiPayBusinessInfoByMobileNo(vm.MobileNo);
            string SendingCountry = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode;
            KiiPayBusinessLocalTransferEnterAmountVM EnterAmountVM = new KiiPayBusinessLocalTransferEnterAmountVM();
            EnterAmountVM.BusinessName = data.KiiPayBusinessInformation.BusinessName;
            EnterAmountVM.CurrencyCode = Common.Common.GetCountryCurrency(data.KiiPayBusinessInformation.BusinessCountry);
            EnterAmountVM.CurrencySymbol = Common.Common.GetCurrencySymbol(data.KiiPayBusinessInformation.BusinessCountry);
            SetKiiPayBusinessLocallTransferEnterAmountVM(EnterAmountVM);
        }

        public KiiPayBusinessLocalTransferEnterAmountVM GetKiiPayBusinessLocalTransferEnterAmountVM()
        {

            KiiPayBusinessLocalTransferEnterAmountVM vm = new KiiPayBusinessLocalTransferEnterAmountVM();

            if (Common.BusinessSession.KiiPayBusinessLocalTransferAmount != null)
            {

                vm = Common.BusinessSession.KiiPayBusinessLocalTransferAmount;
            };

            return vm;
        }
        public void SetKiiPayBusinessLocallTransferEnterAmountVM(KiiPayBusinessLocalTransferEnterAmountVM vm)
        {

            Common.BusinessSession.KiiPayBusinessLocalTransferAmount = vm;
        }


        public KiiPayBusinessPaymentSummaryVM GetLocalTransferPaymentSummary()
        {


            var data = GetKiiPayBusinessLocalTransferEnterAmountVM();
            KiiPayBusinessPaymentSummaryVM vm = new KiiPayBusinessPaymentSummaryVM();
            vm.PaymentReference = data.PaymentReference;
            vm.ReceiverName = data.BusinessName;
            vm.ReceivingAmount = data.Amount;
            vm.SendingAmount = data.Amount;
            vm.ReceivingCountryCurrency = data.CurrencyCode;
            vm.ReceivingCountryCurrencySymbol = data.CurrencySymbol;
            vm.SendingCountryCurrency = data.CurrencyCode;
            vm.SendingCountryCurrencySymbol = data.CurrencySymbol;
            vm.SmsFee = 0;
            vm.TotalAmount = data.Amount;
            vm.PaymentReference = data.PaymentReference;

            return vm;
        }
  

        public KiiPayBusinessCompleteLocalTransferVM CompleteLocalTransfer()
        {

            var paymentSummary = Common.BusinessSession.KiiPayBusinessLocalTransferAmount;

            int PayingFromBusinessID = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId;
            DB.KiiPayBusinessLocalTransaction model = new DB.KiiPayBusinessLocalTransaction()
            {
                AmountSent = paymentSummary.Amount,
                PayedFromKiiPayBusinessInformationId = PayingFromBusinessID,
                PayedFromKiiPayBusinessWalletInformationId = _kiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId(PayingFromBusinessID).Id,
                PayedToKiiPayBusinessInformationId = GetPayingToKiiPayBusinessInfoVM().BusinessId,
                PayedToKiiPayBusinessWalletInformationId = GetPayingToKiiPayBusinessInfoVM().WalletId,
                PaymentReference = paymentSummary.PaymentReference,
                TransactionDate = DateTime.Now,

            };
            var result = SaveKiiPayBusinessLocalPaymentTransaction(model);

            ///ToAccount Balance Add 
            _kiiPayBusinessCommonServices.BalanceIn(model.PayedToKiiPayBusinessWalletInformationId, model.AmountSent);
            ///FromAccount Balance Less 
            _kiiPayBusinessCommonServices.BalanceOut((int)model.PayedFromKiiPayBusinessWalletInformationId, model.AmountSent);


            _kiiPayBusinessCommonServices.UpdateAccountBalance();

            #region Wallet Statement Add 

            KiiPayBusinessWalletStatementServices _kiiPayBusinessWalletStatementServices = new KiiPayBusinessWalletStatementServices();
            KiiPayBusinessWalletStatementVM KiiPayBusinessWalletStatement = new KiiPayBusinessWalletStatementVM()
            {
                SendingAmount = model.AmountSent,
                Fee = 0,
                ReceivingAmount = model.AmountSent,
                SenderCurBal = _kiiPayBusinessCommonServices.GetAccountBalanceByWalletId((int)result.PayedFromKiiPayBusinessWalletInformationId),
                SenderCountry = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode,
                TransactionDate = result.TransactionDate,
                TransactionId = result.Id,
                ReceiverCurBal  = _kiiPayBusinessCommonServices.GetAccountBalanceByWalletId((int)result.PayedToKiiPayBusinessWalletInformationId),
                ReceiverCountry = _kiiPayBusinessCommonServices.GetCountryOfBusinessByBusinessInfoID(result.PayedToKiiPayBusinessInformationId),
                WalletStatmentType = DB.WalletStatmentType.BusinessNationalPayment,
            };

            _kiiPayBusinessWalletStatementServices.AddkiiPayBusinessWalletStatement(KiiPayBusinessWalletStatement);

            #endregion
            KiiPayBusinessCompleteLocalTransferVM vm = new KiiPayBusinessCompleteLocalTransferVM();
            vm.BusinessName = paymentSummary.BusinessName;
            vm.ReceivingAmount = paymentSummary.Amount;
            vm.CurrencySymbol = paymentSummary.CurrencySymbol;

            #region Notification Section 

            DB.Notification notification = new DB.Notification()
            {
                SenderId = model.PayedFromKiiPayBusinessInformationId,
                ReceiverId = model.PayedToKiiPayBusinessInformationId,
                Amount = paymentSummary.CurrencySymbol + " " + model.AmountSent,
                CreationDate = DateTime.Now,
                Title =DB.Title.BusinessLocalPayment,
                Message = "Business Mobile No :" + Common.BusinessSession.LoggedKiiPayBusinessUserInfo.BusinessMobileNo,
                NotificationReceiver = DB.NotificationFor.KiiPayBusiness,
                NotificationSender = DB.NotificationFor.KiiPayBusiness,
                Name = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.BusinessName,
            };

            _kiiPayBusinessCommonServices.SendNotification(notification);
            #endregion

            if (Common.BusinessSession.SendSMSForLocalPayment)
            {

                var senderInfo = _kiiPayBusinessCommonServices.GetKiipayBusinessWalletInfo((int)result.PayedFromKiiPayBusinessWalletInformationId);
                var receiverInfo = _kiiPayBusinessCommonServices.GetKiipayBusinessWalletInfo(result.PayedToKiiPayBusinessWalletInformationId);
                Models.KiiPayBusinessPaymentSmsVM smsModel = new Models.KiiPayBusinessPaymentSmsVM()
                {
                    SenderName = senderInfo.KiiPayBusinessInformation.BusinessName,
                    ReceiverBusinessMobileNo = receiverInfo.KiiPayBusinessInformation.BusinessMobileNo,
                    ReceiverBusinessName = receiverInfo.KiiPayBusinessInformation.BusinessName,
                    PaymentReference = result.PaymentReference,
                    ReceivingAmount = result.AmountSent.ToString(),
                    SendingAmount = result.AmountSent.ToString(),
                    SenderCountry = senderInfo.KiiPayBusinessInformation.BusinessCountry,
                    ReceiverCountry = receiverInfo.KiiPayBusinessInformation.BusinessCountry,
                    SenderPhoneNo = senderInfo.KiiPayBusinessInformation.BusinessMobileNo
                };

                SendLocalPaymentSMS(smsModel);
            }


          
            // Clear Payment Session
            ClearSession();

            return vm;


        }

        public void SendLocalPaymentSMS(Models.KiiPayBusinessPaymentSmsVM vm)
        {

            SmsApi smsApiServices = new SmsApi();
            smsApiServices.SendKiiPayBusinessPaymentSMS(vm);

        }
        public DB.KiiPayBusinessLocalTransaction SaveKiiPayBusinessLocalPaymentTransaction(DB.KiiPayBusinessLocalTransaction model)
        {
            dbContext.KiiPayBusinessLocalTransaction.Add(model);
            dbContext.SaveChanges();
            return model;


        }
        #endregion

        #region International


        public void KiiPayBusinessInternationalSearchBusinessProviderSuccessFul(KiiPayBusinessInternationalSearchBusinessProviderVM vm)
        {

            var data = GetKiiPayBusinessInfoByMobileNo(vm.MobileNo);
            string SendingCountry = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode;
            string ReceivingCountry = data.KiiPayBusinessInformation.BusinessCountry;
            KiiPayBusinessInternationalTransferEnterAmountVM EnterAmountVM = new KiiPayBusinessInternationalTransferEnterAmountVM();
            EnterAmountVM.BusinessName = data.KiiPayBusinessInformation.BusinessName;
            EnterAmountVM.RecevingCurrencyCode = Common.Common.GetCountryCurrency(data.KiiPayBusinessInformation.BusinessCountry);
            EnterAmountVM.RecevingCurrencySymbol = Common.Common.GetCurrencySymbol(data.KiiPayBusinessInformation.BusinessCountry);
            EnterAmountVM.SendingCurrencyCode = Common.Common.GetCountryCurrency(Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode);
            EnterAmountVM.SendingCurrencySymbol = Common.Common.GetCurrencySymbol(Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode);
            EnterAmountVM.ExchangeRate = SExchangeRate.GetExchangeRateValue(SendingCountry, ReceivingCountry);
            SetKiiPayBusinessInternationalTransferEnterAmountVM(EnterAmountVM);
        }

        public List<RecentPaidBusinessesDropDownVM> GetInternationalRecentPaidBusinesses()
        {

            List<RecentPaidBusinessesDropDownVM> internationalrecentPaidBusinesses = new List<RecentPaidBusinessesDropDownVM>();
            return internationalrecentPaidBusinesses;

        }
        public KiiPayBusinessPaymentSummaryVM GetInternationalTransferPaymentSummary()
        {


            var data = GetKiiPayBusinessInternationalTransferEnterAmountVM();
            KiiPayBusinessPaymentSummaryVM vm = new KiiPayBusinessPaymentSummaryVM();
            vm.ExchangeRate = data.ExchangeRate;
            vm.Fee = data.Fee;
            vm.PaymentReference = data.PaymentReference;
            vm.ReceiverName = data.BusinessName;
            vm.ReceivingAmount = data.RecevingAmount;
            vm.SendingAmount = data.SendingAmount;
            vm.ReceivingCountryCurrency = data.RecevingCurrencyCode;
            vm.ReceivingCountryCurrencySymbol = data.RecevingCurrencySymbol;
            vm.SendingCountryCurrency = data.SendingCurrencyCode;
            vm.SendingCountryCurrencySymbol = data.SendingCurrencySymbol;
            vm.SmsFee = 0;
            vm.TotalAmount = data.TotalAmount;
            vm.PaymentReference = data.PaymentReference;

            return vm;

        }




        public void SetKiiPayBusinessInternationalTransferEnterAmountVM(KiiPayBusinessInternationalTransferEnterAmountVM vm)
        {

            Common.BusinessSession.KiiPayBusinessInternationalTransferAmount = vm;
        }



        public KiiPayBusinessInternationalTransferEnterAmountVM GetKiiPayBusinessInternationalTransferEnterAmountVM()
        {

            KiiPayBusinessInternationalTransferEnterAmountVM vm = new KiiPayBusinessInternationalTransferEnterAmountVM();

            if (Common.BusinessSession.KiiPayBusinessInternationalTransferAmount != null)
            {

                vm = Common.BusinessSession.KiiPayBusinessInternationalTransferAmount;
            };

            return vm;
        }

        public KiiPayBusinessCompleteInternationalTransferVM CompleteInternationalTransfer()
        {

            var paymentSummary = Common.BusinessSession.KiiPayBusinessInternationalTransferAmount;

            int PayingFromBusinessID = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId;
            DB.KiiPayBusinessInternationalPaymentTransaction model = new DB.KiiPayBusinessInternationalPaymentTransaction()
            {
                FaxingAmount = paymentSummary.SendingAmount,
                ExchangeRate = paymentSummary.ExchangeRate,
                FaxingFee = paymentSummary.Fee,
                PaymentReference = paymentSummary.PaymentReference,
                ReceiptNumber = "",
                RecievingAmount = paymentSummary.RecevingAmount,
                TotalAmount = paymentSummary.TotalAmount,
                TransactionDate = DateTime.Now,
                PayedFromKiiPayBusinessInformationId = PayingFromBusinessID,
                PayedToKiiPayBusinessInformationId = GetPayingToKiiPayBusinessInfoVM().BusinessId,
                PayedToKiiPayBusinessWalletId = GetPayingToKiiPayBusinessInfoVM().WalletId,
                PayedFromKiiPayBusinessWalletId = _kiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId(PayingFromBusinessID).Id
            };
            var result = SaveKiiPayBusinessInternationalPaymentTransaction(model);



            ///ToAccount Balance Add 
            _kiiPayBusinessCommonServices.BalanceIn(model.PayedToKiiPayBusinessWalletId, model.RecievingAmount);
            ///FromAccount Balance Less 
            _kiiPayBusinessCommonServices.BalanceOut(model.PayedFromKiiPayBusinessWalletId, model.TotalAmount);


            _kiiPayBusinessCommonServices.UpdateAccountBalance();


            #region Wallet Statement Add 

            KiiPayBusinessWalletStatementServices _kiiPayBusinessWalletStatementServices = new KiiPayBusinessWalletStatementServices();
            KiiPayBusinessWalletStatementVM KiiPayBusinessWalletStatement = new KiiPayBusinessWalletStatementVM()
            {
                SendingAmount = model.FaxingAmount,
                Fee = model.FaxingFee,
                ReceivingAmount = model.RecievingAmount,
                SenderCurBal = _kiiPayBusinessCommonServices.GetAccountBalanceByWalletId((int)result.PayedFromKiiPayBusinessWalletId),
                SenderCountry = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode,
                TransactionDate = result.TransactionDate,
                TransactionId = result.Id,
                ReceiverCurBal = _kiiPayBusinessCommonServices.GetAccountBalanceByWalletId((int)result.PayedToKiiPayBusinessWalletId),
                ReceiverCountry = _kiiPayBusinessCommonServices.GetCountryOfBusinessByBusinessInfoID(result.PayedToKiiPayBusinessInformationId),
                WalletStatmentType = DB.WalletStatmentType.BusinessInternationalPayment,
            };

            _kiiPayBusinessWalletStatementServices.AddkiiPayBusinessWalletStatement(KiiPayBusinessWalletStatement);

            #endregion




            KiiPayBusinessCompleteInternationalTransferVM vm = new KiiPayBusinessCompleteInternationalTransferVM();
            vm.BusinessName = paymentSummary.BusinessName;
            vm.ReceivingAmount = paymentSummary.RecevingAmount;
            vm.CurrencySymbol = paymentSummary.RecevingCurrencySymbol;

            #region Notification Section 

            DB.Notification notification = new DB.Notification()
            {
                SenderId = model.PayedFromKiiPayBusinessInformationId,
                ReceiverId = model.PayedToKiiPayBusinessInformationId,
                Amount = paymentSummary.RecevingCurrencySymbol + " " + model.RecievingAmount,
                CreationDate = DateTime.Now,
                Title = DB.Title.BusinessInternationalPayment,
                Message =  "Business Mobile No :" + Common.BusinessSession.LoggedKiiPayBusinessUserInfo.BusinessMobileNo,
                NotificationReceiver = DB.NotificationFor.KiiPayBusiness,
                NotificationSender = DB.NotificationFor.KiiPayBusiness,
                Name = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.BusinessName,
            };

            _kiiPayBusinessCommonServices.SendNotification(notification);
            #endregion


            ClearSession();
            return vm;


        }
        public DB.KiiPayBusinessInternationalPaymentTransaction SaveKiiPayBusinessInternationalPaymentTransaction(DB.KiiPayBusinessInternationalPaymentTransaction model)
        {
            dbContext.KiiPayBusinessInternationalPaymentTransaction.Add(model);
            dbContext.SaveChanges();
            return model;


        }

        #endregion

        public bool IsValidMobileNo(string MobileNo)
        {

            var KiiPayWalletInfo = GetKiiPayBusinessInfoByMobileNo(MobileNo);
            if (KiiPayWalletInfo == null)
            {

                return false;
            }
            return true;
        }

        public bool IsValidTransfer(string MobileNo, bool IsLocalPayment)
        {

            var KiiPayWalletInfo = GetKiiPayBusinessInfoByMobileNo(MobileNo);
            if (KiiPayWalletInfo == null)
            {
                return false;
            }
            else if ((IsLocalPayment == false) && KiiPayWalletInfo.KiiPayBusinessInformation.BusinessCountry.ToLower() == Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode.ToLower())
            {

                return false;
            }
            PayingToKiiPayBusinessInfoVM payBusinessInfoVM = new PayingToKiiPayBusinessInfoVM()
            {
                BusinessId = KiiPayWalletInfo.KiiPayBusinessInformationId,
                WalletId = KiiPayWalletInfo.Id
            };
            SetPayingToKiiPayBusinessInfoVM(payBusinessInfoVM);


            return true;
        }


        public DB.CardStatus? GetCardStatus(string MobileNo)
        {

            if (GetKiiPayBusinessInfoByMobileNo(MobileNo) == null) {

                return null;
            }

            var cardStatus = _kiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId(GetKiiPayBusinessInfoByMobileNo(MobileNo).KiiPayBusinessInformationId).CardStatus;
            return cardStatus;

        }
        public void SetPayingToKiiPayBusinessInfoVM(PayingToKiiPayBusinessInfoVM vm)
        {

            Common.BusinessSession.PayingToKiiPayBusinessInfo = vm;
        }
        public PayingToKiiPayBusinessInfoVM GetPayingToKiiPayBusinessInfoVM()
        {
            var result = Common.BusinessSession.PayingToKiiPayBusinessInfo;
            return result;
        }
        public DB.KiiPayBusinessWalletInformation GetKiiPayBusinessInfoByMobileNo(string MobileNo)
        {

            var data = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformation.BusinessMobileNo == MobileNo).FirstOrDefault();
            return data;

        }


        public void ClearSession()
        {

            HttpContext.Current.Session.Remove("PayingToKiiPayBusinessInfo");
            HttpContext.Current.Session.Remove("KiiPayBusinessInternationalTransferAmount");
            HttpContext.Current.Session.Remove("KiiPayBusinessLocalTransferAmount");
            Common.BusinessSession.SendSMSForLocalPayment = false;
        }



        
    }
}