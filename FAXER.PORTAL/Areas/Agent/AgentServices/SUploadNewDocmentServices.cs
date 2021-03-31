using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class SUploadNewDocmentServices
    {
        DB.FAXEREntities dbContext = null;

        public SUploadNewDocmentServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public AgentNewDocument CreateNewDocument(AgentNewDocument model)
        {
            int agentStaffId = Common.AgentSession.AgentStaffLogin.AgentStaffId;
            var agentStaffInformation = dbContext.AgentStaffInformation.Where(x => x.Id == agentStaffId).FirstOrDefault();
            var vm = new AgentNewDocument()
            {
                AgentStaffId = agentStaffId,
                DocumentType = model.DocumentType,
                DocumentExpires = model.DocumentExpires,
                DocumentTitleName = model.DocumentTitleName,
                DocumentPhotoUrl = model.DocumentPhotoUrl,
                ExpiryDate = model.ExpiryDate,
                UploadDateTime = DateTime.Now,
                //AgentStaffAccountNo = agentStaffInformation.AgentMFSCode,
                //AgentStaffName = agentStaffInformation.FirstName + " " + agentStaffInformation.MiddleName + " " + agentStaffInformation.LastName,
            };
            var data = dbContext.AgentNewDocument.Add(vm);
            dbContext.SaveChanges();

            return vm;
        }



        public ServiceResult<List<AgentNewDocumentViewModel>> GetAgentDocuments(int AgentStaffId = 0)
        {

            var result = (from c in dbContext.AgentNewDocument.Where(x => x.AgentStaffId == AgentStaffId).ToList()
                          join d in dbContext.AgentStaffInformation on c.AgentStaffId equals d.Id
                          join e in dbContext.AgentInformation on d.AgentId equals e.Id
                          select new AgentNewDocumentViewModel()
                          {
                              AgentStaffId = c.AgentStaffId,
                              DocumentExpires = c.DocumentExpires,
                              DocumentPhotoUrl = c.DocumentPhotoUrl,
                              DocumentTitleName = c.DocumentTitleName,
                              DocumentType = c.DocumentType,
                              ExpiryDate = c.ExpiryDate.ToFormatedString("dd/MM/yyyy"),
                              UploadDateTime = c.UploadDateTime,
                              Id = c.Id,
                              AgentStaffAccountNo = e.AccountNo,
                              AgentStaffName = d.FirstName + " " + d.MiddleName + " " + d.LastName
                          }).ToList();

            return new ServiceResult<List<AgentNewDocumentViewModel>>()
            {
                Data = result,
                Status = ResultStatus.OK
            };

        }

        public AgentNewDocument GetAgentDocumentById(int id)
        {
            var Document = dbContext.AgentNewDocument.Where(x => x.Id == id).FirstOrDefault();
            return Document;
        }

        public AgentNewDocument UpdateNewDocument(AgentNewDocument model)
        {
            int agentStaffId = Common.AgentSession.AgentStaffLogin.AgentStaffId;
            var agentStaffInformation = dbContext.AgentStaffInformation.Where(x => x.Id == agentStaffId).FirstOrDefault();
            var agentNewDocument = dbContext.AgentNewDocument.Where(x => x.Id == model.Id).FirstOrDefault();


            agentNewDocument.DocumentType = model.DocumentType;
            agentNewDocument.DocumentExpires = model.DocumentExpires;
            agentNewDocument.DocumentTitleName = model.DocumentTitleName;
            agentNewDocument.DocumentPhotoUrl = model.DocumentPhotoUrl;
            agentNewDocument.ExpiryDate = model.ExpiryDate;
            agentNewDocument.UploadDateTime = agentNewDocument.UploadDateTime;
            //agentNewDocument.AgentStaffAccountNo = agentStaffInformation.AgentMFSCode;
            //agentNewDocument.AgentStaffName = agentStaffInformation.FirstName + " " + agentStaffInformation.MiddleName + " " + agentStaffInformation.LastName;

            dbContext.Entry<AgentNewDocument>(agentNewDocument).State = EntityState.Modified;
            dbContext.SaveChanges();
            return agentNewDocument;
        }
    }
}