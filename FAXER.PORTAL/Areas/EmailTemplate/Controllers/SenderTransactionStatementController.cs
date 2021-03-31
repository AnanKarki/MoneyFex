using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class SenderTransactionStatementController : Controller
    {
        // GET: EmailTemplate/SenderTransactionStatement
        public ActionResult Index(int SenderId=0,int year=0)
        {
            CommonServices CommonService = new CommonServices();
            if (SenderId != 0)
            {
                ViewBag.SenderName = CommonService.GetSenderName(SenderId);
                ViewBag.SenderAccountNo = CommonService.GetSenderAccountNoBySenderId(SenderId);
                ViewBag.SenderCountry = CommonService.getCountryNameFromCode(CommonService.GetSenderCountry(SenderId));
            }
            ViewRegisteredFaxersServices faxer = new ViewRegisteredFaxersServices();
            var result = faxer.GetNewTransactionStatement(SenderId, year);
            result.Monthly = faxer.GetMonthlyTransactionMeter(result.TransactionListDownload, SenderId);

            return View(result);

        }
       
    }
  
}