using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.BankApi;
using FAXER.PORTAL.BankApi.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.MoblieTransferApi.Models;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Models.PaymentSummary;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TransferZero.Sdk.Model;
using static FAXER.PORTAL.BankApi.Models.FlutterWaveViewModel;

namespace FAXER.PORTAL.Services
{
    public class SSenderMobileMoneyTransfer
    {
        DB.FAXEREntities dbContext = null;
        SSenderKiiPayWalletTransfer _senderKiiPayServices = null;
        public SSenderMobileMoneyTransfer()
        {
            dbContext = new DB.FAXEREntities();
            _senderKiiPayServices = new SSenderKiiPayWalletTransfer();
        }
        public SSenderMobileMoneyTransfer(DB.FAXEREntities db)
        {
            dbContext = db;
            _senderKiiPayServices = new SSenderKiiPayWalletTransfer();
        }


        #region International transfer
        public ServiceResult<MobileMoneyTransfer> Add(MobileMoneyTransfer model)
        {
            dbContext.MobileMoneyTransfer.Add(model);
            dbContext.SaveChanges();
            //#region Notification Section 

            //var MobileMoney = dbContext.FaxerInformation.Where(x => x.Id ==model.SenderId).FirstOrDefault();
            //DB.Notification notification = new DB.Notification()
            //{
            //    SenderId = (int)model.PayingStaffId,
            //    ReceiverId = MobileMoney.Id,
            //    Amount = Common.Common.GetCountryCurrency(Common.Common.GetCountryCodeByCountryName(model.SendingCountry)) + " " + model.SendingAmount,
            //    CreationDate = DateTime.Now,
            //    Title = DB.Title.MobileMoneyTransfer,
            //    Message = "Wallet No :" + MobileMoney.PhoneNumber + "transfer successful",
            //    NotificationReceiver = DB.NotificationFor.Sender,
            //    NotificationSender = DB.NotificationFor.Agent,
            //    Name = MobileMoney.FirstName
            //};

            //SenderCommonServices senderCommonServices = new SenderCommonServices();
            //senderCommonServices.SendNotification(notification);
            //#endregion
            return new ServiceResult<MobileMoneyTransfer>()
            {
                Data = model,
                Message = "Save",
                Status = ResultStatus.OK
            };
        }


        public ServiceResult<MobileMoneyTransfer> Update(MobileMoneyTransfer model)
        {
            dbContext.Entry<MobileMoneyTransfer>(model).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

            return new ServiceResult<MobileMoneyTransfer>()
            {
                Data = model,
                Message = "",
                Status = ResultStatus.OK
            };
        }
        public ServiceResult<FaxerInformation> AddSender(FaxerInformation model)
        {
            dbContext.FaxerInformation.Add(model);
            dbContext.SaveChanges();

            return new ServiceResult<FaxerInformation>()
            {
                Data = model,
                Message = "",
                Status = ResultStatus.OK
            };
        }
        public List<DropDownViewModel> GetRecentlyPaidNumbers(int SenderId, DB.Module module, int WalletId = 0)
        {
            var result = (from c in dbContext.MobileMoneyTransfer.Where(x => x.SenderId == SenderId
                          && x.PaidFromModule == module && x.WalletOperatorId == WalletId)
                          select new DropDownViewModel()
                          {
                              Id = c.RecipientId,
                              Code = c.PaidToMobileNo,
                              Name = c.PaidToMobileNo + " ( " + c.ReceiverName + " ) ",
                              CountryCode = c.ReceivingCountry.ToUpper()
                          }).GroupBy(x => x.Name).Select(x => x.FirstOrDefault()).ToList();
            return result;
        }
        public List<DropDownViewModel> GetRecentlyPaidNumbersForAgent(int AgentId, DB.Module module, string Country)
        {
            var result = (from c in dbContext.MobileMoneyTransfer.Where(x => x.PayingStaffId == AgentId && x.PaidFromModule == module && x.ReceivingCountry == Country)
                          select new DropDownViewModel()
                          {
                              Code = c.PaidToMobileNo,
                              Name = c.PaidToMobileNo
                          }).GroupBy(x => x.Name).Select(x => x.FirstOrDefault()).ToList();
            return result;
        }

        public List<DropDownViewModel> GetWallets()
        {
            var list = (from c in dbContext.MobileWalletOperator.ToList()
                        select new DropDownViewModel()
                        {
                            Id = c.Id,
                            Name = c.Name,
                            CountryCode = c.Country.ToUpper()
                        }).ToList();
            return list;

        }

        public ServiceResult<IQueryable<MobileMoneyTransfer>> list()
        {
            return new ServiceResult<IQueryable<MobileMoneyTransfer>>()
            {
                Data = dbContext.MobileMoneyTransfer,
                Message = "",
                Status = ResultStatus.OK

            };
        }
        public void SetSenderMobileMoneyTransfer(SenderMobileMoneyTransferVM vm)
        {

            Common.FaxerSession.SenderMobileMoneyTransfer = vm;

        }

        public SenderMobileMoneyTransferVM GetSenderMobileMoneyTransfer()
        {

            SenderMobileMoneyTransferVM vm = new SenderMobileMoneyTransferVM();

            if (Common.FaxerSession.SenderMobileMoneyTransfer != null)
            {

                vm = Common.FaxerSession.SenderMobileMoneyTransfer;
            }
            return vm;
        }



        public SenderMobileMoneyTransferVM GetInformationFromMobileNo(string MobileNo, int id)
        {

            var data = (from c in dbContext.MobileMoneyTransfer.Where(x => x.PaidToMobileNo == MobileNo && x.Id == id).ToList()

                        select new SenderMobileMoneyTransferVM()
                        {

                            CountryCode = c.ReceivingCountry,
                            CountryPhoneCode = Common.Common.GetCountryPhoneCode(c.ReceivingCountry),
                            RecentlyPaidMobile = c.PaidToMobileNo,
                            ReceiverName = c.ReceiverName,
                            MobileNumber = c.PaidToMobileNo,
                            WalletId = c.WalletOperatorId,


                        }).FirstOrDefault();




            return data;
        }

