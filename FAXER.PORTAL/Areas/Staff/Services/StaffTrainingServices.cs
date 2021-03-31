using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FAXER.PORTAL.Common;

namespace FAXER.PORTAL.Areas.Staff.Services
{
    public class StaffTrainingServices
    {
        DB.FAXEREntities dbContext = null;
        public StaffTrainingServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<ViewModels.StaffTrainingViewModel> GetStaffTrainings()
        {
            int staffId = Common.StaffSession.LoggedStaff.StaffId;

            var staffInformation = dbContext.StaffInformation.Where(x => x.Id == staffId).FirstOrDefault();
            var data = (from c in dbContext.StaffTraining.Where(x => (x.StaffInformationId == staffId
                         || x.StaffInformationId == 0 && x.isDeleted == false) &&
                         (x.Country == staffInformation.Country || x.Country == null) &&
                         (x.City == staffInformation.City || x.City == null)
                          ).ToList()
                        select new ViewModels.StaffTrainingViewModel()
                        {
                            TrainingId = c.Id,
                            TrainingTitle = c.Title,
                            TrainingLink = c.Link,
                            TrainingDeadline = Common.ConversionExtension.ToFormatedString(c.Deadline),
                            CompletedTraining = c.CompleteTraining,
                            OutstandingTraining = c.OutstandingTraining


                        }).ToList();
            return data;
        }
      
    }
}