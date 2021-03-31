using FAXER.PORTAL.Areas.Mobile.Models.MobilePaymentSummary;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Models.PaymentSummary;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace FAXER.PORTAL.Areas.Mobile.Services
{
    public class MobilePaymentSummaryServices
    {
        DB.FAXEREntities dbContext = null;

        public MobilePaymentSummaryServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public ServiceResult<MobilePaymentSummaryVm> GetPaymentSummary(PaymentSummaryArgument args)
        {

            if (args.IsReceivingAmount)
            {
                args.SendingAmount = args.ReceivingAmount;
            }



            var feeInfo = SEstimateFee.GetTransferFee(args.SendingCountry, args.ReceivingCountry,
                args.TransferMethod, args.SendingAmount);
            if (feeInfo == null)
            {
                return new ServiceResult<MobilePaymentSummaryVm>()
                {

                    Data = new MobilePaymentSummaryVm(),
                    Message = "Service not available",
                    Status = ResultStatus.Error
                };
            }
            else
            {



                //var summary = SEstimateFee.CalculateFaxingFee(args.SendingAmount, false, args.IsReceivingAmount,
                //    SExchangeRate.GetExchangeRateValue(args.SendingCountry, args.ReceivingCountry,
                //    args.TransferMethod), feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false);

                var summary = GetTransferSummary(args);

                var result = new MobilePaymentSummaryVm()
                {
                    SendingAmount = summary.SendingAmount,
                    TotalAmount = summary.TotalAmount,
                    Fee = summary.Fee,
                    ActualFee = summary.ActualFee,
                    ExchangeRate = summary.ExchangeRate,
                    TransferMethod = args.TransferMethod,
                    ReceivingAmount = summary.ReceivingAmount,
                    ReceivingCountry = args.ReceivingCountry,
                    ReceivingCurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyCode(args.ReceivingCountry),
                    ReceivingFlagCode = args.ReceivingCountry,
                    ReceivingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbol(args.ReceivingCountry),
                    SendingCountry = args.SendingCountry,
                    SendingCurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyCode(args.SendingCountry),
                    SendingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbol(args.SendingCountry),
                    SendingFlagCode = args.SendingCountry

                };

                //var IntroResult = GetIntroductoryRateSummary(args, feeInfo);
                //if (IntroResult != null)
                //{

                //    result = IntroResult;
                //}
                return new ServiceResult<MobilePaymentSummaryVm>()
                {
                    Data = result,
                    Message = "",
                    Status = ResultStatus.OK
                };
            }
        }

        private PaymentSummaryResponseVm GetTransferSummary(PaymentSummaryArgument args)
        {

            SEstimationSummary estimationSummaryServices = new SEstimationSummary();
            PaymentSummaryRequestParamVm request = new PaymentSummaryRequestParamVm();
            request.SendingCountry = args.SendingCountry;
            request.ReceivingCountry = args.ReceivingCountry;
            request.SendingAmount = args.SendingAmount;
            request.IsReceivingAmount = args.IsReceivingAmount;
            request.ReceivingAmount = args.ReceivingAmount;
            request.SendingCurrency = args.SendingCurrency;
            request.ReceivingCurrency = args.ReceivingCurrency;
            request.TransferMethod = (int)args.TransferMethod;
            request.TransferType = (int)TransactionTransferType.Online;

            var response = new PaymentSummaryResponseVm();
            if (request.IsReceivingAmount)
            {
                request.SendingAmount = request.ReceivingAmount;
            }
            // Check if fee has been setup or not 
            var feeInfo = estimationSummaryServices.GetTransferFee(request);



            var estimateSummary = estimationSummaryServices.GetEstimatedSummary(request, feeInfo);

            var transactionCount = FAXER.PORTAL.Common.Common.GetTotalTransactionCount(args.SenderId);

            
            Log.Write( (args.SenderId.ToString()  ?? "0") + " : Estimation Summary");
            if (transactionCount  ==  0)
            {
                var introductoryRate = estimationSummaryServices.GetIntroductoryRate(request, feeInfo, args.SenderId);
                if (introductoryRate != null)
                {

                    response = estimationSummaryServices.BindEstimateSummaryModelToPaymentSummaryModel(introductoryRate);
                }
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
            response.SendingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbolByCurrencyCode(request.SendingCurrency);
            response.ReceivingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbolByCurrencyCode(request.ReceivingCurrency);
            //estimationSummaryServices.SetEstimationSummaryToSession(response);

            return response;


        }
        private MobilePaymentSummaryVm GetIntroductoryRateSummary(PaymentSummaryArgument args, TransferFeePercentage feeInfo)
        {

            var transactionCount = FAXER.PORTAL.Common.Common.GetTotalTransactionCount(args.SenderId);
            if (transactionCount > 0)
            {
                return null;
            }
            if (args.SenderId == 0)
            {

                return null;
            }
            var summary = SEstimateFee.GetIntroductoryRateEstimation(args.SendingCountry, args.ReceivingCountry, args.SendingAmount
                    , feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false,
                    args.IsReceivingAmount, args.TransferMethod);

            if (summary == null)
            {

                return null;
            }

            var result = new MobilePaymentSummaryVm()
            {
                SendingAmount = summary.FaxingAmount,
                TotalAmount = summary.TotalAmount,
                Fee = summary.FaxingFee,
                ActualFee = summary.ActualFee,
                ExchangeRate = summary.ExchangeRate,
                TransferMethod = args.TransferMethod,
                ReceivingAmount = summary.ReceivingAmount,
                ReceivingCountry = args.ReceivingCountry,
                ReceivingCurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyCode(args.ReceivingCountry),
                ReceivingFlagCode = args.ReceivingCountry,
                ReceivingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbol(args.ReceivingCountry),
                SendingCountry = args.SendingCountry,
                SendingCurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyCode(args.SendingCountry),
                SendingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbol(args.SendingCountry),
                SendingFlagCode = args.SendingCountry,
                IsIntroductoryRate = summary.IsIntroductoryRate,
                IsIntroductoryFee = summary.IsIntroductoryFee
            };

            return result;

        }



        public ServiceResult<List<EnabledTransferMethodVm>> GetEnabledServices(string SendingCountry, string ReceivingCountry)
        {
            var result = (from c in dbContext.TransferServiceMaster.Where(x => x.SendingCountry == SendingCountry
                         && x.ReceivingCountry == ReceivingCountry)
                          join d in dbContext.TransferServiceDetails on c.Id equals d.TransferMasterId
                          select new EnabledTransferMethodVm()
                          {
                              TransferMethod = (int)d.ServiceType,
                              IsEnabled = true

                          }).ToList();
            return new ServiceResult<List<EnabledTransferMethodVm>>()
            {
                Data = result,
                Message = "",
                Status = ResultStatus.OK
            };
        }


        internal ServiceResult<List<EnabledTransferMethodVm>> GetEnabledServices(string sendingCountry, string receivingCountry, string receivingCurrency)
        {
            var transfermaster = (from c in dbContext.TransferServiceMaster.Where(x => x.SendingCountry == sendingCountry &&
                                                                      x.ReceivingCountry == receivingCountry).ToList()
                                  select new TransferServiceMaster()
                                  {
                                      Id = c.Id,
                                      SendingCountry = c.SendingCountry,
                                      ReceivingCountry = c.ReceivingCountry,
                                      ReceivingCurrency = c.ReceivingCurrency == null ? FAXER.PORTAL.Common.Common.GetCountryCurrency(c.ReceivingCountry) : c.ReceivingCurrency,
                                  });


            List<EnabledTransferMethodVm> result = (from c in dbContext.TransferServiceDetails.ToList()
                                                    join d in transfermaster.Where(x => x.ReceivingCurrency == receivingCurrency) on c.TransferMasterId equals d.Id
                                                    select new EnabledTransferMethodVm()
                                                    {
                                                        TransferMethod = (int)c.ServiceType,
                                                        IsEnabled = true
                                                    }).ToList();


            return new ServiceResult<List<EnabledTransferMethodVm>>()
            {
                Data = result,
                Message = "",
                Status = ResultStatus.OK
            };

        }




    }


}