using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{

    public class FormFaxerDetailsViewModel
    {
        public int FaxerId { get; set; }
        
        public string FaxerFirstName { get; set; }
        public string FaxerMiddleName { get; set; }
        public string FaxerLastName { get; set; }
        public string FaxerIDCardNo { get; set; }
        public string FaxerIDCardType { get; set; }
        public string FaxerIDCardExpDate { get; set; }
        public string FaxerIDCardIssuingCountry { get; set; }
        public string FaxerAddress { get; set; }
        public string FaxerCity { get; set; }
        public string FaxerCountry { get; set; }

        
        public string FaxerCountryCode { get; set; }
        public string FaxerTelephone { get; set; }
        public string FaxerEmail { get; set; }
    }

    public class FormBusinessDetailsViewModel
    {
        public int KiiPayBusinessInformationId { get; set; }
        public string BusinessMobileNo { get; set; }
        public string BusinessMerchantName { get; set; }
        public string BusinessAccountNo { get; set; }
        public string BusinessCity { get; set; }
        public string BusinessCountry { get; set; }
        public string BusinessCC { get; set; }
        public bool  BusinessConfirm { get; set; }
    }

    public class FormPaymentDetailsViewModel
    {
        
        public bool PaymentIncludeFee { get; set; }
        public decimal PaymentTopUpAmount { get; set; }
        public decimal PaymentFaxingFee { get; set; }
        public decimal PaymentAmountIncludingFee { get; set; }
        public decimal PaymentExchangeRate { get; set; }
        public decimal PaymentAmountToBeReceived { get; set; }
        public string PaymentReceivingCountry { get; set; }
        public string SenderCurrency { get; set; }
        public string SenderCurrencySymbol { get; set; }
        public string ReceiverCurrency { get; set; }
        public string ReceiverCurrencySymbol { get; set; }
    }

    public class FormCreditCardDetailsViewModel
    {
        public decimal CardAmount { get; set; }
        public string CardNameOnCard { get; set; }
        public string CardNumberForDropDown { get; set; }
        public string CardNumber { get; set; }
        public int CardEndMonth { get; set; }
        public int CardEndYear { get; set; }
        public string CardSecurityNo { get; set; }
    }

    public class FormBillingDetailsViewModel
    {
        public string BillingAddress1 { get; set; }
        public string BillingAddress2 { get; set; }
        public string BillingCity { get; set; }
        public string BillingPostalCode { get; set; }
        public string BillingCountry { get; set; }


    }

    public class FormFaxerMerchantPaymentsViewModel
    {
        public const string BindProperty = "FormFaxerDetails , FormBusinessDetails ,FormPaymentDetails ,FormCreditCardDetails ,FormBillingDetails ,PaymentReference , AcceptTerms" +
            ",BankPayment , FaxerAccountNo,isCardAvailable ";


        public FormFaxerMerchantPaymentsViewModel()
        {
            FormFaxerDetails = new FormFaxerDetailsViewModel();
            FormBusinessDetails = new FormBusinessDetailsViewModel();
            FormPaymentDetails = new FormPaymentDetailsViewModel();
            FormCreditCardDetails = new FormCreditCardDetailsViewModel();
            FormBillingDetails = new FormBillingDetailsViewModel();
            
        }
        


        public FormFaxerDetailsViewModel FormFaxerDetails { get; set; }
        public FormBusinessDetailsViewModel FormBusinessDetails { get; set; }
        public FormPaymentDetailsViewModel FormPaymentDetails { get; set; }
        public FormCreditCardDetailsViewModel FormCreditCardDetails { get; set; }
        public FormBillingDetailsViewModel FormBillingDetails { get; set; }

        public string PaymentReference { get; set; }
        public bool AcceptTerms { get; set; }
        public bool BankPayment { get; set; }
        public string FaxerAccountNo { get; set; }
        public bool isCardAvailable { get; set; }
    }
}