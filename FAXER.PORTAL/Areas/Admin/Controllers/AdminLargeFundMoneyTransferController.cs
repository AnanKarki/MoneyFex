using FAXER.PORTAL.Areas.Agent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class AdminLargeFundMoneyTransferController : Controller
    {
        // GET: Admin/AdminLargeFundMoneyTransfer
        public ActionResult Index()
        {
            List<LargeFundMoneyTransferListVM> vm = new List<LargeFundMoneyTransferListVM>();
            return View(vm);
        }


        public ActionResult SubmitLargeFundMoneyTransfer()
        {
            LargeFundMoneyTransferFormVM vm = new LargeFundMoneyTransferFormVM(); 

            return View(vm);
        }
    }

}