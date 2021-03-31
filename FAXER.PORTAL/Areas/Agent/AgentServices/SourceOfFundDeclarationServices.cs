using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class SourceOfFundDeclarationServices
    {
        DB.FAXEREntities dbContext = null;
        public SourceOfFundDeclarationServices()
        {
            dbContext = new DB.FAXEREntities();
        }
        public List<SourceOfFundDeclarationFormData> List()
        {
            return dbContext.SourceOfFundDeclarationFormData.ToList();
        }


        public bool CreateSenderNonCardWithdrawal(DB.SourceOfFundDeclarationFormData model)
        {

            dbContext.SourceOfFundDeclarationFormData.Add(model);
            dbContext.SaveChanges();
            return true;
        }

        internal SourceOfFundDeclarationFormData Update(SourceOfFundDeclarationFormData model)
        {
            dbContext.Entry<SourceOfFundDeclarationFormData>(model).State = EntityState.Modified;

            dbContext.SaveChanges();
            return model;
        }
    }
}