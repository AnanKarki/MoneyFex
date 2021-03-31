using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class StaffHolidays
    {
        public int Id { get; set; }

        public int StaffInformationId { get; set; }
        [DataType(DataType.Date)]
        public DateTime HolidaysStartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime HolidaysEndDate { get; set; }
        public int TotalNumberOfHolidaysRequeste { get; set; }
        public int HolidaysTaken { get; set; }
        public int HoidaysLeft { get; set; }
        public int HolidaysEntitled { get; set; }
        public HollidayRequestStatus HolidaysRequestStatus { get; set; }
        public string HolidaysReason { get; set; }
        public bool IsDeleted { get; set; }
        public int ModifiedBy { get; set; }

        public int ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
      
        public virtual StaffInformation StaffInformation { get; set; }
    }
    public enum HollidayRequestStatus {
        Approved ,
        Rejected ,
        Requested ,
        Cancel

    }
}