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
    public class VirtualAccountDepositController : Controller
    {
        VirtualAccountDepositServices _services = null;
        KiiPayPersonalCommonServices _kiiPayPersonalCommonServices = null;

        public VirtualAccountDepositController()
        {
            _services = new VirtualAccountDepositServices();
            _kiiPayPersonalCommonServices = new KiiPayPersonalCommonServices();
        }
        // GET: KiiPayPersonal/VirtualAccountDeposit
        public ActionResult Index()
        {
            if (Common.CardUserSession.PayIntoAnotherWalletSession != null)
            {
                Common.CardUserSession.PayIntoAnotherWalletSession = null;
            }
            var vm = new VirtualAccountDepositViewModel();
            vm.CountryPhoneCode = _services.getCountryPhoneCode();
            SetViewBagForPhoneNumbers();
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = VirtualAccountDepositViewModel.BindProperty)]VirtualAccountDepositViewModel model)
        {
            SetViewBagForPhoneNumbers();
            if (ModelState.IsValid)
            {
                if (model.PhoneNumber.Trim() == Common.CardUserSession.LoggedCardUserViewModel.MobileNumber.Trim())
                {
                    ModelState.AddModelError("PhoneNumber", "Invalid Phone Number !");
                    return View(model);
                }
                bool isPhoneNumberValid = _services.isPhoneNumberValid(model.PhoneNumber);
                if (isPhoneNumberValid == false)
                {
                    ModelState.AddModelError("PhoneNumber", "Invalid Phone Number !");
                    return View(model);
                }
                if (Common.CardUserSession.PayIntoAnotherWalletSession != null)
                {
                    Common.CardUserSession.PayIntoAnotherWalletSession = null;
                }
                Common.CardUserSession.PayIntoAnotherWalletSession = new PayIntoAnotherWalletViewModel()
                {
                    SendingPhoneNumber = Common.CardUserSession.LoggedCardUserViewModel.MobileNumber,
                    ReceivingPhoneNumber = model.PhoneNumber,
                    ReceiversName = _kiiPayPersonalCommonServices.getKiiPayUsersNameFromMobile(model.PhoneNumber),
                    PaymentType = DB.PaymentType.Local
                };
                return RedirectToAction("KiiPayEnterAmount");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult SendingMoneyAbroad()
        {
            SetViewBagForCountries();
            SetViewBagForPhoneNumbersInternational();
            var vm = new SendingMoneyAbroadViewModel();
            vm.CountryPhoneCode = "";
            return View(vm);
        }

        [HttpPost]
        public ActionResult SendingMoneyAbroad([Bind(Include = SendingMoneyAbroadViewModel.BindProperty)]SendingMoneyAbroadViewModel model)
        {
            SetViewBagForCountries();
            SetViewBagForPhoneNumbersInternational();
            if (!string.IsNullOrEmpty(model.CountryCode))
            {
                model.CountryPhoneCode = _kiiPayPersonalCommonServices.getCountryPhoneCode(model.CountryCode);
            }

            if (ModelState.IsValid)
            {
                if (model.CountryCode == Common.CardUserSession.LoggedCardUserViewModel.CountryCode)
                {
                    ModelState.AddModelError("CountryCode", "Invalid Country");
                    return View(model);
                }
                //if (model.PhoneNumber.Trim() == Common.CardUserSession.LoggedCardUserViewModel.MobileNumber.Trim())
                //{
                //    ModelState.AddModelError("PhoneNumber", "Invalid Phone Number !");
                //    return View(model);
                //}
                bool isPhoneValid = _services.isphoneNumberValidForCountry(model.PhoneNumber, model.CountryCode);
                if (isPhoneValid == false)
                {
                    ModelState.AddModelError("PhoneNumber", "Invalid Phone Number !");
                    return View(model);
                }
                if (Common.CardUserSession.PayIntoAnotherWalletSession != null)
                {
                    Common.CardUserSession.PayIntoAnotherWalletSession = null;
                }
                Common.CardUserSession.PayIntoAnotherWalletSession = new PayIntoAnotherWalletViewModel()
                {
                    SendingPhoneNumber = Common.CardUserSession.LoggedCardUserViewModel.MobileNumber,
                    ReceivingPhoneNumber = model.PhoneNumber,
                    ReceiversName = _kiiPayPersonalCommonServices.getKiiPayUsersNameFromMobile(model.PhoneNumber),
                    PaymentType = DB.PaymentType.International,
                    ExchangeRate = _kiiPayPersonalCommonServices.calculateExchangeRate(Common.CardUserSession.LoggedCardUserViewModel.CountryCode, model.CountryCode),
                    SendSMSNotification = false,
                    SendingCurrencySymbol = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol,
                    ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(model.CountryCode),
                    SendingCurrencyCode = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrency,
                    ReceivingCurrencyCode = Common.Common.GetCountryCurrency(model.CountryCode)
                };
                return RedirectToAction("KiiPayEnterAmountTwo");

            }
            return View(model);
        }


        [HttpGet]
        public ActionResult KiiPayEnterAmount()
        {
            if (Common.CardUserSession.PayIntoAnotherWalletSession == null)
            {
                return RedirectToAction("Index");
            }
            var vm = _services.getEnterAmountData();
            if (vm != null)
            {
                vm.Name = Common.CardUserSession.PayIntoAnotherWalletSession.ReceiversName;
                return View(vm);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult KiiPayEnterAmount([Bind(Include = KiiPayEnterAmountViewModel.BindProperty)]KiiPayEnterAmountViewModel model)
        {
            if (Common.CardUserSession.PayIntoAnotherWalletSession == null)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                bool validAmount = _services.checkIfAmountIsValid(model.Amount);
                if (validAmount == true)
                {
                    Common.CardUserSession.PayIntoAnotherWalletSession.PayingAmount = model.Amount;
                    Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingAmount = model.Amount;
                    Common.CardUserSession.PayIntoAnotherWalletSession.Amount = model.Amount;
                    Common.CardUserSession.PayIntoAnotherWalletSession.Fee = 0;
                    
                    Common.CardUserSession.PayIntoAnotherWalletSession.PaymentReference = model.PaymentReference;
                    Common.CardUserSession.PayIntoAnotherWalletSession.SendSMSNotification = model.SendSMSNotification;

                    return RedirectToAction("AccountPaymentSummary", "VirtualAccountDeposit");
                }
                else
                {
                    ModelState.AddModelError("Amount", "Invalid Amount !");
                }
            }
            return View(model);

        }
        [HttpGet]
        public ActionResult KiiPayEnterAmountTwo()
        {
            var vm = new KiiPayEnterAmountTwoViewModel()
            {
                SendingCurrencySymbol = Common.CardUserSession.PayIntoAnotherWalletSession.SendingCurrencySymbol,
                SendingCurrencyCode = Common.CardUserSession.PayIntoAnotherWalletSession.SendingCurrencyCode,
                ReceivingCurrencySymbol = Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingCurrencySymbol,
                ReceivingCurrencyCode = Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingCurrencyCode,
                AvailableBalance = Common.CardUserSession.LoggedCardUserViewModel.BalanceOnCard,
                ReceiverName = Common.CardUserSession.PayIntoAnotherWalletSession.ReceiversName,
                PhotoUrl = _services.getPhotoUrl(Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingPhoneNumber),
                SendingAmount = 0,
                ReceivingAmount = 0,
                PayingAmount = 0,
                Fee = 0,
                ExchangeRate = Common.CardUserSession.PayIntoAnotherWalletSession.ExchangeRate
            };
            return View(vm);
        }
        [HttpPost]
        public ActionResult KiiPayEnterAmountTwo([Bind(Include = KiiPayEnterAmountTwoViewModel.BindProperty)]KiiPayEnterAmountTwoViewModel model)
        {
            if (ModelState.IsValid)
            {
                Common.CardUserSession.PayIntoAnotherWalletSession.Amount = model.SendingAmount;
                Common.CardUserSession.PayIntoAnotherWalletSession.PayingAmount = model.PayingAmount;
                Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingAmount = model.ReceivingAmount;
                Common.CardUserSession.PayIntoAnotherWalletSession.Fee = model.Fee;
                Common.CardUserSession.PayIntoAnotherWalletSession.PaymentReference = model.PaymentReference;
                return RedirectToAction("AccountPaymentSummaryAbroad");
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult AccountPaymentSummary(bool isInternationalPayment= false)
        {
            if (Common.CardUserSession.PayIntoAnotherWalletSession == null)
            {
                return RedirectToAction("Index");
            }
            var paymentData = Common.CardUserSession.PayIntoAnotherWalletSession;
            var vm = _kiiPayPersonalCommonServices.CalculateKiiPayPersonalPaymentSummary(paymentData.PayingAmount, paymentData.SendingPhoneNumber, paymentData.ReceivingPhoneNumber, true, paymentData.PaymentReference);
            if (vm != null)
            {
                Common.CardUserSession.PayIntoAnotherWalletSession.Fee = vm.Fee;
                Common.CardUserSession.PayIntoAnotherWalletSession.SMSFee = vm.LocalSMSMessage;
                Common.CardUserSession.PayIntoAnotherWalletSession.PayingAmount = vm.PayingAmount;
                Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingAmount = vm.ReceivingAmount;
                Common.CardUserSession.PayIntoAnotherWalletSession.SendingCurrencyCode = vm.SendingCurrencyCode;
                Common.CardUserSession.PayIntoAnotherWalletSession.SendingCurrencySymbol = vm.SendingCurrencySymbol;
                Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingCurrencyCode = vm.ReceivingCurrencyCode;
                Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingCurrencySymbol = vm.ReceivingCurrencySymbol;
                Common.CardUserSession.PayIntoAnotherWalletSession.ExchangeRate = vm.ExchangeRate;


                vm.ReceiversName = Common.CardUserSession.PayIntoAnotherWalletSession.ReceiversName;

                return View(vm);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult AccountPaymentSummaryAbroad()
        {
            var vm = new AccountPaymentSummaryAbroadViewModel()
            {
                AvailableBalance = Common.CardUserSession.LoggedCardUserViewModel.BalanceOnCard,
                Amount = Common.CardUserSession.PayIntoAnotherWalletSession.Amount,
                Fee = Common.CardUserSession.PayIntoAnotherWalletSession.Fee,
                PayingAmount = Common.CardUserSession.PayIntoAnotherWalletSession.PayingAmount,
                ReceivingAmount = Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingAmount,
                PaymentReference = Common.CardUserSession.PayIntoAnotherWalletSession.PaymentReference,
                SendingCurrency = Common.CardUserSession.PayIntoAnotherWalletSession.SendingCurrencyCode,
                ReceivingCurrency = Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingCurrencyCode,
                SendingCurrencySymbol = Common.CardUserSession.PayIntoAnotherWalletSession.SendingCurrencySymbol,
                ReceivingCurrencySymbol = Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingCurrencySymbol,
                ReceiversName = Common.CardUserSession.PayIntoAnotherWalletSession.ReceiversName
            };
            return View(vm);
        }

        [HttpPost]
        public ActionResult AccountPaymentSummaryAbroad([Bind(Include = AccountPaymentSummaryAbroadViewModel.BindProperty)]AccountPaymentSummaryAbroadViewModel model)
        {
            if (Common.CardUserSession.PayIntoAnotherWalletSession == null)
            {
                return RedirectToAction("Index");
            }
            bool sendMoney = _kiiPayPersonalCommonServices.sendMoneyKiiPayPersonal();
            if (sendMoney == true)
            {
                return RedirectToAction("WalletPaymentSuccess");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult AccountPaymentSummary([Bind(Include = AccountPaymentSummaryViewModel.BindProperty)]AccountPaymentSummaryViewModel model)
        {
            if (Common.CardUserSession.PayIntoAnotherWalletSession == null)
            {
                return RedirectToAction("Index");
            }
            bool sendMoney = _kiiPayPersonalCommonServices.sendMoneyKiiPayPersonal();
            if (sendMoney == true)
            {
                return RedirectToAction("WalletPaymentSuccess");
            }
            return View();
        }




        public ActionResult WalletPaymentSuccess()
        {
            if (Common.CardUserSession.PayIntoAnotherWalletSession == null)
            {
                return RedirectToAction("Index");
            }
            var vm = new WalletPaymentSuccessViewModel()
            {
                CurrencySymbol = Common.CardUserSession.PayIntoAnotherWalletSession.SendingCurrencySymbol,
                Receiver = Common.CardUserSession.PayIntoAnotherWalletSession.ReceiversName,
                SentAmount = Common.CardUserSession.PayIntoAnotherWalletSession.PayingAmount
            };
            Common.CardUserSession.LoggedCardUserViewModel.BalanceOnCard = _kiiPayPersonalCommonServices.getAvailableBalance();
            Common.CardUserSession.PayIntoAnotherWalletSession = null;
            return View(vm);
        }

        public JsonResult getCountryPhoneCode(string countryCode)
        {
            if (!string.IsNullOrEmpty(countryCode))
            {
                var phoneCode = _kiiPayPersonalCommonServices.getCountryPhoneCode(countryCode);
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
                    PayingAmount = result.TotalAmount,
                    Fee = result.FaxingFee
                }, JsonRequestBehavior.AllowGet);


            }
            return Json(new
            {
            }, JsonRequestBehavior.AllowGet);
        }

        private void SetViewBagForPhoneNumbers()
        {
            var phoneNumbers = _services.getRecentPhoneNumbers();
            ViewBag.PhoneNumbers = new SelectList(phoneNumbers, "Code", "Name");
        }

        private void SetViewBagForPhoneNumbersInternational()
        {
            var phoneNumbers = _services.getRecentPhoneNumbersInternational();
            ViewBag.PhoneNumbers = new SelectList(phoneNumbers, "Code", "Name");
        }

        private void SetViewBagForCountries()
        {
            var countries = _kiiPayPersonalCommonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }
    }
}