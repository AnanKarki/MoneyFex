using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewRegisterBusinessViewModel
    {
        public const string BindProperty = "Id , BusinessName ,BusinessRegNumber , BusinessMobileNo,Address , Address2, Street , State ,Country , City,PostalCode ,Telepone , CountryCode" +
            ", Fax, Email,BusinessLicenseNumber ,BusinessTypeEnum , BusinessType, ContactFirstName, ContactLastName, ContactName, ContactDOB, ContactGender, ContactAddress,Website , IsActive" +
            ", Notes ,Confirm , LoginCount, BusinessNoteList";

        public int? Id { get; set; }
        [Required]
        public string BusinessName { get; set; }

        public string BusinessRegNumber { get; set; }
        public string BusinessMobileNo { get; set; }
        [Required]
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Street { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string PostalCode { get; set; }
        [Required]
        //[RegularExpression(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}", ErrorMessage = "Please enter valid Phone Number")]
        public string Telepone { get; set; }
        public string CountryCode { get; set; }
        [Required]
        public string Fax { get; set; }
        [Required]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter valid email")]
        public string Email { get; set; }
        public string BusinessLicenseNumber { get; set; }

        public BusinessType BusinessTypeEnum { get; set; }

        public string BusinessType { get; set; }
        [Required]
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactName { get; set; }
        public string ContactDOB { get; set; }
        public string ContactGender { get; set; }
        public string ContactAddress { get; set; }
        [Required]
        [RegularExpression(@"(http(s)?://)?([\w-]+\.)+[\w-]+(/[\w- ;,./?%&=]*)?", ErrorMessage = "Please enter valid URL")]
        public string Website { get; set; }
        public string IsActive { get; set; }
        public string Notes { get; set; }
        public bool Confirm { get; set; }
        public int LoginCount { get; set; }

        public List<BusinessNoteViewModel> BusinessNoteList { get; set; }

    }



    public class BusinessNoteViewModel
    {

        public string Note { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }

        public string StaffName { get; set; }
    }
}