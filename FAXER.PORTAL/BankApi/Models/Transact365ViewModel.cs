using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.BankApi.Models
{
    public class Transact365ViewModel
    {

        /// <summary>
        /// used for token
        /// </summary>
        public class T365CheckoutRequestParamVm
        {
            public T365CheckOutRequestDataVm checkout { get; set; }

        }

        public class T365CheckoutResponseParaVm
        {
            public string token { get; set; }
            public string redirect_url { get; set; }
        }

        /// <summary>
        /// use t as T365RequestDataVm, T365CapturesAndRefundsRequestVm,T365BalanceRequestVm
        /// </summary>
        /// <typeparam name="t"> </typeparam>
        public class T365RequestVm<t>
        {
            public t request { get; set; }
        }

        /// <summary>
        /// use T365ReponseTransationVm ,T365ResponseDataVm,T365BalanceReponseVm
        /// </summary>
        /// <typeparam name="t"></typeparam>
        public class T365ResponseVm<t>
        {
            public t transaction { get; set; }

        }
        public class T365TransactionsResponseVm<t>
        {
            public List<t> transactions { get; set; }

        }
        public class T365SettingVm
        {
            public string return_url { get; set; }
            public string success_url { get; set; }
            public string decline_url { get; set; }
            public string fail_url { get; set; }
            public string cancel_url { get; set; }
            public string notification_url { get; set; }
            public string button_text { get; set; }
            public string language { get; set; }

        }
        public class T365CustomerFieldsVm
        {

            public string[] visible { get; set; }
            public string[] read_only { get; set; }
        }
        public class T365CreditCardFieldsVm
        {
            public string[] holder { get; set; }
            public string[] read_only { get; set; }
        }
        public class T365OrderVm
        {
            public string currency { get; set; }
            public decimal amount { get; set; }
            public string description { get; set; }

        }
        public class T365CheckOutRequestDataVm
        {

            public string version { get; set; }
            public string test { get; set; }
            public string transaction_type { get; set; }
            public int attempts { get; set; }
            public T365CustomerFieldsVm customer_fields { get; set; }
            public T365CreditCardFieldsVm credit_card_fields { get; set; }
            public T365OrderVm order { get; set; }
            public T365BillingAddressVm customer { get; set; }



        }
        public class T365BalanceRequestVm
        {
            public string account { get; set; }
        }
        public class T365BalanceReponseVm
        {

            public string status { get; set; }
            public T365BalanceResultVm result { get; set; }
        }
        public class T365BalanceResultVm
        {
            public string Account { get; set; }
            public string Amount { get; set; }
            public T365AccountBalanceVM Balance { get; set; }
        }
        public class T365AccountBalanceVM
        {
            public string OperDate { get; set; }
            public decimal Credit { get; set; }
            public decimal CreditRub { get; set; }
            public decimal Debit { get; set; }
            public decimal DebitRub { get; set; }
            public decimal AmountIn { get; set; }
            public decimal AmountInRub { get; set; }
            public decimal AmountOut { get; set; }
            public decimal AmountOutRub { get; set; }

        }

        public class T365RequestMasterVm
        {

            public T365RequestDataVm request { get; set; }

        }
        public class T365RequestDataVm
        {

            public string amount { get; set; }
            public string currency { get; set; }
            public string description { get; set; }
            public string tracking_id { get; set; }
            public string return_url { get; set; }
            public string language { get; set; }

            public bool test { get {

                    return bool.Parse(Common.Common.GetAppSettingValue("Transact365TransactionMode"));
                } }
            public T365BillingAddressVm billing_address { get; set; }
            public T365CreditCardVm credit_card { get; set; }
            public T365CustomerVm customer { get; set; }


        }
        public class T365CapturesAndRefundsRequestVm
        {

            public decimal amount { get; set; }
            //only for captures
            public string parent_uid { get; set; }

            //only for refunds

            public string reason { get; set; }
        }
        public class T365BillingAddressVm
        {
            public string first_name { get; set; }
            public string last_name { get; set; }
            public string country { get; set; }
            public string city { get; set; }
            public string state { get; set; }
            public string zip { get; set; }
            public string address { get; set; }
            public string phone { get; set; }
        }
        public class T365CreditCardVm
        {
            public string number { get; set; }
            public string verification_value { get; set; }
            public string holder { get; set; }
            public string exp_month { get; set; }
            public string exp_year { get; set; }
            public string token { get; set; }



        }
        public class T365CreditCardResponseVm
        {
            public string holder { get; set; }
            public string stamp { get; set; }
            public string brand { get; set; }
            public string product { get; set; }
            public string token_provider { get; set; }
            public string token { get; set; }
            public string first_1 { get; set; }
            public string last_4 { get; set; }
            public int exp_month { get; set; }
            public int exp_year { get; set; }
            public string issuer_country { get; set; }
        }
        public class T365CustomerVm
        {
            public string ip { get; set; }
            public string email { get; set; }


            //For Response
            public string device_id { get; set; }

        }
        public class T365ReponseTransationVm
        {

            public string uid { get; set; }
            public string status { get; set; }
            public string message { get; set; }
            public decimal amount { get; set; }
            public string parent_uid { get; set; }
            public string receipt_url { get; set; }
            public string currency { get; set; }
            public string type { get; set; }
            public bool test { get; set; }

            //  public T365ResponseAsperTransation void { get; set; }

            public T365ResponseAsperTransation refund { get; set; }
        }
        public class T365ResponseAsperTransation
        {
            public string message { get; set; }
            public string ref_id { get; set; }
            public int gateway_id { get; set; }
            public int status { get; set; }

        }
        public class T365ResponseDataVm
        {
            public T365CustomerVm customer { get; set; }
            public T365CreditCardResponseVm credit_card { get; set; }
            public string receipt_url { get; set; }
            public T365AdditionalDataVm additional_data { get; set; }
            public T365BillingAddressVm billing_address { get; set; }
            public T365PaymentVm payment { get; set; }
            public T365PaymentVm authorization { get; set; }


            public T365BeProtectedVerificationVm be_protected_verification { get; set; }
            public string uid { get; set; }
            public string status { get; set; }
            public string message { get; set; }
            public string amount { get; set; }
            public string currency { get; set; }
            public string description { get; set; }
            public string type { get; set; }
            public string tracking_id { get; set; }
            public string language { get; set; }
            public string payment_method_type { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public string redirect_url { get; set; }
            public bool test { get; set; }
            public T365AVSCVCVerificationVm avs_cvc_verification { get; set; }

            //used only for credit
            public T365PaymentVm credit { get; set; }
            public string id { get; set; }


            //For 3D Secure
            public T3653DSecureTransactionVm three_d_secure_verification { get; set; }

        }
        public class T365AdditionalDataVm
        {
            public string[] receipt_text { get; set; }
            public T365PaymentMethod payment_method { get; set; }
        }

        public class T365PaymentMethod
        {
            public string type { get; set; }
        }
        public class T365PaymentVm
        {
            public string auth_code { get; set; }
            public string bank_code { get; set; }
            public string rrn { get; set; }
            public string ref_id { get; set; }
            public string message { get; set; }
            public string gateway_id { get; set; }
            public string billing_descriptor { get; set; }
            public string status { get; set; }
        }
        public class T365BeProtectedVerificationVm
        {
            public string status { get; set; }
            public string message { get; set; }
            public T365LimitVm limit { get; set; }
            public T365WhiteBalckListVm white_black_list { get; set; }
        }
        public class T365LimitVm
        {
            public bool volume { get; set; }
            public bool count { get; set; }
            public bool max { get; set; }
            public int current_volume { get; set; }
            public int current_count { get; set; }
        }
        public class T365WhiteBalckListVm
        {
            public string card_number { get; set; }
            public string ip { get; set; }
            public string email { get; set; }
        }
        public class T365AVSCVCVerificationVm
        {
            public T365VerificationVM cvc_verification { get; set; }
            public T365VerificationVM avs_verification { get; set; }
        }
        public class T365VerificationVM
        {
            public string result_code { get; set; }
        }
        public class T3653DSecureTransactionVm
        {
            public string acs_url { get; set; }
            public string md { get; set; }
            public string message { get; set; }
            public string pa_req { get; set; }
            public string pa_res_url { get; set; }
            public string status { get; set; }
            public string ve_status { get; set; }


        }

        #region WebHook
        public class T365WebHookRequestParamVm
        {
            public T365RequestDataVm request { get; set; }
        }

        public class T36WebHookResposeVm
        {
            public T365RequestDataVm transaction { get; set; }
            public T365CreditCardVm card { get; set; }

            public string created_at { get; set; }
            public T365CustomerVm customer { get; set; }
            public string id { get; set; }

            public T365MainPlanVm plan { get; set; }
            public string renew_at { get; set; }
            public string state { get; set; }
            public string tracking_id { get; set; }
            public string device_id { get; set; }


        }

        public class T365PlanVm
        {
            public decimal amount { get; set; }
            public decimal interval { get; set; }
            public string interval_unit { get; set; }
        }

        public class T365MainPlanVm
        {
            public string currency { get; set; }
            public string id { get; set; }
            public T365PlanVm plan { get; set; }
            public string title { get; set; }
            public T365PlanVm trial { get; set; }

        }

        //public class T365WebHookParamDataVm
        //{
        //    public string uid { get; set; }
        //    public string type { get; set; }
        //    public string status { get; set; }
        //    public decimal amount { get; set; }
        //    public string currency { get; set; }
        //    public string description { get; set; }
        //    public string created_at { get; set; }
        //    public string method_type { get; set; }
        //    public T365PaymentVm payment { get; set; }
        //    public T365CustomerVm customer { get; set; }

        //    public T365MethodNameVm method_name { get; set; }


        //    public T365AdditionalDataVm additional_data { get; set; }
        //    public T365BillingAddressVm billing_address { get; set; }
        //}

        //public class T365MethodNameVm
        //{
        //    public string type { get; set; }
        //    public string account { get; set; }
        //}
        #endregion
    }
}

