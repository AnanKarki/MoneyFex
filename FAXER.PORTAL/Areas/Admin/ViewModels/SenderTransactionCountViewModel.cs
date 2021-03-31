using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class SenderTransactionCountViewModel
    {
        public const string BindProperty = "Id, SenderId, SenderName,SenderAccountNo ,SendingCountry ,SendingCountryName , ReceivingCountry, " +
                                           "ReceivingCountryName, TransactionCount,Frequency ";
        public int Id { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Select Sender")]
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderAccountNo { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string SendingCountry { get; set; }
        public string SendingCountryName { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string ReceivingCountry { get; set; }
        public string ReceivingCountryName { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Enter no of Transaction")]
        public int TransactionCount { get; set; }
        public CrediDebitCardUsageFrequency Frequency { get; set; }

    }
}