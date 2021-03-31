using FAXER.PORTAL.Areas.Agent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class AdminThirdPartyMoneyTransferController : Controller
    {
        // GET: Admin/AdminThirdPartyMoneyTransfer
        public ActionResult Index()
        {
            List<ThirdPartyMoneyTransferListVM> vm = new List<ThirdPartyMoneyTransferListVM>();
            return View(vm);
        }

        public ActionResult SubmitThirdPartyMoneyTransfer()
        {
            ThirdPartyMoneyTransferVM vm = new ThirdPartyMoneyTransferVM();

            return View(vm);
        }
    }
}