using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewNonCardUsersMoneyReceivedViewModel
    {
        public ViewNonCardUsersMoneyReceivedViewModel()
        {
            viewNonCardUsersParties = new List<ViewNonCardUsersPartiesViewModel>();
            viewNonCardUsersDetails = new List<ViewNonCardUsersDetailsViewModel>();
        }
        public List<ViewNonCardUsersPartiesViewModel> viewNonCardUsersParties { get; set; }
        public List<ViewNonCardUsersDetailsViewModel> viewNonCardUsersDetails { get; set; }
    }

    public class ViewNonCardUsersPartiesViewModel
    {
        public int Id { get; set; }
        public string FaxerFirstName { get; set; }
        public string FaxerMiddleName { get; set; }
        public string FaxerLastName { get; set; }
       
        public string FaxerAddress { get; set; }
        public string FaxerCountry { get; set; }
        public string FaxerCity { get; set; }
        public string FaxerTelephone { get; set; }
        public string FaxerEmailAddress { get; set; }
        public string FaxerCurrency { get; set; }
        public decimal AmountFaxed { get; set; }
        public string MFCN { get; set; }
        public string ReceiverFirstName { get; set; }
        public string ReceiverMiddleName { get; set; }
        public string ReceiverLastName { get; set; }
       
        public string ReceiverAddress { get; set; }
        public string ReceiverCountry { get; set; }
        public string ReceiverCity { get; set; }
        public string ReceiverTelephone { get; set; }
        public string ReceiverEmailAddress { get; set; }
        public string ReceiverIDCardNumber { get; set; }
        public string ReceiverIDCardType { get; set; }
        public string ReceiverIDCardExpDate { get; set; }
        public string ReceiverIDCardIssuingCountry { get; set; }
        public string ReceiverCurrency { get; set; }
        public decimal AmountReceived { get; set; }
        
    }
    public class ViewNonCardUsersDetailsViewModel
    {
        public int Id { get; set; }
        public string PayingAgentVerifier { get; set; }
        public string PayingAgentName { get; set; }
        public string PayingAgentMFSCode { get; set; }
        public string StatusOfTransaction  { get; set; }
        public string PaymentRejection { get; set; }
        public string ExchangeRate { get; set; }
        public string Fee { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionTime { get; set; }
        public string ReceiptNo { get; set; }
        public string FaxerFullName { get; set; }
        public string ReceiverFullName { get; set; }
        public string MFCN { get; set; }
        public string ReceiversTelephone { get; set; }
        public string AmountSent { get; set; }
        public string AmountReceived { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }
    }
}