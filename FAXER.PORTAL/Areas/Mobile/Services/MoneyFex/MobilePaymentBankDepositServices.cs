using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Mobile.Models.MobilePaymentSummary;
using FAXER.PORTAL.Areas.Mobile.Models.MoneyFex.BankDeposit;
using FAXER.PORTAL.BankApi.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using Stripe;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TransferZero.Sdk.Model;
using Twilio.Rest.Trunking.V1;

namespace FAXER.PORTAL.Areas.Mobile.Services.MoneyFex
{
    public class MobilePaymentBankDepositServices
    {
        FAXEREntities dbContext = null;
        public MobilePaymentBankDepositServices()
        {
            dbContext = new FAXEREntities();
        }

        public ServiceResult<bool> HasTransactionReceivedToPaymentGateway(string ReceiptNo)
        {

            bool transactionHasBeenReceivedToPaymentGatway = false;
            var transactionDetail = dbContext.SecureTradingApiResponseTransactionLog.
                                     Where(x => x.orderreference == ReceiptNo
                                     && x.requesttypedescription == "AUTH"
                                     && x.errorcode == "0"
                                     && x.status == "Y"
                                     && x.settlestatus != "2"
                                     && x.settlestatus != "3").FirstOrDefault();

            if (transactionDetail != null)
                transactionHasBeenReceivedToPaymentGatway = true;

            return new ServiceResult<bool>()
            {
                Data = transactionHasBeenReceivedToPaymentGatway,
                Status = ResultStatus.OK
            };


        }

        public ServiceResult<MobilePaymentBankDepositVm> SaveIncompleteBankTransaction(MobilePaymentBankDepositVm vm)
        {
            var savedBankTransaction = dbContext.BankAccountDeposit.Where(x => x.ReceiptNo == vm.ReceiptNo).FirstOrDefault();
            if (savedBankTransaction != null)
            {
                vm.TransactionId = savedBankTransaction.Id;
                return new ServiceResult<MobilePaymentBankDepositVm>()
                {
                    Data = vm,
                    Message = "Transaction saved",
                    Status = ResultStatus.OK
                };

            }
            else
            {
                int RecipientId = 0;
                var Recipent = dbContext.Recipients.Where(x => x.ReceiverName.ToLower() == vm.ReceiverDetail.ReceiverName.ToLower() &&
                                        x.MobileNo == vm.ReceiverDetail.MobileNo && x.Service == Service.BankAccount).FirstOrDefault();
                int BankId = getbankIdFromBankCode(vm.ReceiverDetail.BranchORBankORSwiftCode);
                if (Recipent == null)
                {
                    Recipients model = new Recipients()
                    {
                        Country = vm.PaymentSummary.ReceivingCountry,
                        SenderId = vm.SenderId,
                        MobileNo = vm.ReceiverDetail.MobileNo,
                        Service = Service.BankAccount,
                        ReceiverName = vm.ReceiverDetail.ReceiverName,
                        BankId = BankId,
                        BranchCode = vm.ReceiverDetail.BranchORBankORSwiftCode,
                        AccountNo = vm.ReceiverDetail.ReceiverAccountNo,
                        City = vm.ReceiverDetail.ReceiverCity,
                        Email = vm.ReceiverDetail.ReceiverEmailAddress,
                        PostalCode = vm.ReceiverDetail.ReceiverPostCode,
                        Street = vm.ReceiverDetail.ReceiverAddress
                    };
                    var AddRecipient = dbContext.Recipients.Add(model);
                    dbContext.SaveChanges();
                    RecipientId = AddRecipient.Id;
                }
                else
                {
                    RecipientId = Recipent.Id;
                }

                BankAccountDeposit bankAccountDeposit = new BankAccountDeposit()
                {
                    Status = BankDepositStatus.PaymentPending,
                    BankCode = vm.ReceiverDetail.BranchORBankORSwiftCode,
                    BankName = vm.ReceiverDetail.BankName,
                    BankId = BankId,
                    ExchangeRate = vm.PaymentSummary.ExchangeRate,
                    Fee = vm.PaymentSummary.Fee,
                    IsBusiness = vm.ReceiverDetail.IsBusiness,
                    PaidFromModule = Module.Faxer,
                    ReasonForTransfer = vm.ReceiverDetail.ReasonForTransfer,
                    ReceiptNo = vm.ReceiptNo,
                    ReceiverAccountNo = vm.ReceiverDetail.ReceiverAccountNo,
                    ReceiverMobileNo = vm.ReceiverDetail.MobileNo,
                    ReceiverCountry = vm.PaymentSummary.ReceivingCountry,
                    ReceivingCountry = vm.PaymentSummary.ReceivingCountry,
                    ReceiverName = vm.ReceiverDetail.ReceiverName,
                    ReceivingAmount = vm.PaymentSummary.ReceivingAmount,
                    SendingAmount = vm.PaymentSummary.SendingAmount,
                    SendingCountry = vm.PaymentSummary.SendingCountry,
                    TotalAmount = vm.PaymentSummary.TotalAmount,
                    TransactionDate = DateTime.Now,
                    SenderId = vm.SenderId,
                    RecipientId = RecipientId,
                    IsManualDeposit = IsManualBankDeposit(vm.PaymentSummary.SendingCountry, vm.PaymentSummary.ReceivingCountry),
                    IsEuropeTransfer = IsEuropeTransfer(vm.PaymentSummary.ReceivingCountry),
                    SendingCurrency = vm.PaymentSummary.SendingCurrencyCode,
                    ReceivingCurrency = vm.PaymentSummary.ReceivingCurrencyCode
                };
                Log.Write("Transaction Saved " + bankAccountDeposit.ReceiptNo);
                dbContext.BankAccountDeposit.Add(bankAccountDeposit);
                dbContext.SaveChanges();
                vm.TransactionId = bankAccountDeposit.Id;
                return new ServiceResult<MobilePaymentBankDepositVm>()
                {
                    Data = vm,
                    Message = "Transaction saved",
                    Status = ResultStatus.OK
                };
            }
        }

        internal bool SendTransactionPendingEmail(int transactionId, TransactionTransferMethod transactionTransferMethod)
        {
            TransactionPendingViewModel model = new TransactionPendingViewModel();
            switch (transactionTransferMethod)
            {
                case TransactionTransferMethod.BankDeposit:
                    var bank = dbContext.BankAccountDeposit.Where(x => x.Id == transactionId).FirstOrDefault();
                    model = new TransactionPendingViewModel()
                    {
                        BankAccount = bank.ReceiverAccountNo,
                        BankCode = bank.BankCode,
                        BankName = bank.BankName,
                        ExchangeRate = bank.ExchangeRate,
                        Fee = bank.Fee,
                        MFCN = "",
                        MobileNo = bank.ReceiverMobileNo,
                        ReceiptNumber = bank.ReceiptNo,
                        ReceiverFullName = bank.ReceiverName,
                        ReceivingCountry = bank.ReceivingCountry,
                        Receivingurrency = FAXER.PORTAL.Common.Common.GetCountryCurrency(bank.ReceivingCountry),
                        SenderId = bank.SenderId,
                        SendingAmount = bank.SendingAmount,
                        SendingCurrency = FAXER.PORTAL.Common.Common.GetCountryCurrency(bank.SendingCountry),
                        TransactionId = bank.Id,
                        TransferMethod = TransactionServiceType.BankDeposit,
                        WalletName = "",
                        TransactionNumber = bank.ReceiptNo,
                        IsTransactionPending = true
                    };
                    break;
                case TransactionTransferMethod.OtherWallet:
                    var otherWallet = dbContext.MobileMoneyTransfer.Where(x => x.Id == transactionId).FirstOrDefault();
                    var walletName = dbContext.MobileWalletOperator.Where(x => x.Id == otherWallet.WalletOperatorId).Select(x => x.Name).FirstOrDefault();
                    model = new TransactionPendingViewModel()
                    {
                        BankAccount = "",
                        BankCode = "",
                        BankName = "",
                        ExchangeRate = otherWallet.ExchangeRate,
                        Fee = otherWallet.Fee,
                        MFCN = "",
                        MobileNo = otherWallet.PaidToMobileNo,
                        ReceiptNumber = otherWallet.ReceiptNo,
                        ReceiverFullName = otherWallet.ReceiverName,
                        ReceivingCountry = otherWallet.ReceivingCountry,
                        Receivingurrency = FAXER.PORTAL.Common.Common.GetCountryCurrency(otherWallet.ReceivingCountry),
                        SenderId = otherWallet.SenderId,
                        SendingAmount = otherWallet.SendingAmount,
                        SendingCurrency = FAXER.PORTAL.Common.Common.GetCountryCurrency(otherWallet.SendingCountry),
                        TransactionId = otherWallet.Id,
                        TransferMethod = TransactionServiceType.MobileWallet,
                        WalletName = walletName,
                        TransactionNumber = otherWallet.ReceiptNo,
                        IsTransactionPending = true
                    };
                    break;
                case TransactionTransferMethod.CashPickUp:
                    var cashPickUp = dbContext.FaxingNonCardTransaction.Where(x => x.Id == transactionId).FirstOrDefault();
                    var recipentDetails = dbContext.Recipients.Where(x => x.Id == cashPickUp.RecipientId).FirstOrDefault();
                    model = new TransactionPendingViewModel()
                    {
                        BankAccount = "",
                        BankCode = "",
                        BankName = "",
                        ExchangeRate = cashPickUp.ExchangeRate,
                        Fee = cashPickUp.FaxingFee,
                        MFCN = cashPickUp.MFCN,
                        MobileNo = recipentDetails.MobileNo,
                        ReceiptNumber = cashPickUp.ReceiptNumber,
                        ReceiverFullName = recipentDetails.ReceiverName,
                        ReceivingCountry = cashPickUp.ReceivingCountry,
                        Receivingurrency = FAXER.PORTAL.Common.Common.GetCountryCurrency(cashPickUp.ReceivingCountry),
                        SenderId = cashPickUp.SenderId,
                        SendingAmount = cashPickUp.FaxingAmount,
                        SendingCurrency = FAXER.PORTAL.Common.Common.GetCountryCurrency(cashPickUp.SendingCountry),
                        TransactionId = cashPickUp.Id,
                        TransferMethod = TransactionServiceType.CashPickUp,
                        WalletName = "",
                        TransactionNumber = cashPickUp.ReceiptNumber,
                        IsTransactionPending = true
                    };
                    break;
                default:
                    break;
            }
            SSenderBankAccountDeposit _sBankDeposit = new SSenderBankAccountDeposit();
            var senderInfo = _sBankDeposit.GetSenderInfo(model.SenderId);
            string email = senderInfo.Email;
            string senderFirstName = senderInfo.FirstName;
            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";

            body = FAXER.PORTAL.Common.Common.GetTemplate(baseUrl + "/EmailTemplate/PaymentPending?" + "&SenderFristName=" + senderFirstName + "&TransactionNumber=" +
                                             model.TransactionNumber + "&SendingAmount=" + model.SendingAmount + "&SendingCurrency=" + model.SendingCurrency + "&ExchangeRate=" +
                                            model.ExchangeRate + "&Receivingurrency=" + model.Receivingurrency + "&Receivingcountry=" + model.ReceivingCountry + "&Fee=" + model.Fee
                                             + "&ReceiverName=" + model.ReceiverFullName + "&BankName=" + model.BankName + "&BankAccount=" + model.BankAccount + "&BankCode=" + model.BankCode
             + "&TransactionServiceType=" + model.TransferMethod + "&WalletName=" + model.WalletName + "&MFCN=" + model.MFCN + "&TransactionId=" + model.TransactionId
             + "&MobileNo=" + model.MobileNo);

            mail.SendMail(email, "Payment pending" + " " + model.ReceiptNumber, body);
            return true;
        }

