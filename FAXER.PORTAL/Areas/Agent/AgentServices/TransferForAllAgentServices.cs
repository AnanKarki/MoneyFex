using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.BankApi;
using FAXER.PORTAL.BankApi.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.MoblieTransferApi.Models;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class TransferForAllAgentServices
    {
        FAXEREntities dbContext = null;
        private RegisteredAgentType _registeredAgentType;
        public TransferForAllAgentServices()
        {
            dbContext = new FAXEREntities();
            _registeredAgentType = Common.Common.GetRegisteredAgentType(Common.AgentSession.AgentInformation.Id);

        }
        public void SetTransactionSummary(AgentTransactionSummaryVm model)
        {

            Common.AgentSession.AgentTransactionSummaryVm = model;
        }
        public AgentTransactionSummaryVm GetTransactionSummary()
        {
            AgentTransactionSummaryVm vm = new AgentTransactionSummaryVm();
            if (Common.AgentSession.AgentTransactionSummaryVm != null)
            {

                vm = Common.AgentSession.AgentTransactionSummaryVm;
            }
            return vm;
        }
        public int CompleteTransaction(TransactionTransferMethod transferMethod)
        {
            int transactionId = 0;
            switch (transferMethod)
            {
                case TransactionTransferMethod.CashPickUp:
                    transactionId = CompleteCashPickUpTransaction();
                    break;
                case TransactionTransferMethod.KiiPayWallet:
                    break;
                case TransactionTransferMethod.OtherWallet:
                    transactionId = CompleteOtherWalletTransaction();
                    break;
                case TransactionTransferMethod.BankDeposit:
                    transactionId = CompleteBankDepositTransaction();
                    break;
            }

            return transactionId;
        }

        public int CompleteBankDepositTransaction()
        {
            SSenderBankAccountDeposit _senderBankAccountDepositServices = new SSenderBankAccountDeposit();
            var agentInfo = AgentSession.AgentInformation;
            var transactionSummaryvm = GetTransactionSummary();

            int senderId = AddOrUpdateSender(transactionSummaryvm.SenderDetails);
            transactionSummaryvm.RecipientDetails.SenderId = senderId;

            int recipentId = AddOrUpdateRecipient(transactionSummaryvm.RecipientDetails);

            var paymentInfo = transactionSummaryvm.PaymentSummary;
            var receiverInfo = transactionSummaryvm.RecipientDetails;

            var transferType = TransactionTransferType.Agent;
            if (_registeredAgentType == RegisteredAgentType.AuxAgent)
            {
                transferType = TransactionTransferType.AuxAgent;
            }
            var ApiService = Common.Common.GetApiservice(transactionSummaryvm.SenderDetails.Country,
                                  transactionSummaryvm.RecipientDetails.Country, paymentInfo.SendingAmount,
                                   TransactionTransferMethod.BankDeposit, transferType, agentInfo.Id);

            decimal MFRate = Common.Common.GetMFRate(transactionSummaryvm.SenderDetails.Country, transactionSummaryvm.RecipientDetails.Country, TransactionTransferMethod.BankDeposit);

            BankAccountDeposit BankAccountDeposit = new BankAccountDeposit()
            {
                ExchangeRate = paymentInfo.ExchangeRate,
                Fee = paymentInfo.Fee,
                PaidFromModule = Module.Agent,
                TransactionDate = DateTime.Now,
                TotalAmount = paymentInfo.TotalAmount,
                ReceiverAccountNo = transactionSummaryvm.RecipientDetails.AccountNo,
                SenderPaymentMode = PORTAL.Models.SenderPaymentMode.Cash,
                ReceivingAmount = paymentInfo.ReceivingAmount,
                SenderId = senderId,
                ReceivingCountry = paymentInfo.ReceivingCountry,
                SendingAmount = paymentInfo.SendingAmount,
                SendingCountry = paymentInfo.SendingCountry,
                ReceiverName = receiverInfo.ReceiverName,
                ReceiverCountry = paymentInfo.ReceivingCountry,
                ReceiptNo = paymentInfo.IsManualDeposit == false ? Common.Common.GenerateBankAccountDepositReceiptNoforAgnet(6) : Common.Common.GenerateManualBankAccountDepositReceiptNo(6),

                PayingStaffName = Common.AgentSession.LoggedUser.PayingAgentStaffName,
                PayingStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId,
                AgentCommission = paymentInfo.CommissionFee,
                BankId = receiverInfo.BankId,
                BankCode = GetBranchCode(receiverInfo.BankId),
                ReceiverMobileNo = receiverInfo.MobileNo,
                IsManualDeposit = paymentInfo.IsManualDeposit,
                Status = paymentInfo.IsManualDeposit == true ? BankDepositStatus.Incomplete : BankDepositStatus.Confirm,
                Apiservice = ApiService,
                BankName = Common.Common.getBankName(receiverInfo.BankId),
                IsEuropeTransfer = paymentInfo.IsEuropeTransfer,
                ReasonForTransfer = receiverInfo.Reason,
                Margin = Common.Common.GetMargin(MFRate, paymentInfo.ExchangeRate, paymentInfo.SendingAmount, paymentInfo.Fee),
                MFRate = MFRate,
                RecipientId = recipentId,
                SendingCurrency = paymentInfo.SendingCurrency,
                ReceivingCurrency = paymentInfo.ReceivingCurrency,
            };
            if (BankAccountDeposit.SendingCountry == BankAccountDeposit.ReceivingCountry)
            {
                BankAccountDeposit.PaymentType = PaymentType.Local;
            }
            else
            {
                BankAccountDeposit.PaymentType = PaymentType.International;

            }
            var obj = _senderBankAccountDepositServices.Add(BankAccountDeposit).Data;

            SBankDepositResponseStatus sBankDepositResponseStatus = new SBankDepositResponseStatus();

            var bankdepositTransactionResult = new BankDepositResponseVm();


            // Create bank Api response log 


            if (BankAccountDeposit.IsManualDeposit == false)
            {

                var SenderDocumentApprovalStatus = Common.Common.IsSenderIdApproved(senderId, _registeredAgentType);

                if (SenderDocumentApprovalStatus)
                {
                    var transResponse = _senderBankAccountDepositServices.CreateBankTransactionToApi(BankAccountDeposit, transferType);
                    BankAccountDeposit.Status = transResponse.BankAccountDeposit.Status;
                    bankdepositTransactionResult = transResponse.BankDepositApiResponseVm;
                    obj = _senderBankAccountDepositServices.Update(BankAccountDeposit).Data;
                    sBankDepositResponseStatus.AddLog(bankdepositTransactionResult, obj.Id);

                }
                else
                {
                    BankAccountDeposit.Status = BankDepositStatus.IdCheckInProgress;
                }
            }
            obj = _senderBankAccountDepositServices.Update(BankAccountDeposit).Data;

            if (!agentInfo.IsAUXAgent)
            {
                _senderBankAccountDepositServices.SendEmailAndSms(obj);
            }
            else
            {
                UpdateAccountBalanceForAux(agentInfo.Id, obj.TotalAmount);
            }
            return obj.Id;
        }

        private string GetBranchCode(int bankId)
        {
            string branchCode = dbContext.Bank.Where(x => x.Id == bankId).Select(x => x.Code).FirstOrDefault();
            return branchCode;
        }

        private int CompleteCashPickUpTransaction()
        {
            var transactionSummaryvm = GetTransactionSummary();

            var agentInfo = AgentSession.AgentInformation;
            int senderId = AddOrUpdateSender(transactionSummaryvm.SenderDetails);
            transactionSummaryvm.RecipientDetails.SenderId = senderId;

            int recipentId = AddOrUpdateRecipient(transactionSummaryvm.RecipientDetails);

            int nonCardReceiverId = AddOrUpdateNonCardReceiver(transactionSummaryvm.RecipientDetails);

            SFaxingNonCardTransaction getMFCN = new SFaxingNonCardTransaction();

            var MFCN = getMFCN.GetNewMFCNToSave();
            SCashPickUpTransferService sCashPickUpTransferService = new SCashPickUpTransferService();
            var receiptNumber = sCashPickUpTransferService.GetNewAgentMoneyTransferReceipt();

            decimal MFRate = Common.Common.GetMFRate(transactionSummaryvm.SenderDetails.Country,
                                                     transactionSummaryvm.RecipientDetails.Country,
                                                     TransactionTransferMethod.CashPickUp);

            var transferType = TransactionTransferType.Agent;
            if (_registeredAgentType == RegisteredAgentType.AuxAgent)
            {
                transferType = TransactionTransferType.AuxAgent;
            }

            var ApiService = Common.Common.GetApiservice(Common.AgentSession.AgentInformation.CountryCode,
                                                         transactionSummaryvm.RecipientDetails.Country,
                                                         transactionSummaryvm.PaymentSummary.SendingAmount,
                                                         TransactionTransferMethod.CashPickUp,
                                                         transferType, agentInfo.Id);
            var paymentInfo = transactionSummaryvm.PaymentSummary;
            DB.FaxingNonCardTransaction nonCardTransaction = new DB.FaxingNonCardTransaction()
            {
                FaxingAmount = paymentInfo.SendingAmount,
                FaxingFee = paymentInfo.Fee,
                ExchangeRate = paymentInfo.ExchangeRate,
                TransactionDate = DateTime.Now,
                ReceivingAmount = paymentInfo.ReceivingAmount,
                NonCardRecieverId = nonCardReceiverId,
                MFCN = MFCN,
                ReceiptNumber = MFCN,
                OperatingUserType = OperatingUserType.Agent,
                PayingStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId,
                AgentStaffName = Common.AgentSession.LoggedUser.PayingAgentStaffName,
                AgentCommission = paymentInfo.CommissionFee,
                ReceivingCountry = paymentInfo.ReceivingCountry,
                TotalAmount = paymentInfo.TotalAmount,
                SendingCountry = Common.AgentSession.AgentInformation.CountryCode,
                PaymentMethod = Enum.GetName(typeof(SenderPaymentMode), SenderPaymentMode.Cash),
                SenderId = senderId,
                Margin = Common.Common.GetMargin(MFRate, paymentInfo.ExchangeRate,
                                                 paymentInfo.SendingAmount, paymentInfo.Fee),
                MFRate = MFRate,
                RecipientId = recipentId,
                RecipientIdenityCardNumber = transactionSummaryvm.RecipientDetails.IdentityCardNumber,
                RecipientIdentityCardId = transactionSummaryvm.RecipientDetails.IdentityCardId,
                Apiservice = ApiService,
                ReceivingCurrency = paymentInfo.ReceivingCurrency,
                SendingCurrency = paymentInfo.SendingCurrency
            };

            var IsPayoutFlowControlEnabled = Common.Common.IsPayoutFlowControlEnabled(nonCardTransaction.SendingCountry, nonCardTransaction.ReceivingCountry,
                null, TransactionTransferMethod.CashPickUp, 0);
            if (IsPayoutFlowControlEnabled == false)
            {
                nonCardTransaction.FaxingStatus = FaxingStatus.Paused;
            }
            var obj = dbContext.FaxingNonCardTransaction.Add(nonCardTransaction);
            dbContext.SaveChanges();
            #region API Call
            SSenderCashPickUp _cashPickUpServices = new SSenderCashPickUp();

            var SenderDocumentApprovalStatus = Common.Common.IsSenderIdApproved(senderId, _registeredAgentType);

            if (SenderDocumentApprovalStatus)
            {
                BankDepositResponseVm cashPickUpTransactionResult = new BankDepositResponseVm();
                var transResponse = _cashPickUpServices.CreateCashPickTransactionToApi(obj, transferType, Module.Agent);
                obj.FaxingStatus = transResponse.CashPickUp.FaxingStatus;
                obj.TransferReference = transResponse.CashPickUp.TransferReference;
                cashPickUpTransactionResult = transResponse.BankDepositApiResponseVm;
                _cashPickUpServices.AddResponseLog(cashPickUpTransactionResult, obj.Id);
            }
            else
            {
                obj.FaxingStatus = FaxingStatus.IdCheckInProgress;
            }

            dbContext.Entry(obj).State = EntityState.Modified;
            dbContext.SaveChanges();

            if (!agentInfo.IsAUXAgent)
            {
                _cashPickUpServices.SendEmailAndSms(obj);
            }
            else
            {
                UpdateAccountBalanceForAux(agentInfo.Id, obj.TotalAmount);
            }
            #endregion
            #region Notification Section 

            var CashPickUp = dbContext.FaxingNonCardTransaction.Where(x => x.Id == nonCardTransaction.Id).FirstOrDefault();
            DB.Notification notification = new DB.Notification()
            {
                SenderId = (int)nonCardTransaction.PayingStaffId,
                ReceiverId = CashPickUp.NonCardReciever.FaxerID,
                Amount = Common.Common.GetCountryCurrency(nonCardTransaction.SendingCountry) + " " + nonCardTransaction.FaxingAmount,
                CreationDate = DateTime.Now,
                Title = DB.Title.CashPickUpTransfer,
                Message = "MFCN  :" + CashPickUp.MFCN,
                NotificationReceiver = DB.NotificationFor.Sender,
                NotificationSender = DB.NotificationFor.Agent,
                Name = nonCardTransaction.NonCardReciever.FullName
            };

            SenderCommonServices senderCommonServices = new SenderCommonServices();
            senderCommonServices.SendNotification(notification);
            #endregion
            Common.AgentSession.SenderId = senderId;
            return obj.Id;
        }
        private int CompleteOtherWalletTransaction()
        {
            var transactionSummaryvm = GetTransactionSummary();
            var agentInfo = AgentSession.AgentInformation;

            int senderId = AddOrUpdateSender(transactionSummaryvm.SenderDetails);
            transactionSummaryvm.RecipientDetails.SenderId = senderId;


            int recipentId = AddOrUpdateRecipient(transactionSummaryvm.RecipientDetails);

            decimal MFRate = Common.Common.GetMFRate(transactionSummaryvm.SenderDetails.Country,
                                                   transactionSummaryvm.RecipientDetails.Country,
                                                   TransactionTransferMethod.OtherWallet);
            var paymentInfo = transactionSummaryvm.PaymentSummary;
            DB.MobileMoneyTransfer mobileTransferData = new DB.MobileMoneyTransfer()
            {
                ExchangeRate = paymentInfo.ExchangeRate,
                Fee = paymentInfo.Fee,
                PaidFromModule = Module.Agent,
                TransactionDate = DateTime.Now,
                TotalAmount = paymentInfo.TotalAmount,
                PaidToMobileNo = transactionSummaryvm.RecipientDetails.MobileNo,
                PaymentReference = "",
                ReceivingAmount = paymentInfo.ReceivingAmount,
                SenderId = senderId,
                ReceivingCountry = paymentInfo.ReceivingCountry,
                SendingAmount = paymentInfo.SendingAmount,
                SendingCountry = paymentInfo.SendingCountry,
                SenderPaymentMode = PORTAL.Models.SenderPaymentMode.Cash,
                ReceiverName = transactionSummaryvm.RecipientDetails.ReceiverName,
                ReceiptNo = Common.Common.GenerateMobileMoneyTransferReceiptNoForAgent(6),
                PayingStaffName = Common.AgentSession.LoggedUser.PayingAgentStaffName,
                PayingStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId,
                AgentCommission = paymentInfo.CommissionFee,
                WalletOperatorId = transactionSummaryvm.RecipientDetails.MobileWalletProvider,
                Margin = Common.Common.GetMargin(MFRate, paymentInfo.ExchangeRate, paymentInfo.SendingAmount, paymentInfo.Fee),
                MFRate = MFRate,
                SendingCurrency = paymentInfo.SendingCurrency,
                ReceivingCurrency = paymentInfo.ReceivingCurrency,
                RecipientId = recipentId,
            };
            if (mobileTransferData.SendingCountry == mobileTransferData.ReceivingCountry)
            {
                mobileTransferData.PaymentType = PaymentType.Local;
            }
            else
            {
                mobileTransferData.PaymentType = PaymentType.International;

            }

            SSenderMobileMoneyTransfer _mobileMoneyTransferServices = new SSenderMobileMoneyTransfer();
            var transferType = TransactionTransferType.Agent;
            if (_registeredAgentType == RegisteredAgentType.AuxAgent)
            {
                transferType = TransactionTransferType.AuxAgent;
            }
            var ApiService = Common.Common.GetApiservice(mobileTransferData.SendingCountry, mobileTransferData.ReceivingCountry,
                     paymentInfo.SendingAmount, TransactionTransferMethod.OtherWallet, transferType, agentInfo.Id);
            mobileTransferData.Apiservice = ApiService;
            var MobileMoneyTransfer = _mobileMoneyTransferServices.Add(mobileTransferData).Data;

            var SenderDocumentApprovalStatus = Common.Common.IsSenderIdApproved(senderId, _registeredAgentType);
            if (SenderDocumentApprovalStatus)
            {
                #region API Call
                var MobileMoneyTransactionResult = new MTNCameroonResponseParamVm();
                var transferApiResponse = _mobileMoneyTransferServices.CreateTransactionToApi(mobileTransferData, transferType);
                MobileMoneyTransactionResult = transferApiResponse.response;
                MobileMoneyTransfer.Status = transferApiResponse.status;
                #endregion

                #region   Create Transaction Log 
                SMobileMoneyTransferResopnseStatus _sMobileMoneyResposeStatus = new SMobileMoneyTransferResopnseStatus();
                _sMobileMoneyResposeStatus.AddLog(MobileMoneyTransactionResult, MobileMoneyTransfer.Id);
                #endregion
            }
            else
            {
                MobileMoneyTransfer.Status = MobileMoneyTransferStatus.IdCheckInProgress;
            }
            dbContext.Entry(MobileMoneyTransfer).State = EntityState.Modified;
            dbContext.SaveChanges();
            if (!agentInfo.IsAUXAgent)
            {
                _mobileMoneyTransferServices.SendEmailAndSms(MobileMoneyTransfer);
            }
            else
            {
                UpdateAccountBalanceForAux(agentInfo.Id, MobileMoneyTransfer.TotalAmount);
            }
            return MobileMoneyTransfer.Id;
        }



        private int AddOrUpdateSender(CashPickupInformationViewModel senderInfo)
        {
            int senderId = 0;
            SFaxerSignUp faxerSignUpService = new SFaxerSignUp();
            string accountNo = "";
            if (_registeredAgentType == RegisteredAgentType.AuxAgent)
            {
                accountNo = "AMF" + faxerSignUpService.GetNewAccount(6);
            }
            else
            {
                accountNo = faxerSignUpService.GetNewAccount(10);
            }

            if (senderInfo.Id == 0)
            {
                var DOB = new DateTime().AddDays(senderInfo.Day).AddMonths((int)senderInfo.Month).AddYears(senderInfo.Year);

                DB.FaxerInformation FaxerDetails = new DB.FaxerInformation()
                {
                    FirstName = senderInfo.FirstName,
                    MiddleName = senderInfo.MiddleName,
                    LastName = senderInfo.LastName,
                    Address1 = senderInfo.AddressLine1,
                    City = senderInfo.City,
                    Country = senderInfo.Country,
                    Email = senderInfo.Email,
                    PhoneNumber = senderInfo.MobileNo,
                    IdCardNumber = senderInfo.IdNumber,
                    IdCardType = senderInfo.IdType.ToString(),
                    IssuingCountry = senderInfo.IssuingCountry,
                    RegisteredByAgent = true,
                    IsDeleted = false,
                    IdCardExpiringDate = DateTime.Now,
                    AccountNo = accountNo,
                    DateOfBirth = DOB,
                    Address2 = senderInfo.AddressLine2,
                    GGender = senderInfo.Gender.ToInt(),
                };

                dbContext.FaxerInformation.Add(FaxerDetails);
                dbContext.SaveChanges();
                senderId = FaxerDetails.Id;
                SenderRegisteredByAgent senderRegisteredByAgent = new SenderRegisteredByAgent()
                {
                    AgentId = Common.AgentSession.LoggedUser.Id,
                    IsAuxAgent = Common.AgentSession.LoggedUser.IsAUXAgent,
                    SenderId = senderId,
                };
                dbContext.SenderRegisteredByAgent.Add(senderRegisteredByAgent);
                dbContext.SaveChanges();
            }
            else
            {
                var sender = dbContext.FaxerInformation.Where(x => x.Id == senderInfo.Id).FirstOrDefault();
                senderId = sender.Id;
                sender.IdCardNumber = senderInfo.IdNumber;
                sender.IdCardType = senderInfo.IdType.ToString();
                sender.IdCardExpiringDate = senderInfo.ExpiryDate;
                sender.IssuingCountry = senderInfo.IssuingCountry;
                dbContext.Entry(sender).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
            return senderId;
        }
        private int AddOrUpdateRecipient(RecipientsViewModel receiver)
        {
            int RecipientId = 0;
            var Recipent = dbContext.Recipients.Where(x => x.ReceiverName.ToLower() == receiver.ReceiverName.ToLower() &&
                                                           x.MobileNo == receiver.MobileNo &&
                                                           x.Service == Service.CashPickUP).FirstOrDefault();

            var recipients = BindInRecipientModel(receiver);
            if (Recipent == null)
            {
                var AddRecipient = dbContext.Recipients.Add(recipients);
                dbContext.SaveChanges();
                RecipientId = AddRecipient.Id;
            }
            else
            {
                RecipientId = Recipent.Id;
            }
            return RecipientId;
        }
        private Recipients BindInRecipientModel(RecipientsViewModel receiver)
        {
            Recipients recipients = new Recipients()
            {
                Id = receiver.Id,
                Country = receiver.Country,
                SenderId = receiver.SenderId,
                MobileNo = receiver.MobileNo,
                Service = receiver.Service,
                ReceiverName = receiver.ReceiverName,
                Reason = receiver.Reason,
                City = receiver.ReceiverCity,
                Email = receiver.ReceiverEmail,
                AccountNo = receiver.MobileNo,
                IdentificationNumber = receiver.IdentityCardNumber,
                IdentificationTypeId = receiver.IdentityCardId,
                BankId = receiver.BankId,
                BranchCode = receiver.BranchCode,
                IBusiness = receiver.IBusiness,
                IsBanned = false,
                MobileWalletProvider = receiver.MobileWalletProvider,
                PostalCode = receiver.ReceiverPostalCode,
                Street = receiver.ReceiverStreet
            };
            return recipients;
        }
        private int AddOrUpdateNonCardReceiver(RecipientsViewModel receiver)
        {
            int nonCardReceiverId = 0;
            string[] splittedName = receiver.ReceiverName.Trim().Split(null);
            DB.ReceiversDetails receiversDetails = new DB.ReceiversDetails()
            {
                FirstName = splittedName[0],
                MiddleName = splittedName.Count() > 2 ? splittedName[1] : " ",
                LastName = splittedName[splittedName.Count() - 1],
                City = receiver.ReceiverCity,
                Country = receiver.Country,
                CreatedDate = DateTime.Now,
                EmailAddress = receiver.ReceiverEmail,
                PhoneNumber = receiver.MobileNo,
                FaxerID = receiver.SenderId,
                FullName = receiver.ReceiverName,
            };
            var nonCardReceiver = dbContext.ReceiversDetails.Where(x => x.PhoneNumber == receiver.MobileNo).FirstOrDefault();
            if (nonCardReceiver == null)
            {
                dbContext.ReceiversDetails.Add(receiversDetails);
                dbContext.SaveChanges();
                nonCardReceiverId = receiversDetails.Id;
            }
            else
            {
                nonCardReceiverId = receiversDetails.Id;
            }
            return nonCardReceiverId;
        }
        private void UpdateAccountBalanceForAux(int agentId, decimal totalAmount)
        {
            var fundaccountBalance = dbContext.AgentAccountBalance.Where(x => x.AgentId == agentId).FirstOrDefault();
            fundaccountBalance.UpdateDateTime = DateTime.Now;
            fundaccountBalance.TotalBalance = fundaccountBalance.TotalBalance - totalAmount;
            dbContext.Entry(fundaccountBalance).State = EntityState.Modified;
            dbContext.SaveChanges();
        }
    }
}