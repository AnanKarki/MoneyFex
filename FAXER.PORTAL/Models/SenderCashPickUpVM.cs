using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderCashPickUpVM
    {

        public const string BindProperty = "Id ,RecentReceiverId , FullName ,CountryCode,MobileNumber, EmailAddress, Reason, IdenityCardId,IdentityCardNumber ";


        [Range(0, int.MaxValue)]
        public int Id { get; set; }

        public int? RecentReceiverId { get; set; }
        [MaxLength(200)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Select Country")]

        public string CountryCode { get; set; }

        [Required(ErrorMessage = "Enter Mobile Number")]
        public string MobileNumber { get; set; }
        [Range(0, int.MaxValue)]
        public int IdenityCardId { get; set; }
        public string IdentityCardNumber { get; set; }

        [MaxLength(200)]
        public string EmailAddress { get; set; }
        [Required(ErrorMessage = "Select Reason for Transfer")]
        public ReasonForTransfer Reason { get; set; }

        public int TransactionSummaryId { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }




    }
}