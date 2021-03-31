using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Areas.Staff.ViewModels;
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
using Twilio.TwiML.Voice;
using DropDownViewModel = FAXER.PORTAL.Common.DropDownViewModel;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class SCashPickUpTransferService
    {
        DB.FAXEREntities dbContext = null;
        private RegisteredAgentType _registeredAgentType;

        public SCashPickUpTransferService()
        {
            dbContext = new DB.FAXEREntities();
            try
            {

                _registeredAgentType = Common.Common.GetRegisteredAgentType(Common.AgentSession.AgentInformation.Id);

            }
            catch (Exception)
            {
                //_registeredAgentType = RegisteredAgentType.Agent;

            }
        }
        public SCashPickUpTransferService(Module module)
        {
            dbContext = new DB.FAXEREntities();
        }
        public List<DropDownViewModel> GetRecentAccountNumbers()
        {

            int AgentId = Common.AgentSession.LoggedUser.PayingAgentStaffId;
            var result = (from c in dbContext.BankAccountDeposit.Where(x => x.PayingStaffId == AgentId && x.PaidFromModule == Module.Agent)
                          select new DropDownViewModel()
                          {
                              Code = c.ReceiverAccountNo,
                              Name = c.ReceiverAccountNo + " ( " + c.ReceiverName + " ) ",
                              CountryCode = c.ReceiverCountry

                          }).GroupBy(x => x.Code).Select(x => x.FirstOrDefault()).ToList();

            return result;
        }
        public CashPickupInformationViewModel getFaxer(string AccountNoORPHoneNo, CashPickupInformationViewModel vm)
        {

            var data = (from c in dbContext.FaxerInformation.Where(x => x.Email == AccountNoORPHoneNo || x.PhoneNumber == AccountNoORPHoneNo).ToList()
                        select new Models.CashPickupInformationViewModel()
                        {
                            Id = c.Id,
                            FirstName = c.FirstName,
                            MiddleName = c.MiddleName,
                            LastName = c.LastName,
                            SenderFullName = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                            AddressLine1 = c.Address1,
                            AddressLine2 = c.Address2,
                            CountryCode = Common.Common.GetCountryName(c.Country),
                            PostCode = c.PostalCode,
                            Country = c.Country,
                            ExpiryDate = c.IdCardExpiringDate,
                            IdNumber = c.IdCardNumber,
                            IssuingCountry = c.IssuingCountry,
                            IdType = c.IdCardType == null ? 0 : Int32.Parse(c.IdCardType),
                            Email = c.Email,
                            MobileNo = c.PhoneNumber,
                            DOB = c.DateOfBirth,
                            SenderCountryCode = Common.Common.GetCountryPhoneCode(c.Country),
                            Searched = true,
                            Search = vm.Search,
                        }).FirstOrDefault();

            if (data != null)
            {
                var senderIdentificationInfo = dbContext.SenderBusinessDocumentation.Where(x => x.SenderId == data.Id).FirstOrDefault();
                if (senderIdentificationInfo != null)
                {

                    data.ExpiryDate = senderIdentificationInfo.ExpiryDate.GetValueOrDefault();
                    data.IdNumber = senderIdentificationInfo.IdentityNumber;
                    data.IssuingCountry = senderIdentificationInfo.Country;
                    data.IdType = senderIdentificationInfo.IdentificationTypeId;
                }
            }
            return data;
        }
        public CashPickupInformationViewModel getFaxer(string AccountNoORPHoneNo, int agentId, TransactionTransferType transferType = TransactionTransferType.All)
        {
            if (transferType == TransactionTransferType.AuxAgent)
            {
                var senderInfo = dbContext.FaxerInformation.Where(x => x.Email == AccountNoORPHoneNo || x.PhoneNumber == AccountNoORPHoneNo).FirstOrDefault();
                if (senderInfo != null)
                {
                    var senderRegisteredByAgent = dbContext.SenderRegisteredByAgent.Where(x => x.SenderId == senderInfo.Id && x.AgentId == agentId).FirstOrDefault();
                    if (senderRegisteredByAgent == null)
                    {
                        return null;
                    }
                }
            }
            var data = (from c in dbContext.FaxerInformation.Where(x => x.Email == AccountNoORPHoneNo || x.PhoneNumber == AccountNoORPHoneNo).ToList()
                        select new Models.CashPickupInformationViewModel()
                        {
                            Id = c.Id,
                            FirstName = c.FirstName,
                            MiddleName = c.MiddleName,
                            LastName = c.LastName,
                            SenderFullName = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                            AddressLine1 = c.Address1,
                            AddressLine2 = c.Address2,
                            CountryCode = Common.Common.GetCountryName(c.Country),
                            PostCode = c.PostalCode,
                            Country = c.Country,
                            ExpiryDate = c.IdCardExpiringDate,
                            IdNumber = c.IdCardNumber,
                            IssuingCountry = c.IssuingCountry,
                            IdType = c.IdCardType == null ? 0 : Int32.Parse(c.IdCardType),
                            Email = c.Email,
                            MobileNo = c.PhoneNumber,
                            DOB = c.DateOfBirth,
                            SenderCountryCode = Common.Common.GetCountryPhoneCode(c.Country),
                            Searched = true,
                            Search = AccountNoORPHoneNo,
                        }).FirstOrDefault();

            if (data != null)
            {
                var senderIdentificationInfo = dbContext.SenderBusinessDocumentation.Where(x => x.SenderId == data.Id).FirstOrDefault();
                if (senderIdentificationInfo != null)
                {

                    data.ExpiryDate = senderIdentificationInfo.ExpiryDate.GetValueOrDefault();
                    data.IdNumber = senderIdentificationInfo.IdentityNumber;
                    data.IssuingCountry = senderIdentificationInfo.Country;
                    data.IdType = senderIdentificationInfo.IdentificationTypeId;
                }
            }
            return data;
        }

        public CashPickUpReceiverDetailsInformationViewModel GetReceiverDetailsById(int receivierId)
        {
            var result = new CashPickUpReceiverDetailsInformationViewModel();
            var receiverDetails = getReceiverDetails(receivierId);
            if (receiverDetails != null)
            {
                result = new CashPickUpReceiverDetailsInformationViewModel()
                {
                    Country = receiverDetails.Country,
                    MobileCode = Common.Common.GetCountryPhoneCode(receiverDetails.Country),
                    MobileNo = receiverDetails.PhoneNumber,
                    City = receiverDetails.City,
                    Email = receiverDetails.EmailAddress,
                    PreviousReceiver = receiverDetails.FirstName,
                    ReceiverFullName = receiverDetails.FullName,
                    Id = receiverDetails.Id,

                };
            }
            return result;
        }
        public CashPickUpReceiverDetailsInformationViewModel GetReceiverDetailsFromReceiptNumber(string ReceiptNumber)
        {

            var result = (from c in dbContext.FaxingNonCardTransaction.Where(x => x.ReceiptNumber == ReceiptNumber).ToList()
                          select new CashPickUpReceiverDetailsInformationViewModel()
                          {
                              Country = c.ReceivingCountry,
                              MobileCode = Common.Common.GetCountryPhoneCode(c.ReceivingCountry),
                              MobileNo = c.NonCardReciever.PhoneNumber,
                              City = c.NonCardReciever.City,
                              Email = c.NonCardReciever.EmailAddress,
                              PreviousReceiver = c.NonCardReciever.FirstName,
                              ReceiverFullName = c.NonCardReciever.FirstName + " " + c.NonCardReciever.MiddleName + " " + c.NonCardReciever.LastName

                          }).FirstOrDefault();
            return result;
        }
        public List<ReceiversDetails> getExistingReceiver(int faxerid, string Country)
        {
            var data = dbContext.ReceiversDetails.Where(x => x.FaxerID == faxerid && x.Country == Country).ToList();
            return data;
        }
        public DB.ReceiversDetails getReceiverDetails(int receiverId)
        {
            var data = dbContext.ReceiversDetails.Where(x => x.Id == receiverId).FirstOrDefault();
            return data;

        }
        public EstimateFaxingFeeSummary getCalculateDetails(string FaxingCountry, string ReceivingCountry, Decimal FaxAmount, decimal receivingAmount)
        {


            var feeSummary = new EstimateFaxingFeeSummary();
            decimal exchangeRate = 0;
            var exchangeRateObj = dbContext.ExchangeRate.Where(x => x.CountryCode1 == FaxingCountry && x.CountryCode2 == ReceivingCountry).FirstOrDefault();
            if (exchangeRateObj == null)
            {
                var exchangeRateObj2 = dbContext.ExchangeRate.Where(x => x.CountryCode1 == ReceivingCountry && x.CountryCode2 == FaxingCountry).FirstOrDefault();
                if (exchangeRateObj2 != null)
                {
                    exchangeRateObj = exchangeRateObj2;
                    exchangeRateObj.CountryRate1 = Math.Round(1 / exchangeRateObj2.CountryRate1, 6, MidpointRounding.AwayFromZero);
                }

            }
            if (exchangeRateObj != null)
            {

                exchangeRate = exchangeRateObj.CountryRate1;
            }
            if (ReceivingCountry.ToLower() == FaxingCountry.ToLower())
            {

                exchangeRate = 1m;
            }
            if (exchangeRate == 0)
            {
                return null;

            }


            feeSummary = SEstimateFee.CalculateFaxingFee(((receivingAmount > 0) ? receivingAmount : FaxAmount), true, receivingAmount > 0, exchangeRate, SEstimateFee.GetFaxingCommision(FaxingCountry)); //+ 0.01m

            return feeSummary;

        }
        public DB.FaxingNonCardTransaction FaxNonCardTransactionByAgent(Models.CashPickUpEnterAmountViewModel vm)
        {
            // Faxer Details
            CashPickupInformationViewModel model = new CashPickupInformationViewModel();

            var SenderExist = GetCashPickupInformationViewModel();
            FAXER.PORTAL.Services.SFaxingNonCardTransaction getMFCN = new Services.SFaxingNonCardTransaction();

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
            string FaxerCountryPhoneCode = Common.Common.GetCountryPhoneCode(SenderExist.Country);
            if (SenderExist.Id == 0)
            {
                var DOB = new DateTime().AddDays(SenderExist.Day).AddMonths((int)SenderExist.Month).AddYears(SenderExist.Year);

                DB.FaxerInformation FaxerDetails = new DB.FaxerInformation()
                {
                    FirstName = SenderExist.FirstName,
                    MiddleName = SenderExist.MiddleName,
                    LastName = SenderExist.LastName,
                    Address1 = SenderExist.AddressLine1,
                    City = SenderExist.City,
                    Country = SenderExist.Country,
                    Email = SenderExist.Email,
                    PhoneNumber = FaxerCountryPhoneCode + SenderExist.MobileNo,
                    IdCardNumber = SenderExist.IdNumber,
                    IdCardType = SenderExist.IdType.ToString(),
                    IssuingCountry = SenderExist.IssuingCountry,
                    RegisteredByAgent = true,
                    IsDeleted = false,
                    IdCardExpiringDate = DateTime.Now,
                    AccountNo = accountNo,
                    DateOfBirth = DOB,
                    Address2 = SenderExist.AddressLine2,
                    GGender = SenderExist.Gender.ToInt(),

                };

                dbContext.FaxerInformation.Add(FaxerDetails);
                dbContext.SaveChanges();

                SenderRegisteredByAgent senderRegisteredByAgent = new SenderRegisteredByAgent()
                {
                    AgentId = Common.AgentSession.LoggedUser.Id,
                    IsAuxAgent = Common.AgentSession.LoggedUser.IsAUXAgent,
                    SenderId = FaxerDetails.Id,
                };
                dbContext.SenderRegisteredByAgent.Add(senderRegisteredByAgent);
                dbContext.SaveChanges();
            }
            else
            {
                var sender = dbContext.FaxerInformation.Where(x => x.Id == SenderExist.Id).FirstOrDefault();

                sender.IdCardNumber = SenderExist.IdNumber;
                sender.IdCardType = SenderExist.IdType.ToString();
                sender.IdCardExpiringDate = SenderExist.ExpiryDate;
                sender.IssuingCountry = SenderExist.IssuingCountry;

                dbContext.Entry(sender).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
            var SenderInfo = dbContext.FaxerInformation.Where(x => x.Email == SenderExist.Email).FirstOrDefault();


            var receiver = GetCashPickUpReceiverInfoViewModel();
            if (SenderInfo != null)
            {
                int faxerid = SenderInfo.Id;
                string ReceiverCountryPhoneCode = Common.Common.GetCountryPhoneCode(receiver.Country);

                // Receiver Details


                string[] splittedName = receiver.ReceiverFullName.Trim().Split(null);
                DB.ReceiversDetails receiversDetails = new DB.ReceiversDetails()
                {
                    FirstName = splittedName[0],
                    MiddleName = splittedName.Count() > 2 ? splittedName[1] : " ",
                    LastName = splittedName[splittedName.Count() - 1],
                    City = receiver.City,
                    Country = receiver.Country,
                    CreatedDate = DateTime.Now,
                    EmailAddress = receiver.Email,
                    PhoneNumber = receiver.MobileNo,
                    FaxerID = faxerid,
                    FullName = receiver.ReceiverFullName
                };
                var nonCardReceiverExist = dbContext.ReceiversDetails.Where(x => x.PhoneNumber == receiver.MobileNo).FirstOrDefault();
                if (nonCardReceiverExist == null)
                {
                    dbContext.ReceiversDetails.Add(receiversDetails);
                    dbContext.SaveChanges();
                }

            }
            int RecipientId = 0;
            var Recipent = dbContext.Recipients.Where(x => x.ReceiverName.ToLower() == receiver.ReceiverFullName.ToLower() && x.MobileNo == receiver.MobileNo
            && x.Service == Service.CashPickUP).FirstOrDefault();
            if (Recipent == null)
            {
                Recipients recipients = new Recipients()
                {
                    Country = receiver.Country,
                    SenderId = SenderInfo.Id,
                    MobileNo = receiver.MobileNo,
                    Service = Service.CashPickUP,
                    ReceiverName = receiver.ReceiverFullName,
                    Reason = receiver.ReasonForTransfer,
                    City = receiver.City,
                    Email = receiver.Email,
                    AccountNo = receiver.MobileNo,
                    IdentificationNumber = receiver.IdentityCardNumber,
                    IdentificationTypeId = receiver.IdenityCardId
                };
                var AddRecipient = dbContext.Recipients.Add(recipients);
                dbContext.SaveChanges();
                RecipientId = AddRecipient.Id;
            }
            else
            {
                RecipientId = Recipent.Id;

            }



            var nonCardReceiver = dbContext.ReceiversDetails.Where(x => x.PhoneNumber == receiver.MobileNo).FirstOrDefault();
            if (nonCardReceiver != null)
            {

                // Faxing NOn card Transaction
                var MFCN = getMFCN.GetNewMFCNToSave();
                var receiptNumber = GetNewAgentMoneyTransferReceipt();

                decimal MFRate = Common.Common.GetMFRate(SenderInfo.Country, nonCardReceiver.Country, TransactionTransferMethod.CashPickUp);
                var ApiService = Common.Common.GetApiservice(Common.AgentSession.AgentInformation.CountryCode,
            receiver.Country, vm.SendingAmount, TransactionTransferMethod.CashPickUp, TransactionTransferType.Agent);
                DB.FaxingNonCardTransaction nonCardTransaction = new DB.FaxingNonCardTransaction()
                {
                    FaxingAmount = vm.SendingAmount,
                    FaxingFee = vm.Fee,
                    ExchangeRate = vm.ExchangeRate,
                    TransactionDate = DateTime.Now,
                    ReceivingAmount = vm.ReceivingAmount,
                    NonCardRecieverId = nonCardReceiver.Id,
                    MFCN = MFCN,
                    ReceiptNumber = MFCN,
                    OperatingUserType = OperatingUserType.Agent,
                    PayingStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId,
                    AgentStaffName = Common.AgentSession.LoggedUser.PayingAgentStaffName,
                    AgentCommission = vm.AgentCommission,
                    ReceivingCountry = nonCardReceiver.Country,
                    TotalAmount = vm.TotalAmount,
                    SendingCountry = Common.AgentSession.AgentInformation.CountryCode,
                    PaymentMethod = Enum.GetName(typeof(SenderPaymentMode), SenderPaymentMode.Cash),
                    SenderId = SenderInfo.Id,
                    Margin = Common.Common.GetMargin(MFRate, vm.ExchangeRate, vm.SendingAmount, vm.Fee),
                    MFRate = MFRate,
                    RecipientId = RecipientId,
                    RecipientIdenityCardNumber = receiver.IdentityCardNumber,
                    RecipientIdentityCardId = receiver.IdenityCardId,
                    Apiservice = ApiService,
                    ReceivingCurrency = vm.ReceivingCurrency,
                    SendingCurrency = vm.SendingCurrency
                };

                var IsPayoutFlowControlEnabled = Common.Common.IsPayoutFlowControlEnabled(nonCardTransaction.SendingCountry, nonCardTransaction.ReceivingCountry,
                    null, TransactionTransferMethod.CashPickUp, 0);
                if (IsPayoutFlowControlEnabled == false)
                {
                    nonCardTransaction.FaxingStatus = FaxingStatus.Paused;

                }

                var obj = dbContext.FaxingNonCardTransaction.Add(nonCardTransaction);
                dbContext.SaveChanges();
                // End

                #region API Call
                SSenderCashPickUp _cashPickUpServices = new SSenderCashPickUp();

                var SenderDocumentApprovalStatus = Common.Common.IsSenderIdApproved(SenderInfo.Id, _registeredAgentType);

                if (SenderDocumentApprovalStatus)
                {
                    BankDepositResponseVm cashPickUpTransactionResult = new BankDepositResponseVm();

                    var transResponse = _cashPickUpServices.CreateCashPickTransactionToApi(obj, TransactionTransferType.Agent, Module.Agent);
                    obj.FaxingStatus = transResponse.CashPickUp.FaxingStatus;
                    obj.TransferReference = transResponse.CashPickUp.TransferReference;
                    cashPickUpTransactionResult = transResponse.BankDepositApiResponseVm;
                    _cashPickUpServices.AddResponseLog(cashPickUpTransactionResult, obj.Id);
                }
                else
                {
                    nonCardTransaction.FaxingStatus = FaxingStatus.IdCheckInProgress;
                }

                dbContext.Entry<FaxingNonCardTransaction>(obj).State = EntityState.Modified;
                dbContext.SaveChanges();

                var agentInfo = AgentSession.AgentInformation;
                if (!agentInfo.IsAUXAgent)
                {
                    _cashPickUpServices.SendEmailAndSms(obj);
                }
                else
                {
                    var fundaccountBalance = dbContext.AgentAccountBalance.Where(x => x.AgentId == AgentSession.AgentInformation.Id).FirstOrDefault();
                    fundaccountBalance.UpdateDateTime = DateTime.Now;
                    fundaccountBalance.TotalBalance = fundaccountBalance.TotalBalance - obj.TotalAmount;
                    dbContext.Entry(fundaccountBalance).State = EntityState.Modified;
                    dbContext.SaveChanges();
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

                return nonCardTransaction;

            }
            else
            {
                return null;
            }
        }
        public FaxingNonCardTransaction FaxingNonCardInfo(int TransactionId)
        {
            var TranscationInfo = dbContext.FaxingNonCardTransaction.Where(x => x.Id == TransactionId).FirstOrDefault();

            return TranscationInfo;

        }
        internal string GetNewAgentMoneyTransferReceipt()
        {
            //this code should be unique and random with 8 digit length
            var val = "CD" + Common.Common.GenerateRandomDigit(6);

            while (dbContext.FaxingNonCardTransaction.Where(x => x.ReceiptNumber == val).Count() > 0)
            {
                val = GetNewAgentMoneyTransferReceipt();
            }
            return val;
        }
        public void SetCashPickUpEnterAmount(CashPickUpEnterAmountViewModel vm)
        {
            SSenderKiiPayWalletTransfer _senderKiiPayServices = new SSenderKiiPayWalletTransfer();
            if (Common.AgentSession.IsTransferFromCalculateHowMuch == true)
            {
                var dashboarddata = _senderKiiPayServices.GetCommonEnterAmount();
                if (!string.IsNullOrEmpty(dashboarddata.SendingCountryCode))
                {
                    vm.SendingCurrencySymbol = dashboarddata.SendingCurrencySymbol;
                    vm.SendingCurrency = Common.Common.GetCountryCurrency(dashboarddata.SendingCountryCode);
                    vm.ReceivingCurrencySymbol = dashboarddata.ReceivingCurrencySymbol;
                    vm.ReceivingCurrency = Common.Common.GetCountryCurrency(dashboarddata.ReceivingCountryCode);
                    vm.ExchangeRate = dashboarddata.ExchangeRate;
                    vm.SendingAmount = dashboarddata.SendingAmount;
                    vm.ReceivingAmount = dashboarddata.ReceivingAmount;
                    vm.Fee = dashboarddata.Fee;
                    vm.TotalAmount = dashboarddata.TotalAmount;
                    vm.AgentCommission = Common.Common.GetAgentSendingCommission(TransferService.CahPickUp
                                              , Common.AgentSession.LoggedUser.Id, dashboarddata.TotalAmount, dashboarddata.Fee);
                    Common.AgentSession.CashPickUpEnterAmount = vm;

                }
            }
            else
            {
                Common.AgentSession.CashPickUpEnterAmount = vm;
            }
        }
        public CashPickUpEnterAmountViewModel GetCashPickUpEnterAmount()
        {

            CashPickUpEnterAmountViewModel vm = new CashPickUpEnterAmountViewModel();

            if (Common.AgentSession.CashPickUpEnterAmount != null)
            {


                vm = Common.AgentSession.CashPickUpEnterAmount;

            }
            return vm;
        }
        public void SetCashPickupInformationViewModel(CashPickupInformationViewModel vm)
        {

            Common.AgentSession.CashPickupInformationViewModel = vm;
        }

        public CashPickupInformationViewModel GetCashPickupInformationViewModel()
        {

            CashPickupInformationViewModel vm = new CashPickupInformationViewModel();

            if (Common.AgentSession.CashPickupInformationViewModel != null)
            {


                vm = Common.AgentSession.CashPickupInformationViewModel;

            }
            return vm;
        }
        public void SetCashPickUpReceiverInfoViewModel(CashPickUpReceiverDetailsInformationViewModel vm)
        {


            Common.AgentSession.CashPickUpReceiverDetailsInformationViewModel = vm;
        }
        public CashPickUpReceiverDetailsInformationViewModel GetCashPickUpReceiverInfoViewModel()
        {

            CashPickUpReceiverDetailsInformationViewModel vm = new CashPickUpReceiverDetailsInformationViewModel();

            if (Common.AgentSession.CashPickUpReceiverDetailsInformationViewModel != null)
            {


                vm = Common.AgentSession.CashPickUpReceiverDetailsInformationViewModel;

            }
            return vm;
        }

        public void SetTransactionSummary()
        {
            var senderInfo = GetCashPickupInformationViewModel();
            var receiverInfo = GetCashPickUpReceiverInfoViewModel();
            var paymentInfo = GetCashPickUpEnterAmount();
            AgentTransactionSummaryVm model = new AgentTransactionSummaryVm();
            model.SenderDetails = senderInfo;
            model.RecipientDetails = new RecipientsViewModel()
            {
                Country = receiverInfo.Country,
                SenderId = receiverInfo.Id,
                MobileNo = receiverInfo.MobileNo,
                Service = Service.CashPickUP,
                ReceiverName = receiverInfo.ReceiverFullName,
                Reason = receiverInfo.ReasonForTransfer,
                ReceiverCity = receiverInfo.City,
                ReceiverEmail = receiverInfo.Email,
                AccountNo = receiverInfo.MobileNo,
                IdentityCardNumber = receiverInfo.IdentityCardNumber,
                IdentityCardId = receiverInfo.IdenityCardId
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
                SendingCountry = paymentInfo.SendingCountry
            };
            TransferForAllAgentServices _transferForAllAgentServices = new TransferForAllAgentServices();
            _transferForAllAgentServices.SetTransactionSummary(model);
        }

        #region For Admin
        public void SetStaffCashPickUpReceiverInfoViewModel(CashPickUpReceiverDetailsInformationViewModel vm)
        {


            Common.AdminSession.CashPickUpReceiverDetailsInformationViewModel = vm;
        }

        public CashPickUpReceiverDetailsInformationViewModel GetStaffCashPickUpReceiverInfoViewModel()
        {

            CashPickUpReceiverDetailsInformationViewModel vm = new CashPickUpReceiverDetailsInformationViewModel();

            if (Common.AdminSession.CashPickUpReceiverDetailsInformationViewModel != null)
            {


                vm = Common.AdminSession.CashPickUpReceiverDetailsInformationViewModel;

            }
            return vm;
        }
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
        public void SetStaffCashPickupInformationViewModel(CashPickupInformationViewModel vm)
        {

            Common.AdminSession.CashPickupInformationViewModel = vm;
        }

        public CashPickupInformationViewModel GetStaffCashPickupInformationViewModel()
        {

            CashPickupInformationViewModel vm = new CashPickupInformationViewModel();

            if (Common.AdminSession.CashPickupInformationViewModel != null)
            {


                vm = Common.AdminSession.CashPickupInformationViewModel;

            }
            return vm;
        }

        public void SetStaffCashPickUpEnterAmount(CashPickUpEnterAmountViewModel vm)
        {

            Common.AdminSession.CashPickUpEnterAmount = vm;

        }

        public CashPickUpEnterAmountViewModel GetStaffCashPickUpEnterAmount()
        {

            CashPickUpEnterAmountViewModel vm = new CashPickUpEnterAmountViewModel();

            if (Common.AdminSession.CashPickUpEnterAmount != null)
            {


                vm = Common.AdminSession.CashPickUpEnterAmount;

            }
            return vm;
        }

        public LoggedStaff GetLoggedUserData()
        {

            LoggedStaff vm = new LoggedStaff();

            if (Common.StaffSession.LoggedStaff != null)
            {

                vm = Common.StaffSession.LoggedStaff;
            }
            return vm;
        }
        #endregion

    }
}