using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class CalculateHowMuchController : Controller
    {
        DB.FAXEREntities dbContext = new DB.FAXEREntities();
        CommonServices CommonService = new CommonServices();
        // GET: Agent/CalculateHowMuch
        [HttpGet]
        public ActionResult Index()
        {
            CalculateHowMuchViewModel model = new CalculateHowMuchViewModel();
            if (Common.AgentSession.AgentInformation != null)
            {
                model.AgentCountryCode = Common.AgentSession.AgentInformation.CountryCode;
                model.AgentCurrency = Common.Common.GetCountryCurrency(model.AgentCountryCode);
                ViewBag.TransferType = 2;
                var agentInfo = Common.AgentSession.AgentInformation;
                ViewBag.IsAuxAgent = agentInfo.IsAUXAgent;
                ViewBag.AgentId = agentInfo.Id;
                if (agentInfo.IsAUXAgent == true)
                {
                    ViewBag.TransferType = 4;
                }
            }
            ViewBag.SendingCountries = Common.Common.GetSendingCountries();
            ViewBag.ReceivingCountries = Common.Common.GetReceivingCountries();

            return View(model);
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = CalculateHowMuchViewModel.BindProperty)] CalculateHowMuchViewModel vm)
        {
            return View(vm);
        }


        public ActionResult AgentSendMoneyNow(int TransferMethod = 0)
        {

            Common.AgentSession.IsTransferFromCalculateHowMuch = true;
            if (Common.AgentSession.LoggedUser != null)
            {
                if (TransferMethod == 1)
                {
                    return RedirectToAction("SendMoneToKiiPayWallet", "AgentKiiPayWalletTransfer");
                }
                if (TransferMethod == 2)
                {
                    return RedirectToAction("WalletInformation", "AgentMobileMoneyTransfer");

                }
                if (TransferMethod == 3)
                {
                    return RedirectToAction("BankAccountDeposit", "AgentBankAccountDeposit");
                }
                if (TransferMethod == 4)
                {
                    return RedirectToAction("CashPickupInformation", "AgentCashPickUpTransfer");
                }
                return RedirectToAction("Index", "TransferMoney");
            }
            else
            {
                Common.FaxerSession.FromUrl = "/Agent/TransferMoney/Index";
                return RedirectToAction("Login", "AgentLogin");
            }

        }

        //#region old Payment Summary


        //[HttpGet]
        //public JsonResult GetPaymentSummary(decimal SendingAmount = 0, decimal ReceivingAmount = 0, string SendingCountry = "", string ReceivingCountry = "", bool IsReceivingAmount = false, bool IsFeeIncluded = false, int TransferType = 0, int TransferMethod = 0)
        //{
        //    SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
        //    if (IsReceivingAmount)
        //    {

        //        SendingAmount = ReceivingAmount;
        //    }

        //    var result = SEstimateFee.CalculateFaxingFee(SendingAmount, IsFeeIncluded, IsReceivingAmount,
        //        SExchangeRate.GetExchangeRateValue(SendingCountry, ReceivingCountry, (TransactionTransferMethod)TransferMethod, Common.AgentSession.LoggedUser.Id, TransactionTransferType.Agent), SEstimateFee.GetFaxingCommision(SendingCountry));

        //    decimal AgentCommission = 0;

        //    if (TransferMethod == 1)
        //    {
        //        AgentCommission = Common.Common.GetAgentSendingCommission(TransferService.KiiPayWallet, Common.AgentSession.LoggedUser.Id, result.FaxingAmount, result.FaxingFee);
        //    }
        //    if (TransferMethod == 2)
        //    {
        //        AgentCommission = Common.Common.GetAgentSendingCommission(TransferService.OtherWallet, Common.AgentSession.LoggedUser.Id, result.FaxingAmount, result.FaxingFee);

        //    }
        //    if (TransferMethod == 3)
        //    {
        //        AgentCommission = Common.Common.GetAgentSendingCommission(TransferService.BankDeposit, Common.AgentSession.LoggedUser.Id, result.FaxingAmount, result.FaxingFee);


        //    }
        //    if (TransferMethod == 4)
        //    {
        //        AgentCommission = Common.Common.GetAgentSendingCommission(TransferService.CahPickUp, Common.AgentSession.LoggedUser.Id, result.FaxingAmount, result.FaxingFee);

        //    }
        //    CommonEnterAmountViewModel enterAmount = new CommonEnterAmountViewModel()
        //    {
        //        Fee = result.FaxingFee,
        //        SendingAmount = result.FaxingAmount,
        //        ReceivingAmount = result.ReceivingAmount,
        //        TotalAmount = result.TotalAmount,
        //        ExchangeRate = result.ExchangeRate,
        //        SendingCurrencySymbol = Common.Common.GetCurrencySymbol(SendingCountry),
        //        ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(ReceivingCountry),
        //        SendingCountryCode = SendingCountry,
        //        ReceivingCountryCode = ReceivingCountry,
        //        SendingCurrency = Common.Common.GetCountryCurrency(SendingCountry),
        //        ReceivingCurrency = Common.Common.GetCountryCurrency(ReceivingCountry),
        //        AgentCommission = AgentCommission
        //    };

        //    _kiiPaytrasferServices.SetCommonEnterAmount(enterAmount);



        //    if (IsFeeIncluded)
        //    {
        //        CommonEnterAmountViewModel IsFeeEnterAmount = new CommonEnterAmountViewModel()
        //        {
        //            Fee = result.FaxingFee,
        //            SendingAmount = SendingAmount,
        //            ReceivingAmount = result.ReceivingAmount,
        //            TotalAmount = result.TotalAmount,
        //            ExchangeRate = result.ExchangeRate,
        //            SendingCurrencySymbol = Common.Common.GetCurrencySymbol(SendingCountry),
        //            ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(ReceivingCountry),
        //            SendingCountryCode = SendingCountry,
        //            ReceivingCountryCode = ReceivingCountry,
        //            SendingCurrency = Common.Common.GetCountryCurrency(SendingCountry),
        //            ReceivingCurrency = Common.Common.GetCountryCurrency(ReceivingCountry),
        //            AgentCommission = AgentCommission
        //        };
        //        _kiiPaytrasferServices.SetCommonEnterAmount(IsFeeEnterAmount);
        //        result.FaxingAmount = SendingAmount;

        //    }



        //    return Json(new
        //    {
        //        Fee = result.FaxingFee,
        //        TotalAmount = result.TotalAmount,
        //        ReceivingAmount = result.ReceivingAmount,
        //        SendingAmount1 = result.FaxingAmount,
        //        ExchangeRate = result.ExchangeRate,
        //        SendingCurrencySymbol = enterAmount.SendingCurrencySymbol,
        //        ReceivingCurrencySymbol = enterAmount.ReceivingCurrencySymbol,
        //        SendingCurrency = enterAmount.SendingCurrency,
        //        ReceivingCurrency = enterAmount.ReceivingCurrency,
        //        AgentCommission = enterAmount.AgentCommission
        //    }, JsonRequestBehavior.AllowGet);
        //}
        //#endregion

        public string getAgentCountryCode(int agentId = 0)
        {
            SAgentInformation _services = new SAgentInformation();
            var result = _services.list().Data.Where(x => x.Id == agentId).Select(x => x.CountryCode).FirstOrDefault();
            return result;
        }
        public JsonResult GetPaymentSummary(decimal SendingAmount = 0, decimal ReceivingAmount = 0, string SendingCountry = "", string ReceivingCountry = "", bool IsReceivingAmount = false, bool IsFeeIncluded = false, int TransferType = 0, int TransferMethod = 7)
        {
            SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
            int AgentId = Common.AgentSession.AgentInformation.Id;
            var agentCountry = getAgentCountryCode(AgentId);
            string SendingCurrency = Common.Common.GetCountryCurrency(SendingCountry);
            string SendingCurrencySymbol = Common.Common.GetCurrencySymbol(SendingCountry);
            string ReceivingCurrency = Common.Common.GetCountryCurrency(ReceivingCountry);
            string ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(ReceivingCountry);
            var enterAmountData = _kiiPaytrasferServices.GetCommonEnterAmount();
            if (IsReceivingAmount)
            {

                SendingAmount = ReceivingAmount;
            }
            if ((TransactionTransferType)TransferType == TransactionTransferType.Online)
            {

                AgentId = 0;
            }
            var feeInfo = SEstimateFee.GetTransferFee(SendingCountry, ReceivingCountry, (TransactionTransferMethod)TransferMethod, SendingAmount, (TransactionTransferType)TransferType, AgentId);
            if (feeInfo == null)
            {
                return Json(new
                {
                    Fee = 0,
                    TotalAmount = 0,
                    ReceivingAmount = 0,
                    SendingAmount = 0,
                    AgentCommission = 0,
                    SendingCurrencySymbol = SendingCurrencySymbol,
                    ReceivingCurrencySymbol = ReceivingCurrencySymbol,
                    SendingCurrency = SendingCurrency,
                    ReceivingCurrency = ReceivingCurrency,
                }, JsonRequestBehavior.AllowGet);
            }
            var result = new EstimateFaxingFeeSummary();

            result = SEstimateFee.CalculateFaxingFee(SendingAmount, IsFeeIncluded, IsReceivingAmount,
                SExchangeRate.GetExchangeRateValue(SendingCountry, ReceivingCountry,
                (TransactionTransferMethod)TransferMethod, AgentId, (TransactionTransferType)TransferType), feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false);

            var introductoryRateResult = SEstimateFee.GetIntroductoryTransferSummary(agentCountry, ReceivingCountry, result.FaxingAmount
                 , feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false, IsReceivingAmount, (TransactionTransferMethod)TransferMethod, AgentId, true);

            if (introductoryRateResult != null)
            {
                result = introductoryRateResult;
            }

            //var result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
            //    enterAmountData.ExchangeRate, SEstimateFee.GetFaxingCommision(agentCountry));




            var AgentCommission = Common.Common.GetAgentSendingCommission((TransferService)TransferType, AgentId, result.FaxingAmount, result.FaxingFee);
            // Rewrite session with additional value 
            enterAmountData.Fee = result.FaxingFee;
            enterAmountData.SendingAmount = result.FaxingAmount;
            enterAmountData.ReceivingAmount = result.ReceivingAmount;
            enterAmountData.TotalAmount = result.TotalAmount;
            enterAmountData.AgentCommission = AgentCommission;
            enterAmountData.ExchangeRate = result.ExchangeRate;
            enterAmountData.SendingCurrency = SendingCurrency;
            enterAmountData.ReceivingCurrency = ReceivingCurrency;
            enterAmountData.SendingCurrencySymbol = SendingCurrencySymbol;
            enterAmountData.ReceivingCurrencySymbol = ReceivingCurrencySymbol;



            _kiiPaytrasferServices.SetCommonEnterAmount(enterAmountData);

            return Json(new
            {
                Fee = result.FaxingFee,
                TotalAmount = result.TotalAmount,
                ReceivingAmount = result.ReceivingAmount,
                SendingAmount1 = result.FaxingAmount,
                AgentCommission = AgentCommission,
                ExchangeRate = result.ExchangeRate,
                SendingCurrencySymbol = SendingCurrencySymbol,
                ReceivingCurrencySymbol = ReceivingCurrencySymbol,
                SendingCurrency = SendingCurrency,
                ReceivingCurrency = ReceivingCurrency,
            }, JsonRequestBehavior.AllowGet);
        }



    }
}
