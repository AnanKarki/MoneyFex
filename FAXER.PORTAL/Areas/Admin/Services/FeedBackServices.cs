using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class FeedBackServices
    {
        DB.FAXEREntities dbContext = null;
        public FeedBackServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        internal List<FeedBackViewModel> GetFeedBacks(string country = "", int customerType = 0, int platform = 0, string name="")
        {
            var data = dbContext.FeedBacks.ToList();
            if (!string.IsNullOrEmpty(country))
            {
                data = data.Where(x => x.Country == country).ToList();
            }
            if (customerType > 0)
            {
                data = data.Where(x => x.CustomerType == (CustomerType)customerType).ToList();

            }
            if (platform > 0)
            {
                data = data.Where(x => x.Platform == (Platform)platform).ToList();

            }
            var resut = (from c in data
                         select new FeedBackViewModel()
                         {
                             Id = c.Id,
                             Country = Common.Common.GetCountryName(c.Country),
                             CountryFlag = c.Country.ToLower(),
                             Platform = c.Platform,
                             CreatedBy = c.CreatedBy,
                             CustomerType = c.CustomerType,
                             CreatedDate = c.CreatedDate,
                             CustomerName = c.CustomerName,
                             FeedBack = c.FeedBack,
                             CustomerTypeName = Common.Common.GetEnumDescription(c.CustomerType),
                             PlatformName = c.Platform.ToString(),
                             CustomerFirstName = GetCustomerFirstName(c.Id)
                         }).ToList();

            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim();
                resut = resut.Where(x => x.CustomerName.ToLower().Contains(name.ToLower())).ToList();

            }

            return resut;
        }

        private string GetCustomerFirstName(int id)
        {
            var data = dbContext.FeedBacks.Where(x => x.Id == id).FirstOrDefault();
            var name = data.CustomerName.Split(' ');
            string FirstName = name[0];
            return FirstName;
        }

        internal FeedBackViewModel GetFeedBack(int id)
        {
            var data = dbContext.FeedBacks.Where(x => x.Id == id).ToList();

            var resut = (from c in data
                         select new FeedBackViewModel()
                         {
                             Id = c.Id,
                             Country = c.Country,
                             Platform = c.Platform,
                             CreatedBy = c.CreatedBy,
                             CustomerType = c.CustomerType,
                             CreatedDate = c.CreatedDate,
                             CustomerName = c.CustomerName,
                             FeedBack = c.FeedBack,
                             CustomerTypeName = c.CustomerType.ToString(),
                             PlatformName = c.Platform.ToString()
                         }).FirstOrDefault();
            return resut;
        }

        internal void Delete(int id)
        {
            var data = dbContext.FeedBacks.Where(x => x.Id == id).FirstOrDefault();
            dbContext.FeedBacks.Remove(data);
            dbContext.SaveChanges();
        }

        internal void AddFeedback(FeedBackViewModel vm)
        {
            int staffId = Common.StaffSession.LoggedStaff.StaffId;
            FeedBacks model = new FeedBacks()
            {
                Country = vm.Country,
                Platform = vm.Platform,
                FeedBack = vm.FeedBack,
                CustomerType = vm.CustomerType,
                CustomerName = vm.CustomerName,
                CreatedDate = DateTime.Now,
                CreatedBy = staffId,

            };
            dbContext.FeedBacks.Add(model);
            dbContext.SaveChanges();
        }

        internal void UpdateFeedback(FeedBackViewModel vm)
        {
            var model = dbContext.FeedBacks.Where(x => x.Id == vm.Id).FirstOrDefault();
            model.Country = vm.Country;
            model.CustomerName = vm.CustomerName;
            model.CustomerType = vm.CustomerType;
            model.FeedBack = vm.FeedBack;
            model.Platform = vm.Platform;

            dbContext.Entry<FeedBacks>(model).State = EntityState.Modified;
            dbContext.SaveChanges();
        }
        public ServiceResult<IQueryable<FeedBacks>> FeedbackList()
        {
            return new ServiceResult<IQueryable<FeedBacks>>()
            {
                Data = dbContext.FeedBacks,
                Status = ResultStatus.OK
            };

        }
    }
}