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
    public class SenderKiiPayWalletStandingOrdersController : Controller
    {
        // GET: SenderKiiPayWalletStandingOrders
        SOtherMFTCCardAutoTopUpInformation _SOtherMFTCCardAutoTopUpInformation = null;
        public SenderKiiPayWalletStandingOrdersController()
        {
            _SOtherMFTCCardAutoTopUpInformation = new SOtherMFTCCardAutoTopUpInformation();
        }
        public ActionResult SenderKiiPayWalletStandingOrdersIndex(string MobileNo = "")
        {
            var SenderId = FaxerSession.LoggedUser.Id;
            FaxerSession.MobileNo = MobileNo;
            List<SenderKiiPayWalletStandingOrdersViewModel> vmList = new List<SenderKiiPayWalletStandingOrdersViewModel>();
            if (MobileNo == "")
            {
                ViewBag.MobileNo = new SelectList(GetMobileNo(), "MobileNo", "MobileNo");
            }
            else
            {
                ViewBag.MobileNo = new SelectList(GetMobileNo(), "MobileNo", "MobileNo", MobileNo);

                var KiiPayPersonalWalletdetails = _SOtherMFTCCardAutoTopUpInformation.ListofKiiPayPersonalWallet().Data.Where(x => x.MobileNo == MobileNo).FirstOrDefault();
                if (KiiPayPersonalWalletdetails != null)
                {
                    var data = _SOtherMFTCCardAutoTopUpInformation.List().Data.Where(x => x.MFTCCardId == KiiPayPersonalWalletdetails.Id && x.FaxerId == SenderId).ToList();

                    if (data.Count > 0)
                    {
                        vmList = (from c in data
                                  select new SenderKiiPayWalletStandingOrdersViewModel()
                                  {
                                      AutoAmount = c.AutoPaymentAmount,
                                      City = c.MFTCCard.CardUserCity,
                                      Country = c.MFTCCard.CardUserCountry,
                                      Id = c.Id,
                                      WalletName = c.MFTCCard.FirstName + " " + c.MFTCCard.MiddleName + " " + c.MFTCCard.LastName,
                                      FrequencyDetails = c.FrequencyDetails,
                                      AutoTopUp = c.EnableAutoPayment == true ? "Yes" : "No",
                                      MobileNo = c.MFTCCard.MobileNo
                                  }).ToList();

                        foreach (var freqencydetails in vmList)
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
                    }
                    else
                    {
                        vmList = (from c in _SOtherMFTCCardAutoTopUpInformation.ListofKiiPayPersonalWallet().Data.Where(x => x.Id == KiiPayPersonalWalletdetails.Id).ToList()
                                  select new SenderKiiPayWalletStandingOrdersViewModel()
                                  {
                                      AutoAmount = c.AutoTopUpAmount,
                                      City = c.CardUserCity,
                                      Country = c.CardUserCountry,
                                      Id = c.Id,
                                      WalletName = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                                      FrequencyDetails = "",
                                      AutoTopUp = "No",
                                      MobileNo = c.MobileNo
                                  }).ToList();

                    }

                }

            }
            return View(vmList);
        }

        public List<MobileNumberDropDown> GetMobileNo()
        {

            var list = (from c in _SOtherMFTCCardAutoTopUpInformation.ListofKiiPayPersonalWallet().Data.Where(x => x.IsDeleted == false).ToList()
                        select new MobileNumberDropDown()
                        {

                            Id = c.Id,
                            MobileNo = c.MobileNo,
                        }).ToList();


            return list;
        }
        [HttpGet]
        public ActionResult SenderAutoPaymentAddKiiPayWallet(string MobileNo)
        {
            Models.SenderAutoPaymentAddViewModel model = new SenderAutoPaymentAddViewModel();
            FaxerSession.MobileNo = MobileNo;
            var walletInfo = _SOtherMFTCCardAutoTopUpInformation.ListofKiiPayPersonalWallet().Data.Where(x => x.MobileNo == MobileNo).FirstOrDefault();
            var data = _SOtherMFTCCardAutoTopUpInformation.List().Data.Where(x => x.FaxerId == Common.FaxerSession.LoggedUser.Id && x.MFTCCard.MobileNo == MobileNo).FirstOrDefault();

            if (data != null)
            { 
                model.Amount = data.AutoPaymentAmount;
                model.PaymentFrequency = data.AutoPaymentFrequency;
                model.FrequencyDetails = data.FrequencyDetails;
                
            }
            model.SendingCurrencySymbol = Common.Common.GetCurrencySymbol(walletInfo.CardUserCountry);
            model.Currency = Common.Common.GetCountryCurrency(walletInfo.CardUserCountry);
            model.Availablebalance = walletInfo.CurrentBalance;
            ViewBag.FaxerCurrency = Common.Common.GetCountryCurrency(walletInfo.CardUserCountry);
            _SOtherMFTCCardAutoTopUpInformation.SetSenderAddKiiPayStandingOrder(model);
            return View(model);

        }
        [HttpPost]
        public ActionResult SenderAutoPaymentAddKiiPayWallet([Bind(Include = SenderAutoPaymentAddViewModel.BindProperty)]SenderAutoPaymentAddViewModel vm)
        {
            var walletInfo = _SOtherMFTCCardAutoTopUpInformation.ListofKiiPayPersonalWallet().Data.Where(x => x.MobileNo == vm.MobileNo).FirstOrDefault();
            ViewBag.FaxerCurrency = Common.Common.GetCountryCurrency(walletInfo.CardUserCountry);
            if (vm.Amount <= 0)
            {
                ModelState.AddModelError("AutoPaymentAmount", "Please enter amount greater than 0");
                return View(vm);
            }
            if (vm.PaymentFrequency == AutoPaymentFrequency.NoLimitSet)
            {
                ModelState.AddModelError("AutoPaymentFrequency", "Please select a payment frequency");
                return View(vm);

            }
            var senderId = FaxerSession.LoggedUser.Id;
            var data = _SOtherMFTCCardAutoTopUpInformation.List().Data.Where(x => x.FaxerId == Common.FaxerSession.LoggedUser.Id && x.MFTCCard.MobileNo == vm.MobileNo).FirstOrDefault();
            if (data == null)
            {
                OtherMFTCCardAutoTopUpInformation autoTopUpInformation = new OtherMFTCCardAutoTopUpInformation()
                {

                    AutoPaymentAmount = vm.Amount,
                    AutoPaymentFrequency = vm.PaymentFrequency,
                    EnableAutoPayment = true,
                    FaxerId = Common.FaxerSession.LoggedUser.Id,
                    FrequencyDetails = vm.FrequencyDetails,
                    MFTCCardId = walletInfo.Id,


                };
                vm.Availablebalance = walletInfo.CurrentBalance;
                _SOtherMFTCCardAutoTopUpInformation.SetSenderAddKiiPayStandingOrder(vm);
                _SOtherMFTCCardAutoTopUpInformation.Add(autoTopUpInformation);
            }

         


            return RedirectToAction("AddKiiPayWalletStandingOrderSuccess");
        }
        public ActionResult AddKiiPayWalletStandingOrderSuccess()

        {
            var model=_SOtherMFTCCardAutoTopUpInformation.GetSenderAddKiiPayStandingOrder();
            SenderAutoPaymentAddViewModel Vm = new SenderAutoPaymentAddViewModel()
            {
                Amount = model.Amount,
                Availablebalance=model.Availablebalance

            };
            return View(Vm);
        }
 
        public ActionResult DeleteSenderAutoPaymentKiiPayWallet(int Id)
        {

            var data = _SOtherMFTCCardAutoTopUpInformation.List().Data.Where(x => x.Id == Id).FirstOrDefault();
            data.EnableAutoPayment = false;
            data.AutoPaymentAmount = 0;
            data.AutoPaymentFrequency = 0;
            _SOtherMFTCCardAutoTopUpInformation.Update(data);
            return RedirectToAction("SenderKiiPayWalletStandingOrdersIndex");

        }
        public ActionResult SenderAutoPaymentUpdateKiiPayWallet(string MobileNo)
        {
            Models.SenderAutoPaymentAddViewModel model = new SenderAutoPaymentAddViewModel();
            FaxerSession.MobileNo = MobileNo;
            var walletInfo = _SOtherMFTCCardAutoTopUpInformation.ListofKiiPayPersonalWallet().Data.Where(x => x.MobileNo == MobileNo).FirstOrDefault();
            var data = _SOtherMFTCCardAutoTopUpInformation.List().Data.Where(x => x.FaxerId == Common.FaxerSession.LoggedUser.Id && x.MFTCCard.MobileNo == MobileNo).FirstOrDefault();

            if (data != null)
            {
                model.Amount = data.AutoPaymentAmount;
                model.PaymentFrequency = data.AutoPaymentFrequency;
                model.FrequencyDetails = data.FrequencyDetails;

            }
            model.SendingCurrencySymbol = Common.Common.GetCurrencySymbol(walletInfo.CardUserCountry);
            model.Currency = Common.Common.GetCountryCurrency(walletInfo.CardUserCountry);
            model.Availablebalance = walletInfo.CurrentBalance;
            ViewBag.FaxerCurrency = Common.Common.GetCountryCurrency(walletInfo.CardUserCountry);
            _SOtherMFTCCardAutoTopUpInformation.SetSenderAddKiiPayStandingOrder(model);
            return View(model);
        }
        [HttpPost]
        public ActionResult SenderAutoPaymentUpdateKiiPayWallet([Bind(Include = SenderAutoPaymentAddViewModel.BindProperty)]SenderAutoPaymentAddViewModel vm)
        {
            var walletInfo = _SOtherMFTCCardAutoTopUpInformation.ListofKiiPayPersonalWallet().Data.Where(x => x.MobileNo == vm.MobileNo).FirstOrDefault();
            ViewBag.FaxerCurrency = Common.Common.GetCountryCurrency(walletInfo.CardUserCountry);
            if (vm.Amount <= 0)
            {
                ModelState.AddModelError("AutoPaymentAmount", "Please enter amount greater than 0");
                return View(vm);
            }
            if (vm.PaymentFrequency == AutoPaymentFrequency.NoLimitSet)
            {
                ModelState.AddModelError("AutoPaymentFrequency", "Please select a payment frequency");
                return View(vm);

            }

            int SenderId = Common.FaxerSession.LoggedUser.Id;
            var data = _SOtherMFTCCardAutoTopUpInformation.List().Data.Where(x => x.MFTCCard.MobileNo == vm.MobileNo && x.FaxerId == SenderId).FirstOrDefault();


            if (data != null)
            {

                data.AutoPaymentAmount = vm.Amount;
                data.AutoPaymentFrequency = vm.PaymentFrequency;
                data.EnableAutoPayment = true;
                data.FrequencyDetails = vm.FrequencyDetails;
                _SOtherMFTCCardAutoTopUpInformation.Update(data);


            }
           
                 vm.Availablebalance = walletInfo.CurrentBalance;
                _SOtherMFTCCardAutoTopUpInformation.SetSenderAddKiiPayStandingOrder(vm);

            return RedirectToAction("UpdateKiiPayWalletStandingOrderSuccess");
        }
    

        public ActionResult UpdateKiiPayWalletStandingOrderSuccess()
        {
            var model = _SOtherMFTCCardAutoTopUpInformation.GetSenderAddKiiPayStandingOrder();
            SenderAutoPaymentAddViewModel Vm = new SenderAutoPaymentAddViewModel()
            {
                
                Availablebalance = model.Availablebalance

            };
            return View(Vm);
        }


    }
}