using ExCSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class WhatsAppMessage
    {
        public int Id { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public string Body { get; set; }
        public string MessageSId { get; set; }
        public string NumMedia { get; set; }
        public string Media { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public Direction Direction { get; set; }
        public DateTime SentDate { get; set; }
        public string SentBy { get; set; }

    }
    public enum Direction
    {
        Inbound, 
        Outbound
    }
}