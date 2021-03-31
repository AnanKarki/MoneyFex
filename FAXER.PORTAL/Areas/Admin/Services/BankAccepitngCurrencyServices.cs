using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class BankAccepitngCurrencyServices
    {
        FAXEREntities dbContext = null;
        public BankAccepitngCurrencyServices()
        {
            dbContext = new FAXEREntities();
        }
        public IQueryable<BankAcceptingCurrency> BankAcceptingCurrencies()
        {
            return dbContext.BankAcceptingCurrency;
        }
        public void Add(BankAcceptingCurrency model)
        {
            dbContext.BankAcceptingCurrency.Add(model);
            dbContext.SaveChanges();
        }
        public void Update(BankAcceptingCurrency model)
        {
            dbContext.Entry(model).State = EntityState.Modified;
            dbContext.SaveChanges();
        }
        public void Delete(int id)
        {
            var model = BankAcceptingCurrencies().Where(x => x.Id == id).FirstOrDefault();
            dbContext.BankAcceptingCurrency.Remove(model);
            dbContext.SaveChanges();
        }

        public void RemoveRange(IQueryable<BankAcceptingCurrency> bankAcceptingCurrency)
        {
            dbContext.BankAcceptingCurrency.RemoveRange(bankAcceptingCurrency);
            dbContext.SaveChanges();
        }
    }
}