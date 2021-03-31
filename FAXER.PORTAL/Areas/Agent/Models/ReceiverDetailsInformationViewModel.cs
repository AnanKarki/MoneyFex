using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class ReceiverDetailsInformationViewModel
    {
        public const string BindProperty = "Id,Country , MobileWalletProvider ,PreviousMobileNumber ,MobileNumber , ReceiverName,  " +
            "EmailAddress,ReasonForTransfer , MobileCode, ReceivingCurrency";
        public int Id { get; set; }
        public string Country { get; set; }
        [Required(ErrorMessage = "Select Mobile Wallet Provider")]
        public int MobileWalletProvider { get; set; }

        public string PreviousMobileNumber { get; set; }
        [Required(ErrorMessage = "Enter Mobile Number")]
        public string MobileNumber { get; set; }
        [Required(ErrorMessage = "Enter Receiver Name")]
        public string ReceiverName { get; set; }
        [Required(ErrorMessage = "Select Reason")]
        public ReasonForTransfer ReasonForTransfer { get; set; }
        public string MobileCode { get; set; }
        public string ReceivingCurrency { get; set; }
        public string EmailAddress { get; set; }
        public int TransactionSummaryId { get; set; }


    }
    public class MobileWalletProviderVm
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
    public class PreviousMobileNumberVm
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ReceiverMobileNo { get; set; }

    }
    public class ReasonToTransferVm
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}