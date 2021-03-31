using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderSendARequestVM
    {

        public const string BindProperty = "Id,CountryCode,CountryPhoneCode,RecentContactNumber,MobileNumber";
        [Range(0 , int.MaxValue)]
        public int Id { get; set; }
        [MaxLength(200)]
        public string CountryCode { get; set; }
        [MaxLength(200)]
        public string CountryPhoneCode { get; set; }
        [MaxLength(200)]
        public string RecentContactNumber { get; set; }

        [Required(ErrorMessage = "Enter vaildate mobile number")]
        public string MobileNumber { get; set; }
    }
}