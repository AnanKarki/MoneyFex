using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAXER.PORTAL.SecureTradingPaymentGateway
{
    public class SecureTradingApiRequestVm
    {

        //public const string _sitereference = "test_moneyfexltd76662";
        public  string _sitereference =  Common.Common.GetAppSettingValue("SecureTradingApiKey").ToString();
        //public const string _sitereference = "moneyfexltd76663";
        public  string _accounttypedescription = "ECOM";


        public string sitereference
        {

            get { return _sitereference; }
        }
        public string[] requesttypedescriptions
        {
            get; set;
        }
        public string accounttypedescription {  get { return _accounttypedescription; } }

        public string currencyiso3a { get; set; }
        public string baseamount { get; set; }


        public string orderreference { get; set; }
        public string pan { get; set; }

        public string expirydate { get; set; }
        public string securitycode { get; set; }

        public string billingpremise { get; set; }
        public string billingpostcode { get; set; }

        public string termurl { get; set; }
        public string parenttransactionreference { get; set; }

        public string md { get; set; }
        public string pares { get; set; }
        public string accept {  
            get {
                return "text/html,*/*";
            }
         }

        public int SenderId { get; set; }

    }

    public class SecureTradingApiRequestBaseVm {

        private const string _alias = "support@moneyfex.com";
        //private const string _alias = "support@moneyfex.com";
        private const string _version = "1.00";


        public string alias { get { return _alias; } }
        public string version { get { return _version; } }

    }


    public class SecureTradingApiRequestParam : SecureTradingApiRequestBaseVm
    {

        public SecureTradingApiRequestVm request { get; set; }

    }

    public class SecureTradingTransactionQueryRequest : SecureTradingApiRequestBaseVm
    {


        public SecureTradingTransactionQuery request { get; set; }

    }

    public class SecureTradingApiThreeDCreateTransactionVm
    {

        private const string _alias = "support@moneyfex.com";
        //private const string _alias = "support@moneyfex.com";
        private const string _version = "1.00";


        public string alias { get { return _alias; } }
        public string version { get { return _version; } }


    }


    public class SecureTradingApiResponseVm
    {


        public string requestreference { get; set; }
        public string version { get; set; }
        public List<SecureTradingApiResponseTransactionResult> response { get; set; }



    }

    public class SecureTradingApiTransactionQueryResponseVm
    {

        public string requestreference { get; set; }
        public string version { get; set; }
        public List<SecureTradingApiRecordsResponse> response { get; set; }



    }

    public class SecureTradingApiRecordsResponse {

        public string transactionstartedtimestamp { get; set; }
        public string errormessage { get; set; }
        public string errorcode { get; set; }
        public List<SecureTradingApiResponseTransactionResult> records { get; set; }
        public string found { get; set; }
        public string requesttypedescription { get; set; }

    }

    public class SecureTradingApiResponseLogVm : SecureTradingApiResponseVm
    {
        //md = model.md,
        //        pares = model.pares,
        //        parenttransactionreference = model.parenttransactionreference,
        public string md { get; set; }
        public string pares { get; set; }
        public string parenttransactionreference { get; set; }

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

        public string status { get; set; }
        public Module Module { get; set; }


    }

    public class SecureTradingTHREEDQUERYReponseVm
    {


        public string requestreference { get; set; }
        public string version { get; set; }
        public List<THREEDQUERYReponseVm> response { get; set; }


    }

    public class THREEDQUERYReponseVm : SecureTradingApiResponseTransactionResult{


        public string enrolled { get; set; }
        public string md { get; set; }
        public string pareq { get; set; }
        public string acsurl { get; set; }
        public string xid { get; set; }

    }

    public class ThreeDRequestVm
    {

        public string PaReq { get; set; }
        public string termurl { get; set; }
        public string MD { get; set; }
        public string acsurl { get; set; }
        public string UId { get; set; }
        /// <summary>
        /// Y / N (Yes / No)
        /// </summary>
        public string ThreeDEnrolled { get; set; }
        public string ThreeDStatus { get; set; }
        public string redirect_url { get; set; }
        public string html { get; set; }
        public string url { get; set; }
        public CardProcessorApi CardProcessorApi { get; set; }
    }

    public class CustomerResponseVm {

        public const string BindProperty = "parenttransactionreference,PaRes,MD,ThreeDEnrolled ,ThreeDStatus";
        public string parenttransactionreference { get; set; }
        //PaRes’ and ‘MD’
        public string PaRes { get; set; }
        public string MD { get; set; }
        public string ThreeDEnrolled { get; set; }
        public string ThreeDStatus { get; set; }

    }


    public class SecureTradingRefundVm { 
    

    }

    public class SecureTradingTransactionFilterParam {

        public List<FilterKeyVm> sitereference { get; set; }
        public List<FilterKeyVm> currencyiso3a { get; set; }
        public List<FilterKeyVm> transactionreference { get; set; }
    }

    public class FilterKeyVm {

        public FilterKeyVm(string value)
        {

            this.value = value;
        }
        public string value { get; set; }

    }
    public class SecureTradingTransactionQuery 
    {

        public string[] requesttypedescriptions
        {
            get; set;
        }

        public SecureTradingTransactionFilterParam filter { get; set; }
    }
}
