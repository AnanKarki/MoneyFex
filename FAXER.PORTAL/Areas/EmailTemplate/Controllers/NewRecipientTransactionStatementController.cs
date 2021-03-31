using FAXER.PORTAL.Areas.Admin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate.Controllers
{
    public class NewRecipientTransactionStatementController : Controller
    {
        // GET: EmailTemplate/NewRecipientTransactionStatement
        public ActionResult Index(int ReceiverId = 0, int year = 0)
        {
            ReceiverServices _services = new ReceiverServices() ;
            if (ReceiverId != 0)
            {
                var receiverInfo = _services.receiverInfoByReceiverId(ReceiverId);
                ViewBag.ReceiverName = receiverInfo.ReceiverName;
                ViewBag.ReceiverTelephoneNo = Common.Common.GetCountryName(receiverInfo.MobileNo);
                ViewBag.ReceiverCountryName = Common.Common.GetCountryName(receiverInfo.Country);
           
            }
            int pageSize = 10;
            int pageNumber = 1;

            var result = _services.GetNewReceiverTransactionStatement(ReceiverId, year, pageSize, pageNumber);


            return View(result);
        }
    }
}