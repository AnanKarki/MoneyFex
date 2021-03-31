using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class SenderTransactionCountServices
    {
        FAXEREntities dbContext = null;
        public SenderTransactionCountServices()
        {
            dbContext = new FAXEREntities();
        }
        public IQueryable<SenderTransactionCount> List()
        {
            return dbContext.SenderTransactionCount;
        }
        public List<SenderTransactionCountViewModel> GetSenderTransactionCountList()
        {
            return (from c in List().ToList()
                    join d in dbContext.FaxerInformation on c.SenderId equals d.Id into gj
                    from senderInfo in gj.DefaultIfEmpty()
                    select new SenderTransactionCountViewModel()
                    {
                        Id = c.Id,
                        Frequency = c.Frequency,
                        SenderId = c.SenderId,
                        SenderAccountNo = c.SenderId != 0 ? senderInfo.AccountNo : " ",
                        SenderName = c.SenderId != 0 ? senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName : "All",
                        TransactionCount = c.TransactionCount,
                        ReceivingCountry = c.ReceivingCountry,
                        ReceivingCountryName = c.ReceivingCountry == "All" ? "All" :  Common.Common.GetCountryName(c.ReceivingCountry),
                        SendingCountry = c.SendingCountry,
                        SendingCountryName = c.SendingCountry == "All" ? "All" :  Common.Common.GetCountryName(c.SendingCountry)
                    }).ToList();
        }

        public void Add(SenderTransactionCountViewModel vm)
        {
            SenderTransactionCount model = new SenderTransactionCount()
            {
                SenderId = vm.SenderId,
                SendingCountry = vm.SendingCountry,
                ReceivingCountry = vm.ReceivingCountry,
                TransactionCount = vm.TransactionCount,
                Frequency = CrediDebitCardUsageFrequency.Daily
            };
            dbContext.SenderTransactionCount.Add(model);
            dbContext.SaveChanges();
        }
        public void Update(SenderTransactionCountViewModel vm)
        {
            var model = List().SingleOrDefault(x => x.Id == vm.Id);
            model.SenderId = vm.SenderId;
            model.SendingCountry = vm.SendingCountry;
            model.ReceivingCountry = vm.ReceivingCountry;
            model.TransactionCount = vm.TransactionCount;
            dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }


        public void Delete(int id)
        {
            var model = List().SingleOrDefault(x => x.Id == id);
            dbContext.SenderTransactionCount.Remove(model);
            dbContext.SaveChanges();
        }
    }
}