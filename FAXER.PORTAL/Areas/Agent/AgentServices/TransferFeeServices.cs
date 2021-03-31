using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class TransferFeeServices
    {
        FAXEREntities dbContext = null;
        public TransferFeeServices()
        {
            dbContext = new FAXEREntities();
        }

        public List<TransferFeePercentage> List()
        {
            var agentInfo = Common.AgentSession.AgentInformation;
            var data = dbContext.TransferFeePercentage.Where(x => x.AgetnId == agentInfo.Id).ToList();
            return data;
        }
        public IQueryable<TransferFeePercentageByCurrency> TransferFeePercentages()
        {
            var agentInfo = Common.AgentSession.AgentInformation;
            return dbContext.TransferFeePercentageByCurrency.Where(x => x.AgentId == agentInfo.Id);
        }

        public List<TransferFeePercentageViewModel> GetTranferfeeList(string ReceivingCountry = "", int TransferType = 2, int TransferMethod = 0)
        {
            var data = TransferFeePercentages();

            if (!string.IsNullOrEmpty(ReceivingCountry))
            {

                data = data.Where(x => x.ReceivingCountry == ReceivingCountry);
            }
            if (TransferType != 0)
            {
                data = data.Where(x => x.TransferType == (TransactionTransferType)TransferType);
            }
            if (TransferMethod != 0)
            {
                if (TransferMethod != 7)

                {
                    data = data.Where(x => x.TransferMethod == (TransactionTransferMethod)TransferMethod);
                }

            }

            //var result = data.GroupBy(x => new
            //{
            //    x.SendingCountry,
            //    x.ReceivingCountry,
            //    x.TransferMethod,
            //    x.TransferType,


            //}).Select(d => new TransferFeePercentageViewModel()
            //{
            //    Id = d.FirstOrDefault().Id,
            //    Fee = d.FirstOrDefault().Fee,
            //    SendingCountry = Common.Common.GetCountryName(d.FirstOrDefault().SendingCountry),
            //    ReceivingCountry = Common.Common.GetCountryName(d.FirstOrDefault().ReceivingCountry),
            //    SendingCountryFlag = d.FirstOrDefault().SendingCountry.ToLower(),
            //    ReceivingCountryFlag = d.FirstOrDefault().ReceivingCountry.ToLower(),
            //    TransferType = d.FirstOrDefault().TransferType,
            //    Percentage = d.Where(x => x.FeeType == FeeType.Percent && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Fee).FirstOrDefault(),
            //    FlatFee = d.Where(x => x.FeeType == FeeType.FlatFee && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Fee).FirstOrDefault(),
            //    TransferMethod = d.FirstOrDefault().TransferMethod,
            //    Range = d.FirstOrDefault().Range,
            //    RangeList = GetListofRange(d.FirstOrDefault().ReceivingCountry, d.FirstOrDefault().SendingCountry, d.FirstOrDefault().TransferMethod
            //    , d.FirstOrDefault().TransferType),

            //}).ToList();

            var result = (from c in data.ToList()
                          join agentInfo in dbContext.AgentInformation.Where(x => x.IsAUXAgent == true) on c.AgentId equals agentInfo.Id into joined
                          from agentInfo in joined.DefaultIfEmpty()
                          join receivingCountry in dbContext.Country on c.ReceivingCountry equals receivingCountry.CountryCode into RCJoined
                          from receivingCountry in RCJoined.DefaultIfEmpty()
                          select new TransferFeePercentageViewModel()
                          {
                              Id = c.Id,
                              Fee = c.Fee,
                              SendingCountry = c.SendingCountry,
                              ReceivingCurrency = c.ReceivingCurrency,
                              ReceivingCountry = c.ReceivingCountry,
                              ReceivingCountryName = receivingCountry == null ? "All" :receivingCountry.CountryName,
                              SendingCountryFlag = c.SendingCountry != null ? c.SendingCountry.ToLower() : "all",
                              ReceivingCountryFlag = c.ReceivingCountry != null ? c.ReceivingCountry.ToLower() : "all",
                              FeeType = c.FeeType,
                              FeeTypeName = Common.Common.GetEnumDescription(c.FeeType),
                              TransferMethod = c.TransferMethod,
                              TransferMethodName = Common.Common.GetEnumDescription(c.TransferMethod),
                              RangeName = Common.Common.GetRangeName(c.FromRange + "-" + c.ToRange),
                              AgentId = c.AgentId,
                              AgentAccountNo = agentInfo == null ? "" : agentInfo.AccountNo,
                              AgentName = agentInfo == null ? "All" : agentInfo.Name,
                          }).ToList();

            return result;


        }

        public TransferFeePercentageViewModel GetTranferfee(int id)
        {
            var auxAgentTransferFee = List().Where(x => x.Id == id).FirstOrDefault();

            string receivingCountry = "";
            string receivingCurrency = "";
            if (auxAgentTransferFee.TransfeFeeByCurrencyId > 0)
            {
                var EchangeRateByCurrency = List().Where(x => x.TransfeFeeByCurrencyId == auxAgentTransferFee.TransfeFeeByCurrencyId);
                var TransferFeePercentageByCurrency = dbContext.TransferFeePercentageByCurrency.Where(x => x.Id == auxAgentTransferFee.TransfeFeeByCurrencyId).FirstOrDefault();

                receivingCurrency = TransferFeePercentageByCurrency.ReceivingCurrency;


                List<string> receivingCountries = (from c in Common.Common.GetCountriesByCurrency(receivingCurrency)
                                                   join d in EchangeRateByCurrency on c equals d.ReceivingCountry
                                                   select c).Distinct().ToList();

                receivingCountry = receivingCountries.Count() > 1 ? "All" : auxAgentTransferFee.ReceivingCountry;
            }
            else
            {
                receivingCountry = auxAgentTransferFee.ReceivingCountry;
                receivingCurrency = Common.Common.GetCountryCurrency(receivingCountry);
            }
            TransferFeePercentageViewModel vm = new TransferFeePercentageViewModel()
            {
                Id = auxAgentTransferFee.Id,
                AgentId = auxAgentTransferFee.AgetnId,
                ReceivingCurrency = receivingCurrency,
                ReceivingCountry = receivingCountry,
                TransferMethod = auxAgentTransferFee.TransferMethod,
                TransferMethodName = Common.Common.GetEnumDescription(auxAgentTransferFee.TransferMethod),
                FeeType = auxAgentTransferFee.FeeType,
                CreatedDate = auxAgentTransferFee.CreatedDate,
                Fee = auxAgentTransferFee.Fee,
                TransfeFeeByCurrencyId = auxAgentTransferFee.TransfeFeeByCurrencyId,
                OtherRange = auxAgentTransferFee.OtherRange,
                TransferType = auxAgentTransferFee.TransferType,
                RangeName = Common.Common.GetEnumDescription(auxAgentTransferFee.Range)

            };
            return vm;

        }
        public TransferFeePercentageViewModel GetTranferfeeByCurrency(int id)
        {
            var auxAgentTransferFee = TransferFeePercentages().Where(x => x.Id == id).FirstOrDefault();

            TransferFeePercentageViewModel vm = new TransferFeePercentageViewModel()
            {
                Id = auxAgentTransferFee.Id,
                AgentId = auxAgentTransferFee.AgentId,
                ReceivingCurrency = auxAgentTransferFee.ReceivingCurrency,
                ReceivingCountry = auxAgentTransferFee.ReceivingCountry,
                TransferMethod = auxAgentTransferFee.TransferMethod,
                TransferMethodName = Common.Common.GetEnumDescription(auxAgentTransferFee.TransferMethod),
                FeeType = auxAgentTransferFee.FeeType,
                CreatedDate = auxAgentTransferFee.CreatedDate,
                Fee = auxAgentTransferFee.Fee,
                OtherRange = auxAgentTransferFee.OtherRange,
                TransferType = auxAgentTransferFee.TransferType,
                RangeName = Common.Common.GetRangeName(auxAgentTransferFee.FromRange + "-" + auxAgentTransferFee.ToRange)

            };
            return vm;

        }

        internal bool HasExceededTransferFeeLimit(TransferFeePercentageViewModel vm)
        {
            bool HasExceededFeeLimit = true;
            var sendingCurrency = Common.Common.GetCurrencyCode(vm.SendingCountry);
            var receivingCurrency = Common.Common.GetCurrencyCode(vm.ReceivingCountry);
            var TransferFee = dbContext.AuxAgentTransferFeeLimit.Where(x => x.SendingCurrency == sendingCurrency && x.ReceivingCurrency == receivingCurrency
                               && x.SendingCountry == vm.SendingCountry && x.ReceivingCountry == vm.ReceivingCountry
                               && x.TransferMethod == vm.TransferMethod && x.FeeType == vm.FeeType).Select(x => x.TransferFee).FirstOrDefault();
            if (TransferFee == 0)
            {
                TransferFee = dbContext.AuxAgentTransferFeeLimit.Where(x => x.SendingCurrency == sendingCurrency && x.ReceivingCurrency == receivingCurrency
                               && (x.SendingCountry.ToLower() == "all" || x.SendingCountry == vm.SendingCountry) && (x.ReceivingCountry.ToLower() == "all" || x.ReceivingCountry == vm.ReceivingCountry)
                               && (x.TransferMethod == vm.TransferMethod || x.TransferMethod == TransactionTransferMethod.All) && x.FeeType == vm.FeeType).Select(x => x.TransferFee).FirstOrDefault();
            }

            if (TransferFee != 0)
            {
                if (vm.Fee > TransferFee)
                {
                    HasExceededFeeLimit = true;
                }
                else
                {
                    HasExceededFeeLimit = false;
                }
            }
            return HasExceededFeeLimit;
        }

        public void Add(TransferFeePercentageViewModel vm)
        {
            var agentInfo = Common.AgentSession.AgentInformation;
            TransferFeePercentage model = new TransferFeePercentage()
            {
                AgetnId = agentInfo.Id,
                Fee = vm.Fee,
                FeeType = vm.FeeType,
                OtherRange = vm.OtherRange,
                ReceivingCountry = vm.ReceivingCountry,
                SendingCountry = vm.SendingCountry,
                Range = vm.Range,
                TransferMethod = vm.TransferMethod,
                TransferType = vm.TransferType,
                CreatedDate = DateTime.Now,

            };
            dbContext.TransferFeePercentage.Add(model);
            dbContext.SaveChanges();
        }

        internal TransferFeePercentageByCurrency GetRates(string sendingCountry, string receivingCountry, string sendingCurrency, string receivingCurrecny,
            int transfertype, int transferMethod, string rangeName, int feeType, int agentId)
        {
            decimal FromRange = 0;
            decimal ToRange = 0;
            if (!string.IsNullOrEmpty(rangeName))
            {
                string[] Range = rangeName.Split('-');
                FromRange = decimal.Parse(Range[0]);
                ToRange = 0;
                if (Range.Length < 2)
                {
                    ToRange = int.MaxValue;
                }
                else
                {
                    ToRange = decimal.Parse(Range[1]);
                }
            }

            var data = TransferFeePercentages().Where(x => x.SendingCurrency == sendingCurrency && x.ReceivingCurrency == receivingCurrecny &&
            x.SendingCountry == sendingCountry && x.ReceivingCountry == receivingCountry &&
            x.TransferType == (TransactionTransferType)transfertype &&
            x.TransferMethod == (TransactionTransferMethod)transferMethod &&
             x.AgentId == agentId &&
            (x.FromRange <= FromRange && x.ToRange >= ToRange) &&
            x.FeeType == (FeeType)feeType).FirstOrDefault();
            return data;
        }

        public void Update(TransferFeePercentageViewModel vm)
        {
            var agentInfo = Common.AgentSession.AgentInformation;
            var model = List().Where(x => x.Id == vm.Id).FirstOrDefault();
            model.ReceivingCountry = vm.ReceivingCountry;
            model.SendingCountry = agentInfo.CountryCode;
            model.FeeType = vm.FeeType;
            model.Fee = vm.Fee;
            model.TransferMethod = vm.TransferMethod;
            model.TransferType = vm.TransferType;
            model.Range = vm.Range;
            model.OtherRange = vm.OtherRange;

            model.AgetnId = agentInfo.Id;
            dbContext.Entry<TransferFeePercentage>(model).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public List<string> GetListofRange(string receivingCountry, string sendingCountry, TransactionTransferMethod transactionTransferMethod, TransactionTransferType transactionTransferType)
        {
            var data = dbContext.TransferFeePercentage.Where(x => x.SendingCountry == sendingCountry && x.ReceivingCountry == receivingCountry &&
            x.TransferMethod == transactionTransferMethod && x.TransferType == transactionTransferType).ToList();

            List<string> RangeList = new List<string>();
            foreach (var item in data.OrderBy(x => x.Range))
            {
                string RangeName = Common.Common.GetEnumDescription(item.Range);

                RangeList.Add(RangeName);
            }

            return RangeList;
        }



        public TranfserFeeRange GetRange(string Range)
        {
            TranfserFeeRange FeeRange = new TranfserFeeRange();
            if (Range == "0-0")
            {
                FeeRange = TranfserFeeRange.All;
            }
            else if (Range == "1-100")
            {

                FeeRange = TranfserFeeRange.OneToHundred;
            }
            else if (Range == "101-500")
            {
                FeeRange = TranfserFeeRange.HundredOnetoFiveHundred;
            }
            else if (Range == "501-1000")
            {
                FeeRange = TranfserFeeRange.FivehundredOneToThousand;
            }
            else if (Range == "1001-1500")
            {
                FeeRange = TranfserFeeRange.ThousandOneToFifteenHundred;
            }
            else if (Range == "1501-2000")
            {
                FeeRange = TranfserFeeRange.FifteenHundredOneToTwoThousand;
            }

            else if (Range == "2001-3000 ")
            {
                FeeRange = TranfserFeeRange.TwothousandOneToThreeThousand;
            }
            else if (Range == "3001-5000")
            {
                FeeRange = TranfserFeeRange.ThreeTHousandOneToFiveThousand;
            }
            else if (Range == "5001-10000")
            {
                FeeRange = TranfserFeeRange.FivethousandOneToTenThousand;
            }
            else if (Range == "11000+")
            {
                FeeRange = TranfserFeeRange.AboveTenThousand;
            }
            else
            {
                FeeRange = TranfserFeeRange.Select;
            }
            return FeeRange;
        }

        private TransferFeePercentageViewModel GetFromAndToRange(TransferFeePercentageViewModel vm)
        {
            string[] Range = vm.RangeName.Split('-');
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

        internal void UpdateTransferFee(TransferFeePercentageViewModel vm)
        {

            GetFromAndToRange(vm);

            var data = dbContext.TransferFeePercentageByCurrency.Where(x => x.Id == vm.Id).FirstOrDefault();
            data.ReceivingCurrency = vm.ReceivingCurrency;
            data.SendingCurrency = vm.SendingCurrency;
            data.ReceivingCountry = vm.ReceivingCountry;
            data.SendingCountry = vm.SendingCountry;
            data.FeeType = vm.FeeType;
            data.Fee = vm.Fee;
            data.TransferMethod = vm.TransferMethod;
            data.TransferType = vm.TransferType;
            data.FromRange = vm.FromRange;
            data.ToRange = vm.ToRange;
            data.OtherRange = vm.OtherRange;
            data.CreatedDate = data.CreatedDate;
            data.AgentId = vm.AgentId ?? 0;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            RemoveTransferFees(data.Id);
            AddTransferFeePercentage(vm);
        }

        internal void AddTransferFee(TransferFeePercentageViewModel vm)
        {
            GetFromAndToRange(vm);
            AddTransferFeePercentageByCurrency(vm);
            AddTransferFeePercentage(vm);
        }
        internal void AddTransferFeePercentageByCurrency(TransferFeePercentageViewModel vm)
        {
            TransferFeePercentageByCurrency TransferFeePercentageByCurrency = new TransferFeePercentageByCurrency()
            {
                SendingCurrency = vm.SendingCurrency,
                ReceivingCurrency = vm.ReceivingCurrency,
                AgentId = vm.AgentId ?? 0,
                FromRange = vm.FromRange,
                ToRange = vm.ToRange,
                Fee = vm.Fee,
                FeeType = vm.FeeType,
                TransferMethod = vm.TransferMethod,
                TransferType = vm.TransferType,
                CreatedDate = DateTime.Now,
                OtherRange = vm.OtherRange,
                SendingCountry = vm.SendingCountry,
                ReceivingCountry = vm.ReceivingCountry
            };

            var data = dbContext.TransferFeePercentageByCurrency.Add(TransferFeePercentageByCurrency);
            dbContext.SaveChanges();
            vm.TransfeFeeByCurrencyId = data.Id;

        }
        internal void AddTransferFeePercentage(TransferFeePercentageViewModel vm)
        {

            List<string> sendingCountries = CountryCommon.GetCountriesByCurrencyAndCountry(vm.SendingCurrency, vm.SendingCountry);
            List<string> ReceningCountries = CountryCommon.GetCountriesByCurrencyAndCountry(vm.ReceivingCurrency, vm.ReceivingCountry);
            foreach (var item in sendingCountries)
            {
                foreach (var receivingCountry in ReceningCountries)
                {
                    TransferFeePercentage FeePercentage = new TransferFeePercentage()
                    {
                        AgetnId = vm.AgentId ?? 0,
                        CreatedDate = DateTime.Now,
                        ReceivingCountry = receivingCountry,
                        SendingCountry = item,
                        TransferMethod = vm.TransferMethod,
                        TransferType = vm.TransferType,
                        Fee = vm.Fee,
                        FeeType = vm.FeeType,
                        OtherRange = vm.OtherRange,
                        Range = GetRange(vm.RangeName),
                        TransfeFeeByCurrencyId = vm.TransfeFeeByCurrencyId,
                        SendingCurrency = vm.SendingCurrency,
                        ReceivingCurrency = vm.ReceivingCurrency
                    };
                    dbContext.TransferFeePercentage.Add(FeePercentage);
                    dbContext.SaveChanges();

                    TransferFeePercentageHistory history = new TransferFeePercentageHistory()
                    {
                        AgentId = vm.AgentId ?? 0,
                        CreatedDate = DateTime.Now,
                        ReceivingCountry = receivingCountry,
                        SendingCountry = item,
                        TransferMethod = vm.TransferMethod,
                        TransferType = vm.TransferType,
                        Fee = vm.Fee,
                        FeeType = vm.FeeType,
                        OtherRange = vm.OtherRange,
                        Range = GetRange(vm.RangeName),
                        ReceivingCurrency = vm.ReceivingCurrency,
                        SendingCurrency = vm.SendingCurrency
                    };
                    dbContext.TransferFeePercentageHistory.Add(history);
                    dbContext.SaveChanges();
                }
            }

        }

        internal void RemoveTransferFees(int transferFeeByCurrencyId)
        {
            var ListofTransferFeePercentage = dbContext.TransferFeePercentage.Where(x => x.TransfeFeeByCurrencyId == transferFeeByCurrencyId).ToList();
            dbContext.TransferFeePercentage.RemoveRange(ListofTransferFeePercentage);
            dbContext.SaveChanges();

        }
        private void RemoveTransferFee(int Id)
        {
            var transferFee = dbContext.TransferFeePercentage.Where(x => x.Id == Id).FirstOrDefault();
            dbContext.TransferFeePercentage.Remove(transferFee);
            dbContext.SaveChanges();
        }

    }
}