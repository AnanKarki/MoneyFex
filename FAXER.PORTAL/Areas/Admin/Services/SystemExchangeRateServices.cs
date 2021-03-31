using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class SystemExchangeRateServices
    {
        FAXEREntities dbContext = null;
        public SystemExchangeRateServices()
        {
            dbContext = new FAXEREntities();
        }

        public List<SystemExchangeRateViewModel> SystemExchangeRateTypeList()
        {
            var data = dbContext.SystemExchangeRateType.ToList();
            var result = (from c in data
                          select new SystemExchangeRateViewModel()
                          {
                              Id = c.Id,
                              ExchangeRateType = c.ExchangeRateType,
                              ReceivingCurrency = c.ReceivingCurrency,
                              SendingCurrency = c.SendingCurrency,
                              TransferMethod = c.TransferMethod,
                              TransferMethodName = c.TransferMethod.ToString(),
                              IsCurrentExchangeRate = c.ExchangeRateType == ExchangeRateType.CurrentExchangeRate ? true : false,
                              IsTransactionExchangeRate = c.ExchangeRateType == ExchangeRateType.TransactionExchangeRate ? true : false
                          }).ToList();
            return result;
        }
        public SystemExchangeRateViewModel SystemExchangeRateType(int id)
        {
            var exchangeTypedata = SystemExchangeRateTypeById(id);
            return new SystemExchangeRateViewModel()
            {
                Id = exchangeTypedata.Id,
                ExchangeRateType = exchangeTypedata.ExchangeRateType,
                ReceivingCurrency = exchangeTypedata.ReceivingCurrency,
                SendingCurrency = exchangeTypedata.SendingCurrency,
                TransferMethod = exchangeTypedata.TransferMethod,
                TransferMethodName = exchangeTypedata.TransferMethod.ToString()
            };
        }
        public SystemExchangeRateType SystemExchangeRateTypeById(int id)
        {
            return dbContext.SystemExchangeRateType.SingleOrDefault(x => x.Id == id);
        }
        public void Add(SystemExchangeRateViewModel systemExchangeRateTypeVm)
        {
            SystemExchangeRateType SystemExchangeRateType = new SystemExchangeRateType()
            {
                ExchangeRateType = systemExchangeRateTypeVm.ExchangeRateType,
                ReceivingCurrency = systemExchangeRateTypeVm.ReceivingCurrency,
                SendingCurrency = systemExchangeRateTypeVm.SendingCurrency,
                TransferMethod = systemExchangeRateTypeVm.TransferMethod
            };
            dbContext.SystemExchangeRateType.Add(SystemExchangeRateType);
            dbContext.SaveChanges();
        }

        internal void UpdateSystemExhangeRate(int id)
        {
            var data = dbContext.SystemExchangeRateType.Where(x => x.Id == id).FirstOrDefault();
            if (data.ExchangeRateType == ExchangeRateType.CurrentExchangeRate) data.ExchangeRateType = ExchangeRateType.TransactionExchangeRate;
            else data.ExchangeRateType = ExchangeRateType.CurrentExchangeRate;
            dbContext.Entry(data).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        public void Update(SystemExchangeRateViewModel systemExchangeRateTypeVm)
        {
            var data = SystemExchangeRateTypeById(systemExchangeRateTypeVm.Id);
            data.ExchangeRateType = systemExchangeRateTypeVm.ExchangeRateType;
            data.ReceivingCurrency = systemExchangeRateTypeVm.ReceivingCurrency;
            data.SendingCurrency = systemExchangeRateTypeVm.SendingCurrency;
            data.TransferMethod = systemExchangeRateTypeVm.TransferMethod;

            dbContext.Entry(data).State = EntityState.Modified;
            dbContext.SaveChanges();
        }
        public void Delete(int id)
        {
            var data = SystemExchangeRateTypeById(id);

            dbContext.SystemExchangeRateType.Remove(data);
            dbContext.SaveChanges();
        }

    }

}