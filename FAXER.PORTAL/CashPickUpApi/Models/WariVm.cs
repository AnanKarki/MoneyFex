using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.CashPickUpApi.Models
{
    public class WariVm
    {
        public string requestId { get; set; }
        public string transactionId { get; set; }
        public string sessionId { get; set; }
        public string date { get; set; }

    }
    public class SenderInfoForWariVM
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phoneNumber { get; set; }
        public string adresse { get; set; }
        //Sender country identity card
        public string payspiece { get; set; }
        //Identity card Number
        public string numeropiece { get; set; }
        //Identity card type
        public string typepiece { get; set; }
        public string dateexpirationpiece { get; set; }
        //Card issuing date
        public string datepiece { get; set; }
    }
    public class ReceiverInfoForWariVM
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phoneNumber { get; set; }
        public string adresse { get; set; }
        //receiver country identity card
        public string payspiece { get; set; }
        //Identity card Number
        public string numeropiece { get; set; }
        //Identity card type
        public string typepiece { get; set; }
        public string dateexpirationpiece { get; set; }
        //Card issuing date
        public string datepiece { get; set; }
    }

    public class AmountInfoWariVM : TransactionStatusWariVm
    {
        public decimal Amount { get; set; }
        public string LocalCurrency { get; set; }
        public string CountryDestination { get; set; }
        public string CountrySource { get; set; }
        public decimal DestinationAmount { get; set; }
        public decimal SourceAmount { get; set; }
        public string DestinationCurrency { get; set; }
        public string SourceCurrency { get; set; }
        public string SettelmentCurrency { get; set; }
        public decimal SettelmentAmount { get; set; }
    }
    public class TransactionStatusWariVm 
    {
        public string StatusCode { get; set; }
        public string message { get; set; }
        public string date { get; set; }
        public string codeResponse { get; set; }
        public string payeddate { get; set; }
        public string payerId { get; set; }
        public string status { get; set; }
    }

    public class WariLoginRequestVm 
    {
        public string partnerId { get; set; }
        public string password { get; set; }
        public string login { get; set; }
    }
    public class WariLoginResponseVm : WariVm
    {
        public string codeResponse { get; set; }
        public string message { get; set; }
    }
    public class WariTransactionVm : WariVm
    {
        public string incomingAmount { get; set; }
        public string incomingCurrencyCode { get; set; }
        public string incomingCountryCode { get; set; }
        public string destinationCurrencyCode { get; set; }
        public string destinationAmount { get; set; }
        public string destinationCountryCode { get; set; }
        public string reference { get; set; }
        public string message { get; set; }
        public string exchangeRate { get; set; }
    }

    public class WariPushTransactionRequestVm : WariVm
    {
        public SenderInfoForWariVM sender { get; set; }
        public ReceiverInfoForWariVM receiver { get; set; }
        public WariTransactionVm transaction { get; set; }
    }
    public class WariPushTransactionResponseVm :WariVm
    {
        public string code { get; set; }
        public TransactionStatusWariVm transactionStatus { get; set; }

    }
}