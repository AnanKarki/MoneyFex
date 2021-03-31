using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class BankAccountDepositSuccessVM
    {
      
        public decimal Amount { get; set; }
   
        public string Currency { get; set; }
 
        public string ReceiverName{ get; set; }
        public int TransactionId { get; set; }
    }
}