using FAXER.PORTAL.Areas.Mobile.Models.MoneyFex;
using FAXER.PORTAL.Areas.Mobile.Models.MoneyFex.BankDeposit;
using FAXER.PORTAL.Areas.Mobile.Services.Common;
using FAXER.PORTAL.Areas.Mobile.Services.MoneyFex;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.SecureTradingPaymentGateway;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Mobile.Controllers.MoneyFex
{
    public class MobilePaymentBankDepositController : Controller
    {
        // GET: Mobile/MobilePaymentBankDeposit
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveIncompleteBankTransaction(MobilePaymentBankDepositVm transactionDetails)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                MobilePaymentBankDepositServices _services = new MobilePaymentBankDepositServices();
                var result = _services.SaveIncompleteBankTransaction(transactionDetails);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<MobilePaymentBankDepositVm>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateIncompleteBankTransaction(MobilePaymentBankDepositVm transactionDetails)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                MobilePaymentBankDepositServices _services = new MobilePaymentBankDepositServices();
                var result = _services.UpdateIncompleteBankTransaction(transactionDetails);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<MobilePaymentBankDepositVm>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveIncompleteMobileWalletTransaction(MobilePaymentBankDepositVm transactionDetails)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                MobilePaymentBankDepositServices _services = new MobilePaymentBankDepositServices();
                var result = _services.SaveIncompleteMobileWalletTransaction(transactionDetails);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<MobilePaymentBankDepositVm>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult SaveIncompleteCashPickUpTransaction(MobilePaymentBankDepositVm transactionDetails)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                MobilePaymentBankDepositServices _services = new MobilePaymentBankDepositServices();
                var result = _services.SaveIncompleteCashPickUpTransaction(transactionDetails);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<MobilePaymentBankDepositVm>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult SendTransactionPendingEmail(int transactionId, TransactionTransferMethod transactionTransferMethod)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                MobilePaymentBankDepositServices _services = new MobilePaymentBankDepositServices();
                var result = _services.SendTransactionPendingEmail(transactionId, transactionTransferMethod);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult CompleteTransaction(MobilePaymentBankDepositVm bankDeposit)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                MobilePaymentBankDepositServices services = new MobilePaymentBankDepositServices();
                var result = services.CompleteTransaction(bankDeposit);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<BankAccountDeposit>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult CompleteMobileTransaction(MobilePaymentBankDepositVm trans)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {

                MobilePaymentBankDepositServices services = new MobilePaymentBankDepositServices();
                var result = services.CompleteMobileTransaction(trans);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult CompleteCahPickUpTransaction(MobilePaymentBankDepositVm trans)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {

                MobilePaymentBankDepositServices services = new MobilePaymentBankDepositServices();
                var result = services.CompleteCashPickUpTransaction(trans);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetBanks(string CountryCode, string CurrencyCode)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {

                MobilePaymentBankDepositServices services = new MobilePaymentBankDepositServices();
                var result = services.GetBanks(CountryCode, CurrencyCode);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<List<FAXER.PORTAL.Areas.Mobile.Services.MoneyFex.BankVm>>()
                {


                }, JsonRequestBehavior.AllowGet);

            }
        }
        [HttpGet]
        public JsonResult GetBankBranches(int BankId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                MobilePaymentBankDepositServices services = new MobilePaymentBankDepositServices();
                var result = services.GetBankBranches(BankId);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<List<BankBranchVm>>()
                {
                    Data = null,
                    Status = ResultStatus.Warning

                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetWallets(string CountryCode)
        {

            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {

                MobilePaymentBankDepositServices services = new MobilePaymentBankDepositServices();
                var result = services.GetWallets(CountryCode);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<List<WalletDropDownVm>>()
                {
                    Data = null,
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpGet]
        public JsonResult GetReceiptNo(string SendingCountry, string ReceivingCountry = "")
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                MobilePaymentBankDepositServices services = new MobilePaymentBankDepositServices();
                var result = new ServiceResult<string>();
                result.Status = ResultStatus.OK;
                if (FAXER.PORTAL.Common.Common.IsManualDeposit(SendingCountry, ReceivingCountry))
                {
                    result.Data = FAXER.PORTAL.Common.Common.GenerateManualBankAccountDepositReceiptNo(6);
                }
                else
                {
                    result.Data = FAXER.PORTAL.Common.Common.GenerateBankAccountDepositReceiptNo(6);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<string>()
                {
                    Data = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetOtherWalletReceiptNo()
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                MobilePaymentBankDepositServices services = new MobilePaymentBankDepositServices();

                var result = services.GetOtherWalletReceiptNo();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<string>()
                {
                    Data = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetCashPikUpReceiptNo()
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                MobilePaymentBankDepositServices services = new MobilePaymentBankDepositServices();
                var result = services.GetCashPikUpReceiptNo();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<string>()
                {
                    Data = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpGet]
        public JsonResult ThreeDQueryResponseCallBack(string uid, string id)
        {
            try
            {
                CustomerResponseVm vm = new CustomerResponseVm();
                //Transact365Serivces.SenderId = SenderId;
                Transact365Serivces.module = Module.Faxer;
                var result = Transact365Serivces.GetTransationDetails(uid);

                return Json(new ServiceResult<bool>()
                {
                    Data = result.Data != null ? true : false,
                    Message = result.Message,
                    Status = result.Status
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

            }
            return Json(new ServiceResult<bool>()
            {
                Data = false,
                Message = "",
                Status = ResultStatus.Warning
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ThreeDQueryResponseCallBack(CustomerResponseVm responseVm)
        {
            try
            {
                var threeDdata = FAXER.PORTAL.Services.StripServices.GetThreeDRequestLog().Where(x => x.MD == responseVm.MD).FirstOrDefault();
                threeDdata.PaRes = responseVm.PaRes;
                threeDdata.parenttransactionreference = responseVm.parenttransactionreference;
                FAXER.PORTAL.Services.StripServices.AddOrUpdateThreeDLog(threeDdata);
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("Success", new { @PaRes = responseVm.PaRes });
        }

        [HttpPost]
        public JsonResult GetPaRes(ThreeDRequestVm threeDRequest)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var threeDdata = FAXER.PORTAL.Services.StripServices.GetThreeDRequestLog().Where(x => x.MD == threeDRequest.MD).FirstOrDefault();
                return Json(new ServiceResult<string>()
                {
                    Data = threeDdata.PaRes,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<string>()
                {
                    Data = "",
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpGet]
        public JsonResult GetMoneyfexBankInfo(string CountryCode)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                MobilePaymentBankDepositServices services = new MobilePaymentBankDepositServices();

                var result = services.GetMoneyFexBankInfo(CountryCode);
                return Json(new ServiceResult<SenderMoneyFexBankDepositVM>()
                {
                    Data = result.Data,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<SenderMoneyFexBankDepositVM>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);

            }
        }
        [HttpGet]
        public JsonResult IsMoneyFexBankInfoExist(string CountryCode)
        {
            try
            {
                string token = FAXER.PORTAL.Common.Common.RequestToken;
                if (FAXER.PORTAL.Common.Common.ValidateToken(token))
                {
                    MobilePaymentBankDepositServices services = new MobilePaymentBankDepositServices();

                    var result = services.IsMoneyFexBankInfoExist(CountryCode);
                    return Json(new ServiceResult<bool>()
                    {
                        Data = result.Data,
                        Message = "",
                        Status = ResultStatus.OK
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new ServiceResult<bool>()
                    {
                        Data = false,
                        Message = "",
                        Status = ResultStatus.Warning
                    }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                return Json(new ServiceResult<bool>()
                {
                    Data = false,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);

            }

        }
        [HttpGet]
        public ActionResult Success(string PaRes = "")
        {
            return View();
        }

        [HttpPost]

        public string GetThreeDView(ThreeDRequestVm vm)
        {
            ViewBag.acsurl = vm.acsurl;
            ViewBag.pareq = vm.PaReq;
            ViewBag.termurl = vm.termurl;
            ViewBag.md = vm.MD;

            string html = "<!DOCTYPE html><html><head>" +
                "<meta name='viewport' content='width=device-width' /><title>GetThreeDView</title>" +
                "<meta content='text/html;charset=utf-8' http-equiv='Content-Type'>" +
                 "<meta content='utf-8' http-equiv='encoding'>" +
                "<script src='https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js'></script>"
                + "<style>" +
                "img {" +
                "position: absolute;" +
                "top: 50 %;" +
                "left: 50;" +
                "transform: translate(-50 %, -50 %);" +
                "-ms - transform: translate(-50 %, -50 %); /* IE 9 */" +
                "-webkit - transform: translate(-50 %, -50 %); /* Chrome, Safari, Opera */}" +
                "</style>" +
                "</head>" +
                "<body>" + " <img src='/Content/Gif/loading.gif'   />"
                + "<div><form name=" + "form" + " id= " + "acsurl" + " method=" + "post" + " action='" + vm.acsurl + "'>" +
            "<div>"

                      + "<input type='hidden' name='PaReq' id='pareq'" + "value=" + vm.PaReq + " />"
                 + "<input type='hidden' name='TermUrl' id='termurl'" + "value=" + vm.termurl + " />"
                 + "<input type='hidden' name='MD' id='md'" + "value=" + vm.MD + " />"
                 + "<input type='submit' value='submit' id='btn' style='display:none' >"

             + "</div></form>" +
             "</div> " +
             "<script type='text/javascript'> " +
             "function GO(){ " +
             "$('#btn').click();" +
             "  }" +
             "GO();" +
             "</script></body>" +
             "</html>";
            return html;

        }


        public string GetT365ThreeDView(ThreeDRequestVm vm)
        {

            ViewBag.acsurl = vm.acsurl;
            ViewBag.pareq = vm.PaReq;
            ViewBag.termurl = vm.termurl;
            ViewBag.md = vm.MD;

            string html = "<!DOCTYPE html><html><head>" +
                "<meta name='viewport' content='width=device-width' /><title>GetThreeDView</title>" +
                "<meta content='text/html;charset=utf-8' http-equiv='Content-Type'>" +
                 "<meta content='utf-8' http-equiv='encoding'>" +
                "<script src='https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js'></script>"
                + "<style>" +
                "img {" +
                "position: absolute;" +
                "top: 50 %;" +
                "left: 50;" +
                "transform: translate(-50 %, -50 %);" +
                "-ms - transform: translate(-50 %, -50 %); /* IE 9 */" +
                "-webkit - transform: translate(-50 %, -50 %); /* Chrome, Safari, Opera */}" +
                "</style>" +
                "</head>" +
                "<body>" + " <img src='/Content/Gif/loading.gif'   />"
                + "<div><form name=" + "form" + " id= " + "acsurl" + " method=" + "get" + " action='" + vm.redirect_url + "'>" +
            "<div>"

                      + "<input type='hidden' name='PaReq' id='pareq'" + "value=" + vm.PaReq + " />"
                 + "<input type='hidden' name='TermUrl' id='termurl'" + "value=" + vm.termurl + " />"
                 + "<input type='hidden' name='MD' id='md'" + "value=" + vm.MD + " />"
                 + "<input type='submit' value='submit' id='btn' style='display:none' >"

             + "</div></form>" +
             "</div> " +
             "<script type='text/javascript'> " +
             "function GO(){ " +
             "$('#btn').click();" +
             "  }" +
             "GO();" +
             "</script></body>" +
             "</html>";
            return html;

        }

        [HttpPost]
        public JsonResult DebitCreditCardPayment(MobilePaymentCreditDebitCardDetailsVm vm)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {

                var serviceResult = new ServiceResult<ThreeDRequestVm>();
                if (string.IsNullOrEmpty(vm.CardNumber))
                {

                    ModelState.AddModelError("", "Enter Card Number");
                    serviceResult.Data = null;
                    serviceResult.Message = "Enter Card Number";
                    serviceResult.Status = ResultStatus.Error;
                    return Json(serviceResult);
                }
                var number = vm.CardNumber.Split(' ');
                vm.CardNumber = string.Join("", number);

                var senderInfo = FAXER.PORTAL.Common.Common.GetSenderInfo(vm.SenderId);

                var ValidCard = StripServices.IsValidCardNo(new StripeResultIsValidCardVm()
                {
                    //Amount = vm.TotalAmount,
                    CurrencyCode = FAXER.PORTAL.Common.Common.GetCountryCurrency(senderInfo.Country),
                    CardName = "Charge for " + senderInfo.FirstName,
                    ExpiringYear = vm.ExpiryDate.Split('/')[1],
                    Number = vm.CardNumber,
                    ExpirationMonth = vm.ExpiryDate.Split('/')[0],
                    SecurityCode = vm.SecurityCode,
                    billingpostcode = senderInfo.PostalCode,
                    billingpremise = senderInfo.Address1,
                    SenderId = vm.SenderId

                });
                if (ValidCard.IsValid == false)
                {
                    serviceResult.Data = null;
                    serviceResult.Message = ValidCard.Message;
                    serviceResult.Status = ResultStatus.Error;
                    serviceResult.IsLimitMsg = ValidCard.IsLimitMsg;
                    return Json(serviceResult);
                }

                StripeCreateTransactionVM stripeCreateTransaction = new StripeCreateTransactionVM()
                {
                    Amount = vm.TotalAmount,
                    Currency = FAXER.PORTAL.Common.Common.GetCountryCurrency(senderInfo.Country),
                    NameOnCard = "Charge for " + senderInfo.FirstName,
                    StripeTokenId = "",
                    CardNum = vm.CardNumber,
                    ExipiryDate = vm.ExpiryDate,
                    SecurityCode = vm.SecurityCode,
                    termurl = "/Mobile/MobilePaymentBankDeposit/ThreeDQueryResponseCallBack",
                    billingpostcode = senderInfo.PostalCode,
                    billingpremise = senderInfo.Address1,
                    ReceiptNo = vm.OrderNo,
                    SenderId = vm.SenderId,
                    SenderEmail = senderInfo.Email,
                    SenderFirstName = senderInfo.FirstName,
                    SenderLastName = !string.IsNullOrEmpty(senderInfo.MiddleName) == true ? senderInfo.MiddleName + " " + senderInfo.LastName : senderInfo.LastName,
                    SendingCountry = senderInfo.Country,
                    ReceivingCountry =vm.ReceivingCountry,
                    ReceivingCurrency = vm.ReceivingCurrency

                };
                Transact365Serivces.SenderId = vm.SenderId;
                var resultThreedQuery = StripServices.CreateThreedQuery(stripeCreateTransaction,
                                       TransactionTransferType.Online, vm.TransferMethod);

                if (resultThreedQuery.Status == ResultStatus.OK)
                {
                    switch (resultThreedQuery.Data.CardProcessorApi)
                    {
                        case CardProcessorApi.Select:
                            break;
                        case CardProcessorApi.TrustPayment:
                            resultThreedQuery.Data.html = GetThreeDView(resultThreedQuery.Data);
                            resultThreedQuery.Data.url = "https://webapp.securetrading.net";
                            break;
                        case CardProcessorApi.T365:
                            resultThreedQuery.Data.html = GetT365ThreeDView(resultThreedQuery.Data);
                            resultThreedQuery.Data.url = resultThreedQuery.Data.redirect_url;
                            break;
                        default:
                            break;
                    }
                  
                }
                serviceResult.Message = resultThreedQuery.Message;
                serviceResult.Status = resultThreedQuery.Status;
                serviceResult.Data = resultThreedQuery.Data;
                serviceResult.IsGetType3dAuth = resultThreedQuery.IsGetType3dAuth;
                if (serviceResult.Status == ResultStatus.OK)
                {
                    if (serviceResult.Data.ThreeDEnrolled != "Y")
                    {

                        serviceResult.Data = null;
                        serviceResult.Message = "Cannot Proceed Transaction";
                        serviceResult.Status = ResultStatus.Error;

                    }

                }
                else
                {
                    serviceResult.Data = null;
                    serviceResult.Message = resultThreedQuery.Message;
                    serviceResult.Status = ResultStatus.Error;
                    return Json(serviceResult);
                }

                //return Json(new ServiceResult<ThreeDRequestVm>()
                //{
                //    Data = 

                //});
                return Json(serviceResult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<ThreeDRequestVm>()
                {

                    Data = null,
                    Status = ResultStatus.Warning


                }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult CreateTransaction(StripeCreateTransactionVM vm)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var senderInfo = FAXER.PORTAL.Common.Common.GetSenderInfo(vm.SenderId);
                var result = new ServiceResult<bool>();
                var transactionResult = new StripeResult();
                vm.billingpostcode = senderInfo.PostalCode;
                vm.billingpremise = senderInfo.Address1;
                vm.Currency = FAXER.PORTAL.Common.Common.GetCurrencyCode(vm.SendingCountry);

                decimal ExtraFee = 0;
                try
                {
                    ExtraFee = StripServices.GetExtraFeeAmountIfCreditCard(new StripeResultIsValidCardVm()
                    {
                        Number = vm.CardNum,
                        ExpirationMonth = vm.ExipiryDate.Split('/')[0],
                        ExpiringYear = vm.ExipiryDate.Split('/')[1],
                        CurrencyCode = vm.Currency,
                        SecurityCode = vm.SecurityCode
                    }, vm.Amount);

                }
                catch (Exception)
                {
                }
                vm.Amount = vm.Amount + ExtraFee;

                if (!CheckIfHasTransactionReceivedToPaymentGateway(vm.ReceiptNo))
                {
                    var paymentGatewayResult = IntiateTransactionToPaymentGateWay(vm);

                    return Json(new ServiceResult<bool>()
                    {
                        Data = paymentGatewayResult.Data,
                        Message = paymentGatewayResult.Message,
                        Status = paymentGatewayResult.Status
                    }, JsonRequestBehavior.AllowGet);


                }
                else
                {

                    result.Data = true;
                    result.Message = "Transfer completed successfully!";
                    result.Status = ResultStatus.OK;
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<bool>()
                {
                    Data = false,
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetT365PaymentStatusByUid(string uid)
        {
            var status = Transact365Serivces.GetTransationDetails(uid);
            return Json(new ServiceResult<ThreeDRequestVm>()
            {
                Data = status.Data,
                Message = status.Message,
                Status = status.Status,
            }, JsonRequestBehavior.AllowGet);
        }

        private ServiceResult<bool> IntiateTransactionToPaymentGateWay(StripeCreateTransactionVM vm)
        {

            var result = new ServiceResult<bool>();
            var transactionResult = StripServices.CreateTransaction(vm);
            if (transactionResult.IsValid == false)
            {
                SCreditCardAttempt creditCardAttempt = new SCreditCardAttempt();
                var cardAttemptSms = creditCardAttempt.IsValidCardForMobileAttempt(vm.SenderId, vm.CardNum).Message;
                result.Data = false;
                result.Message = cardAttemptSms;
                result.Status = ResultStatus.Error;
            }
            else
            {
                result.Data = true;
                result.Message = "Transfer completed successfully!";
                result.Status = ResultStatus.OK;
            }

            return result;
        }
        private bool CheckIfHasTransactionReceivedToPaymentGateway(string ReceiptNo)
        {
            MobilePaymentBankDepositServices bankDepositServices = new MobilePaymentBankDepositServices();
            var transactionHasBeenReceived = bankDepositServices.HasTransactionReceivedToPaymentGateway(ReceiptNo);
            return transactionHasBeenReceived.Data;

        }

        [HttpGet]
        public JsonResult CancelBankTransaction(int transactionId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = new ServiceResult<bool>();
                MobilePaymentBankDepositServices services = new MobilePaymentBankDepositServices();
                result = services.CancelBankTransaction(transactionId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<bool>()
                {

                    Data = false,
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult CancelMobileWalletTransaction(int transactionId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = new ServiceResult<bool>();
                MobilePaymentBankDepositServices services = new MobilePaymentBankDepositServices();
                result = services.CancelMobileWalletTransaction(transactionId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<bool>()
                {

                    Data = false,
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult CancelCashPickUpTransaction(int transactionId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = new ServiceResult<bool>();
                MobilePaymentBankDepositServices services = new MobilePaymentBankDepositServices();
                result = services.CancelCashPickUpTransaction(transactionId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<bool>()
                {

                    Data = false,
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult IsSenderVerfied(int SenderId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var data = FAXER.PORTAL.Common.Common.GetSenderDocumentStatus(SenderId);

                SenderDocumentationStatusVm statusVm = new SenderDocumentationStatusVm()
                {

                    DocumentStatus = data
                };
                var result = new ServiceResult<SenderDocumentationStatusVm>()
                {

                    Data = statusVm,
                    Message = "",
                    Status = ResultStatus.OK
                };


                return Json(
                result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
             ServiceResult<SenderDocumentationStatusVm>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetIdentificationTypes()
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {

                MobileCommonServices mobileCommonServices = new MobileCommonServices();
                var data = mobileCommonServices.GetIdentificationTypes();
                var result = new ServiceResult<List<FAXER.PORTAL.Areas.Mobile.Services.Common.IdentificationTypeDropDownVm>>()
                {
                    Data = data,
                    Message = "",
                    Status = ResultStatus.OK
                };

                return Json(new
                {
                    result
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json(new ServiceResult<List<FAXER.PORTAL.Areas.Mobile.Services.Common.IdentificationTypeDropDownVm>>()
                {
                    Data = null,
                    Status = ResultStatus.Warning

                }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpGet]
        public JsonResult GetCountries()
        {

            var data = FAXER.PORTAL.Common.Common.GetCountriesForDropDown();
            var result = new ServiceResult<List<CountryViewModel>>()
            {
                Data = data,
                Message = "",
                Status = ResultStatus.OK
            };

            return Json(new
            {
                result
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetSavedDebitCreditCard(int senderId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                MobileCommonServices mobileCommonServices = new MobileCommonServices();

                var data = mobileCommonServices.GetSavedDebitCreditCard(senderId);

                var result = new ServiceResult<MobilePaymentCreditDebitCardDetailsVm>()
                {
                    Data = data,
                    Message = "",
                    Status = ResultStatus.OK
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult HasRecipientTransacionExceedLimit(int senderId, int receipientId, string sendingCountry, string receivingCountry, TransactionTransferMethod transferMethod)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {

                MobilePaymentBankDepositServices services = new MobilePaymentBankDepositServices();
                var result = services.HasRecipientTransacionExceedLimit(senderId, receipientId, sendingCountry, receivingCountry, transferMethod);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<bool>()
                {
                    Data = false,
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult HasSenderTransacionExceedLimit(int senderId, string sendingCountry, string receivingCountry, TransactionTransferMethod transferMethod)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                MobilePaymentBankDepositServices services = new MobilePaymentBankDepositServices();
                var result = services.HasSenderTransacionExceedLimit(senderId, sendingCountry, receivingCountry, transferMethod);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<bool>()
                {
                    Data = false,
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetCreditDebitFee(string CountryCode)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                MobileCommonServices mobileCommonServices = new MobileCommonServices();

                var data = mobileCommonServices.GetCreditDebitFee(CountryCode);

                var result = new ServiceResult<Admin.ViewModels.CustomerPaymentFeeViewModel>()
                {
                    Data = data,
                    Message = "",
                    Status = ResultStatus.OK
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<Admin.ViewModels.CustomerPaymentFeeViewModel>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
        }


    }

    public class MobilePaymentCreditDebitCardDetailsVm
    {

        public int SenderId { get; set; }

        public string CardNumber { get; set; }
        public string DecryptedCardNumber { get; set; }

        public string CardHolderName { get; set; }
        public string EndMonth { get; set; }
        public string EndDate { get; set; }
        public string ExpiryDate { get; set; }
        public string SecurityCode { get; set; }
        public string Address { get; set; }
        public bool SaveCard { get; set; }
        public string OrderNo { get; set; }

        public decimal TotalAmount { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public string ReceiverEmail { get; set; }
        public string ReceivingCountry { get; set; }
        public string ReceivingCurrency { get; set; }
    }
    public class SenderDocumentationStatusVm
    {

        public DocumentApprovalStatus DocumentStatus { get; set; }

    }
}