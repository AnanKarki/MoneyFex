using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class TransferFeePercentageByCurrencyServices
    {
        DB.FAXEREntities dbContext = null;

        public TransferFeePercentageByCurrencyServices()
        {
            dbContext = new DB.FAXEREntities();
        }
        public decimal GetFee(string sendingCurrency = "", string receivingCurrency = "", int transferType = 0, int method = 0, string range = "", int FeeType = 0, string otherRange = "", int AgentId = 0)
        {
            decimal FromRange = 0;
            decimal ToRange = 0;
            if (!string.IsNullOrEmpty(range))
            {
                string[] Range = range.Split('-');
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
            var Rate = dbContext.TransferFeePercentageByCurrency.Where(x => x.SendingCurrency == sendingCurrency &&
                                                                            x.ReceivingCurrency == receivingCurrency &&
                                                                            x.TransferType == (TransactionTransferType)transferType &&
                                                                            x.TransferMethod == (TransactionTransferMethod)method &&
                                                                            x.AgentId == AgentId &&
                                                                            x.FeeType == (FeeType)FeeType &&
                                                                            x.OtherRange == otherRange &&
                                                                            x.FromRange == FromRange &&
                                                                            x.ToRange == ToRange).Select(x => x.Fee).FirstOrDefault();
            return Rate;
        }
        public List<TransferFeePercentageByCurrency> List()
        {
            var data = dbContext.TransferFeePercentageByCurrency.ToList();
            return data;
        }
        public List<TransferFeePercentageByCurrencyViewModel> GetTranferfeeList(string sendingCurrency, string receivingCurrency, int transferType, int transferMethod)
        {
            CommonServices _commonServices = new CommonServices();
            var data = List();

            if (!string.IsNullOrEmpty(sendingCurrency))
            {
                data = data.Where(x => x.SendingCurrency == sendingCurrency).ToList();
            }
            if (!string.IsNullOrEmpty(receivingCurrency))
            {

                data = data.Where(x => x.ReceivingCurrency == receivingCurrency).ToList();
            }
            if (transferType != 0)
            {
                data = data.Where(x => x.TransferType == (TransactionTransferType)transferType).ToList();
            }
            if (transferMethod != 0)
            {
                if (transferMethod == 7)
                {
                    data = List();
                }
                else
                {
                    data = data.Where(x => x.TransferMethod == (TransactionTransferMethod)transferMethod).ToList();

                }

            }

            var result = data.GroupBy(x => new
            {
                x.SendingCurrency,
                x.ReceivingCurrency,
                x.TransferMethod,
                x.TransferType,


            }).Select(d => new TransferFeePercentageByCurrencyViewModel()
            {
                Id = d.FirstOrDefault().Id,
                Fee = d.FirstOrDefault().Fee,
                SendingCurrency = d.FirstOrDefault().SendingCurrency,
                ReceivingCurrency = d.FirstOrDefault().ReceivingCurrency,
                TransferType = d.FirstOrDefault().TransferType,
                Percentage = d.Where(x => x.FeeType == FeeType.Percent && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Fee).FirstOrDefault(),
                FlatFee = d.Where(x => x.FeeType == FeeType.FlatFee && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Fee).FirstOrDefault(),
                TransferMethod = d.FirstOrDefault().TransferMethod,
                Range = Common.Common.GetRangeName(d.FirstOrDefault().FromRange + "-" + d.FirstOrDefault().ToRange),
                RangeList = GetListofRange(d.FirstOrDefault().SendingCurrency, d.FirstOrDefault().ReceivingCurrency, d.FirstOrDefault().TransferMethod
                , d.FirstOrDefault().TransferType),
                SendingCountryFlag = _commonServices.getCountryCodeFromCurrency(d.FirstOrDefault().SendingCurrency).ToLower(),
                ReceivingCountryFlag = _commonServices.getCountryCodeFromCurrency(d.FirstOrDefault().ReceivingCurrency).ToLower(),
                SendingCountry = CountryCommon.GetCountryName(d.FirstOrDefault().SendingCountry),
                ReceivingCountry = CountryCommon.GetCountryName(d.FirstOrDefault().ReceivingCountry),


            }).ToList();



            return result;


        }

        private string GetRange(decimal fromRange, decimal toRange)
        {

            var from = (int)fromRange;
            var to = (int)toRange;
            var range = from + "-" + to;

            return range == null ? "0" : range;
        }

        internal void Add(TransferFeePercentageByCurrencyViewModel vm)
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
            TransferFeePercentageByCurrency TransferFeePercentageByCurrency = new TransferFeePercentageByCurrency()
            {
                SendingCurrency = vm.SendingCurrency,
                ReceivingCurrency = vm.ReceivingCurrency,
                SendingCountry = vm.SendingCountry,
                ReceivingCountry = vm.ReceivingCountry,
                AgentId = vm.AgentId ?? 0,
                FromRange = FromRange,
                ToRange = ToRange,
                Fee = vm.Fee,
                FeeType = vm.FeeType,
                TransferMethod = vm.TransferMethod,
                TransferType = vm.TransferType,
                CreatedBy = AdminSession.StaffId,
                CreatedDate = DateTime.Now,
                OtherRange = vm.OtherRange,
            };
            dbContext.TransferFeePercentageByCurrency.Add(TransferFeePercentageByCurrency);
            dbContext.SaveChanges();
            vm.Id = TransferFeePercentageByCurrency.Id;
            AddTransferFeePercentageAndHisotry(vm);
        }

        private void AddTransferFeePercentageAndHisotry(TransferFeePercentageByCurrencyViewModel vm)
        {
            var sendingCountries = Common.CountryCommon.GetCountriesByCurrencyAndCountry(vm.SendingCurrency, vm.SendingCountry);
            var receivingCountries = Common.CountryCommon.GetCountriesByCurrencyAndCountry(vm.ReceivingCurrency, vm.ReceivingCountry);
            var range = GetRange(vm.Range);
            TransferFeePercentage FeePercentage = new TransferFeePercentage()
            {
                AgetnId = vm.AgentId ?? 0,
                CreatedDate = DateTime.Now,

                TransferMethod = vm.TransferMethod,
                TransferType = vm.TransferType,
                Fee = vm.Fee,
                FeeType = vm.FeeType,
                OtherRange = vm.OtherRange,
                Range = range,
                TransfeFeeByCurrencyId = vm.Id,
                SendingCurrency = vm.SendingCurrency,
                ReceivingCurrency = vm.ReceivingCurrency
            };

            var history = new TransferFeePercentageHistory
            {
                TransferType = vm.TransferType,
                TransferMethod = vm.TransferMethod,
                Range = range,
                OtherRange = vm.OtherRange,
                Fee = vm.Fee,
                FeeType = vm.FeeType,
                CreatedDate = DateTime.Now,
                AgentId = vm.AgentId ?? 0,
                SendingCurrency = vm.SendingCurrency,
                ReceivingCurrency = vm.ReceivingCurrency
            };


            foreach (var sendingCountry in sendingCountries)
            {
                foreach (var receivingCountry in receivingCountries)
                {
                    FeePercentage.ReceivingCountry = receivingCountry;
                    FeePercentage.SendingCountry = sendingCountry;
                    dbContext.TransferFeePercentage.Add(FeePercentage);
                    dbContext.SaveChanges();

                    history.ReceivingCountry = receivingCountry;
                    history.SendingCountry = sendingCountry;
                    dbContext.TransferFeePercentageHistory.Add(history);
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

        internal void Update(TransferFeePercentageByCurrencyViewModel model)
        {

            var data = List().Where(x => x.Id == model.Id).FirstOrDefault();



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
            data.AgentId = model.AgentId ?? 0;

            dbContext.Entry(data).State = EntityState.Modified;
            dbContext.SaveChanges();

            var ListofTransferFeePercentage = dbContext.TransferFeePercentage.Where(x => x.TransfeFeeByCurrencyId == data.Id).ToList();
            dbContext.TransferFeePercentage.RemoveRange(ListofTransferFeePercentage);
            dbContext.SaveChanges();

            AddTransferFeePercentageAndHisotry(model);
        }

        internal TransferFeePercentageByCurrencyViewModel GetTranferFee(int id)
        {
            var data = List().Where(x => x.Id == id);
            TransferFeePercentageByCurrencyViewModel result = (from c in data
                                                               select new TransferFeePercentageByCurrencyViewModel()
                                                               {
                                                                   Id = c.Id,
                                                                   AgentId = c.AgentId,
                                                                   CreatedDate = c.CreatedDate,
                                                                   Fee = c.Fee,
                                                                   FeeType = c.FeeType,
                                                                   Range = Common.Common.GetRangeName(c.FromRange + "-" + c.ToRange),
                                                                   TransferMethod = c.TransferMethod,
                                                                   TransferType = c.TransferType,
                                                                   ReceivingCurrency = c.ReceivingCurrency,
                                                                   SendingCurrency = c.SendingCurrency,
                                                                   FromRange = c.FromRange,
                                                                   ToRange = c.ToRange,
                                                                   OtherRange = c.OtherRange,
                                                                   ReceivingCountry = c.ReceivingCountry,
                                                                   SendingCountry = c.SendingCountry
                                                               }).FirstOrDefault();
            return result;
        }

        internal void Delete(int id)
        {
            var data = List().Where(x => x.Id == id).FirstOrDefault();
            dbContext.TransferFeePercentageByCurrency.Remove(data);
            dbContext.SaveChanges();


            var ListofTransferFeePercentage = dbContext.TransferFeePercentage.Where(x => x.TransfeFeeByCurrencyId == id).ToList();
            dbContext.TransferFeePercentage.RemoveRange(ListofTransferFeePercentage);
            dbContext.SaveChanges();

        }

        private List<string> GetListofRange(string sendingCurrency, string receivingCurrency, TransactionTransferMethod transferMethod, TransactionTransferType transferType)
        {
            var data = dbContext.TransferFeePercentageByCurrency.Where(x => x.SendingCurrency == sendingCurrency && x.ReceivingCurrency == receivingCurrency &&
            x.TransferMethod == transferMethod && x.TransferType == transferType).ToList();

            List<string> RangeList = new List<string>();

            foreach (var item in data.OrderBy(x => x.FromRange))
            {
                var range = Common.Common.GetRangeName(item.FromRange + "-" + item.ToRange);
                RangeList.Add(range);
            }

            return RangeList;
        }
    }
}