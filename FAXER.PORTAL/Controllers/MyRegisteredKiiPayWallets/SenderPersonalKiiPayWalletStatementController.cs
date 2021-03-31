using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.MyRegisteredKiiPayWallets
{
    public class SenderPersonalKiiPayWalletStatementController : Controller
    {

        private SSenderWalletTransactionStatement _senderWalletTransactionStatementServices = null;
        public SenderPersonalKiiPayWalletStatementController()
        {
            _senderWalletTransactionStatementServices = new SSenderWalletTransactionStatement();
        }


        public ActionResult Index(FilterByStatus inout = FilterByStatus.All, string Country = "")
        {

            SenderCommonFunc _senderCommonFunc = new SenderCommonFunc();
            var countries = Common.Common.GetCountries();

            var walletInfo= _senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id);
            ViewBag.Countries = new SelectList(countries, "CountryCode", "CountryName" , Country);
            SenderWalletTransactionStatementVM vm = new SenderWalletTransactionStatementVM();
            var senderWalletTransactionStatementMaster = new SenderWalletTransactionStatementMasterVM();
            senderWalletTransactionStatementMaster.AvailableBalance = _senderCommonFunc.GetCurrentKiiPayWalletBal(walletInfo.Id);
            senderWalletTransactionStatementMaster.Currency = Common.Common.GetCurrencySymbol(walletInfo.CardUserCountry);
            vm.SenderWalletTransactionStatementMaster = senderWalletTransactionStatementMaster;
            InOut? walletStatmentStatus = null;
            switch (inout)
            {
                case FilterByStatus.All:

                    break;
                case FilterByStatus.In:
                    walletStatmentStatus = InOut.In;
                    break;
                case FilterByStatus.Out:
                    walletStatmentStatus = InOut.Out;
                    break;
                default:
                    break;
            }
            vm.SenderWalletTransactionStatementDetail = _senderWalletTransactionStatementServices.GetWalletStatment(walletInfo.Id, walletStatmentStatus, Country);
            vm.SenderWalletTransactionStatementMaster.FilterKey = inout;
            vm.SenderWalletTransactionStatementMaster.Country = Country;
            return View(vm);
        }
    }
}