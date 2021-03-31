using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class PayoutProvider
    {
        public int Id { get; set; }
        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int CreatedBy { get; set; }
    }
    public class PayoutProviderDetails
    {
        public int Id { get; set; }
        public int PayoutProviderId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string BranchName { get; set; }
    }


}