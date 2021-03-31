using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers.Receipts.Agent
{
    public class CashWithdrawalReceiptController : Controller
    {
        // GET: EmailTemplate/CashWithdrawalReceipt
        public ActionResult Index(string ReceiptNo, string Date, string Time, string AgentName, string AgentAccountNo, string WithdrawalType
                               , string StaffName, string StaffCode , string WithdrawalCode , string AdminCodeGenerator , string WithdrawalAmount , string Currency , bool IsWithdrawalByAgent = false)
        {
            ViewBag.ReceiptNo = ReceiptNo;
            ViewBag.Date = Date;
            ViewBag.Time = Time;
            ViewBag.AgentName = AgentName;
            ViewBag.AgentAccountNo = AgentAccountNo;
            ViewBag.WithdrawalType = WithdrawalType;
            ViewBag.StaffName = StaffName;
            ViewBag.StaffCode = StaffCode;
            ViewBag.WithdrawalCode = WithdrawalCode;
            ViewBag.AdminCodeGenerator = AdminCodeGenerator;
            ViewBag.WithdrawalAmount = WithdrawalAmount;
            ViewBag.Currency = Currency;
            ViewBag.IsWithdrawalByAgent = IsWithdrawalByAgent;


            return View();
        }
    }
}