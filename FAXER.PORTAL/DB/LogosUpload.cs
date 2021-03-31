using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class LogosUpload
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public TransactionTransferMethod Service { get; set; }
        public string Title { get; set; }
        public string WebstieUrl { get; set; }
        public string Logo { get; set; }
    }
}