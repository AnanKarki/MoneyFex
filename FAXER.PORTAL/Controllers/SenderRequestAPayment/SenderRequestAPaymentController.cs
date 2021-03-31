using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.SenderRequestAPayment
{
    public class SenderRequestAPaymentController : Controller
    {
        // GET: SenderRequestAPayment
        public ActionResult Index()
        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            ViewBag.WalletCountOfSelf = senderCommonFunc.GetwalletCountOfSelf(Common.FaxerSession.LoggedUser.Id);
            return View();
        }

       
    }
}