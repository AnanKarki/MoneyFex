using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class SARFormServices
    {
        DB.FAXEREntities dbContext = null;
        public SARFormServices()
        {
            dbContext = new DB.FAXEREntities();
        }




        public DB.SARForm CreateSARFormReport(DB.SARForm model)
        {

            dbContext.SARForm.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public List<SARForm> List()
        {
           
            return dbContext.SARForm.ToList();
        }
        public SARForm Update(SARForm model)
        {
            dbContext.Entry<SARForm>(model).State = EntityState.Modified;

            dbContext.SaveChanges();
            return model;
        }


        public List<DB.SARForm_ReasonForSuspicion> CreateSARForm_ReasonForSuspicion(List<DB.SARForm_ReasonForSuspicion> model) {

            dbContext.SARForm_ReasonForSuspicions.AddRange(model);
            dbContext.SaveChanges();
            return model;
        }


        public List<CheckBoxViewModel> GetReasonsForSuspicion() {

            var result = (from c in dbContext.ReasonForSuspicion
                          select new CheckBoxViewModel()
                          {
                              Id = c.Id,
                              Name = c.Name,
                              IsChecked = false
                          }).ToList();
            return result;

        } 



    }

}