using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Staff.ViewModels
{
    public class StaffNextOfKinViewModel
    {
        public const string BindProperty = "Id , NOKFirstName  , NOKMiddleName , NOKLastName , NOKAddress1 , NOKAddress2 , NOKCity , NOKState ," +
                                           " NOKPostalCode , NOKCountry , NOKPhoneNumber , NOKRelationship";
        public int Id { get; set; }
        [Required(ErrorMessage = "The Field is required")]
        public string NOKFirstName { get; set; }
        public string NOKMiddleName { get; set; }

        [Required(ErrorMessage = "The Field is required")]
        public string NOKLastName { get; set; }

        [Required(ErrorMessage = "The Field is required")]
        public string NOKAddress1 { get; set; }
        public string NOKAddress2 { get; set; }

        [Required(ErrorMessage = "The Field is required")]
        public string NOKCity { get; set; }

        [Required(ErrorMessage = "The Field is required")]
        public string NOKState { get; set; }

        [Required(ErrorMessage = "The Field is required")]
        public string NOKPostalCode { get; set; }

        [Required(ErrorMessage = "The Field is required")]
        public string NOKCountry { get; set; }

        [Required(ErrorMessage = "The Field is required")]
        public string NOKPhoneNumber { get; set; }

        [Required(ErrorMessage = "The Field is required")]
        public string NOKRelationship { get; set; }

    }
}