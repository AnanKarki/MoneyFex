using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Staff.ViewModels
{
    public class StaffTrainingViewModel
    {
        public int TrainingId { get; set; }
        public string TrainingTitle { get; set; }

        public string TrainingLink { get; set; }

        public string TrainingDeadline { get; set; }

        public string CompletedTraining { get; set; }
        public string OutstandingTraining { get; set; }
    }
}