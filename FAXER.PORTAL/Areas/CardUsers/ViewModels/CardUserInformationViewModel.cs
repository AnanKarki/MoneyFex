using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.ViewModels
{
    public class CardUserInformationViewModel
    {
        public const string BindProperty = " Id ,FullName ,Address1 ,Address2 ,City , State,ZipCode ,Country ,PhoneNumber , EmailAddress ";
        public int Id { get; set; }
        public string FullName { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }


        public string PhoneNumber { get; set; }

        public string EmailAddress { get; set; }
        
    }
}