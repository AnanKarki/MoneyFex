using FAXER.PORTAL.Areas.CardUsers.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.CardUsers.Controllers
{
    public class CardUserHomeController : Controller
    {
        Services.CardUserCommonServices _cardUserCommonServices = null;
        public CardUserHomeController()
        {
            _cardUserCommonServices = new Services.CardUserCommonServices();
        }


        public ActionResult Home() {

            return View();
        }
        // GET: CardUsers/CardUserHome
        public ActionResult Index()
        {
            DemoLoginModel model = new DemoLoginModel();
            model.UserName = "Demo";
            model.Password = "Demo123@";
            Common.FaxerSession.DemoLoginModel = model;

            if (Common.CardUserSession.LoggedCardUserViewModel == null) {
                return RedirectToAction("Login", "CardUserLogin");
            }
            Services.PayGoodsAndServicesAbroadServices services = new Services.PayGoodsAndServicesAbroadServices();
            ViewBag.AmountonCard = services.GetAmountOnCard();
            Common.CardUserSession.LoggedCardUserViewModel.BalanceOnCard = _cardUserCommonServices.getCurrentBalanceOnCard(Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId);
            ViewBag.Photo = services.getCardPhoto();
            ViewBag.TransferinProgressCount = _cardUserCommonServices.TotalNonCardTransfer();
            Session.Remove("MerchantNationalPayingAmount_CardUserViewModel");
            Session.Remove("MerchantDetailsViewModel_CardUserViewModel");
            Session.Remove("TransactionSummaryURL");
            Common.CardUserSession.MFTCCardPaymentByCardUserSuccessful = false;

            return View();
        }

        public JsonResult GetWithdrawalCode()
        {

            int CardId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId;
            string withdrawalCode  = GetWithdrawalCode(CardId);


            return Json(new
            {
                AccessCode = withdrawalCode
            }, JsonRequestBehavior.AllowGet);

        }



        private string GetWithdrawalCode(int CardId) {


            CardUserWithdrawalServices cardUserWithdrawalServices = new CardUserWithdrawalServices();
            var data = cardUserWithdrawalServices.GetExistingCardUserWithdrawalCode(CardId);

            if (data == null)
            {
                KiiPayPersonalWalletWithdrawalCode model = new KiiPayPersonalWalletWithdrawalCode()
                {
                    KiiPayPersonalWalletId = CardId,
                    AccessCode = Common.Common.GetNewAccessCodeForCardUser(),
                    IsExpired = false,
                    CreatedDateTime = DateTime.Now,

                };
                data = cardUserWithdrawalServices.AddBNewCardWithdrawalCode(model);
            }

            return data.AccessCode;


        }

    }
}