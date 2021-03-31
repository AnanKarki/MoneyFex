using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderSavedBanklAccountVM
    {
        public const string BindProperty = "Id,BankName,AccountNumber,FormattedAccNo,Status";

        [Range(0 , int.MaxValue)]
        public int Id { get; set; }
        [MaxLength(200)]
        public string BankName { get; set; }
        [MaxLength(200)]
        public string AccountNumber { get; set; }
        [MaxLength(200)]
        public string FormattedAccNo { get; set; }
        [MaxLength(200)]
        public string Status { get; set; }
    }
}