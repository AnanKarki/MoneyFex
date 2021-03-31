using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.Models;
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
using static FAXER.PORTAL.Services.SSenderForAllTransfer;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{


    public class SAgentMobileTransferWallet
    {
        private RegisteredAgentType _registeredAgentType;
        DB.FAXEREntities dbContext = null;

        public SAgentMobileTransferWallet()
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
        public SAgentMobileTransferWallet(Module module)
        {
            dbContext = new DB.FAXEREntities();
        }

        public ReceiverDetailsInformationViewModel GetReceiverDetailsFromReceiptNumber(string ReceiptNumber)
        {

            var result = (from c in dbContext.MobileMoneyTransfer.Where(x => x.ReceiptNo == ReceiptNumber).ToList()
                          select new ReceiverDetailsInformationViewModel()
                          {
                              Country = c.ReceivingCountry,
                              MobileNumber = c.PaidToMobileNo,
                              PreviousMobileNumber = c.PaidToMobileNo,
                              ReceiverName = c.ReceiverName,
                              MobileWalletProvider = c.WalletOperatorId,

                          }).FirstOrDefault();
            return result;
        }

        public void SetReceiverDetailsInformation(ReceiverDetailsInformationViewModel vm)
        {

            Common.AgentSession.ReceiverDetailsInformation = vm;
        }

        public ReceiverDetailsInformationViewModel GetReceiverDetailsInformation()
        {

            ReceiverDetailsInformationViewModel vm = new ReceiverDetailsInformationViewModel();

            if (Common.AgentSession.ReceiverDetailsInformation != null)
            {

                vm = Common.AgentSession.ReceiverDetailsInformation;

            }
            return vm;
        }
        public void SetMobileTransferAccessTokeneResponse(MobileTransferAccessTokeneResponse vm)
        {

            Common.AgentSession.MobileTransferAccessTokeneResponse = vm;
        }

        public MobileTransferAccessTokeneResponse GetMobileTransferAccessTokeneResponse()
        {

            MobileTransferAccessTokeneResponse vm = new MobileTransferAccessTokeneResponse();

            if (Common.AgentSession.MobileTransferAccessTokeneResponse != null)
            {

                vm = Common.AgentSession.MobileTransferAccessTokeneResponse;

            }
            return vm;
        }

        public void SetMTNCameroonResponseParamVm(MTNCameroonResponseParamVm vm)
        {

            Common.AgentSession.MTNCameroonResponseParamVm = vm;
        }

        public MTNCameroonResponseParamVm GetMTNCameroonResponseParamVm()
        {

            MTNCameroonResponseParamVm vm = new MTNCameroonResponseParamVm();

            if (Common.AgentSession.MTNCameroonResponseParamVm != null)
            {

                vm = Common.AgentSession.MTNCameroonResponseParamVm;

            }
            return vm;
        }

        public void SetMobileMoneyTransferEnterAmountViewModel(MobileMoneyTransferEnterAmountViewModel vm)
        {

            SSenderKiiPayWalletTransfer _senderKiiPayServices = new SSenderKiiPayWalletTransfer();
            if (Common.AgentSession.IsTransferFromCalculateHowMuch == true)
            {
                var dashboarddata = _senderKiiPayServices.GetCommonEnterAmount();
                if (!string.IsNullOrEmpty(dashboarddata.SendingCountryCode))
                {
                    vm.SendingCurrencySymbol = dashboarddata.SendingCurrencySymbol;
                    vm.SendingCurrencyCode = Common.Common.GetCountryCurrency(dashboarddata.SendingCountryCode);
                    vm.ReceivingCurrencySymbol = dashboarddata.ReceivingCurrencySymbol;
                    vm.ReceivingCurrencyCode = Common.Common.GetCountryCurrency(dashboarddata.ReceivingCountryCode);
                    vm.ExchangeRate = dashboarddata.ExchangeRate;
                    vm.SendingAmount = dashboarddata.SendingAmount;
                    vm.ReceivingAmount = dashboarddata.ReceivingAmount;
                    vm.Fee = dashboarddata.Fee;
                    vm.TotalAmount = dashboarddata.TotalAmount;
                    vm.AgentCommission = Common.Common.GetAgentSendingCommission(DB.TransferService.OtherWallet, Common.AgentSession.LoggedUser.Id, dashboarddata.TotalAmount, dashboarddata.Fee);
                    Common.AgentSession.MobileMoneyTransferEnterAmount = vm;
                }
            }
            else
            {
                Common.AgentSession.MobileMoneyTransferEnterAmount = vm;
            }
        }

        public MobileMoneyTransferEnterAmountViewModel GetMobileMoneyTransferEnterAmountViewModel()
        {

            MobileMoneyTransferEnterAmountViewModel vm = new MobileMoneyTransferEnterAmountViewModel();

            if (Common.AgentSession.MobileMoneyTransferEnterAmount != null)
            {

                vm = Common.AgentSession.MobileMoneyTransferEnterAmount;

            }
            return vm;
        }

        public int TransactionCompleted()
        {
            SCashPickUpTransferService _cashPickUp = new SCashPickUpTransferService();
            SSenderMobileMoneyTransfer _mobileMoneyTransferServices = new SSenderMobileMoneyTransfer();
            int AgentId = Common.AgentSession.LoggedUser.PayingAgentStaffId;
            string agentCountry = Common.AgentSession.AgentInformation.CountryCode;

            var sender = _cashPickUp.GetCashPickupInformationViewModel();
            var mobileTransfer = GetReceiverDetailsInformation();
            var paymentInfo = GetMobileMoneyTransferEnterAmountViewModel();

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
                DB.FaxerInformation FaxerDetails = new DB.FaxerInformation()
                {
                    FirstName = sender.FirstName,
                    MiddleName = sender.MiddleName,
                    LastName = sender.LastName,
                    Address1 = sender.AddressLine1,
                    City = sender.City,
                    Country = sender.Country,
                    Email = sender.Email,
                    PhoneNumber = sender.MobileNo,
                    IdCardNumber = sender.IdNumber,
                    IdCardType = sender.IdType.ToString(),
                    IssuingCountry = sender.IssuingCountry,
                    RegisteredByAgent = true,
                    IsDeleted = false,
                    IdCardExpiringDate = DateTime.Now,
                    AccountNo = accountNo,
                    DateOfBirth = sender.DOB,
                    Address2 = sender.AddressLine2,
                    GGender = sender.Gender.ToInt()
                };
                var SenderAddedNew = _mobileMoneyTransferServices.AddSender(FaxerDetails);
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
            else
            {
                var SenderExist = dbContext.FaxerInformation.Where(x => x.Id == sender.Id).FirstOrDefault();
                SenderExist.IdCardNumber = sender.IdNumber;
                SenderExist.IdCardType = sender.IdType.ToString();
                SenderExist.IdCardExpiringDate = sender.ExpiryDate;
                SenderExist.IssuingCountry = sender.IssuingCountry;
                dbContext.Entry(sender).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
            decimal MFRate = Common.Common.GetMFRate(agentCountry, mobileTransfer.Country, TransactionTransferMethod.OtherWallet);

            DB.MobileMoneyTransfer mobileTransferData = new DB.MobileMoneyTransfer()
            {
                ExchangeRate = paymentInfo.ExchangeRate,
                Fee = paymentInfo.Fee,
                PaidFromModule = Module.Agent,
                TransactionDate = DateTime.Now,
                TotalAmount = paymentInfo.TotalAmount,
                PaidToMobileNo = mobileTransfer.MobileNumber,
                PaymentReference = "",
                ReceivingAmount = paymentInfo.ReceivingAmount,
                SenderId = sender.Id,
                ReceivingCountry = mobileTransfer.Country,
                SendingAmount = paymentInfo.SendingAmount,
                SendingCountry = agentCountry,
                SenderPaymentMode = PORTAL.Models.SenderPaymentMode.Cash,
                ReceiverName = mobileTransfer.ReceiverName,
                ReceiptNo = Common.Common.GenerateMobileMoneyTransferReceiptNoForAgent(6),
                PayingStaffName = Common.AgentSession.LoggedUser.PayingAgentStaffName,
                PayingStaffId = AgentId,
                AgentCommission = paymentInfo.AgentCommission,
                WalletOperatorId = mobileTransfer.MobileWalletProvider,
                Margin = Common.Common.GetMargin(MFRate, paymentInfo.ExchangeRate, paymentInfo.SendingAmount, paymentInfo.Fee),
                MFRate = MFRate,
                SendingCurrency = paymentInfo.SendingCurrencyCode,
                ReceivingCurrency = paymentInfo.ReceivingCurrencyCode
            };
            if (mobileTransferData.SendingCountry == mobileTransferData.ReceivingCountry)
            {
                mobileTransferData.PaymentType = PaymentType.Local;
            }
            else
            {
                mobileTransferData.PaymentType = PaymentType.International;

            }

            var ApiService = Common.Common.GetApiservice(mobileTransferData.SendingCountry, mobileTransferData.ReceivingCountry,
                     paymentInfo.SendingAmount, TransactionTransferMethod.OtherWallet, TransactionTransferType.Agent);
            mobileTransferData.Apiservice = ApiService;
            var MobileMoneyTransfer = _mobileMoneyTransferServices.Add(mobileTransferData).Data;

            var SenderDocumentApprovalStatus = Common.Common.IsSenderIdApproved(sender.Id, _registeredAgentType);
            if (SenderDocumentApprovalStatus)
            {
                #region API Call
                var MobileMoneyTransactionResult = new MTNCameroonResponseParamVm();
                var transferApiResponse = _mobileMoneyTransferServices.CreateTransactionToApi(mobileTransferData, TransactionTransferType.Agent);
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
            var agentInfo = AgentSession.AgentInformation;
            if (!agentInfo.IsAUXAgent)
            {
                _mobileMoneyTransferServices.SendEmailAndSms(MobileMoneyTransfer);
            }
            else
            {
                var fundaccountBalance = dbContext.AgentAccountBalance.Where(x => x.AgentId == AgentSession.AgentInformation.Id).FirstOrDefault();
                fundaccountBalance.UpdateDateTime = DateTime.Now;
                fundaccountBalance.TotalBalance = fundaccountBalance.TotalBalance - MobileMoneyTransfer.TotalAmount;
                dbContext.Entry(fundaccountBalance).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
            return MobileMoneyTransfer.Id;
        }

        public ReceiverDetailsInformationViewModel GetReceiverDetailsByMobileNO(string mobileNo)
        {
            var receveiverDetails = dbContext.MobileMoneyTransfer.Where(x => x.PaidToMobileNo == mobileNo).FirstOrDefault();
            ReceiverDetailsInformationViewModel vm = new ReceiverDetailsInformationViewModel();
            if (receveiverDetails != null)
            {
                vm = new ReceiverDetailsInformationViewModel()
                {
                    Country = receveiverDetails.ReceivingCountry,
                    MobileCode = Common.Common.GetCountryPhoneCode(receveiverDetails.ReceivingCountry),
                    MobileNumber = receveiverDetails.PaidToMobileNo,
                    ReceiverName = receveiverDetails.ReceiverName,
                    MobileWalletProvider = receveiverDetails.WalletOperatorId,
                    Id = receveiverDetails.RecipientId,
                    ReasonForTransfer = receveiverDetails.ReasonForTransfer
                };
            }
            return vm;
        }

        internal int GetSenderFromMobileWallet(int transactionId)
        {
            int data = dbContext.MobileMoneyTransfer.Where(x => x.Id == transactionId).Select(x => x.SenderId).FirstOrDefault();
            return data;
        }

        #region ForStaff
        public PaymentMethodViewModel GetStaffPaymentMethod()
        {

            PaymentMethodViewModel vm = new PaymentMethodViewModel();

            if (Common.AdminSession.PaymentMethodViewModel != null)
            {

                vm = Common.AdminSession.PaymentMethodViewModel;
            }
            return vm;

        }

        public void SetStaffPaymentMethod(PaymentMethodViewModel vm)
        {


            Common.AdminSession.PaymentMethodViewModel = vm;
        }

        public void SetStaffReceiverDetailsInformation(ReceiverDetailsInformationViewModel vm)
        {

            Common.AdminSession.ReceiverDetailsInformation = vm;
        }

        public ReceiverDetailsInformationViewModel GetStaffReceiverDetailsInformation()
        {

            ReceiverDetailsInformationViewModel vm = new ReceiverDetailsInformationViewModel();

            if (Common.AdminSession.ReceiverDetailsInformation != null)
            {

                vm = Common.AdminSession.ReceiverDetailsInformation;

            }
            return vm;
        }

        public void SetStaffMobileMoneyTransferEnterAmountViewModel(MobileMoneyTransferEnterAmountViewModel vm)
        {

            Common.AdminSession.MobileMoneyTransferEnterAmount = vm;

        }

        public MobileMoneyTransferEnterAmountViewModel GetStaffMobileMoneyTransferEnterAmountViewModel()
        {

            MobileMoneyTransferEnterAmountViewModel vm = new MobileMoneyTransferEnterAmountViewModel();

            if (Common.AdminSession.MobileMoneyTransferEnterAmount != null)
            {

                vm = Common.AdminSession.MobileMoneyTransferEnterAmount;

            }
            return vm;
        }


        #endregion


        public void SetTransactionSummary()
        {
            SCashPickUpTransferService _cashPickUp = new SCashPickUpTransferService();
            var senderInfo = _cashPickUp.GetCashPickupInformationViewModel();
            var receiverInfo = GetReceiverDetailsInformation();
            var paymentInfo = GetMobileMoneyTransferEnterAmountViewModel();
            AgentTransactionSummaryVm model = new AgentTransactionSummaryVm();
            model.SenderDetails = senderInfo;
            model.RecipientDetails = new RecipientsViewModel()
            {
                Country = receiverInfo.Country,
                SenderId = receiverInfo.Id,
                MobileNo = receiverInfo.MobileNumber,
                Service = Service.CashPickUP,
                ReceiverName = receiverInfo.ReceiverName,
                Reason = receiverInfo.ReasonForTransfer,
                ReceiverEmail = receiverInfo.EmailAddress,
                AccountNo = receiverInfo.MobileNumber,
                MobileWalletProvider = receiverInfo.MobileWalletProvider
            };
            model.PaymentSummary = new PaymentSummaryForAgentVm()
            {
                ExchangeRate = paymentInfo.ExchangeRate,
                Fee = paymentInfo.Fee,
                ReceivingCurrency = paymentInfo.ReceivingCurrencyCode,
                ReceivingCurrencySymbol = paymentInfo.ReceivingCurrencySymbol,
                SendingAmount = paymentInfo.SendingAmount,
                SendingCurrency = paymentInfo.SendingCurrencyCode,
                SendingCurrencySymbol = paymentInfo.SendingCurrencySymbol,
                TotalAmount = paymentInfo.TotalAmount,
                CommissionFee = paymentInfo.AgentCommission,
                ReceivingCountry = paymentInfo.ReceivingCountry,
                ReceivingAmount = paymentInfo.ReceivingAmount,
                SendingCountry = paymentInfo.SendingCountry
            };
            TransferForAllAgentServices _transferForAllAgentServices = new TransferForAllAgentServices();
            _transferForAllAgentServices.SetTransactionSummary(model);
        }
    }
}