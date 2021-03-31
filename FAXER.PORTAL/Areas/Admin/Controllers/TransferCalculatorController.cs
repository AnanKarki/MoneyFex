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

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class TransferCalculatorController : Controller
    {
        
        CommonServices CommonService = new CommonServices();

        // GET: Admin/TransferCalculator
        public ActionResult Index()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            
            return View();
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

        public JsonResult GetPaymentSummary(decimal SendingAmount = 0, decimal ReceivingAmount = 0, string SendingCountry = "", string ReceivingCountry = "", bool IsReceivingAmount = false, bool IsFeeIncluded = false, int TransferType = 0, int TransferMethod = 7)
        {
            SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
            int StaffId = Common.AdminSession.StaffId;
            var StaffCountry = CommonService.getStaffCountry(StaffId);
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

                StaffId = 0;
            }
            var feeInfo = SEstimateFee.GetTransferFee(SendingCountry, ReceivingCountry, (TransactionTransferMethod)TransferMethod, SendingAmount, (TransactionTransferType)TransferType, StaffId);
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
                (TransactionTransferMethod)TransferMethod, StaffId, (TransactionTransferType)TransferType), feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false);

            //var introductoryRateResult = SEstimateFee.GetIntroductoryTransferSummary(StaffCountry, ReceivingCountry, result.FaxingAmount
            //     , feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false, IsReceivingAmount, (TransactionTransferMethod)TransferMethod, StaffId, true);

            //if (introductoryRateResult != null)
            //{
            //    result = introductoryRateResult;
            //}

            //var result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
            //    enterAmountData.ExchangeRate, SEstimateFee.GetFaxingCommision(agentCountry));




           // var AgentCommission = Common.Common.GetAgentSendingCommission((TransferService)TransferType, StaffId, result.FaxingAmount, result.FaxingFee);
            // Rewrite session with additional value 
            enterAmountData.Fee = result.FaxingFee;
            enterAmountData.SendingAmount = result.FaxingAmount;
            enterAmountData.ReceivingAmount = result.ReceivingAmount;
            enterAmountData.TotalAmount = result.TotalAmount;
          
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
              
                ExchangeRate = result.ExchangeRate,
                SendingCurrencySymbol = SendingCurrencySymbol,
                ReceivingCurrencySymbol = ReceivingCurrencySymbol,
                SendingCurrency = SendingCurrency,
                ReceivingCurrency = ReceivingCurrency,
            }, JsonRequestBehavior.AllowGet);
        }


    }
}