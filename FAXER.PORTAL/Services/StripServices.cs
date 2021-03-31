using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.SecureTradingPaymentGateway;
using Stripe;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Text;
using System.Web;

//using System.Web.Http.ModelBinding;

namespace FAXER.PORTAL.Services
{
    public class StripServices
    {

        #region  Strip portion


        #region 3D Secure payment
        public static ServiceResult<ThreeDRequestVm> CreateThreedQuery(StripeCreateTransactionVM model)
        {

            StripeResult result = new StripeResult();
            result.IsValid = true;
            //StripeConfiguration.SetApiKey("sk_test_OJiTq50I3SNoO3tL3bOJBhTy");

            //var stripeTokenCreateOptions = new StripeTokenCreateOptions
            //{
            //    Card = new StripeCreditCardOptions
            //    {
            //        Number = model.Number,
            //        ExpirationMonth = int.Parse(model.ExpirationMonth),
            //        ExpirationYear = int.Parse(model.ExpiringYear),
            //        Cvc = model.SecurityCode,
            //        Name =model.CardName
            //    }
            //};


            string[] vs = new string[1];
            vs[0] = "THREEDQUERY";

            if (model.ExipiryDate.Split('/')[0].Length == 1)
            {

                model.ExipiryDate = "0" + model.ExipiryDate.Split('/')[0] + "/" + model.ExipiryDate.Split('/')[1];
            }

            SecureTradingApiRequestVm secureTradingApiRequestVm = new SecureTradingApiRequestVm()
            {
                requesttypedescriptions = vs,
                baseamount = ((Int32)(model.Amount * 100)).ToString(),
                currencyiso3a = model.Currency,
                expirydate = model.ExipiryDate,
                orderreference = model.ReceiptNo,
                pan = model.CardNum,
                securitycode = model.SecurityCode,
                termurl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + model.termurl,
                billingpostcode = model.billingpostcode,
                billingpremise = model.billingpremise


            };

            //SecureTradingThreeDRequestVm secureTradingApiRequestVm = new SecureTradingThreeDRequestVm()
            //{
            //    requesttypedescriptions = vs,
            //    baseamount = ((Int32)(model.Amount * 100)).ToString(),
            //    currencyiso3a = model.Currency,
            //    expirydate = model.ExipiryDate,
            //    pan = model.CardNum,
            //    securitycode = model.SecurityCode,
            //    termurl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + model.termurl
            //};
            SecureTradingApiRequestParam secureTradingApiRequestParam = new SecureTradingApiRequestParam()
            {
                request = secureTradingApiRequestVm
            };
            WebServices webServices = new WebServices();
            var SercureTradingResult = webServices.PostTransaction<SecureTradingTHREEDQUERYReponseVm>(webServices.SerializeObject<SecureTradingApiRequestParam>(secureTradingApiRequestParam));

            ThreeDRequestVm threeDRequestVm = new ThreeDRequestVm()
            {
                MD = SercureTradingResult.Result.response.FirstOrDefault().md,
                PaReq = SercureTradingResult.Result.response.FirstOrDefault().pareq,
                termurl = secureTradingApiRequestVm.termurl,
                acsurl = SercureTradingResult.Result.response.FirstOrDefault().acsurl,
                ThreeDEnrolled = SercureTradingResult.Result.response.FirstOrDefault().enrolled

            };

            Log.Write("THREEDQUERY" + " of " + secureTradingApiRequestVm.orderreference);
            //RedirectPage(secureTradingApiRequestVm.termurl , threeDRequestVm);
            AddOrUpdateThreeDLog(new ThreeDRequestLog()
            {
                MD = SercureTradingResult.Result.response.FirstOrDefault().md,
                PaReq = SercureTradingResult.Result.response.FirstOrDefault().pareq,
                termurl = secureTradingApiRequestVm.termurl,
                acsurl = SercureTradingResult.Result.response.FirstOrDefault().acsurl,
                ThreeDEnrolled = SercureTradingResult.Result.response.FirstOrDefault().enrolled,
                CreatedDate = DateTime.Now,
                SenderId = model.SenderId
            });

            ResultStatus status = ResultStatus.OK;
            string message = "";
            if (threeDRequestVm.ThreeDEnrolled == "U")
            {

                message = "Invalid Card";
                status = ResultStatus.Error;
            }
            threeDRequestVm.CardProcessorApi = CardProcessorApi.TrustPayment;
            return new ServiceResult<ThreeDRequestVm>()
            {

                Data = threeDRequestVm,
                Message = message,
                Status = status
            };

        }
        public static ServiceResult<ThreeDRequestVm> CreateThreedQuery(StripeCreateTransactionVM model, TransactionTransferType transferType, TransactionTransferMethod transferMethod)
        {
            var cardProcessor = GetCardProcessor(model, transferType, transferMethod);
            ServiceResult<ThreeDRequestVm> result = new ServiceResult<ThreeDRequestVm>();
            switch (cardProcessor)
            {
                case CardProcessorApi.TrustPayment:

                    result = CreateThreedQuery(model);
                    break;
                case CardProcessorApi.T365:

                    Transact365Serivces.SenderId = model.SenderId;
                    Module module = Module.Faxer;
                    if (transferType == TransactionTransferType.Agent || transferType == TransactionTransferType.AuxAgent)
                    {
                        module = Module.Agent;
                    }
                    if (transferType == TransactionTransferType.Admin)
                    {
                        module = Module.Staff;
                    }
                    Transact365Serivces.module = module;
                    result = Transact365Serivces.CreatePayment(model);
                    break;
            }
            return result;
        }

