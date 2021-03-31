using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.BankApi.Models
{
    public class RefundRequestViewModel
    {
        public string BaseAmount { get; set; }
        public string ChargeDescription { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string OrderReference { get; set; }
        public string ParentTransactionReference { get; set; }
        public string[] RequestTypeDescriptions { get; set; }
        public string SiteReference { get; set; }
    }

    public class RefundResponse
    {
        public string requestreference { get; set; }
        public string version { get; set; }
        public RefundRequestViewModel Request { get; set; }
        public List<RefundResponseResult> Response { get; set; }

    }
    public class RefundResponseResult
    {
        public string TransactionStartedTimeStamp { get; set; }
        public string ParentTransactionReference { get; set; }
        public int LiveStatus { get; set; }
        public string Issuer { get; set; }
        public int DccEnabled { get; set; }
        public string SettleDueDate { get; set; }
        public int ErrorCode { get; set; }
        public string OrderReference { get; set; }
        public string TransactionId { get; set; }
        public string MerchantNumber { get; set; }
        public string MerchantCountryiso2a { get; set; }
        public string TransactionReference { get; set; }
        public string MerchantName { get; set; }
        public string PaymentTypeDescription { get; set; }
        public decimal BaseAmount { get; set; }
        public string AccountTypeDescription { get; set; }
        public string AcquirerResponseCode { get; set; }
        public string RequestTypeDescription { get; set; }
        public string SecurityResponseSecurityCode { get; set; }
        public string Currencyiso3a { get; set; }
        public string AuthCode { get; set; }
        public string ErrorMessage { get; set; }
        public string SecurityResponsePostCode { get; set; }
        public string Maskedpan { get; set; }
        public string SecurityResponseAddress { get; set; }
        public string IssuerCountryiso2a { get; set; }
        public string OperatorName { get; set; }
        public string SettleStatus { get; set; }
    }
}