        public ServiceResult<MobilePaymentBankDepositVm> UpdateIncompleteBankTransaction(MobilePaymentBankDepositVm vm)
        {
            int RecipientId = 0;
            var Recipent = dbContext.Recipients.Where(x => x.ReceiverName.ToLower() == vm.ReceiverDetail.ReceiverName.ToLower() &&
                                    x.MobileNo == vm.ReceiverDetail.MobileNo && x.Service == Service.BankAccount).FirstOrDefault();
            int BankId = getbankIdFromBankCode(vm.ReceiverDetail.BranchORBankORSwiftCode);
            if (Recipent == null)
            {
                Recipients model = new Recipients()
                {
                    Country = vm.PaymentSummary.ReceivingCountry,
                    SenderId = vm.SenderId,
                    MobileNo = vm.ReceiverDetail.MobileNo,
                    Service = Service.BankAccount,
                    ReceiverName = vm.ReceiverDetail.ReceiverName,
                    BankId = BankId,
                    BranchCode = vm.ReceiverDetail.BranchORBankORSwiftCode,
                    AccountNo = vm.ReceiverDetail.ReceiverAccountNo,
                    City = vm.ReceiverDetail.ReceiverCity,
                    Street = vm.ReceiverDetail.ReceiverAddress,
                    PostalCode = vm.ReceiverDetail.ReceiverPostCode,
                    Email = vm.ReceiverDetail.ReceiverEmailAddress,

                };
                var AddRecipient = dbContext.Recipients.Add(model);
                dbContext.SaveChanges();
                RecipientId = AddRecipient.Id;
            }
            else
            {
                RecipientId = Recipent.Id;
            }
            var data = dbContext.BankAccountDeposit.Where(x => x.Id == vm.TransactionId).FirstOrDefault();

            data.Status = BankDepositStatus.PaymentPending;
            data.BankId = BankId;
            data.BankCode = vm.ReceiverDetail.BranchORBankORSwiftCode;
            data.ExchangeRate = vm.PaymentSummary.ExchangeRate;
            data.Fee = vm.PaymentSummary.Fee;
            data.IsBusiness = vm.ReceiverDetail.IsBusiness;
            data.IsManualDeposit = IsManualBankDeposit(vm.PaymentSummary.SendingCountry, vm.PaymentSummary.ReceivingCountry);
            data.PaidFromModule = Module.Faxer;
            data.ReceiverAccountNo = vm.ReceiverDetail.ReceiverAccountNo;
            data.ReceiverMobileNo = vm.ReceiverDetail.MobileNo;
            data.ReceiverCountry = vm.PaymentSummary.ReceivingCountry;
            data.ReceiverName = vm.ReceiverDetail.ReceiverName;
            data.ReceivingAmount = vm.PaymentSummary.ReceivingAmount;
            data.ReceivingCountry = vm.PaymentSummary.ReceivingCountry;
            data.SendingAmount = vm.PaymentSummary.SendingAmount;
            data.TransactionDate = DateTime.Now;
            data.SenderId = vm.SenderId;
            data.TotalAmount = vm.PaymentSummary.TotalAmount;
            data.SendingCountry = vm.PaymentSummary.SendingCountry;
            data.SendingCurrency = vm.PaymentSummary.SendingCurrencyCode;
            data.ReceivingCurrency = vm.PaymentSummary.ReceivingCurrencyCode;
            data.RecipientId = RecipientId;
            vm.TransactionId = data.Id;
            dbContext.Entry<BankAccountDeposit>(data).State = EntityState.Modified;
            dbContext.SaveChanges();
            return new ServiceResult<MobilePaymentBankDepositVm>()
            {
                Data = vm,
                Message = "Transaction saved",
                Status = ResultStatus.OK
            };

        }

        public ServiceResult<MobilePaymentBankDepositVm> SaveIncompleteMobileWalletTransaction(MobilePaymentBankDepositVm vm)
        {
            var savedMobileTransaction = dbContext.MobileMoneyTransfer.Where(x => x.Id == vm.TransactionId).FirstOrDefault();
            if (savedMobileTransaction != null)
            {
                int RecipientId = 0;
                var Recipent = dbContext.Recipients.Where(x => x.ReceiverName.ToLower() == vm.ReceiverDetail.ReceiverName.ToLower() &&
                                        x.MobileNo == vm.ReceiverDetail.MobileNo && x.Service == Service.MobileWallet).FirstOrDefault();
                int BankId = getbankIdFromBankCode(vm.ReceiverDetail.BranchORBankORSwiftCode);
                if (Recipent == null)
                {
                    Recipients model = new Recipients()
                    {
                        Country = vm.PaymentSummary.ReceivingCountry,
                        SenderId = vm.SenderId,
                        MobileNo = vm.ReceiverDetail.MobileNo,
                        Service = Service.BankAccount,
                        ReceiverName = vm.ReceiverDetail.ReceiverName,
                        BankId = BankId,
                        BranchCode = vm.ReceiverDetail.BranchORBankORSwiftCode,
                        AccountNo = vm.ReceiverDetail.ReceiverAccountNo
                    };
                    var AddRecipient = dbContext.Recipients.Add(model);
                    dbContext.SaveChanges();
                    RecipientId = AddRecipient.Id;
                }
                else
                {
                    RecipientId = Recipent.Id;
                }
                savedMobileTransaction.ExchangeRate = vm.PaymentSummary.ExchangeRate;
                savedMobileTransaction.Fee = vm.PaymentSummary.Fee;
                savedMobileTransaction.PaidFromModule = Module.Faxer;
                savedMobileTransaction.TransactionDate = DateTime.Now;
                savedMobileTransaction.TotalAmount = vm.PaymentSummary.TotalAmount;
                savedMobileTransaction.PaidToMobileNo = vm.MobileWallet.WalletNo;
                savedMobileTransaction.ReceivingAmount = vm.PaymentSummary.ReceivingAmount;
                savedMobileTransaction.SenderId = vm.SenderId;
                savedMobileTransaction.ReceivingCountry = vm.PaymentSummary.ReceivingCountry;
                savedMobileTransaction.SendingAmount = vm.PaymentSummary.SendingAmount;
                savedMobileTransaction.SendingCountry = vm.PaymentSummary.SendingCountry;
                savedMobileTransaction.ReceiptNo = vm.ReceiptNo;
                savedMobileTransaction.ReceiverName = vm.MobileWallet.ReceiverName;
                savedMobileTransaction.WalletOperatorId = vm.MobileWallet.WalletId;
                savedMobileTransaction.RecipientId = RecipientId;
                savedMobileTransaction.SendingCurrency = vm.PaymentSummary.SendingCurrencyCode;
                savedMobileTransaction.ReceivingCurrency = vm.PaymentSummary.ReceivingCurrencyCode;
                savedMobileTransaction.Status = MobileMoneyTransferStatus.PaymentPending;
                dbContext.Entry(savedMobileTransaction).State = EntityState.Modified;
                dbContext.SaveChanges();
                vm.TransactionId = savedMobileTransaction.Id;
                return new ServiceResult<MobilePaymentBankDepositVm>()
                {
                    Data = vm,
                    Message = "Transaction saved",
                    Status = ResultStatus.OK
                };

            }
            else
            {

                int RecipientId = 0;
                var Recipent = dbContext.Recipients.Where(x => x.ReceiverName.ToLower() == vm.ReceiverDetail.ReceiverName.ToLower() &&
                                        x.MobileNo == vm.ReceiverDetail.MobileNo && x.Service == Service.MobileWallet).FirstOrDefault();

                if (Recipent == null)
                {
                    Recipients model = new Recipients()
                    {
                        Country = vm.PaymentSummary.ReceivingCountry,
                        SenderId = vm.SenderId,
                        MobileNo = vm.ReceiverDetail.MobileNo,
                        Service = Service.MobileWallet,
                        ReceiverName = vm.ReceiverDetail.ReceiverName,
                        BankId = 0,
                        BranchCode = vm.ReceiverDetail.BranchORBankORSwiftCode,
                        AccountNo = vm.ReceiverDetail.MobileNo
                    };
                    var AddRecipient = dbContext.Recipients.Add(model);
                    dbContext.SaveChanges();
                    RecipientId = AddRecipient.Id;
                }
                else
                {
                    RecipientId = Recipent.Id;
                }
                MobileMoneyTransfer mobileTransferData = new MobileMoneyTransfer()
                {
                    RecipientId = RecipientId,
                    Status = MobileMoneyTransferStatus.PaymentPending,
                    SendingCountry = vm.PaymentSummary.SendingCountry,
                    ReceivingCountry = vm.PaymentSummary.ReceivingCountry,
                    ExchangeRate = vm.PaymentSummary.ExchangeRate,
                    SendingAmount = vm.PaymentSummary.SendingAmount,
                    Fee = vm.PaymentSummary.Fee,
                    PaidFromModule = Module.Faxer,
                    PaidToMobileNo = vm.MobileWallet.WalletNo,
                    ReceiptNo = vm.ReceiptNo,
                    TotalAmount = vm.PaymentSummary.TotalAmount,
                    ReceiverName = vm.MobileWallet.ReceiverName,
                    ReceivingAmount = vm.PaymentSummary.ReceivingAmount,
                    SenderId = vm.SenderId,
                    WalletOperatorId = vm.MobileWallet.WalletId,
                    TransactionDate = DateTime.Now,
                    SendingCurrency = vm.PaymentSummary.SendingCurrencyCode,
                    ReceivingCurrency = vm.PaymentSummary.ReceivingCurrencyCode
                };
                if (mobileTransferData.SendingCountry == mobileTransferData.ReceivingCountry)
                {
                    mobileTransferData.PaymentType = PaymentType.Local;
                }
                else
                {
                    mobileTransferData.PaymentType = PaymentType.International;
                }
                dbContext.MobileMoneyTransfer.Add(mobileTransferData);
                dbContext.SaveChanges();
                vm.TransactionId = mobileTransferData.Id;
                return new ServiceResult<MobilePaymentBankDepositVm>()
                {
                    Data = vm,
                    Message = "Transaction saved",
                    Status = ResultStatus.OK
                };
            }
        }

        internal ServiceResult<bool> HasRecipientTransacionExceedLimit(int senderId, int receipientId, string sendingCountry, string receivingCountry, TransactionTransferMethod transferMethod)
        {
            bool hasRecipientTransacionExceedLimit = FAXER.PORTAL.Common.Common.HasExceededReceiverLimit(senderId,
                       receipientId, sendingCountry, receivingCountry, transferMethod);
            return new ServiceResult<bool>()
            {
                Data = hasRecipientTransacionExceedLimit,
                Message = "Transaction saved",
                Status = ResultStatus.OK
            };
        }
        internal ServiceResult<bool> HasSenderTransacionExceedLimit(int senderId, string sendingCountry, string receivingCountry, TransactionTransferMethod transferMethod)
        {
            bool HasSenderTransacionExceedLimit = FAXER.PORTAL.Common.Common.HasExceededSenderTransactionLimit(senderId,
                       sendingCountry, receivingCountry, transferMethod);
            return new ServiceResult<bool>()
            {
                Data = HasSenderTransacionExceedLimit,
                Message = "Transaction saved",
                Status = ResultStatus.OK
            };
        }

