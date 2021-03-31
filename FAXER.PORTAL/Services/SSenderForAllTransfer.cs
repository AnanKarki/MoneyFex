using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.BankApi;
using FAXER.PORTAL.BankApi.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.MoblieTransferApi.Models;
using FAXER.PORTAL.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TransferZero.Sdk.Api;
using TransferZero.Sdk.Client;
using TransferZero.Sdk.Model;
using static FAXER.PORTAL.BankApi.Models.FlutterWaveViewModel;

namespace FAXER.PORTAL.Services
{
    public class SSenderForAllTransfer
    {
        DB.FAXEREntities dbContext = null;
        SSenderMobileMoneyTransfer _senderMobileMoneyTransfer = null;
        SSenderBankAccountDeposit _senderBankAccountDeposit = null;
        public SSenderForAllTransfer()
        {
            dbContext = new DB.FAXEREntities();
            _senderMobileMoneyTransfer = new SSenderMobileMoneyTransfer();
            _senderBankAccountDeposit = new SSenderBankAccountDeposit();
        }

        public void SetTransactionSummary(TransactionSummaryVM model)
        {
            var id = HttpContext.Current.Session.SessionID;
            // Common.FaxerSession.TransactionSummary = model;
            SSessionTransactionSummary _sessionTransactionSummaryServices = new SSessionTransactionSummary(id);
            _sessionTransactionSummaryServices.SaveSessionTransactionSummary(model);
        }

        public TransactionSummaryVM GetTransactionSummary()
        {
            var id = HttpContext.Current.Session.SessionID;
            TransactionSummaryVM vm = new TransactionSummaryVM();
            //if (Common.FaxerSession.TransactionSummary != null)
            //{

            //    vm = Common.FaxerSession.TransactionSummary;
            //}
            SSessionTransactionSummary _sessionTransactionSummaryServices = new SSessionTransactionSummary(id);
            vm = _sessionTransactionSummaryServices.GetTransactionSummarySession();
            return vm;
        }
        public void SetAdminTransactionSummary(TransactionSummaryVM model)
        {

            Common.AdminSession.TransactionSummary = model;
        }

        public TransactionSummaryVM GetAdminTransactionSummary()
        {

            TransactionSummaryVM vm = new TransactionSummaryVM();
            if (Common.AdminSession.TransactionSummary != null)
            {

                vm = Common.AdminSession.TransactionSummary;
            }
            else
            {
                vm.KiiPayTransferPaymentSummary = new KiiPayTransferPaymentSummary();
            }
            return vm;
        }
        public StripeResult ValidateTransactionUsingStripe(TransactionSummaryVM model)
        {

            StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
            {
                CardName = model.CreditORDebitCardDetials.NameOnCard,
                ExpirationMonth = model.CreditORDebitCardDetials.EndMM,
                ExpiringYear = model.CreditORDebitCardDetials.EndYY,
                Number = model.CreditORDebitCardDetials.CardNumber,
                SecurityCode = model.CreditORDebitCardDetials.SecurityCode,
                billingpostcode = Common.FaxerSession.LoggedUser.PostCode,
                billingpremise = Common.FaxerSession.LoggedUser.HouseNo,
                CurrencyCode = Common.Common.GetCurrencyCode(Common.FaxerSession.LoggedUser.CountryCode),
                Amount = model.CreditORDebitCardDetials.FaxingAmount

            };
            var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm);


            if (!StripeResult.IsValid)
            {

                return StripeResult;
            }

            else
            {



            }
            return StripeResult;


        }
        public StripeResult ValidateTransactionUsingStripe(TransactionSummaryVM model, string SendingCountry, string ReceivingCuuntry)
        {

            StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
            {
                CardName = model.CreditORDebitCardDetials.NameOnCard,
                ExpirationMonth = model.CreditORDebitCardDetials.EndMM,
                ExpiringYear = model.CreditORDebitCardDetials.EndYY,
                Number = model.CreditORDebitCardDetials.CardNumber,
                SecurityCode = model.CreditORDebitCardDetials.SecurityCode,
                billingpostcode = Common.FaxerSession.LoggedUser.PostCode,
                billingpremise = Common.FaxerSession.LoggedUser.HouseNo,
                CurrencyCode = Common.Common.GetCurrencyCode(Common.FaxerSession.LoggedUser.CountryCode),
                Amount = model.CreditORDebitCardDetials.FaxingAmount

            };
            var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm, SendingCountry, ReceivingCuuntry);


            if (!StripeResult.IsValid)
            {

                return StripeResult;
            }

