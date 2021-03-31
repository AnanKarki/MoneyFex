using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Admin.ViewModels.SearchParamViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class StaffDocumentationServices
    {

        DB.FAXEREntities dbContext = null;
        CommonServices _commonServices = null;
        private List<BusinessDocumentationViewModel> staffDocumentVms;
        private IQueryable<StaffDocumentation> staffDocumentations;
        public StaffDocumentationServices()
        {
            dbContext = new DB.FAXEREntities();
            _commonServices = new CommonServices();
            staffDocumentVms = new List<BusinessDocumentationViewModel>();
        }

        public List<BusinessDocumentationViewModel> GetStaffDocumentations(string Country = "", string City = "",
            int staffId = 0, string staffName = "", string documentName = "", string createByName = "", string dateRange = "")
        {
            staffDocumentations = dbContext.StaffDocumentation;

            SearchStaffDocumentationByParam(new StaffSearchPramVM()
            {
                Country = Country,
                City = City,
                StaffId = staffId,
                StaffName = staffName,
                Title = documentName,
                CreatedByName = createByName,
                DateRange = dateRange
            });
            GetStaffDocumentations();
            if (!string.IsNullOrEmpty(createByName))
            {
                createByName = createByName.Trim();
                staffDocumentVms = staffDocumentVms.Where(x => x.CreatedByStaffName.ToLower().Contains(createByName.ToLower())).ToList();
            }

            if (!string.IsNullOrEmpty(staffName))
            {
                staffName = staffName.Trim();
                staffDocumentVms = staffDocumentVms.Where(x => x.StaffName.ToLower().Contains(staffName.ToLower())).ToList();

            }
            return staffDocumentVms;
        }

        private void GetStaffDocumentations()
        {
            staffDocumentVms = (from c in staffDocumentations.ToList()
                                join ctry in dbContext.Country on c.Country equals ctry.CountryCode
                                join staff in dbContext.StaffInformation on c.StaffId equals staff.Id
                                join Creator in dbContext.StaffInformation on c.CreatedBy equals Creator.Id into joined
                                from Creator in joined.DefaultIfEmpty()
                                select new BusinessDocumentationViewModel()
                                {
                                    Country = ctry.CountryName,
                                    CountryFlag = c.Country.ToLower(),
                                    CreatedDate = c.CreatedDate,
                                    AccountNo = c.AccountNo,
                                    Id = c.Id,
                                    City = c.City,
                                    CreatedBy = c.CreatedBy,
                                    DocumentExpires = c.DocumentExpires,
                                    DocumentName = c.DocumentName,
                                    CreatedByStaffName = Creator == null ? "" : Creator.FirstName + " " + Creator.MiddleName + " " + Creator.LastName, //_commonServices.getStaffName(c.CreatedBy),
                                    ExpiryDateString = c.ExpiryDate.ToFormatedString(),
                                    DocumentPhotoUrl = c.DocumentPhotoUrl,
                                    DocumentType = c.DocumentType,
                                    StaffId = c.StaffId,
                                    StaffName = staff == null ? "" : staff.FirstName + " " + staff.MiddleName + " " + staff.LastName,
                                }).ToList();
        }
        private void SearchStaffDocumentationByParam(StaffSearchPramVM searchPram)
        {
            if (!string.IsNullOrEmpty(searchPram.Country))
            {
                staffDocumentations = staffDocumentations.Where(x => x.Country == searchPram.Country);
            }
            if (!string.IsNullOrEmpty(searchPram.City))
            {
                staffDocumentations = staffDocumentations.Where(x => x.City == searchPram.City);
            }
            if (searchPram.StaffId != 0)
            {
                staffDocumentations = staffDocumentations.Where(x => x.StaffId == searchPram.StaffId);
            }

            if (!string.IsNullOrEmpty(searchPram.Title))
            {
                searchPram.Title = searchPram.Title.Trim();
                staffDocumentations = staffDocumentations.Where(x => x.DocumentName.ToLower().Contains(searchPram.Title.ToLower()));

            }
            if (!string.IsNullOrEmpty(searchPram.DateRange))
            {

                var Date = searchPram.DateRange.Split('-');
                var FromDate = DateTime.Parse(Date[0]);
                var ToDate = DateTime.Parse(Date[1]);
                staffDocumentations = staffDocumentations.Where(x => x.CreatedDate >= FromDate && x.CreatedDate <= ToDate);

            }
        }

        internal BusinessDocumentationViewModel GetStaffDocumentInfo(int id)
        {
            var data = dbContext.StaffDocumentation.Where(x => x.Id == id).ToList();
            var result = (from c in data
                          select new BusinessDocumentationViewModel()
                          {
                              Country = c.Country,
                              CreatedDate = c.CreatedDate,
                              AccountNo = c.AccountNo,
                              Id = c.Id,
                              StaffId = c.StaffId,
                              City = c.City,
                              CreatedBy = c.CreatedBy,
                              DocumentExpires = c.DocumentExpires,
                              DocumentName = c.DocumentName,
                              CreatedByStaffName = _commonServices.getStaffName(c.CreatedBy),
                              ExpiryDate = c.ExpiryDate,
                              DocumentPhotoUrl = c.DocumentPhotoUrl,
                              DocumentType = c.DocumentType,

                          }).FirstOrDefault();
            return result;
        }


        internal void Delete(int id)
        {
            var data = dbContext.StaffDocumentation.Where(x => x.Id == id).FirstOrDefault();
            dbContext.StaffDocumentation.Remove(data);
            dbContext.SaveChanges();
        }

        internal void UploadDocument(BusinessDocumentationViewModel vm)
        {
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;

            StaffDocumentation model = new StaffDocumentation()
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
                StaffId = vm.StaffId

            };

            dbContext.StaffDocumentation.Add(model);
            dbContext.SaveChanges();

        }

        internal void UpdateDocument(BusinessDocumentationViewModel vm)
        {
            var data = dbContext.StaffDocumentation.Where(x => x.Id == vm.Id).FirstOrDefault();

            data.ExpiryDate = vm.ExpiryDate;
            data.DocumentType = vm.DocumentType;
            data.DocumentPhotoUrl = vm.DocumentPhotoUrl;
            data.DocumentName = vm.DocumentName;
            data.DocumentExpires = vm.DocumentExpires;
            data.Country = vm.Country;
            data.City = vm.City;
            data.AccountNo = vm.AccountNo;
            data.StaffId = vm.StaffId;
            dbContext.Entry<StaffDocumentation>(data).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

    }
}