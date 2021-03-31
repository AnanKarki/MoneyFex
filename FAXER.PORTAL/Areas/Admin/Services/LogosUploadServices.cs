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
    public class LogosUploadServices
    {
        DB.FAXEREntities db = null;
        public LogosUploadServices()
        {
            db = new DB.FAXEREntities();
        }

        public ServiceResult<LogosUpload> Add(LogosUploadViewModel vm)
        {
            LogosUpload model = new LogosUpload()
            {
                Logo = vm.Logo,
                Title = vm.Title,
                Country = vm.Country,
                Service = vm.Service,
                WebstieUrl = vm.WebstieUrl,

            };

            db.LogosUpload.Add(model);
            db.SaveChanges();
            return new ServiceResult<LogosUpload>()
            {
                Data = model,
                Message = "Save",
                Status = ResultStatus.OK

            };
        }

        public List<LogosUploadViewModel> List(string Country = "", int TransferMethod = 0)
        {
            var data = db.LogosUpload.ToList();
            if (!string.IsNullOrEmpty(Country))
            {
                data = data.Where(x => x.Country == Country).ToList();
            }
            if (TransferMethod != 0)
            {
                if (TransferMethod == 7)
                {
                    data = db.LogosUpload.ToList();
                }
                else
                {
                    data = data.Where(x => x.Service == (TransactionTransferMethod)TransferMethod).ToList();
                }
            }
            List<LogosUploadViewModel> vm = new List<LogosUploadViewModel>();
            vm = (from c in data
                  select new LogosUploadViewModel
                  {
                      Id = c.Id,
                      Logo = c.Logo,
                      Title = c.Title,
                      Country = c.Country,
                      Service = c.Service,
                      WebstieUrl = c.WebstieUrl,
                  }).ToList();

            return vm;
        }
        public ServiceResult<IQueryable<LogosUpload>> LogosUploadData()
        {
            return new ServiceResult<IQueryable<LogosUpload>>()
            {
                Data = db.LogosUpload,
                Status = ResultStatus.OK
            };

        }
        public ServiceResult<LogosUpload> Update(LogosUploadViewModel vm)
        {
            LogosUpload model = new LogosUpload()
            {
                Id = vm.Id,
                Logo = vm.Logo,
                Title = vm.Title,
                Country = vm.Country,
                Service = vm.Service,
                WebstieUrl = vm.WebstieUrl,

            };
            db.Entry<LogosUpload>(model).State = EntityState.Modified;
            db.SaveChanges();
            return new ServiceResult<LogosUpload>()
            {
                Data = model,
                Message = "Update",
                Status = ResultStatus.OK
            };
        }


        public ServiceResult<int> Remove(LogosUpload model)
        {
            db.LogosUpload.Remove(model);
            db.SaveChanges();
            return new ServiceResult<int>()
            {
                Data = 1,
                Message = "Remove",
                Status = ResultStatus.OK
            };
        }

    }
}