        public MobileMoneyTransfer GetMobileInformationWithMoblieNoAndId(string MobileNo, int Id)
        {
            var data = dbContext.MobileMoneyTransfer.Where(x => x.PaidToMobileNo == MobileNo && x.Id == Id).FirstOrDefault();
            return data;
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

            if (Common.FaxerSession.IsTransferFromHomePage == true || Common.FaxerSession.IsCommonEstimationPage)
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

        public SenderMobileEnrterAmountVm GetSenderMobileEnrterAmount()
        {

            SenderMobileEnrterAmountVm vm = new SenderMobileEnrterAmountVm();

            if (Common.FaxerSession.SenderMobileEnrterAmount != null)
            {

                vm = Common.FaxerSession.SenderMobileEnrterAmount;
            }
            return vm;
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

        public void SetPaymentMethod(PaymentMethodViewModel vm)
        {


            Common.FaxerSession.PaymentMethodViewModel = vm;
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

        #endregion



        public SenderLocalEnterAmountVM GetSenderLocalEnterAmount()
        {

            SenderLocalEnterAmountVM vm = new SenderLocalEnterAmountVM();

            if (Common.FaxerSession.SenderLocalEnterAmount != null)
            {

                vm = Common.FaxerSession.SenderLocalEnterAmount;
            }
            return vm;
        }


        public void SetSenderLocalEnterAmount(SenderLocalEnterAmountVM vm)
        {

            Common.FaxerSession.SenderLocalEnterAmount = vm;
        }


        public SenderAccountPaymentSummaryViewModel GetSenderTransferSummaryVm()
        {

            SenderAccountPaymentSummaryViewModel vm = new SenderAccountPaymentSummaryViewModel();

            if (Common.FaxerSession.SenderAccountPaymentSummary != null)
            {

                vm = Common.FaxerSession.SenderAccountPaymentSummary;
            }
            return vm;
        }

        internal void ConfirmMobilePayment(MobileMoneyTransfer data)
        {
            data.Status = MobileMoneyTransferStatus.Paid;
            dbContext.Entry(data).State = EntityState.Modified;
            dbContext.SaveChanges();
            SendEmailAndSms(data);
        }

        public void SetSenderTransferSummaryVm(SenderAccountPaymentSummaryViewModel vm)
        {

            Common.FaxerSession.SenderAccountPaymentSummary = vm;
        }

        public MobileMoneyTransfer MobileMoneyTransferInfo(int transactionId)
        {
            var mobileMoneyTransferInfo = dbContext.MobileMoneyTransfer.Where(x => x.Id == transactionId).FirstOrDefault();
            return mobileMoneyTransferInfo;
        }

        internal SenderAndReceiverDetialVM GetSenderAndReceiverDetails()
        {
            SCashPickUpTransferService _cashPickUp = new SCashPickUpTransferService();
            SAgentMobileTransferWallet _sAgentMobileTransferWalletServices = new SAgentMobileTransferWallet();


            var senderInfo = _cashPickUp.GetCashPickupInformationViewModel();

            var ReceiverInfo = _sAgentMobileTransferWalletServices.GetReceiverDetailsInformation();
            SenderAndReceiverDetialVM vm = new SenderAndReceiverDetialVM()
            {
                ReceiverCountry = ReceiverInfo.Country,
                ReceiverMobileNo = ReceiverInfo.MobileNumber,
                SenderCountry = senderInfo.Country,
                SenderId = senderInfo.Id,

            };
            return vm;
        }

        internal void TransactionCancelledEmail(MobileMoneyTransfer item)
        {
            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);

            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string bankName = "";
            string ReceiverCountryName = Common.Common.GetCountryName(item.ReceivingCountry);
            string SendingCurrency = Common.Common.GetCountryName(item.SendingCountry);
            string ReceivingCurrency = Common.Common.GetCountryName(item.ReceivingCountry);
            string WalletName = dbContext.MobileWalletOperator.Where(x => x.Id == item.WalletOperatorId).Select(x => x.Name).FirstOrDefault();
            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/TransferedCancelledByTZ?" +
                  "&SenderFristName=" + senderInfo.FirstName +
                  "&TransactionNumber=" + item.ReceiptNo +
                  "&RecipentName=" + item.ReceiverName +
                  "&BankName=" + bankName +
                  "&BankAccount=" + "" +
                  "&ReceiverCountry=" + ReceiverCountryName +
                  "&transferMethod=" + TransactionTransferMethod.OtherWallet +
                  "&WalletName=" + WalletName +
                  "&MobileNo=" + item.PaidToMobileNo
                  );
            mail.SendMail(senderInfo.Email, "Money Transfer Cancelled" + " " + item.ReceiptNo, body);
        }

        internal void TransactionCancelledByMoneyFexEmail(MobileMoneyTransfer item)
        {
            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);

            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string bankName = "";
            string ReceiverCountryName = Common.Common.GetCountryName(item.ReceivingCountry);

            string SendingCurrency = Common.Common.GetCountryName(item.SendingCountry);
            string ReceivingCurrency = Common.Common.GetCountryName(item.ReceivingCountry);
            string WalletName = dbContext.MobileWalletOperator.Where(x => x.Id == item.WalletOperatorId).Select(x => x.Name).FirstOrDefault();
            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/TransactionCancelledByMoneyFex?" +
                 "&SenderFristName=" + senderInfo.FirstName +
                 "&TransactionNumber=" + item.ReceiptNo +
                 "&RecipentName=" + item.ReceiverName +
                 "&BankName=" + bankName +
                 "&BankAccount=" + "" +
                 "&ReceiverCountry=" + ReceiverCountryName +
                 "&receivingCurrency=" + ReceivingCurrency +
                 "&sendingCurrency=" + SendingCurrency +
                 "&exchangeRate=" + item.ExchangeRate +
                 "&fee=" + item.Fee +
                 "&receivingAmount=" + item.ReceivingAmount +
                 "&sendingAmount=" + item.SendingAmount +
                 "&bankCode=" + "" +
                 "&transferMethod=" + TransactionTransferMethod.OtherWallet +
                 "&walletName=" + WalletName +
                 "&mobileNo=" + item.PaidToMobileNo
                 );

            mail.SendMail(senderInfo.Email, "Money Transfer Cancelled" + " " + item.ReceiptNo, body);

        }

        internal MobileMoneyTransfer GetOtherWalletInfoByReceiptNo(string receiptNo)
        {
            var otherWalletData = dbContext.MobileMoneyTransfer.Where(x => x.ReceiptNo == receiptNo).FirstOrDefault();
            return otherWalletData;
        }

