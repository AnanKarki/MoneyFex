using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class CreditCardAttemptLimitViewModel
    {
        public const string BindProperty = "Id,Frequency , AttemptLimit ,SendingCountry,SendingCountryName , ReceivingCountry,ReceivingCountryName,SenderId,SenderName, SenderAccountNo ";
        public int Id { get; set; }
        public CrediDebitCardUsageFrequency Frequency { get; set; }
        [Required (ErrorMessage ="Enter no Attempt ")]
        public int AttemptLimit { get; set; }
        [Required(ErrorMessage = "select Country")]
        public string SendingCountry { get; set; }
        public string SendingCountryName { get; set; }

        [Required(ErrorMessage = "select Country")]
        public string ReceivingCountry { get; set; }
        public string ReceivingCountryName { get; set; }
        public int? SenderId { get; set; }
        public string SenderName { get; set;  }
        public string SenderAccountNo{ get; set;  }
    }
}