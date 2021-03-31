using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class PayAReceiveCashPickupReceiverDetailsViewModel
    {

        public const string BindProperty = "Id , FirstName ,MiddleName,LastName , DOB ,IdType,IdNumber , ExpiryDate ,CountryCode,IssuingCountry" +
            ",AddressLine1 ,AddressLine2 ,PostCode ,Country , MobileNo,BirthCountry , StatusOfFax, StatusOfFaxName ,Amount ";

        public int Id { get; set; }
        
        [Required(ErrorMessage = "Enter First Name")]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required(ErrorMessage = "Enter Middle Name")]
        public string LastName { get; set; }
        public DateTime DOB { get; set; }
        [Required(ErrorMessage = "Select ID Type")]
        public string IdType { get; set; }
        [Required(ErrorMessage = "Enter Id Number")]
        public string IdNumber { get; set; }
        [Required(ErrorMessage = "Enter Id Expiry date")]
        public DateTime? ExpiryDate { get; set; }
        [Required(ErrorMessage = "Select Country")]

        public int CountryCode { get; set; }
        [Required(ErrorMessage = "Select Receiver Country")]
        public string IssuingCountry { get; set; }
        [Required(ErrorMessage = "Enter Address")]

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        [Required(ErrorMessage = "Enter Post Code")]

        public string PostCode { get; set; }
        [Required(ErrorMessage ="Select Country")]

        public string Country { get; set; }
        [Required(ErrorMessage = "Enter Mobile Number")]


        public string MobileNo { get; set; }
        [Required(ErrorMessage = "Select Birth Country")]

        public string BirthCountry { get; set; }
        public FaxingStatus StatusOfFax { get; set; }

        public string StatusOfFaxName { get; set; }
       
        public decimal Amount { get; set; }

    }
}