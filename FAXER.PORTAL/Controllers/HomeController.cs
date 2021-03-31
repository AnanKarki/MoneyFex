using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.KiiPayBusiness.Services;
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

namespace FAXER.PORTAL.Controllers
{
    public class HomeController : Controller
    {
        private DB.FAXEREntities dbContext = null;

        string defaultReceivingCountry = Common.Common.GetDefaultReceivingCountryCode();
        string defaultReceivingCurrency = Common.Common.GetDefaultReceivingCurrency();
        public HomeController()
        {
            dbContext = new DB.FAXEREntities();
        }

        public void GetCountriesViewBag()
        {

            ViewBag.SendingCountries = Common.Common.GetSendingCountries();
            ViewBag.ReceivingCountries = Common.Common.GetReceivingCountries();
        }
        [HttpGet]
        public ActionResult Index()
        {
            DemoLoginModel model = new DemoLoginModel();
            model.UserName = "Demo";
            model.Password = "Demo123@";
            Common.FaxerSession.DemoLoginModel = model;

            ViewBag.DefaultReceivingCountry = defaultReceivingCountry;
            ViewBag.DefaultReceivingCurrency = defaultReceivingCurrency;  //"USD";
            ViewBag.Countries = new SelectList(Common.Common.GetCountries().ToList(), "CountryCode", "CountryName");

            HomeViewModel vm = new HomeViewModel();

            var countries = (from c in Common.Common.GetCountries().ToList()
                             select new CountryViewModel()
                             {
                                 CountryCode = c.CountryCode,
                                 CountryName = c.CountryName,

                             }).ToList();

            vm.Countries = countries;

            FeedBackServices _services = new FeedBackServices();
            var data = _services.GetFeedBacks();

            vm.Feedbacks = data;

            vm.PartnersLogo = Common.Common.GetPartners().Select(x => x.LogoUrl).ToList();
            List<ExchangeRateVm> exchangeRateList = new List<ExchangeRateVm>();


            var exchangeRate1 = Common.Common.GetExchangeRate("GB", "NG");

            string[] ER1 = exchangeRate1.ToString().Split('.');
            ExchangeRateVm exchangeRateVm1 = new ExchangeRateVm()
            {
                ExchangeRate = ER1.Length < 2 ? exchangeRate1.ToString() : ER1[0] + "." + ER1[1].Substring(0, 3),
                SendingCountryCurrency = Common.Common.GetCountryCurrency("GB"),
                ReceivingCountryCurrency = Common.Common.GetCountryCurrency("NG"),
                SendingFlagCode = "GB".ToLower(),
                ReceivingFlagCode = "NG".ToLower(),

            };
            exchangeRateList.Add(exchangeRateVm1);
            var exchangeRate2 = Common.Common.GetExchangeRate("GB", "CM");

            string[] ER2 = exchangeRate2.ToString().Split('.');
            ExchangeRateVm exchangeRateVm2 = new ExchangeRateVm()
            {
                ExchangeRate = ER2.Length < 2 ? exchangeRate2.ToString() : ER2[0] + "." + ER2[1].Substring(0, 3),
                SendingCountryCurrency = Common.Common.GetCountryCurrency("GB"),
                ReceivingCountryCurrency = Common.Common.GetCountryCurrency("CM"),
                SendingFlagCode = "GB".ToLower(),
                ReceivingFlagCode = "CM".ToLower()
            };

            exchangeRateList.Add(exchangeRateVm2);
            var exchangeRate3 = Common.Common.GetExchangeRate("GB", "GH");

            string[] ER3 = exchangeRate3.ToString().Split('.');
            ExchangeRateVm exchangeRateVm3 = new ExchangeRateVm()
            {
                ExchangeRate = ER3.Length < 2 ? exchangeRate3.ToString() : ER3[0] + "." + ER3[1].Substring(0, 3),
                SendingCountryCurrency = Common.Common.GetCountryCurrency("GB"),
                ReceivingCountryCurrency = Common.Common.GetCountryCurrency("GH"),
                SendingFlagCode = "GB".ToLower(),
                ReceivingFlagCode = "GH".ToLower()
            };

            exchangeRateList.Add(exchangeRateVm3);
            vm.ExchangeRates = exchangeRateList;
            ViewBag.TrackingNo = Common.Common.GetTrackingNo("Home");
            GetCountriesViewBag();
            return View(vm);
        }

        public TransferService[] GetTransferService(string sendingCountry, string receivingCountry)
        {


            var result = Common.Common.GetTransferServices(sendingCountry, receivingCountry);
            //var services = 
            var enabledServices = result.Select(x => x.ServiceType).ToArray();

            return enabledServices;

        }

