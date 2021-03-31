using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class BusinessDetailsViewModel
    {
        public const string BindProperty = "Id , BusinessName , RegistrationNumber, MiddleName, BusinessLicenseNumber , EmailAddress , NameOfContactPerson";


        public int Id { get; set; }
        [Required]
        public string BusinessName { get; set; }

        public string RegistrationNumber { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string BusinessLicenseNumber {get; set;}
        [Required]

        public string EmailAddress { get; set; }
        [Required]
        public string NameOfContactPerson { get; set; }

    }
}