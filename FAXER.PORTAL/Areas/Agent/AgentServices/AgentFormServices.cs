using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class AgentFormServices
    {
        DB.FAXEREntities dbContext = null;
        int AgentStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId;
        public AgentFormServices()
        {
            dbContext = new DB.FAXEREntities();
        }
        public List<Models.MySubmittedSARFormDetialsVm> GetMySubmittedSARForms(string Year, Month month)
        {

            IQueryable<DB.SARForm> data;
            data = dbContext.SARForm.Where(x => x.AgentStaffId == AgentStaffId);
            if (!string.IsNullOrEmpty(Year))
            {
                data = data.Where(x => x.SubmittedDate.Year == int.Parse(Year));
            }
            if ((int)month > 0)
            {
                data = data.Where(x => x.SubmittedDate.Month == (int)month);
            }
            var result = (from c in data.ToList()
                          select new MySubmittedSARFormDetialsVm()
                          {
                              AgentAccountNo = c.AgentAccountNo,
                              AgentName = c.AgentStaffName,
                              CustomerName = c.FirstName + " " + c.LastName,
                              StaffId = c.AgentStaffLoginCode,
                              TransactionId = c.Id,
                              Year = c.SubmittedDate.Year.ToString(),
                              Month = Enum.GetName(typeof(Month), c.SubmittedDate.Month)
                          }).ToList();
            return result;


        }



        public List<Models.ThirdPartyMoneyTransferListVM> GetThirdPartyMoneyTransferDetails(string Year, Month month)
        {

            IQueryable<DB.ThirdPartyMoneyTransfer> data;
            data = dbContext.ThirdPartyMoneyTransfer.Where(x => x.AgentStaffId == AgentStaffId);
            if (!string.IsNullOrEmpty(Year))
            {
                data = data.Where(x => x.SubmittedDate.Year == int.Parse(Year));
            }
            if ((int)month > 0) 
            {
                data = data.Where(x => x.SubmittedDate.Month == (int)month);
            }
            var result = (from c in data.ToList()
                          select new ThirdPartyMoneyTransferListVM()
                          {
                              AgentAccountNo = c.AgentStaffAccountNo,
                              AgentName = c.AgentStaffName,
                              SenderName = c.SenderORBusinessName,
                              AgentStaffLoginCode = c.AgentStaffLoginCode,
                              TransactionId = c.Id,
                              Year = c.SubmittedDate.Year.ToString(),
                              Month = Enum.GetName(typeof(Month), c.SubmittedDate.Month)
                          }).ToList();
            return result;


        }
        public List<Models.LargeFundMoneyTransferListVM> GetLargeFundMoneyTransferDetails(string Year, Month month)
        {

            AgentCommonServices _agentCommonServices = new AgentCommonServices();
            IQueryable<DB.LargeFundMoneyTransferFormData> data;
            data = dbContext.LargeFundMoneyTransferFormData.Where(x => x.AgentStaffId == AgentStaffId);
            if (!string.IsNullOrEmpty(Year))
            {
                data = data.Where(x => x.SubmittedDate.Year == int.Parse(Year));
            }
            if ((int)month > 0)
            {
                data = data.Where(x => x.SubmittedDate.Month == (int)month);
            }

            var agentStaffLoginInfo = _agentCommonServices.GetAgentStaffLoginInfo().Where(x => x.AgentStaffId == AgentStaffId).FirstOrDefault();
            var result = (from c in data.ToList()
                          select new LargeFundMoneyTransferListVM()
                          {
                              AgentAccountNo = agentStaffLoginInfo.AgentStaff.AgentMFSCode,
                              AgentName = agentStaffLoginInfo.AgentStaff.FirstName + " " + agentStaffLoginInfo.AgentStaff.MiddleName + " " + agentStaffLoginInfo.AgentStaff.LastName,
                              SenderName = c.SenderFullName,
                              AgentStaffLoginCode = agentStaffLoginInfo.StaffLoginCode,
                              TransactionId = c.Id,
                              Year = c.SubmittedDate.Year.ToString(),
                              Month = Enum.GetName(typeof(Month), c.SubmittedDate.Month),
                              ReceiverName = c.ReceiverName
                          }).ToList();
            return result;


        }

        public List<AgentAMLTrainingRecordGridVM> GetAgentAMLTrainingRecordDetails(string Year , Month month) {
            AgentCommonServices _agentCommonServices = new AgentCommonServices();
            IQueryable<DB.AgentAMLTrainingRecordMaster> data;
            data = dbContext.AgentAMLTrainingRecordMaster.Where(x => x.AgentStaffId == AgentStaffId);
            if (!string.IsNullOrEmpty(Year))
            {
                data = data.Where(x => x.SubmittedDate.Year == int.Parse(Year));
            }
            if ((int)month > 0)
            {
                data = data.Where(x => x.SubmittedDate.Month == (int)month);
            }

            var agentStaffLoginInfo = _agentCommonServices.GetAgentStaffLoginInfo().Where(x => x.AgentStaffId == AgentStaffId).FirstOrDefault();
            var result = (from c in data.ToList()
                          select new AgentAMLTrainingRecordGridVM()
                          {
                              AgentAccountNo = agentStaffLoginInfo.AgentStaff.AgentMFSCode,
                              AgentName = agentStaffLoginInfo.AgentStaff.FirstName + " " + agentStaffLoginInfo.AgentStaff.MiddleName + " " + agentStaffLoginInfo.AgentStaff.LastName,
                              AgentStaffLoginCode = agentStaffLoginInfo.StaffLoginCode,
                              TransactionId = c.Id,
                              Year = c.SubmittedDate.Year.ToString(),
                              Month = Enum.GetName(typeof(Month), c.SubmittedDate.Month),

                          }).ToList();
            return result;
        }

        public List<SourceOfFundDeclarationGridVM> GetSourceOfFundDeclarationDetails(string Year, Month month)
        {
            AgentCommonServices _agentCommonServices = new AgentCommonServices();
            IQueryable<DB.SourceOfFundDeclarationFormData> data;
            data = dbContext.SourceOfFundDeclarationFormData.Where(x => x.AgentStaffId == AgentStaffId);
            if (!string.IsNullOrEmpty(Year))
            {
                data = data.Where(x => x.SubmittedDate.Year == int.Parse(Year));
            }
            if ((int)month > 0)
            {
                data = data.Where(x => x.SubmittedDate.Month == (int)month);
            }

            var agentStaffLoginInfo = _agentCommonServices.GetAgentStaffLoginInfo().Where(x => x.AgentStaffId == AgentStaffId).FirstOrDefault();
            var result = (from c in data.ToList()
                          select new SourceOfFundDeclarationGridVM()
                          {
                              AgentAccountNo = agentStaffLoginInfo.AgentStaff.AgentMFSCode,
                              AgentName = agentStaffLoginInfo.AgentStaff.FirstName + " " + agentStaffLoginInfo.AgentStaff.MiddleName + " " + agentStaffLoginInfo.AgentStaff.LastName,
                              AgentStaffLoginCode = agentStaffLoginInfo.StaffLoginCode,
                              TransactionId = c.Id,
                              Year = c.SubmittedDate.Year.ToString(),
                              Month = Enum.GetName(typeof(Month), c.SubmittedDate.Month),
                              ReceiverName = c.ReceiverName,
                              SenderName = c.SenderFullName,
                          }).ToList();
            return result;
        }
    }
}