using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.BankApi.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.MoblieTransferApi.Models;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TransferZero.Sdk.Model;
using static FAXER.PORTAL.Services.SSenderForAllTransfer;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class AdminForAlTransferServices
    {
        FAXEREntities dbContext = null;
        CommonServices _commonServices = new CommonServices();
        public AdminForAlTransferServices()
        {
            dbContext = new FAXEREntities();
        }


        public ServiceResult<bool> CompleteTransaction(TransactionSummaryVM vm)
        {

            switch (vm.TransferType)
            {
                case TransferType.KiiPayWallet:
                    CompleteKiiPayWalletTransaction(vm);
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
                default:
                    break;
            }


            return new ServiceResult<bool>()
            {

                Data = true,
                Message = "Transaction Successfully Completed",
                Status = ResultStatus.OK
            };
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
                    SenderId = senderAndReceiverDetials.SenderId,
                    MobileNo = BankDeposit.MobileNumber,
                    Service = Service.BankAccount,
                    ReceiverName = BankDeposit.AccountOwnerName,
                    BankId = BankDeposit.BankId,
                    BranchCode = BankDeposit.BranchCode,
                    AccountNo = BankDeposit.AccountNumber,
                    IBusiness = BankDeposit.IsBusiness,
                    Email = BankDeposit.ReceiverEmail,
                    City = BankDeposit.ReceiverCity,
                    PostalCode = BankDeposit.ReceiverPostalCode,
                    Street = BankDeposit.ReceiverStreet,


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
             senderAndReceiverDetials.ReceiverCountry, paymentInfo.SendingAmount, TransactionTransferMethod.BankDeposit, TransactionTransferType.Admin);

            //int BankAccountDepositId = Common.AdminSession.TransactionId;

            string receiptNo = Common.AdminSession.ReceiptNo;
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
                    BankAccountDeposit.CardProcessorApi = vm.CardProcessorApi;
                    ExtraFee = vm.CreditORDebitCardDetials.CreditDebitCardFee;
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
            BankAccountDeposit.PaidFromModule = Module.Staff;
            BankAccountDeposit.TransactionDate = DateTime.Now;
            BankAccountDeposit.TotalAmount = paymentInfo.TotalAmount + ExtraFee;
            BankAccountDeposit.ExtraFee = ExtraFee;
            BankAccountDeposit.ReceiverAccountNo = BankDeposit.AccountNumber;
            BankAccountDeposit.SenderPaymentMode = paymentMethod.SenderPaymentMode;
            BankAccountDeposit.ReceivingAmount = paymentInfo.ReceivingAmount;
            BankAccountDeposit.SenderId = senderAndReceiverDetials.SenderId;
            BankAccountDeposit.ReceivingCountry = BankDeposit.CountryCode;
            BankAccountDeposit.SendingAmount = paymentInfo.SendingAmount;
            BankAccountDeposit.SendingCountry = senderAndReceiverDetials.SenderCountry;
            BankAccountDeposit.ReceiverName = paymentInfo.ReceiverName;
            BankAccountDeposit.ReceiverCountry = BankDeposit.CountryCode;
            BankAccountDeposit.BankCode = BankDeposit.BranchCode;
            BankAccountDeposit.BankId = BankDeposit.BankId;
            BankAccountDeposit.ReceiverMobileNo = BankDeposit.MobileNumber;
            BankAccountDeposit.IsManualDeposit = BankDeposit.IsManualDeposit;
            BankAccountDeposit.BankName = BankDeposit.BankName;
            BankAccountDeposit.IsEuropeTransfer = BankDeposit.IsEuropeTransfer;
            BankAccountDeposit.ReasonForTransfer = BankDeposit.ReasonForTransfer;
            BankAccountDeposit.ReceiptNo = receiptNo;
            BankAccountDeposit.PayingStaffId = Common.AdminSession.StaffId;
            BankAccountDeposit.SendingCurrency = paymentInfo.SendingCurrency;
            BankAccountDeposit.ReceivingCurrency = paymentInfo.ReceivingCurrency;

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

                BankAccountDeposit = _senderBankAccountDepositServices.Add(BankAccountDeposit).Data;

            }
            else
            {
                BankAccountDeposit = _senderBankAccountDepositServices.Add(BankAccountDeposit).Data;

            }



            if (vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == Models.SenderPaymentMode.CreditDebitCard
             && vm.CreditORDebitCardDetials.SaveCard == true)
            {

                SaveCreditDebitCard(vm.CreditORDebitCardDetials, vm.TransferType, BankAccountDeposit.Id, senderAndReceiverDetials.SenderId);
            }

            if (vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == SenderPaymentMode.KiiPayWallet)
            {

                SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
                int WalletId = senderCommonFunc.GetSenderKiiPayWalletInfo(senderAndReceiverDetials.SenderId).Id;
                KiiPayWalletBalOut(WalletId, vm.KiiPayTransferPaymentSummary.TotalAmount);
            }

            // Changes Made For MoneyFex bank Account Deposit
            if (vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == SenderPaymentMode.SavedDebitCreditCard &&
                vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == SenderPaymentMode.CreditDebitCard)
            {
                SavedCreditCardTransactionDetails(vm.CreditORDebitCardDetials, vm.TransferType, BankAccountDeposit.Id);
                AddUpdateLog(vm.CreditORDebitCardDetials, senderAndReceiverDetials.SenderId);
            }


            // Send Email Part
            _senderBankAccountDepositServices.SendEmailAndSms(BankAccountDeposit);



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


        public bool CompleteCashPickupTransaction(TransactionSummaryVM vm)
        {

            SSenderCashPickUp _cashPickUpServices = new SSenderCashPickUp();
            var senderAndReceiverDetials = vm.SenderAndReceiverDetail;
            var paymentInfo = vm.KiiPayTransferPaymentSummary;
            var CreditdebitCardDetials = vm.CreditORDebitCardDetials;
            var paymentMethod = vm.PaymentMethodAndAutoPaymentDetail;
            var cashPickUP = vm.CashPickUpVmStaff;
            //transaction history object


            SFaxingNonCardTransaction service = new SFaxingNonCardTransaction();
            //get unique new mfcn 

            var receiptNumber = service.GetNewReceiptNumberToSave();
            SReceiverDetails receiverService = new SReceiverDetails();

            string[] splittedName = cashPickUP.ReceiverFullName.Trim().Split(null);

            DB.ReceiversDetails recDetailObj = new DB.ReceiversDetails()
            {
                City = "",
                CreatedDate = System.DateTime.Now,
                Country = cashPickUP.Country,
                EmailAddress = cashPickUP.Email,
                FaxerID = senderAndReceiverDetials.SenderId,
                FullName = cashPickUP.ReceiverFullName,
                IsDeleted = false,
                PhoneNumber = cashPickUP.MobileNo,
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
                    recevierExist = receiverService.Add(recDetailObj);
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
                NonCardReceiveId = int.Parse(StaffSession.NonCardReceiversDetails.PreviousReceivers);
            }
            int RecipientId = 0;
            var Recipent = dbContext.Recipients.Where(x => x.ReceiverName.ToLower() == cashPickUP.ReceiverFullName.ToLower() && x.MobileNo == cashPickUP.MobileNo
            && x.Service == Service.CashPickUP).FirstOrDefault();
            if (Recipent == null)
            {
                Recipients model = new Recipients()
                {
                    Country = cashPickUP.Country,
                    SenderId = senderAndReceiverDetials.SenderId,
                    MobileNo = cashPickUP.MobileNo,
                    Service = Service.CashPickUP,
                    ReceiverName = cashPickUP.ReceiverFullName,
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

            var ApiService = Common.Common.GetApiservice(senderAndReceiverDetials.SenderCountry,
            senderAndReceiverDetials.ReceiverCountry, paymentInfo.SendingAmount, TransactionTransferMethod.CashPickUp, TransactionTransferType.Admin);

            // int CashPickUpId = Common.AdminSession.TransactionId;

            // var CashPickUpDetails = dbContext.FaxingNonCardTransaction.Where(x => x.Id == CashPickUpId).FirstOrDefault();
            FaxingNonCardTransaction CashPickUpDetails = new FaxingNonCardTransaction();
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
            CashPickUpDetails.OperatingUserType = OperatingUserType.Admin;
            CashPickUpDetails.ReceiptNumber = Common.AdminSession.ReceiptNo;
            CashPickUpDetails.PayingStaffId = Common.AdminSession.StaffId;
            CashPickUpDetails.SenderId = senderAndReceiverDetials.SenderId;
            CashPickUpDetails.MFCN = _commonServices.GetSenderAccountNoBySenderId(senderAndReceiverDetials.SenderId);
            CashPickUpDetails.RecipientIdenityCardNumber = cashPickUP.IdentityCardNumber;
            CashPickUpDetails.RecipientIdentityCardId = cashPickUP.IdenityCardId;
            CashPickUpDetails.Apiservice = ApiService;
            CashPickUpDetails.SendingCurrency = paymentInfo.SendingCurrency;
            CashPickUpDetails.ReceivingCurrency = paymentInfo.ReceivingCurrency;
            var cashPickUp = dbContext.FaxingNonCardTransaction.Add(CashPickUpDetails);
            dbContext.SaveChanges();

            var TransactionLimitAmount = Common.Common.HasExceededAmountLimit(CashPickUpDetails.SenderId, CashPickUpDetails.SendingCountry,
                CashPickUpDetails.ReceivingCountry, CashPickUpDetails.FaxingAmount, Module.Staff);
            if (TransactionLimitAmount)
            {
                cashPickUp.IsComplianceNeededForTrans = true;
                cashPickUp.FaxingStatus = FaxingStatus.Hold;
            }

            if (CashPickUpDetails.SenderPaymentMode == SenderPaymentMode.CreditDebitCard ||
                CashPickUpDetails.SenderPaymentMode == SenderPaymentMode.SavedDebitCreditCard)
            {
                #region API Call


                BankDepositResponseVm cashPickUpTransactionResult = new BankDepositResponseVm();

                var transResponse = _cashPickUpServices.CreateCashPickTransactionToApi(CashPickUpDetails, TransactionTransferType.Admin, Module.Staff);
                CashPickUpDetails.FaxingStatus = transResponse.CashPickUp.FaxingStatus;
                CashPickUpDetails.TransferReference = transResponse.CashPickUp.TransferReference;
                cashPickUpTransactionResult = transResponse.BankDepositApiResponseVm;
                _cashPickUpServices.AddResponseLog(cashPickUpTransactionResult, CashPickUpDetails.Id);

                dbContext.Entry<FaxingNonCardTransaction>(CashPickUpDetails).State = EntityState.Modified;
                dbContext.SaveChanges();

                #endregion

            }

            //var cashPickUp = dbContext.FaxingNonCardTransaction.Add(CashPickUpDetails);
            //dbContext.Entry<FaxingNonCardTransaction>(CashPickUpDetails).State = EntityState.Modified;
            //db.FaxingNonCardTransaction.AddOrUpdate


            //save transaction for non card
            //service.UpdateTransaction(CashPickUpDetails);
            if (vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == Models.SenderPaymentMode.CreditDebitCard
         && vm.CreditORDebitCardDetials.SaveCard == true)
            {
                SaveCreditDebitCard(vm.CreditORDebitCardDetials, vm.TransferType, cashPickUp.Id, senderAndReceiverDetials.SenderId);
            }
            if (CashPickUpDetails.SenderPaymentMode == SenderPaymentMode.KiiPayWallet)
            {
                SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
                int WalletId = senderCommonFunc.GetSenderKiiPayWalletInfo(senderAndReceiverDetials.SenderId).Id;
                KiiPayWalletBalOut(WalletId, CashPickUpDetails.TotalAmount);
            }
            if (vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == SenderPaymentMode.SavedDebitCreditCard ||
               vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == SenderPaymentMode.CreditDebitCard)
            {
                AddUpdateLog(vm.CreditORDebitCardDetials, senderAndReceiverDetials.SenderId);
                SavedCreditCardTransactionDetails(vm.CreditORDebitCardDetials, vm.TransferType, cashPickUp.Id);

                //AddCreditInfoLog(vm.CreditORDebitCardDetials, obj.Id);
            }
            #region SMS
            //sender
            //string senderName = _commonServices.GetSenderName(senderAndReceiverDetials.SenderId);
            //string fullName = senderName;
            //var names = fullName.Split(' ');
            //string senderFirstName = names[0];

            //var receiverFullName = cashPickUP.ReceiverFullName.Split(' ');
            //string reciverFirstName = receiverFullName[0];

            //string senderPhoneNo = Common.Common.GetCountryPhoneCode(senderAndReceiverDetials.SenderCountry) + senderAndReceiverDetials.SenderPhoneNo;

            //string SendingAmountWithCurrencySymbol = Common.Common.GetCurrencySymbol(CashPickUpDetails.SendingCountry) + " " + CashPickUpDetails.FaxingAmount;
            //string ReceivingAmountWithCurrencySymbol = Common.Common.GetCountryCurrency(CashPickUpDetails.ReceivingCountry) + " " + CashPickUpDetails.ReceivingAmount;
            //string FeeAmountWithCurrencySymbol = Common.Common.GetCountryCurrency(CashPickUpDetails.SendingCountry) + " " + CashPickUpDetails.FaxingFee;
            //string SenderMFCN = _commonServices.GetSenderAccountNoBySenderId(senderAndReceiverDetials.SenderId);
            //SendCashPickUpTOSenderSms(senderFirstName, reciverFirstName, SendingAmountWithCurrencySymbol, senderPhoneNo, SenderMFCN);

            //Receiver
            //string SendingCountry = Common.Common.GetCountryName(CashPickUpDetails.SendingCountry);
            //SendCashPickUpToReceiverSms(SendingCountry, cashPickUP.MobileNo);

            #endregion

            #region Email
            _cashPickUpServices.SendEmailAndSms(CashPickUpDetails);
            #endregion

            #region Notification Section 
            if (vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == SenderPaymentMode.MoneyFexBankAccount)
            {
                DB.Notification notification = new DB.Notification()
                {
                    SenderId = senderAndReceiverDetials.SenderId,
                    ReceiverId = 0,
                    Amount = "",
                    CreationDate = DateTime.Now,
                    Message = "New manual bank deposit (" + _cashPickUpServices.GetMoneyFexBankAccountDeposit().PaymentReference + ")",
                    NotificationReceiver = DB.NotificationFor.Admin,
                    NotificationSender = DB.NotificationFor.Sender,
                    Name = cashPickUP.ReceiverFullName,
                    NotificationKey = _cashPickUpServices.GetMoneyFexBankAccountDeposit().PaymentReference
                };

                SenderCommonServices senderCommonServices = new SenderCommonServices();
                senderCommonServices.SendNotificationToAdmin(notification);

                MoneyFexBankAccountLog MoneyFexBankAccount = new MoneyFexBankAccountLog()
                {
                    IsConfirmed = false,
                    TranscationId = cashPickUp.Id,
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

            var mobileTransfer = vm.MobileMoneyTransferAgent;

            var ApiService = Common.Common.GetApiservice(senderAndReceiverDetials.SenderCountry, senderAndReceiverDetials.ReceiverCountry,
           paymentInfo.SendingAmount, TransactionTransferMethod.OtherWallet, TransactionTransferType.Admin);


            int RecipientId = 0;
            var Recipent = dbContext.Recipients.Where(x => x.ReceiverName.ToLower() == mobileTransfer.ReceiverName.ToLower() && x.MobileNo == mobileTransfer.MobileNumber
            && x.Service == Service.MobileWallet).FirstOrDefault();
            if (Recipent == null)
            {
                Recipients model = new Recipients()
                {
                    Country = mobileTransfer.Country,

                    SenderId = senderAndReceiverDetials.SenderId,
                    MobileNo = mobileTransfer.MobileNumber,
                    Service = Service.MobileWallet,
                    ReceiverName = mobileTransfer.ReceiverName,

                };
                var AddRecipient = dbContext.Recipients.Add(model);
                dbContext.SaveChanges();
                RecipientId = AddRecipient.Id;
            }
            else
            {
                RecipientId = Recipent.Id;
            }

            int mobileTransferId = Common.AdminSession.TransactionId;
            string receiptNo = Common.AdminSession.ReceiptNo;

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

                    MobileMoneyTransferDetails.CardProcessorApi = vm.CardProcessorApi;
                    ExtraFee = vm.CreditORDebitCardDetials.CreditDebitCardFee;
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
            MobileMoneyTransferDetails.PaidFromModule = Module.Staff;
            MobileMoneyTransferDetails.TransactionDate = DateTime.Now;
            MobileMoneyTransferDetails.TotalAmount = paymentInfo.TotalAmount + ExtraFee;
            MobileMoneyTransferDetails.ExtraFee = ExtraFee;
            MobileMoneyTransferDetails.PaidToMobileNo = mobileTransfer.MobileNumber;
            MobileMoneyTransferDetails.PaymentReference = _mobileMoneyTransferServices.GetMoneyFexBankAccountDeposit().PaymentReference;
            MobileMoneyTransferDetails.SenderPaymentMode = paymentMethod.SenderPaymentMode;
            MobileMoneyTransferDetails.ReceivingAmount = paymentInfo.ReceivingAmount;
            MobileMoneyTransferDetails.SenderId = senderAndReceiverDetials.SenderId;
            MobileMoneyTransferDetails.ReceivingCountry = mobileTransfer.Country;
            MobileMoneyTransferDetails.ReceivingCurrency = paymentInfo.ReceivingCurrency;
            MobileMoneyTransferDetails.SendingCurrency = paymentInfo.SendingCurrency;
            MobileMoneyTransferDetails.SendingAmount = paymentInfo.SendingAmount;
            MobileMoneyTransferDetails.SendingCountry = senderAndReceiverDetials.SenderCountry;
            MobileMoneyTransferDetails.ReceiverName = mobileTransfer.ReceiverName;
            MobileMoneyTransferDetails.WalletOperatorId = senderAndReceiverDetials.WalletOperatorId;
            MobileMoneyTransferDetails.RecipientId = RecipientId;
            MobileMoneyTransferDetails.ReceiptNo = receiptNo;
            MobileMoneyTransferDetails.PayingStaffId = Common.AdminSession.StaffId;

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
                obj = _mobileMoneyTransferServices.Add(MobileMoneyTransferDetails).Data;
                MobileMoneyTransferDetails.Id = obj.Id;
            }


            #region API Call
            string ReceiverPhoneCode = Common.Common.GetCountryPhoneCode(mobileTransfer.Country);
            var MobileMoneyTransactionResult = new MTNCameroonResponseParamVm();


            if (paymentMethod.SenderPaymentMode == SenderPaymentMode.CreditDebitCard
                || paymentMethod.SenderPaymentMode == SenderPaymentMode.SavedDebitCreditCard)
            {
                if (vm.IsIdCheckInProgress == false)
                {
                    var transferApiResponse = _mobileMoneyTransferServices.CreateTransactionToApi(MobileMoneyTransferDetails);
                    MobileMoneyTransferDetails.Status = transferApiResponse.status;
                    MobileMoneyTransferDetails.TransferReference = transferApiResponse.response.refId;

                    SMobileMoneyTransferResopnseStatus _sMobileMoneyResposeStatus = new SMobileMoneyTransferResopnseStatus();
                    _sMobileMoneyResposeStatus.AddLog(transferApiResponse.response, mobileTransferId);

                }
            }

            #endregion
            // Update status of transfer
            obj = _mobileMoneyTransferServices.Update(MobileMoneyTransferDetails).Data;


            if (vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == Models.SenderPaymentMode.CreditDebitCard
             && vm.CreditORDebitCardDetials.SaveCard == true)
            {
                vm.TransferType = TransferType.MobileTransfer;
                SaveCreditDebitCard(vm.CreditORDebitCardDetials, vm.TransferType, obj.Id, senderAndReceiverDetials.SenderId);
            }

            if (vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == SenderPaymentMode.KiiPayWallet)
            {
                SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
                int WalletId = senderCommonFunc.GetSenderKiiPayWalletInfo(senderAndReceiverDetials.SenderId).Id;
                KiiPayWalletBalOut(WalletId, vm.KiiPayTransferPaymentSummary.TotalAmount);
            }

            if (vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == SenderPaymentMode.SavedDebitCreditCard ||
               vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == SenderPaymentMode.CreditDebitCard)
            {
                SavedCreditCardTransactionDetails(vm.CreditORDebitCardDetials, vm.TransferType, obj.Id);
                AddUpdateLog(vm.CreditORDebitCardDetials, senderAndReceiverDetials.SenderId);
            }
            //AddCreditInfoLog(vm.CreditORDebitCardDetials, obj.Id);

            #region Send Sms and email

            _mobileMoneyTransferServices.SendEmailAndSms(obj);
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

        public void CompleteKiiPayWalletTransaction(TransactionSummaryVM vm)
        {

            switch (vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode)
            {
                case SenderPaymentMode.SavedDebitCreditCard:

                    KiiPayWalletPaymentUsingCreditDebitCard(vm);
                    AddUpdateLog(vm.CreditORDebitCardDetials, vm.SenderAndReceiverDetail.SenderId);
                    break;
                case SenderPaymentMode.CreditDebitCard:
                    KiiPayWalletPaymentUsingCreditDebitCard(vm);
                    AddUpdateLog(vm.CreditORDebitCardDetials, vm.SenderAndReceiverDetail.SenderId);
                    break;
                case SenderPaymentMode.KiiPayWallet:
                    KiiPayWalletBalIN(vm.SenderAndReceiverDetail.ReceiverId, vm.KiiPayTransferPaymentSummary.ReceivingAmount);
                    PayUsingKiiPayWallet(vm);
                    break;
                case SenderPaymentMode.MoneyFexBankAccount:
                    KiiPayWalletPaymentUsingCreditDebitCard(vm);
                    break;
                default:
                    break;
            }


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
                SenderWalletId = senderCommonFunc.GetSenderKiiPayWalletInfo(vm.SenderAndReceiverDetail.SenderId).Id,
                ReceiverWalletId = senderAndReceiverInfo.ReceiverId,
                SendingCountry = senderAndReceiverInfo.SenderCountry,
                ReceivingCountry = senderAndReceiverInfo.ReceiverCountry,
                TransactionDate = DateTime.Now,
                ReceivingMobileNumber = senderAndReceiverInfo.ReceiverMobileNo,
                RecievingAmount = paymentsummary.ReceivingAmount,
                TotalAmount = paymentsummary.TotalAmount,
                SenderId = senderAndReceiverInfo.SenderId,
                TransactionFromPortal = DB.TransactionFrom.AdminPortal
            };


            int senderWalletId = senderCommonFunc.GetSenderKiiPayWalletInfo(senderAndReceiverInfo.SenderId).Id;
            KiiPayWalletBalOut(senderWalletId, vm.KiiPayTransferPaymentSummary.TotalAmount);

            KiipayWalletStandingOrderPayment(vm);
            return true;
        }
        public void KiipayWalletStandingOrderPayment(TransactionSummaryVM vm)
        {

            if ((vm.PaymentMethodAndAutoPaymentDetail.EnableAutoPayment == true)
           && vm.PaymentMethodAndAutoPaymentDetail.AutoPaymentAmount > 0 && (int)vm.PaymentMethodAndAutoPaymentDetail.AutopaymentFrequency > 0)
            {
                var cardDetails = dbContext.SavedCard.Where(x => x.UserId == vm.SenderAndReceiverDetail.SenderId).FirstOrDefault();
                if (cardDetails != null)
                {
                    var data = dbContext.OtherMFTCCardAutoTopUpInformation.Where(x => x.MFTCCardId == vm.SenderAndReceiverDetail.ReceiverId && x.FaxerId == vm.SenderAndReceiverDetail.SenderId).FirstOrDefault();


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
                            FaxerId = vm.SenderAndReceiverDetail.SenderId,
                            TopUpReference = "",
                            FrequencyDetails = vm.PaymentMethodAndAutoPaymentDetail.PaymentDay

                        };
                        dbContext.OtherMFTCCardAutoTopUpInformation.Add(autoTopUpInformation);
                        dbContext.SaveChanges();

                    }
                }
            }


        }


        public void KiiPayWalletBalOut(int WalletId, decimal Amount)
        {


            var senderWalletData = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == WalletId).FirstOrDefault();

            senderWalletData.CurrentBalance = senderWalletData.CurrentBalance - Amount;
            dbContext.Entry(senderWalletData).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();


        }


        public void KiiPayWalletBalIN(int WalletId, decimal Amount)
        {

            var receiverWalletData = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == WalletId).FirstOrDefault();


            //deducting amount from sender's wallet

            receiverWalletData.CurrentBalance = receiverWalletData.CurrentBalance + Amount;
            dbContext.Entry(receiverWalletData).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }

        public bool KiiPayWalletPaymentUsingCreditDebitCard(TransactionSummaryVM vm)
        {
            var paymentsummary = vm.KiiPayTransferPaymentSummary;
            var senderAndReceiverInfo = vm.SenderAndReceiverDetail;
            SAgentKiiPayWalletTransferServices _kiiPayWalletTransfer = new SAgentKiiPayWalletTransferServices();
            var sender = _kiiPayWalletTransfer.GetAdminSendMoneToKiiPayWalletViewModel();
            var receiverinfo = _kiiPayWalletTransfer.GetAdminKiiPayReceiverDetailsInformationViewModel();

            var PaymentMethodAndAutoPaymentFrequencyInfo = vm.PaymentMethodAndAutoPaymentDetail;



            //var KiiPayPersonalWalletInformation = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == receiverinfo.MobileNo).FirstOrDefault();

            //int RecipientId = 0;
            //int KiiPayPersonalWalletId = 0;
            //if (KiiPayPersonalWalletInformation == null)
            //{
            //    Recipients model = new Recipients()
            //    {
            //        Country = receiverinfo.Country,
            //        SenderId = sender.Id,
            //        MobileNo = receiverinfo.MobileNo,
            //        Service = Service.KiiPayWallet,
            //        ReceiverName = receiverinfo.ReceiverFullName,
            //        AccountNo = receiverinfo.MobileNo,

            //    };
            //    var AddRecipient = dbContext.Recipients.Add(model);
            //    dbContext.SaveChanges();
            //    RecipientId = AddRecipient.Id;
            //}
            //else
            //{
            //    RecipientId = senderAndReceiverInfo.ReceiverId;
            //}



            SFaxingTopUpCardTransaction service = new SFaxingTopUpCardTransaction();

            STopUpSomeoneElseCard TopUpSomeoneElseServices = new STopUpSomeoneElseCard();
            string ReceiptNumber = service.GetNewMFTCCardTopUpReceipt();


            DB.TopUpSomeoneElseCardTransaction obj = new DB.TopUpSomeoneElseCardTransaction()
            {
                //Change Static value in KiiPayPersonalWalletId 
                KiiPayPersonalWalletId = 1,
                FaxerId = sender.Id,
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

            if (vm.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode == SenderPaymentMode.CreditDebitCard
                && vm.CreditORDebitCardDetials.SaveCard == true)
            {

                SaveCreditDebitCard(vm.CreditORDebitCardDetials, vm.TransferType, obj.Id, vm.SenderAndReceiverDetail.SenderId);
            }

            SavedCreditCardTransactionDetails(vm.CreditORDebitCardDetials, vm.TransferType, obj.Id);

            KiipayWalletStandingOrderPayment(vm);
            return true;
        }

        public FaxerInformation AddSender(FaxerInformation faxerDetails)
        {
            var data = dbContext.FaxerInformation.Add(faxerDetails);
            dbContext.SaveChanges();
            return data;
        }

        public void SavedCreditCardTransactionDetails(CreditDebitCardViewModel model, TransferType transferType, int TransactionId)
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

        public void SaveCreditDebitCard(CreditDebitCardViewModel model, TransferType transferType, int TransactionId, int SenderId)
        {

            int SavedCardCount = dbContext.SavedCard.Where(x => x.UserId == SenderId).Count();
            if (SavedCardCount < 2)
            {
                DB.SavedCard savedCardObject = new DB.SavedCard()
                {
                    Type = model.CreditDebitCardType,
                    CardName = model.NameOnCard.Encrypt(),
                    EMonth = model.EndMM.Encrypt(),
                    EYear = model.EndYY.Encrypt(),
                    CreatedDate = System.DateTime.Now,
                    UserId = SenderId,
                    Num = model.CardNumber.Encrypt(),
                    ClientCode = model.SecurityCode.Encrypt()

                };
                SSavedCard cardservices = new SSavedCard();
                savedCardObject = cardservices.Add(savedCardObject);

            }

        }

        public bool AddUpdateLog(CreditDebitCardViewModel debitCardViewModel, int SenderId)
        {

            SCreditDebitCardUsage creditDebitCardUsage = new SCreditDebitCardUsage();
            creditDebitCardUsage.AddOrUpdateCreditCardUsageLog(new CreditCardUsageLog()
            {
                CardNum = Common.Common.FormatSavedCardNumber(debitCardViewModel.CardNumber),
                Count = 1,
                Module = Module.Staff,
                SenderId = SenderId,
                UpdatedDateTime = DateTime.Now
            });
            return true;
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
        public BankAccountDeposit getBankAccountDeposit(int id)
        {
            return dbContext.BankAccountDeposit.Find(id);
        }

        public MobileMoneyTransfer UpdateOtherMobile(ReceiverDetailsInformationViewModel model)
        {
            var data = dbContext.MobileMoneyTransfer.Find(model.Id);
            if (data != null)
            {
                data.ReceiverName = model.ReceiverName;
                data.ReceivingCountry = model.Country;
                data.PaidToMobileNo = model.MobileNumber;
                data.WalletOperatorId = model.MobileWalletProvider;

                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                var receipient = dbContext.Recipients.Find(data.RecipientId);
                if (receipient != null)
                {
                    receipient.Country = model.Country;
                    receipient.ReceiverName = model.ReceiverName;
                    receipient.MobileNo = model.MobileNumber;
                    receipient.MobileWalletProvider = model.MobileWalletProvider;

                    dbContext.Entry(receipient).State = EntityState.Modified;
                    dbContext.SaveChanges();

                }
                return data;


            }
            else
            {
                return null;
            }

        }
        public BankAccountDeposit UpdateBankAccountDeposit(SenderBankAccountDepositVm model)
        {
            var data = dbContext.BankAccountDeposit.Find(model.Id);
            if (data != null)
            {
                data.ReceiverName = model.AccountOwnerName;
                data.BankCode = model.BranchCode;
                data.BankId = model.BankId;
                data.ReceiverMobileNo = model.MobileNumber;
                data.ReceiverAccountNo = model.RecentAccountNumber;
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                var receipient = dbContext.Recipients.Find(data.RecipientId);
                if (receipient != null)
                {
                    receipient.AccountNo = model.AccountNumber;
                    receipient.BankId = model.BankId;
                    receipient.BranchCode = model.BranchCode;
                    receipient.MobileNo = model.MobileNumber;
                    receipient.ReceiverName = model.AccountOwnerName;

                    dbContext.Entry(receipient).State = EntityState.Modified;
                    dbContext.SaveChanges();

                }
                return data;


            }
            else
            {
                return null;
            }

        }
        public FaxingNonCardTransaction UpdateCashPickUp(CashPickUpReceiverDetailsInformationViewModel model)
        {
            var data = dbContext.FaxingNonCardTransaction.Find(model.Id);
            if (data != null)
            {

                data.ReceivingCountry = model.Country;

                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                var receipient = dbContext.Recipients.Find(data.RecipientId);
                if (receipient != null)
                {

                    receipient.MobileNo = model.MobileNo;
                    receipient.ReceiverName = model.ReceiverFullName;
                    receipient.Reason = model.ReasonForTransfer;
                    receipient.Country = model.Country;
                    dbContext.Entry(receipient).State = EntityState.Modified;
                    dbContext.SaveChanges();

                }
                return data;


            }
            else
            {
                return null;
            }

        }

    }
}