using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewStaffTrainingViewModel
    {
        public int Id { get; set; }
        public int staffId { get; set; }
        public string StaffFullName { get; set; }
        public string staffFirstName { get; set; }
        public string staffMiddleName { get; set; }
        public string staffLastName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Title { get; set; }
        public string Deadline { get; set; }
        public string Link { get; set; }
        public string OutstandingTraining { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime? StartDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime? EndDate { get; set; }
        public string CountryFlag { get; set; }
        public string FullNotice { get; set; }
        public string CreationDate { get; set; }
        public string Status { get; set; }
        
    }
}