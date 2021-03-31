using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class TransferLimitServices
    {
        DB.FAXEREntities dbContext = null;
        CommonServices _CommonServices = null;
        public TransferLimitServices()
        {
            dbContext = new DB.FAXEREntities();
            _CommonServices = new CommonServices();
        }

        public void Add(TransferLimitViewModel vm)
        {
            TransferLimit model = new TransferLimit()
            {
                Amount = vm.Amount,
                Country = vm.Country,
                UserCategory = vm.UserCategory,
                CreatedBy = Common.StaffSession.LoggedStaff.StaffId,
                CreatedDate = DateTime.Now
            };
            dbContext.TransferLimit.Add(model);
            dbContext.SaveChanges();
        }

        public void Update(TransferLimitViewModel vm)
        {
            var data = List().Where(x => x.Id == vm.Id).FirstOrDefault();
            data.Amount = vm.Amount;
            data.Country = vm.Country;
            data.UserCategory = vm.UserCategory;

            dbContext.Entry<TransferLimit>(data).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public void DeleteTransferLimit(int id)
        {
            var data = List().Where(x => x.Id == id).FirstOrDefault();
            dbContext.TransferLimit.Remove(data);
            dbContext.SaveChanges();
        }

        public List<TransferLimitViewModel> GetTransferLimitList(string country)
        {
            var data = List();
            if (!string.IsNullOrEmpty(country))
            {
                data = data.Where(x => x.Country == country).ToList();
            }

            var result = (from c in data
                          select new TransferLimitViewModel()
                          {
                              Id = c.Id,
                              Amount = c.Amount,
                              Country = Common.Common.GetCountryName(c.Country),
                              UserCategory = c.UserCategory,
                              UserCategoryName = Common.Common.GetEnumDescription(c.UserCategory),

                          }).ToList();
            return result;
        }
        public TransferLimitViewModel GetTransferLimit(int id)
        {
            var data = List().Where(x => x.Id == id).ToList();
            var result = (from c in data
                          select new TransferLimitViewModel()
                          {
                              Id = c.Id,
                              Amount = c.Amount,
                              Country = c.Country,
                              UserCategory = c.UserCategory,
                              UserCategoryName = Common.Common.GetEnumDescription(c.UserCategory),

                          }).FirstOrDefault();
            return result;
        }

        public List<TransferLimit> List()
        {
            var data = dbContext.TransferLimit.ToList();
            return data;
        }
    }
}