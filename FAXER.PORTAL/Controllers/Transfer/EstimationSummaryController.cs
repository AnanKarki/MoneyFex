using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Models.PaymentSummary;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.Transfer
{
    public class EstimationSummaryController : Controller
    {
        // GET: EstimationSummary
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetTransferSummary(PaymentSummaryRequestParamVm request)
        {
            SEstimationSummary estimationSummaryServices = new SEstimationSummary();

            var response = new PaymentSummaryResponseVm();
            //request.TransferType = Trn
            if (request.IsReceivingAmount)
            {
                request.SendingAmount = request.ReceivingAmount;
            }
            if (request.TransferType == 0 )
            {
                request.TransferType = (int)TransactionTransferType.Online;
            }
            if (request.TransferMethod == 0)
            {
                request.TransferMethod = (int)TransactionTransferMethod.All;
            }
            // Check if fee has been setup or not 
            var feeInfo = estimationSummaryServices.GetTransferFee(request);
            if (feeInfo == null) // if feeinfo is null then fee hasn't been setup
            {
                return Json(new
                {

                    response
                });

            }
            else
            {

                var estimateSummary = estimationSummaryServices.GetEstimatedSummary(request, feeInfo);
                var introductoryRate = estimationSummaryServices.GetIntroductoryRate(request, feeInfo , request.SenderId);
                if (introductoryRate != null )
                {


                    response = estimationSummaryServices.BindEstimateSummaryModelToPaymentSummaryModel(introductoryRate);
                }
                else
                {
                    response = estimationSummaryServices.BindEstimateSummaryModelToPaymentSummaryModel(estimateSummary);
                }
                response.IsValid = estimationSummaryServices.CheckIfIsValidTransfer(request, response.ReceivingAmount);
                response.SendingCountry = request.SendingCountry;
                response.ReceivingCountry = request.ReceivingCountry;
                response.ReceivingCurrency = request.ReceivingCurrency;
                response.SendingCurrency = request.SendingCurrency;
                response.SendingCurrencySymbol = Common.Common.GetCurrencySymbolByCurrencyCode(request.SendingCurrency);
                response.ReceivingCurrencySymbol = Common.Common.GetCurrencySymbolByCurrencyCode(request.ReceivingCurrency);
                estimationSummaryServices.SetEstimationSummaryToSession(response);
                return Json(new
                {
                    response

                }, JsonRequestBehavior.AllowGet);
            }

        }

        //public void SetEstimationSummaryToSession(PaymentSummaryResponseVm result)
        //{

        //    SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
        //    CommonEnterAmountViewModel enterAmount = new CommonEnterAmountViewModel()
        //    {
        //        Fee = result.Fee,
        //        SendingAmount = result.SendingAmount,
        //        ReceivingAmount = result.ReceivingAmount,
        //        TotalAmount = result.TotalAmount,
        //        ExchangeRate = result.ExchangeRate,
        //        SendingCurrencySymbol = result.SendingCurrencySymbol,
        //        ReceivingCurrencySymbol = result.ReceivingCurrencySymbol,
        //        SendingCountryCode = result.SendingCountry,
        //        ReceivingCountryCode = result.ReceivingCountry,
        //        SendingCurrency = result.SendingCurrency,
        //        ReceivingCurrency = result.ReceivingCurrency,
        //    };
        //    // Rewrite session with additional value 

        //    _kiiPaytrasferServices.SetCommonEnterAmount(enterAmount);
        //}

        //private ServiceResult<bool> CheckIfIsValidTransfer(PaymentSummaryRequestParamVm request, decimal ReceivingAmount)
        //{

        //    var transactionValidationResult = Common.Common.IsValidTransactionLimit(request.SendingCountry,
        //        request.ReceivingCountry, ReceivingAmount, (TransactionTransferMethod)request.TransferMethod);

        //    return transactionValidationResult;

        //}

        //private PaymentSummaryResponseVm BindEstimateSummaryModelToPaymentSummaryModel(EstimateFaxingFeeSummary model)
        //{
        //    var response = new PaymentSummaryResponseVm()
        //    {
        //        ExchangeRate = model.ExchangeRate,
        //        Fee = model.FaxingFee,
        //        ReceivingAmount = model.ReceivingAmount,
        //        ReceivingCountry = model.ReceivingCountry,
        //        SendingCountry = model.SendingCountry,
        //        SendingCurrency = model.FaxingCurrency,
        //        ReceivingCurrency = model.ReceivingCurrency,
        //        SendingAmount = model.FaxingAmount,
        //        TotalAmount = model.TotalAmount,
        //        IsIntroductoryRate = model.IsIntroductoryRate,
        //        IsIntroductoryFee = model.IsIntroductoryFee,
        //        ReceivingCurrencySymbol = model.ReceivingCurrencySymbol,
        //        SendingCurrencySymbol = model.FaxingCurrencySymbol,
        //        ActualFee = model.ActualFee,
        //    };
        //    return response;

        //}

        //private TransferFeePercentage GetTransferFee(PaymentSummaryRequestParamVm request)
        //{

        //    var feeInfo = SEstimateFee.GetTransferFee(request.SendingCountry, request.ReceivingCountry, (TransactionTransferMethod)request.TransferMethod, request.SendingAmount,
        //                                           (TransactionTransferType)request.TransferType, request.AgentId, request.IsAuxAgnet);

        //    return feeInfo;
        //}
        //private EstimateFaxingFeeSummary GetEstimatedSummary(PaymentSummaryRequestParamVm request, TransferFeePercentage feeInfo)
        //{
        //    var exchangeRateReceivingCountry = Common.Common.GetCountryCodeByCurrency(request.ReceivingCurrency);

        //    decimal exchangeRate = SExchangeRate.GetExchangeRateValue(
        //         request.SendingCountry, exchangeRateReceivingCountry,
        //        (TransactionTransferMethod)request.TransferMethod, request.AgentId, (TransactionTransferType)request.TransferType, request.IsAuxAgnet);
        //    var estimatedSummary = SEstimateFee.CalculateFaxingFee(request.SendingAmount, false,
        //        request.IsReceivingAmount, exchangeRate
        //        , feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false);


        //    return estimatedSummary;
        //}



        //private EstimateFaxingFeeSummary GetIntroductoryRate(PaymentSummaryRequestParamVm request, TransferFeePercentage feeInfo)
        //{

        //    var exchangeRateReceivingCountry = Common.Common.GetCountryCodeByCurrency(request.ReceivingCurrency);

        //    var introductoryRateResult = SEstimateFee.GetIntroductoryTransferSummary(request.SendingCountry,
        //        exchangeRateReceivingCountry, request.SendingAmount
        //       , feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false, false,
        //        (TransactionTransferMethod)request.TransferMethod, request.AgentId, request.ForAgent);

        //    return introductoryRateResult;
        //}

    }
}