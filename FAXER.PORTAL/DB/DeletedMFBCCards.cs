using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class DeletedMFBCCards
    {
        public int Id { get; set; }
        public string MFBCCardNumber { get; set; }
        public int KiiPayBusinessInformationId { get; set; }
        public int DeletedBy { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
    }
}