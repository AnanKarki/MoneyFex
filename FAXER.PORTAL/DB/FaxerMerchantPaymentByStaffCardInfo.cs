using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class FaxerMerchantPaymentByStaffCardInfo
    {
        public int Id { get; set; }
        public int? TransactionId { get; set; }
        public int FaxerId { get; set; }
        public string NameOnCard { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public virtual FaxerInformation Faxer { get; set; }
        public virtual SenderKiiPayBusinessPaymentTransaction Transaction { get; set; }
    }
}