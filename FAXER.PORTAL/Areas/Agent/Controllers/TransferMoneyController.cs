using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Controllers;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class TransferMoneyController : Controller
    {
        TransferMoneyServices _transferMoneyServices = null;
        CommonServices _commonServices = null;

        public TransferMoneyController()
        {
            _transferMoneyServices = new TransferMoneyServices();
            _commonServices = new CommonServices();
        }
        // GET: Agent/TransferMoney
        public ActionResult Index(string message = "", string fileUrl = "")
        {
            if (message == "success")
            {
                ViewBag.Message = "success";
                ViewBag.fileUrl = fileUrl;
            }
            var agentInfo = Common.AgentSession.AgentInformation;
            if (agentInfo.IsAUXAgent)
            {
                AgentCommonServices _AgentCommonServices = new AgentCommonServices();
                decimal AUXAgentAccountBalance = _AgentCommonServices.getAuxAgentAccountBalance(agentInfo.Id, Common.AgentSession.LoggedUser.PayingAgentStaffId);
                if (AUXAgentAccountBalance == 0)
                {
                    ModelState.AddModelError("Insufficientfund", "Insufficient Fund");
                    ViewBag.OutOfBalance = "True";
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult DepositInVirtualAccount(string accountNo = "", string virtualAccountNo = "", string message = "")
        {
            SetViewBagForIDCard();
            SetViewBagForCountries();

            TransferMoneyViewModel vm = new TransferMoneyViewModel();

            if (!string.IsNullOrEmpty(accountNo))
            {

                bool checkAccountNo = _transferMoneyServices.checkSenderAccountNumberExists(accountNo);
                if (checkAccountNo == false)
                {
                    ModelState.AddModelError("SendersAccountNo", "Invalid Account Number !");
                    return View(vm);
                }
                vm = _transferMoneyServices.getSendersInfo(accountNo);
                vm.SendersAccountNo = accountNo;

            }
            vm.NameOfAgency = Common.AgentSession.AgentInformation.Name;
            vm.AgentAccountNo = Common.AgentSession.AgentInformation.AccountNo;
            vm.NameOfPayingStaff = Common.AgentSession.LoggedUser.PayingAgentStaffName;
            return View(vm);
        }

        [HttpPost]
        public ActionResult DepositInVirtualAccount([Bind(Include = TransferMoneyViewModel.BindProperty)] TransferMoneyViewModel model)
        {
            SetViewBagForIDCard();
            SetViewBagForCountries();
            if (model != null)
            {
                if (model.SenderFaxerId == 0)
                {
                    ModelState.AddModelError("SendersAccountNo", "Please enter Sender Account Number !");
                    return View(model);
                }
                if (model.ReceiverMFTCId == 0)
                {
                    ModelState.AddModelError("AccountDetailsVirtualAccountNo", "Please enter Virtual Account Number !");
                    return View(model);
                }
                if (model.SenderIDType == 0)
                {
                    ModelState.AddModelError("SenderIDType", "Please choose a ID Card Type");
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.SenderIDNumber))
                {
                    ModelState.AddModelError("SenderIDNumber", "Please enter ID Card Number !");
                    return View(model);
                }
                if (model.SenderIDExpiringDate == default(DateTime))
                {
                    ModelState.AddModelError("SenderIDExpiringDate", "This field can't be blank !");
                    return View(model);
                }
                if (model.SenderIDExpiringDate <= DateTime.Now)
                {
                    ModelState.AddModelError("SenderIDExpiringDate", "Expiry Date Can't be less than today's date !");
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.SenderIDIssuingCountry))
                {
                    ModelState.AddModelError("SenderIDIssuingCountry", "Plese choose a country !");
                    return View(model);
                }
                if (model.SenderAccountStatus.Trim() != "Active")
                {
                    ModelState.AddModelError("AccountDetailsVirtualAccountNo", "Please provide a active virtual account number !");
                    return View(model);
                }
                if (model.DepositAmount == 0)
                {
                    ModelState.AddModelError("DepositAmount", "This field can't be blank !");
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.PaymentReference))
                {
                    ModelState.AddModelError("PaymentReference", "This field can't be blank !");
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.NameOfPayingStaff))
                {
                    ModelState.AddModelError("NameOfPayingStaff", "Name of Paying Staff can't be empty !");
                    return View(model);
                }
                if (model.VerificationConfirm == false)
                {
                    ModelState.AddModelError("VerificationConfirm", "Please confirm the verification !");
                    return View(model);
                }

                model.ReceiptNumber = _transferMoneyServices.GetVirtualAccountDepositByAgentReceiptNumber();
                bool saveData = _transferMoneyServices.saveVirtualAccountDepositByAgent(model);
                if (saveData == true)
                {
                    MailCommon mail = new MailCommon();
                    var baseUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                    string senderFullName = model.SenderFirstName + " " + model.SenderMiddleName + " " + model.SenderLastName;
                    string url = baseUrl + "/EmailTemplate/DepositInVirtualAccountByAgentReceipt?MFReceiptNumber=" + model.ReceiptNumber + "&TransactionDate=" + DateTime.Now.ToString("dd-MM-yyyy")
                        + "&TransactionTime=" + DateTime.Now.ToString("HH:mm") + "&SenderFullName=" + senderFullName + "&ReceiverFullName=" + model.AccountDetailsName + "&Telephone=" + model.AccountUserDetailsTelephone +
                        "&AgentName=" + model.NameOfAgency + "&AgentCode=" + model.AgentAccountNo + "&AmountSent=" + model.DepositAmount.ToString() + "&ExchangeRate=" + model.CurrentExchangeRate.ToString() + "&Fee=" +
                        model.DepositFees.ToString() + "&AmountReceived=" + model.ReceivingAmount.ToString() + "&TotalAmountSentAndFee=" + model.TotalAmountIncludingFee + "&SendingCurrency=" + model.SenderCurrency +
                        "&ReceivingCurrency=" + model.ReceiverCurrency;
                    //var output = Common.Common.GetPdf(url);
                    //mail.SendMail();
                    Common.AgentSession.TempUrl = url;
                    return RedirectToAction("Index", new { @message = "success", @fileUrl = url });
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult DepositInBusinessVirtualAccount(string senderAccountNo = "", string businessAccountNo = "", string message = "", string fileUrl = "")
        {
            SetViewBagForIDCard();
            SetViewBagForCountries();
            AgentBusinessTransferMoneyViewModel vm = new AgentBusinessTransferMoneyViewModel();
            vm.NameOfPayingStaff = Common.AgentSession.LoggedUser.PayingAgentStaffName;
            if (!string.IsNullOrEmpty(senderAccountNo))
            {
                bool checkAccountNo = _transferMoneyServices.checkSenderAccountNumberExists(senderAccountNo);
                if (checkAccountNo == false)
                {
                    ModelState.AddModelError("SendersAccountNo", "Invalid Account Number !");
                    return View(vm);
                }
                vm = _transferMoneyServices.getSendersInfo1(senderAccountNo);
                vm.SendersAccountNo = senderAccountNo;
            }
            vm.NameOfAgency = Common.AgentSession.AgentInformation.Name;
            vm.AgentAccountNo = Common.AgentSession.AgentInformation.AccountNo;

            return View(vm);
        }

        [HttpPost]
        public ActionResult DepositInBusinessVirtualAccount([Bind(Include = AgentBusinessTransferMoneyViewModel.BindProperty)] AgentBusinessTransferMoneyViewModel model)
        {
            SetViewBagForIDCard();
            SetViewBagForCountries();
            if (model != null)
            {
                if (model.SenderFaxerId == 0)
                {
                    ModelState.AddModelError("SendersAccountNo", "Please enter Sender Account Number !");
                    return View(model);
                }
                if (model.ReceiverKiiPayBusinessInformationId == 0)
                {
                    ModelState.AddModelError("BusinessAccountNo", "Please enter Business Account Number !");
                    return View(model);
                }
                if (model.SenderIDType == 0)
                {
                    ModelState.AddModelError("SenderIDType", "Please choose ID Type !");
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.SenderIDNumber))
                {
                    ModelState.AddModelError("SenderIDNumber", "This field can't be blank !");
                    return View(model);
                }
                if (model.SenderIDExpiringDate == default(DateTime))
                {
                    ModelState.AddModelError("SenderIDExpiringDate", "This field can't be blank !");
                    return View(model);
                }
                if (model.SenderIDExpiringDate <= DateTime.Now)
                {
                    ModelState.AddModelError("SenderIDExpiringDate", "The expiry date can't be smaller than today's date !");
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.SenderIDIssuingCountry))
                {
                    ModelState.AddModelError("SenderIDIssuingCountry", "This field can't be blank !");
                    return View(model);
                }
                if (model.DepositAmount == 0)
                {
                    ModelState.AddModelError("DepositAmount", "This field can't be blank !");
                    return View(model);
                }
                if (model.BusinessAccountStatus.Trim() != "Active")
                {
                    ModelState.AddModelError("BusinessAccountNo", "Please provide a active business account number !");
                    return View(model);
                }
                if (model.ReceivingAmount == 0)
                {
                    ModelState.AddModelError("ReceivingAmount", "This field can't be blank !");
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.PaymentReference))
                {
                    ModelState.AddModelError("PaymentReference", "Please enter Payment Reference !");
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.NameOfPayingStaff))
                {
                    ModelState.AddModelError("NameOfPayingStaff", "This field can't be blank !");
                    return View(model);
                }
                if (model.VerificationConfirm == false)
                {
                    ModelState.AddModelError("VerificationConfirm", "Please confirm that you have verified the information provided !");
                    return View(model);
                }

                model.ReceiptNumber = _transferMoneyServices.GetBusinessAccountDepositByAgentReceiptNumber();
                bool saveData = _transferMoneyServices.saveBusinessAccountDepositByAgent(model);
                if (saveData == true)
                {
                    MailCommon mail = new MailCommon();
                    var baseUrl = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                    string senderFullName = model.SenderFirstName + " " + model.SenderMiddleName + " " + model.SenderLastName;
                    string url = baseUrl + "/EmailTemplate/DepositInBusinessAccountByAgentReceipt?ReceiptNumber=" + model.ReceiptNumber + "&TransactionDate=" + DateTime.Now.ToString("dd-MM-yyyy")
                        + "&TransactionTime=" + DateTime.Now.ToString("HH:mm") + "&SenderFullName=" + senderFullName + "&ReceiverFullName=" + model.BusinessName + "&Telephone=" + model.BusinessTelephoneNumber +
                        "&AgentName=" + model.NameOfAgency + "&AgentCode=" + model.AgentAccountNo + "&AmountSent=" + model.DepositAmount.ToString() + "&ExchangeRate=" + model.CurrentExchangeRate.ToString() + "&Fee=" +
                        model.DepositFees.ToString() + "&AmountReceived=" + model.ReceivingAmount.ToString() + "&TotalAmountSentAndFee=" + model.TotalAmountIncludingFee + "&SendingCurrency=" + model.SenderCurrency +
                        "&ReceivingCurrency=" + model.ReceiverCurrency;
                    //var output = Common.Common.GetPdf(url);
                    //mail.SendMail();
                    Common.AgentSession.TempUrl = url;
                    return RedirectToAction("Index", new { @message = "success", @fileUrl = url });
                }
            }
            return View(model);
        }


        [HttpGet]
        public ActionResult getcardUserDetails(string virtutalAccountNumber)
        {
            if (!string.IsNullOrEmpty(virtutalAccountNumber))
            {
                bool checkVirtualAccountNumber = _transferMoneyServices.checkVirtualAccountNumberExists(virtutalAccountNumber);
                if (checkVirtualAccountNumber == false)
                {
                    TempData["mftcNo"] = "Invalid Virtual Account Number !";
                    return Json(new
                    {
                        InvalidNo = true,
                        AccountNotActive = false,
                    }, JsonRequestBehavior.AllowGet);
                }
                var virtualAccountInfo = _transferMoneyServices.getCardUsersInfo(virtutalAccountNumber);

                return Json(new
                {
                    AccountDetailsVirtualAccountNo = virtualAccountInfo.AccountDetailsVirtualAccountNo,
                    AccountDetailsName = virtualAccountInfo.AccountDetailsName,
                    AccountDetailsNumber = virtualAccountInfo.AccountDetailsNumber,
                    AccountDetailsBalance = virtualAccountInfo.AccountDetailsBalance,
                    AccountDetailsWithdrawalLimit = virtualAccountInfo.AccountDetailsWithdrawalLimit,
                    AccountDetailsLimitType = virtualAccountInfo.AccountDetailsLimitType,
                    AccountUserDetailsFirstName = virtualAccountInfo.AccountUserDetailsFirstName,
                    AccountUserDetailsMiddleName = virtualAccountInfo.AccountUserDetailsMiddleName,
                    AccountUserDetailsLastName = virtualAccountInfo.AccountUserDetailsLastName,
                    AccountUserDetailsAddress = virtualAccountInfo.AccountUserDetailsAddress,
                    AccountUserDetailsCity = virtualAccountInfo.AccountUserDetailsCity,
                    AccountUserDetailsCountry = virtualAccountInfo.AccountUserDetailsCountry,
                    AccountUserDetailsTelephone = virtualAccountInfo.AccountUserDetailsTelephone,
                    ReceiverCurrency = virtualAccountInfo.ReceiverCurrency,
                    ReceiverCurrenySymbol = virtualAccountInfo.ReceiverCurrenySymbol,
                    ReceiverMFTCId = virtualAccountInfo.ReceiverMFTCId,
                    InvalidNo = false,
                    SenderAccountStatus = virtualAccountInfo.SenderAccountStatus,
                    AccountUserDetailsPhoto = virtualAccountInfo.AccountUserDetailsPhoto

                }, JsonRequestBehavior.AllowGet);

            }
            TempData["mftcNo"] = "Please enter Non-Empty Value !";
            return Json(new
            {

            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult getBusinessAccountDetails(string businessAccountNumber)
        {
            if (!string.IsNullOrEmpty(businessAccountNumber))
            {
                bool checkBusinessAccountnumber = _transferMoneyServices.checkBusinessAccountNumberExists(businessAccountNumber);
                if (checkBusinessAccountnumber == false)
                {
                    return Json(new
                    {
                        InvalidNo = true
                    }, JsonRequestBehavior.AllowGet);
                }
                var businessAccountInfo = _transferMoneyServices.getBusinessInfo(businessAccountNumber);
                return Json(new
                {
                    BusinessAccountNo = businessAccountNumber,
                    BusinessName = businessAccountInfo.BusinessName,
                    BusinessLicenseNumber = businessAccountInfo.BusinessLicenseNumber,
                    BusinessAddress = businessAccountInfo.BusinessAddress,
                    BusinessCity = businessAccountInfo.BusinessCity,
                    BusinessCountry = businessAccountInfo.BusinessCountry,
                    BusinessCountryPhoneCode = businessAccountInfo.BusinessCountryPhoneCode,
                    BusinessTelephoneNumber = businessAccountInfo.BusinessTelephoneNumber,
                    BusinessAccountStatus = businessAccountInfo.BusinessAccountStatus,
                    ReceiverKiiPayBusinessInformationId = businessAccountInfo.ReceiverKiiPayBusinessInformationId,
                    ReceiverCurrency = businessAccountInfo.ReceiverCurrency,
                    ReceiverCurrenySymbol = businessAccountInfo.ReceiverCurrenySymbol
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {

            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult getFaxingCalculations(decimal topUpAmount = 0, int senderFaxerId = 0, int receiverMFTCId = 0, decimal RecevingAmount = 0)
        {
            string faxingCountryCode = _transferMoneyServices.getFaxerCountryCode(senderFaxerId);
            string ReceivingCountryCode = _transferMoneyServices.getMFTCCardUserCountrCode(receiverMFTCId);
            decimal exchangeRate = 0;
            decimal faxingFee = 0;
            var exchangeRateObj = _transferMoneyServices.getExchangeRateTable().Where(x => x.CountryCode1 == faxingCountryCode && x.CountryCode2 == ReceivingCountryCode).FirstOrDefault();
            if (exchangeRateObj == null)
            {
                var exchangeRateobj2 = _transferMoneyServices.getExchangeRateTable().Where(x => x.CountryCode1 == ReceivingCountryCode && x.CountryCode2 == faxingCountryCode).FirstOrDefault();
                if (exchangeRateobj2 != null)
                {
                    exchangeRateObj = exchangeRateobj2;
                    exchangeRate = Math.Round(1 / exchangeRateObj.CountryRate1, 6, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                exchangeRate = exchangeRateObj.CountryRate1;
            }
            if (ReceivingCountryCode == faxingCountryCode)
            {
                exchangeRate = 1m;
            }
            if (exchangeRate == 0)
            {
                ViewBag.ExchangeRate = "We are yet to start operations to this country, please try again later!";
            }
            if (RecevingAmount > 0)
            {
                topUpAmount = RecevingAmount;
            }
            var feeSummary = SEstimateFee.CalculateFaxingFee(topUpAmount, false, RecevingAmount > 0, exchangeRate, SEstimateFee.GetFaxingCommision(faxingCountryCode));


            return Json(new
            {
                DepositAmount = feeSummary.FaxingAmount,
                DepositFees = feeSummary.FaxingFee,
                TotalAmountIncludingFee = feeSummary.TotalAmount,
                CurrentExchangeRate = feeSummary.ExchangeRate,
                ReceivingAmount = feeSummary.ReceivingAmount
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult getFaxingCalculationsBusiness(decimal topUpAmount = 0, int senderFaxerId = 0, int receiverKiiPayBusinessInformationId = 0, decimal RecevingAmount = 0)
        {
            string faxingCountryCode = _transferMoneyServices.getFaxerCountryCode(senderFaxerId);
            string ReceivingCountryCode = _transferMoneyServices.getBusinessCountryCode(receiverKiiPayBusinessInformationId);
            decimal exchangeRate = 0;
            decimal faxingFee = 0;
            var exchangeRateObj = _transferMoneyServices.getExchangeRateTable().Where(x => x.CountryCode1 == faxingCountryCode && x.CountryCode2 == ReceivingCountryCode).FirstOrDefault();
            if (exchangeRateObj == null)
            {
                var exchangeRateobj2 = _transferMoneyServices.getExchangeRateTable().Where(x => x.CountryCode1 == ReceivingCountryCode && x.CountryCode2 == faxingCountryCode).FirstOrDefault();
                if (exchangeRateobj2 != null)
                {
                    exchangeRateObj = exchangeRateobj2;
                    exchangeRate = Math.Round(1 / exchangeRateObj.CountryRate1, 6, MidpointRounding.AwayFromZero);
                }
            }
            else
            {
                exchangeRate = exchangeRateObj.CountryRate1;
            }
            if (ReceivingCountryCode == faxingCountryCode)
            {
                exchangeRate = 1m;
            }
            if (exchangeRate == 0)
            {
                ViewBag.ExchangeRate = "We are yet to start operations to this country, please try again later!";
            }
            if (RecevingAmount > 0)
            {
                topUpAmount = RecevingAmount;
            }
            var feeSummary = SEstimateFee.CalculateFaxingFee(topUpAmount, false, RecevingAmount > 0, exchangeRate, SEstimateFee.GetFaxingCommision(faxingCountryCode));


            return Json(new
            {
                DepositAmount = feeSummary.FaxingAmount,
                DepositFees = feeSummary.FaxingFee,
                TotalAmountIncludingFee = feeSummary.TotalAmount,
                CurrentExchangeRate = feeSummary.ExchangeRate,
                ReceivingAmount = feeSummary.ReceivingAmount
            }, JsonRequestBehavior.AllowGet);
        }

        private void SetViewBagForIDCard()
        {
            var idCard = _transferMoneyServices.GetIDCardTypes();
            ViewBag.IDCardDropdown = new SelectList(idCard, "Id", "CardType");
        }

        private void SetViewBagForCountries()
        {
            var countries = _commonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }
        public void printReceipt(string url)
        {
            if (url != null)
            {
                string tempUrl = Common.AgentSession.TempUrl;
                Common.AgentSession.TempUrl = "";
                var output = Common.Common.GetPdf(tempUrl);
                byte[] bytes = output.Save();
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
}