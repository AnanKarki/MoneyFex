using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.BankApi;
using FAXER.PORTAL.BankApi.Models;
using FAXER.PORTAL.CashPickUpApi;
using FAXER.PORTAL.CashPickUpApi.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Models.PaymentSummary;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Web;
using TransferZero.Sdk.Model;

namespace FAXER.PORTAL.Services
{
    public class SSenderCashPickUp
    {

        DB.FAXEREntities dbContext = null;
        SSenderKiiPayWalletTransfer _senderKiiPayServices = null;
        public SSenderCashPickUp()
        {
            dbContext = new DB.FAXEREntities();
            _senderKiiPayServices = new SSenderKiiPayWalletTransfer();
        }

        public SSenderCashPickUp(DB.FAXEREntities db)
        {
            dbContext = db;
            _senderKiiPayServices = new SSenderKiiPayWalletTransfer();
        }


        public bool Update(DB.FaxingNonCardTransaction model)
        {

            dbContext.Entry<FaxingNonCardTransaction>(model).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;
        }
        public ServiceResult<IQueryable<FaxingNonCardTransaction>> List()
        {

            return new ServiceResult<IQueryable<FaxingNonCardTransaction>>()
            {

                Data = dbContext.FaxingNonCardTransaction,
                Status = ResultStatus.OK
            };
        }
        public List<SenderRecentReceiverDropDownVM> GetRecentReceivers(string CountryCode = "")
        {
            int LoggedUserId = FaxerSession.LoggedUser.Id;
            List<ReceiversDetails> receiverDetailsList = dbContext.ReceiversDetails.Where(x => x.FaxerID == LoggedUserId).ToList();

            if (!string.IsNullOrEmpty(CountryCode))
            {
                receiverDetailsList = receiverDetailsList.Where(x => x.Country == CountryCode).ToList();
            }

            //var RecipentList = dbContext.Recipients.Where(x => x.SenderId == LoggedUserId && x.Service == Service.CashPickUP).ToList();
            var result = (from c in receiverDetailsList
                          select new SenderRecentReceiverDropDownVM()
                          {
                              Id = c.Id,
                              ReceiverName = c.FullName,
                              ReceiverMobileNo = c.PhoneNumber
                          }).GroupBy(x => x.ReceiverMobileNo).Select(x => x.FirstOrDefault()).ToList();
            return result;

        }



        public SenderCashPickUpVM GetReceiverInformationFromReceiverId(int receiverId)
        {


            var data = (from c in dbContext.ReceiversDetails.Where(x => x.Id == receiverId).ToList()
                        select new SenderCashPickUpVM()
                        {
                            FullName = c.FullName,
                            MobileNumber = c.PhoneNumber,
                            EmailAddress = c.EmailAddress,
                            CountryCode = c.Country,
                            RecentReceiverId = c.Id
                        }).FirstOrDefault();


            return data;
        }

        public SenderCashPickUpVM GetReceiverInformationFromMFCN(string MfCN, int id)
        {
            var data = (from c in dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MfCN && x.Id == id).ToList()
                        select new SenderCashPickUpVM()
                        {
                            FullName = c.NonCardReciever.FullName,
                            MobileNumber = c.NonCardReciever.PhoneNumber,
                            EmailAddress = c.NonCardReciever.EmailAddress,
                            CountryCode = c.NonCardReciever.Country,
                            RecentReceiverId = c.NonCardRecieverId,
                        }).FirstOrDefault();



            return data;
        }
        public List<DropDownViewModel> GetReasons()
        {

            var result = new List<DropDownViewModel>();
            var res1 = new DropDownViewModel()
            {
                Id = 1,
                Name = "res"
            };
            var res2 = new DropDownViewModel()
            {
                Id = 2,
                Name = "resr"
            };
            result.Add(res1);
            result.Add(res2);

            return result;

        }



        public void SetSenderCashPickUp(SenderCashPickUpVM vm)
        {

            Common.FaxerSession.SenderCashPickUp = vm;

        }

        public SenderCashPickUpVM GetSenderCashPickUp()
        {

            SenderCashPickUpVM vm = new SenderCashPickUpVM();

            if (Common.FaxerSession.SenderCashPickUp != null)
            {

                vm = Common.FaxerSession.SenderCashPickUp;
            }
            return vm;
        }


        public LoggedUser GetLoggedUserData()
        {

            LoggedUser vm = new LoggedUser();

            if (Common.FaxerSession.LoggedUser != null)
            {

                vm = Common.FaxerSession.LoggedUser;
            }
            return vm;
        }

        internal int GetRecipientId(string mobileNumber, Service service)
        {
            return dbContext.Recipients.Where(x => x.MobileNo == mobileNumber && x.Service == service).Select(x => x.Id).FirstOrDefault();
        }

        public void SetSenderMobileEnrterAmount(SenderMobileEnrterAmountVm vm)
        {


            var param = _senderKiiPayServices.GetCommonEnterAmount();
            PaymentSummaryRequestParamVm requestParamVm = new PaymentSummaryRequestParamVm()
            {
                SendingCountry = param.SendingCountryCode,
                ReceivingCountry = param.ReceivingCountryCode,
                SendingAmount = param.SendingAmount,
                SendingCurrency = param.SendingCurrency,
                ReceivingCurrency = param.ReceivingCurrency,
                TransferMethod = (int)param.TransactionTransferMethod,
                TransferType = (int)TransactionTransferType.Online,

            };
            _senderKiiPayServices.SetTransferSummaryAgain(requestParamVm);

            var dashboarddata = _senderKiiPayServices.GetCommonEnterAmount();

            vm.SendingCurrencySymbol = dashboarddata.SendingCurrencySymbol;
            vm.SendingCurrencyCode = dashboarddata.SendingCurrency;//  Common.Common.GetCountryCurrency(dashboarddata.SendingCountryCode);
            vm.ReceivingCurrencySymbol = dashboarddata.ReceivingCurrencySymbol;
            vm.ReceivingCurrencyCode = dashboarddata.ReceivingCurrency; //Common.Common.GetCountryCurrency(dashboarddata.ReceivingCountryCode);
            vm.ExchangeRate = dashboarddata.ExchangeRate;
            vm.SendingAmount = dashboarddata.SendingAmount;
            vm.ReceivingAmount = dashboarddata.ReceivingAmount;
            vm.Fee = dashboarddata.Fee;
            vm.TotalAmount = dashboarddata.TotalAmount;

            if (Common.FaxerSession.IsTransferFromHomePage == true || Common.FaxerSession.IsCommonEstimationPage == true)
            {

                if (!string.IsNullOrEmpty(dashboarddata.SendingCountryCode))
                {
                    vm.SendingCurrencySymbol = dashboarddata.SendingCurrencySymbol;
                    vm.SendingCurrencyCode = dashboarddata.SendingCurrency;
                    vm.ReceivingCurrencySymbol = dashboarddata.ReceivingCurrencySymbol;
                    vm.ReceivingCurrencyCode = dashboarddata.ReceivingCurrency;
                    vm.ExchangeRate = dashboarddata.ExchangeRate;
                    vm.SendingAmount = dashboarddata.SendingAmount;
                    vm.ReceivingAmount = dashboarddata.ReceivingAmount;
                    vm.Fee = dashboarddata.Fee;
                    vm.TotalAmount = dashboarddata.TotalAmount;



                    Common.FaxerSession.SenderMobileEnrterAmount = vm;
                }
            }
            else
            {
                Common.FaxerSession.SenderMobileEnrterAmount = vm;
            }

        }

