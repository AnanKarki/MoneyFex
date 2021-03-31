using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.MoneyFex
{
    public class MobileRecipentViewModel
    {

        public int Id { get; set; }
        public int SenderId { get; set; }
        public Service Service { get; set; }
        public string ReceiverName { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string MobileNo { get; set; }
        public int BankId { get; set; }
        public string BankName { get; set; }
        public string MobileWalletProviderName { get; set; }
        public string AccountNo { get; set; }
        public string BranchCode { get; set; }
        public ReasonForTransfer Reason { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string EmailAddress { get; set; }
        public string PostCode { get; set; }
        public int MobileWalletProvider { get; set; }
        public bool IBusiness { get; set; }
        public int IdentificationTypeId { get; set; }
        public string IdentificationNumber { get; set; }
        public string CountryPhoneCode { get;  set; }
        public string IdentificationTypeName { get;  set; }
    }
}