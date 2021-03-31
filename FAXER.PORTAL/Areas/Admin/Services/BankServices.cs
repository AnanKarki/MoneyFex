using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class BankServices
    {
        DB.FAXEREntities dbContext = null;
        public BankServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        internal void DeleteBank(int id)
        {
            var data = BankList().Where(x => x.Id == id).FirstOrDefault();
            dbContext.Bank.Remove(data);
            dbContext.SaveChanges();
        }
        public List<Bank> BankList()
        {
            var data = dbContext.Bank.ToList();
            return data;
        }

        public List<BankViewModel> GetBankList(string country ="")
        {
            var data = BankList();
            if (!string.IsNullOrEmpty(country))
            {
                data = data.Where(x => x.CountryCode == country).ToList();
            }

            var result = (from c in data
                          select new BankViewModel()
                          {
                              Id = c.Id,
                              CountryCode = c.CountryCode,
                              Address = c.Address,
                              Code = c.Code,
                              Name = c.Name,
                              Country = Common.Common.GetCountryName(c.CountryCode)

                          }).ToList();
            return result;
        }

        internal BankViewModel GetBank(int id)
        {
            var data = BankList().Where(x => x.Id == id).ToList();

            var result = (from c in data
                          select new BankViewModel()
                          {
                              Id = c.Id,
                              CountryCode = c.CountryCode,
                              Address = c.Address,
                              Code = c.Code,
                              Name = c.Name,

                          }).FirstOrDefault();
            return result;
        }

        internal void Add(BankViewModel vm)
        {
            Bank model = new Bank()
            {

                CountryCode = vm.CountryCode,
                Address = vm.Address,
                Code = vm.Code,
                Name = vm.Name,
                CreatedBy = Common.StaffSession.LoggedStaff.StaffId,
                CreatedDate = DateTime.Now
            };
            dbContext.Bank.Add(model);
            dbContext.SaveChanges();
        }

        internal void Update(BankViewModel vm)
        {
            var data = BankList().Where(x => x.Id == vm.Id).FirstOrDefault();
            data.Name = vm.Name;
            data.Address = vm.Address;
            data.Code = vm.Code;
            data.CountryCode = vm.CountryCode;

            dbContext.Entry<Bank>(data).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

    }
}