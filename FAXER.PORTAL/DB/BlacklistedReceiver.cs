using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class BlacklistedReceiver
    {
        public int Id { get; set; }
        public string ReceiverAccountNo { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverCountry { get; set; }
        public DateTime CareatedDate { get; set; }
        public int CreatedByUserId { get; set; }
        public bool IsBlocked { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public string ReceiverTelephone { get; set; }
        public string BankCode{ get; set; }
        public string BankNameOrMobileWalletProvider{ get; set; }
        public bool IsDeleted { get; set; }
    }
}