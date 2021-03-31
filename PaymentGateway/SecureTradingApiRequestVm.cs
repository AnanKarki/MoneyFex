using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentGateway
{
    public class SecureTradingApiRequestVm
    {

        //public const string _sitereference = "test_moneyfexltd76662";
        public const string _sitereference = "test_moneyfexltd76662";

        public const string _accounttypedescription = "ECOM";


        public string sitereference
        {

            get { return _sitereference; }
        }
        public string[] requesttypedescriptions
        {
            get; set;
        }
        public string accounttypedescription { get { return _accounttypedescription; } }

        public string currencyiso3a { get; set; }
        public string baseamount { get; set; }


        public string orderreference { get; set; }
        public string pan { get; set; }

        public string expirydate { get; set; }
        public string securitycode { get; set; }

        public string billingpremise { get; set; }
        public string billingpostcode { get; set; }

    }

    public class SecureTradingApiRequestParam
    {

        private const string _alias = "support@moneyfex.com";
        //private const string _alias = "support@moneyfex.com";
        private const string _version = "1.00";


        public string alias { get { return _alias; } }
        public string version { get { return _version; } }

        public SecureTradingApiRequestVm request { get; set; }

    }

    public class SecureTradingApiResponseVm
    {


        public string requestreference { get; set; }
        public string version { get; set; }
        public List<SecureTradingApiResponseTransactionResult> response { get; set; }



    }


    public class SecureTradingApiResponseTransactionResult
    {

        public string transactionstartedtimestamp { get; set; }
        public string livestatus { get; set; }
        public string issuer { get; set; }
        public string splitfinalnumber { get; set; }
        public string dccenabled { get; set; }
        public string settleduedate { get; set; }
        public string errorcode { get; set; }
        public string[] errordata { get; set; }
        public string orderreference { get; set; }
        public string tid { get; set; }
        public string merchantnumber { get; set; }
        public string merchantcountryiso2a { get; set; }
        public string transactionreference { get; set; }
        public string merchantname { get; set; }
        public string paymenttypedescription { get; set; }
        public string baseamount { get; set; }
        public string accounttypedescription { get; set; }
        public string acquirerresponsecode { get; set; }
        public string requesttypedescription { get; set; }
        public string securityresponsesecuritycode { get; set; }
        public string currencyiso3a { get; set; }
        public string authcode { get; set; }
        public string errormessage { get; set; }
        public string operatorname { get; set; }
        public string securityresponsepostcode { get; set; }
        public string maskedpan { get; set; }
        public string securityresponseaddress { get; set; }
        public string issuercountryiso2a { get; set; }
        public string settlestatus { get; set; }


    }

}
