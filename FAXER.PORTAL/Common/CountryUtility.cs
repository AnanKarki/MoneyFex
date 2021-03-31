using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Common
{
    public static class CountryUtility
    {
        
        public static string GetCountryCurrency(string countryCode)
        {
            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            string countryCurrency = dbContext.Country.Where(x=>x.CountryCode==countryCode).Select(x=>x.Currency).FirstOrDefault();
            return countryCurrency;
        }

        public static string GetCountryCurrencySymbol(string countryCode)
        {
            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            string countryCurrencySymbol = dbContext.Country.Where(x => x.CountryCode == countryCode).Select(x => x.CurrencySymbol).FirstOrDefault();
            if (string.IsNullOrEmpty(countryCurrencySymbol))
            {
                return "";
            }
            return countryCurrencySymbol;

        }
    }
}