using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class ReceiverDetailsController : Controller
    {
        // GET: ReceiverDetails
        FAXER.PORTAL.DB.FAXEREntities dbContext = new DB.FAXEREntities();
        CommonServices CommonService = new CommonServices();
        public ActionResult Index()
        {
            Models.ReceiversDetailsViewModel model = new Models.ReceiversDetailsViewModel();
            var receivingCountryCode = Session["ReceivingCountry"].ToString();
            model.ReceiverCountry = dbContext.Country.Where(x => x.CountryCode.Equals(receivingCountryCode)).Select(x => x.CountryName).FirstOrDefault();
            model.CountryPhoneCode = CommonService.getPhoneCodeFromCountry(receivingCountryCode);
            if (Common.FaxerSession.ReceiversDetails != null){
                model = Common.FaxerSession.ReceiversDetails;
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult SaveReceiverDetails([Bind(Include = ReceiversDetailsViewModel.BindProperty)]ReceiversDetailsViewModel receiversDetailsModel)
        {
            if (ModelState.IsValid)
            {
                var EmailExist = dbContext.ReceiversDetails.Where(x => x.EmailAddress == receiversDetailsModel.ReceiverEmailAddress).FirstOrDefault();
                if (EmailExist != null) {
                    ModelState.AddModelError("ReceiverEmailAddress", "This Receiver's email address is already registered in the system");
                    return View("Index", receiversDetailsModel);
                }
                FaxerSession.ReceiversDetails = receiversDetailsModel;
                if (!string.IsNullOrEmpty(Common.FaxerSession.TransactionSummaryUrl)) {
                    return Redirect(Common.FaxerSession.TransactionSummaryUrl);
                }
                return RedirectToAction("Index", "FraudAlert", new { FormURL = "/PaymentMethod/ReciverPaymentMethod", BackUrl = "/ReceiverDetails/Index" });
                //return RedirectToAction("ReciverPaymentMethod", "PaymentMethod");
            }
            return View("Index", receiversDetailsModel);
        }
    }
}