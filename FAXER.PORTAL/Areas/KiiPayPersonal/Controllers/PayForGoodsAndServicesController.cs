using FAXER.PORTAL.Areas.KiiPayPersonal.Services;
using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Controllers
{
    public class PayForGoodsAndServicesController : Controller
    {
        PayForGoodsAndServicesServices _services = null;
        KiiPayPersonalCommonServices _commonServices = null;
        public PayForGoodsAndServicesController()
        {
            _services = new PayForGoodsAndServicesServices();
            _commonServices = new KiiPayPersonalCommonServices();
        }
        // GET: KiiPayPersonal/PayForGoodsAndServices
        public ActionResult Index()
        {
            if (Common.CardUserSession.PayIntoAnotherWalletSession != null)
            {
                Common.CardUserSession.PayIntoAnotherWalletSession = null;
            }
            SetviewbagForBusiness();
            return View();
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = PayForGoodsAndServicesViewModel.BindProperty)]PayForGoodsAndServicesViewModel model)
        {
            SetviewbagForBusiness();
            if (ModelState.IsValid)
            {
                bool isPhoneValid = _services.isPhoneNumberValid(model.BusinessMobileNumber);
                if (isPhoneValid == false)
                {
                    ModelState.AddModelError("BusinessMobileNumber", "Invalid Mobile Number !");
                    return View(model);
                }
                if (Common.CardUserSession.PayIntoAnotherWalletSession != null)
                {
                    Common.CardUserSession.PayIntoAnotherWalletSession = null;
                }
                Common.CardUserSession.PayIntoAnotherWalletSession = new PayIntoAnotherWalletViewModel()
                {
                    SendingPhoneNumber = Common.CardUserSession.LoggedCardUserViewModel.MobileNumber,
                    ReceivingPhoneNumber = model.BusinessMobileNumber,
                    ReceiversName = _services.getBusinessReceiverName(model.BusinessMobileNumber),
                    PaymentType = DB.PaymentType.Local
                };
                return RedirectToAction("PayForServicesEnterAmount");

            }
            return View(model);
        }
        [HttpGet]
        public ActionResult PayForServicesAbroad()
        {
            if (Common.CardUserSession.PayIntoAnotherWalletSession != null)
            {
                Common.CardUserSession.PayIntoAnotherWalletSession = null;
            }
            SetViewBagForCountries();
            SetViewBagForBusinessInternational();
            return View();
        }

        [HttpPost]
        public ActionResult PayForServicesAbroad([Bind(Include = PayForServicesAbroadViewModel.BindProperty)]PayForServicesAbroadViewModel model)
        {
            SetViewBagForCountries();
            SetViewBagForBusinessInternational();

            if (ModelState.IsValid)
            {

                if (model.CountryCode.Trim() == Common.CardUserSession.LoggedCardUserViewModel.CountryCode.Trim())
                {
                    ModelState.AddModelError("CountryCode", "Invalid Country");
                    return View(model);
                }
                bool isPhoneValid = _services.isInternationalPhoneValid(model.MobileNumber, model.CountryCode);
                if (isPhoneValid == false)
                {
                    ModelState.AddModelError("MobileNumber", "Invalid Mobile Number !");
                    return View(model);
                }
                if (Common.CardUserSession.PayIntoAnotherWalletSession != null)
                {
                    Common.CardUserSession.PayIntoAnotherWalletSession = null;
                }
                Common.CardUserSession.PayIntoAnotherWalletSession = new PayIntoAnotherWalletViewModel()
                {
                    SendingPhoneNumber = Common.CardUserSession.LoggedCardUserViewModel.MobileNumber,
                    ReceivingPhoneNumber = model.MobileNumber,
                    PaymentType = DB.PaymentType.International,
                    ReceiversName = _commonServices.getKiiPayBusinessName(model.MobileNumber),
                    SendingCurrencyCode = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrency,
                    SendingCurrencySymbol = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol,
                    ReceivingCurrencyCode = Common.Common.GetCountryCurrency(model.CountryCode),
                    ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(model.CountryCode),
                    ExchangeRate = _commonServices.calculateExchangeRate(Common.CardUserSession.LoggedCardUserViewModel.CountryCode, model.CountryCode),
                    SendSMSNotification = false,
                    SMSFee = 0
                };
                return RedirectToAction("PayForServicesEnterAmountAbroad");

            }
            return View(model);
        }
        [HttpGet]
        public ActionResult PayForServicesEnterAmount()
        {
            var vm = _services.getPayForServicesEnterAmountViewModel();
            return View(vm);
        }

        [HttpPost]
        public ActionResult PayForServicesEnterAmount([Bind(Include = PayForServicesEnterAmountViewModel.BindProperty)]PayForServicesEnterAmountViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Amount > Common.CardUserSession.LoggedCardUserViewModel.BalanceOnCard)
                {
                    ModelState.AddModelError("Amount", "Invalid Amount !");
                    return View(model);
                }
                Common.CardUserSession.PayIntoAnotherWalletSession.PayingAmount = model.Amount;
                Common.CardUserSession.PayIntoAnotherWalletSession.PaymentReference = model.PaymentReference;
                Common.CardUserSession.PayIntoAnotherWalletSession.SendSMSNotification = model.SendSMSNotification;
                return RedirectToAction("PayForServicesSummary");
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult PayForServicesEnterAmountAbroad()
        {
            var vm = _services.getEnterAmountAbroadViewModel();
            return View(vm);
        }

        [HttpPost]
        public ActionResult PayForServicesEnterAmountAbroad([Bind(Include = PayForServicesEnterAmountAbroadViewModel.BindProperty)]PayForServicesEnterAmountAbroadViewModel model)
        {
            if(ModelState.IsValid)
            {
                if(model.SendingAmount == 0)
                {
                    ModelState.AddModelError("SendingAmount", "Invalid Amount !");
                    return View(model);
                }
                if (model.ReceivingAmount == 0)
                {
                    ModelState.AddModelError("ReceivingAmount", "Invalid Amount !");
                    return View(model);
                }
                if (model.PayingAmount > Common.CardUserSession.LoggedCardUserViewModel.BalanceOnCard)
                {
                    ModelState.AddModelError("SendingAmount", "Invalid Amount !");
                    return View(model);
                }
                Common.CardUserSession.PayIntoAnotherWalletSession.Amount = model.SendingAmount;
                Common.CardUserSession.PayIntoAnotherWalletSession.PayingAmount = model.PayingAmount;
                Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingAmount = model.ReceivingAmount;
                Common.CardUserSession.PayIntoAnotherWalletSession.Fee = model.Fee;
                Common.CardUserSession.PayIntoAnotherWalletSession.PaymentReference = model.PaymentReference;
                return RedirectToAction("PayForServicesSummaryAbroad");
            }
            return View(model);
                 
        }

        [HttpGet]
        public ActionResult PayForServicesSummaryAbroad()
        {
            var vm = _services.getAbroadPaymentSummaryViewModel();
            return View(vm);
        }

        [HttpPost]
        public ActionResult PayForServicesSummaryAbroad([Bind(Include = PayForServicesSummaryAbroadViewModel.BindProperty)]PayForServicesSummaryAbroadViewModel model)
        {
            bool sendMoney = _commonServices.sendMoneyKiiPayPersonalToKiiPayBusiness(DB.PaymentType.International);
            if(sendMoney == true)
            {
                return RedirectToAction("PayForServicePaymentSuccess");
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult PayForServicesSummary()
        {
            if (Common.CardUserSession.PayIntoAnotherWalletSession == null)
            {
                return RedirectToAction("Index");
            }
            var vm = _commonServices.CalculateKiiPayPersonalToKiiPayBusinessPaymentSummary(true, false);
            Common.CardUserSession.PayIntoAnotherWalletSession.PayingAmount = vm.PayingAmout;
            Common.CardUserSession.PayIntoAnotherWalletSession.Amount = vm.Amount;
            Common.CardUserSession.PayIntoAnotherWalletSession.Fee = vm.Fee;
            Common.CardUserSession.PayIntoAnotherWalletSession.SMSFee = vm.LocalSMSMessage;
            Common.CardUserSession.PayIntoAnotherWalletSession.ExchangeRate = vm.ExchangeRate;
            Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingAmount = vm.ReceivingAmount;
            Common.CardUserSession.PayIntoAnotherWalletSession.SendingCurrencySymbol = vm.SendingCurrencySymbol;
            Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingCurrencySymbol = vm.ReceivingCurrencySymbol;
            Common.CardUserSession.PayIntoAnotherWalletSession.SendingCurrencyCode = vm.SendingCurrencyCode;
            Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingCurrencyCode = vm.ReceivingCurrencyCode;
            return View(vm);
        }

        [HttpPost]
        public ActionResult PayForServicesSummary([Bind(Include = PayForServicesSummaryViewModel.BindProperty)]PayForServicesSummaryViewModel model)
        {
            if (Common.CardUserSession.PayIntoAnotherWalletSession == null)
            {
                return RedirectToAction("Index");
            }
            bool sendMoney = _commonServices.sendMoneyKiiPayPersonalToKiiPayBusiness(DB.PaymentType.Local);
            if (sendMoney == true)
            {
                return RedirectToAction("PayForServicePaymentSuccess");
            }

            return View(model);
        }

        public ActionResult PayForServicePaymentSuccess()
        {
            if (Common.CardUserSession.PayIntoAnotherWalletSession == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        

        public JsonResult calculatePaymentSummary(decimal amount, string type)
        {
            if (amount != 0)
            {
                bool isAmountToBeReceived = false;
                if (type == "receiving")
                {
                    isAmountToBeReceived = true;
                }
                var result = SEstimateFee.CalculateFaxingFee(amount, true, isAmountToBeReceived, Common.CardUserSession.PayIntoAnotherWalletSession.ExchangeRate, SEstimateFee.GetFaxingCommision(Common.CardUserSession.LoggedCardUserViewModel.CountryCode));
                return Json(new
                {
                    SendingAmount = result.FaxingAmount,
                    ReceivingAmount = result.ReceivingAmount,
                    Fee = result.FaxingFee,
                    PayingAmount = result.TotalAmount
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
            }, JsonRequestBehavior.AllowGet);
        }




        private void SetViewBagForCountries()
        {
            var countries = _commonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }
        public void SetviewbagForBusiness()
        {
            var businessPhoneNumbers = _services.GetRecentlyPaidBusinessPhoneNumbers();
            ViewBag.PhoneNumbers = new SelectList(businessPhoneNumbers, "Code", "Name");
        }
        public void SetViewBagForBusinessInternational()
        {
            var businessInternationalPhoneNumbers = _services.GetRecentPaidInternationalBusinessPhoneNumbers();
            ViewBag.PhoneNumbers = new SelectList(businessInternationalPhoneNumbers, "Code", "Name");
        }
    }
}