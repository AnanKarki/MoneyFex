using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{

    public class ExchangeRateSettingServices
    {
        FAXEREntities dbContext = null;

        public ExchangeRateSettingServices()
        {
            dbContext = new FAXEREntities();

        }




        /// <summary>
        /// this function will return the exchangerate information for Selected countries.
        /// </summary>
        /// <param name="SourceCountryCode">country Code for Source Country</param>
        /// <param name="DestinationCountryCode">country Code for Destination Country</param>
        /// <returns> ViewModels.ExchangeRateSettingViewModel</returns>
        public ViewModels.ExchangeRateSettingViewModel ShowRate(string SourceCountryCode, string DestinationCountryCode)
        {
            var sourceCountry = dbContext.Country.Where(x => x.CountryCode == SourceCountryCode).FirstOrDefault();
            var destinationCountry = dbContext.Country.Where(x => x.CountryCode == DestinationCountryCode).FirstOrDefault();
            var result = (from c in dbContext.ExchangeRate
                          where (c.CountryCode1 == SourceCountryCode || c.CountryCode1 == DestinationCountryCode) && (c.CountryCode2 == DestinationCountryCode || c.CountryCode2 == SourceCountryCode)
                          select new ViewModels.ExchangeRateSettingViewModel()
                          {
                              Id = c.Id,
                              SourceCountryCode = c.CountryCode1,
                              DestinationCountryCode = c.CountryCode2,
                              ExchangeRate = c.CountryRate1,
                              SourceCountryName = sourceCountry.CountryName,
                              DestinationCountryName = destinationCountry.CountryName,
                              DestinationCurrencyCode = destinationCountry.CountryCode,
                              SourceCurrencyCode = sourceCountry.CountryCode,

                          }
                          ).FirstOrDefault();
            if (result != null)
            {
                if (result.SourceCountryCode == DestinationCountryCode)
                {
                    result.ExchangeRate = Math.Round((1 / result.ExchangeRate), 6, MidpointRounding.AwayFromZero); ;
                    result.SourceCountryCode = sourceCountry.CountryCode;
                    result.DestinationCountryCode = destinationCountry.CountryCode;
                    result.SourceCountryName = sourceCountry.CountryName;
                    result.DestinationCountryName = destinationCountry.CountryName;
                    result.DestinationCurrencyCode = destinationCountry.Currency;
                    result.SourceCurrencyCode = sourceCountry.Currency;
                    return result;

                }
            }


            return result ?? new ViewModels.ExchangeRateSettingViewModel()
            {
                ExchangeRate = 1m,
                SourceCountryCode = sourceCountry.CountryCode,
                DestinationCountryCode = destinationCountry.CountryCode,
                SourceCountryName = sourceCountry.CountryName,
                DestinationCountryName = destinationCountry.CountryName,

                DestinationCurrencyCode = destinationCountry.Currency,
                SourceCurrencyCode = sourceCountry.Currency
            };


        }
        /// <summary>
        /// Nepal -> India
        /// 
        /// </summary>
        /// <param name="sourceCountryCode"></param>
        /// <param name="destinationCountryCode"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        internal bool SetNewRate(string sourceCountryCode, string destinationCountryCode, decimal rate)
        {
            var data = (from c in dbContext.ExchangeRate
                        where (c.CountryCode1 == sourceCountryCode || c.CountryCode1 == destinationCountryCode) && (c.CountryCode2 == destinationCountryCode || c.CountryCode2 == sourceCountryCode)
                        select c
                          ).FirstOrDefault();

            if (data != null)
            {


                if (data.CountryCode1 == sourceCountryCode)
                {

                    data.CountryRate1 = rate;
                    dbContext.Entry(data).State = EntityState.Modified;

                    var data2 = (from c in dbContext.ExchangeRate
                                 where (c.CountryCode1 == destinationCountryCode) && (c.CountryCode2 == sourceCountryCode)
                                 select c
                         ).FirstOrDefault();
                    if (data2 != null)
                    {
                        data2.CountryRate1 = Math.Round((1 / rate) , 6 ,MidpointRounding.AwayFromZero);
                        dbContext.Entry(data2).State = EntityState.Modified;
                    }
                    else
                    {
                        var exchangeRate = new ExchangeRate()
                        {
                            CountryCode1 = destinationCountryCode,
                            CountryCode2 = sourceCountryCode,
                            CountryRate1 = Math.Round((1 / rate ) , 6, MidpointRounding.AwayFromZero)
                        };
                        dbContext.ExchangeRate.Add(exchangeRate);
                    }


                }
                else
                {
                    data.CountryRate1 = Math.Round((1 / rate) , 6, MidpointRounding.AwayFromZero);
                    dbContext.Entry(data).State = EntityState.Modified;

                    var data2 = (from c in dbContext.ExchangeRate
                                 where (c.CountryCode1 == sourceCountryCode) && (c.CountryCode2 == destinationCountryCode)
                                 select c
                          ).FirstOrDefault();
                    if (data2 != null)
                    {
                        data2.CountryRate1 = rate;
                        dbContext.Entry(data2).State = EntityState.Modified;
                    }
                    else
                    {
                        var exchangeRate = new ExchangeRate()
                        {

                            CountryCode1 = sourceCountryCode,
                            CountryCode2 = destinationCountryCode,
                            CountryRate1 = rate
                        };
                        dbContext.ExchangeRate.Add(exchangeRate);
                    }


                }

                dbContext.SaveChanges();
                return true;
            }
            data = new ExchangeRate()
            {
                CountryCode1 = sourceCountryCode,
                CountryCode2 = destinationCountryCode,
                CountryRate1 = rate
            };
            dbContext.ExchangeRate.Add(data);
            dbContext.SaveChanges();

            var newExchangeRate = new ExchangeRate()
            {

                CountryCode1 = destinationCountryCode,
                CountryCode2 = sourceCountryCode,
                CountryRate1 = Math.Round((1 / rate) ,6, MidpointRounding.AwayFromZero)
            };
            dbContext.ExchangeRate.Add(newExchangeRate);
            dbContext.SaveChanges();

            return true;
        }

        public bool deleteRate(int id)
        {
            if (id != 0)
            {
                var data = dbContext.ExchangeRate.Find(id);
                dbContext.ExchangeRate.Remove(data);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }
    }


}