using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessRegisterECardUser
    {
    }

    public class KiiPayBusinessRegisterECardUserPersonalDetailVM
    {
        public const string BindProperty = "FirstName  , MiddleName , LastName, BirthDateDay , BirthDateMonth  , BirthDateYear , Gender";

        [Required(ErrorMessage = "Please enter FirstName")]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required(ErrorMessage = "Please enter LastName")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Please enter Day")]
        public int BirthDateDay { get; set; }
        [Required(ErrorMessage = "Please select Month")]
        [Range(1 , 12 , ErrorMessage = "Please select Month")]
        public Month BirthDateMonth { get; set; }
        [Required(ErrorMessage = "Please enter Year")]
        public int BirthDateYear { get; set; }
        public  Gender  Gender { get; set; }
    }
    public class KiiPayBusinessRegisterECardUserAddressInformationVM
    {
        public const string BindProperty = "Country  , City , AddressLine1, AddressLine2 , PostCodeZipCode ";

        [Required(ErrorMessage = "Please select Country")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Please enter City")]
        public string City { get; set; }
        [Required(ErrorMessage = "Please enter AddressLine1")]
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string PostCodeZipCode { get; set; }
    }
     public class KiiPayBusinessRegisterECardUserIdentificationInformationVM
    {
        public const string BindProperty = "IDCardType  , IDCardNumber , ExpiringDateDay, ExpiringDateMonth , ExpiringDateYear , IDIssueCountry";

        [Required(ErrorMessage = "Please select IDCardType")]
        public string IDCardType { get; set; }
        [Required(ErrorMessage = "Please enter IDCardNumber")]
        public string IDCardNumber { get; set; }
        [Required(ErrorMessage = "Please enter Expiring Day ")]
        public int ExpiringDateDay { get; set; }
        [Required(ErrorMessage = "Please enter Expiring Day ")]
        public Month ExpiringDateMonth { get; set; }
        [Required(ErrorMessage = "Please enter Expiring Year ")]
        public int ExpiringDateYear { get; set; }
        public string IDIssueCountry { get; set; }
    }
    public class KiiPayBusinessRegisterECardUserPhotoInformationVM
    {
        public const string BindProperty = "PhotoUrl"; 
        public string PhotoUrl{ get; set; }
    }

    
}