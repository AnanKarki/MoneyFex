using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class AuxAgentExhchangeRateLimitServices
    {
        FAXEREntities dbContext = null;
        public AuxAgentExhchangeRateLimitServices()
        {
            dbContext = new FAXEREntities();
        }

        public IQueryable<AuxAgentExchangeRateLimit> AuxAgentExchangeRateLimitList()
        {
            return dbContext.AuxAgentExchangeRateLimit;
        }
        public IQueryable<ExchangeRate> AuxAgentExchangeRates()
        {
            return dbContext.ExchangeRate.Where(x => x.TransferType == TransactionTransferType.AuxAgent);
        }

        public List<ExchangeRateSettingViewModel> GetAuxAgentExchangeRateLimitList(string SendingCountry = "", string City = "", string Date = "", int Method = 0)
        {
            var auxAgentEchangeRates = AuxAgentExchangeRateLimitList();
            if (!string.IsNullOrEmpty(SendingCountry))
            {
                auxAgentEchangeRates = auxAgentEchangeRates.Where(x => x.SendingCountry == SendingCountry);
            }
            if (Method > 0)
            {
                auxAgentEchangeRates = auxAgentEchangeRates.Where(x => x.TransferMethod == (TransactionTransferMethod)Method);
            }
            if (!string.IsNullOrEmpty(Date))
            {
                string[] DateString = Date.Split('-');
                DateTime FromDate = Convert.ToDateTime(DateString[0]);
                DateTime ToDate = Convert.ToDateTime(DateString[1]);
                auxAgentEchangeRates = auxAgentEchangeRates.Where(x => x.CreatedDate >= FromDate && x.CreatedDate <= ToDate);
            }

            var data = (from c in auxAgentEchangeRates.ToList()
                        join agentInfo in dbContext.AgentInformation on c.AgentId equals agentInfo.Id into joined
                        from agentInfo in joined.DefaultIfEmpty()
                        select new ExchangeRateSettingViewModel
                        {
                            Id = c.Id,
                            AgentId = c.AgentId,
                            DestinationCurrencyCode = c.ReceivingCurrency,
                            DestinationCountryCode = c.ReceivingCountry,
                            DestinationCountryName = Common.Common.GetCountryName(c.ReceivingCountry),
                            SourceCurrencyCode = c.SendingCurrency,
                            SourceCountryCode = c.SendingCountry,
                            SourceCountryName = Common.Common.GetCountryName(c.SendingCountry),
                            TransferMethod = c.TransferMethod,
                            TransferMethodName = Common.Common.GetEnumDescription(c.TransferMethod),
                            Rate = c.ExchangeRate,
                            AgentAccountNO = c.AgentId == 0 ? "" : agentInfo.AccountNo,
                            AgentName = c.AgentId == 0 ? "All" : agentInfo.Name,
                            CreatedDate = c.CreatedDate,
                            ExchangeRate = c.ExchangeRate
                        }).ToList();


            return data;
        }

        internal List<ExchangeRateSettingViewModel> GetAuxAgentExchangeRates(string sendingCountry = "", string city = "", string date = "", int method = 0)
        {
            var auxAgentEchangeRates = AuxAgentExchangeRates();
            var agentInformation = dbContext.AgentInformation.Where(x => x.IsAUXAgent == true);
            if (!string.IsNullOrEmpty(sendingCountry))
            {
                auxAgentEchangeRates = auxAgentEchangeRates.Where(x => x.CountryCode1 == sendingCountry);
            }
            if (method > 0)
            {
                auxAgentEchangeRates = auxAgentEchangeRates.Where(x => x.TransferMethod == (TransactionTransferMethod)method);
            }
            if (!string.IsNullOrEmpty(date))
            {
                string[] DateString = date.Split('-');
                DateTime FromDate = Convert.ToDateTime(DateString[0]);
                DateTime ToDate = Convert.ToDateTime(DateString[1]);
                auxAgentEchangeRates = auxAgentEchangeRates.Where(x => x.CreatedDate >= FromDate && x.CreatedDate <= ToDate);
            }

            var data = (from c in auxAgentEchangeRates.ToList()
                        join agentInfo in agentInformation on c.AgentId equals agentInfo.Id into joined
                        from agentInfo in joined.DefaultIfEmpty()
                        join SendingCountry in dbContext.Country on c.CountryCode1 equals SendingCountry.CountryCode
                        join ReceivingCountry in dbContext.Country on c.CountryCode2 equals ReceivingCountry.CountryCode
                        select new ExchangeRateSettingViewModel
                        {
                            Id = c.Id,
                            AgentId = c.AgentId,
                            DestinationCurrencyCode = ReceivingCountry.Currency,
                            DestinationCountryCode = c.CountryCode2,
                            DestinationCountryName = ReceivingCountry.CountryName,
                            SourceCurrencyCode = SendingCountry.Currency,
                            SourceCountryCode = c.CountryCode1,
                            SourceCountryName = SendingCountry.CountryName,
                            TransferMethod = c.TransferMethod,
                            TransferMethodName = Common.Common.GetEnumDescription(c.TransferMethod),
                            Rate = c.Rate,
                            AgentAccountNO = agentInfo == null ? "" : agentInfo.AccountNo,
                            AgentName = agentInfo == null ? "All" : agentInfo.Name,
                            CreatedDate = c.CreatedDate,
                            ExchangeRate = c.Rate,
                            TransferFeeByCurrencyId = c.TransferFeeByCurrencyId
                        }).ToList();

            return data;

        }
        internal ExchangeRateSettingViewModel GetAuxAgentExchangeRate(int id)
        {
            var auxAgentExchangeRate = AuxAgentExchangeRates().Where(x => x.Id == id).FirstOrDefault();
            string sendingCountry = "";
            string receivingCountry = "";
            string sendingCurrency = "";
            string receivingCurrency = "";
            if (auxAgentExchangeRate.TransferFeeByCurrencyId > 0)
            {
                var EchangeRateByCurrency = AuxAgentExchangeRates().Where(x => x.TransferFeeByCurrencyId == auxAgentExchangeRate.TransferFeeByCurrencyId);
                var TransferExchangeRateByCurrency = dbContext.TransferExchangeRateByCurrency.Where(x => x.Id == auxAgentExchangeRate.TransferFeeByCurrencyId).FirstOrDefault();

                sendingCurrency = TransferExchangeRateByCurrency.SendingCurrency;
                receivingCurrency = TransferExchangeRateByCurrency.ReceivingCurrency;


                List<string> sendingCountries = (from c in Common.Common.GetCountriesByCurrency(sendingCurrency)
                                                 join d in EchangeRateByCurrency on c equals d.CountryCode1
                                                 select c).Distinct().ToList();
                List<string> receivingCountries = (from c in Common.Common.GetCountriesByCurrency(receivingCurrency)
                                                   join d in EchangeRateByCurrency on c equals d.CountryCode2
                                                   select c).Distinct().ToList();

                sendingCountry = sendingCountries.Count() > 1 ? "All" : auxAgentExchangeRate.CountryCode1;
                receivingCountry = receivingCountries.Count() > 1 ? "All" : auxAgentExchangeRate.CountryCode2;
            }
            else
            {
                sendingCountry = auxAgentExchangeRate.CountryCode1;
                receivingCountry = auxAgentExchangeRate.CountryCode2;
                sendingCurrency = Common.Common.GetCountryCurrency(sendingCountry);
                receivingCurrency = Common.Common.GetCountryCurrency(receivingCountry);
            }
            ExchangeRateSettingViewModel vm = new ExchangeRateSettingViewModel()
            {
                Id = auxAgentExchangeRate.Id,
                AgentId = auxAgentExchangeRate.AgentId,
                DestinationCurrencyCode = receivingCurrency,
                DestinationCountryCode = receivingCountry,
                SourceCurrencyCode = sendingCurrency,
                SourceCountryCode = sendingCountry,
                TransferMethod = auxAgentExchangeRate.TransferMethod,
                TransferMethodName = Common.Common.GetEnumDescription(auxAgentExchangeRate.TransferMethod),
                Range = Common.Common.GetRangeName(auxAgentExchangeRate.FromRange + "-" + auxAgentExchangeRate.ToRange),
                Rate = auxAgentExchangeRate.Rate,
                CreatedDate = auxAgentExchangeRate.CreatedDate,
                ExchangeRate = auxAgentExchangeRate.Rate,
                TransferFeeByCurrencyId = auxAgentExchangeRate.TransferFeeByCurrencyId
            };
            return vm;

        }

        internal void AddAuxAgentExchangeRateLimit(ExchangeRateSettingViewModel vm)
        {
            AuxAgentExchangeRateLimit model = new AuxAgentExchangeRateLimit()
            {
                AgentId = vm.AgentId ?? 0,
                ExchangeRate = vm.ExchangeRate,
                ReceivingCountry = vm.DestinationCountryCode,
                ReceivingCurrency = vm.DestinationCurrencyCode,
                SendingCountry = vm.SourceCountryCode,
                SendingCurrency = vm.SourceCurrencyCode,
                TransferMethod = vm.TransferMethod,
                CreatedDate = DateTime.Now
            };
            dbContext.AuxAgentExchangeRateLimit.Add(model);
            dbContext.SaveChanges();
        }

        internal void UpdateAuxAgentExchangeRateLimit(ExchangeRateSettingViewModel vm)
        {
            var data = AuxAgentExchangeRateLimitList().Where(x => x.Id == vm.Id).FirstOrDefault();
            data.AgentId = vm.AgentId ?? 0;
            data.ExchangeRate = vm.ExchangeRate;
            data.ReceivingCountry = vm.DestinationCountryCode;
            data.ReceivingCurrency = vm.DestinationCurrencyCode;
            data.SendingCountry = vm.SourceCountryCode;
            data.SendingCurrency = vm.SourceCurrencyCode;
            data.TransferMethod = vm.TransferMethod;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }

        internal void UpdateAuxAgentExchangeRate(ExchangeRateSettingViewModel vm)
        {
            int staffId = Common.StaffSession.LoggedStaff.StaffId;
            GetFromAndToRange(vm);

            var data = dbContext.TransferExchangeRateByCurrency.Where(x => x.Id == vm.TransferFeeByCurrencyId).FirstOrDefault();
            int transferFeeByCurrencyId = 0;
            if (data != null)
            {
                data.FromRange = vm.FromRange;
                data.Range = vm.Range;
                data.Rate = vm.ExchangeRate;
                data.ReceivingCurrency = vm.DestinationCurrencyCode;
                data.SendingCurrency = vm.SourceCurrencyCode;
                data.ToRange = vm.ToRange;
                data.TransferMethod = vm.TransferMethod;
                data.TransferType = vm.TransferType;
                data.CreatedBy = staffId;
                data.AgentId = vm.AgentId ?? 0;

                dbContext.Entry<TransferExchangeRateByCurrency>(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                transferFeeByCurrencyId = data.Id;
                RemoveRangeExchangeRate(transferFeeByCurrencyId);
            }
            else
            {
                transferFeeByCurrencyId = AddTransferExchangeRateByCurrency(vm);
                RemoveExchangeRate(vm.Id);
            }
            AddAExchangeRate(vm, transferFeeByCurrencyId);

        }

        private void RemoveRangeExchangeRate(int transferFeeByCurrencyId)
        {
            var ListOfExchangeRate = dbContext.ExchangeRate.Where(x => x.TransferFeeByCurrencyId == transferFeeByCurrencyId).ToList();
            dbContext.ExchangeRate.RemoveRange(ListOfExchangeRate);
            dbContext.SaveChanges();
        }
        private void RemoveExchangeRate(int Id)
        {
            var ExchangeRate = dbContext.ExchangeRate.Where(x => x.Id == Id).FirstOrDefault();
            dbContext.ExchangeRate.Remove(ExchangeRate);
            dbContext.SaveChanges();
        }

        internal void AddAuxAgentExchangeRate(ExchangeRateSettingViewModel vm)
        {
            GetFromAndToRange(vm);
            int transferFeeByCurrencyId = AddTransferExchangeRateByCurrency(vm);
            AddAExchangeRate(vm, transferFeeByCurrencyId);


        }
        private ExchangeRateSettingViewModel GetFromAndToRange(ExchangeRateSettingViewModel vm)
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


        private void AddAExchangeRate(ExchangeRateSettingViewModel vm, int transferFeeByCurrencyId)
        {
            List<string> sendingCountries = new List<string>();
            List<string> ReceningCountries = new List<string>();
            if (vm.SourceCountryCode.ToLower() == "all")
            {
                sendingCountries = Common.Common.GetCountriesByCurrency(vm.SourceCurrencyCode);

            }
            else
            {
                sendingCountries.Add(vm.SourceCountryCode);
            }
            if (vm.DestinationCountryCode.ToLower() == "all")
            {
                ReceningCountries = Common.Common.GetCountriesByCurrency(vm.DestinationCurrencyCode);
            }
            else
            {
                ReceningCountries.Add(vm.DestinationCountryCode);
            }

            int StaffId = Common.StaffSession.LoggedStaff.StaffId;
            foreach (var item in sendingCountries)
            {
                foreach (var receivingCountry in ReceningCountries)
                {
                    ExchangeRate ExchangeRate = new ExchangeRate()
                    {
                        AgentId = vm.AgentId ?? 0,
                        CreatedDate = DateTime.Now,
                        CountryCode2 = receivingCountry,
                        CountryCode1 = item,
                        TransferMethod = vm.TransferMethod,
                        Rate = vm.ExchangeRate,
                        FromRange = vm.FromRange,
                        ToRange = vm.ToRange,
                        CreatedBy = StaffId.ToString(),
                        TransferType = vm.TransferType,
                        TransferFeeByCurrencyId = transferFeeByCurrencyId,
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
                        Rate = vm.ExchangeRate,
                        FromRange = vm.FromRange,
                        ToRange = vm.ToRange,
                        CreatedBy = StaffId.ToString(),
                        TransferType = vm.TransferType
                    };
                    dbContext.TransferExchangeRateHistory.Add(hist);
                    dbContext.SaveChanges();

                }
            }
        }

        private int AddTransferExchangeRateByCurrency(ExchangeRateSettingViewModel vm)
        {
            int staffId = Common.StaffSession.LoggedStaff.StaffId;

            TransferExchangeRateByCurrency model = new TransferExchangeRateByCurrency()
            {
                CreatedBy = staffId,
                AgentId = vm.AgentId ?? 0,
                CreatedDate = DateTime.Now,
                Range = vm.Range,
                Rate = vm.ExchangeRate,
                ReceivingCurrency = vm.DestinationCurrencyCode,
                SendingCurrency = vm.SourceCurrencyCode,
                TransferMethod = vm.TransferMethod,
                TransferType = vm.TransferType,
                ToRange = vm.ToRange,
                FromRange = vm.FromRange,

            };
            var data = dbContext.TransferExchangeRateByCurrency.Add(model);
            dbContext.SaveChanges();

            return data.Id;
        }
        public ExchangeRate GetRates(string sendingCurrency, string ReceivingCurrency, string SendingCountry, string ReceivingCountry,
            int TransferMethod, string Range, int AgentId)
        {

            string[] FullRange = Range.Split('-');
            decimal FromRange = decimal.Parse(FullRange[0]);
            decimal ToRange = 0;
            if (FullRange.Length < 2)
            {
                ToRange = int.MaxValue;
            }
            else
            {
                ToRange = decimal.Parse(FullRange[1]);
            }
            var exchangeRateByCurency = dbContext.TransferExchangeRateByCurrency.Where(x => x.SendingCurrency == sendingCurrency &&
                                                                                            x.ReceivingCurrency == ReceivingCurrency &&
                                                                                            x.TransferMethod == (TransactionTransferMethod)TransferMethod &&
                                                                                            x.TransferType == TransactionTransferType.AuxAgent &&
                                                                                            x.AgentId == AgentId &&
                                                                                            x.FromRange == FromRange && x.ToRange == ToRange).FirstOrDefault();
            ExchangeRate exchangeRate = null;
            if (exchangeRateByCurency != null)
            {
                exchangeRate = dbContext.ExchangeRate.Where(x => x.TransferFeeByCurrencyId == exchangeRateByCurency.Id).FirstOrDefault();
            }
            if (exchangeRate == null)
            {
                var exchangeRates = dbContext.ExchangeRate.Where(x => x.TransferType == TransactionTransferType.AuxAgent
                                     && x.AgentId == AgentId && x.TransferMethod == (TransactionTransferMethod)TransferMethod && x.FromRange == FromRange
                                     && x.ToRange == ToRange).ToList();

                if (SendingCountry == "All")
                {
                    var sendingCountries = dbContext.Country.Where(x => x.Currency == sendingCurrency).ToList();
                    exchangeRates = (from c in exchangeRates
                                     join SCountry in sendingCountries on c.CountryCode1 equals SCountry.CountryCode
                                     select c).ToList();
                }
                else
                {
                    exchangeRates = exchangeRates.Where(x => x.CountryCode1 == SendingCountry).ToList();
                }

                if (ReceivingCountry == "All")
                {
                    var receivingCountries = dbContext.Country.Where(x => x.Currency == ReceivingCurrency).ToList();
                    exchangeRates = (from c in exchangeRates
                                     join RCountry in receivingCountries on c.CountryCode2 equals RCountry.CountryCode
                                     select c).ToList();
                }
                else
                {
                    exchangeRates = exchangeRates.Where(x => x.CountryCode2 == ReceivingCountry).ToList();
                }
                exchangeRate = exchangeRates.FirstOrDefault();
            }

            return exchangeRate;
        }

    }



}