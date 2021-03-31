using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewAgentCashWithdrawalController : Controller
    {
        ViewAgentCashWithdrawalServices service = null;
        public ViewAgentCashWithdrawalController()
        {
            service = new ViewAgentCashWithdrawalServices();
        }
        // GET: Admin/ViewAgentCashWithdrawal
        public ActionResult Index(string message = "", int day = 0, int month = 0, int year = 0,int? page=null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (!string.IsNullOrEmpty(message))
            {
                if (message == "confirmed")
                {
                    ViewBag.Message = "Withdrawal Confirmed !";
                }
                else if (message == "error")
                {
                    ViewBag.Message = "Something went wrong. Please contact Admin !";
                }

            }
            ViewBag.Days = new SelectList(Enumerable.Range(1, 32));
            ViewBag.Years = new SelectList(Enumerable.Range(2018, 10));
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var vm = service.getCashWithdrawalList(pageNumber, pageSize);
            if (day != 0)
            {
                vm.AgentCashWithdrawalList = vm.AgentCashWithdrawalList.Where(x => x.TransactionDate.Day == day).ToList();
                vm.Day = day;
            }
            if (year != 0)
            {
                vm.AgentCashWithdrawalList = vm.AgentCashWithdrawalList.Where(x => x.TransactionDate.Year == year).ToList();
                vm.Year = year;
            }
            if (month != 0)
            {
                vm.AgentCashWithdrawalList = vm.AgentCashWithdrawalList.Where(x => x.TransactionDate.Month == month).ToList();
                vm.Month = (Month)month;
            }
            return View(vm);
        }

        public ActionResult ConfirmWithdrawal(int id, int isWithdrawalByAgent)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (id != 0)
            {
                bool confirmed = service.confirmWithdrawal(id, isWithdrawalByAgent);
                if (confirmed)
                {
                    return RedirectToAction("Index", new { @message = "confirmed" });
                }
            }
            return RedirectToAction("Index", new { @message = "error" });
        }

        public JsonResult ShowMoreDetails(int id, int isWithdrawalByAgent)
        {
            if (id != 0)
            {
                var data = service.getMoreDetails(id, isWithdrawalByAgent);
                if (data != null)
                {
                    return Json(new StaffMoreDetailsViewModel()
                    {
                        Id = data.Id,
                        IDType = data.IDType,
                        IDNo = data.IDNo,
                        ExpiryDate = data.ExpiryDate,
                        IssuingCountry = data.IssuingCountry
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new
            {
            }, JsonRequestBehavior.AllowGet);
        }


    }
}