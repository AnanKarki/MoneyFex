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
    public class KiiPayBusinessPayAnotherKiiPayWalletController : Controller
    {

        KiiPayBusinessPayAnotherKiiPayWalletServices _kiiPayBusinessPayAnotherKiiPayWalletServices = null;
        KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = null;
        public KiiPayBusinessPayAnotherKiiPayWalletController()
        {
            _kiiPayBusinessPayAnotherKiiPayWalletServices = new KiiPayBusinessPayAnotherKiiPayWalletServices();
            _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();
        }
        // GET: KiiPayBusiness/KiiPayBusinessPayAnotherKiiPayWallet
        public ActionResult Index()
        {
            return View();
        }

        #region Local Payment 


        [HttpGet]
        public ActionResult SearchKiiPayPersonalWalletForLocalPayment()
        {


            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();
            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;
            GetRecenltyPaidLocalKiiPayPersonalWallet();
            return View();
        }

        [HttpPost]
        public ActionResult SearchKiiPayPersonalWalletForLocalPayment([Bind(Include = SearchKiiPayPersonalWalletForLocalPaymentVM.BindProperty)]SearchKiiPayPersonalWalletForLocalPaymentVM vm)
        {
            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();
            GetRecenltyPaidLocalKiiPayPersonalWallet();
            if (ModelState.IsValid) {

                bool IsValidMobileNo = _kiiPayBusinessPayAnotherKiiPayWalletServices.IsValidMobileNo(vm.MobileNo);
                var IsValidCardStatus = _kiiPayBusinessCommonServices.ValidCardStatus(_kiiPayBusinessPayAnotherKiiPayWalletServices.GetKiiPayPersonalWalletCardStatus(vm.MobileNo));
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
                else if (!_kiiPayBusinessPayAnotherKiiPayWalletServices.IsValidTransfer(vm.MobileNo, true))
                {
                    kiiPayBusinessResult.Message = "Local transaction cannot be performed to the  mobile no you have entered . Please choose international payment";
                    kiiPayBusinessResult.Status = ResultStatus.Warning;
                }
                else {

                    return RedirectToAction("EnterPayingAmountForLocalPayment");
                }

            }

            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;
            return View(vm);
        }

        [HttpGet]
        public ActionResult EnterPayingAmountForLocalPayment() {


            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();
            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;

            var vm = _kiiPayBusinessPayAnotherKiiPayWalletServices.GetPayingAmountForLocalPaymentInfo();
            return View(vm);
        }

        [HttpPost]
        public ActionResult EnterPayingAmountForLocalPayment([Bind(Include = KiiPayBusinessLocalPaymentPayingAmountDetailsVM.BindProperty)]KiiPayBusinessLocalPaymentPayingAmountDetailsVM vm) {

            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();

            if (ModelState.IsValid) {

                var HasEnoughBal = _kiiPayBusinessCommonServices.DoesAccountHaveEnoughBal(vm.SendingAmount);

                if (HasEnoughBal == false)
                {

                    kiiPayBusinessResult.Message = "You don't have enough balance in you wallet..";
                    kiiPayBusinessResult.Status = ResultStatus.Warning;

                }
                else
                {
                    _kiiPayBusinessPayAnotherKiiPayWalletServices.SetPayingAmountForLocalPaymentInfo(vm);

                    return RedirectToAction("PaymentsummaryForLocalPayment");
                }
            }

            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;
            return View(vm);
        }

        [HttpGet]
        public ActionResult PaymentsummaryForLocalPayment() {

            var vm = _kiiPayBusinessPayAnotherKiiPayWalletServices.GetKiiPersonalPaymentSummary();
            return View(vm);
        }


        [HttpPost]
        public ActionResult CompleteLocalPaymentTransaction()
        {
            var vm = _kiiPayBusinessPayAnotherKiiPayWalletServices.GetLocalPaymentCompletedSummary();
            return View(vm);
        }
        #endregion

        #region International Payment 

        [HttpGet]
        public ActionResult SearchKiiPayPersonalWalletForInternatinalPayment(string Country = "") {

            GetCountryDropDown(Country);
            GetRecenltyPaidInternationalKiiPayPersonalWallet(Country);
            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();
            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;

            return View();
        }

        [HttpPost]
        public ActionResult SearchKiiPayPersonalWalletForInternatinalPayment([Bind(Include = SearchKiiPayPersonalWalletForInternationalPaymentVM.BindProperty)]SearchKiiPayPersonalWalletForInternationalPaymentVM vm)
        {
            GetCountryDropDown(vm.Country);
            GetRecenltyPaidInternationalKiiPayPersonalWallet(vm.Country);
            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();

            if (ModelState.IsValid)
            {

                bool IsValidMobileNo = _kiiPayBusinessPayAnotherKiiPayWalletServices.IsValidMobileNo(vm.MobileNo);
                var IsValidCardStatus = _kiiPayBusinessCommonServices.ValidCardStatus(_kiiPayBusinessPayAnotherKiiPayWalletServices.GetKiiPayPersonalWalletCardStatus(vm.MobileNo));
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
                else if (!_kiiPayBusinessPayAnotherKiiPayWalletServices.IsValidTransfer(vm.MobileNo, false))
                {
                    kiiPayBusinessResult.Message = "Local transaction cannot be performed to the  mobile no you have entered . Please choose international payment";
                    kiiPayBusinessResult.Status = ResultStatus.Warning;
                }
                else
                {

                    return RedirectToAction("EnterPayingAmountForInternationalPayment");
                }

            }

            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;
            return View(vm);
        }

        [HttpGet]
        public ActionResult EnterPayingAmountForInternationalPayment() {

            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();
            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;

            var vm = _kiiPayBusinessPayAnotherKiiPayWalletServices.GetKiiPayBusinessInternationalPaymentPayingAmountDetails();
            return View(vm);
        }
        [HttpPost]
        public ActionResult EnterPayingAmountForInternationalPayment([Bind(Include = KiiPayBusinessInternationalPaymentPayingAmountDetailsVM.BindProperty)]KiiPayBusinessInternationalPaymentPayingAmountDetailsVM vm)
        {
            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();

            if (ModelState.IsValid) {
                var HasEnoughBal = _kiiPayBusinessCommonServices.DoesAccountHaveEnoughBal(vm.SendingAmount);

                if (HasEnoughBal == false)
                {

                    kiiPayBusinessResult.Message = "You don't have enough balance in you wallet..";
                    kiiPayBusinessResult.Status = ResultStatus.Warning;

                }
                else
                {
                    _kiiPayBusinessPayAnotherKiiPayWalletServices.SetKiiPayBusinessInternationalPaymentPayingAmountDetails(vm);
                    return RedirectToAction("PaymentsummaryForInternationalPayment");
                }
            }
            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;

            vm = _kiiPayBusinessPayAnotherKiiPayWalletServices.GetKiiPayBusinessInternationalPaymentPayingAmountDetails();
            return View(vm);
        }
        [HttpGet]
        public ActionResult PaymentsummaryForInternationalPayment()
        {

            var vm = _kiiPayBusinessPayAnotherKiiPayWalletServices.GetKiiPersonalPaymentSummary();
            return View(vm);
        }
        [HttpPost]
        public ActionResult CompleteInternationalPaymentTransaction()
        {
            var vm = _kiiPayBusinessPayAnotherKiiPayWalletServices.GetInternationalPaymentCompletedSummary();
            return View(vm);
        }

        public JsonResult GetPaymentSummary(decimal SendingAmount, decimal ReceivingAmount)
        {
            bool IsReceivingAmount = false;
            var InternationalTransferAmountSummary = Common.BusinessSession.KiiPayBusinessPaymentSummary;
            if ((SendingAmount > 0 && ReceivingAmount > 0) && InternationalTransferAmountSummary.ReceivingAmount != ReceivingAmount)
            {

                SendingAmount = ReceivingAmount;
                IsReceivingAmount = true;
            }

            if (SendingAmount == 0)
            {

                IsReceivingAmount = true;
                SendingAmount = ReceivingAmount;
            }
            var result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
                InternationalTransferAmountSummary.ExchangeRate, SEstimateFee.GetFaxingCommision(Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode));

            // Rewrite session with additional value 
            InternationalTransferAmountSummary.Fee = result.FaxingFee;
            InternationalTransferAmountSummary.SendingAmount = result.FaxingAmount;
            InternationalTransferAmountSummary.ReceivingAmount = result.ReceivingAmount;
            InternationalTransferAmountSummary.TotalAmount = result.TotalAmount;
            _kiiPayBusinessPayAnotherKiiPayWalletServices.SetKiiPersonalPaymentSummary(InternationalTransferAmountSummary);

            return Json(new
            {
                Fee = result.FaxingFee,
                TotalAmount = result.TotalAmount,
                ReceivingAmount = result.ReceivingAmount,
                SendingAmount = result.FaxingAmount
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion
        private void GetCountryDropDown(string Country = "")
        {
            ViewBag.Countries = new SelectList(Common.Common.GetCountriesForDropDown()
               , "CountryCode", "CountryName" , Country);
        }



        public void GetRecenltyPaidLocalKiiPayPersonalWallet() {


            int KiiPayBusinessInfoId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId;
            var RecenltyPaidMobileNoList = _kiiPayBusinessCommonServices.GetAllRecenltyPaidKiiPayPersonalWalletInfo(KiiPayBusinessInfoId).Where(x => x.ReceiverIsLocal == true).ToList();

            ViewBag.RecenltyPaidMobileNo = new SelectList(RecenltyPaidMobileNoList, "MobileNo", "MobileNo");

        }

        public void GetRecenltyPaidInternationalKiiPayPersonalWallet( string Country= "")
        {

            int KiiPayBusinessInfoId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId;
            var RecenltyPaidMobileNoList = _kiiPayBusinessCommonServices.GetAllRecenltyPaidKiiPayPersonalWalletInfo(KiiPayBusinessInfoId).
                                                                                   Where(x => x.ReceiverIsLocal == false && x.Country == Country).ToList();

            ViewBag.RecenltyPaidMobileNo = new SelectList(RecenltyPaidMobileNoList, "MobileNo", "MobileNo");

        }
    }
}