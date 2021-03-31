using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.BankApi.Models
{
    public class MoneyWaveViewModel
    {
        public class AuthRequestViewModel
        {
            public string apiKey { get; set; }
            public string secret { get; set; }
        }
        public class AuthResponseViewModel
        {
            public string status { get; set; }
            public string token { get; set; }
            public AmountLimit config { get; set; }
        }
        public class AmountLimit
        {
            public int daily_limit { get; set; }
            public int minimum_limit { get; set; }
            public int maximum_limit { get; set; }
            public int merchant_fee { get; set; }
        }
        public class ResolveAccountRequestVM
        {
            public string currency { get; set; }
            public string bank_code { get; set; }
            public string account_number { get; set; }

        }

        public class AccountOwner
        {
            public string account_name { get; set; }
        }
        public class TransactionRequestViewModel
        {
            public string accountNumber { get; set; }
            public string currency { get; set; }
            public string bankcode { get; set; }
            public string senderName { get; set; }
            public string amount { get; set; }
            public string narration { get; set; }
            public string @ref { get; set; }
            public string @lock { get; set; }
            public bool cashpickup { get; set; }
            public string x_routing_number { get; set; }
            public string x_recipient_address { get; set; }
            public string x_recipient_country { get; set; }
            public string countryTag { get; set; }
            public string x_recipient_email { get; set; }
            public string x_recipient_phone { get; set; }
            public string x_recipient_name { get; set; }
            public string branchCode { get; set; }
            public string walletUref { get; set; }
        }
        public class TransactionResponseViewModel
        {
            public string responsemessage { get; set; }
            public string responsecode { get; set; }
            public string uniquereference { get; set; }
            public string internalreference { get; set; }
            public string @ref { get; set; }
        }
        public class TransactionResponseVm
        {
            public string status { get; set; }
            public TransactionResponseViewModel data { get; set; }
        }

        public class TransactionStatusResquestVm
        {
            public string @ref { get; set; }
        }
        public class TransactionStatusResponseVm
        {
            public int id { get; set; }
            public decimal amount { get; set; }
            public string status { get; set; }
            public string system_type { get; set; }
            public string @ref { get; set; }
            public string flutterResponseMessage { get; set; }
            public string flutterResponseCode { get; set; }
            public string flutterReference { get; set; }
            public string linkingReference { get; set; }
            public string disburseOrderId { get; set; }
            public string ipr { get; set; }
            public string iprc { get; set; }
            public string r1 { get; set; }
            public string r2 { get; set; }
            public string meta { get; set; }
            public string createdAt { get; set; }
            public string updatedAt { get; set; }
            public BeneficiaryDetails beneficiary { get; set; }
            public bool walletCharged { get; set; }
            public bool refund { get; set; }
            public bool reversed { get; set; }

            public MetaDetails meta_Result { get; set; }

        }

        public class TransactionStatusResponseVm_Result : TransactionStatusResponseVm
        {
            public MetaDetails meta_Result { get; set; }
        }

        public class BeneficiaryDetails
        {
            public string accountNumber { get; set; }
            public string bankCode { get; set; }
            public string accountName { get; set; }

        }
        public class MetaDetails
        {
            public string narration { get; set; }
            public string sender { get; set; }
            public req req { get; set; }
            public decimal balance { get; set; }
        }
        public class req
        {
            public string ip { get; set; }
            public More more { get; set; }
        }
        public class More
        {
            public string name { get; set; }
            public string hostname { get; set; }
            public string pid { get; set; }
            public string type { get; set; }
            public string id { get; set; }

        }
        public class ResponseVm<t>
        {
            public string status { get; set; }
            public string message { get; set; }
            public string code { get; set; }
            public t data { get; set; }
        }


    }

}