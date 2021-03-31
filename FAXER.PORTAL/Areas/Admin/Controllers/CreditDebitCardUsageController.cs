using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class CreditDebitCardUsageController : Controller
    {

        CreditDebitCardUsageServices _services = null;
        CommonServices _CommonServices = null;
        public CreditDebitCardUsageController()
        {
            _services = new CreditDebitCardUsageServices();
            _CommonServices = new CommonServices();
        }

        // GET: Admin/CreditDebitCardUsage
        public ActionResult Index(string Country = "", string senderName="",int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<CreditDebitCardUsageViewModel> vm = _services.GetCreditDebitCardUsageLogList(Country).ToPagedList(pageNumber, pageSize);
            if (!string.IsNullOrEmpty(senderName))
            {
                senderName = senderName.Trim();
                vm = vm.Where(x => x.SenderName.ToLower().Contains(senderName.ToLower())).ToPagedList(pageNumber, pageSize);

            }
            return View(vm);
        }
        public ActionResult BlockSender(int id = 0)
        {
            _services.BlockSender(id);
            return RedirectToAction("Index", "CreditDebitCardUsage");
        }

    }
}