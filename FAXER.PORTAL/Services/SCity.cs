using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SCity
    {

        public static List<City> GetCities(Module Module, string CountryCode)
        {
            DB.FAXEREntities dbContext = new FAXEREntities();
            var cities = dbContext.City.Where(x => x.Module == Module && x.CountryCode == CountryCode).ToList();
            return cities.GroupBy(x => x.Name).Select(x => x.First()).ToList();
            //return cities;
        }

        public static List<DropDownViewModel> GetCitiesBoth( string CountryCode)
        {
            DB.FAXEREntities dbContext = new FAXEREntities();
            var cities = dbContext.City.Where(x => (x.Module == DB.Module.Agent || x.Module == DB.Module.BusinessMerchant) && x.CountryCode == CountryCode).ToList();

            
            var DistinctItems = cities.GroupBy(x => x.Name).Select(y => y.First());
            var dfsd = (from c in DistinctItems
                       select new DropDownViewModel()
                       {
                           Id = c.Id,
                           Name = c.Name
                       }).ToList();


            return dfsd;



        }


        public static City Save(City city)
        {
            DB.FAXEREntities dbContext = new FAXEREntities();
            var result= dbContext.City.Where(x => x.Name == city.Name && x.CountryCode == city.CountryCode && x.Module == city.Module).FirstOrDefault();
            if(result==null)
            {
                dbContext.City.Add(city);
                dbContext.SaveChanges();
            }
            return city;

        }
        public static City Save(string Name,string CountryCode,Module Module)
        {
            DB.FAXEREntities dbContext = new FAXEREntities();
            var result = dbContext.City.Where(x => x.Name == Name && x.CountryCode == CountryCode && x.Module == Module).FirstOrDefault();
            var city = new City()
            {
                CountryCode = CountryCode,
                Module = Module,
                Name = Name

            };
            if (result == null)
            {
                dbContext.City.Add(city);
                dbContext.SaveChanges();
            }
            return city;
        }
    }
}