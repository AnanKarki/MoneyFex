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
    public class KiiPayBusinessTopUpAnAccountController : Controller
    {
        KiiPayBusinessTopUpAnAccountServices _kiiPayBusinessTopUpAnAccountServices = null;
        public KiiPayBusinessTopUpAnAccountController()
        {
            _kiiPayBusinessTopUpAnAccountServices = new KiiPayBusinessTopUpAnAccountServices();
        }

        // GET: KiiPayBusiness/KiiPayBusinessTopUpAnAccount
        public ActionResult Index()
        {
            return View();
        }

        #region LocalTopUpAnAccount
        [HttpGet]
        public ActionResult LocalTopUpAnAccount()
        {
            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();
            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;
            GetLocalRecentSuppliersDropDown();
            var vm = _kiiPayBusinessTopUpAnAccountServices.GetKiiPayBusinessSearchSuppliers();
            return View(vm);
        }


        [HttpPost]
        public ActionResult LocalTopUpAnAccount([Bind(Include = KiiPayBusinessSearchSuppliersVM.BindProperty)]KiiPayBusinessSearchSuppliersVM vm)
        {
            GetLocalRecentSuppliersDropDown();
            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();

            if (ModelState.IsValid)
            {


                bool IsValidWalletNo = _kiiPayBusinessTopUpAnAccountServices.IsValidWalletNo(vm.WalletNo);
                if (!IsValidWalletNo)
                {

                    kiiPayBusinessResult.Message = "Please enter the valid Wallet no..";
                    kiiPayBusinessResult.Status = ResultStatus.Warning;

                }
                else if (!_kiiPayBusinessTopUpAnAccountServices.IsValidTransfer(vm.WalletNo, true))
                {


                    kiiPayBusinessResult.Message = "Local transaction cannot be performed to the  wallet no you have entered . Please choose international payment";
                    kiiPayBusinessResult.Status = ResultStatus.Warning;
                }
                else
                {
                    _kiiPayBusinessTopUpAnAccountServices.SetKiiPayBusinessSearchSuppliers(vm);
                    return RedirectToAction("LocalTopUpEnterAccountNo");
                }
            }
            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;
            return View(vm);

        }

        [HttpGet]
        public ActionResult LocalTopUpEnterAccountNo()
        {

            var vm = _kiiPayBusinessTopUpAnAccountServices.GetKiiPayBusinessEnterAccountNo();
            return View(vm);

        }
        [HttpPost]
        public ActionResult LocalTopUpEnterAccountNo([Bind(Include = KiiPayBusinessLocalTopUpEnterAccountNoVM.BindProperty)]KiiPayBusinessLocalTopUpEnterAccountNoVM vm)
        {
            vm.AccountNo = "123456789";
           // if (ModelState.IsValid)
            //{
                _kiiPayBusinessTopUpAnAccountServices.SetKiiPayBusinessEnterAccountNo(vm);
                return RedirectToAction("LocalTopUpEnterAmount");
           // }

            //return View(vm);

        }
        [HttpGet]
        public ActionResult LocalTopUpEnterAmount()
        {
            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();
            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;
            KiiPayBusinessLocalTopUpEnterAmountVM vm = new KiiPayBusinessLocalTopUpEnterAmountVM();
            vm.AccountNo = "";
            vm.CurrencyCode = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode;
            vm.CurrencySymbol = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CurrencySymbol;
            return View(vm);

        }

        
        [HttpPost]
        public ActionResult LocalTopUpEnterAmount([Bind(Include = KiiPayBusinessLocalTopUpEnterAmountVM.BindProperty)]KiiPayBusinessLocalTopUpEnterAmountVM vm)
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
                    KiiPayBusinessLocalTopUpSuccessVM SessionUpdate = new KiiPayBusinessLocalTopUpSuccessVM()
                    {
                        AccountNo = Common.BusinessSession.KiiPayBusinessTopUpEnterAccountNo.AccountNo,
                        Amount = vm.Amount
                    };
                    _kiiPayBusinessTopUpAnAccountServices.SetKiiPayBusinessTopUpSuccess(SessionUpdate);
                    _kiiPayBusinessTopUpAnAccountServices.CompleteLocalTopup(vm);
                    return RedirectToAction("LocalTopUpSuccess");
                }

            }

            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;
            return View(vm);

        }

        [HttpGet]
        public ActionResult LocalTopUpSuccess([Bind(Include = KiiPayBusinessLocalTopUpSuccessVM.BindProperty)]KiiPayBusinessLocalTopUpSuccessVM vm)
        {
            var result = _kiiPayBusinessTopUpAnAccountServices.GetKiiPayBusinessTopUpSuccess();
            return View(result);
        }

        public void GetLocalRecentSuppliersDropDown()
        {

            ViewBag.Suppliers = new SelectList(_kiiPayBusinessTopUpAnAccountServices.GetLocalSuppliers()
                , "WalletNo", "SuppliersName");
        }
        #endregion

        #region InternationalTopUpAnAccount
        [HttpGet]
        public ActionResult InternationalTopUpAnAccount()
        {
            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();
            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;
            GetInternationalRecentSuppliersDropDown();
            GetCountryDropDown();
            var vm = _kiiPayBusinessTopUpAnAccountServices.GetKiiPayBusinessSearchSuppliers();
            return View();
        }
        [HttpPost]
        public ActionResult InternationalTopUpAnAccount([Bind(Include = KiiPayBusinessSearchCountryVM.BindProperty)]KiiPayBusinessSearchCountryVM vm)
        {
            GetCountryDropDown();
            GetInternationalRecentSuppliersDropDown();
            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();

            if (ModelState.IsValid)
            {
                bool IsValidWalletNo = _kiiPayBusinessTopUpAnAccountServices.IsValidWalletNo(vm.WalletNo);
                if (!IsValidWalletNo)
                {

                    kiiPayBusinessResult.Message = "Please enter the valid Wallet no..";
                    kiiPayBusinessResult.Status = ResultStatus.Warning;

                }
                else if (!_kiiPayBusinessTopUpAnAccountServices.IsValidTransfer(vm.WalletNo, true))
                {


                    kiiPayBusinessResult.Message = "Local transaction cannot be performed to the  wallet no you have entered . Please choose international payment";
                    kiiPayBusinessResult.Status = ResultStatus.Warning;
                }
                else
                {
                    _kiiPayBusinessTopUpAnAccountServices.SetKiiPayBusinessSearchSuppliers(vm);
                    return RedirectToAction("InternationalTopUpEnterAccountNo");
                }
            }
            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;
            return View(vm);

        }

        [HttpGet]
        public ActionResult InternationalTopUpEnterAccountNo()
        {

            var result = _kiiPayBusinessTopUpAnAccountServices.GetKiiPayBusinessEnterAccountNo();
            KiiPayBusinessInternationalTopUpEnterAccountNoVM vm = new KiiPayBusinessInternationalTopUpEnterAccountNoVM();
            vm.AccountNo = result.AccountNo;
            return View(vm);

        }
        [HttpPost]
        public ActionResult InternationalTopUpEnterAccountNo([Bind(Include = KiiPayBusinessInternationalTopUpEnterAccountNoVM.BindProperty)]KiiPayBusinessInternationalTopUpEnterAccountNoVM vm)
        {
            vm.AccountNo = "123456789";
            //if (ModelState.IsValid)
            //{
            KiiPayBusinessLocalTopUpEnterAccountNoVM SessionUpdate = new KiiPayBusinessLocalTopUpEnterAccountNoVM()
            {
                AccountNo = vm.AccountNo
            };
            _kiiPayBusinessTopUpAnAccountServices.SetKiiPayBusinessEnterAccountNo(SessionUpdate);
            return RedirectToAction("InternationalTopUpEnterAmount");
            //}

            //return View(vm);

        }

        [HttpGet]
        public ActionResult InternationalTopUpEnterAmount()
        {
            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();
            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;
            KiiPayBusinessInternationalTopUpEnterAmountVM vm = new KiiPayBusinessInternationalTopUpEnterAmountVM();
            vm.AccountNo = "";
            vm.SendingCurrencySymbol = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CurrencySymbol;
            vm.SendingCurrencyCode = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode;
            return View(vm);

        }



        [HttpPost]
        public ActionResult InternationalTopUpEnterAmount([Bind(Include = KiiPayBusinessInternationalTopUpEnterAmountVM.BindProperty)]KiiPayBusinessInternationalTopUpEnterAmountVM vm)
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
                    KiiPayBusinessLocalTopUpSuccessVM SessionUpdate = new KiiPayBusinessLocalTopUpSuccessVM()
                    {
                        AccountNo = Common.BusinessSession.KiiPayBusinessTopUpEnterAccountNo.AccountNo,
                        Amount = vm.SendingAmount
                    };
                    _kiiPayBusinessTopUpAnAccountServices.SetKiiPayBusinessTopUpSuccess(SessionUpdate);
                    _kiiPayBusinessTopUpAnAccountServices.CompleteInternationalTopup(vm);
                    return RedirectToAction("InternationalTopUpSuccess");
                }

            }

            ViewBag.kiiPayBusinessResult = kiiPayBusinessResult;
            return View(vm);

        }

        [HttpGet]
        public ActionResult InternationalTopUpSuccess([Bind(Include = KiiPayBusinessInternationalTopUpSuccessVM.BindProperty)]KiiPayBusinessInternationalTopUpSuccessVM vm)
        {
            var result = _kiiPayBusinessTopUpAnAccountServices.GetKiiPayBusinessTopUpSuccess();
            vm.AccountNo = result.AccountNo;
            vm.Amount = result.Amount;
            return View(vm);
        }
    
        public void GetInternationalRecentSuppliersDropDown()
        {

            ViewBag.Suppliers = new SelectList(_kiiPayBusinessTopUpAnAccountServices.GetInternationalSuppliers()
                , "WalletNo", "SuppliersName");
        }

        private void GetCountryDropDown()
        {
            ViewBag.Countries = new SelectList(Common.Common.GetCountriesForDropDown()
               , "CountryCode", "CountryName");
        }
        #endregion

    }
}