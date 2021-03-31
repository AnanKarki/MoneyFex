using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class PayoutProviderRateServices
    {
        FAXEREntities dbContext = null;
        public PayoutProviderRateServices()
        {
            dbContext = new FAXEREntities();
        }

        public IQueryable<PayoutProviderRate> PayoutProviderRates()
        {
            return dbContext.PayoutProviderRate;
        }
        public void AddpayoutProviderRate(PayoutProviderRate model)
        {
            dbContext.PayoutProviderRate.Add(model);
            dbContext.SaveChanges();
        }
        public void UpdatepayoutProviderRate(PayoutProviderRate model)
        {
            dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }
        public void DeletepayoutProviderRate(int id)
        {
            var model = PayoutProviderRates().Where(x => x.Id == id).FirstOrDefault();
            dbContext.PayoutProviderRate.Remove(model);
            dbContext.SaveChanges();
        }

        public List<PayoutProviderRateViewModel> GetPayoutProviderRateViewModel()
        {
            return (from c in PayoutProviderRates()
                    join sendingCountry in dbContext.Country on c.SendingCountry equals sendingCountry.CountryCode into sc
                    from sendingCountry in sc.DefaultIfEmpty()
                    join recevingCountry in dbContext.Country on c.RecevingCountry equals recevingCountry.CountryCode into rc
                    from recevingCountry in rc.DefaultIfEmpty()
                    select new PayoutProviderRateViewModel()
                    {
                        Id = c.Id,
                        PayoutProvider = c.PayoutProvider,
                        PayoutProviderName = c.PayoutProvider.ToString(),
                        Rate = c.Rate,
                        RecevingCountry = c.RecevingCountry,
                        RecevingCountryName = c.RecevingCountry.ToLower() == "all" ? "All" : recevingCountry.CountryName,
                        RecevingCurrency = c.RecevingCurrency,
                        SendingCountryName = c.SendingCountry.ToLower() == "all" ? "All" :  sendingCountry.CountryName,
                        SendingCountry = c.SendingCountry,
                        SendingCurrency = c.SendingCurrency
                    }).ToList();
        }

        public PayoutProviderRate GetPayoutProviderRateModel(PayoutProviderRateViewModel vm)
        {
            PayoutProviderRate payoutProviderRate = new PayoutProviderRate()
            {
                Id = vm.Id,
                PayoutProvider = vm.PayoutProvider,
                Rate = vm.Rate,
                RecevingCountry = vm.RecevingCountry,
                RecevingCurrency = vm.RecevingCurrency,
                SendingCountry = vm.SendingCountry,
                SendingCurrency = vm.SendingCurrency
            };
            return payoutProviderRate;
        }
    }
}