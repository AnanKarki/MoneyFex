using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class Context
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime LastLogin { get; set; }
        public TimeSpan TimeOut { get; set; }
        public string Token { get; set; }
    }
}