        public static CardProcessorApi GetCardProcessor(StripeCreateTransactionVM model, TransactionTransferType transferType, TransactionTransferMethod transferMethod)
        {
            FAXEREntities dbContext = new DB.FAXEREntities();
            int paymentGateway = 1;
            bool parsePaymentGateway = int.TryParse(Common.Common.GetAppSettingValue("DefaultPaymentGateway"), out paymentGateway);


            CardProcessorApi cardProcessorApi = (CardProcessorApi)paymentGateway;


            int cardProcessorApiId = dbContext.CardProcessorSelection.Where(x => x.SendingCurrency == model.Currency).
                                                                         Select(x => x.CardProcessorId).FirstOrDefault();
            if (cardProcessorApiId != 0)
            {
                cardProcessorApi = dbContext.CardProcessor.Where(x => x.Id == cardProcessorApiId).Select(x => x.CardProcessorApi).FirstOrDefault();
            }
            return cardProcessorApi;

            var cardprocesser = dbContext.CardProcessorSelection;
            var cardProcessorSelections = cardprocesser.Where(x => x.SendingCurrency == model.Currency &&
                                                                                      x.ReceivingCurrency == model.ReceivingCurrency);
            if (cardProcessorSelections.Count() == 0)
            {
                cardProcessorSelections = cardprocesser.Where(x => x.SendingCurrency == model.Currency &&
                                                                                      x.ReceivingCurrency.ToLower() == "all");
            }
            var cardProcessorSelectionsAccToCountries = cardProcessorSelections.Where(x => x.SendingCountry == model.SendingCountry &&
                                                                                          x.ReceivingCountry == model.ReceivingCountry);
            if (cardProcessorSelectionsAccToCountries.Count() == 0)
            {
                cardProcessorSelectionsAccToCountries = cardProcessorSelections.Where(x => x.SendingCountry == model.SendingCountry &&
                                                                                          x.ReceivingCountry.ToLower() == "all");
                if (cardProcessorSelectionsAccToCountries.Count() == 0)
                {
                    cardProcessorSelectionsAccToCountries = cardProcessorSelections.Where(x => x.SendingCountry.ToLower() == "all" &&
                                                                                             x.ReceivingCountry == model.ReceivingCountry);
                    if (cardProcessorSelectionsAccToCountries.Count() == 0)
                    {
                        cardProcessorSelectionsAccToCountries = cardProcessorSelections.Where(x => x.SendingCountry.ToLower() == "all" &&
                                                                                                  x.ReceivingCountry.ToLower() == "all");
                    }
                }
            }
            var cardProcessorWithRange = cardProcessorSelectionsAccToCountries.Where(x => x.FromRange <= model.Amount && x.ToRange >= model.Amount);
            if (cardProcessorWithRange.Count() == 0)
            {
                cardProcessorWithRange = cardProcessorSelectionsAccToCountries.Where(x => x.FromRange == 0 && x.ToRange == 0);
            }
            var cardProcessorwithTransferType = cardProcessorWithRange.Where(x => x.TransferType == transferType);
            if (cardProcessorwithTransferType.Count() == 0)
            {
                cardProcessorwithTransferType = cardProcessorWithRange.Where(x => x.TransferType == TransactionTransferType.All);
            }

            var cardProcessorwithTransferMethod = cardProcessorwithTransferType.Where(x => x.TransferMethod == transferMethod);
            if (cardProcessorwithTransferMethod.Count() == 0)
            {
                cardProcessorwithTransferMethod = cardProcessorwithTransferType.Where(x => x.TransferMethod == TransactionTransferMethod.All);
            }
            var cardProcessor = cardProcessorwithTransferMethod.FirstOrDefault();
            if (cardProcessor != null)
            {
                cardProcessorApi = dbContext.CardProcessor.Where(x => x.Id == cardProcessor.Id).Select(x => x.CardProcessorApi).FirstOrDefault();
            }
            return cardProcessorApi;
            //var cardProcessorId = dbContext.CardProcessorSelection.Where(x => x.SendingCurrency == model.Currency && x.ReceivingCurrency == model.ReceivingCurrency &&
            //                                                                   (x.SendingCountry == model.SendingCountry || x.SendingCountry.ToLower() == "all")
            //                                                                   &&
            //                                                                   (x.ReceivingCountry == model.ReceivingCountry || x.ReceivingCountry.ToLower() == "all")
            //                                                                   &&
            //                                                                   (x.FromRange <= model.Amount && x.ToRange >= model.Amount)
            //                                                                   && x.TransferType == transferType &&
            //                                                                   (x.TransferMethod == transferMethod
            //                                                                   || x.TransferMethod == TransactionTransferMethod.All)).
            //                                                                   Select(x => x.CardProcessorId).FirstOrDefault();

            //cardProcessorId = dbContext.CardProcessorSelection.Where(x => x.SendingCurrency == model.Currency).
            //                                                             Select(x => x.CardProcessorId).FirstOrDefault();
            //if (cardProcessorId != 0)
            //{
            //    cardProcessorApi = dbContext.CardProcessor.Where(x => x.Id == cardProcessorId).Select(x => x.CardProcessorApi).FirstOrDefault();
            //}
            //return cardProcessorApi;
        }

        public static ServiceResult<ThreeDRequestVm> CheckIfThreeHasAlreadyCreated(StripeCreateTransactionVM model)
        {
            return new ServiceResult<ThreeDRequestVm>();
        }
        #endregion

