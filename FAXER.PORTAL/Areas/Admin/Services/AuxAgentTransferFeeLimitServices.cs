using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class AuxAgentTransferFeeLimitServices
    {
        FAXEREntities dbContext = null;
        public AuxAgentTransferFeeLimitServices()
        {
            dbContext = new FAXEREntities();
        }
        public IQueryable<AuxAgentTransferFeeLimit> AuxAgentTransferFeeList()
        {
            return dbContext.AuxAgentTransferFeeLimit;
        }
        public IQueryable<TransferFeePercentage> AuxAgentTransferFees()
        {
            return dbContext.TransferFeePercentage.Where(x => x.TransferType == TransactionTransferType.AuxAgent);
        }

        public AuxAgentTransferFeeLimit GetAuxAgentTransferFeeById(int id)
        {
            return AuxAgentTransferFeeList().Where(x => x.Id == id).FirstOrDefault();
        }


        internal List<TransferFeePercentageViewModel> GetAuxAgentTranferfees(string sendingCountry = "", int agentId = 0, string date = "", int method = 0)
        {
            var data = AuxAgentTransferFees().Where(x => x.TransferType == TransactionTransferType.Agent);

            CommonServices commonServices = new CommonServices();

            if (!string.IsNullOrEmpty(sendingCountry))
            {
                data = data.Where(x => x.SendingCountry == sendingCountry);
            }
            if (method > 0)
            {
                data = data.Where(x => x.TransferMethod == (TransactionTransferMethod)method);
            }
            if (!string.IsNullOrEmpty(date))
            {
                string[] DateString = date.Split('-');
                DateTime FromDate = Convert.ToDateTime(DateString[0]);
                DateTime ToDate = Convert.ToDateTime(DateString[1]);
                data = data.Where(x => x.CreatedDate >= FromDate && x.CreatedDate <= ToDate);
            }

            //var result = data.GroupBy(x => new
            //{
            //    x.SendingCountry,
            //    x.ReceivingCountry,
            //    x.TransferMethod,
            //}).Select(d => new TransferFeePercentageViewModel()
            //{
            //    Id = d.FirstOrDefault().Id,
            //    Fee = d.FirstOrDefault().Fee,
            //    SendingCountry = d.FirstOrDefault().SendingCountry,
            //    SendingCurrency = Common.Common.GetCountryCurrency(d.FirstOrDefault().SendingCountry),
            //    ReceivingCurrency = Common.Common.GetCountryCurrency(d.FirstOrDefault().ReceivingCountry),
            //    ReceivingCountry = d.FirstOrDefault().ReceivingCountry,
            //    SendingCountryName = Common.Common.GetCountryName(d.FirstOrDefault().SendingCountry),
            //    ReceivingCountryName = Common.Common.GetCountryName(d.FirstOrDefault().ReceivingCountry),
            //    SendingCountryFlag = d.FirstOrDefault().SendingCountry.ToLower(),
            //    ReceivingCountryFlag = d.FirstOrDefault().ReceivingCountry.ToLower(),
            //    Percentage = d.Where(x => x.FeeType == FeeType.Percent).Select(x => x.Fee).FirstOrDefault(),
            //    FlatFee = d.Where(x => x.FeeType == FeeType.FlatFee).Select(x => x.Fee).FirstOrDefault(),
            //    TransferMethod = d.FirstOrDefault().TransferMethod,
            //    TransferMethodName = Common.Common.GetEnumDescription(d.FirstOrDefault().TransferMethod),
            //    Range = d.FirstOrDefault().Range,
            //    AgentId = d.FirstOrDefault().AgetnId,
            //    AgentName = commonServices.getAgentName(d.FirstOrDefault().AgetnId),
            //    AgentAccountNo = commonServices.GetAgentAccNo(d.FirstOrDefault().AgetnId),
            //    RangeList = GetListofRange(d.FirstOrDefault().ReceivingCountry, d.FirstOrDefault().SendingCountry, d.FirstOrDefault().TransferMethod),
            //}).ToList();

            var result = (from c in data.ToList()
                          join agentInfo in dbContext.AgentInformation.Where(x => x.IsAUXAgent == true) on c.AgetnId equals agentInfo.Id into joined
                          from agentInfo in joined.DefaultIfEmpty()
                          join SendingCountry in dbContext.Country on c.SendingCountry equals SendingCountry.CountryCode
                          join ReceivingCountry in dbContext.Country on c.ReceivingCountry equals ReceivingCountry.CountryCode
                          select new TransferFeePercentageViewModel()
                          {
                              Id = c.Id,
                              Fee = c.Fee,
                              SendingCountry = c.SendingCountry,
                              SendingCurrency = SendingCountry.Currency,
                              ReceivingCurrency = ReceivingCountry.Currency,
                              ReceivingCountry = c.ReceivingCountry,
                              SendingCountryName = SendingCountry.CountryName,
                              ReceivingCountryName = ReceivingCountry.CountryName,
                              SendingCountryFlag = c.SendingCountry.ToLower(),
                              ReceivingCountryFlag = c.ReceivingCountry.ToLower(),
                              FeeType = c.FeeType,
                              TransferMethod = c.TransferMethod,
                              TransferMethodName = Common.Common.GetEnumDescription(c.TransferMethod),
                              RangeName = Common.Common.GetEnumDescription(c.Range),
                              AgentId = c.AgetnId,
                              Range = c.Range,
                              AgentAccountNo = agentInfo == null ? "" : agentInfo.AccountNo,
                              AgentName = agentInfo == null ? "All" : agentInfo.Name,
                          }).ToList();

            return result;

        }
        public List<string> GetListofRange(string receivingCountry, string sendingCountry, TransactionTransferMethod transactionTransferMethod)
        {
            var data = AuxAgentTransferFees().Where(x => x.SendingCountry == sendingCountry && x.ReceivingCountry == receivingCountry &&
            x.TransferMethod == transactionTransferMethod).ToList();

            List<string> RangeList = new List<string>();
            foreach (var item in data.OrderBy(x => x.Range))
            {
                string RangeName = Common.Common.GetEnumDescription(item.Range);

                RangeList.Add(RangeName);
            }

            return RangeList;
        }


        internal TransferFeePercentageViewModel GetAuxAgentTranferFee(int id = 0)
        {
            var auxAgentTransferFee = AuxAgentTransferFees().Where(x => x.Id == id).FirstOrDefault();
            string sendingCountry = "";
            string receivingCountry = "";
            string sendingCurrency = "";
            string receivingCurrency = "";
            if (auxAgentTransferFee.TransfeFeeByCurrencyId > 0)
            {
                var EchangeRateByCurrency = AuxAgentTransferFees().Where(x => x.TransfeFeeByCurrencyId == auxAgentTransferFee.TransfeFeeByCurrencyId);
                var TransferFeePercentageByCurrency = dbContext.TransferFeePercentageByCurrency.Where(x => x.Id == auxAgentTransferFee.TransfeFeeByCurrencyId).FirstOrDefault();

                sendingCurrency = TransferFeePercentageByCurrency.SendingCurrency;
                receivingCurrency = TransferFeePercentageByCurrency.ReceivingCurrency;


                List<string> sendingCountries = (from c in Common.Common.GetCountriesByCurrency(sendingCurrency)
                                                 join d in EchangeRateByCurrency on c equals d.SendingCountry
                                                 select c).Distinct().ToList();
                List<string> receivingCountries = (from c in Common.Common.GetCountriesByCurrency(receivingCurrency)
                                                   join d in EchangeRateByCurrency on c equals d.ReceivingCountry
                                                   select c).Distinct().ToList();

                sendingCountry = sendingCountries.Count() > 1 ? "All" : auxAgentTransferFee.SendingCountry;
                receivingCountry = receivingCountries.Count() > 1 ? "All" : auxAgentTransferFee.ReceivingCountry;
            }
            else
            {
                sendingCountry = auxAgentTransferFee.SendingCountry;
                receivingCountry = auxAgentTransferFee.ReceivingCountry;
                sendingCurrency = Common.Common.GetCountryCurrency(sendingCountry);
                receivingCurrency = Common.Common.GetCountryCurrency(receivingCountry);
            }
            TransferFeePercentageViewModel vm = new TransferFeePercentageViewModel()
            {
                Id = auxAgentTransferFee.Id,
                AgentId = auxAgentTransferFee.AgetnId,
                ReceivingCurrency = receivingCurrency,
                ReceivingCountry = receivingCountry,
                SendingCurrency = sendingCurrency,
                SendingCountry = sendingCountry,
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

        public TranfserFeeRange GetRange(string Range)
        {
            TranfserFeeRange FeeRange = new TranfserFeeRange();
            if (Range == "1")
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

        internal TransferFeePercentage GetRates(string sendingCurrency, string receivingCurrency, string sendingCountry,
            string receivingCountry, int transferMethod, string range, int AgentId, int Feetype)
        {
            string[] FullRange = range.Split('-');
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
            var exchangeFeeByCurency = dbContext.TransferFeePercentageByCurrency.Where(x => x.SendingCurrency == sendingCurrency &&
                                                                                            x.ReceivingCurrency == receivingCurrency &&
                                                                                            x.TransferMethod == (TransactionTransferMethod)transferMethod &&
                                                                                            x.TransferType == TransactionTransferType.AuxAgent &&
                                                                                            x.FeeType == (FeeType)Feetype &&
                                                                                            x.AgentId == AgentId &&
                                                                                            x.FromRange == FromRange && x.ToRange == ToRange).FirstOrDefault();
            TransferFeePercentage exchangeFee = null;
            if (exchangeFeeByCurency != null)
            {
                exchangeFee = dbContext.TransferFeePercentage.Where(x => x.TransfeFeeByCurrencyId == exchangeFeeByCurency.Id).FirstOrDefault();
            }
            if (exchangeFee == null)
            {
                var TranfserFeeRange = GetRange(range);
                var exchangeFees = dbContext.TransferFeePercentage.Where(x => x.TransferType == TransactionTransferType.AuxAgent
                                     && x.FeeType == (FeeType)Feetype && x.TransferMethod == (TransactionTransferMethod)transferMethod && x.AgetnId == AgentId && x.Range == TranfserFeeRange).ToList();

                if (sendingCountry == "All")
                {
                    var sendingCountries = dbContext.Country.Where(x => x.Currency == sendingCurrency).ToList();
                    exchangeFees = (from c in exchangeFees
                                    join SCountry in sendingCountries on c.SendingCountry equals SCountry.CountryCode
                                    select c).ToList();
                }
                else
                {
                    exchangeFees = exchangeFees.Where(x => x.SendingCountry == sendingCountry).ToList();
                }

                if (receivingCountry == "All")
                {
                    var receivingCountries = dbContext.Country.Where(x => x.Currency == receivingCurrency).ToList();
                    exchangeFees = (from c in exchangeFees
                                    join RCountry in receivingCountries on c.ReceivingCountry equals RCountry.CountryCode
                                    select c).ToList();
                }
                else
                {
                    exchangeFees = exchangeFees.Where(x => x.ReceivingCountry == receivingCountry).ToList();
                }
                exchangeFee = exchangeFees.FirstOrDefault();
            }

            return exchangeFee;
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

            var data = dbContext.TransferFeePercentageByCurrency.Where(x => x.Id == vm.TransfeFeeByCurrencyId).FirstOrDefault();
            int transferFeeByCurrencyId = 0;

            if (data != null)
            {
                data.ReceivingCurrency = vm.ReceivingCurrency;
                data.SendingCurrency = vm.SendingCurrency;
                data.FeeType = vm.FeeType;
                data.Fee = vm.Fee;
                data.TransferMethod = vm.TransferMethod;
                data.TransferType = vm.TransferType;
                data.FromRange = vm.FromRange;
                data.ToRange = vm.ToRange;
                data.OtherRange = vm.OtherRange;
                data.CreatedDate = data.CreatedDate;
                data.AgentId = vm.AgentId ?? 0;
                dbContext.Entry<TransferFeePercentageByCurrency>(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                transferFeeByCurrencyId = data.Id;
                RemoveTransferFees(transferFeeByCurrencyId);
            }
            else
            {
                AddTransferFeePercentageByCurrency(vm);
                RemoveTransferFee(vm.Id);
            }
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
            int staffId = Common.StaffSession.LoggedStaff.StaffId;
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
                CreatedBy = staffId,
                CreatedDate = DateTime.Now,
                OtherRange = vm.OtherRange,
            };

            var data = dbContext.TransferFeePercentageByCurrency.Add(TransferFeePercentageByCurrency);
            dbContext.SaveChanges();
            vm.TransfeFeeByCurrencyId = data.Id;

        }
        internal void AddTransferFeePercentage(TransferFeePercentageViewModel vm)
        {

            List<string> sendingCountries = new List<string>();
            List<string> ReceningCountries = new List<string>();
            if (vm.SendingCountry.ToLower() == "all")
            {
                sendingCountries = Common.Common.GetCountriesByCurrency(vm.SendingCurrency);
            }
            else
            {
                sendingCountries.Add(vm.SendingCountry);
            }
            if (vm.ReceivingCountry.ToLower() == "all")
            {
                ReceningCountries = Common.Common.GetCountriesByCurrency(vm.ReceivingCurrency);
            }
            else
            {
                ReceningCountries.Add(vm.ReceivingCountry);
            }

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
                        TransfeFeeByCurrencyId = vm.TransfeFeeByCurrencyId
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
        #region aux transfer fee limit

        internal TransferFeePercentageViewModel GetAuxAgentTranferfeeLimit(int Id = 0)
        {

            var data = (from c in AuxAgentTransferFeeList().Where(x => x.Id == Id).ToList()
                        join d in dbContext.AgentInformation on c.AgentId equals d.Id into joined
                        from agentInfo in joined.DefaultIfEmpty()
                        select new TransferFeePercentageViewModel
                        {
                            Id = c.Id,
                            AgentId = c.AgentId,
                            ReceivingCurrency = c.ReceivingCurrency,
                            ReceivingCountry = c.ReceivingCountry,
                            ReceivingCountryName = Common.Common.GetCountryName(c.ReceivingCountry),
                            SendingCurrency = c.SendingCurrency,
                            SendingCountry = c.SendingCountry,
                            SendingCountryName = Common.Common.GetCountryName(c.SendingCountry),
                            TransferMethod = c.TransferMethod,
                            TransferMethodName = Common.Common.GetEnumDescription(c.TransferMethod),
                            FeeType = c.FeeType,
                            AgentAccountNo = c.AgentId == 0 ? "" : agentInfo.AccountNo,
                            AgentName = c.AgentId == 0 ? "All" : agentInfo.Name,
                            CreatedDate = c.CreatedDate,
                            Range = c.Range,
                            RangeName = Common.Common.GetEnumDescription(c.Range),
                            Fee = c.TransferFee,

                        }).FirstOrDefault();
            return data;
        }

        internal void RemoveAuxAgentTransferFeeLimit(int id)
        {
            var data = GetAuxAgentTransferFeeById(id);
            dbContext.AuxAgentTransferFeeLimit.Remove(data);
            dbContext.SaveChanges();
        }

        internal void AddAuxAgentTransferFeeLimit(TransferFeePercentageViewModel vm)
        {


            AuxAgentTransferFeeLimit model = new AuxAgentTransferFeeLimit()
            {
                ReceivingCountry = vm.ReceivingCountry,
                ReceivingCurrency = vm.ReceivingCurrency,
                SendingCountry = vm.SendingCountry,
                AgentId = vm.AgentId ?? 0,
                SendingCurrency = vm.SendingCurrency,
                TransferFee = vm.Fee,
                TransferMethod = vm.TransferMethod,
                FeeType = vm.FeeType,
                Range = vm.Range,
                CreatedDate = DateTime.Now,
                OtherRange = vm.OtherRange

            };
            dbContext.AuxAgentTransferFeeLimit.Add(model);
            dbContext.SaveChanges();
        }

        internal void UpdateAuxAgentTransferFeeLimit(TransferFeePercentageViewModel vm)
        {
            var data = GetAuxAgentTransferFeeById(vm.Id);
            data.ReceivingCountry = vm.ReceivingCountry;
            data.ReceivingCurrency = vm.ReceivingCurrency;
            data.SendingCountry = vm.SendingCountry;
            data.AgentId = vm.AgentId ?? 0;
            data.SendingCurrency = vm.SendingCurrency;
            data.TransferFee = vm.Fee;
            data.TransferMethod = vm.TransferMethod;
            data.FeeType = vm.FeeType;
            data.Range = vm.Range;
            data.OtherRange = vm.OtherRange;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }
        #endregion
    }
}