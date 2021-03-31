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
    public class SendersWalletTransactionStatementController : Controller
    {
        private SSenderWalletTransactionStatement _senderWalletTransactionStatementServices = null;

        private SSenderFamilyAndFriends _senderFamilyAndFriendsServices = null;

        public SendersWalletTransactionStatementController()
        {
            _senderWalletTransactionStatementServices = new SSenderWalletTransactionStatement();
            _senderFamilyAndFriendsServices = new SSenderFamilyAndFriends();
        }
        // GET: SendersWalletTransactionStatement
        public ActionResult Index(InOut? inout, int walletId = 0, string Country = "")

        {
            SenderCommonFunc _senderCommonFunc = new SenderCommonFunc();
            var wallet = _senderFamilyAndFriendsServices.GetRegisteredWallets();
            ViewBag.Wallets = new SelectList(wallet, "Id", "Name", walletId);

   
          //if(walletId==0)
          //  {
          //      ViewBag.Wallets = new SelectList(wallet, "Id", "Name");
          //  }
          //  else
          //  {
          //      ViewBag.Wallets = new SelectList(wallet, "Id", "Name", walletId);
          //  }
            
            SenderWalletTransactionStatementVM vm = new SenderWalletTransactionStatementVM();
            vm.SenderWalletTransactionStatementMaster = new SenderWalletTransactionStatementMasterVM();
            var senderWalletTransactionStatementMaster = new SenderWalletTransactionStatementMasterVM();
            
            var walletInfo = _senderCommonFunc.GetKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id, walletId);
            if (walletInfo != null)
            {
                senderWalletTransactionStatementMaster.AvailableBalance = _senderCommonFunc.GetCurrentKiiPayWalletBal(walletInfo.Id);
                senderWalletTransactionStatementMaster.Currency = Common.Common.GetCurrencySymbol(walletInfo.CardUserCountry);
                vm.SenderWalletTransactionStatementMaster = senderWalletTransactionStatementMaster;
            }
            vm.SenderWalletTransactionStatementDetail = _senderWalletTransactionStatementServices.GetWalletStatment(walletId, inout, Country);

            vm.SenderWalletTransactionStatementMaster.WalletId = walletId;
          
       
            return View(vm);

        }
    }
}