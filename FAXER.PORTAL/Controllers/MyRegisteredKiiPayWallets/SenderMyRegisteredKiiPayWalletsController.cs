using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.MyRegisteredKiiPayWallets
{
    public class SenderMyRegisteredKiiPayWalletsController : Controller
    {
        // GET: SenderMyRegisteredKiiPayWallets
        private SSenderRegisteredWallets _senderRegisteredWalletsServices = null;
        public SenderMyRegisteredKiiPayWalletsController()
        {
            _senderRegisteredWalletsServices = new SSenderRegisteredWallets();
        }
       [HttpGet]
        public ActionResult Index()
        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            ViewBag.WalletCount = senderCommonFunc.GetwalletCountOfSender(Common.FaxerSession.LoggedUser.Id);

            ViewBag.SelfWalletCount = senderCommonFunc.GetwalletCountOfSelf(Common.FaxerSession.LoggedUser.Id);
            return View();
        }

        #region  SenderPersonalKiiPayWallet
        [HttpGet]

        
        public ActionResult SenderPersonalKiiPayWalletIndex()
        {
            return View();
        }

        public ActionResult SenderFamilyAndFriendsKiiPayWallet()
        {
            ViewBag.Wallets = new SelectList(Wallets(), "Id", "FirstName");
            return View();
        }

        public List<KiiPayPersonalWalletInformation> Wallets()
        {
            var SenderId = FaxerSession.LoggedUser.Id;
            var SendingWallets = (from c in _senderRegisteredWalletsServices.List().Data.ToList()
                                  join d in _senderRegisteredWalletsServices.ListofSender().Data.Where(x => x.SenderId == SenderId) on c.Id equals d.KiiPayPersonalWalletId
                                  select c).ToList();
            return SendingWallets;

        }
        #endregion

        public JsonResult GetWithdrawalCode(int WalletId)
        {

            DB.FAXEREntities db = new FAXEREntities();


            var data = db.KiiPayPersonalWalletWithdrawalCode.Where(x => x.KiiPayPersonalWalletId == WalletId && x.IsExpired == false).FirstOrDefault();

            if (data == null)
            {

                KiiPayPersonalWalletWithdrawalCode cardUserWithdrawalCode = new KiiPayPersonalWalletWithdrawalCode()
                {
                    KiiPayPersonalWalletId = WalletId,
                    AccessCode = Common.Common.GetNewAccessCodeForCardUser(),
                    IsExpired = false,
                    CreatedDateTime = DateTime.Now,

                };
                data = db.KiiPayPersonalWalletWithdrawalCode.Add(cardUserWithdrawalCode);
                db.SaveChanges();

            }

            return Json(new
            {
                AccessCode = data.AccessCode
            }, JsonRequestBehavior.AllowGet);

        }
    }
}