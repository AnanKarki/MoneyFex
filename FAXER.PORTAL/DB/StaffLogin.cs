using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class StaffLogin
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string ActivationCode { get; set; }
        public int LoginFailedCount { get; set; }
        public string LoginCode { get; set; }
        [DataType(DataType.Time)]
        public string LoginStartTime { get; set; }
        public string LoginEndTIme { get; set;}
        public DayOfWeek? LoginStartDay { get; set; }
        public DayOfWeek? LoginEndDay { get; set; } 

        public virtual StaffInformation Staff { get; set; }

        
    }

  
}