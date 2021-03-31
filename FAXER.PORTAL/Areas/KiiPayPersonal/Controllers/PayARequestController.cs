using FAXER.PORTAL.Areas.KiiPayPersonal.Services;
using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Controllers
{
    public class PayARequestController : Controller
    {
        PayARequestServices _service = null;
        public PayARequestController()
        {
            _service = new PayARequestServices();
        }
        // GET: KiiPayPersonal/PayARequest
        public ActionResult Index(int filterKey = 0)
        {
            SetViewBagForFilter();
            var vm = _service.getPayRequestsPageViewModel();
            if (filterKey != 0)
            {
                if (filterKey == 1)
                {
                    vm.RequestsList = vm.RequestsList.Where(x => x.Status == RequestPaymentStatus.Paid).ToList();
                }
                if (filterKey == 2)
                {
                    vm.RequestsList = vm.RequestsList.Where(x => x.Status == RequestPaymentStatus.UnPaid).ToList();
                }
                if (filterKey == 3)
                {
                    vm.RequestsList = vm.RequestsList.Where(x => x.Status == RequestPaymentStatus.Cancelled).ToList();
                }

            }
            vm.StatusList = filterKey;
            return View(vm);
        }

        [HttpGet]
        public ActionResult PayARequest(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index");
            }
            bool isRequestAlreadyPaid = _service.isRequestAlreadyPaid(id);
            if (isRequestAlreadyPaid == true)
            {
                return RedirectToAction("Index");
            }
            bool isRequestCancelled = _service.isRequestCancelled(id);
            if(isRequestCancelled == true)
            {
                return RedirectToAction("Index");
            }
              

            var vm = _service.getPaymentSummaryPageViewModel(id);
            return View(vm);
        }

        [HttpPost]
        public ActionResult PayARequest([Bind(Include = PayAPaymentRequestViewModel.BindProperty)]PayAPaymentRequestViewModel model)
        {
            if (Common.CardUserSession.LoggedCardUserViewModel.BalanceOnCard < model.PayingAmount)
            {
                ModelState.AddModelError("PayingAmount", "You don't have sufficient balance !");
                return View(model);
            }
            if (Common.CardUserSession.PayARequestSession != null)
            {
                bool makePayment = _service.payAPaymentRequest();
                if (makePayment == true)
                {
                    return RedirectToAction("PaymentSuccess");
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult PaymentSuccess()
        {
            if (Common.CardUserSession.PayARequestSession == null)
            {
                return RedirectToAction("Index");
            }
            var vm = new WalletPaymentSuccessViewModel()
            {
                SentAmount = Common.CardUserSession.PayARequestSession.Amount,
                Receiver = Common.CardUserSession.PayARequestSession.ReceiverName
            };
            Common.CardUserSession.PayARequestSession = null;
            return View(vm);
        }

        

        public void SetViewBagForFilter()
        {
            var filterList = _service.getPaidStatusList();
            ViewBag.Filter = new SelectList(filterList, "Key", "Value");
        }
    }
}