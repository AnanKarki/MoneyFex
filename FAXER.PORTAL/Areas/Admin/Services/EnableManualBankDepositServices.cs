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
    public class EnableManualBankDepositServices
    {
        DB.FAXEREntities dbContext = null;
        public EnableManualBankDepositServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public bool Add(EnableManualBankDepositViewModel model)
        {
            ManualDepositEnable manualDepositEnable = new ManualDepositEnable()
            {
                IsEnabled = true,
                CreatedDate = DateTime.Now,
                PayingCountry = model.PayingCountry,
                CreatedById = model.CreatedById,
                Agent = model.Agent,
                AgentAccountNo = model.AgentAccountNo,
                MobileNo = model.MobileNo,
                AgentAddress = model.AgentAddress,

            };

            dbContext.ManualDepositEnable.Add(manualDepositEnable);
            dbContext.SaveChanges();
            return true;

        }

        public List<EnableManualBankDepositViewModel> List(string PayingCountry = "", int AgentId = 0)
        {
            var data = dbContext.ManualDepositEnable.ToList();
            if (!string.IsNullOrEmpty(PayingCountry))
            {
                data = data.Where(x => x.PayingCountry == PayingCountry).ToList();
            }
            if (AgentId != 0)
            {
                data = data.Where(x => x.Agent == AgentId.ToString()).ToList();
            }
            CommonServices _commonServices = new CommonServices();
            List<EnableManualBankDepositViewModel> Data = (from c in data
                                                           select new EnableManualBankDepositViewModel()
                                                           {
                                                               Id = c.Id,
                                                               PayingCountry = Common.Common.GetCountryName(c.PayingCountry),
                                                               IsEnabled = c.IsEnabled,
                                                               Agent = _commonServices.getAgentName(c.Agent.ToInt()),
                                                               AgentAccountNo = c.AgentAccountNo,
                                                               MobileNo = c.MobileNo,
                                                               AgentId = c.Agent.ToInt()
                                                           }).ToList();
            return Data;

        }

        public AgentInformation GetAgentInfo(int agentId)
        {
            var agentInfo = dbContext.AgentInformation.Where(x => x.Id == agentId).FirstOrDefault();
            return agentInfo;
        }

        public bool Update(EnableManualBankDepositViewModel model)
        {
            var data = dbContext.ManualDepositEnable.Where(x => x.Id == model.Id).FirstOrDefault();
            data.MobileNo = model.MobileNo;
            data.PayingCountry = model.PayingCountry;
            data.Agent = model.Agent;
            data.AgentAccountNo = model.AgentAccountNo;
            data.AgentAddress = model.AgentAddress;

            dbContext.Entry<ManualDepositEnable>(data).State = EntityState.Modified;
            dbContext.SaveChanges();
            return true;
        }

        public EnableManualBankDepositViewModel GetManualBankDepositData(int Id)
        {
            var data = dbContext.ManualDepositEnable.Where(x => x.Id == Id).ToList();
            EnableManualBankDepositViewModel Data = (from c in data
                                                     select new EnableManualBankDepositViewModel()
                                                     {
                                                         Id = c.Id,
                                                         PayingCountry = c.PayingCountry,
                                                         IsEnabled = c.IsEnabled,
                                                         Agent = c.Agent,
                                                         MobileNo = c.MobileNo,
                                                         AgentAccountNo = c.AgentAccountNo,
                                                         AgentAddress = c.AgentAddress,
                                                         MobileCode = Common.Common.GetCountryPhoneCode(c.PayingCountry)
                                                     }).FirstOrDefault();
            return Data;

        }

        public bool Delete(int id)
        {
            var data = dbContext.ManualDepositEnable.Where(x => x.Id == id).FirstOrDefault();
            dbContext.ManualDepositEnable.Remove(data);
            dbContext.SaveChanges();
            return true;
        }

    }
}