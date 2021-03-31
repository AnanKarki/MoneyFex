using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.Common
{
    public class MobileRecipientDropdownViewModel
    {

        public int Id { get; set; }
        public int SenderId { get; set; }
        public Service Service { get; set; }
        public string ServiceName { get; set; }
        public string MobileWalletProviderName { get; set; }
        public string ReceiverName { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string MobileNo { get; set; }
        public int BankId { get; set; }
        public string AccountNo { get; set; }
        public string BranchCode { get; set; }
        public ReasonForTransfer Reason { get; set; }
        public int MobileWalletProvider { get; set; }
        public bool IBusiness { get; set; }
        public string CurrencyCode { get;  set; }
        //public string BankName
        //{
        //    get
        //    {
        //        switch (Service)
        //        {
        //            case Service.Select:
        //                return "";
        //            case Service.BankAccount:
        //                //return new BankServices().BankList().Where(x => x.Id == BankId).FirstOrDefault().Name;
        //                return BankName;
        //            case Service.MobileWallet:
        //                return "Mobile Wallet";
        //            case Service.CashPickUP:
        //                return "Cash Pickup";
        //            case Service.KiiPayWallet:
        //                return "KiiPay Wallet";
        //            default:
        //                break;
        //        }
        //        return "";
        //    }
        //    set { }
        //}
        public string BankName { get; set; }

    }
}