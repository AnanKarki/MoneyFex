using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class Career
    {
        public int Id { get; set; }
        public string JobTitle { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string ContractType { get; set; }
        public string SalaryRange { get; set; }
        public string Image { get; set; }
        public DateTime ClosingDate { get; set; }
       
        public DateTime PublishedDateTime { get; set; }
        public int PublishedBy { get; set; }
        public int LastModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}