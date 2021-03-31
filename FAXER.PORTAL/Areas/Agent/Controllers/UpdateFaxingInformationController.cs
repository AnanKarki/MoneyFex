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
    public class UpdateFaxingInformationController : Controller
    {

        Admin.Services.CommonServices common = new Admin.Services.CommonServices();
        // GET: Agent/UpdateFaxingInformation
        [HttpGet]
        public ActionResult Index(string MFCN = "" )
        {
            AgentInformation agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            string agencyName = agentInfo.Name;
            string agencyMFSCode = agentInfo.AccountNo;
            int agentId = agentInfo.Id;
            if (agentId == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            UpdateFaxingInformationViewModel vm = new UpdateFaxingInformationViewModel();
            vm.NameofUpdatingAgent = Common.AgentSession.LoggedUser.PayingAgentStaffName;
            AgentServices.UpdateFaxingInformationServices updateservices = new AgentServices.UpdateFaxingInformationServices();
            
            if (MFCN != "" )
            {

                var FaxMoneyDetails = updateservices.getFaxerInformation(MFCN);
                if (FaxMoneyDetails != null)
                {
                    AgentServices.AgentFaxMoneyServices agentservices = new AgentServices.AgentFaxMoneyServices();
                    //var FaxingCaculatedDetails = agentservices.getCalculateDetails(FaxMoneyDetails.FaxerCountryCode, FaxMoneyDetails.ReceiverCountryCode,Convert.ToDecimal(FaxMoneyDetails.FaxingAmount));
                    //vm = FaxMoneyDetails;
                    ////vm.FaxingAmount = String.Format("{0:n}", FaxingCaculatedDetails.FaxingAmount);
                    //vm.FaxingFee = String.Format("{0:n}", FaxingCaculatedDetails.FaxingFee) ;
                    //vm.TotalAmountincludingFee = String.Format("{0:n}", FaxingCaculatedDetails.TotalAmount);
                    //vm.AmountToBeReceived = String.Format("{0:n}", FaxingCaculatedDetails.ReceivingAmount);
                    //vm.CurrentExchangeRate = String.Format("{0:n}", FaxingCaculatedDetails.ExchangeRate);
                    vm = FaxMoneyDetails;
                    vm.AgentId = agentId;
                    ViewBag.AgentResult = getAgentFaxMoneyStatus(vm);
                    return View(vm);
                }
                else {
                    AgentResult agentResult = new AgentResult();
                    //agentResult.Message = "MFCN number does not exit";
                    //agentResult.Status = ResultStatus.Warning;
                    ModelState.AddModelError("Invalid", "MFCN number does not exit");
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
              
              
            }
           
        
        ViewBag.AgentResult = new AgentResult();
            vm.StatusOfFaxName = "";
            return View(vm);
  
        }

        private AgentResult getAgentFaxMoneyStatus(UpdateFaxingInformationViewModel vm)
        {
            AgentResult agentResult = new AgentResult();
            if (vm == null)
            {
                agentResult.Message = "MFCN does not exist.Please enter a valid MFCN";
                agentResult.Status = ResultStatus.Warning;
                return agentResult;
            }
            else
            {
                if (vm.StatusOfFax == FaxingStatus.Received)
                {
                    agentResult.Message = "This transaction has been Received.";
                    agentResult.Status = ResultStatus.Warning;
                    agentResult.Data = vm;
                }
                else if (vm.StatusOfFax == FaxingStatus.Refund)
                {
                    agentResult.Message = "This transaction has been Refunded.";
                    agentResult.Status = ResultStatus.Warning;
                }
                else if (vm.StatusOfFax == FaxingStatus.Cancel)
                {
                    agentResult.Message = "This transaction has been Cancelled.";
                    agentResult.Status = ResultStatus.Warning;
                }
                else if (vm.StatusOfFax == FaxingStatus.Hold)
                {
                    agentResult.Message = "This transaction has been held.";
                    agentResult.Status = ResultStatus.Warning;

                }
            }
            return agentResult;
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = UpdateFaxingInformationViewModel.BindProperty)] UpdateFaxingInformationViewModel vm) {
            AgentResult agentResult = new AgentResult();
            AgentServices.UpdateFaxingInformationServices agentservices = new AgentServices.UpdateFaxingInformationServices();
           
            if (ModelState.IsValid) {
                if (vm.StatusOfFax == FaxingStatus.Received)
                {
                    agentResult.Message = "Sorry You cannot update the Information ,the transaction has  been Received.";
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
                if (vm.StatusOfFax == FaxingStatus.Refund)
                {
                    agentResult.Message = "Sorry You cannot update the Information, the transaction has been  Refunded.";
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
                 if (vm.StatusOfFax == FaxingStatus.Cancel)
                {
                    agentResult.Message = "Sorry You cannot update the Information,  the transaction has been Cancelled.";
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
                if (vm.StatusOfFax == FaxingStatus.Hold)
                {
                    agentResult.Message = "Sorry You cannot update the Information ,  the transaction has been Held. ";
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
                if (vm.FaxingFee == null) {
                    ModelState.AddModelError("FaxingFee", "Faxing Fee Required");
                    ViewBag.AgentResult = new AgentResult();
                    return View(vm);

                }
                if (vm.PayingAgentName == null) {
                    ModelState.AddModelError("PayingAgentName","Please Enter Paying Agent Name");
                    ViewBag.AgentResult = new AgentResult();
                    return View(vm);
                }
                if (vm.NameofUpdatingAgent == null) {
                    ModelState.AddModelError("NameofUpdatingAgent", "Please Enter Name of Updating Agent");
                    ViewBag.AgentResult = new AgentResult();
                    return View(vm);
                }
                if (!vm.IsConfirmed) {
                    agentResult.Message = "Confirmation for the information is required to either pay or rejection this transaction has been fully verified by yourself";
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
                var updateAgentFaxMoney = agentservices.updateFaxMoneyInformation(vm);
                if (updateAgentFaxMoney == true)
                {
                    agentResult.Message = "Faxing Information Updated Successfully";
                    agentResult.Status = ResultStatus.OK;
                    ViewBag.AgentResult = agentResult;
                    ModelState.Clear();
                    var model = new Models.UpdateFaxingInformationViewModel();
                    model.StatusOfFaxName = "";
                    return View(model);
                }
                else {
                    agentResult.Message = "Sorry Faxing Information Cannot be Updated";
                    agentResult.Status = ResultStatus.OK;
                    ViewBag.AgentResult = agentResult;

                    return View(vm);
                }
            }
            ViewBag.AgentResult = new AgentResult();
            return View(vm);
        }
      
    }
}