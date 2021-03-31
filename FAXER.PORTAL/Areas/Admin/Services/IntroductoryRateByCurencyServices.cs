using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class IntroductoryRateByCurencyServices
    {
        DB.FAXEREntities dbContext = null;
        public IntroductoryRateByCurencyServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<IntroductoryRateByCurrency> List()
        {
            var data = dbContext.IntroductoryRateByCurrency.ToList();
            return data;
        }
        public List<IntroductoryRateByCurencyViewModel> GetIntroductoryRateList(string sendingCurrency, string receivingCurrency, int transferType, int agent,int transferMethod)
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
                data = data.Where(x => x.TransactionTransferType == (TransactionTransferType)transferType).ToList();
            }
            if (agent != 0)
            {
                data = data.Where(x => x.AgentId == agent).ToList();
            }
            if (transferMethod != 0)
            {
                data = data.Where(x => x.TransactionTransferMethod == (TransactionTransferMethod)transferMethod).ToList();
            }




            var result = data.GroupBy(x => new
            {
                x.SendingCurrency,
                x.ReceivingCurrency,
                x.TransactionTransferMethod,
                x.TransactionTransferType,
            }).Select(c => new IntroductoryRateByCurencyViewModel()
            {
                Id = c.FirstOrDefault().Id,
                //Range = c.FirstOrDefault().FromRange + "-" + c.FirstOrDefault().ToRange,
                Rate = c.FirstOrDefault().Rate,
                ReceivingCurrency = c.FirstOrDefault().ReceivingCurrency,
                SendingCurrency = c.FirstOrDefault().SendingCurrency,
                TransactionTransferMethodName = c.FirstOrDefault().TransactionTransferMethod.ToString(),
                TransactionTransferMethod = c.FirstOrDefault().TransactionTransferMethod,
                AgentName = c.FirstOrDefault().AgentId != 0 ? getAgentName(c.FirstOrDefault().AgentId) : "All",
                Range = c.FirstOrDefault().Range,
                RangeList = GetListofRange(c.FirstOrDefault().ReceivingCurrency, c.FirstOrDefault().SendingCurrency, c.FirstOrDefault().TransactionTransferMethod
                , c.FirstOrDefault().TransactionTransferType),
                SendingCountryFlag = _commonServices.getCountryCodeFromCurrency(c.FirstOrDefault().SendingCurrency).ToLower(),
                ReceivingCountryFlag = _commonServices.getCountryCodeFromCurrency(c.FirstOrDefault().ReceivingCurrency).ToLower(),


            }).ToList();

            return result;
        }

        internal IntroductoryRateByCurencyViewModel GetIntroductoryRate(int id)
        {
            var data = List().Where(x => x.Id == id).ToList();
            var result = (from c in data
                          select new IntroductoryRateByCurencyViewModel()
                          {
                              NoOfTransaction = c.NoOfTransaction,
                              AgentId = c.AgentId,
                              CreatedBy = c.CreatedBy,
                              CreatedDate = c.CreatedDate,
                              Id = c.Id,
                              Range = c.Range,
                              FromRange = c.FromRange,
                              Rate = c.Rate,
                              ReceivingCurrency = c.ReceivingCurrency,
                              SendingCurrency = c.SendingCurrency,
                              ToRange = c.ToRange,
                              TransactionTransferMethod = c.TransactionTransferMethod,
                              TransactionTransferType = c.TransactionTransferType,
                              TransactionTransferMethodName = c.TransactionTransferMethod.ToString()

                          }).FirstOrDefault();

            return result;
        }

        public string getAgentName(int id)
        {
            string data = dbContext.AgentInformation.Where(x => x.Id == id).Select(x => x.Name).FirstOrDefault();
            return data;
        }
        public IQueryable<IntroductoryRateByCurrency> GetRates()
        {

            return dbContext.IntroductoryRateByCurrency;
        }
        internal void UpdateIntroductoryRate(IntroductoryRateByCurencyViewModel vm)
        {
            var data = List().Where(x => x.Id == vm.Id).FirstOrDefault();
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
            data.SendingCurrency = vm.SendingCurrency;
            data.ReceivingCurrency = vm.ReceivingCurrency;
            data.AgentId = vm.AgentId ?? 0;
            data.FromRange = FromRange;
            data.ToRange = ToRange;
            data.Rate = vm.Rate;
            data.TransactionTransferMethod = vm.TransactionTransferMethod;
            data.TransactionTransferType = vm.TransactionTransferType;
            data.NoOfTransaction = vm.NoOfTransaction;
            data.CreatedBy = Common.AdminSession.StaffId;
            data.CreatedDate = data.CreatedDate;




            dbContext.Entry<IntroductoryRateByCurrency>(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

            var ListOfIntroducoryRate = dbContext.IntroductoryRate.Where(x => x.IntroductoryRateByCurrencyId == data.Id).ToList();
            dbContext.IntroductoryRate.RemoveRange(ListOfIntroducoryRate);
            dbContext.SaveChanges();
            var sendingCountries = Common.Common.GetCountriesByCurrency(vm.SendingCurrency);
            var ReceningCountries = Common.Common.GetCountriesByCurrency(vm.ReceivingCurrency);

            int StaffId = Common.StaffSession.LoggedStaff.StaffId;

            foreach (var item in sendingCountries)
            {
                foreach (var receivingCountry in ReceningCountries)
                {
                    IntroductoryRate introductoryRate = new IntroductoryRate()
                    {
                        AgentId = vm.AgentId ?? 0,
                        CreatedDate = DateTime.Now,
                        FromRange = FromRange,
                        NoOfTransaction = vm.NoOfTransaction,
                        Rate = vm.Rate,
                        ReceivingCountry = Common.Common.GetCountryCodeByCurrency(vm.ReceivingCurrency),
                        CreatedBy = StaffId,
                        TransactionTransferType = vm.TransactionTransferType,
                        TransactionTransferMethod = vm.TransactionTransferMethod,
                        ToRange = ToRange,
                        SendingCountry = item,
                        IntroductoryRateByCurrencyId = data.Id,

                    };
                    dbContext.IntroductoryRate.Add(introductoryRate);
                    dbContext.SaveChanges();

                }
            }
        }

        internal void Delete(int id)
        {
            var data = List().Where(x => x.Id == id).FirstOrDefault();
            dbContext.IntroductoryRateByCurrency.Remove(data);
            dbContext.SaveChanges();


            var ListOfIntroducoryRate = dbContext.IntroductoryRate.Where(x => x.IntroductoryRateByCurrencyId == id).ToList();
            dbContext.IntroductoryRate.RemoveRange(ListOfIntroducoryRate);
            dbContext.SaveChanges();

        }

        internal void AddIntroductoryRate(IntroductoryRateByCurencyViewModel vm)
        {
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;

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

            IntroductoryRateByCurrency model = new IntroductoryRateByCurrency()
            {
                AgentId = vm.AgentId ?? 0,
                CreatedBy = StaffId,
                CreatedDate = DateTime.Now,
                NoOfTransaction = vm.NoOfTransaction,
                Range = vm.Range,
                Rate = vm.Rate,
                ReceivingCurrency = vm.ReceivingCurrency,
                SendingCurrency = vm.SendingCurrency,
                TransactionTransferMethod = vm.TransactionTransferMethod,
                TransactionTransferType = vm.TransactionTransferType,
                FromRange = FromRange,
                ToRange = ToRange
            };
            var data = dbContext.IntroductoryRateByCurrency.Add(model);
            dbContext.SaveChanges();

            var sendingCountries = Common.Common.GetCountriesByCurrency(vm.SendingCurrency);
            var ReceningCountries = Common.Common.GetCountriesByCurrency(vm.ReceivingCurrency);

            foreach (var item in sendingCountries)
            {
                foreach (var receivingCountry in ReceningCountries)
                {
                    IntroductoryRate introductoryRate = new IntroductoryRate()
                    {
                        AgentId = vm.AgentId ?? 0,
                        CreatedDate = DateTime.Now,
                        FromRange = FromRange,
                        NoOfTransaction = vm.NoOfTransaction,
                        Rate = vm.Rate,
                        ReceivingCountry = receivingCountry,
                        CreatedBy = StaffId,
                        TransactionTransferType = vm.TransactionTransferType,
                        TransactionTransferMethod = vm.TransactionTransferMethod,
                        ToRange = ToRange,
                        SendingCountry = item,
                        IntroductoryRateByCurrencyId = data.Id,

                    };
                    dbContext.IntroductoryRate.Add(introductoryRate);
                    dbContext.SaveChanges();

                }
            }
        }

        public List<string> GetListofRange(string ReceivingCurrency, string SendingCurrency, TransactionTransferMethod transactionTransferMethod, TransactionTransferType transactionTransferType)
        {
            var data = List().Where(x => x.SendingCurrency == SendingCurrency && x.ReceivingCurrency == ReceivingCurrency &&
            x.TransactionTransferMethod == transactionTransferMethod && x.TransactionTransferType == transactionTransferType).ToList();

            List<string> RangeList = new List<string>();
            foreach (var item in data.OrderBy(x => x.FromRange))
            {
                RangeList.Add(item.FromRange + "-" + item.ToRange);
            }

            return RangeList;
        }


    }
}