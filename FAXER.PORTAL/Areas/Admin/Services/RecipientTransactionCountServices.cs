using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class RecipientTransactionCountServices
    {
        FAXEREntities dbContext = null;
        public RecipientTransactionCountServices()
        {
            dbContext = new FAXEREntities();

        }
        public IQueryable<RecipientTransactionCount> List()
        {
            return dbContext.RecipientTransactionCount;
        }
        public List<RecipientTransactionCountViewModel> GetRecipientTransactionCountList()
        {
            return (from c in List().ToList()
                    join d in dbContext.FaxerInformation on c.SenderId equals d.Id into gj
                    from senderInfo in gj.DefaultIfEmpty()
                    join e in dbContext.Recipients on c.RecipientId equals e.Id into jk
                    from recipient in jk.DefaultIfEmpty()
                    select new RecipientTransactionCountViewModel()
                    {
                        Id = c.Id,
                        Frequency = c.Frequency,
                        SenderId = c.SenderId,
                        SenderAccountNo = c.SenderId != 0 ? senderInfo.AccountNo : " ",
                        SenderName = c.SenderId != 0 ? senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName : "All",
                        TransactionCount = c.TransactionCount,
                        ReceivingCountry = c.ReceivingCountry,
                        ReceivingCountryName = Common.Common.GetCountryName(c.ReceivingCountry),
                        SendingCountry = c.SendingCountry,
                        SendingCountryName = Common.Common.GetCountryName(c.SendingCountry),
                        RecipientId = c.RecipientId,
                        RecipientName = c.RecipientId != 0 ? recipient.ReceiverName : "All"
                    }).ToList();
        }

        public void Add(RecipientTransactionCountViewModel vm)
        {
            RecipientTransactionCount model = new RecipientTransactionCount()
            {
                SenderId = vm.SenderId,
                SendingCountry = vm.SendingCountry,
                ReceivingCountry = vm.ReceivingCountry,
                TransactionCount = vm.TransactionCount,
                RecipientId = vm.RecipientId,
                Frequency = CrediDebitCardUsageFrequency.Daily
            };
            dbContext.RecipientTransactionCount.Add(model);
            dbContext.SaveChanges();
        }
        public void Update(RecipientTransactionCountViewModel vm)
        {
            var model = List().SingleOrDefault(x => x.Id == vm.Id);
            model.SenderId = vm.SenderId;
            model.SendingCountry = vm.SendingCountry;
            model.ReceivingCountry = vm.ReceivingCountry;
            model.TransactionCount = vm.TransactionCount;
            model.RecipientId = vm.RecipientId;
            dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }


        public void Delete(int id)
        {
            var model = List().SingleOrDefault(x => x.Id == id);
            dbContext.RecipientTransactionCount.Remove(model);
            dbContext.SaveChanges();
        }
    }

}