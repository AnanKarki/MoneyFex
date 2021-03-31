using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Areas.Businesses.Services;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Businesses.Controllers
{
    public class MyBusinessMoneyFaxCreditSheetController : Controller
    {
        DB.FAXEREntities dbContext = null;
        CommonServices CommonService = new CommonServices();

        public MyBusinessMoneyFaxCreditSheetController()
        {
            dbContext = new DB.FAXEREntities();
        }
        // GET: Businesses/MyBusinessMoneyFaxCreditSheet
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MyBusinessMoneyFaxCreditSheet(string year = "", int monthId = 0)
        {

            if (Common.BusinessSession.LoggedBusinessMerchant == null)
            {

                return RedirectToAction("Login", "BusinessLogin");
            }
            ViewModels.MyBusinessMoneyFaxCreditSheetViewModel vm = new ViewModels.MyBusinessMoneyFaxCreditSheetViewModel();
            vm.TotalMonthlySalesCount = "0";
            vm.TotalYearlyCreditedAmount = "0.00";
            vm.TotalMonthlyCreditedAmount = "0.00";
            vm.OutstandingCredit = "0.00";
            vm.Year = year;
            vm.Month = (Month)monthId;
            vm.Currency = CommonService.getBusinessCurrency(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId);
            vm.CurrencySymbol = CommonService.getBusinessCurrencySymbol(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId);
            if (!string.IsNullOrEmpty(year))
            {
                int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
                Services.MFBCCardProfileServices mFBCCardProfileServices = new Services.MFBCCardProfileServices();
                int yearParam = int.Parse(year);
                var MFBCCardDetails = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId).FirstOrDefault();
                Services.MoneyFaxCreditSheetServices moneyFaxCreditSheetServices = new Services.MoneyFaxCreditSheetServices();
                var MFBCtransactionYearwise = dbContext.KiiPayBusinessLocalTransaction.Where(x => x.TransactionDate.Year == yearParam && x.PayedToKiiPayBusinessInformationId == KiiPayBusinessInformationId).ToList();
                var FaxerMFBCCardTransactionYearwise = moneyFaxCreditSheetServices.FaxerMerchantPaymentSumYearWise(yearParam);

                var CardUserMerchantPaymentYearWise = moneyFaxCreditSheetServices.CardUserMerchantPaymentSumYearWise(yearParam);

                var MerchanttoMerchantPaymentYearWise = moneyFaxCreditSheetServices.MerchantPaymentyYearWiseByBusinessMerchant(yearParam);
                if ((MFBCCardDetails != null) && MFBCtransactionYearwise.Count() > 0)
                {

                    vm.TotalYearlyCreditedAmount = (MFBCtransactionYearwise.Sum(x => x.AmountSent) + FaxerMFBCCardTransactionYearwise + CardUserMerchantPaymentYearWise).ToString();


                }
                else
                {
                    vm.TotalYearlyCreditedAmount = (FaxerMFBCCardTransactionYearwise + CardUserMerchantPaymentYearWise + MerchanttoMerchantPaymentYearWise).ToString();

                }
                if (monthId != 0)
                {
                    var MFBCtransactionMontwise = dbContext.KiiPayBusinessLocalTransaction.Where(x => x.TransactionDate.Year == yearParam && x.TransactionDate.Month == monthId && x.PayedToKiiPayBusinessInformationId == KiiPayBusinessInformationId);
                    var FaxerMFBCCardTransactionMonthwise = moneyFaxCreditSheetServices.FaxerMerchantPaymentSumMonthWise(yearParam, monthId);
                    decimal FaxerMerchantPaymentSum = 0;
                    decimal FaxerMerchantPaymentCount = 0;
                    //get Sum and count of Card User to merchant payment 
                    foreach (var item in FaxerMFBCCardTransactionMonthwise)
                    {
                        if (item.Key == "Sum")
                        {
                            FaxerMerchantPaymentSum = item.Value;
                        }
                        else
                        {

                            FaxerMerchantPaymentCount = item.Value;
                        }
                    }


                    var CardUserMerchantPaymentMonthWise = moneyFaxCreditSheetServices.CardUserMerchantPaymentSumMonthWise(yearParam, monthId);

                    decimal CardUserMerchantPaymentSum = 0;
                    decimal CardUserMerchantPaymentCount = 0;
                    //get Sum and count of Card User to merchant payment 
                    foreach (var item in CardUserMerchantPaymentMonthWise)
                    {
                        if (item.Key == "Sum")
                        {
                            CardUserMerchantPaymentSum = item.Value;
                        }
                        else
                        {

                            CardUserMerchantPaymentCount = item.Value;
                        }
                    }

                    var MerchantToMerchantPaymentMonthWise = moneyFaxCreditSheetServices.MerchantPaymentyMonthWiseByBusinessMerchant(yearParam, monthId);
                    decimal MerchantToMerchantPaymentSum = 0;
                    decimal MerchantToMerchantPaymentCount = 0;

                    // get Sum And Count of Merchant To Merchant Payment 
                    foreach (var keyValuePair in MerchantToMerchantPaymentMonthWise)
                    {
                        if (keyValuePair.Key == "Sum")
                        {
                            MerchantToMerchantPaymentSum = keyValuePair.Value;
                        }
                        else
                        {

                            MerchantToMerchantPaymentCount = keyValuePair.Value;
                        }


                    }
                    if ((MFBCCardDetails != null) && MFBCtransactionMontwise.Count() > 0)
                    {

                        vm.TotalMonthlySalesCount = (MFBCtransactionMontwise.Count()
                                                    + FaxerMerchantPaymentCount
                                                    + CardUserMerchantPaymentCount
                                                    + MerchantToMerchantPaymentCount).ToString();
                        vm.TotalMonthlyCreditedAmount = (MFBCtransactionMontwise.Sum(x => x.AmountSent)
                                                     + FaxerMerchantPaymentSum
                                                     + CardUserMerchantPaymentSum
                                                     + MerchantToMerchantPaymentSum).ToString();
                    }
                    else
                    {
                        vm.TotalMonthlySalesCount = (FaxerMerchantPaymentCount
                                                     + CardUserMerchantPaymentCount
                                                     + MerchantToMerchantPaymentCount).ToString();
                        vm.TotalMonthlyCreditedAmount = (FaxerMerchantPaymentSum
                                                        + CardUserMerchantPaymentSum
                                                        + MerchantToMerchantPaymentSum).ToString();

                    }
                }
                vm.OutstandingCredit = moneyFaxCreditSheetServices.MFBCCardCurrentBalance().ToString();
            }
            return View(vm);
        }
    }
}