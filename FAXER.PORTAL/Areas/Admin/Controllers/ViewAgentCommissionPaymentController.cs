using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewAgentCommissionPaymentController : Controller
    {
        ViewAgentCommissionPaymentServices Service = new ViewAgentCommissionPaymentServices();
        CommonServices CommonService = new CommonServices();
        // GET: Admin/ViewAgentCommissionPayment
        public ActionResult Index(string CountryCode = "", string City = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            var vm = Service.getList(CountryCode, City);
            if (!string.IsNullOrEmpty(CountryCode))
            {
                ViewBag.Country = CountryCode;
            }
            if (!string.IsNullOrEmpty(City))
            {
                ViewBag.City = City;
            }
            
            
            return View(vm);
        }

        public ActionResult deletePayment(int id)
        {
            if (id != 0)
            {
                bool result = Service.deletePayment(id);
                if (result)
                {
                    return RedirectToAction("Index");
                }
                
            }
            return null;
        }

        public void SetViewBagForSCities(string Country = "")
        {
            var cities = SCity.GetCities(DB.Module.Agent, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }
        private void SetViewBagForCountries()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }
        public void PrinCommisionReceipt(int Id)
        {

            var AgentCommisionDetials = Service.GetCommissionPaymentDetials(Id);
            var AgentInfo = Service.GetAgentInformation(AgentCommisionDetials.AgentId);
            var staffName = CommonService.getStaffName(AgentCommisionDetials.VerifiedBy);
            var staffLoginInfo = Service.GetStaffLoginInfo(AgentCommisionDetials.VerifiedBy);
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            var ReceiptUrl = baseUrl + "/EmailTemplate/AgentCommisionPaymentReceipt?ReceiptNumber=" + AgentCommisionDetials.ReceiptNo+ 
                "&AgentName=" + AgentInfo.Name + "&AgentMFCode=" + AgentInfo.AccountNo + 
                "&TransferredCommision=" + AgentCommisionDetials.TotalSentCommission +  "&ReceivedCommission=" + AgentCommisionDetials.TotalReceivedCommission 
                + "&TotalCommission=" + AgentCommisionDetials.TotalCommission +  "&StaffName=" + staffName + 
                "&Date=" +AgentCommisionDetials.TransactionDateTime.ToString("dd/MM/yyyy") + "&Time=" + AgentCommisionDetials.TransactionDateTime.ToString("HH:mm") + "&AgentCurrency=" + CommonService.getCurrencyCodeFromCountry(AgentInfo.CountryCode) + 
                "&CommisionPaymentType=" + staffLoginInfo.LoginCode;

             var ReceiptPDF = Common.Common.GetPdf(ReceiptUrl);
            byte[] bytes = ReceiptPDF.Save();
            string mimeType = "Application/pdf";
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.Flush();
            Response.End();



        }
    }
}