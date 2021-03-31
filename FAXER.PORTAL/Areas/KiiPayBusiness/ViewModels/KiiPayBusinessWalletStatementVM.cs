
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessWalletStatementVM
    {
        public int Id { get; set; }
        public string SenderCountry { get; set; }
        public string ReceiverCountry { get; set; }
        public int TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal SendingAmount { get; set; }

        public decimal Fee { get; set; }
        public decimal ReceivingAmount { get; set; }

        public decimal SenderCurBal { get; set; }
        public decimal ReceiverCurBal { get; set; }

        public WalletStatmentType WalletStatmentType { get; set; }
        public WalletStatmentStatus WalletStatmentStatus { get; set; }
    }
}