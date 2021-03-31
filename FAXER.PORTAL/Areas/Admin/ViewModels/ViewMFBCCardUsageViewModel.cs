using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewMFBCCardUsageViewModel
    {
        public int Id { get; set; }
        public string BusinessName { get; set; }
        public string BusinessMobileNo { get; set; }
        public string CardUserFullName { get; set; }
        public string CardUserPhotoIDType { get; set; }
        public string CardUserIDNumber { get; set; }
        public string CardUserPhotoIDExpDate { get; set; }
        public string CardUserPhotoIDIssuingCountry { get; set; }
        public string CardUserCity { get; set; }
        public string CardUserCountry { get; set; }
        public string MFBCCardNumber { get; set; }
        public decimal CreditOnMFBCCard { get; set; }
        public string UserCurrency { get; set; }
        public decimal MoneyWDAmount { get; set; }
        public string MoneyWDTime { get; set; }
        public string MoneyWDDate { get; set; }
        public string PayingAgentVerifier { get; set; }
        public string PayingAgentName { get; set; }
        public string PayingAgentMFSCode { get; set; }
        public string PayingAgentCity { get; set; }


    }
}