using FAXER.PORTAL.Areas.Staff.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class StaffAddresses
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public string StaffAddress1 { get; set; }
        public string StaffAddress2 { get; set; }
        public string StaffCity { get; set; }
        public string StaffState { get; set; }
        public string StaffPostalCode { get; set; }
        public string StaffCountry { get; set; }
        public string StaffPhoneNumber { get; set; }
        public BeenLivingSince BeenLivingSince { get; set; }

        public virtual StaffInformation Staff { get; set; }
    }
}