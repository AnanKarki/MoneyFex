using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderProfileEditViewModel
    {
        public const string BindProperty = "Id,Address,AddressLine2,PostCode,PostCode,City,Country,MobileNumber, MobileCode" +
            ",EmailAddress,IdCardType,IdCardTypeName,IdCardNumber,IdExpiringDate,Day,Month,IdCardStatus" +
             ",Year,IdIssuingCountry,PinCode,UserEnterPinCode,AccountNo,DateOfBirth";

        public int Id { get; set; }
        [Required(ErrorMessage = "Enter Address line 1")]
        public string Address { get; set; }
        public string AddressLine2 { get; set; }
        [Required(ErrorMessage = "Enter postcode / zipcode")]
        public string PostCode { get; set; }
        [Required(ErrorMessage = "Enter City")]
        public string City { get; set; }
        [Required(ErrorMessage = "selct Country")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Enter Mobile number")]
        public string MobileNumber { get; set; }
        public string MobileCode { get; set; }
        [Required(ErrorMessage = "Enter Email Address")]
        public string EmailAddress { get; set; }
        public int IdCardType { get; set; }
        public string IdCardTypeName { get; set; }
        public string IdCardNumber { get; set; }
        public string IdCardStatus { get; set; }

        public DateTime IdExpiringDate { get; set; }
        public int Day { get; set; }
        public Month Month { get; set; }

        public int Year { get; set; }
        public string IdIssuingCountry { get; set; }
        public string PinCode { get; set; }
        public string UserEnterPinCode { get; set; }
        public string AccountNo { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Enter Birth of Date")]
        public DateTime? DateOfBirth{ get; set; }

    }
}