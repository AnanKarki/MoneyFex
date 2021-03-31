using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.SenderRequestAPayment
{
    public class SenderPayARequestController : Controller
    {
        SSenderPayARequest _payARequestServices = null;
        public SenderPayARequestController()
        {
            _payARequestServices = new SSenderPayARequest();
        }
        // GET: SenderPayARequest
        public ActionResult Index(int filterKey = 0)
        {
            SetViewBagForFilter();
            var vm = _payARequestServices.GetPayRequests();
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

        public ActionResult PayARequest(int id)
        {
            SenderPayAPaymentRequestViewModel model = _payARequestServices.GetValidRequestReceived(id);
            if (model != null)
            {
                return View(model);
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult PayARequest([Bind(Include = SenderPayAPaymentRequestViewModel.BindProperty)] SenderPayAPaymentRequestViewModel model)
        {
            if (model.AvailableBalance < model.PayingAmount)
            {
                ModelState.AddModelError("PayingAmount", "You don't have sufficient balance !");
                return View(model);
            }
            bool makePayment = _payARequestServices.PayAPaymentRequest();
            if (makePayment == true)
            {
                return RedirectToAction("PaymentSuccess");
            }
            return View(model);
        }


        public ActionResult PaymentSuccess()
        {
            var data = _payARequestServices.GetSenderPayARequestSession();
            return View(data);
        }
        public void SetViewBagForFilter()
        {
            var filterList = _payARequestServices.getPaidStatusList();
            ViewBag.Filter = new SelectList(filterList, "Key", "Value");
        }
    }
}