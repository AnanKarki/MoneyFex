using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderWalletProfileVM
    {

        public const string BindProperty = "Id , WalletId , Name,City," +
            "Country,EmailAddress,WalletStatus,MobileNo,WalletBallanceAvaiable,Address,Code,UserEnterPinCode";
        [Range(0 , int.MaxValue)]
        public int Id { get; set; }
        [Range(0, int.MaxValue)]
        public int WalletId { get; set; }
        [Required(ErrorMessage ="Enter Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Enter City")]
        public string City { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Enter Email Address")]
        public string EmailAddress { get; set; }
        [MaxLength(200)]
        public string WalletStatus { get; set; }
        [Required(ErrorMessage = "Enter Mobile/Wallet Number")]
        public string MobileNo { get; set; }
        [Required(ErrorMessage = "Enter Wallet Balance")]
        public string WalletBallanceAvaiable { get; set; }
        [MaxLength(200)]
        public string Address { get; set; }
        [Required(ErrorMessage = "Enter Code")]
        public string Code { get; set; }
        [MaxLength(200)]
        public string UserEnterPinCode { get; set; }
    }
}