        public static StripeResult IsValidCardNo(StripeResultIsValidCardVm model)
        {
            StripeResult result = new StripeResult();
            result.IsValid = true;
            //StripeConfiguration.SetApiKey("sk_test_OJiTq50I3SNoO3tL3bOJBhTy");

            //var stripeTokenCreateOptions = new StripeTokenCreateOptions
            //{
            //    Card = new StripeCreditCardOptions
            //    {
            //        Number = model.Number,
            //        ExpirationMonth = int.Parse(model.ExpirationMonth),
            //        ExpirationYear = int.Parse(model.ExpiringYear),
            //        Cvc = model.SecurityCode,
            //        Name =model.CardName
            //    }
            //};

            SCreditCardAttempt creditCardAttemptServices = new SCreditCardAttempt();
            var AttemptResponse = creditCardAttemptServices.IsValidCardAttempt(model.SenderId, model.Number);

            if (AttemptResponse.Data == false)
            {
                result.Message = AttemptResponse.Message;
                result.IsValid = false;
                result.IsLimitMsg = true;
                return result;
            }
            string[] vs = new string[1];
            vs[0] = "ACCOUNTCHECK";

            if (model.ExpirationMonth == null)
            {

                model.ExpirationMonth = "0" + model.ExpirationMonth;
            }
            if (model.ExpirationMonth.Length == 1)
            {

                model.ExpirationMonth = "0" + model.ExpirationMonth;
            }

            SecureTradingApiRequestVm secureTradingApiRequestVm = new SecureTradingApiRequestVm()
            {
                requesttypedescriptions = vs,
                baseamount = "0",
                currencyiso3a = model.CurrencyCode,
                expirydate = model.ExpirationMonth + "/" + model.ExpiringYear,
                orderreference = Guid.NewGuid().ToString(),
                pan = model.Number,
                securitycode = model.SecurityCode,

                //billingpostcode = Common.FaxerSession.LoggedUser.PostCode,
                //billingpremise = Common.FaxerSession.LoggedUser.HouseNo
                billingpostcode = model.billingpostcode,
                billingpremise = model.billingpremise
            };

            SecureTradingApiRequestParam secureTradingApiRequestParam = new SecureTradingApiRequestParam()
            {
                request = secureTradingApiRequestVm
            };
            WebServices webServices = new WebServices();
            var SercureTradingResult = webServices.PostTransaction<SecureTradingApiResponseVm>(webServices.SerializeObject<SecureTradingApiRequestParam>(secureTradingApiRequestParam));

            var response = SercureTradingResult.Result.response.FirstOrDefault();
            if (SercureTradingResult.Result.response.FirstOrDefault().errordata != null)
            {
                if (SercureTradingResult.Result.response.FirstOrDefault().errordata.Contains("pan"))
                {

                    result.Message = "Invalid Card Number";
                    result.IsValid = false;
                }
                else if (SercureTradingResult.Result.response.FirstOrDefault().errordata.Contains("expirydate"))
                {


                    result.Message = "Invalid expiry date";
                    result.IsValid = false;
                }
                else if (SercureTradingResult.Result.response.FirstOrDefault().errordata.Contains("securitycode"))
                {

                    result.Message = "Invalid security code";
                    result.IsValid = false;
                }
                else if (SercureTradingResult.Result.response.FirstOrDefault().errordata.Count() > 0)
                {

                    result.Message = "Card " + SercureTradingResult.Result.response.FirstOrDefault().errormessage;
                    result.IsValid = false;

                }

            }
            else if (int.Parse(SercureTradingResult.Result.response.FirstOrDefault().errorcode) != 0)
            {

                result.Message = SercureTradingResult.Result.response.FirstOrDefault().errormessage;
                result.IsValid = false;

            }
            //else if (int.Parse(response.securityresponseaddress) == 4 || int.Parse(response.securityresponsepostcode) == 4)
            //{
            //    result.Message = "Invalid billing address";
            //    result.IsValid = false;

            //}
            else
            {
                result.Message = "";
                result.IsValid = true;

                string cardtype = SercureTradingResult.Result.response.FirstOrDefault().paymenttypedescription;
                Common.AgentSession.CardType = cardtype;
                if (cardtype.ToLower() == "visa"
                    || cardtype.ToLower() == "master")
                {
                    result.IsCreditCard = true;
                }
            }
            try
            {
                if (SercureTradingResult.Result.response.FirstOrDefault().errorcode == "70000")
                {
                    result.Message = "Your payment has not been authorised, " +
                        "please contact your bank and try again or use another " +
                        "payment method.";

                    try
                    {

                        creditCardAttemptServices.AddCreditCardAttemptLog(new CreditCardAttemptLog()
                        {
                            AttemptedDateTime = DateTime.Now,
                            CardNum = model.Number.Encrypt(),
                            CreditCardAttemptStatus = result.IsValid == true ? CreditCardAttemptStatus.Success :
                            CreditCardAttemptStatus.Failure,
                            Message = result.Message,
                            Module = Module.Faxer,
                            SenderId = model.SenderId
                        });

                    }
                    catch (Exception)
                    {

                    }
                    result.IsCardUsageMsg = true;
                    result.CardUsageMsg = AttemptResponse.Message;

                    //result.Message = "Declined: Contact your card issuer and try again!";   
                }
            }
            catch (Exception)
            {

            }
            string token = "";
            //var tokenService = new StripeTokenService();

            //StripeResponse stripeResponse = new StripeResponse();
            //try
            //{
            //    var stripeToken = tokenService.Create(stripeTokenCreateOptions);

            //    result.StripeTokenId = stripeToken.Id;
            //}
            //catch (Exception ex)
            //{

            //    result.Message = ex.Message;
            //    result.IsValid = false;

            //    //Log.Write("Merchant Auto Payment Exception : " + ex.Message);
            //}

            //AddPaymentRequestLog(SercureTradingResult.Result , new SecureTradingApiRequestVm());
            #endregion


            try
            {

                //creditCardAttemptServices.AddCreditCardAttemptLog(new CreditCardAttemptLog()
                //{
                //    AttemptedDateTime = DateTime.Now,
                //    CardNum = model.Number.Encrypt(),
                //    CreditCardAttemptStatus = result.IsValid == true ? CreditCardAttemptStatus.Success : CreditCardAttemptStatus.Failure,
                //    Message = result.Message,
                //    Module = Module.Faxer,
                //    SenderId = model.SenderId
                //});

            }
            catch (Exception)
            {

            }

            //SCreditDebitCardUsage creditDebitCardUsage = new SCreditDebitCardUsage();
            //var cardusageLimit = creditDebitCardUsage.GetLimitLeftMessage(new CreditCardUsageLog()
            //{

            //    CardNum = model.Number,
            //    Module = DB.Module.Faxer,
            //    SenderId = model.SenderId,
            //    UpdatedDateTime = DateTime.Now
            //});
            //if (cardusageLimit.Data) {

            //    result.CardUsageMsg = cardusageLimit.Message;
            //    result.IsCardUsageMsg = true;
            //}

            return result;


        }

        public static decimal GetExtraFeeAmountIfCreditCard(StripeResultIsValidCardVm vm, decimal Amount)
        {


            if (IsCrebitCard(vm))
            {
                decimal FeeAmount = (decimal)Math.Round((0.601 / 100), 2) * Amount;
                return FeeAmount;
            }
            return 0;

        }
        public static bool IsCrebitCard(StripeResultIsValidCardVm vm)
        {
            try
            {
                var StripeResult = IsValidCardNo(vm);
                return StripeResult.IsCreditCard;
            }
            catch (Exception ex)
            {

                return false;
            }

        }
        public static StripeResult IsValidCardNo(StripeResultIsValidCardVm model, string SendingCountry, string ReceivingCountry)
        {
            StripeResult result = new StripeResult();
            result.IsValid = true;

            SCreditCardAttempt creditCardAttemptServices = new SCreditCardAttempt();
            var AttemptResponse = creditCardAttemptServices.IsValidCardAttempt(model.SenderId, model.Number, SendingCountry, ReceivingCountry);

            if (AttemptResponse.Data == false)
            {
                result.Message = AttemptResponse.Message;
                result.IsValid = false;
                return result;
            }
            string[] vs = new string[1];
            vs[0] = "ACCOUNTCHECK";


            if (model.ExpirationMonth.Length == 1)
            {

                model.ExpirationMonth = "0" + model.ExpirationMonth;
            }

            SecureTradingApiRequestVm secureTradingApiRequestVm = new SecureTradingApiRequestVm()
            {
                requesttypedescriptions = vs,
                baseamount = "0",
                currencyiso3a = model.CurrencyCode,
                expirydate = model.ExpirationMonth + "/" + model.ExpiringYear,
                orderreference = Guid.NewGuid().ToString(),
                pan = model.Number,
                securitycode = model.SecurityCode,

                //billingpostcode = Common.FaxerSession.LoggedUser.PostCode,
                //billingpremise = Common.FaxerSession.LoggedUser.HouseNo
                billingpostcode = model.billingpostcode,
                billingpremise = model.billingpremise
            };

            SecureTradingApiRequestParam secureTradingApiRequestParam = new SecureTradingApiRequestParam()
            {
                request = secureTradingApiRequestVm
            };

            WebServices webServices = new WebServices();
            var SercureTradingResult = webServices.PostTransaction<SecureTradingApiResponseVm>(webServices.SerializeObject<SecureTradingApiRequestParam>(secureTradingApiRequestParam));

            var response = SercureTradingResult.Result.response.FirstOrDefault();
            if (SercureTradingResult.Result.response.FirstOrDefault().errordata != null)
            {
                if (SercureTradingResult.Result.response.FirstOrDefault().errordata.Contains("pan"))
                {

                    result.Message = "Invalid Card Number";
                    result.IsValid = false;
                }
                else if (SercureTradingResult.Result.response.FirstOrDefault().errordata.Contains("expirydate"))
                {


                    result.Message = "Invalid expiry date";
                    result.IsValid = false;
                }
                else if (SercureTradingResult.Result.response.FirstOrDefault().errordata.Contains("securitycode"))
                {

                    result.Message = "Invalid security code";
                    result.IsValid = false;
                }
                else if (SercureTradingResult.Result.response.FirstOrDefault().errordata.Count() > 0)
                {

                    result.Message = "Card " + SercureTradingResult.Result.response.FirstOrDefault().errormessage;
                    result.IsValid = false;

                }

            }
            else if (int.Parse(SercureTradingResult.Result.response.FirstOrDefault().errorcode) != 0)
            {

                result.Message = SercureTradingResult.Result.response.FirstOrDefault().errormessage;
                result.IsValid = false;

            }
            //else if (int.Parse(response.securityresponseaddress) == 4 || int.Parse(response.securityresponsepostcode) == 4)
            //{
            //    result.Message = "Invalid billing address";
            //    result.IsValid = false;

            //}
            else
            {
                result.Message = "";
                result.IsValid = true;

                string cardtype = SercureTradingResult.Result.response.FirstOrDefault().paymenttypedescription;
                Common.AgentSession.CardType = cardtype;
                if (cardtype.ToLower() == "visa"
                    || cardtype.ToLower() == "master")
                {
                    result.IsCreditCard = true;
                }
            }
            try
            {
                if (SercureTradingResult.Result.response.FirstOrDefault().errorcode == "70000")
                {
                    result.Message = "Your payment has not been authorised," +
                        "please contact your bank and try again or use another " +
                        "payment method";

                    //result.Message = "Declined: Contact your card issuer and try again!";   
                }
            }
            catch (Exception)
            {

            }
            string token = "";
            //var tokenService = new StripeTokenService();

            //StripeResponse stripeResponse = new StripeResponse();
            //try
            //{
            //    var stripeToken = tokenService.Create(stripeTokenCreateOptions);

            //    result.StripeTokenId = stripeToken.Id;
            //}
            //catch (Exception ex)
            //{

            //    result.Message = ex.Message;
            //    result.IsValid = false;

            //    //Log.Write("Merchant Auto Payment Exception : " + ex.Message);
            //}

            //AddPaymentRequestLog(SercureTradingResult.Result , new SecureTradingApiRequestVm());


            try
            {

                creditCardAttemptServices.AddCreditCardAttemptLog(new CreditCardAttemptLog()
                {
                    AttemptedDateTime = DateTime.Now,
                    CardNum = model.Number.Encrypt(),
                    CreditCardAttemptStatus = result.IsValid == true ? CreditCardAttemptStatus.Success : CreditCardAttemptStatus.Failure,
                    Message = result.Message,
                    Module = Module.Faxer,
                    SenderId = model.SenderId
                });

            }
            catch (Exception)
            {

            }


            return result;


        }

