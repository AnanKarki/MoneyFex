using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Models.PaymentSummary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SSenderKiiPayWalletTransfer
    {

        DB.FAXEREntities dbContext = null;

        public SSenderKiiPayWalletTransfer()
        {
            dbContext = new DB.FAXEREntities();

        }
        public SSenderKiiPayWalletTransfer(FAXEREntities dbContext)
        {
            this.dbContext = dbContext;

        }


        public DB.KiiPayPersonalWalletInformation ValidateMobileNo(string MobileNo, string CountrCode)
        {
            var result = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == MobileNo && x.CardUserCountry == CountrCode).FirstOrDefault();
            return result;
        }
        public void SetSearchKiiPayWallet(SearchKiiPayWalletVM vm)
        {


            Common.FaxerSession.SearchKiiPayWalletVM = vm;

        }


        public void SetSenderAndReceiverDetails(SenderAndReceiverDetialVM vm)
        {

            Common.FaxerSession.SenderAndReceiverDetials = vm;
        }
        public SenderAndReceiverDetialVM GetSenderAndReceiverDetails()
        {

            SenderAndReceiverDetialVM vm = new SenderAndReceiverDetialVM();

            if (Common.FaxerSession.SenderAndReceiverDetials != null)
            {

                vm = Common.FaxerSession.SenderAndReceiverDetials;
            }
            return vm;
        }


        public SearchKiiPayWalletVM GetSearchKiiPayWallet()
        {



            SearchKiiPayWalletVM vm = new SearchKiiPayWalletVM();
            if (Common.FaxerSession.SearchKiiPayWalletVM != null)
            {

                vm = Common.FaxerSession.SearchKiiPayWalletVM;
            }

            return vm;
        }

        public void SetTransferSummaryAgain(PaymentSummaryRequestParamVm request)
        {
            SEstimationSummary estimationSummaryServices = new SEstimationSummary();

            var response = new PaymentSummaryResponseVm();

            var feeInfo = estimationSummaryServices.GetTransferFee(request);
            var estimateSummary = estimationSummaryServices.GetEstimatedSummary(request, feeInfo);

            var introductoryRate = estimationSummaryServices.GetIntroductoryRate(request, feeInfo);
            if (introductoryRate != null)
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
        }


        public void SetCommonEnterAmount(CommonEnterAmountViewModel vm)
        {
            Common.FaxerSession.CommonEnterAmountViewModel = vm;

        }

        public CommonEnterAmountViewModel GetCommonEnterAmount()
        {

            CommonEnterAmountViewModel vm = new CommonEnterAmountViewModel();
            if (Common.FaxerSession.CommonEnterAmountViewModel != null)
            {

                vm = Common.FaxerSession.CommonEnterAmountViewModel;
            }

            return vm;
        }

        public void SetKiiPayTransferPaymentSummary(KiiPayTransferPaymentSummary vm)
        {
            if (Common.FaxerSession.IsTransferFromHomePage == true)
            {
                var dashboarddata = GetCommonEnterAmount();
                if (dashboarddata != null && !string.IsNullOrEmpty(dashboarddata.SendingCountryCode))
                {
                    vm.SendingCurrencySymbol = dashboarddata.SendingCurrencySymbol;

                    vm.ReceivingCurrencySymbol = dashboarddata.ReceivingCurrencySymbol;

                    vm.ExchangeRate = dashboarddata.ExchangeRate;
                    vm.SendingAmount = dashboarddata.SendingAmount;
                    vm.ReceivingAmount = dashboarddata.ReceivingAmount;
                    vm.Fee = dashboarddata.Fee;
                    vm.TotalAmount = dashboarddata.TotalAmount;

                    Common.FaxerSession.KiiPayTransferPaymentSummary = vm;
                }
            }
            {
                Common.FaxerSession.KiiPayTransferPaymentSummary = vm;
            }


        }

        public List<RecipientsViewModel> GetReceiverInformation(int faxerId = 0, string Text = "")
        {
            var data = dbContext.Recipients.Where(x => x.SenderId == faxerId);
            if (!string.IsNullOrEmpty(Text))
            {

                data = data.Where(x => x.ReceiverName.ToLower().StartsWith(Text.ToLower()));
            }

            var result = (from c in data.ToList()
                          select new RecipientsViewModel()
                          {
                              Id = c.Id,
                              SenderId = c.SenderId,
                              MobileNo = c.MobileNo,
                              Country = Common.Common.GetCountryCurrency(c.Country),
                              ReceiverName = c.ReceiverName,
                              AccountNo = c.AccountNo,
                              BankId = c.BankId,
                              BankName = Common.Common.getBankName(c.BankId),
                              BranchCode = c.BranchCode,
                              IBusiness = c.IBusiness,
                              MobileWalletProvider = c.MobileWalletProvider,
                              Service = c.Service,
                              ServiceName = Common.Common.GetEnumDescription(c.Service),
                              MobileWalletProviderName = GetMobileWalletname(c.MobileWalletProvider),
                              ReceiverCountryLower = c.Country != null ? c.Country.ToLower() : "",
                              ReciverFirstLetter = GetReceiverFirstLetter(c.ReceiverName),
                          }).OrderByDescending(x => x.Id).ToList();

            return result;
        }

        private string GetReceiverFirstLetter(string receiverName)
        {
            string name = receiverName;
            if (string.IsNullOrEmpty(receiverName))
            {

                return "";
            }
            var firstletter = name.Substring(0, 1);
            return firstletter;

        }

        public string GetMobileWalletname(int mobileWalletProvider)
        {
            string MobileWalletProvide = dbContext.MobileWalletOperator.Where(x => x.Id == mobileWalletProvider).Select(x => x.Name).FirstOrDefault();
            return MobileWalletProvide;
        }

        public KiiPayTransferPaymentSummary GetKiiPayTransferPaymentSummary()
        {

            KiiPayTransferPaymentSummary vm = new KiiPayTransferPaymentSummary();
            if (Common.FaxerSession.KiiPayTransferPaymentSummary != null)
            {

                vm = Common.FaxerSession.KiiPayTransferPaymentSummary;
            }

            return vm;
        }


        public void SetPaymentMethod(PaymentMethodViewModel vm)
        {


            Common.FaxerSession.PaymentMethodViewModel = vm;
        }

        public PaymentMethodViewModel GetPaymentMethod()
        {

            PaymentMethodViewModel vm = new PaymentMethodViewModel();

            if (Common.FaxerSession.PaymentMethodViewModel != null)
            {

                vm = Common.FaxerSession.PaymentMethodViewModel;
            }
            return vm;

        }

        //public void SetPaymentCompletedVM() {

        //    PaymentCompletedVM vm = new PaymentCompletedVM();
        //    Common.FaxerSession.PaymentCompletedVM = vm;
        //}
        public PaymentCompletedVM CompletePayment()
        {


            PaymentCompletedVM vm = new PaymentCompletedVM();
            return vm;
        }
        public SearchKiiPayWalletVM GetReceiverInformationFromMobileNo(string MobileNo, int id)
        {
            var data = (from c in dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == MobileNo && x.Id == id).ToList()
                        select new SearchKiiPayWalletVM()
                        {
                            ReceiverName = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                            CountryCode = c.CardUserCountry,
                            MobileNo = c.MobileNo,
                            RecentMobileNo = c.MobileNo
                        }).FirstOrDefault();



            return data;
        }

        public SenderMobileEnrterAmountVm GetRepeatedTranscationIfo(string mobileNo, int id)
        {
            var data = (from c in dbContext.MobileMoneyTransfer.Where(x => x.PaidToMobileNo == mobileNo && x.Id == id).ToList()
                        select new SenderMobileEnrterAmountVm()
                        {
                            SendingAmount = c.SendingAmount,
                            ExchangeRate = c.ExchangeRate,
                            Fee = c.Fee,
                            ReceivingAmount = c.ReceivingAmount,
                            TotalAmount = c.TotalAmount,
                            ReceiverName = c.ReceiverName,
                            ReceivingCountryCode = c.ReceivingCountry,
                            ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(c.ReceivingCountry),
                            ReceivingCurrencyCode = Common.Common.GetCurrencyCode(c.ReceivingCountry),
                            SendingCountryCode = c.SendingCountry,
                            SendingCurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                            SendingCurrencyCode = Common.Common.GetCurrencyCode(c.SendingCountry),

                        }).FirstOrDefault();



            return data;


        }

        public void setPaymentSummary(TransactionTransferMethod transferMethod)
        {


            //SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();

            var param = GetCommonEnterAmount();

            //var feeInfo = SEstimateFee.GetTransferFee(param.SendingCountryCode, param.ReceivingCountryCode, (TransactionTransferMethod)transferMethod, param.SendingAmount);

            //var result = new EstimateFaxingFeeSummary();


            //result = SEstimateFee.CalculateFaxingFee(param.SendingAmount, false, false,
            //    SExchangeRate.GetExchangeRateValue(param.SendingCountryCode, param.ReceivingCountryCode, (TransactionTransferMethod)transferMethod), feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false);


            //var introductoryRateResult = SEstimateFee.GetIntroductoryTransferSummary(param.SendingCountryCode, param.ReceivingCountryCode, result.FaxingAmount
            //    , feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false, false, (TransactionTransferMethod)transferMethod);

            //if (introductoryRateResult != null)
            //{

            //    result = introductoryRateResult;
            //}

            //CommonEnterAmountViewModel enterAmount = new CommonEnterAmountViewModel()
            //{
            //    Fee = result.FaxingFee,
            //    SendingAmount = result.FaxingAmount,
            //    ReceivingAmount = result.ReceivingAmount,
            //    TotalAmount = result.TotalAmount,
            //    ExchangeRate = result.ExchangeRate,
            //    SendingCurrencySymbol = param.SendingCurrencySymbol, //Common.Common.GetCurrencySymbol(param.SendingCountryCode),
            //    ReceivingCurrencySymbol = param.ReceivingCurrencySymbol,  //Common.Common.GetCurrencySymbol(param.ReceivingCountryCode),
            //    SendingCountryCode = param.SendingCountryCode,
            //    ReceivingCountryCode = param.ReceivingCountryCode,
            //    SendingCurrency = param.SendingCurrency, //Common.Common.GetCountryCurrency(param.SendingCountryCode),
            //    ReceivingCurrency = param.ReceivingCurrency,//Common.Common.GetCountryCurrency(param.ReceivingCountryCode),
            //};
            //// Rewrite session with additional value 

            //SetCommonEnterAmount(enterAmount);
        }

        #region For Staff
        public void SetStaffCommonEnterAmount(CommonEnterAmountViewModel vm)
        {
            Common.StaffSession.CommonEnterAmountViewModel = vm;

        }

        public CommonEnterAmountViewModel GetStaffCommonEnterAmount()
        {

            CommonEnterAmountViewModel vm = new CommonEnterAmountViewModel();
            if (Common.StaffSession.CommonEnterAmountViewModel != null)
            {

                vm = Common.StaffSession.CommonEnterAmountViewModel;
            }

            return vm;
        }

        #endregion
        public Recipients GetRecipientsInfo(int Id = 0)
        {
            var data = dbContext.Recipients.Where(x => x.Id == Id).FirstOrDefault();
            return data;
        }
    }
}