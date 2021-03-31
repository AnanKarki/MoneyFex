using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class MyBusinessProfileViewModel
    {
        public const string BindProperty = "Id , BusinessName , BusinessLicenseNumber, Address1 , Address2 , City , State, PostalCode," +
                                              " Country ,PhoneNumber,FaxNo ,EmailAddress ,Website,NameOfContactPerson ";

        public int Id { get; set; }
        public string BusinessName { get; set; }

        public string BusinessLicenseNumber { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }

        public string Country { get; set; }
        
        public string PhoneNumber { get; set; }

        public string FaxNo { get; set; }
        public string EmailAddress { get; set; }

        public string Website { get; set; }

        public string NameOfContactPerson { get; set; }
    }
}