using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewAgentMFBCPaymentsViewModel
    {
        public int Id { get; set; }
        public string BusinessName { get; set; }
        public string BusinessMobileNo { get; set; }
        public string CardUserFullName { get; set; }
        public string CardUserPhotoIDType { get; set; }
        public string CardUserPhotoIDNumber { get; set; }
        public string CardUserPhotoIDExpDate { get; set; }
        public string CardUserPhotoIDIssuingCountry { get; set; }
        public string MFBCNumber { get; set; }
        public decimal TotalCreditOnMFBC { get; set; }
        public decimal MoneyWDAmount { get; set; }
        public string Currency { get; set; }
        public string MoneyWDTime { get; set; }
        public string MoneyWDDate { get; set; }
        public string AgentVerifier { get; set; }
        public string AgentName { get; set; }
        public string AgentMFSCode { get; set; }
        public decimal BalanceOnBusinessCard { get; set; }
        public string ReceiptNo { get; set; }
        public string PhoneNo { get; set; }






    }
}