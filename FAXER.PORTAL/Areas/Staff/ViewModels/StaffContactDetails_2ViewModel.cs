using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Staff.ViewModels
{
    public class StaffContactDetails_2ViewModel
    {
        public const string BindProperty = "Id , StaffAddress1 , StaffAddress2 , StaffCity , StaffState , StaffPostalCode ,StaffCountry, StaffPhoneNumber , BeenLivingSince ";

        public int Id { get; set; }
        [Required(ErrorMessage = "The Field is required")]
        public string StaffAddress1 { get; set; }
        public string StaffAddress2 { get; set; }

        [Required(ErrorMessage = "The Field is required")]
        public string StaffCity { get; set; }

        [Required(ErrorMessage = "The Field is required")]
        public string StaffState { get; set; }

        [Required(ErrorMessage = "The Field is required")]
        public string StaffPostalCode { get; set; }

        [Required(ErrorMessage = "The Field is required")]
        public string StaffCountry { get; set; }

        [Required(ErrorMessage = "The Field is required")]
        public string StaffPhoneNumber { get; set; }
        public BeenLivingSince? BeenLivingSince { get; set; }
    }
}