        public ServiceResult<MobilePaymentBankDepositVm> SaveIncompleteCashPickUpTransaction(MobilePaymentBankDepositVm vm)
        {
            var savedCashPickpTransaction = dbContext.FaxingNonCardTransaction.Where(x => x.Id == vm.TransactionId).FirstOrDefault();
            if (savedCashPickpTransaction != null)
            {
                int RecipientId = 0;
                var Recipent = dbContext.Recipients.Where(x => x.ReceiverName.ToLower() == vm.ReceiverDetail.ReceiverName.ToLower() &&
                                        x.MobileNo == vm.ReceiverDetail.MobileNo && x.Service == Service.CashPickUP).FirstOrDefault();
                if (Recipent == null)
                {
                    Recipients model = new Recipients()
                    {
                        Country = vm.PaymentSummary.ReceivingCountry,
                        SenderId = vm.SenderId,
                        MobileNo = vm.ReceiverDetail.MobileNo,
                        Service = Service.CashPickUP,
                        ReceiverName = vm.ReceiverDetail.ReceiverName,
                        BankId = 0,
                        BranchCode = vm.ReceiverDetail.BranchORBankORSwiftCode,
                        AccountNo = vm.ReceiverDetail.ReceiverAccountNo,
                        IdentificationTypeId = vm.ReceiverDetail.RecipientIdentityCardId,
                        IdentificationNumber = vm.ReceiverDetail.RecipientIdenityCardNumber

                    };
                    var AddRecipient = dbContext.Recipients.Add(model);
                    dbContext.SaveChanges();
                    RecipientId = AddRecipient.Id;
                }
                else
                {
                    RecipientId = Recipent.Id;
                }

                string[] splittedName = vm.ReceiverDetail.ReceiverName.Trim().Split(null);
                DB.ReceiversDetails recDetailObj = new DB.ReceiversDetails()
                {
                    City = "",
                    CreatedDate = System.DateTime.Now,
                    Country = vm.PaymentSummary.ReceivingCountry,
                    //EmailAddress = cashPickUP.EmailAddress,
                    FaxerID = vm.SenderId,
                    FullName = vm.ReceiverDetail.ReceiverName,
                    IsDeleted = false,
                    PhoneNumber = vm.ReceiverDetail.MobileNo,
                    FirstName = splittedName[0],
                    MiddleName = splittedName.Count() > 2 ? splittedName[1] : " ",
                    LastName = splittedName[splittedName.Count() - 1]
                };
                int NonCardReceiveId;
                SReceiverDetails receiverService = new SReceiverDetails();
                var recevierExist = dbContext.ReceiversDetails.Where(x => x.FullName == recDetailObj.FullName && x.PhoneNumber == recDetailObj.PhoneNumber).FirstOrDefault();
                if (recevierExist == null)
                {
                    receiverService.Add(recDetailObj);
                    NonCardReceiveId = recDetailObj.Id;
                }
                else
                {
                    NonCardReceiveId = recevierExist.Id;
                }


                savedCashPickpTransaction.ExchangeRate = vm.PaymentSummary.ExchangeRate;
                savedCashPickpTransaction.FaxingFee = vm.PaymentSummary.Fee;
                savedCashPickpTransaction.TransactionDate = DateTime.Now;
                savedCashPickpTransaction.TotalAmount = vm.PaymentSummary.TotalAmount;
                savedCashPickpTransaction.ReceivingAmount = vm.PaymentSummary.ReceivingAmount;
                savedCashPickpTransaction.SenderId = vm.SenderId;
                savedCashPickpTransaction.ReceivingCountry = vm.PaymentSummary.ReceivingCountry;
                savedCashPickpTransaction.FaxingAmount = vm.PaymentSummary.SendingAmount;
                savedCashPickpTransaction.SendingCountry = vm.PaymentSummary.SendingCountry;
                savedCashPickpTransaction.SendingCurrency = vm.PaymentSummary.SendingCurrencyCode;
                savedCashPickpTransaction.ReceivingCurrency = vm.PaymentSummary.ReceivingCurrencyCode;
                savedCashPickpTransaction.ReceiptNumber = vm.ReceiptNo;
                savedCashPickpTransaction.RecipientId = RecipientId;
                savedCashPickpTransaction.FaxingStatus = FaxingStatus.PaymentPending;
                savedCashPickpTransaction.RecipientIdentityCardId = vm.ReceiverDetail.RecipientIdentityCardId;
                savedCashPickpTransaction.RecipientIdenityCardNumber = vm.ReceiverDetail.RecipientIdenityCardNumber;
                dbContext.Entry(savedCashPickpTransaction).State = EntityState.Modified;
                dbContext.SaveChanges();
                vm.TransactionId = savedCashPickpTransaction.Id;
                return new ServiceResult<MobilePaymentBankDepositVm>()
                {
                    Data = vm,
                    Message = "Transaction saved",
                    Status = ResultStatus.OK
                };

            }
            else
            {

                int RecipientId = 0;
                var Recipent = dbContext.Recipients.Where(x => x.ReceiverName.ToLower() == vm.ReceiverDetail.ReceiverName.ToLower() &&
                                        x.MobileNo == vm.ReceiverDetail.MobileNo && x.Service == Service.CashPickUP).FirstOrDefault();

                if (Recipent == null)
                {
                    Recipients model = new Recipients()
                    {
                        Country = vm.PaymentSummary.ReceivingCountry,
                        SenderId = vm.SenderId,
                        MobileNo = vm.ReceiverDetail.MobileNo,
                        Service = Service.CashPickUP,
                        ReceiverName = vm.ReceiverDetail.ReceiverName,
                        BankId = 0,
                        BranchCode = vm.ReceiverDetail.BranchORBankORSwiftCode,
                        AccountNo = vm.ReceiverDetail.MobileNo
                    };
                    var AddRecipient = dbContext.Recipients.Add(model);
                    dbContext.SaveChanges();
                    RecipientId = AddRecipient.Id;
                }
                else
                {
                    RecipientId = Recipent.Id;
                }

                string[] splittedName = vm.ReceiverDetail.ReceiverName.Trim().Split(null);
                DB.ReceiversDetails recDetailObj = new DB.ReceiversDetails()
                {
                    City = "",
                    CreatedDate = System.DateTime.Now,
                    Country = vm.PaymentSummary.ReceivingCountry,
                    //EmailAddress = cashPickUP.EmailAddress,
                    FaxerID = vm.SenderId,
                    FullName = vm.ReceiverDetail.ReceiverName,
                    IsDeleted = false,
                    PhoneNumber = vm.ReceiverDetail.MobileNo,
                    FirstName = splittedName[0],
                    MiddleName = splittedName.Count() > 2 ? splittedName[1] : " ",
                    LastName = splittedName[splittedName.Count() - 1]
                };
                int NonCardReceiveId;
                SReceiverDetails receiverService = new SReceiverDetails();
                var recevierExist = dbContext.ReceiversDetails.Where(x => x.FullName == recDetailObj.FullName && x.PhoneNumber == recDetailObj.PhoneNumber).FirstOrDefault();
                if (recevierExist == null)
                {
                    receiverService.Add(recDetailObj);
                    NonCardReceiveId = recDetailObj.Id;
                }
                else
                {
                    NonCardReceiveId = recevierExist.Id;
                }

                SFaxingNonCardTransaction _faxingNonCardTransactionService = new SFaxingNonCardTransaction();
                string MFCN = _faxingNonCardTransactionService.GetNewMFCNToSave();
                DB.FaxingNonCardTransaction cashpickUp = new DB.FaxingNonCardTransaction()
                {
                    NonCardRecieverId = NonCardReceiveId,
                    UserId = 0,
                    FaxingStatus = FaxingStatus.PaymentPending,
                    ReceiptNumber = vm.ReceiptNo,
                    FaxingMethod = "PM001",
                    FaxingAmount = vm.PaymentSummary.SendingAmount,
                    ReceivingAmount = vm.PaymentSummary.ReceivingAmount,
                    ExchangeRate = vm.PaymentSummary.ExchangeRate,
                    FaxingFee = vm.PaymentSummary.Fee,
                    TotalAmount = vm.PaymentSummary.TotalAmount,
                    TransactionDate = System.DateTime.Now,
                    SendingCountry = vm.PaymentSummary.SendingCountry,
                    ReceivingCountry = vm.PaymentSummary.ReceivingCountry,
                    RecipientId = RecipientId,
                    MFCN = vm.ReceiptNo,
                    SenderId = vm.SenderId,
                    RecipientIdenityCardNumber = vm.ReceiverDetail.RecipientIdenityCardNumber,
                    RecipientIdentityCardId = vm.ReceiverDetail.RecipientIdentityCardId,
                    SendingCurrency = vm.PaymentSummary.SendingCurrencyCode,
                    ReceivingCurrency = vm.PaymentSummary.ReceivingCurrencyCode,
                };


                dbContext.FaxingNonCardTransaction.Add(cashpickUp);
                dbContext.SaveChanges();
                vm.TransactionId = cashpickUp.Id;

                return new ServiceResult<MobilePaymentBankDepositVm>()
                {
                    Data = vm,
                    Message = "Transaction saved",
                    Status = ResultStatus.OK
                };

            }
        }

