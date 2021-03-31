using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class SecureTradingApiResponseTransactionLog
    {
        public int Id { get; set; }
        public int SenderId { get; set; }

        public string requestreference { get; set; }
        public string version { get; set; }
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

        public string md { get; set; }
        public string pares { get; set; }

        public string status { get; set; }
        public Module Module { get; set; }

    }

    public class ThreeDRequestLog
    {

        public int Id { get; set; }
        public string PaReq { get; set; }
        public string termurl { get; set; }
        public string MD { get; set; }
        public string acsurl { get; set; }
        /// <summary>
        /// Y / N (Yes / No)
        /// </summary>
        public string ThreeDEnrolled { get; set; }
        public string ThreeDStatus { get; set; }
        public string ReceiptNo { get; set; }

        public string parenttransactionreference { get; set; }
        public string PaRes { get; set; }
        public int SenderId { get; set; }
        public DateTime? CreatedDate { get; set; }

    }
}