        public static StripeResult IsValidCardNo(StripeResultIsValidCardVm model, Module module)
        {
            StripeResult result = new StripeResult();
            result.IsValid = true;
            //StripeConfiguration.SetApiKey("sk_test_OJiTq50I3SNoO3tL3bOJBhTy");

            //var stripeTokenCreateOptions = new StripeTokenCreateOptions
            //{
            //    Card = new StripeCreditCardOptions
            //    {
            //        Number = model.Number,
            //        ExpirationMonth = int.Parse(model.ExpirationMonth),
            //        ExpirationYear = int.Parse(model.ExpiringYear),
            //        Cvc = model.SecurityCode,
            //        Name =model.CardName
            //    }
            //};

            SCreditCardAttempt creditCardAttemptServices = new SCreditCardAttempt();
            var AttemptResponse = creditCardAttemptServices.IsValidCardAttempt(model.SenderId, model.Number);

            if (AttemptResponse.Data == false)
            {
                result.Message = AttemptResponse.Message;
                result.IsValid = false;
                result.IsLimitMsg = true;
                return result;
            }
            string[] vs = new string[1];
            vs[0] = "ACCOUNTCHECK";


            if (model.ExpirationMonth.Length == 1)
            {

                model.ExpirationMonth = "0" + model.ExpirationMonth;
            }

            SecureTradingApiRequestVm secureTradingApiRequestVm = new SecureTradingApiRequestVm()
            {
                requesttypedescriptions = vs,
                baseamount = "0",
                currencyiso3a = model.CurrencyCode,
                expirydate = model.ExpirationMonth + "/" + model.ExpiringYear,
                orderreference = Guid.NewGuid().ToString(),
                pan = model.Number,
                securitycode = model.SecurityCode,
                billingpostcode = model.billingpostcode,
                billingpremise = model.billingpremise
            };
            SecureTradingApiRequestParam secureTradingApiRequestParam = new SecureTradingApiRequestParam()
            {
                request = secureTradingApiRequestVm
            };
            WebServices webServices = new WebServices();
            var SercureTradingResult = webServices.PostTransaction<SecureTradingApiResponseVm>(webServices.SerializeObject<SecureTradingApiRequestParam>(secureTradingApiRequestParam));
            var response = SercureTradingResult.Result.response.FirstOrDefault();
            if (SercureTradingResult.Result.response.FirstOrDefault().errordata != null)
            {
                if (SercureTradingResult.Result.response.FirstOrDefault().errordata.Contains("pan"))
                {
                    result.Message = "Invalid Card Number";
                    result.IsValid = false;
                }
                else if (SercureTradingResult.Result.response.FirstOrDefault().errordata.Contains("expirydate"))
                {
                    result.Message = "Invalid expiry date";
                    result.IsValid = false;
                }
                else if (SercureTradingResult.Result.response.FirstOrDefault().errordata.Contains("securitycode"))
                {
                    result.Message = "Invalid security code";
                    result.IsValid = false;
                }
                else if (SercureTradingResult.Result.response.FirstOrDefault().errordata.Count() > 0)
                {
                    result.Message = "Card " + SercureTradingResult.Result.response.FirstOrDefault().errormessage;
                    result.IsValid = false;
                }

            }
            else if (int.Parse(SercureTradingResult.Result.response.FirstOrDefault().errorcode) != 0)
            {

                result.Message = SercureTradingResult.Result.response.FirstOrDefault().errormessage;
                result.IsValid = false;

            }
            //else if (int.Parse(response.securityresponseaddress) == 4 || int.Parse(response.securityresponsepostcode) == 4)
            //{
            //    result.Message = "Invalid billing address";
            //    result.IsValid = false;

            //}
            else
            {
                result.Message = "";
                result.IsValid = true;

                string cardtype = SercureTradingResult.Result.response.FirstOrDefault().paymenttypedescription;
                Common.AgentSession.CardType = cardtype;
                if (cardtype.ToLower() == "visa"
                    || cardtype.ToLower() == "master")
                {
                    result.IsCreditCard = true;
                }
            }
            try
            {
                if (SercureTradingResult.Result.response.FirstOrDefault().errorcode == "70000")
                {
                    result.Message = "Your payment has not been authorised, " +
                        "please contact your bank and try again or use another " +
                        "payment method.";

                    result.IsCardUsageMsg = true;
                    result.CardUsageMsg = AttemptResponse.Message;

                    //result.Message = "Declined: Contact your card issuer and try again!";   
                }
            }
            catch (Exception)
            {

            }
            string token = "";
            //var tokenService = new StripeTokenService();

            //StripeResponse stripeResponse = new StripeResponse();
            //try
            //{
            //    var stripeToken = tokenService.Create(stripeTokenCreateOptions);

            //    result.StripeTokenId = stripeToken.Id;
            //}
            //catch (Exception ex)
            //{

            //    result.Message = ex.Message;
            //    result.IsValid = false;

            //    //Log.Write("Merchant Auto Payment Exception : " + ex.Message);
            //}

            //AddPaymentRequestLog(SercureTradingResult.Result , new SecureTradingApiRequestVm());

            try
            {

                //creditCardAttemptServices.AddCreditCardAttemptLog(new CreditCardAttemptLog()
                //{
                //    AttemptedDateTime = DateTime.Now,
                //    CardNum = model.Number.Encrypt(),
                //    CreditCardAttemptStatus = result.IsValid == true ? CreditCardAttemptStatus.Success : CreditCardAttemptStatus.Failure,
                //    Message = result.Message,
                //    Module = Module.Faxer,
                //    SenderId = model.SenderId
                //});

            }
            catch (Exception)
            {

            }

            //SCreditDebitCardUsage creditDebitCardUsage = new SCreditDebitCardUsage();
            //var cardusageLimit = creditDebitCardUsage.GetLimitLeftMessage(new CreditCardUsageLog()
            //{
            //    CardNum = model.Number,
            //    Module = DB.Module.Faxer,
            //    SenderId = model.SenderId,
            //    UpdatedDateTime = DateTime.Now
            //});
            //if (cardusageLimit.Data) {

            //    result.CardUsageMsg = cardusageLimit.Message;
            //    result.IsCardUsageMsg = true;
            //}

            return result;


        }


