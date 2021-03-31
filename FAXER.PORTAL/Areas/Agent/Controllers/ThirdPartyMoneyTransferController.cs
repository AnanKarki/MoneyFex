using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class ThirdPartyMoneyTransferController : Controller
    {

        ThirdPartyMoneyTransferServices _thirdPartyMoneyTransferServices = null;
        public ThirdPartyMoneyTransferController()
        {
            _thirdPartyMoneyTransferServices = new ThirdPartyMoneyTransferServices();
        }
        // GET: Agent/ThirdPartyMoneyTransfer
        [HttpGet]
        public ActionResult Index()
        {
            AgentCommonServices _agentCommonServices = new AgentCommonServices();
            AgentResult agentResult = new AgentResult();

            var vm = GetThirdPartyMoneyTransferVMIntialDetail();
            agentResult.Message = "";
            ViewBag.AgentResult = agentResult;
            ViewBag.Countries = new SelectList(_agentCommonServices.GetCountries(), "Code", "Name");
            
            return View(vm);
        }
        public ActionResult Index([Bind(Include = ThirdPartyMoneyTransferVM.BindProperty)] ThirdPartyMoneyTransferVM vm)
        {
            AgentResult agentResult = new AgentResult();
            AgentCommonServices _agentCommonServices = new AgentCommonServices();

            ViewBag.Countries = new SelectList(_agentCommonServices.GetCountries(), "Code", "Name");
            if (ModelState.IsValid)
            {
                var DOB = vm.DateOfBirth;
                var currentYear = DateTime.Now.Year;
                var DOBYear = DOB.Year;
                var age = currentYear - DOBYear;
                if (age <= 18)
                {
                    ModelState.AddModelError("InvalidAge", "Sender must be 18 years above");
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }

                if (vm.IdExpiryDate < DateTime.Now)
                {
                    ModelState.AddModelError("IdExpired", "Id Expired");
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }

                var data = ThirdPartyMoneyTransferModelBinding(vm);
                var result = _thirdPartyMoneyTransferServices.SaveThirdPartyMoneyTransfer(data);
                agentResult.Status = ResultStatus.OK;
                agentResult.Message = "Form Submitted Successfully";
                ViewBag.AgentResult = agentResult;
                ModelState.Clear();

                return View(vm);
            }
            agentResult.Message = "";
            ViewBag.AgentResult = agentResult;
            return View(vm);
        }


        private DB.ThirdPartyMoneyTransfer ThirdPartyMoneyTransferModelBinding(Models.ThirdPartyMoneyTransferVM vm)
        {

            DB.ThirdPartyMoneyTransfer model = new DB.ThirdPartyMoneyTransfer()
            {
                AgentId = vm.AgentId,
                Address = vm.Address,
                AgentStaffAccountNo = vm.AgentStaffAccountNo,
                AgentStaffCountry = vm.AgentStaffCountry,
                AgentStaffEmail = vm.AgentStaffEmail,
                AgentStaffId = vm.AgentStaffId,
                AgentStaffName = vm.AgentStaffName,
                AgentStaffPhoneNo = vm.AgentStaffPhoneNo,
                SenderORBusinessName = vm.SenderORBusinessName,
                PhoneNo = vm.PhoneNo,
                Occupation = vm.Occupation,
                MFCN = vm.MFCN,
                MainAmount = vm.MainAmount,
                AppovedTitle = vm.AppovedTitle,
                ApprovedByName = vm.ApprovedByName,
                ApprovedDate = vm.ApprovedDate,
                Country = vm.Country,
                DateOfBirth = vm.DateOfBirth,
                DeclinedByName = vm.DeclinedByName,
                DeclinedDate = vm.DeclinedDate,
                DeclinedTitle = vm.DeclinedTitle,
                EmailAddress = vm.EmailAddress,
                Fee = vm.Fee,
                Gender = vm.Gender,
                IdCardType = vm.IdCardType,
                IdExpiryDate = vm.IdExpiryDate,
                IdIssuingCountry = vm.IdIssuingCountry,
                IdNumber = vm.IdNumber,
                IsThirdPartyTransfer = vm.IsThirdPartyTransfer,
                SubmittedDate = DateTime.Now,
                AgentStaffLoginCode = vm.AgentStaffLoginCode



            };
            return model;

        }

        private ThirdPartyMoneyTransferVM GetThirdPartyMoneyTransferVMIntialDetail()
        {


            AgentServices.AgentCommonServices _agentCommonServices = new AgentServices.AgentCommonServices();
            ThirdPartyMoneyTransferVM vm = new ThirdPartyMoneyTransferVM();
            var agentStaffLoginInfo = _agentCommonServices.GetAgentStaffLoginInfo().Where(x => x.AgentStaffId == Common.AgentSession.LoggedUser.PayingAgentStaffId).FirstOrDefault();
            vm.AgentId = Common.AgentSession.AgentInformation.Id;
            vm.AgentStaffAccountNo = Common.AgentSession.LoggedUser.PayingAgentAccountNumber;
            vm.AgentStaffName = agentStaffLoginInfo.AgentStaff.FirstName + " " + agentStaffLoginInfo.AgentStaff.MiddleName + " " + agentStaffLoginInfo.AgentStaff.LastName;
            vm.AgentStaffCountry = agentStaffLoginInfo.AgentStaff.Country;
            vm.AgentStaffEmail = agentStaffLoginInfo.AgentStaff.EmailAddress;
            vm.AgentStaffId = agentStaffLoginInfo.AgentStaffId;
            vm.AgentStaffPhoneNo = agentStaffLoginInfo.AgentStaff.PhoneNumber;
            vm.AgentStaffLoginCode = agentStaffLoginInfo.StaffLoginCode;

            return vm;

        }



    }
    public class CountryDropDownVm
    {

        public string Code { get; set; }
        public string Name { get; set; }

    }
}