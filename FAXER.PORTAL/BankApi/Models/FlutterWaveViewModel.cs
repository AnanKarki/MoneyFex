using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.BankApi.Models
{
    public class FlutterWaveViewModel
    {

        public class FlutterRequestDataVm
        {


            public string PBFPubKey { get; set; }
            public string client { get; set; }
            public string alg { get; set; }

            //for validation
            public string transactionreference { get; set; }
            public string otp { get; set; }
            public bool use_access { get; set; }

            //for verification

            public string txref { get; set; }
            public string SECKEY { get; set; }

        }

        #region Webhook
        /// <summary>
        /// used by mobile money
        /// </summary>
        public class WebHookResposeVm
        {
            public int id { get; set; }
            public string txRef { get; set; }
            public string flwRef { get; set; }
            public string orderRef { get; set; }
            public string paymentPlan { get; set; }
            public string createdAt { get; set; }
            public decimal amount { get; set; }
            public decimal charged_amount { get; set; }
            public string status { get; set; }
            public string IP { get; set; }
            public string currency { get; set; }

            public FlutterCustomerVm customer { get; set; }
            public EntityVm entity { get; set; }

            public string event_type { get; set; }


        }

        public class TransferWebhookRequestVm
        {
            public string event_type { get; set; }
            public FultterTransferVm transfer { get; set; }
        }


        public class FultterTransferVm
        {
            public int id { get; set; }
            public string account_number { get; set; }
            public string bank_code { get; set; }
            public string fullname { get; set; }
            public string date_created { get; set; }
            public string currency { get; set; }
            public decimal amount { get; set; }
            public decimal fee { get; set; }
            public string status { get; set; }
            public string reference { get; set; }
            public string narration { get; set; }
            public string approver { get; set; }
            public string complete_message { get; set; }
            public bool requires_approval { get; set; }
            public bool is_approved { get; set; }
            public string bank_name { get; set; }
        }


        public class EntityVm
        {
            public int id { get; set; }
            //For Account
            public string account_number { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }

            //For Recurring Payment

            public string card6 { get; set; }
            public string card_last4 { get; set; }


        }



        #endregion

        /// <summary>
        /// Customer details used for bank account and mobile
        /// </summary>
        public class FlutterCommonCustomerDetailsVm
        {


            public string PBFPubKey { get; set; }
            public string accountbank { get; set; }
            public string accountnumber { get; set; }
            public string currency { get; set; }
            public string payment_type { get; set; }
            public string country { get; set; }
            public string amount { get; set; }
            public string email { get; set; }
            public string passcode { get; set; }
            public string bvn { get; set; }
            public string phonenumber { get; set; }
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string IP { get; set; }
            public string txRef { get; set; }

            public string device_fingerprint { get; set; }
            public string redirect_url { get; set; }
            public string ip { get; set; }
            public bool is_bank_transfer { get; set; }
            //for Mobile
            public int is_mobile_money_gh { get; set; }
            public string orderRef { get; set; }
            public string voucher { get; set; }
            public string network { get; set; }

            public List<FlutterMetaVM> meta { get; set; }

        }

        public class FlutterMetaVM
        {
            public string metaname { get; set; }
            public string metavalue { get; set; }

        }

        /// <summary>
        /// Response For charge and validate 
        /// </summary>
        public class FlutterCommonResponseVm
        {
            public string status { get; set; }
            public string message { get; set; }
            public FlutterCommonResponseDataVm data { get; set; }
            public string[] meta { get; set; }

        }
        ///// <summary>
        /////  verify response for bank and mobile
        ///// </summary>
        //public class FlutterCommonVerifyResponseVm
        //{
        //    public string status { get; set; }
        //    public string message { get; set; }
        //    public FlutterCommonVerifyResponseDataVm data { get; set; }
        //    public string[] meta { get; set; }

        //}

        public class FlutterCommonVerifyResponseDataVm
        {
            public int txid { get; set; }
            public string txref { get; set; }
            public string flwref { get; set; }
            public string devicefingerprint { get; set; }
            public string cycle { get; set; }
            public decimal amount { get; set; }
            public string currency { get; set; }
            public decimal chargedamount { get; set; }
            public decimal appfee { get; set; }
            public decimal merchantfee { get; set; }
            public decimal merchantbearsfee { get; set; }
            public string chargecode { get; set; }
            public string chargemessage { get; set; }
            public string authmodel { get; set; }
            public string ip { get; set; }
            public string narration { get; set; }
            public string status { get; set; }
            public string vbvcode { get; set; }
            public string vbvmessage { get; set; }
            public string authurl { get; set; }
            public string acctcode { get; set; }
            public string acctmessage { get; set; }
            public string paymenttype { get; set; }
            public string paymentid { get; set; }
            public string fraudstatus { get; set; }
            public string chargetype { get; set; }
            public int createdday { get; set; }
            public string createddayname { get; set; }
            public int createdweek { get; set; }
            public int createdmonth { get; set; }
            public string createdmonthname { get; set; }
            public int createdquarter { get; set; }
            public int createdyear { get; set; }
            public bool createdyearisleap { get; set; }
            public int createddayispublicholiday { get; set; }
            public int createdhour { get; set; }
            public int createdminute { get; set; }
            public string createdpmam { get; set; }
            public string created { get; set; }
            public int customerid { get; set; }
            public string custphone { get; set; }
            public string custnetworkprovider { get; set; }
            public string custname { get; set; }
            public string custemail { get; set; }
            public string custemailprovider { get; set; }
            public string custcreated { get; set; }
            public int accountid { get; set; }
            public string acctbusinessname { get; set; }
            public string acctcontactperson { get; set; }
            public string acctcountry { get; set; }
            public int acctbearsfeeattransactiontime { get; set; }
            public int acctparent { get; set; }
            public string acctvpcmerchant { get; set; }
            public string acctalias { get; set; }
            public bool acctisliveapproved { get; set; }
            public string orderref { get; set; }
            public string paymentplan { get; set; }
            public string paymentpage { get; set; }
            public string raveref { get; set; }
            public FlutterAccountVm account { get; set; }

            //For Mobile 
            public decimal amountsettledforthistransaction { get; set; }




        }


        public class FlutterAccountVm
        {
            public int id { get; set; }
            public string account_number { get; set; }
            public string account_bank { get; set; }
            public string first_name { get; set; }
            public string last_name { get; set; }
            public bool account_is_blacklisted { get; set; }
            public string createdAt { get; set; }
            public string updatedAt { get; set; }
            public string deletedAt { get; set; }
            public FlutterAccountTokenVm account { get; set; }
        }

        public class FlutterAccountTokenVm
        {
            public string token { get; set; }
        }

        public class InitailChargeRequestForGTBAccountPaymentVm
        {

            public string PBFPubKey { get; set; }
            public string accountbank { get; set; }
            public string currency { get; set; }
            public string payment_type { get; set; }
            public string country { get; set; }
            public string amount { get; set; }
            public string email { get; set; }
            public string phonenumber { get; set; }
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string IP { get; set; }
            public string redirect_url { get; set; }
            public string txRef { get; set; }
            public string device_fingerprint { get; set; }
        }

        public class FlutterCommonResponseDataVm
        {
            public int id { get; set; }
            public string txRef { get; set; }
            public string orderRef { get; set; }
            public string flwRef { get; set; }
            public string redirectUrl { get; set; }
            public string device_fingerprint { get; set; }
            public string settlement_token { get; set; }
            public string cycle { get; set; }
            public decimal amount { get; set; }
            public decimal charged_amount { get; set; }
            public decimal appfee { get; set; }
            public decimal merchantfee { get; set; }
            public decimal merchantbearsfee { get; set; }
            public string chargeResponseCode { get; set; }
            public string raveRef { get; set; }
            public string chargeResponseMessage { get; set; }
            public string authModelUsed { get; set; }
            public string currency { get; set; }
            public string IP { get; set; }
            public string narration { get; set; }
            public string status { get; set; }
            public string modalauditid { get; set; }
            public string vbvrespmessage { get; set; }
            public string authurl { get; set; }
            public string vbvrespcode { get; set; }
            public string acctvalrespmsg { get; set; }
            public string acctvalrespcode { get; set; }
            public string paymentType { get; set; }
            public string paymentPlan { get; set; }
            public string paymentPage { get; set; }
            public string paymentId { get; set; }
            public string fraud_status { get; set; }
            public string charge_type { get; set; }
            public bool is_live { get; set; }
            public string retry_attempt { get; set; }
            public string getpaidBatchId { get; set; }
            public string createdAt { get; set; }
            public string updatedAt { get; set; }
            public string deletedAt { get; set; }
            public int customerId { get; set; }
            public int AccountId { get; set; }

            public FlutterCustomerVm customer { get; set; }
            public ValidateInstructionsVm validateInstructions { get; set; }
        }

        public class FlutterCustomerVm
        {
            public int id { get; set; }
            public string phone { get; set; }
            public string fullName { get; set; }
            public string customertoken { get; set; }
            public string email { get; set; }
            public string createdAt { get; set; }
            public string updatedAt { get; set; }
            public string deletedAt { get; set; }
            public int AccountId { get; set; }


        }

        public class ValidateInstructionsVm
        {
            public string[] valparams { get; set; }
            public string instruction { get; set; }
        }



        public class FWReponse<t>
        {
            public t data { get; set; }
            public string status { get; set; }
            public string message { get; set; }

        }
        public class FWAllTransactionReponse<t>
        {
            public string status { get; set; }
            public string message { get; set; }
            public FlutterWavemeta meta { get; set; }
            public List<t> data { get; set; }
        }

        public class FlutterWavemeta
        {

            public page_info page_info { get; set; }

        }

        public class page_info
        {
            public int total { get; set; }
            public int current_page { get; set; }
            public int total_pages { get; set; }
        }
        public class FlutterWaveVm
        {

            public string account_bank { get; set; }
            public string account_number { get; set; }
            public decimal amount { get; set; }
            public string narration { get; set; }
            public string currency { get; set; }
            public string reference { get; set; }
            public string callback_url { get; set; }
            public string debit_currency { get; set; }
            public string bank_code { get; set; }
            public FlutterWaveReceiverVM meta { get; set; }

        }
        public class FlutterWaveAllTransactionResonse
        {
            public int id { get; set; }
            public string account_number { get; set; }
            public string bank_code { get; set; }
            public string full_name { get; set; }
            public string created_at { get; set; }
            public string currency { get; set; }
            public string debit_currency { get; set; }
            public string amount { get; set; }
            public string fee { get; set; }
            public string status { get; set; }
            public string reference { get; set; }
            //public FlutterWaveReceiverVM meta { get; set; }
            public string narration { get; set; }
            public string complete_message { get; set; }
            public bool requires_approval { get; set; }
            public bool is_approved { get; set; }
            public string bank_name { get; set; }
            public string approver { get; set; }
        }

        public class FlutterWaveReceiverVM
        {

            public string first_name { get; set; }
            public string last_name { get; set; }
            public string email { get; set; }
            public string mobile_number { get; set; }

        }

        public class FlutterWaveResonse
        {
            public int id { get; set; }
            public string account_number { get; set; }
            public string bank_code { get; set; }
            public string full_name { get; set; }
            public string created_at { get; set; }
            public string currency { get; set; }
            public string debit_currency { get; set; }
            public string amount { get; set; }
            public string fee { get; set; }
            public string status { get; set; }
            public string reference { get; set; }
            public FlutterWaveReceiverVM meta { get; set; }
            public string narration { get; set; }
            public string complete_message { get; set; }
            public bool requires_approval { get; set; }
            public bool is_approved { get; set; }
            public string bank_name { get; set; }
            public string approver { get; set; }
        }

        public class FlutterWaveBulkRequestVm
        {
            public string title { get; set; }
            public List<FlutterWaveVm> bulk_data { get; set; }
        }
        public class FlutterWaveRateResponseVm
        {
            public decimal rate { get; set; }
            public CurrencyAndRateVm source { get; set; }
            public CurrencyAndRateVm destination { get; set; }
        }

        public class CurrencyAndRateVm
        {
            public string currency { get; set; }
            public string amount { get; set; }
        }
    }
}