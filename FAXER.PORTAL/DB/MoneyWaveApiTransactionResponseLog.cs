using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class MoneyWaveApiTransactionResponseLog
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string TransactionStatus { get; set; }
        public string System_Type { get; set; }
        public string Ref { get; set; }
        public string FlutterResponseMessage { get; set; }
        public string FlutterResponseCode { get; set; }
        public string FlutterReference { get; set; }
        public string LinkingReference { get; set; }
        public string DisburseOrderId { get; set; }
        public string IPR { get; set; }
        public string IPRC { get; set; }
        public string R1 { get; set; }
        public string R2 { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string AccountName { get; set; }
        public string BeneficiaryBankCode { get; set; }
        public string BeneficiaryAccountNumber { get; set; }
        public bool WalletCharged { get; set; }
        public bool Refund { get; set; }
        public bool Reversed { get; set; }
        public string MetaNarration { get; set; }
        public string MetaSender { get; set; }
        public string ReqIp { get; set; }
        public string MoreName { get; set; }
        public string MoreHostName { get; set; }
        public string MorePID { get; set; }
        public string MoreType { get; set; }
        public string MoreId { get; set; }
        public decimal balance { get; set; }
        public int CreatedById{ get; set; }
        public Module Module{ get; set; }
    }
}