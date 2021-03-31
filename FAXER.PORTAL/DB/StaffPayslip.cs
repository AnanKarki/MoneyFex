using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class StaffPayslip
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public Month Month { get; set; }
        public int StaffId { get; set; }
        public string PayslipURL { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int CreatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public int DeletedBy { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public virtual StaffInformation Staff { get; set; }
    }
}