using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class StaffNextOfKinViewModel
    {
        public const string BindProperty = "Id , NOKFirstName ,NOKMiddleName ,NOKLastName , NOKAddress1 ,NOKAddress2 , NOKCity, NOKState " +
            ",NOKPostalCode , NOKCountry ,NOKPhoneNumber ,CountryCode , NOKRelationship ";

        public int Id { get; set; }
        public string NOKFirstName { get; set; }
        public string NOKMiddleName { get; set; }
        public string NOKLastName { get; set; }
        public string NOKAddress1 { get; set; }
        public string NOKAddress2 { get; set; }
        public string NOKCity { get; set; }
        public string NOKState { get; set; }
        public string NOKPostalCode { get; set; }
        public string NOKCountry { get; set; }
        public string NOKPhoneNumber { get; set; }
        public string CountryCode { get; set; }
        public string NOKRelationship { get; set; }

    }
}