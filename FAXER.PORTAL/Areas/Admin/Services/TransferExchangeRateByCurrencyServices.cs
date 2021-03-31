using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class TransferExchangeRateByCurrencyServices
    {
        DB.FAXEREntities dbContext = null;
        CommonServices _commonServices = new CommonServices();
        public TransferExchangeRateByCurrencyServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<TransferExchangeRateByCurrency> List()
        {
            var data = dbContext.TransferExchangeRateByCurrency.ToList();
            return data;
        }
        public TransferExchangeRateByCurrencyViewModel GetRate(string sendingCurrency = "", string receivingCurrency = "",
            string sendingCountry = "", string receivingCountry = "", int transferType = 0, int method = 0, int agent = 0, string range = "")
        {
            decimal fromRange = 0m;
            decimal toRange = 0m;

            if (!string.IsNullOrEmpty(range))
            {
                var splittedRange = range.Split('-');
                fromRange = splittedRange[0].ToDecimal();
                toRange = splittedRange[1].ToDecimal();
            }

            var exchangerates = dbContext.TransferExchangeRateByCurrency.Where(x => x.SendingCurrency == sendingCurrency &&
              x.ReceivingCurrency == receivingCurrency &&
              x.SendingCountry == sendingCountry &&
              x.ReceivingCountry == receivingCountry &&
              x.TransferType == (TransactionTransferType)transferType &&
               x.TransferMethod == (TransactionTransferMethod)method && x.FromRange == fromRange && x.ToRange == toRange);

            var exchangeRate = exchangerates.FirstOrDefault();
            if (transferType == 2 || transferType == 4)
            {
                exchangeRate = exchangerates.Where(x => x.AgentId == agent).FirstOrDefault();
            }
            if (exchangeRate != null)
            {
                var exchageRateVm = new TransferExchangeRateByCurrencyViewModel()
                {
                    Id = exchangeRate.Id,
                    SendingCurrency = exchangeRate.SendingCurrency,
                    ReceivingCurrency= exchangeRate.ReceivingCurrency,
                    AgentId = exchangeRate.AgentId,
                    Rate = exchangeRate.Rate,
                    Range = Common.Common.GetRangeName(exchangeRate.FromRange + "-" + exchangeRate.ToRange),
                    TransferMethod = exchangeRate.TransferMethod,
                    TransferType = exchangeRate.TransferType,
                    SendingCountry = exchangeRate.SendingCountry,
                    ReceivingCountry = exchangeRate.ReceivingCountry,
                };
                return exchageRateVm;
            }
            else
            {
                return null;
            }

        }
        public List<TransferExchangeRateByCurrencyViewModel> GetExchangeRateList(string sendingCurrency, string receivingCurrecny, int transferType, int agent, int transferMethod)
        {
            IQueryable<TransferExchangeRateByCurrency> data = dbContext.TransferExchangeRateByCurrency;

            if (!string.IsNullOrEmpty(sendingCurrency))
            {
                data = data.Where(x => x.SendingCurrency == sendingCurrency);
            }
            if (!string.IsNullOrEmpty(receivingCurrecny))
            {

                data = data.Where(x => x.ReceivingCurrency == receivingCurrecny);
            }
            if (transferType != 0)
            {
                data = data.Where(x => x.TransferType == (TransactionTransferType)transferType);
            }
            if (transferMethod != 0)
            {
                data = data.Where(x => x.TransferMethod == (TransactionTransferMethod)transferMethod);
            }
            if (agent != 0)
            {
                data = data.Where(x => x.AgentId == agent);
            }
            var result = (from c in data.ToList()
                          //join a in dbContext.AgentInformation on c.AgentId equals a.Id into Agent
                          //from a in Agent.DefaultIfEmpty()
                          select new TransferExchangeRateByCurrencyViewModel()
                          {
                              Id = c.Id,
                              Range = Common.Common.GetRangeName(c.FromRange + "-" + c.ToRange),
                              Rate = Math.Round(c.Rate, 2),
                              TransferMethodName = Enum.GetName(typeof(TransactionTransferMethod), c.TransferMethod),
                              AgentName = "",
                              SendingCurrency = c.SendingCurrency,
                              ReceivingCurrency = c.ReceivingCurrency,
                              AgentId = c.AgentId,
                              SendingCountry = c.SendingCountry,
                              ReceivingCountry = c.ReceivingCountry,
                              SendingCountryFlag = string.IsNullOrEmpty(c.SendingCountry) == true   ? "" :  c.SendingCountry.ToLower(),
                              ReceivingCountryFlag = string.IsNullOrEmpty(c.ReceivingCountry) == true ? "" : c.ReceivingCountry.ToLower(),
                          }).ToList();
            return result;
        }

        public TransferExchangeRateByCurrencyViewModel GetExchangeRate(int id)
        {
            var data = List().Where(x => x.Id == id).ToList();
            var result = (from c in data
                          select new TransferExchangeRateByCurrencyViewModel()
                          {
                              Id = c.Id,
                              AgentId = c.AgentId,
                              Range = c.Range,
                              Rate = c.Rate,
                              ReceivingCurrency = c.ReceivingCurrency,
                              SendingCurrency = c.SendingCurrency,
                              TransferMethod = c.TransferMethod,
                              TransferType = c.TransferType,
                              ReceivingCountry = c.ReceivingCountry,
                              SendingCountry = c.SendingCountry
                          }).FirstOrDefault();
            return result;
        }




        public void UpdateRate(TransferExchangeRateByCurrencyViewModel vm)
        {
            int staffId = Common.StaffSession.LoggedStaff.StaffId;
            GetFromAndToRange(vm);

            var data = List().Where(x => x.Id == vm.Id).FirstOrDefault();
            data.FromRange = vm.FromRange;
            data.Range = vm.Range;
            data.Rate = vm.Rate;
            data.SendingCountry = vm.SendingCountry;
            data.ReceivingCountry = vm.ReceivingCountry;
            data.ReceivingCurrency = vm.ReceivingCurrency;
            data.SendingCurrency = vm.SendingCurrency;
            data.ToRange = vm.ToRange;
            data.TransferMethod = vm.TransferMethod;
            data.TransferType = vm.TransferType;
            data.CreatedBy = staffId;
            data.AgentId = vm.AgentId ?? 0;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

            var ListOfExchangeRate = dbContext.ExchangeRate.Where(x => x.TransferFeeByCurrencyId == data.Id).ToList();
            dbContext.ExchangeRate.RemoveRange(ListOfExchangeRate);
            dbContext.SaveChanges();

            AddExchangeRateAndExchangeRateHistory(vm);

        }

        public void AddRate(TransferExchangeRateByCurrencyViewModel vm)
        {
            int staffId = Common.StaffSession.LoggedStaff.StaffId;
            GetFromAndToRange(vm);
            TransferExchangeRateByCurrency model = new TransferExchangeRateByCurrency()
            {
                CreatedBy = staffId,
                AgentId = vm.AgentId ?? 0,
                CreatedDate = DateTime.Now,
                Range = vm.Range,
                Rate = vm.Rate,
                ReceivingCurrency = vm.ReceivingCurrency,
                SendingCurrency = vm.SendingCurrency,
                TransferMethod = vm.TransferMethod,
                TransferType = vm.TransferType,
                ToRange = vm.ToRange,
                FromRange = vm.FromRange,
                SendingCountry = vm.SendingCountry,
                ReceivingCountry = vm.ReceivingCountry
            };
            var obj = dbContext.TransferExchangeRateByCurrency.Add(model);
            dbContext.SaveChanges();
            vm.Id = obj.Id;
            AddExchangeRateAndExchangeRateHistory(vm);
        }
        public void Delete(int id)
        {
            var data = List().Where(x => x.Id == id).FirstOrDefault();
            dbContext.TransferExchangeRateByCurrency.Remove(data);
            dbContext.SaveChanges();

            var ListOfExchangeRate = dbContext.ExchangeRate.Where(x => x.TransferFeeByCurrencyId == id).ToList();
            dbContext.ExchangeRate.RemoveRange(ListOfExchangeRate);
            dbContext.SaveChanges();
        }


        private TransferExchangeRateByCurrencyViewModel GetFromAndToRange(TransferExchangeRateByCurrencyViewModel vm)
        {

            string[] Range = vm.Range.Split('-');
            decimal FromRange = decimal.Parse(Range[0]);
            decimal ToRange = 0;
            if (Range.Length < 2)
            {
                ToRange = int.MaxValue;
            }
            else
            {
                ToRange = decimal.Parse(Range[1]);
            }
            vm.FromRange = FromRange;
            vm.ToRange = ToRange;
            return vm;
        }

        private void AddExchangeRateAndExchangeRateHistory(TransferExchangeRateByCurrencyViewModel vm)
        {
            var sendingCountries = new List<string>();
            if (vm.SendingCountry.ToLower() == "all")
            {
                sendingCountries = Common.Common.GetCountriesByCurrency(vm.SendingCurrency);
            }
            else
            {
                sendingCountries.Add(vm.SendingCountry);
            }

            var ReceivingCountries = new List<string>();
            if (vm.ReceivingCountry.ToLower() == "all")
            {
                ReceivingCountries = Common.Common.GetCountriesByCurrency(vm.ReceivingCurrency);
            }
            else
            {
                sendingCountries.Add(vm.ReceivingCountry);
            }

            int StaffId = Common.StaffSession.LoggedStaff.StaffId;
            foreach (var item in sendingCountries)
            {
                foreach (var receivingCountry in ReceivingCountries)
                {
                    ExchangeRate ExchangeRate = new ExchangeRate()
                    {
                        AgentId = vm.AgentId ?? 0,
                        CreatedDate = DateTime.Now,
                        CountryCode2 = receivingCountry,
                        CountryCode1 = item,
                        TransferMethod = vm.TransferMethod,
                        Rate = vm.Rate,
                        ToRange = vm.FromRange,
                        FromRange = vm.ToRange,
                        CreatedBy = StaffId.ToString(),
                        TransferType = vm.TransferType,
                        TransferFeeByCurrencyId = vm.Id,
                        SendingCurrency = vm.SendingCurrency,
                        RecevingCurrency = vm.ReceivingCurrency
                    };
                    dbContext.ExchangeRate.Add(ExchangeRate);
                    dbContext.SaveChanges();
                    TransferExchangeRateHistory hist = new TransferExchangeRateHistory()
                    {
                        AgentId = vm.AgentId ?? 0,
                        CreatedDate = DateTime.Now,
                        SendingCountry = item,
                        ReceivingCountry = receivingCountry,
                        TransferMethod = vm.TransferMethod,
                        Rate = vm.Rate,
                        ToRange = vm.FromRange,
                        FromRange = vm.ToRange,
                        CreatedBy = StaffId.ToString(),
                        TransferType = vm.TransferType,
                        ReceivingCurrency = vm.ReceivingCurrency,
                        SendingCurrency = vm.SendingCurrency
                    };
                    dbContext.TransferExchangeRateHistory.Add(hist);
                    dbContext.SaveChanges();
                }
            }

        }
    }
}