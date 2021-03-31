using FAXER.PORTAL.Areas.Staff.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class StaffInformation
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string BirthCountry { get; set; }
        public int Gender { get; set; }
        public string EmailAddress { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public BeenLivingSince BeenLivingSince { get; set; }
        public string StaffMFSCode { get; set; }

        public StaffHolidaysEntiltlement StaffHolidaysEntitlemant { get; set; }
        public int StaffNoOFHolidays { get; set; }

        public SystemLoginLevel SytemLoginLevelOfStaff { get; set; }
        public string NOKFirstName { get; set; }
        public string NOKMiddleName { get; set; }
        public string NOKLastName { get; set; }
        public string NOKAddress1 { get; set; }
        public string NOKAddress2 { get; set; }
        public string NOKCity { get; set; }
        public string NOKState { get; set; }
        public string NOKPostalCode { get; set; }
        public string NOKCountry { get; set; }
        public string NOKPhoneNumber { get; set; }
        public string NOKRelationship { get; set; }



        public string ResidencePermitUrl { get; set; }
        public string ResidencePermitType { get; set; }
        public DateTime? ResidencePermitExpiryDate { get; set; }
        public string PassportSide1Url { get; set; }
        public string PassportSide2Url { get; set; }
        public string UtilityBillUrl { get; set; }
        public string CVUrl { get; set; }
        public string HighestQualificationUrl { get; set; }
    }
    public enum StaffHolidaysEntiltlement
    {
        [Display(Name = "No", Order = 0)]
        No = 0,
        [Display(Name = "Yes", Order = 1)]
        Yes = 1
    }
    /// <summary>
    /// [Display(Name = "Level 3", Order = 0)]
    /// Level3 = 0,
    /// [Display(Name = "Level 2", Order = 1)]
    /// Level2 = 1,
    /// [Display(Name = "Level 1", Order = 2)]
    /// Level1 = 2
    /// </summary>
    public enum SystemLoginLevel
    {
        [Display(Name = "Level 4", Order = 0)]
        Level4 = 3,
        [Display(Name = "Level 3", Order = 0)]
        Level3 = 0,
        [Display(Name = "Level 2", Order = 1)]
        Level2 = 1,
        [Display(Name = "Level 1", Order = 2)]
        Level1 = 2,
        [Display(Name = "Level 5", Order = 3)]
        level5 = 4
    }
}