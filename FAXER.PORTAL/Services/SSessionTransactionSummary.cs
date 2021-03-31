using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.SecureTradingPaymentGateway;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Services
{
    public class SSessionTransactionSummary
    {
        DB.FAXEREntities db = null;
        WebServices webServices = null;
        private string SessionId = "";
        public SSessionTransactionSummary()
        {
            db = new DB.FAXEREntities();
            webServices = new WebServices();
        }
        public SSessionTransactionSummary(string SessionId)
        {
            db = new DB.FAXEREntities();
            webServices = new WebServices();
            this.SessionId = SessionId;
        }
        public TransactionSummaryVM SaveSessionTransactionSummary(TransactionSummaryVM model)
        {
            ClearSessionTransactionSummary(SessionId);
            var transactionSummary = AddSessionTransactionSummary(new SessionTransactionSummary()
            {
                SessionId = SessionId,
                ReceiptNo = model.ReceiptNo,
                IsIdCheckInProgress = model.IsIdCheckInProgress,
                IsLocalPayment = model.IsLocalPayment,
                TransferType = model.TransferType,
                CardProcessorApi = model.CardProcessorApi

            });
            if (model.SenderAndReceiverDetail != null)
            {
                model.SenderAndReceiverDetail.TransactionSummaryId = transactionSummary.Id;
                var result_SessionSenderAndReceiverDetial = AddSessionSenderAndReceiverDetial(model.SenderAndReceiverDetail);
            }
            if (model.KiiPayTransferPaymentSummary != null)
            {
                model.KiiPayTransferPaymentSummary.TransactionSummaryId = transactionSummary.Id;
                var result_SessionKiiPayTransferPaymentSummary = AddSessionKiiPayTransferPaymentSummary(model.KiiPayTransferPaymentSummary);
            }
            if (model.PaymentMethodAndAutoPaymentDetail != null)
            {
                model.PaymentMethodAndAutoPaymentDetail.TransactionSummaryId = transactionSummary.Id;
                var result_SessionPaymentMethod = AddSessionPaymentMethod(model.PaymentMethodAndAutoPaymentDetail);
            }
            if (model.CreditORDebitCardDetials != null)
            {
                model.CreditORDebitCardDetials.TransactionSummaryId = transactionSummary.Id;
                var result_SessionCreditDebitCard = AddSessionCreditDebitCardViewModel(model.CreditORDebitCardDetials);
            }
            if (model.MoneyFexBankDeposit != null)
            {
                model.MoneyFexBankDeposit.TransactionSummaryId = transactionSummary.Id;
                var result_SessionSenderMoneyFexBankDeposit = AddSessionSenderMoneyFexBankDeposit(model.MoneyFexBankDeposit);
            }
            if (model.CashPickUpVM != null)
            {
                model.CashPickUpVM.TransactionSummaryId = transactionSummary.Id;
                var result_SessionSenderCashPickUp = AddSessionSenderCashPickUp(model.CashPickUpVM);
            }
            if (model.CashPickUpVmStaff != null)
            {
                model.CashPickUpVmStaff.TransactionSummaryId = transactionSummary.Id;
                var result_essionCashPickUpReceiverDetail = AddSessionCashPickUpReceiverDetailsInformation(model.CashPickUpVmStaff);
            }
            if (model.MobileMoneyTransfer != null)
            {
                model.MobileMoneyTransfer.TransactionSummaryId = transactionSummary.Id;
                var result_SessionSenderMobileMoneyTransfer = AddSessionSenderMobileMoneyTransfer(model.MobileMoneyTransfer);
            }
            if (model.MobileMoneyTransferAgent != null)
            {
                model.MobileMoneyTransferAgent.TransactionSummaryId = transactionSummary.Id;
                var result_SessionReceiverDetailsInformation = AddSessionReceiverDetailsInformation(model.MobileMoneyTransferAgent);
            }
            if (model.BankAccountDeposit != null)
            {

                model.BankAccountDeposit.TransactionSummaryId = transactionSummary.Id;
                var result_SessionSenderBankAccountDeposit = AddSessionSenderBankAccountDeposit(model.BankAccountDeposit);
            }
            return new TransactionSummaryVM();
        }
        public TransactionSummaryVM GetTransactionSummarySession()
        {

            var data = db.SessionTransactionSummary.Where(x => x.SessionId == SessionId).FirstOrDefault();
            if (data == null)
            {
                return new TransactionSummaryVM();
            }

            TransactionSummaryVM model = new TransactionSummaryVM()
            {
                IsIdCheckInProgress = data.IsIdCheckInProgress,
                ReceiptNo = data.ReceiptNo,
                IsLocalPayment = data.IsLocalPayment,
                TransferType = data.TransferType,
                CardProcessorApi = data.CardProcessorApi,
                BankAccountDeposit = GetBankAccountDepositSession(data.Id),
                CashPickUpVM = GetCashPickUpSession(data.Id),
                CashPickUpVmStaff = GetStaffCashPickUpSession(data.Id),
                CreditORDebitCardDetials = GetCreditOrDebitCardDetailsSession(data.Id),
                KiiPayTransferPaymentSummary = GetKiiPayTransferPaymentSummarySession(data.Id),
                MobileMoneyTransfer = GetMobileMoneyTransferSession(data.Id),
                MobileMoneyTransferAgent = GetMobileMoneyTransferAgentSession(data.Id),
                MoneyFexBankDeposit = GetMoneyFexBankDepositSession(data.Id),
                PaymentMethodAndAutoPaymentDetail = GetPaymentMethodAndAutoPaymentDetailSession(data.Id),
                SenderAndReceiverDetail = GetSenderAndReceiverDetailSession(data.Id)

            };
            return model;
        }

        private SenderAndReceiverDetialVM GetSenderAndReceiverDetailSession(int id)
        {
            var data = (from c in db.SessionSenderAndReceiverDetial.Where(x => x.TransactionSummaryId == id)
                        select new SenderAndReceiverDetialVM()
                        {
                            ReceiverCountry = c.ReceiverCountry,
                            ReceiverId = c.ReceiverId,
                            ReceiverMobileNo = c.ReceiverMobileNo,
                            SenderCountry = c.SenderCountry,
                            SenderId = c.SenderId,
                            SenderPhoneNo = c.SenderPhoneNo,
                            SenderWalletId = c.SenderWalletId,
                            TransactionSummaryId = c.TransactionSummaryId,
                            WalletOperatorId = c.WalletOperatorId
                        }).FirstOrDefault();
            return data;
        }

        private PaymentMethodViewModel GetPaymentMethodAndAutoPaymentDetailSession(int id)
        {
            var data = (from c in db.SessionPaymentMethod.Where(x => x.TransactionSummaryId == id)
                        select new PaymentMethodViewModel()
                        {
                            AutoPaymentAmount = c.AutoPaymentAmount,
                            AutopaymentFrequency = c.AutopaymentFrequency,
                            EnableAutoPayment = c.EnableAutoPayment,
                            TransactionSummaryId = c.TransactionSummaryId,
                            HasEnableMoneyFexBankAccount = c.HasEnableMoneyFexBankAccount,
                            HasKiiPayWallet = c.HasKiiPayWallet,
                            KiipayWalletBalance = c.KiipayWalletBalance,
                            PaymentDay = c.PaymentDay,
                            PaymentMethod = c.PaymentMethod,
                            SenderPaymentMode = c.SenderPaymentMode,
                            SendingCurrencySymbol = c.SendingCurrencySymbol,
                            TotalAmount = c.TotalAmount

                        }).FirstOrDefault();
            return data;
        }

        private SenderMoneyFexBankDepositVM GetMoneyFexBankDepositSession(int id)
        {
            var data = (from c in db.SessionSenderMoneyFexBankDeposit.Where(x => x.TransactionSummaryId == id)
                        select new SenderMoneyFexBankDepositVM()
                        {
                            AccountNumber = c.AccountNumber,
                            Amount = c.Amount,
                            AvailableBalance = c.AvailableBalance,
                            BankFee = c.BankFee,
                            HasMadePaymentToBankAccount = c.HasMadePaymentToBankAccount,
                            LabelName = c.LabelName,
                            PaymentReference = c.PaymentReference,
                            SendingCurrencyCode = c.SendingCurrencyCode,
                            SendingCurrencySymbol = c.SendingCurrencySymbol,
                            ShortCode = c.ShortCode,
                            TransactionSummaryId = c.TransactionSummaryId
                        }).FirstOrDefault();
            return data;
        }

        private ReceiverDetailsInformationViewModel GetMobileMoneyTransferAgentSession(int id)
        {
            var data = (from c in db.SessionReceiverDetailsInformation.Where(x => x.TransactionSummaryId == id)
                        select new ReceiverDetailsInformationViewModel()
                        {
                            Country = c.Country,
                            MobileCode = c.MobileCode,
                            MobileNumber = c.MobileNumber,
                            MobileWalletProvider = c.MobileWalletProvider,
                            PreviousMobileNumber = c.PreviousMobileNumber,
                            ReasonForTransfer = c.ReasonForTransfer,
                            ReceiverName = c.ReceiverName,
                            TransactionSummaryId = c.TransactionSummaryId
                        }).FirstOrDefault();
            return data;
        }

        private SenderMobileMoneyTransferVM GetMobileMoneyTransferSession(int id)
        {
            var data = (from c in db.SessionSenderMobileMoneyTransfer.Where(x => x.TransactionSummaryId == id)
                        select new SenderMobileMoneyTransferVM()
                        {
                            CountryCode = c.CountryCode,
                            CountryPhoneCode = c.CountryPhoneCode,
                            MobileNumber = c.MobileNumber,
                            ReceiverName = c.ReceiverName,
                            RecentlyPaidMobile = c.RecentlyPaidMobile,
                            TransactionSummaryId = c.TransactionSummaryId,
                            WalletId = c.WalletId
                        }).FirstOrDefault();

            return data;
        }

        private CreditDebitCardViewModel GetCreditOrDebitCardDetailsSession(int id)
        {
            var data = (from c in db.SessionCreditDebitCardViewModel.Where(x => x.TransactionSummaryId == id)
                        select new CreditDebitCardViewModel()
                        {
                            AddressLineOne = c.AddressLineOne,
                            AddressLineTwo = c.AddressLineTwo,
                            AutoTopUp = c.AutoTopUp,
                            AutoTopUpAmount = c.AutoTopUpAmount,
                            CardNumber = c.CardNumber,
                            CardUsageMsg = c.CardUsageMsg,
                            CityName = c.CityName,
                            Confirm = c.Confirm,
                            CountyName = c.CountyName,
                            CreditDebitCardFee = c.CreditDebitCardFee,
                            CreditDebitCardType = c.CreditDebitCardType,
                            EndMM = c.EndMM,
                            EndYY = c.EndYY,
                            ErrorMsg = c.ErrorMsg,
                            ExpiryDate = c.ExpiryDate,
                            FaxingAmount = c.FaxingAmount,
                            FaxingCurrency = c.FaxingCurrency,
                            FaxingCurrencySymbol = c.FaxingCurrencySymbol,
                            IsCardUsageMsg = c.IsCardUsageMsg,
                            NameOnCard = c.NameOnCard,
                            PaymentDay = c.PaymentDay,
                            PaymentFrequency = c.PaymentFrequency,
                            ReceiverName = c.ReceiverName,
                            SaveCard = c.SaveCard,
                            SecurityCode = c.SecurityCode,
                            StripeTokenID = c.StripeTokenID,
                            ThreeDEnrolled = c.ThreeDEnrolled,
                            TransactionSummaryId = c.TransactionSummaryId,
                            UserImage = c.UserImage,
                            ZipCode = c.ZipCode
                        }).FirstOrDefault();
            return data;
        }
        private KiiPayTransferPaymentSummary GetKiiPayTransferPaymentSummarySession(int id)
        {
            var data = (from c in db.SessionKiiPayTransferPaymentSummary.Where(x => x.TransactionSummaryId == id)
                        select new KiiPayTransferPaymentSummary()
                        {
                            ExchangeRate = c.ExchangeRate,
                            Fee = c.Fee,
                            PaymentReference = c.PaymentReference,
                            ReceiverCity = c.ReceiverCity,
                            ReceivingAmount = c.ReceivingAmount,
                            ReceiverName = c.ReceiverName,
                            ReceivingCurrency = c.ReceivingCurrency,
                            ReceivingCurrencySymbol = c.ReceivingCurrencySymbol,
                            SendingAmount = c.SendingAmount,
                            SendingCurrency = c.SendingCurrency,
                            SendingCurrencySymbol = c.SendingCurrencySymbol,
                            SendSMS = c.SendSMS,
                            SMSFee = c.SMSFee,
                            TotalAmount = c.TotalAmount,
                            TransactionSummaryId = c.TransactionSummaryId
                        }).FirstOrDefault();
            return data;
        }

        private CashPickUpReceiverDetailsInformationViewModel GetStaffCashPickUpSession(int id)
        {
            var data = (from c in db.SessionCashPickUpReceiverDetailsInformation.Where(x => x.TransactionSummaryId == id)
                        select new CashPickUpReceiverDetailsInformationViewModel()
                        {
                            City = c.City,
                            Country = c.Country,
                            Email = c.Email,
                            MobileCode = c.MobileCode,
                            MobileNo = c.MobileNo,
                            PreviousReceiver = c.PreviousReceiver,
                            ReasonForTransfer = c.ReasonForTransfer,
                            ReceiverFullName = c.ReceiverFullName,
                            Searched = c.Searched,
                            TransactionSummaryId = c.TransactionSummaryId

                        }).FirstOrDefault();
            return data;
        }

        private SenderCashPickUpVM GetCashPickUpSession(int id)
        {
            var data = (from c in db.SessionSenderCashPickUp.Where(x => x.TransactionSummaryId == id)
                        select new SenderCashPickUpVM()
                        {
                            CountryCode = c.CountryCode,
                            EmailAddress = c.EmailAddress,
                            FullName = c.FullName,
                            MobileNumber = c.MobileNumber,
                            Reason = c.Reason,
                            RecentReceiverId = c.RecentReceiverId,
                            TransactionSummaryId = c.TransactionSummaryId,
                            IdenityCardId = c.IdenityCardId,
                            IdentityCardNumber = c.IdentityCardNumber
                        }).FirstOrDefault();
            return data;
        }

        private SenderBankAccountDepositVm GetBankAccountDepositSession(int id)
        {
            var data = (from c in db.SessionSenderBankAccountDeposit.Where(x => x.TransactionSummaryId == id)
                        select new SenderBankAccountDepositVm()
                        {
                            AccountNumber = c.AccountNumber,
                            AccountOwnerName = c.AccountOwnerName,
                            BankId = c.BankId,
                            BankName = c.BankName,
                            BranchCode = c.BranchCode,
                            BranchId = c.BranchId,
                            CountryCode = c.CountryCode,
                            CountryPhoneCode = c.CountryPhoneCode,
                            IsBusiness = c.IsBusiness,
                            IsEuropeTransfer = c.IsEuropeTransfer,
                            IsManualDeposit = c.IsManualDeposit,
                            MobileNumber = c.MobileNumber,
                            ReasonForTransfer = c.ReasonForTransfer,
                            ReceipientId = c.ReceipientId,
                            RecentAccountNumber = c.RecentAccountNumber,
                            walletId = c.walletId,
                            TransactionSummaryId = c.TransactionSummaryId,
                            IsSouthAfricaTransfer = c.IsSouthAfricaTransfer,
                            ReceiverPostalCode = c.ReceiverPostalCode,
                            ReceiverCity = c.ReceiverCity,
                            ReceiverStreet = c.ReceiverStreet,
                            ReceiverEmail = c.ReceiverEmail,
                            IsWestAfricaTransfer = c.IsWestAfricaTransfer
                        }).FirstOrDefault();
            return data;
        }

        public SessionTransactionSummary AddSessionTransactionSummary(SessionTransactionSummary model)
        {
            db.SessionTransactionSummary.Add(model);
            db.SaveChanges();
            return model;
        }
        public SessionSenderAndReceiverDetial AddSessionSenderAndReceiverDetial(SenderAndReceiverDetialVM vm)
        {
            var SessionSenderAndReceiverDetialSerialize = webServices.
                                                        SerializeObject<SenderAndReceiverDetialVM>
                                                        (vm);
            var model = webServices.DeserializeObject<SessionSenderAndReceiverDetial>
                        (SessionSenderAndReceiverDetialSerialize).Result;
            //var model = new SessionSenderAndReceiverDetial();
            db.SessionSenderAndReceiverDetial.Add(model);
            db.SaveChanges();
            return model;
        }
        public SessionKiiPayTransferPaymentSummary AddSessionKiiPayTransferPaymentSummary(KiiPayTransferPaymentSummary vm)
        {

            var SessionKiiPayTransferPaymentSummarySerialize = webServices.
                                                            SerializeObject<KiiPayTransferPaymentSummary>
                                                             (vm);
            var model = webServices.DeserializeObject<SessionKiiPayTransferPaymentSummary>
                (SessionKiiPayTransferPaymentSummarySerialize).Result;

            db.SessionKiiPayTransferPaymentSummary.Add(model);
            db.SaveChanges();
            return model;
        }
        public SessionPaymentMethod AddSessionPaymentMethod(PaymentMethodViewModel vm)
        {
            var SessionPaymentMethodSerialize = webServices.SerializeObject<PaymentMethodViewModel>(vm);

            var model = webServices.DeserializeObject<SessionPaymentMethod>
                (SessionPaymentMethodSerialize).Result;

            db.SessionPaymentMethod.Add(model);
            db.SaveChanges();
            return model;
        }
        public SessionCreditDebitCardViewModel AddSessionCreditDebitCardViewModel(CreditDebitCardViewModel vm)
        {
            var SerializedModel = webServices.SerializeObject<CreditDebitCardViewModel>(vm);

            var model = webServices.DeserializeObject<SessionCreditDebitCardViewModel>
                (SerializedModel).Result;
            db.SessionCreditDebitCardViewModel.Add(model);
            db.SaveChanges();
            return model;
        }
        public SessionSenderMoneyFexBankDeposit AddSessionSenderMoneyFexBankDeposit(SenderMoneyFexBankDepositVM vm)
        {
            var SerializedModel = webServices.SerializeObject<SenderMoneyFexBankDepositVM>(vm);
            var model = webServices.DeserializeObject<SessionSenderMoneyFexBankDeposit>
                (SerializedModel).Result;
            db.SessionSenderMoneyFexBankDeposit.Add(model);
            db.SaveChanges();
            return model;
        }
        public SessionSenderCashPickUp AddSessionSenderCashPickUp(SenderCashPickUpVM vm)
        {

            var SerializedModel = webServices.SerializeObject<SenderCashPickUpVM>(vm);
            var model = webServices.DeserializeObject<SessionSenderCashPickUp>
                (SerializedModel).Result;
            db.SessionSenderCashPickUp.Add(model);
            db.SaveChanges();
            return model;
        }
        public SessionCashPickUpReceiverDetailsInformation AddSessionCashPickUpReceiverDetailsInformation(CashPickUpReceiverDetailsInformationViewModel vm)
        {

            var SerializedModel = webServices.SerializeObject<CashPickUpReceiverDetailsInformationViewModel>(vm);
            var model = webServices.DeserializeObject<SessionCashPickUpReceiverDetailsInformation>
                (SerializedModel).Result;
            db.SessionCashPickUpReceiverDetailsInformation.Add(model);
            db.SaveChanges();
            return model;
        }

        public SessionSenderMobileMoneyTransfer AddSessionSenderMobileMoneyTransfer(SenderMobileMoneyTransferVM vm)
        {
            var SerializedModel = webServices.SerializeObject<SenderMobileMoneyTransferVM>(vm);
            var model = webServices.DeserializeObject<SessionSenderMobileMoneyTransfer>
                (SerializedModel).Result;
            db.SessionSenderMobileMoneyTransfer.Add(model);
            db.SaveChanges();
            return model;
        }
        public SessionReceiverDetailsInformation AddSessionReceiverDetailsInformation(ReceiverDetailsInformationViewModel vm)
        {
            var SerializedModel = webServices.SerializeObject<ReceiverDetailsInformationViewModel>(vm);
            var model = webServices.DeserializeObject<SessionReceiverDetailsInformation>
                (SerializedModel).Result;
            db.SessionReceiverDetailsInformation.Add(model);
            db.SaveChanges();
            return model;
        }

        public SessionSenderBankAccountDeposit AddSessionSenderBankAccountDeposit(SenderBankAccountDepositVm vm)
        {
            var SerializedModel = webServices.SerializeObject<SenderBankAccountDepositVm>(vm);
            var model = webServices.DeserializeObject<SessionSenderBankAccountDeposit>
                (SerializedModel).Result;
            db.SessionSenderBankAccountDeposit.Add(model);
            db.SaveChanges();
            return model;
        }

        public void ClearSessionTransactionSummary(string SessionId)
        {

            var data = db.SessionTransactionSummary.Where(x => x.SessionId == SessionId).FirstOrDefault();
            if (data != null)
            {
                db.SessionTransactionSummary.Remove(data);
                db.SaveChanges();

            }
        }

    }
}