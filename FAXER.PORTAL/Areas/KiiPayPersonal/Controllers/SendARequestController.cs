using FAXER.PORTAL.Areas.KiiPayPersonal.Services;
using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Controllers
{
    public class SendARequestController : Controller
    {
        SendARequestServices _service = null;
        KiiPayPersonalCommonServices _commonServices = null;
        public SendARequestController()
        {
            _service = new SendARequestServices();
            _commonServices = new KiiPayPersonalCommonServices();
        }
        // GET: KiiPayPersonal/SendARequest
        public ActionResult Index()
        {
            if (Common.CardUserSession.RequestAPaymentSession != null)
            {
                Common.CardUserSession.RequestAPaymentSession = null;
            }
            SetViewBagForPhoneNumbersLocal();
            var vm = new SendARequestViewModel()
            {
                CountryPhoneCode = Common.Common.GetCountryPhoneCode(Common.CardUserSession.LoggedCardUserViewModel.CountryCode)
            };
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = SendARequestViewModel.BindProperty)]SendARequestViewModel model)
        {
            SetViewBagForPhoneNumbersLocal();
            if (ModelState.IsValid)
            {
                bool isPhoneNumberValid = _service.IsPhoneValidForLocalPaymentRequest(model.MobileNumber);
                if (isPhoneNumberValid == false)
                {
                    ModelState.AddModelError("MobileNumber", "Invalid Mobile Number !");
                    return View(model);
                }
                if (Common.CardUserSession.RequestAPaymentSession != null)
                {
                    Common.CardUserSession.RequestAPaymentSession = null;
                }
                Common.CardUserSession.RequestAPaymentSession = new KiiPayRequestAPaymentSessionViewModel()
                {
                    ReceivingMobileNumber = model.MobileNumber
                };
                return RedirectToAction("SendRequestEnterAmount");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult SendRequestEnterAmount()
        {
            if (Common.CardUserSession.RequestAPaymentSession == null)
            {
                return RedirectToAction("Index");
            }
            var vm = _service.getRequestEnterAmountLocalVM();
            return View(vm);
        }
        [HttpPost]
        public ActionResult SendRequestEnterAmount([Bind(Include = SendRequestEnterAmountViewModel.BindProperty)]SendRequestEnterAmountViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Amount == 0)
                {
                    ModelState.AddModelError("Amount", "Invalid Amount !");
                    return View(model);
                }
                Common.CardUserSession.RequestAPaymentSession.PayingAmount = model.Amount;
                Common.CardUserSession.RequestAPaymentSession.ReceivingAmount = model.Amount;
                Common.CardUserSession.RequestAPaymentSession.SendingAmount = model.Amount;
                Common.CardUserSession.RequestAPaymentSession.ExchangeRate = 1;
                Common.CardUserSession.RequestAPaymentSession.Fee = 0;
                Common.CardUserSession.RequestAPaymentSession.Note = model.Note;
                Common.CardUserSession.RequestAPaymentSession.ReceiverName = _commonServices.getKiiPayUsersNameFromMobile(Common.CardUserSession.RequestAPaymentSession.ReceivingMobileNumber);


                bool requestPayment = _service.requestForPayment(DB.PaymentType.Local);
                if (requestPayment == true)
                {
                    return RedirectToAction("RequestSuccess");
                }

            }
            return View(model);
        }

        public ActionResult RequestSuccess()
        {
            if (Common.CardUserSession.RequestAPaymentSession == null)
            {
                return RedirectToAction("Index");
            }
            var vm = new RequestSuccessViewModel()
            {
                Amount = Common.CardUserSession.RequestAPaymentSession.ReceivingAmount,
                ReceiverName = Common.CardUserSession.RequestAPaymentSession.ReceiverName
            };
            Common.CardUserSession.RequestAPaymentSession = null;
            return View(vm);
        }

        [HttpGet]
        public ActionResult SendARequestAbroad()
        {
            if (Common.CardUserSession.RequestAPaymentSession != null)
            {
                Common.CardUserSession.RequestAPaymentSession = null;
            }
            SetViewBagForCountries();
            SetViewBagForPhoneNumbersInternational();
            var vm = new SendARequestAbroadViewModel()
            {
                CountryPhoneCode = ""
            };
            return View(vm);
        }

        [HttpPost]
        public ActionResult SendARequestAbroad([Bind(Include = SendARequestAbroadViewModel.BindProperty)]SendARequestAbroadViewModel model)
        {
            SetViewBagForCountries();
            SetViewBagForPhoneNumbersInternational();
            if (!string.IsNullOrEmpty(model.CountryCode))
            {
                model.CountryPhoneCode = _commonServices.getCountryPhoneCode(model.CountryCode);
            }
            if (ModelState.IsValid)
            {
                bool isPhoneValid = _service.IsPhoneValidForInternationalPaymentRequest(model.MobileNumber, model.CountryCode);
                if (isPhoneValid == false)
                {
                    ModelState.AddModelError("MobileNumber", "Invalid Mobile Number !");
                    return View(model);
                }
                if (Common.CardUserSession.RequestAPaymentSession != null)
                {
                    Common.CardUserSession.RequestAPaymentSession = null;
                }
                Common.CardUserSession.RequestAPaymentSession = new KiiPayRequestAPaymentSessionViewModel()
                {
                    ReceivingMobileNumber = model.MobileNumber
                };
                return RedirectToAction("SendRequesEnterAmountAbroad");
            }
            
            return View(model);
        }


        [HttpGet]
        public ActionResult SendRequesEnterAmountAbroad()
        {
            var vm = _service.getRequestEnterAmountInternationalVM();
            return View(vm);
        }

        [HttpPost]
        public ActionResult SendRequesEnterAmountAbroad([Bind(Include = SendRequesEnterAmountAbroadViewModel.BindProperty)]SendRequesEnterAmountAbroadViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.ForeignAmount == 0)
                {
                    ModelState.AddModelError("ForeignAmount", "Invalid Amount");
                    return View(model);
                }
                if (model.LocalAmount == 0)
                {
                    ModelState.AddModelError("LocalAmount", "Invalid Amount !");
                    return View(model);
                }
                Common.CardUserSession.RequestAPaymentSession.PayingAmount = model.ForeignAmount;
                Common.CardUserSession.RequestAPaymentSession.ReceivingAmount = model.LocalAmount;
                Common.CardUserSession.RequestAPaymentSession.SendingAmount = model.ForeignAmount;
                Common.CardUserSession.RequestAPaymentSession.ExchangeRate = model.ExchangeRate;
                Common.CardUserSession.RequestAPaymentSession.Fee = 0;
                Common.CardUserSession.RequestAPaymentSession.Note = model.Note;
                Common.CardUserSession.RequestAPaymentSession.ReceiverName = _commonServices.getKiiPayUsersNameFromMobile(Common.CardUserSession.RequestAPaymentSession.ReceivingMobileNumber);
                bool requestPayment = _service.requestForPayment(DB.PaymentType.International);
                
                if(requestPayment == true)
                {
                    return RedirectToAction("RequestSuccess");
                }

            }
            return View(model);
        }

        

        public JsonResult getCountryPhoneCode(string countryCode)
        {
            if (!string.IsNullOrEmpty(countryCode))
            {
                var phoneCode = _commonServices.getCountryPhoneCode(countryCode);
                return Json(new
                {
                    CountryPhoneCode = phoneCode
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                CountryPhoneCode = ""
            }, JsonRequestBehavior.AllowGet);
        }

        public void SetViewBagForPhoneNumbersLocal()
        {
            var phoneNumbers = _service.getRecentPhoneNumbersLocal();
            ViewBag.PhoneNumbers = new SelectList(phoneNumbers, "Code", "Name");
        }
        public void SetViewBagForPhoneNumbersInternational()
        {
            var phoneNumbers = _service.getRecentPhoneNumbersInternational();
            ViewBag.PhoneNumbers = new SelectList(phoneNumbers, "Code", "Name");
        }
        public void SetViewBagForCountries()
        {
            var countries = _commonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }
    }
}