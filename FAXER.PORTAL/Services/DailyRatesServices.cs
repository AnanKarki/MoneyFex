using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class DailyRatesServices
    {
        DB.FAXEREntities dbContext = null;
        public DailyRatesServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<DailyRatesViewModel> GetDailyRates(string date = "")
        {
            var ExchangeRate = dbContext.ExchangeRate.ToList();
            var introductoryRate = dbContext.IntroductoryRate.ToList();

            List<IntroductoryRate> IntroductoryRateList = new List<IntroductoryRate>();
            List<ExchangeRate> ExchangeRateList = new List<ExchangeRate>();
            List<DailyRatesViewModel> result = new List<DailyRatesViewModel>();
            foreach (var item in ExchangeRate)
            {
                var introRate = introductoryRate.Where(x => x.SendingCountry == item.CountryCode1 && x.ReceivingCountry == item.CountryCode2).FirstOrDefault();
                if (introRate != null)
                {
                    IntroductoryRateList.Add(introRate);
                }
                else
                {
                    ExchangeRateList.Add(item);
                }
            }

            if (!string.IsNullOrEmpty(date))
            {
                var DateRange = date.Split('-');
                var dtTo = DateTime.Parse(DateRange[1]);
                var dtFrom = DateTime.Parse(DateRange[0]);
                IntroductoryRateList = IntroductoryRateList.Where(x => x.CreatedDate> dtFrom && x.CreatedDate < dtTo).ToList();
                ExchangeRateList = ExchangeRateList.Where(x => x.CreatedDate > dtFrom && x.CreatedDate <dtTo).ToList();

            }

            List<DailyRatesViewModel> data1 = (from c in IntroductoryRateList
                                               select new DailyRatesViewModel()
                                               {
                                                   Id = c.Id,
                                                   ReceivingAmount = Math.Round(c.Rate, 2),
                                                   ReceivingCountry = c.ReceivingCountry,
                                                   ReceivingCurrency = Common.Common.GetCurrencyCode(c.ReceivingCountry),
                                                   SendingCurrency = Common.Common.GetCurrencyCode(c.SendingCountry),
                                                   SendingCountry = c.SendingCountry,
                                                   Date = c.CreatedDate,
                                                   ShrotDate = c.CreatedDate.ToShortDateString(),
                                                   ReceivingCountryLower = c.ReceivingCountry.ToLower(),
                                                   SendingCountryLower = c.SendingCountry.ToLower()
                                               }).ToList();

            List<DailyRatesViewModel> data2 = (from c in ExchangeRateList
                                               select new DailyRatesViewModel()
                                               {
                                                   Id = c.Id,
                                                   ReceivingAmount = Math.Round(c.Rate, 2),
                                                   ReceivingCountry = c.CountryCode2,
                                                   ReceivingCurrency = Common.Common.GetCurrencyCode(c.CountryCode2),
                                                   SendingCurrency = Common.Common.GetCurrencyCode(c.CountryCode1),
                                                   SendingCountry = c.CountryCode1,
                                                   Date = c.CreatedDate,
                                                   ShrotDate = c.CreatedDate.ToString(),
                                                   ReceivingCountryLower = c.CountryCode2.ToLower(),
                                                   SendingCountryLower = c.CountryCode1.ToLower()
                                               }).ToList();




            return data1.Concat(data2).ToList();
        }
    }
}