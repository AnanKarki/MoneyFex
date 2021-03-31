using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class FormAgentCommissionPaymentController : Controller
    {
        FormAgentCommissionPaymentServices Service = new FormAgentCommissionPaymentServices();
        CommonServices CommonService = new CommonServices();
        // GET: Admin/FormAgentCommissionPayment
        public ActionResult Index(string year = "", int monthId = 0, string CountryCode = "", string City = "", string agent = "", string message="")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));
            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            SetViewBagForAgents(CountryCode, City);
            if (message== "success")
            {
                ViewBag.Message = "Agent Commsion Payment successful Completed!";
                ViewBag.Value = 0;
                message = "";
            }


            int agentId = agent == "" ? 0 : Convert.ToInt32(agent);


            //var vm = new FormAgentCommissionPaymentViewModel();
            FormAgentCommissionPaymentViewModel vm = new FormAgentCommissionPaymentViewModel();
            vm.Month = (Month)monthId;
            if (!string.IsNullOrEmpty(CountryCode))
            {
                vm.Country = CountryCode;
            }
            if (!string.IsNullOrEmpty(year))
            {
                vm.Year = year;
            }
            if (!string.IsNullOrEmpty(City))
            {
                vm.City = City;
            }
            if (agentId != 0 && monthId != 0 && !string.IsNullOrEmpty(year) && !string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                vm.AgentId = agentId;
                var CommissionRateAvailable = Service.CommissionRateSetup(CountryCode);
                if (CommissionRateAvailable == true)
                {
                    vm = Service.getInfo(vm);
                }
                else {
                 
                        ViewBag.Message = "Agent commision rate has not Setup for this Country!";
                        ViewBag.Value = 1;
                        message = "";
                

                }
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = FormAgentCommissionPaymentViewModel.BindProperty)]FormAgentCommissionPaymentViewModel model)
        {
            if (model != null)
            {
                bool paymentDone = Service.checkPayment(model.AgentId, model.Year, model.Month);
                if (paymentDone)
                {
                    ModelState.AddModelError("AgentId", "Payment for this agent for this date is already done !");
                    ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));
                    SetViewBagForCountries();
                    SetViewBagForSCities(model.Country);
                    SetViewBagForAgents(model.Country, model.City);
                    return View(model);

                }
                if (model.TotalCommission == 0)
                {
                    ModelState.AddModelError("TotalCommission", "Sorry, there's no amount to be paid !");
                    ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));
                    SetViewBagForCountries();
                    SetViewBagForSCities(model.Country);
                    SetViewBagForAgents(model.Country, model.City);
                    return View(model);
                }

                bool valid = true;
                if (model.IsVerified == false)
                {
                    ModelState.AddModelError("IsVerified", "Please verify all the information before continuing !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.Year))
                {
                    ModelState.AddModelError("Year", "This field can't be blank !");
                    valid = false;
                }
                if (model.Month == 0)
                {
                    ModelState.AddModelError("Month", "This field can't be blank !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.Country))
                {
                    ModelState.AddModelError("Country", "This field can't be blank !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.City))
                {
                    ModelState.AddModelError("City", "This field can't be blank !");
                    valid = false;
                }
                if (model.AgentId == 0)
                {
                    ModelState.AddModelError("AgentId", "This field can't be blank !");
                    valid = false;
                }
                if (valid == true)
                {
                    bool result = Service.savePaymentDetails(model);
                    if (result)
                    {
                        return RedirectToAction("Index", new { @message="success" });
                    }
                }
            }
            ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));
            SetViewBagForCountries();
            SetViewBagForSCities(model.Country);
            SetViewBagForAgents(model.Country, model.City);
            return View(model);
        }

        private void SetViewBagForCountries()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }
        private void SetViewBagForSCities(string Country = "")
        {
            var cities = SCity.GetCities(DB.Module.Agent, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }
        private void SetViewBagForAgents(string CountryCode = "", string City = "")
        {
            var agents = Service.GetAgents(CountryCode, City);
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");
        }

        //public void PrinCommisionReceipt(string Id)
        //{

        //    AgentServices.RefundRequestServices services = new AgentServices.RefundRequestServices();
        //    var nonCardTrans = services.GetFaxingNonCardTransaction(MFCN);

        //    var RefundDetails = services.GetRefundNonCardDetails(nonCardTrans.Id);

        //    var ReceiverInfo = services.GetReceiversDetails(nonCardTrans.NonCardRecieverId);
        //    var FaxerInfo = services.GetFaxerInformation(ReceiverInfo.FaxerID);
        //    var agentInfo = services.GetAgentInformation(RefundDetails.Agent_id);
        //    string ReceiverPhoneCode = Common.Common.GetCountryPhoneCode(ReceiverInfo.Country);
        //    string FaxerCuurency = Common.Common.GetCountryCurrency(FaxerInfo.Country);
        //    var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            

        //    var ReceiptPdf = Common.Common.GetPdf(ReceiptUrl);
        //    var ReceiptPDF = Common.Common.GetPdf(ReceiptUrl);
        //    byte[] bytes = ReceiptPDF.Save();
        //    string mimeType = "Application/pdf";
        //    Response.Buffer = true;
        //    Response.Clear();
        //    Response.ContentType = mimeType;
        //    Response.OutputStream.Write(bytes, 0, bytes.Length);
        //    Response.Flush();
        //    Response.End();



        //}
    }
}