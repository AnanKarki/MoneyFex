using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class PublicHolidays
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Name { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int CreatedByStaff { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ModifiedByStaff { get; set; }
        public bool IsDeleted { get; set; }
    }
}