using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class SendMoneToKiiPayWalletViewModel
    {
        public const string BindProperty = "Id , FirstName ,MiddleName,LastName , Search ,DOB,IdType , IdNumber ,ExpiryDate ,CountryCode , IssuingCountry ,AddressLine1," +
            "AddressLine2 , PostCode ,Country , MobileNo, Email, Searched, CountryPhoneCode,City,Day,Month ,Year ,Gender,SenderFullName";

        [Range(0, int.MaxValue)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter First Name")]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required(ErrorMessage = "Enter Last Name")]
        public string LastName { get; set; }

        public string Search { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }
        [Required(ErrorMessage = "Enter IDType")]
        public int IdType { get; set; }
        [Required(ErrorMessage = "Enter Id Number")]
        public string IdNumber { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Enter Expiry Date")]
        public DateTime ExpiryDate { get; set; }
       
        public string CountryCode { get; set; }
        [Required(ErrorMessage = "Select Issusing Country")]
        public string IssuingCountry { get; set; }
        [Required(ErrorMessage = "Enter Address")]
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
      
        public string PostCode { get; set; }
      
        public string Country { get; set; }

        [Required(ErrorMessage = "Enter Mobile Number")]
        public string MobileNo { get; set; }

        public string Email { get; set; }
        public bool Searched { get; set; }
        public string CountryPhoneCode {get; set;}
        public string City {get; set;}
        public int Day { get; set; }
        public Month Month { get; set; }
        public int Year { get; set; }
        public Gender Gender { get; set; }
        public string SenderFullName { get; set; }

    }
}