        [HttpGet]
        public JsonResult GetTransferServiceInJson(string sendingCountry, string receivingCountry)
        {


            var services = Common.Common.GetTransferServices(sendingCountry, receivingCountry);
            //var services = 
            if (services != null)
            {
                var enabledServices = services.Select(x => x.ServiceType).ToArray();
                var result = string.Join(",", enabledServices);

                return Json(new
                {
                    Data = result
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                Data = ""
            }, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public JsonResult GetUploadedServicesLogoInJson(string sendingCountry = "", string receivingCountry = "", int transferMethod = 0)
        {
            LogoAssignServices _services = new LogoAssignServices();
            LogosUploadServices _logosUploadServices = new LogosUploadServices();

            var masterData = _services.MasterData().Data.Where(x => x.SendingCountry == sendingCountry && x.ReceivingCountry == receivingCountry && x.Services == (TransactionTransferMethod)transferMethod).FirstOrDefault();

            if (masterData != null)
            {
                var detailsData = _services.DetailsData().Data.Where(x => x.LogoAssignId == masterData.Id).ToList();

                var result = (from c in detailsData
                              join d in _logosUploadServices.LogosUploadData().Data on c.ServiceProvider equals d.Id
                              select d.Logo).ToList();



                return Json(new
                {
                    LogoList = result,
                    HeaderText = masterData.Label
                }, JsonRequestBehavior.AllowGet);

            }

            return Json(new
            {
                LogoList = "",
                HeaderText = ""
            }, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GetPaymentSummary(decimal SendingAmount = 0, decimal ReceivingAmount = 0,
            string SendingCountry = "", string ReceivingCountry = "", bool IsReceivingAmount = false, int transferMethod = 0)
        {
            SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
            if (IsReceivingAmount)
            {

                SendingAmount = ReceivingAmount;
            }

            var feeInfo = SEstimateFee.GetTransferFee(SendingCountry, ReceivingCountry, (TransactionTransferMethod)transferMethod, SendingAmount);
            if (feeInfo == null)
            {

                return Json(new
                {
                    Fee = "",
                    TotalAmount = "",
                    ReceivingAmount = "",
                    SendingAmount = "",
                    ExchangeRate = "",
                    SendingCurrencySymbol = "",
                    ReceivingCurrencySymbol = "",
                    SendingCurrency = "",
                    ReceivingCurrency = "",
                    ExchangeRateText = "1 = 1"

                }, JsonRequestBehavior.AllowGet);
            }
            var result = new EstimateFaxingFeeSummary();


            result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
                SExchangeRate.GetExchangeRateValue(SendingCountry, ReceivingCountry, (TransactionTransferMethod)transferMethod), feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false);


            var introductoryRateResult = SEstimateFee.GetIntroductoryTransferSummary(SendingCountry, ReceivingCountry, result.FaxingAmount
                , feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false, false, (TransactionTransferMethod)transferMethod);

            if (introductoryRateResult != null)
            {

                result = introductoryRateResult;
            }

            CommonEnterAmountViewModel enterAmount = new CommonEnterAmountViewModel()
            {
                Fee = result.FaxingFee,
                SendingAmount = result.FaxingAmount,
                ReceivingAmount = IsReceivingAmount == true ? SendingAmount : result.ReceivingAmount,
                TotalAmount = result.TotalAmount,
                ExchangeRate = result.ExchangeRate,
                SendingCurrencySymbol = Common.Common.GetCurrencySymbol(SendingCountry),
                ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(ReceivingCountry),
                SendingCountryCode = SendingCountry,
                ReceivingCountryCode = ReceivingCountry,
                SendingCurrency = Common.Common.GetCountryCurrency(SendingCountry),
                ReceivingCurrency = Common.Common.GetCountryCurrency(ReceivingCountry),

            };
            // Rewrite session with additional value 

            _kiiPaytrasferServices.SetCommonEnterAmount(enterAmount);
            var validationResult = Common.Common.IsValidTransactionLimit(SendingCountry, ReceivingCountry,
                 result.ReceivingAmount, (TransactionTransferMethod)transferMethod);

            return Json(new
            {
                Fee = result.FaxingFee,
                TotalAmount = result.TotalAmount,
                ReceivingAmount = IsReceivingAmount == true ? SendingAmount : result.ReceivingAmount,
                SendingAmount = result.FaxingAmount,
                ExchangeRate = result.ExchangeRate,
                SendingCurrencySymbol = enterAmount.SendingCurrencySymbol,
                ReceivingCurrencySymbol = enterAmount.ReceivingCurrencySymbol,
                SendingCurrency = enterAmount.SendingCurrency,
                ReceivingCurrency = enterAmount.ReceivingCurrency,
                IsIntroductoryRate = result.IsIntroductoryRate,
                IsIntroductoryFee = result.IsIntroductoryFee,
                ActualFee = result.ActualFee,
                IsValid = validationResult,
                ExchangeRateText = "1 " + enterAmount.SendingCurrency + " = " + result.ExchangeRate + " " + enterAmount.ReceivingCurrency

            }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetTransactionEstimatedSummary(PaymentSummaryRequestParamVm vm)
        {

            return Json(new
            {
                Data = ""

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getFlagCode(string SendingCountry, string ReceivingCountry)
        {

            return Json(new
            {
                SendingCountryFlagCode = Common.Common.GetCountryFlagCode(SendingCountry),
                ReceivingCountryFlagCode = Common.Common.GetCountryFlagCode(ReceivingCountry),


            }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult SendMoneyNow(string transferMethod)
        {

            Common.FaxerSession.IsTransferFromHomePage = true;
            if (Common.FaxerSession.LoggedUser != null)
            {
                switch (transferMethod)
                {
                    case "BankAccount":
                        return RedirectToAction("Index", "senderBankAccountDeposit");
                        break;
                    case "MobileWallet":
                        return RedirectToAction("Index", "SenderMobileMoneyTransfer");
                        break;
                    case "CashPickup":
                        //window.location.href = "/SenderMobileMoneyTransfer/Index";
                        break;
                    default:
                        break;
                }
                return RedirectToAction("Index", "SenderTransferMoneyNow");
            }
            else
            {
                Common.FaxerSession.TransferMethod = transferMethod;
                Common.FaxerSession.FromUrl = "/Home/TransferMethod";
                return RedirectToAction("Login", "FaxerAccount");

            }

        }

        public ActionResult TransferMethod()
        {
            int senderId = Common.FaxerSession.LoggedUser.Id;
            var status = Common.Common.SenderStatus(senderId);
            if (status == DocumentApprovalStatus.Approved)
            {
                string transferMethod = Common.FaxerSession.TransferMethod;
                switch (transferMethod)
                {
                    case "BankAccount":
                        return RedirectToAction("Index", "senderBankAccountDeposit");
                        break;
                    case "MobileWallet":
                        return RedirectToAction("Index", "SenderMobileMoneyTransfer");
                        break;
                    case "CashPickup":
                        return RedirectToAction("Index", "SenderCashPickUp");
                        //window.location.href = "/SenderMobileMoneyTransfer/Index";
                        break;
                    default:
                        break;
                }

            }
            else
            {
                return RedirectToAction("Index", "SenderTransferMoneyNow", new { @IsFormHomePage = true });

            }
            return View();
        }

        public ActionResult Test(string uid, string id )
        {

            return Json(new
            {
                Data = uid

            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult BankAccountDeposit()
        {


            if (Common.FaxerSession.LoggedUser != null)
            {
                return RedirectToAction("Index", "SenderBankAccountDeposit");
            }
            else
            {
                Common.FaxerSession.FromUrl = "/SenderBankAccountDeposit/Index";
                return RedirectToAction("Login", "FaxerAccount");

            }

        }
        public ActionResult MobileMoneyTransfer()
        {


            if (Common.FaxerSession.LoggedUser != null)
            {
                return RedirectToAction("Index", "SenderMobileMoneyTransfer");
            }
            else
            {
                Common.FaxerSession.FromUrl = "/SenderMobileMoneyTransfer/Index";
                return RedirectToAction("Login", "FaxerAccount");

            }

        }

        [HttpGet]
        public ActionResult Test() {

            return Json(new
            {
               Data =  "Hello world"
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public JsonResult Countries()
        {


            var data = (from c in dbContext.Country
                        select new CountryVM()
                        {
                            Code = c.CountryCode,
                            Name = c.CountryName,
                        }).ToArray();

            return Json(new
            {

                Countries = data
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult allIsoCountries()
        {

            var result = (from c in dbContext.Country
                          select new allIsoCountriesVM()
                          {
                              n = "<div>" + c.Currency + "</div> <span> " + c.CountryName + "</span>",
                              i = c.CountryCode,
                          }).ToList();

            return Json(new
            {
                Data = result
            }, JsonRequestBehavior.AllowGet);
        }





    }
    public class allIsoCountriesVM
    {

        public string n { get; set; }
        public string i { get; set; }

    }

    public class CountryVM
    {

        public string Code { get; set; }
        public string Name { get; set; }

    }




}