using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.BankApi.Models
{
    public class ZenithVm
    {
        public string ResponseMessage { get; set; }
        public string ResponseCode { get; set; }
        public BankList BanksList { get; set; }
    }

    public class BankList
    {
        public string BankName { get; set; }
        public string BankSortCode { get; set; }
    }
    public class AccountBalance
    {
        public string ResponseMesage { get; set; }
        public string ResponseCode { get; set; }
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        public string Balance { get; set; }
    }
    public class TransactionStaus
    {
        public string Reference { get; set; }
        public string Amount { get; set; }
        public string TransferChannel { get; set; }
        public string DestAccount { get; set; }
        public string DestBankName { get; set; }
        public string DestBankSortCode { get; set; }
        public string PurposeOfTransfer { get; set; }
        public string FromAccount { get; set; }
        public string FromAccountName { get; set; }
        public string SendeingPartyname { get; set; }
    }
    public class TransactionHistory
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Reference { get; set; }
        public string Amount { get; set; }
        public string FromAccount { get; set; }
        public string TransferChannel { get; set; }
        public string DestAccount { get; set; }
    }
}