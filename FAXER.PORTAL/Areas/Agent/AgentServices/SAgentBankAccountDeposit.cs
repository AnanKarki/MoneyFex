using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.BankApi;
using FAXER.PORTAL.BankApi.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TransferZero.Sdk.Model;
using Twilio.TwiML.Voice;
using static FAXER.PORTAL.Services.SSenderForAllTransfer;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class SAgentBankAccountDeposit
    {

        DB.FAXEREntities dbContext = null;
        private RegisteredAgentType _registeredAgentType;

        public SAgentBankAccountDeposit()
        {
            dbContext = new DB.FAXEREntities();
            try
            {
                _registeredAgentType = Common.Common.GetRegisteredAgentType(Common.AgentSession.AgentInformation.Id);   
            }
            catch (Exception)
            {

            }
            

        }
        public SAgentBankAccountDeposit(Module module)
        {
            dbContext = new DB.FAXEREntities();
        }
        public void SetAgentBankAccountDeposit(SenderBankAccountDepositVm vm)
        {

            Common.AgentSession.AgentBankAccountDeposit = vm;
        }



        public List<Models.DropDownViewModel> GetRecentAccountNumbers(int senderId = 0)
        {

            int AgentId = Common.AgentSession.LoggedUser.PayingAgentStaffId;
            var result = (from c in dbContext.BankAccountDeposit.Where(x => x.PayingStaffId == AgentId && x.PaidFromModule == Module.Agent && x.SenderId == senderId)
                          select new Models.DropDownViewModel()
                          {
                              Code = c.ReceiverAccountNo,
                              Name = c.ReceiverAccountNo + " ( " + c.ReceiverName + " ) ",
                              CountryCode = c.ReceiverCountry,


                          }).GroupBy(x => x.Code).Select(x => x.FirstOrDefault()).ToList();

            return result;
        }
        public SenderBankAccountDepositVm GetReceiverDetailsFromReceiptNumber(string ReceiptNumber)
        {

            var result = (from c in dbContext.BankAccountDeposit.Where(x => x.ReceiptNo == ReceiptNumber).ToList()
                          select new SenderBankAccountDepositVm()
                          {
                              CountryCode = c.ReceiverCountry,
                              MobileNumber = c.ReceiverMobileNo,
                              AccountNumber = c.ReceiverAccountNo,
                              AccountOwnerName = c.ReceiverName,
                              BankId = c.BankId,
                              BranchCode = c.BankCode,
                              CountryPhoneCode = Common.Common.GetCountryPhoneCode(Common.Common.GetCountryCodeByCountryName(c.ReceiverCountry)),
                              RecentAccountNumber = c.ReceiverAccountNo,

                          }).FirstOrDefault();
            return result;
        }
        public SenderBankAccountDepositVm GetAgentBankAccountDeposit()
        {

            SenderBankAccountDepositVm vm = new SenderBankAccountDepositVm();

            if (Common.AgentSession.AgentBankAccountDeposit != null)
            {

                vm = Common.AgentSession.AgentBankAccountDeposit;

            }
            return vm;
        }

        public void SetBankDepositAbroadEnterAmount(BankDepositAbroadEnterAmountVM vm)
        {
            SSenderKiiPayWalletTransfer _senderKiiPayServices = new SSenderKiiPayWalletTransfer();
            if (Common.AgentSession.IsTransferFromCalculateHowMuch == true)
            {
                var dashboarddata = _senderKiiPayServices.GetCommonEnterAmount();
                if (!string.IsNullOrEmpty(dashboarddata.SendingCountryCode))
                {
                    vm.SendingCurrencySymbol = dashboarddata.SendingCurrencySymbol;
                    vm.SendingCurrency = dashboarddata.SendingCurrency;
                    vm.ReceivingCurrencySymbol = dashboarddata.ReceivingCurrencySymbol;
                    vm.ReceivingCurrency = dashboarddata.ReceivingCurrency;
                    vm.ExchangeRate = dashboarddata.ExchangeRate;
                    vm.SendingAmount = dashboarddata.SendingAmount;
                    vm.ReceivingAmount = dashboarddata.ReceivingAmount;
                    vm.Fee = dashboarddata.Fee;
                    vm.TotalAmount = dashboarddata.TotalAmount;
                    vm.AgentCommission = Common.Common.GetAgentSendingCommission(DB.TransferService.BankDeposit, Common.AgentSession.LoggedUser.Id, dashboarddata.TotalAmount, dashboarddata.Fee);
                    Common.AgentSession.BankDepositAbroadEnterAmount = vm;
                }
            }
            else
            {
                Common.AgentSession.BankDepositAbroadEnterAmount = vm;
            }

        }

        public BankDepositAbroadEnterAmountVM GetBankDepositAbroadEnterAmount()
        {

            BankDepositAbroadEnterAmountVM vm = new BankDepositAbroadEnterAmountVM();

            if (Common.AgentSession.BankDepositAbroadEnterAmount != null)
            {

                vm = Common.AgentSession.BankDepositAbroadEnterAmount;

            }
            return vm;
        }
        public BankDepositAbroadEnterAmountVM GetReceiverInformationFromAccountNumnber(string accountNo)
        {
            if (accountNo != null)
            {
                var accountOwner = dbContext.SavedBank.Where(x => x.AccountNumber == accountNo).FirstOrDefault();
                if (accountOwner != null)
                {
                    return new BankDepositAbroadEnterAmountVM()
                    {
                        ReceiverName = accountOwner.OwnerName,
                        ImageUrl = "",
                        ReceiverId = accountOwner.Id,

                    };
                }
            }
            return new BankDepositAbroadEnterAmountVM()
            {
                ReceiverName = Common.AgentSession.AgentBankAccountDeposit.AccountOwnerName,
                ImageUrl = "",
                ReceiverId = 0

            };
        }


        public List<Common.DropDownViewModel> getBanksList(string Country)
        {
            var result = (from c in dbContext.Bank.Where(x => x.CountryCode == Country)
                          select new Common.DropDownViewModel()
                          {
                              Id = c.Id,
                              Name = c.Name
                          }).ToList();
            return result;
        }
        internal Bank GetBankCode(int bankId)
        {
            var code = dbContext.Bank.Where(x => x.Id == bankId).FirstOrDefault();
            return code;

        }



        public BankDepositResponseVm GetBankApiPaymentResponse(string refNo = "")
        {

            var transactionSummaryvm = GetBankDepositAbroadEnterAmount();
            var bankAccountDepositSummary = GetAgentBankAccountDeposit();
            BankDepositApi bankDepositApi = new BankDepositApi();

            //BankDepositLocalRequest bankDepositLocalRequest = new BankDepositLocalRequest()
            //{
            //    partnerTransactionReference = Common.Common.getOrderNo(),
            //    baseCurrencyCode = Common.Common.GetCurrencyCode(transactionSummaryvm.SenderAndReceiverDetail.SenderCountry),
            //    targetCurrencyCode = Common.Common.GetCurrencyCode(transactionSummaryvm.SenderAndReceiverDetail.ReceiverCountry),
            //    baseCurrencyAmount = transactionSummaryvm.KiiPayTransferPaymentSummary.SendingAmount,
            //    targetCurrencyAmount = transactionSummaryvm.KiiPayTransferPaymentSummary.ReceivingAmount,
            //    partnerCode = "3000",
            //    purpose = transactionSummaryvm.KiiPayTransferPaymentSummary.PaymentReference,
            //    accountNumber = transactionSummaryvm.BankAccountDeposit.AccountNumber,
            //    bankCode = transactionSummaryvm.BankAccountDeposit.BranchCode,
            //    baseCountryCode = transactionSummaryvm.SenderAndReceiverDetail.SenderCountry,
            //    targetCountryCode = transactionSummaryvm.SenderAndReceiverDetail.ReceiverCountry,
            //    payerName = "VGG",
            //    payermobile = transactionSummaryvm.BankAccountDeposit.MobileNumber
            //};
            //BankDepositLocalRequest bankDepositLocalRequest = new BankDepositLocalRequest()
            //{
            //    partnerTransactionReference = Common.Common.getOrderNo(),
            //    baseCurrencyCode = "GBP",
            //    targetCurrencyCode = "NGN",
            //    baseCurrencyAmount = transactionSummaryvm.KiiPayTransferPaymentSummary.SendingAmount,
            //    targetCurrencyAmount = transactionSummaryvm.KiiPayTransferPaymentSummary.ReceivingAmount,
            //    partnerCode = "3000",
            //    purpose = transactionSummaryvm.KiiPayTransferPaymentSummary.PaymentReference,
            //    accountNumber = transactionSummaryvm.BankAccountDeposit.AccountNumber,
            //    bankCode = transactionSummaryvm.BankAccountDeposit.BranchCode,
            //    baseCountryCode = "3",
            //    targetCountryCode = "3",
            //    payerName = "VGG",
            //    payermobile = transactionSummaryvm.BankAccountDeposit.MobileNumber
            //};

            BankDepositLocalRequest bankDepositLocalRequest = new BankDepositLocalRequest()
            {
                partnerTransactionReference = refNo,
                baseCurrencyCode = transactionSummaryvm.SendingCurrency,
                targetCurrencyCode = transactionSummaryvm.ReceivingCurrency,
                baseCurrencyAmount = transactionSummaryvm.SendingAmount,
                targetCurrencyAmount = transactionSummaryvm.ReceivingAmount,
                partnerCode = "3000",
                //partnerCode = null,

                purpose = "Payment to" + bankAccountDepositSummary.AccountOwnerName,
                accountNumber = bankAccountDepositSummary.AccountNumber,
                bankCode = bankAccountDepositSummary.BranchCode,
                baseCountryCode = Common.Common.GetCountryCodeByCurrency(transactionSummaryvm.SendingCurrency),
                targetCountryCode = Common.Common.GetCountryCodeByCurrency(transactionSummaryvm.ReceivingCurrency),
                payerName = Common.AgentSession.AgentInformation.Name,
                payermobile = /*"7440395950"*/ Common.AgentSession.AgentInformation.PhoneNumber

            };


            try
            {
                BankDepositApi api = new BankDepositApi();
                var accessToken = api.Login<AccessTokenVM>();
                //        api.Test(
                //bankDepositLocalRequest.bankCode, bankDepositLocalRequest.accountNumber, accessToken.Result);
                var validateAccountNo = api.ValidateAccountNo<AccountNoLookUpResponse>(
                    bankDepositLocalRequest.bankCode, bankDepositLocalRequest.accountNumber, accessToken.Result);
                var transaction = api.Post<BankDepositResponseResult>(CommonExtension.SerializeObject(bankDepositLocalRequest), accessToken.Result);

                var transactionResult = api.TransactionConfirmation<BankDepositResponseVm>(transaction.Result, Common.FaxerSession.BankAccessToken);

                return transactionResult.Result;
            }
            catch (Exception)
            {

                BankDepositResponseVm bankDepositResponseVm = new BankDepositResponseVm();
                bankDepositResponseVm.result = new BankDepositResponseResult();
                return bankDepositResponseVm;
            }


        }
        public string getAgentCountryCode(int agentId = 0)
        {
            SAgentInformation _sFaxerInfromationServices = new SAgentInformation();
            _sFaxerInfromationServices = new SAgentInformation();
            var result = _sFaxerInfromationServices.list().Data.Where(x => x.Id == agentId).Select(x => x.CountryCode).FirstOrDefault();
            return result;
        }

        public int TransctionCompleted()
        {
            SCashPickUpTransferService _cashPickUp = new SCashPickUpTransferService();
            SSenderBankAccountDeposit _senderBankAccountDepositServices = new SSenderBankAccountDeposit();
            int AgentId = Common.AgentSession.AgentInformation.Id;
            string agentCountry = getAgentCountryCode(AgentId);
            var sender = _cashPickUp.GetCashPickupInformationViewModel();
            var BankDeposit = GetAgentBankAccountDeposit();
            var paymentInfo = GetBankDepositAbroadEnterAmount();

            if (sender.Id == 0)
            {
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

                var DOB = new DateTime().AddDays(sender.Day).AddMonths((int)sender.Month).AddYears(sender.Year);

                DB.FaxerInformation FaxerDetails = new DB.FaxerInformation()
                {
                    FirstName = sender.FirstName,
                    MiddleName = sender.MiddleName,
                    LastName = sender.LastName,
                    Address1 = sender.AddressLine1,
                    City = sender.City,
                    Country = agentCountry,
                    Email = sender.Email,
                    PhoneNumber = sender.MobileNo,
                    IdCardNumber = sender.IdNumber,
                    IdCardType = sender.IdType.ToString(),
                    IssuingCountry = sender.IssuingCountry,
                    RegisteredByAgent = true,
                    IsDeleted = false,
                    IdCardExpiringDate = DateTime.Now,
                    AccountNo = accountNo,
                    DateOfBirth = DOB,
                    Address2 = sender.AddressLine2,
                    GGender = sender.Gender.ToInt()
                };
                var SenderAddedNew = _senderBankAccountDepositServices.AddSender(FaxerDetails);
                sender.Id = SenderAddedNew.Data.Id;
                SenderRegisteredByAgent senderRegisteredByAgent = new SenderRegisteredByAgent()
                {
                    AgentId = Common.AgentSession.LoggedUser.Id,
                    IsAuxAgent = Common.AgentSession.LoggedUser.IsAUXAgent,
                    SenderId = sender.Id,
                };
                dbContext.SenderRegisteredByAgent.Add(senderRegisteredByAgent);
                dbContext.SaveChanges();
            }

            int RecipientId = 0;
            var Recipent = dbContext.Recipients.Where(x => x.ReceiverName.ToLower() == BankDeposit.AccountOwnerName.ToLower() && x.MobileNo == BankDeposit.MobileNumber
            && x.Service == Service.BankAccount).FirstOrDefault();
            if (Recipent == null)
            {
                Recipients model = new Recipients()
                {
                    Country = BankDeposit.CountryCode,
                    SenderId = sender.Id,
                    MobileNo = BankDeposit.MobileNumber,
                    Service = Service.BankAccount,
                    ReceiverName = BankDeposit.AccountOwnerName,
                    Reason = BankDeposit.ReasonForTransfer,
                    PostalCode = BankDeposit.ReceiverPostalCode,
                    City = BankDeposit.ReceiverCity,
                    Email = BankDeposit.ReceiverEmail,
                    AccountNo = BankDeposit.AccountNumber,
                    Street = BankDeposit.ReceiverStreet,
                    BankId = BankDeposit.BankId,
                    BranchCode = BankDeposit.BranchCode,


                };
                var AddRecipient = dbContext.Recipients.Add(model);
                dbContext.SaveChanges();
                RecipientId = AddRecipient.Id;
            }
            else
            {
                RecipientId = Recipent.Id;

            }

            var ApiService = Common.Common.GetApiservice(sender.Country,
                                  BankDeposit.CountryCode, paymentInfo.SendingAmount, TransactionTransferMethod.BankDeposit, TransactionTransferType.Agent);

            decimal MFRate = Common.Common.GetMFRate(agentCountry, BankDeposit.CountryCode, TransactionTransferMethod.BankDeposit);

            BankAccountDeposit BankAccountDeposit = new BankAccountDeposit()
            {
                ExchangeRate = paymentInfo.ExchangeRate,
                Fee = paymentInfo.Fee,
                PaidFromModule = Module.Agent,
                TransactionDate = DateTime.Now,
                TotalAmount = paymentInfo.TotalAmount,
                ReceiverAccountNo = BankDeposit.AccountNumber,
                SenderPaymentMode = PORTAL.Models.SenderPaymentMode.Cash,
                ReceivingAmount = paymentInfo.ReceivingAmount,
                SenderId = sender.Id,
                ReceivingCountry = BankDeposit.CountryCode,
                SendingAmount = paymentInfo.SendingAmount,
                SendingCountry = agentCountry,
                ReceiverName = BankDeposit.AccountOwnerName,
                ReceiverCountry = BankDeposit.CountryCode,
                ReceiptNo = BankDeposit.IsManualDeposit == false ? Common.Common.GenerateBankAccountDepositReceiptNoforAgnet(6) : Common.Common.GenerateManualBankAccountDepositReceiptNo(6),
                //ReceiptNo = Common.Common.GenerateBankAccountDepositReceiptNoforAgnet(6),
                PayingStaffName = Common.AgentSession.LoggedUser.PayingAgentStaffName,
                PayingStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId,
                AgentCommission = paymentInfo.AgentCommission,
                BankId = BankDeposit.BankId,
                BankCode = BankDeposit.BranchCode,
                ReceiverMobileNo = BankDeposit.MobileNumber,
                IsManualDeposit = BankDeposit.IsManualDeposit,
                Status = BankDeposit.IsManualDeposit == true ? BankDepositStatus.Incomplete : BankDepositStatus.Confirm,
                Apiservice = ApiService,
                BankName = BankDeposit.BankName,
                IsEuropeTransfer = BankDeposit.IsEuropeTransfer,
                ReasonForTransfer = BankDeposit.ReasonForTransfer,
                Margin = Common.Common.GetMargin(MFRate, paymentInfo.ExchangeRate, paymentInfo.SendingAmount, paymentInfo.Fee),
                MFRate = MFRate,
                RecipientId = RecipientId,
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
            var obj = new BankAccountDeposit();
            obj = _senderBankAccountDepositServices.Add(BankAccountDeposit).Data;

            SSenderForAllTransfer senderForAllTransfer = new SSenderForAllTransfer();
            string ReceiverFirstname = BankAccountDeposit.ReceiverName.Split(' ')[0];

            SBankDepositResponseStatus sBankDepositResponseStatus = new SBankDepositResponseStatus();

            var bankdepositTransactionResult = new BankDepositResponseVm();

            TransactionSummaryVM transactionSummaryvm = new TransactionSummaryVM();

            transactionSummaryvm.SenderAndReceiverDetail = _senderBankAccountDepositServices.GetSenderAndReceiverDetails();
            transactionSummaryvm.BankAccountDeposit = _senderBankAccountDepositServices.GetMobileMoneyTransferDetails(BankAccountDeposit);
            transactionSummaryvm.KiiPayTransferPaymentSummary = _senderBankAccountDepositServices.GetKiiPayTransferPaymentSummary(BankAccountDeposit);


            // Create bank Api response log 

            senderForAllTransfer.SetTransactionSummary(transactionSummaryvm);

            if (BankAccountDeposit.IsManualDeposit == false)
            {

                BankDepositApi api = new BankDepositApi();
                BankDepositStatus bankDepositStatus = new BankDepositStatus();

                //switch (ApiService)
                //{
                //    case DB.Apiservice.VGG:
                //        bankdepositTransactionResult = GetBankApiPaymentResponse(BankAccountDeposit.ReceiptNo);
                //        var transcationStatus = bankdepositTransactionResult.result.transactionStatus;
                //        if (transcationStatus == 1)
                //        {
                //            BankAccountDeposit.Status = BankDepositStatus.Incomplete;
                //        }
                //        else if (transcationStatus == 2)
                //        {
                //            BankAccountDeposit.Status = BankDepositStatus.Confirm;
                //        }
                //        else if (transcationStatus == 3)
                //        {
                //            BankAccountDeposit.Status = BankDepositStatus.Failed;
                //        }
                //        else if (transcationStatus == 0)
                //        {
                //            BankAccountDeposit.Status = BankDepositStatus.Incomplete;
                //        }
                //        break;
                //    case DB.Apiservice.TransferZero:

                //        string TransactionId = BankAccountDeposit.ReceiptNo;
                //        var transferZeroResponse = senderForAllTransfer.GetBankDepositTransferZeroTransactionResponse(TransactionId);
                //        var status = Common.Common.GetTransferZeroTransactionStatus(transferZeroResponse);
                //        var transferZeroTransactionResult = transferZeroResponse;
                //        var responseModel = senderForAllTransfer.PrepareTransferZeroResponse(transferZeroTransactionResult);
                //        responseModel.result.beneficiaryAccountName = BankAccountDeposit.ReceiverAccountNo;
                //        responseModel.result.beneficiaryBankCode = BankAccountDeposit.BankCode;
                //        responseModel.result.beneficiaryAccountName = BankAccountDeposit.ReceiverName;
                //        responseModel.result.amountInBaseCurrency = BankAccountDeposit.SendingAmount;
                //        responseModel.result.targetAmount = BankAccountDeposit.ReceivingAmount;
                //        responseModel.result.partnerTransactionReference = BankAccountDeposit.ReceiptNo;
                //        bankdepositTransactionResult = responseModel;
                //        break;

                //    case DB.Apiservice.EmergentApi:
                //        SSenderForAllTransfer _sSenderForAllTransfer = new SSenderForAllTransfer();
                //        var EmergentApiResponse = _sSenderForAllTransfer.GetEmergentApiBankAccountDepositApiResponse(BankAccountDeposit);
                //        status = BankDepositStatus.Incomplete;
                //        switch (EmergentApiResponse.status_code)
                //        {
                //            case 0: //Failed
                //                status = BankDepositStatus.Failed;
                //                break;
                //            case 1: // Success
                //                status = BankDepositStatus.Confirm;
                //                break;
                //            case 2: // Pending

                //                status = BankDepositStatus.Incomplete;
                //                break;
                //            case 3: // Expired

                //                status = BankDepositStatus.Failed;
                //                break;
                //            case 4: // Reversed
                //                status = BankDepositStatus.Cancel;
                //                break;
                //            case 5: //In Clearing
                //                status = BankDepositStatus.Incomplete;
                //                break;
                //            default:
                //                break;
                //        }
                //        responseModel = _sSenderForAllTransfer.PrepareEmergentApiBankDepositResponse(EmergentApiResponse);
                //        responseModel.result.beneficiaryAccountName = BankAccountDeposit.ReceiverAccountNo;
                //        responseModel.result.beneficiaryBankCode = BankAccountDeposit.BankCode;
                //        responseModel.result.beneficiaryAccountName = BankAccountDeposit.ReceiverName;
                //        responseModel.result.amountInBaseCurrency = BankAccountDeposit.SendingAmount;
                //        responseModel.result.targetAmount = BankAccountDeposit.ReceivingAmount;
                //         responseModel.result.partnerTransactionReference = BankAccountDeposit.ReceiptNo;
                //        bankdepositTransactionResult = responseModel;

                //        BankAccountDeposit.Status = status;
                //        bankDepositStatus = status;
                //        break;

                //    default:
                //        break;
                //}
                var SenderDocumentApprovalStatus = Common.Common.IsSenderIdApproved(sender.Id, _registeredAgentType);

                if (SenderDocumentApprovalStatus)
                {
                    var transResponse = _senderBankAccountDepositServices.CreateBankTransactionToApi(BankAccountDeposit, TransactionTransferType.Agent);
                    BankAccountDeposit.Status = transResponse.BankAccountDeposit.Status;
                    bankDepositStatus = transResponse.BankAccountDeposit.Status;
                    bankdepositTransactionResult = transResponse.BankDepositApiResponseVm;
                    obj = _senderBankAccountDepositServices.Update(BankAccountDeposit).Data;
                    sBankDepositResponseStatus.AddLog(bankdepositTransactionResult, obj.Id);

                }
                else
                {
                    BankAccountDeposit.Status = BankDepositStatus.IdCheckInProgress;
                }
            }

            else
            {
                obj = _senderBankAccountDepositServices.Update(BankAccountDeposit).Data;
            }

            //if (BankAccountDeposit.Status == BankDepositStatus.Confirm)
            //{
            //    // Send Email And SMS 

            //    senderForAllTransfer.SendBankDepositSms(sender.FirstName, Common.Common.GetCurrencyCode(BankAccountDeposit.ReceivingCountry) + " " + BankAccountDeposit.ReceivingAmount
            //           , BankAccountDeposit.ReceiptNo, Common.Common.GetCountryPhoneCode(sender.Country) + sender.MobileNo, ReceiverFirstname, BankAccountDeposit.Status);

            //    senderForAllTransfer.SendMoneyTransferedEmail(sender.FirstName, BankAccountDeposit.Fee, BankAccountDeposit.SendingAmount, Common.Common.GetCurrencyCode(BankAccountDeposit.SendingCountry),
            //    BankAccountDeposit.ReceiverAccountNo, Common.Common.GetCurrencyCode(BankAccountDeposit.ReceivingCountry), BankAccountDeposit.ReceivingAmount, BankAccountDeposit.BankId,
            //    BankAccountDeposit.ReceiverName, BankAccountDeposit.SenderId, BankAccountDeposit.ReceiptNo, BankAccountDeposit.BankCode, BankAccountDeposit.ReceiverCountry, BankAccountDeposit.Status);
            //}
            //else {

            //    senderForAllTransfer.SendManualBankDepositFirstSmsToSender(ReceiverFirstname, Common.Common.GetCurrencyCode(BankAccountDeposit.ReceivingCountry) + " " + BankAccountDeposit.ReceivingAmount,
            //                          Common.Common.GetCountryName(BankAccountDeposit.ReceivingCountry),
            //                          BankAccountDeposit.ReceiptNo, Common.Common.GetCountryPhoneCode(BankAccountDeposit.SendingCountry) + sender.MobileNo);

            //    string bankName = Common.Common.getBankName(BankAccountDeposit.BankId);
            //    //DB.FAXEREntities dbContext = new FAXEREntities();
            //    string AgentPhoneNo = dbContext.ManualDepositEnable
            //                  .Where(x => x.PayingCountry == BankAccountDeposit.ReceivingCountry
            //                    && x.IsEnabled == true).Select(x => x.MobileNo).FirstOrDefault();
            //    if (BankAccountDeposit.IsManualDeposit == true)
            //    {
            //        senderForAllTransfer.SendManualBankDepositSmsToAgent(BankAccountDeposit.ReceiptNo, BankAccountDeposit.ReceiverName, bankName, BankAccountDeposit.ReceiverAccountNo,
            //                                        BankAccountDeposit.BankCode, BankAccountDeposit.SendingAmount + " " + Common.Common.GetCurrencyCode(BankAccountDeposit.ReceivingCountry),
            //                                        Common.Common.GetCountryPhoneCode(BankAccountDeposit.ReceivingCountry) + AgentPhoneNo);
            //    }
            //    senderForAllTransfer.SendManualDepositInProgressEmail(sender.FirstName, BankAccountDeposit.Fee, BankAccountDeposit.SendingAmount, Common.Common.GetCurrencyCode(BankAccountDeposit.SendingCountry),
            //    BankAccountDeposit.ReceiverAccountNo, Common.Common.GetCurrencyCode(BankAccountDeposit.ReceivingCountry), BankAccountDeposit.ReceivingAmount, BankAccountDeposit.BankId,
            //    BankAccountDeposit.ReceiverName, BankAccountDeposit.SenderId, BankAccountDeposit.ReceiptNo, BankAccountDeposit.BankCode, BankAccountDeposit.ReceiverCountry, ReceiverFirstname
            //     , BankAccountDeposit.PaymentReference, BankAccountDeposit.SenderPaymentMode);

            //}
            var agentInfo = AgentSession.AgentInformation;
            if (!agentInfo.IsAUXAgent)
            {
                _senderBankAccountDepositServices.SendEmailAndSms(obj);

            }
            else
            {
                var fundaccountBalance = dbContext.AgentAccountBalance.Where(x => x.AgentId == AgentSession.AgentInformation.Id).FirstOrDefault();
                fundaccountBalance.UpdateDateTime = DateTime.Now;
                fundaccountBalance.TotalBalance = fundaccountBalance.TotalBalance - obj.TotalAmount;
                dbContext.Entry(fundaccountBalance).State = EntityState.Modified;
                dbContext.SaveChanges();
            }

            return obj.Id;
        }

        public int GetSenderFromBankeposit(int id)
        {
            var senderId = dbContext.BankAccountDeposit.Where(x => x.Id == id).Select(x => x.SenderId).FirstOrDefault();
            return senderId;

        }

        public void SetTransactionSummary()
        {
            SCashPickUpTransferService _cashPickUp = new SCashPickUpTransferService();
            var senderInfo = _cashPickUp.GetCashPickupInformationViewModel();
            var receiverInfo = GetAgentBankAccountDeposit();
            var paymentInfo = GetBankDepositAbroadEnterAmount();
            AgentTransactionSummaryVm model = new AgentTransactionSummaryVm();
            model.SenderDetails = senderInfo;
            model.RecipientDetails = new RecipientsViewModel()
            {
                Country = receiverInfo.CountryCode,
                SenderId = receiverInfo.Id,
                MobileNo = receiverInfo.MobileNumber,
                Service = Service.BankAccount,
                ReceiverName = receiverInfo.AccountOwnerName,
                Reason = receiverInfo.ReasonForTransfer,
                ReceiverCity = receiverInfo.ReceiverCity,
                ReceiverEmail = receiverInfo.ReceiverEmail,
                AccountNo = receiverInfo.AccountNumber,
                IdentityCardNumber = receiverInfo.IdentityCardNumber,
                IdentityCardId = receiverInfo.IdenityCardId,
                BankId = receiverInfo.BankId,

            };
            model.PaymentSummary = new PaymentSummaryForAgentVm()
            {
                ExchangeRate = paymentInfo.ExchangeRate,
                Fee = paymentInfo.Fee,
                ReceivingCurrency = paymentInfo.ReceivingCurrency,
                ReceivingCurrencySymbol = paymentInfo.ReceivingCurrencySymbol,
                SendingAmount = paymentInfo.SendingAmount,
                SendingCurrency = paymentInfo.SendingCurrency,
                SendingCurrencySymbol = paymentInfo.SendingCurrencySymbol,
                TotalAmount = paymentInfo.TotalAmount,
                CommissionFee = paymentInfo.AgentCommission,
                ReceivingCountry = paymentInfo.ReceivingCountry,
                ReceivingAmount = paymentInfo.ReceivingAmount,
                SendingCountry = paymentInfo.SendingCountry,
                IsManualDeposit = receiverInfo.IsManualDeposit,
                IsEuropeTransfer = receiverInfo.IsEuropeTransfer,


            };
            TransferForAllAgentServices _transferForAllAgentServices = new TransferForAllAgentServices();
            _transferForAllAgentServices.SetTransactionSummary(model);
        }
    }
}