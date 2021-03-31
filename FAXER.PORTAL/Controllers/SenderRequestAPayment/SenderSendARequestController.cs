using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.SenderRequestAPayment
{
    public class SenderSendARequestController : Controller
    {

        SSenderSendARequest _sendARequestServices = null;
        public SenderSendARequestController()
        {
            _sendARequestServices = new SSenderSendARequest();
        }
        // GET: SenderSendARequest

        #region International
        public ActionResult Index()
        {
            GetRecentNumbersInternational();
            GetCountriesDropDown();
            SenderSendARequestVM vm = new SenderSendARequestVM()
            {
                CountryPhoneCode = ""
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult Index( [Bind(Include = SenderSendARequestVM.BindProperty)]SenderSendARequestVM model)
        {
            GetRecentNumbersInternational();
            GetCountriesDropDown();
            if (ModelState.IsValid)
            {
                _sendARequestServices.SetSendARequest(model);


                //SetData in EnterAmount Session

                var loggedInSenderData = _sendARequestServices.GetLoggedUserData();
                var receiverInfo = _sendARequestServices.GetReceiverInformationFromMobileNumber(model.MobileNumber);
                if (receiverInfo.MobileNo == null)
                {
                    ModelState.AddModelError("MobileNo", "Enter valid mobile no");

                    return View(model);
                }
                SenderMobileEnrterAmountVm enterAmountData = new SenderMobileEnrterAmountVm()
                {
                    ReceiverName = receiverInfo.FirstName + " " + receiverInfo.MiddleName + " " + receiverInfo.LastName,
                    ReceiverId = receiverInfo.Id,
                    SendingCurrencySymbol = Common.Common.GetCurrencySymbol(loggedInSenderData.CountryCode),
                    SendingCurrencyCode = Common.Common.GetCountryCurrency(loggedInSenderData.CountryCode),

                    //Use receivers country code later

                    ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(receiverInfo.CardUserCountry),
                    ReceivingCurrencyCode = Common.Common.GetCountryCurrency(receiverInfo.CardUserCountry),
                    ExchangeRate = Common.Common.GetExchangeRate(loggedInSenderData.CountryCode, receiverInfo.CardUserCountry)
                };
                _sendARequestServices.SetSendRequestEnterAmount(enterAmountData);
                return RedirectToAction("SendRequestEnterAmount", "SenderSendARequest");
            }
            return View(model);
        }


        public ActionResult SendRequestEnterAmount()
        {
            var enterAmountData = _sendARequestServices.GetSendRequestEnterAmount();
            SenderMobileEnrterAmountVm vm = new SenderMobileEnrterAmountVm()
            {
                ReceiverName = enterAmountData.ReceiverName,
                ReceiverId = enterAmountData.ReceiverId,
                SendingCurrencySymbol = enterAmountData.SendingCurrencySymbol,
                SendingCurrencyCode = enterAmountData.SendingCurrencyCode,
                ReceivingCurrencySymbol = enterAmountData.ReceivingCurrencySymbol,
                ReceivingCurrencyCode = enterAmountData.ReceivingCurrencyCode,
                ExchangeRate = enterAmountData.ExchangeRate,
                PaymentReference = enterAmountData.PaymentReference
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult SendRequestEnterAmount([Bind(Include = SenderMobileEnrterAmountVm.BindProperty)]SenderMobileEnrterAmountVm model)
        {
            if (ModelState.IsValid)
            {
                _sendARequestServices.SetSendRequestEnterAmount(model);

                var result = _sendARequestServices.MakePaymentRequest(PaymentType.International);
                if (result == true)
                {
                    return RedirectToAction("SendARequestSuccess", "SenderSendARequest");
                }

            }
            return View(model);
        }


        public ActionResult SendARequestSuccess()
        {
            var requestingAmountData = _sendARequestServices.GetSendRequestEnterAmount();
            SenderCashPickUpSuccessVm vm = new SenderCashPickUpSuccessVm()
            {
                ReceiverName = requestingAmountData.ReceiverName,
                SentAmount = requestingAmountData.SendingAmount,
                SendingCurrency = requestingAmountData.SendingCurrencySymbol
            };
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditInternationalRequest(int id)
        {
            var vm = _sendARequestServices.GetSendRequestEnterAmountInternationalForEdit(id);
            if (vm == null)
            {
                return RedirectToAction("RequestHistory");
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditInternationalRequest([Bind(Include = SenderMobileEnrterAmountVm.BindProperty)]SenderMobileEnrterAmountVm model)
        {
            if (ModelState.IsValid)
            {
                if (model.ReceivingAmount == 0)
                {
                    ModelState.AddModelError("ReceivingAmount", "Invalid Amount !");
                    return View(model);
                }
                if (model.SendingAmount == 0)
                {
                    ModelState.AddModelError("SendingAmount", "Invalid Amount !");
                    return View(model);
                }

                var data = _sendARequestServices.List().Where(x => x.Id == model.Id).FirstOrDefault();
                if (data != null)
                {
                    data.RequestSendingAmount = model.SendingAmount;
                    data.RequestReceivingAmount = model.ReceivingAmount;
                    data.TotalAmount = model.TotalAmount;
                    data.ExchangeRate = model.ExchangeRate;
                    data.RequestNote = model.PaymentReference;

                    var updateData = _sendARequestServices.Update(data);
                    return RedirectToAction("Index", "SenderRequestHiistory");
                }
           
            }
            return View(model);
        }

        #endregion

        #region Local
        public ActionResult SendARequestLocal()
        {
            GetRecentNumbers();
            return View();
        }

        [HttpPost]
        public ActionResult SendARequestLocal([Bind(Include = SenderSendARequestVM.BindProperty)]SenderSendARequestVM model)
        {
            GetRecentNumbers();
            if (ModelState.IsValid)
            {
                _sendARequestServices.SetSendARequest(model);

                //for enter amount Page
                var loggedInSenderData = _sendARequestServices.GetLoggedUserData();
                var receiverInformation = _sendARequestServices.GetReceiverInformationFromMobileNumber(model.MobileNumber);
                SenderLocalEnterAmountVM localsendRequestEnterAmount = new SenderLocalEnterAmountVM()
                {
                    ReceiverName = receiverInformation.FirstName + " " + receiverInformation.MiddleName + " " + receiverInformation.LastName,
                    CurrencySymbol = Common.Common.GetCurrencySymbol(loggedInSenderData.CountryCode),
                    CurrencyCode = Common.Common.GetCountryCurrency(loggedInSenderData.CountryCode),
                    ReceiverId = receiverInformation.Id,
                    ReceiverImage = receiverInformation.UserPhoto,
                };

                _sendARequestServices.SetSendRequestEnterAmountLocal(localsendRequestEnterAmount);
                return RedirectToAction("SendRequestEnterAmountLocal", "SenderSendARequest");
            }
            return View(model);
        }


        public ActionResult SendRequestEnterAmountLocal()
        {
            var loggedUserData = _sendARequestServices.GetLoggedUserData();
            var localRequestAmount = _sendARequestServices.GetSendRequestEnterAmountLocal();

            SenderLocalEnterAmountVM vm = new SenderLocalEnterAmountVM()
            {
                CurrencySymbol = Common.Common.GetCurrencySymbol(loggedUserData.CountryCode),
                CurrencyCode = Common.Common.GetCountryCurrency(loggedUserData.CountryCode),
                ReceiverImage = localRequestAmount.ReceiverImage,
                ReceiverName = localRequestAmount.ReceiverName,
                ReceiverId = localRequestAmount.ReceiverId
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult SendRequestEnterAmountLocal([Bind(Include = SenderLocalEnterAmountVM.BindProperty)]SenderLocalEnterAmountVM model)
        {
            if (ModelState.IsValid)
            {
                _sendARequestServices.SetSendRequestEnterAmountLocal(model);
                var result = _sendARequestServices.MakePaymentRequest(PaymentType.Local);
                return RedirectToAction("SendARequestSuccessLocal", "SenderSendARequest");
            }
            return View(model);
        }
        public ActionResult SendARequestSuccessLocal()
        {
            var requestingAmountData = _sendARequestServices.GetSendRequestEnterAmountLocal();
            SenderCashPickUpSuccessVm vm = new SenderCashPickUpSuccessVm()
            {
                ReceiverName = requestingAmountData.ReceiverName,
                SentAmount = requestingAmountData.Amount,
                SendingCurrency = requestingAmountData.CurrencySymbol
            };
            return View(vm);
        }

        public ActionResult EditLocalRequest(int id)
        {
            var vm = _sendARequestServices.GetSendRequestEnterAmountLocalForEdit(id);

            if (vm == null)
            {
                return RedirectToAction("Index", "SenderRequestHiistory");
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditLocalRequest([Bind(Include = SenderLocalEnterAmountVM.BindProperty)]SenderLocalEnterAmountVM model)
        {
            if (ModelState.IsValid)
            {
                if (model.Amount == 0)
                {
                    ModelState.AddModelError("Amount", "Invalid Amount !");
                    return View(model);
                }

                var data = _sendARequestServices.List().Where(x => x.Id == model.Id).FirstOrDefault();
                if(data != null)
                {
                    data.RequestSendingAmount = model.Amount;
                    data.RequestReceivingAmount = model.Amount;
                    data.TotalAmount = model.Amount;
                    var updateData = _sendARequestServices.Update(data);
                    return RedirectToAction("Index", "SenderRequestHiistory");
                }

            }
            return View(model);
        }
        #endregion

        private void GetRecentNumbers()
        {
            var recentNumbers = _sendARequestServices.GetRecentNumbers().ToList();
            ViewBag.RecentMobileNumbers = new SelectList(recentNumbers, "Name", "Name");
        }
        private void GetRecentNumbersInternational()
        {

            var recentNumbers = _sendARequestServices.GetRecentNumbersInternational().ToList();
            ViewBag.RecentMobileNumbers = new SelectList(recentNumbers, "Name", "Name");
        }

        private void GetCountriesDropDown(string Country = "")
        {
            ViewBag.Countries = new SelectList(Common.Common.GetCountriesForDropDown()
               , "CountryCode", "CountryName", Country);
        }

        public JsonResult GetPhoneCode(string countryCode)
        {
            var phoneCode = Common.Common.GetCountryPhoneCode(countryCode);
            return Json(new
            {
                PhoneCode = phoneCode
            }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetPaymentSummary(decimal SendingAmount, decimal ReceivingAmount)
        {
            bool IsReceivingAmount = false;
            var enterAmountData = _sendARequestServices.GetSendRequestEnterAmount();
            var loggedInSenderData = _sendARequestServices.GetLoggedUserData();
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

            _sendARequestServices.SetSendRequestEnterAmount(enterAmountData);
            return Json(new
            {
                Fee = result.FaxingFee,
                TotalAmount = result.TotalAmount,
                ReceivingAmount = result.ReceivingAmount,
                SendingAmount = result.FaxingAmount
            }, JsonRequestBehavior.AllowGet);
        }

    }
}