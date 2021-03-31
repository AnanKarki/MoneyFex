using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderMobileMoneyTransferVM
    {
        public const string BindProperty = "Id,CountryCode,WalletId,MobileNumber,RecentlyPaidMobile,ReceiverName,ReceiverEmail";

        public int Id { get; set; }


        public string CountryCode { get; set; }


        public string CountryPhoneCode { get; set; }


        [Required(ErrorMessage = "Select Wallet")]
        public int WalletId { get; set; }

        [Required(ErrorMessage = "Enter Mobile Number")]
        public string MobileNumber { get; set; }

        public string RecentlyPaidMobile { get; set; }

        [Required(ErrorMessage = "Enter Receiver Name")]
        public string ReceiverName { get; set; }
        public string ReceiverEmail { get; set; }

        public int TransactionSummaryId { get; set; }

    }
}