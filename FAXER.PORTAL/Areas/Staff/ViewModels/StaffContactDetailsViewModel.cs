using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Staff.ViewModels
{
    public class StaffContactDetailsViewModel
    {
        public const string BindProperty = "Id , StaffAddress1 , StaffAddress2 , StaffCity , StaffState , StaffPostalCode ,StaffCountry, StaffPhoneNumber , BeenLivingSince ";

        public int Id { get; set; }
        public string StaffAddress1 { get; set; }
        public string StaffAddress2 { get; set; }
        public string StaffCity { get; set; }
        public string StaffState { get; set; }
        public string StaffPostalCode { get; set; }
        public string StaffCountry { get; set; }
        public string StaffPhoneNumber { get; set; }
        public BeenLivingSince? BeenLivingSince { get; set; }
    }
    public enum BeenLivingSince {



        [Description("One Year")]
        OneYear = 1,

        [Description("Two Year")]
        TwoYear = 2,

        [Description("Three Year")]
        ThreeYear = 3,

        [Description("Four Year")]
        FourYear = 4,

        [Description("Five Year")]
        FiveYear = 5,

        [Description("More Then Five Year")]
        MoreThanFive = 6,
    }
}