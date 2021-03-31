using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class Suppliers
    {
        public int Id { get; set; }
        public int KiiPayBusinessInformationId { get; set; }
        public string RefCode { get; set; }
        public string Country { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsActive { get; set; }
        public virtual KiiPayBusinessInformation KiiPayBusinessInformation { get; set; }
    }
}