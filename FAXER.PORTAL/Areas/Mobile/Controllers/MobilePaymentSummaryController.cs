using FAXER.PORTAL.Areas.Mobile.Models.MobilePaymentSummary;
using FAXER.PORTAL.Areas.Mobile.Services;
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

namespace FAXER.PORTAL.Areas.Mobile.Controllers
{
    public class MobilePaymentSummaryController : Controller
    {
        // GET: Mobile/MobilePaymentSummary

        [HttpPost]
        public JsonResult GetPaymentSummary(PaymentSummaryArgument model)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                MobilePaymentSummaryServices paymentSummaryServices = new MobilePaymentSummaryServices();
                var paymentSummary = paymentSummaryServices.GetPaymentSummary(model);
                var services = paymentSummaryServices.GetEnabledServices(model.SendingCountry, model.ReceivingCountry);
                var IsValid = FAXER.PORTAL.Common.Common.IsValidTransactionLimit(model.SendingCountry, model.ReceivingCountry, paymentSummary.Data.ReceivingAmount, model.TransferMethod);


                MobilePaymentAndServiceSummaryVm result = new MobilePaymentAndServiceSummaryVm()
                {
                    PaymentSummary = paymentSummary.Data,
                    Services = services.Data,
                    IsValidAmount = IsValid.Data,
                    AmountValidationMessage = IsValid.Message
                };

                return Json(new ServiceResult<MobilePaymentAndServiceSummaryVm>()
                {
                    Data = result,
                    Message = "AddedSuccess",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<MobilePaymentAndServiceSummaryVm>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);

            }

        }



        [HttpPost]
        public JsonResult GetTransferSummary(PaymentSummaryArgument request)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                MobilePaymentSummaryServices paymentSummaryServices = new MobilePaymentSummaryServices();

                var response = new MobilePaymentAndServiceSummaryVm();
                if (request.IsReceivingAmount)
                {
                    request.SendingAmount = request.ReceivingAmount;
                }
                // Check if fee has been setup or not 
                var feeInfo = GetTransferFee(request);
                if (feeInfo == null) // if feeinfo is null then fee hasn't been setup
                {
                    response.PaymentSummary.SendingCountry = request.SendingCountry;
                    response.PaymentSummary.ReceivingCountry = request.ReceivingCountry;
                    response.PaymentSummary.SendingCurrencyCode = request.SendingCurrency;
                    response.PaymentSummary.ReceivingCurrencyCode = request.ReceivingCurrency;
                    response.PaymentSummary.SendingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbolByCurrencyCode(request.SendingCurrency);
                    response.PaymentSummary.ReceivingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbolByCurrencyCode(request.ReceivingCurrency);
                    response.PaymentSummary.TransferMethod = request.TransferMethod;
                    response.Services = paymentSummaryServices.GetEnabledServices(request.SendingCountry, request.ReceivingCountry, request.ReceivingCurrency).Data;
                    var isValid = CheckIfIsValidTransfer(request, response.PaymentSummary.ReceivingAmount);
                    response.IsValidAmount = isValid.Data;
                    response.AmountValidationMessage = isValid.Message;
                    return Json(new ServiceResult<MobilePaymentAndServiceSummaryVm>()
                    {
                        Data = response,
                        Message = "",
                        Status = ResultStatus.Warning
                    }, JsonRequestBehavior.AllowGet);


                }
                else
                {

                    var estimateSummary = GetEstimatedSummary(request, feeInfo);

                    var introductoryRate = GetIntroductoryRate(request, feeInfo);
                    if (introductoryRate != null)
                    {

                        response.PaymentSummary = BindEstimateSummaryModelToPaymentSummaryModel(introductoryRate);
                    }
                    else
                    {
                        response.PaymentSummary = BindEstimateSummaryModelToPaymentSummaryModel(estimateSummary);
                    }
                    var result_data = paymentSummaryServices.GetPaymentSummary(request);
                    response.PaymentSummary = result_data.Data;


                    response.Services = paymentSummaryServices.GetEnabledServices(request.SendingCountry, request.ReceivingCountry, request.ReceivingCurrency).Data;
                    var isValid = CheckIfIsValidTransfer(request, response.PaymentSummary.ReceivingAmount);
                    response.IsValidAmount = isValid.Data;
                    response.AmountValidationMessage = isValid.Message;
                    response.PaymentSummary.SendingCountry = request.SendingCountry;
                    response.PaymentSummary.ReceivingCountry = request.ReceivingCountry;
                    response.PaymentSummary.SendingCurrencyCode = request.SendingCurrency;
                    response.PaymentSummary.ReceivingCurrencyCode = request.ReceivingCurrency;
                    response.PaymentSummary.SendingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbolByCurrencyCode(request.SendingCurrency);
                    response.PaymentSummary.ReceivingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbolByCurrencyCode(request.ReceivingCurrency);
                    response.PaymentSummary.TransferMethod = request.TransferMethod;
                    return Json(new ServiceResult<MobilePaymentAndServiceSummaryVm>()
                    {
                        Data = response,
                        Message = "",
                        Status = ResultStatus.OK
                    }, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                return Json(new ServiceResult<MobilePaymentAndServiceSummaryVm>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);

            }
        }


