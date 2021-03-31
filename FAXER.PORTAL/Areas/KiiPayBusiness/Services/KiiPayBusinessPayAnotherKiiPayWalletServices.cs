using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Services
{
    public class KiiPayBusinessPayAnotherKiiPayWalletServices
    {
        DB.FAXEREntities dbContext = null;

        public KiiPayBusinessPayAnotherKiiPayWalletServices()
        {
            dbContext = new DB.FAXEREntities();

        }


        public bool IsValidMobileNo(string MobileNo)
        {

            var KiiPayWalletInfo = GetKiiPayPersonalWalletInfoByMobileNo(MobileNo);
            if (KiiPayWalletInfo == null)
            {
                return false;
            }
            return true;
        }

        public bool IsValidTransfer(string MobileNo, bool IsLocalPayment)
        {

            string SendingCountry = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode;
            string ReceivingCountry = "";
            var KiiPayWalletInfo = GetKiiPayPersonalWalletInfoByMobileNo(MobileNo);

            if (KiiPayWalletInfo == null)
            {
                return false;
            }
            ReceivingCountry = KiiPayWalletInfo.CardUserCountry;
            if (!IsLocalPayment && ReceivingCountry == SendingCountry)
            {
                return false;
            }


            KiiPayBusinessPaymentSummaryVM vm = new KiiPayBusinessPaymentSummaryVM()
            {
                ReceiverWalletId = KiiPayWalletInfo.Id,
                ReceiverName = KiiPayWalletInfo.FirstName + " " + KiiPayWalletInfo.MiddleName + " " + KiiPayWalletInfo.LastName,
                ExchangeRate = SExchangeRate.GetExchangeRateValue(SendingCountry, ReceivingCountry),
                SmsFee = Common.Common.GetSmsFee(ReceivingCountry),
                ReceivingCountryCurrency = Common.Common.GetCountryCurrency(ReceivingCountry),
                ReceivingCountryCurrencySymbol = Common.Common.GetCurrencySymbol(ReceivingCountry),
                SendingCountryCurrency = Common.Common.GetCountryCurrency(SendingCountry),
                SendingCountryCurrencySymbol = Common.Common.GetCurrencySymbol(SendingCountry)
            };
            SetKiiPersonalPaymentSummary(vm);


            return true;
        }
        public DB.KiiPayPersonalWalletInformation GetKiiPayPersonalWalletInfoByMobileNo(string MobileNo)
        {

            var kiiPayPersonalWalletInformation = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == MobileNo).FirstOrDefault();
            return kiiPayPersonalWalletInformation;
        }

        public DB.CardStatus? GetKiiPayPersonalWalletCardStatus(string MobileNo) { 

            if (GetKiiPayPersonalWalletInfoByMobileNo(MobileNo) == null)
            {
                return null;
            }
            var cardStatus = GetKiiPayPersonalWalletInfoByMobileNo(MobileNo).CardStatus;
            return cardStatus;
        }

        public void SetKiiPersonalPaymentSummary(KiiPayBusinessPaymentSummaryVM vm)
        {

            Common.BusinessSession.KiiPayBusinessPaymentSummary = vm;
        }


        public ViewModels.KiiPayBusinessPaymentSummaryVM GetKiiPersonalPaymentSummary()
        {

            ViewModels.KiiPayBusinessPaymentSummaryVM vm = new ViewModels.KiiPayBusinessPaymentSummaryVM();
            if (Common.BusinessSession.KiiPayBusinessPaymentSummary != null)
            {
                vm = Common.BusinessSession.KiiPayBusinessPaymentSummary;
            }
            return vm;
        }

        #region Local Payment 


        public ViewModels.KiiPayBusinessPaymentCompletedVM GetLocalPaymentCompletedSummary()
        {
            // Param Is Local Payment true
            var result = CompletePayment(true);

            return result;
        }

        public void SetPayingAmountForLocalPaymentInfo(KiiPayBusinessLocalPaymentPayingAmountDetailsVM vm)
        {

            var result = GetKiiPersonalPaymentSummary();
            decimal SMSFee = 0M;
            if (vm.SendSms == true)
            {
                Common.BusinessSession.SendSMSForLocalPayment = vm.SendSms;
                SMSFee = result.SmsFee;
            }
            result.SendingAmount = vm.SendingAmount;
            result.ReceivingAmount = vm.SendingAmount;
            result.TotalAmount = vm.SendingAmount + SMSFee;
            result.PaymentReference = vm.PaymentReference;
            SetKiiPersonalPaymentSummary(result);

        }
        public KiiPayBusinessLocalPaymentPayingAmountDetailsVM GetPayingAmountForLocalPaymentInfo()
        {

            var summary = GetKiiPersonalPaymentSummary();

            KiiPayBusinessLocalPaymentPayingAmountDetailsVM vm = new KiiPayBusinessLocalPaymentPayingAmountDetailsVM()
            {
                PaymentReference = summary.PaymentReference,
                ReceiverName = summary.ReceiverName,
                SendingAmount = summary.SendingAmount,
                SendingCountryCurrency = summary.SendingCountryCurrency,
                SendingCountryCurrencySymbol = summary.SendingCountryCurrencySymbol,
            };
            return vm;

        }
        #endregion
        #region International payment 
        public ViewModels.KiiPayBusinessPaymentCompletedVM GetInternationalPaymentCompletedSummary()
        {

            // Param Is Local Payment true
            var result = CompletePayment(false);
            return result;
        }


        public void SetKiiPayBusinessInternationalPaymentPayingAmountDetails(KiiPayBusinessInternationalPaymentPayingAmountDetailsVM vm)
        {

            var summary = GetKiiPersonalPaymentSummary();
            summary.PaymentReference = vm.PaymentReference;

        }

        public KiiPayBusinessInternationalPaymentPayingAmountDetailsVM GetKiiPayBusinessInternationalPaymentPayingAmountDetails()
        {
            KiiPayBusinessInternationalPaymentPayingAmountDetailsVM vm = new KiiPayBusinessInternationalPaymentPayingAmountDetailsVM();
            var summary = GetKiiPersonalPaymentSummary();

            if (summary != null)
            {
                vm = new KiiPayBusinessInternationalPaymentPayingAmountDetailsVM()
                {
                    ExchangeRate = summary.ExchangeRate,
                    Fee = summary.Fee,
                    PaymentReference = summary.PaymentReference,
                    ReceiverName = summary.ReceiverName,
                    ReceivingAmount = summary.ReceivingAmount,
                    ReceivingCountryCurrency = summary.ReceivingCountryCurrency,
                    ReceivingCountryCurrencySymbol = summary.ReceivingCountryCurrencySymbol,
                    SendingAmount = summary.SendingAmount,
                    SendingCountryCurrency = summary.SendingCountryCurrency,
                    SendingCountryCurrencySymbol = summary.SendingCountryCurrencySymbol,
                    TotalAmount = summary.TotalAmount
                };
            }
            return vm;


        }


        #endregion


        public KiiPayBusinessPaymentCompletedVM CompletePayment(bool IsLocalPayment)
        {

                int BusinessId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId;
                var paymentSummary = GetKiiPersonalPaymentSummary();

                // TO DO Complete the model binding 

                KiiPayBusinessCommonServices _KiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();

                DB.KiiPayPersonalWalletPaymentByKiiPayBusiness model = new DB.KiiPayPersonalWalletPaymentByKiiPayBusiness()
                {
                    KiiPayBusinessInformationId = BusinessId,
                    KiiPayBusinessWalletInformationId = _KiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId(BusinessId).Id,
                    KiiPayPersonalWalletInformationId = paymentSummary.ReceiverWalletId,
                    ExchangeRate = 1,
                    Fee = 0,
                    PayingAmount = paymentSummary.SendingAmount,
                    PaymentReference = paymentSummary.PaymentReference,
                    RecievingAmount = paymentSummary.ReceivingAmount,
                    TotalAmount = paymentSummary.TotalAmount,
                    PaymentType = IsLocalPayment == true ? DB.PaymentType.Local : DB.PaymentType.International,
                    ReceiptNumber = "",
                    TransactionDate = DateTime.Now
                };
                var result = SaveKiiPayPersonalInternationalByKiiPayBusinessPayment(model);

                ViewModels.KiiPayBusinessPaymentCompletedVM vm = new ViewModels.KiiPayBusinessPaymentCompletedVM()
                {
                    SendingAmount = result.PayingAmount,
                    ReceivingAmount = result.RecievingAmount,
                    ReceiverName = paymentSummary.ReceiverName,
                    SendingCurrencySymbol = paymentSummary.SendingCountryCurrencySymbol,
                    ReceivingCurrencySymbol = paymentSummary.ReceivingCountryCurrencySymbol

                };

                // Balance In To Account
                _KiiPayBusinessCommonServices.BalanceOut(result.KiiPayBusinessWalletInformationId, result.PayingAmount);

                // Balance Out from Account
                Common.Common.KiiPayPersonalWalletBalanceIN(result.KiiPayPersonalWalletInformationId, result.RecievingAmount);


                _KiiPayBusinessCommonServices.UpdateAccountBalance();


                #region Wallet Statement Add 

                KiiPayBusinessWalletStatementServices _kiiPayBusinessWalletStatementServices = new KiiPayBusinessWalletStatementServices();
                KiiPayBusinessWalletStatementVM KiiPayBusinessWalletStatement = new KiiPayBusinessWalletStatementVM()
                {
                    SendingAmount = result.PayingAmount,
                    Fee = model.Fee,
                    ReceivingAmount = model.RecievingAmount,
                    SenderCurBal = _KiiPayBusinessCommonServices.GetAccountBalanceByWalletId((int)result.KiiPayBusinessWalletInformationId),
                    SenderCountry = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode,
                    TransactionDate = result.TransactionDate,
                    TransactionId = result.Id,
                    ReceiverCurBal = _KiiPayBusinessCommonServices.GetPersonalAccountBalanceByWalletId((int)result.KiiPayPersonalWalletInformationId).CurrentBalance,
                    //result.KiiPayPersonalWalletInformation.CurrentBalance,
                    ReceiverCountry = _KiiPayBusinessCommonServices.GetPersonalAccountBalanceByWalletId((int)result.KiiPayPersonalWalletInformationId).CardUserCountry,
                    WalletStatmentType = DB.WalletStatmentType.KiiPayPersoanlPayment,
                };

                _kiiPayBusinessWalletStatementServices.AddkiiPayBusinessWalletStatement(KiiPayBusinessWalletStatement);

                #endregion

                // Send SMS 
                if (IsLocalPayment) {

                    var senderInfo = _KiiPayBusinessCommonServices.GetKiipayBusinessWalletInfo(result.KiiPayBusinessWalletInformationId);
                    var receiverInfo = _KiiPayBusinessCommonServices.GetKiipayBusinessWalletInfo(result.KiiPayPersonalWalletInformationId);
                    if (Common.BusinessSession.SendSMSForLocalPayment)
                    {
                        Models.KiiPayPersonalPaymentSMSVM smsModel = new Models.KiiPayPersonalPaymentSMSVM()
                        {
                            SenderName = senderInfo.KiiPayBusinessInformation.BusinessName,
                            ReceiverCountry = receiverInfo.Country,
                            ReceiverPhoneNo = receiverInfo.MobileNo,
                            ReceivingAmount = result.RecievingAmount.ToString(),
                            SendingAmount = result.PayingAmount.ToString(),
                            SenderCountry = senderInfo.KiiPayBusinessInformation.BusinessCountry,
                            SenderPhoneNo = senderInfo.KiiPayBusinessInformation.BusinessMobileNo
                        };
                        SendLocalPaymentSms(smsModel);
                    }
                }
                // Clear All payment Session 
                ClearSession();

                return vm;

        }
        public DB.KiiPayPersonalWalletPaymentByKiiPayBusiness SaveKiiPayPersonalInternationalByKiiPayBusinessPayment(DB.KiiPayPersonalWalletPaymentByKiiPayBusiness model)
        {

            dbContext.KiiPayPersonalWalletPaymentByKiiPayBusiness.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public void SendLocalPaymentSms(Models.KiiPayPersonalPaymentSMSVM model) {

            SmsApi smsApiServices = new SmsApi();
            smsApiServices.SendKiiPayPersonalPaymentSMS(model);

        } 

        public void ClearSession()
        {

            HttpContext.Current.Session.Remove("KiiPayBusinessPaymentSummary");
            Common.BusinessSession.SendSMSForLocalPayment = false;
            //HttpContext.Current.Session.Remove("KiiPayBusinessPaymentSummary");

        }

     

    }
}