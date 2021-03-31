using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Mobile.Models.MoneyFex;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class TransactionAmountLimitServices
    {
        FAXEREntities dbContext = null;
        CommonServices _commonServices = null;

        public TransactionAmountLimitServices()
        {
            dbContext = new FAXEREntities();
            _commonServices = new CommonServices();
        }

        public IQueryable<TransactionAmountLimit> List()
        {
            return dbContext.TransactionAmountLimit;
        }


        public List<TransactionAmountLimitViewModel> GetTransactionAmountLimitList(Module module)
        {
            var result = List().Where(x => x.ForModule == module).ToList();
            var data = (from c in result
                        join d in dbContext.FaxerInformation on c.SenderId equals d.Id into fg
                        from senderInfo in fg.DefaultIfEmpty()
                        join e in dbContext.StaffInformation on c.StaffId equals e.Id into hi
                        from staffInfo in hi.DefaultIfEmpty()
                        select new TransactionAmountLimitViewModel()
                        {
                            Amount = c.Amount,
                            Id = c.Id,
                            ReceivingCountry = c.ReceivingCountry,
                            ReceivingCurrency = c.ReceivingCurrency,
                            SenderAccountNo = c.SenderId == 0 ? " " : senderInfo.AccountNo,
                            SenderId = c.SenderId,
                            SenderName = c.SenderId == 0 ? "All" : senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName,
                            SendingCountry = c.SendingCountry,
                            SendingCurrency = c.SendingCurrency,
                            ReceivingCountryName = c.ReceivingCountry == "All" ? "All" : Common.Common.GetCountryName(c.ReceivingCountry),
                            SendingCountryName = c.SendingCountry == "All" ? "All" : Common.Common.GetCountryName(c.SendingCountry),
                            StaffAccountNo = c.StaffId != 0 ? staffInfo.StaffMFSCode : " ",
                            StaffId = c.StaffId,
                            StaffName = c.StaffId != 0 ? staffInfo.FirstName + " " + staffInfo.MiddleName + " " + staffInfo.LastName : "All",
                        }).ToList();
            return data;
        }

        public void Delete(int id)
        {
            var data = List().SingleOrDefault(x => x.Id == id);

            dbContext.TransactionAmountLimit.Remove(data);
            dbContext.SaveChanges();
        }

        public void Update(TransactionAmountLimitViewModel vm)
        {
            var data = List().SingleOrDefault(x => x.Id == vm.Id);
            data.ReceivingCountry = vm.ReceivingCountry;
            data.ReceivingCurrency = vm.ReceivingCurrency;
            data.SenderId = vm.SenderId;
            data.SendingCountry = vm.SendingCountry;
            data.SendingCurrency = vm.SendingCurrency;
            data.Amount = vm.Amount;
            data.StaffId = vm.StaffId;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

        }

        public void Add(TransactionAmountLimitViewModel vm)
        {
            TransactionAmountLimit model = new TransactionAmountLimit()
            {
                Amount = vm.Amount,
                ReceivingCountry = vm.ReceivingCountry,
                ReceivingCurrency = vm.ReceivingCurrency,
                SenderId = vm.SenderId,
                SendingCountry = vm.SendingCountry,
                SendingCurrency = vm.SendingCurrency,
                ForModule = vm.ForModule,
                StaffId = vm.StaffId,
            };
            dbContext.TransactionAmountLimit.Add(model);
            dbContext.SaveChanges();
        }
    }
}