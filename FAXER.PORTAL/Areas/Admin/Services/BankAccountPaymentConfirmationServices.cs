using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class BankAccountPaymentConfirmationServices
    {
        DB.FAXEREntities dbContext = null;
        public BankAccountPaymentConfirmationServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public BankAccountPaymentConfirmationViewModel GetBankAccountPaymentDetails(string reference)
        {
            var model = dbContext.MoneyFexBankAccountLog.Where(x => x.PaymentReference == reference).FirstOrDefault();

            BankAccountPaymentConfirmationViewModel result = new BankAccountPaymentConfirmationViewModel();


            if (model == null)
            {
                return result;
            }
            switch (model.TrasnferMethod)
            {

                case FAXER.PORTAL.DB.TransactionTransferMethod.BankDeposit:
                    var bankDeposit = dbContext.BankAccountDeposit.Where(x => x.Id == model.TranscationId).ToList();
                    result = (from c in bankDeposit
                              select new BankAccountPaymentConfirmationViewModel()
                              {
                                  Id = c.Id,
                                  Fee = c.Fee,
                                  PaymentReference = c.PaymentReference,
                                  ReceiverEmail = "",
                                  ReceiverName = c.ReceiverName,
                                  ReceiverPhoneNo = c.ReceiverMobileNo,
                                  ReceivingAmount = c.ReceivingAmount,
                                  ReceivingCurrency = Common.Common.GetCountryCurrency(c.ReceivingCountry),
                                  RecipetNo = c.ReceiptNo,
                                  ReciverCity = c.ReceiverCity,
                                  ReciverCountry = Common.Common.GetCountryName(c.ReceivingCountry),
                                  ReciverFirstname = GetFirstName(c.ReceiverName),
                                  SenderEmail = GetSenderInfo(c.SenderId).Email,
                                  senderDOB = GetSenderInfo(c.SenderId).DateOfBirth.ToString(),
                                  SenderName = GetSenderInfo(c.SenderId).FirstName + " " + GetSenderInfo(c.SenderId).MiddleName + " "
                                  + GetSenderInfo(c.SenderId).LastName,
                                  senderPhoneNo = GetSenderInfo(c.SenderId).PhoneNumber,
                                  sendingAmount = c.SendingAmount,
                                  SendingCurrency = Common.Common.GetCountryCurrency(c.SendingCountry),
                                  SendingCurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                                  SenderCountry = c.SendingCountry,
                                  TotalAmount = c.TotalAmount,
                                  TranactionDate = c.TransactionDate.ToString(),
                                  BankName = Common.Common.getBankName(c.BankId),
                                  BankCode = c.BankCode,
                                  AccountNumber = c.ReceiverAccountNo,
                                  ExchangeRate = c.ExchangeRate,
                                  Method = TransactionTransferMethod.BankDeposit,
                                  Status = c.Status.ToString().ToLower(),
                                  MoneyFexBankAccountLogId = model.Id,
                                  SenderId = c.SenderId
                              }).FirstOrDefault();

                    break;

                case FAXER.PORTAL.DB.TransactionTransferMethod.CashPickUp:

                    var CashPickUp = dbContext.FaxingNonCardTransaction.Where(x => x.Id == model.TranscationId).ToList();


                    result = (from c in CashPickUp
                              select new BankAccountPaymentConfirmationViewModel()
                              {
                                  Id = c.Id,
                                  Fee = c.FaxingFee,
                                  MFCN = c.MFCN,
                                  PaymentReference = c.PaymentReference,
                                  ReceiverEmail = c.NonCardReciever.EmailAddress,
                                  ReceiverName = c.NonCardReciever.FullName,
                                  ReceiverPhoneNo = c.NonCardReciever.PhoneNumber,
                                  ReceivingAmount = c.ReceivingAmount,
                                  ReceivingCurrency = Common.Common.GetCountryCurrency(c.ReceivingCountry),
                                  RecipetNo = c.ReceiptNumber,
                                  ReciverCity = c.NonCardReciever.City,
                                  ReciverCountry = Common.Common.GetCountryName(c.ReceivingCountry),
                                  ReciverFirstname = c.NonCardReciever.FirstName,
                                  SenderEmail = GetSenderInfo(c.NonCardReciever.FaxerID).Email,
                                  senderDOB = GetSenderInfo(c.NonCardReciever.FaxerID).DateOfBirth.ToString(),
                                  SenderName = GetSenderInfo(c.NonCardReciever.FaxerID).FirstName + " " + GetSenderInfo(c.NonCardReciever.FaxerID).MiddleName + " "
                                  + GetSenderInfo(c.NonCardReciever.FaxerID).LastName,
                                  senderPhoneNo = GetSenderInfo(c.NonCardReciever.FaxerID).PhoneNumber,
                                  sendingAmount = c.FaxingAmount,
                                  SendingCurrency = Common.Common.GetCountryCurrency(c.SendingCountry),
                                  SendingCurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                                  SenderCountry = c.SendingCountry,
                                  TotalAmount = c.TotalAmount,
                                  TranactionDate = c.TransactionDate.ToString(),
                                  ExchangeRate = c.ExchangeRate,
                                  Method = TransactionTransferMethod.CashPickUp,
                                  Status = c.FaxingStatus.ToString().ToLower(),
                                  MoneyFexBankAccountLogId = model.Id,
                                  SenderId = c.NonCardReciever.FaxerID

                              }).FirstOrDefault();


                    break;
                case FAXER.PORTAL.DB.TransactionTransferMethod.OtherWallet:

                    var mobileTransfer = dbContext.MobileMoneyTransfer.Where(x => x.Id == model.TranscationId).ToList();
                    result = (from c in mobileTransfer
                              select new BankAccountPaymentConfirmationViewModel()
                              {
                                  Id = c.Id,
                                  Fee = c.Fee,
                                  PaymentReference = c.PaymentReference,
                                  ReceiverEmail = "",
                                  ReceiverName = c.ReceiverName,
                                  ReceiverPhoneNo = c.PaidToMobileNo,
                                  ReceivingAmount = c.ReceivingAmount,
                                  ReceivingCurrency = Common.Common.GetCountryCurrency(c.ReceivingCountry),
                                  RecipetNo = c.ReceiptNo,
                                  ReciverCity = c.ReceiverCity,
                                  ReciverCountry = Common.Common.GetCountryName(c.ReceivingCountry),
                                  ReciverFirstname = GetFirstName(c.ReceiverName),
                                  SenderEmail = GetSenderInfo(c.SenderId).Email,
                                  senderDOB = GetSenderInfo(c.SenderId).DateOfBirth.ToString(),
                                  SenderName = GetSenderInfo(c.SenderId).FirstName + " " + GetSenderInfo(c.SenderId).MiddleName + " "
                                  + GetSenderInfo(c.SenderId).LastName,
                                  senderPhoneNo = GetSenderInfo(c.SenderId).PhoneNumber,
                                  sendingAmount = c.SendingAmount,
                                  SendingCurrency = Common.Common.GetCountryCurrency(c.SendingCountry),
                                  SendingCurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                                  SenderCountry = c.SendingCountry,
                                  TotalAmount = c.TotalAmount,
                                  TranactionDate = c.TransactionDate.ToString(),
                                  WalletProvider = GetWalletOperatorName(c.WalletOperatorId),
                                  ExchangeRate = c.ExchangeRate,
                                  Method = TransactionTransferMethod.OtherWallet,
                                  Status = c.Status.ToString().ToLower(),
                                  MoneyFexBankAccountLogId = model.Id,
                                  SenderId = c.SenderId

                              }).FirstOrDefault();

                    break;

                default:
                    break;
            }


            SetBankAccountPaymentConfirmationViewModel(result);
            return result;
        }

        internal void CompleteTransaction()
        {
            var transferSummary = GetBankAccountPaymentConfirmationViewModel();
            SenderDocumentationServices senderDocumentationServices = new SenderDocumentationServices();
            switch (transferSummary.Method)
            {

                case FAXER.PORTAL.DB.TransactionTransferMethod.BankDeposit:
                    var bankDeposit = dbContext.BankAccountDeposit.Where(x => x.Id == transferSummary.Id).FirstOrDefault();
                    if (bankDeposit != null)
                    {
                        if (string.IsNullOrEmpty(bankDeposit.TransferZeroSenderId))
                        {
                            senderDocumentationServices.ReInitialBankDepositTransaction(bankDeposit);
                        }
                    }
                    //bankDeposit.Status = BankDepositStatus.Confirm;
                    //dbContext.Entry<BankAccountDeposit>(bankDeposit).State = System.Data.Entity.EntityState.Modified;
                    //dbContext.SaveChanges();
                    break;

                case FAXER.PORTAL.DB.TransactionTransferMethod.CashPickUp:
                    var CashPickUp = dbContext.FaxingNonCardTransaction.Where(x => x.Id == transferSummary.Id && x.FaxingStatus == FaxingStatus.PendingBankdepositConfirmtaion).FirstOrDefault();
                    if (CashPickUp != null)
                    {
                        CashPickUp.FaxingStatus = FaxingStatus.Received;
                        dbContext.Entry<FaxingNonCardTransaction>(CashPickUp).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                    }


                    break;
                case FAXER.PORTAL.DB.TransactionTransferMethod.OtherWallet:
                    var mobileTransfer = dbContext.MobileMoneyTransfer.Where(x => x.Id == transferSummary.Id).FirstOrDefault();
                    if (string.IsNullOrEmpty(mobileTransfer.TransferZeroSenderId))
                    {
                        senderDocumentationServices.ReInitialMobileDepositTransaction(mobileTransfer);
                    }
                    //mobileTransfer.Status = MobileMoneyTransferStatus.Paid;
                    //dbContext.Entry<MobileMoneyTransfer>(mobileTransfer).State = System.Data.Entity.EntityState.Modified;
                    //dbContext.SaveChanges();

                    break;

                default:
                    break;
            }



            var model = dbContext.MoneyFexBankAccountLog.Where(x => x.Id == transferSummary.MoneyFexBankAccountLogId).FirstOrDefault();
            model.IsConfirmed = true;
            dbContext.Entry<MoneyFexBankAccountLog>(model).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();


        }

        internal void ReInitialTransaction()
        {
            SenderDocumentationServices senderDocumentationServices = new SenderDocumentationServices();
            var transferSummary = GetBankAccountPaymentConfirmationViewModel();
            switch (transferSummary.Method)
            {

                case FAXER.PORTAL.DB.TransactionTransferMethod.BankDeposit:
                    var bankDeposit = dbContext.BankAccountDeposit.Where(x => x.Id == transferSummary.Id).FirstOrDefault();
                    senderDocumentationServices.ReInitialBankDepositTransaction(bankDeposit);


                    break;

                case FAXER.PORTAL.DB.TransactionTransferMethod.CashPickUp:

                    var CashPickUp = dbContext.FaxingNonCardTransaction.Where(x => x.Id == transferSummary.Id).FirstOrDefault();

                    break;
                case FAXER.PORTAL.DB.TransactionTransferMethod.OtherWallet:
                    var mobileTransfer = dbContext.MobileMoneyTransfer.Where(x => x.Id == transferSummary.Id).FirstOrDefault();

                    senderDocumentationServices.ReInitialMobileDepositTransaction(mobileTransfer);
                    break;

                default:
                    break;
            }

            CompleteTransaction();
        }

        public bool IsPaymentReference(string reference)
        {
            var model = dbContext.MoneyFexBankAccountLog.Where(x => x.PaymentReference == reference && x.IsConfirmed == false).FirstOrDefault();
            if (model == null)
            {
                return false;
            }

            return true;
        }

        public string GetWalletOperatorName(int walletOperatorId)
        {
            var data = dbContext.MobileWalletOperator.Where(x => x.Id == walletOperatorId).Select(x => x.Name).FirstOrDefault();
            return data;
        }

        public string GetFirstName(string receiverName)
        {
            string Firstname = receiverName.Split(' ')[0];
            return Firstname;
        }

        public FaxerInformation GetSenderInfo(int senderId)
        {
            var data = dbContext.FaxerInformation.Where(x => x.Id == senderId).FirstOrDefault();
            return data;
        }


        public void SetBankAccountPaymentConfirmationViewModel(BankAccountPaymentConfirmationViewModel vm)
        {
            Common.FaxerSession.BankAccountPaymentConfirmationViewModel = vm;
        }
        public BankAccountPaymentConfirmationViewModel GetBankAccountPaymentConfirmationViewModel()
        {


            BankAccountPaymentConfirmationViewModel vm = new BankAccountPaymentConfirmationViewModel();
            if (Common.FaxerSession.BankAccountPaymentConfirmationViewModel != null)
            {
                vm = Common.FaxerSession.BankAccountPaymentConfirmationViewModel;

            }
            return vm;
        }

        public void SetDebitCreditCardDetail(CreditDebitCardViewModel vm)
        {

            Common.FaxerSession.CreditDebitDetails = vm;
        }

        public CreditDebitCardViewModel GetDebitCreditCardDetail()
        {

            CreditDebitCardViewModel vm = new CreditDebitCardViewModel();

            if (Common.FaxerSession.CreditDebitDetails != null)
            {

                vm = Common.FaxerSession.CreditDebitDetails;
            }
            return vm;
        }
    }
}