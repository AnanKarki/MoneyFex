using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.BankApi.Models
{
    public class BankDepositRequestViewModel
    {

        public SenderInformationVm sender { get; set; }
        public BeneficiaryInformationVm beneficiary { get; set; }

        public CreditParty_IdentifierVm CreditParty_Identifier { get; set; }

        public decimal source_amount { get; set; }
        public decimal payment_amount { get; set; }

        public string source_currency { get; set; }
        public string payment_currency { get; set; }

        public string external_id { get; set; }
        public string serviceType { get; set; }


        // "source_amount": 4,
        //"payment_amount": 4,
        //"source_currency": "GBP",
        //"payment_currency": "EUR",
        //"external_id": "ACCe0COdfhh4frIIJhhD4kiookjffe5970ff663dd",
        //"serviceType":"Bank"


    }


    public class BankDepositLocalRequest
    {
        public string partnerTransactionReference { get; set; }
        public string baseCurrencyCode { get; set; }
        public string targetCurrencyCode { get; set; }
        public decimal baseCurrencyAmount { get; set; }
        public decimal targetCurrencyAmount { get; set; }

        public string partnerCode { get; set; }
        public string purpose { get; set; }
        public string accountNumber { get; set; }
        public string bankCode { get; set; }
        public string baseCountryCode { get; set; }
        public string targetCountryCode { get; set; }
        public string payerName { get; set; }
        public string payermobile { get; set; }




    }
    public class SenderInformationVm
    {
        public string senderFirstname { get; set; }
        public string senderLastname { get; set; }
        public string senderEmail { get; set; }
        public string senderAddress { get; set; }
        public string senderCity { get; set; }
        public string senderCountry { get; set; }
        public string senderPhoneNumber { get; set; }
        public string state { get; set; }
        public string id_type { get; set; }
        public string id_number { get; set; }
        public string IdentificationIssuedDate { get; set; }
        public string IdentificationExpiryDate { get; set; }

        //        "senderFirstname": "Dhruv",
        //"senderLastname": "Patel",
        //"senderEmail": "ayfadipe@gmail.com",
        //"senderAddress": "17 dudyemi street",
        //"senderCity": "Ikeja",
        //"senderCountry": "GB",
        //"senderPhoneNumber":"08062362436",
        //"state":"Lagos",
        //"id_type":"PAS",
        //"id_number":"P12345678",
        //"IdentificationIssuedDate":"2009-12-06",
        //"IdentificationExpiryDate":"2019-10-05"
    }

    public class BeneficiaryInformationVm
    {
        public string city { get; set; }
        public string address { get; set; }
        public string country { get; set; }
        public string lastname { get; set; }
        public string firstname { get; set; }
        public string msisdn { get; set; }
        public string state { get; set; }
        public string email { get; set; }
        public string beneficiaryMobileOperator { get; set; }

        public string beneficiaryBankRoutingNumber { get; set; }

        public string BeneficiaryBankAccountNumber { get; set; }


        //        "city": "Test",
        //"address": "17 dudyemi street",
        //"country": "AT",
        //"lastname": "Doe",
        //"firstname": "John",
        //"msisdn": "08062362436",
        //"state":"Test",
        //"email": "ayfadipe@gmail.com",
        //"beneficiaryMobileOperator":"MTN",
        //"beneficiaryBankRoutingNumber":"300304",
        //"BeneficiaryBankAccountNumber":"6031010249361"



    }

    public class CreditParty_IdentifierVm
    {
        public string bankName { get; set; }
        public string msisdn { get; set; }
        public string bank_account_number { get; set; }
        public string iban { get; set; }
        public string swift_bic_code { get; set; }
        public string Account_Name { get; set; }
        public string payer_id { get; set; }
        public string bankAddress { get; set; }
        public string bankCity { get; set; }
        public string bankCountry { get; set; }
        public string bankBranch { get; set; }



        //        "bankName":"ErsteGroup Bank",
        //"msisdn":"",
        //"bank_account_number":"AT611904300234573201",
        //"iban":"",
        //"swift_bic_code":"",
        //"Account_Name":"",
        //"payer_id":"",
        //"bankAddress":"",
        //"bankCity":"",
        //"bankCountry":"",
        //"bankBranch":""

    }

    public class BankDepositResponseVm
    {
        public bool status { get; set; }    
        public string message { get; set; }
        public BankDepositResponseResult result { get; set; }

        public int response { get; set; }

        public string extraResult { get; set; }

    }

    public class BankDepositResponseResult
    {

        public decimal paymentAmount { get; set; }
        public string transactionReference { get; set; }

        public string partnerTransactionReference { get; set; }
        public string baseCurrencyCode { get; set; }

        public string targetCurrencyCode { get; set; }

        public decimal sourceAmount { get; set; }

        public string senderName { get; set; }
        public int transactionStatus { get; set; }
        public string transactionStatusDescription { get; set; }
        public string transactiondate { get; set; }
        public string transactioncharge { get; set; }
        public string payername { get; set; }
        public string errorDescription { get; set; }
        public string errorCode { get; set; }

        public string beneficiaryBankCode { get; set; }


        public string beneficiaryAccountNumber { get; set; }

        public string beneficiaryAccountName { get; set; }

        public decimal targetAmount { get; set; }

        public int retriedCount { get; set; }

        public string processorGateway { get; set; }

        public decimal amountInBaseCurrency { get; set; }






        //"status": false,
        //"message": "Transaction status : Failed",
        //"response": 0,
        //"result": {
        //  "targetAmount": 200.00,
        //  "beneficiaryBankCode": "058",
        //  "beneficiaryAccountNumber": "0110412853",
        //  "beneficiaryAccountName": "Test Account Name",
        //  "transactionReference": "SWVPDGXC9XDD0UZNDXBJ00TJW",
        //  "partnerTransactionReference": "SWV123254478412",
        //  "baseCurrencyCode": "3",
        //  "targetCurrencyCode": "NGN",
        //  "amountInBaseCurrency": 200.00,
        //  "senderName": "VGG",
        //  "transactionStatus": 3,
        //  "transactionStatusDescription": "Failed",
        //  "errorDescription": "Failed, Contact Support Team",
        //  "benefiarybank": "Guaranty trust bank",
        //  "transactioncharge": 140.00,
        //  "transactiondate": "2019-11-07T10:43:59.137",
        //  "payername": "VGG",
        //  "retriedCount": 0,
        //  "processorGateway": "GTBank"
        //},
        //"extraResult": null



    }


    public class AccessTokenVM
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public string TokenType { get; set; }


    }


    public class AccountNoLookUpResponse
    {

        public bool status { get; set; }

        public string message { get; set; }

        public AccountNoLookUpResponseData data { get; set; }

        //        "status": true, 
        //"data": { 
        //"account_name": "Dayo Okesola", 
        //"account_number": "0044438185" 
        //}, 
        //"message": "Account Number Resolved"
    }

    public class AccountNoLookUpResponseData
    {
        public string account_name { get; set; }
        public string account_number { get; set; }


        //"account_name": "Dayo Okesola",
        //"account_number": "0044438185" 

    }
}



