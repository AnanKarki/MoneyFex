using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class BankAccountDepositSecondVM
    {
        [Required(ErrorMessage ="Enter Country")]
        public string Country { get; set; }
        public int PreviousAccountNo { get; set; }
        [Required(ErrorMessage = "Enter= Account Owner Name")]
        public string  AccountOwnerName{ get; set; }
        public int MobileNo { get; set; }
        [Required(ErrorMessage = "Enter Account Number  ")]
        public string AccountNo { get; set; }
        public int BankId { get; set; }
        public int BranchId { get; set; }
        [Required(ErrorMessage = "Enter Branch Code")]
        public string BranchCode{ get; set; }
    }
}