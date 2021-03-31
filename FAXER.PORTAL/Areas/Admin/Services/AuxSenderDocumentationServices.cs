using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Admin.ViewModels.SearchParamViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TransferZero.Sdk.Model;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class AuxSenderDocumentationServices
    {

        DB.FAXEREntities dbContext = null;
        CommonServices _commonServices = null;
        private IQueryable<SenderBusinessDocumentation> senderBusinessDocumentations;
        private IQueryable<FaxerInformation> senderInformation;
        private List<BusinessDocumentationViewModel> businessDocumentationViewModels;
        public AuxSenderDocumentationServices()
        {
            dbContext = new DB.FAXEREntities();
            _commonServices = new CommonServices();
            businessDocumentationViewModels = new List<BusinessDocumentationViewModel>();
        }

        public List<BusinessDocumentationViewModel> GetAuxSenderDocuments(string Country = "", string City = "", string SenderName = "",
            string CustomerNo = "", string DocumentName = "", string Uploader = "", int Status = 0, string DateRange = "")
        {
            senderBusinessDocumentations = dbContext.SenderBusinessDocumentation;
            senderInformation = dbContext.FaxerInformation.Where(x => x.RegisteredByAgent == true);
            SearchAuxSenderDocumentationByParam(new SenderDocumentSearchParamVm()
            {
                AccountNo = CustomerNo,
                City = City,
                SenderName = SenderName,
                Country = Country,
                DocumentName = DocumentName,
                Uploader = Uploader,
                DateRange = DateRange,
                Status = Status,

            });


            GetAuxSendersDocuments();
            return businessDocumentationViewModels;
        }

        private void GetAuxSendersDocuments()
        {
            var AuxSenderAgentList = dbContext.SenderRegisteredByAgent.Where(x => x.IsAuxAgent == true);
            businessDocumentationViewModels = (from c in senderBusinessDocumentations
                                               join d in senderInformation on c.SenderId equals d.Id
                                               join e in AuxSenderAgentList on c.SenderId equals e.SenderId
                                               join f in dbContext.AgentInformation on e.AgentId equals f.Id
                                               join ctry in dbContext.Country on c.Country equals ctry.CountryCode
                                               join staff in dbContext.StaffInformation on c.CreatedBy equals staff.Id into joined
                                               from staff in joined.DefaultIfEmpty()
                                               select new BusinessDocumentationViewModel()
                                               {
                                                   Country = ctry.CountryName,
                                                   CountryFlag = c.Country.ToLower(),
                                                   CreatedDate = c.CreatedDate,
                                                   AccountNo = c.AccountNo,
                                                   Id = c.Id,
                                                   SenderId = c.SenderId,
                                                   City = c.City,
                                                   CreatedBy = c.CreatedBy,
                                                   DocumentExpires = c.DocumentExpires,
                                                   DocumentName = c.DocumentName,
                                                   CreatedByStaffName = c.IsUploadedFromAuxAgentAdmin == true ? staff == null ? "" : staff.FirstName + " " + staff.MiddleName + " " + staff.LastName : f.Name,
                                                   ExpiryDateString = c.ExpiryDate.ToString(),
                                                   DocumentPhotoUrl = c.DocumentPhotoUrl,
                                                   DocumentType = c.DocumentType,
                                                   SenderName = d.FirstName + " " + d.MiddleName + " " + d.LastName,
                                                   StatusName = c.Status.ToString()
                                               }).ToList();
        }

        private void SearchAuxSenderDocumentationByParam(SenderDocumentSearchParamVm searchParam)
        {

            if (!string.IsNullOrEmpty(searchParam.Country))
            {
                senderBusinessDocumentations = senderBusinessDocumentations.Where(x => x.Country == searchParam.Country);
            }
            if (!string.IsNullOrEmpty(searchParam.City))
            {
                senderBusinessDocumentations = senderBusinessDocumentations.Where(x => x.City == searchParam.City);
            }
            if (!string.IsNullOrEmpty(searchParam.SenderName))
            {
                searchParam.SenderName = searchParam.SenderName.Trim();
                senderInformation = senderInformation.Where(x => x.FirstName.ToLower().Contains(searchParam.SenderName.ToLower()) ||
                                                                 x.MiddleName.ToLower().Contains(searchParam.SenderName.ToLower()) ||
                                                                 x.LastName.ToLower().Contains(searchParam.SenderName.ToLower()));

            }
            if (!string.IsNullOrEmpty(searchParam.AccountNo))
            {
                searchParam.AccountNo = searchParam.AccountNo.Trim();
                senderInformation = senderInformation.Where(x => x.AccountNo.ToLower().Contains(searchParam.AccountNo.ToLower()));

            }
            if (!string.IsNullOrEmpty(searchParam.DocumentName))
            {
                searchParam.DocumentName = searchParam.DocumentName.Trim();
                senderBusinessDocumentations = senderBusinessDocumentations.Where(x => x.DocumentName.ToLower().Contains(searchParam.DocumentName.ToLower()));

            }
            if (!string.IsNullOrEmpty(searchParam.Uploader))
            {
                searchParam.Uploader = searchParam.Uploader.Trim();
                //result = result.Where(x => x.CreatedByStaffName.ToLower().Contains(searchParam.Uploader.ToLower())).ToList();

            }
            if (searchParam.Status != 3)
            {
                senderBusinessDocumentations = senderBusinessDocumentations.Where(x => x.Status == (DocumentApprovalStatus)searchParam.Status);

            }
            if (!string.IsNullOrEmpty(searchParam.DateRange))
            {
                var Date = searchParam.DateRange.Split('-');
                var FromDate = DateTime.Parse(Date[0]);
                var ToDate = DateTime.Parse(Date[1]);
                senderBusinessDocumentations = senderBusinessDocumentations.Where(x => DbFunctions.TruncateTime(x.CreatedDate) >= DbFunctions.TruncateTime(FromDate) &&
                                                                                       DbFunctions.TruncateTime(x.CreatedDate) <= DbFunctions.TruncateTime(ToDate));
            }


        }

        internal BusinessDocumentationViewModel GetUploadedDocumentInfo(int id)
        {
            var data = dbContext.SenderBusinessDocumentation.Where(x => x.Id == id).ToList();
            var result = (from c in data
                          select new BusinessDocumentationViewModel()
                          {
                              Country = c.Country,
                              CreatedDate = c.CreatedDate,
                              AccountNo = c.AccountNo,
                              Id = c.Id,
                              SenderId = c.SenderId,
                              City = c.City,
                              CreatedBy = c.CreatedBy,
                              DocumentExpires = c.DocumentExpires,
                              DocumentName = c.DocumentName,
                              CreatedByStaffName = _commonServices.getStaffName(c.CreatedBy),
                              ExpiryDate = c.ExpiryDate,
                              DocumentPhotoUrl = c.DocumentPhotoUrl,
                              DocumentType = c.DocumentType,
                              SenderName = _commonServices.getFaxerName(c.SenderId),
                              Status = c.Status,
                              IssuingCountry = c.IssuingCountry

                          }).FirstOrDefault();
            return result;
        }

        internal void Delete(int id)
        {
            var data = dbContext.SenderBusinessDocumentation.Where(x => x.Id == id).FirstOrDefault();
            dbContext.SenderBusinessDocumentation.Remove(data);
            dbContext.SaveChanges();
        }
        public List<DropDownViewModel> GetAuxSender()
        {
            var result = (from c in dbContext.FaxerInformation.Where(x => x.IsBusiness == true).ToList()
                          join d in dbContext.BusinessRelatedInformation on c.Id equals d.FaxerId
                          select new DropDownViewModel()
                          {
                              Id = c.Id,
                              Name = d.BusinessName,
                          }).GroupBy(x => x.Name).Select(x => x.FirstOrDefault()).ToList();
            return result;

        }

        internal void UploadAuxSenderDocument(BusinessDocumentationViewModel vm)
        {
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;

            SenderBusinessDocumentation model = new SenderBusinessDocumentation()
            {
                AccountNo = vm.AccountNo,
                City = vm.City,
                Country = vm.Country,
                CreatedBy = StaffId,
                CreatedDate = DateTime.Now,
                DocumentExpires = vm.DocumentExpires,
                DocumentName = vm.DocumentName,
                DocumentPhotoUrl = vm.DocumentPhotoUrl,
                DocumentType = vm.DocumentType,
                ExpiryDate = vm.ExpiryDate,
                SenderId = vm.SenderId,
                IsUploadedFromAuxAgentAdmin = true,
                Status = vm.Status,
                IssuingCountry = vm.IssuingCountry


            };

            dbContext.SenderBusinessDocumentation.Add(model);
            dbContext.SaveChanges();

        }

        internal void UpdateAuxSenderDocument(BusinessDocumentationViewModel vm)
        {
            var data = dbContext.SenderBusinessDocumentation.Where(x => x.Id == vm.Id).FirstOrDefault();
            data.SenderId = vm.SenderId;
            data.ExpiryDate = vm.ExpiryDate;
            data.DocumentType = vm.DocumentType;
            data.DocumentPhotoUrl = vm.DocumentPhotoUrl;
            data.DocumentName = vm.DocumentName;
            data.DocumentExpires = vm.DocumentExpires;
            data.Country = vm.Country;
            data.City = vm.City;
            data.AccountNo = vm.AccountNo;
            data.IsUploadedFromAuxAgentAdmin = data.IsUploadedFromAuxAgentAdmin;
            data.Status = vm.Status;
            data.IssuingCountry = vm.IssuingCountry;
            dbContext.Entry<SenderBusinessDocumentation>(data).State = EntityState.Modified;
            dbContext.SaveChanges();

        }
    }
}