        public ServiceResult<BankAccountDeposit> CompleteTransaction(MobilePaymentBankDepositVm vm)
        {
            //var bankDepositData = dbContext.BankAccountDeposit.Where(x => x.ReceiptNo == vm.ReceiptNo).FirstOrDefault();
            //if (bankDepositData != null)
            //{
            //    return new ServiceResult<BankAccountDeposit>()
            //    {
            //        Data = bankDepositData,
            //        Message = "Transaction Completed",
            //        Status = ResultStatus.OK
            //    };
            //}
            //else
            //{

            var bankAccountDeposit = dbContext.BankAccountDeposit.Where(x => x.Id == vm.TransactionId).FirstOrDefault();
            if (bankAccountDeposit == null)
            {

                bankAccountDeposit = new BankAccountDeposit();
            }
            Log.Write("Mobile Api : " + vm.ReceiptNo);
            decimal ExtraFee = 0;
            BankDepositStatus bankDepositStatus = BankDepositStatus.Incomplete;

            var stripeCreateTransaction = new StripeCreateTransactionVM()
            {
                Currency = vm.PaymentSummary.SendingCurrencyCode,
                ReceivingCurrency = vm.PaymentSummary.ReceivingCurrencyCode,
                SendingCountry = vm.PaymentSummary.SendingCountry,
                ReceivingCountry = vm.MobileWallet.CountryCode,
                Amount = vm.PaymentSummary.SendingAmount,
            };
            var cardProcessor = StripServices.GetCardProcessor(stripeCreateTransaction,
                                                             TransactionTransferType.Online, TransactionTransferMethod.BankDeposit);

            switch (vm.PaymentMethodDetail.PaymentMode)
            {
                case PORTAL.Models.SenderPaymentMode.CreditDebitCard:
                    ExtraFee = StripServices.GetExtraFeeAmountIfCreditCard(new StripeResultIsValidCardVm()
                    {
                        Number = vm.PaymentMethodDetail.CreditDebitCardDetail.CardNumber,
                        ExpirationMonth = vm.PaymentMethodDetail.CreditDebitCardDetail.EndMonth,
                        ExpiringYear = vm.PaymentMethodDetail.CreditDebitCardDetail.EndYear,
                        CurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyCode(vm.PaymentSummary.SendingCountry),
                        SecurityCode = vm.PaymentMethodDetail.CreditDebitCardDetail.SecurityCode

                    }, vm.PaymentSummary.TotalAmount);
                    if (vm.IsIdCheckIsProgress == true)
                    {

                        bankDepositStatus = BankDepositStatus.IdCheckInProgress;
                    }
                    else
                    {
                        bankDepositStatus = BankDepositStatus.Incomplete;
                    }
                    bankAccountDeposit.CardProcessorApi = cardProcessor;
                    break;
                case PORTAL.Models.SenderPaymentMode.SavedDebitCreditCard:
                    ExtraFee = StripServices.GetExtraFeeAmountIfCreditCard(new StripeResultIsValidCardVm()
                    {
                        Number = vm.PaymentMethodDetail.CreditDebitCardDetail.CardNumber,
                        ExpirationMonth = vm.PaymentMethodDetail.CreditDebitCardDetail.EndMonth,
                        ExpiringYear = vm.PaymentMethodDetail.CreditDebitCardDetail.EndYear,
                        CurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyCode(vm.PaymentSummary.SendingCountry),
                        SecurityCode = vm.PaymentMethodDetail.CreditDebitCardDetail.SecurityCode

                    }, vm.PaymentSummary.TotalAmount);
                    if (vm.IsIdCheckIsProgress == true)
                    {

                        bankDepositStatus = BankDepositStatus.IdCheckInProgress;
                    }
                    else
                    {
                        bankDepositStatus = BankDepositStatus.Incomplete;
                    }
                    bankAccountDeposit.CardProcessorApi = cardProcessor;
                    break;
                case PORTAL.Models.SenderPaymentMode.KiiPayWallet:
                    break;
                case PORTAL.Models.SenderPaymentMode.MoneyFexBankAccount:
                    bankDepositStatus = BankDepositStatus.PendingBankdepositConfirmtaion;
                    break;
                case PORTAL.Models.SenderPaymentMode.Cash:
                    break;
                default:
                    break;
            }

            int RecipientId = 0;
            var Recipent = dbContext.Recipients.Where(x => x.ReceiverName.ToLower() == vm.ReceiverDetail.ReceiverName.ToLower() &&
                                    x.MobileNo == vm.ReceiverDetail.MobileNo && x.Service == Service.BankAccount).FirstOrDefault();
            int BankId = getbankIdFromBankCode(vm.ReceiverDetail.BranchORBankORSwiftCode);

            if (Recipent == null)
            {
                Recipients model = new Recipients()
                {
                    Country = vm.PaymentSummary.ReceivingCountry,
                    SenderId = vm.SenderId,
                    MobileNo = vm.ReceiverDetail.MobileNo,
                    Service = Service.BankAccount,
                    ReceiverName = vm.ReceiverDetail.ReceiverName,
                    BankId = BankId,
                    BranchCode = vm.ReceiverDetail.BranchORBankORSwiftCode,
                    AccountNo = vm.ReceiverDetail.ReceiverAccountNo
                };
                var AddRecipient = dbContext.Recipients.Add(model);
                dbContext.SaveChanges();
                RecipientId = AddRecipient.Id;
            }
            else
            {
                RecipientId = Recipent.Id;
            }

            var ApiService = FAXER.PORTAL.Common.Common.GetApiservice(
                vm.PaymentSummary.SendingCountry, vm.PaymentSummary.ReceivingCountry,
                vm.PaymentSummary.SendingAmount,
                TransactionTransferMethod.BankDeposit, TransactionTransferType.Online);

            bankAccountDeposit.BankCode = vm.ReceiverDetail.BranchORBankORSwiftCode;
            bankAccountDeposit.BankName = vm.ReceiverDetail.BankName;
            bankAccountDeposit.BankId = BankId;
            bankAccountDeposit.ExchangeRate = vm.PaymentSummary.ExchangeRate;
            bankAccountDeposit.ExtraFee = ExtraFee;
            bankAccountDeposit.Fee = vm.PaymentSummary.Fee;
            bankAccountDeposit.HasMadePaymentToBankAccount =
                                        vm.PaymentMethodDetail.PaymentMode ==
                                        PORTAL.Models.SenderPaymentMode.MoneyFexBankAccount ? true : false;
            bankAccountDeposit.IsBusiness = vm.ReceiverDetail.IsBusiness;
            bankAccountDeposit.PaidFromModule = Module.Faxer;
            bankAccountDeposit.PaymentReference = vm.PaymentMethodDetail.BankPayment.Reference;
            bankAccountDeposit.PaymentType = vm.PaymentSummary.PaymentType;
            bankAccountDeposit.ReasonForTransfer = vm.ReceiverDetail.ReasonForTransfer;
            bankAccountDeposit.ReceiptNo = vm.ReceiptNo;
            bankAccountDeposit.ReceiverAccountNo = vm.ReceiverDetail.ReceiverAccountNo;
            bankAccountDeposit.ReceiverMobileNo = vm.ReceiverDetail.MobileNo;
            bankAccountDeposit.ReceiverCountry = vm.PaymentSummary.ReceivingCountry;
            bankAccountDeposit.ReceivingCountry = vm.PaymentSummary.ReceivingCountry;
            bankAccountDeposit.ReceiverName = vm.ReceiverDetail.ReceiverName;
            bankAccountDeposit.ReceivingAmount = vm.PaymentSummary.ReceivingAmount;
            bankAccountDeposit.SendingAmount = vm.PaymentSummary.SendingAmount;
            bankAccountDeposit.SendingCountry = vm.PaymentSummary.SendingCountry;
            bankAccountDeposit.SendingCurrency = vm.PaymentSummary.SendingCurrencyCode;
            bankAccountDeposit.ReceivingCurrency = vm.PaymentSummary.ReceivingCurrencyCode;
            bankAccountDeposit.TotalAmount = vm.PaymentSummary.TotalAmount;
            bankAccountDeposit.ExtraFee = ExtraFee;

            bankAccountDeposit.TransactionDate = DateTime.Now;
            bankAccountDeposit.SenderPaymentMode = vm.PaymentMethodDetail.PaymentMode;
            bankAccountDeposit.SenderId = vm.SenderId;
            // bankAccountDeposit.RecipientId = vm.ReceiverDetail.ReceiverId;
            bankAccountDeposit.RecipientId = RecipientId;
            bankAccountDeposit.Status = bankDepositStatus;
            bankAccountDeposit.IsManualDeposit = IsManualBankDeposit(vm.PaymentSummary.SendingCountry, vm.PaymentSummary.ReceivingCountry);
            bankAccountDeposit.IsEuropeTransfer = IsEuropeTransfer(vm.PaymentSummary.ReceivingCountry);
            bankAccountDeposit.Apiservice = ApiService;

            FAXER.PORTAL.Services.SSenderBankAccountDeposit senderBankAccountDepositServices = new PORTAL.Services.SSenderBankAccountDeposit();

            //var obj = senderBankAccountDepositServices.Add(bankAccountDeposit).Data;

            senderBankAccountDepositServices.Update(bankAccountDeposit);

            if (!bankAccountDeposit.IsManualDeposit && (vm.PaymentMethodDetail.PaymentMode == SenderPaymentMode.CreditDebitCard
                || vm.PaymentMethodDetail.PaymentMode == SenderPaymentMode.SavedDebitCreditCard))
            {

                // Create bank Api response log 
                SBankDepositResponseStatus sBankDepositResponseStatus = new SBankDepositResponseStatus();
                var bankdepositTransactionResult = new BankDepositResponseVm();

                if (vm.IsIdCheckIsProgress == false)
                {
                    var apiReponse = GetPayoutApiResponse(bankAccountDeposit);
                    if (apiReponse != null)
                    {
                        bankAccountDeposit.Status = apiReponse.BankAccountDeposit.Status;
                        sBankDepositResponseStatus.AddLog(bankdepositTransactionResult, bankAccountDeposit.Id);

                    }
                }
                if (vm.IsIdCheckIsProgress == true)
                {
                    bankAccountDeposit.Status = BankDepositStatus.IdCheckInProgress;
                }

                SSenderForAllTransfer sSenderForAllTransfer = new SSenderForAllTransfer();
                bankAccountDeposit.Status = vm.PaymentMethodDetail.PaymentMode == SenderPaymentMode.MoneyFexBankAccount
               ? BankDepositStatus.PendingBankdepositConfirmtaion :
               vm.IsIdCheckIsProgress == true ? BankDepositStatus.IdCheckInProgress : sSenderForAllTransfer.GetBankStatus(bankAccountDeposit.IsManualDeposit, bankAccountDeposit.Status);

                senderBankAccountDepositServices.Update(bankAccountDeposit);

            }
            else
            {
                //obj = _senderBankAccountDeposit.Add(BankAccountDeposit).Data;
                senderBankAccountDepositServices.Update(bankAccountDeposit);
            }

            if (vm.PaymentMethodDetail.PaymentMode == SenderPaymentMode.CreditDebitCard
            && vm.PaymentMethodDetail.CreditDebitCardDetail.SaveCard == true)
            {
                SaveCreditDebitCard(vm.PaymentMethodDetail.CreditDebitCardDetail, TransferType.BankDeposit, bankAccountDeposit.Id, vm.SenderId);
            }
            if (bankAccountDeposit.SenderPaymentMode == SenderPaymentMode.KiiPayWallet)
            {
                SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
                int WalletId = senderCommonFunc.GetSenderKiiPayWalletInfo(vm.SenderId).Id;
                KiiPayWalletBalOut(WalletId, bankAccountDeposit.TotalAmount);
            }
            if (vm.PaymentMethodDetail.PaymentMode == SenderPaymentMode.SavedDebitCreditCard ||
                vm.PaymentMethodDetail.PaymentMode == SenderPaymentMode.CreditDebitCard)
            {
                AddUpdateLog(vm.PaymentMethodDetail.CreditDebitCardDetail, vm.SenderId);
                SavedCreditCardTransactionDetails(vm.PaymentMethodDetail.CreditDebitCardDetail, TransferType.BankDeposit, bankAccountDeposit.Id);

                //AddCreditInfoLog(vm.CreditORDebitCardDetials, obj.Id);
            }
            #region Notification Section ,Sms and Email
            if (bankAccountDeposit.SenderPaymentMode == SenderPaymentMode.MoneyFexBankAccount)
            {

                DB.Notification notification = new DB.Notification()
                {
                    SenderId = bankAccountDeposit.SenderId,
                    ReceiverId = 0,
                    Amount = "",
                    CreationDate = DateTime.Now,
                    Message = "New manual bank deposit (" + bankAccountDeposit.PaymentReference + ")",
                    NotificationReceiver = DB.NotificationFor.Admin,
                    NotificationSender = DB.NotificationFor.Sender,
                    Name = bankAccountDeposit.ReceiverName,
                    NotificationKey = bankAccountDeposit.PaymentReference
                };

                SenderCommonServices senderCommonServices = new SenderCommonServices();
                senderCommonServices.SendNotificationToAdmin(notification);

                MoneyFexBankAccountLog MoneyFexBankAccount = new MoneyFexBankAccountLog()
                {
                    IsConfirmed = false,
                    TranscationId = bankAccountDeposit.Id,
                    TrasnferMethod = TransactionTransferMethod.BankDeposit,
                    PaymentReference = bankAccountDeposit.PaymentReference
                };
                dbContext.MoneyFexBankAccountLog.Add(MoneyFexBankAccount);
                dbContext.SaveChanges();

            }

            senderBankAccountDepositServices.SendEmailAndSms(bankAccountDeposit);

            #endregion


            return new ServiceResult<BankAccountDeposit>()
            {
                Data = bankAccountDeposit,
                Message = "Transaction Completed",
                Status = ResultStatus.OK
            };
            //}
        }

