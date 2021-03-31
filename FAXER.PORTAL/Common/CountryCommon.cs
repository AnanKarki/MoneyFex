using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Common
{
    public static class CountryCommon
    {

        public static List<DB.Country> GetCountries()
        {
            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            var countries = dbContext.Country.ToList();
            return countries;
        }

        public static string GetCountryName(string CountryCode)
        {
            if (!string.IsNullOrEmpty(CountryCode) && CountryCode.Trim().ToLower() == "all")
            {
                return "All";
            }
            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            var CountryName = dbContext.Country.Where(x => x.CountryCode == CountryCode).Select(x => x.CountryName).FirstOrDefault();
            return CountryName;
        }
        public static string GetCountryCurrency(string CountryCode)
        {
            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            var currency = dbContext.Country.Where(x => x.CountryCode == CountryCode).Select(x => x.Currency).FirstOrDefault();
            return currency;
        }

        public static string GetCurrencySymbol(string CountryCode)
        {
            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            var currencySymbol = dbContext.Country.Where(x => x.CountryCode == CountryCode).Select(x => x.CurrencySymbol).FirstOrDefault();
            return currencySymbol;
        }

        public static string GetCountryCurrencyName(string CountryCode)
        {
            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            var currencyName = dbContext.Country.Where(x => x.CountryCode == CountryCode).Select(x => x.CurrencyName).FirstOrDefault();
            return currencyName;
        }

        public static string GetCountryPhoneCode(this string CountryCode)
        {
            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            var CountryPhoneCode = dbContext.Country.Where(x => x.CountryCode == CountryCode).Select(x => x.CountryPhoneCode).FirstOrDefault();
            if (CountryPhoneCode != null)
            {
                return CountryPhoneCode;
            }
            return "";
        }
        public static List<string> GetCountriesByCurrency(this string Currency)
        {
            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            var result = dbContext.Country.Where(x => x.Currency.ToLower() == Currency.ToLower()).Select(x => x.CountryCode).ToList();
            return result;
        }
        public static string GetCountryCodeByCountryName(this string countryName)
        {

            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            var result = dbContext.Country.Where(x => x.CountryName.ToLower() == countryName.ToLower()).Select(x => x.CountryCode).FirstOrDefault();
            return result;
        }
        public static string GetCountryFlagCode(this string CountryCode)
        {
            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            string FlagCode = dbContext.Country.Where(x => x.CountryCode == CountryCode).Select(x => x.FlagCode).FirstOrDefault();
            return FlagCode;
        }
        public static string GetCountryCodeByCurrency(string currency)
        {
            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            string code = dbContext.Country.Where(x => x.Currency.ToLower() == currency).Select(x => x.CountryCode).FirstOrDefault();
            return code;
        }
        public static List<string> GetCountriesByCurrencyAndCountry(string Currency, string Country)
        {
            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            var countries = new List<string>();
            if (string.IsNullOrEmpty(Country))
            {
                countries = GetCountriesByCurrency(Currency);
                return countries;

            }
            if (Country.ToLower() == "all")
            {
                countries = GetCountriesByCurrency(Currency);
            }
            else
            {
                countries.Add(Country);
            }

            return countries;
        }


    }
}