            else
            {



            }
            return StripeResult;


        }
        public ServiceResult<bool> CompleteTransaction(TransactionSummaryVM vm)
        {

            switch (vm.TransferType)
            {
                case TransferType.KiiPayWallet:
                    CompleteKiiPayWalletTransaction(vm);
                    break;
                case TransferType.PayForServices:
                    CompletePayForServicesTransaction(vm);
                    break;
                case TransferType.CashPickup:
                    CompleteCashPickupTransaction(vm);
                    break;
                case TransferType.MobileTransfer:
                    CompleteMobileTransferTransaction(vm);
                    break;
                case TransferType.BankDeposit:
                    CompleteBankDepositTransaction(vm);
                    break;
                case TransferType.PayARequest:
                    CompletePayARequestTransaction(vm);
                    break;
                default:
                    break;
            }

            HttpContext.Current.Session.Remove("TransactionId");
            HttpContext.Current.Session.Remove("RecipientId");
            HttpContext.Current.Session.Remove("IsTransactionOnpending");

            return new ServiceResult<bool>()
            {

                Data = true,
                Message = "Transaction Successfully Completed",
                Status = ResultStatus.OK
            };
        }
        public void KiiPayWalletBalIN(int WalletId, decimal Amount)
        {

            var receiverWalletData = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == WalletId).FirstOrDefault();


            //deducting amount from sender's wallet

            receiverWalletData.CurrentBalance = receiverWalletData.CurrentBalance + Amount;
            dbContext.Entry(receiverWalletData).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }

        public void KiiPayWalletBalOut(int WalletId, decimal Amount)
        {


            var senderWalletData = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == WalletId).FirstOrDefault();

            senderWalletData.CurrentBalance = senderWalletData.CurrentBalance - Amount;
            dbContext.Entry(senderWalletData).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();


        }
        public bool CompleteKiiPayWalletTransaction(TransactionSummaryVM vm)
        {



            switch (vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode)
            {
                case Models.SenderPaymentMode.SavedDebitCreditCard:

                    KiiPayWalletPaymentUsingCreditDebitCard(vm);
                    AddUpdateLog(vm.CreditORDebitCardDetials);
                    break;
                case Models.SenderPaymentMode.CreditDebitCard:
                    KiiPayWalletPaymentUsingCreditDebitCard(vm);
                    AddUpdateLog(vm.CreditORDebitCardDetials);
                    break;
                case Models.SenderPaymentMode.KiiPayWallet:
                    KiiPayWalletBalIN(vm.SenderAndReceiverDetail.ReceiverId, vm.KiiPayTransferPaymentSummary.ReceivingAmount);
                    PayUsingKiiPayWallet(vm);
                    break;
                case Models.SenderPaymentMode.MoneyFexBankAccount:
                    KiiPayWalletPaymentUsingCreditDebitCard(vm);
                    break;
                default:
                    break;
            }


            return true;
        }


        public FWReponse<FlutterWaveResonse> CreateFlutterWaveTransaction(FlutterWaveVm model)
        {
            FlutterWaveApi webapi = new FlutterWaveApi();
            var response = webapi.CreateTransaction(CommonExtension.SerializeObject(model));
            return response;
        }


        public FWReponse<FlutterWaveResonse> GetFlutterWaveTransactionById(int id)
        {
            FlutterWaveApi webapi = new FlutterWaveApi();
            var response = webapi.GetTransactionById(id);
            return response;
        }
        public FWReponse<FlutterCommonResponseDataVm> ValidateFlutterWaveTransaction(string flwRef, string otp)
        {
            FlutterWaveApi webapi = new FlutterWaveApi();
            var response = webapi.ValidatedTransaction(flwRef, otp);
            return response;
        }

        public FWReponse<FlutterCommonVerifyResponseDataVm> GetFlutterWaveStatus(string txRef)
        {
            FlutterWaveApi webapi = new FlutterWaveApi();
            var response = webapi.VerifyTransation(txRef);
            return response;
        }
        public bool PayUsingKiiPayWallet(TransactionSummaryVM vm)
        {
            var paymentsummary = vm.KiiPayTransferPaymentSummary;
            var senderAndReceiverInfo = vm.SenderAndReceiverDetail;
            var PaymentMethodAndAutoPaymentFrequencyInfo = vm.PaymentMethodAndAutoPaymentDetail;

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            DB.KiiPayPersonalWalletPaymentByKiiPayPersonal payement = new DB.KiiPayPersonalWalletPaymentByKiiPayPersonal()
            {
                ExchangeRate = paymentsummary.ExchangeRate,
                FaxingAmount = paymentsummary.SendingAmount,
                FaxingFee = paymentsummary.Fee,
                IsAutoPayment = false,
                IsRefunded = false,
                PaymentReference = paymentsummary.PaymentReference,
                PaymentType = vm.IsLocalPayment == true ? DB.PaymentType.Local : DB.PaymentType.International,
                ReceiptNumber = Common.Common.GetKiiPayPersWalletReceiptNo(),
                SenderWalletId = senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id).Id,
                ReceiverWalletId = senderAndReceiverInfo.ReceiverId,
                SendingCountry = senderAndReceiverInfo.SenderCountry,
                ReceivingCountry = senderAndReceiverInfo.ReceiverCountry,
                TransactionDate = DateTime.Now,
                ReceivingMobileNumber = senderAndReceiverInfo.ReceiverMobileNo,
                RecievingAmount = paymentsummary.ReceivingAmount,
                TotalAmount = paymentsummary.TotalAmount,
                SenderId = senderAndReceiverInfo.SenderId,
                TransactionFromPortal = DB.TransactionFrom.SenderPortal
            };


            int senderWalletId = senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id).Id;
            KiiPayWalletBalOut(senderWalletId, vm.KiiPayTransferPaymentSummary.TotalAmount);

            KiipayWalletStandingOrderPayment(vm);
            return true;
        }


        public void KiipayWalletStandingOrderPayment(TransactionSummaryVM vm)
        {

            if ((vm.PaymentMethodAndAutoPaymentDetail.EnableAutoPayment == true)
           && vm.PaymentMethodAndAutoPaymentDetail.AutoPaymentAmount > 0 && (int)vm.PaymentMethodAndAutoPaymentDetail.AutopaymentFrequency > 0)
            {
                var cardDetails = dbContext.SavedCard.Where(x => x.UserId == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
                if (cardDetails != null)
                {
                    var data = dbContext.OtherMFTCCardAutoTopUpInformation.Where(x => x.MFTCCardId == vm.SenderAndReceiverDetail.ReceiverId && x.FaxerId == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();


                    if (data != null)
                    {

                        data.AutoPaymentAmount = vm.PaymentMethodAndAutoPaymentDetail.AutoPaymentAmount;
                        data.AutoPaymentFrequency = vm.PaymentMethodAndAutoPaymentDetail.AutopaymentFrequency;
                        data.EnableAutoPayment = true;
                        data.FrequencyDetails = vm.PaymentMethodAndAutoPaymentDetail.PaymentDay;
                        data.TopUpReference = "";
                        dbContext.Entry(data).State = EntityState.Modified;
                        dbContext.SaveChanges();

                    }
                    else
                    {
                        OtherMFTCCardAutoTopUpInformation autoTopUpInformation = new OtherMFTCCardAutoTopUpInformation()
                        {

                            MFTCCardId = vm.SenderAndReceiverDetail.ReceiverId,
                            AutoPaymentAmount = vm.PaymentMethodAndAutoPaymentDetail.AutoPaymentAmount,
                            AutoPaymentFrequency = vm.PaymentMethodAndAutoPaymentDetail.AutopaymentFrequency,
                            EnableAutoPayment = true,
                            FaxerId = Common.FaxerSession.LoggedUser.Id,
                            TopUpReference = "",
                            FrequencyDetails = vm.PaymentMethodAndAutoPaymentDetail.PaymentDay

                        };
                        dbContext.OtherMFTCCardAutoTopUpInformation.Add(autoTopUpInformation);
                        dbContext.SaveChanges();

                    }
                }
            }


        }

        public ServiceResult<bool> CancelTransaction(int transactionId, TransactionServiceType transactionServiceType)
        {
            var result = new ServiceResult<bool>();
            switch (transactionServiceType)
            {
                case TransactionServiceType.All:
                    break;
                case TransactionServiceType.MobileWallet:
                    result = CancelMobileWalletTransaction(transactionId);
                    break;
                case TransactionServiceType.KiiPayWallet:
                    break;
                case TransactionServiceType.BillPayment:
                    break;
                case TransactionServiceType.ServicePayment:
                    break;
                case TransactionServiceType.CashPickUp:
                    result = CancelCashPickUPTransaction(transactionId);
                    break;
                case TransactionServiceType.BankDeposit:
                    result = CancelBankDepositTransaction(transactionId);
                    break;
                default:
                    break;
            }
            return result;

        }

        internal ServiceResult<bool> CancelBankDepositTransaction(int transactionId)
        {
            var bankDeposit = dbContext.BankAccountDeposit.Where(x => x.Id == transactionId).FirstOrDefault();


            TransferZero.Sdk.Client.Configuration configuration = new TransferZero.Sdk.Client.Configuration();
            //configuration.ApiKey = "H0IwflB19SlXy3oAj65r6Q3zAHvgJWVol+p06XqcxgXJoroa5DBdULMRduJ0D4iIYn8pWp+cq2WtvZp3/6n62Q==";
            //configuration.ApiSecret = "Haa6fKwe6oN3Y+V3TWpNrGty576MKHXvlDOdX21vJRCk9zAVa4a9cV1ED4C/N42dLXqxBv/Kfs0jzaCizONT7Q==";
            //configuration.BasePath = "https://api-sandbox.transferzero.com/v1";

            configuration.ApiKey = Common.Common.GetAppSettingValue("TransferZeroApiKey");
            configuration.ApiSecret = Common.Common.GetAppSettingValue("TransferZeroApiSecret");
            configuration.BasePath = Common.Common.GetAppSettingValue("TransferZeroUrl");
            TransactionsApi api = new TransactionsApi(configuration);
            //Guid? guid = Guid.Parse(id);
            //TransactionResponse response = api.GetTransactionStatus(receiptNo);
            TransferZeroApi transferZeroApi = new TransferZeroApi();
            SSenderBankAccountDeposit senderBankAccountDepositServices = new SSenderBankAccountDeposit();
            var transaction = transferZeroApi.GetTransactionStatus(bankDeposit.ReceiptNo);

            try
            {
                if (transaction != null && transaction.Recipients != null)
                {
                    var recipientID = transaction.Recipients[0].Id;

                    var apiInstance = new RecipientsApi(configuration);
                    //var recipientID = new Guid?(); // Guid? | ID of recipient to cancel.  Example: `/v1/recipients/9d4d7b73-a94c-4979-ab57-09074fd55d33`
                    //recipientID = Guid.Parse(receiptId);
                    // Cancelling a recipient
                    RecipientResponse result = apiInstance.DeleteRecipient(recipientID);
                    //Debug.WriteLine(result);
                    var bankDetails = senderBankAccountDepositServices.GetBankDepositInfoByReceiptNo(bankDeposit.ReceiptNo);
                    //sent transaction cancelled Email
                    senderBankAccountDepositServices.TransactionCancelled(bankDetails);
                    return new ServiceResult<bool>()
                    {
                        Data = true,
                        Message = "Cancelled Successfully",
                        Status = ResultStatus.OK
                    };
                }
                else
                {
                    //cancelled transaction and sent transaction cancelled Emai
                    senderBankAccountDepositServices.CancelTransaction(bankDeposit.ReceiptNo);
                    return new ServiceResult<bool>()
                    {
                        Data = true,
                        Message = "Cancelled Successfully",
                        Status = ResultStatus.OK
                    };

                }
            }
            catch (ApiException e)
            {
                if (e.IsValidationError)
                {
                    // In case there was a validation error, obtain the object
                    RecipientResponse result = e.ParseObject<RecipientResponse>();

                }
                else
                {

                }

                return new ServiceResult<bool>() { Data = false, Message = "Cannot Cancel ", Status = ResultStatus.OK };
            }


        }
        internal ServiceResult<bool> CancelCashPickUPTransaction(int transactionId)
        {
            var cashPickUp = dbContext.FaxingNonCardTransaction.Where(x => x.Id == transactionId).FirstOrDefault();

            SSenderCashPickUp _senderCashpickUpServices = new SSenderCashPickUp();
            switch (cashPickUp.Apiservice)
            {
                case Apiservice.TransferZero:
                    TransferZero.Sdk.Client.Configuration configuration = new TransferZero.Sdk.Client.Configuration();

                    configuration.ApiKey = Common.Common.GetAppSettingValue("TransferZeroApiKey");
                    configuration.ApiSecret = Common.Common.GetAppSettingValue("TransferZeroApiSecret");
                    configuration.BasePath = Common.Common.GetAppSettingValue("TransferZeroUrl");
                    TransactionsApi api = new TransactionsApi(configuration);

                    TransferZeroApi transferZeroApi = new TransferZeroApi();
                    var transaction = transferZeroApi.GetTransactionStatus(cashPickUp.ReceiptNumber);
                    try
                    {
                        if (transaction != null && transaction.Recipients != null)
                        {
                            var recipientID = transaction.Recipients[0].Id;
                            var apiInstance = new RecipientsApi(configuration);
                            RecipientResponse result = apiInstance.DeleteRecipient(recipientID);

                            //sent transaction cancelled Email
                            _senderCashpickUpServices.TransactionCancelled(cashPickUp);

                            return new ServiceResult<bool>()
                            {
                                Data = true,
                                Message = "Cancelled Successfully",
                                Status = ResultStatus.OK
                            };
                        }
                        else
                        {
                            //cancelled transaction 
                            cashPickUp.FaxingStatus = FaxingStatus.Cancel;
                            dbContext.Entry<FaxingNonCardTransaction>(cashPickUp).State = EntityState.Modified;
                            dbContext.SaveChanges();
                            //send transaction cancelled Email
                            _senderCashpickUpServices.TransactionCancelledByMoneyFex(cashPickUp);

                            return new ServiceResult<bool>()
                            {
                                Data = true,
                                Message = "Cancelled Successfully",
                                Status = ResultStatus.OK
                            };
                        }
                    }
                    catch (ApiException e)
                    {
                        if (e.IsValidationError)
                        {
                            // In case there was a validation error, obtain the object
                            RecipientResponse result = e.ParseObject<RecipientResponse>();
                        }
                        else
                        {
                        }
                        return new ServiceResult<bool>() { Data = false, Message = "Cannot Cancel ", Status = ResultStatus.OK };

                    }
                    break;
                case Apiservice.Wari:
                    SSenderCashPickUp sSenderCashPickUp = new SSenderCashPickUp();
                    var responseResult = dbContext.CashPickUpTransactionResponseResult.Where(x => x.partnerTransactionReference == cashPickUp.ReceiptNumber).FirstOrDefault();
                    if (responseResult != null)
                    {
                        string transactionReference = responseResult.transactionReference;
                        var status = sSenderCashPickUp.CancelWariTransaction(transactionReference);
                        if (status == FaxingStatus.Cancel)
                        {
                            cashPickUp.FaxingStatus = status;
                            dbContext.Entry<FaxingNonCardTransaction>(cashPickUp).State = EntityState.Modified;
                            dbContext.SaveChanges();
                            //sent transaction cancelled Email
                            _senderCashpickUpServices.TransactionCancelled(cashPickUp);
                            return new ServiceResult<bool>()
                            {
                                Data = true,
                                Message = "Cancelled Successfully",
                                Status = ResultStatus.OK
                            };
                        }
                        else
                        {
                            return new ServiceResult<bool>() { Data = false, Message = "Cannot Cancel ", Status = ResultStatus.OK };
                        }
                    }
                    else
                    {
                        return new ServiceResult<bool>() { Data = false, Message = "Cannot Cancel ", Status = ResultStatus.OK };
                    }
                    break;
                default:
                    cashPickUp.FaxingStatus = FaxingStatus.Cancel;
                    dbContext.Entry<FaxingNonCardTransaction>(cashPickUp).State = EntityState.Modified;
                    dbContext.SaveChanges();
                    _senderCashpickUpServices.TransactionCancelledByMoneyFex(cashPickUp);
                    return new ServiceResult<bool>()
                    {
                        Data = true,
                        Message = "Cancelled Successfully",
                        Status = ResultStatus.OK
                    };
                    break;
            }

        }
        internal ServiceResult<bool> CancelMobileWalletTransaction(int transactionId)
        {
            var mobileWallet = dbContext.MobileMoneyTransfer.Where(x => x.Id == transactionId).FirstOrDefault();
            TransferZero.Sdk.Client.Configuration configuration = new TransferZero.Sdk.Client.Configuration();

            configuration.ApiKey = Common.Common.GetAppSettingValue("TransferZeroApiKey");
            configuration.ApiSecret = Common.Common.GetAppSettingValue("TransferZeroApiSecret");
            configuration.BasePath = Common.Common.GetAppSettingValue("TransferZeroUrl");
            TransactionsApi api = new TransactionsApi(configuration);

            TransferZeroApi transferZeroApi = new TransferZeroApi();
            SSenderMobileMoneyTransfer _senderMobileMoneyTransaferServices = new SSenderMobileMoneyTransfer();
            var transaction = transferZeroApi.GetTransactionStatus(mobileWallet.ReceiptNo);

            try
            {
                if (transaction != null && transaction.Recipients != null)
                {
                    var recipientID = transaction.Recipients[0].Id;

                    var apiInstance = new RecipientsApi(configuration);

                    RecipientResponse result = apiInstance.DeleteRecipient(recipientID);
                    //sent transaction cancelled Email
                    _senderMobileMoneyTransaferServices.TransactionCancelledEmail(mobileWallet);
                    return new ServiceResult<bool>()
                    {
                        Data = true,
                        Message = "Cancelled Successfully",
                        Status = ResultStatus.OK
                    };
                }
                else
                {
                    mobileWallet.Status = MobileMoneyTransferStatus.Cancel;
                    dbContext.Entry<MobileMoneyTransfer>(mobileWallet).State = EntityState.Modified;
                    dbContext.SaveChanges();
                    //sent transaction cancelled Email
                    _senderMobileMoneyTransaferServices.TransactionCancelledByMoneyFexEmail(mobileWallet);
                    return new ServiceResult<bool>()
                    {
                        Data = true,
                        Message = "Cancelled Successfully",
                        Status = ResultStatus.OK
                    };

                }
            }
            catch (ApiException e)
            {
                if (e.IsValidationError)
                {
                    // In case there was a validation error, obtain the object
                    RecipientResponse result = e.ParseObject<RecipientResponse>();

                }
                else
                {

                }

                return new ServiceResult<bool>() { Data = false, Message = "Cannot Cancel ", Status = ResultStatus.OK };
            }


        }

        internal ServiceResult<bool> ApproveTransaction(int Id, TransactionServiceType method)
        {

            var result = new ServiceResult<bool>();
            var exchangeRateType = ExchangeRateType.TransactionExchangeRate;
            switch ((TransactionServiceType)method)
            {
                case TransactionServiceType.All:
                    break;
                case TransactionServiceType.MobileWallet:
                    SSenderMobileMoneyTransfer senderMobileMoneyTransferServices = new SSenderMobileMoneyTransfer();
                    var data = senderMobileMoneyTransferServices.list().Data.Where(x => x.Id == Id).FirstOrDefault();

                    if (data.Status == DB.MobileMoneyTransferStatus.IdCheckInProgress)
                    {
                        result.Data = false;
                        result.Status = ResultStatus.Error;
                        result.Message = "Approve the identification document process first";
                        //return Json(new
                        //{
                        //    result
                        //}, JsonRequestBehavior.AllowGet);
                        return result;
                    }
                    try
                    {
                        exchangeRateType = Common.Common.SystemExchangeRateType(data.SendingCountry, data.ReceivingCountry, TransactionTransferMethod.OtherWallet);
                    }
                    catch (Exception)
                    {
                    }

                    if (exchangeRateType == ExchangeRateType.CurrentExchangeRate)
                    {

                        var newExchange = Common.Common.GetModifedAmount(data.SendingCountry, data.ReceivingCountry,
                            TransactionTransferMethod.OtherWallet, data.SendingAmount, TransactionTransferType.Admin);
                        data.ReceivingAmount = newExchange.ReceivingAmount;
                        data.ExchangeRate = newExchange.ExchangeRate;
                        data.Fee = newExchange.Fee;
                        data.TotalAmount = newExchange.TotalAmount;

                    }
                    // senderMobileMoneyTransferServices.CreateTransactionToApi(data);
                    var MobileMoneyTransactionResult = new MTNCameroonResponseParamVm();
                    SSenderMobileMoneyTransfer _senderMobileMoneyTransferServices = new SSenderMobileMoneyTransfer();
                    if (data.IsComplianceApproved == false)
                    {
                        data.IsComplianceApproved = true;
                        data.ComplianceApprovedBy = StaffSession.LoggedStaff.StaffId;
                        data.ComplianceApprovedDate = DateTime.Now;
                        var transferApiResponse = _senderMobileMoneyTransferServices.CreateTransactionToApi(data, TransactionTransferType.Online);
                        Log.Write(data.ReceiptNo + "Mobile Transaction Api Success ");
                        data.Status = transferApiResponse.status;
                        MobileMoneyTransactionResult = transferApiResponse.response;
                        senderMobileMoneyTransferServices.Update(data);

                        SMobileMoneyTransferResopnseStatus _sMobileMoneyResposeStatus = new SMobileMoneyTransferResopnseStatus();
                        _sMobileMoneyResposeStatus.AddLog(MobileMoneyTransactionResult, data.Id);
                        Log.Write(data.ReceiptNo + "Mobile Transaction Api Successful ");
                        _senderMobileMoneyTransferServices.SendEmailAndSms(data);
                    }
                    break;
                case TransactionServiceType.KiiPayWallet:
                    break;
                case TransactionServiceType.BillPayment:
                    break;
                case TransactionServiceType.ServicePayment:
                    break;
                case TransactionServiceType.CashPickUp:
                    SSenderCashPickUp senderCashPickUpServices = new SSenderCashPickUp();
                    var cashTrans = senderCashPickUpServices.List().Data.Where(x => x.Id == Id).FirstOrDefault();
                    if (cashTrans.FaxingStatus == FaxingStatus.IdCheckInProgress)
                    {

                        result.Data = false;
                        result.Status = ResultStatus.Error;
                        result.Message = "Approve the identification document process first";
                        return result;
                    }
                    try
                    {
                        exchangeRateType = Common.Common.SystemExchangeRateType(cashTrans.SendingCountry, cashTrans.ReceivingCountry, TransactionTransferMethod.CashPickUp);

                    }
                    catch (Exception)
                    {

                    }

                    if (exchangeRateType == ExchangeRateType.CurrentExchangeRate)
                    {

                        var newExchange = Common.Common.GetModifedAmount(cashTrans.SendingCountry, cashTrans.ReceivingCountry,
                            TransactionTransferMethod.CashPickUp, cashTrans.FaxingAmount, TransactionTransferType.Admin);
                        cashTrans.ReceivingAmount = newExchange.ReceivingAmount;
                        cashTrans.ExchangeRate = newExchange.ExchangeRate;
                        cashTrans.FaxingFee = newExchange.Fee;
                        cashTrans.TotalAmount = newExchange.TotalAmount;

                    }
                    cashTrans.FaxingStatus = FaxingStatus.NotReceived;
                    cashTrans.IsComplianceApproved = true;
                    cashTrans.ComplianceApprovedBy = StaffSession.LoggedStaff.StaffId;
                    cashTrans.ComplianceApprovedDate = DateTime.Now;
                    senderCashPickUpServices.Update(cashTrans);
                    break;
                case TransactionServiceType.BankDeposit:
                    SSenderBankAccountDeposit senderBankAccountDepositServices = new SSenderBankAccountDeposit();

                    var item = senderBankAccountDepositServices.List().Data.Where(x => x.Id == Id).FirstOrDefault();
                    var bankdepositTransactionResult = new BankDepositResponseVm();

                    if (item.Status == DB.BankDepositStatus.IdCheckInProgress)
                    {
                        result.Data = false;
                        result.Status = ResultStatus.Error;
                        result.Message = "Approve the identification document process first";
                        return result;
                    }
                    try
                    {

                        exchangeRateType = Common.Common.SystemExchangeRateType(item.SendingCountry, item.ReceivingCountry, TransactionTransferMethod.BankDeposit);

                    }
                    catch (Exception)
                    {

                    }

                    if (exchangeRateType == ExchangeRateType.CurrentExchangeRate)
                    {

                        var newExchange = Common.Common.GetModifedAmount(item.SendingCountry, item.ReceivingCountry,
                            TransactionTransferMethod.BankDeposit, item.SendingAmount, TransactionTransferType.Admin);
                        item.ReceivingAmount = newExchange.ReceivingAmount;
                        item.ExchangeRate = newExchange.ExchangeRate;
                        item.Fee = newExchange.Fee;
                        item.TotalAmount = newExchange.TotalAmount;

                    }
                    SSenderBankAccountDeposit senderBankAccountDeposit = new SSenderBankAccountDeposit();
                    if (item.IsComplianceApproved == false)
                    {
                        item.IsComplianceApproved = true;
                        item.ComplianceApprovedBy = StaffSession.LoggedStaff.StaffId;
                        item.ComplianceApprovedDate = DateTime.Now;

                        var transResponse = senderBankAccountDeposit.CreateBankTransactionToApi(item);
                        item.Status = transResponse.BankAccountDeposit.Status;
                        bankdepositTransactionResult = transResponse.BankDepositApiResponseVm;
                        senderBankAccountDepositServices.Update(item);
                        SBankDepositResponseStatus sBankDepositResponseStatus = new SBankDepositResponseStatus();
                        sBankDepositResponseStatus.AddLog(bankdepositTransactionResult, item.Id);
                        senderBankAccountDeposit.SendEmailAndSms(item);
                    }
                    break;
                default:
                    break;
            }
            result.Data = true;
            result.Message = "Approve Successfully";
            result.Status = ResultStatus.OK;
            return result;
        }


        internal RefundTransactionViewModel GetTransactionDetailsForRefund(int transactionId, TransactionServiceType transactionServiceType)
        {
            RefundTransactionViewModel result = new RefundTransactionViewModel();
            switch (transactionServiceType)
            {
                case TransactionServiceType.BankDeposit:
                    var bankAccount = dbContext.BankAccountDeposit.Where(x => x.Id == transactionId).FirstOrDefault();
                    result.ReceiptNo = bankAccount.ReceiptNo;
                    result.ReceiverName = bankAccount.ReceiverName;
                    result.RefundingAmount = bankAccount.TotalAmount;
                    break;
                case TransactionServiceType.CashPickUp:
                    var cashPickUp = dbContext.FaxingNonCardTransaction.Where(x => x.Id == transactionId).FirstOrDefault();
                    result.ReceiptNo = cashPickUp.ReceiptNumber;
                    result.ReceiverName = cashPickUp.NonCardReciever.FullName;
                    result.RefundingAmount = cashPickUp.TotalAmount;
                    break;
                case TransactionServiceType.MobileWallet:
                    var otherWallet = dbContext.MobileMoneyTransfer.Where(x => x.Id == transactionId).FirstOrDefault();
                    result.ReceiptNo = otherWallet.ReceiptNo;
                    result.ReceiverName = otherWallet.ReceiverName;
                    result.RefundingAmount = otherWallet.TotalAmount;
                    break;
            }
            var reintitalizedTransaction = dbContext.ReinitializeTransaction.Where(x => x.NewReceiptNo == result.ReceiptNo).FirstOrDefault();
            if (reintitalizedTransaction != null)
            {
                result.ReceiptNo = reintitalizedTransaction.ReceiptNo;
            }
            return result;
        }

        public TransactionReInitializationReponseVm ResetTransaction(int transactionId, int CreatedById = 0, string CreatedName = "",
            TransactionServiceType transactionServiceType = TransactionServiceType.All)
        {
            TransactionReInitializationReponseVm vm = new TransactionReInitializationReponseVm();
            switch (transactionServiceType)
            {
                case TransactionServiceType.BankDeposit:
                    vm = ResetBankAccountDeposit(transactionId, CreatedById, CreatedName);
                    break;
                case TransactionServiceType.CashPickUp:
                    vm = ResetCashPickUp(transactionId, CreatedById, CreatedName);
                    break;
                case TransactionServiceType.MobileWallet:
                    vm = ResetMobileWallet(transactionId, CreatedById, CreatedName);
                    break;
            }
            return vm;
        }

        internal TransactionReInitializationReponseVm ResetMobileWallet(int transactionId, int createdById, string createdName)
        {
            SSenderMobileMoneyTransfer sSenderMobileMoneyTransfer = new SSenderMobileMoneyTransfer();
            SSenderBankAccountDeposit senderBankAccountDepositServices = new SSenderBankAccountDeposit();

            var mobileWallet = dbContext.MobileMoneyTransfer.Where(x => x.Id == transactionId).FirstOrDefault();

            var NewReceiptNo = "";
            string receiptNo = mobileWallet.ReceiptNo;



            TransferZero.Sdk.Client.Configuration configuration = new TransferZero.Sdk.Client.Configuration();
            configuration.ApiKey = Common.Common.GetAppSettingValue("TransferZeroApiKey");
            configuration.ApiSecret = Common.Common.GetAppSettingValue("TransferZeroApiSecret");
            configuration.BasePath = Common.Common.GetAppSettingValue("TransferZeroUrl");
            TransactionsApi api = new TransactionsApi(configuration);
            TransferZeroApi transferZeroApi = new TransferZeroApi();

            var transaction = transferZeroApi.GetTransactionStatus(receiptNo);

            try
            {

                if (string.IsNullOrEmpty(transaction.ExternalId) == false && transaction.Recipients[0].TransactionState == TransactionState.Refunded)
                {
                    var bankDetails = senderBankAccountDepositServices.GetBankDepositInfoByReceiptNo(receiptNo);

                    NewReceiptNo = Common.Common.GenerateMobileMoneyTransferReceiptNo(6);
                    mobileWallet.ReceiptNo = NewReceiptNo;
                    mobileWallet.Status = MobileMoneyTransferStatus.InProgress;
                    mobileWallet.IsComplianceNeededForTrans = true;
                    mobileWallet.IsComplianceApproved = false;
                    sSenderMobileMoneyTransfer.Update(mobileWallet);
                    senderBankAccountDepositServices.AddReIntializedTrans(new DB.ReinitializeTransaction()
                    {
                        Date = DateTime.Now,
                        NewReceiptNo = NewReceiptNo,
                        ReceiptNo = receiptNo,
                        CreatedByName = createdName,
                        CreatedById = createdById
                    });

                    return new TransactionReInitializationReponseVm()
                    {
                        OldReceiptNo = receiptNo,
                        NewReceiptNo = NewReceiptNo,
                        Status = "ReInitiated"

                    };
                }
                if (transaction.Recipients != null)
                {
                    var recipientID = transaction.Recipients[0].Id;

                    var apiInstance = new RecipientsApi(configuration);
                    RecipientResponse result = apiInstance.DeleteRecipient(recipientID);

                    NewReceiptNo = Common.Common.GenerateMobileMoneyTransferReceiptNo(6);
                    mobileWallet.ReceiptNo = NewReceiptNo;
                    mobileWallet.Status = MobileMoneyTransferStatus.InProgress;
                    mobileWallet.IsComplianceNeededForTrans = true;
                    mobileWallet.IsComplianceApproved = false;
                    sSenderMobileMoneyTransfer.Update(mobileWallet);

                    senderBankAccountDepositServices.AddReIntializedTrans(new DB.ReinitializeTransaction()
                    {
                        Date = DateTime.Now,
                        NewReceiptNo = NewReceiptNo,
                        ReceiptNo = receiptNo
                    });

                    return new TransactionReInitializationReponseVm()
                    {
                        OldReceiptNo = receiptNo,
                        NewReceiptNo = NewReceiptNo,
                        Status = Enum.GetName(typeof(TransactionState), result.Object.TransactionState)

                    };
                }
                else
                {

                    mobileWallet.Status = MobileMoneyTransferStatus.Held;
                    mobileWallet.IsComplianceNeededForTrans = true;
                    mobileWallet.IsComplianceApproved = false;
                    sSenderMobileMoneyTransfer.Update(mobileWallet);

                    return new TransactionReInitializationReponseVm()
                    {

                    };
                }

            }
            catch (Exception ex)
            {

                return new TransactionReInitializationReponseVm();
            }

        }

        internal TransactionReInitializationReponseVm ResetCashPickUp(int transactionId, int createdById, string createdName)
        {
            SSenderCashPickUp sSenderCashPickUp = new SSenderCashPickUp();
            SSenderBankAccountDeposit senderBankAccountDepositServices = new SSenderBankAccountDeposit();
            var cashPickUp = dbContext.FaxingNonCardTransaction.Where(x => x.Id == transactionId).FirstOrDefault();

            var NewReceiptNo = "";
            string receiptNo = cashPickUp.ReceiptNumber;
            NewReceiptNo = Common.Common.GenerateCashPickUpReceiptNo(6);
            cashPickUp.ReceiptNumber = NewReceiptNo;
            cashPickUp.FaxingStatus = DB.FaxingStatus.Hold;
            cashPickUp.IsComplianceNeededForTrans = true;
            cashPickUp.IsComplianceApproved = false;

            dbContext.Entry<FaxingNonCardTransaction>(cashPickUp).State = EntityState.Modified;
            dbContext.SaveChanges();

            senderBankAccountDepositServices.AddReIntializedTrans(new DB.ReinitializeTransaction()
            {
                Date = DateTime.Now,
                NewReceiptNo = NewReceiptNo,
                ReceiptNo = receiptNo,
                CreatedByName = createdName,
                CreatedById = createdById
            });

            return new TransactionReInitializationReponseVm()
            {
                OldReceiptNo = receiptNo,
                NewReceiptNo = NewReceiptNo,
                Status = "ReInitiated"

            };
        }

        internal TransactionReInitializationReponseVm ResetBankAccountDeposit(int transactionId, int CreatedById = 0, string CreatedName = "")
        {

            var bankDeposit = dbContext.BankAccountDeposit.Where(x => x.Id == transactionId).FirstOrDefault();
            TransferZero.Sdk.Client.Configuration configuration = new TransferZero.Sdk.Client.Configuration();
            configuration.ApiKey = Common.Common.GetAppSettingValue("TransferZeroApiKey");
            configuration.ApiSecret = Common.Common.GetAppSettingValue("TransferZeroApiSecret");
            configuration.BasePath = Common.Common.GetAppSettingValue("TransferZeroUrl");
            TransactionsApi api = new TransactionsApi(configuration);
            TransferZeroApi transferZeroApi = new TransferZeroApi();
            SSenderBankAccountDeposit senderBankAccountDepositServices = new SSenderBankAccountDeposit();
            string receiptNo = bankDeposit.ReceiptNo;
            var transaction = transferZeroApi.GetTransactionStatus(receiptNo);

            var NewReceiptNo = "";
            try
            {

                if (string.IsNullOrEmpty(transaction.ExternalId) == false && transaction.Recipients[0].TransactionState == TransactionState.Refunded)
                {
                    var bankDetails = senderBankAccountDepositServices.GetBankDepositInfoByReceiptNo(receiptNo);

                    NewReceiptNo = Common.Common.GenerateBankAccountDepositReceiptNo(6);
                    bankDetails.ReceiptNo = NewReceiptNo;
                    bankDetails.Status = DB.BankDepositStatus.UnHold;
                    bankDetails.IsComplianceNeededForTrans = true;
                    bankDetails.IsComplianceApproved = false;
                    senderBankAccountDepositServices.Update(bankDetails);
                    senderBankAccountDepositServices.AddReIntializedTrans(new DB.ReinitializeTransaction()
                    {
                        Date = DateTime.Now,
                        NewReceiptNo = NewReceiptNo,
                        ReceiptNo = receiptNo,
                        CreatedByName = CreatedName,
                        CreatedById = CreatedById
                    });

                    return new TransactionReInitializationReponseVm()
                    {
                        OldReceiptNo = receiptNo,
                        NewReceiptNo = NewReceiptNo,
                        Status = "ReInitiated"

                    };
                }
                if (transaction.Recipients != null)
                {
                    var recipientID = transaction.Recipients[0].Id;

                    var apiInstance = new RecipientsApi(configuration);
                    //var recipientID = new Guid?(); // Guid? | ID of recipient to cancel.  Example: `/v1/recipients/9d4d7b73-a94c-4979-ab57-09074fd55d33`
                    //recipientID = Guid.Parse(receiptId);
                    // Cancelling a recipient
                    RecipientResponse result = apiInstance.DeleteRecipient(recipientID);
                    //Debug.WriteLine(result);
                    var bankDetails = senderBankAccountDepositServices.GetBankDepositInfoByReceiptNo(receiptNo);

                    NewReceiptNo = Common.Common.GenerateBankAccountDepositReceiptNo(6);
                    bankDetails.ReceiptNo = NewReceiptNo;
                    bankDetails.Status = DB.BankDepositStatus.UnHold;
                    bankDetails.IsComplianceNeededForTrans = true;
                    bankDetails.IsComplianceApproved = false;
                    senderBankAccountDepositServices.Update(bankDetails);
                    senderBankAccountDepositServices.AddReIntializedTrans(new DB.ReinitializeTransaction()
                    {
                        Date = DateTime.Now,
                        NewReceiptNo = NewReceiptNo,
                        ReceiptNo = receiptNo
                    });

                    return new TransactionReInitializationReponseVm()
                    {
                        OldReceiptNo = receiptNo,
                        NewReceiptNo = NewReceiptNo,
                        Status = Enum.GetName(typeof(TransactionState), result.Object.TransactionState)

                    };
                    //sent transaction cancelled Email

                    //return Json(new { Data = NewReceiptNo, Message = "Successfully Set to Reinitialization Point" }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var bankDepositData = dbContext.BankAccountDeposit.Where(x => x.ReceiptNo == receiptNo).FirstOrDefault();

                    bankDepositData.Status = BankDepositStatus.Held;
                    bankDepositData.IsComplianceNeededForTrans = true;
                    bankDepositData.IsComplianceApproved = false;
                    dbContext.Entry<BankAccountDeposit>(bankDepositData).State = EntityState.Modified;
                    dbContext.SaveChanges();

                    //cancelled transaction and sent transaction cancelled Emai
                    //senderBankAccountDepositServices.CancelTransaction(receiptNo);
                    // return Json(new { Data = true, Message = "cancelled Successfully" }, JsonRequestBehavior.AllowGet);
                    return new TransactionReInitializationReponseVm()
                    {

                    };
                }

            }
            catch (Exception ex)
            {

                return new TransactionReInitializationReponseVm();
            }
        }

        internal static string GetCreditCardLastDigit(SenderPaymentMode senderPaymentMode, TransferType transferType, int id)
        {

            if (senderPaymentMode == SenderPaymentMode.CreditDebitCard || senderPaymentMode == SenderPaymentMode.SavedDebitCreditCard)
            {
                DB.FAXEREntities dbContext = new FAXEREntities();
                var transactionCard = dbContext.CardTopUpCreditDebitInformation.Where(x => x.TransferType == (int)transferType && x.CardTransactionId == id).FirstOrDefault();
                if (transactionCard != null)
                {

                    return Common.Common.GetEnumDescription(senderPaymentMode) + " " + transactionCard.CardNumber.Right(9);
                }
            }
            return Common.Common.GetEnumDescription(senderPaymentMode);
        }

        public bool KiiPayWalletPaymentUsingCreditDebitCard(TransactionSummaryVM vm)
        {
            var paymentsummary = vm.KiiPayTransferPaymentSummary;
            var senderAndReceiverInfo = vm.SenderAndReceiverDetail;
            var PaymentMethodAndAutoPaymentFrequencyInfo = vm.PaymentMethodAndAutoPaymentDetail;




            SFaxingTopUpCardTransaction service = new SFaxingTopUpCardTransaction();

            STopUpSomeoneElseCard TopUpSomeoneElseServices = new STopUpSomeoneElseCard();
            string ReceiptNumber = service.GetNewMFTCCardTopUpReceipt();


            DB.TopUpSomeoneElseCardTransaction obj = new DB.TopUpSomeoneElseCardTransaction()
            {
                KiiPayPersonalWalletId = senderAndReceiverInfo.ReceiverId,
                FaxerId = FaxerSession.LoggedUser.Id,
                FaxingAmount = paymentsummary.SendingAmount,
                RecievingAmount = paymentsummary.ReceivingAmount,
                ExchangeRate = paymentsummary.ExchangeRate,
                FaxingFee = paymentsummary.Fee,
                TotalAmount = paymentsummary.TotalAmount,
                ReceiptNumber = ReceiptNumber,
                TransactionDate = System.DateTime.Now,
                TopUpReference = paymentsummary.PaymentReference,
                PaymentMethod = "PM001",
                TransferToMobileNo = senderAndReceiverInfo.ReceiverMobileNo,
                SenderPaymentMode = vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode,
                PaymentType = vm.IsLocalPayment == true ? DB.PaymentType.Local : DB.PaymentType.International,
                SendingCountry = senderAndReceiverInfo.SenderCountry,
                ReceivingCountry = senderAndReceiverInfo.ReceiverCountry,
                StripeTokenId = vm.CreditORDebitCardDetials.StripeTokenID

            };
            obj = TopUpSomeoneElseServices.SaveTransaction(obj);

            if (vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == Models.SenderPaymentMode.CreditDebitCard
                && vm.CreditORDebitCardDetials.SaveCard == true)
            {

                SaveCreditDebitCard(vm.CreditORDebitCardDetials, vm.TransferType, obj.Id);
            }

            SavedCreditCardTransactionDetails(vm.CreditORDebitCardDetials, vm.TransferType, obj.Id);

            KiipayWalletStandingOrderPayment(vm);
            return true;
        }


        public void SaveCreditDebitCard(Models.CreditDebitCardViewModel model, TransferType transferType, int TransactionId)
        {

            int SavedCardCount = dbContext.SavedCard.Where(x => x.UserId == FaxerSession.LoggedUser.Id).Count();
            if (SavedCardCount < 2)
            {
                DB.SavedCard savedCardObject = new DB.SavedCard()
                {
                    Type = model.CreditDebitCardType,
                    CardName = model.NameOnCard.Encrypt(),
                    EMonth = model.EndMM.Encrypt(),
                    EYear = model.EndYY.Encrypt(),
                    CreatedDate = System.DateTime.Now,
                    UserId = FaxerSession.LoggedUser.Id,
                    Num = model.CardNumber.Encrypt(),
                    ClientCode = model.SecurityCode.Encrypt()

                };
                SSavedCard cardservices = new SSavedCard();
                savedCardObject = cardservices.Add(savedCardObject);

            }





        }

        public void SavedCreditCardTransactionDetails(Models.CreditDebitCardViewModel model, TransferType transferType, int TransactionId)
        {

            DB.CardTopUpCreditDebitInformation cardDetails = new DB.CardTopUpCreditDebitInformation()
            {
                CardTransactionId = TransactionId,
                NameOnCard = model.NameOnCard,
                ExpiryDate = model.EndMM + "/" + model.EndYY,
                CardNumber = "xxxx-xxxx-xxxx-" + model.CardNumber.Right(4),
                IsSavedCard = false,
                AutoRecharged = model.AutoTopUp,
                TransferType = (int)transferType,
                CreatedDate = DateTime.Now
            };
            SSavedCard cardInformationservices = new SSavedCard();
            cardDetails = cardInformationservices.Save(cardDetails);



        }

        public bool CompletePayForServicesTransaction(TransactionSummaryVM vm)
        {

            bool valid = true;
            Services.SFaxerSignUp faService = new Services.SFaxerSignUp();
            var faInformation = faService.GetInformation(FaxerSession.LoggedUser.UserName);
            DB.SenderKiiPayBusinessPaymentInformation obj = new DB.SenderKiiPayBusinessPaymentInformation();
            if (valid)
            {

                //transaction history object
                obj = new DB.SenderKiiPayBusinessPaymentInformation()
                {
                    KiiPayBusinessInformationId = vm.SenderAndReceiverDetail.ReceiverId,
                    SenderInformationId = FaxerSession.LoggedUser.Id,
                    PaymentAmount = vm.KiiPayTransferPaymentSummary.SendingAmount,
                    PaymentRefrence = vm.KiiPayTransferPaymentSummary.PaymentReference,
                };
                SFaxerMerchantPaymentInformation service = new SFaxerMerchantPaymentInformation();
                obj = service.PayBusinessMerchant(obj, obj.KiiPayBusinessInformationId);


                if (vm.PaymentMethodAndAutoPaymentDetail != null && (vm.PaymentMethodAndAutoPaymentDetail.EnableAutoPayment == true)
                    && vm.PaymentMethodAndAutoPaymentDetail.AutoPaymentAmount > 0 && (int)vm.PaymentMethodAndAutoPaymentDetail.AutopaymentFrequency > 0)
                {
                    var cardDetails = dbContext.SavedCard.Where(x => x.UserId == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
                    if (cardDetails != null)
                    {

                        var businessWalletInformation = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == obj.KiiPayBusinessInformationId).FirstOrDefault();
                        businessWalletInformation.AutoTopUp = true;
                        dbContext.Entry(businessWalletInformation).State = System.Data.Entity.EntityState.Modified;
                        var PaymentInformation = dbContext.FaxerMerchantPaymentInformation.Where(x => x.KiiPayBusinessInformationId == obj.KiiPayBusinessInformationId && x.SenderInformationId == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
                        PaymentInformation.AutoPaymentAmount = vm.PaymentMethodAndAutoPaymentDetail.AutoPaymentAmount;
                        PaymentInformation.AutoPaymentFrequency = vm.PaymentMethodAndAutoPaymentDetail.AutopaymentFrequency;
                        PaymentInformation.EnableAutoPayment = true;
                        PaymentInformation.FrequencyDetails = vm.PaymentMethodAndAutoPaymentDetail.PaymentDay;
                        PaymentInformation.PaymentRefrence = vm.KiiPayTransferPaymentSummary.PaymentReference;
                        dbContext.Entry(PaymentInformation).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();


                    }
                }

            }

            if (vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == Models.SenderPaymentMode.CreditDebitCard
                && vm.CreditORDebitCardDetials.SaveCard == true)
            {


                SaveCreditDebitCard(vm.CreditORDebitCardDetials, vm.TransferType, obj.Id);
            }


            if (vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == SenderPaymentMode.KiiPayWallet)
            {

                SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
                int senderWalletId = senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id).Id;
                KiiPayWalletBalOut(senderWalletId, vm.KiiPayTransferPaymentSummary.TotalAmount);
            }

            AddUpdateLog(vm.CreditORDebitCardDetials);
            return true;
        }
        public bool CompleteCashPickupTransaction(TransactionSummaryVM vm)
        {

            SSenderCashPickUp _cashPickUpServices = new SSenderCashPickUp();
            var senderAndReceiverDetials = vm.SenderAndReceiverDetail;
            var paymentInfo = vm.KiiPayTransferPaymentSummary;
            var CreditdebitCardDetials = vm.CreditORDebitCardDetials;
            var paymentMethod = vm.PaymentMethodAndAutoPaymentDetail;
            var cashPickUP = vm.CashPickUpVM;
            //transaction history object


            SFaxingNonCardTransaction service = new SFaxingNonCardTransaction();
            //get unique new mfcn 

            var receiptNumber = service.GetNewReceiptNumberToSave();
            Services.SReceiverDetails receiverService = new SReceiverDetails();

            string[] splittedName = cashPickUP.FullName.Trim().Split(null);

            DB.ReceiversDetails recDetailObj = new DB.ReceiversDetails()
            {
                City = "",
                CreatedDate = System.DateTime.Now,
                Country = cashPickUP.CountryCode,
                EmailAddress = cashPickUP.EmailAddress,
                FaxerID = FaxerSession.LoggedUser.Id,
                FullName = cashPickUP.FullName,
                IsDeleted = false,
                PhoneNumber = cashPickUP.MobileNumber,
                FirstName = splittedName[0],
                MiddleName = splittedName.Count() > 2 ? splittedName[1] : " ",
                LastName = splittedName[splittedName.Count() - 1]
            };
            int NonCardReceiveId;
            if (cashPickUP.Id == 0)
            {
                var recevierExist = dbContext.ReceiversDetails.Where(x => x.FullName == recDetailObj.FullName && x.PhoneNumber == recDetailObj.PhoneNumber).FirstOrDefault();
                if (recevierExist == null)
                {
                    receiverService.Add(recDetailObj);
                }
                NonCardReceiveId = recevierExist.Id;
                // Add City in city table 
                City newCity = new City()
                {
                    CountryCode = recDetailObj.Country,
                    Module = Module.Faxer,
                    Name = recDetailObj.City
                };
                SCity.Save(newCity);
                //End
            }
            else
            {
                NonCardReceiveId = int.Parse(FaxerSession.NonCardReceiversDetails.PreviousReceivers);
            }
            int RecipientId = 0;
            var Recipent = dbContext.Recipients.Where(x => x.ReceiverName.ToLower() == cashPickUP.FullName.ToLower() && x.MobileNo == cashPickUP.MobileNumber
            && x.Service == Service.CashPickUP).FirstOrDefault();
            if (Recipent == null)
            {
                Recipients model = new Recipients()
                {
                    Country = cashPickUP.CountryCode,
                    SenderId = FaxerSession.LoggedUser.Id,
                    MobileNo = cashPickUP.MobileNumber,
                    Service = Service.CashPickUP,
                    ReceiverName = cashPickUP.FullName,
                    IdentificationNumber = cashPickUP.IdentityCardNumber,
                    IdentificationTypeId = cashPickUP.IdenityCardId
                };
                var AddRecipient = dbContext.Recipients.Add(model);
                dbContext.SaveChanges();
                RecipientId = AddRecipient.Id;
            }
            else
            {
                RecipientId = Recipent.Id;

            }

            int CashPickUpId = Common.FaxerSession.TransactionId;

            var CashPickUpDetails = dbContext.FaxingNonCardTransaction.Where(x => x.Id == CashPickUpId).FirstOrDefault();

            decimal ExtraFee = 0;
            switch (paymentMethod.SenderPaymentMode)
            {
                case SenderPaymentMode.CreditDebitCard:
                    ExtraFee = vm.CreditORDebitCardDetials.CreditDebitCardFee;
                    CashPickUpDetails.CardProcessorApi = vm.CardProcessorApi;
                    break;
                case SenderPaymentMode.SavedDebitCreditCard:
                    ExtraFee = vm.CreditORDebitCardDetials.CreditDebitCardFee;
                    CashPickUpDetails.CardProcessorApi = vm.CardProcessorApi;
                    break;
                case SenderPaymentMode.KiiPayWallet:
                    break;
                case SenderPaymentMode.MoneyFexBankAccount:
                    ExtraFee = vm.MoneyFexBankDeposit.BankFee;
                    break;
                case SenderPaymentMode.Cash:
                    break;
                default:
                    break;
            }

            var ApiService = Common.Common.GetApiservice(senderAndReceiverDetials.SenderCountry,
             senderAndReceiverDetials.ReceiverCountry, paymentInfo.SendingAmount, TransactionTransferMethod.CashPickUp, TransactionTransferType.Online);
            CashPickUpDetails.stripe_ChargeId = CreditdebitCardDetials.StripeTokenID;
            CashPickUpDetails.FaxingStatus = vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == SenderPaymentMode.MoneyFexBankAccount
                 ? FaxingStatus.PendingBankdepositConfirmtaion :
                 vm.IsIdCheckInProgress == true ?
                FaxingStatus.IdCheckInProgress : FaxingStatus.NotReceived;
            CashPickUpDetails.FaxingMethod = "PM001";
            CashPickUpDetails.FaxingAmount = paymentInfo.SendingAmount;
            CashPickUpDetails.ReceivingAmount = paymentInfo.ReceivingAmount;
            CashPickUpDetails.ExchangeRate = paymentInfo.ExchangeRate;
            CashPickUpDetails.FaxingFee = paymentInfo.Fee;
            CashPickUpDetails.TotalAmount = paymentInfo.TotalAmount + ExtraFee;
            CashPickUpDetails.ExtraFee = ExtraFee;
            CashPickUpDetails.TransactionDate = System.DateTime.Now;
            CashPickUpDetails.SenderPaymentMode = paymentMethod.SenderPaymentMode;
            CashPickUpDetails.SendingCountry = senderAndReceiverDetials.SenderCountry;
            CashPickUpDetails.ReceivingCountry = senderAndReceiverDetials.ReceiverCountry;
            CashPickUpDetails.RecipientId = RecipientId;
            CashPickUpDetails.NonCardRecieverId = NonCardReceiveId;
            CashPickUpDetails.PaymentReference = _cashPickUpServices.GetMoneyFexBankAccountDeposit().PaymentReference;
            CashPickUpDetails.TransferReference = "";
            CashPickUpDetails.RecipientIdentityCardId = cashPickUP.IdenityCardId;
            CashPickUpDetails.RecipientIdenityCardNumber = cashPickUP.IdentityCardNumber;
            CashPickUpDetails.Reason = cashPickUP.Reason;
            CashPickUpDetails.Apiservice = ApiService;
            CashPickUpDetails.SendingCurrency = paymentInfo.SendingCurrency;
            CashPickUpDetails.ReceivingCurrency = paymentInfo.ReceivingCurrency;


            //if (CashPickUpDetails.SenderPaymentMode == SenderPaymentMode.CreditDebitCard ||
            //    CashPickUpDetails.SenderPaymentMode == SenderPaymentMode.SavedDebitCreditCard)
            //{
            //    var TransactionLimitAmount = Common.Common.GetAppSettingValue("TransactionLimit");
            //    if (CashPickUpDetails.FaxingAmount > decimal.Parse(TransactionLimitAmount))
            //    {
            //        CashPickUpDetails.IsComplianceNeededForTrans = true;
            //        CashPickUpDetails.FaxingStatus = FaxingStatus.Hold;
            //    }

            //    var IsPayoutFlowControlEnabled = Common.Common.IsPayoutFlowControlEnabled(CashPickUpDetails.SendingCountry, CashPickUpDetails.ReceivingCountry,
            //           null, TransactionTransferMethod.CashPickUp, 0);
            //    if (IsPayoutFlowControlEnabled == false)
            //    {
            //        CashPickUpDetails.FaxingStatus = FaxingStatus.Paused;

            //    }
            //}

            #region API Call

            if (paymentMethod.SenderPaymentMode == SenderPaymentMode.CreditDebitCard
                || paymentMethod.SenderPaymentMode == SenderPaymentMode.SavedDebitCreditCard)
            {
                BankDepositResponseVm cashPickUpTransactionResult = new BankDepositResponseVm();
                if (vm.IsIdCheckInProgress == false)
                {

                    var transResponse = _cashPickUpServices.CreateCashPickTransactionToApi(CashPickUpDetails);
                    CashPickUpDetails.FaxingStatus = transResponse.CashPickUp.FaxingStatus;
                    CashPickUpDetails.TransferReference = transResponse.CashPickUp.TransferReference;
                    cashPickUpTransactionResult = transResponse.BankDepositApiResponseVm;
                    _cashPickUpServices.AddResponseLog(cashPickUpTransactionResult, CashPickUpDetails.Id);

                }
            }

            #endregion



            dbContext.Entry<FaxingNonCardTransaction>(CashPickUpDetails).State = EntityState.Modified;
            dbContext.SaveChanges();

            //save transaction for non card
            //service.UpdateTransaction(CashPickUpDetails);
            if (vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == Models.SenderPaymentMode.CreditDebitCard
             && vm.CreditORDebitCardDetials.SaveCard == true)
            {
                SaveCreditDebitCard(vm.CreditORDebitCardDetials, vm.TransferType, CashPickUpId);
            }
            if (CashPickUpDetails.SenderPaymentMode == SenderPaymentMode.KiiPayWallet)
            {
                SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
                int WalletId = senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id).Id;
                KiiPayWalletBalOut(WalletId, CashPickUpDetails.TotalAmount);
            }
            if (vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == SenderPaymentMode.SavedDebitCreditCard ||
               vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == SenderPaymentMode.CreditDebitCard)
            {
                AddUpdateLog(vm.CreditORDebitCardDetials);
                SavedCreditCardTransactionDetails(vm.CreditORDebitCardDetials, vm.TransferType, CashPickUpId);

                //AddCreditInfoLog(vm.CreditORDebitCardDetials, obj.Id);
            }
            #region SMS
            //sender
            //string senderName = Common.FaxerSession.LoggedUser.FullName;
            //string fullName = senderName;
            //var names = fullName.Split(' ');
            //string senderFirstName = names[0];

            //var receiverFullName = cashPickUP.FullName.Trim().Split(' ');
            //string reciverFirstName = receiverFullName[0];

            //string senderPhoneNo = Common.FaxerSession.LoggedUser.CountryPhoneCode + Common.FaxerSession.LoggedUser.PhoneNo;
            //string SendingAmountWithCurrencySymbol = Common.Common.GetCurrencySymbol(CashPickUpDetails.SendingCountry) + " " + CashPickUpDetails.FaxingAmount;
            //string ReceivingAmountWithCurrencySymbol = Common.Common.GetCountryCurrency(CashPickUpDetails.ReceivingCountry) + " " + CashPickUpDetails.ReceivingAmount;
            //string FeeAmountWithCurrencySymbol = Common.Common.GetCountryCurrency(CashPickUpDetails.SendingCountry) + " " + CashPickUpDetails.FaxingFee;

            //SendCashPickUpTOSenderSms(senderFirstName, reciverFirstName, SendingAmountWithCurrencySymbol, senderPhoneNo);

            ////Receiver
            //string SendingCountry = Common.Common.GetCountryName(CashPickUpDetails.SendingCountry);
            //SendCashPickUpToReceiverSms(SendingCountry, cashPickUP.MobileNumber);

            #endregion

            #region Email
            _cashPickUpServices.SendEmailAndSms(CashPickUpDetails);
            #endregion

            #region Notification Section 
            if (vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == SenderPaymentMode.MoneyFexBankAccount)
            {


                DB.Notification notification = new DB.Notification()
                {
                    SenderId = Common.FaxerSession.LoggedUser.Id,
                    ReceiverId = 0,
                    Amount = "",
                    CreationDate = DateTime.Now,
                    Message = "New manual bank deposit (" + _cashPickUpServices.GetMoneyFexBankAccountDeposit().PaymentReference + ")",
                    NotificationReceiver = DB.NotificationFor.Admin,
                    NotificationSender = DB.NotificationFor.Sender,
                    Name = cashPickUP.FullName,
                    NotificationKey = _cashPickUpServices.GetMoneyFexBankAccountDeposit().PaymentReference
                };

                SenderCommonServices senderCommonServices = new SenderCommonServices();
                senderCommonServices.SendNotificationToAdmin(notification);

                MoneyFexBankAccountLog MoneyFexBankAccount = new MoneyFexBankAccountLog()
                {
                    IsConfirmed = false,
                    TranscationId = CashPickUpId,
                    TrasnferMethod = TransactionTransferMethod.CashPickUp,
                    PaymentReference = _cashPickUpServices.GetMoneyFexBankAccountDeposit().PaymentReference
                };
                dbContext.MoneyFexBankAccountLog.Add(MoneyFexBankAccount);
                dbContext.SaveChanges();

            }

            #endregion

            return true;
        }



        public bool CompleteMobileTransferTransaction(TransactionSummaryVM vm)
        {
            SSenderMobileMoneyTransfer _mobileMoneyTransferServices = new SSenderMobileMoneyTransfer();
            var senderAndReceiverDetials = vm.SenderAndReceiverDetail;
            var paymentInfo = vm.KiiPayTransferPaymentSummary;
            var CreditdebitCardDetials = vm.CreditORDebitCardDetials;

            var paymentMethod = vm.PaymentMethodAndAutoPaymentDetail;

            var mobileTransfer = vm.MobileMoneyTransfer;

            var ApiService = Common.Common.GetApiservice(senderAndReceiverDetials.SenderCountry, senderAndReceiverDetials.ReceiverCountry,
           paymentInfo.SendingAmount, TransactionTransferMethod.OtherWallet, TransactionTransferType.Online);


            int RecipientId = 0;
            var Recipent = dbContext.Recipients.Where(x => x.ReceiverName.ToLower() == mobileTransfer.ReceiverName.ToLower() && x.MobileNo == mobileTransfer.MobileNumber
            && x.Service == Service.MobileWallet).FirstOrDefault();
            if (Recipent == null)
            {
                Recipients model = new Recipients()
                {
                    Country = mobileTransfer.CountryCode,

                    SenderId = FaxerSession.LoggedUser.Id,
                    MobileNo = mobileTransfer.MobileNumber,
                    Service = Service.MobileWallet,
                    ReceiverName = mobileTransfer.ReceiverName,
                    Email = mobileTransfer.ReceiverEmail
                };
                var AddRecipient = dbContext.Recipients.Add(model);
                dbContext.SaveChanges();
                RecipientId = AddRecipient.Id;
            }
            else
            {
                RecipientId = Recipent.Id;
            }

            int mobileTransferId = Common.FaxerSession.TransactionId;
            string receiptNo = Common.FaxerSession.ReceiptNo;

            //var MobileMoneyTransferDetails = dbContext.MobileMoneyTransfer.Where(x => x.Id == mobileTransferId).FirstOrDefault();
            var MobileMoneyTransferDetails = dbContext.MobileMoneyTransfer.Where(x => x.ReceiptNo == receiptNo).FirstOrDefault();

            if (MobileMoneyTransferDetails == null)
            {
                MobileMoneyTransferDetails = new MobileMoneyTransfer();
            }


            decimal ExtraFee = 0;
            switch (paymentMethod.SenderPaymentMode)
            {
                case SenderPaymentMode.CreditDebitCard:
                    ExtraFee = vm.CreditORDebitCardDetials.CreditDebitCardFee;
                    MobileMoneyTransferDetails.CardProcessorApi = vm.CardProcessorApi;
                    break;
                case SenderPaymentMode.SavedDebitCreditCard:
                    ExtraFee = vm.CreditORDebitCardDetials.CreditDebitCardFee;
                    MobileMoneyTransferDetails.CardProcessorApi = vm.CardProcessorApi;
                    break;
                case SenderPaymentMode.KiiPayWallet:
                    break;
                case SenderPaymentMode.MoneyFexBankAccount:
                    ExtraFee = vm.MoneyFexBankDeposit.BankFee;
                    break;
                case SenderPaymentMode.Cash:
                    break;
                default:
                    break;
            }
            MobileMoneyTransferDetails.ExchangeRate = paymentInfo.ExchangeRate;
            MobileMoneyTransferDetails.Fee = paymentInfo.Fee;
            MobileMoneyTransferDetails.PaidFromModule = Module.Faxer;
            MobileMoneyTransferDetails.TransactionDate = DateTime.Now;
            MobileMoneyTransferDetails.TotalAmount = paymentInfo.TotalAmount + ExtraFee;
            MobileMoneyTransferDetails.ExtraFee = ExtraFee;
            MobileMoneyTransferDetails.PaidToMobileNo = mobileTransfer.MobileNumber;
            MobileMoneyTransferDetails.PaymentReference = _mobileMoneyTransferServices.GetMoneyFexBankAccountDeposit().PaymentReference;
            MobileMoneyTransferDetails.SenderPaymentMode = paymentMethod.SenderPaymentMode;
            MobileMoneyTransferDetails.ReceivingAmount = paymentInfo.ReceivingAmount;
            MobileMoneyTransferDetails.SenderId = Common.FaxerSession.LoggedUser.Id;
            MobileMoneyTransferDetails.ReceivingCountry = mobileTransfer.CountryCode;
            MobileMoneyTransferDetails.SendingAmount = paymentInfo.SendingAmount;
            MobileMoneyTransferDetails.SendingCountry = Common.FaxerSession.LoggedUser.CountryCode;
            MobileMoneyTransferDetails.ReceiverName = mobileTransfer.ReceiverName;
            MobileMoneyTransferDetails.WalletOperatorId = vm.SenderAndReceiverDetail.WalletOperatorId;
            MobileMoneyTransferDetails.RecipientId = RecipientId;
            MobileMoneyTransferDetails.SendingCurrency = paymentInfo.SendingCurrency;
            MobileMoneyTransferDetails.ReceivingCurrency = paymentInfo.ReceivingCurrency;
            //MobileMoneyTransferDetails.Status = vm.IsIdCheckInProgress == true ? 
            //    MobileMoneyTransferStatus.IdCheckInProgress :
            //    paymentMethod.SenderPaymentMode == SenderPaymentMode.MoneyFexBankAccount ?
            //    MobileMoneyTransferStatus.PendingBankdepositConfirmtaion
            //    : MobileMoneyTransferStatus.InProgress;
            MobileMoneyTransferDetails.Status =
                paymentMethod.SenderPaymentMode == SenderPaymentMode.MoneyFexBankAccount ?
                MobileMoneyTransferStatus.PendingBankdepositConfirmtaion
                : vm.IsIdCheckInProgress == true ?
                MobileMoneyTransferStatus.IdCheckInProgress : MobileMoneyTransferStatus.InProgress;


            MobileMoneyTransferDetails.Apiservice = ApiService;
            if (MobileMoneyTransferDetails.SendingCountry == MobileMoneyTransferDetails.ReceivingCountry)
            {
                MobileMoneyTransferDetails.PaymentType = PaymentType.Local;
            }
            else
            {
                MobileMoneyTransferDetails.PaymentType = PaymentType.International;

            }
            var obj = new MobileMoneyTransfer();
            if (mobileTransferId == 0)
            {
                obj = _senderMobileMoneyTransfer.Add(MobileMoneyTransferDetails).Data;
                MobileMoneyTransferDetails.Id = obj.Id;
            }


            #region API Call
            string ReceiverPhoneCode = Common.Common.GetCountryPhoneCode(mobileTransfer.CountryCode);
            var MobileMoneyTransactionResult = new MTNCameroonResponseParamVm();


            if (paymentMethod.SenderPaymentMode == SenderPaymentMode.CreditDebitCard
                || paymentMethod.SenderPaymentMode == SenderPaymentMode.SavedDebitCreditCard)
            {
                if (vm.IsIdCheckInProgress == false)
                {
                    var transferApiResponse = _senderMobileMoneyTransfer.CreateTransactionToApi(MobileMoneyTransferDetails);
                    MobileMoneyTransferDetails.Status = transferApiResponse.status;
                    MobileMoneyTransferDetails.TransferReference = transferApiResponse.response.refId;

                    SMobileMoneyTransferResopnseStatus _sMobileMoneyResposeStatus = new SMobileMoneyTransferResopnseStatus();
                    _sMobileMoneyResposeStatus.AddLog(transferApiResponse.response, mobileTransferId);

                }
            }

            #endregion
            // Update status of transfer
            obj = _senderMobileMoneyTransfer.Update(MobileMoneyTransferDetails).Data;


            if (vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == Models.SenderPaymentMode.CreditDebitCard
             && vm.CreditORDebitCardDetials.SaveCard == true)
            {
                vm.TransferType = TransferType.MobileTransfer;
                SaveCreditDebitCard(vm.CreditORDebitCardDetials, vm.TransferType, obj.Id);
            }

            if (vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == SenderPaymentMode.KiiPayWallet)
            {
                SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
                int WalletId = senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id).Id;
                KiiPayWalletBalOut(WalletId, vm.KiiPayTransferPaymentSummary.TotalAmount);
            }

            if (vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == SenderPaymentMode.SavedDebitCreditCard ||
               vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == SenderPaymentMode.CreditDebitCard)
            {
                SavedCreditCardTransactionDetails(vm.CreditORDebitCardDetials, vm.TransferType, obj.Id);
                AddUpdateLog(vm.CreditORDebitCardDetials);
            }
            //AddCreditInfoLog(vm.CreditORDebitCardDetials, obj.Id);

            #region Send Sms and email

            _senderMobileMoneyTransfer.SendEmailAndSms(obj);
            #endregion

            #region Notification Section 
            if (paymentMethod.SenderPaymentMode == SenderPaymentMode.MoneyFexBankAccount)
            {


                var model = _mobileMoneyTransferServices.GetMoneyFexBankAccountDeposit();
                DB.Notification notification = new DB.Notification()
                {
                    SenderId = MobileMoneyTransferDetails.SenderId,
                    ReceiverId = 0,
                    Amount = "",
                    CreationDate = DateTime.Now,
                    Message = "New manual bank deposit (" + model.PaymentReference + ")",
                    NotificationReceiver = DB.NotificationFor.Admin,
                    NotificationSender = DB.NotificationFor.Sender,
                    Name = MobileMoneyTransferDetails.ReceiverName,
                    NotificationKey = model.PaymentReference
                };

                SenderCommonServices senderCommonServices = new SenderCommonServices();
                senderCommonServices.SendNotificationToAdmin(notification);

                MoneyFexBankAccountLog MoneyFexBankAccount = new MoneyFexBankAccountLog()
                {
                    IsConfirmed = false,
                    TranscationId = obj.Id,
                    TrasnferMethod = TransactionTransferMethod.OtherWallet,
                    PaymentReference = model.PaymentReference
                };
                dbContext.MoneyFexBankAccountLog.Add(MoneyFexBankAccount);
                dbContext.SaveChanges();

            }
            #endregion


            return true;
        }

        public bool CompleteBankDepositTransaction(TransactionSummaryVM vm)
        {
            SSenderBankAccountDeposit _senderBankAccountDepositServices = new SSenderBankAccountDeposit();


            var senderAndReceiverDetials = vm.SenderAndReceiverDetail;
            var paymentInfo = vm.KiiPayTransferPaymentSummary;
            var CreditdebitCardDetials = vm.CreditORDebitCardDetials;

            var paymentMethod = vm.PaymentMethodAndAutoPaymentDetail;

            var BankDeposit = vm.BankAccountDeposit;

            var Recipent = dbContext.Recipients.Where(x => x.ReceiverName.ToLower() == BankDeposit.AccountOwnerName.ToLower()
            && x.AccountNo == BankDeposit.AccountNumber
           && x.Service == Service.BankAccount).FirstOrDefault();

            int RecipientId = 0;
            if (Recipent == null)
            {
                Recipients model = new Recipients()
                {
                    Country = BankDeposit.CountryCode,
                    SenderId = FaxerSession.LoggedUser.Id,
                    MobileNo = BankDeposit.MobileNumber,
                    Service = Service.BankAccount,
                    ReceiverName = BankDeposit.AccountOwnerName,
                    BankId = BankDeposit.BankId,
                    BranchCode = BankDeposit.BranchCode,
                    AccountNo = BankDeposit.AccountNumber,
                    IBusiness = BankDeposit.IsBusiness,
                    City = BankDeposit.ReceiverCity,
                    Email = BankDeposit.ReceiverEmail,
                    PostalCode = BankDeposit.ReceiverPostalCode,
                    Street = BankDeposit.ReceiverStreet
                };
                var AddRecipient = dbContext.Recipients.Add(model);
                dbContext.SaveChanges();
                RecipientId = AddRecipient.Id;
            }
            else
            {
                RecipientId = Recipent.Id;
            }

            var ApiService = Common.Common.GetApiservice(senderAndReceiverDetials.SenderCountry,
             senderAndReceiverDetials.ReceiverCountry, paymentInfo.SendingAmount, TransactionTransferMethod.BankDeposit, TransactionTransferType.Online);

            int BankAccountDepositId = Common.FaxerSession.TransactionId;

            string receiptNo = Common.FaxerSession.ReceiptNo;
            //var BankAccountDeposit = dbContext.BankAccountDeposit.Where(x => x.Id == BankAccountDepositId).FirstOrDefault();
            var BankAccountDeposit = dbContext.BankAccountDeposit.Where(x => x.ReceiptNo == receiptNo).FirstOrDefault();
            if (BankAccountDeposit == null)
            {

                BankAccountDeposit = new BankAccountDeposit();
            }
            decimal ExtraFee = 0;
            switch (paymentMethod.SenderPaymentMode)
            {
                case SenderPaymentMode.CreditDebitCard:
                    ExtraFee = vm.CreditORDebitCardDetials.CreditDebitCardFee;
                    BankAccountDeposit.CardProcessorApi = vm.CardProcessorApi;
                    break;
                case SenderPaymentMode.SavedDebitCreditCard:
                    ExtraFee = vm.CreditORDebitCardDetials.CreditDebitCardFee;
                    BankAccountDeposit.CardProcessorApi = vm.CardProcessorApi;
                    break;
                case SenderPaymentMode.KiiPayWallet:
                    break;
                case SenderPaymentMode.MoneyFexBankAccount:
                    ExtraFee = vm.MoneyFexBankDeposit.BankFee;
                    break;
                case SenderPaymentMode.Cash:
                    break;
                default:
                    break;
            }
            BankAccountDeposit.ExchangeRate = paymentInfo.ExchangeRate;
            BankAccountDeposit.Fee = paymentInfo.Fee;
            BankAccountDeposit.PaidFromModule = Module.Faxer;
            BankAccountDeposit.TransactionDate = DateTime.Now;
            BankAccountDeposit.TotalAmount = paymentInfo.TotalAmount + ExtraFee;
            BankAccountDeposit.ExtraFee = ExtraFee;
            BankAccountDeposit.ReceiverAccountNo = BankDeposit.AccountNumber;
            BankAccountDeposit.SenderPaymentMode = paymentMethod.SenderPaymentMode;
            BankAccountDeposit.ReceivingAmount = paymentInfo.ReceivingAmount;
            BankAccountDeposit.SenderId = Common.FaxerSession.LoggedUser.Id;
            BankAccountDeposit.ReceivingCountry = BankDeposit.CountryCode;
            BankAccountDeposit.SendingAmount = paymentInfo.SendingAmount;
            BankAccountDeposit.SendingCountry = Common.FaxerSession.LoggedUser.CountryCode;
            BankAccountDeposit.ReceiverName = paymentInfo.ReceiverName;
            BankAccountDeposit.ReceiverCountry = BankDeposit.CountryCode;
            BankAccountDeposit.BankCode = BankDeposit.BranchCode;
            BankAccountDeposit.BankId = BankDeposit.BankId;
            BankAccountDeposit.ReceiverMobileNo = BankDeposit.MobileNumber;
            BankAccountDeposit.IsManualDeposit = BankDeposit.IsManualDeposit;
            BankAccountDeposit.BankName = BankDeposit.BankName;
            BankAccountDeposit.IsEuropeTransfer = BankDeposit.IsEuropeTransfer;
            BankAccountDeposit.ReasonForTransfer = BankDeposit.ReasonForTransfer;
            BankAccountDeposit.SendingCurrency = paymentInfo.SendingCurrency;
            BankAccountDeposit.ReceivingCurrency = paymentInfo.ReceivingCurrency;

            //Changes Made For MoneyFex Bank Account Deposit
            //BankAccountDeposit.Status = vm.IsIdCheckInProgress == true ? BankDepositStatus.IdCheckInProgress :
            //     paymentMethod.SenderPaymentMode == SenderPaymentMode.MoneyFexBankAccount
            //    ? BankDepositStatus.PendingBankdepositConfirmtaion :
            //    GetBankStatus(BankDeposit.IsManualDeposit);

            BankAccountDeposit.Status = paymentMethod.SenderPaymentMode == SenderPaymentMode.MoneyFexBankAccount
                ? BankDepositStatus.PendingBankdepositConfirmtaion :
                vm.IsIdCheckInProgress == true ? BankDepositStatus.IdCheckInProgress
               : GetBankStatus(BankDeposit.IsManualDeposit);


            BankAccountDeposit.IsBusiness = BankDeposit.IsBusiness;
            BankAccountDeposit.Apiservice = ApiService;
            BankAccountDeposit.RecipientId = RecipientId;
            // For MaoneyFex Bank Account Deposit 

            BankAccountDeposit.PaymentReference = _senderBankAccountDepositServices.GetMoneyFexBankAccountDeposit().PaymentReference;
            BankAccountDeposit.HasMadePaymentToBankAccount = vm.MoneyFexBankDeposit.HasMadePaymentToBankAccount;

            if (BankAccountDeposit.SendingCountry == BankAccountDeposit.ReceivingCountry)
            {
                BankAccountDeposit.PaymentType = PaymentType.Local;
            }
            else
            {
                BankAccountDeposit.PaymentType = PaymentType.International;

            }




            if (!BankAccountDeposit.IsManualDeposit && (paymentMethod.SenderPaymentMode == SenderPaymentMode.CreditDebitCard
                || paymentMethod.SenderPaymentMode == SenderPaymentMode.SavedDebitCreditCard))
            {

                // Create bank Api response log 
                SBankDepositResponseStatus sBankDepositResponseStatus = new SBankDepositResponseStatus();

                var bankdepositTransactionResult = new BankDepositResponseVm();
                BankDepositStatus bankDepositStatus = new BankDepositStatus();
                if (vm.IsIdCheckInProgress == false)
                {
                    var transResponse = _senderBankAccountDepositServices.CreateBankTransactionToApi(BankAccountDeposit);
                    BankAccountDeposit.Status = transResponse.BankAccountDeposit.Status;
                    BankAccountDeposit.TransferReference = transResponse.BankAccountDeposit.TransferReference;
                    bankdepositTransactionResult = transResponse.BankDepositApiResponseVm;
                    sBankDepositResponseStatus.AddLog(bankdepositTransactionResult, BankAccountDeposit.Id);
                }
                if (vm.IsIdCheckInProgress == true)
                {
                    BankAccountDeposit.Status = BankDepositStatus.IdCheckInProgress;
                }

                BankAccountDeposit.Status = paymentMethod.SenderPaymentMode == SenderPaymentMode.MoneyFexBankAccount
               ? BankDepositStatus.PendingBankdepositConfirmtaion :
               vm.IsIdCheckInProgress == true ? BankDepositStatus.IdCheckInProgress : GetBankStatus(BankDeposit.IsManualDeposit, BankAccountDeposit.Status);

                _senderBankAccountDeposit.Update(BankAccountDeposit);

            }
            else
            {
                //obj = _senderBankAccountDeposit.Add(BankAccountDeposit).Data;
                _senderBankAccountDeposit.Update(BankAccountDeposit);
            }



            if (vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == Models.SenderPaymentMode.CreditDebitCard
             && vm.CreditORDebitCardDetials.SaveCard == true)
            {

                SaveCreditDebitCard(vm.CreditORDebitCardDetials, vm.TransferType, BankAccountDeposit.Id);
            }

            if (vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == SenderPaymentMode.KiiPayWallet)
            {

                SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
                int WalletId = senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id).Id;
                KiiPayWalletBalOut(WalletId, vm.KiiPayTransferPaymentSummary.TotalAmount);
            }

            // Changes Made For MoneyFex bank Account Deposit
            if (vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == SenderPaymentMode.SavedDebitCreditCard &&
                vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == SenderPaymentMode.CreditDebitCard)
            {
                SavedCreditCardTransactionDetails(vm.CreditORDebitCardDetials, vm.TransferType, BankAccountDeposit.Id);
                AddUpdateLog(vm.CreditORDebitCardDetials);
            }


            // Send Email Part
            _senderBankAccountDeposit.SendEmailAndSms(BankAccountDeposit);



            #region Notification Section 
            if (BankAccountDeposit.SenderPaymentMode == SenderPaymentMode.MoneyFexBankAccount)
            {

                var model = _senderBankAccountDepositServices.GetMoneyFexBankAccountDeposit();
                DB.Notification notification = new DB.Notification()
                {
                    SenderId = BankAccountDeposit.SenderId,
                    ReceiverId = 0,
                    Amount = "",
                    CreationDate = DateTime.Now,
                    Message = "New manual bank deposit (" + model.PaymentReference + ")",
                    NotificationReceiver = DB.NotificationFor.Admin,
                    NotificationSender = DB.NotificationFor.Sender,
                    Name = BankAccountDeposit.ReceiverName,
                    NotificationKey = model.PaymentReference
                };

                SenderCommonServices senderCommonServices = new SenderCommonServices();
                senderCommonServices.SendNotificationToAdmin(notification);

                MoneyFexBankAccountLog MoneyFexBankAccount = new MoneyFexBankAccountLog()
                {
                    IsConfirmed = false,
                    TranscationId = BankAccountDeposit.Id,
                    TrasnferMethod = TransactionTransferMethod.BankDeposit,
                    PaymentReference = model.PaymentReference
                };
                dbContext.MoneyFexBankAccountLog.Add(MoneyFexBankAccount);
                dbContext.SaveChanges();

            }
            #endregion


            return true;
        }

        public string GenerateReceiptNoForBankDepoist(bool IsManualBankDeposit)
        {
            var ReceiptNo = Common.Common.GenerateBankAccountDepositReceiptNo(6);
            if (IsManualBankDeposit == true)
            {
                ReceiptNo = Common.Common.GenerateManualBankAccountDepositReceiptNo(6);
                return ReceiptNo;
            }
            return ReceiptNo;
        }

        public BankDepositStatus GetBankStatus(bool IsMankDeposit, BankDepositStatus bankDepositStatus = BankDepositStatus.PaymentPending)
        {

            var status = bankDepositStatus;

            if (IsMankDeposit == true)
            {
                status = BankDepositStatus.Incomplete;
            }
            return status;
        }





        public void SendCashPickUpTOSenderSms(string senderFirstName, string receiverFirstName, string Amount, string PhoneNo)
        {
            SmsApi smsApi = new SmsApi();
            string msg = smsApi.GetCashPickUPReceivedMessage(senderFirstName, receiverFirstName, Common.FaxerSession.MFCN, Amount);
            smsApi.SendSMS(PhoneNo, msg);
        }



        public void SendTransactionPausedEmail(string receiptNo,
            string sendingAmount, string receivingAmount, string receivingCountry, string fee, string receiverFirstName
            , string receiverAccountNo, string bankCode,
            int bankId, int senderId, int WalletId, string MFCN, TransactionServiceType TransactionServiceType)
        {
            FaxerSession.TransactionEmailTypeSession = TransactionEmailType.IDCheck;
            SSenderBankAccountDeposit _sBankDeposit = new SSenderBankAccountDeposit();
            var senderInfo = _sBankDeposit.GetSenderInfo(senderId);
            string email = senderInfo.Email;
            string SenderFristName = senderInfo.FirstName;

            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string bankName = Common.Common.getBankName(bankId);
            string ReceivingCountry = Common.Common.GetCountryName(receivingCountry);
            string WalletName = "";
            if (WalletId != 0)
            {
                WalletName = Common.Common.GetMobileWalletInfo(WalletId).Name;

            }
            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/TransactionPaused?" + "&SenderFristName=" + SenderFristName + "&TransactionNumber=" + receiptNo
                + "&SendingAmount=" + sendingAmount + "&ReceivingAmount=" + receivingAmount + "&Receivingcountry=" + ReceivingCountry + "&Fee=" + fee
                + "&ReceiverFirstName=" + receiverFirstName + "&BankName=" + bankName + "&BankAccount=" + receiverAccountNo + "&BankCode=" + bankCode
                + "&TransactionServiceType=" + TransactionServiceType + "&WalletName=" + WalletName + "&MFCN=" + MFCN);


            mail.SendMail(email, "Your transfer has been paused", body);

        }


        public void SendTransactionPendingEmail()
        {

            Common.Common.SetTransactionEmailTypeSession(TransactionEmailType.TransactionPending);
            var model = Common.FaxerSession.TransactionPendingViewModel;
            SSenderBankAccountDeposit _sBankDeposit = new SSenderBankAccountDeposit();
            var senderInfo = _sBankDeposit.GetSenderInfo(model.SenderId);
            string email = senderInfo.Email;
            string senderFirstName = senderInfo.FirstName;

            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";

            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/PaymentPending?" + "&SenderFristName=" + senderFirstName + "&TransactionNumber=" +
                                             model.TransactionNumber + "&SendingAmount=" + model.SendingAmount + "&SendingCurrency=" + model.SendingCurrency + "&ExchangeRate=" +
                                            model.ExchangeRate + "&Receivingurrency=" + model.Receivingurrency + "&Receivingcountry=" + model.ReceivingCountry + "&Fee=" + model.Fee
                                             + "&ReceiverName=" + model.ReceiverFullName + "&BankName=" + model.BankName + "&BankAccount=" + model.BankAccount + "&BankCode=" + model.BankCode
             + "&TransactionServiceType=" + model.TransferMethod + "&WalletName=" + model.WalletName + "&MFCN=" + model.MFCN + "&TransactionId=" + model.TransactionId
             + "&MobileNo=" + model.MobileNo);

            mail.SendMail(email, "Payment pending" + " " + model.ReceiptNumber, body);

        }


        public void SendCashPickUpEmail(string senderName, string receiverName, string receiverFirstName, string MFCN, string sendingAmount, string ReceivingAmount, string Fee,
            string ReceivingCountry, string PaymentReference, SenderPaymentMode SenderPaymentMode, int SenderId)
        {

            FaxerSession.TransactionEmailTypeSession = TransactionEmailType.TransactionInProgress;
            SSenderBankAccountDeposit _sBankDeposit = new SSenderBankAccountDeposit();
            var senderInfo = _sBankDeposit.GetSenderInfo(SenderId);
            string email = senderInfo.Email;
            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string receivingCountry = Common.Common.GetCountryName(ReceivingCountry);
            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/CashPickUPEmail?" +
                 "&senderName=" + senderName + "&receiverName=" + receiverName + "&receiverFirstName=" + receiverFirstName + "&MFCN=" + MFCN
                 + "&sendingAmount=" + sendingAmount + "&ReceivingAmount=" + ReceivingAmount + "&Fee=" + Fee + "&receivingCountry=" + receivingCountry
                 + "&PaymentReference=" + PaymentReference + "&SenderPaymentMode=" + SenderPaymentMode);

            mail.SendMail(email, "Confirmation of transfer to" + " " + receiverFirstName, body);

        }
        public void SendCashPickUpSuccessEmail(string senderName, string receiverName, string receiverFirstName, string sendingAmount, string ReceivingCountry, int SenderId, string City)
        {

            FaxerSession.TransactionEmailTypeSession = TransactionEmailType.TransactionCompleted;
            SSenderBankAccountDeposit _sBankDeposit = new SSenderBankAccountDeposit();
            var senderInfo = _sBankDeposit.GetSenderInfo(SenderId);
            string email = senderInfo.Email;
            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string receivingCountry = Common.Common.GetCountryName(ReceivingCountry);
            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/CashPickUPSuccessEmail?" +
                 "&senderName=" + senderName + "&receiverName=" + receiverName + "&city=" + City + "&sendingAmount=" + sendingAmount
                 + "&receivingCountry=" + receivingCountry);

            mail.SendMail(email, "Confirmation of transfer to" + " " + receiverFirstName, body);

        }



        public bool RetryBankTransaction(int TransactionId)
        {

            string refno = "";
            string PartnerRef = "";
            bool IsterminatedTrans = false;
            var BankDepositResponseStatus = dbContext.BankDepositResponseStatus.Where(x => x.TransactionId == TransactionId).FirstOrDefault();
            var BankDepositTransactionResponseResult = dbContext.BankDepositTransactionResponseResult.Where(x => x.BankDepositResponseStatusId == BankDepositResponseStatus.Id).FirstOrDefault();
            if (BankDepositTransactionResponseResult != null)
            {
                refno = BankDepositTransactionResponseResult.transactionReference;
                PartnerRef = BankDepositTransactionResponseResult.partnerTransactionReference;
            }
            else
            {

                var bankDepositDetail = dbContext.BankAccountDeposit.Where(x => x.Id == TransactionId).FirstOrDefault();
                PartnerRef = bankDepositDetail.ReceiptNo;
                IsterminatedTrans = true;
            }
            BankDepositApi bankDepositApi = new BankDepositApi();

            bankDepositApi.BankDepositGetStatusCallBack(refno, PartnerRef, IsterminatedTrans);



            return true;
        }
        public TransactionStatusApiVm RecheckTransaction(int transactionId, TransactionServiceType transactionServiceType)
        {
            var result = new TransactionStatusApiVm();
            switch (transactionServiceType)
            {
                case TransactionServiceType.All:
                    break;
                case TransactionServiceType.MobileWallet:
                    result = GetMobileWaletTransactionStatusFromApi(transactionId);
                    break;
                case TransactionServiceType.KiiPayWallet:
                    break;
                case TransactionServiceType.BillPayment:
                    break;
                case TransactionServiceType.ServicePayment:
                    break;
                case TransactionServiceType.CashPickUp:
                    result = GetCashPickUpTransactionStatusFromApi(transactionId);
                    break;
                case TransactionServiceType.BankDeposit:
                    result = GetBankAccountDepositTransactionStatusFromApi(transactionId);
                    break;
                default:
                    break;
            }
            return result;
        }
        public TransactionStatusApiVm GetBankAccountDepositTransactionStatusFromApi(int TransactionId)
        {

            try
            {
                var bankDeposit = dbContext.BankAccountDeposit.Where(x => x.Id == TransactionId).FirstOrDefault();
                string errorMsg = "";
                string reference = "";
                string status = "";
                switch (bankDeposit.Apiservice)
                {
                    case Apiservice.TransferZero:
                        TransferZeroApi transferZeroApi = new TransferZeroApi();
                        var result = transferZeroApi.GetTransactionStatus(bankDeposit.ReceiptNo);
                        if (result.Id == null)
                        {

                            return new TransactionStatusApiVm()
                            {
                                ErrorMessage = "Transaction has not hit the api till yet",
                                ReceiptNo = "",
                                Reference = "",
                                Status = "Not Intiated To Api"
                            };
                        }
                        errorMsg = result.Recipients[0].StateReason;
                        reference = result.Recipients[0].TransactionId;
                        status = Enum.GetName(typeof(TransactionState), result.Recipients[0].TransactionState);
                        break;
                    case Apiservice.CashPot:
                        SSenderBankAccountDeposit sSenderBankAccountDeposit = new SSenderBankAccountDeposit();
                        var statusRsponse = sSenderBankAccountDeposit.GetStatusResponse(bankDeposit.ReceiptNo);
                        status = Common.Common.GetEnumDescription(sSenderBankAccountDeposit.GetCashPotTransactionStatus(statusRsponse.STATUS_CODE));
                        errorMsg = statusRsponse.MSG_NOTE;
                        reference = statusRsponse.REFERENCE_CODE;
                        break;
                }
                return new TransactionStatusApiVm()
                {
                    ErrorMessage = errorMsg,
                    ReceiptNo = bankDeposit.ReceiptNo,
                    Reference = reference,
                    Status = status
                };
            }
            catch (Exception ex)
            {
                Log.Write("Exception Occur in SSenderForAllTransfer class function " +
                    "GetBankAccountDepositTransactionStatusFromApi( +" + TransactionId + ")" + "/n" +
                    ex.Message);
                return new TransactionStatusApiVm()
                {
                    ErrorMessage = "Internal System Error"
                };
            }

        }
        public TransactionStatusApiVm GetMobileWaletTransactionStatusFromApi(int TransactionId)
        {

            try
            {
                string errorMsg = "";
                string Reference = "";
                string Status = "";
                var mobileWallet = dbContext.MobileMoneyTransfer.Where(x => x.Id == TransactionId).FirstOrDefault();
                switch (mobileWallet.Apiservice)
                {
                    case Apiservice.VGG:
                        break;
                    case Apiservice.TransferZero:
                        TransferZeroApi transferZeroApi = new TransferZeroApi();
                        var result = transferZeroApi.GetTransactionStatus(mobileWallet.ReceiptNo);
                        if (result.Id == null)
                        {
                            errorMsg = "Transaction has not hit the api till yet";
                            Reference = "";
                            Status = "Not Intiated To Api";
                        }
                        else
                        {
                            errorMsg = result.Recipients[0].StateReason;
                            Reference = result.Recipients[0].TransactionId;
                            Status = Enum.GetName(typeof(TransactionState), result.Recipients[0].TransactionState);
                        }
                        break;
                    case Apiservice.EmergentApi:
                        break;
                    case Apiservice.MTN:
                        MoblieTransferApi.MobileTransferApi mobileTransferApiServices = new MoblieTransferApi.MobileTransferApi();
                        var configModel = new MobileTransferApiConfigurationVm()
                        {
                            apiKey = "",
                            apirefId = Guid.NewGuid().ToString(),
                            apiUrl = "",
                            subscriptionKey = Common.Common.GetAppSettingValue("MTNApiSubscriptionKey")
                        };
                        var tokenModel = mobileTransferApiServices.Login<MobileTransferAccessTokeneResponse>(configModel).Result;
                        tokenModel.apirefId = configModel.apirefId;
                        var transactionStatus = mobileTransferApiServices.GetTransactionStatus<MTNCameroonResponseParamVm>(tokenModel.apirefId, tokenModel.access_token).Result;
                        if (transactionStatus.status == null)
                        {
                            errorMsg = "Transaction has not hit the api till yet";
                            Reference = "";
                            Status = "Not Intiated To Api";
                        }
                        else
                        {
                            errorMsg = transactionStatus.reason;
                            Reference = "";
                            Status = transactionStatus.status;
                        }

                        break;
                    case Apiservice.Zenith:
                        break;
                    case Apiservice.Wari:
                        break;
                    default:
                        break;
                }

                return new TransactionStatusApiVm()
                {
                    ErrorMessage = errorMsg,
                    ReceiptNo = mobileWallet.ReceiptNo,
                    Reference = Reference,
                    Status = Status
                };

            }
            catch (Exception ex)
            {
                Log.Write("Exception Occur in SSenderForAllTransfer class function " +
                    "GetBankAccountDepositTransactionStatusFromApi( +" + TransactionId + ")" + "/n" +
                    ex.Message);
                return new TransactionStatusApiVm()
                {
                    ErrorMessage = "Internal System Error"
                };
            }

        }

        public TransactionStatusApiVm GetCashPickUpTransactionStatusFromApi(int transactionId)
        {
            try
            {
                string errorMsg = "";
                string Reference = "";
                string Status = "";

                var CashPickUp = dbContext.FaxingNonCardTransaction.Where(x => x.Id == transactionId).FirstOrDefault();

                switch (CashPickUp.Apiservice)
                {
                    case Apiservice.VGG:
                        break;
                    case Apiservice.TransferZero:
                        TransferZeroApi transferZeroApi = new TransferZeroApi();
                        var result = transferZeroApi.GetTransactionStatus(CashPickUp.ReceiptNumber);
                        if (result.Id == null)
                        {
                            errorMsg = "Transaction has not hit the api till yet";
                            Reference = "";
                            Status = "Not Intiated To Api";
                        }
                        else
                        {
                            errorMsg = result.Recipients[0].StateReason;
                            Reference = result.Recipients[0].TransactionId;
                            Status = Enum.GetName(typeof(TransactionState), result.Recipients[0].TransactionState);
                        }
                        break;
                    case Apiservice.EmergentApi:
                        break;
                    case Apiservice.MTN:
                        break;
                    case Apiservice.Zenith:
                        break;
                    case Apiservice.Wari:
                        break;
                    case Apiservice.CashPot:
                        SSenderCashPickUp sSenderCashPickUp = new SSenderCashPickUp();
                        var statusResponse = sSenderCashPickUp.GetStatusResponse(CashPickUp.ReceiptNumber);
                        Status = Common.Common.GetEnumDescription(sSenderCashPickUp.GetCashPotCAshPickUpTransactionStatus(statusResponse.STATUS_CODE));
                        errorMsg = statusResponse.MSG_NOTE;
                        Reference = statusResponse.REFERENCE_CODE;
                        break;
                    default:
                        Status = CashPickUp.FaxingStatus.ToString();
                        break;
                }

                return new TransactionStatusApiVm()
                {
                    ErrorMessage = errorMsg,
                    ReceiptNo = CashPickUp.ReceiptNumber,
                    Reference = Reference,
                    Status = Status
                };

            }
            catch (Exception ex)
            {
                Log.Write(ex.Message, ErrorType.UnSpecified, "GetCashPickUpTransactionStatusFromApi");
                return new TransactionStatusApiVm()
                {
                    ErrorMessage = "Internal System Error"
                };
            }
        }


        public bool CompletePayARequestTransaction(TransactionSummaryVM vm)
        {


            return true;
        }



        #region Credit and debit Card usage Information 

        public bool AddUpdateLog(CreditDebitCardViewModel debitCardViewModel)
        {

            SCreditDebitCardUsage creditDebitCardUsage = new SCreditDebitCardUsage();
            creditDebitCardUsage.AddOrUpdateCreditCardUsageLog(new CreditCardUsageLog()
            {
                CardNum = Common.Common.FormatSavedCardNumber(debitCardViewModel.CardNumber),
                Count = 1,
                Module = Module.Faxer,
                SenderId = Common.FaxerSession.LoggedUser.Id,
                UpdatedDateTime = DateTime.Now
            });
            return true;
        }
        #endregion

        #region For Staff

        public void SetStaffTransactionSummary(TransactionSummaryVM model)
        {

            Common.AdminSession.TransactionSummary = model;
        }

        public TransactionSummaryVM GetStaffTransactionSummary()
        {

            TransactionSummaryVM vm = new TransactionSummaryVM();
            if (Common.AdminSession.TransactionSummary != null)
            {

                vm = Common.AdminSession.TransactionSummary;
            }
            return vm;
        }


        #endregion

    }
    public class TransactionSummaryVM
    {
        public Models.SenderAndReceiverDetialVM SenderAndReceiverDetail { get; set; }
        public Models.KiiPayTransferPaymentSummary KiiPayTransferPaymentSummary { get; set; }
        public Models.PaymentMethodViewModel PaymentMethodAndAutoPaymentDetail { get; set; }
        /// <summary>
        /// If sender Payment Model is credit debit card Or Saved Debit credit card (add value to view Model)
        /// </summary>
        public Models.CreditDebitCardViewModel CreditORDebitCardDetials { get; set; }
        /// <summary>
        /// If sender Payment Model is MoneyFex BAnk Deposit(add value to view Model)
        /// </summary
        public Models.SenderMoneyFexBankDepositVM MoneyFexBankDeposit { get; set; }
        public Models.SenderCashPickUpVM CashPickUpVM { get; set; }
        public CashPickUpReceiverDetailsInformationViewModel CashPickUpVmStaff { get; set; }
        public SenderMobileMoneyTransferVM MobileMoneyTransfer { get; set; }
        public ReceiverDetailsInformationViewModel MobileMoneyTransferAgent { get; set; }
        public SenderBankAccountDepositVm BankAccountDeposit { get; set; }
        public TransferType TransferType { get; set; }
        public bool IsLocalPayment { get; set; }
        public bool IsIdCheckInProgress { get; set; }

        public string ReceiptNo { get; set; }
        public CardProcessorApi CardProcessorApi { get; set; }

    }

    public class TransactionStatusApiVm
    {

        public string ReceiptNo { get; set; }
        public string Reference { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class TransactionReInitializationReponseVm
    {

        public string OldReceiptNo { get; set; }
        public string NewReceiptNo { get; set; }
        public string Status { get; set; }
        public string TransactionReport { get; set; }
    }

}