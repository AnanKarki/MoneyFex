using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class AgentManualDepositController : Controller
    {
        SSenderBankAccountDeposit _bankdeposit = null;
        DailyTransactionStatementServices _dailyTransactionStatementServices = null;

        AgentInformation agentInfo = null;

        public AgentManualDepositController()
        {
            agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            _dailyTransactionStatementServices = new DailyTransactionStatementServices();

            _bankdeposit = new SSenderBankAccountDeposit();
        }
        // GET: Agent/AgentManualDeposit v
        public ActionResult Index(int Year = 0, int Month = 0, int Day = 0)
        {

            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }

            ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));

            ViewBag.Month = Month;
            ViewBag.Day = Day;

            List<ManualBankDepositViewModel> vm = _bankdeposit.GetAgentManualBankDeposit(agentInfo.Id, Year, Month, Day).Data;

            return View(vm);


        }

        public ActionResult AgentManualBankdepositDetails(int id, TransactionType transactionService)
        {
            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }

            ViewBag.AgentId = agentInfo.Id;

            int AgentId = _dailyTransactionStatementServices.GetPayingStaffId(agentInfo.Id);

            AgentTransactionHistoryViewModel vm = new AgentTransactionHistoryViewModel();
            vm.TransactionHistoryList = _dailyTransactionStatementServices.GetManualBankAccountDepositTrasactionDetails(id);
            vm.FilterKey = transactionService;

            return View(vm);
        }


        public ActionResult Confirm(int id)
        {
            if (id != 0)
            {
                var data = _bankdeposit.List().Data.Where(x => x.Id == id).FirstOrDefault();
                if (data != null)
                {
                    data.Status = BankDepositStatus.Confirm;
                }
                _bankdeposit.Update(data);

                #region SMS AND EMAIL
                //SMS
                //var senderInfo = _bankdeposit.GetSenderInfo(data.SenderId);
                //string senderFirstName = senderInfo.FirstName;
                //string senderPhoneNo = Common.Common.GetCountryPhoneCode(senderInfo.Country) +  senderInfo.PhoneNumber;
                //SSenderForAllTransfer _senderAllTransferService = new SSenderForAllTransfer();
                //string SendingAmountWithCurrency = Common.Common.GetCountryCurrency(data.SendingCountry) + " " + data.SendingAmount;
                //string ReceivingAmountWithCurrency = Common.Common.GetCountryCurrency(data.ReceivingCountry) + " " + data.ReceivingAmount;
                //string FeeAmountWithCurrency = Common.Common.GetCountryCurrency(data.SendingCountry) + " " + data.Fee;
                //string ReceiverFirstName = data.ReceiverName.Split(' ')[0]; ;

                //_senderAllTransferService.SendManualBankDepositSecondSmsToSender(data.ReceiverName, data.SendingAmount + " " + Common.Common.GetCountryCurrency(data.SendingCountry), 
                //    data.ReceiptNo , senderPhoneNo);
                //_senderAllTransferService.SendManualBankDepositSmsToReceiver(senderFirstName, data.ReceivingAmount + " " + Common.Common.GetCountryCurrency(data.ReceivingCountry), data.SendingCountry,
                //                       Common.Common.GetCountryPhoneCode(data.ReceivingCountry) + data.ReceiverMobileNo);

                ////Email
                //_senderAllTransferService.SendManualDepositSuccessEmail(senderFirstName, FeeAmountWithCurrency, SendingAmountWithCurrency, data.ReceiverAccountNo,
                // ReceivingAmountWithCurrency, data.BankId, data.ReceiverName, data.ReceiptNo, data.BankCode, data.ReceivingCountry, ReceiverFirstName, data.SenderId);

                SSenderBankAccountDeposit _senderBankAccountDepositServices = new SSenderBankAccountDeposit();
                _senderBankAccountDepositServices.SendEmailAndSms(data);
                #endregion

            }
            return RedirectToAction("Index", "AgentManualDeposit");
        }


    }
}