using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class KiiPayPersonalToBankAccountTransfer
    {
        public int Id { get; set; }
        public int KiiPayPersonalWalletId { get; set; }
        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }
        public string AccountOwnerName { get; set; }
        public string BankAccountNumber { get; set; }
        public string AccountHolderPhoneNo { get; set; }
        public int BankId { get; set; }
        public int BankBranchId { get; set; }
        public string BankBranchCode { get; set; }
        public decimal SendingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public decimal Fee { get; set; }
        public decimal SmsFee { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal TotalAmount { get; set; }
        public PaymentType PaymentType { get; set; }
        public DateTime TransactionDate { get; set; }

        public virtual KiiPayPersonalWalletInformation KiiPayPersonalWallet { get; set; }
    }
}