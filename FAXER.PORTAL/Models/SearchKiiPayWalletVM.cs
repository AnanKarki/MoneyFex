using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SearchKiiPayWalletVM
    {


        public const string BindProperty = "Id,CountryCode,RecentMobileNo, CountryPhoneCode" +
            ", MobileNo ,ReceiverName ";


        public int Id { get; set; }
        [MaxLength(200)]
        [Required(ErrorMessage = "Enter Country")]

        public string CountryCode { get; set; }

        [MaxLength(200)]
        public string RecentMobileNo { get; set; }

        [MaxLength(200)]
        public string CountryPhoneCode { get; set; }
        /// <summary>
        /// Wallet No 
        /// </summary>
        [Required(ErrorMessage = "Enter Mobile Number")]
        public string MobileNo { get; set; }

        [MaxLength]
        public string ReceiverName { get; set; }


    }
}