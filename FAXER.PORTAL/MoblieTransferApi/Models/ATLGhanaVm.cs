using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.MoblieTransferApi.Models
{
    public class ATLGhanaVm
    {
        public string Message { get; set; }
        public TransactionVm Transcation { get; set; }
        public int CurrentTransactions { get; set; }
        public int TotalTransactions { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
       
    }

    public class TransactionVm
    {
        public int Id { get; set; }
        public string FromCountry { get; set; }
        public string FromCurrency { get; set; }
        public decimal SendAmount { get; set; }
        public string ToCountry { get; set; }
        public string ToCurrency { get; set; }
        public decimal PayoutAmount { get; set; }
        public string PayoutMethod { get; set; }
        public string PayoutPartner { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal Fee { get; set; }
        public string SettlementCurrency { get; set; }
        public decimal SettlementAmount { get; set; }
        public decimal Commission { get; set; }
        public decimal TotalSettlement { get; set; }
        public string DeliveryReference { get; set; }
        public string ThirdPartyReference { get; set; }
        public CustomerVm Customer { get; set; }
        public RecipientVm Recipient { get; set; }
        public string Status { get; set; }
        public string Purpose { get; set; }
        public string PoiDocument { get; set; }
        public string PoiIdNumber { get; set; }
        public DateTime PoiValidFrom { get; set; }
        public DateTime PoiExpiry { get; set; }
        public string Message { get; set; }
        public DateTime CreatedOn { get; set; }

    }
    public class CustomerVm
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string BirthCountry { get; set; }
        public string Nationality { get; set; }
        public string BirthNationality { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress { get; set; }

    }

    public class RecipientVm
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Type { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostCode { get; set; }
        public string Email { get; set; }
        public string Relation { get; set; }

    }

}