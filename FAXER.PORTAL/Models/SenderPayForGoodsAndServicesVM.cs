using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderPayForGoodsAndServicesVM
    {

        public const string BindProperty = "Id, CountryCode,RecentlyPaidBusiness , BusinessMobileNo ,ReceiverName ," +
            "ReceiverId , CountryPhoneCode";
        [Range(0 , int.MaxValue)]
        public int Id { get; set; }

        [MaxLength(200)]
        public string CountryCode { get; set; }

        [MaxLength(200)]
        public string RecentlyPaidBusiness { get; set; }

        [Required(ErrorMessage="Enter Business Mobile Number") ]
        public string BusinessMobileNo { get; set; }

        [MaxLength(200)]
        public string ReceiverName { get; set; }
        [Range(0 , int.MaxValue)]
        public int ReceiverId { get; set; }


        [MaxLength(200)]
        public string CountryPhoneCode { get; set; }

    }
}