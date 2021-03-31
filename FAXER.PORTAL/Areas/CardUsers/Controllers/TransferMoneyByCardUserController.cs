using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.CardUsers.Controllers
{
    public class TransferMoneyByCardUserController : Controller
    {
        // GET: CardUsers/TransferMoneyByCardUser
        public ActionResult Index()
        {
            if (Common.CardUserSession.LoggedCardUserViewModel == null) {

                return RedirectToAction("Login", "CardUserLogin");
            }
            return View();
        }
    }
}