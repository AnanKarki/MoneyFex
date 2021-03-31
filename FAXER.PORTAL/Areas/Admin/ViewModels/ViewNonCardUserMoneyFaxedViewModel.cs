using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewNonCardUserMoneyFaxedFaxerViewModel
    {
        public int Id { get; set; }
        public string FaxerFirstName { get; set; }
        public string FaxerMiddleName { get; set; }
        public string FaxerLastName { get; set; }
        public string FaxerAddress { get; set; }
        public string FaxerCity { get; set; }
        public string FaxerCountry { get; set; }
        public string FaxerTelephone { get; set; }
        public string FaxerEmail { get; set; }
        public string FaxerIDCardNumber { get; set; }
        public string FaxerIDCardType { get; set; }
        public string FaxerIDCardExpDate { get; set; }
        public string FaxerIDCardIssuingCountry { get; set; }
        public decimal FaxedAmount { get; set; }
        public string FaxingCurrency { get; set; }
        public decimal Fee { get; set; }
        public string PaymentMethod { get; set; }
        public string FaxingMethod { get; set; }
        public string PortalFaxingAdminName { get; set; }
        public string MFCN { get; set; }

        public string ReceiverFirstName { get; set; }
        public string ReceiverMiddleName { get; set; }
        public string ReceiverLastName { get; set; }
        public string ReceiverAddress { get; set; }
        public string ReceiverCountry { get; set; }
        public string ReceiverCity { get; set; }
        public string ReceiverTelephone { get; set; }
        public string ReceiverEmail { get; set; }
        public string FaxingAgentName { get; set; }
        public string FaxingAgentMFSCode { get; set; }
        public string FaxingAgentVerifier { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string FaxingUpdate { get; set; }
        public string UpdatingName { get; set; }
        public string UpdatingDate { get; set; }
        public string UpdatingTime { get; set; }
        public string Status { get; set; }

        public DB.FaxingStatus FaxingStatus { get; set; }


        public string NameofSendingPortal { get; set; }

    }

    public class ViewNonCardUserMoneyFaxedReceiverViewModel
    {
        public int Id { get; set; }
        
        
    }
    public class ViewNonCardUserMoneyFaxedViewModel
    {
        public ViewNonCardUserMoneyFaxedViewModel()
        {
            ViewNonCardUserFaxer = new List<ViewNonCardUserMoneyFaxedFaxerViewModel>();
            ViewNonCardUserReceiver = new List<ViewNonCardUserMoneyFaxedReceiverViewModel>();
        }
        public List<ViewNonCardUserMoneyFaxedFaxerViewModel> ViewNonCardUserFaxer { get; set; }
        public List<ViewNonCardUserMoneyFaxedReceiverViewModel> ViewNonCardUserReceiver { get; set; }
    }

    
}