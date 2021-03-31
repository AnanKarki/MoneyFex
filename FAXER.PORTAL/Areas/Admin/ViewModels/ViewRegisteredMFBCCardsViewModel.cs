using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewRegisteredMFBCCardsViewModel
    {
        public int Id { get; set; }
        public int KiiPayBusinessInformationId { get; set; }
        public string BusinessName { get; set; }
        public string BusinessRegNumber { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string CountryPhoneCode { get; set; }
        public string Email { get; set; }
        public string MFBCCardNumber { get; set; }
        public string TempSMSMFBC { get; set; }
        public decimal CreditOnCard { get; set; }
        public string Currency { get; set; }
        public int NumberOfRegMFTCCard { get; set; }
        public string CardUserFulllName { get; set; }
        public DateTime CardUserDoB { get; set; }
        public string CardUserGender { get; set; }
        public string CardUserFullAddress { get; set; }
        public string CardUserState { get; set; }
        public string CardUserTelephone { get; set; }
        public string CarduserEmail { get; set; }
        public string CardUserPhoto { get; set; }
        public string CardPhoto { get; set; }
        public string CardUsageStatus { get; set; }
    }
}