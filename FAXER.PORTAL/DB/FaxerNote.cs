using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class FaxerNote
    {
        public int Id { get; set; }

        public int FaxerId { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public string Note { get; set; }

        public string CreatedByStaffName { get; set; }

        public int CreatedByStaffId { get; set; }
    }
}