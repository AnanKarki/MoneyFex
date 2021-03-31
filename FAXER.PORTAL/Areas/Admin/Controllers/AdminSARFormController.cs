using FAXER.PORTAL.Areas.Agent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class AdminSARFormController : Controller
    {
        // GET: Admin/AdminSARForm
        public ActionResult Index()
        {
            List<SARFormVM> vm = new List<SARFormVM>();
            return View(vm);
        }

        public ActionResult SubmitSARForm()
        {
            SARFormVM vm = new SARFormVM();
            return View(vm);
        }

    }
}