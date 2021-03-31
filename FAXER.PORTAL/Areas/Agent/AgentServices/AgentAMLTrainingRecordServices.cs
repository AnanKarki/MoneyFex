using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Web;
using AgentAMLTrainingRecordDetails = FAXER.PORTAL.DB.AgentAMLTrainingRecordDetails;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class AgentAMLTrainingRecordServices
    {
        DB.FAXEREntities dbContext = null;
        public AgentAMLTrainingRecordServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public bool CreateAgentAMLTrainingRecord(AgentAMLTrainingRecordVM vm)
        {
            var master = new AgentAMLTrainingRecordMaster()
            {
                AgentId = Common.AgentSession.AgentInformation.Id,
                AgentStaffAccountNo = Common.AgentSession.LoggedUser.PayingAgentAccountNumber,
                AgentStaffNameAndAddress = vm.AgentStaffNameAndAddress,
                AgentStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId,
                SubmittedDate = DateTime.Now
            };

            var masterResult = AddAgentAMLTrainingRecordMaster(master);


            var details = (from c in vm.AgentAMLTrainingRecordDetails
                           select new AgentAMLTrainingRecordDetails()
                           {
                               AgentAMLTrainingRecordMasterId = masterResult.Id,
                               DateOfTraining = c.DateOfTraining,
                               EmployeeInitials = c.EmployeeInitials,
                               EmployeeName = c.EmployeeName,
                               IsInitial = c.IsInitial,
                               IsOnGoing = c.IsOnGoing,
                               TrainingConductedBy = c.TrainingConductedBy,
                           }).ToList();

            var detailsResult = AddAgentAMLTrainingRecordDetails(details);


            return true;


        }

        public string GetAgenStaffAccNo(int AgentId)
        {
            var AgntStaffAcc = dbContext.AgentStaffInformation.Where(x => x.AgentId == AgentId).Select(x => x.AgentMFSCode).FirstOrDefault();
            return AgntStaffAcc;
        }

        public string GetNameAndAddressOfAgent(int AgentId)
        {

            var agentInformation = dbContext.AgentInformation.Where(x => x.Id == AgentId).FirstOrDefault();
            var AgentNameAndAddress = agentInformation.Name + "/" + agentInformation.Address1;
            return AgentNameAndAddress;
        }

        public AgentAMLTrainingRecordMaster AddAgentAMLTrainingRecordMaster(AgentAMLTrainingRecordMaster model)
        {

            dbContext.AgentAMLTrainingRecordMaster.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public bool AddAgentAMLTrainingRecordDetails(List<AgentAMLTrainingRecordDetails> list)
        {

            dbContext.AgentAMLTrainingRecordDetail.AddRange(list);
            dbContext.SaveChanges();
            return true;
        }
        internal AgentAMLTrainingRecordMaster Update(AgentAMLTrainingRecordMaster model)
        {
            dbContext.Entry<AgentAMLTrainingRecordMaster>(model).State = EntityState.Modified;

            dbContext.SaveChanges();
            return model;
        }
        public List<AgentAMLTrainingRecordMaster> List()
        {
            return dbContext.AgentAMLTrainingRecordMaster.ToList();
        }
        public ServiceResult<IQueryable<AgentAMLTrainingRecordDetails>> ListDetails()
        {

            return new ServiceResult<IQueryable<AgentAMLTrainingRecordDetails>>()
            {
                Data = dbContext.AgentAMLTrainingRecordDetail,
                Status = ResultStatus.OK,
            };
        }



    }
}