        internal void CancelOtherWalletTransaction(string receiptNo)
        {
            var data = GetOtherWalletInfoByReceiptNo(receiptNo);
            data.Status = MobileMoneyTransferStatus.Cancel;
            dbContext.Entry<MobileMoneyTransfer>(data).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        internal SenderMobileMoneyTransferVM GetMobileMoneyTransferDetails(MobileMoneyTransfer vm)
        {
            SenderMobileMoneyTransferVM model = new SenderMobileMoneyTransferVM()
            {
                CountryCode = vm.ReceivingCountry,
                MobileNumber = vm.PaidToMobileNo,
                ReceiverName = vm.ReceiverName,
            };

            return model;

        }

        internal KiiPayTransferPaymentSummary GetKiiPayTransferPaymentSummary(MobileMoneyTransfer mobileTransferData)
        {
            KiiPayTransferPaymentSummary model = new KiiPayTransferPaymentSummary()
            {
                ReceivingAmount = mobileTransferData.ReceivingAmount
            };
            return model;
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

        internal MobileMoneyTransfer SaveIncompleteTransaction()
        {

            var paymentInfo = GetSenderMobileEnrterAmount();
            var mobileTransfer = GetSenderMobileMoneyTransfer();
            Common.FaxerSession.ReceiptNo = Common.Common.GenerateMobileMoneyTransferReceiptNo(6);
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
                    MobileWalletProvider = mobileTransfer.WalletId,
                    Email = mobileTransfer.ReceiverEmail,
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
                ExchangeRate = paymentInfo.ExchangeRate,
                Fee = paymentInfo.Fee,
                PaidFromModule = Module.Faxer,
                TransactionDate = DateTime.Now,
                TotalAmount = paymentInfo.TotalAmount,
                PaidToMobileNo = mobileTransfer.MobileNumber,
                PaymentReference = paymentInfo.PaymentReference,
                ReceivingAmount = paymentInfo.ReceivingAmount,
                SenderId = Common.FaxerSession.LoggedUser.Id,
                ReceivingCountry = mobileTransfer.CountryCode,
                SendingAmount = paymentInfo.SendingAmount,
                SendingCountry = Common.FaxerSession.LoggedUser.CountryCode,
                ReceiptNo = Common.FaxerSession.ReceiptNo,
                ReceiverName = mobileTransfer.ReceiverName,
                WalletOperatorId = mobileTransfer.WalletId,
                RecipientId = RecipientId,
                Status = MobileMoneyTransferStatus.PaymentPending,
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


            var data = dbContext.MobileMoneyTransfer.Where(x => x.Id == Common.FaxerSession.TransactionId).FirstOrDefault();
            if (data != null && data.Status == MobileMoneyTransferStatus.PaymentPending
                && Common.FaxerSession.TransactionId != null && Common.FaxerSession.TransactionId > 0)
            {

                data.ExchangeRate = paymentInfo.ExchangeRate;
                data.Fee = paymentInfo.Fee;
                data.PaidFromModule = Module.Faxer;
                data.TransactionDate = DateTime.Now;
                data.TotalAmount = paymentInfo.TotalAmount;
                data.PaidToMobileNo = mobileTransfer.MobileNumber;
                data.PaymentReference = paymentInfo.PaymentReference;
                data.ReceivingAmount = paymentInfo.ReceivingAmount;
                data.SenderId = Common.FaxerSession.LoggedUser.Id;
                data.ReceivingCountry = mobileTransfer.CountryCode;
                data.SendingAmount = paymentInfo.SendingAmount;
                data.SendingCountry = Common.FaxerSession.LoggedUser.CountryCode;
                data.ReceiptNo = Common.FaxerSession.ReceiptNo;
                data.ReceiverName = mobileTransfer.ReceiverName;
                data.WalletOperatorId = mobileTransfer.WalletId;
                data.RecipientId = RecipientId;
                data.Status = MobileMoneyTransferStatus.PaymentPending;
                data.SendingCurrency = paymentInfo.SendingCurrencyCode;
                data.ReceivingCurrency = paymentInfo.ReceivingCurrencyCode;
                dbContext.Entry(data).State = EntityState.Modified;
                dbContext.SaveChanges();
                return data;

            }
            else
            {

                dbContext.MobileMoneyTransfer.Add(mobileTransferData);
                dbContext.SaveChanges();
            }


            return mobileTransferData;
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
                TotalAmount = paymentInfo.TotalAmount

            };
            SetSenderMobileEnrterAmount(vm);

            SenderMobileMoneyTransferVM model = new SenderMobileMoneyTransferVM()
            {
                CountryCode = data.Country,
                MobileNumber = data.MobileNo,
                ReceiverName = data.ReceiverName,
                WalletId = data.MobileWalletProvider,
                CountryPhoneCode = Common.Common.GetCountryPhoneCode(data.Country)
            };
            SetSenderMobileMoneyTransfer(model);
        }

        internal bool RepeatTransaction(int TransactionId, int RecipientId)
        {
            string ReceivingCountry = "";
            string ReceiverMobileNo = "";
            decimal SendingAmount = 0;
            string ErrorMessage = "";

            if (TransactionId != 0)
            {
                var model = dbContext.MobileMoneyTransfer.Where(x => x.Id == TransactionId).FirstOrDefault();
                ReceivingCountry = model.ReceivingCountry;
                ReceiverMobileNo = model.PaidToMobileNo;
                SendingAmount = model.SendingAmount;
            }

            if (RecipientId != 0)
            {
                var model = dbContext.Recipients.Where(x => x.Id == RecipientId).FirstOrDefault();
                var sendingAmountData = GetSenderMobileEnrterAmount();
                ReceivingCountry = model.Country;
                ReceiverMobileNo = model.MobileNo;
                SendingAmount = sendingAmountData.SendingAmount;

            }
            bool IsValidReceiver = Common.Common.IsValidBankDepositReceiver(ReceiverMobileNo, Service.MobileWallet);

            if (IsValidReceiver == false)
            {
                ErrorMessage = "Receiver is banned";
                Common.FaxerSession.ErrorMessage = ErrorMessage;

                return false;
            }

            string CountryPhoneCode = Common.Common.GetCountryPhoneCode(ReceivingCountry);
            var Apiservice = Common.Common.GetApiservice(Common.FaxerSession.LoggedUser.CountryCode, ReceivingCountry,
                   SendingAmount, TransactionTransferMethod.OtherWallet, TransactionTransferType.Online);
            if (Apiservice == null)
            {
                ErrorMessage = "Service Not Avialable";
                Common.FaxerSession.ErrorMessage = ErrorMessage;

                return false;

            }
            switch (Apiservice)
            {
                case DB.Apiservice.MTN:

                    FAXER.PORTAL.MoblieTransferApi.MobileTransferApi mobileTransferApiServices = new FAXER.PORTAL.MoblieTransferApi.MobileTransferApi();

                    MobileTransferApiConfigurationVm configurationVm = new MobileTransferApiConfigurationVm()
                    {
                        apirefId = Common.Common.GetNewRefIdForMobileTransfer(),
                        apiKey = "",
                        apiUrl = "",
                        subscriptionKey = "9277bd87e874418e928cfdba3032b423"
                        //subscriptionKey = Common.Common.GetAppSettingValue("MTNApiSubscriptionKey")
                    };

                    var accesstoken = mobileTransferApiServices.Login<MobileTransferAccessTokeneResponse>(configurationVm);

                    MobileTransferAccessTokeneResponse tokenModel = new MobileTransferAccessTokeneResponse();
                    tokenModel = accesstoken.Result;
                    tokenModel.apirefId = configurationVm.apirefId;
                    tokenModel.apiKey = configurationVm.apiKey;
                    tokenModel.apiUrl = "";
                    tokenModel.subscriptionKey = configurationVm.subscriptionKey;

                    MobileNoLookUpResponseData mobileNoLookUp = new MobileNoLookUpResponseData()
                    {
                        accountHolderIdType = "MSISDN",
                        accountHolderId = CountryPhoneCode + "" + ReceiverMobileNo
                    };
                    SAgentMobileTransferWallet _sAgentMobileTransferWalletServices = new SAgentMobileTransferWallet();
                    _sAgentMobileTransferWalletServices.SetMobileTransferAccessTokeneResponse(tokenModel);
                    break;

                case DB.Apiservice.TransferZero:
                    TransferZeroApi transferZeroApi = new TransferZeroApi();
                    string[] phoneCode = CountryPhoneCode.Split('+');
                    string receiverPhoneCode = phoneCode[phoneCode.Length - 1];
                    AccountValidationRequest accountValidationRequest = new AccountValidationRequest(
                                    phoneNumber: receiverPhoneCode + ReceiverMobileNo,
                                    country: (AccountValidationRequest.CountryEnum)Common.Common.getAccountValidationCountryCodeForTZ(ReceivingCountry),
                                    currency: (AccountValidationRequest.CurrencyEnum)Common.Common.getAccountValidationCountryCurrencyForTZ(ReceivingCountry),
                                    method: AccountValidationRequest.MethodEnum.Mobile
                     );
                    var result = transferZeroApi.ValidateAccountNo(accountValidationRequest);
                    var IsValidateAccount = false;
                    try
                    {

                        IsValidateAccount = result.Meta == null ? true : false;
                    }
                    catch (Exception)
                    {

                    }
                    if (IsValidateAccount == false)
                    {
                        ErrorMessage = "Invalid Phone Number";
                        Common.FaxerSession.ErrorMessage = ErrorMessage;

                        return false;
                    }
                    break;
                default:
                    break;
            }

            SmsApi smsApi = new SmsApi();
            var IsValidMobileNo = smsApi.IsValidMobileNo(CountryPhoneCode + "" + ReceiverMobileNo);
            if (IsValidMobileNo == false)
            {
                ErrorMessage = "Invalid Phone Number";
                Common.FaxerSession.ErrorMessage = ErrorMessage;

                return false;
            }

            //var CashPckUPId = SaveIncompleteTransaction();
            //Common.FaxerSession.TransactionId = CashPckUPId.Id;
            return true;
        }

        internal bool HasOneCardSaved()
        {

            var data = dbContext.SavedCard.Where(x => x.UserId == FaxerSession.LoggedUser.Id && x.IsDeleted == false).ToList();
            if (data.Count > 0)
            {
                return true;
            }

            return false;
        }

        internal void setAmount(int TransactionId)
        {
            var data = dbContext.MobileMoneyTransfer.Where(x => x.Id == TransactionId).FirstOrDefault();


            var paymentInfo = GetSenderMobileEnrterAmount();
            if (data != null && paymentInfo.ReceivingCountryCode == data.ReceivingCountry)
            {
                SenderMobileEnrterAmountVm vm = new SenderMobileEnrterAmountVm()
                {
                    ExchangeRate = paymentInfo.ExchangeRate,
                    Fee = paymentInfo.Fee,
                    PaymentReference = paymentInfo.PaymentReference,
                    ReceiverId = data.RecipientId,
                    ReceiverName = data.ReceiverName,
                    ReceivingAmount = paymentInfo.ReceivingAmount,
                    ReceivingCountryCode = paymentInfo.ReceivingCountryCode,
                    ReceivingCurrencyCode = paymentInfo.ReceivingCurrencyCode,
                    ReceivingCurrencySymbol = paymentInfo.ReceivingCurrencySymbol,
                    SendingCurrencyCode = data.SendingCurrency,
                    SendingCurrencySymbol = Common.Common.GetCurrencySymbol(data.SendingCountry),
                    SendingAmount = paymentInfo.SendingAmount,
                    SendingCountryCode = data.SendingCountry,
                    TotalAmount = paymentInfo.TotalAmount
                };
                SenderMobileMoneyTransferVM model = new SenderMobileMoneyTransferVM()
                {
                    CountryCode = data.ReceivingCountry,
                    MobileNumber = data.PaidToMobileNo,
                    ReceiverName = data.ReceiverName,
                    WalletId = data.WalletOperatorId,
                };

                SetSenderMobileEnrterAmount(vm);
                SetSenderMobileMoneyTransfer(model);
            }
        }

        internal bool IsValidDeposit(string receiptNo)
        {

            var result = dbContext.MobileMoneyTransfer.Where(x => x.ReceiptNo == receiptNo).FirstOrDefault();
            if (result == null)
            {

                return true;
            }
            return false;
        }
        #region For Staf
        public void SetStaffDebitCreditCardDetail(CreditDebitCardViewModel vm)
        {

            Common.AdminSession.CreditDebitDetails = vm;
        }

        public CreditDebitCardViewModel GetStaffDebitCreditCardDetail()
        {

            CreditDebitCardViewModel vm = new CreditDebitCardViewModel();

            if (Common.AdminSession.CreditDebitDetails != null)
            {

                vm = Common.AdminSession.CreditDebitDetails;
            }
            return vm;
        }

        #endregion


        #region Create Transaction 

        public MobileTransferApiResponse CreateTransactionToApi(MobileMoneyTransfer moneyTransfer, TransactionTransferType transactionTransferType
            = TransactionTransferType.Online)
        {

            if (moneyTransfer.IsComplianceApproved == false && moneyTransfer.PaidFromModule == Module.Faxer)
            {
                if (moneyTransfer.SenderPaymentMode == SenderPaymentMode.CreditDebitCard ||
                    moneyTransfer.SenderPaymentMode == SenderPaymentMode.SavedDebitCreditCard)
                {
                    try
                    {

                        var TransactionLimitAmount = Common.Common.HasExceededAmountLimit(moneyTransfer.SenderId, moneyTransfer.SendingCountry, moneyTransfer.ReceivingCountry, moneyTransfer.TotalAmount, Module.Faxer);

                        if (TransactionLimitAmount)
                        {
                            moneyTransfer.IsComplianceNeededForTrans = true;
                            moneyTransfer.Status = MobileMoneyTransferStatus.Held;
                            Update(moneyTransfer);
                            return new MobileTransferApiResponse()
                            {

                                status = moneyTransfer.Status,
                                response = new MTNCameroonResponseParamVm()
                            };
                        }

                    }
                    catch (Exception)
                    {

                    }

                }

            }
            if (moneyTransfer.PaidFromModule == Module.Faxer || moneyTransfer.PaidFromModule == Module.Agent)
            {
                var IsPayoutFlowControlEnabled = Common.Common.IsPayoutFlowControlEnabled(moneyTransfer.SendingCountry, moneyTransfer.ReceivingCountry,
                             moneyTransfer.Apiservice, TransactionTransferMethod.OtherWallet, moneyTransfer.WalletOperatorId);
                if (IsPayoutFlowControlEnabled == false)
                {
                    moneyTransfer.Status = MobileMoneyTransferStatus.Paused;

                    Update(moneyTransfer);
                    return new MobileTransferApiResponse()
                    {

                        status = moneyTransfer.Status,
                        response = new MTNCameroonResponseParamVm()
                    };

                }
            }
            var MobileMoneyTransactionResult = new MTNCameroonResponseParamVm();

            int agentId = 0;
            if (moneyTransfer.PaidFromModule == Module.Agent)
            {
                agentId = Common.Common.GetAgentIdByPayingId(moneyTransfer.PayingStaffId);
            }
            var ApiService = Common.Common.GetApiservice(moneyTransfer.SendingCountry, moneyTransfer.ReceivingCountry,
           moneyTransfer.SendingAmount, TransactionTransferMethod.OtherWallet, transactionTransferType, agentId);

            
            string ReceiverPhoneCode = Common.Common.GetCountryPhoneCode(moneyTransfer.ReceivingCountry);


            Log.Write("Transaction Intiation ", ErrorType.T365, "Mobile");

            switch (ApiService)
            {
                case DB.Apiservice.MTN:
                    FAXER.PORTAL.MoblieTransferApi.MobileTransferApi mobileTransferApiServices = new FAXER.PORTAL.MoblieTransferApi.MobileTransferApi();
                    try
                    {

                        PayeeInfo payeeInfo = new PayeeInfo()
                        {
                            partyIdType = "MSISDN",
                            //partyId = "46733123454"
                            partyId = (ReceiverPhoneCode.Contains("+") ? ReceiverPhoneCode.Remove(0, 1) : ReceiverPhoneCode)
                        + "" + moneyTransfer.PaidToMobileNo
                        };
                        string mtnamount = moneyTransfer.ReceivingAmount.ToString().Split('.')[0];
                        MTNCameroonRequestParamVm model = new MTNCameroonRequestParamVm()
                        {
                            //amount = moneyTransfer.ReceivingAmount.ToString(),
                            amount = mtnamount,
                            currency = "XAF",
                            externalId = moneyTransfer.ReceiptNo,
                            payerMessage = "Money Transfer",
                            payeeNote = "Money Transfer",
                            payee = payeeInfo
                        };
                        var configModel = new MobileTransferApiConfigurationVm()
                        {
                            apiKey = "",
                            apirefId = Guid.NewGuid().ToString(),
                            apiUrl = "",
                            subscriptionKey = Common.Common.GetAppSettingValue("MTNApiSubscriptionKey")
                        };
                        SAgentMobileTransferWallet _sAgentMobileTransferWalletServices = new SAgentMobileTransferWallet();
                        //var tokenModel = _sAgentMobileTransferWalletServices.GetMobileTransferAccessTokeneResponse();

                        Log.Write("MTNLogin");
                        var tokenModel = mobileTransferApiServices.Login<MobileTransferAccessTokeneResponse>(configModel).Result;
                        tokenModel.apirefId = configModel.apirefId;
                        var postTransaction = mobileTransferApiServices.Post<MTNCameroonResponseParamVm>(mobileTransferApiServices.SerializeObject<MTNCameroonRequestParamVm>(model), tokenModel);
                        var transactionStatus = mobileTransferApiServices.GetTransactionStatus<MTNCameroonResponseParamVm>(tokenModel.apirefId, tokenModel.access_token);
                        _sAgentMobileTransferWalletServices.SetMTNCameroonResponseParamVm(transactionStatus.Result);
                        MobileMoneyTransactionResult = _sAgentMobileTransferWalletServices.GetMTNCameroonResponseParamVm();
                        MobileMoneyTransactionResult.refId = tokenModel.apirefId;
                        var transcationStatus = MobileMoneyTransactionResult.status;
                        moneyTransfer.TransferReference = MobileMoneyTransactionResult.refId;
                        switch (transcationStatus.ToLower())
                        {
                            case "success":
                                moneyTransfer.Status = MobileMoneyTransferStatus.Paid;
                                break;
                            case "failed":
                                moneyTransfer.Status = MobileMoneyTransferStatus.InProgress;
                                break;
                            case "pending":
                                moneyTransfer.Status = MobileMoneyTransferStatus.InProgress;
                                break;
                            case "processing":
                                moneyTransfer.Status = MobileMoneyTransferStatus.InProgress;
                                break;
                            case "fail":
                                moneyTransfer.Status = MobileMoneyTransferStatus.InProgress;
                                break;
                            default:
                                moneyTransfer.Status = MobileMoneyTransferStatus.InProgress;
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        moneyTransfer.Status = MobileMoneyTransferStatus.InProgress;
                        Log.Write(ex.Message, ErrorType.MTN, "CreateTransactionToApi");
                    }
                    break;
                case DB.Apiservice.TransferZero:
                    //string TransactionId = Guid.NewGuid().ToString();
                    string TransactionId = moneyTransfer.ReceiptNo;
                    var transferZeroResponse = GetMobileWalletTransferZeroTransactionResponse(moneyTransfer);
                    var transferZeroTransactionResult = transferZeroResponse;
                    MobileMoneyTransactionResult = PrepareMobileTransactionTransferZeroResponse(transferZeroTransactionResult);
                    MobileMoneyTransactionResult.refId = moneyTransfer.ReceiptNo;
                    MobileMoneyTransactionResult.amount = transferZeroTransactionResult.InputAmount.ToString();
                    moneyTransfer.Status = Common.Common.GetTransferZeroTransactionStatusForMobileWallet(transferZeroResponse);
                    moneyTransfer.TransferReference = transferZeroResponse.Id.ToString();
                    try
                    {
                        moneyTransfer.TransferZeroSenderId = transferZeroResponse.Sender.Id.ToString();

                        PayeeInfo payeeInfo = new PayeeInfo()
                        {
                            partyIdType = " ",
                            //partyId = "46733123454"
                            partyId = (ReceiverPhoneCode.Contains("+") ? ReceiverPhoneCode.Remove(0, 1) : ReceiverPhoneCode)
                        + "" + moneyTransfer.PaidToMobileNo
                        };

                        MobileMoneyTransactionResult = new MTNCameroonResponseParamVm()
                        {
                            amount = moneyTransfer.ReceivingAmount.ToString(),
                            code = moneyTransfer.ReceiptNo,
                            currency = Common.Common.GetCountryCurrency(moneyTransfer.ReceivingCountry),
                            externalId = moneyTransfer.ReceiptNo,
                            status = transferZeroResponse.State.ToString(),
                            financialTransactionId = moneyTransfer.ReceiptNo,
                            refId = moneyTransfer.ReceiptNo,
                            payee = payeeInfo
                        };

                    }
                    catch (Exception ex)
                    {
                        Log.Write(ex.Message, ErrorType.TransferZero, "CreateTransactionToApi");
                    }
                    break;
                case Apiservice.EmergentApi:

                    try
                    {

                        var emergentApiResponse = GetEmergentApiMobilePaymentApiResponse(moneyTransfer);
                        MobileMoneyTransactionResult = PrepareEmergentApiMobilePaymentResponse(emergentApiResponse);
                        MobileMoneyTransactionResult.refId = moneyTransfer.ReceiptNo;
                        MobileMoneyTransactionResult.amount = moneyTransfer.SendingAmount.ToString();
                        moneyTransfer.TransferReference = MobileMoneyTransactionResult.refId;
                        switch (emergentApiResponse.status_code)
                        {
                            case 1: // Success
                                moneyTransfer.Status = MobileMoneyTransferStatus.Paid;
                                break;
                            case 2: // Pending
                                moneyTransfer.Status = MobileMoneyTransferStatus.InProgress;
                                break;
                            case 3: // Expired
                                moneyTransfer.Status = MobileMoneyTransferStatus.Failed;
                                break;
                            case 4: // Reversed
                                moneyTransfer.Status = MobileMoneyTransferStatus.Cancel;
                                break;
                            case 5: //In Clearing
                                moneyTransfer.Status = MobileMoneyTransferStatus.InProgress;
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Write(ex.Message, ErrorType.EmergentApi, "CreateTransactionToApi");
                    }
                    break;
                case Apiservice.Zenith:
                    try
                    {
                        ZenithApi zenithApi = new ZenithApi();
                        var ZenithMobileTransactionResponse = zenithApi.CreateMobileMoneyTransaction(moneyTransfer);
                        MobileMoneyTransactionResult = new MTNCameroonResponseParamVm()
                        {
                            amount = moneyTransfer.ReceivingAmount.ToString(),
                            code = moneyTransfer.ReceiptNo,
                            currency = Common.Common.GetCountryCurrency(moneyTransfer.ReceivingCountry),
                            externalId = ZenithMobileTransactionResponse.TransRefNo,
                            status = ZenithMobileTransactionResponse.Status.ToString(),
                            financialTransactionId = ZenithMobileTransactionResponse.TransRefNo,
                            refId = ZenithMobileTransactionResponse.TransRefNo,
                        };
                        moneyTransfer.Status = ZenithMobileTransactionResponse.Status;
                        moneyTransfer.TransferReference = ZenithMobileTransactionResponse.TransRefNo;

                    }
                    catch (Exception ex)
                    {
                        Log.Write(ex.Message, ErrorType.Zenith, "CreateTransactionToApi");
                    }
                    break;
                case Apiservice.CashPot:
                    try
                    {
                        var ExchangeRate = GetRateCashPot(moneyTransfer);
                        if (ExchangeRate != 0)
                        {
                            moneyTransfer.ExchangeRate = ExchangeRate;
                        }
                        var postResponse = GetMobileTransferCashPotTransactionResponse(moneyTransfer);
                        var statusResonse = GetStatusResponse(postResponse.REFERENCE_CODE);
                        moneyTransfer.Status = GetCashPotTransferTransactionStatus(statusResonse.STATUS_CODE);


                        MobileMoneyTransactionResult = new MTNCameroonResponseParamVm()
                        {
                            amount = moneyTransfer.ReceivingAmount.ToString(),
                            code = moneyTransfer.ReceiptNo,
                            currency = Common.Common.GetCountryCurrency(moneyTransfer.ReceivingCountry),
                            externalId = moneyTransfer.ReceiptNo,
                            status = statusResonse.STATUS_CODE,
                            financialTransactionId = moneyTransfer.ReceiptNo,
                            refId = moneyTransfer.ReceiptNo,
                        };
                    }
                    catch (Exception ex)
                    {
                        Log.Write(ex.Message, ErrorType.CashPot, "CreateBankTransactionToApi");
                    }
                    break;
                case Apiservice.FlutterWave:

                    try
                    {
                        SSenderForAllTransfer sSenderForAllTransfer = new SSenderForAllTransfer();
                        var flutterCommonCustomerDetailsVm = BindToFlutterCommonCustomerDetailsVm(moneyTransfer);
                        var postResponse = sSenderForAllTransfer.CreateFlutterWaveTransaction(flutterCommonCustomerDetailsVm);
                        if (postResponse.data != null)
                        {
                            //var validateTransactionResponse = sSenderForAllTransfer.ValidateFlutterWaveTransaction(postResponse.data.flwRef, otp);
                            //var statusResponse = sSenderForAllTransfer.GetFlutterWaveStatus(postResponse.data.txRef);
                            var getTrasactionById = sSenderForAllTransfer.GetFlutterWaveTransactionById(postResponse.data.id);
                            moneyTransfer.Status = GetMobileTransferStatusFromFWResponse(getTrasactionById.data.status);

                            MobileMoneyTransactionResult = new MTNCameroonResponseParamVm()
                            {
                                amount = moneyTransfer.ReceivingAmount.ToString(),
                                code = moneyTransfer.ReceiptNo,
                                currency = Common.Common.GetCountryCurrency(moneyTransfer.ReceivingCountry),
                                externalId = moneyTransfer.ReceiptNo,
                                status = getTrasactionById.data.status,
                                financialTransactionId = moneyTransfer.ReceiptNo,
                                refId = postResponse.data.id.ToString(),
                            };
                        }

                    }
                    catch (Exception ex)
                    {
                        Log.Write(ex.Message, ErrorType.FlutterWave, "CreateBankTransactionToApi");
                    }
                    break;
                default:
                    break;
            }

            MobileTransferApiResponse transferApiResponse = new MobileTransferApiResponse()
            {

                status = moneyTransfer.Status,
                response = MobileMoneyTransactionResult
            };
            Log.Write(moneyTransfer.ReceiptNo + "Transaction intiated to Api");
            return transferApiResponse;
        }

        private MobileMoneyTransferStatus GetMobileTransferStatusFromFWResponse(string status)
        {
            MobileMoneyTransferStatus moneyTransferStatus = MobileMoneyTransferStatus.InProgress;
            switch (status.ToLower())
            {
                case "successful":
                    moneyTransferStatus = MobileMoneyTransferStatus.Paid;
                    break;
                case "pending":
                    moneyTransferStatus = MobileMoneyTransferStatus.InProgress;
                    break;
                case "failed":
                    moneyTransferStatus = MobileMoneyTransferStatus.Failed;
                    break;
            }
            return moneyTransferStatus;
        }

        private FlutterWaveVm BindToFlutterCommonCustomerDetailsVm(MobileMoneyTransfer item)
        {
            var recipient = dbContext.Recipients.Where(x => x.Id == item.RecipientId).FirstOrDefault();
            var walletname = dbContext.MobileWalletOperator.Where(x => x.Id == item.WalletOperatorId).Select(x => x.Name).FirstOrDefault();
            string[] Name = item.ReceiverName.Split(' ');
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

            FlutterWaveVm vm = new FlutterWaveVm()
            {
                account_bank = walletname,
                account_number = item.PaidToMobileNo,
                amount = item.TotalAmount,
                narration = "Payment for goods purchased",
                currency = item.SendingCurrency,
                reference = item.ReceiptNo,
                callback_url = "",
                debit_currency = item.ReceivingCurrency,
                meta = new FlutterWaveReceiverVM()
                {
                    first_name = FirstName,
                    last_name = LastName,
                    email = recipient.Email,
                    mobile_number = recipient.MobileNo,
                }
            };

            return vm;
        }
        private SendTransGenericResponseVm GetMobileTransferCashPotTransactionResponse(MobileMoneyTransfer item)
        {
            CashPotApi cashPotApi = new CashPotApi();
            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);

            string[] Name = item.ReceiverName.Split(' ');
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
                REFERENCE_CODE = item.ReceiptNo,
                DATE = item.TransactionDate.ToString(),
                TRANSSTATUS = "1",
                SENDING_CURRENCY = Common.Common.GetCountryCurrency(item.SendingCountry),
                RECEIVER_CURENCY = Common.Common.GetCurrencyCode(item.ReceivingCountry),
                RATE = item.ExchangeRate.ToString(),
                FEE = item.Fee.ToString(),
                LOCATION_ID = "",
                PAYER_ID = "88899378",
                SEND_AMOUNT = item.SendingAmount.ToString(),
                SENDER_FIRST_NAME = senderInfo.FirstName,
                SENDER_LAST_NAME = senderInfo.MiddleName != null ? senderInfo.MiddleName + " " + senderInfo.LastName : senderInfo.LastName,
                SENDER_ADDRESS = senderInfo.Address1,
                SENDER_COUNTRY = senderInfo.Country,
                RECEIVER_FIRST_NAME = FirstName,
                RECEIVER_LAST_NAME = LastName,
                RECEIVER_MOBILE_NUMBER = item.PaidToMobileNo,
                RECEIVING_AMOUNT = item.ReceivingAmount.ToString(),
                RECEIVER_ADDRESS = "",
                RECEIVER_CITY = "",
                RECEIVER_COUNTRY = item.ReceivingCountry,
                TRANSACTION_TYPE = "11",//Mobile transfer transfer transaction type = 11
                SECRET_ANSWER = item.ReceiptNo,
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
        private TransactionStatusResposeCashPotVm GetStatusResponse(string referenceCode)
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

        private MobileMoneyTransferStatus GetCashPotTransferTransactionStatus(string StatusCode)
        {
            MobileMoneyTransferStatus status = MobileMoneyTransferStatus.InProgress;
            if (StatusCode == "1" || StatusCode == "2" || StatusCode == "3" ||
              StatusCode == "5" || StatusCode == "13" || StatusCode == "15" ||
              StatusCode == "16" || StatusCode == "17" || StatusCode == "18" ||
              StatusCode == "19" || StatusCode == "20")
            {
                status = MobileMoneyTransferStatus.InProgress;
            }
            else if (StatusCode == "6" || StatusCode == "7")
            {
                status = MobileMoneyTransferStatus.Cancel;
            }
            else if (StatusCode == "10")
            {
                status = MobileMoneyTransferStatus.Paid;
            }
            else if (StatusCode == "14")
            {
                status = MobileMoneyTransferStatus.FullRefund;
            }
            return status;
        }

        private decimal GetRateCashPot(MobileMoneyTransfer item)
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
                TRANS_TYPE_ID = "11",
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

        public TransferZero.Sdk.Model.Transaction GetMobileWalletTransferZeroTransactionResponse(MobileMoneyTransfer moneyTransfer)
        {
            var senderDetail = Common.Common.GetSenderInfo(moneyTransfer.SenderId);

            string receivingCurrency = Common.Common.GetCountryCurrency(moneyTransfer.ReceivingCountry);
            string sendingCurrency = Common.Common.GetCountryCurrency(moneyTransfer.SendingCountry);

            string[] phoneCode = Common.Common.GetCountryPhoneCode(moneyTransfer.ReceivingCountry).Split('+');
            string receiverPhoneCode = "";
            string[] Name = moneyTransfer.ReceiverName.Split(' ');
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
            if (receivingCurrency == "XOF")
            {
                PayoutMethodMobileProviderEnum PayoutMethodMobileProviderEnum = PayoutMethodMobileProviderEnum.Orange;
                var MobileProvider = dbContext.MobileWalletOperator.Where(x => x.Id == moneyTransfer.WalletOperatorId).FirstOrDefault();
                if (MobileProvider != null)
                {
                    if (MobileProvider.Code.ToLower() == "tigo")
                    {
                        PayoutMethodMobileProviderEnum = PayoutMethodMobileProviderEnum.Tigo;
                    }
                }
                details = new PayoutMethodDetails(
                     firstName: FirstName,
                     lastName: LastName,
                     mobileProvider: PayoutMethodMobileProviderEnum,
                     phoneNumber: receiverPhoneCode + moneyTransfer.PaidToMobileNo
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
                     phoneNumber: receiverPhoneCode + moneyTransfer.PaidToMobileNo
                     //bankAccount: transferSummary.BankAccountDeposit.AccountNumber,
                     //bankCode: transferSummary.BankAccountDeposit.BranchCode,
                     //bankAccountType: PayoutMethodBankAccountTypeEnum._20
                     );
            }
            PayoutMethod payout = new PayoutMethod(
              type: receivingCurrency + "::Mobile",
              details: details);

            Recipient recipient = new Recipient(
              requestedAmount: moneyTransfer.ReceivingAmount,
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
              externalId: moneyTransfer.ReceiptNo);
            TransactionRequest request = new TransactionRequest(
                transaction: transaction);
            TransferZeroApi transferZeroApi = new TransferZeroApi();
            TransactionError transactionError = new TransactionError()
            {
                ReceiptNo = moneyTransfer.ReceiptNo,
                TransactionId = moneyTransfer.Id,
                TransferMethod = TransactionTransferMethod.OtherWallet,
            };

            transferZeroApi.transactionError = transactionError;
            var result = transferZeroApi.CreateTransaction(request);

            var resultStatus = transferZeroApi.GetTransactionStatus(result.Object.ExternalId);
            return resultStatus;

        }

        public MTNCameroonResponseParamVm PrepareMobileTransactionTransferZeroResponse(Transaction transaction)
        {


            MTNCameroonResponseParamVm mobileResponse = new MTNCameroonResponseParamVm()
            {
                amount = transaction.PaidAmount.ToString(),
                code = transaction.ExternalId,
                externalId = transaction.ExternalId,
                currency = transaction.InputCurrency,
                status = transaction.State.ToString(),
                reason = "",
                refId = transaction.ExternalId

            };
            return mobileResponse;
        }

        public EmergentApiGetStatusResponseModel GetEmergentApiMobilePaymentApiResponse(MobileMoneyTransfer item)
        {
            var senderDetail = Common.Common.GetSenderInfo(item.SenderId);

            string receivingCurrency = Common.Common.GetCountryCurrency(item.ReceivingCountry);
            string sendingCurrency = Common.Common.GetCountryCurrency(item.SendingCountry);

            string receiverPhoneCode = Common.Common.GetCountryPhoneCode(item.ReceivingCountry);

            EmergentApi emergentApi = new EmergentApi();
            EmergentApiRequestParamModel paramModel = new EmergentApiRequestParamModel()
            {

                transaction_date = "/Date(" + DateTime.Now.ToString("yyyy-MM-dd") + ")/",
                expiry_date = "/Date(" + DateTime.Now.ToString("yyyy-MM-dd") + ")/",
                transaction_type = "local",
                payment_mode = "MMT",
                payee_name = senderDetail.FirstName + " " + senderDetail.LastName,
                payee_email = senderDetail.Email,
                trans_currency = "GHS",
                trans_amount = item.ReceivingAmount,
                merch_trans_ref_no = item.ReceiptNo,
                payee_mobile = senderDetail.PhoneNumber,
                recipient_mobile = receiverPhoneCode + item.PaidToMobileNo,
                recipient_name = item.ReceiverName,
                recipient_mobile_operator = Common.Common.GetMobileWalletInfo(item.WalletOperatorId).Code
            };
            var result = emergentApi.CreateTransaction<EmergentApiTransactionResponseModel>(CommonExtension.SerializeObject(paramModel));

            EmergentApiGetTransactionStatusParamModel statusModel = new EmergentApiGetTransactionStatusParamModel()
            {
                merch_trans_ref_no = paramModel.merch_trans_ref_no
            };
            var status = emergentApi.GetTransactionStatus<EmergentApiGetStatusResponseModel>(CommonExtension.SerializeObject(statusModel));
            return status.Result;
        }

        public MTNCameroonResponseParamVm PrepareEmergentApiMobilePaymentResponse(EmergentApiGetStatusResponseModel emergentApiResponse)
        {
            MTNCameroonResponseParamVm mobileResponse = new MTNCameroonResponseParamVm()
            {
                code = emergentApiResponse.status_code.ToString(),
                externalId = emergentApiResponse.trans_ref_no,
                currency = "GHS",
                status = emergentApiResponse.status_code.ToString(),
                reason = emergentApiResponse.status_message,
                refId = emergentApiResponse.trans_ref_no

            };
            return mobileResponse;

        }

        public ServiceResult<bool> IsValidMobileAccount(SenderMobileMoneyTransferVM model, decimal SendingAmount, string SendingCountry,
            TransactionTransferType transactionTransfer = TransactionTransferType.Online)
        {

            var result = new ServiceResult<bool>();
            result.Data = true;
            var Apiservice = Common.Common.GetApiservice(SendingCountry, model.CountryCode,
                   SendingAmount, TransactionTransferMethod.OtherWallet, TransactionTransferType.Online);

            switch (Apiservice)
            {
                case DB.Apiservice.MTN:

                    FAXER.PORTAL.MoblieTransferApi.MobileTransferApi mobileTransferApiServices = new FAXER.PORTAL.MoblieTransferApi.MobileTransferApi();

                    MobileTransferApiConfigurationVm configurationVm = new MobileTransferApiConfigurationVm()
                    {
                        apirefId = Common.Common.GetNewRefIdForMobileTransfer(),
                        apiKey = "",
                        apiUrl = "",
                        //subscriptionKey = "9277bd87e874418e928cfdba3032b423"
                        subscriptionKey = Common.Common.GetAppSettingValue("MTNApiSubscriptionKey")
                    };

                    var accesstoken = mobileTransferApiServices.Login<MobileTransferAccessTokeneResponse>(configurationVm);

                    MobileTransferAccessTokeneResponse tokenModel = new MobileTransferAccessTokeneResponse();
                    ////tokenModel = accesstoken.Result;
                    ////tokenModel.apirefId = configurationVm.apirefId;
                    ////tokenModel.apiKey = configurationVm.apiKey;
                    ////tokenModel.apiUrl = "";
                    ////tokenModel.subscriptionKey = configurationVm.subscriptionKey;

                    MobileNoLookUpResponseData mobileNoLookUp = new MobileNoLookUpResponseData()
                    {
                        accountHolderIdType = "MSISDN",
                        accountHolderId = model.CountryPhoneCode + "" + model.MobileNumber
                    };
                    //var mtnResponse = mobileTransferApiServices.ValidateMobileNo<MobileNoLookUpResponse>(
                    //    mobileNoLookUp.accountHolderIdType, mobileNoLookUp.accountHolderId, accesstoken.Result.access_token);
                    //if (mtnResponse.Result.status == false)
                    //{

                    //    result.Data = false;
                    //    result.Message = "Enter valid mobile number";

                    //}
                    SAgentMobileTransferWallet _sAgentMobileTransferWalletServices = new SAgentMobileTransferWallet();
                    _sAgentMobileTransferWalletServices.SetMobileTransferAccessTokeneResponse(tokenModel);
                    break;

                case DB.Apiservice.TransferZero:
                    TransferZeroApi transferZeroApi = new TransferZeroApi();
                    if (model.CountryCode == "GH" || model.CountryCode == "NG")
                    {
                        string[] phoneCode = model.CountryPhoneCode.Split('+');
                        string receiverPhoneCode = phoneCode[phoneCode.Length - 1];
                        AccountValidationRequest accountValidationRequest = new AccountValidationRequest(
                                        phoneNumber: receiverPhoneCode + model.MobileNumber,
                                        country: (AccountValidationRequest.CountryEnum)Common.Common.getAccountValidationCountryCodeForTZ(model.CountryCode),
                                        currency: (AccountValidationRequest.CurrencyEnum)Common.Common.getAccountValidationCountryCurrencyForTZ(model.CountryCode),
                                        method: AccountValidationRequest.MethodEnum.Mobile
                         );
                        var transferZeroApitResult = transferZeroApi.ValidateAccountNo(accountValidationRequest);
                        var IsValidateAccount = false;
                        try
                        {
                            if (transferZeroApitResult != null)
                            {
                                IsValidateAccount = transferZeroApitResult.Meta == null ? true : false;
                            }
                        }
                        catch (Exception)
                        {

                        }
                        if (IsValidateAccount == false)
                        {
                            result.Data = false;
                            result.Message = "Enter valid mobile number";


                        }
                    }
                    break;
                case DB.Apiservice.EmergentApi:
                    EmergentApi emergentApiServices = new EmergentApi();
                    EmergentApiMobileMoneyCustomerCheck customerCheck = new EmergentApiMobileMoneyCustomerCheck()
                    {
                        mobile = model.CountryPhoneCode + model.MobileNumber
                    };
                    var emergentApiResponse = emergentApiServices.IsValidMobileCustomer(customerCheck);
                    if (emergentApiResponse.Data == false)
                    {
                        result.Data = false;
                        result.Message = "Enter valid mobile number";
                    }
                    break;
                case DB.Apiservice.Zenith:
                    ZenithApi zenithApiServices = new ZenithApi();
                    var zenithApiMobileNetworks = zenithApiServices.GetMobileNetworks();
                    string mobileNetworkName = Common.Common.GetMobileWalletInfo(model.WalletId).Code;
                    var zenithApiResponse = zenithApiServices.VerifyAccount(new ZenithTransferVerifyAccountModel()
                    {
                        DestSortCode = zenithApiMobileNetworks.Where(x => x.BankName == mobileNetworkName).
                                     Select(x => x.BankSortCode).FirstOrDefault(),
                        TargetAccountNo = model.MobileNumber
                    });
                    if (zenithApiResponse.ResponseCode == "005")
                    {
                        result.Data = false;
                        result.Message = "Enter valid mobile number";
                    }
                    break;
                default:
                    result.Data = false;
                    result.Message = "Service Not Avialable";
                    break;
            }

            return result;
        }


        public void SendEmailAndSms(MobileMoneyTransfer item)
        {

            TransactionEmailType _transactionEmailType = TransactionEmailType.CustomerSupport;
            switch (item.Status)
            {
                case MobileMoneyTransferStatus.Failed:
                    break;
                case MobileMoneyTransferStatus.InProgress:

                    SetTransactionEmailType(TransactionEmailType.TransactionInProgress);
                    SendTransactionInProgressSms(item);
                    TransferInProgressEmail(item);
                    break;
                case MobileMoneyTransferStatus.Paid:


                    SetTransactionEmailType(TransactionEmailType.TransactionCompleted);
                    SendTransactionCompletedSms(item);
                    TransferCompletionEmail(item);
                    break;
                case MobileMoneyTransferStatus.Cancel:

                    _transactionEmailType = TransactionEmailType.TransactionCancelled;
                    TransactionCancellationEmail(item);
                    break;
                case MobileMoneyTransferStatus.PaymentPending:

                    _transactionEmailType = TransactionEmailType.TransactionPending;
                    break;
                case MobileMoneyTransferStatus.IdCheckInProgress:


                    SetTransactionEmailType(TransactionEmailType.IDCheck);
                    IdCheckInProgress(item);
                    break;
                case MobileMoneyTransferStatus.PendingBankdepositConfirmtaion:



                    SetTransactionEmailType(TransactionEmailType.TransactionInProgress);
                    TransferInProgressEmail(item);
                    break;
                case MobileMoneyTransferStatus.Abnormal:
                    break;
                case MobileMoneyTransferStatus.FullRefund:


                    SetTransactionEmailType(TransactionEmailType.TransactionCompleted);
                    TransactionRefundEmail(item);
                    break;
                case MobileMoneyTransferStatus.PartailRefund:

                    SetTransactionEmailType(TransactionEmailType.TransactionCompleted);
                    TransactionRefundEmail(item);
                    break;
                default:
                    break;
            }



        }

        private void SetTransactionEmailType(TransactionEmailType transactionEmailType)
        {


            Common.Common.SetTransactionEmailTypeSession(transactionEmailType);
        }



        public void TransactionRefundEmail(MobileMoneyTransfer item)
        {
            // Sending Email Code goes here 
            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);
            string email = senderInfo.Email;

            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string bankName = "";
            string WalletName = dbContext.MobileWalletOperator.Where(x => x.Id == item.WalletOperatorId).Select(x => x.Name).FirstOrDefault();
            string sendingCountryCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(item.SendingCurrency, item.SendingCountry);
            string receivingCountryCurrecy = Common.Common.GetCurrencyByCurrencyOrCountry(item.ReceivingCurrency, item.ReceivingCountry);
            string ReceivingCountryName = Common.Common.GetCountryName(item.ReceivingCountry);
            string receiverFirstName = "";
            try
            {
                receiverFirstName = item.ReceiverName.Split(' ')[0];
            }
            catch (Exception)
            {
            }
            var refundHistory = dbContext.RefundHistory.Where(x => x.TransactionId == item.Id && x.TransactionServiceType == TransactionServiceType.MobileWallet).FirstOrDefault();

            if (refundHistory != null)
            {
                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/RefundIssued/Index?" +
                    "&SenderFristName=" + senderInfo.FirstName +
                    "&SendingCurrency=" + sendingCountryCurrency +
                    "&RefundAmount=" + refundHistory.RefundedAmount +
                    "&TransactionNumber=" + item.ReceiptNo +
                    "&SendingAmount=" + item.SendingAmount +
                    "&SendingCountry=" + item.SendingCountry +
                    "&Fee=" + item.Fee +
                    "&ReceiverFirstName=" + receiverFirstName +
                    "&ReceivingCurrency=" + receivingCountryCurrecy +
                    "&ReceivingAmount=" + item.ReceivingAmount +
                    "&BankName=" + bankName +
                    "&BankAccount=" + "" +
                    "&BranchCode=" + "" +
                    "&transactionServiceType=" + TransactionServiceType.MobileWallet +
                    "&WalletName=" + WalletName +
                    "&MobileNo=" + item.PaidToMobileNo);
                mail.SendMail(email, "Refund Issued -" + item.ReceiptNo, body);
            }
        }


        public void TransferInProgressEmail(MobileMoneyTransfer item)
        {
            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);
            string SenderfirstName = senderInfo.FirstName;
            string ReceiverName = "";
            try
            {
                ReceiverName = item.ReceiverName.Split(' ')[0];
            }
            catch (Exception)
            {

            }
            string ReceivingCountryName = Common.Common.GetCountryName(item.ReceivingCountry);
            string WalletName = Common.Common.GetMobileWalletInfo(item.WalletOperatorId).Name;
            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";


            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/SenderMobileMoneyTranferEmail/Index?" +
                 "&ReceiverName=" + ReceiverName + "&ReceiptNo=" + item.ReceiptNo +
                 "&ReceivingCountry=" + ReceivingCountryName +
                 "&SendingAmount=" + item.SendingAmount
                 + "&WalletName=" + WalletName + "&PaidToMobileNo=" + item.PaidToMobileNo +
                 "&Fee=" + item.Fee + "&ReceiverFirstName=" + ReceiverName
                 + "&ReceivingAmount=" + item.ReceivingAmount + "&SenderFirstName=" + SenderfirstName +
                 "&PaymentReference=" + item.PaymentReference + "&senderPaymentMode=" + item.SenderPaymentMode
                 + "&IsPaid=" + false);

