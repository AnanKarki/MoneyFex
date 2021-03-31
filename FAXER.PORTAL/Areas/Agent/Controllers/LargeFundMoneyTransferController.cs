using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class LargeFundMoneyTransferController : Controller
    {
        LargeFundMoneyTransferServices _largeFundMoneyTransferServices = null;
        public LargeFundMoneyTransferController()
        {
            _largeFundMoneyTransferServices = new LargeFundMoneyTransferServices();
        }
        // GET: Agent/LargeFundMoneyTransfer
        [HttpGet]
        public ActionResult Index()
        {
            AgentResult agentResult = new AgentResult();

            agentResult.Message = "";
            ViewBag.AgentResult = agentResult;
            ViewBag.Countries = new SelectList(GetCountries(), "Code", "Name");
            return View();
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = LargeFundMoneyTransferFormVM.BindProperty)] LargeFundMoneyTransferFormVM vm)
        {


            AgentResult agentResult = new AgentResult();

            ViewBag.Countries = new SelectList(GetCountries(), "Code", "Name");
            if (ModelState.IsValid)
            {

                var DOB = vm.SenderDateOfBirth;
                var currentYear = DateTime.Now.Year;
                var DOBYear = DOB.Year;
                var senderAge = currentYear - DOBYear;

                if (senderAge <= 18)
                {
                    ModelState.AddModelError("InvalidAge", "Sender must be 18 years above");
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }

                if (vm.SenderIdExpiryDate < DateTime.Now)
                {
                    ModelState.AddModelError("SenderIdExpired", "Id Expired");
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
                if (vm.ReceiverIdCardExpiryDate < DateTime.Now)
                {
                    ModelState.AddModelError("ReceiverIdExpired", "Id Expired");
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
                if (string.IsNullOrEmpty(vm.DestinationCountryAndCity))
                {
                    ModelState.AddModelError("DestinationCountryAndCity", "Enter Destination and Country");
                    ViewBag.AgentResult = agentResult;
                    return View(vm);

                }
                else
                {
                    var country = vm.DestinationCountryAndCity.Split(' ');
                    if (country.Length < 2)
                    {
                        ModelState.AddModelError("DestinationCountryAndCity", "Enter Destination and Country");
                        ViewBag.AgentResult = agentResult;
                        return View(vm);
                    }

                    vm.SenderCountry = Common.Common.GetCountryCodeByCountryName(country[1]) ?? " ";


                }
                if (string.IsNullOrEmpty(vm.RelationshipBetnSenderAndReceiver))
                {
                    ModelState.AddModelError("RelationshipBetnSenderAndReceiver", "Enter The Relationship between sender and receiver. ");
                    ViewBag.AgentResult = agentResult;
                    return View(vm);

                }

                var data = ThirdPartyMoneyTransferModelBinding(vm);
                var result = _largeFundMoneyTransferServices.CreateLargeFundMoneyTransfer(data);
                agentResult.Message = "Form Submitted Successfully";
                agentResult.Status = ResultStatus.OK;
                ViewBag.AgentResult = agentResult;

            }
            ViewBag.AgentResult = agentResult;
            return View(vm);
        }
        private DB.LargeFundMoneyTransferFormData ThirdPartyMoneyTransferModelBinding(LargeFundMoneyTransferFormVM vm)
        {

            DB.LargeFundMoneyTransferFormData model = new DB.LargeFundMoneyTransferFormData()
            {

                AgentId = Common.AgentSession.AgentInformation.Id,
                AgentStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId,
                AmountInLocalCurrency = vm.AmountInLocalCurrency,
                AmountInUSD = vm.AmountInUSD,
                DestinationCountryAndCity = vm.DestinationCountryAndCity,
                ExpectedPaymentDate = vm.ExpectedPaymentDate,
                ReceiverIdCardExpiryDate = vm.ReceiverIdCardExpiryDate,
                ReceiverIdCardNumber = vm.ReceiverIdCardNumber,
                IsCheckedTheCountryRestriction = vm.IsCheckedTheCountryRestriction,
                IsDocumentAttached = vm.IsDocumentAttached,
                IsMoneyFormAttached = vm.IsMoneyFormAttached,
                IsSenderAwareOfCharges = vm.IsSenderAwareOfCharges,
                MoneyTransferCountInLastThreeMonth = int.Parse(vm.MoneyTransferCountInLastThreeMonth),
                ReceiverNationality = vm.ReceiverNationality,
                PurposeOfTransfer = vm.PurposeOfTransfer,
                ReceiverAddress = vm.ReceiverAddress,
                ReceiverIdCardType = vm.ReceiverIdCardType,
                ReceiverName = vm.ReceiverName,
                ReceiverTelephone = vm.ReceiverTelephone,
                RelationshipBetnSenderAndReceiver = vm.RelationshipBetnSenderAndReceiver,
                SenderAddress = vm.SenderAddress,
                SenderCountry = vm.SenderCountry,
                SenderDateOfBirth = vm.SenderDateOfBirth,
                SenderEmailAddress = vm.SenderEmailAddress,
                SenderFullName = vm.SenderFullName,
                SenderGender = vm.SenderGender,
                SenderIdCardType = vm.SenderIdCardType,
                SenderIdExpiryDate = vm.SenderIdExpiryDate,
                SenderIdIssuingCountry = vm.SenderIdIssuingCountry,
                SenderIdNumber = vm.SenderIdNumber,
                SenderOccupation = vm.SenderOccupation,
                SenderPhoneNo = vm.SenderPhoneNo,
                SubmittedDate = DateTime.Now



            };
            return model;

        }



        private List<CountryDropDownVm> GetCountries()
        {

            var result = (from c in Common.Common.GetCountries()
                          select new CountryDropDownVm()
                          {
                              Code = c.CountryCode,
                              Name = c.CountryName
                          }).ToList();
            return result;
        }

    }
}