using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class StaffDocumentation
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public int StaffId { get; set; }
        public string AccountNo { get; set; }
        public DocumentType DocumentType { get; set; }
        public DocumentExpires DocumentExpires { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPhotoUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public string IssuingCountry { get; set; }
        public DocumentApprovalStatus Status { get; set; }
        


    }
}