using FAXER.PORTAL.Areas.Agent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class BankAccountCreditUpdateController : Controller
    {
        AgentServices.BankAccountCreditUpdateServices bankAccountCreditUpdateServices = null;
        DB.FAXEREntities dbContext = null;
        public BankAccountCreditUpdateController()
        {
            bankAccountCreditUpdateServices = new AgentServices.BankAccountCreditUpdateServices();
            dbContext = new DB.FAXEREntities();
        }
        // GET: Agent/BankAccountCreditUpdate
        [HttpGet]
        public ActionResult Index()
        {
            if (Common.AgentSession.AgentStaffLogin.AgentStaff.AgentStaffType == StaffType.Transaction)
            {
                return RedirectToAction("Index", "AgentDashboard");
            }
            var vm = bankAccountCreditUpdateServices.GetCreditDetails();

            return View(vm);
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = BankAccountCreditViewModel.BindProperty)] BankAccountCreditViewModel vm)
        {

            if (vm.NewAccountDeposit <= 0)
            {
                ModelState.AddModelError("NewAccountDeposit", "Amount should be greater than zero {0}");
                return View(vm);
            }
            if (string.IsNullOrEmpty(vm.NameOfUpdater))
            {
                ModelState.AddModelError("NameOfUpdater", "Please enter the name of updater");
                return View(vm);
            }
            DB.BaankAccountCreditUpdateByAgent model = new DB.BaankAccountCreditUpdateByAgent()
            {

                AgentId = Common.AgentSession.AgentInformation.Id,
                BankDeposit = vm.NewAccountDeposit,
                CreatedDateTime = DateTime.Now,
                NameOfUpdater = vm.NameOfUpdater,
                ReceiptNo = bankAccountCreditUpdateServices.GetReceiptNoForBankAccountDeposit()
            };
            var result = bankAccountCreditUpdateServices.SaveNewAccountDeposit(model);

            ModelState.Clear();
            var newmodel = bankAccountCreditUpdateServices.GetCreditDetails();

            ViewBag.TransactionSuccessul = 1;

            ViewBag.TransactionId = result.Id;
            return View(newmodel);
        }


        public void MoneyFexBankDeposit(int transactionId) 
        {


            var BankDepositDetails = dbContext.BaankAccountCreditUpdateByAgent.Where(x => x.Id == transactionId).FirstOrDefault();
            
            var AgentDetails = dbContext.AgentInformation.Where(x => x.Id == BankDepositDetails.AgentId).FirstOrDefault();

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            string ReceiptURL = baseUrl + "/EmailTemplate/MoneyFexAccountDepositReceipt?ReceiptNo=" + BankDepositDetails.ReceiptNo
                + "&Date=" + BankDepositDetails.CreatedDateTime.ToString("dd/MM/yyyy")
                + "&Time=" + BankDepositDetails.CreatedDateTime.ToString("HH:mm") + "&AgentName=" + AgentDetails.Name +
                "&AgentCode=" + AgentDetails.AccountNo + "&NameOfUpdater=" + BankDepositDetails.NameOfUpdater + "&DepositedAmount=" + BankDepositDetails.BankDeposit + "&Currency=" + Common.Common.GetCountryCurrency(AgentDetails.CountryCode);

            var Receipt = Common.Common.GetPdf(ReceiptURL);
            byte[] bytes = Receipt.Save();
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