        public static IQueryable<SecureTradingApiResponseTransactionLog> GetTransactionLog()
        {

            DB.FAXEREntities dbContext = new FAXEREntities();
            return dbContext.SecureTradingApiResponseTransactionLog;

        }

        public static IQueryable<ThreeDRequestLog> GetThreeDRequestLog()
        {

            DB.FAXEREntities dbContext = new FAXEREntities();
            return dbContext.ThreeDRequestLog;
        }

        public static void AddOrUpdateThreeDLog(ThreeDRequestLog model)
        {

            DB.FAXEREntities dbContext = new FAXEREntities();
            var data = GetThreeDRequestLog().Where(x => x.MD == model.MD).FirstOrDefault();
            if (data != null)
            {
                data.MD = model.MD;
                data.parenttransactionreference = model.parenttransactionreference;
                data.PaReq = model.PaReq;
                data.PaRes = model.PaRes;
                data.ReceiptNo = model.ReceiptNo;
                data.termurl = model.termurl;
                data.ThreeDEnrolled = model.ThreeDEnrolled;
                data.ThreeDStatus = model.ThreeDStatus;

                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

            }
            else
            {
                dbContext.ThreeDRequestLog.Add(model);
                dbContext.SaveChanges();
            }

        }

        public string isCardActiveExpiredStatus(string month, string year)
        {
            int intMonth = month.Decrypt().ToInt();
            int intYear = year.Decrypt().ToInt();
            intYear = intYear + 2000;
            if (intYear < DateTime.Now.Year)
            {
                return "Expired";
            }
            if (intYear > DateTime.Now.Year)
            {
                return "Active";
            }
            if (intYear == DateTime.Now.Year)
            {
                if (intMonth < DateTime.Now.Month)
                {
                    return "Expired";
                }
                if (intMonth >= DateTime.Now.Month)
                {
                    return "Active";
                }
            }
            return "Expired";
        }

