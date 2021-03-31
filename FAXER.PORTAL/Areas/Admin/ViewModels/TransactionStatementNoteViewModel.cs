using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class TransactionStatementNoteViewModel
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public int SenderId { get; set; }
        public TransactionServiceType TransactionMethod { get; set; }
        public string TransactionMethodName { get; set; }
        public string Note { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedTime { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public bool IsRead { get; set; }
        public NoteType NoteType { get; set; }
    }
}