using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TransferZero.Sdk.Model;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class AuxAgentDocumentationServices
    {

        DB.FAXEREntities dbContext = null;
        CommonServices _commonServices = null;
        public AuxAgentDocumentationServices()
        {
            dbContext = new DB.FAXEREntities();
            _commonServices = new CommonServices();
        }

        public List<BusinessDocumentationViewModel> GetAuxAgentDocumentation(string SendingCountry = "", int AgentId = 0, int DocumentType = 0, string City = "",
            string AgentAccountNo = "", string DocumentName = "", string DateRange = "", string Staff = "")
        {
            var data = dbContext.AgentNewDocument.ToList();
            var payingStaff = dbContext.AgentStaffInformation.ToList();
            var agentInfo = dbContext.AgentInformation.Where(x => x.IsAUXAgent == true).ToList();
            if (AgentId > 0)
            {
                payingStaff = payingStaff.Where(x => x.AgentId == AgentId).ToList();
            }

            if (DocumentType != 0)
            {
                data = data.Where(x => x.DocumentType == (DocumentType)DocumentType).ToList();
            }

            var result = (from c in data
                          join d in payingStaff on c.AgentStaffId equals d.Id
                          join agent in agentInfo on d.AgentId equals agent.Id
                          join staff in dbContext.StaffInformation on c.CreatedBy equals staff.Id into joined
                          from staff in joined.DefaultIfEmpty()
                          select new BusinessDocumentationViewModel()
                          {
                              Country = _commonServices.getCountryNameFromCode(d.Country),
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
                          }).ToList();

            if (!string.IsNullOrEmpty(SendingCountry))
            {
                result = result.Where(x => x.CountryCode == SendingCountry).ToList();
            }
            if (!string.IsNullOrEmpty(City))
            {
                City = City.Trim();
                result = result.Where(x => x.City.ToLower().Contains(City.ToLower())).ToList();

            }
            if (!string.IsNullOrEmpty(AgentAccountNo))
            {
                AgentAccountNo = AgentAccountNo.Trim();
                result = result.Where(x => x.AccountNo.ToLower().Contains(AgentAccountNo.ToLower())).ToList();

            }
            if (!string.IsNullOrEmpty(DocumentName))
            {
                DocumentName = DocumentName.Trim();
                result = result.Where(x => x.DocumentName.ToLower().Contains(DocumentName.ToLower())).ToList();

            }
            if (!string.IsNullOrEmpty(DateRange))
            {

                var Date = DateRange.Split('-');
                var FromDate = DateTime.Parse(Date[0]);
                var ToDate = DateTime.Parse(Date[1]);
                result = result.Where(x => x.CreatedDate >= FromDate && x.CreatedDate <= ToDate).ToList();


            }
            if (!string.IsNullOrEmpty(Staff))
            {
                Staff = Staff.Trim();
                result = result.Where(x => x.CreatedByStaffName.ToLower().Contains(Staff.ToLower())).ToList();

            }


            return result;
        }

        internal int GetNumberOfSenderRegisteredByAuxAgnet()
        {
            var numberofSenderRegisteredBYAuxAgent = dbContext.SenderRegisteredByAgent.Where(x => x.IsAuxAgent == true).Count(); 
            return numberofSenderRegisteredBYAuxAgent;
        }

        internal int GetNumberOfRegisteredAuxAgnet()
        {
            var numberofRegisteredAgent = dbContext.AgentInformation.Where(x => x.IsAUXAgent == true).Count();
            return numberofRegisteredAgent;
        }

        internal BusinessDocumentationViewModel GetUploadedDocumentInfo(int id)
        {
            var data = dbContext.AgentNewDocument.Where(x => x.Id == id).ToList();
            var result = (from c in data.ToList()
                          join agentstaff in dbContext.AgentStaffInformation on c.AgentStaffId equals agentstaff.Id
                          join d in dbContext.AgentInformation on agentstaff.AgentId equals d.Id
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

        internal void Delete(int id)
        {
            var data = dbContext.AgentNewDocument.Where(x => x.Id == id).FirstOrDefault();
            dbContext.AgentNewDocument.Remove(data);
            dbContext.SaveChanges();
        }


        internal void UploadAuxAgentDocument(BusinessDocumentationViewModel vm)
        {
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;

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
                AgentStaffId = vm.AgentId,
                Status = vm.Status,
                IssuingCountry = vm.IssuingCountry,

            };

            dbContext.AgentNewDocument.Add(model);
            dbContext.SaveChanges();

        }

        internal void UpdateAuxAgentDocument(BusinessDocumentationViewModel vm)
        {
            var data = dbContext.AgentNewDocument.Where(x => x.Id == vm.Id).FirstOrDefault();

            data.ExpiryDate = vm.ExpiryDate;
            data.DocumentType = vm.DocumentType;
            data.DocumentPhotoUrl = vm.DocumentPhotoUrl;
            data.DocumentTitleName = vm.DocumentName;
            data.DocumentExpires = vm.DocumentExpires;
            data.AgentStaffAccountNo = vm.AccountNo;
            data.Status = vm.Status;
            data.IssuingCountry = vm.IssuingCountry;
            data.AgentStaffId = vm.AgentId;

            data.ExpiryDate = vm.ExpiryDate;

            dbContext.Entry<AgentNewDocument>(data).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

    }
}