        public ServiceResult<List<BankVm>> GetBanks(string receivingCountry = "", string Currency = "")
        {
            SSenderBankAccountDeposit _senderBankAccountDepositServices = new SSenderBankAccountDeposit();
            var banks = _senderBankAccountDepositServices.getBanksByCurrency(receivingCountry, Currency);

            DB.FAXEREntities dbContext = new FAXEREntities();
            var result = (from c in banks.ToList()
                          select new BankVm()
                          {
                              BankId = c.Id,
                              BankCode = c.Code,
                              BankName = c.Name,
                              CountryCode = c.CountryCode
                          }).ToList();
            return new ServiceResult<List<BankVm>>()
            {
                Data = result,
                Status = ResultStatus.OK
            };
        }



        public int getbankIdFromBankCode(string bankcode)
        {
            int bankId = dbContext.Bank.Where(x => x.Code == bankcode).Select(x => x.Id).FirstOrDefault();
            return bankId;
        }

        internal ServiceResult<bool> CancelBankTransaction(int transactionId)
        {
            var bankAccountDeposit = dbContext.BankAccountDeposit.Where(x => x.Id == transactionId).FirstOrDefault();
            bankAccountDeposit.Status = BankDepositStatus.Cancel;
            dbContext.Entry(bankAccountDeposit).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

            SSenderBankAccountDeposit senderBankAccountDeposit = new SSenderBankAccountDeposit(dbContext);

            senderBankAccountDeposit.SendEmailAndSms(bankAccountDeposit);
            return new ServiceResult<bool>()
            {
                Data = true,
                Message = "Transaction Cancelled",
                Status = ResultStatus.OK
            };
        }
        internal ServiceResult<bool> CancelMobileWalletTransaction(int transactionId)
        {
            var mobileMoneyTransfer = dbContext.MobileMoneyTransfer.Where(x => x.Id == transactionId).FirstOrDefault();
            mobileMoneyTransfer.Status = MobileMoneyTransferStatus.Cancel;
            dbContext.Entry(mobileMoneyTransfer).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

            SSenderMobileMoneyTransfer senderMobileMoneyTransfer = new SSenderMobileMoneyTransfer(dbContext);

            senderMobileMoneyTransfer.SendEmailAndSms(mobileMoneyTransfer);
            return new ServiceResult<bool>()
            {
                Data = true,
                Message = "Transaction Cancelled",
                Status = ResultStatus.OK
            };
        }
        internal ServiceResult<bool> CancelCashPickUpTransaction(int transactionId)
        {
            var cashPickUp = dbContext.FaxingNonCardTransaction.Where(x => x.Id == transactionId).FirstOrDefault();
            cashPickUp.FaxingStatus = FaxingStatus.Cancel;
            dbContext.Entry(cashPickUp).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

            SSenderCashPickUp sSenderCashPickUp = new SSenderCashPickUp(dbContext);
            sSenderCashPickUp.SendEmailAndSms(cashPickUp);
            return new ServiceResult<bool>()
            {
                Data = true,
                Message = "Transaction Cancelled",
                Status = ResultStatus.OK
            };
        }

