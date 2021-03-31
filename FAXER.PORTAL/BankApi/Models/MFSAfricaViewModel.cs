using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.BankApi.Models
{
    public class MFSAfricaViewModel
    {
        /// <summary>
        /// For Cancel trans Request , get Trans Status Request ,trans_com request
        /// </summary>
        public class TransRequestParam
        {
            public string corporate_code { get; set; }
            public string password { get; set; }
            public string trans_id { get; set; }
        }

        /// <summary>
        /// For Cancel trans Response , get Trans Status Response and trans_com Response
        /// </summary>
        public class TransResponseParam
        {

            public string code { get; set; }
            public string e_trans_id { get; set; }
            public string message { get; set; }
            public string mfs_trans_id { get; set; }
            public string third_party_trans_id { get; set; }
        }

        public class GetVoucherStatusRequestParam
        {

            public string corporate_code { get; set; }
            public string Password { get; set; }
            public string mfs_trans_id { get; set; }
            public string voucher_code { get; set; }
            public string voucher_partner { get; set; }
            //<URL> urlConfirmation </URL>
            public string urlConfirmation { get; set; }

        }

        public class GetVoucherStatusResponseParam
        {
            public string mfs_trans_id { get; set; }
            public string Voucher_code { get; set; }
            public string receive_partner { get; set; }
            public MFSAfricaStatusVm status { get; set; }

        }

        /// <summary>
        /// For Status 
        /// </summary>
        public class MFSAfricaStatusVm
        {
            public string status_code { get; set; }
            public string code { get; set; }
            public string message { get; set; }
        }

        /// <summary>
        /// For account_request request, for get_bank request,for get Partners
        /// </summary>
        public class AccountRequestParam
        {

            public string corporate_code { get; set; }
            public string password { get; set; }
            public string to_country { get; set; }
            public string msisdn { get; set; }
        }

        /// <summary>
        /// For account_request response
        /// </summary>
        public class AccountRequestResponseParam
        {
            public string msisdn { get; set; }
            public string partner_code { get; set; }
            public MFSAfricaStatusVm status { get; set; }

        }

        public class GetBankResponseParam
        {
            public BankLimitVm bank_limit { get; set; }

            public string bank_name { get; set; }
            public string bic { get; set; }
            public string country_code { get; set; }
            public string currency_code { get; set; }
            public string dom_bank_code { get; set; }
            public string iban { get; set; }
            public string mfs_bank_code { get; set; }
        }

        public class BankLimitVm
        {
            public decimal min_per_tx_limit { get; set; }
            public decimal max_per_tx_limit { get; set; }
            public decimal max_daily_value { get; set; }
            public decimal max_weekly_value { get; set; }
            public decimal max_monthly_value { get; set; }

        }


        public class GetPartnersResponseParam
        {

            public string country_code { get; set; }
            public string currency_code { get; set; }
            public BankLimitVm limits { get; set; }
            public string partner_code { get; set; }
            public string parter_name { get; set; }

        }


        public class GetRateRequestParam
        {
            public string corporate_code { get; set; }
            public string password { get; set; }
            public string to_country { get; set; }
            public string from_currency { get; set; }
            public string to_currency { get; set; }
        }

        public class GetRateResponseParam
        {
            public string from_currency { get; set; }
            public decimal fx_rate { get; set; }
            public string partner_code { get; set; }
            public DateTime time_stamp { get; set; }
            public string to_currency { get; set; }
        }


        /// <summary>
        /// For Mm_trans_log request and Bank_trans_Log request
        /// </summary>
        public class MmAndBankTransLogRequestParam
        {

            public string corporate_code { get; set; }
            public string password { get; set; }
            public SendAmountVm send_amount { get; set; }
            public SendAmountVm fee { get; set; }
            public SenderRecipientDetailVm sender { get; set; }
            public SenderRecipientDetailVm recipient { get; set; }
            public BankAccountVm account { get; set; }
            public string third_party_trans_id { get; set; }
            public char reference { get; set; }
          
        }

        /// <summary>
        /// for send_amount and fee
        /// </summary>
        public class SendAmountVm
        {
            public decimal amount { get; set; }
            public string currency_code { get; set; }
        }

        public class SenderRecipientDetailVm
        {
            public string address { get; set; }
            public string city { get; set; }
            public DateTime date_of_birth { get; set; }
            public SenderRecipientDocumentDetailVm document { get; set; }
            public string email { get; set; }
            public string from_country { get; set; }
            public string msisdn { get; set; }
            public string name { get; set; }
            public string postal_code { get; set; }
            public string state { get; set; }

            public MFSAfricaStatusVm status { get; set; }
            public string surname { get; set; }
            public string to_country { get; set; }
   

        }

        public class SenderRecipientDocumentDetailVm
        {

            public string id_number { get; set; }
            public string id_type { get; set; }
            public string id_country { get; set; }
            public DateTime id_expiry { get; set; }

        }
        /// <summary>
        /// For Mm_trans_log  response and Bank_trans_Log response
        /// </summary>
        public class MmTransLogResponseParam
        {
            public decimal fx_rate { get; set; }
            public string mfs_trans_id { get; set; }
            public string name_match { get; set; }
            public string partner_code { get; set; }
            public  SendAmountVm receive_amount { get; set; }
            public  string sanction_list_recipient { get; set; }
            public  string sanction_list_sender { get; set; }
            public  string third_party_trans_id { get; set; }
            public  MFSAfricaStatusVm status { get; set; }
         
        }
        public class BankAccountVm
        {
            public string account_number { get; set; }
            public string mfs_bank_code { get; set; }
        }
 
    }
}