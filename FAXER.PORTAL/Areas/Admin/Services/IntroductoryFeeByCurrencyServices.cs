using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class IntroductoryFeeByCurrencyServices
    {
        DB.FAXEREntities dbContext = null;
        CommonServices _CommonServices = null;
        public IntroductoryFeeByCurrencyServices()
        {
            dbContext = new DB.FAXEREntities();

            _CommonServices = new CommonServices();
        }
        public decimal GetFee(string sendingCurrency = "", string receivingCurrency = "", int transferType = 0, int method = 0, string range = "", int FeeType = 0, string otherRange = "", int AgentId = 0)
        {
            string[] Range = range.Split('-');
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
            var Rate = dbContext.IntroductoryFeeByCurrency.Where(x => x.SendingCurrency == sendingCurrency &&
                                                                      x.ReceivingCurrency == receivingCurrency && 
                                                                      x.TransferType == (TransactionTransferType)transferType &&
                                                                      x.TransferMethod == (TransactionTransferMethod)method &&
                                                                      x.AgentId == AgentId && x.FeeType == (FeeType)FeeType && 
                                                                      x.OtherRange == otherRange && 
                                                                      x.FromRange == FromRange && 
                                                                      x.ToRange == ToRange).Select(x => x.Fee).FirstOrDefault();
            return Rate;
        }

        public List<IntroductoryFeeByCurrency> List()
        {
            var data = dbContext.IntroductoryFeeByCurrency.ToList();
            return data;
        }

        public List<IntroductoryFeeByCurrencyViewModel> GetIntroductoryfeeList(string sendingCurrency, string receivingCurrency, int transferType, int transferMethod)
        {
            var data = List();

            if (!string.IsNullOrEmpty(sendingCurrency))
            {
                data = data.Where(x => x.SendingCurrency == sendingCurrency).ToList();
            }
            if (!string.IsNullOrEmpty(receivingCurrency))
            {

                data = data.Where(x => x.ReceivingCurrency == receivingCurrency).ToList();
            }
            if (transferType > 0)
            {

                data = data.Where(x => x.TransferType == (TransactionTransferType)transferType).ToList();
            }
            if (transferMethod > 0)
            {

                data = data.Where(x => x.TransferMethod == (TransactionTransferMethod)transferMethod).ToList();

            }
            var result = data.GroupBy(x => new
            {
                x.SendingCurrency,
                x.ReceivingCurrency,
                x.TransferMethod,
                x.TransferType
            }).Select(d => new IntroductoryFeeByCurrencyViewModel()
            {
                Id = d.FirstOrDefault().Id,
                Fee = d.FirstOrDefault().Fee,
                SendingCurrency = d.FirstOrDefault().SendingCurrency,
                ReceivingCurrency = d.FirstOrDefault().ReceivingCurrency,
                TransferType = d.FirstOrDefault().TransferType,
                Percentage = d.Where(x => x.FeeType == FeeType.Percent && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Fee).FirstOrDefault(),
                FlatFee = d.Where(x => x.FeeType == FeeType.FlatFee && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Fee).FirstOrDefault(),
                TransferMethod = d.FirstOrDefault().TransferMethod,
                Range = _CommonServices.GetRange(d.FirstOrDefault().FromRange, d.FirstOrDefault().ToRange),
                RangeList = GetListofRange(d.FirstOrDefault().SendingCurrency, d.FirstOrDefault().ReceivingCurrency, d.FirstOrDefault().TransferMethod
                , d.FirstOrDefault().TransferType),
                SendingCountryFlag = _CommonServices.getCountryCodeFromCurrency(d.FirstOrDefault().SendingCurrency).ToLower(),
                ReceivingCountryFlag = _CommonServices.getCountryCodeFromCurrency(d.FirstOrDefault().ReceivingCurrency).ToLower(),

            }).ToList();

            return result;
        }

        internal IntroductoryFeeByCurrencyViewModel GetIntroductoryFee(int id)
        {
            var data = List().Where(x => x.Id == id).ToList();
            var result = (from c in data
                          select new IntroductoryFeeByCurrencyViewModel()
                          {
                              Id = c.Id,
                              AgentId = c.AgentId,
                              Fee = c.Fee,
                              FeeType = c.FeeType,
                              CreatedDate = c.CreatedDate,
                              Range = _CommonServices.GetRange(c.FromRange, c.ToRange),
                              NumberOfTransaction = c.NumberOfTransaction,
                              ReceivingCurrency = c.ReceivingCurrency,
                              SendingCurrency = c.SendingCurrency,
                              TransferMethod = c.TransferMethod,
                              TransferType = c.TransferType,


                          }).FirstOrDefault();
            return result;
        }

        internal IntroductoryFeeByCurrency GetIntoductroyFeeDetials(string sendingCurrency, string receivingCurrency, int transferType, int method,
            string range, int feeType, int agentId)
        {

            string[] Range = range.Split('-');
            decimal fromRange = decimal.Parse(Range[0]);
            decimal toRange = 0;
            if (Range.Length < 2)
            {
                toRange = int.MaxValue;
            }
            else
            {
                toRange = decimal.Parse(Range[1]);
            }

            var data = dbContext.IntroductoryFeeByCurrency.Where(x => x.SendingCurrency == sendingCurrency && x.ReceivingCurrency == receivingCurrency && x.TransferType == (TransactionTransferType)transferType
            && x.TransferMethod == (TransactionTransferMethod)method && x.FromRange == fromRange && x.ToRange == toRange && x.FeeType == (FeeType)feeType
             && x.AgentId == agentId).FirstOrDefault();

            return data;

        }

        private List<string> GetListofRange(string sendingCurrency, string receivingCurrency, TransactionTransferMethod transferMethod, TransactionTransferType transferType)
        {
            var data = List().Where(x => x.SendingCurrency == sendingCurrency && x.ReceivingCurrency == receivingCurrency &&
            x.TransferMethod == transferMethod && x.TransferType == transferType).ToList();

            List<string> RangeList = new List<string>();

            foreach (var item in data.OrderBy(x => x.FromRange))
            {
                RangeList.Add(item.FromRange + "-" + item.ToRange);
            }

            return RangeList;
        }


        internal void Add(IntroductoryFeeByCurrencyViewModel vm)
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


            IntroductoryFeeByCurrency model = new IntroductoryFeeByCurrency()
            {
                NumberOfTransaction = vm.NumberOfTransaction,
                FromRange = FromRange,
                ToRange = ToRange,
                Fee = vm.Fee,
                CreatedDate = DateTime.Now,
                AgentId = vm.AgentId ?? 0,
                FeeType = vm.FeeType,
                OtherRange = vm.OtherRange,
                ReceivingCurrency = vm.ReceivingCurrency,
                SendingCurrency = vm.SendingCurrency,
                TransferMethod = vm.TransferMethod,
                TransferType = vm.TransferType
            };

            var data = dbContext.IntroductoryFeeByCurrency.Add(model);
            dbContext.SaveChanges();

            var sendingCountries = Common.Common.GetCountriesByCurrency(vm.SendingCurrency);
            var ReceningCountries = Common.Common.GetCountriesByCurrency(vm.ReceivingCurrency);

            foreach (var item in sendingCountries)
            {
                foreach (var receivingCountry in ReceningCountries)
                {

                    IntroductoryFee IntroductoryFee = new IntroductoryFee()
                    {
                        AgentId = vm.AgentId ?? 0,
                        CreatedDate = DateTime.Now,
                        Fee = vm.Fee,
                        ReceivingCountry = receivingCountry,
                        SendingCountry = item,
                        FeeType = vm.FeeType,
                        NumberOfTransaction = vm.NumberOfTransaction,
                        TransferMethod = vm.TransferMethod,
                        OtherRange = vm.OtherRange,
                        TransferType = vm.TransferType,
                        Range = GetRange(vm.Range),
                        IntroductoryFeeByCurrencyId = data.Id

                    };
                    dbContext.IntroductoryFee.Add(IntroductoryFee);
                    dbContext.SaveChanges();


                }
            }
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
        internal void Update(IntroductoryFeeByCurrencyViewModel model)
        {
            string[] Range = model.Range.Split('-');
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

            var data = List().Where(x => x.Id == model.Id).FirstOrDefault();

            data.ReceivingCurrency = model.ReceivingCurrency;
            data.SendingCurrency = model.SendingCurrency;
            data.FeeType = model.FeeType;
            data.Fee = model.Fee;
            data.TransferMethod = model.TransferMethod;
            data.TransferType = model.TransferType;
            data.FromRange = FromRange;
            data.ToRange = ToRange;
            data.OtherRange = model.OtherRange;
            data.CreatedDate = data.CreatedDate;
            data.NumberOfTransaction = model.NumberOfTransaction;
            data.AgentId = model.AgentId ?? 0;


            dbContext.Entry<IntroductoryFeeByCurrency>(data).State = EntityState.Modified;
            dbContext.SaveChanges();


            var ListOfIntroductoryFee = dbContext.IntroductoryFee.Where(x => x.IntroductoryFeeByCurrencyId == data.Id).ToList();

            dbContext.IntroductoryFee.RemoveRange(ListOfIntroductoryFee);
            dbContext.SaveChanges();

            var sendingCountries = Common.Common.GetCountriesByCurrency(model.SendingCurrency);
            var ReceningCountries = Common.Common.GetCountriesByCurrency(model.ReceivingCurrency);

            foreach (var item in sendingCountries)
            {
                foreach (var receivingCountry in ReceningCountries)
                {

                    IntroductoryFee IntroductoryFee = new IntroductoryFee()
                    {
                        AgentId = model.AgentId ?? 0,
                        CreatedDate = DateTime.Now,
                        Fee = model.Fee,
                        ReceivingCountry = receivingCountry,
                        SendingCountry = item,
                        FeeType = model.FeeType,
                        NumberOfTransaction = model.NumberOfTransaction,
                        TransferMethod = model.TransferMethod,
                        OtherRange = model.OtherRange,
                        TransferType = model.TransferType,
                        Range = GetRange(model.Range),
                        IntroductoryFeeByCurrencyId = data.Id,
                    };
                    dbContext.IntroductoryFee.Add(IntroductoryFee);
                    dbContext.SaveChanges();


                }
            }

        }

        internal void Delete(int id)
        {
            var data = List().Where(x => x.Id == id).FirstOrDefault();
            dbContext.IntroductoryFeeByCurrency.Remove(data);
            dbContext.SaveChanges();
            var ListOfIntroductoryFee = dbContext.IntroductoryFee.Where(x => x.IntroductoryFeeByCurrencyId == id).ToList();

            dbContext.IntroductoryFee.RemoveRange(ListOfIntroductoryFee);
            dbContext.SaveChanges();


        }
    }
}