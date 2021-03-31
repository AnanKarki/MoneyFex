using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class LoggerViewModel
    {
        public int Id { get; set; }
        public ErrorType ErrorType { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime DateTime { get; set; }
        public string Source { get; set; }
    }
}