using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class CashWithdrawalController : Controller
    {
        AgentInformation agentInfo = null;
        CommonServices _commonServices = null;
        AgentCommonServices _agentCommonServices = null;
        CashWithdrawalServices _cashWithdrawalServices = null;
        public CashWithdrawalController()
        {
            agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            _commonServices = new CommonServices();
            _agentCommonServices = new AgentCommonServices();
            _cashWithdrawalServices = new CashWithdrawalServices();
        }

        // GET: Agent/CashWithdrawal
        public ActionResult Index(int withdrawal = 0, int month = 0, int year = 0, int transactionType = 0, string searchText = "")
        {
            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            if (Common.AgentSession.AgentStaffLogin.AgentStaff.AgentStaffType == StaffType.Transaction)
            {
                return RedirectToAction("Index", "AgentDashboard");
            }
            ViewBag.Years = new SelectList(Enumerable.Range(2018, 10));
            CashWithdrawalViewModel vm = new CashWithdrawalViewModel();
            vm.NameOfAgent = Common.AgentSession.AgentInformation.Name;
            vm.AgentAccountNumber = Common.AgentSession.AgentInformation.AccountNo;
            vm.AgentCountry = _commonServices.getCountryNameFromCode(Common.AgentSession.AgentInformation.CountryCode);
            vm.AgentCity = Common.AgentSession.AgentInformation.City;
            vm.AccountBalance = _agentCommonServices.getAgentAccountBalance(Common.AgentSession.AgentInformation.Id);
            vm.WithdrawalList = _cashWithdrawalServices.getCashWithdrawalList();
            if (withdrawal != 0)
            {
                vm.Withdrawal = (CashWithdrawalType)withdrawal;
            }
            if (month != 0)
            {
                vm.WithdrawalList = vm.WithdrawalList.Where(x => x.DateAndTime.Month == month).ToList();
                vm.Month = (Month)month;
            }
            if (year != 0)
            {
                vm.WithdrawalList = vm.WithdrawalList.Where(x => x.DateAndTime.Year == year).ToList();
                vm.Year = year;
            }
            if (transactionType != 0)
            {
                vm.WithdrawalList = vm.WithdrawalList.Where(x => x.WithdrawalType == (CashWithdrawalType)transactionType).ToList();
                vm.TransactionType = (CashWithdrawalType)transactionType;
            }
            if (!string.IsNullOrEmpty(searchText))
            {
                vm.SearchText = searchText;
            }
            foreach (var item in vm.WithdrawalList)
            {
                item.FormatedDateTime = item.DateAndTime.ToString("dd/MM/yyyy HH:mm");
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult CashWithdrawalByAgent()
        {
            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            AgentWithdrawViewModel vm = new AgentWithdrawViewModel();
            vm.AgentName = Common.AgentSession.AgentInformation.Name;
            vm.AgentAccountNumber = Common.AgentSession.AgentInformation.AccountNo;
            vm.AgentStaffName = Common.AgentSession.LoggedUser.PayingAgentStaffName;
            ViewBag.TransactionSuccessul = 0;
            ViewBag.TransactionId = 0;
            ViewBag.IsWithdrawalByAgent = true;
            return View(vm);

        }

        [HttpPost]
        public ActionResult CashWithdrawalByAgent([Bind(Include = AgentWithdrawViewModel.BindProperty)] AgentWithdrawViewModel model)
        {
            if (ModelState.IsValid)
            { 
                if (string.IsNullOrEmpty(model.AgentStaffName))
                {
                    ModelState.AddModelError("AgentStaffName", "Please enter the paying agent name.");
                    return View(model);
                }
                if (model.WithdrawAmount == 0)
                {
                    ModelState.AddModelError("WithdrawAmount", "Please enter the amount above zero {0}");
                    return View(model);
                }
                var saveData = _cashWithdrawalServices.saveCashWithdrawalByAgent(model);
                if (saveData != null)
                {
                    model.WithdrawAmount = 0;
                    ViewBag.TransactionSuccessul = 1;
                    ViewBag.TransactionId = saveData.Id;
                    ViewBag.IsWithdrawalByAgent = true;
                    
                    return View(model);
                }

            }

            return View(model);
        }
        [HttpGet]
        public ActionResult CashWithdrawalByStaff(string staffCode = "", string WithdrawalCode = "")
        {


            AgentResult agentResult = new AgentResult();
            if (agentInfo.Id == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            SetViewBagForIDCard();
            SetViewBagForCountries();
            StaffWithdrawalViewModel vm = new StaffWithdrawalViewModel();
            if (!string.IsNullOrEmpty(staffCode))
            {
                bool staffExists = _cashWithdrawalServices.isStaffExist(staffCode);
                if (staffExists == false)
                {
                    //agentResult.Message = "Please enter a valid staff code";
                    //agentResult.Status = ResultStatus.Warning;

                    ModelState.AddModelError("Invalid", "Please enter a valid staff code");
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
                if (staffExists == true && string.IsNullOrEmpty(WithdrawalCode))
                {

                    ViewBag.AgentResult = agentResult;

                    ViewBag.StaffCode = staffCode;
                    ViewBag.WithdrawalCodeIsEntered = 0;
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
                else
                {

                    var withdrawalCodeIsValid = _cashWithdrawalServices.IsValidWithdrawalCode(WithdrawalCode, staffCode);
                    if (withdrawalCodeIsValid == false)
                    {
                        //agentResult.Message = "Please enter a valid withdrawl code";
                        //agentResult.Status = ResultStatus.Warning;
                        ModelState.AddModelError("InvalidCode", "Please enter a valid withdrawl code");
                        ViewBag.AgentResult = agentResult;

                        ViewBag.StaffCode = staffCode;
                        ViewBag.WithdrawalCodeIsEntered = 1;
                        return View(vm);
                    }

                    Common.AgentSession.CashWithdrawalCode = WithdrawalCode;


                }
                vm = _cashWithdrawalServices.getStaffData(staffCode);
            }
            vm.AgentAccountNo = Common.AgentSession.AgentInformation.AccountNo;
            vm.AgentName = Common.AgentSession.AgentInformation.Name;
            //agentResult.Message = "Please enter the valid staff code.";
            //agentResult.Status = ResultStatus.Warning;
            ViewBag.AgentResult = agentResult;
            ViewBag.TransactionSuccessul = 0;
            ViewBag.TransactionId = 0;
            ViewBag.IsWithdrawalByAgent = false;
            return View(vm);
        }

        [HttpPost]
        public ActionResult CashWithdrawalByStaff([Bind(Include = StaffWithdrawalViewModel.BindProperty)] StaffWithdrawalViewModel model)
        {

            AgentResult agentResult = new AgentResult();
            SetViewBagForIDCard();
            SetViewBagForCountries();
            if (model != null)
            {
                bool isValid = true;
                if (model.StaffId == 0)
                {
                    ModelState.AddModelError("StaffCode", "Please provide valid staff code !");
                    isValid = false;
                }
                if (model.IDType == 0)
                {
                    ModelState.AddModelError("IDType", "Please choose a ID Card Type !");
                    isValid = false;
                }
                if (string.IsNullOrEmpty(model.IDNumber))
                {
                    ModelState.AddModelError("IDNumber", "This field can't be blank !");
                    isValid = false;
                }
                if (model.IDExpiryDate == default(DateTime))
                {
                    ModelState.AddModelError("IDExpiryDate", "This field can't be blank !");
                    isValid = false;
                }
                if (model.IDExpiryDate <= DateTime.Now)
                {
                    ModelState.AddModelError("IDExpiryDate", "This ID Card is expired !");
                    isValid = false;
                }
                if (string.IsNullOrEmpty(model.IssuingCountry))
                {
                    ModelState.AddModelError("IssuingCountry", "Please choose a country !");
                    isValid = false;
                }
                if (string.IsNullOrEmpty(model.AgentStaffName))
                {
                    ModelState.AddModelError("AgentStaffName", "This field can't be blank !");
                    isValid = false;
                }
                if (model.WithdrawalAmount == 0)
                {
                    ModelState.AddModelError("WithdrawalAmount", "This field can't be blank !");
                    isValid = false;
                }
                if (model.ConfirmVerification == false)
                {
                    ModelState.AddModelError("ConfirmVerification", "Please confirm that you have verifed all the provided information !");
                    isValid = false;
                }
                if (isValid == true)
                {
                    var saveData = _cashWithdrawalServices.saveCashWithdrawalByStaff(model);
                    bool UpdateWithdrawalCodeStatus = _cashWithdrawalServices.UpdateCashWithdrawalCode(Common.AgentSession.CashWithdrawalCode);

                    if (saveData != null)
                    {
                        //agentResult.Message = "Payment completed Success";
                        //agentResult.Status = ResultStatus.OK;
                        ModelState.Clear();
                        ViewBag.TransactionSuccessul = 1;
                        ViewBag.TransactionId = saveData.Id;
                        ViewBag.IsWithdrawalByAgent = false;
                        ViewBag.AgentResult = agentResult;
                        return View();
                    }
                }
            }
            ViewBag.AgentResult = agentResult;
            return View(model);

        }

        private void SetViewBagForIDCard()
        {
            var idCard = _agentCommonServices.GetIDCardTypes();
            ViewBag.IDCardDropdown = new SelectList(idCard, "Id", "CardType");
        }

        private void SetViewBagForCountries()
        {
            var countries = _commonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }


        public void PrintCashWithdrawalReceipt(int transactionId, bool IsWithdrawalByAgent = false)
        {



            var ReceiptDetails = _cashWithdrawalServices.GetCashWithdrawalReceiptDetails(transactionId, IsWithdrawalByAgent);

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            var ReceiptUrl = baseUrl + "/EmailTemplate/CashWithdrawalReceipt?ReceiptNo=" + ReceiptDetails.ReceiptNo
                                                        + "&Date=" + ReceiptDetails.Date + "&Time=" + ReceiptDetails.Time + "&AgentName=" + ReceiptDetails.AgentName
                                                        + "&AgentAccountNo=" + ReceiptDetails.AgentAccountNO + "&WithdrawalType="
                                                        + ReceiptDetails.WithDrawalType + "&StaffName=" + ReceiptDetails.StaffName + "&StaffCode=" + ReceiptDetails.StaffCode
                                                        + "&WithdrawalCode=" + ReceiptDetails.WithdrawalCode + "&AdminCodeGenerator=" + ReceiptDetails.AdminCodeGenerator
                                                        + "&WithdrawalAmount=" + ReceiptDetails.WithdrawalAmount + "&Currency=" + ReceiptDetails.Currency
                                                        + "&IsWithdrawalByAgent=" + ReceiptDetails.IsWithdrawalByAgent;

            var ReceiptPdf = Common.Common.GetPdf(ReceiptUrl);

            //var path = HostingEnvironment.MapPath(@"/Documents/") + "attachment.pdf";
            //ReceiptPdf.Save(path);
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