        public static StripeResult CreateTransaction(StripeCreateTransactionVM model)
        {


            #region  Strip portion
            StripeResult result = new StripeResult();
            result.IsValid = true;
            //StripeConfiguration.SetApiKey("sk_test_OJiTq50I3SNoO3tL3bOJBhTy");

            //StripeConfiguration.SetApiKey(System.Configuration.ConfigurationManager.AppSettings["StripeApiKey"]);

            //var chargeOptions = new StripeChargeCreateOptions()
            //{

            //    Amount = (Int32)model.Amount * 100,
            //    Currency = model.Currency,
            //    Description = "Charge for " + model.NameOnCard,
            //    SourceTokenOrExistingSourceId = model.StripeTokenId // obtained with Stripe.js
            //};
            WebServices webServices = new WebServices();
            string[] vs = new string[1];
            vs[0] = "AUTH";
            //if (model.ExipiryDate.Split('/')[0].Length == 1)
            //{

            //    model.ExipiryDate = "0" + model.ExipiryDate.Split('/')[0] + "/" + model.ExipiryDate.Split('/')[1];
            //}
            SecureTradingApiRequestVm secureTradingApiRequestVm = new SecureTradingApiRequestVm()
            {
                requesttypedescriptions = vs,
                baseamount = ((Int32)(model.Amount * 100)).ToString(),
                currencyiso3a = model.Currency,
                expirydate = model.ExipiryDate,
                orderreference = model.ReceiptNo,
                pan = model.CardNum,
                securitycode = model.SecurityCode,
                md = model.md,
                pares = model.pares,
                parenttransactionreference = model.parenttransactionreference,
                billingpostcode = model.billingpostcode,
                billingpremise = model.billingpremise
            };

            DB.FAXEREntities dbContext = new FAXEREntities();
            var transactionDetail = dbContext.SecureTradingApiResponseTransactionLog.Where(x => x.md == model.md &&
            x.requesttypedescription == "AUTH").FirstOrDefault();
            if (transactionDetail != null)
            {
                if (transactionDetail.errorcode == "0" && transactionDetail.status == "Y" &&
                    (transactionDetail.settlestatus != "2" && transactionDetail.settlestatus != "3"))
                {
                    //result.IsValid = false;
                    return result;
                }
            }

            //var content = "";
            //if (model.ThreeDEnrolled)
            //{
            //    ThreeDEnableTransactionRequestVm secureTradingApiRequestVm = new ThreeDEnableTransactionRequestVm()
            //    {
            //        requesttypedescriptions = vs,

            //    };

            //    content = webServices.SerializeObject<ThreeDEnableTransactionRequestVm>(secureTradingApiRequestVm);
            //}
            //else {
            //    ThreeDNotEnableTransactionRequestVm secureTradingApiRequestVm = new ThreeDNotEnableTransactionRequestVm()
            //    {
            //        requesttypedescriptions = vs,
            //        parenttransactionreference = model.parenttransactionreference
            //    };

            //    content = webServices.SerializeObject<ThreeDNotEnableTransactionRequestVm>(secureTradingApiRequestVm);
            //}
            SecureTradingApiRequestParam secureTradingApiRequestParam = new SecureTradingApiRequestParam()
            {
                request = secureTradingApiRequestVm
            };
            var SercureTradingResult = webServices.PostTransaction<SecureTradingApiResponseVm>(webServices.SerializeObject<SecureTradingApiRequestParam>(secureTradingApiRequestParam));

            int settlestatus = SercureTradingResult.Result.response.FirstOrDefault().settlestatus == null ? 5 : int.Parse(SercureTradingResult.Result.response.FirstOrDefault().settlestatus);


            if (SercureTradingResult.Result.response.FirstOrDefault().errordata != null
            && SercureTradingResult.Result.response.FirstOrDefault().errordata.Length > 0)
            {
                result.Message = SercureTradingResult.Result.response.FirstOrDefault().errormessage;
                result.IsValid = false;
            }

            else if (SercureTradingResult.Result.response.FirstOrDefault().requesttypedescription.ToLower() == "error")
            {


                result.Message = SercureTradingResult.Result.response.FirstOrDefault().errormessage;
                result.IsValid = false;

            }


            else if (settlestatus == 2 || settlestatus == 3)
            {
                switch (settlestatus)
                {
                    case 0:
                        result.Message = "Transaction is pending settlement";
                        break;
                    case 1:
                        //result.Message = "Your card payment attempt was unsuccessful because your bank did not authorise the payment." +
                        //    " Contact your bank ask them to allow card payments to MoneyFex." +
                        //    "Alternatively, you can fund this transfer via bank transfer or by using a different card." +
                        //    "Message for failed 3D authentication";
                        result.Message = "Your payment has not been authorised, please contact your bank and try again or use another payment method";
                        break;
                    case 2:

                        result.Message = "Your payment has not been authorised, please contact your bank and try again or use another payment method";
                        //result.Message = "Your card payment attempt was unsuccessful because your bank did not authorise the payment." +
                        //    " Contact your bank ask them to allow card payments to MoneyFex." +
                        //    "Alternatively, you can fund this transfer via bank transfer or by using a different card." +
                        //    "Message for failed 3D authentication";
                        break;

                    case 3:

                        result.Message = "Your payment has not been authorised, please contact your bank and try again or use another payment method";
                        //result.Message = "Your card payment attempt was unsuccessful because" +
                        //    " your bank did not authorise the payment." +
                        //    " Contact your bank ask them to allow card payments to MoneyFex." +
                        //    "Alternatively, you can fund this transfer via bank transfer or by using a different card." +
                        //    "Message for failed 3D authentication";
                        break;


                }
                //result.Message = "Sorry cannot complete the transaction";
                result.IsValid = false;

            }

            else if (SercureTradingResult.Result.response.FirstOrDefault().status != "Y")
            {
                result.IsValid = false;
                result.Message = "Authenticate your payment";
            }

            else if (SercureTradingResult.Result.response.FirstOrDefault().status == "A")
            {
                result.IsValid = false;
                result.Message = "Authenticate your payment";
            }
            else if (SercureTradingResult.Result.response.FirstOrDefault().status == "")
            {
                result.IsValid = false;
                result.Message = "Authenticate your payment";
            }

            if (result.IsValid == true && SercureTradingResult.Result.response.FirstOrDefault().status == "Y")
            {
                result.IsValid = true;
                result.Message = "";
            }


            SCreditCardAttempt creditCardAttemptServices = new SCreditCardAttempt();
            try
            {

                creditCardAttemptServices.AddCreditCardAttemptLog(new CreditCardAttemptLog()
                {
                    AttemptedDateTime = DateTime.Now,
                    CardNum = model.CardNum.Encrypt(),
                    CreditCardAttemptStatus = result.IsValid == true ? CreditCardAttemptStatus.Success :
                    CreditCardAttemptStatus.Failure,
                    Message = result.Message,
                    Module = Module.Faxer,
                    SenderId = model.SenderId
                });

            }
            catch (Exception)
            {

            }


            //var chargeService = new StripeChargeService();

            //try
            //{
            //    StripeCharge charge = chargeService.Create(chargeOptions);
            //}
            //catch (Exception ex)
            //{

            //    result.Message = ex.Message;
            //    result.IsValid = false;

            //    //Log.Write("Merchant Auto Payment Exception : " + ex.Message);
            //}


            Log.Write(secureTradingApiRequestVm.orderreference + "  Payment Gateway Transaction Intiated");
            AddPaymentRequestLog(SercureTradingResult.Result, secureTradingApiRequestVm, model.SenderId);

            var data = creditCardAttemptServices.IsValidCardAttempt(model.SenderId, model.CardNum);

            if (data.IsCardUsageMsg == true)
            {
                result.CardUsageMsg = data.Message;
                result.IsCardUsageMsg = true;
            }
            #endregion
            return result;
        }

