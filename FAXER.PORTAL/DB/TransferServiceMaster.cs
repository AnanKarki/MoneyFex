using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class TransferServiceMaster
    {
        public int Id { get; set; }
        public string SendingCountry { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCountry { get; set; }
        public string ReceivingCurrency { get; set; }
        public int CreatedById { get; set; }
        public DateTime CreatedDateTime { get; set; }

        public int? TransferServicesByCurrencyId { get; set; }
    }

    public class TransferServiceDetails
    {
        public int Id { get; set; }
        public int TransferMasterId { get; set; }
        public TransferService ServiceType { get; set; }
        public TransferServiceMaster TransferMaster { get; set; }
    }

}