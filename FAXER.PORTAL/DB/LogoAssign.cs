using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class LogoAssign
    {
        public int Id { get; set; }
        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }
        public TransactionTransferMethod Services { get; set; }
        public string Label { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
    }
    public class LogoAssignDetails
    {
        public int Id { get; set; }
        public int LogoAssignId { get; set; }
        public LogoAssign LogoAssign { get; set; }
        public int ServiceProvider { get; set; }

    }

}