        //[HttpPost]
        //public JsonResult GetTransferSummary(PaymentSummaryArgument request)
        //{
        //    string token = FAXER.PORTAL.Common.Common.RequestToken;
        //    if (FAXER.PORTAL.Common.Common.ValidateToken(token))
        //    {
        //        MobilePaymentSummaryServices paymentSummaryServices = new MobilePaymentSummaryServices();

        //        var response = new MobilePaymentAndServiceSummaryVm();

        //        var result_Data = paymentSummaryServices.GetPaymentSummary(request);

        //        response.PaymentSummary.SendingCountry = request.SendingCountry;
        //        response.PaymentSummary.ReceivingCountry = request.ReceivingCountry;
        //        response.PaymentSummary.SendingCurrencyCode = request.SendingCurrency;
        //        response.PaymentSummary.ReceivingCurrencyCode = request.ReceivingCurrency;
        //        response.PaymentSummary.SendingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbolByCurrencyCode(request.SendingCurrency);
        //        response.PaymentSummary.ReceivingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbolByCurrencyCode(request.ReceivingCurrency);
        //        response.PaymentSummary.TransferMethod = request.TransferMethod;
        //        response.Services = paymentSummaryServices.GetEnabledServices(request.SendingCountry, request.ReceivingCountry, request.ReceivingCurrency).Data;
        //        var isValid = CheckIfIsValidTransfer(request, response.PaymentSummary.ReceivingAmount);
        //        response.IsValidAmount = isValid.Data;
        //        response.AmountValidationMessage = isValid.Message;

        //        // Check if fee has been setup or not 
        //        //var feeInfo = GetTransferFee(request);
        //        //if (feeInfo == null) // if feeinfo is null then fee hasn't been setup
        //        //{
        //        //    response.PaymentSummary.SendingCountry = request.SendingCountry;
        //        //    response.PaymentSummary.ReceivingCountry = request.ReceivingCountry;
        //        //    response.PaymentSummary.SendingCurrencyCode = request.SendingCurrency;
        //        //    response.PaymentSummary.ReceivingCurrencyCode = request.ReceivingCurrency;
        //        //    response.PaymentSummary.SendingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbolByCurrencyCode(request.SendingCurrency);
        //        //    response.PaymentSummary.ReceivingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbolByCurrencyCode(request.ReceivingCurrency);
        //        //    response.PaymentSummary.TransferMethod = request.TransferMethod;

        //        //    return Json(new ServiceResult<MobilePaymentAndServiceSummaryVm>()
        //        //    {
        //        //        Data = response,
        //        //        Message = "",
        //        //        Status = ResultStatus.Warning
        //        //    }, JsonRequestBehavior.AllowGet);


        //        //}
        //        //else
        //        //{

        //        //    var estimateSummary = GetEstimatedSummary(request, feeInfo);

        //        //    var introductoryRate = GetIntroductoryRate(request, feeInfo);
        //        //    if (introductoryRate != null)
        //        //    {

        //        //        response.PaymentSummary = BindEstimateSummaryModelToPaymentSummaryModel(introductoryRate);
        //        //    }
        //        //    else
        //        //    {
        //        //        response.PaymentSummary = BindEstimateSummaryModelToPaymentSummaryModel(estimateSummary);
        //        //    }

