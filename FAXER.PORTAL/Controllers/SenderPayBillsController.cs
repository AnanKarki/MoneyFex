using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class SenderPayBillsController : Controller
    {
        SSuppliers _sSuppliersServices = null;
        // GET: SenderPayBills
        SPayBill _payBillServices = null;
        STopUpToSupplier _topUpToSupplierServices = null;
        public SenderPayBillsController()
        {
            _sSuppliersServices = new SSuppliers();
            _payBillServices = new SPayBill();
            _topUpToSupplierServices = new STopUpToSupplier();
        }
        public ActionResult Index()
        {
            return View();
        }

        #region Pay a Monthly Bill

        public List<Country> GetSenderCountries()
        {

            var countries = Common.Common.GetCountries();
            List<Country> list = (from c in countries.ToList()
                                  join d in _sSuppliersServices.List().Data.ToList() on c.CountryCode equals d.Country
                                  select c).ToList();
            return list;

        }

        public List<SenderSupplierDropDownVM> GetSuppliers(string country = "")
        {

            var data = _sSuppliersServices.List().Data;
            if (!string.IsNullOrEmpty(country))
            {

                data = data.Where(x => x.Country.ToLower() == country.ToLower());
            }
            else
            {
                data = data.Where(x => x.Country.ToLower() == Common.FaxerSession.LoggedUser.CountryCode);
            }
            var result = (from c in data.ToList()
                          select new SenderSupplierDropDownVM()
                          {
                              MobileNo = c.KiiPayBusinessInformation.BusinessMobileNo,
                              Name = c.KiiPayBusinessInformation.BusinessName,
                          }).ToList();
            return result;

        }

        [HttpGet]
        public ActionResult PayMonthlyBill()
        {

            return View();
        }

        [HttpGet]
        public ActionResult SenderBillingServices()
        {

            SenderPayMonthlyBillVM vm = new SenderPayMonthlyBillVM();
            vm = _payBillServices.GetSenderPayMonthlyBillVM();
            ViewBag.SenderCountries = new SelectList(GetSenderCountries(), "CountryCode", "CountryName", vm.SupplierCountryCode);
            return View(vm);
        }

        [HttpPost]
        public ActionResult SenderBillingServices([Bind(Include = SenderPayMonthlyBillVM.BindProperty)]SenderPayMonthlyBillVM vm)
        {
            ViewBag.SenderCountries = new SelectList(GetSenderCountries(), "CountryCode", "CountryName", vm.SupplierCountryCode);
            if (ModelState.IsValid)
            {
                Common.FaxerSession.PayBillSupplierCountryCode = vm.SupplierCountryCode;
                return RedirectToAction("PayingSupplierAbroadReference");
            }
            return View(vm);

        }

        [HttpGet]
        public ActionResult PayingALocalSupplier()
        {

            SenderPayingSupplierAbroadReferenceVM model = new SenderPayingSupplierAbroadReferenceVM();
            model.ReferenceNo = "HV8";

            return View(model);

        }

        [HttpPost]
        public ActionResult PayingALocalSupplier([Bind(Include = SenderPayingSupplierAbroadReferenceVM.BindProperty)]SenderPayingSupplierAbroadReferenceVM vm)
        {
            vm.ReferenceNo = "HV8";

            Common.FaxerSession.PaymentReference = vm.ReferenceNo + vm.ReferenceNo1 + vm.ReferenceNo2 + vm.ReferenceNo3;
            var supplier = _sSuppliersServices.List().Data.Where(x => x.RefCode == Common.FaxerSession.PaymentReference).FirstOrDefault();

            if (supplier != null)
            {
                return RedirectToAction("PayingALocalSupplierReferenceOne");
            }
            else
            {
                ModelState.AddModelError("ReferenceNo", "Suppliers Reference does not match");
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult PayingALocalSupplierReferenceOne()
        {
            var supplier = _sSuppliersServices.List().Data.Where(x => x.RefCode == Common.FaxerSession.PaymentReference).FirstOrDefault();

            var transactionsummary = SEstimateFee.CalculateFaxingFee(4350M, false, true,
                                                         SExchangeRate.GetExchangeRateValue(Common.FaxerSession.LoggedUser.CountryCode, supplier.Country)
                                                          , SEstimateFee.GetFaxingCommision(Common.FaxerSession.LoggedUser.CountryCode));

            SenderPayingSupplierAbroadReferenceOneVM model = new SenderPayingSupplierAbroadReferenceOneVM()
            {
                PhotoUrl = "",
                ReceiverName = supplier.KiiPayBusinessInformation.BusinessName,
                ReferenceNo = Common.FaxerSession.PaymentReference,
                BillNo = "39658607",
                Amount = transactionsummary.ReceivingAmount,
                Fee = transactionsummary.FaxingFee,
                Total = transactionsummary.TotalAmount,
                ExchangeRate = transactionsummary.ExchangeRate,
                SendingAmount = transactionsummary.FaxingAmount,
                Currency = Common.Common.GetCountryCurrency(Common.FaxerSession.LoggedUser.CountryCode),
            };
            _payBillServices.SetSenderPayingSupplierAbroadReferenceOne(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult PayingALocalSupplierReferenceOne([Bind(Include = SenderPayingSupplierAbroadReferenceOneVM.BindProperty)]SenderPayingSupplierAbroadReferenceOneVM model)
        {
            var supplier = _sSuppliersServices.List().Data.Where(x => x.RefCode == Common.FaxerSession.PaymentReference).FirstOrDefault();

            var senderPayingSupplierAbroad = _payBillServices.GetSenderPayingSupplierAbroadReferenceOne();

            if (supplier != null)
            {
                PayBill payBill = new PayBill()
                {
                    BillNo = senderPayingSupplierAbroad.BillNo,
                    Amount = senderPayingSupplierAbroad.Amount,
                    Fee = senderPayingSupplierAbroad.Fee,
                    Total = senderPayingSupplierAbroad.Total,
                    PayerId = Common.FaxerSession.LoggedUser.Id,
                    Module = Module.Faxer,
                    RefCode = senderPayingSupplierAbroad.ReferenceNo,
                    SupplierId = supplier.Id,
                    PayerCountry = Common.Common.GetCountryName(supplier.Country),
                    SupplierCountry = Common.Common.GetCountryName(supplier.Country),
                    PaymentDate = DateTime.Now,
                    PaymentType = PaymentType.Local,
                    ExchangeRate = senderPayingSupplierAbroad.ExchangeRate,
                    SendingAmount = senderPayingSupplierAbroad.SendingAmount
                };
                _payBillServices.Add(payBill);
                return RedirectToAction("PayBillsReferenceSuccess");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult PayingSupplierAbroadReference()
        {

            SenderPayingSupplierAbroadReferenceVM model = new SenderPayingSupplierAbroadReferenceVM();
            model.ReferenceNo = "HV8";

            return View(model);

        }


        [HttpPost]
        public ActionResult PayingSupplierAbroadReference([Bind(Include = SenderPayingSupplierAbroadReferenceVM.BindProperty)]SenderPayingSupplierAbroadReferenceVM vm)
        {
            vm.ReferenceNo = "HV8";

            Common.FaxerSession.PaymentReference = vm.ReferenceNo + vm.ReferenceNo1 + vm.ReferenceNo2 + vm.ReferenceNo3;
            var supplier = _sSuppliersServices.List().Data.Where(x => x.RefCode == Common.FaxerSession.PaymentReference).FirstOrDefault();

            if (supplier != null)
            {
                return RedirectToAction("PayingSupplierAbroadReferenceOne");
            }
            else
            {
                ModelState.AddModelError("ReferenceNo", "Suppliers Reference does not match");
                return View(vm);
            }

        }

        [HttpGet]
        public ActionResult PayingSupplierAbroadReferenceOne()
        {
            var supplier = _sSuppliersServices.List().Data.Where(x => x.RefCode == Common.FaxerSession.PaymentReference).FirstOrDefault();
            var transactionsummary = SEstimateFee.CalculateFaxingFee(4350M, false, true,
                                                        SExchangeRate.GetExchangeRateValue(Common.FaxerSession.LoggedUser.CountryCode, supplier.Country)
                                                         , SEstimateFee.GetFaxingCommision(Common.FaxerSession.LoggedUser.CountryCode));

            SenderPayingSupplierAbroadReferenceOneVM model = new SenderPayingSupplierAbroadReferenceOneVM()
            {
                PhotoUrl = "",
                ReceiverName = supplier.KiiPayBusinessInformation.BusinessName,
                ReferenceNo = Common.FaxerSession.PaymentReference,
                BillNo = "39658607",
                Amount = transactionsummary.ReceivingAmount,
                Fee = transactionsummary.FaxingFee,
                Total = transactionsummary.TotalAmount,
                ExchangeRate = transactionsummary.ExchangeRate,
                SendingAmount = transactionsummary.FaxingAmount,
                Currency = Common.Common.GetCountryCurrency(Common.FaxerSession.LoggedUser.CountryCode),

            };
            _payBillServices.SetSenderPayingSupplierAbroadReferenceOne(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult PayingSupplierAbroadReferenceOne([Bind(Include = SenderPayingSupplierAbroadReferenceOneVM.BindProperty)]SenderPayingSupplierAbroadReferenceOneVM model)
        {
            var supplier = _sSuppliersServices.List().Data.Where(x => x.RefCode == Common.FaxerSession.PaymentReference).FirstOrDefault();

            var senderPayingSupplierAbroad = _payBillServices.GetSenderPayingSupplierAbroadReferenceOne();
            var sender = Common.FaxerSession.LoggedUser;

            var result = SEstimateFee.CalculateFaxingFee(senderPayingSupplierAbroad.Amount, false, false,
                senderPayingSupplierAbroad.ExchangeRate, SEstimateFee.GetFaxingCommision(sender.CountryCode));
            if (supplier != null)
            {
                PayBill payBill = new PayBill()
                {
                    BillNo = senderPayingSupplierAbroad.BillNo,
                    Amount = senderPayingSupplierAbroad.Amount,
                    Fee = senderPayingSupplierAbroad.Fee,
                    Total = senderPayingSupplierAbroad.Total,
                    PayerId = Common.FaxerSession.LoggedUser.Id,
                    Module = Module.Faxer,
                    RefCode = senderPayingSupplierAbroad.ReferenceNo,
                    SupplierId = supplier.Id,
                    PayerCountry = Common.FaxerSession.FaxerCountry,
                    SupplierCountry = Common.Common.GetCountryName(supplier.Country),
                    PaymentDate = DateTime.Now,
                    PaymentType = PaymentType.International,
                    ExchangeRate = senderPayingSupplierAbroad.ExchangeRate,
                    SendingAmount = senderPayingSupplierAbroad.SendingAmount
                };
                _payBillServices.Add(payBill);
                return RedirectToAction("PayBillsReferenceSuccess");
            }
            return View(model);
        }

        public ActionResult PayBillsReferenceSuccess()
        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            var senderPayingSupplierAbroad = _payBillServices.GetSenderPayingSupplierAbroadReferenceOne();
            SenderPayingSupplierAbroadReferenceOneVM model = new SenderPayingSupplierAbroadReferenceOneVM()
            {
                ReceiverName = senderPayingSupplierAbroad.ReceiverName,
                ReferenceNo = senderPayingSupplierAbroad.ReferenceNo,
            };
            senderCommonFunc.ClearPayBillsSession();
            return View(model);
        }
        #endregion

        #region Top-Up an Amount
        [HttpGet]
        public ActionResult TopUpAnAccount()
        {
            return View();
        }
        [HttpGet]
        public ActionResult SenderTopUpServices(string country = "")
        {
            SenderTopUpAnAccountVM model = new SenderTopUpAnAccountVM();
            model = _topUpToSupplierServices.GetSenderTopUpAnAccount();
            if (model.Country != null)
            {

                ViewBag.SenderCountries = new SelectList(GetSenderCountries(), "CountryName", "CountryName", model.Country);
                ViewBag.SenderSuppliers = new SelectList(GetSuppliers(country), "Name", "Name", model.Supplier);
            }
            else
            {
                ViewBag.SenderCountries = new SelectList(GetSenderCountries(), "CountryCode", "CountryName", country);
                ViewBag.SenderSuppliers = new SelectList(GetSuppliers(country), "MobileNo", "Name", country);

            }
            return View(model);
        }

        [HttpPost]
        public ActionResult SenderTopUpServices([Bind(Include = SenderTopUpAnAccountVM.BindProperty)]SenderTopUpAnAccountVM vm)
        {
            ViewBag.SenderCountries = new SelectList(GetSenderCountries(), "CountryCode", "CountryName", vm.Country);
            ViewBag.SenderSuppliers = new SelectList(GetSuppliers(vm.Country), "MobileNo", "Name");
            _topUpToSupplierServices.SetSenderTopUpAnAccount(vm);
            if (ModelState.IsValid)
            {
                return RedirectToAction("TopUpSupplierAbroadAccountNmber");
            }

            return View(vm);

        }
        [HttpGet]
        public ActionResult TopUpSupplierAbroadAccountNmber()
        {
            var Vm = _topUpToSupplierServices.GetSenderTopUpAnAccount();
            var SupplierName = _sSuppliersServices.List().Data.Where(x => x.KiiPayBusinessInformation.BusinessMobileNo == Vm.Supplier || x.KiiPayBusinessInformation.BusinessMobileNo == Vm.WalletNo).Select(x => x.KiiPayBusinessInformation.BusinessName).FirstOrDefault();
            SenderTopUpSupplierAbroadAccountNmberVM model = new SenderTopUpSupplierAbroadAccountNmberVM()
            {
                Name = SupplierName,
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult TopUpSupplierAbroadAccountNmber([Bind(Include = SenderTopUpSupplierAbroadAccountNmberVM.BindProperty)]SenderTopUpSupplierAbroadAccountNmberVM vm)
        {
            Common.FaxerSession.SenderTopUpAccountNumber = vm.AccountNo1 + vm.AccountNo2 + vm.AccountNo3 + vm.AccountNo4 + vm.AccountNo5 + vm.AccountNo6;
            var supplier = _topUpToSupplierServices.GetSenderTopUpAnAccount();
            var supplierRefNo = _sSuppliersServices.List().Data.Where(x => x.KiiPayBusinessInformation.BusinessMobileNo == supplier.Supplier || x.KiiPayBusinessInformation.BusinessMobileNo == supplier.WalletNo).Select(x => x.RefCode).FirstOrDefault();

            if (ModelState.IsValid)
            {
                if (supplierRefNo == Common.FaxerSession.SenderTopUpAccountNumber)
                {
                    return RedirectToAction("TopUpSupplierAbroadAbroadEnterAmont");
                }

                else
                {
                    ModelState.AddModelError(vm.AccountNo1, "Please Enter the Correct Account Number");
                    return View(vm);
                }
            }
            return View(vm);
        }
        [HttpGet]
        public ActionResult TopUpSupplierAbroadAbroadEnterAmont()
        {
            var SenderTopUpAnAccount = _topUpToSupplierServices.GetSenderTopUpAnAccount();
            var senderCountry = Common.FaxerSession.LoggedUser.CountryCode;

            var supplierDetails = _sSuppliersServices.List().Data.Where(x => x.KiiPayBusinessInformation.BusinessMobileNo == SenderTopUpAnAccount.Supplier || x.KiiPayBusinessInformation.BusinessMobileNo == SenderTopUpAnAccount.WalletNo).FirstOrDefault();
            SenderTopUpSupplierAbroadAbroadEnterAmontVM vm = new SenderTopUpSupplierAbroadAbroadEnterAmontVM()
            {
                ExchangeRate = Common.Common.GetExchangeRate(senderCountry, supplierDetails.Country),
                ReceiverName = supplierDetails.KiiPayBusinessInformation.BusinessName,
                ReceiverAccountNo = supplierDetails.RefCode,
                SendingCurrencySymbol = Common.Common.GetCurrencySymbol(senderCountry),
                ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(supplierDetails.Country),
                SendingCurrency = Common.Common.GetCountryCurrency(senderCountry),
                ReceivingCurrency = Common.Common.GetCountryCurrency(supplierDetails.Country),
                PhotoUrl = "",


            };
            _topUpToSupplierServices.SetSenderTopUpSupplierAbroadAbroadEnterAmont(vm);
            return View(vm);
        }

        [HttpPost]
        public ActionResult TopUpSupplierAbroadAbroadEnterAmont([Bind(Include = SenderTopUpSupplierAbroadAbroadEnterAmontVM.BindProperty)]SenderTopUpSupplierAbroadAbroadEnterAmontVM vm)
        {
            var SenderTopUpAnAccount = _topUpToSupplierServices.GetSenderTopUpAnAccount();
            var supplierDetails = _sSuppliersServices.List().Data.Where(x => x.KiiPayBusinessInformation.BusinessMobileNo == SenderTopUpAnAccount.WalletNo).FirstOrDefault();

            var result = _topUpToSupplierServices.GetSenderTopUpSupplierAbroadAbroadEnterAmont();
            if (result != null)
            {
                TopUpToSupplier addTopUp = new TopUpToSupplier()
                {
                    PayerId = Common.FaxerSession.LoggedUser.Id,
                    EcxhangeRate = result.ExchangeRate,
                    PaymentDate = DateTime.Now,
                    Fee = result.Fee,
                    PayingCountry = Common.Common.GetCountryName(Common.FaxerSession.LoggedUser.CountryCode),
                    PaymentModule = Module.Faxer,
                    PaymentType = PaymentType.International,
                    ReceivingAmount = result.ReceivingAmount,
                    SendingAmount = result.SendingAmount,
                    SuplierId = supplierDetails.Id,
                    SupplierAccountNo = result.ReceiverAccountNo,
                    SupplierCountry = Common.Common.GetCountryName(supplierDetails.Country),
                    TotalAmount = result.TotalAmount,
                    WalletNo = SenderTopUpAnAccount.WalletNo,
                };
                _topUpToSupplierServices.Add(addTopUp);
            }
            else
            {
                return View(vm);
            }

            if (vm.SetStandingOrderPayment == true)
            {
                if (vm.Amount > 0)
                {
                    SupplierStandingOrderPayment standingOrderPayment = new SupplierStandingOrderPayment()
                    {
                        FrequenncyDetails = result.FrequencyDetails,
                        Amount = vm.Amount,
                        PayerId = Common.FaxerSession.LoggedUser.Id,
                        PaymentFrequency = vm.PaymentFrequencyId,
                        SupplierId = supplierDetails.KiiPayBusinessInformation.Id
                    };
                    _topUpToSupplierServices.AddSupplierStandingOrderPayment(standingOrderPayment);
                }
                else
                {
                    ModelState.AddModelError(vm.Amount.ToString(), "Please enter the amount");
                    return View(vm);
                }
            }

            return RedirectToAction("TopUpAccountAbroadSuccess", "SenderPayBills");





        }


        public ActionResult TopUpAccountAbroadSuccess()
        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            var result = _topUpToSupplierServices.GetSenderTopUpSupplierAbroadAbroadEnterAmont();
            SenderTopUpSupplierAbroadAbroadEnterAmontVM vm = new SenderTopUpSupplierAbroadAbroadEnterAmontVM()
            {
                ReceiverAccountNo = result.ReceiverAccountNo,
                SendingAmount = result.SendingAmount,
                SendingCurrencySymbol = result.SendingCurrencySymbol,
            };
            senderCommonFunc.ClearPayBillsSession();
            return View(vm);
        }

        //this action used for change
        [HttpGet]
        public ActionResult TopUpSupplierAbroad()
        {

            SenderTopUpSupplierAbroadVm model = new SenderTopUpSupplierAbroadVm();
            model = _topUpToSupplierServices.GetSenderTopUpSupplierAbroadVm();

            ViewBag.Suppliers = new SelectList(GetSuppliers(), "MobileNo", "Name");
            if (model.Supplier != null)
            {
                ViewBag.Suppliers = new SelectList(GetSuppliers(), "Name", "Name");

            }
            return View(model);
        }
        [HttpPost]
        public ActionResult TopUpSupplierAbroad([Bind(Include = SenderTopUpSupplierAbroadAccountNmberVM.BindProperty)]SenderTopUpSupplierAbroadVm vm)
        {

            ViewBag.Suppliers = new SelectList(GetSuppliers(), "MobileNo", "Name");

            if (ModelState.IsValid)
            {
                _topUpToSupplierServices.SetSenderTopUpSupplierLocal(vm);
                return RedirectToAction("TopUpAccountNumber", vm);

            }
            return View(vm);
        }
        [HttpGet]
        public ActionResult TopUpAccountNumber([Bind(Include = SenderTopUpSupplierAbroadAccountNmberVM.BindProperty)]SenderTopUpSupplierAbroadVm vm)
        {

            var SupplierName = _sSuppliersServices.List().Data.Where(x => x.KiiPayBusinessInformation.BusinessMobileNo == vm.Supplier).Select(x => x.KiiPayBusinessInformation.BusinessName).FirstOrDefault();
            SenderTopUpSupplierAbroadAccountNmberVM model = new SenderTopUpSupplierAbroadAccountNmberVM()
            {
                Name = SupplierName,
            };
            return View(model);
        }
        [HttpPost]
        public ActionResult TopUpAccountNumber([Bind(Include = SenderTopUpSupplierAbroadAccountNmberVM.BindProperty)]SenderTopUpSupplierAbroadAccountNmberVM vm)
        {


            Common.FaxerSession.SenderTopUpAccountNumber = vm.AccountNo1 + vm.AccountNo2 + vm.AccountNo3 + vm.AccountNo4 + vm.AccountNo5 + vm.AccountNo6;
            var supplierNumber = _topUpToSupplierServices.GetSenderTopUpSupplierLocal();
            var supplierRefNo = _sSuppliersServices.List().Data.Where(x => x.KiiPayBusinessInformation.BusinessMobileNo == supplierNumber.WalletNo).Select(x => x.RefCode).FirstOrDefault();
            if (supplierRefNo == Common.FaxerSession.SenderTopUpAccountNumber)
            {
                return RedirectToAction("TopUpAccountEnterAmount");
            }

            else
            {
                ModelState.AddModelError(vm.AccountNo1, "Please Enter the Correct Account Number");
                return View(vm);
            }
        }
        [HttpGet]
        public ActionResult TopUpAccountEnterAmount()
        {
            var SenderTopUpAnAccount = _topUpToSupplierServices.GetSenderTopUpSupplierLocal();
            var senderCountry = Common.FaxerSession.LoggedUser.CountryCode;

            var supplierDetails = _sSuppliersServices.List().Data.Where(x => x.KiiPayBusinessInformation.BusinessMobileNo == SenderTopUpAnAccount.WalletNo).FirstOrDefault();
            SenderTopUpSupplierAbroadAbroadEnterAmontVM vm = new SenderTopUpSupplierAbroadAbroadEnterAmontVM()
            {
                ExchangeRate = Common.Common.GetExchangeRate(senderCountry, supplierDetails.Country),
                ReceiverName = supplierDetails.KiiPayBusinessInformation.BusinessName,
                ReceiverAccountNo = supplierDetails.RefCode,
                SendingCurrencySymbol = Common.Common.GetCurrencySymbol(senderCountry),
                ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(supplierDetails.Country),
                SendingCurrency = Common.Common.GetCountryCurrency(senderCountry),
                ReceivingCurrency = Common.Common.GetCountryCurrency(supplierDetails.Country),
                PhotoUrl = "",


            };
            _topUpToSupplierServices.SetSenderTopUpSupplierAbroadAbroadEnterAmont(vm);
            return View(vm);
        }
        [HttpPost]
        public ActionResult TopUpAccountEnterAmount([Bind(Include = SenderTopUpSupplierAbroadAbroadEnterAmontVM.BindProperty)]SenderTopUpSupplierAbroadAbroadEnterAmontVM vm)
        {
            var SenderTopUpAnAccount = _topUpToSupplierServices.GetSenderTopUpSupplierLocal();
            var supplierDetails = _sSuppliersServices.List().Data.Where(x => x.KiiPayBusinessInformation.BusinessMobileNo == SenderTopUpAnAccount.WalletNo).FirstOrDefault();

            var result = _topUpToSupplierServices.GetSenderTopUpSupplierAbroadAbroadEnterAmont();
            result.SendingAmount = vm.SendingAmount;
            result.ReceivingAmount = vm.SendingAmount;
            result.TotalAmount = vm.SendingAmount;
            _topUpToSupplierServices.SetSenderTopUpSupplierAbroadAbroadEnterAmont(result);
            if (result != null)
            {
                TopUpToSupplier addTopUp = new TopUpToSupplier()
                {
                    PayerId = Common.FaxerSession.LoggedUser.Id,
                    EcxhangeRate = result.ExchangeRate,
                    PaymentDate = DateTime.Now,
                    Fee = result.Fee,
                    PayingCountry = Common.Common.GetCountryName(Common.FaxerSession.LoggedUser.CountryCode),
                    PaymentModule = Module.Faxer,
                    PaymentType = PaymentType.Local,
                    ReceivingAmount = vm.SendingAmount,
                    SendingAmount = vm.SendingAmount,
                    SuplierId = supplierDetails.KiiPayBusinessInformation.Id,
                    SupplierAccountNo = result.ReceiverAccountNo,
                    SupplierCountry = Common.Common.GetCountryName(supplierDetails.Country),
                    TotalAmount = vm.SendingAmount,
                    WalletNo = SenderTopUpAnAccount.WalletNo,
                    
                };
                _topUpToSupplierServices.Add(addTopUp);
            }
            else
            {
                return View(vm);
            }

            if (vm.SetStandingOrderPayment == true)
            {
                if (vm.Amount > 0)
                {
                    SupplierStandingOrderPayment standingOrderPayment = new SupplierStandingOrderPayment()
                    {
                        FrequenncyDetails = result.FrequencyDetails,
                        Amount = vm.Amount,
                        PayerId = Common.FaxerSession.LoggedUser.Id,
                        PaymentFrequency = vm.PaymentFrequencyId,
                        SupplierId = supplierDetails.KiiPayBusinessInformation.Id
                    };
                    _topUpToSupplierServices.AddSupplierStandingOrderPayment(standingOrderPayment);
                }
                else
                {
                    ModelState.AddModelError(vm.Amount.ToString(), "Please enter the amount");
                    return View(vm);
                }
            }

            return RedirectToAction("TopUpAccountAbroadSuccess", "SenderPayBills");





        }

        public JsonResult GetPaymentSummary(decimal SendingAmount, decimal ReceivingAmount)
        {
            bool IsReceivingAmount = false;
            var enterAmountData = _topUpToSupplierServices.GetSenderTopUpSupplierAbroadAbroadEnterAmont();
            var loggedInSenderData = Common.FaxerSession.LoggedUser;
            if ((SendingAmount > 0 && ReceivingAmount > 0) && enterAmountData.ReceivingAmount != ReceivingAmount)
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
                enterAmountData.ExchangeRate, SEstimateFee.GetFaxingCommision(loggedInSenderData.CountryCode));

            // Rewrite session with additional value 
            enterAmountData.Fee = result.FaxingFee;
            enterAmountData.SendingAmount = result.FaxingAmount;
            enterAmountData.ReceivingAmount = result.ReceivingAmount;
            enterAmountData.TotalAmount = result.TotalAmount;

            _topUpToSupplierServices.SetSenderTopUpSupplierAbroadAbroadEnterAmont(enterAmountData);
            return Json(new
            {
                Fee = result.FaxingFee,
                TotalAmount = result.TotalAmount,
                ReceivingAmount = result.ReceivingAmount,
                SendingAmount = result.FaxingAmount

            }, JsonRequestBehavior.AllowGet);

        }
        #endregion
    }
}