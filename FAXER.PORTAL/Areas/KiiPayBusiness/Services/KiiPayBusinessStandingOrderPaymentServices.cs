using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Services
{
    public class KiiPayBusinessStandingOrderPaymentServices
    {
        DB.FAXEREntities dbContext = null;
        public KiiPayBusinessStandingOrderPaymentServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<ViewModels.KiiPayBusinessStandingOrderPaymentListVM> GetBusinessStandingOrderList(int ReceiverBusinessId = 0)
        {
            
            var result = GetStandingOrderDetails(ReceiverBusinessId);
            return result;

        }


 
        public List<KiiPayBusinessStandingOrderPaymentListVM> GetStandingOrderDetails(int ReceiverBusinessId = 0)
        {

            int BusinessId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId;
            var KiiPayBusinessInfo = dbContext.KiiPayBusinessInformation.AsQueryable();
            if (ReceiverBusinessId > 0)
            {
                KiiPayBusinessInfo = KiiPayBusinessInfo.Where(x => x.Id == ReceiverBusinessId);
                var result = (from c in KiiPayBusinessInfo.ToList()
                              select new KiiPayBusinessStandingOrderPaymentListVM()
                              {
                                  Amount = 0,
                                  City = c.BusinessOperationCity,
                                  Country = Common.Common.GetCountryName(c.BusinessCountry),
                                  CurrencySymbol = Common.Common.GetCurrencySymbol(Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode),
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
                              join d in dbContext.KiiPayBusinessBusinessStandingOrderInfo.Where(x => x.SenderId == BusinessId) on c.Id equals d.ReceiverId
                              select new KiiPayBusinessStandingOrderPaymentListVM()
                              {
                                  TransactionId =d.Id,
                                  Amount = d.Amount,
                                  City = c.BusinessOperationCity,
                                  Country = Common.Common.GetCountryName(c.BusinessCountry),
                                  CurrencySymbol = Common.Common.GetCurrencySymbol(Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode),
                                  PaymentFrequency = d.Frequency,
                                  FrequencyDetail = Common.Common.GetPaymentFrequncyDetail(d.Frequency , d.FrequencyDetail),
                                  MobileNo = c.BusinessMobileNo,
                                  Name = c.BusinessName,
                                  ReceiverId = c.Id,
                                  IsEnabled = true
                              }).ToList();
                return result;
            }

        }

        internal UpdateBusinessStandingOrdervm GetBusinessStandingOrderDetail(int TransactionId)
        {

            string SenderCountryCode = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode;
            var result = (from c in dbContext.KiiPayBusinessBusinessStandingOrderInfo.Where(x => x.Id == TransactionId).ToList()
                          select new UpdateBusinessStandingOrdervm()
                          {
                              PreviousAmount = c.Amount,
                              FrequencyDetials = c.FrequencyDetail,
                              PaymentFrequency  =c.Frequency,
                              TransactionId = c.Id,
                              SenderCurrencyCode = Common.Common.GetCountryCurrency(SenderCountryCode),
                              SenderCurrencySymbol = Common.Common.GetCurrencySymbol(SenderCountryCode)



                          }).FirstOrDefault();
            return result;

        }

    
        internal void DeleteStandingOrderPayment(int id)
        {
            var data = dbContext.KiiPayBusinessBusinessStandingOrderInfo.Where(x => x.Id == id).FirstOrDefault();
            dbContext.KiiPayBusinessBusinessStandingOrderInfo.Remove(data);
            dbContext.SaveChanges();
        }

        public AddNewBusinessStandingOrderSuccessvm CompleteBusinessStandingOrderSetup(BusinessStandingOrdervm vm ){

            KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();
            var receiverInfo = _kiiPayBusinessCommonServices.GetKiipayBusinessInfo(vm.ReceiverId);
            DB.KiiPayBusinessBusinessStandingOrderInfo model = new DB.KiiPayBusinessBusinessStandingOrderInfo()
            {
                ReceiverId = vm.ReceiverId,
                ReceiverMobileNo = receiverInfo.BusinessMobileNo,
                SenderId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId,
                Amount = vm.Amount,
                SendingCountry = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode,
                Frequency = vm.PaymentFrequency,
                FrequencyDetail =  vm.FrequencyDetials,
                ReceivingCountry = receiverInfo.BusinessCountry,
                CreationDate = DateTime.Now
            };

            var result =  SaveBusinessStandingOrder(model);

            AddNewBusinessStandingOrderSuccessvm successvm = new AddNewBusinessStandingOrderSuccessvm()
            {
                Amount = vm.Amount,
                BusinessName = receiverInfo.BusinessName,
                CurrencySymbol = vm.CurrencySymbol
            };

            return successvm;


        }
        public DB.KiiPayBusinessBusinessStandingOrderInfo SaveBusinessStandingOrder(DB.KiiPayBusinessBusinessStandingOrderInfo model) {

            dbContext.KiiPayBusinessBusinessStandingOrderInfo.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public DB.KiiPayBusinessBusinessStandingOrderInfo UpdateBusinessStandingOrder(UpdateBusinessStandingOrdervm standingOrdervm)
        {

            var data = dbContext.KiiPayBusinessBusinessStandingOrderInfo.Where(x => x.Id == standingOrdervm.TransactionId).FirstOrDefault();
            data.Amount = standingOrdervm.Amount;
            data.Frequency = standingOrdervm.PaymentFrequency;
            data.FrequencyDetail = standingOrdervm.FrequencyDetials;
            dbContext.Entry<KiiPayBusinessBusinessStandingOrderInfo>(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

            return data;




        }

        #region KiiPay Standing Order Detials 

        public List<KiiPayBusinessStandingOrderPaymentListVM> GetKiiPayPersonalStandingOrderDetails(int ReceiverId = 0)
        {

            int BusinessId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId;
            var KiiPayPersonalWalletInfo = dbContext.KiiPayPersonalWalletInformation.AsQueryable();
            if (ReceiverId > 0)
            {
                KiiPayPersonalWalletInfo = KiiPayPersonalWalletInfo.Where(x => x.Id == ReceiverId);
                var result = (from c in KiiPayPersonalWalletInfo.ToList()
                              select new KiiPayBusinessStandingOrderPaymentListVM()
                              {
                                  Amount = 0,
                                  City = c.CardUserCity,
                                  Country = Common.Common.GetCountryName(c.CardUserCountry),
                                  CurrencySymbol = Common.Common.GetCurrencySymbol(Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode),
                                  FrequencyDetail = "None",
                                  MobileNo = c.MobileNo,
                                  Name = c.FirstName + " "  + c.MiddleName + " " + c.LastName,
                                  ReceiverId = c.Id,
                                  IsEnabled = false
                              }).ToList();
                return result;
            }
            else
            {
                var result = (from c in KiiPayPersonalWalletInfo.ToList()
                              join d in dbContext.KiiPayBusinessKiiPayPersonalStandingOrderInfo.Where(x => x.SenderId == BusinessId) on c.Id equals d.ReceiverId
                              select new KiiPayBusinessStandingOrderPaymentListVM()
                              {
                                  TransactionId = d.Id,
                                  Amount = d.Amount,
                                  City = c.CardUserCity,
                                  Country = Common.Common.GetCountryName(c.CardUserCountry),
                                  CurrencySymbol = Common.Common.GetCurrencySymbol(Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode),
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



        internal UpdateBusinessStandingOrdervm GetKiiPayPersonalStandingOrderDetail(int TransactionId)
        {

            string SenderCountryCode = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode;
            var result = (from c in dbContext.KiiPayBusinessKiiPayPersonalStandingOrderInfo.Where(x => x.Id == TransactionId).ToList()
                          select new UpdateBusinessStandingOrdervm()
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
            var data = dbContext.KiiPayBusinessKiiPayPersonalStandingOrderInfo.Where(x => x.Id == id).FirstOrDefault();
            dbContext.KiiPayBusinessKiiPayPersonalStandingOrderInfo.Remove(data);
            dbContext.SaveChanges();
        }

        public AddNewBusinessStandingOrderSuccessvm CompleteKiiPayPersonalStandingOrderSetup(BusinessStandingOrdervm vm)
        {

            KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();
            var receiverInfo = _kiiPayBusinessCommonServices.GetKiiPayPersonalWalletInfo(vm.ReceiverId);
            DB.KiiPayBusinessKiiPayPersonalStandingOrderInfo model = new DB.KiiPayBusinessKiiPayPersonalStandingOrderInfo()
            {
                ReceiverId = vm.ReceiverId,
                ReceiverMobileNo = receiverInfo.MobileNo,
                SenderId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId,
                Amount = vm.Amount,
                SenderCountry = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode,
                PaymentFrequency = vm.PaymentFrequency,
                FrequencyDetials = vm.FrequencyDetials,
                ReceiverCountry = receiverInfo.CardUserCountry,
                CreationDate = DateTime.Now
            };

            var result = SaveKiiPayPersonalStandingOrder(model);

            AddNewBusinessStandingOrderSuccessvm successvm = new AddNewBusinessStandingOrderSuccessvm()
            {
                Amount = vm.Amount,
                BusinessName = receiverInfo.FirstName  +  " " + receiverInfo.MiddleName + " " + receiverInfo.LastName,
                CurrencySymbol = vm.CurrencySymbol
            };

            return successvm;


        }
        public DB.KiiPayBusinessKiiPayPersonalStandingOrderInfo SaveKiiPayPersonalStandingOrder(DB.KiiPayBusinessKiiPayPersonalStandingOrderInfo model)
        {

            dbContext.KiiPayBusinessKiiPayPersonalStandingOrderInfo.Add(model);
            dbContext.SaveChanges();
            return model;
        }
        public DB.KiiPayBusinessKiiPayPersonalStandingOrderInfo UpdateKiiPayPersonalStandingOrder(UpdateBusinessStandingOrdervm standingOrdervm)
        {

            var data = dbContext.KiiPayBusinessKiiPayPersonalStandingOrderInfo.Where(x => x.Id == standingOrdervm.TransactionId).FirstOrDefault();
            data.Amount = standingOrdervm.Amount;
            data.PaymentFrequency = standingOrdervm.PaymentFrequency;
            data.FrequencyDetials= standingOrdervm.FrequencyDetials;
            dbContext.Entry<KiiPayBusinessKiiPayPersonalStandingOrderInfo>(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

            return data;




        }
        #endregion
    }
}