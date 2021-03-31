using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class StaffEmployment
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public string Position { get; set; }
        public decimal Salary { get; set; }
        public ModeOfJob Mode { get; set; }
        public DateTime EmploymentDate { get; set; }
        public DateTime? LeavingDate { get; set; }
        public virtual StaffInformation Staff { get; set; }
    }
}