        public FaxingNonCardTransaction CompleteCashPickUpTransaction(MobilePaymentBankDepositVm vm)
        {
            //var cashPickUpData = dbContext.FaxingNonCardTransaction.Where(x => x.ReceiptNumber == vm.ReceiptNo).FirstOrDefault();
            //if (cashPickUpData != null)
            //{
            //    return cashPickUpData;

            //}
            //else
            //{
            decimal ExtraFee = 0;
            FaxingStatus status = FaxingStatus.Hold;


            var stripeCreateTransaction = new StripeCreateTransactionVM()
            {
                Currency = vm.PaymentSummary.SendingCurrencyCode,
                ReceivingCurrency = vm.PaymentSummary.ReceivingCurrencyCode,
                SendingCountry = vm.PaymentSummary.SendingCountry,
                ReceivingCountry = vm.MobileWallet.CountryCode,
                Amount = vm.PaymentSummary.SendingAmount,
            };
            var cardProcessor = StripServices.GetCardProcessor(stripeCreateTransaction,
                                                                TransactionTransferType.Online, TransactionTransferMethod.CashPickUp);
            var CashPickUpDetails = dbContext.FaxingNonCardTransaction.Where(x => x.Id == vm.TransactionId).FirstOrDefault();

            switch (vm.PaymentMethodDetail.PaymentMode)
            {
                case PORTAL.Models.SenderPaymentMode.CreditDebitCard:
                    CashPickUpDetails.CardProcessorApi = cardProcessor;
                    ExtraFee = StripServices.GetExtraFeeAmountIfCreditCard(new StripeResultIsValidCardVm()
                    {
                        Number = vm.PaymentMethodDetail.CreditDebitCardDetail.CardNumber,
                        ExpirationMonth = vm.PaymentMethodDetail.CreditDebitCardDetail.EndMonth,
                        ExpiringYear = vm.PaymentMethodDetail.CreditDebitCardDetail.EndYear,
                        CurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyCode(vm.PaymentSummary.SendingCountry),
                        SecurityCode = vm.PaymentMethodDetail.CreditDebitCardDetail.SecurityCode

                    }, vm.PaymentSummary.TotalAmount);
                    if (vm.IsIdCheckIsProgress == true)
                    {

                        status = FaxingStatus.IdCheckInProgress;
                    }
                    else
                    {
                        status = FaxingStatus.Hold;
                    }
                    break;
                case PORTAL.Models.SenderPaymentMode.SavedDebitCreditCard:
                    CashPickUpDetails.CardProcessorApi = cardProcessor;
                    ExtraFee = StripServices.GetExtraFeeAmountIfCreditCard(new StripeResultIsValidCardVm()
                    {
                        Number = vm.PaymentMethodDetail.CreditDebitCardDetail.CardNumber,
                        ExpirationMonth = vm.PaymentMethodDetail.CreditDebitCardDetail.EndMonth,
                        ExpiringYear = vm.PaymentMethodDetail.CreditDebitCardDetail.EndYear,
                        CurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyCode(vm.PaymentSummary.SendingCountry),
                        SecurityCode = vm.PaymentMethodDetail.CreditDebitCardDetail.SecurityCode

                    }, vm.PaymentSummary.TotalAmount);

                    if (vm.IsIdCheckIsProgress == true)
                    {

                        status = FaxingStatus.IdCheckInProgress;
                    }
                    else
                    {
                        status = FaxingStatus.Hold;
                    }
                    break;
                case PORTAL.Models.SenderPaymentMode.KiiPayWallet:
                    break;
                case PORTAL.Models.SenderPaymentMode.MoneyFexBankAccount:
                    status = FaxingStatus.PendingBankdepositConfirmtaion;
                    break;
                case PORTAL.Models.SenderPaymentMode.Cash:
                    break;
                default:
                    break;
            }


            int RecipientId = 0;
            var Recipent = dbContext.Recipients.Where(x => x.ReceiverName.ToLower() == vm.ReceiverDetail.ReceiverName.ToLower() &&
                                    x.MobileNo == vm.ReceiverDetail.MobileNo && x.Service == Service.CashPickUP).FirstOrDefault();
            if (Recipent == null)
            {
                Recipients model = new Recipients()
                {
                    Country = vm.PaymentSummary.ReceivingCountry,
                    SenderId = vm.SenderId,
                    MobileNo = vm.ReceiverDetail.MobileNo,
                    Service = Service.CashPickUP,
                    ReceiverName = vm.ReceiverDetail.ReceiverName,
                    AccountNo = vm.ReceiverDetail.MobileNo
                };
                var AddRecipient = dbContext.Recipients.Add(model);
                dbContext.SaveChanges();
                RecipientId = AddRecipient.Id;
            }
            else
            {
                RecipientId = Recipent.Id;

            }
            string[] splittedName = vm.ReceiverDetail.ReceiverName.Trim().Split(null);

            DB.ReceiversDetails recDetailObj = new DB.ReceiversDetails()
            {
                City = "",
                CreatedDate = System.DateTime.Now,
                Country = vm.PaymentSummary.ReceivingCountry,
                //EmailAddress = cashPickUP.EmailAddress,
                FaxerID = vm.SenderId,
                FullName = vm.ReceiverDetail.ReceiverName,
                IsDeleted = false,
                PhoneNumber = vm.ReceiverDetail.MobileNo,
                FirstName = splittedName[0],
                MiddleName = splittedName.Count() > 2 ? splittedName[1] : " ",
                LastName = splittedName[splittedName.Count() - 1]
            };

            int NonCardReceiveId;
            SReceiverDetails receiverService = new SReceiverDetails();


            var recevierExist = dbContext.ReceiversDetails.Where(x => x.FullName == recDetailObj.FullName && x.PhoneNumber == recDetailObj.PhoneNumber).FirstOrDefault();
            if (recevierExist == null)
            {
                receiverService.Add(recDetailObj);
                NonCardReceiveId = recDetailObj.Id;

            }
            else
            {
                NonCardReceiveId = recevierExist.Id;
            }

            //SFaxingNonCardTransaction _faxingNonCardTransactionService = new SFaxingNonCardTransaction();
            //string MFCN = _faxingNonCardTransactionService.GetNewMFCNToSave();


            //FaxingNonCardTransaction cashPickUpTransfer = new FaxingNonCardTransaction()
            //{
            //    SendingCountry = vm.PaymentSummary.SendingCountry,
            //    ReceivingCountry = vm.PaymentSummary.ReceivingCountry,
            //    ExchangeRate = vm.PaymentSummary.ExchangeRate,
            //    FaxingAmount = vm.PaymentSummary.SendingAmount,
            //    ExtraFee = ExtraFee,
            //    FaxingFee = vm.PaymentSummary.Fee,
            //    PaymentReference = vm.PaymentMethodDetail.BankPayment.Reference,
            //    ReceiptNumber = vm.ReceiptNo,
            //    SenderPaymentMode = vm.PaymentMethodDetail.PaymentMode,
            //    TotalAmount = vm.PaymentSummary.TotalAmount,
            //    ReceivingAmount = vm.PaymentSummary.ReceivingAmount,
            //    FaxingStatus = status,
            //    SenderId = vm.SenderId,
            //    TransactionDate = DateTime.Now,
            //    RecipientId = RecipientId,
            //    MFCN = MFCN,
            //    NonCardRecieverId = NonCardReceiveId

            //};


            var ApiService = FAXER.PORTAL.Common.Common.GetApiservice(vm.PaymentSummary.SendingCountry,
          vm.PaymentSummary.ReceivingCountry, vm.PaymentSummary.SendingAmount, TransactionTransferMethod.CashPickUp, TransactionTransferType.Online);

            CashPickUpDetails.FaxingStatus = vm.PaymentMethodDetail.PaymentMode == SenderPaymentMode.MoneyFexBankAccount
                 ? FaxingStatus.PendingBankdepositConfirmtaion :
                 vm.IsIdCheckIsProgress == true ?
                FaxingStatus.IdCheckInProgress : FaxingStatus.NotReceived;
            CashPickUpDetails.FaxingMethod = "PM001";
            CashPickUpDetails.FaxingAmount = vm.PaymentSummary.SendingAmount;
            CashPickUpDetails.ReceivingAmount = vm.PaymentSummary.ReceivingAmount;
            CashPickUpDetails.ExchangeRate = vm.PaymentSummary.ExchangeRate;
            CashPickUpDetails.FaxingFee = vm.PaymentSummary.Fee;
            CashPickUpDetails.TotalAmount = vm.PaymentSummary.TotalAmount + ExtraFee;
            CashPickUpDetails.ExtraFee = ExtraFee;
            CashPickUpDetails.TransactionDate = System.DateTime.Now;
            CashPickUpDetails.SenderPaymentMode = vm.PaymentMethodDetail.PaymentMode;
            CashPickUpDetails.SendingCountry = vm.PaymentSummary.SendingCountry;
            CashPickUpDetails.ReceivingCountry = vm.PaymentSummary.ReceivingCountry;
            CashPickUpDetails.RecipientId = RecipientId;
            CashPickUpDetails.NonCardRecieverId = NonCardReceiveId;
            CashPickUpDetails.PaymentReference = vm.PaymentMethodDetail.BankPayment.Reference;
            CashPickUpDetails.TransferReference = "";
            CashPickUpDetails.RecipientIdentityCardId = vm.ReceiverDetail.RecipientIdentityCardId;
            CashPickUpDetails.RecipientIdenityCardNumber = vm.ReceiverDetail.RecipientIdenityCardNumber;
            CashPickUpDetails.Reason = vm.ReceiverDetail.ReasonForTransfer;
            CashPickUpDetails.Apiservice = ApiService;
            CashPickUpDetails.ExtraFee = ExtraFee;
            CashPickUpDetails.SendingCurrency = vm.PaymentSummary.SendingCurrencyCode;
            CashPickUpDetails.ReceivingCurrency = vm.PaymentSummary.ReceivingCurrencyCode;


            //save transaction for non card
            //service.UpdateTransaction(CashPickUpDetails);

            #region Create bank Api response log 
            SBankDepositResponseStatus sBankDepositResponseStatus = new SBankDepositResponseStatus();
            var cashPickUpTransactionResult = new BankDepositResponseVm();
            FAXER.PORTAL.Services.SSenderCashPickUp _cashPickUpServices = new PORTAL.Services.SSenderCashPickUp();

            if (vm.PaymentMethodDetail.PaymentMode == SenderPaymentMode.CreditDebitCard
              || vm.PaymentMethodDetail.PaymentMode == SenderPaymentMode.SavedDebitCreditCard)
            {
                if (vm.IsIdCheckIsProgress == false)
                {

                    var apiResponse = GetCashPickUpPayoutApiResponse(CashPickUpDetails);
                    if (apiResponse != null)
                    {

                        CashPickUpDetails.FaxingStatus = apiResponse.CashPickUp.FaxingStatus;
                        cashPickUpTransactionResult = apiResponse.BankDepositApiResponseVm;
                        _cashPickUpServices.AddResponseLog(cashPickUpTransactionResult, CashPickUpDetails.Id);
                    }
                }
            }

            #endregion
            dbContext.Entry<FaxingNonCardTransaction>(CashPickUpDetails).State = EntityState.Modified;
            dbContext.SaveChanges();


            if (vm.PaymentMethodDetail.PaymentMode == SenderPaymentMode.CreditDebitCard
             && vm.PaymentMethodDetail.CreditDebitCardDetail.SaveCard == true)
            {
                SaveCreditDebitCard(vm.PaymentMethodDetail.CreditDebitCardDetail, TransferType.CashPickup, CashPickUpDetails.Id, vm.SenderId);
            }
            if (CashPickUpDetails.SenderPaymentMode == SenderPaymentMode.KiiPayWallet)
            {
                SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
                int WalletId = senderCommonFunc.GetSenderKiiPayWalletInfo(vm.SenderId).Id;
                KiiPayWalletBalOut(WalletId, CashPickUpDetails.TotalAmount);
            }
            if (vm.PaymentMethodDetail.PaymentMode == SenderPaymentMode.SavedDebitCreditCard ||
                vm.PaymentMethodDetail.PaymentMode == SenderPaymentMode.CreditDebitCard)
            {
                AddUpdateLog(vm.PaymentMethodDetail.CreditDebitCardDetail, vm.SenderId);
                SavedCreditCardTransactionDetails(vm.PaymentMethodDetail.CreditDebitCardDetail, TransferType.CashPickup, CashPickUpDetails.Id);

                //AddCreditInfoLog(vm.CreditORDebitCardDetials, obj.Id);
            }


            if (CashPickUpDetails.SenderPaymentMode == SenderPaymentMode.MoneyFexBankAccount)
            {
                DB.Notification notification = new DB.Notification()
                {
                    SenderId = CashPickUpDetails.SenderId,
                    ReceiverId = 0,
                    Amount = "",
                    CreationDate = DateTime.Now,
                    Message = "New manual bank deposit (" + CashPickUpDetails.PaymentReference + ")",
                    NotificationReceiver = DB.NotificationFor.Admin,
                    NotificationSender = DB.NotificationFor.Sender,
                    Name = vm.ReceiverDetail.ReceiverName,
                    NotificationKey = CashPickUpDetails.PaymentReference
                };

                SenderCommonServices senderCommonServices = new SenderCommonServices();
                senderCommonServices.SendNotificationToAdmin(notification);

                MoneyFexBankAccountLog MoneyFexBankAccount = new MoneyFexBankAccountLog()
                {
                    IsConfirmed = false,
                    TranscationId = CashPickUpDetails.Id,
                    TrasnferMethod = TransactionTransferMethod.CashPickUp,
                    PaymentReference = CashPickUpDetails.PaymentReference
                };
                dbContext.MoneyFexBankAccountLog.Add(MoneyFexBankAccount);
                dbContext.SaveChanges();



            }
            #region Email

            _cashPickUpServices.SendEmailAndSms(CashPickUpDetails);
            //var senderInfo = FAXER.PORTAL.Common.Common.GetSenderInfo(vm.SenderId);

            //string SendingAmountWithCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbol(CashPickUpDetails.SendingCountry) + " " + CashPickUpDetails.FaxingAmount;
            //string ReceivingAmountWithCurrencySymbol = FAXER.PORTAL.Common.Common.GetCountryCurrency(CashPickUpDetails.ReceivingCountry) + " " + CashPickUpDetails.ReceivingAmount;
            //string FeeAmountWithCurrencySymbol = FAXER.PORTAL.Common.Common.GetCountryCurrency(CashPickUpDetails.SendingCountry) + " " + CashPickUpDetails.FaxingFee;

            //SSenderForAllTransfer _senderForAllTransfer = new SSenderForAllTransfer();
            //if (vm.IsIdCheckIsProgress == false)
            //{
            //    _senderForAllTransfer.SendCashPickUpEmail(senderInfo.FirstName, recDetailObj.FullName, recDetailObj.FirstName, CashPickUpDetails.MFCN,
            //                        SendingAmountWithCurrencySymbol, ReceivingAmountWithCurrencySymbol,
            //                       FeeAmountWithCurrencySymbol, CashPickUpDetails.ReceivingCountry, CashPickUpDetails.PaymentReference, CashPickUpDetails.SenderPaymentMode, vm.SenderId);
            //}
            //else if (vm.IsIdCheckIsProgress == true)
            //{

            //    _senderForAllTransfer.SendTransactionPausedEmail(CashPickUpDetails.ReceiptNumber, FAXER.PORTAL.Common.Common.GetCurrencyCode(CashPickUpDetails.SendingCountry) + " " + CashPickUpDetails.FaxingAmount,
            //        FAXER.PORTAL.Common.Common.GetCurrencyCode(CashPickUpDetails.ReceivingCountry) + " " + CashPickUpDetails.ReceivingAmount, CashPickUpDetails.ReceivingCountry,
            //         FAXER.PORTAL.Common.Common.GetCurrencyCode(CashPickUpDetails.SendingCountry) + " " + CashPickUpDetails.FaxingAmount, recDetailObj.FirstName,
            //         "", "", 0, vm.SenderId, 0, CashPickUpDetails.MFCN, TransactionServiceType.CashPickUp);

            //}

            #endregion

            return CashPickUpDetails;


            //}
        }
        public void SavedCreditCardTransactionDetails(MobilePaymentCreditDebitCardDetailVm model, TransferType transferType, int TransactionId)
        {

            DB.CardTopUpCreditDebitInformation cardDetails = new DB.CardTopUpCreditDebitInformation()
            {
                CardTransactionId = TransactionId,
                NameOnCard = model.CardHolderName,
                ExpiryDate = model.EndMonth + "/" + model.EndYear,
                CardNumber = "xxxx-xxxx-xxxx-" + model.CardNumber.Right(4),
                IsSavedCard = false,
                //AutoRecharged = model.AutoTopUp,
                TransferType = (int)transferType,
                CreatedDate = DateTime.Now
            };
            SSavedCard cardInformationservices = new SSavedCard();
            cardDetails = cardInformationservices.Save(cardDetails);


        }

        public bool AddUpdateLog(MobilePaymentCreditDebitCardDetailVm debitCardViewModel, int senderId)
        {

            try
            {
                SCreditDebitCardUsage creditDebitCardUsage = new SCreditDebitCardUsage();
                creditDebitCardUsage.AddOrUpdateCreditCardUsageLog(new CreditCardUsageLog()
                {
                    CardNum = FAXER.PORTAL.Common.Common.FormatSavedCardNumber(debitCardViewModel.CardNumber),
                    Count = 1,
                    Module = Module.Faxer,
                    SenderId = senderId,
                    UpdatedDateTime = DateTime.Now
                });
            }
            catch (Exception)
            {
            }

            return true;
        }

        public void KiiPayWalletBalOut(int WalletId, decimal Amount)
        {
            var senderWalletData = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == WalletId).FirstOrDefault();
            senderWalletData.CurrentBalance = senderWalletData.CurrentBalance - Amount;
            dbContext.Entry(senderWalletData).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

        }
        public void SaveCreditDebitCard(MobilePaymentCreditDebitCardDetailVm model, TransferType transferType, int TransactionId, int senderId)
        {

            int SavedCardCount = dbContext.SavedCard.Where(x => x.UserId == senderId).Count();
            if (SavedCardCount < 2)
            {
                DB.SavedCard savedCardObject = new DB.SavedCard()
                {
                    //Type = model.CreditDebitCardType,
                    CardName = model.CardHolderName.Encrypt(),
                    EMonth = model.EndMonth.Encrypt(),
                    EYear = model.EndYear.Encrypt(),
                    CreatedDate = System.DateTime.Now,
                    UserId = senderId,
                    Num = model.CardNumber.Encrypt(),
                    ClientCode = model.SecurityCode.Encrypt()

                };
                SSavedCard cardservices = new SSavedCard();
                savedCardObject = cardservices.Add(savedCardObject);

            }
        }

