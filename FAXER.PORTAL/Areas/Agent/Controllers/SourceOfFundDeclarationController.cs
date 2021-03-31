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
    public class SourceOfFundDeclarationController : Controller
    {
        SourceOfFundDeclarationServices _sourceOfFundDeclarationServices = null;
        public SourceOfFundDeclarationController()
        {
            _sourceOfFundDeclarationServices = new SourceOfFundDeclarationServices();
        }
        // GET: Agent/SourceOfFundDeclaration
        [HttpGet]
        public ActionResult Index()
        {
            AgentInformation agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            int agentId = agentInfo.Id;
            if (agentId == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            AgentResult agentResult = new AgentResult();
            AgentCommonServices _agentCommonServices = new AgentCommonServices();

            ViewBag.Countries = new SelectList(_agentCommonServices.GetCountries(), "Code", "Name");
            var vm = SetSourceOfFundDeclarationVMInitialValue();
            ViewBag.AgentResult = agentResult;
            return View(vm);
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = SourceOfFundDeclarationVM.BindProperty)]SourceOfFundDeclarationVM vm)
        {
            AgentResult agentResult = new AgentResult();
            AgentCommonServices _agentCommonServices = new AgentCommonServices();
            ViewBag.Countries = new SelectList(_agentCommonServices.GetCountries(), "Code", "Name");
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
                var model = SourceOfFundDeclarationModelBinding(vm);
                var result = _sourceOfFundDeclarationServices.CreateSenderNonCardWithdrawal(model);
                agentResult.Message = "Form Submitted Successfully";
                agentResult.Status = ResultStatus.OK;
                ModelState.Clear();

            }
            ViewBag.AgentResult = agentResult;
            return View(vm);
        }

        private SourceOfFundDeclarationFormData SourceOfFundDeclarationModelBinding(SourceOfFundDeclarationVM vm)
        {

            SourceOfFundDeclarationFormData model = new SourceOfFundDeclarationFormData()
            {
                AdminStaffLoginCode = vm.AdminStaffLoginCode,
                AdminStaffName = vm.AdminStaffName,
                AgentCountry = vm.AgentCountry,
                AgentId = Common.AgentSession.AgentInformation.Id,
                AgentLoginCode = vm.AdminStaffLoginCode,
                AgentStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId,
                AgentStaffAccountNo = vm.AgentStaffAccountNo,
                AgentStaffName = vm.AgentStaffName,
                BankStatementLegalLetter = vm.BankStatementLegalLetter,
                BusinessVirtualAccount = vm.BusinessVirtualAccount,
                CashToCash = vm.CashToCash,
                Email = vm.Email,
                FaceToFace = vm.FaceToFace,
                IDAvailable = vm.IDAvailable,
                IsConfirm = vm.IsConfirm,
                LinkedAmount = vm.LinkedAmount,
                MeansOfVerification = vm.MeansOfVerification,
                OneOffAmount = vm.OneOffAmount,
                Others = vm.Others,
                ProofOfPurpose = vm.ProofOfPurpose,
                ProofOfSourceOfIncome = vm.ProofOfSourceOfIncome,
                ReceiverAddress = vm.ReceiverAddress,
                ReceiverCity = vm.ReceiverCity,
                ReceiverEmail = vm.ReceiverEmail,
                ReceiverName = vm.ReceiverName,
                ReceiverTelephone = vm.ReceiverTelephone,
                ReceverCountry = vm.ReceverCountry,
                RelationShipToSender = vm.RelationShipToSender,
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
                SubmittedDate = DateTime.Now,
                TransferType = vm.TransferType,
                VirtualAccount = vm.VirtualAccount,
            };
            return model;
        }

        private SourceOfFundDeclarationVM SetSourceOfFundDeclarationVMInitialValue()
        {


            AgentServices.AgentCommonServices agentCommonServices = new AgentServices.AgentCommonServices();
            var agentInfo = agentCommonServices.GetAgentStaffLoginInfo().Where(x => x.AgentStaffId == Common.AgentSession.LoggedUser.PayingAgentStaffId).FirstOrDefault();
            SourceOfFundDeclarationVM vm = new SourceOfFundDeclarationVM()
            {

                AgentCountry = agentInfo.AgentStaff.Country,
                AgentLoginCode = agentInfo.StaffLoginCode,
                AgentStaffName = agentInfo.AgentStaff.FirstName + " " + agentInfo.AgentStaff.MiddleName + " " + agentInfo.AgentStaff.LastName,
                AgentStaffAccountNo = agentInfo.AgentStaff.AgentMFSCode
            };
            return vm;
        }
    }
}