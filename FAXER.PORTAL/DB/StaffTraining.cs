using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class StaffTraining
    {
        public int Id { get; set; }
        public int StaffInformationId { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        //[DataType(DataType.Date)]
        public DateTime? Deadline { get; set; }
        public string CompleteTraining { get; set; }
        public string OutstandingTraining { get; set; }
        public bool isDeleted { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int TrainingAddedByStaff { get; set; }
        public DateTime TrainingAddedDate { get; set; }
        public string FullNotice { get;  set; }
    }
}