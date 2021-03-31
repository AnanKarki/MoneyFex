using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class SenderDashBoardController : Controller
    {
        DB.FAXEREntities dbContext = new FAXEREntities();
        SenderCommonFunc funcServices = null;
        // GET: DashBoard

        public SenderDashBoardController()
        {
            funcServices = new SenderCommonFunc(dbContext);
        }
        public ActionResult Index()
        {

            if ((FaxerSession.LoggedUser == null))
            {
                FaxerSession.FromUrl = "/SenderDashBoard";
                return RedirectToAction("Login", "FaxerAccount");
            }

            ViewBag.TransationInProgressCount = Common.Common.GetTransactionInProgressCount();
            ViewBag.FaxerAccountNo = Common.FaxerSession.LoggedUser.FaxerMFCode;

            SSenderCashPickUp SenderCashPickServices = new SSenderCashPickUp();
            ViewBag.CashPickUpinProgessCount = SenderCashPickServices.GetCashPickUpInProgressTransCount(Common.FaxerSession.LoggedUser.Id);
            Session.Remove("FaxingAmountSummary");
            Session.Remove("TransactionSummaryUrl");
            Session.Remove("CreditDebitDetails");
            Session.Remove("SavedCreditDebitCardDetails");
            Session.Remove("ReceivingCountry");
            Session.Remove("NonCardReceiverId");
            Session.Remove("FromUrl");
            Session.Remove("CommonEnterAmountViewModel");

            funcServices.ClearCashPickUpSession();
            funcServices.ClearFamilyAndFriendSession();
            funcServices.ClearKiiPayTransferSession();
            funcServices.ClearPayBillsSession();
            funcServices.ClearTransferBankDepositSession();
            funcServices.ClearPayForServiceSession();
            funcServices.ClearMobileTransferSession();

            //Session.Remove("IsTransferFromHomePage");

            Common.FaxerSession.IsTransferFromHomePage = false;

            Common.FaxerSession.BackButtonURL = Request.Url.ToString();

            Common.FaxerSession.BackButtonURLMyMoneyFex = Request.Url.ToString();
            var balance = funcServices.GetMonthlyTransactionMeter(Common.FaxerSession.LoggedUser.Id);
            SenderMonthlyTransactionMeterViewModel Vm = new SenderMonthlyTransactionMeterViewModel();
            Vm.SenderMonthyTransactionMeterBalance = balance;
            Vm.SenderCurrencySymbol  = Common.Common.GetCurrencySymbol(Common.FaxerSession.LoggedUser.CountryCode);
            ViewBag.TrackingNo = Common.Common.GetTrackingNo("Dashboard");
            return View(Vm);
        }


    }
}