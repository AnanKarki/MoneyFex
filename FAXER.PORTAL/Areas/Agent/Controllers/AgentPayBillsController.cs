using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class AgentPayBillsController : Controller
    {
        FAXER.PORTAL.Services.SSuppliers _suppliersServices = null;

        SPayBill _paybillServices = null;
        // GET: Agent/AgentPayBills
        SAgentInformation _sFaxerInfromationServices = null;
        STopUpToSupplier _TopUpToSupplierServices = null;
        public AgentPayBillsController()
        {
            _paybillServices = new SPayBill();
            _suppliersServices = new Services.SSuppliers();
            _TopUpToSupplierServices = new STopUpToSupplier();
        }
        public ActionResult Index()
        {
            return View();
        }
        public List<Country> GetCountries()
        {
            var countries = Common.Common.GetCountries();
            List<Country> list = (from c in countries.ToList()
                                  join d in _suppliersServices.List().Data.ToList() on c.CountryCode equals d.Country
                                  select c).GroupBy(x => x.CountryName).Select(x => x.FirstOrDefault()).ToList();
            return list;



        }
        public string getAgentCountryCode(int agentId = 0)
        {
            _sFaxerInfromationServices = new SAgentInformation();
            var result = _sFaxerInfromationServices.list().Data.Where(x => x.Id == agentId).Select(x => x.CountryCode).FirstOrDefault();
            return result;
        }
        public List<SupplierDropDownVM> GetSuppliers(string country = "")
        {
            int AgentId = Common.AgentSession.LoggedUser.PayingAgentStaffId;
            var data = _suppliersServices.List().Data;
            var agentCountry = getAgentCountryCode(AgentId);
            if (!string.IsNullOrEmpty(country))
            {

                data = data.Where(x => x.Country.ToLower() == country.ToLower());
            }
            else
            {
                data = data.Where(x => x.Country.ToLower() == agentCountry);
            }
            var result = (from c in data.ToList()
                          select new SupplierDropDownVM()
                          {
                              Id = c.Id,
                              Name = c.KiiPayBusinessInformation.BusinessName,
                              ReferenceNo = c.RefCode,
                          }).GroupBy(x => x.Name).Select(x => x.FirstOrDefault()).ToList();
            return result;

        }


        #region Pay a Monthly Bill

        [HttpGet]
        public ActionResult PayAMonthlyBill(string country = "")
        {
            ViewBag.Countries = new SelectList(GetCountries(), "CountryCode", "CountryName");
            ViewBag.Suppliers = new SelectList(GetSuppliers(country), "Id", "Name");
            PayMonthlyBillViewModel vm = _paybillServices.GetPayMonthlyBillViewModel();
            return View(vm);
        }

        [HttpPost]
        public ActionResult PayAMonthlyBill([Bind(Include = PayMonthlyBillViewModel.BindProperty)]PayMonthlyBillViewModel vm)
        {
            ViewBag.Countries = new SelectList(GetCountries(), "CountryCode", "CountryName");
            ViewBag.Suppliers = new SelectList(GetSuppliers(vm.Country), "Id", "Name");
            var reference = "HV8" + vm.ReferenceNo;
            vm.ReferenceNo = reference;
            var suppliers = _suppliersServices.List().Data.Where(x => x.RefCode == reference && x.Country == vm.Country).FirstOrDefault();

            if (ModelState.IsValid)
            {
                _paybillServices.SetPayMonthlyBillViewModel(vm);
                if (suppliers != null)
                {
                    return RedirectToAction("PayingSupplierReference", vm);
                }
                else
                {
                    ModelState.AddModelError("ReferenceNo", "Enter a valid reference no");
                }

            }
            return View(vm);
        }

        public ActionResult PayingSupplierReference([Bind(Include = PayMonthlyBillViewModel.BindProperty)] PayMonthlyBillViewModel vm)
        {
            int AgentId = Common.AgentSession.AgentInformation.Id;
            var supplier = _suppliersServices.List().Data.Where(x => x.RefCode == vm.ReferenceNo).FirstOrDefault();
            var agentCountry = getAgentCountryCode(AgentId);
            var transactionsummary = SEstimateFee.CalculateFaxingFee(4350M, false, true,
                                                        SExchangeRate.GetExchangeRateValue(agentCountry, supplier.Country)
                                                         , SEstimateFee.GetFaxingCommision(agentCountry));
            var agentCommission = Common.Common.GetAgentSendingCommission(TransferService.PayBills, AgentId, transactionsummary.FaxingAmount, transactionsummary.FaxingFee);
            PayingSupplierReferenceViewModel model = new PayingSupplierReferenceViewModel()
            {
                PhotoUrl = "",
                ReceiverName = supplier.KiiPayBusinessInformation.BusinessName,
                ReferenceNo = vm.ReferenceNo,
                BillNo = "39658607",
                Amount = transactionsummary.ReceivingAmount,
                Fee = transactionsummary.FaxingFee,
                Total = transactionsummary.TotalAmount,
                Currency = Common.Common.GetCountryCurrency(agentCountry),
                ExchangeRate = transactionsummary.ExchangeRate,
                AgentCommission = agentCommission,
                CurrencySymbol = Common.Common.GetCurrencySymbol(agentCountry),
            };
            _paybillServices.SetAgentPayingSupplierReference(model);
            return View(model);

        }

        [HttpPost]
        public ActionResult PayingSupplierReference([Bind(Include = PayingSupplierReferenceViewModel.BindProperty)] PayingSupplierReferenceViewModel model)
        {

            int AgentId = Common.AgentSession.AgentInformation.Id;
            int PayingStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId;
            var AgentPayingSupplierAbroad = _paybillServices.GetAgentPayingSupplierReference();
            var agentCountry = getAgentCountryCode(AgentId);
            var supplier = _suppliersServices.List().Data.Where(x => x.RefCode == AgentPayingSupplierAbroad.ReferenceNo).FirstOrDefault();
            var result = SEstimateFee.CalculateFaxingFee(AgentPayingSupplierAbroad.Amount, false, false,
                AgentPayingSupplierAbroad.ExchangeRate, SEstimateFee.GetFaxingCommision(agentCountry));


            if (supplier != null)
            {
                PayBill payBill = new PayBill()
                {
                    BillNo = AgentPayingSupplierAbroad.BillNo,
                    Amount = AgentPayingSupplierAbroad.Amount,
                    Fee = AgentPayingSupplierAbroad.Fee,
                    Total = AgentPayingSupplierAbroad.Total,
                    PayerId = AgentId,
                    Module = Module.Agent,
                    RefCode = AgentPayingSupplierAbroad.ReferenceNo,
                    SupplierId = supplier.Id,
                    PayerCountry = Common.Common.GetCountryName(getAgentCountryCode(AgentId)),
                    SupplierCountry = Common.Common.GetCountryName(supplier.Country),
                    PaymentDate = DateTime.Now,
                    ExchangeRate = AgentPayingSupplierAbroad.ExchangeRate,
                    SendingAmount = AgentPayingSupplierAbroad.Amount,
                    PayingStaffName = Common.AgentSession.LoggedUser.PayingAgentStaffName,
                    AgentCommission = AgentPayingSupplierAbroad.AgentCommission,
                    PayingStaffId = PayingStaffId,
                    ReceiptNo = Common.Common.GenerateAgentPayBillMonthlyReceiptNo(6),

                };
                if (payBill.PayerCountry == payBill.SupplierCountry)
                {
                    payBill.PaymentType = PaymentType.Local;
                }
                else
                {
                    payBill.PaymentType = PaymentType.International;
                }
                var PaybillTransaction = _paybillServices.Add(payBill).Data;
                return RedirectToAction("PayBillsReferenceSuccess", "AgentPayBills", new { Id = PaybillTransaction.Id });
            }
            return View(model);
        }

        public ActionResult PayBillsReferenceSuccess(int Id)
        {
            var data = _paybillServices.GetAgentPayingSupplierReference();
            PayBillsReferenceSuccessViewModel model = new PayBillsReferenceSuccessViewModel()
            {
                ReferenceNo = data.ReferenceNo,
                ReceiverName = data.ReceiverName,
            };
            //AgentCommonServices cs = new AgentCommonServices();
            //cs.ClearAgentPayBillsMonthly();
            ViewBag.TransactionId = Id;
            return View(model);
        }

        public void PrintReceiptOfPayBillMonthly(int TransactionId)
        {

            var senderInfo = _paybillServices.GetPayMonthlyBillViewModel();
            var transaction = _paybillServices.GetAgentPayingSupplierReference();
            var PayBillTransaction = _paybillServices.payBill(TransactionId);
            var agentInformatioin = Common.AgentSession.AgentInformation;

            string name = transaction.ReceiverName;
            string firstname = name.Split(' ').First();


            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            var ReceiptURL = baseUrl + "/EmailTemplate/AgentTransactionStatement/PrintPayBillTopUpStatement?MFReceiptNumber=" + PayBillTransaction.ReceiptNo +
             "&TransactionDate=" + PayBillTransaction.PaymentDate.ToString("dd/MM/yyyy") +
             "&Name=" + transaction.ReceiverName +
             "&WalletNumber=" + PayBillTransaction.RefCode +
             "&CustomerAccountNumber=" + senderInfo.ReferenceNo +
             "&AgentName=" + agentInformatioin.Name +
             "&AgentAcountNumber=" + agentInformatioin.AccountNo +
             "&StaffName=" + PayBillTransaction.PayingStaffName +
             "&TotalAmount=" + Common.Common.GetCurrencySymbol(senderInfo.Country) + "" + PayBillTransaction.Amount +
             "&Fee=" + Common.Common.GetCurrencySymbol(senderInfo.Country) + "" + PayBillTransaction.Fee +
             "&SendingAmount=" + Common.Common.GetCurrencySymbol(senderInfo.Country) + "" + PayBillTransaction.SendingAmount +
             "&ReceivingAmount=" + Common.Common.GetCurrencySymbol(senderInfo.Country) + "" + PayBillTransaction.SendingAmount + Common.Common.GetCurrencyCode(senderInfo.Country) +
             "&ExchangeRate=" + PayBillTransaction.ExchangeRate +
             "&SendingCurrency=" + Common.Common.GetCurrencyCode(senderInfo.Country) +
             "&ReceivingCurrency=" + "" +
             "&ReceiverName=" + firstname +
             "&PaymentMethod=" + PayBillTransaction.PaymentType;

            var ReceiptPDF = Common.Common.GetPdf(ReceiptURL);
            //var path = HostingEnvironment.MapPath(@"/Documents/") + "attachment.pdf";
            //ReceiptPDF.Save(path);
            byte[] bytes = ReceiptPDF.Save();
            string mimeType = "Application/pdf";
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.Flush();
            Response.End();
            //return File(path, "application/pdf");
        }

        #endregion

        #region Top up an Account

        [HttpGet]
        public ActionResult TopUpAnAccount(string country = "")
        {
            ViewBag.Countries = new SelectList(GetCountries(), "CountryCode", "CountryName");
            ViewBag.Suppliers = new SelectList(GetSuppliers(country), "ReferenceNo", "Name");
            TopUpAnAccountViewModel vm = _TopUpToSupplierServices.GetTopUpAnAccountViewModel();
            return View(vm);
        }

        [HttpPost]
        public ActionResult TopUpAnAccount([Bind(Include = TopUpAnAccountViewModel.BindProperty)] TopUpAnAccountViewModel vm)
        {
            ViewBag.Countries = new SelectList(GetCountries(), "CountryCode", "CountryName");
            ViewBag.Suppliers = new SelectList(GetSuppliers(vm.Country), "ReferenceNo", "Name");

            if (ModelState.IsValid)
            {
                _TopUpToSupplierServices.SetTopUpAnAccountViewModel(vm);
                return RedirectToAction("TopUpSupplierEnterAmount", vm);

            }
            return View(vm);
        }
        [HttpGet]
        public ActionResult TopUpSupplierEnterAmount([Bind(Include = TopUpAnAccountViewModel.BindProperty)] TopUpAnAccountViewModel vm)
        {

            int AgentId = Common.AgentSession.AgentInformation.Id;
            var supplier = _suppliersServices.List().Data.Where(x => x.RefCode == vm.AccountNo).FirstOrDefault();
            var agentCountry = getAgentCountryCode(AgentId);
            //  var agentCommission = Common.Common.GetAgentSendingCommission(TransferService.PayBills, AgentId, vm.AccountNo , transactionsummary.FaxingFee);

            TopUpSupplierEnterAmountVM model = new TopUpSupplierEnterAmountVM()
            {
                ExchangeRate = Common.Common.GetExchangeRate(agentCountry, supplier.Country),
                ReceiverName = supplier.KiiPayBusinessInformation.BusinessName,
                ReceiverAccountNo = supplier.RefCode,
                SendingCurrencySymbol = Common.Common.GetCurrencySymbol(agentCountry),
                ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(supplier.Country),
                SendingCurrency = Common.Common.GetCountryCurrency(agentCountry),
                ReceivingCurrency = Common.Common.GetCountryCurrency(supplier.Country),
                PhotoUrl = "",
                AccountNo = vm.AccountNo,
                AgentCommission = 0,

            };
            _TopUpToSupplierServices.SetAgentTopUpSupplierEnterAmount(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult TopUpSupplierEnterAmount([Bind(Include = TopUpSupplierEnterAmountVM.BindProperty)] TopUpSupplierEnterAmountVM vm)
        {
            var result = _TopUpToSupplierServices.GetAgentTopUpSupplierEnterAmount();

            int AgentId = Common.AgentSession.AgentInformation.Id;
            int StaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId;
            var agentCountry = getAgentCountryCode(AgentId);
            var supplierDetails = _suppliersServices.List().Data.Where(x => x.RefCode == result.AccountNo).FirstOrDefault();

            if (result != null)
            {
                if (ModelState.IsValid)
                {
                    TopUpToSupplier addTopUp = new TopUpToSupplier()
                    {
                        PayerId = AgentId,
                        EcxhangeRate = result.ExchangeRate,
                        PaymentDate = DateTime.Now,
                        Fee = result.Fee,
                        PayingCountry = Common.Common.GetCountryName(agentCountry),
                        PaymentModule = Module.Agent,
                        ReceivingAmount = result.ReceivingAmount,
                        SendingAmount = result.SendingAmount,
                        SuplierId = supplierDetails.Id,
                        SupplierAccountNo = result.ReceiverAccountNo,
                        SupplierCountry = Common.Common.GetCountryName(supplierDetails.Country),
                        TotalAmount = result.TotalAmount,
                        WalletNo = supplierDetails.KiiPayBusinessInformation.BusinessMobileNo,
                        PayingStaffName = Common.AgentSession.LoggedUser.PayingAgentStaffName,
                        AgentCommission = result.AgentCommission,
                        ReceiptNo = Common.Common.GenerateAgentPayBillTopUpReceiptNo(6),
                        PayingStaffId = StaffId
    ,
                    };
                    if (addTopUp.PayingCountry == addTopUp.SupplierCountry)
                    {
                        addTopUp.PaymentType = PaymentType.Local;
                    }
                    else
                    {
                        addTopUp.PaymentType = PaymentType.International;
                    }
                    var TopUpTransaction = _TopUpToSupplierServices.Add(addTopUp).Data;


                    return RedirectToAction("TopUpAccountSuccess", "AgentPayBills", new { Id = TopUpTransaction.Id });
                }
                else
                {
                    vm.ExchangeRate = result.ExchangeRate;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }

        }
        public ActionResult TopUpAccountSuccess(int Id)
        {
            var data = _TopUpToSupplierServices.GetAgentTopUpSupplierEnterAmount();
            TopUpAccountSuccessVM model = new TopUpAccountSuccessVM()
            {
                Amount = data.SendingAmount,
                Currency = data.SendingCurrencySymbol,
                ReceiverAccountNo = data.AccountNo,
            };
            ViewBag.TransactionId = Id;
            //AgentCommonServices cs = new AgentCommonServices();
            //cs.ClearAgentPayBillsTopUp();
            return View(model);
        }
        public void PrintReceiptOfPayBillTopUp(int TransactionId)
        {

            var senderInfo = _TopUpToSupplierServices.GetTopUpAnAccountViewModel();
            var transaction = _TopUpToSupplierServices.GetAgentTopUpSupplierEnterAmount();
            var topUpToTransaction = _TopUpToSupplierServices.TopUpToSupplier(TransactionId);
            var agentInformatioin = Common.AgentSession.AgentInformation;

            string name = transaction.ReceiverName;
            string firstname = name.Split(' ').First();


            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            var ReceiptURL = baseUrl + "/EmailTemplate/AgentTransactionStatement/PrintPayBillTopUpStatement?MFReceiptNumber=" + topUpToTransaction.ReceiptNo +
             "&TransactionDate=" + topUpToTransaction.PaymentDate +
             "&Name=" + transaction.ReceiverName +
             "&WalletNumber=" + topUpToTransaction.WalletNo +
             "&CustomerAccountNumber=" + topUpToTransaction.SupplierAccountNo +
             "&AgentName=" + agentInformatioin.Name +
             "&AgentAcountNumber=" + agentInformatioin.AccountNo +
             "&StaffName=" + topUpToTransaction.PayingStaffName +
             "&TotalAmount=" + Common.Common.GetCurrencySymbol(senderInfo.Country) + "" + topUpToTransaction.TotalAmount +
             "&Fee=" + Common.Common.GetCurrencySymbol(senderInfo.Country) + "" + topUpToTransaction.Fee +
             "&SendingAmount=" + Common.Common.GetCurrencySymbol(senderInfo.Country) + "" + topUpToTransaction.SendingAmount +
             "&ReceivingAmount=" + Common.Common.GetCurrencySymbol(senderInfo.Country) + "" + topUpToTransaction.ReceivingAmount + Common.Common.GetCurrencyCode(senderInfo.Country) +
             "&ExchangeRate=" + topUpToTransaction.EcxhangeRate +
             "&SendingCurrency=" + Common.Common.GetCurrencyCode(senderInfo.Country) +
             "&ReceivingCurrency=" + transaction.ReceivingCurrency +
             "&ReceiverName=" + firstname +
             "&PaymentMethod=" + topUpToTransaction.PaymentType;

            var ReceiptPDF = Common.Common.GetPdf(ReceiptURL);
            //var path = HostingEnvironment.MapPath(@"/Documents/") + "attachment.pdf";
            //ReceiptPDF.Save(path);
            byte[] bytes = ReceiptPDF.Save();
            string mimeType = "Application/pdf";
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.Flush();
            Response.End();
            //return File(path, "application/pdf");
        }
        public JsonResult GetPaymentSummary(decimal SendingAmount, decimal ReceivingAmount, bool IsReceivingAmount)
        {

            var enterAmountData = _TopUpToSupplierServices.GetAgentTopUpSupplierEnterAmount();

            int AgentId = Common.AgentSession.AgentInformation.Id;

            var agentCountry = getAgentCountryCode(AgentId);
            //if ((SendingAmount > 0 && ReceivingAmount > 0) && enterAmountData.ReceivingAmount != ReceivingAmount)
            //{

            //    SendingAmount = ReceivingAmount;
            //    IsReceivingAmount = true;
            //}

            //if (SendingAmount == 0)
            //{

            //    IsReceivingAmount = true;
            //    SendingAmount = ReceivingAmount;
            //}
            if (IsReceivingAmount)
            {

                SendingAmount = ReceivingAmount;
            }
            var result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
                enterAmountData.ExchangeRate, SEstimateFee.GetFaxingCommision(agentCountry));
            var agentCommission = Common.Common.GetAgentSendingCommission(TransferService.PayBills, AgentId, result.FaxingAmount, result.FaxingFee);

            // Rewrite session with additional value 
            enterAmountData.Fee = result.FaxingFee;
            enterAmountData.SendingAmount = result.FaxingAmount;
            enterAmountData.ReceivingAmount = result.ReceivingAmount;
            enterAmountData.TotalAmount = result.TotalAmount;
            enterAmountData.AgentCommission = agentCommission;

            _TopUpToSupplierServices.SetAgentTopUpSupplierEnterAmount(enterAmountData);
            return Json(new
            {
                Fee = result.FaxingFee,
                TotalAmount = result.TotalAmount,
                ReceivingAmount = result.ReceivingAmount,
                SendingAmount = result.FaxingAmount,
                AgentCommission = agentCommission

            }, JsonRequestBehavior.AllowGet);

        }
        #endregion
    }
}