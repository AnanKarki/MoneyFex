using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class FaxerEmailUpdateEmailController : Controller
    {
        // GET: EmailTemplate/FaxerEmailUpdateEmail
        public ActionResult Index(string SenderName , string UpdatedPara)
        {
            ViewBag.SenderName = SenderName;
            @ViewBag.WhatisUpdated = UpdatedPara;
            return View();
        }
    }
}