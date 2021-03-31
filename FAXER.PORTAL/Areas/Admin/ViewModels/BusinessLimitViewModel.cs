using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class BusinessLimitViewModel
    {
        public const string BindProperty = "Id,SenderId ,BusinessName,SenderName , AccountNumber,Country , City,TransferMethod ,Frequency  , " +
                                           "FrequencyName, TransferMethodName, FrequencyAmount , DateTime,IsBusiness";
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string BusinessName { get; set; }
        public string SenderName { get; set; }
        public string AccountNumber { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Select City")]
        public string City { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public AutoPaymentFrequency Frequency { get; set; }
        public string FrequencyName { get; set; }
        public string TransferMethodName { get; set; }
        [Required(ErrorMessage = "Enter Amount")]
        public string FrequencyAmount { get; set; }
        public string DateTime { get; set; }
        public bool IsBusiness { get; set; }

    }
}