        internal MobileMoneyTransfer CompleteMobileTransaction(MobilePaymentBankDepositVm vm)
        {
            var mobileMoneyTransferData = dbContext.MobileMoneyTransfer.Where(x => x.Id == vm.TransactionId).FirstOrDefault();


            //if (mobileMoneyTransferData != null)
            //{
            //    return mobileMoneyTransferData;
            //}
            //else
            //{
            var MobileMoneyTransferDetails = dbContext.MobileMoneyTransfer.Where(x => x.Id == vm.TransactionId).FirstOrDefault();

            decimal ExtraFee = 0;
            MobileMoneyTransferStatus status = MobileMoneyTransferStatus.InProgress;
            var stripeCreateTransaction = new StripeCreateTransactionVM()
            {
                Currency = vm.PaymentSummary.SendingCurrencyCode,
                ReceivingCurrency = vm.PaymentSummary.ReceivingCurrencyCode,
                SendingCountry = vm.PaymentSummary.SendingCountry,
                ReceivingCountry = vm.MobileWallet.CountryCode,
                Amount = vm.PaymentSummary.SendingAmount,
            };
            var cardProcessor = StripServices.GetCardProcessor(stripeCreateTransaction,
                                                                TransactionTransferType.Online, TransactionTransferMethod.OtherWallet);


            switch (vm.PaymentMethodDetail.PaymentMode)
            {
                case PORTAL.Models.SenderPaymentMode.CreditDebitCard:
                    ExtraFee = StripServices.GetExtraFeeAmountIfCreditCard(new StripeResultIsValidCardVm()
                    {
                        Number = vm.PaymentMethodDetail.CreditDebitCardDetail.CardNumber,
                        ExpirationMonth = vm.PaymentMethodDetail.CreditDebitCardDetail.EndMonth,
                        ExpiringYear = vm.PaymentMethodDetail.CreditDebitCardDetail.EndYear,
                        CurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyCode(vm.PaymentSummary.SendingCountry),
                        SecurityCode = vm.PaymentMethodDetail.CreditDebitCardDetail.SecurityCode

                    }, vm.PaymentSummary.TotalAmount);
                    if (vm.IsIdCheckIsProgress == true)
                    {

                        status = MobileMoneyTransferStatus.IdCheckInProgress;
                    }
                    else
                    {
                        status = MobileMoneyTransferStatus.InProgress;
                    }

                    MobileMoneyTransferDetails.CardProcessorApi = cardProcessor;
                    break;
                case PORTAL.Models.SenderPaymentMode.SavedDebitCreditCard:
                    ExtraFee = StripServices.GetExtraFeeAmountIfCreditCard(new StripeResultIsValidCardVm()
                    {
                        Number = vm.PaymentMethodDetail.CreditDebitCardDetail.CardNumber,
                        ExpirationMonth = vm.PaymentMethodDetail.CreditDebitCardDetail.EndMonth,
                        ExpiringYear = vm.PaymentMethodDetail.CreditDebitCardDetail.EndYear,
                        CurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyCode(vm.PaymentSummary.SendingCountry),
                        SecurityCode = vm.PaymentMethodDetail.CreditDebitCardDetail.SecurityCode

                    }, vm.PaymentSummary.TotalAmount);
                    if (vm.IsIdCheckIsProgress == true)
                    {

                        status = MobileMoneyTransferStatus.IdCheckInProgress;
                    }
                    else
                    {
                        status = MobileMoneyTransferStatus.InProgress;
                    }

                    MobileMoneyTransferDetails.CardProcessorApi = cardProcessor;
                    break;
                case PORTAL.Models.SenderPaymentMode.KiiPayWallet:
                    break;
                case PORTAL.Models.SenderPaymentMode.MoneyFexBankAccount:
                    status = MobileMoneyTransferStatus.PendingBankdepositConfirmtaion;
                    break;
                case PORTAL.Models.SenderPaymentMode.Cash:
                    break;
                default:
                    break;
            }

            var ApiService = FAXER.PORTAL.Common.Common.GetApiservice(vm.PaymentSummary.SendingCountry, vm.PaymentSummary.ReceivingCountry,
           vm.PaymentSummary.SendingAmount, TransactionTransferMethod.OtherWallet, TransactionTransferType.Online);

            int RecipientId = 0;
            var Recipent = dbContext.Recipients.Where(x => x.ReceiverName.ToLower() == vm.ReceiverDetail.ReceiverName.ToLower() &&
                                    x.MobileNo == vm.ReceiverDetail.MobileNo && x.Service == Service.MobileWallet).FirstOrDefault();
            if (Recipent == null)
            {
                Recipients model = new Recipients()
                {
                    Country = vm.PaymentSummary.ReceivingCountry,
                    SenderId = vm.SenderId,
                    MobileNo = vm.ReceiverDetail.MobileNo,
                    Service = Service.MobileWallet,
                    ReceiverName = vm.ReceiverDetail.ReceiverName,
                    AccountNo = vm.MobileWallet.WalletNo,
                    MobileWalletProvider = vm.MobileWallet.WalletId
                };
                var AddRecipient = dbContext.Recipients.Add(model);
                dbContext.SaveChanges();
                RecipientId = AddRecipient.Id;
            }
            else
            {
                RecipientId = Recipent.Id;

            }
            PaymentType paymentType = PaymentType.International;
            if (vm.PaymentSummary.SendingCountry == vm.PaymentSummary.ReceivingCountry)
            {
                paymentType = PaymentType.Local;
            }
            //   MobileMoneyTransfer mobileMoneyTransfer = new MobileMoneyTransfer()
            //   {
            //       SendingCountry = vm.PaymentSummary.SendingCountry,
            //       ReceivingCountry = vm.PaymentSummary.ReceivingCountry,
            //       ExchangeRate = vm.PaymentSummary.ExchangeRate,
            //       SendingAmount = vm.PaymentSummary.SendingAmount,
            //       Apiservice = ApiService,
            //       ExtraFee = ExtraFee,
            //       Fee = vm.PaymentSummary.Fee,
            //       PaidFromModule = Module.Faxer,
            //       PaidToMobileNo = vm.MobileWallet.WalletNo,
            //       PaymentReference = vm.PaymentMethodDetail.BankPayment.Reference,
            //       ReceiptNo = vm.ReceiptNo,
            //       PaymentType = paymentType,
            //       SenderPaymentMode = vm.PaymentMethodDetail.PaymentMode,
            //       TotalAmount = vm.PaymentSummary.TotalAmount,
            //       ReceiverName = vm.MobileWallet.ReceiverName,
            //       ReceivingAmount = vm.PaymentSummary.ReceivingAmount,
            //       Status = status,
            //       SenderId = vm.SenderId,
            //       WalletOperatorId = vm.MobileWallet.WalletId,
            //       TransactionDate = DateTime.Now,
            //       RecipientId = RecipientId
            //   };
            //   mobileMoneyTransfer.Status = vm.PaymentMethodDetail.PaymentMode == SenderPaymentMode.MoneyFexBankAccount ?
            //MobileMoneyTransferStatus.PendingBankdepositConfirmtaion : vm.IsIdCheckIsProgress == true ?
            //MobileMoneyTransferStatus.IdCheckInProgress : MobileMoneyTransferStatus.InProgress;

            MobileMoneyTransferDetails.ExchangeRate = vm.PaymentSummary.ExchangeRate;
            MobileMoneyTransferDetails.Fee = vm.PaymentSummary.Fee;
            MobileMoneyTransferDetails.PaidFromModule = Module.Faxer;
            MobileMoneyTransferDetails.TransactionDate = DateTime.Now;
            MobileMoneyTransferDetails.TotalAmount = vm.PaymentSummary.TotalAmount + ExtraFee;
            MobileMoneyTransferDetails.ExtraFee = ExtraFee;
            MobileMoneyTransferDetails.PaidToMobileNo = vm.MobileWallet.WalletNo;
            MobileMoneyTransferDetails.PaymentReference = vm.PaymentMethodDetail.BankPayment.Reference;
            MobileMoneyTransferDetails.SenderPaymentMode = vm.PaymentMethodDetail.PaymentMode;
            MobileMoneyTransferDetails.ReceivingAmount = vm.PaymentSummary.ReceivingAmount;
            MobileMoneyTransferDetails.SenderId = vm.SenderId;
            MobileMoneyTransferDetails.ReceivingCountry = vm.MobileWallet.CountryCode;
            MobileMoneyTransferDetails.SendingAmount = vm.PaymentSummary.SendingAmount;
            MobileMoneyTransferDetails.SendingCountry = vm.PaymentSummary.SendingCountry;
            MobileMoneyTransferDetails.ReceiverName = vm.ReceiverDetail.ReceiverName;
            MobileMoneyTransferDetails.WalletOperatorId = vm.MobileWallet.WalletId;
            MobileMoneyTransferDetails.RecipientId = RecipientId;
            MobileMoneyTransferDetails.SendingCurrency = vm.PaymentSummary.SendingCurrencyCode;
            MobileMoneyTransferDetails.ReceivingCurrency = vm.PaymentSummary.ReceivingCurrencyCode;

            //MobileMoneyTransferDetails.Status = vm.IsIdCheckInProgress == true ? 
            //    MobileMoneyTransferStatus.IdCheckInProgress :
            //    paymentMethod.SenderPaymentMode == SenderPaymentMode.MoneyFexBankAccount ?
            //    MobileMoneyTransferStatus.PendingBankdepositConfirmtaion
            //    : MobileMoneyTransferStatus.InProgress;
            MobileMoneyTransferDetails.Status =
              vm.PaymentMethodDetail.PaymentMode == SenderPaymentMode.MoneyFexBankAccount ?
                MobileMoneyTransferStatus.PendingBankdepositConfirmtaion
                : vm.IsIdCheckIsProgress == true ?
                MobileMoneyTransferStatus.IdCheckInProgress : MobileMoneyTransferStatus.InProgress;
            MobileMoneyTransferDetails.ExtraFee = ExtraFee;

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
            SSenderMobileMoneyTransfer _senderMobileMoneyTransfer = new SSenderMobileMoneyTransfer();

            if (MobileMoneyTransferDetails.Id == 0)
            {
                obj = _senderMobileMoneyTransfer.Add(MobileMoneyTransferDetails).Data;
                MobileMoneyTransferDetails.Id = obj.Id;
            }



            //var obj = mobileMoneyTransferServices.Add(mobileMoneyTransfer).Data;

            if (vm.PaymentMethodDetail.PaymentMode == SenderPaymentMode.CreditDebitCard
                || vm.PaymentMethodDetail.PaymentMode == SenderPaymentMode.SavedDebitCreditCard)
            {
                if (vm.IsIdCheckIsProgress == false)
                {

                    var apiResponse = GetMobileWalletPayoutApiResponse(MobileMoneyTransferDetails);
                    if (apiResponse != null)
                    {

                        MobileMoneyTransferDetails.Status = apiResponse.status;
                        SMobileMoneyTransferResopnseStatus _sMobileMoneyResposeStatus = new SMobileMoneyTransferResopnseStatus();
                        _sMobileMoneyResposeStatus.AddLog(apiResponse.response, MobileMoneyTransferDetails.Id);
                    }

                }
            }

            obj = _senderMobileMoneyTransfer.Update(MobileMoneyTransferDetails).Data;


            if (vm.PaymentMethodDetail.PaymentMode == SenderPaymentMode.CreditDebitCard
              && vm.PaymentMethodDetail.CreditDebitCardDetail.SaveCard == true)
            {
                SaveCreditDebitCard(vm.PaymentMethodDetail.CreditDebitCardDetail, TransferType.MobileTransfer, obj.Id, vm.SenderId);
            }
            if (MobileMoneyTransferDetails.SenderPaymentMode == SenderPaymentMode.KiiPayWallet)
            {
                SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
                int WalletId = senderCommonFunc.GetSenderKiiPayWalletInfo(vm.SenderId).Id;
                KiiPayWalletBalOut(WalletId, MobileMoneyTransferDetails.TotalAmount);
            }
            if (vm.PaymentMethodDetail.PaymentMode == SenderPaymentMode.SavedDebitCreditCard ||
                vm.PaymentMethodDetail.PaymentMode == SenderPaymentMode.CreditDebitCard)
            {
                AddUpdateLog(vm.PaymentMethodDetail.CreditDebitCardDetail, vm.SenderId);
                SavedCreditCardTransactionDetails(vm.PaymentMethodDetail.CreditDebitCardDetail, TransferType.MobileTransfer, obj.Id);

                //AddCreditInfoLog(vm.CreditORDebitCardDetials, obj.Id);
            }
            _senderMobileMoneyTransfer.SendEmailAndSms(obj);

            #region Notification Section 
            if (obj.SenderPaymentMode == SenderPaymentMode.MoneyFexBankAccount)
            {
                DB.Notification notification = new DB.Notification()
                {
                    SenderId = obj.SenderId,
                    ReceiverId = 0,
                    Amount = "",
                    CreationDate = DateTime.Now,
                    Message = "New manual bank deposit (" + obj.PaymentReference + ")",
                    NotificationReceiver = DB.NotificationFor.Admin,
                    NotificationSender = DB.NotificationFor.Sender,
                    Name = obj.ReceiverName,
                    NotificationKey = obj.PaymentReference
                };

                SenderCommonServices senderCommonServices = new SenderCommonServices();
                senderCommonServices.SendNotificationToAdmin(notification);

                MoneyFexBankAccountLog MoneyFexBankAccount = new MoneyFexBankAccountLog()
                {
                    IsConfirmed = false,
                    TranscationId = obj.Id,
                    TrasnferMethod = TransactionTransferMethod.OtherWallet,
                    PaymentReference = obj.PaymentReference
                };
                dbContext.MoneyFexBankAccountLog.Add(MoneyFexBankAccount);
                dbContext.SaveChanges();

            }
            #endregion


            return MobileMoneyTransferDetails;
            //}
        }

