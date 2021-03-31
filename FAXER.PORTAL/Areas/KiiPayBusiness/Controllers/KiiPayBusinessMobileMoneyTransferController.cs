using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Controllers
{
    public class KiiPayBusinessMobileMoneyTransferController : Controller
    {
        // GET: KiiPayBusiness/KiiPayBusinessMobileMoneyTransfer
        public ActionResult Index()
        {

            var recentNumbers = new List<string>();
            ViewBag.RecentNumbers = new SelectList(recentNumbers);

            KiiPayBusinessMobileMoneyTransferVM vm = new KiiPayBusinessMobileMoneyTransferVM();


            return View(vm);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = KiiPayBusinessMobileMoneyTransferVM.BindProperty)]KiiPayBusinessMobileMoneyTransferVM vm)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("NationalMobileMoneySending", "KiiPayBusinessMobileMoneyTransfer");
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult NationalMobileMoneySending()
        {
            KiiPayBusinessNationalMobileMoneySendingVM vm = new KiiPayBusinessNationalMobileMoneySendingVM();
            return View(vm);
        }

        [HttpPost]
        public ActionResult NationalMobileMoneySending([Bind(Include = KiiPayBusinessNationalMobileMoneySendingVM.BindProperty)]KiiPayBusinessNationalMobileMoneySendingVM vm)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("NationalMobileAccountPaymentSummary", "KiiPayBusinessMobileMoneyTransfer");
            }

            return View(vm);
        }

        [HttpGet]
        public ActionResult NationalMobileAccountPaymentSummary()
        {
            KiiPayBusinessNationalMobileAccountPaymentSummaryVM vm = new KiiPayBusinessNationalMobileAccountPaymentSummaryVM();
            return View(vm);
        }

        [HttpPost]
        public ActionResult NationalMobileAccountPaymentSummary([Bind(Include = KiiPayBusinessNationalMobileAccountPaymentSummaryVM.BindProperty)]KiiPayBusinessNationalMobileAccountPaymentSummaryVM vm)
        {
            return RedirectToAction("NationalPaymentSuccess", "KiiPayBusinessMobileMoneyTransfer");
        }

        [HttpGet]
        public ActionResult NationalPaymentSuccess()
        {
            KiiPayBusinessNAtionalMobilePaymentSuccessVM vm = new KiiPayBusinessNAtionalMobilePaymentSuccessVM();
            return View(vm);
        }

        [HttpGet]
        public ActionResult InternationalMoileMonueySending()
        {
            var recentNumbers = new List<string>();
            ViewBag.RecentNumbers = new SelectList(recentNumbers);
            ViewBag.Countries = new SelectList(recentNumbers);

            KiiPayBusinessInternationalMobileMoneySendingVM vm = new KiiPayBusinessInternationalMobileMoneySendingVM();
            return View(vm);
        }

        [HttpPost]
        public ActionResult InternationalMoileMonueySending([Bind(Include = KiiPayBusinessInternationalMobileMoneySendingVM.BindProperty)]KiiPayBusinessInternationalMobileMoneySendingVM vm)
        {
            var recentNumbers = new List<string>();
            ViewBag.RecentNumbers = new SelectList(recentNumbers);
            ViewBag.Countries = new SelectList(recentNumbers);
            if (ModelState.IsValid)
            {
                return RedirectToAction("InternationalEnterMobileAmount", "KiiPayBusinessMobileMoneyTransfer");
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult InternationalEnterMobileAmount()
        {
            KiiPayBusinessInternationalEnterMobileAmountVM vm = new KiiPayBusinessInternationalEnterMobileAmountVM();
            return View(vm);
        }

        [HttpPost]
        public ActionResult InternationalEnterMobileAmount([Bind(Include = KiiPayBusinessInternationalEnterMobileAmountVM.BindProperty)]KiiPayBusinessInternationalEnterMobileAmountVM vm)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("InternationalMobileTransferSummary", "KiiPayBusinessMobileMoneyTransfer");
            }

            return View(vm);
        }

        [HttpGet]
        public ActionResult InternationalMobileTransferSummary()
        {
            KiiPayBusinessInternationalMobileTransferSummaryVM vm = new KiiPayBusinessInternationalMobileTransferSummaryVM();
            return View(vm);
        }

        [HttpPost]

        public ActionResult InternationalMobileTransferSummary([Bind(Include = KiiPayBusinessInternationalMobileTransferSummaryVM.BindProperty)]KiiPayBusinessInternationalMobileTransferSummaryVM vm)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("InternationalMobilePaymentSuccess", "KiiPayBusinessMobileMoneyTransfer");
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult InternationalMobilePaymentSuccess()
        {
            KiiPayBusinessInternationalMobilePaymentSuccessVM vm = new KiiPayBusinessInternationalMobilePaymentSuccessVM();
            return View(vm);
        }

     
    }
}