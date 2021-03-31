using FAXER.PORTAL.Areas.Businesses.Services;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Businesses.Controllers
{
    public class BusinessHomeController : Controller
    {
        BusinessMoneyTransferInProgressServices _businessMoneyTransferInProgressServices = null;

        CommonServices CommonServices = null;
        public BusinessHomeController()
        {
            _businessMoneyTransferInProgressServices = new BusinessMoneyTransferInProgressServices();
            CommonServices = new CommonServices();
        }

        public ActionResult Home() {

            DemoLoginModel model = new DemoLoginModel();
            model.UserName = "Demo";
            model.Password = "Demo123@";
            Common.FaxerSession.DemoLoginModel = model;

            return View();

        }
        // GET: Businesses/BusinessHome
        public ActionResult Index()
        {
            if (Common.BusinessSession.LoggedBusinessMerchant == null) {

                return RedirectToAction("Login", "BusinessLogin");
            }
            ViewBag.BusinessMobileNo = Common.BusinessSession.LoggedBusinessMerchant.BusinessMobileNo;
            

            Services.BusinessAlertsServices alertsServices = new Services.BusinessAlertsServices();
            var model = alertsServices.GetAlerts();
            ViewBag.AlertCount = model.Count();
            string FirstLogin = Common.BusinessSession.FirstLogin;
            ViewBag.FirstLogin = FirstLogin;

            Services.BusinessCardServices cardServices = new Services.BusinessCardServices();
            ViewBag.CardPhoto = cardServices.getCardPhoto(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId);

            var CardCount = cardServices.GetMFBCCardInformation(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId);
            if (CardCount == null)
            {
                ViewBag.CardCount = 0;
            }
            else {
                ViewBag.CardCount = CardCount.Id;
            }

            ViewBag.CreditonCard = Common.Common.GetCurrencySymbol(Common.BusinessSession.LoggedBusinessMerchant.CountryCode) + " " + cardServices.GetCreditOnCard();
            ViewBag.Currency = Common.Common.GetCurrencySymbol(Common.BusinessSession.LoggedBusinessMerchant.CountryCode);
            ViewBag.TransactionOnProgressCount = _businessMoneyTransferInProgressServices.GetTransferInProgressList().Count;
            Session.Remove("FaxingAmountSummary");
            Session.Remove("TransactionSummaryURL");
            Session.Remove("MFTCCardNumber");
            return View(model);
        }

        public ActionResult ClearAlert() {

            Session.Remove("FirstLogin");
            return RedirectToAction("Index");

        }
        public JsonResult GetAlertsFullDetails(int id)
        {
            
            Services.BusinessAlertsServices alertsServices = new Services.BusinessAlertsServices();
            var result = alertsServices.GetAlertsDetialsById(id);
            return Json(new
            {
                AlertHeading = result.Heading,
                AlertFullMessage = result.FullMessage,
                AlertPhoto = result.PhotoUrl
            }, JsonRequestBehavior.AllowGet);

        }


        public JsonResult GetWithdrawalCode()
        {
            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;



            DB.FAXEREntities db = new DB.FAXEREntities();


            var data = db.KiiPayBusinessWalletWithdrawalCode.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId && x.IsExpired == false).FirstOrDefault();

            if (data == null)
            {

                DB.KiiPayBusinessWalletWithdrawalCode BusinessWithdrawalCode = new DB.KiiPayBusinessWalletWithdrawalCode()
                {
                    KiiPayBusinessInformationId = KiiPayBusinessInformationId,
                    AccessCode = CommonServices.GetNewAccessCodeForBusinessMerchant(),
                    IsExpired = false,
                    CreatedDateTime = DateTime.Now,

                };
                data = db.KiiPayBusinessWalletWithdrawalCode.Add(BusinessWithdrawalCode);
                db.SaveChanges();

            }

            return Json(new
            {
                AccessCode = data.AccessCode
            }, JsonRequestBehavior.AllowGet);

        }
    }
}