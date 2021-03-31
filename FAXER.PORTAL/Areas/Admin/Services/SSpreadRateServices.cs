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
    public class SSpreadRateServices
    {
        DB.FAXEREntities dbContext = null;

        public SSpreadRateServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<DropDownViewModel> GetCountries(string CountryCode = "")
        {
            if (!string.IsNullOrEmpty(CountryCode))
            {
                var result = (from c in dbContext.Country.Where(x => x.CountryCode == CountryCode)
                              select new DropDownViewModel()
                              {

                                  Code = c.CountryCode,
                                  Name = c.CountryName
                              }).GroupBy(x => x.Name).Select(x => x.FirstOrDefault()).ToList();
                return result;
            }
            else
            {
                var result = (from c in dbContext.Country
                              select new DropDownViewModel()
                              {

                                  Code = c.CountryCode,
                                  Name = c.CountryName
                              }).GroupBy(x => x.Name).Select(x => x.FirstOrDefault()).ToList();
                return result;
            }

        }

        public List<AgentsListDropDown> GetAgent()
        {

            var agents = (from c in dbContext.AgentInformation
                          select new AgentsListDropDown()
                          {
                              AgentId = c.Id,
                              AgentName = c.Name
                          }).ToList();
            return agents;
        }

        public ServiceResult<List<SpreadRateViewModel>> GetSpreadRate(int StaffId = 0, string SendingCountry = "", string ReceivingCountry = "", int TransferType = 0, int Agent = 0)
        {


            //List<CustData> myList = GetCustData();

            //var query = myList
            //    .GroupBy(c => c.CustId)
            //    .Select(g => new
            //    {
            //        CustId = g.Key,
            //        Jan = g.Where(c => c.OrderDate.Month == 1).Sum(c => c.Qty),
            //        Feb = g.Where(c => c.OrderDate.Month == 2).Sum(c => c.Qty),
            //        March = g.Where(c => c.OrderDate.Month == 3).Sum(c => c.Qty)
            //    });
            //var result = (from c in dbContext.SpreadRate.Where(x => x.CreatedById == StaffId).ToList()
            //              join d in dbContext.AgentInformation on c.AgentId equals d.Id
            //              select new SpreadRateViewModel()
            //              {
            //                  Id = c.Id,
            //                  AgentName = d.Name,
            //                  SendingCountry = Common.Common.GetCountryName(c.SendingCountry),
            //                  ReceivingCountry = Common.Common.GetCountryName(c.ReceivingCountry),

            //              }).ToList();
            var data = dbContext.SpreadRate.ToList();

            if (!string.IsNullOrEmpty(SendingCountry))
            {
                data = data.Where(x => x.SendingCountry == SendingCountry).ToList();
            }
            if (!string.IsNullOrEmpty(ReceivingCountry))
            {
                data = data.Where(x => x.ReceivingCountry == ReceivingCountry).ToList();
            }
            if (TransferType != 0)
            {
                data = data.Where(x => x.TransferType == (TransactionTransferType)TransferType).ToList();
            }
            if (Agent != 0)
            {

                data = data.Where(x => x.AgentId == Agent).ToList();
            }


            var result = data.GroupBy(x => new
            {
                x.SendingCountry,
                x.ReceivingCountry,
                x.AgentId,
                x.TransferType
            }).Select(d => new SpreadRateViewModel()
            {
                Id = d.FirstOrDefault().Id,
                AgentId = d.FirstOrDefault().AgentId,
                SendingCountry = d.FirstOrDefault().SendingCountry,
                ReceivingCountry = d.FirstOrDefault().ReceivingCountry,
                SendingCountryFlag = d.FirstOrDefault().SendingCountry.ToLower(),
                ReceivingCountryFlag = d.FirstOrDefault().ReceivingCountry.ToLower(),
                TransferType = d.FirstOrDefault().TransferType,
                BankDeposit = d.Where(x => x.TransferMethod == TransactionTransferMethod.BankDeposit && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
                KiiPayWallet = d.Where(x => x.TransferMethod == TransactionTransferMethod.KiiPayWallet && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
                OtherWallet = d.Where(x => x.TransferMethod == TransactionTransferMethod.OtherWallet && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
                CashPickUp = d.Where(x => x.TransferMethod == TransactionTransferMethod.CashPickUp && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
                ServicePayment = d.Where(x => x.TransferMethod == TransactionTransferMethod.ServicePayment && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
                BillPayment = d.Where(x => x.TransferMethod == TransactionTransferMethod.BillPayment && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
            }).ToList();
            foreach (var item in result)
            {
                item.AgentName = dbContext.AgentInformation.Where(x => x.Id == item.AgentId).Select(x => x.Name).FirstOrDefault();
                item.SendingCountry = Common.Common.GetCountryName(item.SendingCountry);
                item.ReceivingCountry = Common.Common.GetCountryName(item.ReceivingCountry);
            }


            return new ServiceResult<List<SpreadRateViewModel>>()
            {
                Data = result,
                Status = ResultStatus.OK
            };

        }

        public ServiceResult<List<SpreadRateHistoryViewModel>> GetSpreadRateHistory(string SendingCountry = "", string ReceivingCountry = "", int TransferType = 0, int Agent = 0, int Year = 0, int Month = 0, int Day = 0)

        {

            var data = dbContext.SpreadRateHistory.ToList();
            if (!string.IsNullOrEmpty(SendingCountry))
            {
                data = data.Where(x => x.SendingCountry == SendingCountry).ToList();
            }
            if (!string.IsNullOrEmpty(ReceivingCountry))
            {
                data = data.Where(x => x.ReceivingCountry == ReceivingCountry).ToList();
            }
            if (TransferType != 0)
            {
                data = data.Where(x => x.TransferType == (TransactionTransferType)TransferType).ToList();
            }
            if (Agent != 0)
            {
                data = data.Where(x => x.AgentId == Agent).ToList();
            }
            if (Year != 0)
            {
                data = data.Where(x => x.CreatedDate.Year == Year).ToList();
            }
            if (Month != 0)
            {
                data = data.Where(x => x.CreatedDate.Month == Month).ToList();
            }
            if (Day != 0)
            {
                data = data.Where(x => x.CreatedDate.Day == Day).ToList();
            }


            var result = data.GroupBy(x => new
            {
                x.SendingCountry,
                x.ReceivingCountry,
                x.AgentId,
                x.TransferType
            }).Select(d => new SpreadRateHistoryViewModel()
            {
                Id = d.FirstOrDefault().Id,
                AgentId = d.FirstOrDefault().AgentId,
                SendingCountry = d.FirstOrDefault().SendingCountry,
                ReceivingCountry = d.FirstOrDefault().ReceivingCountry,
                SendingCountryFlag = d.FirstOrDefault().SendingCountry.ToLower(),
                ReceivingCountryFlag = d.FirstOrDefault().ReceivingCountry.ToLower(),
                TransferType = d.FirstOrDefault().TransferType,
                BankDeposit = d.Where(x => x.TransferMethod == TransactionTransferMethod.BankDeposit && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
                KiiPayWallet = d.Where(x => x.TransferMethod == TransactionTransferMethod.KiiPayWallet && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
                OtherWallet = d.Where(x => x.TransferMethod == TransactionTransferMethod.OtherWallet && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
                CashPickUp = d.Where(x => x.TransferMethod == TransactionTransferMethod.CashPickUp && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
                ServicePayment = d.Where(x => x.TransferMethod == TransactionTransferMethod.ServicePayment && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
                BillPayment = d.Where(x => x.TransferMethod == TransactionTransferMethod.BillPayment && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
                CreatedDate = d.FirstOrDefault().CreatedDate.ToString("dd/MM/yyyy"),
            }).ToList();
            foreach (var item in result)
            {
                item.AgentName = dbContext.AgentInformation.Where(x => x.Id == item.AgentId).Select(x => x.Name).FirstOrDefault();
                item.SendingCountry = Common.Common.GetCountryName(item.SendingCountry);
                item.ReceivingCountry = Common.Common.GetCountryName(item.ReceivingCountry);
            }


            return new ServiceResult<List<SpreadRateHistoryViewModel>>()
            {
                Data = result,
                Status = ResultStatus.OK
            };
        }

        public decimal GetRate(string sendingCountry = "", string receivingCounrty = "", int transferType = 0, int method = 0, int agent = 0)
        {
            var Rate = dbContext.SpreadRate.Where(x => x.SendingCountry == sendingCountry && x.ReceivingCountry == receivingCounrty && x.TransferType == (TransactionTransferType)transferType
            && x.TransferMethod == (TransactionTransferMethod)method && x.AgentId == agent).Select(x => x.Rate).FirstOrDefault();
            return Rate;
        }
        public SpreadRate GetRateDetials(string sendingCountry = "", string receivingCounrty = "", int transferType = 0, int method = 0, int agent = 0)
        {

            var data = dbContext.SpreadRate.Where(x => x.SendingCountry == sendingCountry && x.ReceivingCountry == receivingCounrty && x.TransferType == (TransactionTransferType)transferType
            && x.TransferMethod == (TransactionTransferMethod)method && x.AgentId == agent).FirstOrDefault();
            return data;
        }
        public SpreadRateViewModel AddSpreadRate(SpreadRateViewModel model)
        {
            int agentId = 0;
            if (model.AgentId != null)
            {
                agentId = (int)model.AgentId;
            }

            var spreadRate = GetRateDetials(model.SendingCountry, model.ReceivingCountry, (int)model.TransferType, (int)model.TransferMethod, agentId);

            if (spreadRate != null)
            {

                spreadRate.Rate = model.Rate;
                dbContext.Entry<SpreadRate>(spreadRate).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
            else
            {
                int StaffId = Common.StaffSession.LoggedStaff.StaffId;
                var vm = new SpreadRate()
                {
                    CreatedDate = DateTime.Now,
                    TransferType = model.TransferType,
                    TransferMethod = model.TransferMethod,
                    ReceivingCountry = model.ReceivingCountry,
                    SendingCountry = model.SendingCountry,
                    Rate = model.Rate,
                    CreatedById = StaffId,
                    AgentId = model.AgentId
                };
                var data = dbContext.SpreadRate.Add(vm);
                dbContext.SaveChanges();
            }
            return model;
        }


        public SpreadRateHistory AddSpreadRateHistory(SpreadRateViewModel model)
        {
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;
            var dateAndTime = DateTime.Now;

            var vm = new SpreadRateHistory()
            {
                CreatedDate = dateAndTime.Date,
                TransferType = model.TransferType,
                TransferMethod = model.TransferMethod,
                ReceivingCountry = model.ReceivingCountry,
                SendingCountry = model.SendingCountry,
                Rate = model.Rate,
                CreatedById = StaffId,
                AgentId = model.AgentId
            };
            var SpreadRatehistory = dbContext.SpreadRateHistory.Add(vm);
            dbContext.SaveChanges();
            return null;
        }

        public SpreadRateViewModel GetSpreadRateById(int id)
        {
            var spreadRate = dbContext.SpreadRate.Where(x => x.Id == id).FirstOrDefault();
            var vm = new SpreadRateViewModel()
            {
                CreatedDate = spreadRate.CreatedDate,
                TransferType = spreadRate.TransferType,
                TransferMethod = spreadRate.TransferMethod,
                ReceivingCountry = spreadRate.ReceivingCountry,
                SendingCountry = spreadRate.SendingCountry,
                Rate = spreadRate.Rate,
                CreatedById = spreadRate.CreatedById,
                AgentId = spreadRate.AgentId
            };
            return vm;
        }

        public ServiceResult<IQueryable<SpreadRate>> List()
        {
            return new ServiceResult<IQueryable<SpreadRate>>()
            {
                Data = dbContext.SpreadRate,
                Status = ResultStatus.OK
            };

        }

        public SpreadRate UpdateSpreadRate(SpreadRateViewModel model)
        {

            var spreadRate = dbContext.SpreadRate.Where(x => x.Id == model.Id).FirstOrDefault();

            spreadRate.ReceivingCountry = model.ReceivingCountry;
            spreadRate.SendingCountry = model.SendingCountry;
            spreadRate.CreatedById = spreadRate.CreatedById;
            spreadRate.CreatedDate = spreadRate.CreatedDate;
            spreadRate.AgentId = model.AgentId;
            spreadRate.Rate = model.Rate;
            spreadRate.TransferMethod = model.TransferMethod;
            spreadRate.TransferType = model.TransferType;

            dbContext.Entry<SpreadRate>(spreadRate).State = EntityState.Modified;
            dbContext.SaveChanges();
            return spreadRate;
        }


        public ServiceResult<int> Remove(SpreadRate model)
        {
            dbContext.SpreadRate.Remove(model);
            dbContext.SaveChanges();
            return new ServiceResult<int>()
            {
                Data = 1,
                Message = "Remove",
                Status = ResultStatus.OK
            };
        }
    }
}