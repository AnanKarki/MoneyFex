using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class KiiPayBusinessUserPersonalInfo
    {

        public int Id { get; set; }

        public int KiiPayBusinessInformationId { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string PostCodeORZipCode { get; set; }

        public virtual KiiPayBusinessInformation KiiPayBusinessInformation { get; set; }

    }
}