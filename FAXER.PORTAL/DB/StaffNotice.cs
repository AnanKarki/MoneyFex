using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class StaffNotice
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public DateTime Date { get; set; }
        public string Headline { get; set; }
        public string FullNotice { get; set; }
        public int ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }

        //public virtual StaffInformation Staff { get; set; }




    }
}