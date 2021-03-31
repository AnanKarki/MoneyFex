using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.BankApi.Models
{
    public class CashPotVm
    {
        public string USER { get; set; }
        public string PASSWORD { get; set; }
        public string PARTNER_ID { get; set; }
        public string LOCATION_ID { get; set; }
        public string PAYER_ID { get; set; }
    }
    public class SendTransGenericRequestVm : CashPotVm
    {
        public string REFERENCE_CODE { get; set; }
        public string DATE { get; set; }
        public string TRANSSTATUS { get; set; }
        public string SENDING_CURRENCY { get; set; }
        public string RECEIVER_CURENCY { get; set; }
        public string RATE { get; set; }
        public string FEE { get; set; }
        public string AGENT { get; set; }
        public string RECEIVING_AMOUNT { get; set; }
        public string SEND_AMOUNT { get; set; }

        public string SENDER_USER_NAME { get; set; }
        public string SENDER_FIRST_NAME { get; set; }
        public string SENDER_LAST_NAME { get; set; }
        public string SENDER_OCCUPATION { get; set; }
        public string SENDER_ADDRESS { get; set; }
        public string SENDER_COUNTRY { get; set; }
        public string SENDER_MOBILE { get; set; }
        public string RECEIVER_TYPE { get; set; }
        public string COMPANYNAME { get; set; }
        public string RECEIVER_FIRST_NAME { get; set; }
        public string RECEIVER_LAST_NAME { get; set; }
        public string RECEIVER_PHONE_NUMBER { get; set; }
        public string RECEIVER_MOBILE_NUMBER { get; set; }
        public string LOCAL_AMOUNT { get; set; }
        public string RECEIVER_COUNTRY { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public string RECEIVER_CITY { get; set; }
        public string RECEIVER_ZIP { get; set; }
        public string RECEIVER_STATE { get; set; }
        public string RECEIVER_ADDRESS { get; set; }
        public string RECEIVER_BANK_NAME { get; set; }
        public string RECEIVER_BANK_SORT { get; set; }
        public string RECEIVER_BANK_CODE { get; set; }
        public string RECEIVER_BRANCH_CODE { get; set; }
        public string RECEIVER_BRANCH_ADDRESS { get; set; }
        public string RECEIVER_BANK_ACCOUNT_TITLE { get; set; }
        public string RECEIVER_BANK_ACCOUNT_NO { get; set; }
        public string RECEIVER_BANK_ACCOUNT_TYPE { get; set; }
        public string RECEIVER_BANK_ROUTING { get; set; }
        public string RECEIVER_BANK_SWIFT { get; set; }
        public string RECEIVER_BANK_IBAN { get; set; }
        public string TRANSACTION_PURPOSE { get; set; }
        public string TRANSACTION_DETAILS { get; set; }
        public string SECRET_QUESTION { get; set; }
        public string SECRET_ANSWER { get; set; }
        public string SENDER_ID_NUMBER { get; set; }
        public string SENDER_ID_ISSUE_DATE { get; set; }
        public string SENDER_ID_EXPIRY_DATE { get; set; }
        public string SENDER_DOB { get; set; }
        public string SENDER_POST_CODE { get; set; }
        public string SITE_LOCATION { get; set; }
        public string PAYING_PARTNER { get; set; }
    }


    public class RateGenericRequest : CashPotVm
    {
        public string SENDING_CURRENCY { get; set; }
        public string RECEIVING_CURENCY { get; set; }
        public string SENDING_COUNTRY { get; set; }
        public string RECEIVING_COUNTRY { get; set; }
        public string AMOUNT { get; set; }
        public string TRANS_TYPE_ID { get; set; }
    }
    public class TransactionStatusRequest : CashPotVm
    {
        public string REFERENCE_CODE { get; set; }

    }

    public class UpdateStatusRequest : CashPotVm
    {

        public string REFERENCE_CODE { get; set; }

        public string TRANS_STATUS_ID { get; set; }

        public string OTP_CODE { get; set; }
    }


    public class CancelTransactionRequest : CashPotVm
    {

        public string REFERENCE_CODE { get; set; }
        
        public string CANCELDATE { get; set; }

        public string STATUS_CODE { get; set; }



    }

    public class CashPotResponseVm
    {
        public string DATE { get; set; }
        public string MSG_CODE { get; set; }
        public string MSG_NOTE { get; set; }
    }

    public class BankAccountValidataionRequest : CashPotResponseVm
    {

        public string REFERENCE_CODE { get; set; }

    }
    public class TransactionStatusResposeCashPotVm : CashPotResponseVm
    {
        public string REFERENCE_CODE { get; set; }
        public string STATUS_CODE { get; set; }
    }
    public class RateGenericResponse : CashPotResponseVm
    {
        public string RATE { get; set; }
    }
    public class CancelTransStatus : TransactionStatusResposeCashPotVm
    {
        //public string DATE { get; set; }
        public string CANCELDATE { get; set; }

    }
    public class PaidTransStatus : TransactionStatusResposeCashPotVm
    {
        public string PAIDDATE { get; set; }
    }
    public class SendTransGenericResponseVm : TransactionStatusResposeCashPotVm
    {
        public string TRANS_ID { get; set; }
    }

}