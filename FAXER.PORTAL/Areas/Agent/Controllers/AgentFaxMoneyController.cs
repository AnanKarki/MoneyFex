using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{

    public class AgentFaxMoneyController : Controller
    {

        Admin.Services.CommonServices common = new Admin.Services.CommonServices();

        // GET: Agent/FaxMoney
        [Route("/agent/agentfaxmoney")]
        [HttpGet]
        public ActionResult Index(string AccountNoORPhoneNo = "")
        {
            //Session.Remove("FirstLogin");
            AgentInformation agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            AgentResult agentResult = new AgentResult();
            string agencyName = agentInfo.Name;
            string agencyMFSCode = agentInfo.AccountNo;
            int agentId = agentInfo.Id;
            if (agentId == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }

            var countries = common.GetCountries();
            ViewBag.countries = new SelectList(countries, "Code", "Name");
            var identifyCardType = common.GetCardType();
            ViewBag.IDTypes = new SelectList(identifyCardType, "CardType", "CardType");

            AgentServices.AgentFaxMoneyServices agentservices = new AgentServices.AgentFaxMoneyServices();
            Models.AgentFaxMoneyViewModel vm = new AgentFaxMoneyViewModel();
            vm.NameOfAgency = agencyName;
            vm.AgencyMFSCode = agencyMFSCode;
            vm.AgentId = agentId;
            vm.DateTime = DateTime.Now;
            vm.PayingAgentName = Common.AgentSession.LoggedUser.PayingAgentStaffName;
            // Get faxer information by Faxer account no
            if (AccountNoORPhoneNo != "")
            {

                var result = agentservices.getFaxer(AccountNoORPhoneNo, vm);
                if (result != null)
                {
                    if (result.FaxerId != 0)
                    {
                        var existingReceiver = agentservices.getExistingReceiver(result.FaxerId);
                        ViewBag.existingReceiver = new SelectList(existingReceiver, "Id", "FirstName");
                        ViewBag.AgentResult = new AgentResult();
                        result.FaxingCountry = agentInfo.CountryCode;
                        return View(result);
                    }
                }
                else
                {
                    agentResult.Message = "The Faxer Account does not exist";
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }


            }


            //ViewBag.AgentResult = new AgentResult();

            vm.FaxingCountry = agentInfo.CountryCode;
            ViewBag.AgentResult = agentResult;
            return View(vm);

        }

        //public ActionResult getReceiverDetails(int receiverId) 
        //{


        //        AgentServices.AgentFaxMoneyServices agentservices = new AgentServices.AgentFaxMoneyServices();

        //        var result = agentservices.getReceiverDetails(receiverId);
        //        var existingReceiver = agentservices.getExistingReceiver(result.FaxerId);
        //        ViewBag.existingReceiver = new SelectList(existingReceiver, "Id", "FirstName");
        //        ViewBag.AgentResult = new AgentResult();
        //        return RedirectToAction("Index",result);

        //}

        //Get faxer details 

        [HttpGet ]
        
        public JsonResult getCurrency(string CountryCode)
        {

            return Json(new
            {

                Currency = Common.Common.GetCurrencySymbol(CountryCode)
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = AgentFaxMoneyViewModel.BindProperty)] AgentFaxMoneyViewModel vm)
        {

            AgentResult agentResult = new AgentResult();
            var countries = common.GetCountries();
            ViewBag.countries = new SelectList(countries, "Code", "Name");
            AgentServices.AgentFaxMoneyServices agentservices = new AgentServices.AgentFaxMoneyServices();
            var identifyCardType = common.GetCardType();
            ViewBag.IDTypes = new SelectList(identifyCardType, "CardType", "CardType");

            var existingReceiver = agentservices.getExistingReceiver(vm.FaxerId);
            ViewBag.existingReceiver = new SelectList(existingReceiver, "Id", "FirstName");

            //get Faxing Amount Detials if Details does not exist 


            if (ModelState.IsValid)
            {

                if (vm.FaxedAmount == 0)
                {

                    ModelState.AddModelError("FaxedAmount", "Please Enter Sending Amount ");
                    ViewBag.AgentResult = agentResult;
                    return View(vm);

                }
                if (vm.RecevingAmount == 0)
                {

                    ModelState.AddModelError("RecevingAmount", "Please Enter Sending Amount ");
                    ViewBag.AgentResult = agentResult;
                    return View(vm);

                }
                if (string.IsNullOrEmpty(vm.FaxingFee))
                {

                    ModelState.AddModelError("FaxingFee", "Please calculate estimated fee ");
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
                if (vm.ReceiverFirstName == null)
                {

                    ModelState.AddModelError("ReceiverFirstName", "Please Enter Receiver First Name");
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
                if (vm.ReceiverLastName == null)
                {
                    ModelState.AddModelError("ReceiverLastName", "Please Enter Receiver Last Name");
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }

                if (vm.ReceiverCountry == null)
                {
                    ModelState.AddModelError("ReceiverCountry", "Please Select Receiver Country");
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
                if (vm.PayingAgentName == null)
                {
                    ModelState.AddModelError("PayingAgentName", "Please Enter Paying Agent Name");
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
                if (vm.FaxerEmail != null)
                {
                    if (vm.FaxerSearched == false)
                    {

                        //var FaxerEmailExist = agentservices.getFaxerEmail(vm.FaxerEmail);
                        if (vm.FaxingCountry != vm.FaxerCountry)
                        {
                            agentResult.Message = "The transaction can't be proceeded because the sender country and agent country are different to each other";
                            agentResult.Status = ResultStatus.Warning;
                            ViewBag.AgentResult = agentResult;
                            return View(vm);
                        }
                    }
                }
                if (vm.IdCardExpiringDate < DateTime.Now)
                {
                    agentResult.Message = "Sender's Identity card has been expired.";
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }


                if (!vm.IsConfirmed)
                {
                    agentResult.Message = "Confirmation for the information is required to either pay or rejection this transaction has been fully verified by yourself";
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }




                var nonCardTransaction = agentservices.FaxNonCardTransactionByAgent(vm);
                if (nonCardTransaction != null)
                {

                    agentResult.Message = "Payment Completed Successfully";
                    agentResult.Status = ResultStatus.OK;
                    ViewBag.AgentResult = agentResult;
                    ViewBag.AgentResult.Data = nonCardTransaction.MFCN;
                    ModelState.Clear();
                    var model = new Models.AgentFaxMoneyViewModel();
                    return View(model);
                }
                else
                {
                    agentResult.Message = "Error Occured Wile Doing Transaction";
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
            }

            ViewBag.AgentResult = new AgentResult();
            return View(vm);



        }


        [HttpGet]
        public ActionResult GetPhoneCode(string countryCode)
        {
            var phoneCode = common.getPhoneCodeFromCountry(countryCode);
            return Json(new
            {
                ReceiverCountryPhoneCode = phoneCode
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetReceiverDetails(int id, string RecevingCountry)
        {
            AgentServices.AgentFaxMoneyServices agentservices = new AgentServices.AgentFaxMoneyServices();
            var receiverDetails = agentservices.getReceiverDetails(id);

            if (receiverDetails.Country.ToLower() != RecevingCountry.ToLower())
            {

                return Json(new
                {

                    InvalidReceivingCountry = true
                }, JsonRequestBehavior.AllowGet);

            }
            var CountryPhoneCode = Common.Common.GetCountryPhoneCode(receiverDetails.Country);
            return Json(new
            {
                ReceiverFirstName = receiverDetails.FirstName,
                ReceiverMiddleName = receiverDetails.MiddleName,
                ReceiverLastName = receiverDetails.LastName,
                ReceiverAddress = receiverDetails.City + " , " + receiverDetails.Country,
                ReceiverEmailAddress = receiverDetails.EmailAddress,
                ReceiverTelephone = receiverDetails.PhoneNumber,
                ReceiverCountry = receiverDetails.Country,
                ReceiverCity = receiverDetails.City,
                ReceiverSelected = true,
                ReceiverCurrency = common.getCurrencyCodeFromCountry(receiverDetails.Country),
                ReceiverCurrencySymbol = common.getCurrencySymbol(receiverDetails.Country),
                ReceiverCountryPhoneCode = common.getPhoneCodeFromCountry(receiverDetails.Country),

            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getCalculatedDetails(string FaxerCountry, string ReceiverCountry, Decimal FaxAmount, decimal ReceivingAmount)
        {
            //Decimal FaxAmount = 0;
            //string ReceiverCountry = "";
            AgentServices.AgentFaxMoneyServices agentservices = new AgentServices.AgentFaxMoneyServices();

            
            FaxerCountry = Common.AgentSession.AgentInformation.CountryCode;

            var result = agentservices.getCalculateDetails(FaxerCountry, ReceiverCountry, FaxAmount, ReceivingAmount);

            string ReceiverPhoneCode = Common.Common.GetCountryPhoneCode(ReceiverCountry);
            if (result != null)
            {
                return Json(new
                {
                    FaxedAmount = result.FaxingAmount,
                    FaxingFee = result.FaxingFee,
                    CurrentExchangeRate = result.ExchangeRate,
                    TotalAmountIncludingFee = Math.Round(result.TotalAmount),
                    RecevingAmount = result.ReceivingAmount,
                    ReceiverCurrency = common.getCurrencyCodeFromCountry(ReceiverCountry),
                    ReceiverCurrencySymbol = common.getCurrencySymbol(ReceiverCountry),
                    FaxingCurrency = common.getCurrencyCodeFromCountry(FaxerCountry),
                    FaxingCurrencySymbol = common.getCurrencySymbol(FaxerCountry),
                    NoExchangeRateSetup = false,
                    ReceiverPhoneCode = ReceiverPhoneCode


                }, JsonRequestBehavior.AllowGet);


            }
            else
            {

                return Json(new
                {
                    NoExchangeRateSetup = true,
                    FaxedAmount = 0,
                    FaxingFee = "",
                    CurrentExchangeRate = "",
                    TotalAmountIncludingFee = "",
                    RecevingAmount = 0,

                }, JsonRequestBehavior.AllowGet);
            }
        }
      


    }




}
