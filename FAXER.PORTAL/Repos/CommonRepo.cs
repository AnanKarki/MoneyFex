using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Repos
{
    public class CommonRepo
    {

        DB.FAXEREntities dbContext = new DB.FAXEREntities();

        public List<SelectListItem> GetCountryList()
        {
            List<SelectListItem> countryList = dbContext.Country.AsNoTracking().OrderBy(x => x.CountryName).Select(x => new SelectListItem
            {
                Value = x.CountryCode,
                Text = x.CountryName
            }).ToList();

            return countryList;
        }

        public FAXER.PORTAL.Models.EstimateFaxingFeeSummary GetFaxingFeeSummary(string fromCountry, string toCountry, double faxingAmount, double recAmont)
        {

            Models.EstimateFaxingFeeSummary modelFaxingFee = new Models.EstimateFaxingFeeSummary();

            return modelFaxingFee;
            

        }
    }
}