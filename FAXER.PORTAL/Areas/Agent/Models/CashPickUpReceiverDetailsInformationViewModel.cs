using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class CashPickUpReceiverDetailsInformationViewModel
    {
        public const string BindProperty = "Id , Country , City, PreviousReceiver, MobileNo, ReceiverFullName, MobileCode, Email," +
                                             " ReasonForTransfer ,Searched , IdenityCardId , IdentityCardNumber , ReceivingCurrency";

        public int Id { get; set; }
        [Required(ErrorMessage ="Select Country")]
        public string Country { get; set; }

        public string City { get; set; }
        public string PreviousReceiver{ get; set; }
        [Required(ErrorMessage = "Enter Mobile Number")]
        public string MobileNo { get; set; }
        [Required(ErrorMessage = "Enter Receiver Name")]
        public string ReceiverFullName { get; set; }
        public string MobileCode { get; set; }
        public string Email { get; set; }
        [Required(ErrorMessage = "Select Reason ")]
        public ReasonForTransfer ReasonForTransfer { get; set; }
        public bool Searched { get; set; }
        public int TransactionSummaryId { get; set; }
        [Range(0, int.MaxValue)]
        public int IdenityCardId { get; set; }
        public string IdentityCardNumber { get; set; }
        public string ReceivingCurrency { get; set; }

    }
    public class PreviousReceiverVm
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}