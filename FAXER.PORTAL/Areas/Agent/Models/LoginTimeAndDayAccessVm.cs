using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class LoginTimeAndDayAccessVm
    {

        public const string BindProperty = "Id , StaffName ,StaaffType,StartTime , EndTime ,StartDay,EndDay , Status ," +
                 "UpdatedBy,UpdatedTime , TimeZone";
        public int Id { get; set; }

        public string StaffName { get; set; }

        
        public StaffType StaaffType { get; set; }

        [Required(ErrorMessage ="Enter Start Time")]
        public string StartTime { get; set; }

        [Required(ErrorMessage ="Enter End Time")]
        public string EndTime { get; set; }

        [Required(ErrorMessage ="Select Start Day")]
        public DayOfWeek StartDay { get; set; }

        [Required(ErrorMessage="Select End Day")]
        public DayOfWeek EndDay { get; set; }

        public string Status { get; set; }

        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedTime { get; set; }

        public TimeZone TimeZone { get; set; }
    }
}