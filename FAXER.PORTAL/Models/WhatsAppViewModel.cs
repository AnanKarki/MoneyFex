using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class WhatsAppViewModel
    {
        public int Id { get; set; }
        public string To { get; set; }
        public string From { get; set; }
        public string Body { get; set; }
        public string MessageSId { get; set; }
        public string NumMedia { get; set; }
        public string FileURL { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public Direction DirectionEnum { get; set; }
        public string Direction { get; set; }
        public DateTime SentDate { get; set; }

    }
    public class MessageViewModel
    {
        public string SenderName { get; set; }
        public string SentMessageWithTime { get; set; }
        public string ReceivedMessageWithTime { get; set; }
        public string FileURL { get; set; }
        public DateTime DateTime { get; set; }

    }
    public class ReceiverViewModel
    {
        public int Id { get; set; }
        public string ReceiverName { get; set; }
        public string PhoneNumber { get; set; }
     
    }

}