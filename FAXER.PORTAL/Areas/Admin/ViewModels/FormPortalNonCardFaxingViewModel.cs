using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{

    public class FormPortalNonCardFaxerDetailsViewModel
    {
        public int Id { get; set; }
        public int faxerId { get; set; }
        public string faxerFirstName { get; set; }
        public string faxerMiddleName { get; set; }
        public string faxerLastName { get; set; }
        public string faxerIDCardNumber { get; set; }
        public string faxerIDCardType { get; set; }
        public string faxerIDCardExpDate { get; set; }
        public string faxerIDCardIssuingCountry { get; set; }
        public string faxerAddress { get; set; }
        public string faxerCity { get; set; }
        public string faxerCountry { get; set; }
        public string faxerCountryCode { get; set; }
        public string faxerTelephone { get; set; }
        public string faxerEmailAddress { get; set; }
        public string faxerPostalCode { get; set; }
        public int receiverId { get; set; }

    }
    public class FormPortalNonCardReceiverDetailsViewModel
    {
        public int Id { get; set; }
        public int receiverId { get; set; }
        public string receiverFirstName { get; set; }
        public string receiverMiddleName { get; set; }
        public string receiverLastName { get; set; }
        public string receiverAddress { get; set; }
        public string receiverCity { get; set; }
        public string receiverCountry { get; set; }
        public string receiverTelephone { get; set; }
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter valid email")]

        public string receiverEmailAddress { get; set; }
        public string receiversCurrency { get; set; }
        public string receiversCurSymbol { get; set; }

    }

    public class FormPortalNonCardFaxingCalculationsViewModel
    {
        public int Id { get; set; }
        public decimal faxingAmount { get; set; }
        public decimal faxingFee { get; set; }
        public decimal totalAmount { get; set; }
        public decimal exchangeRate { get; set; }
        public decimal receivingAmount { get; set; }
        public string receivingCountry { get; set; }
        public string sendingCurrency { get; set; }
        public string sendingCurrencySymbol { get; set; }
        public string receivingCurrency { get; set; }
        public string receivingCurrencySymbol { get; set; }

    }

    public class FormPortalNonCardPaymentDetailsViewModel
    {
        public int Id { get; set; }
        public decimal faxingAmount { get; set; }
        public string nameOnCard { get; set; }
        public string cardNumber { get; set; }
        public int endMonth { get; set; }
        public int endYear { get; set; }
        public string securityCode { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string city { get; set; }
        public string postalCode { get; set; }
        public string country { get; set; }

    }

    public class FormPortalNonCardFaxingViewModel
    {
        public const string BindProperty = "StripeChargeId , PortalNonCardFaxerDetails ,PortalNonCardReceiverDetails , PortalNonCardFaxingCalculations,PortalNonCardFaxingPaymentDetails" +
            " ,checkTermsAndConditions ,isCardAvailable , isBankToBankPayment, faxerAccountNo, RecCurrency,RecCurSymbol ,SendingCurrency , SendingCurSymbol, CurValue";


        public string StripeChargeId { get; set; }
        public FormPortalNonCardFaxingViewModel()
        {
            PortalNonCardFaxerDetails = new FormPortalNonCardFaxerDetailsViewModel();
            PortalNonCardReceiverDetails = new FormPortalNonCardReceiverDetailsViewModel();
            PortalNonCardFaxingCalculations = new FormPortalNonCardFaxingCalculationsViewModel();
            PortalNonCardFaxingPaymentDetails = new FormPortalNonCardPaymentDetailsViewModel();

        }

        public FormPortalNonCardFaxerDetailsViewModel PortalNonCardFaxerDetails { get; set; }
        public FormPortalNonCardReceiverDetailsViewModel PortalNonCardReceiverDetails { get; set; }
        public FormPortalNonCardFaxingCalculationsViewModel PortalNonCardFaxingCalculations { get; set; }
        public FormPortalNonCardPaymentDetailsViewModel PortalNonCardFaxingPaymentDetails { get; set; }

        public bool checkTermsAndConditions { get; set; }
        public bool isCardAvailable { get; set; }
        public bool isBankToBankPayment { get; set; }
        public string faxerAccountNo { get; set; }
        public string RecCurrency { get; set; }
        public string RecCurSymbol { get; set; }
        public string SendingCurrency { get; set; }
        public string SendingCurSymbol { get; set; }
        public string CurValue { get; set; }

    }
}