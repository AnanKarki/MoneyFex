using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.BankApi;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.SecureTradingPaymentGateway;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Http.Results;
using Twilio.TwiML.Voice;
using static FAXER.PORTAL.BankApi.Models.Transact365ViewModel;

namespace FAXER.PORTAL.Services
{
    public class Transact365Serivces
    {
        public static int SenderId = 0;
        public static Module module = Module.Faxer;

        private static PayingSenderInformation GetPayingSenderInformation()
        {
            PayingSenderInformation payingSenderInformation = new PayingSenderInformation();

            FAXEREntities dbContext = new FAXEREntities();
            switch (module)
            {
                case Module.Faxer:
                    int senderId = 0;
                    if (Common.FaxerSession.LoggedUser == null)
                    {
                        senderId = SenderId;
                    }
                    else
                    {
                        senderId = Common.FaxerSession.LoggedUser.Id;
                    }

                    var senderInfo = dbContext.FaxerInformation.Where(x => x.Id == senderId).FirstOrDefault();
                    payingSenderInformation = new PayingSenderInformation()
                    {
                        billingAddress = senderInfo.Address2,
                        city = senderInfo.City,
                        postcode = senderInfo.PostalCode,
                        state = senderInfo.State,
                        Phone = senderInfo.PhoneNumber

                    };
                    break;
                case Module.CardUser:
                    break;
                case Module.BusinessMerchant:
                    break;
                case Module.Agent:
                    int AgentId = Common.AgentSession.LoggedUser.Id;
                    var agentInfo = dbContext.AgentStaffInformation.Where(x => x.Id == AgentId).FirstOrDefault();
                    payingSenderInformation = new PayingSenderInformation()
                    {
                        billingAddress = agentInfo.Address2,
                        city = agentInfo.City,
                        postcode = agentInfo.PostalCode,
                        state = agentInfo.State,
                        Phone = agentInfo.PhoneNumber

                    };
                    break;
                case Module.Staff:
                    break;
                case Module.KiiPayBusiness:
                    break;
                case Module.KiiPayPersonal:
                    break;
                default:
                    break;
            }

            return payingSenderInformation;



        }
        public static ServiceResult<ThreeDRequestVm> CreatePayment(StripeCreateTransactionVM vm)
        {
            Transact365Api transact365Api = new Transact365Api();
            var baseurl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            T365RequestVm<T365RequestDataVm> model = new T365RequestVm<T365RequestDataVm>();

            var date = vm.ExipiryDate.Split('/');
            string expirymonth = date[0];
            if (date[0].Length == 1)
            {
                expirymonth = "0" + date[0];
            }
            string currentYear = (DateTime.Now.Year).ToString();

            string expiryYear = "20" + date[1];
            try
            {
                expiryYear = currentYear.Substring(0, 2) + date[1];
            }
            catch (Exception ex)
            {
                Log.Write("Transact 365 api expiry year parsing error " + ex.Message, ErrorType.UnSpecified, "Transact 365 api ");
            }
            //string expiryYear = "20" + date[1];


            var senderInformation = GetPayingSenderInformation();
            T365RequestDataVm data = new T365RequestDataVm()
            {
                amount = ((int)(vm.Amount * 100)).ToString(),
                currency = vm.Currency,
                tracking_id = vm.ReceiptNo,
                language = "en",
                description = "Payment",
                return_url = baseurl + vm.termurl,
                billing_address = new T365BillingAddressVm
                {
                    first_name = vm.SenderFirstName,
                    last_name = vm.SenderLastName,
                    country = vm.SendingCountry,
                    phone = senderInformation.Phone,
                    city = senderInformation.city ?? "Denver",
                    state = vm.SendingCountry == "US" ? senderInformation.state : null, // state is not required 
                    zip = senderInformation.postcode ?? "96002",
                    address = senderInformation.billingAddress ?? "1st Street"
                },
                credit_card = new T365CreditCardVm
                {
                    number = vm.CardNum,
                    verification_value = vm.SecurityCode,
                    holder = vm.NameOnCard,
                    exp_month = expirymonth,
                    exp_year = expiryYear,
                },
                customer = new T365CustomerVm
                {
                    ip = "127.0.0.1",
                    email = vm.SenderEmail
                    //email = "john@example.com"
                }

            };
            model.request = data;

            var response = transact365Api.CreatePayment(model);


            var Status = new ResultStatus();
            string message = "";
            if (response == null)
            {

                message = "Invalid Card Details";
                Status = ResultStatus.Error;

                return new ServiceResult<ThreeDRequestVm>()
                {
                    Data = new ThreeDRequestVm(),
                    Status = Status,
                    Message = message,
                    IsGetType3dAuth = true,
                };
            }
            ThreeDRequestVm threeDRequestVm = new ThreeDRequestVm()
            {
                MD = response.transaction.three_d_secure_verification.md,
                PaReq = response.transaction.three_d_secure_verification.pa_req,
                termurl = response.transaction.redirect_url,
                acsurl = response.transaction.three_d_secure_verification.acs_url,
                redirect_url = response.transaction.redirect_url,
                ThreeDEnrolled = response.transaction.three_d_secure_verification.ve_status,
                UId = response.transaction.uid,
                CardProcessorApi = CardProcessorApi.T365
            };
            if (response.transaction.three_d_secure_verification.ve_status == "Y")
            {
                Status = ResultStatus.OK;
                message = response.transaction.message;
            }
            else
            {
                message = "Invalid Card";
                Status = ResultStatus.Error;
            }
            AddTransact365ApiResponseTransationLog(response.transaction);

            ServiceResult<ThreeDRequestVm> result = new ServiceResult<ThreeDRequestVm>()
            {
                Data = threeDRequestVm,
                Status = Status,
                Message = message,
                IsGetType3dAuth = true,
            };
            return result;
        }
        public static ServiceResult<ThreeDRequestVm> GetTransationDetails(string uid)
        {
            Transact365Api transact365Api = new Transact365Api();

            var response = transact365Api.GetTransaction(uid);

            ThreeDRequestVm threeDRequestVm = new ThreeDRequestVm()
            {
                MD = response.transaction.three_d_secure_verification.md,
                PaReq = response.transaction.three_d_secure_verification.pa_req,
                termurl = response.transaction.redirect_url,
                acsurl = response.transaction.three_d_secure_verification.acs_url,
                redirect_url = response.transaction.redirect_url,
                ThreeDEnrolled = response.transaction.three_d_secure_verification.ve_status,
                UId = response.transaction.uid
            };
            var Status = new ResultStatus();
            if (threeDRequestVm.ThreeDEnrolled == "Y" && response.transaction.three_d_secure_verification.status == "successful"
                && (response.transaction.status == "successful" || response.transaction.status == "incomplete"))
            {
                Status = ResultStatus.OK;
            }
            else
            {
                Status = ResultStatus.Error;
            }
            AddTransact365ApiResponseTransationLog(response.transaction);
            ServiceResult<ThreeDRequestVm> result = new ServiceResult<ThreeDRequestVm>()
            {
                Data = threeDRequestVm,
                Status = Status,
                Message = response.transaction.message,
                IsGetType3dAuth = true,
            };
            return result;
        }
        public static ServiceResult<ThreeDRequestVm> GetTransationDetails(string uid, string trackingid)
        {
            Transact365Api transact365Api = new Transact365Api();

            var response = transact365Api.GetTransaction(uid, trackingid);

            ThreeDRequestVm threeDRequestVm = new ThreeDRequestVm()
            {
                MD = response.transaction.three_d_secure_verification.md,
                PaReq = response.transaction.three_d_secure_verification.pa_req,
                termurl = response.transaction.redirect_url,
                acsurl = response.transaction.three_d_secure_verification.acs_url,
                redirect_url = response.transaction.redirect_url,
                ThreeDEnrolled = response.transaction.three_d_secure_verification.ve_status,
                UId = response.transaction.uid
            };

            var Status = new ResultStatus();
            if (threeDRequestVm.ThreeDEnrolled == "Y" &&
                response.transaction.three_d_secure_verification.status == "successful" &&
                (response.transaction.status == "successful" || response.transaction.status == "incomplete"))
            {
                Status = ResultStatus.OK;

            }
            else
            {
                Status = ResultStatus.Error;
            }
            AddTransact365ApiResponseTransationLog(response.transaction);
            AddT365CardAttempt(Status, response.transaction.message);
            ServiceResult<ThreeDRequestVm> result = new ServiceResult<ThreeDRequestVm>()
            {
                Data = threeDRequestVm,
                Status = Status,
                Message = response.transaction.message,
                IsGetType3dAuth = true,
            };
            return result;
        }

