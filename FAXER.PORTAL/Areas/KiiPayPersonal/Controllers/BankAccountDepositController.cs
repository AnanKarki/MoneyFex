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
    public class BankAccountDepositController : Controller
    {
        // GET: KiiPayPersonal/BankAccountDeposit
        BankAccountDepositServices _services = null;
        KiiPayPersonalCommonServices _commonServices = null;
        public BankAccountDepositController()
        {
            _services = new BankAccountDepositServices();
            _commonServices = new KiiPayPersonalCommonServices();
        }
        public ActionResult Index()
        {
            SetViewBagForRecentNumbersNational();
            SetViewBagForBanks();
            SetViewBagForBranches();
            return View();
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = BankAccountDepositViewModel.BindProperty)]BankAccountDepositViewModel model)
        {
            SetViewBagForRecentNumbersNational();
            SetViewBagForBanks();
            SetViewBagForBranches();
            if (ModelState.IsValid)
            {
                if (Common.CardUserSession.PayToBankAccountSession != null)
                {
                    Common.CardUserSession.PayToBankAccountSession = null;
                }
                Common.CardUserSession.PayToBankAccountSession = new PayToBankAccountSessionViewModel()
                {
                    ReceivingCountry = Common.CardUserSession.LoggedCardUserViewModel.CountryCode,
                    PaymentType = DB.PaymentType.Local,
                    AccountOwnerName = model.AccountOwnerName,
                    AccountHolderPhoneNumber = model.MobileNumber,
                    BankAccountNumber = model.AccountNumber,
                    BankId = model.BankId,
                    BranchId = model.Branch,
                    BranchCode = model.BranchCode,
                    ExchangeRate = 1,
                    Fee = 0

                };
                return RedirectToAction("BankAccountAmount");
            }
            return View();
        }


        [HttpGet]
        public ActionResult BankAccountAmount()
        {
            var vm = new KiiPayEnterAmountViewModel()
            {
                PhotoUrl = "",
                Name = ""
            };
            return View(vm);
        }

        [HttpPost]
        public ActionResult BankAccountAmount([Bind(Include = KiiPayEnterAmountViewModel.BindProperty)]KiiPayEnterAmountViewModel model)
        {
            if (Common.CardUserSession.PayToBankAccountSession == null)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                if (model.Amount > Common.CardUserSession.LoggedCardUserViewModel.BalanceOnCard)
                {
                    ModelState.AddModelError("Amount", "Invalid Amount !");
                    return View(model);
                }
                Common.CardUserSession.PayToBankAccountSession.SendingAmount = model.Amount;
                Common.CardUserSession.PayToBankAccountSession.ReceivingAmount = model.Amount;
                decimal smsFee = 0;
                if (model.SendSMSNotification == true)
                {
                    smsFee = Common.Common.GetSmsFee(Common.CardUserSession.LoggedCardUserViewModel.CountryCode);
                }
                Common.CardUserSession.PayToBankAccountSession.TotalAmount = model.Amount + smsFee;
                Common.CardUserSession.PayToBankAccountSession.PaymentReferene = model.PaymentReference;

                return RedirectToAction("BankAccountDepositSummary");
            }
            return View();
        }

        [HttpGet]
        public ActionResult BankAccountDepositSummary()
        {
            var vm = new AccountPaymentSummaryViewModel()
            {
                SendingCurrencySymbol = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol,
                ReceivingCurrencySymbol = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol,
                SendingCurrencyCode = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrency,
                ReceivingCurrencyCode = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrency,
                Amount = Common.CardUserSession.PayToBankAccountSession.SendingAmount,
                Fee = Common.CardUserSession.PayToBankAccountSession.Fee,
                ExchangeRate = Common.CardUserSession.PayToBankAccountSession.ExchangeRate,
                LocalSMSMessage = Common.CardUserSession.PayToBankAccountSession.SmsFee,
                PayingAmount = Common.CardUserSession.PayToBankAccountSession.TotalAmount,
                ReceivingAmount = Common.CardUserSession.PayToBankAccountSession.ReceivingAmount,
                PaymentReference = Common.CardUserSession.PayToBankAccountSession.PaymentReferene,
                ReceiversName = ""
            };
            return View(vm);
        }

        [HttpPost]
        public ActionResult BankAccountDepositSummary([Bind(Include = AccountPaymentSummaryViewModel.BindProperty)]AccountPaymentSummaryViewModel model)
        {
            if (Common.CardUserSession.PayToBankAccountSession == null)
            {
                return RedirectToAction("Index");
            }

            bool save = _services.savePaymentTransferData();
            if (save == true)
            {
                string amount = Common.CardUserSession.PayToBankAccountSession.SendingAmount.ToString();
                string receiverName = Common.CardUserSession.PayToBankAccountSession.AccountOwnerName;
                Common.CardUserSession.PayToBankAccountSession = null;
                return RedirectToAction("BankDepositSuccess", new { @amount = amount, @receiverName = receiverName });
            }
            return RedirectToAction("BankAccountDepositSummary");
        }

        public ActionResult BankDepositSuccess(string amount, string receiverName)
        {
            if (!string.IsNullOrEmpty(amount) && !string.IsNullOrEmpty(receiverName))
            {
                ViewBag.Amount = amount;
                ViewBag.ReceiverName = receiverName;
                return View();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult BankDepositAbroad()
        {
            SetViewBagForCountries();
            SetViewBagForRecentNumbersInternational();
            SetViewBagForBanks();
            SetViewBagForBranches();
            return View();
        }

        [HttpPost]
        public ActionResult BankDepositAbroad([Bind(Include = BankAccountDepositAbroadViewModel.BindProperty)]BankAccountDepositAbroadViewModel model)
        {
            SetViewBagForCountries();
            SetViewBagForRecentNumbersInternational();
            SetViewBagForBanks();
            SetViewBagForBranches();
            if (ModelState.IsValid)
            {
                if (Common.CardUserSession.PayToBankAccountSession != null)
                {
                    Common.CardUserSession.PayToBankAccountSession = null;
                }
                Common.CardUserSession.PayToBankAccountSession = new PayToBankAccountSessionViewModel()
                {
                    ReceivingCountry = model.Country,
                    ReceivingCountryPhoneCode = Common.Common.GetCountryPhoneCode(model.Country),
                    ReceivingCountryCurrency = Common.Common.GetCurrencySymbol(model.Country),
                    ReceivingCoutnryCurrencySymbol = Common.Common.GetCurrencySymbol(model.Country),
                    PaymentType = DB.PaymentType.International,
                    AccountOwnerName = model.AccountOwner,
                    BankAccountNumber = model.AccountNumber,
                    AccountHolderPhoneNumber = model.TelephoneNumber,
                    BankId = model.BankName,
                    BranchId = model.Branch,
                    BranchCode = model.BranchCode,
                    Fee = SEstimateFee.GetFaxingCommision(Common.CardUserSession.LoggedCardUserViewModel.CountryCode),
                    ExchangeRate = _commonServices.calculateExchangeRate(Common.CardUserSession.LoggedCardUserViewModel.CountryCode, model.Country)

                };
                return RedirectToAction("BankAccountAmountAbroad");
            }
            return View();
        }

        [HttpGet]
        public ActionResult BankAccountAmountAbroad()
        {
            if (Common.CardUserSession.PayToBankAccountSession == null)
            {
                return RedirectToAction("BankDepositAbroad");
            }
            var vm = new KiiPayEnterAmountTwoViewModel()
            {
                SendingCurrencySymbol = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol,
                SendingCurrencyCode = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrency,
                ReceivingCurrencySymbol = Common.CardUserSession.PayToBankAccountSession.ReceivingCoutnryCurrencySymbol,
                ReceivingCurrencyCode = Common.CardUserSession.PayToBankAccountSession.ReceivingCountryCurrency,
                ReceiverName = Common.CardUserSession.PayToBankAccountSession.AccountOwnerName,
                PhotoUrl = "",
                Fee = Common.CardUserSession.PayToBankAccountSession.Fee,
                ExchangeRate = Common.CardUserSession.PayToBankAccountSession.ExchangeRate
            };
            return View(vm);
        }
        [HttpPost]
        public ActionResult BankAccountAmountAbroad([Bind(Include = KiiPayEnterAmountTwoViewModel.BindProperty)]KiiPayEnterAmountTwoViewModel model)
        {
            if (ModelState.IsValid)
            {
                Common.CardUserSession.PayToBankAccountSession.ReceivingAmount = model.ReceivingAmount;
                Common.CardUserSession.PayToBankAccountSession.SendingAmount = model.SendingAmount;
                Common.CardUserSession.PayToBankAccountSession.TotalAmount = model.PayingAmount;
                Common.CardUserSession.PayToBankAccountSession.PaymentReferene = model.PaymentReference;
                return RedirectToAction("BankAccountDepositSummaryAbroad");
            }
            return View(model);

        }

        [HttpGet]
        public ActionResult BankAccountDepositSummaryAbroad()
        {
            if(Common.CardUserSession.PayToBankAccountSession == null)
            {
                return RedirectToAction("Index");
            }
            var vm = new AccountPaymentSummaryViewModel()
            {
                SendingCurrencyCode = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrency,
                SendingCurrencySymbol = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol,
                ReceivingCurrencyCode = Common.CardUserSession.PayToBankAccountSession.ReceivingCountryCurrency,
                ReceivingCurrencySymbol = Common.CardUserSession.PayToBankAccountSession.ReceivingCoutnryCurrencySymbol,
                Amount = Common.CardUserSession.PayToBankAccountSession.SendingAmount,
                Fee = Common.CardUserSession.PayToBankAccountSession.Fee,
                ExchangeRate = Common.CardUserSession.PayToBankAccountSession.ExchangeRate,
                LocalSMSMessage = 0,
                PayingAmount = Common.CardUserSession.PayToBankAccountSession.TotalAmount,
                ReceivingAmount = Common.CardUserSession.PayToBankAccountSession.ReceivingAmount,
                PaymentReference = Common.CardUserSession.PayToBankAccountSession.PaymentReferene,
                ReceiversName = Common.CardUserSession.PayToBankAccountSession.AccountOwnerName
            };
            return View(vm);
        }

        [HttpPost]
        public ActionResult BankAccountDepositSummaryAbroad([Bind(Include = AccountPaymentSummaryViewModel.BindProperty)]AccountPaymentSummaryViewModel model)
        {
            if(Common.CardUserSession.PayToBankAccountSession == null)
            {
                return RedirectToAction("Index");
            }
            bool save = _services.savePaymentTransferData();
            if (save == true)
            {
                string amount = Common.CardUserSession.PayToBankAccountSession.SendingAmount.ToString();
                string receiverName = Common.CardUserSession.PayToBankAccountSession.AccountOwnerName;
                Common.CardUserSession.PayToBankAccountSession = null;
                return RedirectToAction("BankDepositSuccess", new { @amount = amount, @receiverName = receiverName });
            }
            return View();
        }

        private void SetViewBagForRecentNumbersNational()
        {
            var recentNumbers = _services.getRecentAccountNumberNational();
            ViewBag.RecentAccountNumbers = new SelectList(recentNumbers, "Code", "Name");
        }

        private void SetViewBagForRecentNumbersInternational()
        {
            var recentNumbers = _services.getRecentAccountNumberInternational();
            ViewBag.RecentAccountNumbers = new SelectList(recentNumbers, "Code", "Name");
        }

        private void SetViewBagForBanks()
        {
            var banks = _services.getBanksList();
            ViewBag.BankNames = new SelectList(banks, "Code", "Name");
        }

        private void SetViewBagForBranches()
        {
            var branches = _services.getBranchesList();
            ViewBag.Branches = new SelectList(branches, "Code", "Name");
        }

        private void SetViewBagForCountries()
        {
            var countries = _commonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
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
                var result = SEstimateFee.CalculateFaxingFee(amount, true, isAmountToBeReceived, Common.CardUserSession.PayToBankAccountSession.ExchangeRate, SEstimateFee.GetFaxingCommision(Common.CardUserSession.LoggedCardUserViewModel.CountryCode));
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

    }
}