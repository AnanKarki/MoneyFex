using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Services
{
    public class KiiPayBusinessTopUpAnAccountServices
    {
        DB.FAXEREntities dbContext = null;
        public KiiPayBusinessTopUpAnAccountServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        #region Local
        public List<SuppliersDropDownVM> GetLocalSuppliers()
        {

            List<SuppliersDropDownVM> localsuppliers = new List<SuppliersDropDownVM>();
            return localsuppliers;

        }

        public List<PaymentFrequencyDropDownVM> GetLocalPaymentFrequency()
        {

            List<PaymentFrequencyDropDownVM> localPaymentFrequency = new List<PaymentFrequencyDropDownVM>();
            return localPaymentFrequency;

        }

        public KiiPayBusinessLocalTopUpSuccessVM LocalTopUpSuccess()
        {


            KiiPayBusinessLocalTopUpSuccessVM vm = new KiiPayBusinessLocalTopUpSuccessVM();
            return vm;


        }

        public KiiPayBusinessSearchSuppliersVM GetKiiPayBusinessSearchSuppliers()
        {
            KiiPayBusinessSearchSuppliersVM vm = new KiiPayBusinessSearchSuppliersVM();
            if (Common.BusinessSession.KiiPayBusinessSearchSuppliers != null)
            {
                vm = Common.BusinessSession.KiiPayBusinessSearchSuppliers;
            }

            return vm;

        }
        public void SetKiiPayBusinessSearchSuppliers(KiiPayBusinessSearchSuppliersVM vm)
        {

            Common.BusinessSession.KiiPayBusinessSearchSuppliers = vm;
        }
        public KiiPayBusinessLocalTopUpSuccessVM GetKiiPayBusinessTopUpSuccess()
        {
            KiiPayBusinessLocalTopUpSuccessVM vm = new KiiPayBusinessLocalTopUpSuccessVM();
            if (Common.BusinessSession.KiiPayBusinessTopUpSuccess != null)
            {
                vm = Common.BusinessSession.KiiPayBusinessTopUpSuccess;
            }

            return vm;

        }
        public void SetKiiPayBusinessTopUpSuccess(KiiPayBusinessLocalTopUpSuccessVM vm)
        {

            Common.BusinessSession.KiiPayBusinessTopUpSuccess = vm;
        }
        public KiiPayBusinessLocalTopUpEnterAccountNoVM GetKiiPayBusinessEnterAccountNo()
        {
            KiiPayBusinessLocalTopUpEnterAccountNoVM vm = new KiiPayBusinessLocalTopUpEnterAccountNoVM();
            if (Common.BusinessSession.KiiPayBusinessTopUpEnterAccountNo != null)
            {
                vm = Common.BusinessSession.KiiPayBusinessTopUpEnterAccountNo;
            }

            return vm;

        }
        public void SetKiiPayBusinessEnterAccountNo(KiiPayBusinessLocalTopUpEnterAccountNoVM vm)
        {

            Common.BusinessSession.KiiPayBusinessTopUpEnterAccountNo = vm;
        }
        public bool IsValidWalletNo(string WalletNo)
        {

            var KiiPayWalletInfo = GetKiiPayBusinessInfoByWalletNo(WalletNo);
            if (KiiPayWalletInfo == null)
            {
                return false;
            }
            return true;
        }

        public bool IsValidTransfer(string WalletNo, bool IsLocalPayment)
        {

            var KiiPayWalletInfo = GetKiiPayBusinessInfoByWalletNo(WalletNo);
            if (KiiPayWalletInfo == null)
            {
                return false;
            }
            return true;
        }

        public DB.KiiPayBusinessWalletInformation GetKiiPayBusinessInfoByWalletNo(string WalletNo)
        {

            DB.KiiPayBusinessWalletInformation KiiPayBusinessWalletInformation = new DB.KiiPayBusinessWalletInformation();
            return KiiPayBusinessWalletInformation;
        }
        #endregion

        #region International

        public List<SuppliersDropDownVM> GetInternationalSuppliers()
        {

            List<SuppliersDropDownVM> Internationalsuppliers = new List<SuppliersDropDownVM>();
            return Internationalsuppliers;

        }
        public List<PaymentFrequencyDropDownVM> GetInternationalPaymentFrequency()
        {

            List<PaymentFrequencyDropDownVM> internationalPaymentFrequency = new List<PaymentFrequencyDropDownVM>();
            return internationalPaymentFrequency;

        }

        public KiiPayBusinessInternationalTopUpSuccessVM InternationalTopUpSuccess()
        {
            KiiPayBusinessInternationalTopUpSuccessVM vm = new KiiPayBusinessInternationalTopUpSuccessVM();
            return vm;
        }

        public bool CompleteLocalTopup(KiiPayBusinessLocalTopUpEnterAmountVM vm)
        {


            var SuppliersInfo = Common.BusinessSession.KiiPayBusinessSearchSuppliers;
            string AccountNumber = Common.BusinessSession.KiiPayBusinessTopUpEnterAccountNo.AccountNo;



            DB.KiiPayBusinessLocalTopUpWithSuppliers KiiPayBusinessLocalTopUpWithSuppliers = new DB.KiiPayBusinessLocalTopUpWithSuppliers()
            {
                CreationDate = DateTime.Now,
                Frequency = vm.PaymentFrequency,
                FrequencyDetail = vm.FrequencyDetials,
                ReceiverId = SuppliersInfo.SupplierId,
                ReceiverMobileNo = SuppliersInfo.WalletNo,
                ReceivingCountry = "",
                SenderId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId,
                SendingCountry = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode,
                Amount = vm.Amount,
                AccountNo = AccountNumber,
            };
            var kiiPayBusinessLocalTransactionWithSuppliers_result = SaveKiiPayBusinessLocalTopUpWithSuppliers(KiiPayBusinessLocalTopUpWithSuppliers);
            if (vm.FrequencyDetials!="")
            {
                DB.KiiPayBusinessBusinessStandingOrderInfo model = new DB.KiiPayBusinessBusinessStandingOrderInfo()
                {
                    ReceiverId = kiiPayBusinessLocalTransactionWithSuppliers_result.ReceiverId,
                    ReceiverMobileNo = kiiPayBusinessLocalTransactionWithSuppliers_result.ReceiverMobileNo,
                    SenderId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId,
                    Amount = vm.StandingOrderPaymentAmount,
                    SendingCountry = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode,
                    Frequency = vm.PaymentFrequency,
                    FrequencyDetail = vm.FrequencyDetials,
                    ReceivingCountry = kiiPayBusinessLocalTransactionWithSuppliers_result.ReceivingCountry,
                    CreationDate = DateTime.Now
                };

                var result = SaveBusinessStandingOrder(model);
            }

            return true;

        }
        public DB.KiiPayBusinessBusinessStandingOrderInfo SaveBusinessStandingOrder(DB.KiiPayBusinessBusinessStandingOrderInfo model)
        {

            dbContext.KiiPayBusinessBusinessStandingOrderInfo.Add(model);
            dbContext.SaveChanges();
            return model;
        }
        public DB.KiiPayBusinessLocalTopUpWithSuppliers SaveKiiPayBusinessLocalTopUpWithSuppliers(DB.KiiPayBusinessLocalTopUpWithSuppliers model)
        {


            dbContext.KiiPayBusinessLocalTopUpWithSuppliers.Add(model);
            dbContext.SaveChanges();
            return model;

        }
        public DB.KiiPayBusinessInternationalTopUpWithSuppliers SaveKiiPayBusinessInternationalTopUpWithSuppliers(DB.KiiPayBusinessInternationalTopUpWithSuppliers model)
        {


            dbContext.KiiPayBusinessInternationalTopUpWithSuppliers.Add(model);
            dbContext.SaveChanges();
            return model;

        }

        public bool CompleteInternationalTopup(KiiPayBusinessInternationalTopUpEnterAmountVM vm)
        {


            var SuppliersInfo = Common.BusinessSession.KiiPayBusinessSearchSuppliers;
            string AccountNumber = Common.BusinessSession.KiiPayBusinessTopUpEnterAccountNo.AccountNo;



            DB.KiiPayBusinessInternationalTopUpWithSuppliers KiiPayBusinessInternationalTopUpWithSuppliers = new DB.KiiPayBusinessInternationalTopUpWithSuppliers()
            {
                CreationDate = DateTime.Now,
                Frequency = vm.PaymentFrequency,
                FrequencyDetail = vm.FrequencyDetails,
                ReceiverId = SuppliersInfo.SupplierId,
                ReceiverMobileNo = SuppliersInfo.WalletNo,
                ReceivingCountry = "",
                SenderId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId,
                SendingCountry = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode,
                ExchangeRate = 0,
                FaxingAmount = vm.SendingAmount,
                FaxingFee = vm.Fee,
                TotalAmount = vm.SendingAmount + vm.Fee,
                RecievingAmount = vm.RecevingAmount,
                AccountNo = AccountNumber,
            };
            var KiiPayBusinessInternationalTopUpWithSuppliers_result = SaveKiiPayBusinessInternationalTopUpWithSuppliers(KiiPayBusinessInternationalTopUpWithSuppliers);
            if (vm.FrequencyDetails != "")
            {


                DB.KiiPayBusinessBusinessStandingOrderInfo model = new DB.KiiPayBusinessBusinessStandingOrderInfo()
                {
                    ReceiverId = KiiPayBusinessInternationalTopUpWithSuppliers_result.ReceiverId,
                    ReceiverMobileNo = KiiPayBusinessInternationalTopUpWithSuppliers_result.ReceiverMobileNo,
                    SenderId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId,
                    Amount = vm.StandingOrderPaymentAmount,
                    SendingCountry = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode,
                    Frequency = vm.PaymentFrequency,
                    FrequencyDetail = vm.FrequencyDetails,
                    ReceivingCountry = KiiPayBusinessInternationalTopUpWithSuppliers_result.ReceivingCountry,
                    CreationDate = DateTime.Now
                };

                var result = SaveBusinessStandingOrder(model);
            }





            return true;


        }
        #endregion
    }
}