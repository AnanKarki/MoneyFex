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
    public class LogoAssignServices
    {
        DB.FAXEREntities db = null;
        public LogoAssignServices()
        {
            db = new DB.FAXEREntities();
        }
        public bool AddLogoAssign(LogoAssignViewModel vm)
        {
            int staffId = Common.StaffSession.LoggedStaff.StaffId;

            LogoAssign logoAssignMaster = new LogoAssign()
            {
                ReceivingCountry = vm.Master.ReceivingCountry,
                SendingCountry = vm.Master.SendingCountry,
                CreatedBy = staffId,
                CreatedDate = DateTime.Now,
                Label = vm.Master.Label,
                Services = vm.Master.Services
            };
            var logoAssign = db.LogoAssign.Add(logoAssignMaster);
            db.SaveChanges();

            List<LogoAssignDetails> details = (from c in vm.Details.Where(x => x.IsChecked == true)
                                               select new LogoAssignDetails()
                                               {
                                                   LogoAssignId = logoAssign.Id,
                                                   ServiceProvider = c.LogoUploadId,
                                               }).ToList();

            TransferServiceDetails(details);
            return true;
        }

        internal List<LogoAssignViewModel> GetLogoAssignedData()
        {

            List<LogoAssignViewModel> data = new List<LogoAssignViewModel>();

            List<LogoAssignMasterViewModel> MasterData = (from c in db.LogoAssign.ToList()
                                                          select new LogoAssignMasterViewModel()
                                                          {
                                                              Id = c.Id,
                                                              ReceivingCountry =Common.Common.GetCountryName(c.ReceivingCountry),
                                                              SendingCountry = Common.Common.GetCountryName(c.SendingCountry),
                                                              Services = c.Services,
                                                              Label = c.Label
                                                          }).ToList();



            foreach (var item in MasterData)
            {

                List<LogoAssignDetailsViewModel> detailList = GetAsignedLogoDetails(item.Id);

                data.Add(new LogoAssignViewModel()
                {
                    Details = detailList,
                    Master = item
                });
            }
            return data;
        }
        public List<LogoAssignDetailsViewModel> GetAsignedLogoDetails(int transferServiceId)
        {
            List<LogoAssignDetailsViewModel> detailList = (from c in db.LogoAssignDetails
                                                           select new LogoAssignDetailsViewModel()
                                                           {
                                                               Id = c.Id,
                                                               LogoAssignId = c.LogoAssignId,
                                                               LogoUploadId = c.ServiceProvider,
                                                              IsChecked = true
                                                           }).ToList();


            return detailList;
        }
        public List<LogoAssignDetailsViewModel> GetLogos(string ReceivingCountry , int Services = 0)
        {
            var Logos = db.LogosUpload.Where(x => x.Country == ReceivingCountry && x.Service == (TransactionTransferMethod)Services ).ToList();
            List<LogoAssignDetailsViewModel> details = new List<LogoAssignDetailsViewModel>();
            details = (from c in Logos
                       select new LogoAssignDetailsViewModel()
                       {
                           LogoUploadId = c.Id,
                           ImageUrl = c.Logo,
                           LogoUpload = c.Title,
                       }).ToList();
            return details;
        }

        internal void UpdateLogoAssign(LogoAssignViewModel vm)
        {
            var master = db.LogoAssign.Where(x => x.Id == vm.Master.Id).FirstOrDefault();
            master.Id = vm.Master.Id;
            master.SendingCountry = vm.Master.SendingCountry;
            master.ReceivingCountry = vm.Master.ReceivingCountry;
            master.CreatedBy = vm.Master.CreatedBy;
            master.CreatedDate = master.CreatedDate;
            master.Label = vm.Master.Label;
            master.Services = vm.Master.Services;

            db.Entry<LogoAssign>(master).State = EntityState.Modified;
            db.SaveChanges();

            var details = db.LogoAssignDetails.Where(x => x.LogoAssignId == master.Id).ToList();
            db.LogoAssignDetails.RemoveRange(details);
            db.SaveChanges();

            List<LogoAssignDetails> logoAssignDetails = (from c in vm.Details.Where(x => x.IsChecked == true)
                                                         select new LogoAssignDetails()
                                                         {
                                                             LogoAssignId = master.Id,
                                                             ServiceProvider = c.LogoUploadId,

                                                         }).ToList();
            TransferServiceDetails(logoAssignDetails);

        }
        public bool TransferServiceDetails(List<LogoAssignDetails> details)
        {
            db.LogoAssignDetails.AddRange(details);
            db.SaveChanges();
            return true;
        }

        public bool Remove(int id)
        {
            var master = db.LogoAssign.Where(x => x.Id == id).FirstOrDefault();
            db.LogoAssign.Remove(master);
            db.SaveChanges();
            return true;
        }

        public ServiceResult<IQueryable<LogoAssign>> MasterData()
        {
            return new ServiceResult<IQueryable<LogoAssign>>()
            {
                Data = db.LogoAssign,
                Status = ResultStatus.OK
            };

        }
        public ServiceResult<IQueryable<LogoAssignDetails>> DetailsData()
        {
            return new ServiceResult<IQueryable<LogoAssignDetails>>()
            {
                Data = db.LogoAssignDetails,
                Status = ResultStatus.OK
            };

        }

    }
}