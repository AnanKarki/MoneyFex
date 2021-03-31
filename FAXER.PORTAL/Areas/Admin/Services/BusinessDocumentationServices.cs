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
    public class BusinessDocumentationServices
    {
        DB.FAXEREntities dbContext = null;
        CommonServices _commonServices = null;
        public BusinessDocumentationServices()
        {
            dbContext = new DB.FAXEREntities();
            _commonServices = new CommonServices();
        }

        public List<BusinessDocumentationViewModel> GetBusinessDocuments(string Country = "", string City = "", string SenderName = "", string CustomerNo = "",
            string DocumentName = "", string staffName = "", string DateRange = "")
        {
            IQueryable<SenderBusinessDocumentation> senderBusinessDocumentations = dbContext.SenderBusinessDocumentation;
            IQueryable<FaxerInformation> senderInfo = dbContext.FaxerInformation.Where(x => x.IsBusiness == true);
            IQueryable<StaffInformation> staffInfo = dbContext.StaffInformation;
            if (!string.IsNullOrEmpty(Country))
            {
                senderBusinessDocumentations = senderBusinessDocumentations.Where(x => x.Country == Country);
            }
            if (!string.IsNullOrEmpty(City))
            {
                senderBusinessDocumentations = senderBusinessDocumentations.Where(x => x.City == City);
            }
            if (!string.IsNullOrEmpty(SenderName))
            {
                SenderName = SenderName.Trim();
                senderInfo = senderInfo.Where(x => x.FirstName.ToLower().Contains(SenderName.ToLower()) ||
                                                   x.MiddleName.ToLower().Contains(SenderName.ToLower()) ||
                                                   x.LastName.ToLower().Contains(SenderName.ToLower()));
            }
            if (!string.IsNullOrEmpty(CustomerNo))
            {
                CustomerNo = CustomerNo.Trim();
                senderInfo = senderInfo.Where(x => x.AccountNo.ToLower().Contains(CustomerNo.ToLower()));

            }
            if (!string.IsNullOrEmpty(DocumentName))
            {
                DocumentName = DocumentName.Trim();
                senderBusinessDocumentations = senderBusinessDocumentations.Where(x => x.DocumentName.ToLower().Contains(DocumentName.ToLower()));

            }
            if (!string.IsNullOrEmpty(staffName))
            {
                staffName = staffName.Trim();
                staffInfo = staffInfo.Where(x => x.FirstName.ToLower().Contains(staffName.ToLower()) ||
                                                   x.MiddleName.ToLower().Contains(staffName.ToLower()) ||
                                                   x.LastName.ToLower().Contains(staffName.ToLower()));
            }
            if (!string.IsNullOrEmpty(DateRange))
            {

                var Date = DateRange.Split('-');
                DateTime startDate = Date[0].ToDateTime();
                DateTime endDate = Date[1].ToDateTime();
                senderBusinessDocumentations = senderBusinessDocumentations.Where(x => x.CreatedDate >= startDate && x.CreatedDate <= endDate);

            }
            var result = (from c in senderBusinessDocumentations
                          join d in senderInfo on c.SenderId equals d.Id
                          join ctry in dbContext.Country on c.Country equals ctry.CountryCode
                          join staff in staffInfo on c.CreatedBy equals staff.Id into joined
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
                              CreatedByStaffName = staff == null ? "" : staff.FirstName + " " + staff.MiddleName + " " + staff.LastName, //_commonServices.getStaffName(c.CreatedBy),
                              ExpiryDate = c.ExpiryDate,
                              DocumentPhotoUrl = c.DocumentPhotoUrl,
                              DocumentPhotoUrlTwo = c.DocumentPhotoUrlTwo,
                              DocumentType = c.DocumentType,
                              SenderName = d.FirstName + " " + d.MiddleName + " " + d.LastName,
                              StatusName = c.Status.ToString()
                          }).OrderBy(x => x.CreatedDate).ToList();
            return result;
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
                              DocumentPhotoUrlTwo = c.DocumentPhotoUrlTwo,
                              DocumentType = c.DocumentType,
                              SenderName = _commonServices.getFaxerName(c.SenderId),
                              Status = c.Status,
                              ReasonForDisApproval = c.ReasonForDisApproval,
                              ReasonForDisApprovalByAdmin = c.ReasonForDisApprovalByAdmin
                          }).FirstOrDefault();
            return result;
        }
        public string GetDocumentPhotoUrl(int id)
        {
            var data = dbContext.SenderBusinessDocumentation.Where(x => x.Id == id).Select(x => x.DocumentPhotoUrl).FirstOrDefault();
            return data;
        }

        public SenderBusinessDocumentation GetDocumentDetails(int id)
        {
            var data = dbContext.SenderBusinessDocumentation.Where(x => x.Id == id).FirstOrDefault();
            return data;
        }

        internal void Delete(int id)
        {
            var data = dbContext.SenderBusinessDocumentation.Where(x => x.Id == id).FirstOrDefault();
            dbContext.SenderBusinessDocumentation.Remove(data);
            dbContext.SaveChanges();
        }
        public List<DropDownViewModel> GetBuisnessSender()
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

        internal void UploadDocument(BusinessDocumentationViewModel vm)
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
                DocumentPhotoUrlTwo = vm.DocumentPhotoUrlTwo,
                DocumentType = vm.DocumentType,
                ExpiryDate = vm.ExpiryDate,
                SenderId = vm.SenderId,
                Status = vm.Status,
                ReasonForDisApproval = vm.ReasonForDisApproval,
                ReasonForDisApprovalByAdmin = vm.ReasonForDisApprovalByAdmin,

            };

            dbContext.SenderBusinessDocumentation.Add(model);
            dbContext.SaveChanges();

        }

        internal void UpdateDocument(BusinessDocumentationViewModel vm)
        {
            var data = dbContext.SenderBusinessDocumentation.Where(x => x.Id == vm.Id).FirstOrDefault();
            data.SenderId = vm.SenderId;
            data.ExpiryDate = vm.ExpiryDate;
            data.DocumentType = vm.DocumentType;
            data.DocumentPhotoUrl = vm.DocumentPhotoUrl;
            data.DocumentPhotoUrlTwo = vm.DocumentPhotoUrlTwo;
            data.DocumentName = vm.DocumentName;
            data.DocumentExpires = vm.DocumentExpires;
            data.Country = vm.Country;
            data.City = vm.City;
            data.AccountNo = vm.AccountNo;
            data.Status = vm.Status;
            data.ReasonForDisApproval = vm.ReasonForDisApproval;
            data.ReasonForDisApprovalByAdmin = vm.ReasonForDisApprovalByAdmin;
            dbContext.Entry<SenderBusinessDocumentation>(data).State = EntityState.Modified;
            dbContext.SaveChanges();
            if (vm.Status == DocumentApprovalStatus.Approved)
            {
                SenderDocumentationServices _senderDocumentationServices = new SenderDocumentationServices();
                _senderDocumentationServices.ReinitialAllTransaction(data.SenderId);
            }
        }
    }
}