using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class PayAReceiverKiipayWalletSuccessViewModel
    {
        public int Id { get; set; }
        public KiiPayWalletType KiiPayWalletType { get; set; }
        public int TransactionId { get; set; }
    }
}