        internal ServiceResult<List<WalletDropDownVm>> GetWallets(string countryCode)
        {
            DB.FAXEREntities dbContext = new FAXEREntities();
            var data = (from c in dbContext.MobileWalletOperator.Where(x => x.Country == countryCode)
                        select new WalletDropDownVm()
                        {

                            WalletId = c.Id,
                            WalletName = c.Name,
                            Code = c.Code
                        }).ToList();
            return new ServiceResult<List<WalletDropDownVm>>()
            {
                Data = data,
                Message = "",
                Status = ResultStatus.OK
            };
        }

        public BankTransactionApiResponse GetPayoutApiResponse(BankAccountDeposit accountDeposit)
        {


            FAXER.PORTAL.Services.SSenderBankAccountDeposit senderBankAccountDepositServices = new PORTAL.Services.SSenderBankAccountDeposit();

            if (accountDeposit.IsManualDeposit == false && accountDeposit.Status != BankDepositStatus.IdCheckInProgress)
            {
                BankTransactionApiResponse apiResponseResult = new BankTransactionApiResponse();
                switch (accountDeposit.SenderPaymentMode)
                {
                    case PORTAL.Models.SenderPaymentMode.CreditDebitCard:
                        apiResponseResult = senderBankAccountDepositServices.CreateBankTransactionToApi(accountDeposit);
                        break;
                    case PORTAL.Models.SenderPaymentMode.SavedDebitCreditCard:
                        apiResponseResult = senderBankAccountDepositServices.CreateBankTransactionToApi(accountDeposit);
                        break;
                    case PORTAL.Models.SenderPaymentMode.KiiPayWallet:
                        break;
                    case PORTAL.Models.SenderPaymentMode.MoneyFexBankAccount:
                        break;
                    case PORTAL.Models.SenderPaymentMode.Cash:
                        break;
                    default:
                        break;
                }
                return apiResponseResult;
            }
            return null;
        }


        public MobileTransferApiResponse GetMobileWalletPayoutApiResponse(MobileMoneyTransfer mobileMoneyTransfer)
        {

            FAXER.PORTAL.Services.SSenderMobileMoneyTransfer mobileTransferServices = new PORTAL.Services.SSenderMobileMoneyTransfer();

            if (mobileMoneyTransfer.Status != MobileMoneyTransferStatus.IdCheckInProgress)
            {
                MobileTransferApiResponse apiResponseResult = new MobileTransferApiResponse();
                switch (mobileMoneyTransfer.SenderPaymentMode)
                {
                    case PORTAL.Models.SenderPaymentMode.CreditDebitCard:
                        apiResponseResult = mobileTransferServices.CreateTransactionToApi(mobileMoneyTransfer);
                        break;
                    case PORTAL.Models.SenderPaymentMode.SavedDebitCreditCard:
                        apiResponseResult = mobileTransferServices.CreateTransactionToApi(mobileMoneyTransfer);
                        break;
                    case PORTAL.Models.SenderPaymentMode.KiiPayWallet:
                        break;
                    case PORTAL.Models.SenderPaymentMode.MoneyFexBankAccount:
                        break;
                    case PORTAL.Models.SenderPaymentMode.Cash:
                        break;
                    default:
                        break;
                }
                return apiResponseResult;
            }
            return null;
        }
        public CashPickUpTransactionApiResponse GetCashPickUpPayoutApiResponse(FaxingNonCardTransaction cashPickUp)
        {

            FAXER.PORTAL.Services.SSenderCashPickUp cashPickupService = new PORTAL.Services.SSenderCashPickUp();

            if (cashPickUp.FaxingStatus != FaxingStatus.IdCheckInProgress)
            {
                CashPickUpTransactionApiResponse apiResponseResult = new CashPickUpTransactionApiResponse();
                switch (cashPickUp.SenderPaymentMode)
                {
                    case PORTAL.Models.SenderPaymentMode.CreditDebitCard:
                        apiResponseResult = cashPickupService.CreateCashPickTransactionToApi(cashPickUp);
                        break;
                    case PORTAL.Models.SenderPaymentMode.SavedDebitCreditCard:
                        apiResponseResult = cashPickupService.CreateCashPickTransactionToApi(cashPickUp);
                        break;
                    case PORTAL.Models.SenderPaymentMode.KiiPayWallet:
                        break;
                    case PORTAL.Models.SenderPaymentMode.MoneyFexBankAccount:
                        break;
                    case PORTAL.Models.SenderPaymentMode.Cash:
                        break;
                    default:
                        break;
                }
                return apiResponseResult;
            }
            return null;
        }


        public ServiceResult<string> GetReceiptNo()
        {

            var receiptNo = FAXER.PORTAL.Common.Common.GenerateBankAccountDepositReceiptNo(6);
            return new ServiceResult<string>()
            {

                Data = receiptNo,
                Message = "",
                Status = ResultStatus.OK
            };
        }
        public ServiceResult<string> GetOtherWalletReceiptNo()
        {

            var receiptNo = FAXER.PORTAL.Common.Common.GenerateMobileMoneyTransferReceiptNo(6);
            return new ServiceResult<string>()
            {
                Data = receiptNo,
                Message = "",
                Status = ResultStatus.OK
            };
        }
        public ServiceResult<string> GetCashPikUpReceiptNo()
        {

            var receiptNo = FAXER.PORTAL.Common.Common.GenerateCashPickUpReceiptNo(6);
            return new ServiceResult<string>()
            {

                Data = receiptNo,
                Message = "",
                Status = ResultStatus.OK
            };
        }
        public ServiceResult<bool> IsMoneyFexBankInfoExist(string CountryCode)
        {
            DB.FAXEREntities dbContext = new FAXEREntities();
            ServiceResult<bool> result = new ServiceResult<bool>();
            var bankDetial = dbContext.BankAccount.Where(x => x.CountryCode == CountryCode && x.IsDeleted == false).FirstOrDefault();
            if (bankDetial != null)
            {
                return new ServiceResult<bool>()
                {

                    Data = true,
                    Message = "",
                    Status = ResultStatus.OK
                };
            }

            return new ServiceResult<bool>()
            {

                Data = false,
                Message = "",
                Status = ResultStatus.OK
            };
        }
        public ServiceResult<SenderMoneyFexBankDepositVM> GetMoneyFexBankInfo(string CountryCode)
        {
            DB.FAXEREntities dbContext = new FAXEREntities();
            SenderMoneyFexBankDepositVM result = new SenderMoneyFexBankDepositVM(CountryCode);
            var bankDetial = dbContext.BankAccount.Where(x => x.CountryCode == CountryCode && x.IsDeleted == false).FirstOrDefault();
            if (bankDetial != null)
            {
                result.AccountNumber = bankDetial.AccountNo;
                result.PaymentReference = FAXER.PORTAL.Common.Common.GenerateBankPaymentReceiptNo();
                result.ShortCode = bankDetial.LabelValue;
                result.LabelName = bankDetial.LabelName;
            }

            return new ServiceResult<SenderMoneyFexBankDepositVM>()
            {

                Data = result,
                Message = "",
                Status = ResultStatus.OK
            };
        }
        public bool IsManualBankDeposit(string sendingCountry, string ReceivingCountry)
        {

            var result = FAXER.PORTAL.Common.Common.IsManualDeposit(sendingCountry, ReceivingCountry);

            return result;

        }

        public bool IsEuropeTransfer(string ReceivingCountry)
        {

            var result = FAXER.PORTAL.Common.Common.IsEuropeTransfer(ReceivingCountry);

            return result;

        }

        public ServiceResult<List<BankVm>> GetBanks(string CountryCode)
        {

            DB.FAXEREntities dbContext = new FAXEREntities();
            var result = (from c in dbContext.Bank.Where(x => x.CountryCode == CountryCode).ToList()
                          select new BankVm()
                          {
                              BankId = c.Id,
                              BankCode = c.Code,
                              BankName = c.Name,
                              CountryCode = c.CountryCode

                          }).ToList();
            return new ServiceResult<List<BankVm>>()
            {
                Data = result,
                Status = ResultStatus.OK
            };
        }

        public ServiceResult<List<BankBranchVm>> GetBankBranches(int BankId)
        {

            DB.FAXEREntities dbContext = new FAXEREntities();
            var result = (from c in dbContext.BankBranch.Where(x => x.BankId == BankId).ToList()
                          select new BankBranchVm()
                          {
                              Id = c.Id,
                              BranchCode = c.BranchCode,
                              BranchName = c.BranchName
                          }).ToList();
            return new ServiceResult<List<BankBranchVm>>()
            {
                Data = result,
                Status = ResultStatus.OK
            };
        }
    }

    public class BankVm
    {


        public int BankId { get; set; }
        public string BankName { get; set; }

        public string BankCode { get; set; }

        public string CountryCode { get; set; }



    }
    public class BankBranchVm
    {

        public int Id { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }

    }

    public class WalletDropDownVm
    {

        public int WalletId { get; set; }
        public string WalletName { get; set; }
        public string Code { get; set; }


    }

}