        private static void AddT365CardAttempt(ResultStatus status, string message)
        {

            try
            {
                SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
                var transferSummary = _senderForAllTransferServices.GetTransactionSummary();
                var cardInfo = transferSummary.CreditORDebitCardDetials;

                int SenderId = Common.FaxerSession.LoggedUser.Id;

                SCreditCardAttempt creditCardAttemptServices = new SCreditCardAttempt();
                creditCardAttemptServices.AddCreditCardAttemptLog(new CreditCardAttemptLog()
                {
                    AttemptedDateTime = DateTime.Now,
                    CardNum = cardInfo.CardNumber.Encrypt(),
                    CreditCardAttemptStatus = status == ResultStatus.OK ? CreditCardAttemptStatus.Success :
                    CreditCardAttemptStatus.Failure,
                    Message = message,
                    Module = Module.Faxer,
                    SenderId = SenderId
                });
            }
            catch (Exception)
            {


            }


        }

        public static void AddTransact365ApiResponseTransationLog(T365ResponseDataVm response)
        {
            try
            {
                Transact365ApiResponseTransationLog model = new Transact365ApiResponseTransationLog()
                {
                    Amount = response.amount.ToDecimal(),
                    BillingAddressAddress = response.billing_address.address,
                    BillingAddressCity = response.billing_address.city,
                    BillingAddressCountry = response.billing_address.country,
                    BillingAddressFirstName = response.billing_address.first_name,
                    BillingAddressLastName = response.billing_address.last_name,
                    BillingAddressPhone = response.billing_address.phone,
                    BillingAddressState = response.billing_address.state,
                    BillingAddressZip = response.billing_address.zip,
                    Brand = response.credit_card.brand,
                    CreatedAt = response.created_at,
                    Currency = response.currency,
                    Description = response.description,
                    ExpMonth = response.credit_card.exp_month,
                    ExpYear = response.credit_card.exp_year,
                    First1DigitCardNum = response.credit_card.first_1,
                    Holder = response.credit_card.holder,
                    Language = response.language,
                    Last4DigitCardNum = response.credit_card.last_4,
                    Message = response.message,
                    PaymentMethodType = response.payment_method_type,
                    Product = response.credit_card.product,
                    ReceiptUrl = response.receipt_url,
                    RedirectUrl = response.redirect_url,
                    Status = response.status,
                    Test = response.test,
                    ThreeDAcsUrl = response.three_d_secure_verification.acs_url,
                    ThreeDMd = response.three_d_secure_verification.md,
                    ThreeDMessage = response.three_d_secure_verification.message,
                    ThreeDPaReq = response.three_d_secure_verification.pa_req,
                    ThreeDPaResUrl = response.three_d_secure_verification.pa_res_url,
                    ThreeDStatus = response.three_d_secure_verification.status,
                    ThreeDVeStatus = response.three_d_secure_verification.ve_status,
                    Token = response.credit_card.token,
                    TokenProvider = response.credit_card.token_provider,
                    TrackingId = response.tracking_id,
                    TransationId = response.id,
                    Type = response.type,
                    UId = response.uid,
                    UpdatedAt = response.updated_at,
                    SenderId = SenderId,
                    IssuerCountry = response.credit_card.issuer_country,
                    Module = module,
                    //SettledAt = response.settled_at,
                    //PaidAt = response.paid_at,
                    //Bin = response.credit_card.bin,
                    //ClosedAt = response.closed_at,
                    //IssuerCountry = response.credit_card.issuer_country,
                    //IssuerName = response.credit_card.issuer_name,
                    //ExpiredAt = response.expired_at,
                    //ManuallyCorrectedAt = response.manually_corrected_at,
                    //ThreeDPaStatus = response.three_d_secure_verification.pa_status,
                    //ThreeDXId = response.three_d_secure_verification.xid,
                    //ThreeDCavv = response.three_d_secure_verification.cavv,
                    //ThreeDCavvAlgorithm = response.three_d_secure_verification.cavv_algorithm,
                    //ThreeDECI = response.three_d_secure_verification.eci,
                    //ThreeDFailReason = response.three_d_secure_verification.fail_reason,
                };
                AddOrUpdateLog(model);
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message, ErrorType.T365, "AddTransact365ApiResponseTransationLog");
            }

        }

