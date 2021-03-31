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
    public class MobileMoneyTransferController : Controller
    {
        // GET: KiiPayPersonal/MobileMoneyTransfer
        MobileMoneyTransferServices _services = null;
        KiiPayPersonalCommonServices _commonServices = null;
        public MobileMoneyTransferController()
        {
            _services = new MobileMoneyTransferServices();
            _commonServices = new KiiPayPersonalCommonServices();
        }
        public ActionResult Index()
        {
            SetViewBagForRecentNumbersNational();
            return View();
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = MobileMoneyTransferViewModel.BindProperty)]MobileMoneyTransferViewModel model)
        {
            if (ModelState.IsValid)
            {

                if (Common.CardUserSession.PayIntoAnotherWalletSession != null)
                {
                    Common.CardUserSession.PayIntoAnotherWalletSession = null;
                }
                Common.CardUserSession.PayIntoAnotherWalletSession = new PayIntoAnotherWalletViewModel()
                {
                    ReceivingPhoneNumber = model.MobileNumber,
                    PaymentType = DB.PaymentType.Local
                };
                return RedirectToAction("SendMoneyToMobile");
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult SendingMoneyAbroadMobile()
        {
            var vm = new SendingMoneyAbroadMobileViewModel()
            {
                CountryPhoneCode = "",
            };

            SetViewBagForRecentNumbersInterNational();
            SetViewBagForCountries();
            return View(vm);
        }

        [HttpPost]
        public ActionResult SendingMoneyAbroadMobile([Bind(Include = SendingMoneyAbroadMobileViewModel.BindProperty)]SendingMoneyAbroadMobileViewModel model)
        {
            if (!string.IsNullOrEmpty(model.CountryCode))
            {
                model.CountryPhoneCode = Common.Common.GetCountryPhoneCode(model.CountryCode);
            }
            if (ModelState.IsValid)
            {
                if (Common.CardUserSession.PayIntoAnotherWalletSession != null)
                {
                    Common.CardUserSession.PayIntoAnotherWalletSession = null;
                }
                Common.CardUserSession.PayIntoAnotherWalletSession = new PayIntoAnotherWalletViewModel()
                {
                    ReceivingCountryCode = model.CountryCode,
                    ReceivingPhoneNumber = model.MobileNumber,
                    ExchangeRate = _commonServices.calculateExchangeRate(Common.CardUserSession.LoggedCardUserViewModel.CountryCode, model.CountryCode),
                    Fee = SEstimateFee.GetFaxingCommision(Common.CardUserSession.LoggedCardUserViewModel.CountryCode),
                    SMSFee = 0,
                    PaymentType = DB.PaymentType.International,
                    SendSMSNotification = false,
                    SendingCurrencyCode = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrency,
                    SendingCurrencySymbol = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol,
                    ReceivingCurrencyCode = Common.Common.GetCountryCurrency(model.CountryCode),
                    ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(model.CountryCode)
                };
                return RedirectToAction("EnterAmountAbroad");
            }
            SetViewBagForRecentNumbersInterNational();
            SetViewBagForCountries();
            return View(model);
        }

        [HttpGet]
        public ActionResult EnterAmountAbroad()
        {
            if (Common.CardUserSession.PayIntoAnotherWalletSession == null)
            {
                return RedirectToAction("Index");
            }
            var vm = new KiiPayEnterAmountTwoViewModel()
            {
                SendingCurrencySymbol = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol,
                SendingCurrencyCode = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrency,
                ReceivingCurrencyCode = Common.Common.GetCountryCurrency(Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingCountryCode),
                ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingCountryCode),
                ReceiverName = "",
                PhotoUrl = "",
                ExchangeRate = Common.CardUserSession.PayIntoAnotherWalletSession.ExchangeRate,
                Fee = Common.CardUserSession.PayIntoAnotherWalletSession.Fee
            };
            return View(vm);
        }

        [HttpPost]
        public ActionResult EnterAmountAbroad([Bind(Include = KiiPayEnterAmountTwoViewModel.BindProperty)]KiiPayEnterAmountTwoViewModel model)
        {
            if (model.SendingAmount == 0)
            {
                ModelState.AddModelError("SendingAmount", "Invalid Value");
                return View(model);
            }
            if (model.ReceivingAmount == 0)
            {
                ModelState.AddModelError("ReceivingAmount", "Invalid Value");
                return View(model);
            }
            if (model.SendingAmount > Common.CardUserSession.LoggedCardUserViewModel.BalanceOnCard)
            {
                ModelState.AddModelError("SendingAmount", "Invalid Value");
                return View(model);
            }
            Common.CardUserSession.PayIntoAnotherWalletSession.Amount = model.SendingAmount;
            Common.CardUserSession.PayIntoAnotherWalletSession.PayingAmount = model.PayingAmount;
            Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingAmount = model.ReceivingAmount;
            
            return RedirectToAction("PaymentSummary");


        }

        [HttpGet]
        public ActionResult SendMoneyToMobile()
        {
            var vm = new SendMoneyToMobileViewModel()
            {
                PhotoUrl = "",
                ReceiverName = "",
                CurrencySymbol = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol,
                CurrencyCode = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrency
            };
            return View(vm);
        }

        [HttpPost]
        public ActionResult SendMoneyToMobile([Bind(Include = SendMoneyToMobileViewModel.BindProperty)]SendMoneyToMobileViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Amount == 0)
                {
                    ModelState.AddModelError("Amount", "Invalid Amount !");
                    return View(model);
                }
                if (model.Amount > Common.CardUserSession.LoggedCardUserViewModel.BalanceOnCard)
                {
                    ModelState.AddModelError("Amount", "Invalid Amount !");
                    return View(model);
                }
                if (Common.CardUserSession.PayIntoAnotherWalletSession == null)
                {
                    return RedirectToAction("Index");
                }
                Common.CardUserSession.PayIntoAnotherWalletSession.Amount = model.Amount;
                Common.CardUserSession.PayIntoAnotherWalletSession.PayingAmount = model.Amount;
                Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingAmount = model.Amount;
                Common.CardUserSession.PayIntoAnotherWalletSession.SendSMSNotification = model.SendSMSNotification;
                Common.CardUserSession.PayIntoAnotherWalletSession.ReceiversName = model.ReceiverName;


                return RedirectToAction("PaymentSummary");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult PaymentSummary()
        {
            if (Common.CardUserSession.PayIntoAnotherWalletSession == null)
            {
                return RedirectToAction("Index");
            }
            var vm = new MobileTransferPaymentSummaryViewModel()
            {
                ReceivingCurrencyCode = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrency,
                ReceivingCurrnecySymbol = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol,
                SendingCurrencyCode = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrency,
                SendingCurrencySymbol = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol,
                Amount = Common.CardUserSession.PayIntoAnotherWalletSession.Amount,
                Fee = 0,
                LocalSmsFee = 0,
                TotalAmount = Common.CardUserSession.PayIntoAnotherWalletSession.PayingAmount,
                ReceivingAmount = Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingAmount,
                PaymentReference = "",
                ReceiversName = Common.CardUserSession.PayIntoAnotherWalletSession.ReceiversName
            };
            return View(vm);
        }

        [HttpPost]
        public ActionResult PaymentSummary([Bind(Include = MobileTransferPaymentSummaryViewModel.BindProperty)]MobileTransferPaymentSummaryViewModel model)
        {
            if (Common.CardUserSession.PayIntoAnotherWalletSession.PaymentType == DB.PaymentType.Local)
            {
                bool saveMobileTransfer = _services.saveMobileTransferNational();
                if (saveMobileTransfer == true)
                {
                    string receiverName = Common.CardUserSession.PayIntoAnotherWalletSession.ReceiversName;
                    string amount = Common.CardUserSession.PayIntoAnotherWalletSession.Amount.ToString();
                    Common.CardUserSession.PayIntoAnotherWalletSession = null;
                    return RedirectToAction("PaymentSuccess", new { @receiverName = receiverName, @amount = amount });
                }

            }
            else {
                bool saveMobileTransferInternational = _services.saveMobileTransferInternational();
                if(saveMobileTransferInternational == true)
                {
                    string receiverName = Common.CardUserSession.PayIntoAnotherWalletSession.ReceiversName;
                    string amount = Common.CardUserSession.PayIntoAnotherWalletSession.Amount.ToString();
                    Common.CardUserSession.PayIntoAnotherWalletSession = null;
                    return RedirectToAction("PaymentSuccess", new { @receiverName = receiverName, @amount = amount });

                }
            }
            return View(model);
        }

        public ActionResult PaymentSuccess(string receiverName, string amount)
        {
            if (string.IsNullOrEmpty(receiverName) || string.IsNullOrEmpty(amount))
            {
                return RedirectToAction("Index");
            }
            ViewBag.Amount = amount;
            ViewBag.ReceiverName = receiverName;
            return View();
        }


        private void SetViewBagForRecentNumbersNational()
        {
            var mobileNumbers = _services.getRecentMobileNumbersNational();
            ViewBag.RecentMobileNumbers = new SelectList(mobileNumbers, "Code", "Name");
        }

        private void SetViewBagForRecentNumbersInterNational()
        {
            var mobileNumbers = _services.getRecentMobileNumbersInterNational();
            ViewBag.RecentMobileNumbers = new SelectList(mobileNumbers, "Code", "Name");
        }

        private void SetViewBagForCountries()
        {
            var countries = _commonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }
        public JsonResult GetCountryPhoneCode(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                var phoneCode = Common.Common.GetCountryPhoneCode(code);
                if (!string.IsNullOrEmpty(phoneCode))
                {
                    return Json(new
                    {
                        code = phoneCode
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new
            {
                code = ""
            }, JsonRequestBehavior.AllowGet);
        }
    }
}