using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class ExchangeRateSercives
    {
        FAXEREntities dbContext = null;
        public ExchangeRateSercives()
        {
            dbContext = new FAXEREntities();
        }
        public List<ExchangeRate> List()
        {
            var agentInfo = Common.AgentSession.AgentInformation;
            var data = dbContext.ExchangeRate.Where(x => x.AgentId == agentInfo.Id).ToList();
            return data;
        }
        public IQueryable<TransferExchangeRateByCurrency> TransferExchangeRateByCurrency()
        {
            var agentInfo = Common.AgentSession.AgentInformation;
            return dbContext.TransferExchangeRateByCurrency.Where(x => x.AgentId == agentInfo.Id);
        }


        public List<ExchangeRateSettingViewModel> GetExchangeRateList(string SendingCountry = "", string ReceivingCountry = "", int TransferType = 0, int transferMethod = 0)
        {
            var data = TransferExchangeRateByCurrency();
            if (!string.IsNullOrEmpty(SendingCountry))
            {
                data = data.Where(x => x.SendingCountry == SendingCountry);
            }
            if (!string.IsNullOrEmpty(ReceivingCountry))
            {
                data = data.Where(x => x.ReceivingCountry == ReceivingCountry);
            }
            if (TransferType != 0)
            {
                data = data.Where(x => x.TransferMethod == (TransactionTransferMethod)transferMethod);
            }
            if (transferMethod != 0)
            {
                if (transferMethod != 7)
                {
                    data = data.Where(x => x.TransferMethod == (TransactionTransferMethod)transferMethod);
                }
            }

            //var result = (from c in data.ToList()
            //              select new ExchangeRateSettingViewModel()
            //              {
            //                  Id = c.Id,
            //                  Range = c.FromRange + "-" + c.ToRange,
            //                  Rate = Math.Round(c.Rate, 2),
            //                  SourceCountryName = Common.Common.GetCountryName(c.CountryCode1),
            //                  DestinationCountryName = Common.Common.GetCountryName(c.CountryCode2),
            //                  TransferMethodName = Enum.GetName(typeof(TransactionTransferMethod), c.TransferMethod),
            //              }).ToList();

            //return result;

            var result = data.ToList().GroupBy(x => new
            {
                x.SendingCountry,
                x.ReceivingCountry,
                x.TransferMethod,
                x.Rate,

            }).Select(d => new ExchangeRateSettingViewModel()
            {
                Id = d.FirstOrDefault().Id,
                TransferType = d.FirstOrDefault().TransferType,
                TransferMethod = d.FirstOrDefault().TransferMethod,
                RangeList = GetListofRange(d.FirstOrDefault().ReceivingCountry, d.FirstOrDefault().SendingCountry, d.FirstOrDefault().TransferMethod
                , d.FirstOrDefault().TransferType),
                Range = Common.Common.GetRangeName(d.FirstOrDefault().FromRange + "-" + d.FirstOrDefault().ToRange),
                Rate = Math.Round(d.FirstOrDefault().Rate, 2),
                SourceCountryName = Common.Common.GetCountryName(d.FirstOrDefault().SendingCountry),
                DestinationCountryName = Common.Common.GetCountryName(d.FirstOrDefault().ReceivingCountry),
                TransferMethodName = Enum.GetName(typeof(TransactionTransferMethod), d.FirstOrDefault().TransferMethod),
            }).ToList();

            return result;

        }

        internal bool HasExceedRateLimit(ExchangeRateSettingViewModel vm)
        {
            bool HasExceedRateLimit = true;
            var sendingCurrency = Common.Common.GetCurrencyCode(vm.SourceCountryCode);
            var ExchangeRate = dbContext.AuxAgentExchangeRateLimit.Where(x => x.SendingCurrency == sendingCurrency && x.ReceivingCurrency == vm.DestinationCurrencyCode
                               && x.SendingCountry == vm.SourceCountryCode && x.ReceivingCountry == vm.DestinationCountryCode
                               && x.TransferMethod == vm.TransferMethod).Select(x => x.ExchangeRate).FirstOrDefault();
            if (ExchangeRate == 0)
            {
                ExchangeRate = dbContext.AuxAgentExchangeRateLimit.Where(x => x.SendingCurrency == sendingCurrency && x.ReceivingCurrency == vm.DestinationCurrencyCode
                               && (x.SendingCountry.ToLower() == "all" || x.SendingCountry == vm.SourceCountryCode) && (x.ReceivingCountry.ToLower() == "all" || x.ReceivingCountry == vm.SourceCountryCode)
                               && (x.TransferMethod == vm.TransferMethod || x.TransferMethod == TransactionTransferMethod.All)).Select(x => x.ExchangeRate).FirstOrDefault();
            }

            if (ExchangeRate != 0)
            {
                if (vm.ExchangeRate > ExchangeRate)
                {
                    HasExceedRateLimit = true;
                }
                else
                {
                    HasExceedRateLimit = false;
                }
            }
            return HasExceedRateLimit;
        }

        public TransferExchangeRateByCurrency GetRates(string SendingCountry, string ReceivingCountry, int TransferMethod, string Range,
            string sendingCurrency, string receivingCurrency, int agnetId)
        {
            decimal FromRange = 0;
            decimal ToRange = 0;
            if (!string.IsNullOrEmpty(Range))
            {
                string[] FullRange = Range.Split('-');
                FromRange = decimal.Parse(FullRange[0]);
                ToRange = 0;
                if (FullRange.Length < 2)
                {
                    ToRange = int.MaxValue;
                }
                else
                {
                    ToRange = decimal.Parse(FullRange[1]);
                }
            }

            var data = TransferExchangeRateByCurrency().Where(x => x.SendingCountry == SendingCountry && x.ReceivingCountry == ReceivingCountry
                                      && x.SendingCurrency == sendingCurrency && x.ReceivingCurrency == receivingCurrency
                                      && x.AgentId == agnetId
                                      && x.TransferType == TransactionTransferType.AuxAgent
                                      && x.TransferMethod == (TransactionTransferMethod)TransferMethod && (x.FromRange <= FromRange
                                      && x.ToRange >= ToRange)).FirstOrDefault();
            return data;
        }

        private List<string> GetListofRange(string countryCode2, string countryCode1, TransactionTransferMethod transferMethod, TransactionTransferType transferType)
        {
            var data = TransferExchangeRateByCurrency().Where(x => x.SendingCountry == countryCode1 && x.ReceivingCountry == countryCode2 &&
           x.TransferMethod == transferMethod && x.TransferType == transferType).ToList();

            List<string> RangeList = new List<string>();

            foreach (var item in data.OrderBy(x => x.FromRange))
            {
                var range = Common.Common.GetRangeName(item.FromRange + "-" + item.ToRange);
                RangeList.Add(range);
            }


            return RangeList;
        }

        public ExchangeRateSettingViewModel GetExchangeRate(int id)
        {
            var auxAgentExchangeRate = List().Where(x => x.Id == id).FirstOrDefault();

            string receivingCountry = "";

            string receivingCurrency = "";
            if (auxAgentExchangeRate.TransferFeeByCurrencyId > 0)
            {
                var EchangeRateByCurrency = List().Where(x => x.TransferFeeByCurrencyId == auxAgentExchangeRate.TransferFeeByCurrencyId);
                var TransferExchangeRateByCurrency = dbContext.TransferExchangeRateByCurrency.Where(x => x.Id == auxAgentExchangeRate.TransferFeeByCurrencyId).FirstOrDefault();

                receivingCurrency = TransferExchangeRateByCurrency.ReceivingCurrency;


                List<string> receivingCountries = (from c in Common.Common.GetCountriesByCurrency(receivingCurrency)
                                                   join d in EchangeRateByCurrency on c equals d.CountryCode2
                                                   select c).Distinct().ToList();

                receivingCountry = receivingCountries.Count() > 1 ? "All" : auxAgentExchangeRate.CountryCode2;
            }
            else
            {
                receivingCountry = auxAgentExchangeRate.CountryCode2;
                receivingCurrency = Common.Common.GetCountryCurrency(receivingCountry);
            }
            ExchangeRateSettingViewModel vm = new ExchangeRateSettingViewModel()
            {
                Id = auxAgentExchangeRate.Id,
                AgentId = auxAgentExchangeRate.AgentId,
                DestinationCurrencyCode = receivingCurrency,
                DestinationCountryCode = receivingCountry,
                TransferMethod = auxAgentExchangeRate.TransferMethod,
                TransferMethodName = Common.Common.GetEnumDescription(auxAgentExchangeRate.TransferMethod),
                Rate = auxAgentExchangeRate.Rate,
                CreatedDate = auxAgentExchangeRate.CreatedDate,
                ExchangeRate = auxAgentExchangeRate.Rate,
                Range = Common.Common.GetRangeName(auxAgentExchangeRate.FromRange + "-" + auxAgentExchangeRate.ToRange),
                TransferFeeByCurrencyId = auxAgentExchangeRate.TransferFeeByCurrencyId
            };
            return vm;


        }
        public ExchangeRateSettingViewModel GetExchangeRateByCurrency(int id)
        {
            var auxAgentExchangeRate = TransferExchangeRateByCurrency().Where(x => x.Id == id).FirstOrDefault();
            ExchangeRateSettingViewModel vm = new ExchangeRateSettingViewModel()
            {
                Id = auxAgentExchangeRate.Id,
                AgentId = auxAgentExchangeRate.AgentId,
                DestinationCurrencyCode = auxAgentExchangeRate.ReceivingCurrency,
                DestinationCountryCode = auxAgentExchangeRate.ReceivingCountry,
                TransferMethod = auxAgentExchangeRate.TransferMethod,
                TransferMethodName = Common.Common.GetEnumDescription(auxAgentExchangeRate.TransferMethod),
                Rate = auxAgentExchangeRate.Rate,
                CreatedDate = auxAgentExchangeRate.CreatedDate,
                ExchangeRate = auxAgentExchangeRate.Rate,
                Range = Common.Common.GetRangeName(auxAgentExchangeRate.FromRange + "-" + auxAgentExchangeRate.ToRange),
            };
            return vm;
        }

        internal void Add(ExchangeRateSettingViewModel vm)
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
            var agentInfo = Common.AgentSession.AgentInformation;
            ExchangeRate model = new ExchangeRate()
            {
                AgentId = agentInfo.Id,
                CountryCode1 = vm.SourceCountryCode,
                CountryCode2 = vm.DestinationCountryCode,
                Rate = vm.ExchangeRate,
                TransferMethod = vm.TransferMethod,
                TransferType = TransactionTransferType.Agent,
                FromRange = FromRange,
                ToRange = ToRange,

            };
            dbContext.ExchangeRate.Add(model);
            dbContext.SaveChanges();

        }

        internal void Update(ExchangeRateSettingViewModel vm)
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
            var agentInfo = Common.AgentSession.AgentInformation;
            var model = List().Where(x => x.Id == vm.Id).FirstOrDefault();
            model.AgentId = agentInfo.Id;
            model.CountryCode1 = vm.SourceCountryCode;
            model.CountryCode2 = vm.DestinationCountryCode;
            model.Rate = vm.ExchangeRate;
            model.TransferMethod = vm.TransferMethod;
            model.TransferType = TransactionTransferType.Agent;
            model.FromRange = FromRange;
            model.ToRange = ToRange;

            dbContext.Entry<ExchangeRate>(model).State = System.Data.Entity.EntityState.Modified;
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
            List<string> sendingCountries = CountryCommon.GetCountriesByCurrencyAndCountry(vm.SourceCurrencyCode, vm.SourceCountryCode);
            List<string> ReceningCountries = CountryCommon.GetCountriesByCurrencyAndCountry(vm.DestinationCurrencyCode, vm.DestinationCountryCode);

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
                        TransferType = vm.TransferType,
                        TransferFeeByCurrencyId = transferFeeByCurrencyId,
                        RecevingCurrency = vm.DestinationCurrencyCode,
                        SendingCurrency = vm.SourceCurrencyCode
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
                        TransferType = vm.TransferType,
                        ReceivingCurrency = vm.DestinationCurrencyCode,
                        SendingCurrency = vm.SourceCurrencyCode
                    };
                    dbContext.TransferExchangeRateHistory.Add(hist);
                    dbContext.SaveChanges();

                }
            }
        }

        private int AddTransferExchangeRateByCurrency(ExchangeRateSettingViewModel vm)
        {
            TransferExchangeRateByCurrency model = new TransferExchangeRateByCurrency()
            {
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
                SendingCountry = vm.SourceCountryCode,
                ReceivingCountry = vm.DestinationCountryCode
            };
            var data = dbContext.TransferExchangeRateByCurrency.Add(model);
            dbContext.SaveChanges();

            return data.Id;
        }



        internal void UpdateAuxAgentExchangeRate(ExchangeRateSettingViewModel vm)
        {
            GetFromAndToRange(vm);
            var data = dbContext.TransferExchangeRateByCurrency.Where(x => x.Id == vm.Id).FirstOrDefault();

            data.FromRange = vm.FromRange;
            data.Range = vm.Range;
            data.Rate = vm.ExchangeRate;
            data.ReceivingCurrency = vm.DestinationCurrencyCode;
            data.SendingCurrency = vm.SourceCurrencyCode;
            data.ReceivingCountry = vm.DestinationCountryCode;
            data.SendingCountry = vm.SourceCountryCode;
            data.ToRange = vm.ToRange;
            data.TransferMethod = vm.TransferMethod;
            data.TransferType = vm.TransferType;

            data.AgentId = vm.AgentId ?? 0;

            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            RemoveRangeExchangeRate(data.Id);
            AddAExchangeRate(vm, data.Id);

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

    }
}