using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.MoblieTransferApi.Models
{
    public class MTNCameroonRequestParamVm
    {

        public string amount { get; set; }
        public string currency { get; set; }
        public string externalId { get; set; }
        public PayeeInfo payee { get; set; }
        public string payerMessage { get; set; }
        public string payeeNote { get; set; }   





    }

    public class MTNCameroonResponseParamVm : MTNCameroonRequestParamVm
    {

        #region Request Response  detials property
        public string financialTransactionId { get; set; }

        public string status { get; set; }

        public string reason { get; set; }

        public string code { get; set; }
        public string message { get; set; }

        public string refId { get; set; }

        #endregion
    }


    public class CreateApiUserVm

    {

        private string _providerCallbackHost = "moneyfex.com";
        public string providerCallbackHost
        {
            get
            {
                return _providerCallbackHost;
            }
        }



    }



    public class PayeeInfo
    {


        /// <summary>
        /// MSISDN (Mobile Station International Subscriber Directory Number)
        /// </summary>
        public string partyIdType { get; set; }

        /// <summary>
        /// Mobile Number
        /// </summary>
        public string partyId { get; set; }
    }

    public class MobileNoLookUpResponseData
    {
        /// <summary>
        /// MSISDN (Mobile Station International Subscriber Directory Number)
        /// </summary>
        public string accountHolderIdType { get; set; }
        /// <summary>
        /// Mobile Number
        /// </summary>
        public string accountHolderId { get; set; }
    }
    public class MobileNoLookUpResponse
    {

        public bool status { get; set; }

        public string message { get; set; }

        public MobileNoLookUpResponseData data { get; set; }

    }


}