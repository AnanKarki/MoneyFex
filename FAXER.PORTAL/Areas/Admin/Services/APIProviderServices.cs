using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class APIProviderServices
    {
        DB.FAXEREntities dbContext = null;
        public APIProviderServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<APIProviderViewModel> GetAPIProviderList(string country = "", int TransferMethod = 0)
        {
            var data = dbContext.APIProvider.ToList();
            if (!string.IsNullOrEmpty(country))
            {
                data = data.Where(x => x.Country == country).ToList();
            }
            if (TransferMethod > 0)
            {
                data = data.Where(x => x.TransferMethod == (TransactionTransferMethod)TransferMethod).ToList();
            }

            var result = (from c in data
                          select new APIProviderViewModel()
                          {
                              Id = c.Id,
                              APIProviderName = c.APIProviderName,
                              ContactPerson = c.ContactPerson,
                              Country = c.Country,
                              CountryFlag = c.Country.ToLower(),
                              CreatedBY = c.CreatedBY,
                              CreatedDate = c.CreatedDate,
                              Email = c.Email,
                              Telephone = c.Telephone,
                              TransferMethod = c.TransferMethod,
                              TransferMethodName = Common.Common.GetEnumDescription(c.TransferMethod),

                          }).ToList();
            return result;
        }

        internal void Delete(int id)
        {
            var data = dbContext.APIProvider.Where(x => x.Id == id).FirstOrDefault();
            dbContext.APIProvider.Remove(data);
            dbContext.SaveChanges();
        }

        internal void AddAPiProvider(APIProviderViewModel vm)
        {
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;
            APIProvider model = new APIProvider()
            {
                TransferMethod = vm.TransferMethod,
                APIProviderName = vm.APIProviderName,
                ContactPerson = vm.ContactPerson,
                Country = vm.Country,
                CreatedBY = StaffId,
                CreatedDate = DateTime.Now,
                Email = vm.Email,
                Telephone = vm.Telephone
            };

            dbContext.APIProvider.Add(model);
            dbContext.SaveChanges();
        }
        internal void UpdateAPiProvider(APIProviderViewModel vm)
        {
            var data = dbContext.APIProvider.Where(x => x.Id == vm.Id).FirstOrDefault();
            data.APIProviderName = vm.APIProviderName;
            data.ContactPerson = vm.ContactPerson;
            data.Country = vm.Country;
            data.Email = vm.Email;
            data.Telephone = vm.Telephone;
            data.TransferMethod = vm.TransferMethod;
            dbContext.Entry<APIProvider>(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }

    }
}