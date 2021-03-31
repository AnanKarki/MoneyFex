using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderWalletTransactionStatementVM
    {

        public SenderWalletTransactionStatementMasterVM SenderWalletTransactionStatementMaster { get; set; }
        public List<WalletStatementList> SenderWalletTransactionStatementDetail { get; set; }
    }

    public class SenderWalletTransactionStatementMasterVM
    {
        public int Id { get; set; }
        public string Currency { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal AvailableBalanceCents { get; set; }
        public int WalletId { get; set; }
        public int SenderWalletStatementTransactionType { get; set; }

        public string Country { get; set; }
        public FilterByStatus FilterKey { get; set; }

    }

    public enum FilterByStatus {

        All,
        In,

        Out
    }
    public class SenderWalletTransactionStatementDetailVM
    {
      

        public int Id { get; set; }
        public string Date { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string AccountNumber { get; set; }
        public string Reference { get; set; }
        public string Gross { get; set; }
        public string Fee { get; set; }
        public string Net { get; set; }
        public decimal NetDecimal { get; set; }
        public string Balance { get; set; }
        public decimal BalanceDecimal { get; set; }
        public KiiPayPersonalWalletPaymentType PaymentType { get; set; }
        public bool IsRefunded { get; set; }
        public WalletStatmentStatus InOut { get; set; }

        public string SendingCurrency { get; set; }

    }

}