        //        //    response.Services = paymentSummaryServices.GetEnabledServices(request.SendingCountry, request.ReceivingCountry, request.ReceivingCurrency).Data;
        //        //    var isValid = CheckIfIsValidTransfer(request, response.PaymentSummary.ReceivingAmount);
        //        //    response.IsValidAmount = isValid.Data;
        //        //    response.AmountValidationMessage = isValid.Message;
        //        //    response.PaymentSummary.SendingCountry = request.SendingCountry;
        //        //    response.PaymentSummary.ReceivingCountry = request.ReceivingCountry;
        //        //    response.PaymentSummary.SendingCurrencyCode = request.SendingCurrency;
        //        //    response.PaymentSummary.ReceivingCurrencyCode = request.ReceivingCurrency;
        //        //    response.PaymentSummary.SendingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbolByCurrencyCode(request.SendingCurrency);
        //        //    response.PaymentSummary.ReceivingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbolByCurrencyCode(request.ReceivingCurrency);
        //        //    response.PaymentSummary.TransferMethod = request.TransferMethod;
        //        //    return Json(new ServiceResult<MobilePaymentAndServiceSummaryVm>()
        //        //    {
        //        //        Data = response,
        //        //        Message = "",
        //        //        Status = ResultStatus.OK
        //        //    }, JsonRequestBehavior.AllowGet);

        //        //}

        //        return Json(new ServiceResult<MobilePaymentAndServiceSummaryVm>()
        //        {
        //            Data = null,
        //            Message = "",
        //            Status = ResultStatus.Warning
        //        }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(new ServiceResult<MobilePaymentAndServiceSummaryVm>()
        //        {
        //            Data = null,
        //            Message = "",
        //            Status = ResultStatus.Warning
        //        }, JsonRequestBehavior.AllowGet);

        //    }
        //}



        private ServiceResult<bool> CheckIfIsValidTransfer(PaymentSummaryArgument request, decimal ReceivingAmount)
        {

            var transactionValidationResult = FAXER.PORTAL.Common.Common.IsValidTransactionLimit(request.SendingCountry,
                request.ReceivingCountry, ReceivingAmount, (TransactionTransferMethod)request.TransferMethod);

            return transactionValidationResult;

        }

        private MobilePaymentSummaryVm BindEstimateSummaryModelToPaymentSummaryModel(EstimateFaxingFeeSummary model)
        {
            var response = new MobilePaymentSummaryVm()
            {
                ExchangeRate = model.ExchangeRate,
                Fee = model.FaxingFee,
                ReceivingAmount = model.ReceivingAmount,
                ReceivingCountry = model.ReceivingCountry,
                SendingCountry = model.SendingCountry,
                SendingCurrencyCode = model.FaxingCurrency,
                ReceivingCurrencyCode = model.ReceivingCurrency,
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

        private TransferFeePercentage GetTransferFee(PaymentSummaryArgument request)
        {

            var feeInfo = SEstimateFee.GetTransferFee(request.SendingCountry, request.ReceivingCountry, (TransactionTransferMethod)request.TransferMethod, request.SendingAmount);

            return feeInfo;
        }
        private EstimateFaxingFeeSummary GetEstimatedSummary(PaymentSummaryArgument request, TransferFeePercentage feeInfo)
        {
            var exchangeRateReceivingCountry = FAXER.PORTAL.Common.Common.GetCountryCodeByCurrency(request.ReceivingCurrency);

            decimal exchangeRate = SExchangeRate.GetExchangeRateValue(
                 request.SendingCountry, exchangeRateReceivingCountry,
                (TransactionTransferMethod)request.TransferMethod);
            var estimatedSummary = SEstimateFee.CalculateFaxingFee(request.SendingAmount, false,
                request.IsReceivingAmount, exchangeRate
                , feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false);


            return estimatedSummary;
        }



        private EstimateFaxingFeeSummary GetIntroductoryRate(PaymentSummaryArgument request, TransferFeePercentage feeInfo)
        {

            var exchangeRateReceivingCountry = FAXER.PORTAL.Common.Common.GetCountryCodeByCurrency(request.ReceivingCurrency);

            var introductoryRateResult = SEstimateFee.GetIntroductoryTransferSummary(request.SendingCountry,
                exchangeRateReceivingCountry, request.SendingAmount
               , feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false, false,
                (TransactionTransferMethod)request.TransferMethod);

            return introductoryRateResult;
        }


    }

    public class MobilePaymentAndServiceSummaryVm
    {

        public MobilePaymentSummaryVm PaymentSummary { get; set; }
        public List<EnabledTransferMethodVm> Services { get; set; }

        public bool IsValidAmount { get; set; }
        public string AmountValidationMessage { get; set; }

    }
}