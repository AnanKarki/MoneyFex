using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class KiiPayPersonalWalletWithdrawalCode
    {

        public int Id { get; set; }
        public int KiiPayPersonalWalletId { get; set; }
        public string AccessCode { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ExpiredDateTime { get; set; }
        public bool IsExpired { get; set; }

    }
}