        public static StripeResult CreateTransaction(StripeCreateTransactionVM model, Module module = Module.Faxer)
        {


            #region  Strip portion
            StripeResult result = new StripeResult();
            result.IsValid = true;
            //StripeConfiguration.SetApiKey("sk_test_OJiTq50I3SNoO3tL3bOJBhTy");

            //StripeConfiguration.SetApiKey(System.Configuration.ConfigurationManager.AppSettings["StripeApiKey"]);

            //var chargeOptions = new StripeChargeCreateOptions()
            //{

            //    Amount = (Int32)model.Amount * 100,
            //    Currency = model.Currency,
            //    Description = "Charge for " + model.NameOnCard,
            //    SourceTokenOrExistingSourceId = model.StripeTokenId // obtained with Stripe.js
            //};
            WebServices webServices = new WebServices();
            string[] vs = new string[1];
            vs[0] = "AUTH";
            //if (model.ExipiryDate.Split('/')[0].Length == 1)
            //{

            //    model.ExipiryDate = "0" + model.ExipiryDate.Split('/')[0] + "/" + model.ExipiryDate.Split('/')[1];
            //}
            SecureTradingApiRequestVm secureTradingApiRequestVm = new SecureTradingApiRequestVm()
            {
                requesttypedescriptions = vs,
                baseamount = ((Int32)(model.Amount * 100)).ToString(),
                currencyiso3a = model.Currency,
                expirydate = model.ExipiryDate,
                orderreference = model.ReceiptNo,
                pan = model.CardNum,
                securitycode = model.SecurityCode,
                md = model.md,
                pares = model.pares,
                parenttransactionreference = model.parenttransactionreference,
                billingpostcode = model.billingpostcode,
                billingpremise = model.billingpremise
            };

            DB.FAXEREntities dbContext = new FAXEREntities();
            var transactionDetail = dbContext.SecureTradingApiResponseTransactionLog.Where(x => x.md == model.md &&
            x.requesttypedescription == "AUTH").FirstOrDefault();
            if (transactionDetail != null)
            {
                if (transactionDetail.errorcode == "0" && transactionDetail.status == "Y" &&
                    (transactionDetail.settlestatus != "2" && transactionDetail.settlestatus != "3"))
                {
                    //result.IsValid = false;
                    return result;
                }
            }

            //var content = "";
            //if (model.ThreeDEnrolled)
            //{
            //    ThreeDEnableTransactionRequestVm secureTradingApiRequestVm = new ThreeDEnableTransactionRequestVm()
            //    {
            //        requesttypedescriptions = vs,

            //    };

            //    content = webServices.SerializeObject<ThreeDEnableTransactionRequestVm>(secureTradingApiRequestVm);
            //}
            //else {
            //    ThreeDNotEnableTransactionRequestVm secureTradingApiRequestVm = new ThreeDNotEnableTransactionRequestVm()
            //    {
            //        requesttypedescriptions = vs,
            //        parenttransactionreference = model.parenttransactionreference
            //    };

            //    content = webServices.SerializeObject<ThreeDNotEnableTransactionRequestVm>(secureTradingApiRequestVm);
            //}
            SecureTradingApiRequestParam secureTradingApiRequestParam = new SecureTradingApiRequestParam()
            {
                request = secureTradingApiRequestVm
            };
            var SercureTradingResult = webServices.PostTransaction<SecureTradingApiResponseVm>(webServices.SerializeObject<SecureTradingApiRequestParam>(secureTradingApiRequestParam));

            int settlestatus = SercureTradingResult.Result.response.FirstOrDefault().settlestatus == null ? 5 : int.Parse(SercureTradingResult.Result.response.FirstOrDefault().settlestatus);


            if (SercureTradingResult.Result.response.FirstOrDefault().errordata != null
            && SercureTradingResult.Result.response.FirstOrDefault().errordata.Length > 0)
            {
                result.Message = SercureTradingResult.Result.response.FirstOrDefault().errormessage;
                result.IsValid = false;
            }

            else if (SercureTradingResult.Result.response.FirstOrDefault().requesttypedescription.ToLower() == "error")
            {


                result.Message = SercureTradingResult.Result.response.FirstOrDefault().errormessage;
                result.IsValid = false;

            }


            else if (settlestatus == 2 || settlestatus == 3)
            {
                switch (settlestatus)
                {
                    case 0:
                        result.Message = "Transaction is pending settlement";
                        break;
                    case 1:
                        //result.Message = "Your card payment attempt was unsuccessful because your bank did not authorise the payment." +
                        //    " Contact your bank ask them to allow card payments to MoneyFex." +
                        //    "Alternatively, you can fund this transfer via bank transfer or by using a different card." +
                        //    "Message for failed 3D authentication";
                        result.Message = "Your payment has not been authorised, please contact your bank and try again or use another payment method";
                        break;
                    case 2:

                        result.Message = "Your payment has not been authorised, please contact your bank and try again or use another payment method";
                        //result.Message = "Your card payment attempt was unsuccessful because your bank did not authorise the payment." +
                        //    " Contact your bank ask them to allow card payments to MoneyFex." +
                        //    "Alternatively, you can fund this transfer via bank transfer or by using a different card." +
                        //    "Message for failed 3D authentication";
                        break;

                    case 3:

                        result.Message = "Your payment has not been authorised, please contact your bank and try again or use another payment method";
                        //result.Message = "Your card payment attempt was unsuccessful because" +
                        //    " your bank did not authorise the payment." +
                        //    " Contact your bank ask them to allow card payments to MoneyFex." +
                        //    "Alternatively, you can fund this transfer via bank transfer or by using a different card." +
                        //    "Message for failed 3D authentication";
                        break;


                }
                //result.Message = "Sorry cannot complete the transaction";
                result.IsValid = false;

            }

            else if (SercureTradingResult.Result.response.FirstOrDefault().status != "Y")
            {
                result.IsValid = false;
                result.Message = "Authenticate your payment";
            }

            else if (SercureTradingResult.Result.response.FirstOrDefault().status == "A")
            {
                result.IsValid = false;
                result.Message = "Authenticate your payment";
            }
            else if (SercureTradingResult.Result.response.FirstOrDefault().status == "")
            {
                result.IsValid = false;
                result.Message = "Authenticate your payment";
            }

            if (result.IsValid == true && SercureTradingResult.Result.response.FirstOrDefault().status == "Y")
            {
                result.IsValid = true;
                result.Message = "";
            }


            SCreditCardAttempt creditCardAttemptServices = new SCreditCardAttempt();
            try
            {

                //creditCardAttemptServices.AddCreditCardAttemptLog(new CreditCardAttemptLog()
                //{
                //    AttemptedDateTime = DateTime.Now,
                //    CardNum = model.CardNum.Encrypt(),
                //    CreditCardAttemptStatus = result.IsValid == true ? CreditCardAttemptStatus.Success :
                //    CreditCardAttemptStatus.Failure,
                //    Message = result.Message,
                //    Module = Module.Faxer,
                //    SenderId = model.SenderId
                //});

            }
            catch (Exception)
            {

            }
            Log.Write(secureTradingApiRequestVm.orderreference + "  Payment Gateway Transaction Intiated");
            AddPaymentRequestLog(SercureTradingResult.Result, secureTradingApiRequestVm, model.SenderId, module);

            var data = creditCardAttemptServices.IsValidCardAttempt(model.SenderId, model.CardNum, module);

            if (data.IsCardUsageMsg == true)
            {
                result.CardUsageMsg = data.Message;
                result.IsCardUsageMsg = true;
            }
            #endregion
            return result;
        }


        public static PGTransactionResultVm GetTransactionDetails(string referenceNo, string currency)
        {

            StripeResult result = new StripeResult();
            result.IsValid = true;
            WebServices webServices = new WebServices();
            string[] vs = new string[1];
            vs[0] = "TRANSACTIONQUERY";
            SecureTradingTransactionQuery secureTradingApiRequestVm = new SecureTradingTransactionQuery();
            secureTradingApiRequestVm.requesttypedescriptions = vs;
            secureTradingApiRequestVm.filter = new SecureTradingTransactionFilterParam();
            secureTradingApiRequestVm.filter.sitereference = new List<FilterKeyVm>();
            secureTradingApiRequestVm.filter.sitereference.Add(new FilterKeyVm(new SecureTradingApiRequestVm().sitereference));
            secureTradingApiRequestVm.filter.currencyiso3a = new List<FilterKeyVm>();
            secureTradingApiRequestVm.filter.currencyiso3a.Add(new FilterKeyVm(currency));
            secureTradingApiRequestVm.filter.transactionreference = new List<FilterKeyVm>();
            secureTradingApiRequestVm.filter.transactionreference.Add(new FilterKeyVm(referenceNo));
            SecureTradingTransactionQueryRequest secureTradingApiRequestParam = new SecureTradingTransactionQueryRequest()
            {
                request = secureTradingApiRequestVm
            };
            var SercureTradingResult = webServices.PostTransaction<SecureTradingApiTransactionQueryResponseVm>(webServices.SerializeObject<SecureTradingTransactionQueryRequest>(secureTradingApiRequestParam));

            var response = SercureTradingResult.Result.response[0].records[0];
            PGTransactionResultVm transactionResult = new PGTransactionResultVm();
            if (response.orderreference != null)
            {
                transactionResult = new PGTransactionResultVm()
                {
                    Status = GetStatusName(response.settlestatus.ToInt()),
                    Amount = (decimal.Parse(response.baseamount) / 100).ToString() + ' ' + response.currencyiso3a,

                    Date = response.transactionstartedtimestamp,
                    Reference = response.orderreference
                };
            }
            return transactionResult;
        }

        public static string GetStatusName(int status)
        {

            string statusName = "";
            switch (status)
            {
                case 0:
                    statusName = " Pending settlement";
                    break;
                case 1:

                    statusName = "Pending settlement (manual override)";
                    break;
                case 2:
                    statusName = "Suspended";
                    break;
                case 3:
                    statusName = "Cancelled";
                    break;
                case 10:
                    statusName = "Settling";
                    break;
                case 100:
                    statusName = "Settled";
                    break;
                default:
                    break;
            }
            return statusName;
        }



