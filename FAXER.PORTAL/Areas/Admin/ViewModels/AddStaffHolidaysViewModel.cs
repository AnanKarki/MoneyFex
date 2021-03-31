using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AddStaffHolidaysViewModel
    {
        public const string BindProperty = "Id , Country ,City  ,StaffId, StaffName, NoOfDays ,NoOfDaysEntitled, StartDate, FinishDate,NoTaken ,NoLeft ,AlreadyTaken";

        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public int NoOfDays { get; set; }
        public int NoOfDaysEntitled{ get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime FinishDate { get; set; }
        public int NoTaken { get; set; }
        public int NoLeft { get; set; }
        public bool AlreadyTaken { get; set; }

    }
}