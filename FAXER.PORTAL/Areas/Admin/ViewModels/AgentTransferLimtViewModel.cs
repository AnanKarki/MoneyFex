using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AgentTransferLimtViewModel
    {
        public int? Id { get; set; }
        public string Country { get; set; }
  
        public string City { get; set; }
        public int? AgentId { get; set; }
        
        public string AccountNo { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
       
       public LimitType LimitType { get; set; }
        public AutoPaymentFrequency Frequency { get; set; }
       
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; } 
       public int CreatedBy { get; set; }

        //used for list view and limit history
        public string CountryCurrencySymbol { get; set; }
        public string AgentName { get; set; }
        public string TransferMethodName { get; set; }
        public string FrequencyName { get; set; }
        public string CreationDate { get; set; }

        //For reciverLimit

        public int? ReceiverId { get; set; }
        public string ReceiverName { get; set; }

    }
}