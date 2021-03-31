using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AddStaffTrainingViewModel
    {

        public const string BindProperty = "Id , Country ,countryName , City , staffId ,staffName , Title ,Link , Deadline,completeTraining , outstandingTraining,StartDate,EndDate,FullNotice";

        public int Id { get; set; }
        public string Country { get; set; }
        public string countryName { get; set; }
        public string City { get; set; }
        public int staffId { get; set; }
        public string staffName { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
       // [DataType(DataType.Date)]
        public DateTime? Deadline { get; set; }

        public string completeTraining { get; set; }
        public string outstandingTraining { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime? StartDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
  
        public DateTime? EndDate{ get; set; }
        public string FullNotice{ get; set; }
    }
}