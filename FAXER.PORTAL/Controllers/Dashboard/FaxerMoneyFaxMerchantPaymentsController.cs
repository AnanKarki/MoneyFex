using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.Dashboard
{
    public class FaxerMoneyFaxMerchantPaymentsController : Controller
    {
        DB.FAXEREntities context = new DB.FAXEREntities();
        int FaxerId = Common.FaxerSession.LoggedUser == null ? 0 :  Common.FaxerSession.LoggedUser.Id;
        // GET: FaxerMoneyFaxMerchantPayments
        public ActionResult Index(string Fromdate = "", string ToDate = "")
        {
            List<Models.FaxerMoneyFaxMerchantPaymentsViewModel> data = new List<Models.FaxerMoneyFaxMerchantPaymentsViewModel>();
            if (!string.IsNullOrEmpty(Fromdate) && !string.IsNullOrEmpty(ToDate))
            {

                var fromDate = Convert.ToDateTime(Fromdate);
                var toDate = Convert.ToDateTime(ToDate);

                data = (from c in context.FaxerMerchantPaymentTransaction.Where(x => DbFunctions.TruncateTime(x.PaymentDate) >= fromDate.Date
                          && DbFunctions.TruncateTime(x.PaymentDate) <= toDate.Date).OrderByDescending(x => x.PaymentDate).ToList()
                        join d in context.FaxerMerchantPaymentInformation.Where(x => x.SenderInformationId == FaxerId) on c.SenderKiiPayBusinessPaymentInformationId equals d.Id
                        join e in context.Country on d.KiiPayBusinessInformation.BusinessOperationCountryCode equals e.CountryCode
                        select new Models.FaxerMoneyFaxMerchantPaymentsViewModel()
                        {
                            MerchantAccountNumber = d.KiiPayBusinessInformation.BusinessMobileNo,
                            MerchantCity = d.KiiPayBusinessInformation.BusinessOperationAddress1,
                            MerchantCountry = e.CountryName,
                            MerchantName = d.KiiPayBusinessInformation.BusinessName,
                            PaymentAmount = c.PaymentAmount,
                            PaymentRefrence = c.PaymentReference,
                            CurrencySymbol = Common.Common.GetCurrencySymbol(c.SenderKiiPayBusinessPaymentInformation.SenderInformation.Country),
                            Date = c.PaymentDate.ToString("dd/MM/yyyy"),
                            Time = c.PaymentDate.ToString("hh:mm"),
                        }).ToList();


            }
            else
            {


                data = (from c in context.FaxerMerchantPaymentTransaction.OrderByDescending(x => x.PaymentDate).ToList()
                        join d in context.FaxerMerchantPaymentInformation.Where(x => x.SenderInformationId == FaxerId) on c.SenderKiiPayBusinessPaymentInformationId equals d.Id
                        join e in context.Country on d.KiiPayBusinessInformation.BusinessOperationCountryCode equals e.CountryCode
                        select new Models.FaxerMoneyFaxMerchantPaymentsViewModel()
                        {
                            MerchantAccountNumber = d.KiiPayBusinessInformation.BusinessMobileNo,
                            MerchantCity = d.KiiPayBusinessInformation.BusinessOperationAddress1,
                            MerchantCountry = e.CountryName,
                            MerchantName = d.KiiPayBusinessInformation.BusinessName,
                            PaymentAmount = c.PaymentAmount,
                            PaymentRefrence = c.PaymentReference,
                            CurrencySymbol = Common.Common.GetCurrencySymbol(c.SenderKiiPayBusinessPaymentInformation.SenderInformation.Country),
                            Date = c.PaymentDate.ToString("dd/MM/yyyy"),
                            Time = c.PaymentDate.ToString("hh:mm"),
                        }).ToList();
            }

            Common.FaxerSession.PayGoodsAndServicesBackURL = "/FaxerMoneyFaxMerchantPayments/Index";
            return View(data);
        }
        public ActionResult MerchantDetailsAfterLogin()
        {
            return View();
        }
        public ActionResult MerchantCountryAfterLogin()
        {
            return View();
        }
        public ActionResult MerchantAccountNumberAfterLogin()
        {
            return View();
        }
        public ActionResult MerchantDetails()
        {
            return View();
        }
        public ActionResult MerchantAmount()
        {
            return View();
        }
        public ActionResult MerchantPayingDetails()
        {
            return View();
        }
        public ActionResult MerchantLoginMessage()
        {
            return View();
        }
        public ActionResult MerchantLogin()
        {
            return View();
        }
        public ActionResult Signup()
        {
            return View();
        }
    }
}