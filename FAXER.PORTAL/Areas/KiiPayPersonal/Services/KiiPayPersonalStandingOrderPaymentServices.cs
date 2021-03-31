using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Services
{
    public class KiiPayPersonalStandingOrderPaymentServices
    {
        DB.FAXEREntities dbContext = null;
        public KiiPayPersonalStandingOrderPaymentServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        #region Business StandingOrder
        public List<KiiPayPersonalStandingOrderPaymentListVM> GetBusinessStandingOrderList(int ReceiverBusinessId = 0)
        {

            var result = GetStandingOrderDetails(ReceiverBusinessId);
            return result;

        }


        public List<KiiPayPersonalStandingOrderPaymentListVM> GetStandingOrderDetails(int ReceiverBusinessId = 0)
        {

            int PersonalWalletId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalWalletId;
            var KiiPayBusinessInfo = dbContext.KiiPayBusinessInformation.AsQueryable();
            if (ReceiverBusinessId > 0)
            {
                KiiPayBusinessInfo = KiiPayBusinessInfo.Where(x => x.Id == ReceiverBusinessId);
                var result = (from c in KiiPayBusinessInfo.ToList()
                              select new KiiPayPersonalStandingOrderPaymentListVM()
                              {
                                  Amount = 0,
                                  City = c.BusinessOperationCity,
                                  Country = Common.Common.GetCountryName(c.BusinessCountry),
                                  CurrencySymbol = Common.Common.GetCurrencySymbol(Common.CardUserSession.LoggedCardUserViewModel.CountryCode),
                                  FrequencyDetail = "None",
                                  MobileNo = c.BusinessMobileNo,
                                  Name = c.BusinessName,
                                  ReceiverId = c.Id,
                                  IsEnabled = false
                              }).ToList();
                return result;
            }
            else
            {
                var result = (from c in KiiPayBusinessInfo.ToList()
                              join d in dbContext.KiiPayPersonalBusinessStandingOrderInfo.Where(x => x.SenderId == PersonalWalletId) on c.Id equals d.ReceiverId
                              select new KiiPayPersonalStandingOrderPaymentListVM()
                              {
                                  TransactionId = d.Id,
                                  Amount = d.Amount,
                                  City = c.BusinessOperationCity,
                                  Country = Common.Common.GetCountryName(c.BusinessCountry),
                                  CurrencySymbol = Common.Common.GetCurrencySymbol(Common.CardUserSession.LoggedCardUserViewModel.CountryCode),
                                  PaymentFrequency = d.Frequency,
                                  FrequencyDetail = Common.Common.GetPaymentFrequncyDetail(d.Frequency, d.FrequencyDetail),
                                  MobileNo = c.BusinessMobileNo,
                                  Name = c.BusinessName,
                                  ReceiverId = c.Id,
                                  IsEnabled = true
                              }).ToList();
                return result;
            }

        }

        public DB.KiiPayPersonalBusinessStandingOrderInfo UpdateBusinessStandingOrder(KiiPayPersonalUpdateBusinessStandingOrdervm standingOrdervm)
        {

            var data = dbContext.KiiPayPersonalBusinessStandingOrderInfo.Where(x => x.Id == standingOrdervm.TransactionId).FirstOrDefault();
            data.Amount = standingOrdervm.Amount;
            data.Frequency = standingOrdervm.PaymentFrequency;
            data.FrequencyDetail = standingOrdervm.FrequencyDetials;
            dbContext.Entry<KiiPayPersonalBusinessStandingOrderInfo>(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

            return data;
        }

        internal void DeleteStandingOrderPayment(int id)
        {
            var data = dbContext.KiiPayPersonalBusinessStandingOrderInfo.Where(x => x.Id == id).FirstOrDefault();
            dbContext.KiiPayPersonalBusinessStandingOrderInfo.Remove(data);
            dbContext.SaveChanges();
        }

        internal KiiPayPersonalUpdateBusinessStandingOrdervm GetBusinessStandingOrderDetail(int TransactionId)
        {

            string SenderCountryCode = Common.CardUserSession.LoggedCardUserViewModel.CountryCode;
            var result = (from c in dbContext.KiiPayPersonalBusinessStandingOrderInfo.Where(x => x.Id == TransactionId).ToList()
                          select new KiiPayPersonalUpdateBusinessStandingOrdervm()
                          {
                              PreviousAmount = c.Amount,
                              FrequencyDetials = c.FrequencyDetail,
                              PaymentFrequency = c.Frequency,
                              TransactionId = c.Id,
                              SenderCurrencyCode = Common.Common.GetCountryCurrency(SenderCountryCode),
                              SenderCurrencySymbol = Common.Common.GetCurrencySymbol(SenderCountryCode)
                          }).FirstOrDefault();
            return result;

        }

        public KiiPayPersonalAddNewBusinessStandingOrderSuccessvm CompleteBusinessStandingOrderSetup(KiiPayPersonalBusinessStandingOrdervm vm)
        {

            KiiPayPersonalCommonServices _kiiPayPersonalCommonServices = new KiiPayPersonalCommonServices();
            var receiverInfo = _kiiPayPersonalCommonServices.GetKiipayBusinessInfo(vm.ReceiverId);
            DB.KiiPayPersonalBusinessStandingOrderInfo model = new DB.KiiPayPersonalBusinessStandingOrderInfo()
            {
                ReceiverId = vm.ReceiverId,
                ReceiverMobileNo = receiverInfo.BusinessMobileNo,
                SenderId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalWalletId,
                Amount = vm.Amount,
                SendingCountry = Common.CardUserSession.LoggedCardUserViewModel.CountryCode,
                Frequency = vm.PaymentFrequency,
                FrequencyDetail = vm.FrequencyDetials,
                ReceivingCountry = receiverInfo.BusinessCountry,
                CreationDate = DateTime.Now
            };

            var result = SaveBusinessStandingOrder(model);

            KiiPayPersonalAddNewBusinessStandingOrderSuccessvm successvm = new KiiPayPersonalAddNewBusinessStandingOrderSuccessvm()
            {
                Amount = vm.Amount,
                BusinessName = receiverInfo.BusinessName,
                CurrencySymbol = vm.CurrencySymbol
            };

            return successvm;


        }

        public DB.KiiPayPersonalBusinessStandingOrderInfo SaveBusinessStandingOrder(DB.KiiPayPersonalBusinessStandingOrderInfo model)
        {

            dbContext.KiiPayPersonalBusinessStandingOrderInfo.Add(model);
            dbContext.SaveChanges();
            return model;
        }
        #endregion



        #region KiiPay Standing Order Detials 

        public List<KiiPayPersonalStandingOrderPaymentListVM> GetKiiPayPersonalStandingOrderDetails(int ReceiverId = 0)
        {

            int PersonalWalletId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalWalletId;
            var KiiPayPersonalWalletInfo = dbContext.KiiPayPersonalWalletInformation.AsQueryable();
            if (ReceiverId > 0)
            {
                KiiPayPersonalWalletInfo = KiiPayPersonalWalletInfo.Where(x => x.Id == ReceiverId);
                var result = (from c in KiiPayPersonalWalletInfo.ToList()
                              select new KiiPayPersonalStandingOrderPaymentListVM()
                              {
                                  Amount = 0,
                                  City = c.CardUserCity,
                                  Country = Common.Common.GetCountryName(c.CardUserCountry),
                                  CurrencySymbol = Common.Common.GetCurrencySymbol(Common.CardUserSession.LoggedCardUserViewModel.CountryCode),
                                  FrequencyDetail = "None",
                                  MobileNo = c.MobileNo,
                                  Name = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                                  ReceiverId = c.Id,
                                  IsEnabled = false
                              }).ToList();
                return result;
            }
            else
            {
                var result = (from c in KiiPayPersonalWalletInfo.ToList()
                              join d in dbContext.KiiPayPersonalKiiPayPersonalStandingOrderInfo.Where(x => x.SenderId == PersonalWalletId) on c.Id equals d.ReceiverId
                              select new KiiPayPersonalStandingOrderPaymentListVM()
                              {
                                  TransactionId = d.Id,
                                  Amount = d.Amount,
                                  City = c.CardUserCity,
                                  Country = Common.Common.GetCountryName(c.CardUserCountry),
                                  CurrencySymbol = Common.Common.GetCurrencySymbol(Common.CardUserSession.LoggedCardUserViewModel.CountryCode),
                                  PaymentFrequency = d.PaymentFrequency,
                                  FrequencyDetail = Common.Common.GetPaymentFrequncyDetail(d.PaymentFrequency, d.FrequencyDetials),
                                  MobileNo = c.MobileNo,
                                  Name = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                                  ReceiverId = c.Id,
                                  IsEnabled = true
                              }).ToList();
                return result;
            }

        }



        internal KiiPayPersonalUpdateBusinessStandingOrdervm GetKiiPayPersonalStandingOrderDetail(int TransactionId)
        {

            string SenderCountryCode = Common.CardUserSession.LoggedCardUserViewModel.CountryCode;
            var result = (from c in dbContext.KiiPayPersonalKiiPayPersonalStandingOrderInfo.Where(x => x.Id == TransactionId).ToList()
                          select new KiiPayPersonalUpdateBusinessStandingOrdervm()
                          {
                              PreviousAmount = c.Amount,
                              FrequencyDetials = c.FrequencyDetials,
                              PaymentFrequency = c.PaymentFrequency,
                              TransactionId = c.Id,
                              SenderCurrencyCode = Common.Common.GetCountryCurrency(SenderCountryCode),
                              SenderCurrencySymbol = Common.Common.GetCurrencySymbol(SenderCountryCode)



                          }).FirstOrDefault();
            return result;

        }
        internal void DeleteKiiPayPersonalStandingOrderPayment(int id)
        {
            var data = dbContext.KiiPayPersonalKiiPayPersonalStandingOrderInfo.Where(x => x.Id == id).FirstOrDefault();
            dbContext.KiiPayPersonalKiiPayPersonalStandingOrderInfo.Remove(data);
            dbContext.SaveChanges();
        }

        public KiiPayPersonalAddNewBusinessStandingOrderSuccessvm CompleteKiiPayPersonalStandingOrderSetup(KiiPayPersonalBusinessStandingOrdervm vm)
        {

            KiiPayPersonalCommonServices _kiiPayPersonalCommonServices = new KiiPayPersonalCommonServices();
            var receiverInfo = _kiiPayPersonalCommonServices.GetKiipayPersonalWalletInfo(vm.ReceiverId);
            DB.KiiPayPersonalKiiPayPersonalStandingOrderInfo model = new DB.KiiPayPersonalKiiPayPersonalStandingOrderInfo()
            {
                ReceiverId = vm.ReceiverId,
                ReceiverMobileNo = receiverInfo.MobileNo,
                SenderId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalWalletId,
                Amount = vm.Amount,
                SenderCountry = Common.CardUserSession.LoggedCardUserViewModel.CountryCode,
                PaymentFrequency = vm.PaymentFrequency,
                FrequencyDetials = vm.FrequencyDetials,
                ReceiverCountry = receiverInfo.CardUserCountry,
                CreationDate = DateTime.Now
            };

            var result = SaveKiiPayPersonalStandingOrder(model);

            KiiPayPersonalAddNewBusinessStandingOrderSuccessvm successvm = new KiiPayPersonalAddNewBusinessStandingOrderSuccessvm()
            {
                Amount = vm.Amount,
                BusinessName = receiverInfo.FirstName + " " + receiverInfo.MiddleName + " " + receiverInfo.LastName,
                CurrencySymbol = vm.CurrencySymbol
            };

            return successvm;


        }
        public DB.KiiPayPersonalKiiPayPersonalStandingOrderInfo SaveKiiPayPersonalStandingOrder(DB.KiiPayPersonalKiiPayPersonalStandingOrderInfo model)
        {

            dbContext.KiiPayPersonalKiiPayPersonalStandingOrderInfo.Add(model);
            dbContext.SaveChanges();
            return model;
        }
        public DB.KiiPayPersonalKiiPayPersonalStandingOrderInfo UpdateKiiPayPersonalStandingOrder(KiiPayPersonalUpdateBusinessStandingOrdervm standingOrdervm)
        {

            var data = dbContext.KiiPayPersonalKiiPayPersonalStandingOrderInfo.Where(x => x.Id == standingOrdervm.TransactionId).FirstOrDefault();
            data.Amount = standingOrdervm.Amount;
            data.PaymentFrequency = standingOrdervm.PaymentFrequency;
            data.FrequencyDetials = standingOrdervm.FrequencyDetials;
            dbContext.Entry<KiiPayPersonalKiiPayPersonalStandingOrderInfo>(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

            return data;




        }
        #endregion

    }
}