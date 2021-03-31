using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using TransferZero.Sdk.Model;
using static TransferZero.Sdk.Model.FieldDescription;

namespace FAXER.PORTAL.Models
{
    public class WebhookCallbackModel
    {
        public const string BindProperty = " webhook, Event,Object";
        public string webhook { get; set; }
        public string Event { get; set; }

        public TransactionObject Object { get; set; }

    }

    public class TransactionObject
    {
        public decimal input_amount { get; set; }
        public string input_currency { get; set; }
        public string id { get; set; }
        public object metadata { get; set; }
        public TransferZero.Sdk.Model.TransactionState state { get; set; }
        public string payin_reference { get; set; }

        public WebHookCallbackSender sender { get; set; }
        public List<string> payin_methods { get; set; }

        public decimal paid_amount { get; set; }
        public decimal due_amount { get; set; }

        public List<WebhookCallbackReceipent> recipients { get; set; }


        public DateTime created_at { get; set; }
        public DateTime expires_at { get; set; }
        public object traits { get; set; }
        public string external_id { get; set; }


    }

    public class WebHookCallbackSender
    {


        public string Id { get; set; }

        public string type { get; set; }
        public string state { get; set; }
        public string state_reason { get; set; }
        public string country { get; set; }
        public string street { get; set; }
        public string postal_code { get; set; }
        public string city { get; set; }
        public string phone_country { get; set; }
        public string phone_number { get; set; }
        public string email { get; set; }
        public string ip { get; set; }

        public string address_description { get; set; }
        public string identification_number { get; set; }
        public string identification_type { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }

        public string last_name { get; set; }
        public string birth_date { get; set; }
        public string occupation { get; set; }
        public string nationality { get; set; }
        public string source_of_funds { get; set; }
        public List<Object> documents { get; set; }
        public object metadata { get; set; }
        public object providers { get; set; }
        public string onboarding_status { get; set; }
        public string external_id { get; set; }
        public List<string> politically_exposed_people { get; set; }

    }


    public class WebhookCallbackReceipent
    {
        public DateTime created_at { get; set; }
        public bool editable { get; set; }
        public string id { get; set; }
        public decimal input_usd_amount { get; set; }
        public bool may_cancel { get; set; }
        public object metadata { get; set; }
        public TransferZero.Sdk.Model.RecipientState state { get; set; }

        public string transaction_id { get; set; }

        public TransferZero.Sdk.Model.TransactionState transaction_state { get; set; }
        public bool retriable { get; set; }
        public decimal exchange_rate { get; set; }
        public decimal fee_fractional { get; set; }
        public string transaction_external_id { get; set; }
        public decimal requested_amount { get; set; }
        public string requested_currency { get; set; }
        public decimal input_amount { get; set; }
        public string input_currency { get; set; }
        public decimal output_amount { get; set; }
        public string output_currency { get; set; }
        public WebHookPayoutMethods payout_method { get; set; }

    }

    public class WebHookPayoutMethods
    {

        public string Type { get; set; }
        public WebHookPayoutMethodDetials details { get; set; }
        public object metadata { get; set; }
        public string Id { get; set; }
        public string providers { get; set; }
       // public Dictionary<string, FieldDescription> Fields { get; set; }

    }

    public class WebhookFieldDescrition
    {
        
        public TypeEnum? type { get; set; }
        
        public FieldValidation validations { get; set; }

    }
    public class WebhooKFieldValidation
    {

        
        public bool? presence { get; set; }
        
        public FieldSelectValidation inclusion { get; set; }
        
        public object format { get; }
    }
    public class WebHookPayoutMethodDetials
    {

        public string reference { get; set; }

        public string identity_card_id { get; set; }

        public PayoutMethodIdentityCardTypeEnum identity_card_type { get; set; }
        public string reason { get; set; }

        public PayoutMethodGenderEnum sender_gender { get; set; }

        public string sender_country_of_birth { get; set; }

        public string sender_city_of_birth { get; set; }

        public string sender_identity_card_id { get; set; }

        public PayoutMethodIdentityCardTypeEnum sender_identity_card_type { get; set; }

        public string bic { get; set; }

        public string bank_country { get; set; }

        public string bank_name { get; set; }

        public string iban { get; set; }

        public PayoutMethodMobileProviderEnum mobile_provider { get; set; }

        public string phone_number { get; set; }

        public PayoutMethodBankAccountTypeEnum bank_account_type { get; set; }

        public string bank_account { get; set; }

        public string bank_code { get; set; }

        public string last_name { get; set; }

        public string first_name { get; set; }

        public string name { get; set; }

        public string address { get; set; }
    }

}