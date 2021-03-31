using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Staff.Services
{
    public class CommonServices
    {
        FAXEREntities dbContext = new FAXEREntities();


        public List<CountryDropDownViewModel> GetCountries()
        {
            var result = (from c in dbContext.Country
                          select new CountryDropDownViewModel()
                          {
                              Id = c.Id,
                              Code = c.CountryCode,
                              Name = c.CountryName
                          }
                      ).ToList();

            return result;
        }
    }

    public class CountryDropDownViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}