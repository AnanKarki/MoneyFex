using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class CreditCardAttemptLimitServices
    {
        FAXEREntities dbContext = null;
        CommonServices _CommonServices = null;
        public CreditCardAttemptLimitServices()
        {
            dbContext = new FAXEREntities();
            _CommonServices = new CommonServices();
        }

        public IQueryable<CreditCardAttemptLimit> List()
        {
            return dbContext.CreditCardAttemptLimit;
        }

        public List<CreditCardAttemptLimitViewModel> GetCreditCardAttemptLimitList()
        {
            return (from c in List().ToList()
                    join d in dbContext.FaxerInformation on c.SenderId equals d.Id into gj
                    from e in gj.DefaultIfEmpty()
                    select new CreditCardAttemptLimitViewModel()
                    {
                        AttemptLimit = c.AttemptLimit,
                        Id = c.Id,
                        ReceivingCountryName = Common.Common.GetCountryName(c.ReceivingCountry),
                        ReceivingCountry = c.ReceivingCountry,
                        SenderId = c.SenderId,
                        SendingCountry = c.SendingCountry,
                        SendingCountryName = Common.Common.GetCountryName(c.SendingCountry),
                        SenderName = c.SenderId == null ? "All" : e.FirstName + " " + e.MiddleName + " " + e.LastName,
                        SenderAccountNo = c.SenderId == null ? "" : e.AccountNo
                    }).ToList();
        }
        public void Add(CreditCardAttemptLimitViewModel vm)
        {
            CreditCardAttemptLimit model = new CreditCardAttemptLimit()
            {
                AttemptLimit = vm.AttemptLimit,
                ReceivingCountry = vm.ReceivingCountry,
                SenderId = vm.SenderId,
                SendingCountry = vm.SendingCountry,
                Frequency = CrediDebitCardUsageFrequency.Daily,
            };
            dbContext.CreditCardAttemptLimit.Add(model);
            dbContext.SaveChanges();
        }
        public void Update(CreditCardAttemptLimitViewModel vm)
        {
            CreditCardAttemptLimit model = List().SingleOrDefault(x => x.Id == vm.Id);
            model.AttemptLimit = vm.AttemptLimit;
            model.ReceivingCountry = vm.ReceivingCountry;
            model.SenderId = vm.SenderId;
            model.SendingCountry = vm.SendingCountry;
            dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }
        public void Delete(int id)
        {
            CreditCardAttemptLimit model = List().SingleOrDefault(x => x.Id == id);
            dbContext.CreditCardAttemptLimit.Remove(model);
            dbContext.SaveChanges();
        }
    }
}