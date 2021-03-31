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
    public class RefundRequestController : Controller
    {
        Admin.Services.CommonServices common = new Admin.Services.CommonServices();

        // GET: Agent/RefundRequest
        [HttpGet]
        public ActionResult Index(string MFCN = "")
        {
            //Session.Remove("FirstLogin");
            AgentResult agenResult = new AgentResult();
            AgentInformation agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            string agencyName = agentInfo.Name;
            string agencyMFSCode = agentInfo.AccountNo;
            int agentId = agentInfo.Id;
            if (agentId == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            Models.RefundRequestViewModel vm = new Models.RefundRequestViewModel();
            vm.PayingAgentName = Common.AgentSession.LoggedUser.PayingAgentStaffName;
            AgentServices.RefundRequestServices refundServices = new AgentServices.RefundRequestServices();

            if (MFCN != "")
            {

                var RefundDetails = refundServices.getFaxerReceiverInformation(MFCN);
                if (RefundDetails != null)
                {
                    
                    vm = RefundDetails;
                    vm.NameOfAgency = agencyName;
                    vm.AgencyMFSCode = agencyMFSCode;
                    vm.AgentId = agentId;
                    ViewBag.AgentResult = getAgentFaxMoneyStatus(vm);
                    return View(vm);
                }
                else
                {
                    AgentResult agentResult = new AgentResult();
                    //agentResult.Message = "MFCN Number Doesnot Exist";
                    //agentResult.Status = ResultStatus.Warning;
                    ModelState.AddModelError("", "MFCN Number Doesnot Exist");

                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }


            }


            ViewBag.AgentResult = new AgentResult();
            vm.StatusOfFax = FaxingStatus.NotReceived;
            vm.StatusOfFaxName = "";
            return View(vm);

        }
        private AgentResult getAgentFaxMoneyStatus(Models.RefundRequestViewModel vm)
        {
            AgentResult agentResult = new AgentResult();
            if (vm == null)
            {
                //agentResult.Message = "MFCN does not exist.Please enter a valid MFCN";
                //agentResult.Status = ResultStatus.Warning;
                ModelState.AddModelError("", "MFCN does not exist.Please enter a valid MFCN");
                return agentResult;

            }
            else
            {
                if (vm.StatusOfFax == FaxingStatus.Received)
                {
                    //agentResult.Message = "This transaction has been receiveded.";
                    //agentResult.Status = ResultStatus.Warning;

                    agentResult.Data = vm;
                    ModelState.AddModelError("", "This transaction has been receiveded.");
                }
                else if (vm.StatusOfFax == FaxingStatus.Refund)
                {
                //    agentResult.Message = "This transaction has been  refunded.";
                //    agentResult.Status = ResultStatus.Warning;
                    ModelState.AddModelError("", "This transaction has been refunded.");
                }
                else if (vm.StatusOfFax == FaxingStatus.Cancel)
                {
                    //agentResult.Message = "This transaction has been cancelled";
                    //agentResult.Status = ResultStatus.Warning;
                    ModelState.AddModelError("", "This transaction has been cancelled.");
                }
                else if (vm.StatusOfFax == FaxingStatus.Hold) {


                    //agentResult.Message = "This transaction has been held , Sorry you cannot make refund";
                    //agentResult.Status = ResultStatus.Warning;
                    ModelState.AddModelError("", "This transaction has held , Sorry you cannot make refund.");
                }
            }
            return agentResult;
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = RefundRequestViewModel.BindProperty)] RefundRequestViewModel vm)
        {

            AgentResult agentResult = new AgentResult();
            AgentServices.RefundRequestServices refundServices = new AgentServices.RefundRequestServices();

            if (ModelState.IsValid)
            {
                if (vm.StatusOfFax == FaxingStatus.Received)
                {
                    //agentResult.Message = "This transaction has been receiveded.";
                    //agentResult.Status = ResultStatus.Warning;

                    ModelState.AddModelError("" , "This transaction has been receiveded.");
                    
                }
                else if (vm.StatusOfFax == FaxingStatus.Refund)
                {
                    //agentResult.Message = "This transaction has been  refunded.";
                    //agentResult.Status = ResultStatus.Warning;

                    ModelState.AddModelError("", "This transaction has been refunded.");

                }
                else if (vm.StatusOfFax == FaxingStatus.Cancel)
                {
                    //agentResult.Message = "This transaction has been cancelled";
                    //agentResult.Status = ResultStatus.Warning;

                    ModelState.AddModelError("", "This transaction has been cancelled.");

                }
                else if (vm.StatusOfFax == FaxingStatus.Hold)
                {


                    //agentResult.Message = "This transaction has been held , Sorry you cannot make refund";
                    //agentResult.Status = ResultStatus.Warning;

                    ModelState.AddModelError("", "This transaction has been cancelled.");
                }
                else if (vm.NameOfRefunder == null)
                {
                    ModelState.AddModelError("NameOfRefunder", "Please Enter The Name of Refunder");
                    ViewBag.AgentResult = agentResult;
                    return View(vm);

                }
                else if (vm.NoConfirmed)
                {
                    //agentResult.Message = "Please Check yes If you want Refund";
                    //agentResult.Status = ResultStatus.Warning;

                    ModelState.AddModelError("Check", "Please Check yes If you want Refund.");
                    ViewBag.AgentResult = agentResult;

                    return View(vm);
                }
                else if (!vm.YesConfirmed)
                {
                    //agentResult.Message = "Please Confirm You want Refund";
                    //agentResult.Status = ResultStatus.Warning;

                    ModelState.AddModelError("Check", "Please Confirm If You want Refund.");
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
                else
                {
                    string RefundReceiptNumber = refundServices.GetNewRefundReceiptNumber();
                    vm.RefundReceiptNumber = RefundReceiptNumber;
                    var result = refundServices.RefundFaxMoney(vm);
                    if (result == true)
                    {

                        agentResult.Message = "Refunded SuccessFully";
                        agentResult.Status = ResultStatus.OK;
                        agentResult.Data = vm.MFCNNumber;
                        ViewBag.AgentResult = agentResult;
                        ModelState.Clear();
                        Models.RefundRequestViewModel model = new Models.RefundRequestViewModel();
                        model.StatusOfFaxName = "";
                        return View(model);
                    }
                    else
                    {
                        //agentResult.Message = "Sorry Transaction Umcompleted";
                        //agentResult.Status = ResultStatus.Error;

                        ModelState.AddModelError("", "Sorry Transaction is Incompleted.");
                        ViewBag.AgentResult = agentResult;
                        return View(vm);
                    }
                }
            }
            ViewBag.AgentResult = agentResult;
            return View(vm);
        }

        public void PrintRefundReceipt(string MFCN)
        {
            AgentServices.RefundRequestServices services = new AgentServices.RefundRequestServices();
            var nonCardTrans = services.GetFaxingNonCardTransaction(MFCN);

            var RefundDetails = services.GetRefundNonCardDetails(nonCardTrans.Id);

            var ReceiverInfo = services.GetReceiversDetails(nonCardTrans.NonCardRecieverId);
            var FaxerInfo = services.GetFaxerInformation(ReceiverInfo.FaxerID);
            var agentInfo = services.GetAgentInformation(RefundDetails.Agent_id);
            string ReceiverPhoneCode = Common.Common.GetCountryPhoneCode(ReceiverInfo.Country);
            string FaxerCuurency = Common.Common.GetCountryCurrency(FaxerInfo.Country);
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            
            var ReceiptUrl = baseUrl + "/EmailTemplate/AgentRefundReceipt?ReceiptNumber=" + RefundDetails.ReceiptNumber +
                    "&TransactionReceiptNumber=" + nonCardTrans.ReceiptNumber + "&Date=" + RefundDetails.RefundedDate.ToString("dd/MM/yyyy") +
                    "&Time=" + RefundDetails.RefundedDate.ToString("HH:mm") +
                    "&SenderFullName=" + FaxerInfo.FirstName + " " + FaxerInfo.MiddleName + " " + FaxerInfo.LastName
                    + "&MFCN=" + nonCardTrans.MFCN +
                    "&ReceiverFullName=" + ReceiverInfo.FirstName + " " + ReceiverInfo.MiddleName + " " + ReceiverInfo.LastName +
                    "&Telephone=" + ReceiverPhoneCode + " " + ReceiverInfo.PhoneNumber
                    + "&RefundingAgentName=" + RefundDetails.NameofRefunder + "&RefundingAgentCode=" + agentInfo.AccountNo
                    + "&OrignalAmountSent=" + nonCardTrans.FaxingAmount + " " + FaxerCuurency +
                    "&RefundedAmount=" + nonCardTrans.FaxingAmount + " " + FaxerCuurency + "&AgentCountry=" + Common.Common.GetCountryName(agentInfo.CountryCode)
                    + "&AgentCity=" + agentInfo.City + "&AgentTelephoneNumber=" + Common.Common.GetCountryPhoneCode(agentInfo.CountryCode) + " " + agentInfo.PhoneNumber;



            var ReceiptPDF = Common.Common.GetPdf(ReceiptUrl);
            byte[] bytes = ReceiptPDF.Save();
            string mimeType = "Application/pdf";
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.Flush();
            Response.End();

        }

    }
}