using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class AgentHoldCashLimitServices
    {
        DB.FAXEREntities db = null;
        public AgentHoldCashLimitServices()
        {
            db = new DB.FAXEREntities();
        }

        public List<AgentTransferLimit> List()
        {
            var data = db.AgentTransferLimit.ToList();
            return data;
        }

        internal List<ViewModels.AgentTransferLimtViewModel> GetAgentHoldCashimit(string country, string City, int AgentId, string AccountNo)
        {
            var AgentHoldCashLimitList = db.AgentTransferLimit.Where(x => x.LimitType == LimitType.CashHold).ToList();


            if (!string.IsNullOrEmpty(country.Trim()))
            {

                AgentHoldCashLimitList = AgentHoldCashLimitList.Where(x => x.Country.ToLower().Trim() == country.ToLower().Trim()).ToList();
            }

            if (AgentId > 0)
            {
                AgentHoldCashLimitList = AgentHoldCashLimitList.Where(x => x.AgentId == AgentId).ToList();
            }
            if (!string.IsNullOrEmpty(City.Trim()))
            {
                AgentHoldCashLimitList = AgentHoldCashLimitList.Where(x => x.City.ToLower().Trim() == City.ToLower().Trim()).ToList();
            }
            CommonServices _commonServices = new CommonServices();



            var data = (from c in AgentHoldCashLimitList
                        select new AgentTransferLimtViewModel()
                        {
                            Id = c.Id,
                            Country = Common.Common.GetCountryName(c.Country),
                            AccountNo = c.AccountNo,
                            City = c.City,
                            AgentName = c.AgentId != 0 ? getAgentName(c.AgentId) : "All",
                            Amount = c.Amount,
                            FrequencyName = c.Frequency.ToString(),
                            CountryCurrencySymbol = _commonServices.getCurrencySymbol(c.Country),
                        }).ToList();


            if (!string.IsNullOrEmpty(AccountNo))
            {
                AccountNo = AccountNo.Trim();
                data = data.Where(x => x.AccountNo.ToLower().Contains(AccountNo.ToLower())).ToList();

            }

            return data;
        }

        public string getAgentName(int id)
        {
            string data = db.AgentInformation.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefault();
            return data;
        }


        public bool UpdateAgentHoldCashLimit(AgentTransferLimtViewModel vm)
        {

            AgentTransferLimit AgentTransferLimit = new AgentTransferLimit()
            {
                Id = vm.Id ?? 0,
                Country = vm.Country,

                AgentId = vm.AgentId ?? 0,
                Amount = vm.Amount,
                LimitType = LimitType.CashHold,
                Frequency = vm.Frequency,
                CreatedBy = AdminSession.StaffId,
                CreatedDate = DateTime.Now,
                AccountNo = vm.AccountNo,
                City = vm.City,
            };

            db.Entry<AgentTransferLimit>(AgentTransferLimit).State = System.Data.Entity.EntityState.Modified;

            AgentTransferLimitHistory agentTransferLimitHistory = new AgentTransferLimitHistory()
            {
                Country = vm.Country,

                AgentId = vm.AgentId ?? 0,
                AccountNo = vm.AccountNo,
                Amount = vm.Amount,
                City = vm.City,
                Frequency = vm.Frequency,
                LimitType = LimitType.CashHold,
                TransferMethod = vm.TransferMethod,
                CreatedBy = AdminSession.StaffId,
                CreatedDate = DateTime.Now,


            };
            db.AgentTransferLimitHistory.Add(agentTransferLimitHistory);
            db.SaveChanges();
            return true;

        }
        public bool AddAgentHoldCashLimit(AgentTransferLimtViewModel vm)
        {



            AgentTransferLimit AgentTransferLimit = new AgentTransferLimit()
            {

                Country = vm.Country,

                AgentId = vm.AgentId ?? 0,
                Amount = vm.Amount,
                LimitType = LimitType.CashHold,
                Frequency = vm.Frequency,
                CreatedBy = AdminSession.StaffId,
                CreatedDate = DateTime.Now,
                AccountNo = vm.AccountNo,
                City = vm.City,



            };
            AgentTransferLimitHistory agentTransferLimitHistory = new AgentTransferLimitHistory()
            {
                Country = vm.Country,

                AgentId = vm.AgentId ?? 0,
                AccountNo = vm.AccountNo,
                Amount = vm.Amount,
                City = vm.City,
                Frequency = vm.Frequency,
                LimitType = LimitType.CashHold,
                CreatedBy = AdminSession.StaffId,
                CreatedDate = DateTime.Now,


            };
            AddAgentCashHoldLimittHistory(agentTransferLimitHistory);
            db.AgentTransferLimit.Add(AgentTransferLimit);
            db.SaveChanges();

            return true;
        }
        public bool AddAgentCashHoldLimittHistory(AgentTransferLimitHistory agentTransferLimitHistory)
        {
            db.AgentTransferLimitHistory.Add(agentTransferLimitHistory);
            db.SaveChanges();
            return true;
        }

        internal bool Delete(int id)
        {
            var data = db.AgentTransferLimit.Where(x => x.Id == id).FirstOrDefault();
            db.AgentTransferLimit.Remove(data);
            db.SaveChanges();
            return true;
        }
        internal List<AgentTransferLimtViewModel> GetAgentHoldCashLimitHistory(string Country = "", string city = "", int AgentId = 0, string AccountNo = "")
        {
            CommonServices _commonServices = new CommonServices();



            var AgentTransferLimitListHistory = db.AgentTransferLimitHistory.Where(x => x.LimitType == LimitType.CashHold).ToList();


            if (!string.IsNullOrEmpty(Country.Trim()))
            {

                AgentTransferLimitListHistory = AgentTransferLimitListHistory.Where(x => x.Country.ToLower().Trim() == Country.ToLower().Trim()).ToList();
            }

            if (AgentId > 0)
            {
                AgentTransferLimitListHistory = AgentTransferLimitListHistory.Where(x => x.AgentId == AgentId).ToList();
            }
            if (!string.IsNullOrEmpty(city.Trim()))
            {
                AgentTransferLimitListHistory = AgentTransferLimitListHistory.Where(x => x.City.ToLower().Trim() == city.ToLower().Trim()).ToList();
            }
            var result = (from c in AgentTransferLimitListHistory.OrderByDescending(x => x.CreatedDate).ToList()
                          select new AgentTransferLimtViewModel()
                          {
                              Id = c.Id,
                              Country = Common.Common.GetCountryName(c.Country),
                              AccountNo = c.AccountNo,
                              AgentName = c.AgentId != 0 ? getAgentName(c.AgentId) : "All",
                              Amount = c.Amount,
                              FrequencyName = c.Frequency.ToString(),
                              CountryCurrencySymbol = _commonServices.getCurrencySymbol(c.Country),
                              CreationDate = c.CreatedDate.ToString("dd/MM/yyyy"),
                              City = c.City,
                          }).ToList();
            if (!string.IsNullOrEmpty(AccountNo))
            {
                AccountNo = AccountNo.Trim();
                result = result.Where(x => x.AccountNo.ToLower().Contains(AccountNo.ToLower())).ToList();

            }

            return result;

        }



    }
}