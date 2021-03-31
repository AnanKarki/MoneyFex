using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Services
{
    public class KiiPayBusinessPayMonthlyBillServices
    {
        DB.FAXEREntities dbContext = null;
        public KiiPayBusinessPayMonthlyBillServices()
        {
            dbContext = new DB.FAXEREntities();
        }
        public KiiPayBusinessPayBillsReferenceOneVM GetLocalPayBillsReferenceOne()
        {
            KiiPayBusinessPayBillsReferenceOneVM vm = new KiiPayBusinessPayBillsReferenceOneVM();
            return vm;

        }

        public KiiPayBusinessInternationalPayBillsReferenceOneVM GetInternationalPayBillsReferenceOne()
        {
            KiiPayBusinessInternationalPayBillsReferenceOneVM vm = new KiiPayBusinessInternationalPayBillsReferenceOneVM();
            return vm;

        }
        public KiiPayBusinessEnterPaymentReferenceVM GetKiiPayBusinessEnterPaymentReference()
        {
            KiiPayBusinessEnterPaymentReferenceVM vm = new KiiPayBusinessEnterPaymentReferenceVM();
            if (Common.BusinessSession.KiiPayBusinessEnterPaymentReference != null)
            {
                vm = Common.BusinessSession.KiiPayBusinessEnterPaymentReference;
            }

            return vm;

        }
        public KiiPayBusinessInternationalSearchCountryVM GetKiiPayBusinessInternationalSelectCountry()
        {
            KiiPayBusinessInternationalSearchCountryVM vm = new KiiPayBusinessInternationalSearchCountryVM();
            if (Common.BusinessSession.KiiPayBusinessInternationalSearchCountry != null)
            {
                vm = Common.BusinessSession.KiiPayBusinessInternationalSearchCountry;
            }

            return vm;

        }

        public void SetKiiPayBusinessInternationalSelectCountry(KiiPayBusinessInternationalSearchCountryVM vm)
        {

            Common.BusinessSession.KiiPayBusinessInternationalSearchCountry = vm;
        }

        public void SetKiiPayBusinessEnterPaymentReference(KiiPayBusinessEnterPaymentReferenceVM vm)
        {

            Common.BusinessSession.KiiPayBusinessEnterPaymentReference = vm;
        }

        public bool CompletePayMothlyBillServices(KiiPayBusinessPayBillsReferenceOneVM vm)
        {


            var ReferenceInfo = Common.BusinessSession.KiiPayBusinessEnterPaymentReference;



            DB.KiiPayBusinessLocalTransactionWithSuppliers kiiPayBusinessLocalTransactionWithSuppliers = new DB.KiiPayBusinessLocalTransactionWithSuppliers()
            {
                AmountSent = 500,
                BusinessId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId,
                IsAutoPayment = false,
                ReferenceNo = ReferenceInfo.ReferenceNo0+ReferenceInfo.ReferenceNo1+ReferenceInfo.ReferenceNo2,
                TransactionDate = DateTime.Now,
                SuppliersId = 1,
                IsRefunded = false,
            };

            var kiiPayBusinessLocalTransactionWithSuppliers_result = SaveKiiPayBusinessLocalTransactionWithSuppliers(kiiPayBusinessLocalTransactionWithSuppliers);
           

            return true;


        }
        public bool CompleteInternationalPayMothlyBillServices(KiiPayBusinessInternationalPayBillsReferenceOneVM vm)
        {


            var ReferenceInfo = Common.BusinessSession.KiiPayBusinessEnterPaymentReference;



            DB.KiiPayBusinessInternationalTransactionWithSuppliers kiiPayBusinessInternationalTransactionWithSuppliers = new DB.KiiPayBusinessInternationalTransactionWithSuppliers()
            {
                FaxingAmount = vm.Amount,
                IsRefunded = false,
                TransactionDate=DateTime.Now,
                TotalAmount = vm.Amount,
                SuppliersId = 1,
                ReferenceNo = ReferenceInfo.ReferenceNo0+ ReferenceInfo.ReferenceNo1 + ReferenceInfo.ReferenceNo2 ,
                RecievingAmount = vm.Amount,
                IsAutoPayment = false,
                FaxingFee =vm.Fee,
                ExchangeRate = 0,
                BusinessId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId,
            };

            var kiiPayBusinessInternationalTransactionWithSuppliers_result = SaveKiiPayBusinessInternationalTransactionWithSuppliers(kiiPayBusinessInternationalTransactionWithSuppliers);
           

            return true;


        }

        public DB.KiiPayBusinessLocalTransactionWithSuppliers SaveKiiPayBusinessLocalTransactionWithSuppliers(DB.KiiPayBusinessLocalTransactionWithSuppliers model)
        {


            dbContext.KiiPayBusinessLocalTransactionWithSuppliers.Add(model);
            dbContext.SaveChanges();
            return model;

        }
        public DB.KiiPayBusinessInternationalTransactionWithSuppliers SaveKiiPayBusinessInternationalTransactionWithSuppliers(DB.KiiPayBusinessInternationalTransactionWithSuppliers model)
        {


            dbContext.KiiPayBusinessInternationalTransactionWithSuppliers.Add(model);
            dbContext.SaveChanges();
            return model;

        }

        public List<SuppliersDropDownVM> GetInternationalSuppliers()
        {

            List<SuppliersDropDownVM> Internationalsuppliers = new List<SuppliersDropDownVM>();
            return Internationalsuppliers;

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
    }
}