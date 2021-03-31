using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class Transact365ApiResponseTransationLog
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string UId { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string PaymentMethodType { get; set; }
        public string TrackingId { get; set; }
        public string Message { get; set; }
        public bool Test { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
        public string PaidAt { get; set; }
        public string ExpiredAt { get; set; }
        public string ClosedAt { get; set; }
        public string SettledAt { get; set; }
        public string ManuallyCorrectedAt { get; set; }
        public string Language { get; set; }
        public string RedirectUrl { get; set; }
        public string Holder { get; set; }
        public string Brand { get; set; }
        public string Last4DigitCardNum { get; set; }
        public string First1DigitCardNum { get; set; }
        public string Bin { get; set; }
        public string IssuerCountry { get; set; }
        public string IssuerName { get; set; }
        public string Product { get; set; }
        public int ExpMonth { get; set; }
        public int ExpYear { get; set; }
        public string TokenProvider { get; set; }
        public string Token { get; set; }
        public string ReceiptUrl { get; set; }
        public string TransationId { get; set; }
        public string ThreeDStatus { get; set; }
        public string ThreeDMessage { get; set; }
        public string ThreeDVeStatus { get; set; }
        public string ThreeDAcsUrl { get; set; }
        public string ThreeDPaReq { get; set; }
        public string ThreeDMd { get; set; }
        public string ThreeDPaResUrl { get; set; }
        public string ThreeDECI { get; set; }
        public string ThreeDPaStatus { get; set; }
        public string ThreeDXId { get; set; }
        public string ThreeDCavv { get; set; }
        public string ThreeDCavvAlgorithm { get; set; }
        public string ThreeDFailReason { get; set; }
        public string BillingAddressFirstName { get; set; }
        public string BillingAddressLastName { get; set; }
        public string BillingAddressAddress { get; set; }
        public string BillingAddressCountry { get; set; }
        public string BillingAddressCity { get; set; }
        public string BillingAddressZip { get; set; }
        public string BillingAddressState { get; set; }
        public string BillingAddressPhone { get; set; }
        public Module Module { get; set; }  
    }
}
