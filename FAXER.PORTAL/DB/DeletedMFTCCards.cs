using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class DeletedMFTCCards
    {
        public int Id { get; set; }
        public string MoblieNumber { get; set; }
        public int FaxerId { get; set; }
        public int DeletedBy { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
    }
}