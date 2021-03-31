using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class SenderBusinesStandingOrdersController : Controller
    {
        // GET: SenderBusinesStandingOrders

        SSenderSendingBusinessOrderServices _senderSendingBusinessOrderServices = null;


        public SenderBusinesStandingOrdersController()
        {
            _senderSendingBusinessOrderServices = new SSenderSendingBusinessOrderServices();
        }

        public ActionResult SenderSendingBusinessOrdersIndex(string MobileNo = "")
        {

            //List<SenderKiiPayBusinessPaymentInformation> lst = context.FaxerMerchantPaymentInformation.Where(x => x.SenderInformationId == SenderId).ToList();
            var lst = _senderSendingBusinessOrderServices.GetList();
            var list = _senderSendingBusinessOrderServices.GetMobileNumber();
            List<SenderSendingBusinessOrdersViewModel> gridLst = new List<SenderSendingBusinessOrdersViewModel>();

            if (MobileNo == "")
            {
                ViewBag.MobileNo = new SelectList(list, "MobileNo", "MobileNo");
            }
            else
            {
                ViewBag.MobileNo = new SelectList(list, "MobileNo", "MobileNo", MobileNo);
                gridLst = _senderSendingBusinessOrderServices.GetGridList(MobileNo);
                foreach (var freqencydetails in gridLst)
                {

                    var paymentDay = Convert.ToInt32(freqencydetails.FrequencyDetails);
                    if (freqencydetails.Frequency == AutoPaymentFrequency.Weekly)
                    {
                        freqencydetails.FrequencyDetails = Enum.GetName(typeof(DayOfWeek), paymentDay) + " every Week";
                    }
                    else if (freqencydetails.Frequency == AutoPaymentFrequency.Monthly)
                    {
                        string abbreviation = "";
                        if (paymentDay == 01 || paymentDay == 21 || paymentDay == 31)
                        {

                            abbreviation = "st";
                        }
                        else if (paymentDay == 02 || paymentDay == 22)
                        {
                            abbreviation = "nd";
                        }
                        else if (paymentDay == 03 || paymentDay == 23)
                        {
                            abbreviation = "rd";
                        }
                        else
                        {
                            abbreviation = "th";
                        }

                        freqencydetails.FrequencyDetails = paymentDay + abbreviation + " of the every Month";
                    }
                    else if (freqencydetails.Frequency == AutoPaymentFrequency.Yearly)
                    {
                        string PaymentDate = freqencydetails.FrequencyDetails;
                        int Month = int.Parse(PaymentDate.Substring(0, 2));
                        int Day = int.Parse(PaymentDate.Substring(2, 2));
                        string MonthName = Enum.GetName(typeof(Month), Month);
                        string abbreviation = "";
                        if (Day == 01 || Day == 21 || Day == 31)
                        {

                            abbreviation = "st";
                        }
                        else if (Day == 02 || Day == 22)
                        {
                            abbreviation = "nd";
                        }
                        else if (Day == 03 || Day == 23)
                        {
                            abbreviation = "rd";
                        }
                        else
                        {
                            abbreviation = "th";
                        }
                        freqencydetails.FrequencyDetails = MonthName + " " + Day + abbreviation + " of  the every Year";

                    }
                    else
                    {
                        freqencydetails.FrequencyDetails = "None";
                    }
                }
                FaxerSession.MobileNo = MobileNo;
            }
            return View(gridLst);
        }


        public ActionResult SenderAutoPaymentAdd()
        {
            SenderAutoPaymentAddViewModel model = new SenderAutoPaymentAddViewModel();
            model.Currency = Common.Common.GetCurrencySymbol(Common.FaxerSession.FaxerCountry);
            model.SendingCurrencySymbol = Common.Common.GetCountryCurrency(Common.FaxerSession.FaxerCountry);
            ViewBag.FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.FaxerCountry);
            //if (SavedCard == null)
            var savedCard = _senderSendingBusinessOrderServices.GetSavedCard();
            if (savedCard == null)
            {
                @TempData["CardCount"] = 0;
                return RedirectToAction("SenderSendingBusinessOrdersIndex");
                //ModelState.AddModelError("Error", "Please Add Creidt/Debit Card to Set AutoPayment for Merchant ");
                //return View();

            }
            var merchantPaymentInformation = _senderSendingBusinessOrderServices.GetList().Where(x => x.KiiPayBusinessInformation.BusinessMobileNo == FaxerSession.MobileNo).FirstOrDefault();
            if (merchantPaymentInformation.EnableAutoPayment == true)
            {
                model.Amount = merchantPaymentInformation.AutoPaymentAmount;
                model.PaymentFrequency = merchantPaymentInformation.AutoPaymentFrequency;
                model.SendingCurrencySymbol = ViewBag.FaxerCurrency;
                model.Currency = ViewBag.FaxerCurrency;
            }



            return View(model);
        }

        [HttpPost]
        public ActionResult SenderAutoPaymentAdd([Bind(Include = SenderAutoPaymentAddViewModel.BindProperty)]SenderAutoPaymentAddViewModel model)
        {
            model.Currency = Common.Common.GetCurrencySymbol(Common.FaxerSession.FaxerCountry);
            ViewBag.FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.FaxerCountry);
            if (model.PaymentFrequency != 0)
            {

                if (model.PaymentFrequency == AutoPaymentFrequency.NoLimitSet)
                {
                    ModelState.AddModelError("AutoPaymentFrequency", "Please select payment frequency");
                    return View(model);
                }
                var savedCard = _senderSendingBusinessOrderServices.GetSavedCard();
                if (savedCard == null)
                {
                    ModelState.AddModelError("Error", "Please Add Creidt/Debit Card to Set AutoPayment for Merchant ");
                    return View(model);
                }
                var merchantPaymentInformation = _senderSendingBusinessOrderServices.GetList().Where(x => x.KiiPayBusinessInformation.BusinessMobileNo == FaxerSession.MobileNo).FirstOrDefault();

                merchantPaymentInformation.AutoPaymentAmount = model.Amount;
                merchantPaymentInformation.AutoPaymentFrequency = model.PaymentFrequency;
                merchantPaymentInformation.EnableAutoPayment = true;
                merchantPaymentInformation.FrequencyDetails = model.FrequencyDetails;
                //merchantPaymentInformation.PaymentRefrence = model.PaymentReference;

                _senderSendingBusinessOrderServices.Update(merchantPaymentInformation);


                _senderSendingBusinessOrderServices.SetSenderAddKiiPayStandingOrder(model);
                return RedirectToAction("AddBusinessStandingOrderSuccess", "SenderBusinesStandingOrders");
            }
            else
            {
                ModelState.AddModelError("Error", "Auto payment Amount is Required.");
            }
            return View();
        }

        public ActionResult AddBusinessStandingOrderSuccess()
        {
            var model = _senderSendingBusinessOrderServices.GetSenderAddStandingOrder();
            SenderAutoPaymentAddViewModel Vm = new SenderAutoPaymentAddViewModel()
            {
                Amount = model.Amount,
                Availablebalance = model.Availablebalance

            };
            return View(Vm);
        }

        public ActionResult UpdateBusinessStandingOrder()
        {
            SenderAutoPaymentAddViewModel model = new SenderAutoPaymentAddViewModel();
            var data = _senderSendingBusinessOrderServices.GetList().Where(x => x.KiiPayBusinessInformation.BusinessMobileNo == FaxerSession.MobileNo).FirstOrDefault();
            if (data != null)
            {
                data.AutoPaymentAmount = model.Amount;
                data.AutoPaymentFrequency = model.PaymentFrequency;
                data.FrequencyDetails = model.FrequencyDetails;

            }
            model.Currency = Common.Common.GetCurrencySymbol(Common.FaxerSession.FaxerCountry);
            model.SendingCurrencySymbol = Common.Common.GetCountryCurrency(Common.FaxerSession.FaxerCountry);
            ViewBag.FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.FaxerCountry);
            return View(model);
        }
        [HttpPost]
        public ActionResult UpdateBusinessStandingOrder([Bind(Include = SenderAutoPaymentAddViewModel.BindProperty)]SenderAutoPaymentAddViewModel model)
        {
            model.Currency = Common.Common.GetCurrencySymbol(Common.FaxerSession.FaxerCountry);
            model.SendingCurrencySymbol = Common.Common.GetCountryCurrency(Common.FaxerSession.FaxerCountry);
            ViewBag.FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.FaxerCountry);
            if (model.Amount <= 0)
            {
                ModelState.AddModelError("AutoPaymentAmount", "Please enter amount greater than 0");
                return View(model);
            }
            if (model.PaymentFrequency == AutoPaymentFrequency.NoLimitSet)
            {
                ModelState.AddModelError("AutoPaymentFrequency", "Please select a payment frequency");
                return View(model);

            }
            var data = _senderSendingBusinessOrderServices.GetList().Where(x => x.KiiPayBusinessInformation.BusinessMobileNo == FaxerSession.MobileNo).FirstOrDefault();

            if (data != null)
            {

                data.AutoPaymentAmount = model.Amount;
                data.AutoPaymentFrequency = model.PaymentFrequency;
                data.EnableAutoPayment = true;
                data.FrequencyDetails = model.FrequencyDetails;
                _senderSendingBusinessOrderServices.Update(data);


            }


            return RedirectToAction("UpdateBusinessStandingOrderSuccess");

        }

        public ActionResult UpdateBusinessStandingOrderSuccess()
        {
            var model = _senderSendingBusinessOrderServices.GetSenderAddStandingOrder();
            SenderAutoPaymentAddViewModel Vm = new SenderAutoPaymentAddViewModel()
            {

                Availablebalance = model.Availablebalance

            };
            return View(Vm);
        }

        public ActionResult DeleteMerchantAutoPayments(int Id)
        {

            if (Id != 0)
            {
                var merchantPaymentInformation = _senderSendingBusinessOrderServices.GetList().Where(x => x.KiiPayBusinessInformation.Id == Id).FirstOrDefault();
                string AutoPaymentAmount = merchantPaymentInformation.AutoPaymentAmount.ToString();
                merchantPaymentInformation.AutoPaymentAmount = 0;
                merchantPaymentInformation.AutoPaymentFrequency = 0;
                merchantPaymentInformation.EnableAutoPayment = false;
                merchantPaymentInformation.FrequencyDetails = null;
                _senderSendingBusinessOrderServices.Update(merchantPaymentInformation);
                return RedirectToAction("SenderSendingBusinessOrdersIndex", "SenderBusinesStandingOrders");
            }
            return View();
        }



    }

}