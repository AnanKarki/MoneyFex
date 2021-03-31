using FAXER.PORTAL.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class NewSenderTransactionStatementListVm
    {

        public int Id { get; set; }
        public int TranasactionId { get; set; }
        public string Receiver { get; set; }
        public string ReceiptNo { get; set; }

        public string Date { get; set; }
        public DateTime DateTime { get; set; }

        public string Last4Digits { get; set; }
        public string CardIssuer { get; set; }
        public decimal Amount { get; set; }
        public string CountryCurrency { get; set; }
        public string ReceivingCurrency { get; set; }
        public decimal Fee { get; set; }
        public string BillingAddress { get; set; }
        public string Status { get; set; }


    }

    public class MonthlyTransactionMeter
    {
        public string Currency { get; set; }
        public string Jan { get; set; }
        public string Feb { get; set; }
        public string March { get; set; }
        public string April { get; set; }
        public string May { get; set; }
        public string Jun { get; set; }
        public string July { get; set; }
        public string Aug { get; set; }
        public string Sep { get; set; }
        public string Oct { get; set; }
        public string Nov { get; set; }
        public string Dec { get; set; }

    }
    public class NewSenderTransactionViewModel
    {

        public MonthlyTransactionMeter Monthly { get; set; }
        public IPagedList<NewSenderTransactionStatementListVm> TransactionList { get; set; }
        public List<NewSenderTransactionStatementListVm> TransactionListDownload { get; set; }
    }
}