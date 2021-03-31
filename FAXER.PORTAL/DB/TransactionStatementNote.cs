using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class TransactionStatementNote
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public int SenderId { get; set; }
        public TransactionServiceType TransactionMethod { get; set; }
        public string Note { get; set; }
        public DateTime CreatedDateAndTime { get; set; }
        public int CreatedBy { get; set; }
        public NoteType NoteType { get; set; }
        public bool IsRead { get; set; }
    }

    public enum NoteType
    {
        TransactionStatementNote,
        SenderDocumentation
    }
}