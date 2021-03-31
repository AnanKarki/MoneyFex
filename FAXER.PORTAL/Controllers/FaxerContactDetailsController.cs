using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class FaxerContactDetailsController : Controller
    {
        // GET: FaxerContactDetails
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [HandleError]
        public ActionResult Index([Bind(Include = FaxerContactDetails.BindProperty)]FaxerContactDetails model)
        {
            if (ModelState.IsValid)
            {
                

                
                return RedirectToAction("Register", "Account");

                
            }
            return View();
        }
    }
}