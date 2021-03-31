using FAXER.PORTAL.DB;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class BusinessRegistrationsSignUpVm
    {
        public const string BindProperty = "Id , BusinessName , RegistrationNumber, BusinessType, AddressLine1, AddressLine2 , City, State," +
                                             " ZipCode ,Country,MobileNumber,OfficeNumber,ContactPersonName,ContactNo,Email," +
                                             " Password ,ConfirmPassword";


        public int Id { get; set; }

        [Required, DisplayName("Company/Business Name")]
        public string BusinessName { get; set; }

        [Required, DisplayName("Registration Number")]
        public string RegistrationNumber { get; set; }

        [Required, DisplayName("Business Type")]
        public BusinessType BusinessType { get; set; }

        [Required, DisplayName("Address Line1")]
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        [Required, DisplayName("City")]
        public string City { get; set; }

        [Required, DisplayName("State")]
        public string State { get; set; }

        [Required, DisplayName("Zip Code")]
        public string ZipCode { get; set; }

        [Required, DisplayName("Country")]
        public string Country { get; set; }

        [Required, DisplayName("Mobile Number")]
        public string MobileNumber{ get; set; }
        
        public string OfficeNumber{ get; set; }

        [Required, DisplayName("Contact Person Name")]
        public string ContactPersonName{ get; set; }

        [Required, DisplayName("Contact Number")]
        public string ContactNo{ get; set; }

        [Required, DisplayName("Email Address"),DataType(DataType.EmailAddress)]
        public string Email{ get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password{ get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword{ get; set; }

    }
}