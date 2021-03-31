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
    public class STransferExchangeRateServices
    {
        DB.FAXEREntities dbContext = null;
        public STransferExchangeRateServices()
        {
            dbContext = new DB.FAXEREntities();
        }
        public ServiceResult<IQueryable<ExchangeRate>> List()
        {
            return new ServiceResult<IQueryable<ExchangeRate>>()
            {
                Data = dbContext.ExchangeRate,
                Status = ResultStatus.OK
            };
        }

        public ServiceResult<List<ExchangeRateSettingViewModel>> GetExchangeRate(int StaffId = 0, string SendingCountry = "", string ReceivingCountry = "", int TransferType = 0, int Agent = 0, int transferMethod = 0)
        {
            IQueryable<ExchangeRate> data = List().Data;
            if (!string.IsNullOrEmpty(SendingCountry))
            {
                data = data.Where(x => x.CountryCode1 == SendingCountry);
            }
            if (!string.IsNullOrEmpty(ReceivingCountry))
            {
                data = data.Where(x => x.CountryCode2 == ReceivingCountry);
            }
            if (TransferType != 0)
            {
                data = data.Where(x => x.TransferType == (TransactionTransferType)TransferType);
            }
            if (transferMethod != 0)
            {
                data = data.Where(x => x.TransferMethod == (TransactionTransferMethod)transferMethod);
            }
            if (Agent != 0)
            {
                data = data.Where(x => x.AgentId == Agent);
            }

            #region old design

            //var result = data.GroupBy(x => new
            //{
            //    x.CountryCode1,
            //    x.CountryCode2,
            //    x.AgentId,
            //    x.TransferType
            //}).Select(d => new ExchangeRateSettingViewModel()
            //{
            //    Id = d.FirstOrDefault().Id,
            //    AgentId = d.FirstOrDefault().AgentId,
            //    SourceCountryName = d.FirstOrDefault().CountryCode1,
            //    DestinationCountryName = d.FirstOrDefault().CountryCode2,
            //    TransferType = d.FirstOrDefault().TransferType,
            //    BankDeposit = d.Where(x => x.TransferMethod == TransactionTransferMethod.BankDeposit && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
            //    KiiPayWallet = d.Where(x => x.TransferMethod == TransactionTransferMethod.KiiPayWallet && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
            //    OtherWallet = d.Where(x => x.TransferMethod == TransactionTransferMethod.OtherWallet && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
            //    CashPickUp = d.Where(x => x.TransferMethod == TransactionTransferMethod.CashPickUp && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
            //    ServicePayment = d.Where(x => x.TransferMethod == TransactionTransferMethod.ServicePayment && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
            //    BillPayment = d.Where(x => x.TransferMethod == TransactionTransferMethod.BillPayment && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
            //}).ToList();
            //foreach (var item in result)
            //{
            //    item.AgentName = dbContext.AgentInformation.Where(x => x.Id == item.AgentId).Select(x => x.Name).FirstOrDefault();
            //    item.SourceCountryName = Common.Common.GetCountryName(item.SourceCountryName);
            //    item.DestinationCountryName = Common.Common.GetCountryName(item.DestinationCountryName);
            //}
            #endregion

            var result = (from c in data.ToList()
                          join a in dbContext.AgentInformation on c.AgentId equals a.Id into agent
                          from a in agent.DefaultIfEmpty()
                          select new ExchangeRateSettingViewModel()
                          {
                              Id = c.Id,
                              Range = Common.Common.GetRangeName(c.FromRange + "-" + c.ToRange),
                              Rate = Math.Round(c.Rate, 2),
                              SourceCountryName = Common.Common.GetCountryName(c.CountryCode1),
                              SourceCountryNameLower = c.CountryCode1.ToLower(),
                              DestinationCountryNameLower = c.CountryCode2.ToLower(),
                              DestinationCountryName = Common.Common.GetCountryName(c.CountryCode2),
                              TransferMethodName = Enum.GetName(typeof(TransactionTransferMethod), c.TransferMethod),
                              AgentName = a == null ? "" : a.Name,
                              DestinationCurrencyCode = c.RecevingCurrency,
                              SourceCurrencyCode = c.SendingCurrency
                          }).ToList();
            return new ServiceResult<List<ExchangeRateSettingViewModel>>()
            {
                Data = result,
                Status = ResultStatus.OK
            };
        }

        public ExchangeRateSettingViewModel GetExchangeRateById(int id)
        {
            var ExchangeRate = dbContext.ExchangeRate.Where(x => x.Id == id).FirstOrDefault();
            var vm = new ExchangeRateSettingViewModel()
            {
                CreatedDate = ExchangeRate.CreatedDate,
                TransferType = ExchangeRate.TransferType,
                TransferMethod = ExchangeRate.TransferMethod,
                DestinationCountryCode = ExchangeRate.CountryCode2,
                SourceCountryCode = ExchangeRate.CountryCode1,
                ExchangeRate = ExchangeRate.Rate,
                CreatedBy = ExchangeRate.CreatedBy,
                AgentId = ExchangeRate.AgentId,
                Range = Common.Common.GetRangeName(ExchangeRate.FromRange + "-" + ExchangeRate.ToRange),
                DestinationCurrencyCode = ExchangeRate.RecevingCurrency,
                SourceCurrencyCode = ExchangeRate.SendingCurrency
            };
            return vm;
        }
        //public ExchangeRateSettingViewModel GetExchangeRateById(int id)
        //{
        //    var exchangeRate = List().Data.Where(x => x.Id == id).FirstOrDefault();
        //    string sendingCountry = "";
        //    string receivingCountry = "";

        //    if (exchangeRate.TransferFeeByCurrencyId > 0)
        //    {
        //        var TransferExchangeRateByCurrency = dbContext.TransferExchangeRateByCurrency.Where(x => x.Id == exchangeRate.TransferFeeByCurrencyId).FirstOrDefault();
        //        sendingCountry = TransferExchangeRateByCurrency.SendingCountry;
        //        receivingCountry = TransferExchangeRateByCurrency.ReceivingCountry;
        //    }
        //    else
        //    {
        //        sendingCountry = exchangeRate.CountryCode1;
        //        receivingCountry = exchangeRate.CountryCode2;
        //    }
        //    ExchangeRateSettingViewModel vm = new ExchangeRateSettingViewModel()
        //    {
        //        Id = exchangeRate.Id,
        //        AgentId = exchangeRate.AgentId,
        //        SourceCountryCode = sendingCountry,
        //        SourceCurrencyCode = exchangeRate.SendingCurrency,
        //        DestinationCurrencyCode = exchangeRate.RecevingCurrency,
        //        DestinationCountryCode = receivingCountry,
        //        TransferMethod = exchangeRate.TransferMethod,
        //        TransferMethodName = Common.Common.GetEnumDescription(exchangeRate.TransferMethod),
        //        Rate = exchangeRate.Rate,
        //        CreatedDate = exchangeRate.CreatedDate,
        //        ExchangeRate = exchangeRate.Rate,
        //        Range = Common.Common.GetRangeName(exchangeRate.FromRange + "-" + exchangeRate.ToRange),
        //        TransferFeeByCurrencyId = exchangeRate.TransferFeeByCurrencyId,
        //        TransferType = exchangeRate.TransferType
        //    };
        //    return vm;
        //}

        public ExchangeRateSettingViewModel GetRate(string sendingCurrency = "", string receivingCurrecncy = "", string sendingCountry = "",
            string receivingCounrty = "", int transferType = 0, int method = 0, int agent = 0, string range = "")
        {
            decimal fromRange = 0m;
            decimal toRange = 0m;

            if (!string.IsNullOrEmpty(range))
            {
                var splittedRange = range.Split('-');
                fromRange = splittedRange[0].ToDecimal();
                toRange = splittedRange[1].ToDecimal();
            }

            var exchangerates = dbContext.ExchangeRate.Where(x => x.SendingCurrency == sendingCurrency && x.RecevingCurrency == receivingCurrecncy &&
              x.CountryCode1 == sendingCountry && x.CountryCode2 == receivingCounrty && x.TransferType == (TransactionTransferType)transferType
              && x.TransferMethod == (TransactionTransferMethod)method && x.FromRange == fromRange && x.ToRange == toRange);

            var exchangeRate = exchangerates.FirstOrDefault();
            if (transferType == 2 || transferType == 4)
            {
                exchangeRate = exchangerates.Where(x => x.AgentId == agent).FirstOrDefault();
            }
            if (exchangeRate != null)
            {
                var exchageRateVm = new ExchangeRateSettingViewModel()
                {
                    Id = exchangeRate.Id,
                    SourceCurrencyCode = exchangeRate.SendingCurrency,
                    DestinationCurrencyCode = exchangeRate.RecevingCurrency,
                    AgentId = exchangeRate.AgentId,
                    ExchangeRate = exchangeRate.Rate,
                    Rate = exchangeRate.Rate,
                    Range = Common.Common.GetRangeName(exchangeRate.FromRange + "-" + exchangeRate.ToRange),
                    TransferMethod = exchangeRate.TransferMethod,
                    TransferType = exchangeRate.TransferType,
                    SourceCountryCode = exchangeRate.CountryCode1,
                    DestinationCountryCode = exchangeRate.CountryCode2,
                    TransferFeeByCurrencyId = exchangeRate.TransferFeeByCurrencyId
                };
                return exchageRateVm;
            }
            else
            {
                return null;
            }
        }

        public ExchangeRateSettingViewModel AddExchangeRate(ExchangeRateSettingViewModel model)
        {
            GetFromAndToRange(model);
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;
            model.CreatedBy = StaffId.ToString();
            //AddTransferExchangeRateByCurrency(model);
            AddExchangeRateAndExchangeRateHistory(model);
            return model;
        }

        private void AddTransferExchangeRateByCurrency(ExchangeRateSettingViewModel model)
        {
            TransferExchangeRateByCurrency transferExchangeRateByCurrency = new TransferExchangeRateByCurrency()
            {
                CreatedBy = model.CreatedBy.ToInt(),
                AgentId = model.AgentId ?? 0,
                CreatedDate = DateTime.Now,
                Range = model.Range,
                Rate = model.ExchangeRate,
                ReceivingCurrency = model.DestinationCurrencyCode,
                SendingCurrency = model.SourceCurrencyCode,
                TransferMethod = model.TransferMethod,
                TransferType = model.TransferType,
                ToRange = model.FromRange,
                FromRange = model.ToRange,
                SendingCountry = model.SourceCountryCode,
                ReceivingCountry = model.DestinationCountryCode
            };
            dbContext.TransferExchangeRateByCurrency.Add(transferExchangeRateByCurrency);
            dbContext.SaveChanges();
            model.TransferFeeByCurrencyId = transferExchangeRateByCurrency.Id;
        }

        public ServiceResult<List<TransferExchangerateHistoryViewModel>> GetExchangeRateHistory(string SendingCountry = "", string ReceivingCountry = "", int TransferType = 0, int Agent = 0, int Year = 0, int Month = 0, int Day = 0)
        {
            IQueryable<TransferExchangeRateHistory> data = dbContext.TransferExchangeRateHistory;
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
                data = data.Where(x => x.TransferType == (TransactionTransferType)TransferType);
            }
            if (Agent != 0)
            {
                data = data.Where(x => x.AgentId == Agent);
            }
            if (Year != 0)
            {
                data = data.Where(x => x.CreatedDate.Year == Year);
            }
            if (Month != 0)
            {
                data = data.Where(x => x.CreatedDate.Month == Month);
            }
            if (Day != 0)
            {
                data = data.Where(x => x.CreatedDate.Day == Day);
            }

            #region old Design
            //var result = data.GroupBy(x => new
            //{
            //    x.SendingCountry,
            //    x.ReceivingCountry,
            //    x.AgentId,
            //    x.TransferType
            //}).Select(d => new TransferExchangerateHistoryViewModel()
            //{
            //    Id = d.FirstOrDefault().Id,
            //    AgentId = d.FirstOrDefault().AgentId,
            //    SendingCountry = d.FirstOrDefault().SendingCountry,
            //    ReceivingCountry = d.FirstOrDefault().ReceivingCountry,
            //    TransferType = d.FirstOrDefault().TransferType,
            //    BankDeposit = d.Where(x => x.TransferMethod == TransactionTransferMethod.BankDeposit && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
            //    KiiPayWallet = d.Where(x => x.TransferMethod == TransactionTransferMethod.KiiPayWallet && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
            //    OtherWallet = d.Where(x => x.TransferMethod == TransactionTransferMethod.OtherWallet && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
            //    CashPickUp = d.Where(x => x.TransferMethod == TransactionTransferMethod.CashPickUp && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
            //    ServicePayment = d.Where(x => x.TransferMethod == TransactionTransferMethod.ServicePayment && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
            //    BillPayment = d.Where(x => x.TransferMethod == TransactionTransferMethod.BillPayment && x.TransferType == d.FirstOrDefault().TransferType).Select(x => x.Rate).FirstOrDefault(),
            //    CreatedDate = d.FirstOrDefault().CreatedDate.ToString("dd-MM-yyyy"),
            //}).ToList();
            //foreach (var item in result)
            //{
            //    item.AgentName = dbContext.AgentInformation.Where(x => x.Id == item.AgentId).Select(x => x.Name).FirstOrDefault();
            //    item.SendingCountry = Common.Common.GetCountryName(item.SendingCountry);
            //    item.ReceivingCountry = Common.Common.GetCountryName(item.ReceivingCountry);
            //}

            #endregion

            var result = (from c in data.ToList()
                          select new TransferExchangerateHistoryViewModel()
                          {
                              Id = c.Id,
                              Range = Common.Common.GetRangeName(c.FromRange + "-" + c.ToRange),
                              Rate = Math.Round(c.Rate, 2),
                              SendingCountry = Common.Common.GetCountryName(c.SendingCountry),
                              SendingCountryFlag = c.SendingCountry.ToLower(),
                              ReceivingCountryFlag = c.ReceivingCountry.ToLower(),
                              ReceivingCountry = Common.Common.GetCountryName(c.ReceivingCountry),
                              TransferMethodName = Enum.GetName(typeof(TransactionTransferMethod), c.TransferMethod),
                              AgentName = dbContext.AgentInformation.Where(x => x.Id == c.AgentId).Select(x => x.Name).FirstOrDefault(),
                              CreatedDate = c.CreatedDate.ToString("dd-MM-yyyy"),
                              ReceivingCurrency = c.ReceivingCurrency,
                              SendingCurrency = c.SendingCurrency
                          }).ToList();

            return new ServiceResult<List<TransferExchangerateHistoryViewModel>>()
            {
                Data = result,
                Status = ResultStatus.OK
            };
        }


        public void UpdateExchangeRate(ExchangeRateSettingViewModel vm)
        {
            GetFromAndToRange(vm);
            vm.CreatedBy = Common.StaffSession.LoggedStaff.StaffId.ToString();
            var exchangeRate = dbContext.ExchangeRate.Where(x => x.Id == vm.Id).FirstOrDefault();


            //var data = dbContext.TransferExchangeRateByCurrency.Where(x => x.Id == exchangeRate.TransferFeeByCurrencyId).FirstOrDefault();
            //if (data != null)
            //{
            //    data.FromRange = vm.FromRange;
            //    data.Range = vm.Range;
            //    data.Rate = vm.ExchangeRate;
            //    data.ReceivingCurrency = vm.DestinationCurrencyCode;
            //    data.SendingCurrency = vm.SourceCurrencyCode;
            //    data.ToRange = vm.ToRange;
            //    data.TransferMethod = vm.TransferMethod;
            //    data.TransferType = vm.TransferType;
            //    data.SendingCountry = vm.SourceCountryCode;
            //    data.ReceivingCountry = vm.DestinationCountryCode;
            //    data.AgentId = vm.AgentId ?? 0;
            //    dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            //    dbContext.SaveChanges();
            //}
            //else
            //{
            //    AddTransferExchangeRateByCurrency(vm);
            //}

            exchangeRate.AgentId = vm.AgentId ?? 0;
            exchangeRate.CreatedDate = DateTime.Now;
            exchangeRate.TransferMethod = vm.TransferMethod;
            exchangeRate.Rate = vm.ExchangeRate;
            exchangeRate.ToRange = vm.ToRange;
            exchangeRate.FromRange = vm.FromRange;
            exchangeRate.CreatedBy = vm.CreatedBy;
            exchangeRate.TransferType = vm.TransferType;
            exchangeRate.CountryCode1 = vm.SourceCountryCode;
            exchangeRate.CountryCode2 = vm.DestinationCountryCode;
            exchangeRate.SendingCurrency = vm.SourceCurrencyCode;
            exchangeRate.RecevingCurrency = vm.DestinationCurrencyCode;
            dbContext.Entry(exchangeRate).State = EntityState.Modified;
            dbContext.SaveChanges();
            AddExchangeRateHistory(vm);
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
        private void AddExchangeRateAndExchangeRateHistory(ExchangeRateSettingViewModel vm)
        {
            ExchangeRate ExchangeRate = new ExchangeRate()
            {
                AgentId = vm.AgentId ?? 0,
                CreatedDate = DateTime.Now,
                TransferMethod = vm.TransferMethod,
                Rate = vm.ExchangeRate,
                ToRange = vm.ToRange,
                FromRange = vm.FromRange,
                CreatedBy = vm.CreatedBy,
                TransferType = vm.TransferType,
                CountryCode1 = vm.SourceCountryCode,
                CountryCode2 = vm.DestinationCountryCode,
                SendingCurrency = vm.SourceCurrencyCode,
                RecevingCurrency = vm.DestinationCurrencyCode,
            };
            dbContext.ExchangeRate.Add(ExchangeRate);
            dbContext.SaveChanges();
            AddExchangeRateHistory(vm);
        }

        private void AddExchangeRateHistory(ExchangeRateSettingViewModel vm)
        {
            TransferExchangeRateHistory hist = new TransferExchangeRateHistory()
            {
                AgentId = vm.AgentId ?? 0,
                CreatedDate = DateTime.Now,
                TransferMethod = vm.TransferMethod,
                Rate = vm.ExchangeRate,
                ToRange = vm.ToRange,
                FromRange = vm.FromRange,
                CreatedBy = vm.CreatedBy,
                TransferType = vm.TransferType,
                ReceivingCurrency = vm.DestinationCurrencyCode,
                SendingCurrency = vm.SourceCurrencyCode,
                SendingCountry = vm.SourceCountryCode,
                ReceivingCountry = vm.DestinationCountryCode,
            };
            dbContext.TransferExchangeRateHistory.Add(hist);
            dbContext.SaveChanges();


        }

        private void RemoveRangeExchangeRate(int transferFeeByCurrencyId)
        {
            var ListOfExchangeRate = dbContext.ExchangeRate.Where(x => x.TransferFeeByCurrencyId == transferFeeByCurrencyId).ToList();
            dbContext.ExchangeRate.RemoveRange(ListOfExchangeRate);
            dbContext.SaveChanges();
        }
        internal void RemoveExchangeRate(int Id)
        {
            var exchangeRate = dbContext.ExchangeRate.Where(x => x.Id == Id).FirstOrDefault();
            dbContext.ExchangeRate.Remove(exchangeRate);
            dbContext.SaveChanges();
        }
    }
}