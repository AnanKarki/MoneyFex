using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class StaffEmploymentViewModel
    {
        public const string BindProperty = "Id , Position ,Salary , ModeOfJob , EmploymentDate ,DateOfLeaving ";

        public int Id { get; set; }
        public string Position { get; set; }
        public decimal Salary { get; set; }
        public ModeOfJob? ModeOfJob { get; set; }
        public DateTime EmploymentDate { get; set; }
        public DateTime DateOfLeaving { get; set; }
    }

    public enum ModeOfJob
    {
        [Display(Name="Part Time")]
        PartTime = 0,
        [Display(Name = "Full Time")]
        FullTime = 1,
        
    }
}