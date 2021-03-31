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
    public class RequestAPaymentController : Controller
    {
        PayARequestServices _payARequestServices = null;
        RequestAPaymentServices _services = null;
        public RequestAPaymentController()
        {
            _services = new RequestAPaymentServices();
            _payARequestServices = new PayARequestServices();
        }
        // GET: KiiPayPersonal/RequestAPayment
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RequestHistory(int filterKey =0)
        {
            SetViewBagForFilter();
            var vm = _services.getRequestHistory();
            if(filterKey == 1)
            {
                vm.RequestHistoryList = vm.RequestHistoryList.Where(x => x.Status == DB.RequestPaymentStatus.Paid).ToList();
            }
            else if(filterKey == 2)
            {
                vm.RequestHistoryList = vm.RequestHistoryList.Where(x => x.Status == DB.RequestPaymentStatus.UnPaid).ToList();
            }
            else if (filterKey == 3)
            {
                vm.RequestHistoryList = vm.RequestHistoryList.Where(x => x.Status == DB.RequestPaymentStatus.Cancelled).ToList();
            }
            vm.FilterKey = filterKey;
            return View(vm);
        }

        public ActionResult DeleteRequest(int id)
        {
            bool isPaid = _services.isRequestAlreadyPaid(id);
            if (isPaid == true)
            {
                return RedirectToAction("RequestHistory");
            }
            _services.deleteRequest(id);
            return RedirectToAction("RequestHistory");
        }

        public ActionResult CancelRequest(int id)
        {
            bool isPaid = _services.isRequestAlreadyPaid(id);
            if (isPaid == true)
            {
                return RedirectToAction("RequestHistory");
            }
            _services.cancelRequest(id);
            return RedirectToAction("RequestHistory");
        }

        public ActionResult EditRequest(int id)
        {
            if(id != 0)
            {
                bool isPaid = _services.isRequestAlreadyPaid(id);
                if(isPaid == true)
                {
                    return RedirectToAction("RequestHistory");
                }
                PaymentType? type = _services.getRequestType(id);
                if(type == PaymentType.Local)
                {
                    return RedirectToAction("EditLocalRequest", new { @id=id});
                }
                else if(type == PaymentType.International)
                {
                    return RedirectToAction("EditInternationalRequest", new { @id=id});
                }
            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult EditLocalRequest(int id)
        {
            var vm = _services.getEnterAmountLocalRequestVM(id);
            
            if (vm == null)
            {
                return RedirectToAction("RequestHistory");
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditLocalRequest([Bind(Include = SendRequestEnterAmountViewModel.BindProperty)]SendRequestEnterAmountViewModel model)
        {
            if(ModelState.IsValid)
            {
                if(model.Amount == 0)
                {
                    ModelState.AddModelError("Amount", "Invalid Amount !");
                    return View(model);
                }
                if(model.Amount > Common.CardUserSession.LoggedCardUserViewModel.BalanceOnCard)
                {
                    ModelState.AddModelError("Amount", "Invalid Amount !");
                    return View(model);
                }
                bool updateData = _services.updateLocalRequestData(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult EditInternationalRequest(int id)
        {
            var vm = _services.getEnterAmountInternationalRequestVM(id);
            if(vm == null)
            {
                return RedirectToAction("RequestHistory");
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditInternationalRequest([Bind(Include = SendRequesEnterAmountAbroadViewModel.BindProperty)]SendRequesEnterAmountAbroadViewModel model)
        {
            if(ModelState.IsValid)
            {
                if(model.ForeignAmount == 0)
                {
                    ModelState.AddModelError("ForeignAmount", "Invalid Amount !");
                    return View(model);
                }
                if(model.LocalAmount == 0)
                {
                    ModelState.AddModelError("LocalAmount", "Invalid Amount !");
                    return View(model);
                }
                if(model.ForeignAmount > Common.CardUserSession.LoggedCardUserViewModel.BalanceOnCard)
                {
                    ModelState.AddModelError("ForeignAmount", "Invalid Amount !");
                    return View(model);
                }
                bool updateData = _services.updateInternationalRequestData(model);
                return RedirectToAction("RequestHistory");

            }
            return View(model);
        }


        public void SetViewBagForFilter()
        {
            var filterList = _payARequestServices.getPaidStatusList();
            ViewBag.Filter = new SelectList(filterList, "Key", "Value");
        }
    }
}