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
    public class SARFormController : Controller
    {
        SARFormServices _SARFormServices = null;
        public SARFormController()
        {
            _SARFormServices = new SARFormServices();
        }
        // GET: Agent/SARForm
        [HttpGet]
        public ActionResult Index()
        {

            AgentResult agentResult = new AgentResult();
            AgentServices.AgentCommonServices agentCommonServices = new AgentCommonServices();
            SARFormVM vm = new SARFormVM();
            vm.ReasonsForSuspicion = _SARFormServices.GetReasonsForSuspicion();
            vm.AgentId = Common.AgentSession.AgentInformation.Id;
            vm.AgentAccountNo = Common.AgentSession.LoggedUser.PayingAgentAccountNumber;
            vm.AgentStaffName = Common.AgentSession.LoggedUser.PayingAgentStaffName;
            vm.AgentStaffLoginCode = agentCommonServices.GetAgentStaffLoginCode();
            vm.SubmittedDate = DateTime.Now;

            agentResult.Message = "";
            ViewBag.AgentResult = agentResult;

            return View(vm);
        }
        public ActionResult Index([Bind(Include = SARFormVM.BindProperty)] SARFormVM vm)
        {
            
            AgentResult agentResult = new AgentResult();
            if (vm.SARTransactionType == SARTransactionType.CashToCash && string.IsNullOrEmpty(vm.MFCN))
            {


                ModelState.AddModelError("MFCN", "Please enter the MFCN No");
                ViewBag.AgentResult = agentResult;
                return View(vm);

            }
            else if (vm.SARTransactionType == SARTransactionType.VirtualAccount && string.IsNullOrEmpty(vm.VirtualAccountNo))
            {


                ModelState.AddModelError("VirtualAccountNo", "Please enter the virtual account no.");
                ViewBag.AgentResult = agentResult;

                return View(vm);


            }
            else if (vm.SARTransactionType == SARTransactionType.BusinessVirtualAccount && string.IsNullOrEmpty(vm.BusinessVirtualAccountNo))
            {



                ModelState.AddModelError("BusinessVirtualAccountNo", "Please enter the business account no");
                ViewBag.AgentResult = agentResult;

                return View(vm);

            }
            else if (vm.TransactionDate == default(DateTime))
            {


                ModelState.AddModelError("TransactionDate", "Please enter the transaction Date");
                ViewBag.AgentResult = agentResult;

                return View(vm);

            }
            else if (string.IsNullOrEmpty(vm.TransactionTime))
            {



                ModelState.AddModelError("TransactionTime", "Please enter the transaction time");
                ViewBag.AgentResult = agentResult;

                return View(vm);

            }
            else if (ModelState.IsValid)
            {
                if (vm.DateOfBirth == default(DateTime))
                {

                    ModelState.AddModelError("DateOfBirth", "Please enter the date of birth .");
                    ViewBag.AgentResult = agentResult;

                    return View(vm);
                }
                else if (vm.IdentificationExpiryDate == default(DateTime))
                {

                    ModelState.AddModelError("IdentificationExpiryDate", "Please enter the Id expiry date.");
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }




                SARForm model = new SARForm()
                {
                    AgentId = vm.AgentId,
                    AgentStaffLoginCode = vm.AgentStaffLoginCode,
                    Address = vm.Address,
                    AgentAccountNo = vm.AgentAccountNo,
                    AgentStaffName = vm.AgentStaffName,
                    BusinessVirtualAccountNo = vm.BusinessVirtualAccountNo,
                    DateOfBirth = vm.DateOfBirth,
                    FirstName = vm.FirstName,
                    IdentificationExpiryDate = vm.IdentificationExpiryDate,
                    IdType = vm.IdType,
                    IdNumber = vm.IdNumber,
                    InvestigationDate = vm.InvestigationDate,
                    IsSAR = vm.IsSAR,
                    LastName = vm.LastName,
                    MFCN = vm.MFCN,
                    OtherSuspiciousReason = vm.OtherSuspiciousReason,
                    SARTransactionType = vm.SARTransactionType,
                    StaffAccountNo = vm.StaffAccountNo,
                    StaffName = vm.StaffName,
                    SubmittedDate = vm.SubmittedDate,
                    TransactionDate = vm.TransactionDate,
                    TransactionTime = vm.TransactionTime,
                    VirtualAccountNo = vm.VirtualAccountNo,
                    AgentStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId,

                };
                var result = _SARFormServices.CreateSARFormReport(model);

                var SuspicionActivityCheckedList = (from c in vm.ReasonsForSuspicion.Where(x => x.IsChecked == true)
                                                    select new SARForm_ReasonForSuspicion()
                                                    {
                                                        ReasonForSuspicionId = c.Id,
                                                        SARFormId = result.Id
                                                    }).ToList();
                var resultSuspicionActivityChecked = _SARFormServices.CreateSARForm_ReasonForSuspicion(SuspicionActivityCheckedList);

                agentResult.Status = ResultStatus.OK;
                agentResult.Message = "Form Submitted Successfully!";
                ViewBag.AgentResult = agentResult;
                ModelState.Clear();
                return View(vm);
                

            }
            agentResult.Message = "";
            ViewBag.AgentResult = agentResult;

            return View(vm);

        }
    }
}