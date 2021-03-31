using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class BusinessAlerts
    {
        public int Id { get; set; }
        public string Heading { get; set; }
        public string FullMessage { get; set; } 
        public string Country { get; set; }
        public string City { get; set; }
        public int? SenderId { get; set; }
        public string PhotoUrl { get; set; }
        public DateTime PublishedDateAndTime { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}