        public static StripeResult RefundTransaction(string referenceNo, decimal Amount, RefundType refundType)
        {

            StripeResult result = new StripeResult();
            result.IsValid = true;
            //StripeConfiguration.SetApiKey("sk_test_OJiTq50I3SNoO3tL3bOJBhTy");

            //StripeConfiguration.SetApiKey(System.Configuration.ConfigurationManager.AppSettings["StripeApiKey"]);

            //var chargeOptions = new StripeChargeCreateOptions()
            //{

            //    Amount = (Int32)model.Amount * 100,
            //    Currency = model.Currency,
            //    Description = "Charge for " + model.NameOnCard,
            //    SourceTokenOrExistingSourceId = model.StripeTokenId // obtained with Stripe.js
            //};
            WebServices webServices = new WebServices();
            string[] vs = new string[1];
            vs[0] = "REFUND";
            //if (model.ExipiryDate.Split('/')[0].Length == 1)
            //{

            //    model.ExipiryDate = "0" + model.ExipiryDate.Split('/')[0] + "/" + model.ExipiryDate.Split('/')[1];
            //}

            //DB.FAXEREntities db = new FAXEREntities();
            //var parentTransactionReference = db.SecureTradingApiResponseTransactionLog.Where(x => x.orderreference == referenceNo
            // && x.requesttypedescription == "AUTH" && x.errorcode == "0" && x.status == "Y").FirstOrDefault().transactionreference;

            SecureTradingApiRequestVm secureTradingApiRequestVm = new SecureTradingApiRequestVm();
            secureTradingApiRequestVm.requesttypedescriptions = vs;
            //secureTradingApiRequestVm.parenttransactionreference = parentTransactionReference;
            if (refundType == RefundType.Partial)
            {
                secureTradingApiRequestVm.baseamount = ((Int32)(Amount * 100)).ToString();
            }
            secureTradingApiRequestVm.parenttransactionreference = referenceNo;
            SecureTradingApiRequestParam secureTradingApiRequestParam = new SecureTradingApiRequestParam()
            {
                request = secureTradingApiRequestVm
            };
            var SercureTradingResult = webServices.PostTransaction<SecureTradingApiResponseVm>(webServices.SerializeObject<SecureTradingApiRequestParam>(secureTradingApiRequestParam));

            return result;
        }



        public static void AddPaymentRequestLog(SecureTradingApiResponseVm vm, SecureTradingApiRequestVm requestVm, int SenderId = 0, Module module = Module.Faxer)
        {
            try
            {
                var result = vm.response.FirstOrDefault();
                SecureTradingApiResponseTransactionLog model = new SecureTradingApiResponseTransactionLog()
                {
                    accounttypedescription = result.accounttypedescription,
                    acquirerresponsecode = result.acquirerresponsecode,
                    authcode = result.authcode,
                    baseamount = result.baseamount,
                    currencyiso3a = result.currencyiso3a,
                    dccenabled = result.dccenabled,
                    errorcode = result.errorcode,
                    errordata = result.errordata,
                    errormessage = result.errormessage,
                    issuer = result.issuer,
                    issuercountryiso2a = result.issuercountryiso2a,
                    livestatus = result.livestatus,
                    maskedpan = result.maskedpan,
                    merchantcountryiso2a = result.merchantcountryiso2a,
                    merchantname = result.merchantname,
                    merchantnumber = result.merchantnumber,
                    operatorname = result.operatorname,
                    orderreference = result.orderreference,
                    paymenttypedescription = result.paymenttypedescription,
                    requestreference = vm.requestreference,
                    requesttypedescription = result.requesttypedescription,
                    securityresponseaddress = result.securityresponseaddress,
                    securityresponsepostcode = result.securityresponsepostcode,
                    securityresponsesecuritycode = result.securityresponsesecuritycode,
                    settleduedate = result.settleduedate,
                    settlestatus = result.settlestatus,
                    splitfinalnumber = result.splitfinalnumber,
                    tid = result.tid,
                    transactionreference = result.transactionreference,
                    transactionstartedtimestamp = result.transactionstartedtimestamp,
                    version = vm.version,
                    SenderId = SenderId,  //FaxerSession.SenderId,
                    md = requestVm.md,
                    pares = requestVm.pares,
                    status = result.status,
                    Module = module

                };
                DB.FAXEREntities db = new FAXEREntities();
                db.SecureTradingApiResponseTransactionLog.Add(model);
                db.SaveChanges();

            }
            catch (Exception)
            {

            }


        }

    }

    public class StripeResult
    {
        public string StripeTokenId { get; set; }
        public bool IsValid { get; set; }
        public string Message { get; set; }

        public string CardUsageMsg { get; set; }
        public bool IsCardUsageMsg { get; set; }
        public bool IsCreditCard { get; set; }

        public bool IsLimitMsg { get; set; }

    }

    public class ThreeDEnableTransactionRequestVm : SecureTradingApiThreeDCreateTransactionVm
    {


        public string[] requesttypedescriptions
        {
            get; set;
        }

        public string md { get; set; }
        public string pares { get; set; }

    }
    public class ThreeDNotEnableTransactionRequestVm : SecureTradingApiThreeDCreateTransactionVm
    {

        public string _sitereference = Common.Common.GetAppSettingValue("SecureTradingApiKey").ToString();
        public string[] requesttypedescriptions
        {
            get; set;
        }

        public string sitereference
        {

            get { return _sitereference; }
        }

        public string parenttransactionreference { get; set; }

    }

    public class StripeCreateTransactionVM
    {
        private FaxerInformation faxerInformation;

        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string ReceivingCurrency { get; set; }
        public string StripeTokenId { get; set; }
        public string NameOnCard { get; set; }

        public string CardNum { get; set; }
        public string ExipiryDate { get; set; }

        public string SecurityCode { get; set; }

        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }
        public string SenderEmail { get; set; }
        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }

        public string md { get; set; }
        public string pares { get; set; }

        public string parenttransactionreference { get; set; }

        public string termurl { get; set; }

        public bool ThreeDEnrolled { get; set; }


        public string ReceiptNo { get; set; }


        public string billingpostcode { get; set; }
        public string billingpremise { get; set; }

        public int SenderId { get; set; }
        public string city { get; internal set; }
        public string state { get; internal set; }
        public string Phone { get; internal set; }
    }

    public class StripeResultIsValidCardVm
    {
        [StringLength(16)]
        public string Number { get; set; }

        [StringLength(2)]
        public string ExpirationMonth { get; set; }

        [MaxLength(4)]
        public string ExpiringYear { get; set; }

        [MaxLength(3)]
        public string SecurityCode { get; set; }
        [StringLength(200)]
        public string CardName { get; set; }
        public string billingpostcode { get; set; }
        public string billingpremise { get; set; }
        public string CurrencyCode { get; set; }
        public decimal Amount { get; set; }

        public int SenderId { get; set; }
        public bool IsCreditDebitCardCheck { get; set; }

    }

    public class PGTransactionResultVm
    {

        public string Status { get; set; }
        public string Amount { get; set; }
        public string Reference { get; set; }
        public string Date { get; set; }
    }

}