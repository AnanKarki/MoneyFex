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
    public class AgentDocumentationServices
    {
        FAXEREntities dbContext = null;
        CommonServices _commonServices = null;

        public AgentDocumentationServices()
        {
            dbContext = new FAXEREntities();
            _commonServices = new CommonServices();
        }

        public List<AgentNewDocument> GetAgentDocumentationList()
        {
            var data = dbContext.AgentNewDocument.ToList();
            return data;
        }
        public List<BusinessDocumentationViewModel> GetAgentDocumentation(string SendingCountry = "", int AgentId = 0, int DocumentType = 0,
            string city = "", string staffName = "", string AccountNo = "")
        {
            var data = GetAgentDocumentationList();


            var result = (from c in data
                          join d in dbContext.AgentStaffInformation on c.AgentStaffId equals d.Id
                          join e in dbContext.AgentInformation.Where(x => x.IsAUXAgent == false) on d.AgentId equals e.Id
                          join staff in dbContext.StaffInformation on c.CreatedBy equals staff.Id into joined
                          from staff in joined.DefaultIfEmpty()
                          select new BusinessDocumentationViewModel()
                          {
                              Country = Common.Common.GetCountryName(d.Country),
                              CountryCode = d.Country,
                              CountryFlag = d.Country.ToLower(),
                              CreatedDate = c.UploadDateTime,
                              AccountNo = d.AgentMFSCode,
                              Id = c.Id,
                              City = d.City,
                              DocumentExpires = c.DocumentExpires,
                              DocumentName = c.DocumentTitleName,
                              ExpiryDateString = c.ExpiryDate.ToFormatedString(),
                              DocumentPhotoUrl = c.DocumentPhotoUrl,
                              DocumentType = c.DocumentType,
                              //Treated sendername as agent name
                              SenderName = d.FirstName + " " + d.MiddleName + " " + d.LastName,
                              CreatedByStaffName = staff == null ? "" : staff.FirstName + " " + staff.MiddleName + " " + staff.LastName,
                              //Treated agent name as agent Id
                              AgentId = d.AgentId
                          }).ToList();

            if (!string.IsNullOrEmpty(SendingCountry))
            {
                result = result.Where(x => x.CountryCode == SendingCountry).ToList();
            }
            if (DocumentType != 0)
            {
                result = result.Where(x => x.DocumentType == (DocumentType)DocumentType).ToList();
            }

            if (!string.IsNullOrEmpty(city))
            {
                city = city.Trim();
                result = result.Where(x => x.City.ToLower().Contains(city.ToLower())).ToList();

            }
            if (!string.IsNullOrEmpty(staffName))
            {
                staffName = staffName.Trim();
                result = result.Where(x => x.CreatedByStaffName.ToLower().Contains(staffName.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(AccountNo))
            {
                AccountNo = AccountNo.Trim();
                result = result.Where(x => x.AccountNo.ToLower().Contains(AccountNo.ToLower())).ToList();
            }
            return result;
        }

        internal BusinessDocumentationViewModel GetUploadedDocumentInfo(int id)
        {
            var data = GetAgentDocumentationList().Where(x => x.Id == id).ToList();
            var result = (from c in data.ToList()
                          join agentStaff in dbContext.AgentStaffInformation on c.AgentStaffId equals agentStaff.Id
                          join d in dbContext.AgentInformation on agentStaff.AgentId equals d.Id
                          select new BusinessDocumentationViewModel()
                          {
                              Country = d.CountryCode,
                              CreatedDate = c.UploadDateTime,
                              AccountNo = c.AgentStaffAccountNo,
                              Id = c.Id,
                              City = d.City,
                              CreatedBy = c.CreatedBy,
                              DocumentExpires = c.DocumentExpires,
                              DocumentName = c.DocumentTitleName,
                              CreatedByStaffName = _commonServices.getStaffName(c.CreatedBy),
                              ExpiryDate = c.ExpiryDate,
                              DocumentPhotoUrl = c.DocumentPhotoUrl,
                              DocumentType = c.DocumentType,
                              SenderName = d.Name,
                              Status = c.Status,
                              IssuingCountry = c.IssuingCountry,
                              AgentId = d.Id

                          }).FirstOrDefault();
            return result;

        }

        public int GetPayingStaffId(int AgentId)
        {
            var data = dbContext.AgentStaffInformation.Where(x => x.AgentId == AgentId).Select(x => x.Id).FirstOrDefault();
            return data;

        }
        internal void UploadAgentDocument(BusinessDocumentationViewModel vm)
        {
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;
            int AgentStaff = GetPayingStaffId(vm.AgentId);

            AgentNewDocument model = new AgentNewDocument()
            {
                AgentStaffAccountNo = vm.AccountNo,
                CreatedBy = StaffId,
                UploadDateTime = DateTime.Now,
                DocumentExpires = vm.DocumentExpires,
                DocumentTitleName = vm.DocumentName,
                DocumentPhotoUrl = vm.DocumentPhotoUrl,
                DocumentType = vm.DocumentType,
                ExpiryDate = vm.ExpiryDate,
                AgentStaffId = AgentStaff,
                Status = vm.Status,
                IssuingCountry = vm.IssuingCountry,

            };

            dbContext.AgentNewDocument.Add(model);
            dbContext.SaveChanges();

        }

        internal void UpdateAgentDocument(BusinessDocumentationViewModel vm)
        {
            int AgentStaff = GetPayingStaffId(vm.AgentId);

            var data = dbContext.AgentNewDocument.Where(x => x.Id == vm.Id).FirstOrDefault();

            data.ExpiryDate = vm.ExpiryDate;
            data.DocumentType = vm.DocumentType;
            data.DocumentPhotoUrl = vm.DocumentPhotoUrl;
            data.DocumentTitleName = vm.DocumentName;
            data.DocumentExpires = vm.DocumentExpires;
            data.AgentStaffAccountNo = vm.AccountNo;
            data.Status = vm.Status;
            data.IssuingCountry = vm.IssuingCountry;
            data.AgentStaffId = AgentStaff;
            data.ExpiryDate = vm.ExpiryDate;

            dbContext.Entry<AgentNewDocument>(data).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        internal void Delete(int id)
        {
            var data = GetAgentDocumentationList().Where(x => x.Id == id).FirstOrDefault();
            dbContext.AgentNewDocument.Remove(data);
            dbContext.SaveChanges();
        }
    }
}