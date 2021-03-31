using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class APIProvider
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public string  APIProviderName { get; set; }
        public string  ContactPerson { get; set; }
        public string  Telephone { get; set; }
        public string  Email { get; set; }
        public int  CreatedBY { get; set; }
        public DateTime CreatedDate{ get; set; }
    }
}