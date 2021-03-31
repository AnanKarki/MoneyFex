using FAXER.PORTAL.Areas.KiiPayPersonal.Services;
using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Controllers
{
    public class KiiPayPersonalStandingOrderPaymentController : Controller
    {
        KiiPayPersonalStandingOrderPaymentServices _kiiPayPersonalStandingOrderPaymentServices = null;
        KiiPayPersonalCommonServices _commonServices = null;
        public KiiPayPersonalStandingOrderPaymentController()
        {
            _commonServices = new KiiPayPersonalCommonServices();
            _kiiPayPersonalStandingOrderPaymentServices = new KiiPayPersonalStandingOrderPaymentServices();
        }

        // GET: KiiPayPersonal/KiiPayPersonalStandingOrderPayment
        public ActionResult Index()
        {
            return View();
        }

        #region BusinessStandingOrder
        public ActionResult BusinessStandingOrder(int ReceiverBusinessId = 0)
        {
            int PersonalWalletId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalWalletId;
            ViewBag.RecentltyPaidBusinesses = new SelectList(_commonServices.GetAllRecenltyPaidKiiPayPersonal(PersonalWalletId), "BusinessId", "MobileNo");

            var vm = _kiiPayPersonalStandingOrderPaymentServices.GetBusinessStandingOrderList(ReceiverBusinessId);
            return View(vm);
        }

        [HttpGet]
        public ActionResult AddNewBusinessStandingOrder(int Id)
        {
            KiiPayPersonalBusinessStandingOrdervm vm = new KiiPayPersonalBusinessStandingOrdervm();
            vm.ReceiverId = Id;
            vm.CurrencyCode = Common.Common.GetCountryCurrency(Common.CardUserSession.LoggedCardUserViewModel.CountryCode);
            vm.CurrencySymbol = Common.Common.GetCurrencySymbol(Common.CardUserSession.LoggedCardUserViewModel.CountryCode);
            return View(vm);
        }
        [HttpPost]
        public ActionResult AddNewBusinessStandingOrder([Bind(Include = KiiPayPersonalBusinessStandingOrdervm.BindProperty)]KiiPayPersonalBusinessStandingOrdervm vm)
        {
            if (ModelState.IsValid)
            {
                var result = _kiiPayPersonalStandingOrderPaymentServices.CompleteBusinessStandingOrderSetup(vm);
                return RedirectToAction("AddNewBusinessStandingOrderSuccess", result);
            }
            return View(vm);
        }
        [HttpGet]
        public ActionResult AddNewBusinessStandingOrderSuccess([Bind(Include = KiiPayPersonalAddNewBusinessStandingOrderSuccessvm.BindProperty)]KiiPayPersonalAddNewBusinessStandingOrderSuccessvm vm)
        {

            return View(vm);
        }
        [HttpGet]
        public ActionResult UpdateExistingBusinessStandingOrder(int Id)
        {

            var vm = _kiiPayPersonalStandingOrderPaymentServices.GetBusinessStandingOrderDetail(Id);
            return View(vm);
        }
        [HttpPost]
        public ActionResult UpdateExistingBusinessStandingOrder([Bind(Include = KiiPayPersonalUpdateBusinessStandingOrdervm.BindProperty)]KiiPayPersonalUpdateBusinessStandingOrdervm vm)
        {
            if (ModelState.IsValid)
            {
                _kiiPayPersonalStandingOrderPaymentServices.UpdateBusinessStandingOrder(vm);
                return RedirectToAction("UpdateBusinessStandingOrderSuccess");
            }
            return View(vm);
        }
        [HttpGet]
        public ActionResult UpdateBusinessStandingOrderSuccess()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DeleteBusinessStandingOrder(int Id)
        {
            _kiiPayPersonalStandingOrderPaymentServices.DeleteStandingOrderPayment(Id);
            return RedirectToAction("BusinessStandingOrder");
        }

        #endregion



        #region KiiPayWalletStandingOrder
        public ActionResult KiiPayWalletStandingOrder(int ReceiverId = 0)
        {
            int PersonalWalletId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalWalletId;
            ViewBag.GetAllRecenltyPaidKiiPayPersonalWalletInfo = new SelectList(_commonServices.GetAllRecenltyPaidKiiPayPersonalWalletInfo(PersonalWalletId)
                                                                                 , "WalletId", "MobileNo");
            var vm = _kiiPayPersonalStandingOrderPaymentServices.GetKiiPayPersonalStandingOrderDetails(ReceiverId);
            return View(vm);
        }



        [HttpGet]
        public ActionResult AddNewKiiPayWalletStandingOrder(int Id)
        {
            KiiPayPersonalBusinessStandingOrdervm vm = new KiiPayPersonalBusinessStandingOrdervm();
            vm.ReceiverId = Id;
            vm.CurrencyCode = Common.Common.GetCountryCurrency(Common.CardUserSession.LoggedCardUserViewModel.CountryCode);
            vm.CurrencySymbol = Common.Common.GetCurrencySymbol(Common.CardUserSession.LoggedCardUserViewModel.CountryCode);
            return View(vm);
        }
        [HttpPost]
        public ActionResult AddNewKiiPayWalletStandingOrder([Bind(Include = KiiPayPersonalBusinessStandingOrdervm.BindProperty)]KiiPayPersonalBusinessStandingOrdervm vm)
        {
            if (ModelState.IsValid)
            {
                var result = _kiiPayPersonalStandingOrderPaymentServices.CompleteKiiPayPersonalStandingOrderSetup(vm);
                return RedirectToAction("AddNewKiiPayWalletStandingOrderSuccess", result);
            }
            return View();
        }
        [HttpGet]
        public ActionResult AddNewKiiPayWalletStandingOrderSuccess([Bind(Include = KiiPayPersonalAddNewBusinessStandingOrderSuccessvm.BindProperty)]KiiPayPersonalAddNewBusinessStandingOrderSuccessvm vm)
        {
            return View(vm);
        }
        [HttpGet]
        public ActionResult UpdateExistingKiiPayWalletStandingOrder(int Id)
        {
            var vm = _kiiPayPersonalStandingOrderPaymentServices.GetKiiPayPersonalStandingOrderDetail(Id);
            //vm.WalletId = Id;
            return View(vm);
        }
        [HttpPost]
        public ActionResult UpdateExistingKiiPayWalletStandingOrder([Bind(Include = KiiPayPersonalUpdateBusinessStandingOrdervm.BindProperty)]KiiPayPersonalUpdateBusinessStandingOrdervm vm)
        {
            if (ModelState.IsValid)
            {

                var result = _kiiPayPersonalStandingOrderPaymentServices.UpdateKiiPayPersonalStandingOrder(vm);
                return RedirectToAction("UpdateKiiPayWalletStandingOrderSuccess");
            }
            return View();
        }
        [HttpGet]
        public ActionResult UpdateKiiPayWalletStandingOrderSuccess()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DeleteKiiPayWalletStandingOrder(int Id)
        {
            _kiiPayPersonalStandingOrderPaymentServices.DeleteKiiPayPersonalStandingOrderPayment(Id);
            return RedirectToAction("KiiPayWalletStandingOrder");
        }
        #endregion



    }
}