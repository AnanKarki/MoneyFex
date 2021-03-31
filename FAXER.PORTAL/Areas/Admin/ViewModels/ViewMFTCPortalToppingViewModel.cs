using FAXER.PORTAL.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewMFTCPortalToppingFaxerDetailViewModel
    {
        public int Id { get; set; }
        public int FaxerId { get; set; }
        public string FaxerFirstName { get; set; }
        public string FaxerMiddleName { get; set; }
        public string FaxerLastName { get; set; }
        public string FaxerIDCardNumber { get; set; }
        public string FaxerIDCardType { get; set; }
        public string FaxerIDCardExpDate { get; set; }
        public string FaxingIDCardIssuingCountry { get; set; }
        public string FaxerAddress { get; set; }
        public string FaxerCity { get; set; }
        public string FaxerCountry { get; set; }
        public string FaxerPostalCode { get; set; }
        public string FaxerCountryCode { get; set; }
        public string FaxerTelephone { get; set; }
        public string FaxerEmailAddress { get; set; }
        public string MFTCCardNumber { get; set; }
    }

    public class ViewMFTCPortalToppingCardUserDetailViewModel
    {
        public int Id { get; set; }
        public int CardUserId { get; set; }
        public string CardUserFirstName { get; set; }
        public string CardUserMiddleName { get; set; }
        public string CardUserLastName { get; set; }
        public string CardUserAddress { get; set; }
        public string CardUserCity { get; set; }
        public string CardUserCountry { get; set; }
        public string CardUserCountryCode { get; set; }
        public string CardUserTelephone { get; set; }
        public string CardUserPhoto { get; set; }
    }

    public class MFTCPortalToppingPaymentViewModel
    {

        //[Required]
        public decimal FaxingAmount { get; set; }
        public string NameOnCard { get; set; }
        public string CardNumber { get; set; }
        //[RegularExpression("[0-9]", ErrorMessage = "Only Numbers accepted !"), StringLength(2)]
        public int EndMonth { get; set; }
        //[RegularExpression("[0-9]", ErrorMessage = "Only Numbers accepted !"), StringLength(2)]
        public int EndYear { get; set; }
        public string SecurityCode { get; set; }

    }
    public class MFTCPortalToppingBillingAddressViewModel
    {

        public string Addressline1 { get; set; }
        public string Addressline2 { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }


    }
    public class ViewMFTCPortalToppingViewModel
    {
        public const string BindProperty = "ViewMFTCPortalFaxer , ViewMFTCPortalCardUser ,CalculateFaxingFee ,ToppingPayment , ToppingBillingAddress, checkTermsAndConditions,isCardAvailable , bankToBankPayment ,AdminName";

        public ViewMFTCPortalToppingViewModel()
        {
            ViewMFTCPortalFaxer = new ViewMFTCPortalToppingFaxerDetailViewModel();
            ViewMFTCPortalCardUser = new ViewMFTCPortalToppingCardUserDetailViewModel();
            CalculateFaxingFee = new CalculateFaxingFeeVm();
            ToppingPayment = new MFTCPortalToppingPaymentViewModel();
            ToppingBillingAddress = new MFTCPortalToppingBillingAddressViewModel();
        }
        public ViewMFTCPortalToppingFaxerDetailViewModel ViewMFTCPortalFaxer { get; set; }
        public ViewMFTCPortalToppingCardUserDetailViewModel ViewMFTCPortalCardUser { get; set; }
        public CalculateFaxingFeeVm CalculateFaxingFee { get; set; }
        public MFTCPortalToppingPaymentViewModel ToppingPayment { get; set; }
        public MFTCPortalToppingBillingAddressViewModel ToppingBillingAddress { get; set; }

        public bool checkTermsAndConditions { get; set; }
        public bool isCardAvailable { get; set; }
        public bool bankToBankPayment { get; set; }
        public string AdminName { get; set; }

    }
}