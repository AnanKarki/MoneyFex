using FAXER.PORTAL.Areas.Agent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class AdminSourceOfFundDeclarationController : Controller
    {
        // GET: Admin/AdminSourceOfFundDeclaration
        public ActionResult Index()
        {
            List<SourceOfFundDeclarationGridVM> vm = new List<SourceOfFundDeclarationGridVM>();
            return View(vm);
        }

        public ActionResult SubmitSourceOfFund()
        {
            SourceOfFundDeclarationVM vm = new SourceOfFundDeclarationVM();
            return View(vm);
        }
    }
}