        public static void AddOrUpdateLog(Transact365ApiResponseTransationLog model)
        {
            FAXEREntities dbContext = new FAXEREntities();

            var Transact365ApiResponseTransationLog = dbContext.Transact365ApiResponseTransationLog.Where(x => x.UId == model.UId &&
                                                      x.TrackingId == model.TrackingId).FirstOrDefault();
            if (Transact365ApiResponseTransationLog == null)
            {
                dbContext.Transact365ApiResponseTransationLog.Add(model);
                dbContext.SaveChanges();
            }
            else
            {
                Transact365ApiResponseTransationLog.Amount = model.Amount;
                Transact365ApiResponseTransationLog.BillingAddressAddress = model.BillingAddressAddress;
                Transact365ApiResponseTransationLog.BillingAddressCity = model.BillingAddressCity;
                Transact365ApiResponseTransationLog.BillingAddressCountry = model.BillingAddressCountry;
                Transact365ApiResponseTransationLog.BillingAddressFirstName = model.BillingAddressFirstName;
                Transact365ApiResponseTransationLog.BillingAddressLastName = model.BillingAddressLastName;
                Transact365ApiResponseTransationLog.BillingAddressPhone = model.BillingAddressPhone;
                Transact365ApiResponseTransationLog.BillingAddressState = model.BillingAddressState;
                Transact365ApiResponseTransationLog.BillingAddressZip = model.BillingAddressZip;
                Transact365ApiResponseTransationLog.Brand = model.Brand;
                Transact365ApiResponseTransationLog.CreatedAt = model.CreatedAt;
                Transact365ApiResponseTransationLog.Currency = model.Currency;
                Transact365ApiResponseTransationLog.Description = model.Description;
                Transact365ApiResponseTransationLog.ExpMonth = model.ExpMonth;
                Transact365ApiResponseTransationLog.ExpYear = model.ExpYear;
                Transact365ApiResponseTransationLog.First1DigitCardNum = model.First1DigitCardNum;
                Transact365ApiResponseTransationLog.Holder = model.Holder;
                Transact365ApiResponseTransationLog.Language = model.Language;
                Transact365ApiResponseTransationLog.Last4DigitCardNum = model.Last4DigitCardNum;
                Transact365ApiResponseTransationLog.Message = model.Message;
                Transact365ApiResponseTransationLog.PaymentMethodType = model.PaymentMethodType;
                Transact365ApiResponseTransationLog.Product = model.Product;
                Transact365ApiResponseTransationLog.ReceiptUrl = model.ReceiptUrl;
                Transact365ApiResponseTransationLog.RedirectUrl = model.RedirectUrl;
                Transact365ApiResponseTransationLog.Status = model.Status;
                Transact365ApiResponseTransationLog.Test = model.Test;
                Transact365ApiResponseTransationLog.ThreeDAcsUrl = model.ThreeDAcsUrl;
                Transact365ApiResponseTransationLog.ThreeDMd = model.ThreeDMd;
                Transact365ApiResponseTransationLog.ThreeDMessage = model.ThreeDMessage;
                Transact365ApiResponseTransationLog.ThreeDPaReq = model.ThreeDPaReq;
                Transact365ApiResponseTransationLog.ThreeDPaResUrl = model.ThreeDPaResUrl;
                Transact365ApiResponseTransationLog.ThreeDStatus = model.ThreeDStatus;
                Transact365ApiResponseTransationLog.ThreeDVeStatus = model.ThreeDVeStatus;
                Transact365ApiResponseTransationLog.Token = model.Token;
                Transact365ApiResponseTransationLog.TokenProvider = model.TokenProvider;
                Transact365ApiResponseTransationLog.TrackingId = model.TrackingId;
                Transact365ApiResponseTransationLog.TransationId = model.TransationId;
                Transact365ApiResponseTransationLog.Type = model.Type;
                Transact365ApiResponseTransationLog.UId = model.UId;
                Transact365ApiResponseTransationLog.UpdatedAt = model.UpdatedAt;
                Transact365ApiResponseTransationLog.SenderId = model.SenderId;
                Transact365ApiResponseTransationLog.IssuerCountry = model.IssuerCountry;
                Transact365ApiResponseTransationLog.Module = model.Module;
                dbContext.Entry(Transact365ApiResponseTransationLog).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
        }

        public static PGTransactionResultVm GetTranstionStatus(string uid, string trackingId = "")
        {
            PGTransactionResultVm result = new PGTransactionResultVm();
            Transact365Api transact365Api = new Transact365Api();
            var response = transact365Api.GetTransactions(uid, trackingId);
            if (response != null)
            {
                try
                {
                    var transaction = response.transactions[0];
                    result = new PGTransactionResultVm()
                    {
                        Status = transaction.status,
                        Amount = transaction.amount + " " + transaction.currency,
                        Date = transaction.created_at,
                        Reference = transaction.tracking_id
                    };
                }
                catch (Exception)
                {

                }

            }
            return result;
        }
    }

    public class PayingSenderInformation
    {


        public string city { get; set; }
        public string state { get; set; }
        public string postcode { get; set; }
        public string billingAddress { get; set; }

        public string Phone { get; set; }

    }
}