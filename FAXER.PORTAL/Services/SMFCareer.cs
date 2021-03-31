using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SMFCareer
    {
        DB.FAXEREntities dbContext = null;
        public SMFCareer()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<Models.MFCareerListViewModel> GetMFCareerList()
        {
            var CurrentDate = DateTime.Now.Date;

            var CurrentTime = DateTime.Now.TimeOfDay;
            var result = (from c in dbContext.Career.Where(x => x.IsDeleted == false && DbFunctions.TruncateTime(x.ClosingDate) >= CurrentDate).ToList()
                          select new Models.MFCareerListViewModel()
                          {

                              Id = c.Id,
                              JobTitle = c.JobTitle,
                              JobDescription = c.Description,
                              ContractType = c.ContractType,
                              Location = c.City + " " + Common.Common.GetCountryName(c.Country),
                              SalaryRange = c.SalaryRange,
                              ClosingDate = c.ClosingDate.ToString("dd/MM/yyyy")
                          }).ToList();

            return result;

        }

        public DB.JobApplicant SaveJobApplicant(DB.JobApplicant jobApplicant)
        {


            dbContext.JobApplicant.Add(jobApplicant);

            dbContext.SaveChanges();

            return jobApplicant;


        }

        public DB.Career GetJob(int JobID)
        {

            var result = dbContext.Career.Where(x => x.Id == JobID).FirstOrDefault();

            return result;
        }


    }
}