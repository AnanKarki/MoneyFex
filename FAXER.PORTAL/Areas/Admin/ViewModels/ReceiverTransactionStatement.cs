using FAXER.PORTAL.DB;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ReceiverTransactionStatement
    {
        public int TransactionId { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string SenderName { get; set; }
        public string SenderFirstName { get; set; }
        public string SenderMiddleName { get; set; }
        public string SenderLastName { get; set; }
        public string SenderFullName
        {
            get
            {
                return SenderFirstName + " " + (string.IsNullOrEmpty(SenderMiddleName) ? "" : SenderMiddleName + " ") + SenderLastName;
            }
        }
        public string ReceiverName { get; set; }
        public string ReceiverCountry { get; set; }
        public string ReceiverCountryCurrency { get; set; }
        public Service Service { get; set; }
        public string ServiceType { get; set; }
        public decimal Amount { get; set; }
        public decimal Fee { get; set; }
        public string Identifier { get; set; }
        public DateTime DateTime { get; set; }
        public string BanKWalletNumber { get; set; }
        public string BanKWalletName { get; set; }
        public string Status { get; set; }

    }
    public class NewReceiverTransactionStatement
    {
        public IPagedList<ReceiverTransactionStatement> TransactionList { get; set; }
        public MonthlyTransactionMeter Monthly { get; set; }
    }
}