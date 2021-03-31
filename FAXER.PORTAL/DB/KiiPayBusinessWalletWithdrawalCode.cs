using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class KiiPayBusinessWalletWithdrawalCode
    {
        public int Id { get; set; }
        public int KiiPayBusinessInformationId { get; set; }
        public int KiiPayBusinessWalletId { get; set; }
        public int KiiPayUserId { get; set; }
        public string AccessCode { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ExpiredDateTime { get; set; }
        public bool IsExpired { get; set; }
    }
}