using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewMerchantMFTCTopUpViewModel
    {

        #region Sender Information

        public string MerchantName { get; set; }
        public string MerchantAccountNo { get; set; }
        public string MerchantCountry { get; set; }
        public string MerchantCity { get; set; }

        #endregion

        #region Receiver Information


        public string CardUserName { get; set; }
        public string CardUserMFTCCardNumber { get; set; }
        public string CardUserCity { get; set; }
        public string CardUserCountry { get; set; }
        public string CardUserEmail { get; set; }

        #endregion

        #region Transaction Information 

        public int TransactionId { get; set; }
        public decimal TopUpAmount { get; set; }
        public decimal TopUpFee { get; set; }
        public string TopUpType { get; set; }
        public DB.PaymentType TopUpTypeEnum { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionTime { get; set; }


        #endregion
    }
}