using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.DB;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewAgentCashWithdrawalServices
    {
        FAXEREntities dbcontext = null;
        CommonServices commonServices = null;
        DailyTransactionStatementServices dailyTransactionStatementServices = null;
        public ViewAgentCashWithdrawalServices()
        {
            dbcontext = new FAXEREntities();
            commonServices = new CommonServices();
            dailyTransactionStatementServices = new DailyTransactionStatementServices();
        }

        public ViewAgentCashWithdrawalViewModel getCashWithdrawalList(int pageNumber,int pageSize)
        {
            ViewAgentCashWithdrawalViewModel vm = new ViewAgentCashWithdrawalViewModel();
            var list1 = (from c in dbcontext.CashWithdrawalByAgent.ToList()
                         join e in dbcontext.AgentStaffInformation on c.AgentId equals e.AgentId
                         select new ViewAgentCashWithdrawalList()
                         {
                             Id = c.Id,
                             AgentId=c.AgentId,
                             Name = c.Agent.Name,
                             Country = commonServices.getCountryNameFromCode(c.Agent.CountryCode),
                             City = c.Agent.City,
                             AccountNo = c.Agent.AccountNo,
                             WithdrawalType = "Agent",
                             NameOfStaffAgent = c.AgentStaffName,
                             StaffCode = commonServices.GetAgentStaffMFSCode(c.AgentStaffId),
                             Amount = c.WithdrawalAmount,
                             Date = c.TransactionDateTime.ToString("yyyy/MM/dd"),
                             Time = c.TransactionDateTime.ToString("HH:mm"),
                             Status = c.Status,
                             AccountBalance = dailyTransactionStatementServices.getAgentAccountBalance(c.AgentId, e.Id),
                             ReceiptUrl = "",
                             IsWithdrawalByAgent = 1,
                             TransactionDate = c.TransactionDateTime,
                             CurrencySymbol = commonServices.getCurrencySymbol(c.Agent.CountryCode),

                         }).ToList();

            var list2 = (from c in dbcontext.CashWithdrawalByStaff.ToList()
                         join e in dbcontext.AgentStaffInformation on c.AgentId equals e.AgentId
                         select new ViewAgentCashWithdrawalList()
                         {
                             Id = c.Id,
                             AgentId=c.AgentId,
                             Name = c.Agent.Name,
                             Country = commonServices.getCountryNameFromCode(c.Agent.CountryCode),
                             City = c.Agent.City,
                             AccountNo = c.Agent.AccountNo,
                             WithdrawalType = "MoneyFex Staff",
                             NameOfStaffAgent = c.Staff.FirstName + " " + c.Staff.MiddleName + " " + c.Staff.LastName,
                             StaffCode = c.Staff.StaffMFSCode,
                             Amount = c.WithdrawalAmount,
                             Date = c.TransactionDateTime.ToString("yyyy/MM/dd"),
                             Time = c.TransactionDateTime.ToString("HH:mm"),
                             Status = c.Status,
                             AccountBalance = dailyTransactionStatementServices.getAgentAccountBalance(c.AgentId, e.Id),
                             ReceiptUrl = "",
                             IsWithdrawalByAgent = 0,
                             TransactionDate = c.TransactionDateTime,
                             CurrencySymbol = commonServices.getCurrencySymbol(c.Staff.Country),
                         }).ToList();

            vm.AgentCashWithdrawalIPagedList = list1.Concat(list2).OrderBy(x => x.TransactionDate).ToPagedList(pageNumber,pageSize);

            vm.StaffDetails = new StaffMoreDetailsViewModel()
            {
                Id = 0,
                IDType = "",
                IDNo = "",
                ExpiryDate = "",
                IssuingCountry = ""
            };
            return vm;
        }

        public MoreWithdrawalDetails getCashMoreDetails(int agentId)
        {

            var data = dbcontext.AgentCashWithdrawalCode.Where(x => x.AgentId == agentId && x.Status == AgentWithdrawalCodeStatus.NoUse).FirstOrDefault();
            MoreWithdrawalDetails vm = new MoreWithdrawalDetails();
            if (data != null)
            {
                vm = new MoreWithdrawalDetails()
                {
                    
                    GeneratedStaffName = commonServices.getStaffName(data.CodeGeneratorId),
                    WithdrawalCode = data.WithdrawalCode,
                    GeneratedDate = data.GeneratedDate.ToString("MMM-dd-yyyy")
                };
                return vm;
            }
            else
            {
                return vm;
            }

        }
        public bool confirmWithdrawal(int id, int isWithdrawalByAgent)
        {
            if (id == 0)
            {
                return false;
            }
            if (isWithdrawalByAgent == 1)
            {
                var data = dbcontext.CashWithdrawalByAgent.Where(x => x.Id == id).FirstOrDefault();
                if (data != null)
                {


                    data.Status = Agent.Models.WithdrawalStatus.Confirmed;
                    data.ConfirmedBy = Common.StaffSession.LoggedStaff.StaffId;
                    data.ConfirmedDateTime = DateTime.Now;
                    dbcontext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    dbcontext.SaveChanges();
                    return true;
                }
            }
            else
            {
                var data = dbcontext.CashWithdrawalByStaff.Where(x => x.Id == id).FirstOrDefault();
                if (data != null)
                {
                    data.Status = Agent.Models.WithdrawalStatus.Confirmed;
                    data.ConfirmedBy = Common.StaffSession.LoggedStaff.StaffId;
                    data.ConfirmedDateTime = DateTime.Now;
                    dbcontext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    dbcontext.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public StaffMoreDetailsViewModel getMoreDetails(int id, int isWithdrawalByAgent)
        {
            if (id != 0)
            {
                if (isWithdrawalByAgent == 1)
                {
                    var data = dbcontext.CashWithdrawalByAgent.Where(x => x.Id == id).FirstOrDefault();
                    if (data != null)
                    {
                        int agentStaffId = data.AgentStaffId;
                        var result = (from c in dbcontext.AgentStaffInformation.Where(x => x.Id == agentStaffId).ToList()
                                      select new StaffMoreDetailsViewModel()
                                      {
                                          Id = c.Id,
                                          IDType = c.IdCardType,
                                          IDNo = c.IdCardNumber,
                                          ExpiryDate = c.IdCardExpiryDate.ToString("yyyy/MM/dd"),
                                          IssuingCountry = commonServices.getCountryNameFromCode(c.IssuingCountry)
                                      }).FirstOrDefault();
                        return result;
                    }

                }
                else
                {
                    var data = dbcontext.CashWithdrawalByStaff.Where(x => x.Id == id).FirstOrDefault();
                    if (data != null)
                    {
                        int agentStaffId = data.AgentStaffId;
                        var result = (from c in dbcontext.AgentStaffInformation.Where(x => x.Id == agentStaffId).ToList()
                                      select new StaffMoreDetailsViewModel()
                                      {
                                          Id = c.Id,
                                          IDType = c.IdCardType,
                                          IDNo = c.IdCardNumber,
                                          ExpiryDate = c.IdCardExpiryDate.ToString("yyyy/MM/dd"),
                                          IssuingCountry = commonServices.getCountryNameFromCode(c.IssuingCountry)

                                      }).FirstOrDefault();
                        return result;
                    }
                }
            }
            return null;
        }
    }
}