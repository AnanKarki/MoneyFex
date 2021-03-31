using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class FaxingCommissionSettingServices
    {

        FAXEREntities dbContext = null;

        public FaxingCommissionSettingServices()
        {
            dbContext = new FAXEREntities();

        }

        public List<FaxingCommissionSettingViewModel> getlist()
        {
            var result = (from c in dbContext.Commission.ToList()
                          join d in dbContext.Continent on c.Continent1 equals d.Code
                          select new FaxingCommissionSettingViewModel()
                          {
                              Id = c.Id,
                              Commission = c.Rate,
                              Continent = d.Name,
                              Code = d.Code
                          }).ToList();

            return result;

        }

        public bool SaveCommission(string Continent, decimal? Commission)
        {
            var con = dbContext.Commission.Where(x => x.Continent1 == Continent).FirstOrDefault();
            if (con == null)
            {
                Commission model = new Commission()
                {
                    Continent1 = Continent,
                    Rate = Commission
                };

                dbContext.Commission.Add(model);
                dbContext.SaveChanges();
                return true;
            }
            else
            {

                Commission data = dbContext.Commission.Where(x => x.Continent1 == Continent).FirstOrDefault();
                if (data != null)
                {
                    data.Rate = Commission;
                    dbContext.Entry(data).State = EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
                return false;
            }
            

        }

        public bool DeleteCommission(int id)
        {
            if (id != 0)
            {
                var data = dbContext.Commission.Find(id);
                data.Rate = 0;
                dbContext.Entry(data).State = EntityState.Modified;
                //dbContext.Commission.Remove(data);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }




    }
}