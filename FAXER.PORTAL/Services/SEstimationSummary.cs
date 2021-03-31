using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Models.PaymentSummary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SEstimationSummary
    {

        public void SetEstimationSummaryToSession(PaymentSummaryResponseVm result)
        {

            SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
            CommonEnterAmountViewModel enterAmount = new CommonEnterAmountViewModel()
            {
                Fee = result.Fee,
                SendingAmount = result.SendingAmount,
                ReceivingAmount = result.ReceivingAmount,
                TotalAmount = result.TotalAmount,
                ExchangeRate = result.ExchangeRate,
                SendingCurrencySymbol = result.SendingCurrencySymbol,
                ReceivingCurrencySymbol = result.ReceivingCurrencySymbol,
                SendingCountryCode = result.SendingCountry,
                ReceivingCountryCode = result.ReceivingCountry,
                SendingCurrency = result.SendingCurrency,
                ReceivingCurrency = result.ReceivingCurrency,
            };
            // Rewrite session with additional value 

            _kiiPaytrasferServices.SetCommonEnterAmount(enterAmount);
        }

        public ServiceResult<bool> CheckIfIsValidTransfer(PaymentSummaryRequestParamVm request, decimal ReceivingAmount)
        {


            var transactionValidationResult = Common.Common.IsValidTransactionLimit(request.SendingCountry,
                request.ReceivingCountry, ReceivingAmount, (TransactionTransferMethod)request.TransferMethod);
            //var transactionValidationResult = Common.Common.IsValidTransactionLimit(request, ReceivingAmount);
            return transactionValidationResult;

        }

        public PaymentSummaryResponseVm BindEstimateSummaryModelToPaymentSummaryModel(EstimateFaxingFeeSummary model)
        {
            var response = new PaymentSummaryResponseVm()
            {
                ExchangeRate = model.ExchangeRate,
                Fee = model.FaxingFee,
                ReceivingAmount = model.ReceivingAmount,
                ReceivingCountry = model.ReceivingCountry,
                SendingCountry = model.SendingCountry,
                SendingCurrency = model.FaxingCurrency,
                ReceivingCurrency = model.ReceivingCurrency,
                SendingAmount = model.FaxingAmount,
                TotalAmount = model.TotalAmount,
                IsIntroductoryRate = model.IsIntroductoryRate,
                IsIntroductoryFee = model.IsIntroductoryFee,
                ReceivingCurrencySymbol = model.ReceivingCurrencySymbol,
                SendingCurrencySymbol = model.FaxingCurrencySymbol,
                ActualFee = model.ActualFee,
            };
            return response;

        }

        public TransferFeePercentage GetTransferFee(PaymentSummaryRequestParamVm request)
        {

            //var feeInfo = SEstimateFee.GetTransferFee(request.SendingCountry, request.ReceivingCountry,
            //    (TransactionTransferMethod)request.TransferMethod, request.SendingAmount,
            //                                       (TransactionTransferType)request.TransferType,

            //                                       request.AgentId, request.IsAuxAgnet);

            var feeInfo = SEstimateFee.GetTransferFee(new TransferFeeRequestParam()
            {
                SendingCountry = request.SendingCountry,
                ReceivingCountry = request.ReceivingCountry,
                Amount = request.SendingAmount,
                AgentId = request.AgentId,
                IsAuxAgent = request.IsAuxAgnet,
                TransactionTransferMethod = (TransactionTransferMethod)request.TransferMethod,
                transactiontransfertype = (TransactionTransferType)request.TransferType
            });
            return feeInfo;
        }
        public EstimateFaxingFeeSummary GetEstimatedSummary(PaymentSummaryRequestParamVm request, TransferFeePercentage feeInfo)
        {
            var exchangeRateReceivingCountry = Common.Common.GetCountryCodeByCurrency(request.ReceivingCurrency);

            decimal exchangeRate = SExchangeRate.GetExchangeRateValue(
                 request.SendingCountry, exchangeRateReceivingCountry,
                (TransactionTransferMethod)request.TransferMethod, request.AgentId, (TransactionTransferType)request.TransferType, request.IsAuxAgnet);
            var estimatedSummary = SEstimateFee.CalculateFaxingFee(request.SendingAmount, false,
                request.IsReceivingAmount, exchangeRate
                , feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false);


            return estimatedSummary;
        }



        public EstimateFaxingFeeSummary GetIntroductoryRate(PaymentSummaryRequestParamVm request, TransferFeePercentage feeInfo , int SenderId = 0)
        {

            var exchangeRateReceivingCountry = Common.Common.GetCountryCodeByCurrency(request.ReceivingCurrency);

            var introductoryRateResult = SEstimateFee.GetIntroductoryTransferSummary(request.SendingCountry,
                exchangeRateReceivingCountry, request.SendingAmount
               , feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false, false,
                (TransactionTransferMethod)request.TransferMethod, request.AgentId, request.ForAgent , SenderId );

            return introductoryRateResult;
        }

    }
}