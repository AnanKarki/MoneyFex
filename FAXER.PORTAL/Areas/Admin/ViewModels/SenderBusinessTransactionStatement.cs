using FAXER.PORTAL.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class SenderBusinessTransactionStatement
    {
        public int Id { get; set; }
        public TransactionServiceType TransactionServiceType { get; set; }
        public string Method { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string Amount { get; set; }
        public string Fee { get; set; }
        public string Identifier { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public int TotalCount { get; set; }
        public DateTime TransactionDate { get; set; }

    }

    public class SenderBusinessTransactionStatementWithSenderDetails
    {
        public List<SenderBusinessTransactionStatement> SenderBusinessTransactionStatement { get; set; }
        public string SenderName { get; set; }
        public string SenderAccountNumber { get; set; }
        public string SenderCountry { get; set; }
    }


}