using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class WalletInformationViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string LastName { get; set; }

        public string WalletNo { get; set; }
        public DateTime DOB { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string IdType { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string IdNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public int CountryCode { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string IssuingCountry { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string PostCode { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string Country { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string MobileNo { get; set; }
        [Required(ErrorMessage = "This field is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }
       
    }
}