using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class MFTCCardInformationUpdateViewModel
    {
        public const string BindProperty = "Id, TopUpUserFirstName," +
            "TopUpUserLastName,TopUpUserDateOfBirth,Address1,Address2," +
            "State,PostalCode,City,Country,PhoneNumber,EmailAddress";

        [Range(0 , int.MaxValue)]
        public int Id { get; set; }
        [MaxLength()]
        public string TopUpUserFirstName { get; set; }
        [MaxLength(200)]
        public string TopUpUserMiddleName { get; set; }
        [MaxLength(200)]
        public string TopUpUserLastName { get; set; }
        [MaxLength(200)]
        public string TopUpUserDateOfBirth { get; set; }
        [Required(ErrorMessage = "Enter Address")]

        public string Address1 { get; set; }
        [MaxLength(200)]
        public string Address2 { get; set; }
        [Required(ErrorMessage = "Enter State")]
        public string State { get; set; }
        [Required(ErrorMessage = "Enter Postal Code.")]
        public string PostalCode { get; set; }
        [Required(ErrorMessage = "Enter City")]
        public string City { get; set; }
        [MaxLength(200)]
        public string Country { get; set; }
        [Required(ErrorMessage = "Enter Phone Number")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Enter Email Address")]
        public string EmailAddress { get; set; }
    }
}