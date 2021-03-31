using FAXER.PORTAL.Areas.KiiPayBusiness.Services;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Controllers
{
    public class KiiPayBusinessPayMonthlyBillController : Controller
    {
        KiiPayBusinessPayMonthlyBillServices _kiiPayBusinessPayMonthlyBillServices = null;
        public KiiPayBusinessPayMonthlyBillController()
        {
            _kiiPayBusinessPayMonthlyBillServices = new KiiPayBusinessPayMonthlyBillServices();
        }

        // GET: KiiPayBusiness/KiiPayBusinessPayMonthlyBill
        public ActionResult Index()
        {
            return View();
        }

        #region LocalPayMonthlyBill
        [HttpGet]
        public ActionResult LocalPayMonthlyBill()
        {
            var vm = _kiiPayBusinessPayMonthlyBillServices.GetKiiPayBusinessEnterPaymentReference();
            return View(vm);

        }

        [HttpPost]
        public ActionResult LocalPayMonthlyBill([Bind(Include = KiiPayBusinessEnterPaymentReferenceVM.BindProperty)]KiiPayBusinessEnterPaymentReferenceVM vm)
        {
            if (ModelState.IsValid)
            {
                _kiiPayBusinessPayMonthlyBillServices.SetKiiPayBusinessEnterPaymentReference(vm);
                return RedirectToAction("LocalPayBillsReferenceOne");
            }
            return View(vm);

        }
        [HttpGet]
        public ActionResult LocalPayBillsReferenceOne()
        {
            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();
            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;
            var data = _kiiPayBusinessPayMonthlyBillServices.GetKiiPayBusinessEnterPaymentReference();
            var vm = _kiiPayBusinessPayMonthlyBillServices.GetLocalPayBillsReferenceOne();
            vm.ReferanceNo0 = data.ReferenceNo0;
            vm.ReferanceNo1 = data.ReferenceNo1;
            vm.ReferanceNo2 = data.ReferenceNo2;
            return View(vm);
        }

        [HttpPost]
        public ActionResult LocalPayBillsReferenceOne([Bind(Include = KiiPayBusinessPayBillsReferenceOneVM.BindProperty)]KiiPayBusinessPayBillsReferenceOneVM vm)
        {
            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();

            KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();
            if (ModelState.IsValid)
            {

                var HaveEnoughBal = _kiiPayBusinessCommonServices.DoesAccountHaveEnoughBal(vm.Amount);
                if (!HaveEnoughBal)
                {

                    kiiPayBusinessResult.Message = "You don't have enough balance in your wallet.";
                    kiiPayBusinessResult.Status = ResultStatus.Warning;
                }
                else
                {

                    _kiiPayBusinessPayMonthlyBillServices.CompletePayMothlyBillServices(vm);
                    return RedirectToAction("LocalPayBillsReferenceSuccess");
                }

            }

            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;
            return View(vm);

        }

        [HttpGet]
        public ActionResult LocalPayBillsReferenceSuccess()
        {
            var vm = _kiiPayBusinessPayMonthlyBillServices.GetKiiPayBusinessEnterPaymentReference();
            return View(vm);
        }
        #endregion

        #region InternationalPayMonthlyBill
        [HttpGet]
        public ActionResult InternationalPayMonthlyBill()
        {
            GetCountryDropDown();
            var vm = _kiiPayBusinessPayMonthlyBillServices.GetKiiPayBusinessInternationalSelectCountry();
            return View();
        }
        [HttpPost]
        public ActionResult InternationalPayMonthlyBill([Bind(Include = KiiPayBusinessInternationalSearchCountryVM.BindProperty)]KiiPayBusinessInternationalSearchCountryVM vm)
        {

            GetCountryDropDown();
            if (ModelState.IsValid)
            {
                _kiiPayBusinessPayMonthlyBillServices.SetKiiPayBusinessInternationalSelectCountry(vm);
                return RedirectToAction("InternationalPayBillsReference");
            }
            return View(vm);

        }
        [HttpGet]
        public ActionResult InternationalPayBillsReference()
        {
            var vm = _kiiPayBusinessPayMonthlyBillServices.GetKiiPayBusinessEnterPaymentReference();
            return View(vm);
        }
        [HttpPost]
        public ActionResult InternationalPayBillsReference([Bind(Include = KiiPayBusinessEnterPaymentReferenceVM.BindProperty)]KiiPayBusinessEnterPaymentReferenceVM vm)
        {
            if (ModelState.IsValid)
            {
                _kiiPayBusinessPayMonthlyBillServices.SetKiiPayBusinessEnterPaymentReference(vm);
                return RedirectToAction("InternationalPayBillsReferenceOne");
            }
            return View(vm);

        }

        [HttpGet]
        public ActionResult InternationalPayBillsReferenceOne()
        {
            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();
            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;
            var data = _kiiPayBusinessPayMonthlyBillServices.GetKiiPayBusinessEnterPaymentReference();
            var vm = _kiiPayBusinessPayMonthlyBillServices.GetInternationalPayBillsReferenceOne();
            vm.ReferanceNo0 = data.ReferenceNo0;
            vm.ReferanceNo1 = data.ReferenceNo1;
            vm.ReferanceNo2 = data.ReferenceNo2;
            return View(vm);
        }

        [HttpPost]
        public ActionResult InternationalPayBillsReferenceOne([Bind(Include = KiiPayBusinessInternationalPayBillsReferenceOneVM.BindProperty)]KiiPayBusinessInternationalPayBillsReferenceOneVM vm)
        {
            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();

            KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();
            if (ModelState.IsValid)
            {

                var HaveEnoughBal = _kiiPayBusinessCommonServices.DoesAccountHaveEnoughBal(vm.Amount);
                if (!HaveEnoughBal)
                {

                    kiiPayBusinessResult.Message = "You don't have enough balance in your wallet.";
                    kiiPayBusinessResult.Status = ResultStatus.Warning;
                }
                else
                {
                    _kiiPayBusinessPayMonthlyBillServices.CompleteInternationalPayMothlyBillServices(vm);
                    return RedirectToAction("InternationalPayBillsReferenceSuccess");
                }

            }

            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;
            return View(vm);

        }

        [HttpGet]
        public ActionResult InternationalPayBillsReferenceSuccess()
        {
            var vm = _kiiPayBusinessPayMonthlyBillServices.GetKiiPayBusinessEnterPaymentReference();
            return View(vm);
        }
        private void GetCountryDropDown()
        {
            ViewBag.Countries = new SelectList(Common.Common.GetCountriesForDropDown()
               , "CountryCode", "CountryName");
        }


        #endregion

    }
}