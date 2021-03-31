using FAXER.PORTAL.Areas.KiiPayBusiness.Services;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Controllers
{
    public class KiiPayBusinessStandingOrderPaymentController : Controller
    {
        KiiPayBusinessStandingOrderPaymentServices _kiiPayBusinessStandingOrderPaymenServices = null;
        KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = null;
        public KiiPayBusinessStandingOrderPaymentController()
        {
            _kiiPayBusinessStandingOrderPaymenServices = new KiiPayBusinessStandingOrderPaymentServices();
            _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();
        }
        // GET: KiiPayBusiness/KiiPayBusinessStandingOrderPayment
        public ActionResult Index()
        {
            return View();
        }

        #region BusinessStandingOrder
        public ActionResult BusinessStandingOrder(int ReceiverBusinessId = 0)
        {
            int BusinessId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId;
            ViewBag.RecentltyPaidBusinesses = new SelectList( _kiiPayBusinessCommonServices.GetAllRecenltyPaidKiiPayBusinesses(BusinessId) , "BusinessId", "MobileNo");

            var vm = _kiiPayBusinessStandingOrderPaymenServices.GetBusinessStandingOrderList(ReceiverBusinessId);
            return View(vm);
        }
       
        [HttpGet]
        public ActionResult AddNewBusinessStandingOrder(int Id)
        {
            BusinessStandingOrdervm vm = new BusinessStandingOrdervm();
            vm.ReceiverId = Id;
            vm.CurrencyCode = Common.Common.GetCountryCurrency(Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode);
            vm.CurrencySymbol = Common.Common.GetCurrencySymbol(Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode);
            return View(vm);
        }
        [HttpPost]
        public ActionResult AddNewBusinessStandingOrder([Bind(Include = BusinessStandingOrdervm.BindProperty)]BusinessStandingOrdervm vm)
        {
            if (ModelState.IsValid)
            {
                var result = _kiiPayBusinessStandingOrderPaymenServices.CompleteBusinessStandingOrderSetup(vm);
                return RedirectToAction("AddNewBusinessStandingOrderSuccess" , result);
            }
            return View(vm);
        }
        [HttpGet]
        public ActionResult AddNewBusinessStandingOrderSuccess([Bind(Include = AddNewBusinessStandingOrderSuccessvm.BindProperty)]AddNewBusinessStandingOrderSuccessvm vm)
        {

            return View(vm);
        }
        [HttpGet]
        public ActionResult UpdateExistingBusinessStandingOrder(int Id)
        {
            
            var vm = _kiiPayBusinessStandingOrderPaymenServices.GetBusinessStandingOrderDetail(Id);
            return View(vm);
        }
        [HttpPost]
        public ActionResult UpdateExistingBusinessStandingOrder([Bind(Include = UpdateBusinessStandingOrdervm.BindProperty)]UpdateBusinessStandingOrdervm vm)
        {
            if (ModelState.IsValid)
            {
                _kiiPayBusinessStandingOrderPaymenServices.UpdateBusinessStandingOrder(vm);
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
            _kiiPayBusinessStandingOrderPaymenServices.DeleteStandingOrderPayment(Id);
            return RedirectToAction("BusinessStandingOrder");
        }

        #endregion



        #region KiiPayWalletStandingOrder
        public ActionResult KiiPayWalletStandingOrder( int ReceiverId = 0)
        {
            int BusinessId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId;
            ViewBag.GetAllRecenltyPaidKiiPayPersonalWalletInfo = new SelectList(_kiiPayBusinessCommonServices.GetAllRecenltyPaidKiiPayPersonalWalletInfo(BusinessId)
                                                                                 , "WalletId" , "MobileNo");
            var vm = _kiiPayBusinessStandingOrderPaymenServices.GetKiiPayPersonalStandingOrderDetails(ReceiverId);
            return View(vm);
        }
       
       
    
        [HttpGet]
        public ActionResult AddNewKiiPayWalletStandingOrder(int Id)
        {
            BusinessStandingOrdervm vm = new BusinessStandingOrdervm();
            vm.ReceiverId = Id;
            vm.CurrencyCode = Common.Common.GetCountryCurrency(Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode);
            vm.CurrencySymbol = Common.Common.GetCurrencySymbol(Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode);
            return View(vm);
        }
        [HttpPost]
        public ActionResult AddNewKiiPayWalletStandingOrder([Bind(Include = BusinessStandingOrdervm.BindProperty)]BusinessStandingOrdervm vm)
        {
            if (ModelState.IsValid)
            {
                var result = _kiiPayBusinessStandingOrderPaymenServices.CompleteKiiPayPersonalStandingOrderSetup(vm);
                return RedirectToAction("AddNewKiiPayWalletStandingOrderSuccess" , result);
            }
            return View();
        }
        [HttpGet]
        public ActionResult AddNewKiiPayWalletStandingOrderSuccess([Bind(Include = AddNewBusinessStandingOrderSuccessvm.BindProperty)]AddNewBusinessStandingOrderSuccessvm vm)
        {
            return View(vm);
        }
        [HttpGet]
        public ActionResult UpdateExistingKiiPayWalletStandingOrder(int Id)
        {
            var vm = _kiiPayBusinessStandingOrderPaymenServices.GetKiiPayPersonalStandingOrderDetail(Id);
            //vm.WalletId = Id;
            return View(vm);
        }
        [HttpPost]
        public ActionResult UpdateExistingKiiPayWalletStandingOrder([Bind(Include = UpdateBusinessStandingOrdervm.BindProperty)]UpdateBusinessStandingOrdervm vm)
        {
            if (ModelState.IsValid)
            {

                var result = _kiiPayBusinessStandingOrderPaymenServices.UpdateKiiPayPersonalStandingOrder(vm);
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
            _kiiPayBusinessStandingOrderPaymenServices.DeleteKiiPayPersonalStandingOrderPayment(Id);
            return RedirectToAction("KiiPayWalletStandingOrder");
        }
        #endregion
    }
}