using FAXER.PORTAL.Areas.KiiPayBusiness.Services;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Controllers
{
    public class KiiPayBusinessPayForServicesController : Controller
    {
        KiiPayBusinessPayForServicesServices _kiiPayBusinessPayForServicesServices = null;

        KiiPayBusinessCommonServices _KiiPayBusinessCommonServices = null;
        public KiiPayBusinessPayForServicesController()
        {
            _kiiPayBusinessPayForServicesServices = new KiiPayBusinessPayForServicesServices();
            _KiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();
        }
        // GET: KiiPayBusiness/KiiPayBusinessPayForServices
        public ActionResult Index()
        {
            return View();
        }

        #region Local Transaction 


        [HttpGet]
        public ActionResult LocalSearchBusinessProvider()
        {


            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();
            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;
            GetRecenltyPaidNationalBusinesses();

            return View();

        }

        [HttpPost]
        public ActionResult LocalSearchBusinessProvider([Bind(Include = KiiPayBusinessSearchBusinessProviderVM.BindProperty )]KiiPayBusinessSearchBusinessProviderVM vm)
        {


            GetRecenltyPaidNationalBusinesses();
            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();

            if (ModelState.IsValid)
            {


                bool IsValidMobileNo = _kiiPayBusinessPayForServicesServices.IsValidMobileNo(vm.MobileNo);
                var IsValidCardStatus = _KiiPayBusinessCommonServices.ValidCardStatus(_kiiPayBusinessPayForServicesServices.GetCardStatus(vm.MobileNo));
                if (!IsValidMobileNo)
                {

                    kiiPayBusinessResult.Message = "Please enter the valid mobile no..";
                    kiiPayBusinessResult.Status = ResultStatus.Warning;

                }
                else if (IsValidCardStatus.Status != ResultStatus.OK)
                {

                    kiiPayBusinessResult.Message = IsValidCardStatus.Message;
                    kiiPayBusinessResult.Status = ResultStatus.Warning;

                }
                else if (!_kiiPayBusinessPayForServicesServices.IsValidTransfer(vm.MobileNo, true))
                {


                    kiiPayBusinessResult.Message = "Local transaction cannot be performed to the  mobile no you have entered . Please choose international payment";
                    kiiPayBusinessResult.Status = ResultStatus.Warning;
                }
                else
                {
                    _kiiPayBusinessPayForServicesServices.KiiPayBusinessLocalSearchBusinessProviderSuccessFul(vm);
                    return RedirectToAction("LocalTransferEnterAmount");
                }
            }
            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;
            return View(vm);

        }

        [HttpGet]
        public ActionResult LocalTransferEnterAmount()
        {

            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();
            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;
            var vm = _kiiPayBusinessPayForServicesServices.GetKiiPayBusinessLocalTransferEnterAmountVM();
            return View(vm);

        }
        [HttpPost]
        public ActionResult LocalTransferEnterAmount([Bind(Include = KiiPayBusinessLocalTransferEnterAmountVM.BindProperty)]KiiPayBusinessLocalTransferEnterAmountVM vm)
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
                    var kiiPayBusinessLocalTransferAmountSummary = Common.BusinessSession.KiiPayBusinessLocalTransferAmount;
                    kiiPayBusinessLocalTransferAmountSummary.PaymentReference = vm.PaymentReference;
                    kiiPayBusinessLocalTransferAmountSummary.Amount = vm.Amount;
                    _kiiPayBusinessPayForServicesServices.SetKiiPayBusinessLocallTransferEnterAmountVM(kiiPayBusinessLocalTransferAmountSummary);
                    if (vm.SendSMS == true) {

                        Common.BusinessSession.SendSMSForLocalPayment = true;
                    }
                    return RedirectToAction("LocalTransferPaymentSummary");
                }

            }

            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;
            return View(vm);

        }


        [HttpGet]
        public ActionResult LocalTransferPaymentSummary()
        {

            var vm = _kiiPayBusinessPayForServicesServices.GetLocalTransferPaymentSummary();
            return View(vm);
        }

        [HttpPost]
        public ActionResult CompleteLocalTransfer([Bind(Include = KiiPayBusinessPaymentSummaryVM.BindProperty)]KiiPayBusinessPaymentSummaryVM vm)
        {


            var result = _kiiPayBusinessPayForServicesServices.CompleteLocalTransfer();
            return View(result);
        }

        public void GetRecenltyPaidNationalBusinesses()
        {

            int KiiPayBusinessInfoId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId;
            var RecenltyPaidNationalBusinesses = _KiiPayBusinessCommonServices.GetRecentlyPaidNationalKiiPayBusiness(KiiPayBusinessInfoId);
            ViewBag.RecenltyPaidNationalBusinesses = new SelectList(RecenltyPaidNationalBusinesses, "MobileNo", "MobileNo");
        }
        #endregion

        #region International Transaction 
        [HttpGet]
        public ActionResult InternationalSearchBusinessProvider(string Country="")
        {
            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();
            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;
            GetCountryDropDown(Country);
            GetRecenltyPaidInternationalBusinesses(Country);
            return View();

        }

        [HttpPost]
        public ActionResult InternationalSearchBusinessProvider([Bind(Include = KiiPayBusinessInternationalSearchBusinessProviderVM.BindProperty)]KiiPayBusinessInternationalSearchBusinessProviderVM vm)
        {

            GetCountryDropDown(vm.Country);
            GetRecenltyPaidInternationalBusinesses(vm.Country);
            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();
            if (ModelState.IsValid)
            {
                bool IsValidMobileNo = _kiiPayBusinessPayForServicesServices.IsValidMobileNo(vm.MobileNo);
                if (!IsValidMobileNo)
                {

                    kiiPayBusinessResult.Message = "Please enter the valid mobile no..";
                    kiiPayBusinessResult.Status = ResultStatus.Warning;

                }
                else if (!_kiiPayBusinessPayForServicesServices.IsValidTransfer(vm.MobileNo, true))
                {


                    kiiPayBusinessResult.Message = "international transaction cannot be performed to the  mobile no you have entered . Please choose local payment";
                    kiiPayBusinessResult.Status = ResultStatus.Warning;
                }
                else
                {
                    _kiiPayBusinessPayForServicesServices.KiiPayBusinessInternationalSearchBusinessProviderSuccessFul(vm);

                    return RedirectToAction("InternationalTransferEnterAmount");
                }
            }
            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;
            return View(vm);

        }
        private void GetCountryDropDown(string Country= "")
        {
            ViewBag.Countries = new SelectList(Common.Common.GetCountriesForDropDown()
               , "CountryCode", "CountryName" , Country);
        }

        

        [HttpGet]
        public ActionResult InternationalTransferEnterAmount()
        {

            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();
            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;
            var vm = _kiiPayBusinessPayForServicesServices.GetKiiPayBusinessInternationalTransferEnterAmountVM();
            return View(vm);

        }
        [HttpPost]
        public ActionResult InternationalTransferEnterAmount([Bind(Include = KiiPayBusinessInternationalTransferEnterAmountVM.BindProperty)]KiiPayBusinessInternationalTransferEnterAmountVM vm)
        {
            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();

            KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();
            if (ModelState.IsValid)
            {

                var HaveEnoughBal = _kiiPayBusinessCommonServices.DoesAccountHaveEnoughBal(vm.SendingAmount);
                if (!HaveEnoughBal)
                {

                    kiiPayBusinessResult.Message = "You don't have enough balance in your wallet.";
                    kiiPayBusinessResult.Status = ResultStatus.Warning;
                }
                else
                {
                    var kiiPayBusinessInternationalTransferAmountSummary = Common.BusinessSession.KiiPayBusinessInternationalTransferAmount;
                    kiiPayBusinessInternationalTransferAmountSummary.PaymentReference = vm.PaymentReference;
                    _kiiPayBusinessPayForServicesServices.SetKiiPayBusinessInternationalTransferEnterAmountVM(kiiPayBusinessInternationalTransferAmountSummary);
                    return RedirectToAction("InternationalTransferPaymentSummary");
                }

            }

            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;
            return View(vm);

        }

        public JsonResult GetPaymentSummary(decimal SendingAmount, decimal ReceivingAmount)
        {
            bool IsReceivingAmount = false;
            var kiiPayBusinessInternationalTransferAmountSummary = Common.BusinessSession.KiiPayBusinessInternationalTransferAmount;
            if ((SendingAmount > 0 && ReceivingAmount > 0) && kiiPayBusinessInternationalTransferAmountSummary.RecevingAmount != ReceivingAmount) {

                SendingAmount = ReceivingAmount;
                IsReceivingAmount = true;
            }

            if (SendingAmount == 0) {

                IsReceivingAmount = true;
                SendingAmount = ReceivingAmount;
            }
            var result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
                kiiPayBusinessInternationalTransferAmountSummary.ExchangeRate, SEstimateFee.GetFaxingCommision(Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode));

            // Rewrite session with additional value 
            kiiPayBusinessInternationalTransferAmountSummary.Fee = result.FaxingFee;
            kiiPayBusinessInternationalTransferAmountSummary.SendingAmount = result.FaxingAmount;
            kiiPayBusinessInternationalTransferAmountSummary.RecevingAmount = result.ReceivingAmount;
            kiiPayBusinessInternationalTransferAmountSummary.TotalAmount = result.TotalAmount;
            _kiiPayBusinessPayForServicesServices.SetKiiPayBusinessInternationalTransferEnterAmountVM(kiiPayBusinessInternationalTransferAmountSummary);

            return Json(new
            {
                Fee = result.FaxingFee,
                TotalAmount = result.TotalAmount,
                RecevingAmount = result.ReceivingAmount,
                SendingAmount = result.FaxingAmount
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult InternationalTransferPaymentSummary()
        {

            var vm = _kiiPayBusinessPayForServicesServices.GetInternationalTransferPaymentSummary();
            return View(vm);
        }

        [HttpPost]
        public ActionResult CompleteInternationalTransfer([Bind(Include = KiiPayBusinessPaymentSummaryVM.BindProperty)]KiiPayBusinessPaymentSummaryVM vm)
        {
            var result = _kiiPayBusinessPayForServicesServices.CompleteInternationalTransfer();
            return View(result);
        }

        public void GetRecenltyPaidInternationalBusinesses(string Country = "")
        {


            int KiiPayBusinessInfoId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId;
            var RecenltyPaidInternationalBusinesses = _KiiPayBusinessCommonServices.GetRecentlyPaidInternationalKiiPayBusiness(KiiPayBusinessInfoId)
                                                                                    .Where(x => x.Country == Country).ToList();
            ViewBag.RecenltyPaidInternationalBusinesses = new SelectList(RecenltyPaidInternationalBusinesses, "MobileNo", "MobileNo");
        }
        #endregion

    }

}