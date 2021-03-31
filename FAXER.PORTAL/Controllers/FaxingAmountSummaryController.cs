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
    public class FaxingAmountSummaryController : Controller
    {
        CommonServices CommonService = new CommonServices();
        // GET: FaxingAmountSummary

        [HandleError]
        public ActionResult Index(Models.EstimateFaxingFeeSummary model)
        {
          
            try
            {

                if (Session["SummaryFaxingAmount"] != null)
                {
                    model.FaxingAmount = Convert.ToDecimal(Session["SummaryFaxingAmount"]);
                    model.FaxingFee = Convert.ToDecimal(Session["SummaryFaxingFee"]);
                    model.TotalAmount = Convert.ToDecimal(Session["SummaryTotalAmount"]);
                    model.ExchangeRate = Convert.ToDecimal(Session["SummaryExchangeRate"]);
                    model.ReceivingAmount = Convert.ToDecimal(Session["SummaryReceivingAmount"]);
                    
                }
                if (Common.FaxerSession.FaxingAmountSummary != null) {


                    model = Common.FaxerSession.FaxingAmountSummary;

                }
            }
            catch (Exception)
            {


            }
            model.FaxingCurrency = CommonService.getCurrencyCodeFromCountry(Common.FaxerSession.FaxingCountry);
            model.FaxingCurrencySymbol = CommonService.getCurrencySymbol(FaxerSession.FaxingCountry);
            model.ReceivingCurrency = CommonService.getCurrencyCodeFromCountry(FaxerSession.ReceivingCountry);
            model.ReceivingCurrencySymbol = CommonService.getCurrencySymbol(FaxerSession.ReceivingCountry);
            return View(model);
        }
        [HttpPost]
        [HandleError]
        public ActionResult FeeSummary([Bind(Include = EstimateFaxingFeeSummary.BindProperty)]EstimateFaxingFeeSummary summaryModel)
            {
            try
            {
                FaxerSession.FaxingAmountSummary = summaryModel;
                //Session["SummaryFaxingAmount"] = summaryModel.FaxingAmount;
                //Session["SummaryFaxingFee"] = summaryModel.FaxingFee;
                //Session["SummaryTotalAmount"] = summaryModel.TotalAmount;
                //Session["SummaryExchangeRate"] = summaryModel.ExchangeRate;
                //Session["SummaryReceivingAmount"] = summaryModel.ReceivingAmount;
            }
            catch (Exception)
            {


            }
            if (!string.IsNullOrEmpty(Common.FaxerSession.TransactionSummaryUrl)) {
                return Redirect(Common.FaxerSession.TransactionSummaryUrl);
            }
            return RedirectToAction("Index", "ReceiverDetails");
        }
    }
}