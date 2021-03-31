using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderTransferMoneyBetweenWalletSuccess
    {

        public int Id { get; set; }
        public string Currency { get; set; }
        public decimal AvailableAmount { get; set; }
       
    }
}