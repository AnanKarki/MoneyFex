using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewRegisteredPartnersViewModel
    {
        public int Id { get; set; }
        public string NameOfPartner { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string PartnerType { get; set; }
        public string PartnerLogo { get; set; }
        public string Status { get; set; }
        public string FirstLetterOfPartner { get;  set; }
    }
}