            mail.SendMail(senderInfo.Email, " Confirmation of transfer to" + " " + ReceiverName, body);
        }
        public void SendTransactionInProgressSms(MobileMoneyTransfer item)
        {


            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);
            string phoneNo = Common.Common.GetCountryPhoneCode(item.SendingCountry) + senderInfo.PhoneNumber;
            SmsApi smsApi = new SmsApi();
            string[] Name = item.ReceiverName.Split(' ');
            string FirstName = Name[0];
            string LastName = "";
            try
            {
                if (Name.Length > 1)
                {

                    for (int i = 1; i < Name.Length; i++)
                    {
                        LastName = LastName + Name[i] + " ";
                    }
                }
                else
                {

                    LastName = FirstName;
                }

            }
            catch (Exception)
            {

            }
            string msg = smsApi.GetMobileTransferInProgressMsg(FirstName,
                 Common.Common.GetCurrencyByCurrencyOrCountry(item.ReceivingCurrency, item.ReceivingCountry) + " " +
                 item.ReceivingAmount, item.ReceiptNo);
            smsApi.SendSMS(phoneNo, msg);
        }
        public void SendTransactionCompletedSms(MobileMoneyTransfer item)
        {
            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);
            string phoneNo = Common.Common.GetCountryPhoneCode(item.SendingCountry) + senderInfo.PhoneNumber;
            string[] Name = item.ReceiverName.Split(' ');
            string FirstName = Name[0];
            SmsApi smsApi = new SmsApi();
            string msg = smsApi.GetMobileTransferMsg(FirstName,
                 Common.Common.GetCurrencyByCurrencyOrCountry(item.ReceivingCurrency, item.ReceivingCountry) + " " +
                 item.ReceivingAmount, item.ReceiptNo);
            smsApi.SendSMS(phoneNo, msg);

        }
        public void TransferCompletionEmail(MobileMoneyTransfer item)
        {

            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);
            string SenderfirstName = senderInfo.FirstName;
            string ReceiverName = "";
            try
            {
                ReceiverName = item.ReceiverName.Split(' ')[0];
            }
            catch (Exception)
            {

            }
            string ReceivingCountryName = Common.Common.GetCountryName(item.ReceivingCountry);
            string WalletName = Common.Common.GetMobileWalletInfo(item.WalletOperatorId).Name;
            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";


            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/SenderMobileMoneyTranferEmail/Index?" +
                 "&ReceiverName=" + ReceiverName + "&ReceiptNo=" + item.ReceiptNo +
                 "&ReceivingCountry=" + ReceivingCountryName +
                 "&SendingAmount=" + item.SendingAmount
                 + "&WalletName=" + WalletName + "&PaidToMobileNo=" + item.PaidToMobileNo +
                 "&Fee=" + item.Fee + "&ReceiverFirstName=" + ReceiverName
                 + "&ReceivingAmount=" + item.ReceivingAmount + "&SenderFirstName=" + SenderfirstName +
                 "&PaymentReference=" + item.PaymentReference + "&senderPaymentMode=" + item.SenderPaymentMode
                 + "&IsPaid=" + true);

            mail.SendMail(senderInfo.Email, " Confirmation of transfer to" + " " + ReceiverName, body);
        }

        public void IdCheckInProgress(MobileMoneyTransfer item)
        {
            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);
            string email = senderInfo.Email;
            string SenderFristName = senderInfo.FirstName;

            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string bankName = "";
            string ReceivingCountry = Common.Common.GetCountryName(item.ReceivingCountry);
            string WalletName = Common.Common.GetMobileWalletInfo(item.WalletOperatorId).Name;


            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/TransactionPaused?" +
                "&SenderFristName=" + senderInfo.FirstName + "&TransactionNumber=" + item.ReceiptNo
                + "&SendingAmount=" + item.SendingAmount + "&ReceivingAmount=" + item.ReceivingAmount +
                "&Receivingcountry=" + ReceivingCountry + "&Fee=" + item.Fee
                + "&ReceiverFirstName=" + item.ReceiverName + "&BankName=" + bankName +
                "&BankAccount=" + "" + "&BankCode=" + ""
                + "&TransactionServiceType=" + TransactionServiceType.MobileWallet +
                "&WalletName=" + WalletName + "&MFCN=" + "");

            try
            {

                mail.SendMail(email, "Your transfer has been paused", body);
            }
            catch (Exception)
            {

            }
            
        }

        public void TransactionCancellationEmail(MobileMoneyTransfer item)
        {
            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);

            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string bankName = "";
            string ReceiverCountryName = Common.Common.GetCountryName(item.ReceivingCountry);


            string WalletName = Common.Common.GetMobileWalletInfo(item.SenderId).Name;
            string SendingCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(item.SendingCurrency, item.SendingCountry);
            string ReceivingCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(item.ReceivingCurrency, item.ReceivingCountry);

            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/TransactionCancelled?" +
                 "&SenderFristName=" + senderInfo.FirstName + "&TransactionNumber=" + item.ReceiptNo +
                 "&SendingAmount=" + item.SendingAmount + "&Receivingcountry=" + ReceiverCountryName
                 + "&Fee=" + item.Fee + "&ReceiverName=" + item.ReceiverName + "&BankName=" + bankName +
                 "&BankAccount=" + ""
                 + "&BankCode=" + "" + "&TransactionServiceType=" +
                 TransactionServiceType.MobileWallet + "&WalletName=" + WalletName + "&MFCN=" + ""
                   + "&SendingCurrency=" + SendingCurrency + "&ReceivingCurrency=" + ReceivingCurrency);

            mail.SendMail(senderInfo.Email, "Transfer cancelled" + " " + item.ReceiptNo, body);



        }
        #endregion
        internal int GetRecipientId(int walletId, string AccountNo)
        {
            return dbContext.Recipients.Where(x => x.MobileNo == AccountNo && x.MobileWalletProvider == walletId).Select(x => x.Id).FirstOrDefault();
        }
    }

    public class MobileTransferApiResponse
    {

        public MobileMoneyTransferStatus status { get; set; }
        public MTNCameroonResponseParamVm response { get; set; }

    }
}