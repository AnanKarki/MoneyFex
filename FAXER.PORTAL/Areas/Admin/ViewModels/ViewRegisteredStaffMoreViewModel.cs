using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewRegisteredStaffMoreViewModel
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public SystemLoginLevel SystemLoginLevel { get; set; }
        public string NOKFirstName { get; set; }
        public string NOKMiddleName { get; set; }
        public string NOKLastName { get; set; }
        public string NOKRelation { get; set; }
        public string NOKAddress1 { get; set; }
        public string NOKCity { get; set; }
        public string NOKCountry { get; set; }
        public string NOKTelephone { get; set; }
        public string EmpPosition { get; set; }
        public decimal EmpSalary { get; set; }
        public ModeOfJob EmpMode { get; set; }
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString ="{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public string DateOfEmployment { get; set; }
        public string DateOfLeaving { get; set; }
        public string LogInStartTime { get; set; }
        public DayOfWeek LogInStartDay { get; set; }
        public string LogInEndTime { get; set; }
        public DayOfWeek LogInEndDay { get; set; }
        public StaffHolidaysEntiltlement HolidaysEntitlement { get; set; }
        public int HolidaysNoOfDays { get; set; }

        public string StaffFirstName { get; set; }
        public string StaffMiddleName { get; set; }
        public string StaffLastName { get; set; }
        public string ResidencePermitUrl { get; set; }
        public string Type { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string PassportSide1Url { get; set; }
        public string PassportSide2Url { get; set; }
        public string UtilityBillUrl { get; set; }
        public string CVUrl { get; set; }
        public string HighestQualUrl { get; set; }
        public string RefDoc { get; set; }

    }
    
}