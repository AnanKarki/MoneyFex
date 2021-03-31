using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewAgentCommissionPaymentServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        Services.CommonServices CommonService = new Services.CommonServices();

        public List<ViewAgentCommissionPaymentViewModel> getList(string CountryCode = "", string City = "")
        {
            var data = dbContext.AgentCommissionPayment.Where(x => x.IsDeleted == false).ToList();

            if (!string.IsNullOrEmpty(CountryCode) && string.IsNullOrEmpty(City))
            {
                data = dbContext.AgentCommissionPayment.Where(x => x.IsDeleted == false && x.Agent.CountryCode == CountryCode).ToList();
            }
            else if (string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = dbContext.AgentCommissionPayment.Where(x => x.IsDeleted == false && x.Agent.City.ToLower() == City.ToLower()).ToList();
            }
            else if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = dbContext.AgentCommissionPayment.Where(x => x.IsDeleted == false && x.Agent.City.ToLower() == City.ToLower() && x.Agent.CountryCode == CountryCode).ToList();
            }
            else
            {
                data = dbContext.AgentCommissionPayment.Where(x => x.IsDeleted == false).ToList();
            }

            var result = (from c in data.OrderByDescending(x=>x.TransactionDateTime)
                          select new ViewAgentCommissionPaymentViewModel()
                          {
                              Id = c.Id,
                              Year = c.Year,
                              Month = Enum.GetName(typeof(Month), c.Month),
                              Country = CommonService.getCountryNameFromCode(c.Agent.CountryCode),
                              City = c.Agent.City,
                              AgentId = c.AgentId,
                              AgentName = c.Agent.Name,
                              AgentMFSCode = c.Agent.AccountNo,
                              Status = Enum.GetName(typeof(AgentStatus), c.Agent.AgentStatus),
                              TotalSentPayment = c.TotalSentPayment,
                              SentCommissionRate = c.SendingCommissionRate,
                              TotalSentCommission = c.TotalSentCommission,
                              TotalReceivedPayment = c.TotalReceivedPayment,
                              ReceivedCommissionRate = c.ReceivingCommissionRate,
                              TotalReceivedCommission = c.TotalReceivedCommission,
                              TotalCommission = c.TotalCommission,
                              NameOfVerifier = CommonService.getStaffName(c.VerifiedBy),
                              Date = c.TransactionDateTime.ToFormatedString(),
                              Time = c.TransactionDateTime.ToString("HH:mm"),
                              AgentCurrencySymbol = CommonService.getCurrencySymbol(c.Agent.CountryCode)
                          }).ToList();

            return result;
        }

        public bool deletePayment(int id)
        {
            var data = dbContext.AgentCommissionPayment.Find(id);
            if (data != null)
            {
                data.IsDeleted = true;
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;

            }
            return false;
        }
        public DB.AgentCommissionPayment GetCommissionPaymentDetials(int Id)
        {

            var result = dbContext.AgentCommissionPayment.Where(x => x.Id == Id).FirstOrDefault();
            return result;

        }

        public DB.AgentInformation GetAgentInformation(int AgentId)
        {
            var result = dbContext.AgentInformation.Where(x => x.Id == AgentId).FirstOrDefault();
            return result;
        }

        public DB.StaffInformation GetStaffInformation(int StaffId) {

            var result = dbContext.StaffInformation.Where(x => x.Id == StaffId).FirstOrDefault();
            return result;

        }

        public DB.StaffLogin GetStaffLoginInfo(int StaffId) {

            var result = dbContext.StaffLogin.Where(x => x.StaffId == StaffId).FirstOrDefault();
            return result;

        }
        
    }
}