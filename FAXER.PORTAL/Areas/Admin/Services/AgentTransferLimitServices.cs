using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class AgentTransferLimitServices
    {
        DB.FAXEREntities db = null;
        public AgentTransferLimitServices()
        {
            db = new DB.FAXEREntities();
        }

        public List<AgentTransferLimit> List()
        {
            var data = db.AgentTransferLimit.ToList();
            return data;
        }

        internal List<ViewModels.AgentTransferLimtViewModel> GetAgentTransferLimit(string country, int Services, string City, int AgentId)
        {
            var AgentTransferLimitList = db.AgentTransferLimit.Where(x => x.LimitType == LimitType.TransferLimit).ToList();


            if (!string.IsNullOrEmpty(country.Trim()))
            {

                AgentTransferLimitList = AgentTransferLimitList.Where(x => x.Country.ToLower().Trim() == country.ToLower().Trim()).ToList();
            }



            if (Services > 0)
            {
                AgentTransferLimitList = AgentTransferLimitList.Where(x => x.TransferMethod == (TransactionTransferMethod)Services).ToList();
            }
            if (AgentId > 0)
            {
                AgentTransferLimitList = AgentTransferLimitList.Where(x => x.AgentId == AgentId).ToList();
            }
            if (!string.IsNullOrEmpty(City.Trim()))
            {
                AgentTransferLimitList = AgentTransferLimitList.Where(x => x.City.ToLower().Trim() == City.ToLower().Trim()).ToList();
            }
            CommonServices _commonServices = new CommonServices();



            var data = (from c in AgentTransferLimitList
                        select new AgentTransferLimtViewModel()
                        {
                            Id = c.Id,
                            Country = Common.Common.GetCountryName(c.Country),
                            TransferMethodName = c.TransferMethod.ToString(),
                            AgentName = c.AgentId != 0 ? getAgentName(c.AgentId) : "All",
                            Amount = c.Amount,
                            FrequencyName = c.Frequency.ToString(),
                            CountryCurrencySymbol = _commonServices.getCurrencySymbol(c.Country),
                        }).ToList();

            return data;

        }



        public bool UpdateAgentTransferLimit(AgentTransferLimtViewModel vm)
        {

            AgentTransferLimit AgentTransferLimit = new AgentTransferLimit()
            {
                Id = vm.Id ?? 0,
                Country = vm.Country,

                AgentId = vm.AgentId ?? 0,
                Amount = vm.Amount,

                TransferMethod = vm.TransferMethod,
                LimitType = LimitType.TransferLimit,
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
                LimitType = LimitType.TransferLimit,
                TransferMethod = vm.TransferMethod,

                CreatedBy = AdminSession.StaffId,
                CreatedDate = DateTime.Now,


            };
            db.AgentTransferLimitHistory.Add(agentTransferLimitHistory);
            db.SaveChanges();
            return true;

        }
        public bool AddAgentTransferLimit(AgentTransferLimtViewModel vm)
        {



            AgentTransferLimit AgentTransferLimit = new AgentTransferLimit()
            {
                
                Country = vm.Country,

                AgentId = vm.AgentId ?? 0,
                Amount = vm.Amount,

                TransferMethod = vm.TransferMethod,
                LimitType = LimitType.TransferLimit,
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
                LimitType = LimitType.TransferLimit,
                TransferMethod = vm.TransferMethod,

                CreatedBy = AdminSession.StaffId,
                CreatedDate = DateTime.Now,


            };
            AddAgentTransferLimitHistory(agentTransferLimitHistory);
            db.AgentTransferLimit.Add(AgentTransferLimit);
            db.SaveChanges();

            return true;
        }
        public bool AddAgentTransferLimitHistory(AgentTransferLimitHistory agentTransferLimitHistory)
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
        internal List<AgentTransferLimtViewModel> GetAgentTransferLimitHistory(string Country = "", int Services = 0, string city = "", int AgentId = 0)
        {
            CommonServices _commonServices = new CommonServices();



            var AgentTransferLimitListHistory = db.AgentTransferLimitHistory.Where(x => x.LimitType == LimitType.TransferLimit).ToList();


            if (!string.IsNullOrEmpty(Country.Trim()))
            {

                AgentTransferLimitListHistory = AgentTransferLimitListHistory.Where(x => x.Country.ToLower().Trim() == Country.ToLower().Trim()).ToList();
            }



            if (Services > 0)
            {
                AgentTransferLimitListHistory = AgentTransferLimitListHistory.Where(x => x.TransferMethod == (TransactionTransferMethod)Services).ToList();
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
                              TransferMethodName = c.TransferMethod.ToString(),
                              AgentName = c.AgentId != 0 ? getAgentName(c.AgentId) : "All",
                              Amount = c.Amount,
                              FrequencyName = c.Frequency.ToString(),
                              CountryCurrencySymbol = _commonServices.getCurrencySymbol(Country),
                              CreationDate = c.CreatedDate.ToString("dd/MM/yyyy")
                          }).ToList();
            return result;

        }

        public string getAgentName(int id)
        {
            string data = db.AgentInformation.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefault();
            return data;
        }
    }
}