        public SenderMobileEnrterAmountVm GetRepeatedTransactionInfo(string MFCN, int id)
        {
            var data = (from c in dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCN && x.Id == id).ToList()
                        select new SenderMobileEnrterAmountVm()
                        {
                            ExchangeRate = c.ExchangeRate,
                            Fee = c.FaxingFee,
                            ReceivingAmount = c.ReceivingAmount,
                            SendingAmount = c.FaxingFee,
                            ReceivingCountryCode = c.ReceivingCountry,
                            SendingCurrencyCode = Common.Common.GetCurrencyCode(c.SendingCountry),
                            ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(c.ReceivingCountry),
                            ReceivingCurrencyCode = Common.Common.GetCurrencyCode(c.ReceivingCountry),
                            SendingCountryCode = c.SendingCountry,
                            SendingCurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                            TotalAmount = c.TotalAmount,

                        }).FirstOrDefault();
            return data;

        }

        public SenderMobileEnrterAmountVm GetSenderMobileEnrterAmount()
        {

            SenderMobileEnrterAmountVm vm = new SenderMobileEnrterAmountVm();

            if (Common.FaxerSession.SenderMobileEnrterAmount != null)
            {

                vm = Common.FaxerSession.SenderMobileEnrterAmount;
            }
            return vm;
        }
        public void SetPaymentMethod(PaymentMethodViewModel vm)
        {


            Common.FaxerSession.PaymentMethodViewModel = vm;
        }

        public PaymentMethodViewModel GetPaymentMethod()
        {

            PaymentMethodViewModel vm = new PaymentMethodViewModel();

            if (Common.FaxerSession.PaymentMethodViewModel != null)
            {

                vm = Common.FaxerSession.PaymentMethodViewModel;
            }
            return vm;

        }


        public List<DropDownViewModel> GetAddress()
        {
            List<DropDownViewModel> list = new List<DropDownViewModel>();

            DropDownViewModel vm1 = new DropDownViewModel()
            {
                Id = 1,
                Name = "Ktm"
            };

            DropDownViewModel vm2 = new DropDownViewModel()
            {
                Id = 1,
                Name = "Bkt"
            };

            list.Add(vm1);
            list.Add(vm2);
            return list;
        }

        internal void setReciverInfo(int recipientId)
        {
            SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();

            var paymentInfo = _kiiPaytrasferServices.GetCommonEnterAmount();
            var data = dbContext.Recipients.Where(x => x.Id == recipientId).FirstOrDefault();
            SenderMobileEnrterAmountVm vm = new SenderMobileEnrterAmountVm()
            {
                ExchangeRate = paymentInfo.ExchangeRate,
                Fee = paymentInfo.Fee,
                ReceiverName = data.ReceiverName,
                ReceivingAmount = paymentInfo.ReceivingAmount,
                ReceivingCountryCode = paymentInfo.ReceivingCountryCode,
                ReceivingCurrencyCode = Common.Common.GetCurrencyCode(paymentInfo.ReceivingCountryCode),
                ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(paymentInfo.ReceivingCountryCode),
                SendingCountryCode = paymentInfo.SendingCountryCode,
                SendingAmount = paymentInfo.SendingAmount,
                SendingCurrencySymbol = Common.Common.GetCurrencySymbol(paymentInfo.SendingCountryCode),
                SendingCurrencyCode = Common.Common.GetCurrencyCode(paymentInfo.SendingCountryCode),
                TotalAmount = paymentInfo.TotalAmount,


            };


            var recipientIdInfo = GetSenderCashPickUp();


            SenderCashPickUpVM model = new SenderCashPickUpVM()
            {
                CountryCode = data.Country,
                FullName = data.ReceiverName,
                MobileNumber = data.MobileNo,
                IdenityCardId = recipientIdInfo.IdenityCardId,
                IdentityCardNumber = recipientIdInfo.IdentityCardNumber,
            };

            SetSenderMobileEnrterAmount(vm);
            SetSenderCashPickUp(model);
        }

        internal FaxingNonCardTransaction GetCashPickUpInfoByReceiptNo(string receiptNo)
        {
            var cashPickUpDetails = dbContext.FaxingNonCardTransaction.Where(x => x.ReceiptNumber == receiptNo).FirstOrDefault();
            return cashPickUpDetails;
        }
        internal void CancelCashPickUpTransaction(string receiptNo)
        {
            var data = GetCashPickUpInfoByReceiptNo(receiptNo);
            data.FaxingStatus = FaxingStatus.Cancel;
            dbContext.Entry<FaxingNonCardTransaction>(data).State = EntityState.Modified;
            dbContext.SaveChanges();
        }
        internal void setAmount(int transactionId)
        {
            SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();

            var paymentInfo = _kiiPaytrasferServices.GetCommonEnterAmount();
            var data = dbContext.FaxingNonCardTransaction.Where(x => x.Id == transactionId).FirstOrDefault();

            if (data != null && data.ReceivingCountry == paymentInfo.ReceivingCountryCode)
            {
                SenderMobileEnrterAmountVm vm = new SenderMobileEnrterAmountVm()
                {
                    ExchangeRate = paymentInfo.ExchangeRate,
                    Fee = paymentInfo.Fee,
                    ReceiverId = data.NonCardRecieverId,
                    ReceiverName = data.NonCardReciever.FirstName + " " + data.NonCardReciever.MiddleName + " " + data.NonCardReciever.LastName,
                    ReceivingAmount = data.ReceivingAmount,
                    ReceivingCountryCode = data.ReceivingCountry,
                    ReceivingCurrencyCode = Common.Common.GetCountryCurrency(data.ReceivingCountry),
                    ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(data.ReceivingCountry),
                    SendingAmount = data.FaxingAmount,
                    SendingCountryCode = data.SendingCountry,
                    SendingCurrencyCode = Common.Common.GetCountryCurrency(data.SendingCountry),
                    SendingCurrencySymbol = Common.Common.GetCurrencySymbol(data.SendingCountry),
                    TotalAmount = data.TotalAmount
                };

                SenderCashPickUpVM model = new SenderCashPickUpVM()
                {
                    CountryCode = data.ReceivingCountry,
                    EmailAddress = data.NonCardReciever.EmailAddress,
                    FullName = data.NonCardReciever.FirstName + " " + data.NonCardReciever.MiddleName + " " + data.NonCardReciever.LastName,
                    MobileNumber = data.NonCardReciever.PhoneNumber,
                    IdenityCardId = data.RecipientIdentityCardId,
                    IdentityCardNumber = data.RecipientIdenityCardNumber

                };
                SetSenderMobileEnrterAmount(vm);
                SetSenderCashPickUp(model);
            }
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

        internal bool RepeatTransaction(int transactionId, int RecipientId)
        {
            var cashPickup = GetSenderCashPickUp();

            string ErrorMessage = "";
            bool IsValidChasPickUpReceiver = Common.Common.IsValidBankDepositReceiver(cashPickup.MobileNumber, Service.CashPickUP);

            if (IsValidChasPickUpReceiver == false)
            {

                ErrorMessage = " Receiver is banned";
                Common.FaxerSession.ErrorMessage = ErrorMessage;
                return false;

            }

            //var CashPckUPId = SaveIncompleteTransaction();
            //Common.FaxerSession.TransactionId = CashPckUPId.Id;
            return true;
        }

        public void SetMoneyFexBankAccountDeposit(SenderMoneyFexBankDepositVM vm)
        {
            BankAccount bankDetial = Common.Common.GetBankAccountInfo(Common.FaxerSession.LoggedUser.CountryCode, TransferTypeForBankAccount.Online);
            vm.AccountNumber = bankDetial.AccountNo;
            vm.PaymentReference = Common.Common.GenerateBankPaymentReceiptNo();
            vm.ShortCode = bankDetial.LabelValue;
            vm.LabelName = bankDetial.LabelName;
            Common.FaxerSession.SenderMoneyFexBankDeposit = vm;
        }

        public SenderMoneyFexBankDepositVM GetMoneyFexBankAccountDeposit()
        {

            SenderMoneyFexBankDepositVM vm = new SenderMoneyFexBankDepositVM();

            if (Common.FaxerSession.SenderMoneyFexBankDeposit != null)
            {

                vm = Common.FaxerSession.SenderMoneyFexBankDeposit;
            }
            return vm;
        }
        public void SetTransactionPendingViewModel(TransactionPendingViewModel vm)
        {
            Common.FaxerSession.TransactionPendingViewModel = vm;
        }

        public TransactionPendingViewModel GetTransactionPendingViewModel()
        {

            TransactionPendingViewModel vm = new TransactionPendingViewModel();

            if (Common.FaxerSession.TransactionPendingViewModel != null)
            {

                vm = Common.FaxerSession.TransactionPendingViewModel;
            }
            return vm;
        }

        internal FaxingNonCardTransaction SaveIncompleteTransaction()
        {

            Services.SReceiverDetails receiverService = new SReceiverDetails();
            SFaxingNonCardTransaction service = new SFaxingNonCardTransaction();

            string MFCN = service.GetNewMFCNToSave();

            Common.FaxerSession.ReceiptNo = MFCN;
            var paymentInfo = GetSenderMobileEnrterAmount();
            var cashPickUP = GetSenderCashPickUp();

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
                receiverService.Add(recDetailObj);
                NonCardReceiveId = recDetailObj.Id;
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
            Common.FaxerSession.MFCN = MFCN;
            DB.FaxingNonCardTransaction obj = new DB.FaxingNonCardTransaction()
            {
                NonCardRecieverId = NonCardReceiveId,
                UserId = 0,
                FaxingStatus = FaxingStatus.PaymentPending,
                ReceiptNumber = MFCN,
                FaxingMethod = "PM001",
                FaxingAmount = paymentInfo.SendingAmount,
                ReceivingAmount = paymentInfo.ReceivingAmount,
                ExchangeRate = paymentInfo.ExchangeRate,
                FaxingFee = paymentInfo.Fee,
                TotalAmount = paymentInfo.TotalAmount,
                TransactionDate = System.DateTime.Now,
                SendingCountry = paymentInfo.SendingCountryCode,
                ReceivingCountry = paymentInfo.ReceivingCountryCode,
                RecipientId = RecipientId,
                MFCN = MFCN,
                SenderId = Common.FaxerSession.SenderId,
                RecipientIdenityCardNumber = cashPickUP.IdentityCardNumber,
                RecipientIdentityCardId = cashPickUP.IdenityCardId,
                SendingCurrency = cashPickUP.SendingCurrency,
                ReceivingCurrency = cashPickUP.ReceivingCurrency

            };
            var data = dbContext.FaxingNonCardTransaction.Where(x => x.Id == Common.FaxerSession.TransactionId).FirstOrDefault();

            if (data != null && data.FaxingStatus == FaxingStatus.PaymentPending
                && Common.FaxerSession.TransactionId != null && Common.FaxerSession.TransactionId > 0)
            {


                data.NonCardRecieverId = NonCardReceiveId;
                data.UserId = 0;
                data.FaxingStatus = FaxingStatus.PaymentPending;
                data.ReceiptNumber = Common.FaxerSession.ReceiptNo;
                data.FaxingMethod = "PM001";
                data.FaxingAmount = paymentInfo.SendingAmount;
                data.ReceivingAmount = paymentInfo.ReceivingAmount;
                data.ExchangeRate = paymentInfo.ExchangeRate;
                data.FaxingFee = paymentInfo.Fee;
                data.TotalAmount = paymentInfo.TotalAmount;
                data.TransactionDate = System.DateTime.Now;
                data.SendingCountry = paymentInfo.SendingCountryCode;
                data.ReceivingCountry = paymentInfo.ReceivingCountryCode;
                data.ReceivingCurrency = cashPickUP.ReceivingCurrency;
                data.SendingCurrency = cashPickUP.SendingCurrency;
                data.RecipientId = RecipientId;
                data.MFCN = MFCN;
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                return data;
            }
            else
            {
                dbContext.FaxingNonCardTransaction.Add(obj);
                dbContext.SaveChanges();
            }


            return obj;
        }

        public List<SenderCashPickUpDetialVM> GetCashPickUpInProgressTrans()
        {

            int SenderId = Common.FaxerSession.LoggedUser.Id;
            string SenderCountry = Common.FaxerSession.LoggedUser.CountryCode;
            var result = (from c in dbContext.FaxingNonCardTransaction.Where(
                                        x => x.FaxingStatus == FaxingStatus.NotReceived || x.FaxingStatus == FaxingStatus.Hold
                                        ).ToList().OrderByDescending(x => x.TransactionDate)
                          join d in dbContext.ReceiversDetails.Where(x => x.FaxerID == SenderId) on c.NonCardRecieverId equals d.Id
                          select new Models.SenderCashPickUpDetialVM()
                          {
                              Id = c.Id,
                              ReceiverName = d.FirstName == null ? d.FullName : (d.FirstName + " " + d.LastName),
                              ReceiverCity = d.City,
                              ReceiverCountry = Common.Common.GetCountryName(d.Country),
                              SentAmount = (c.FaxingAmount + c.FaxingFee).ToString("#00.00") + " ",
                              SentDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                              SentTime = c.TransactionDate.ToString("HH:mm"),
                              MFCN = c.MFCN,
                              OperatingUserType = c.OperatingUserType,
                              StatusOfFax = c.FaxingStatus,
                              Faxingstatus = Common.Common.GetEnumDescription((FaxingStatus)c.FaxingStatus),
                              SendingCurrencyCode = Common.Common.GetCountryCurrency(SenderCountry),
                              SendingCurrencySymbol = Common.Common.GetCurrencySymbol(SenderCountry),
                              AmountPaid = "" + c.FaxingAmount.ToString(),
                              ExchangeRate = c.ExchangeRate,
                              Fee = "" + c.FaxingFee,
                              ReceivingAmount = "" + c.ReceivingAmount,
                              PaymentMethod = SSenderForAllTransfer.GetCreditCardLastDigit(c.SenderPaymentMode, TransferType.CashPickup, c.Id),
                              ReceiverCurrencyCode = Common.Common.GetCountryCurrency(d.Country),
                              ReceiverCurrencySymbol = Common.Common.GetCurrencySymbol(d.Country),


                          }).ToList();
            return result;

        }
        public List<SenderBankAccountDepositVm> GetBAnkDepositInProgressTrans()
        {

            int SenderId = Common.FaxerSession.LoggedUser.Id;
            string SenderCountry = Common.FaxerSession.LoggedUser.CountryCode;
            var result = (from c in dbContext.BankAccountDeposit.Where(x => (x.Status == BankDepositStatus.Incomplete ||
                          x.Status == BankDepositStatus.Held) && x.SenderId == SenderId).ToList().OrderByDescending(x => x.TransactionDate)
                          select new Models.SenderBankAccountDepositVm()
                          {
                              Id = c.Id,
                              AccountNumber = c.ReceiverAccountNo,
                              AccountOwnerName = c.ReceiverName,
                              BankId = c.BankId,
                              BranchCode = c.BankCode,
                              CountryCode = c.ReceivingCountry,
                              CountryPhoneCode = Common.Common.GetCountryPhoneCode(c.ReceivingCountry),
                              MobileNumber = c.ReceiverMobileNo
                          }).ToList();
            return result;

        }
        public SenderTransactionHistoryViewModel GetTransactionInProgress(int faxerId = 0)
        {
            SenderTransactionHistoryViewModel transactionHistory = new SenderTransactionHistoryViewModel();
            transactionHistory.TransactionHistoryList = new List<SenderTransactionHistoryList>();
            transactionHistory.TransactionHistoryList = GetCashPickUpDetails(faxerId).
                                                         Concat(GetBankDepositDetails(faxerId)).Take(5).ToList();

            transactionHistory.TransactionHistoryList = transactionHistory.TransactionHistoryList.OrderByDescending(x => x.TransactionDate).ToList();

            return transactionHistory;
        }
        public SenderTransactionHistoryViewModel GetAllTransactionInProgress(int faxerId = 0)
        {
            SenderTransactionHistoryViewModel transactionHistory = new SenderTransactionHistoryViewModel();
            transactionHistory.TransactionHistoryList = new List<SenderTransactionHistoryList>();
            transactionHistory.TransactionHistoryList = GetCashPickUpDetails(faxerId).
                                                         Concat(GetBankDepositDetails(faxerId)).ToList();

            transactionHistory.TransactionHistoryList = transactionHistory.TransactionHistoryList.OrderByDescending(x => x.TransactionDate).ToList();

            return transactionHistory;
        }
        public List<SenderTransactionHistoryList> GetCashPickUpDetails(int senderId)
        {

            var data = dbContext.FaxingNonCardTransaction.Where(x => x.NonCardReciever.FaxerID == senderId
                           && x.FaxingStatus == FaxingStatus.NotReceived || x.FaxingStatus == FaxingStatus.Hold).ToList();

            var result = (from c in data
                          join creditDebitCardInfo in dbContext.CardTopUpCreditDebitInformation.Where(x => x.TransferType == (int)TransferType.CashPickup)
                   on c.Id equals creditDebitCardInfo.CardTransactionId into cardInfo
                          from creditDebitCardInfo in cardInfo.DefaultIfEmpty()
                          select new SenderTransactionHistoryList()
                          {
                              Id = c.Id,
                              AccountNumber = "",
                              Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                              Fee = c.FaxingFee,
                              GrossAmount = c.FaxingAmount,
                              ReceiverName = c.NonCardReciever.FullName,
                              Reference = c.MFCN,
                              Status = c.FaxingStatus,
                              StatusName = Common.Common.GetEnumDescription(c.FaxingStatus),
                              TransactionType = Common.Common.GetEnumDescription(TransactionServiceType.CashPickUp),
                              ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(c.ReceivingCountry),
                              ReceivingCurrrency = Common.Common.GetCountryCurrency(c.ReceivingCountry),
                              SendingCurrency = Common.Common.GetCountryCurrency(c.SendingCountry),
                              SendingCurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                              ReceivingAmount = c.ReceivingAmount,
                              TotalAmount = c.TotalAmount,
                              SenderPaymentMode = c.SenderPaymentMode,
                              CardNumber = creditDebitCardInfo == null ? "" : creditDebitCardInfo.CardNumber,

                              //PaymentMethod = SSenderForAllTransfer.GetCreditCardLastDigit(c.SenderPaymentMode, TransferType.CashPickup, c.Id),
                              ReceiverCity = c.NonCardReciever.City,
                              ReceiverCountry = Common.Common.GetCountryName(c.NonCardReciever.Country),
                              ExchangeRate = c.ExchangeRate,
                              TransactionServiceType = TransactionServiceType.CashPickUp,
                              TransactionDate = c.TransactionDate,
                              TransactionIdentifier = c.ReceiptNumber,

                          }).ToList();
            return result;

        }
        public List<SenderTransactionHistoryList> GetBankDepositDetails(int SenderId)
        {
            List<SenderTransactionHistoryList> result = new List<SenderTransactionHistoryList>();
            var data = dbContext.BankAccountDeposit.Where(x => x.SenderId == SenderId && x.PaidFromModule == Module.Faxer &&
                        (x.Status == BankDepositStatus.Incomplete || x.Status == BankDepositStatus.Held || x.Status == BankDepositStatus.UnHold)).ToList();

            result = (from c in data
                      join creditDebitCardInfo in dbContext.CardTopUpCreditDebitInformation.Where(x => x.TransferType == (int)TransferType.BankDeposit)
                       on c.Id equals creditDebitCardInfo.CardTransactionId into cardInfo
                      from creditDebitCardInfo in cardInfo.DefaultIfEmpty()
                      select new SenderTransactionHistoryList()
                      {
                          Id = c.Id,
                          AccountNumber = c.ReceiverAccountNo,
                          ReceiverName = c.ReceiverName,
                          ReceiverCity = c.ReceiverCity,
                          ReceiverCountry = Common.Common.GetCountryName(c.ReceiverCountry),
                          Fee = c.Fee,
                          GrossAmount = c.SendingAmount,
                          StatusOfBankDepoist = c.Status,
                          StatusName = Common.Common.GetEnumDescription(c.Status),
                          TransactionType = Common.Common.GetEnumDescription(TransactionServiceType.BankDeposit),
                          ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(c.ReceivingCountry),
                          ReceivingCurrrency = Common.Common.GetCountryCurrency(c.ReceivingCountry),
                          SendingCurrency = Common.Common.GetCountryCurrency(c.SendingCountry),
                          SendingCurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                          ReceivingAmount = c.ReceivingAmount,
                          TotalAmount = c.TotalAmount,
                          SenderPaymentMode = c.SenderPaymentMode,
                          CardNumber = creditDebitCardInfo == null ? "" : creditDebitCardInfo.CardNumber,
                          //PaymentMethod = SSenderForAllTransfer.GetCreditCardLastDigit(c.SenderPaymentMode, TransferType.BankDeposit, c.Id),
                          ExchangeRate = c.ExchangeRate,
                          Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                          TransactionServiceType = TransactionServiceType.BankDeposit,
                          TransactionDate = c.TransactionDate,
                          Reference = c.ReceiptNo,
                          BankCode = c.BankCode,
                          BankName = Common.Common.getBankName(c.BankId),
                          TransactionIdentifier = c.ReceiptNo

                      }).ToList();
            return result;

        }
        internal int GetCashPickUpInProgressTransCount(int SenderId)
        {

            var result = dbContext.FaxingNonCardTransaction.Where(x => x.NonCardReciever.FaxerID == SenderId
                                                               && (x.FaxingStatus == FaxingStatus.NotReceived
                                                               || x.FaxingStatus == FaxingStatus.Hold)).Count();

            var bankAccountDepositCount = dbContext.BankAccountDeposit.Where(x => x.SenderId == SenderId
                                                                        && (x.Status == BankDepositStatus.Incomplete

                                                                        || x.Status == BankDepositStatus.Held || x.Status == BankDepositStatus.UnHold)).Count();
            return result + bankAccountDepositCount;

        }

        public void SendEmailAndSms(FaxingNonCardTransaction item)
        {
            TransactionEmailType _transactionEmailType = TransactionEmailType.CustomerSupport;

            switch (item.FaxingStatus)
            {
                case FaxingStatus.Hold:
                    Common.Common.SetTransactionEmailTypeSession(TransactionEmailType.TransactionInProgress);
                    TransactionCompleteEmail(item, true);
                    break;
                case FaxingStatus.Paused:
                    Common.Common.SetTransactionEmailTypeSession(TransactionEmailType.IDCheck);
                    SendTransactionPausedEmail(item);
                    break;
                case FaxingStatus.Cancel:
                    Common.Common.SetTransactionEmailTypeSession(TransactionEmailType.TransactionCancelled);
                    TransactionCancelled(item);
                    break;
                case FaxingStatus.Completed:
                    Common.Common.SetTransactionEmailTypeSession(TransactionEmailType.TransactionCompleted);
                    TransactionCompleteEmail(item);
                    TransactionCompleteSms(item);

                    //SendTransactionCompletedSms(item);
                    //TransactionCompletedEmail(item);
                    break;
                case FaxingStatus.NotReceived:
                    Common.Common.SetTransactionEmailTypeSession(TransactionEmailType.TransactionInProgress);
                    TransactionCompleteEmail(item, true);
                    TransactionInProgressSms(item);
                    //TransactionCompleteEmail(item);
                    //if (item.IsManualDeposit == true)
                    //{
                    //    ManualBankDepositEmail(item);
                    //}
                    //else
                    //{
                    //    SendTransactionInProgressSms(item);
                    //    TransactionInProgressEmail(item);
                    //}
                    break;
                case FaxingStatus.PaymentPending:
                    break;
                case FaxingStatus.IdCheckInProgress:
                    Common.Common.SetTransactionEmailTypeSession(TransactionEmailType.TransactionInProgress);
                    IdCheckInProgress(item);
                    break;
                case FaxingStatus.PendingBankdepositConfirmtaion:
                    break;
                case FaxingStatus.FullRefund:

                    _transactionEmailType = TransactionEmailType.TransactionCompleted;
                    Common.Common.SetTransactionEmailTypeSession(_transactionEmailType);
                    TransactionRefundEmail(item);
                    break;
                case FaxingStatus.PartailRefund:

                    _transactionEmailType = TransactionEmailType.TransactionCompleted;
                    Common.Common.SetTransactionEmailTypeSession(_transactionEmailType);
                    TransactionRefundEmail(item);
                    break;
                default:
                    break;
            }
            ///FaxerSession.TransactionEmailTypeSession = _transactionEmailType;


        }

        private void IdCheckInProgress(FaxingNonCardTransaction item)
        {
            SSenderBankAccountDeposit _sBankDeposit = new SSenderBankAccountDeposit();
            var senderInfo = _sBankDeposit.GetSenderInfo(item.SenderId);
            string email = senderInfo.Email;

            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string receivingCountry = Common.Common.GetCountryName(item.ReceivingCountry);
            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/CashPickUPEmail?" +
                                     "&senderName=" + senderInfo.FirstName +
                                     "&receiverName=" + item.NonCardReciever.FullName +
                                     "&receiverFirstName=" + item.NonCardReciever.FirstName +
                                     "&MFCN=" + item.MFCN +
                                     "&sendingAmount=" + item.FaxingAmount +
                                     "&ReceivingAmount=" + item.ReceivingAmount +
                                     "&Fee=" + item.FaxingFee +
                                     "&receivingCountry=" + receivingCountry +
                                     "&PaymentReference=" + item.PaymentReference +
                                     "&SenderPaymentMode=" + item.SenderPaymentMode);

            mail.SendMail(email, "Confirmation of transfer to" + " " + item.NonCardReciever.FirstName, body);
        }

        public void TransactionRefundEmail(FaxingNonCardTransaction item)
        {
            // Sending Email Code goes here 
            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);
            string email = senderInfo.Email;

            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string bankName = "";
            string WalletName = "";
            string sendingCountryCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(item.SendingCurrency, item.SendingCountry);
            string receivingCountryCurrecy = Common.Common.GetCurrencyByCurrencyOrCountry(item.ReceivingCurrency, item.ReceivingCountry);
            string ReceivingCountryName = Common.Common.GetCountryName(item.ReceivingCountry);
            string receiverFirstName = "";
            try
            {
                receiverFirstName = item.NonCardReciever.FullName.Trim().Split(' ')[0];
            }
            catch (Exception)
            {
            }
            var refundHistory = dbContext.RefundHistory.Where(x => x.TransactionId == item.Id && x.TransactionServiceType == TransactionServiceType.CashPickUp).FirstOrDefault();

            if (refundHistory != null)
            {
                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/RefundIssued/Index?" +
                    "&SenderFristName=" + senderInfo.FirstName +
                    "&SendingCurrency=" + sendingCountryCurrency +
                    "&RefundAmount=" + refundHistory.RefundedAmount +
                    "&TransactionNumber=" + item.ReceiptNumber +
                    "&SendingAmount=" + item.FaxingAmount +
                    "&SendingCountry=" + item.SendingCountry +
                    "&Fee=" + item.FaxingFee +
                    "&ReceiverFirstName=" + receiverFirstName +
                    "&ReceivingCurrency=" + receivingCountryCurrecy +
                    "&ReceivingAmount=" + item.ReceivingAmount +
                    "&BankName=" + bankName +
                    "&BankAccount=" + "" +
                    "&BranchCode=" + "" +
                    "&transactionServiceType=" + TransactionServiceType.CashPickUp +
                    "&WalletName=" + WalletName +
                    "&MobileNo=" + item.NonCardReciever.PhoneNumber);
                mail.SendMail(email, "Refund Issued -" + item.ReceiptNumber, body);
            }
        }


        public void TransactionCompleteSms(FaxingNonCardTransaction cashPickUp)
        {
            var senderInfo = Common.Common.GetSenderInfo(cashPickUp.SenderId);
            string phoneNo = Common.Common.GetCountryPhoneCode(cashPickUp.SendingCountry) + senderInfo.PhoneNumber;
            SmsApi smsApi = new SmsApi();
            var receiverDetail = dbContext.ReceiversDetails.Where(x => x.Id == cashPickUp.NonCardRecieverId).FirstOrDefault();
            string receiverFirstName = receiverDetail.FirstName;
            string senderFirstName = senderInfo.FirstName;
            string MFCN = cashPickUp.MFCN;
            string AmountReceive = Common.Common.GetCurrencyByCurrencyOrCountry(cashPickUp.ReceivingCurrency, cashPickUp.ReceivingCountry) + " " + cashPickUp.ReceivingAmount;
            //sender
            string msg = smsApi.GetCashPickUPReceivedMessage(senderFirstName, receiverFirstName, MFCN, AmountReceive);
            smsApi.SendSMS(phoneNo, msg);
            //Receiver
            string SendingCountry = Common.Common.GetCountryName(cashPickUp.SendingCountry);
            SendCashPickUpToReceiverSms(SendingCountry, receiverDetail.PhoneNumber);
        }
        public void SendCashPickUpToReceiverSms(string senderCountry, string PhoneNo)
        {
            SmsApi smsApi = new SmsApi();
            string msg = smsApi.GetCashPickUpReceivedToReceiverMessage(senderCountry);
            smsApi.SendSMS(PhoneNo, msg);
        }
        public void TransactionInProgressSms(FaxingNonCardTransaction cashPickUp)

        {
            var senderInfo = Common.Common.GetSenderInfo(cashPickUp.SenderId);
            string phoneNo = Common.Common.GetCountryPhoneCode(cashPickUp.SendingCountry) + senderInfo.PhoneNumber;
            SmsApi smsApi = new SmsApi();
            var receiverDetail = dbContext.ReceiversDetails.Where(x => x.Id == cashPickUp.NonCardRecieverId).FirstOrDefault();
            string receiverFirstName = receiverDetail.FirstName;
            string senderFirstName = senderInfo.FirstName;
            string MFCN = cashPickUp.MFCN;
            string AmountReceive = Common.Common.GetCurrencyByCurrencyOrCountry(cashPickUp.ReceivingCurrency, cashPickUp.ReceivingCountry) + " " + cashPickUp.ReceivingAmount;
            string msg = smsApi.GetCashPickUPTransferMessage(senderFirstName, receiverFirstName, MFCN, AmountReceive);
            smsApi.SendSMS(phoneNo, msg);

        }


        public void TransactionCancelled(FaxingNonCardTransaction item)
        {

            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);

            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string bankName = "";
            string ReceiverCountryName = Common.Common.GetCountryName(item.ReceivingCountry);
            string SendingCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(item.SendingCurrency, item.SendingCountry);
            string ReceivingCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(item.ReceivingCurrency, item.ReceivingCountry);
            string WalletName = "";

            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/TransferedCancelledByTZ?" +
                 "&SenderFristName=" + senderInfo.FirstName +
                 "&TransactionNumber=" + item.ReceiptNumber +
                 "&RecipentName=" + item.NonCardReciever.FullName +
                 "&BankName=" + bankName +
                 "&BankAccount=" + "" +
                 "&ReceiverCountry=" + ReceiverCountryName +
                 "&transferMethod=" + TransactionTransferMethod.CashPickUp +
                 "&WalletName=" + WalletName +
                 "&MobileNo=" + item.NonCardReciever.PhoneNumber
                 );

            mail.SendMail(senderInfo.Email, "Money Transfer Cancelled" + " " + item.ReceiptNumber, body);

        }

        public void TransactionCancelledByMoneyFex(FaxingNonCardTransaction item)
        {

            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);

            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string bankName = "";
            string ReceiverCountryName = Common.Common.GetCountryName(item.ReceivingCountry);

            string SendingCurrency = Common.Common.GetCountryName(item.SendingCountry);
            string ReceivingCurrency = Common.Common.GetCountryName(item.ReceivingCountry);

            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/TransactionCancelledByMoneyFex?" +
                 "&SenderFristName=" + senderInfo.FirstName +
                 "&TransactionNumber=" + item.ReceiptNumber +
                 "&RecipentName=" + item.NonCardReciever.FullName +
                 "&BankName=" + bankName +
                 "&BankAccount=" + "" +
                 "&ReceiverCountry=" + ReceiverCountryName +
                 "&receivingCurrency=" + ReceivingCurrency +
                 "&sendingCurrency=" + SendingCurrency +
                 "&exchangeRate=" + item.ExchangeRate +
                 "&fee=" + item.FaxingFee +
                 "&receivingAmount=" + item.ReceivingAmount +
                 "&sendingAmount=" + item.FaxingAmount +
                 "&bankCode=" + "" +
                 "&transferMethod=" + TransactionTransferMethod.CashPickUp +
                 "&walletName=" + "" +
                 "&mobileNo=" + item.NonCardReciever.PhoneNumber
                 );

            mail.SendMail(senderInfo.Email, "Money Transfer Cancelled" + " " + item.ReceiptNumber, body);

        }



        public void TransactionCompleteEmail(FaxingNonCardTransaction cashPickUp, bool IsInProgress = false)
        {
            var senderInfo = dbContext.FaxerInformation.Where(x => x.Id == cashPickUp.SenderId).FirstOrDefault();
            string email = senderInfo.Email;
            string SenderFristName = senderInfo.FirstName;

            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string ReceivingCountry = Common.Common.GetCountryName(cashPickUp.ReceivingCountry);

            string sendingAmountWithCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(cashPickUp.SendingCurrency, cashPickUp.SendingCountry) + " " + cashPickUp.FaxingAmount;
            string receivingAmountWithCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(cashPickUp.ReceivingCurrency, cashPickUp.ReceivingCountry) + " " +
                                                 cashPickUp.ReceivingAmount + Common.Common.GetCountryName(cashPickUp.ReceivingCountry);
            string feeWithCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(cashPickUp.SendingCurrency, cashPickUp.SendingCountry) + " " + cashPickUp.FaxingAmount;

            //   public ActionResult Index(string SenderName = "", string receiverName = "", string MFCN = "",
            //string SentAmount = "", string Fee = "", string receiverFirstName = "",
            //string receivingAmount = "", string ReceivingCountry = "", string ReceivingCur = "",
            //string SendingCur = "")
            //   {
            string receiverName = "";
            string receivierFirstName = "";
            var receiverInfo = dbContext.ReceiversDetails.Where(x => x.Id == cashPickUp.NonCardRecieverId).FirstOrDefault();
            if (receiverInfo != null)
            {
                receiverName = receiverInfo.FullName;
                receivierFirstName = receiverInfo.FirstName;
            }

            string SendingCur = Common.Common.GetCurrencyByCurrencyOrCountry(cashPickUp.SendingCurrency, cashPickUp.SendingCountry);
            string ReceivingCur = Common.Common.GetCurrencyByCurrencyOrCountry(cashPickUp.ReceivingCurrency, cashPickUp.ReceivingCountry);
            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/CashPickUpCompletedEmail?" +
                "&SenderFristName=" + SenderFristName
            + "&MFCN=" + cashPickUp.ReceiptNumber
            + "&SentAmount=" + cashPickUp.FaxingAmount
            + "&receiverName=" + receiverName
            + "&Fee=" + cashPickUp.FaxingFee
            + "&Receivingcountry=" + ReceivingCountry
            + "&receivingAmount=" + cashPickUp.ReceivingAmount
            + "&ReceiverFirstName=" + receivierFirstName
            + "&ReceivingCur=" + ReceivingCur
            + "&SendingCur=" + SendingCur
             + "&IsInProgress=" + IsInProgress);


            mail.SendMail(email, "Confirmation of money transferred cash pickup", body);

        }


        public void SendTransactionPausedEmail(FaxingNonCardTransaction cashPickUp)
        {
            SSenderBankAccountDeposit _sBankDeposit = new SSenderBankAccountDeposit();
            var senderInfo = _sBankDeposit.GetSenderInfo(cashPickUp.SenderId);
            string email = senderInfo.Email;
            string SenderFristName = senderInfo.FirstName;

            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string ReceivingCountry = Common.Common.GetCountryName(cashPickUp.ReceivingCountry);

            string sendingAmountWithCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(cashPickUp.SendingCurrency, cashPickUp.SendingCountry) + " " + cashPickUp.FaxingAmount;
            string receivingAmountWithCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(cashPickUp.ReceivingCurrency, cashPickUp.ReceivingCountry) + " " +
                                                 cashPickUp.ReceivingAmount + Common.Common.GetCountryName(cashPickUp.ReceivingCountry);
            string feeWithCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(cashPickUp.SendingCurrency, cashPickUp.SendingCountry) + " " + cashPickUp.FaxingAmount;

            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/TransactionPaused?" + "&SenderFristName=" + SenderFristName
                + "&TransactionNumber=" + cashPickUp.ReceiptNumber
                + "&SendingAmount=" + sendingAmountWithCurrency
                + "&ReceivingAmount=" + receivingAmountWithCurrency
                + "&Receivingcountry=" + ReceivingCountry
                + "&Fee=" + feeWithCurrency
                + "&ReceiverFirstName=" + cashPickUp.NonCardReciever.FirstName
                + "&BankName=" + ""
                + "&BankAccount=" + ""
                + "&BankCode=" + 0
                + "&TransactionServiceType=" + TransactionServiceType.CashPickUp
                + "&WalletName=" + ""
                + "&MFCN=" + cashPickUp.MFCN);


            mail.SendMail(email, "Your transfer has been paused", body);

        }






        public CashPickUpTransactionApiResponse CreateCashPickTransactionToApi(FaxingNonCardTransaction item, TransactionTransferType TransactionTransferType = TransactionTransferType.Online, Module module = Module.Faxer)
        {
            if (item.IsComplianceApproved == false)
            {

                if (item.SenderPaymentMode == SenderPaymentMode.CreditDebitCard ||
                    item.SenderPaymentMode == SenderPaymentMode.SavedDebitCreditCard)
                {
                    try
                    {

                        var TransactionLimitAmount = Common.Common.HasExceededAmountLimit(item.SenderId, item.SendingCountry, item.ReceivingCountry, item.FaxingAmount, module);
                        if (TransactionLimitAmount)
                        {
                            var cashPickUp = dbContext.FaxingNonCardTransaction.Where(x => x.Id == item.Id).FirstOrDefault();
                            cashPickUp.IsComplianceNeededForTrans = true;
                            cashPickUp.FaxingStatus = FaxingStatus.Hold;
                            dbContext.Entry<FaxingNonCardTransaction>(cashPickUp).State = EntityState.Modified;
                            dbContext.SaveChanges();

                            return new CashPickUpTransactionApiResponse()
                            {
                                CashPickUp = cashPickUp,
                                BankDepositApiResponseVm = new BankApi.Models.BankDepositResponseVm()
                            };

                        }
                    }
                    catch (Exception ex)
                    {
                        return new CashPickUpTransactionApiResponse()
                        {
                            CashPickUp = item,
                            BankDepositApiResponseVm = new BankApi.Models.BankDepositResponseVm()
                        };
                    }

                }
            }

            //if (item.PaidFromModule == Module.Faxer || item.PaidFromModule == Module.Agent)
            //{
            //var IsPayoutFlowControlEnabled = Common.Common.IsPayoutFlowControlEnabled(item.SendingCountry, item.ReceivingCountry,
            // item.Apiservice, TransactionTransferMethod.BankDeposit, item.BankId);
            //if (IsPayoutFlowControlEnabled == false)
            //{
            //    item.Status = BankDepositStatus.Paused;
            //    Update(item);
            //    return new BankTransactionApiResponse()
            //    {
            //        BankAccountDeposit = item,
            //        BankDepositApiResponseVm = new BankDepositResponseVm()
            //    };
            //}

            var IsPayoutFlowControlEnabled = Common.Common.IsPayoutFlowControlEnabled(item.SendingCountry, item.ReceivingCountry,
                        null, TransactionTransferMethod.CashPickUp, 0);
            if (IsPayoutFlowControlEnabled == false)
            {
                var cashPickUp = dbContext.FaxingNonCardTransaction.Where(x => x.Id == item.Id).FirstOrDefault();
                cashPickUp.FaxingStatus = FaxingStatus.Paused;
                dbContext.Entry<FaxingNonCardTransaction>(cashPickUp).State = EntityState.Modified;
                dbContext.SaveChanges();
                return new CashPickUpTransactionApiResponse()
                {
                    CashPickUp = cashPickUp,
                    BankDepositApiResponseVm = new BankApi.Models.BankDepositResponseVm()
                };
            }
            //}
            int agentId = 0;
            if (item.OperatingUserType == OperatingUserType.Agent)
            {
                agentId = Common.Common.GetAgentIdByPayingId(item.PayingStaffId);
            }
            var ApiService = Common.Common.GetApiservice(item.SendingCountry,
             item.ReceivingCountry, item.FaxingAmount, TransactionTransferMethod.CashPickUp, TransactionTransferType, agentId);
            var bankdepositTransactionResult = new BankApi.Models.BankDepositResponseVm();
            var Receiver = dbContext.Recipients.Where(x => x.Id == item.RecipientId).FirstOrDefault();
            Log.Write(item.ReceiptNumber + " Transaction Started");
            switch (ApiService)
            {

                case DB.Apiservice.TransferZero:
                    Log.Write("Hello");
                    string TransactionId = item.ReceiptNumber;
                    var transferZeroResponse = GetCashPickUpTransferZeroTransactionResponse(item);
                    var status = GetTransferZeroTransactionStatus(transferZeroResponse);
                    var transferZeroTransactionResult = transferZeroResponse;
                    var responseModel = PrepareCashPickUpTransactionTransferZeroResponse(transferZeroTransactionResult);
                    //responseModel.result.beneficiaryAccountName = item.ReceiverAccountNo;
                    //responseModel.result.beneficiaryBankCode = item.BankCode;
                    responseModel.result.beneficiaryAccountName = Receiver.ReceiverName;
                    responseModel.result.amountInBaseCurrency = item.FaxingAmount;
                    responseModel.result.targetAmount = item.ReceivingAmount;
                    responseModel.result.partnerTransactionReference = item.ReceiptNumber;
                    bankdepositTransactionResult = responseModel;
                    item.FaxingStatus = status;
                    item.TransferReference = transferZeroResponse.Id.ToString();
                    try
                    {
                        item.TransferZeroSenderId = transferZeroResponse.Sender.Id.ToString();

                        Log.Write("Cash Pick Transaction has been Intiated" + item.ReceiptNumber);
                    }
                    catch (Exception ex)
                    {
                        Log.Write(ex.Message, ErrorType.TransferZero, "CreateCashPickTransactionToApi");
                    }
                    break;
                case Apiservice.Wari:
                    try
                    {
                        var wariLogin = GetWariLogin();
                        var wariTransactionResponse = GetWariApiResponse(item, wariLogin.sessionId);
                        var faxingStatus = GetWariTransactionStatus(wariTransactionResponse.transactionId, wariLogin.sessionId);

                        bankdepositTransactionResult.result = new BankDepositResponseResult()
                        {
                            beneficiaryAccountName = Receiver.ReceiverName,
                            amountInBaseCurrency = item.FaxingAmount,
                            targetAmount = item.ReceivingAmount,
                            partnerTransactionReference = item.ReceiptNumber,
                            transactionReference = wariTransactionResponse.transactionId,
                            transactiondate = wariTransactionResponse.date,

                        };
                        bankdepositTransactionResult.extraResult = wariTransactionResponse.transactionId;
                        item.FaxingStatus = faxingStatus;
                        Log.Write("Cash Pick Transaction has been Intiated" + item.ReceiptNumber);
                    }
                    catch (Exception ex)
                    {
                        Log.Write(ex.Message, ErrorType.Wari, "CreateCashPickTransactionToApi");
                    }

                    break;
                case Apiservice.CashPot:
                    try
                    {
                        var ExchangeRate = GetRateCashPot(item);
                        if (ExchangeRate != 0)
                        {
                            item.ExchangeRate = ExchangeRate;
                        }
                        var postResponse = GetCashPikUpCashPotTransactionResponse(item);
                        var statusResonse = GetStatusResponse(postResponse.REFERENCE_CODE);
                        item.FaxingStatus = GetCashPotCAshPickUpTransactionStatus(statusResonse.STATUS_CODE);

                        BankDepositResponseVm bankDepositResponse = new BankDepositResponseVm()
                        {
                            extraResult = postResponse.REFERENCE_CODE,
                            status = true,
                            response = (int)item.FaxingStatus,
                            result = new BankDepositResponseResult()
                            {
                                transactionStatus = (int)item.FaxingStatus,
                                transactionReference = postResponse.REFERENCE_CODE,
                                transactiondate = postResponse.DATE,
                                beneficiaryAccountName = Receiver.ReceiverName,
                                amountInBaseCurrency = item.FaxingAmount,
                                targetAmount = item.ReceivingAmount,
                                partnerTransactionReference = item.ReceiptNumber,
                            }
                        };
                        bankdepositTransactionResult = bankDepositResponse;
                    }
                    catch (Exception ex)
                    {
                        Log.Write(ex.Message, ErrorType.CashPot, "CreateBankTransactionToApi");
                    }
                    break;
                default:
                    break;
            }

            return new CashPickUpTransactionApiResponse()
            {

                CashPickUp = item,
                BankDepositApiResponseVm = bankdepositTransactionResult,
            };
        }

        public TransactionStatusResposeCashPotVm GetStatusResponse(string referenceCode)
        {
            CashPotApi cashPotApi = new CashPotApi();
            TransactionStatusRequest request = new TransactionStatusRequest()
            {

                USER = Common.Common.GetAppSettingValue("CashPotUser"),
                PASSWORD = Common.Common.GetAppSettingValue("CashPotPASSWORD"),
                PARTNER_ID = Common.Common.GetAppSettingValue("CashPotPARTNER_ID"),
                REFERENCE_CODE = referenceCode,

            };
            var response = cashPotApi.GetTransactionStatus(request);
            return response;
        }

        private decimal GetRateCashPot(FaxingNonCardTransaction item)
        {
            decimal Rate = 0;
            CashPotApi cashPotApi = new CashPotApi();
            string sendingCurrency = Common.Common.GetCountryCurrency(item.SendingCountry);
            string receivingCurrency = Common.Common.GetCountryCurrency(item.ReceivingCountry);
            RateGenericRequest rateGenericRequest = new RateGenericRequest()
            {
                USER = Common.Common.GetAppSettingValue("CashPotUser"),
                PASSWORD = Common.Common.GetAppSettingValue("CashPotPASSWORD"),
                PARTNER_ID = Common.Common.GetAppSettingValue("CashPotPARTNER_ID"),
                SENDING_CURRENCY = sendingCurrency,
                RECEIVING_COUNTRY = item.ReceivingCountry,
                RECEIVING_CURENCY = receivingCurrency,
                AMOUNT = item.TotalAmount.ToString(),
                SENDING_COUNTRY = item.SendingCountry,
                TRANS_TYPE_ID = "1",
            };
            try
            {
                var rateResponse = cashPotApi.GetRates(rateGenericRequest);
                if (rateResponse.MSG_CODE == "0005")
                {
                    Rate = rateResponse.RATE.ToDecimal();
                }
                else
                {
                    Rate = Common.Common.GetPayoutProviderRate(item.SendingCountry, item.ReceivingCountry, Apiservice.CashPot);
                }

            }
            catch (Exception ex)
            {
                Log.Write(ex.Message, ErrorType.CashPot, "GetRateCashPot");
                Rate = Common.Common.GetPayoutProviderRate(item.SendingCountry, item.ReceivingCountry, Apiservice.CashPot);
            }
            return Rate;
        }

        public FaxingStatus GetCashPotCAshPickUpTransactionStatus(string StatusCode)
        {
            FaxingStatus status = FaxingStatus.NotReceived;
            if (StatusCode == "1" || StatusCode == "2" || StatusCode == "3" ||
             StatusCode == "5" || StatusCode == "13" || StatusCode == "15" ||
             StatusCode == "16" || StatusCode == "17" || StatusCode == "18" ||
             StatusCode == "19" || StatusCode == "20")
            {
                status = FaxingStatus.NotReceived;
            }
            else if (StatusCode == "6" || StatusCode == "7")
            {
                status = FaxingStatus.Cancel;
            }
            else if (StatusCode == "10")
            {
                status = FaxingStatus.Completed;
            }
            else if (StatusCode == "14")
            {
                status = FaxingStatus.FullRefund;
            }
            return status;
        }

        private SendTransGenericResponseVm GetCashPikUpCashPotTransactionResponse(FaxingNonCardTransaction item)
        {
            CashPotApi cashPotApi = new CashPotApi();
            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);

            var receiverInfo = dbContext.Recipients.Where(x => x.Id == item.RecipientId).FirstOrDefault();
            string[] Name = receiverInfo.ReceiverName.Trim().Split(' ');
            string FirstName = "";
            string LastName = "";
            try
            {
                FirstName = Name[0];

                if (Name.Length > 1)
                {
                    for (int i = 1; i < Name.Length; i++)
                    {
                        LastName = LastName + Name[i] + " ";
                    }
                }
                if (string.IsNullOrEmpty(LastName))
                {

                    LastName = FirstName;
                }

            }
            catch (Exception)
            {

            }


            SendTransGenericRequestVm sendTransGenericRequest = new SendTransGenericRequestVm()
            {

                USER = Common.Common.GetAppSettingValue("CashPotUser"),
                PASSWORD = Common.Common.GetAppSettingValue("CashPotPASSWORD"),
                PARTNER_ID = Common.Common.GetAppSettingValue("CashPotPARTNER_ID"),
                REFERENCE_CODE = item.ReceiptNumber,
                DATE = item.TransactionDate.ToString(),
                TRANSSTATUS = "1",
                SENDING_CURRENCY = Common.Common.GetCountryCurrency(item.SendingCountry),
                RECEIVER_CURENCY = Common.Common.GetCurrencyCode(item.ReceivingCountry),
                RATE = item.ExchangeRate.ToString(),
                FEE = item.FaxingFee.ToString(),
                LOCATION_ID = "",
                PAYER_ID = "10246 ",
                SEND_AMOUNT = item.FaxingAmount.ToString(),
                SENDER_FIRST_NAME = senderInfo.FirstName,
                SENDER_LAST_NAME = senderInfo.MiddleName != null ? senderInfo.MiddleName + " " + senderInfo.LastName : senderInfo.LastName,
                SENDER_ADDRESS = senderInfo.Address1,
                SENDER_COUNTRY = senderInfo.Country,
                RECEIVER_FIRST_NAME = FirstName,
                RECEIVER_LAST_NAME = LastName,
                RECEIVER_MOBILE_NUMBER = receiverInfo.MobileNo,
                RECEIVING_AMOUNT = item.ReceivingAmount.ToString(),
                RECEIVER_ADDRESS = "",
                RECEIVER_CITY = "",
                RECEIVER_COUNTRY = item.ReceivingCountry,
                TRANSACTION_TYPE = "1",//CahPick up transfer transaction type = 1
                SECRET_ANSWER = item.ReceiptNumber,
                SECRET_QUESTION = "What is your pickup reference number ?",
                SENDER_DOB = senderInfo.DateOfBirth.ToString(),
                SENDER_POST_CODE = senderInfo.PostalCode,
                SITE_LOCATION = ""

            };
            var senderDoument = dbContext.SenderBusinessDocumentation.Where(x => x.SenderId == item.SenderId).FirstOrDefault();
            if (senderDoument != null)
            {
                sendTransGenericRequest.SENDER_ID_NUMBER = senderDoument.IdentityNumber;
                sendTransGenericRequest.SENDER_ID_ISSUE_DATE = "";
                sendTransGenericRequest.SENDER_ID_EXPIRY_DATE = senderDoument.ExpiryDate.ToString();
            }
            var response = cashPotApi.PostTransaction(sendTransGenericRequest);
            return response;
        }

        #region Wari 
        public WariLoginResponseVm GetWariLogin()
        {
            WariLoginRequestVm wariRequestVm = new WariLoginRequestVm()
            {
                login = "16129",
                partnerId = "DIRHAM_TEST",
                password = "Dirh@mTest"
            };
            WariApi wariApi = new WariApi();
            var login = wariApi.Login<WariLoginResponseVm>(CommonExtension.SerializeObject<WariLoginRequestVm>(wariRequestVm));
            return login.Result;
        }
        public WariPushTransactionResponseVm GetWariApiResponse(FaxingNonCardTransaction item, string sessionId)
        {
            WariApi wariApi = new WariApi();
            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);
            var Receiver = dbContext.Recipients.Where(x => x.Id == item.RecipientId).FirstOrDefault();
            var receivername = Receiver.ReceiverName.Trim();
            string[] Name = receivername.Split(' ');
            string FirstName = "";
            string LastName = "";
            try
            {
                FirstName = Name[0];
                if (Name.Length > 1)
                {
                    for (int i = 1; i < Name.Length; i++)
                    {
                        LastName = LastName + Name[i] + " ";
                    }
                }
                if (string.IsNullOrEmpty(LastName))
                {
                    LastName = FirstName;
                }
            }
            catch (Exception)
            {

            }

            var sender = new SenderInfoForWariVM()
            {
                adresse = senderInfo.Address1,
                firstName = senderInfo.FirstName,
                lastName = senderInfo.LastName,
                phoneNumber = senderInfo.PhoneNumber,
                payspiece = "",
                typepiece = "",
                numeropiece = "",
                datepiece = "",
                dateexpirationpiece = ""
            };
            var receiver = new ReceiverInfoForWariVM()
            {
                adresse = Receiver.Street,
                firstName = FirstName,
                lastName = LastName,
                phoneNumber = Receiver.MobileNo
            };
            string sendingCurrency = Common.Common.GetCountryCurrency(item.SendingCountry);
            string receivingCurrency = Common.Common.GetCountryCurrency(item.ReceivingCountry);
            var transaction = new WariTransactionVm
            {
                transactionId = item.Id.ToString(),
                incomingAmount = item.FaxingAmount.ToString(),
                incomingCurrencyCode = sendingCurrency,
                incomingCountryCode = item.SendingCountry,
                destinationCurrencyCode = receivingCurrency,
                destinationAmount = item.ReceivingAmount.ToString(),
                destinationCountryCode = item.ReceivingCountry,
                reference = item.MFCN,
                message = "",
                exchangeRate = item.ExchangeRate.ToString()
            };
            var requestId = Common.Common.GenerateRandomDigit(4);
            WariPushTransactionRequestVm WariPushTransactionRequestVm = new WariPushTransactionRequestVm()
            {
                sessionId = sessionId,
                requestId = requestId,
                receiver = receiver,
                sender = sender,
                transaction = transaction
            };
            var transactionResponse = wariApi.PushTransaction<WariPushTransactionResponseVm>(CommonExtension.SerializeObject<WariPushTransactionRequestVm>(WariPushTransactionRequestVm));
            return transactionResponse.Result;
        }
        public FaxingStatus GetWariTransactionStatus(string transactionId, string sessionId)
        {

            FaxingStatus status = FaxingStatus.NotReceived;

            WariVm wari = new WariVm()
            {
                sessionId = sessionId,
                transactionId = transactionId,
            };
            WariApi wariApi = new WariApi();

            var transactionStatus = wariApi.TransactionSatuts<TransactionStatusWariVm>(CommonExtension.SerializeObject<WariVm>(wari)).Result;
            if (transactionStatus.status != null)
            {
                switch (transactionStatus.status.ToLower())
                {
                    case "p":
                        status = FaxingStatus.PaymentPending;
                        break;
                    case "v":
                        status = FaxingStatus.Completed;
                        break;
                    case "c":
                        status = FaxingStatus.Cancel;
                        break;
                }
            }
            return status;
        }

        public FaxingStatus CancelWariTransaction(string transactionId)
        {
            FaxingStatus status = FaxingStatus.NotReceived;

            var WariLoginResponse = GetWariLogin();
            WariApi wariApi = new WariApi();

            WariVm wari = new WariVm()
            {
                sessionId = WariLoginResponse.sessionId,
                transactionId = transactionId,
            };

            var cancelTransaction = wariApi.CancelTransaction<TransactionStatusWariVm>(CommonExtension.SerializeObject<WariVm>(wari)).Result;
            if (cancelTransaction.codeResponse == "0")
            {
                status = FaxingStatus.Cancel;
            }
            return status;
        }
        #endregion



        public TransferZero.Sdk.Model.Transaction GetCashPickUpTransferZeroTransactionResponse(FaxingNonCardTransaction cashPickUp)
        {
            var senderDetail = Common.Common.GetSenderInfo(cashPickUp.SenderId);

            string receivingCurrency = Common.Common.GetCountryCurrency(cashPickUp.ReceivingCountry);
            string sendingCurrency = Common.Common.GetCountryCurrency(cashPickUp.SendingCountry);

            string[] phoneCode = Common.Common.GetCountryPhoneCode(cashPickUp.ReceivingCountry).Split('+');
            string receiverPhoneCode = "";
            var Receiver = dbContext.Recipients.Where(x => x.Id == cashPickUp.RecipientId).FirstOrDefault();
            var receivername = Receiver.ReceiverName.Trim();
            string[] Name = receivername.Split(' ');
            string FirstName = "";
            string LastName = "";
            try
            {
                receiverPhoneCode = phoneCode[phoneCode.Length - 1];
                FirstName = Name[0];

                if (Name.Length > 1)
                {

                    for (int i = 1; i < Name.Length; i++)
                    {
                        LastName = LastName + Name[i] + " ";
                    }
                }
                if (string.IsNullOrEmpty(LastName))
                {

                    LastName = FirstName;
                }

            }
            catch (Exception)
            {

            }

            Sender sender = new Sender(
                   //id: Guid.NewGuid(),
                   firstName: senderDetail.FirstName,
                   lastName: senderDetail.LastName,
                   phoneCountry: senderDetail.Country,
                   phoneNumber: senderDetail.PhoneNumber,
                   country: senderDetail.Country,
                   city: senderDetail.City,
                   street: senderDetail.Address1,
                   postalCode: senderDetail.PostalCode,
                   addressDescription: "",
                   birthDate: senderDetail.DateOfBirth,

                   // you can usually use your company's contact email address here
                   email: senderDetail.Email,
                   externalId: senderDetail.AccountNo,

                   // you'll need to set these fields but usually you can leave them the default
                   ip: "127.0.0.1",
                   documents: new List<Document>()
          );

            // Senagalese ko lagi matra param different huncha 
            PayoutMethodDetails details = new PayoutMethodDetails();


            var senderDocument = dbContext.SenderBusinessDocumentation.Where(x => x.SenderId == cashPickUp.SenderId).FirstOrDefault();
            string senderIdentityCardId = "";
            PayoutMethodIdentityCardTypeEnum senderIdentityCardType = PayoutMethodIdentityCardTypeEnum.O;

            if (senderDocument != null)
            {
                senderIdentityCardId = senderDocument.IdentityNumber;

                string SenderIdCardType = dbContext.IdentityCardType.Where(x => x.Id == senderDocument.IdentificationTypeId).Select(x => x.CardType).FirstOrDefault();

                if (SenderIdCardType.ToLower().Contains("national"))
                {
                    senderIdentityCardType = PayoutMethodIdentityCardTypeEnum.NI;
                }
                else if (SenderIdCardType.ToLower().Contains("passport"))
                {
                    senderIdentityCardType = PayoutMethodIdentityCardTypeEnum.PP;
                }


            }
            PayoutMethodGenderEnum senderGender = PayoutMethodGenderEnum.F;
            if (senderDetail.GGender == 0)
            {
                senderGender = PayoutMethodGenderEnum.M;
            }

            if (receivingCurrency == "MAD")
            {

                string IdCardType = dbContext.IdentityCardType.Where(x => x.Id == cashPickUp.RecipientIdentityCardId).Select(x => x.CardType).FirstOrDefault();
                PayoutMethodIdentityCardTypeEnum cartTypeEnum = PayoutMethodIdentityCardTypeEnum.O;
                if (IdCardType != null)
                {
                    if (IdCardType.ToLower().Contains("national"))
                    {
                        cartTypeEnum = PayoutMethodIdentityCardTypeEnum.NI;
                    }
                    else if (IdCardType.ToLower().Contains("passport"))
                    {
                        cartTypeEnum = PayoutMethodIdentityCardTypeEnum.PP;
                    }
                }

                details = new PayoutMethodDetails(
                     firstName: FirstName,
                     lastName: LastName,
                      //mobileProvider: PayoutMethodMobileProviderEnum.Orange,
                      phoneNumber: receiverPhoneCode + Receiver.MobileNo,
                     //phoneNumber: "212625151552",
                     //bankAccount: transferSummary.BankAccountDeposit.AccountNumber,
                     //bankCode: transferSummary.BankAccountDeposit.BranchCode,
                     //bankAccountType: PayoutMethodBankAccountTypeEnum._20,
                     //reason: cashPickUp.Reason.ToString(),
                     identityCardType: cartTypeEnum,
                     identityCardId: cashPickUp.RecipientIdenityCardNumber,
                      senderCountryOfBirth: senderDetail.Country,
                      senderGender: senderGender,
                      senderCityOfBirth: senderDetail.City,
                      senderIdentityCardId: senderIdentityCardId,
                      senderIdentityCardType: senderIdentityCardType
                     );
            }
            if (receivingCurrency == "XOF")
            {
                details = new PayoutMethodDetails(
                     firstName: FirstName.Trim(),
                     lastName: LastName.Trim(),
                     //mobileProvider: PayoutMethodMobileProviderEnum.Orange,
                     phoneNumber: Receiver.MobileNo

                     //bankAccount: transferSummary.BankAccountDeposit.AccountNumber,
                     //bankCode: transferSummary.BankAccountDeposit.BranchCode,
                     //bankAccountType: PayoutMethodBankAccountTypeEnum._20

                     );
            }
            else
            {
                details = new PayoutMethodDetails(
                     firstName: FirstName,
                     lastName: LastName,
                     //mobileProvider: PayoutMethodMobileProviderEnum.Orange,
                     phoneNumber: receiverPhoneCode + Receiver.MobileNo
                     //bankAccount: transferSummary.BankAccountDeposit.AccountNumber,
                     //bankCode: transferSummary.BankAccountDeposit.BranchCode,
                     //bankAccountType: PayoutMethodBankAccountTypeEnum._20

                     );
            }
            PayoutMethod payout = new PayoutMethod(
              type: receivingCurrency + "::Cash",
              details: details);

            Recipient recipient = new Recipient(
              requestedAmount: cashPickUp.ReceivingAmount,
              requestedCurrency: receivingCurrency,
              payoutMethod: payout);

            //Recipient recipient = new Recipient(
            //  requestedAmount: transferSummary.KiiPayTransferPaymentSummary.ReceivingAmount,
            //  requestedCurrency: receivingCurrency,
            //  payoutMethod: payout);


            Transaction transaction = new Transaction(
              sender: sender,
              recipients: new List<Recipient>() { recipient },
              inputCurrency: sendingCurrency,
              externalId: cashPickUp.ReceiptNumber);
            TransactionRequest request = new TransactionRequest(
                transaction: transaction);
            TransferZeroApi transferZeroApi = new TransferZeroApi();
            try
            {
                TransactionError transactionError = new TransactionError()
                {
                    ReceiptNo = cashPickUp.ReceiptNumber,
                    TransactionId = cashPickUp.Id,
                    TransferMethod = TransactionTransferMethod.CashPickUp,
                };
                transferZeroApi.transactionError = transactionError;
                var result = transferZeroApi.CreateTransaction(request);


                var resultStatus = transferZeroApi.GetTransactionStatus(result.Object.ExternalId);
                return resultStatus;

            }
            catch (Exception ex)
            {

                Log.Write("Cashpickuperror" + ex.Message, ErrorType.TransferZero);

            }

            return new Transaction();
        }


        public BankDepositResponseVm PrepareCashPickUpTransactionTransferZeroResponse(Transaction transaction)
        {
            BankDepositResponseVm bankDepositResponse = new BankDepositResponseVm()
            {
                extraResult = transaction.ExternalId,
                status = true,
                response = (int)transaction.State,
                result = new BankDepositResponseResult()
                {

                    transactionStatus = (int)transaction.State,
                    transactionReference = transaction.ExternalId,
                    transactiondate = transaction.CreatedAt == null ? "" : transaction.CreatedAt.GetValueOrDefault().ToString("dd/MM/yyyy")
                }

            };
            return bankDepositResponse;

        }


        public static FaxingStatus GetTransferZeroTransactionStatus(Transaction transactionState)
        {
            FaxingStatus status;
            var transcationStatus = transactionState.State;

            if (transcationStatus == TransactionState.Initial || transcationStatus == TransactionState.Approved
                || transcationStatus == TransactionState.Pending || transcationStatus == TransactionState.Received
                || transcationStatus == TransactionState.Processing)
            {
                return FaxingStatus.NotReceived;
            }
            if (transcationStatus == TransactionState.Paid)
            {
                return FaxingStatus.Completed;
            }
            if (transcationStatus == TransactionState.Canceled)
            {
                return FaxingStatus.Cancel;
            }
            if (transcationStatus == TransactionState.Exception)
            {
                return FaxingStatus.Failed;
            }
            else
            {

                return FaxingStatus.NotReceived;
            }

            return status;

        }
        public void AddResponseLog(BankDepositResponseVm cashPickUpTransactionResult, int transactionId)
        {

            try
            {

                CashPickUpResponseStatus cashPickUpResponseStatus = new CashPickUpResponseStatus()
                {
                    extraResult = cashPickUpTransactionResult.extraResult,
                    message = cashPickUpTransactionResult.message,
                    response = cashPickUpTransactionResult.response,
                    status = cashPickUpTransactionResult.status,
                    TransactionDateTime = DateTime.Now,
                    TransactionId = transactionId
                };

                var result_cashPickUpResponseStatus = dbContext.CashPickUpResponseStatus.Add(cashPickUpResponseStatus);
                dbContext.SaveChanges();

                CashPickUpTransactionResponseResult cashPickUpTransactionResponseResult = new CashPickUpTransactionResponseResult()
                {

                    amountInBaseCurrency = cashPickUpTransactionResult.result.amountInBaseCurrency,
                    CashPickUpResponseStatusId = result_cashPickUpResponseStatus.Id,
                    beneficiaryBankCode = cashPickUpTransactionResult.result.beneficiaryBankCode,
                    beneficiaryAccountNumber = cashPickUpTransactionResult.result.beneficiaryAccountNumber,
                    beneficiaryAccountName = cashPickUpTransactionResult.result.beneficiaryAccountName,
                    baseCurrencyCode = cashPickUpTransactionResult.result.baseCurrencyCode,
                    errorCode = cashPickUpTransactionResult.result.errorCode,
                    errorDescription = cashPickUpTransactionResult.result.errorDescription,
                    partnerTransactionReference = cashPickUpTransactionResult.result.partnerTransactionReference,
                    payername = cashPickUpTransactionResult.result.payername,
                    paymentAmount = cashPickUpTransactionResult.result.paymentAmount,
                    processorGateway = cashPickUpTransactionResult.result.processorGateway,
                    retriedCount = cashPickUpTransactionResult.result.retriedCount,
                    senderName = cashPickUpTransactionResult.result.senderName,
                    sourceAmount = cashPickUpTransactionResult.result.sourceAmount,
                    targetAmount = cashPickUpTransactionResult.result.targetAmount,
                    targetCurrencyCode = cashPickUpTransactionResult.result.targetCurrencyCode,
                    transactioncharge = cashPickUpTransactionResult.result.transactioncharge,
                    transactiondate = cashPickUpTransactionResult.result.transactiondate,
                    transactionReference = cashPickUpTransactionResult.result.transactionReference,
                    transactionStatus = cashPickUpTransactionResult.result.transactionStatus,
                    transactionStatusDescription = cashPickUpTransactionResult.result.transactionStatusDescription,

                };
                var result_cashPickUpTransactionResponseResult = dbContext.CashPickUpTransactionResponseResult.Add(cashPickUpTransactionResponseResult);
                dbContext.SaveChanges();

            }
            catch (Exception ex)
            {

                Log.Write("cash pick up Api " + DateTime.Now + " " + "Transaction Cannot added " + transactionId);
            }

        }


        public CashPickUpTransactionResponseResult GetCashPickupTransactionReponse(string transactionRef)
        {
            var cashPickUpTransactionResponseResult = dbContext.CashPickUpTransactionResponseResult.Where(x => x.transactionReference == transactionRef).FirstOrDefault();
            return cashPickUpTransactionResponseResult;
        }
        public bool UpdateTransferZeroTransactionStatus(CashPickUpTransactionResponseResult cashPickUpTransactionResponse)
        {
            dbContext.Entry(cashPickUpTransactionResponse).State = System.Data.Entity.EntityState.Modified;
            var cashPickUpStatus = dbContext.CashPickUpResponseStatus.
            Where(x => x.Id == cashPickUpTransactionResponse.CashPickUpResponseStatusId).FirstOrDefault();
            var cashPickup = dbContext.FaxingNonCardTransaction.Where(x => x.Id == cashPickUpStatus.TransactionId).FirstOrDefault();
            switch ((TransferZero.Sdk.Model.TransactionState)cashPickUpTransactionResponse.transactionStatus)
            {
                case TransferZero.Sdk.Model.TransactionState.Initial:
                    cashPickup.FaxingStatus = FaxingStatus.NotReceived;
                    break;
                case TransferZero.Sdk.Model.TransactionState.Approved:
                    cashPickup.FaxingStatus = FaxingStatus.NotReceived;
                    break;
                case TransferZero.Sdk.Model.TransactionState.Pending:
                    cashPickup.FaxingStatus = FaxingStatus.NotReceived;
                    break;
                case TransferZero.Sdk.Model.TransactionState.Received:
                    cashPickup.FaxingStatus = FaxingStatus.Received;
                    break;
                case TransferZero.Sdk.Model.TransactionState.Mispaid:
                    cashPickup.FaxingStatus = FaxingStatus.Failed;
                    break;
                case TransferZero.Sdk.Model.TransactionState.Manual:
                    cashPickup.FaxingStatus = FaxingStatus.Completed;
                    break;
                case TransferZero.Sdk.Model.TransactionState.Paid:
                    cashPickup.FaxingStatus = FaxingStatus.Received;
                    break;
                case TransferZero.Sdk.Model.TransactionState.Canceled:
                    cashPickup.FaxingStatus = FaxingStatus.Cancel;
                    break;
                case TransferZero.Sdk.Model.TransactionState.Refunded:
                    break;
                case TransferZero.Sdk.Model.TransactionState.Processing:
                    cashPickup.FaxingStatus = FaxingStatus.NotReceived;
                    break;
                case TransferZero.Sdk.Model.TransactionState.Exception:
                    cashPickup.FaxingStatus = FaxingStatus.Failed;
                    break;
                default:
                    break;
            }
            dbContext.Entry(cashPickup).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;
        }
    }

    public class CashPickUpTransactionApiResponse
    {
        public FaxingNonCardTransaction CashPickUp { get; set; }
        public BankDepositResponseVm BankDepositApiResponseVm { get; set; }

    }

}