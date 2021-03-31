using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class AgentCommisionController : Controller
    {
        DB.FAXEREntities dbContext = new DB.FAXEREntities();
        // GET: Agent/AgentCommision
        public ActionResult Index(string year = "", int monthId = 0)
        {

            AgentInformation agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            int StaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId;
            int agentId = agentInfo.Id;
            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            if (Common.AgentSession.AgentStaffLogin.AgentStaff.AgentStaffType == StaffType.Transaction)
            {
                return RedirectToAction("Index", "AgentDashboard");
            }
            var vm = new Models.AgentCommissionViewModel();
            vm.Year = year;
            vm.Month = (Month)monthId;
            vm.AgentMFSCode = agentInfo.AccountNo;
            vm.AgentName = agentInfo.Name;

            var AgentCurrency = Common.Common.GetCurrencySymbol(agentInfo.CountryCode);

            AgentServices.AgentCommissionServices services = new AgentServices.AgentCommissionServices();
            AgentServices.AgentCommonServices _commonServies = new AgentServices.AgentCommonServices();

            //var FaxedTransaction = services.GetFaxedCommission(year, monthId);
            //vm.TotalFaxedCommission = AgentCurrency +  Math.Round(FaxedTransaction, 2).ToString();


            //var ReceivedCommision = services.GetReceivedCommsion(year, monthId);
            //vm.TotalReceivedCommission = AgentCurrency+  Math.Round(ReceivedCommision, 2).ToString();

            // vm.FaxedAndReceivedCommission = AgentCurrency + Math.Round(FaxedTransaction + ReceivedCommision, 2).ToString();
          
            var SendingCommission = services.GetSendingCommission(year, monthId , StaffId);
            vm.TotalFaxedCommission = AgentCurrency + Math.Round(SendingCommission, 2).ToString();

            var ReceivingCommission = services.GetReceivingCommission(year, monthId, StaffId);
            vm.TotalReceivedCommission = AgentCurrency + Math.Round(ReceivingCommission, 2).ToString();

            vm.FaxedAndReceivedCommission = AgentCurrency + Math.Round(SendingCommission + ReceivingCommission, 2).ToString();

            if (!string.IsNullOrEmpty(year) && monthId > 0)
            {
                var Status = services.CommisionPaymentStatus(year, monthId);
                vm.status = Enum.GetName(typeof(AgentCommisionPaymentStatus), Status);
            }
            return View(vm);

        }

        public void PrintAgentCommsionReceipt(string Year, Month MonthId)
        {
            AgentServices.AgentCommissionServices service = new AgentServices.AgentCommissionServices();

            var AgentCommsionPaymentDetails = service.GetAgentCommissionPaymentDetials(Year,(int)MonthId);

            var AgentInfo = service.GetAgentInformation(AgentCommsionPaymentDetails.AgentId);

            var staffName = service.GetStaffName(AgentCommsionPaymentDetails.VerifiedBy);
            var AgentCurrency = Common.Common.GetCountryCurrency(AgentInfo.CountryCode);

            var staffLoginInfo = service.GetStaffLoginInfo(AgentCommsionPaymentDetails.VerifiedBy);

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            var ReceiptUrl = baseUrl + "/EmailTemplate/AgentCommisionPaymentReceipt?ReceiptNumber=" + AgentCommsionPaymentDetails.ReceiptNo +
                "&AgentName=" + AgentInfo.Name + "&AgentMFCode=" + AgentInfo.AccountNo +
                "&TransferredCommision=" + AgentCommsionPaymentDetails.TotalSentCommission +
                "&ReceivedCommission=" + AgentCommsionPaymentDetails.TotalReceivedCommission +
                "&TotalCommission=" + AgentCommsionPaymentDetails.TotalCommission + 
                "&StaffName=" + staffName + "&Date=" + AgentCommsionPaymentDetails.TransactionDateTime.ToString("dd/MM/yyyy")
                + "&Time=" + AgentCommsionPaymentDetails.TransactionDateTime.ToString("HH:mm") + "&AgentCurrency=" + AgentCurrency 
                +"&CommisionPaymentType=" + staffLoginInfo.LoginCode;

            var ReceiptPdf = Common.Common.GetPdf(ReceiptUrl);
            byte[] bytes = ReceiptPdf.Save();
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