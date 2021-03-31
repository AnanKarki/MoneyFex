using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class MobileWalletOperatorServices
    {
        DB.FAXEREntities dbContext = null;
        public MobileWalletOperatorServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        internal void DeleteMobileWalletOperator(int id)
        {
            var data = MobileWalletOperatorList().Where(x => x.Id == id).FirstOrDefault();
            dbContext.MobileWalletOperator.Remove(data);
            dbContext.SaveChanges();
        }
        public List<MobileWalletOperator> MobileWalletOperatorList()
        {
            var data = dbContext.MobileWalletOperator.ToList();
            return data;
        }

        public List<MobileWalletOperatorViewModel> GetMobileWalletOperatorList(string country)
        {
            var data = MobileWalletOperatorList();
            if (!string.IsNullOrEmpty(country))
            {
                data = data.Where(x => x.Country == country).ToList();
            }

            var result = (from c in data
                          select new MobileWalletOperatorViewModel()
                          {
                              Id = c.Id,
                              Country = Common.Common.GetCountryName(c.Country),
                              Code = c.Code,
                              Name = c.Name,

                          }).ToList();
            return result;
        }

        internal MobileWalletOperatorViewModel GetMobileWalletOperator(int id)
        {
            var data = MobileWalletOperatorList().Where(x => x.Id == id).ToList();

            var result = (from c in data
                          select new MobileWalletOperatorViewModel()
                          {
                              Id = c.Id,
                              Country = c.Country,
                              Code = c.Code,
                              Name = c.Name,

                          }).FirstOrDefault();
            return result;
        }

        internal void Add(MobileWalletOperatorViewModel vm)
        {
            MobileWalletOperator model = new MobileWalletOperator()
            {

                Country = vm.Country,
                Code = vm.Code,
                Name = vm.Name,
                CreatedBy = Common.StaffSession.LoggedStaff.StaffId,
                CreatedDate = DateTime.Now,
            };
            dbContext.MobileWalletOperator.Add(model);
            dbContext.SaveChanges();
        }

        internal void Update(MobileWalletOperatorViewModel vm)
        {
            var data = MobileWalletOperatorList().Where(x => x.Id == vm.Id).FirstOrDefault();
            data.Name = vm.Name;
            data.Code = vm.Code;
            data.Country = vm.Country;

            dbContext.Entry<MobileWalletOperator>(data).State = EntityState.Modified;
            dbContext.SaveChanges();
        }
    }
}