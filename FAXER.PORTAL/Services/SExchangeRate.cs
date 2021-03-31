using FAXER.PORTAL.Areas.Mobile.Models.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public static class SExchangeRate
    {
        static DB.FAXEREntities dbContext = null;
        static decimal SendingAmount = 0;


        public static DB.ExchangeRate GetExchangeRate(ExchangeRateRequestParam requestParam) {

            IQueryable<ExchangeRate> rate_query = (from c in dbContext.ExchangeRate.
                                                  Where(x => x.CountryCode1 == requestParam.SendingCountry
                                                  && x.CountryCode2 == requestParam.ReceivingCountry)
                                                   select c);
            rate_query = filterExchangeRateByType(rate_query, requestParam.TransferType);
            rate_query = filterExchangeRateByMethod(rate_query, requestParam.TransactionMethod);
            if (requestParam.AgentId > 0)
            {
                rate_query = filterExchangeRateByAgent(rate_query, requestParam.AgentId);
            }
            if (SendingAmount > 0)
            {
                rate_query = filterExchangeRateByRange(rate_query, SendingAmount);
            }
            return rate_query.FirstOrDefault();
        }

        private static IQueryable<ExchangeRate> filterExchangeRateByType(IQueryable<ExchangeRate> exchangeRates , TransactionTransferType transferType ) {

            var data = exchangeRates.Where(x => x.TransferType == transferType);
            if (data.Count() == 0) { 
                data= exchangeRates.Where(x => x.TransferType == TransactionTransferType.All);
                if (data.Count() == 0) {
                    return exchangeRates;
                }
            };
            return data;
        }
        private static IQueryable<ExchangeRate> filterExchangeRateByMethod(IQueryable<ExchangeRate> exchangeRates, TransactionTransferMethod transactMethod)
        {
            var data = exchangeRates.Where(x => x.TransferMethod == transactMethod);
            if (data.Count() == 0)
            {
                data = exchangeRates.Where(x => x.TransferMethod == TransactionTransferMethod.All);
                if (data.Count() == 0)
                {
                    return exchangeRates;
                }
            };
            return data;
        }

        private static IQueryable<ExchangeRate> filterExchangeRateByAgent(IQueryable<ExchangeRate> exchangeRates, int agentId)
        {
            var data = exchangeRates.Where(x => x.AgentId == agentId);
            if (data.Count() == 0)
            {
                return exchangeRates;
            };
            return data;
        }

        private static IQueryable<ExchangeRate> filterExchangeRateByRange(IQueryable<ExchangeRate> exchangeRates, decimal Amount)
        {

            var data = exchangeRates.Where(x => x.FromRange >= Amount && x.ToRange <= Amount);
            if (data.Count() == 0) {
                // this is all condition if 
                // exchange has been set for all range
                data = exchangeRates.Where(x => x.FromRange == 0 && x.ToRange == 0);
                if (data.Count() == 0)
                {
                    data = GetRatesByRanges(exchangeRates, Amount);
                }
                return data;
            }
            return data;
        }
        private static IQueryable<ExchangeRate> GetRatesByRanges(IQueryable<ExchangeRate> exchangeRates , decimal Amount)
        {
            var ranges = GetRangeArray();
            foreach (var range in ranges)
            {
                decimal from = 0;
                decimal to = 0;
                string[] range_values = range.Split('-');
                decimal.TryParse(range_values[0], out from);
                decimal.TryParse(range_values[1], out to);
                IQueryable<ExchangeRate> range_rate_query;
                if (to > 0)
                {
                    range_rate_query = exchangeRates.Where(x => x.FromRange >= to && x.FromRange <= to);
                
                }
                else {
                    range_rate_query = exchangeRates.Where(x => x.FromRange >= from);
                }

                if (range_rate_query.Count() > 0) {
                    return range_rate_query;
                    //break;
                }

            }
            return exchangeRates;
            
        }
        private static string[] GetRangeArray() {
            string[] ranges = new string[9];
            ranges[0] = "1-100";
            ranges[1] = "101-500";
            ranges[2] = "501-1000";
            ranges[3] = "1001-1500";
            ranges[4] = "1501-2000";
            ranges[5] = "2001-3000";
            ranges[6] = "3001-5000";
            ranges[7] = "5001-10000";
            ranges[8] = "11000";
            return ranges;
        }


        public static DB.ExchangeRate GetExchangeRate(string SendingCountry, string ReceivingCountry,
            TransactionTransferMethod TransactionTransferMethod, int agentId = 0,
            TransactionTransferType transactiontransfertype = TransactionTransferType.Online, bool IsAuxAgent = false)
        {
            dbContext = new DB.FAXEREntities();



            var exchangeRateObj = dbContext.ExchangeRate.Where(x => x.CountryCode1 == SendingCountry
            && x.CountryCode2 == ReceivingCountry && x.TransferMethod == TransactionTransferMethod
            && x.AgentId == agentId && x.TransferType == transactiontransfertype).FirstOrDefault();

            if (IsAuxAgent)
            {
                if (exchangeRateObj == null)
                {
                    exchangeRateObj = dbContext.ExchangeRate.Where(x => x.CountryCode1 == SendingCountry
                    && x.CountryCode2 == ReceivingCountry && x.TransferMethod == TransactionTransferMethod.All
                    && x.AgentId == agentId && x.TransferType == transactiontransfertype).FirstOrDefault();
                    if (exchangeRateObj == null)
                    {
                        return null;
                    }
                }

            }

            if (exchangeRateObj == null)
            {
                exchangeRateObj = dbContext.ExchangeRate.Where(x => x.CountryCode1 == SendingCountry
            && x.CountryCode2 == ReceivingCountry && x.TransferMethod == TransactionTransferMethod
            && x.TransferType == transactiontransfertype).FirstOrDefault();
                if (exchangeRateObj == null)
                {

                    exchangeRateObj = dbContext.ExchangeRate.Where(x => x.CountryCode1 == SendingCountry
                    && x.CountryCode2 == ReceivingCountry
                    && x.AgentId == agentId && x.TransferType == transactiontransfertype).FirstOrDefault();

                    if (exchangeRateObj == null)
                    {
                        exchangeRateObj = dbContext.ExchangeRate.Where(x => x.CountryCode1 == SendingCountry
                        && x.CountryCode2 == ReceivingCountry && x.TransferType == transactiontransfertype).FirstOrDefault();


                        if (exchangeRateObj == null && agentId == 0)
                        {
                            exchangeRateObj = dbContext.ExchangeRate.Where(x => x.CountryCode1 == SendingCountry
                            && x.CountryCode2 == ReceivingCountry).FirstOrDefault();


                        }
                    }
                }
            }
            return exchangeRateObj;

        }

        /// <summary>
        /// param( SendingCountry , ReceivingCountry )
        /// </summary>
        /// <param name="FaxingCountryCode"></param>
        /// <param name="ReceivingCountryCode"></param>
        /// <returns></returns>
        public static decimal GetExchangeRateValue(string FaxingCountryCode, string ReceivingCountryCode,
            TransactionTransferMethod transactionTransferMethod = TransactionTransferMethod.Select, int AgentId = 0,
            TransactionTransferType transactiontransfertype = TransactionTransferType.Online, bool IsAuxAgent = false)
        {

            if (transactionTransferMethod == TransactionTransferMethod.Select)
            {

                transactionTransferMethod = TransactionTransferMethod.All;
            }
            dbContext = new DB.FAXEREntities();
            decimal exchangeRate = 0;

            //var exchangeRateObj = GetExchangeRate(FaxingCountryCode, ReceivingCountryCode, transactionTransferMethod, AgentId, transactiontransfertype, IsAuxAgent);

            var exchangeRateObj = GetExchangeRate(new ExchangeRateRequestParam()
            {
                SendingCountry = FaxingCountryCode,
                ReceivingCountry = ReceivingCountryCode,
                TransferType = transactiontransfertype,
                TransactionMethod = transactionTransferMethod,
                AgentId = AgentId,
                IsAuxAgent = IsAuxAgent
            });
            if (IsAuxAgent)
            {
                if (exchangeRateObj == null)
                {
                    return exchangeRate;
                }
            }

            if (exchangeRateObj == null)
            {
                //var exchangeRateobj2 = GetExchangeRate(ReceivingCountryCode, FaxingCountryCode, transactionTransferMethod, AgentId, transactiontransfertype);

                var exchangeRateobj2 = GetExchangeRate(new ExchangeRateRequestParam() { 
                SendingCountry = FaxingCountryCode,
                ReceivingCountry = ReceivingCountryCode,
                TransferType = transactiontransfertype,
                TransactionMethod = transactionTransferMethod,
                AgentId = AgentId
                });


                if (exchangeRateobj2 != null)
                {
                    exchangeRateObj = exchangeRateobj2;

                    exchangeRate = Math.Round(1 / exchangeRateObj.Rate, 6, MidpointRounding.AwayFromZero);
                }

            }
            else
            {
                exchangeRate = exchangeRateObj.Rate;
            }
            if (FaxingCountryCode == ReceivingCountryCode)
            {

                return 1;
            }
            return Math.Round(exchangeRate, 3);
        }

        public static List<MobileCurrencyDropDownViewModel> GetAllCurrencyExchange(string CountryCode)
        {
            dbContext = new DB.FAXEREntities();
            var data = dbContext.ExchangeRate.Where(x => x.CountryCode1.Trim().ToLower() == CountryCode.Trim().ToLower());
            if (data != null)
            {
                var result = (from c in data.ToList()
                              select new MobileCurrencyDropDownViewModel()
                              {
                                  CurrencyName = c.CountryCode1,
                                  ExchangeRate = c.CountryRate1,
                                  SendingCountryCode = c.CountryCode1,
                                  ReceivingCountryCode = c.CountryCode2,
                                  ReceivingCurrency = Common.Common.GetCountryCurrency(c.CountryCode2),
                                  SendingCurrency = Common.Common.GetCountryCurrency(c.CountryCode1),
                                  ReceivingCurrencySymbol = Common.Common.GetCountryCurrency(c.CountryCode2),
                                  SendingCurrencySymbol = Common.Common.GetCountryCurrency(c.CountryCode1),

                              }).ToList();
                return result;
            }
            return null;

        }
    }

    public class ExchangeRateRequestParam {

        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }
        public decimal Amount { get; set; }
        public TransactionTransferMethod TransactionMethod { get; set; }

        public TransactionTransferType TransferType { get; set; }

        public int AgentId { get; set; }
        public bool IsAuxAgent { get; set; }

    }
}