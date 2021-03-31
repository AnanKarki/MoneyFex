using ExCSS;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.BankApi;
using FAXER.PORTAL.BankApi.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Models.PaymentSummary;
using Microsoft.Office.Core;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using RestSharp.Validation;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using TransferZero.Sdk.Model;
using static FAXER.PORTAL.BankApi.Models.FlutterWaveViewModel;

namespace FAXER.PORTAL.Services
{
    public class SSenderBankAccountDeposit
    {
        FAXER.PORTAL.DB.FAXEREntities db = null;
        SSenderKiiPayWalletTransfer _senderKiiPayServices = null;

        private TransactionEmailType _transactionEmailType = TransactionEmailType.CustomerSupport;


        public SSenderBankAccountDeposit()
        {
            db = new DB.FAXEREntities();
            _senderKiiPayServices = new SSenderKiiPayWalletTransfer(db);
        }
        public SSenderBankAccountDeposit(FAXEREntities db)
        {
            this.db = db;
            _senderKiiPayServices = new SSenderKiiPayWalletTransfer(db);
        }


        public void UpdateTransactionStatus()
        {

            var data = db.BankAccountDeposit.Where(x => x.Apiservice == Apiservice.Zenith && x.Status == BankDepositStatus.Incomplete).ToList();
            foreach (var item in data)
            {


                try
                {

                    ZenithApi zenithApi = new ZenithApi();
                    var statusResponse = zenithApi.GetTransactionStatus(new ZenithTransferTransactionStatusModel()
                    {
                        Reference = "MF" + item.ReceiptNo
                    });

                    item.Status = BankDepositStatus.Incomplete;
                    switch (statusResponse.Transaction.StatusCode)
                    {
                        /// Status
                        case "000": /// 000 SUCCESS
                            item.Status = BankDepositStatus.Confirm;
                            break;
                        /// 005 ACCOUNT VERIFICATION FAILURE
                        case "005":
                            item.Status = BankDepositStatus.Failed;
                            break;
                        /// 007 REQUIRED FIELDS VALIDATION ERROR(S)
                        case "007":
                            item.Status = BankDepositStatus.Failed;
                            break;
                        /// 009 GIP TRANSFER FAILED
                        case "009":
                            item.Status = BankDepositStatus.Failed;
                            break;
                        /// 006 INVALID TRANSFER CHANNEL
                        case "006":
                            item.Status = BankDepositStatus.Failed;
                            break;
                        /// 017 TRANSACTION(S) NOT FOUND
                        case "017":
                            item.Status = BankDepositStatus.Failed;
                            break;
                        /// 010 ACCOUNT DAILY LIMIT REACHED
                        case "010":
                            item.Status = BankDepositStatus.Failed;
                            break;
                        /// 011 ACCOUNT TRANS LIMIT REACHED
                        case "011":
                            item.Status = BankDepositStatus.Failed;
                            break;
                        /// 012 ACCOUNT DAILY LIMIT REACHED
                        case "012":
                            item.Status = BankDepositStatus.Failed;
                            break;
                        /// 013 ACCOUNT TRANSACTION LIMIT REACHED
                        case "013":
                            item.Status = BankDepositStatus.Failed;
                            break;
                        /// 018 GLOBAL DAILY LIMIT REACHED
                        case "018":
                            item.Status = BankDepositStatus.Failed;
                            break;
                        /// 019 INSUFFICIENT FUNDS
                        case "019":
                            item.Status = BankDepositStatus.Incomplete;
                            break;
                        default:
                            break;
                    }
                    Update(item);
                    if (item.Status == BankDepositStatus.Confirm)
                    {
                        SendEmailAndSms(item);
                    }
                }
                catch (Exception)
                {

                }

            }
        }
        public ServiceResult<BankAccountDeposit> Add(BankAccountDeposit model)
        {
            db.BankAccountDeposit.Add(model);
            db.SaveChanges();

            return new ServiceResult<BankAccountDeposit>()
            {
                Data = model,
                Message = "",
                Status = ResultStatus.OK
            };
        }

        public ServiceResult<FaxerInformation> AddSender(FaxerInformation model)
        {
            db.FaxerInformation.Add(model);
            db.SaveChanges();

            return new ServiceResult<FaxerInformation>()
            {
                Data = model,
                Message = "",
                Status = ResultStatus.OK
            };
        }

        public List<DropDownViewModel> GetRecentAccountNumbers(string currency = "" , string CountryCode = "")
        {
            int senderId = Common.FaxerSession.LoggedUser.Id;
            var result = (from c in db.BankAccountDeposit.Where(x => x.SenderId == senderId && x.PaidFromModule == Module.Faxer
                          && (x.ReceivingCurrency == currency || x.ReceiverCountry == CountryCode))
                          group c by new
                          {
                              c.ReceiverAccountNo,
                              c.ReceiverCountry,
                              c.BankId
                          } into gcs
                          select new DropDownViewModel()
                          {
                              Code = gcs.FirstOrDefault().ReceiverAccountNo,
                              Name = gcs.FirstOrDefault().ReceiverAccountNo + " ( " + gcs.FirstOrDefault().ReceiverName + " ) ",
                              CountryCode = gcs.FirstOrDefault().ReceiverCountry
                          }).ToList();

            return result;
        }

        public List<DropDownViewModel> GetRecentAccountNumbers(int senderId)
        {




            var result = (from c in db.BankAccountDeposit.Where(x => x.SenderId == senderId && x.PaidFromModule == Module.Faxer)
                          group c by new
                          {
                              c.ReceiverAccountNo,
                              c.ReceiverCountry,
                              c.BankId
                          } into gcs
                          select new DropDownViewModel()
                          {
                              Code = gcs.FirstOrDefault().ReceiverAccountNo,
                              Name = gcs.FirstOrDefault().ReceiverAccountNo + " ( " + gcs.FirstOrDefault().ReceiverName + " ) ",
                              CountryCode = gcs.FirstOrDefault().ReceiverCountry
                          }).ToList();

            return result;
        }

        public BankAccountDeposit GetBankDepositInfoByReceiptNo(string receiptNo)
        {
            var Bankdetails = db.BankAccountDeposit.Where(x => x.ReceiptNo == receiptNo).FirstOrDefault();
            return Bankdetails;
        }

        internal void CancelTransaction(string receiptNo)
        {
            var data = GetBankDepositInfoByReceiptNo(receiptNo);
            data.Status = BankDepositStatus.Cancel;
            db.Entry<BankAccountDeposit>(data).State = EntityState.Modified;
            db.SaveChanges();
        }

        public List<DropDownViewModel> GetStaffRecentAccountNumbers(int senderId)
        {


            var result = (from c in db.BankAccountDeposit.Where(x => x.SenderId == senderId && x.PaidFromModule == Module.Staff)
                          group c by new
                          {
                              c.ReceiverAccountNo,
                              c.ReceiverCountry,
                              c.BankId
                          } into gcs
                          select new DropDownViewModel()
                          {
                              Code = gcs.FirstOrDefault().ReceiverAccountNo,
                              Name = gcs.FirstOrDefault().ReceiverAccountNo + " ( " + gcs.FirstOrDefault().ReceiverName + " ) ",
                              CountryCode = gcs.FirstOrDefault().ReceiverCountry
                          }).ToList();

            return result;
        }


        internal Bank GetBankCode(int bankId)
        {
            var code = db.Bank.Where(x => x.Id == bankId).FirstOrDefault();
            return code;

        }
        internal int GetRecipientId(int bankId, string AccountNo)
        {
            return db.Recipients.Where(x => x.AccountNo == AccountNo && x.BankId == bankId).Select(x => x.Id).FirstOrDefault();
        }
        public bool IsEuropeTransfer(string Country)
        {
            var result = Common.Common.IsEuropeTransfer(Country);
            return result;
        }
        public bool IsSouthAfricanTransfer(string Country)
        {

            var result = Common.Common.IsSouthAfricanTransfer(Country);
            return result;
        }
        public bool IsWestAfricanTransfer(string Country)
        {

            var result = Common.Common.IsWestAfricanTransfer(Country);
            return result;
        }

        public List<DropDownViewModel> GetBranches(int bankId = 0)
        {

            List<DropDownViewModel> list = new List<DropDownViewModel>();

            var result = (from c in db.BankBranch.Where(x => x.BankId == bankId).ToList()
                          select new DropDownViewModel()
                          {
                              Code = c.BranchCode,
                              Name = c.BranchName,
                              Id = c.Id
                          }).ToList();
            return result;
        }

        public List<DropDownViewModel> getBanksList(string Country = "")
        {
            var result = new List<DropDownViewModel>();
            if (!string.IsNullOrEmpty(Country))
            {
                result = (from c in db.Bank.Where(x => x.CountryCode == Country && x.IsDeleted == false)
                          select new DropDownViewModel()
                          {
                              Id = c.Id,
                              Name = c.Name
                          }).ToList();


                return result;
            }
            else
            {
                result = (from c in db.Bank
                          select new DropDownViewModel()
                          {
                              Id = c.Id,
                              Name = c.Name
                          }).ToList();


                return result;
            }
        }

        public List<DropDownViewModel> getBanksByCurrency(string receivingCountry = "", string Currency = "")
        {
            var result = new List<DropDownViewModel>();

            string defaultCurrency = Common.Common.GetCurrencyCode(receivingCountry);
            if (Currency != defaultCurrency)
            {

                var currenyAcceptingBank = GetCurrencyAcceptingBanks(receivingCountry, Currency);
                result = (from c in db.Bank.Where(x => x.CountryCode == receivingCountry && x.IsDeleted == false)
                          join d in currenyAcceptingBank on c.Id equals d.BankId
                          select new DropDownViewModel()
                          {
                              Id = c.Id,
                              Name = c.Name,
                              CountryCode = c.CountryCode,
                              Code = c.Code
                          }).ToList();


                return result;
            }


            if (!string.IsNullOrEmpty(receivingCountry))
            {
                result = (from c in db.Bank.Where(x => x.CountryCode == receivingCountry && x.IsDeleted == false)
                              //join d in currenyAcceptingBank on c.Id equals d.BankId
                          select new DropDownViewModel()
                          {
                              Id = c.Id,
                              Name = c.Name,
                              CountryCode = c.CountryCode,
                              Code = c.Code
                          }).ToList();


                return result;
            }
            else
            {
                result = (from c in db.Bank
                          select new DropDownViewModel()
                          {
                              Id = c.Id,
                              Name = c.Name,
                              CountryCode = c.CountryCode,
                              Code = c.Code
                          }).ToList();


                return result;
            }
        }

        private IQueryable<BankAcceptingCurrency> GetCurrencyAcceptingBanks(string receivingCountry = "", string currency = "")
        {

            var bankService = db.TransferServiceMaster.Where(x => x.ReceivingCountry == receivingCountry);
            if (!string.IsNullOrEmpty(currency))
            {
                bankService = bankService.Where(x => x.ReceivingCurrency == currency);
            }

            var bankResult = (from c in bankService
                              join d in db.BankAcceptingCurrency on c.Id equals d.ServiceSettingId
                              select d);
            return bankResult;
        }

        public void SetSenderBankAccountDeposit(SenderBankAccountDepositVm vm)
        {
            Common.FaxerSession.SenderBankAccountDeposit = vm;
        }

        public SenderBankAccountDepositVm GetSenderBankAccountDeposit()
        {
            SenderBankAccountDepositVm vm = new SenderBankAccountDepositVm();
            if (Common.FaxerSession.SenderBankAccountDeposit != null)
            {
                vm = Common.FaxerSession.SenderBankAccountDeposit;
            }
            return vm;
        }

        public SenderBankAccountDepositVm GetAccountInformationFromAccountNumber(string accountNo, string Country = "")
        {
            var trans = db.BankAccountDeposit.Where(x => x.ReceiverAccountNo == accountNo);
            if (!string.IsNullOrEmpty(Country))
            {

                trans = trans.Where(x => x.ReceivingCountry == Country);
            }

            var data = (from c in trans.ToList()
                        join d in db.Recipients on c.RecipientId equals d.Id into joined
                        from d in joined.DefaultIfEmpty()
                        select new SenderBankAccountDepositVm()
                        {
                            AccountNumber = c.ReceiverAccountNo,
                            AccountOwnerName = c.ReceiverName,
                            MobileNumber = c.ReceiverMobileNo,
                            CountryCode = c.ReceiverCountry,
                            BankId = c.BankId,
                            BranchCode = c.BankCode,
                            CountryPhoneCode = Common.Common.GetCountryPhoneCode(c.ReceiverCountry),
                            RecentAccountNumber = c.ReceiverAccountNo,
                            IsManualDeposit = c.IsManualDeposit,
                            ReceiverCity = d == null ? "" : d.City,
                            ReceiverPostalCode = d == null ? "" : d.PostalCode,
                            ReceiverEmail = d == null ? "" : d.Email,
                            ReceiverStreet = d == null ? "" : d.Street
                        }).FirstOrDefault();
            return data;
        }
        public SenderBankAccountDepositVm GetAccountInformationFromAccountNumberAndId(string accountNo = "", int id = 0)
        {
            IQueryable<BankAccountDeposit> bankAccountDeposits = db.BankAccountDeposit;
            if (!string.IsNullOrEmpty(accountNo))
            {
                bankAccountDeposits = bankAccountDeposits.Where(x => x.ReceiverAccountNo == accountNo);
            }
            if (id > 0)
            {
                bankAccountDeposits = bankAccountDeposits.Where(x => x.Id == id);
            }

            var data = (from c in bankAccountDeposits
                        join country in db.Country on c.ReceivingCountry equals country.CountryCode
                        select new SenderBankAccountDepositVm()
                        {
                            AccountNumber = c.ReceiverAccountNo,
                            AccountOwnerName = c.ReceiverName,
                            MobileNumber = c.ReceiverMobileNo,
                            CountryCode = c.ReceiverCountry,
                            BankId = c.BankId,
                            BranchCode = c.BankCode,
                            CountryPhoneCode = country.CountryPhoneCode,
                            RecentAccountNumber = c.ReceiverAccountNo,
                            IsManualDeposit = c.IsManualDeposit,
                        }).FirstOrDefault();
            return data;
        }
        public string GetBankName(int bankId)
        {

            string bankName = db.Bank.Where(x => x.Id == bankId).Select(x => x.Name).FirstOrDefault();
            return bankName;
        }

        public SenderBankAccountDepositVm GetAccountInformationFromId(int Id)
        {
            var data = (from c in db.BankAccountDeposit.Where(x => x.Id == Id).ToList()
                        select new SenderBankAccountDepositVm()
                        {
                            AccountNumber = c.ReceiverAccountNo,
                            AccountOwnerName = c.ReceiverName,
                            MobileNumber = c.ReceiverMobileNo,
                            CountryCode = c.ReceiverCountry,
                            BankId = c.BankId,
                            BranchCode = c.BankCode,
                            CountryPhoneCode = Common.Common.GetCountryPhoneCode(c.ReceiverCountry),
                        }).FirstOrDefault();
            return data;
        }
        public SenderBankAccoutDepositEnterAmountVm GetRepeatedTransactionInfo(int id)
        {
            var data = (from c in db.BankAccountDeposit.Where(x => x.Id == id).ToList()
                        select new SenderBankAccoutDepositEnterAmountVm()
                        {
                            ExchangeRate = c.ExchangeRate,
                            Fee = c.Fee,
                            ReceivingAmount = c.ReceivingAmount,
                            SendingAmount = c.SendingAmount,
                            ReceivingCountryCode = c.ReceivingCountry,
                            SendingCountryCode = c.SendingCountry,
                            TotalAmount = c.TotalAmount,
                            ReceiverName = c.ReceiverName,
                            ReceivingCurrency = Common.Common.GetCountryCurrency(c.ReceivingCountry),
                            SendingCurrency = Common.Common.GetCountryCurrency(c.SendingCountry),
                            SendingCurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                            ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(c.ReceivingCountry),
                            ReceivingCurrencyCode = Common.Common.GetCurrencyCode(c.ReceivingCountry),
                            SendingCurrencyCode = Common.Common.GetCurrencyCode(c.SendingCountry),


                        }).FirstOrDefault();
            return data;
        }
        public BankAccountDeposit GetBankDepositInfo(string accountNo, int Id)
        {
            var result = db.BankAccountDeposit.Where(x => x.ReceiverAccountNo == accountNo && x.Id == Id).FirstOrDefault();
            return result;
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


        public void SetSenderBankAccoutDepositEnterAmount(SenderBankAccoutDepositEnterAmountVm vm)
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
               

           
                vm.TotalAmount = dashboarddata.TotalAmount;

                if (!string.IsNullOrEmpty(dashboarddata.SendingCountryCode))
                {
                    vm.SendingCurrencySymbol = dashboarddata.SendingCurrencySymbol;
                    vm.SendingCurrencyCode = dashboarddata.SendingCurrency;//  Common.Common.GetCountryCurrency(dashboarddata.SendingCountryCode);
                    vm.ReceivingCurrencySymbol = dashboarddata.ReceivingCurrencySymbol;
                    vm.ReceivingCurrencyCode = dashboarddata.ReceivingCurrency; //Common.Common.GetCountryCurrency(dashboarddata.ReceivingCountryCode);
                    vm.ExchangeRate = dashboarddata.ExchangeRate;
                    vm.SendingAmount = dashboarddata.SendingAmount;
                    vm.ReceivingAmount = dashboarddata.ReceivingAmount;
                    vm.Fee = dashboarddata.Fee;
                    vm.TotalAmount = dashboarddata.TotalAmount;
                    Common.FaxerSession.SenderBankAccoutDepositEnterAmount = vm;
                }
                else
                {
                    


                    Common.FaxerSession.SenderBankAccoutDepositEnterAmount = vm;
                }
            }
            else
            {
                Common.FaxerSession.SenderBankAccoutDepositEnterAmount = vm;
            }

        }
        public SenderBankAccoutDepositEnterAmountVm GetSenderBankAccoutDepositEnterAmount()
        {

            SenderBankAccoutDepositEnterAmountVm vm = new SenderBankAccoutDepositEnterAmountVm();

            if (Common.FaxerSession.SenderBankAccoutDepositEnterAmount != null)
            {

                vm = Common.FaxerSession.SenderBankAccoutDepositEnterAmount;
            }
            return vm;
        }

        public SenderBankAccoutDepositEnterAmountVm GetReceiverInformationFromAccountNumnber(string accountNo)
        {
            if (accountNo != null)
            {
                var accountOwner = db.SavedBank.Where(x => x.AccountNumber == accountNo).FirstOrDefault();
                if (accountOwner != null)
                {

                    return new SenderBankAccoutDepositEnterAmountVm()
                    {
                        ReceiverName = accountOwner.OwnerName,
                        Image = "",
                        ReceiverId = accountOwner.Id,

                    };
                }
            }

            return new SenderBankAccoutDepositEnterAmountVm()
            {
                ReceiverName = null,
                Image = "",
                ReceiverId = 0

            };
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


        #region Local 



        public void SetSenderLocalBankAccountEnterAmount(SenderLocalEnterAmountVM vm)
        {

            Common.FaxerSession.SenderLocalEnterAmount = vm;

        }

        public SenderLocalEnterAmountVM GetSenderLocalBankAccountEnterAmount()
        {

            SenderLocalEnterAmountVM vm = new SenderLocalEnterAmountVM();

            if (Common.FaxerSession.SenderLocalEnterAmount != null)
            {

                vm = Common.FaxerSession.SenderLocalEnterAmount;
            }
            return vm;
        }

        public SenderAccountPaymentSummaryViewModel GetLocalBankDepositSummary()
        {

            SenderAccountPaymentSummaryViewModel vm = new SenderAccountPaymentSummaryViewModel();

            if (Common.FaxerSession.SenderLocalEnterAmount != null)
            {

                vm = Common.FaxerSession.SenderAccountPaymentSummary;
            }
            return vm;
        }


        public void SetLocalBankDepositSummary(SenderAccountPaymentSummaryViewModel vm)
        {

            Common.FaxerSession.SenderAccountPaymentSummary = vm;
        }

        public BankAccountDeposit bankAccountDepositInfo(int transactionId)
        {
            var bankAccountDeposit = db.BankAccountDeposit.Where(x => x.Id == transactionId).FirstOrDefault();
            return bankAccountDeposit;
        }
        #endregion


        #region manual bank Deposit 

        public string GetSenderName(int SenderId)
        {
            var Sender = db.FaxerInformation.Where(x => x.Id == SenderId).FirstOrDefault();
            string senderFirstName = "";
            string senderMiddleName = "";
            string senderLastName = "";

            if (Sender != null)
            {
                senderFirstName = Sender.FirstName;
                senderMiddleName = Sender.MiddleName;
                senderLastName = Sender.LastName;

            }
            string SenderFullname = senderFirstName + " " + senderMiddleName + " " + senderLastName;

            return SenderFullname;
        }

        public AgentInformation AgentInformation(int AgentId)
        {
            var data = db.AgentInformation.Where(x => x.Id == AgentId).FirstOrDefault();
            return data;
        }
        public FaxerInformation GetSenderInfo(int senderId)
        {
            var data = db.FaxerInformation.Where(x => x.Id == senderId).FirstOrDefault();
            return data;
        }

        internal void setAmount(int transactionId)

        {
            SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();

            var paymentInfo = _kiiPaytrasferServices.GetCommonEnterAmount();

            var data = db.BankAccountDeposit.Where(x => x.Id == transactionId).FirstOrDefault();

            if (data != null && paymentInfo.ReceivingCountryCode == data.ReceiverCountry)
            {

                SenderBankAccoutDepositEnterAmountVm vm = new SenderBankAccoutDepositEnterAmountVm()
                {
                    ExchangeRate = paymentInfo.ExchangeRate,
                    Fee = paymentInfo.Fee,
                    ReceiverName = data.ReceiverName,
                    ReceivingAmount = data.ReceivingAmount,
                    ReceivingCountryCode = data.ReceivingCountry,
                    ReceivingCurrency = paymentInfo.ReceivingCurrency,
                    ReceivingCurrencyCode = paymentInfo.ReceivingCurrency,
                    ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(data.ReceivingCountry),
                    SendingCountryCode = data.SendingCountry,
                    SendingAmount = data.SendingAmount,
                    SendingCurrencySymbol = Common.Common.GetCurrencySymbol(data.SendingCountry),
                    SendingCurrency = paymentInfo.SendingCurrency,
                    SendingCurrencyCode = paymentInfo.SendingCurrency,
                    TotalAmount = data.TotalAmount

                };
                SetSenderBankAccoutDepositEnterAmount(vm);

                SenderBankAccountDepositVm model = new SenderBankAccountDepositVm()
                {
                    AccountNumber = data.ReceiverAccountNo,
                    AccountOwnerName = data.ReceiverName,
                    BankId = data.BankId,
                    BranchCode = data.BankCode,
                    CountryCode = data.ReceivingCountry,
                    IsBusiness = data.IsBusiness,
                    IsManualDeposit = data.IsManualDeposit,
                    MobileNumber = data.ReceiverMobileNo,
                    SendingCurrency = paymentInfo.SendingCurrency,
                    ReceivingCurrency = paymentInfo.ReceivingCurrency


                };
                SetSenderBankAccountDeposit(model);
            }
        }

        internal void setReciverInfo(int RecipientId)
        {
            SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();

            var paymentInfo = _kiiPaytrasferServices.GetCommonEnterAmount();
            var data = db.Recipients.Where(x => x.Id == RecipientId).FirstOrDefault();
            SenderBankAccoutDepositEnterAmountVm vm = new SenderBankAccoutDepositEnterAmountVm()
            {
                ExchangeRate = paymentInfo.ExchangeRate,
                Fee = paymentInfo.Fee,
                ReceiverName = data.ReceiverName,
                ReceivingAmount = paymentInfo.ReceivingAmount,
                ReceivingCountryCode = paymentInfo.ReceivingCountryCode,
                ReceivingCurrency = Common.Common.GetCurrencyCode(paymentInfo.ReceivingCountryCode),
                ReceivingCurrencyCode = Common.Common.GetCurrencyCode(paymentInfo.ReceivingCountryCode),
                ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(paymentInfo.ReceivingCountryCode),
                SendingCountryCode = paymentInfo.SendingCountryCode,
                SendingAmount = paymentInfo.SendingAmount,
                SendingCurrencySymbol = Common.Common.GetCurrencySymbol(paymentInfo.SendingCountryCode),
                SendingCurrency = Common.Common.GetCurrencyCode(paymentInfo.SendingCountryCode),
                SendingCurrencyCode = Common.Common.GetCurrencyCode(paymentInfo.SendingCountryCode),
                TotalAmount = paymentInfo.TotalAmount

            };
            SetSenderBankAccoutDepositEnterAmount(vm);

            SenderBankAccountDepositVm model = new SenderBankAccountDepositVm()
            {
                AccountNumber = data.AccountNo,
                AccountOwnerName = data.ReceiverName,
                BankId = data.BankId,
                BranchCode = data.BranchCode,
                CountryCode = data.Country,
                IsBusiness = data.IBusiness,
                MobileNumber = data.MobileNo,
            };
            SetSenderBankAccountDeposit(model);
        }

        public AgentLogin AgentLoginInformation(int AgentId)
        {
            var data = db.AgentLogin.Where(x => x.AgentId == AgentId).FirstOrDefault();
            return data;
        }


        public ServiceResult<IQueryable<BankAccountDeposit>> List()
        {
            return new ServiceResult<IQueryable<BankAccountDeposit>>()
            {
                Data = db.BankAccountDeposit,
                Status = ResultStatus.OK
            };

        }

        public bool IsValidDeposit(string receiptNo)
        {

            var result = db.BankAccountDeposit.Where(x => x.ReceiptNo == receiptNo).FirstOrDefault();
            if (result == null)
            {

                return true;

            }
            return false;

        }

        public ServiceResult<BankAccountDeposit> Update(BankAccountDeposit model)
        {
            db.Entry<BankAccountDeposit>(model).State = EntityState.Modified;
            db.SaveChanges();
            return new ServiceResult<BankAccountDeposit>()
            {
                Data = model,
                Message = "Update",
                Status = ResultStatus.OK
            };
        }

        public ServiceResult<ReinitializeTransaction> AddReIntializedTrans(ReinitializeTransaction model)
        {
            db.ReinitializeTransaction.Add(model);
            db.SaveChanges();
            return new ServiceResult<ReinitializeTransaction>()
            {
                Data = model,
                Message = "Update",
                Status = ResultStatus.OK
            };
        }
        public static T Clone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public string GenerateBankAccountDepositReceiptNo(bool IsManualDeposit = false)
        {

            //var code = (IsManualDeposit == true ? "MBD" : "BD") + Common.Common.GenerateRandomDigit(6);
            //while (db.BankAccountDeposit.Where(x => x.ReceiptNo == code).Count() > 0)
            //{
            //    GenerateBankAccountDepositReceiptNo(IsManualDeposit);
            //}
            var code = db.Sp_GetBankDepositReceiptNo_result(IsManualDeposit).ReceiptNo;



            return code;
        }
        public BankAccountDeposit CreateDuplicateTransaction(int TransactionId, BankAccountDeposit accountDeposit)
        {

            var bankDepositData = db.BankAccountDeposit.Where(x => x.Id == TransactionId).FirstOrDefault();


            //bankDepositData.Id = 0;
            //bankDepositData.ReceiverName = accountDeposit.ReceiverName;
            //bankDepositData.ReceiverAccountNo = accountDeposit.ReceiverAccountNo;
            //bankDepositData.BankId = accountDeposit.BankId;
            //bankDepositData.BankCode = accountDeposit.BankCode;
            //bankDepositData.BankName = accountDeposit.BankName;
            //bankDepositData.IsEuropeTransfer = accountDeposit.IsEuropeTransfer;
            //bankDepositData.Status = BankDepositStatus.Held;
            //bankDepositData.IsComplianceNeededForTrans = true;
            //bankDepositData.IsComplianceApproved = false;
            //bankDepositData.PayingStaffId = Common.StaffSession.LoggedStaff.StaffId;
            //bankDepositData.PayingStaffName = Common.StaffSession.LoggedStaff.FirstName + " "
            //  + Common.StaffSession.LoggedStaff.MiddleName + " " + Common.StaffSession.LoggedStaff.LastName;
            //bankDepositData.PaidFromModule = Module.Staff;
            //bankDepositData.IsTransactionDuplicated = true;
            //bankDepositData.DuplicateTransactionReceiptNo = bankDepositData.ReceiptNo;
            //bankDepositData.ReceiptNo = Common.Common.GenerateBankAccountDepositReceiptNo(6);

            //var result = Clone<BankAccountDeposit>(bankDepositData);

            BankAccountDeposit result = new BankAccountDeposit()
            {
                ReceiverName = accountDeposit.ReceiverName,
                ReceiverAccountNo = accountDeposit.ReceiverAccountNo,
                BankId = accountDeposit.BankId,
                BankCode = accountDeposit.BankCode,
                BankName = accountDeposit.BankName,
                IsEuropeTransfer = accountDeposit.IsEuropeTransfer,
                Status = BankDepositStatus.Held,
                IsComplianceNeededForTrans = true,
                PayingStaffId = Common.StaffSession.LoggedStaff.StaffId,
                PaidFromModule = Module.Staff,
                IsTransactionDuplicated = true,
                DuplicateTransactionReceiptNo = bankDepositData.ReceiptNo,
                Apiservice = bankDepositData.Apiservice,
                ReceiptNo = Common.Common.GenerateBankAccountDepositReceiptNo(6),

                IsBusiness = bankDepositData.IsBusiness,
                IsManualDeposit = bankDepositData.IsManualDeposit,
                MFRate = bankDepositData.MFRate,
                HasMadePaymentToBankAccount = bankDepositData.HasMadePaymentToBankAccount,
                PayingStaffName = Common.StaffSession.LoggedStaff.FirstName + " "
                + Common.StaffSession.LoggedStaff.MiddleName + " " + Common.StaffSession.LoggedStaff.LastName,
                PaymentReference = bankDepositData.PaymentReference,
                PaymentType = bankDepositData.PaymentType,
                ReasonForTransfer = bankDepositData.ReasonForTransfer,
                ReceiverCity = bankDepositData.ReceiverCity,
                ReceiverCountry = bankDepositData.ReceiverCountry,
                ReceiverMobileNo = bankDepositData.ReceiverMobileNo,
                ReceivingCountry = bankDepositData.ReceivingCountry,
                SendingCountry = bankDepositData.SendingCountry,
                RecipientId = accountDeposit.RecipientId,
                TransactionDate = DateTime.Now,
                SenderPaymentMode = bankDepositData.SenderPaymentMode,
                SenderId = bankDepositData.SenderId,
                SendingAmount = accountDeposit.SendingAmount,
                ExchangeRate = accountDeposit.ExchangeRate,
                TotalAmount = accountDeposit.TotalAmount,
                //ExtraFee = bankDepositData.ExtraFee,
                Fee = accountDeposit.Fee,
                ReceivingAmount = accountDeposit.ReceivingAmount,
                ReceivingCurrency = accountDeposit.ReceivingCurrency,
                SendingCurrency = accountDeposit.SendingCurrency,
                TransactionDescription = "This transaction is the duplicate of " + bankDepositData.ReceiptNo + "this transaction"
            };

            SManualApprovalTransactionCountry manualApprovalTransactionServices = new SManualApprovalTransactionCountry();

            bool IsManualApproveTransaction = manualApprovalTransactionServices.IsManaulApprovalTran(
                bankDepositData.ReceiverCountry, bankDepositData.ReceivingCurrency, TransactionTransferMethod.BankDeposit);
            if (IsManualApproveTransaction)
            {

                //item.IsComplianceNeededForTrans = true;
                result.IsManualApproveNeeded = true;
                result.Status = BankDepositStatus.Incomplete;
            }

            db.BankAccountDeposit.Add(result);
            db.SaveChanges();
            //bankDepositData.Status = BankDepositStatus.Cancel;
            //bankDepositData.TransactionDescription = "This transaction has been duplicate to " + result.ReceiptNo + "this transaction";

            //db.Entry<BankAccountDeposit>(bankDepositData).State = EntityState.Modified;


            //Update Reciepient
            //var recipient = db.Recipients.Where(x => x.Id == result.RecipientId).FirstOrDefault();
            //if (recipient != null)
            //{
            //    SSenderTransactionHistory _senderTransactionHistoryServices = new SSenderTransactionHistory();

            //    recipient.MobileNo = result.ReceiverMobileNo;
            //    recipient.AccountNo = result.ReceiverAccountNo;
            //    recipient.BankId = result.BankId;
            //    recipient.BranchCode = result.BankCode;
            //    var data = Common.AdminSession.SenderBankAccountDeposit;
            //    if (data != null)
            //    {
            //        recipient.City = data.ReceiverCity;
            //        recipient.Email = data.ReceiverEmail;
            //        recipient.PostalCode = data.ReceiverPostalCode;
            //        recipient.ReceiverName = data.AccountOwnerName;
            //        recipient.Street = data.ReceiverStreet;
            //    }
            //    RecipientServices _recipientServices = new RecipientServices();
            //    _recipientServices.UpdateReceipts(recipient);
            //}
            return result;



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

        public BankAccountDeposit SaveIncompleteTransaction()
        {

            var sendingAmountData = GetSenderBankAccoutDepositEnterAmount();
            var bankDetails = GetSenderBankAccountDeposit();
            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();
            bool IsManualDeposit = Common.Common.IsManualDeposit(Common.FaxerSession.LoggedUser.CountryCode, sendingAmountData.ReceivingCountryCode);
            //Common.FaxerSession.ReceiptNo = _senderForAllTransferServices.GenerateReceiptNoForBankDepoist(IsManualDeposit);
            Common.FaxerSession.ReceiptNo = GenerateBankAccountDepositReceiptNo(IsManualDeposit);
            //if (bankDetails.IsWestAfricaTransfer == false)
            //{
            //    Bank bank = new Bank()
            //    {
            //        Code = bankDetails.BankName,
            //        CountryCode = bankDetails.CountryCode,
            //        Name = bankDetails.BankName,

            //    };
            //    db.Bank.Add(bank);
            //    db.SaveChanges();
            //    bankDetails.BankId = bank.Id;
            //}

            BankAccountDeposit model = new BankAccountDeposit()
            {
                Status = BankDepositStatus.PaymentPending,
                BankId = bankDetails.BankId,
                BankCode = bankDetails.BranchCode,
                ExchangeRate = sendingAmountData.ExchangeRate,
                Fee = sendingAmountData.Fee,
                IsBusiness = bankDetails.IsBusiness,
                IsManualDeposit = IsManualDeposit,
                PaidFromModule = Module.Faxer,
                ReceiverAccountNo = bankDetails.AccountNumber,
                ReceiverMobileNo = bankDetails.MobileNumber,
                ReceiverCountry = bankDetails.CountryCode,
                ReceiverName = bankDetails.AccountOwnerName,
                ReceivingAmount = sendingAmountData.ReceivingAmount,
                ReceivingCountry = sendingAmountData.ReceivingCountryCode,
                SendingAmount = sendingAmountData.SendingAmount,
                TransactionDate = DateTime.Now,
                SenderId = Common.FaxerSession.LoggedUser.Id,
                TotalAmount = sendingAmountData.TotalAmount,
                SendingCountry = sendingAmountData.SendingCountryCode,
                ReceiptNo = Common.FaxerSession.ReceiptNo,
                BankName = bankDetails.BankName,
                IsEuropeTransfer = bankDetails.IsEuropeTransfer,
                ReasonForTransfer = bankDetails.ReasonForTransfer,
                RecipientId = bankDetails.ReceipientId,
                ReceivingCurrency = bankDetails.ReceivingCurrency,
                SendingCurrency = bankDetails.SendingCurrency
            };


            var data = db.BankAccountDeposit.Where(x => x.Id == Common.FaxerSession.TransactionId).FirstOrDefault();

            var Recipent = db.Recipients.Where(x => x.ReceiverName.ToLower() == bankDetails.AccountOwnerName.ToLower()
           && x.AccountNo == bankDetails.AccountNumber
          && x.Service == Service.BankAccount).FirstOrDefault();

            int RecipientId = 0;
            if (Recipent == null)
            {
                Recipients recipent = new Recipients()
                {
                    Country = bankDetails.CountryCode,
                    SenderId = FaxerSession.LoggedUser.Id,
                    MobileNo = bankDetails.MobileNumber,
                    Service = Service.BankAccount,
                    ReceiverName = bankDetails.AccountOwnerName,
                    BankId = bankDetails.BankId,
                    BranchCode = bankDetails.BranchCode,
                    AccountNo = bankDetails.AccountNumber,
                    IBusiness = bankDetails.IsBusiness,
                    City = bankDetails.ReceiverCity,
                    Email = bankDetails.ReceiverEmail,
                    PostalCode = bankDetails.ReceiverPostalCode,
                    Street = bankDetails.ReceiverStreet
                };
                var AddRecipient = db.Recipients.Add(recipent);
                db.SaveChanges();
                RecipientId = AddRecipient.Id;
            }
            else
            {
                RecipientId = Recipent.Id;
            }

            if (data != null && data.Status == BankDepositStatus.PaymentPending
                 && (Common.FaxerSession.TransactionId != null && Common.FaxerSession.TransactionId > 0))
            {
                data.Status = BankDepositStatus.PaymentPending;
                data.BankId = bankDetails.BankId;
                data.BankCode = bankDetails.BranchCode;
                data.ExchangeRate = sendingAmountData.ExchangeRate;
                data.Fee = sendingAmountData.Fee;
                data.IsBusiness = bankDetails.IsBusiness;
                data.IsManualDeposit = IsManualDeposit;
                data.PaidFromModule = Module.Faxer;
                data.ReceiverAccountNo = bankDetails.AccountNumber;
                data.ReceiverMobileNo = bankDetails.MobileNumber;
                data.ReceiverCountry = bankDetails.CountryCode;
                data.ReceiverName = bankDetails.AccountOwnerName;
                data.ReceivingAmount = sendingAmountData.ReceivingAmount;
                data.ReceivingCountry = sendingAmountData.ReceivingCountryCode;
                data.SendingAmount = sendingAmountData.SendingAmount;
                data.TransactionDate = DateTime.Now;
                data.SenderId = Common.FaxerSession.LoggedUser.Id;
                data.TotalAmount = sendingAmountData.TotalAmount;
                data.SendingCountry = sendingAmountData.SendingCountryCode;
                data.RecipientId = RecipientId;
                data.ReceivingCurrency = bankDetails.ReceivingCurrency;
                data.SendingCurrency = bankDetails.SendingCurrency;
                //data.ReceiptNo = Common.FaxerSession.ReceiptNo;
                Common.FaxerSession.ReceiptNo = data.ReceiptNo;
                db.Entry<BankAccountDeposit>(data).State = EntityState.Modified;
                db.SaveChanges();
                return data;

            }
            else
            {
                Log.Write("Transaction Saved " + model.ReceiptNo);
                model.RecipientId = RecipientId;
                db.BankAccountDeposit.Add(model);
                db.SaveChanges();

            }
            Common.FaxerSession.IsTransactionOnpending = true;


            return model;
        }

        internal bool RepeatTransaction(int transactionId, int RecipientId)
        {
            string ReceivingCountry = "";
            string ReceiverAccountNo = "";
            string BankCode = "";
            decimal SendingAmount = 0;
            string ErrorMessage = "";
            if (transactionId != 0)
            {
                var model = db.BankAccountDeposit.Where(x => x.Id == transactionId).FirstOrDefault();
                ReceivingCountry = model.ReceivingCountry;
                ReceiverAccountNo = model.ReceiverAccountNo;
                BankCode = model.BankCode;
                SendingAmount = model.SendingAmount;
            }
            if (RecipientId != 0)
            {
                var model = db.Recipients.Where(x => x.Id == RecipientId).FirstOrDefault();
                var sendingAmountData = GetSenderBankAccoutDepositEnterAmount();

                ReceivingCountry = model.Country;
                ReceiverAccountNo = model.AccountNo;
                BankCode = model.BranchCode;
                SendingAmount = sendingAmountData.SendingAmount;

            }


            bool IsManualDeposit = Common.Common.IsManualDeposit(Common.FaxerSession.LoggedUser.CountryCode, ReceivingCountry);

            bool IsValidBankDepositReceiver = Common.Common.IsValidBankDepositReceiver(ReceiverAccountNo, Service.BankAccount);

            if (IsValidBankDepositReceiver == false)
            {

                ErrorMessage = "Account no. not accepted";
                Common.FaxerSession.ErrorMessage = ErrorMessage;
                return false;
            }

            var Apiservice = Common.Common.GetApiservice(Common.FaxerSession.LoggedUser.CountryCode, ReceivingCountry, SendingAmount,
                        TransactionTransferMethod.BankDeposit, TransactionTransferType.Online);

            bool IsValidateAccountNo = true;
            if (!IsManualDeposit)
            {
                switch (Apiservice)
                {
                    case DB.Apiservice.VGG:
                        // validate Bank Account
                        BankDepositApi api = new BankDepositApi();
                        var accessToken = api.Login<AccessTokenVM>();

                        Common.FaxerSession.BankAccessToken = accessToken.Result;
                        if (accessToken.Result == null)
                        {
                            ErrorMessage = "Receiver's bank account number validation is taking longer than expected, please try again later!";
                            Common.FaxerSession.ErrorMessage = ErrorMessage;
                            return false;
                        }
                        if (accessToken.Result == null && string.IsNullOrEmpty(accessToken.Result.AccessToken))
                        {
                            ErrorMessage = "Receiver's bank account number validation is taking longer than expected, please try again later!";
                            Common.FaxerSession.ErrorMessage = ErrorMessage;
                            return false;
                        }

                        var validateAccountNo = api.ValidateAccountNo<AccountNoLookUpResponse>(
                             BankCode, ReceiverAccountNo, accessToken.Result);
                        IsValidateAccountNo = validateAccountNo.Result.status;
                        break;
                    case DB.Apiservice.TransferZero:
                        TransferZeroApi transferZeroApi = new TransferZeroApi();

                        AccountValidationRequest accountValidationRequest = new AccountValidationRequest(
                                        bankAccount: ReceiverAccountNo,
                                        bankCode: BankCode,
                                        country: (AccountValidationRequest.CountryEnum)Common.Common.getAccountValidationCountryCodeForTZ(ReceivingCountry),
                                        currency: (AccountValidationRequest.CurrencyEnum)Common.Common.getAccountValidationCountryCurrencyForTZ(ReceivingCountry),
                                        method: AccountValidationRequest.MethodEnum.Bank
                         );
                        var result = transferZeroApi.ValidateAccountNo(accountValidationRequest);

                        try
                        {

                            IsValidateAccountNo = result.Meta == null ? true : false;
                        }
                        catch (Exception)
                        {

                        }
                        break;
                    default:
                        break;


                }
            }

            if (!IsValidateAccountNo)
            {
                ErrorMessage = "Enter a validate account number";
                Common.FaxerSession.ErrorMessage = ErrorMessage;
                return false;
            }

            //var bankDepositModel = SaveIncompleteTransaction();
            //Common.FaxerSession.TransactionId = bankDepositModel.Id;


            return true;
        }

        public ServiceResult<List<ManualBankDepositViewModel>> GetAgentManualBankDeposit(int agentId = 0, int Year = 0, int Month = 0, int Day = 0)
        {
            var ManualDepositEnabledCountry = db.ManualDepositEnable.Where(x => x.IsEnabled == true && x.Agent == agentId.ToString()).Select(x => x.PayingCountry).FirstOrDefault();

            int payingStaffId = db.AgentStaffInformation.Where(x => x.AgentId == agentId).Select(x => x.Id).FirstOrDefault();

            var data = db.BankAccountDeposit.Where(x => x.ReceivingCountry == ManualDepositEnabledCountry && x.IsManualDeposit == true).ToList();
            if (Year != 0)
            {
                data = data.Where(x => x.TransactionDate.Year == Year).ToList();
            }
            if (Month != 0)
            {
                data = data.Where(x => x.TransactionDate.Month == Month).ToList();
            }
            if (Day != 0)
            {
                data = data.Where(x => x.TransactionDate.Day == Day).ToList();
            }


            var result = (from c in data.ToList()
                          join d in db.AgentInformation on c.ReceiverCountry equals d.CountryCode
                          where d.Id == agentId
                          select new ManualBankDepositViewModel()
                          {
                              Id = c.Id,
                              SendingCountry = Common.Common.GetCountryName(c.SendingCountry),
                              SenderName = GetSenderName(c.SenderId),
                              Amount = c.SendingAmount,
                              Fee = c.Fee,
                              TotalAmount = c.TotalAmount,
                              Status = c.Status,
                              StatusName = Common.Common.GetEnumDescription(c.Status),
                              ReceiverName = c.ReceiverName,
                              ReferenceNo = c.ReceiptNo,
                              TransactionDate = c.TransactionDate.ToShortDateString(),

                          }).ToList();

            return new ServiceResult<List<ManualBankDepositViewModel>>()
            {
                Data = result,
                Status = ResultStatus.OK
            };
        }

        internal SenderAndReceiverDetialVM GetSenderAndReceiverDetails()
        {
            SCashPickUpTransferService _cashPickUp = new SCashPickUpTransferService();
            SAgentBankAccountDeposit _agentBankAccountDepositServices = new SAgentBankAccountDeposit();

            var senderInfo = _cashPickUp.GetCashPickupInformationViewModel();
            var ReceiverInfo = _agentBankAccountDepositServices.GetAgentBankAccountDeposit();
            SenderAndReceiverDetialVM vm = new SenderAndReceiverDetialVM()
            {
                ReceiverCountry = ReceiverInfo.CountryCode,
                ReceiverMobileNo = ReceiverInfo.MobileNumber,
                SenderCountry = senderInfo.Country,
                SenderId = senderInfo.Id,

            };
            return vm;
        }

        internal SenderBankAccountDepositVm GetMobileMoneyTransferDetails(BankAccountDeposit bankAccountDeposit)
        {
            SAgentBankAccountDeposit _agentBankAccountDepositServices = new SAgentBankAccountDeposit();

            var ReceiverInfo = _agentBankAccountDepositServices.GetAgentBankAccountDeposit();

            SenderBankAccountDepositVm model = new SenderBankAccountDepositVm()
            {
                AccountOwnerName = ReceiverInfo.AccountOwnerName,
                AccountNumber = bankAccountDeposit.ReceiverAccountNo,
                BranchCode = bankAccountDeposit.BankCode,
                BankId = bankAccountDeposit.BankId,
                BankName = bankAccountDeposit.BankName,
                IsEuropeTransfer = bankAccountDeposit.IsEuropeTransfer,
                MobileNumber = bankAccountDeposit.ReceiverMobileNo,
                CountryCode = bankAccountDeposit.ReceivingCountry,
                ReasonForTransfer = bankAccountDeposit.ReasonForTransfer,
                IsManualDeposit = bankAccountDeposit.IsManualDeposit
            };
            return model;
        }

        internal IdentificationDetailModel GetIdentificationDetailViewModel(int senderId)
        {
            Areas.Admin.Services.CommonServices _CommonServices = new Areas.Admin.Services.CommonServices();
            var senderDocumentation = _CommonServices.GetSenderDocumentation(senderId);
            IdentificationDetailModel identificationDetailModel = (from c in senderDocumentation
                                                                   select new IdentificationDetailModel()
                                                                   {
                                                                       IdentificationTypeId = c.IdentificationTypeId,
                                                                       DocumentName = c.DocumentName,
                                                                       IdentityNumber = c.IdentityNumber,
                                                                       IssuingCountry = c.IssuingCountry,
                                                                       Status = c.Status,
                                                                       DocumentUrl = c.DocumentPhotoUrl,
                                                                       DocumentUrlTwo = c.DocumentPhotoUrlTwo,
                                                                       SenderBusinessDocumentationId = c.Id,
                                                                       Day = c.ExpiryDate.HasValue == true ? c.ExpiryDate.Value.Day : 0,
                                                                       Month = c.ExpiryDate.HasValue == true ? (Month)c.ExpiryDate.Value.Month : 0,
                                                                       Year = c.ExpiryDate.HasValue == true ? c.ExpiryDate.Value.Year : 0,
                                                                       ExpiryDate = c.ExpiryDate
                                                                   }).FirstOrDefault();

            return identificationDetailModel;
        }

        internal KiiPayTransferPaymentSummary GetKiiPayTransferPaymentSummary(BankAccountDeposit bankAccountDeposit)
        {
            KiiPayTransferPaymentSummary model = new KiiPayTransferPaymentSummary()
            {
                ReceivingAmount = bankAccountDeposit.ReceivingAmount,
            };
            return model;
        }




        public BankDepositResponseVm GetVGNApiResponse(string refNo = "", BankAccountDeposit bankAccountDeposit = null)
        {

            BankDepositApi bankDepositApi = new BankDepositApi();

            BankDepositLocalRequest bankDepositLocalRequest = new BankDepositLocalRequest();

            bankDepositLocalRequest = new BankDepositLocalRequest()
            {
                partnerTransactionReference = bankAccountDeposit.ReceiptNo,
                baseCurrencyCode = Common.Common.GetCurrencyCode(bankAccountDeposit.SendingCountry),
                targetCurrencyCode = Common.Common.GetCurrencyCode(bankAccountDeposit.ReceiverCountry),
                baseCurrencyAmount = bankAccountDeposit.SendingAmount,
                targetCurrencyAmount = bankAccountDeposit.ReceivingAmount,
                partnerCode = null,
                purpose = "Payment to" + bankAccountDeposit.ReceiverName,
                accountNumber = bankAccountDeposit.ReceiverAccountNo,
                bankCode = bankAccountDeposit.BankCode,
                baseCountryCode = bankAccountDeposit.SendingCountry,
                targetCountryCode = bankAccountDeposit.ReceiverCountry,
                payerName = Common.Common.GetSenderInfo(bankAccountDeposit.SenderId).FirstName,
                payermobile = "7440395950"

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

                try
                {

                    if (transactionResult.Result.result == null)
                    {

                        transactionResult.Result.result = transaction.Result;
                    }
                }
                catch (Exception ex)
                {
                    transactionResult.Result.result = transaction.Result;
                }

                return transactionResult.Result;
            }
            catch (Exception ex)
            {

                BankDepositResponseVm bankDepositResponseVm = new BankDepositResponseVm();
                bankDepositResponseVm.result = new BankDepositResponseResult();
                Log.Write(ex.Message, ErrorType.VGG, "GetVGNApiResponse");
                return bankDepositResponseVm;
            }


        }


        public TransferZero.Sdk.Model.Transaction GetBankDepositTransferZeroTransactionResponse(BankAccountDeposit accountDeposit)
        {


            var senderDetail = Common.Common.GetSenderInfo(accountDeposit.SenderId);
            var bankDetails = Common.Common.getBank(accountDeposit.BankId);
            string receivingCurrency = Common.Common.GetCountryCurrency(accountDeposit.ReceiverCountry);
            string sendingCurrency = Common.Common.GetCountryCurrency(accountDeposit.SendingCountry);
            Sender sender = new Sender(
                   //id: Guid.NewGuid(),
                   firstName: senderDetail.FirstName,
                   lastName: senderDetail.LastName,

                   phoneCountry: "GB",
                   phoneNumber: "02081445215",
                   country: senderDetail.Country,
                   city: senderDetail.City,
                   street: senderDetail.Address1,
                   postalCode: senderDetail.PostalCode,
                   addressDescription: "",

                   birthDate: senderDetail.DateOfBirth,

                   // you can usually use your company's contact email address here
                   email: senderDetail.Email.Trim(),

                   //externalId: Guid.NewGuid().ToString(),
                   externalId: senderDetail.AccountNo,

                   // you'll need to set these fields but usually you can leave them the default
                   ip: "127.0.0.1",
                   documents: new List<Document>()
          );
            PayoutMethodDetails details = new PayoutMethodDetails();
            string[] Name = accountDeposit.ReceiverName.Split(' ');
            string FirstName = "";
            string LastName = "";
            try
            {
                accountDeposit.ReceiverAccountNo = accountDeposit.ReceiverAccountNo.Trim();

                string[] nameArr = accountDeposit.ReceiverName.Trim().Split(' ');
                switch (nameArr.Length)
                {
                    case 1:
                        FirstName = nameArr[0];
                        break;
                    case 2:

                        FirstName = nameArr[0];
                        LastName = nameArr[1];
                        break;
                    case 3:

                        FirstName = nameArr[0];
                        LastName = nameArr[1] + " " + nameArr[2];
                        break;
                    case 4:
                        FirstName = nameArr[0];
                        LastName = nameArr[1] + " " + nameArr[2] + " " + nameArr[3];
                        break;
                    default:
                        break;
                }

            }
            catch (Exception)
            {
            }
            if (Common.Common.IsEuropeTransfer(accountDeposit.ReceivingCountry))
            {
                details = new PayoutMethodDetails(
                     firstName: FirstName,
                     lastName: LastName,
                     bankName: accountDeposit.BankName,
                     iban: accountDeposit.ReceiverAccountNo,
                     bic: accountDeposit.BankCode
                     );

            }
            else if (Common.Common.IsWestAfricanTransfer(accountDeposit.ReceivingCountry))
            {
                details = new PayoutMethodDetails(
                     firstName: FirstName,
                     lastName: LastName,
                     bankName: bankDetails.Name,
                     iban: accountDeposit.ReceiverAccountNo,
                     bankCountry: bankDetails.CountryCode
                     );
            }
            else if (Common.Common.IsSouthAfricanTransfer(accountDeposit.ReceivingCountry))
            {
                var recipent = db.Recipients.Where(x => x.Id == accountDeposit.RecipientId).FirstOrDefault();
                var countryPhoneCode = Common.Common.GetCountryPhoneCode(accountDeposit.ReceivingCountry);
                details = new PayoutMethodDetails(
                       firstName: FirstName,
                       lastName: LastName,
                        //bankName: bankDetails.Name,
                        bankAccount: accountDeposit.ReceiverAccountNo,
                 bankCode: accountDeposit.BankCode,
                 phoneNumber: countryPhoneCode + accountDeposit.ReceiverMobileNo,
                 city: recipent.City,
                 email: recipent.Email,
                 street: recipent.Street, // should include house number as well
                 postalCode: recipent.PostalCode,
                 transferReasonCode: "185"
                       );
            }
            else
            {
                details = new PayoutMethodDetails(
                     firstName: FirstName,
                     lastName: LastName,
                     //mobileProvider: PayoutMethodMobileProviderEnum.Orange,
                     //phoneNumber: "7087661234"
                     bankAccount: accountDeposit.ReceiverAccountNo,
                     bankCode: accountDeposit.BankCode,
                     bankAccountType: PayoutMethodBankAccountTypeEnum._20
                     );
            }

            PayoutMethod payout = new PayoutMethod(
              type: receivingCurrency + "::Bank",
              details: details);

            Recipient recipient = new Recipient(
              requestedAmount: accountDeposit.ReceivingAmount,
              requestedCurrency: receivingCurrency,
              payoutMethod: payout);


            Transaction transaction = new Transaction(
              sender: sender,
              recipients: new List<Recipient>() { recipient },
              inputCurrency: sendingCurrency,
              externalId: accountDeposit.ReceiptNo);
            TransactionRequest request = new TransactionRequest(
                transaction: transaction);
            TransferZeroApi transferZeroApi = new TransferZeroApi();
            TransactionError transactionError = new TransactionError()
            {
                ReceiptNo = accountDeposit.ReceiptNo,
                TransactionId = accountDeposit.Id,
                TransferMethod = TransactionTransferMethod.BankDeposit,

            };
            transferZeroApi.transactionError = transactionError;
            var result = transferZeroApi.CreateTransaction(request);
            var resultStatus = transferZeroApi.GetTransactionStatus(result.Object.ExternalId);

            return resultStatus;
        }

        public BankDepositResponseVm PrepareTransferZeroBankDepositResponse(Transaction transaction)
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


        public EmergentApiGetStatusResponseModel GetEmergentApiBankAccountDepositApiResponse(BankAccountDeposit item)
        {

            var senderDetail = Common.Common.GetSenderInfo(item.SenderId);

            string receivingCurrency = Common.Common.GetCountryCurrency(item.ReceiverCountry);
            string sendingCurrency = Common.Common.GetCountryCurrency(item.SendingCountry);

            string[] receiverPhoneCode = Common.Common.GetCountryPhoneCode(item.ReceiverCountry).Split('+');

            EmergentApi emergentApi = new EmergentApi();
            EmergentApiRequestParamModel paramModel = new EmergentApiRequestParamModel()
            {

                transaction_date = "/Date(" + DateTime.Now.ToString("yyyy-MM-dd") + ")/",
                expiry_date = "/Date(" + DateTime.Now.ToString("yyyy-MM-dd") + ")/",
                transaction_type = "local",
                payment_mode = "BAT",
                payee_name = senderDetail.FirstName + " " + senderDetail.LastName,
                payee_email = senderDetail.Email,
                trans_currency = "GHS",
                trans_amount = item.ReceivingAmount,
                merch_trans_ref_no = item.ReceiptNo,
                payee_mobile = senderDetail.PhoneNumber,
                recipient_mobile = "",
                recipient_name = item.ReceiverName,
                bank_branch_sort_code = item.BankCode,
                bank_account_no = item.ReceiverAccountNo,
                bank_account_title = item.ReceiverName,
                bank_name = Common.Common.getBankName(item.BankId)
            };
            var result = emergentApi.CreateTransaction<EmergentApiTransactionResponseModel>(CommonExtension.SerializeObject(paramModel));

            EmergentApiGetTransactionStatusParamModel statusModel = new EmergentApiGetTransactionStatusParamModel()
            {


                merch_trans_ref_no = paramModel.merch_trans_ref_no
            };
            var status = emergentApi.GetTransactionStatus<EmergentApiGetStatusResponseModel>(CommonExtension.SerializeObject(statusModel));
            return status.Result;
        }

        public BankDepositResponseVm PrepareEmergentApiBankDepositResponse(EmergentApiGetStatusResponseModel emergentApiResponse)
        {

            BankDepositResponseVm bankDepositResponse = new BankDepositResponseVm()
            {
                extraResult = emergentApiResponse.trans_ref_no,
                status = true,
                response = (int)emergentApiResponse.status_code,
                result = new BankDepositResponseResult()
                {

                    transactionStatus = (int)emergentApiResponse.status_code,
                    transactionReference = emergentApiResponse.trans_ref_no,
                    transactiondate = emergentApiResponse.payment_date == null ? "" : emergentApiResponse.payment_date,
                    transactionStatusDescription = emergentApiResponse.status_message
                }

            };
            return bankDepositResponse;

        }

        public BankTransactionApiResponse CreateBankTransactionToApi(BankAccountDeposit item, TransactionTransferType TransactionTransferType = TransactionTransferType.Online)
        {


            SManualApprovalTransactionCountry manualApprovalTransactionServices = new SManualApprovalTransactionCountry();

            bool IsManualApproveTransaction = manualApprovalTransactionServices.IsManaulApprovalTran(
                item.ReceiverCountry, item.ReceivingCurrency, TransactionTransferMethod.BankDeposit);
            if (IsManualApproveTransaction)
            {

                //item.IsComplianceNeededForTrans = true;
                item.IsManualApproveNeeded = true;
                item.Status = BankDepositStatus.Incomplete;
                Update(item);
                return new BankTransactionApiResponse()
                {
                    BankAccountDeposit = item,
                    BankDepositApiResponseVm = new BankDepositResponseVm()
                };
            }

            if (item.IsComplianceApproved == false && item.PaidFromModule == Module.Faxer)
            {

                if (item.SenderPaymentMode == SenderPaymentMode.CreditDebitCard ||
                    item.SenderPaymentMode == SenderPaymentMode.SavedDebitCreditCard)
                {
                    try
                    {

                        var TransactionLimitAmount = Common.Common.HasExceededAmountLimit(item.SenderId, item.SendingCountry, item.ReceivingCountry, item.SendingAmount, Module.Faxer);
                        if (TransactionLimitAmount)
                        {
                            item.IsComplianceNeededForTrans = true;
                            item.Status = BankDepositStatus.Held;
                            Update(item);
                            return new BankTransactionApiResponse()
                            {
                                BankAccountDeposit = item,
                                BankDepositApiResponseVm = new BankDepositResponseVm()
                            };

                        }
                    }
                    catch (Exception)
                    {

                    }

                }
            }

            if (item.PaidFromModule == Module.Faxer || item.PaidFromModule == Module.Agent)
            {
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

            }

            int agentId = 0;
            if (item.PaidFromModule == Module.Agent)
            {
                agentId = Common.Common.GetAgentIdByPayingId(item.PayingStaffId);
            }
            var ApiService = Common.Common.GetApiservice(item.SendingCountry,
             item.ReceiverCountry, item.SendingAmount, TransactionTransferMethod.BankDeposit, TransactionTransferType, agentId);
            var bankdepositTransactionResult = new BankDepositResponseVm();
            SSenderForAllTransfer senderForAllTransfer = new SSenderForAllTransfer();
            switch (ApiService)
            {
                case DB.Apiservice.VGG:

                    bankdepositTransactionResult = GetVGNApiResponse("", item);
                    var transcationStatus = bankdepositTransactionResult.result.transactionStatus;

                    item.TransferReference = bankdepositTransactionResult.result.partnerTransactionReference;
                    if (transcationStatus == 1)
                    {
                        item.Status = BankDepositStatus.Incomplete;
                    }
                    else if (transcationStatus == 2)
                    {
                        item.Status = BankDepositStatus.Confirm;
                    }
                    else if (transcationStatus == 3)
                    {
                        item.Status = BankDepositStatus.Failed;
                    }
                    else if (transcationStatus == 0)
                    {
                        item.Status = BankDepositStatus.Incomplete;
                    }
                    break;
                case DB.Apiservice.TransferZero:

                    //string TransactionId = Guid.NewGuid().ToString();
                    string TransactionId = item.ReceiptNo;
                    var transferZeroResponse = GetBankDepositTransferZeroTransactionResponse(item);
                    var status = Common.Common.GetTransferZeroTransactionStatus(transferZeroResponse);
                    var transferZeroTransactionResult = transferZeroResponse;
                    var responseModel = PrepareTransferZeroBankDepositResponse(transferZeroTransactionResult);
                    responseModel.result.beneficiaryAccountName = item.ReceiverAccountNo;
                    responseModel.result.beneficiaryBankCode = item.BankCode;
                    responseModel.result.beneficiaryAccountName = item.ReceiverName;
                    responseModel.result.amountInBaseCurrency = item.SendingAmount;
                    responseModel.result.targetAmount = item.ReceivingAmount;
                    responseModel.result.partnerTransactionReference = item.ReceiptNo;
                    bankdepositTransactionResult = responseModel;

                    item.Status = status;
                    item.TransferReference = transferZeroResponse.Id.ToString();
                    try
                    {
                        item.TransferZeroSenderId = transferZeroResponse.Sender.Id.ToString();
                    }
                    catch (Exception ex)
                    {



                        Log.Write(ex.Message, ErrorType.TransferZero, "CreateBankTransactionToApi");
                    }
                    break;

                case Apiservice.EmergentApi:
                    try
                    {
                        var EmergentApiResponse = GetEmergentApiBankAccountDepositApiResponse(item);
                        status = BankDepositStatus.Incomplete;
                        switch (EmergentApiResponse.status_code)
                        {
                            case 1: // Success
                                status = BankDepositStatus.Confirm;
                                break;
                            case 2: // Pending

                                status = BankDepositStatus.Incomplete;
                                break;
                            case 3: // Expired

                                status = BankDepositStatus.Failed;
                                break;
                            case 4: // Reversed
                                status = BankDepositStatus.Cancel;
                                break;
                            case 5: //In Clearing
                                status = BankDepositStatus.Incomplete;
                                break;
                            default:
                                break;
                        }
                        responseModel = PrepareEmergentApiBankDepositResponse(EmergentApiResponse);
                        responseModel.result.beneficiaryAccountName = item.ReceiverAccountNo;
                        responseModel.result.beneficiaryBankCode = item.BankCode;
                        responseModel.result.beneficiaryAccountName = item.ReceiverName;
                        responseModel.result.amountInBaseCurrency = item.SendingAmount;
                        responseModel.result.targetAmount = item.ReceivingAmount;
                        responseModel.result.partnerTransactionReference = item.ReceiptNo;
                        bankdepositTransactionResult = responseModel;

                        item.Status = status;
                        item.TransferReference = responseModel.result.transactionReference;
                    }
                    catch (Exception ex)
                    {
                        Log.Write(ex.Message, ErrorType.EmergentApi, "CreateBankTransactionToApi");
                    }
                    break;
                case Apiservice.Zenith:
                    try
                    {
                        ZenithApi zenithApi = new ZenithApi();
                        var zenithResponse = zenithApi.CreateBankDepositTransaction(item);

                        BankDepositResponseVm bankDepositResponse = new BankDepositResponseVm()
                        {
                            extraResult = zenithResponse.TransRefNo,
                            status = true,
                            response = (int)zenithResponse.Status,
                            result = new BankDepositResponseResult()
                            {
                                transactionStatus = (int)zenithResponse.Status,
                                transactionReference = zenithResponse.TransRefNo,
                                transactiondate = zenithResponse.TransactionDate.ToString("dd/MM/yyyy")
                            }
                        };
                        bankDepositResponse.result.beneficiaryAccountName = item.ReceiverAccountNo;
                        bankDepositResponse.result.beneficiaryBankCode = item.BankCode;
                        bankDepositResponse.result.beneficiaryAccountName = item.ReceiverName;
                        bankDepositResponse.result.amountInBaseCurrency = item.SendingAmount;
                        bankDepositResponse.result.targetAmount = item.ReceivingAmount;
                        bankDepositResponse.result.partnerTransactionReference = item.ReceiptNo;
                        bankdepositTransactionResult = bankDepositResponse;
                        item.Status = zenithResponse.Status;
                        item.TransferReference = zenithResponse.TransRefNo;
                    }
                    catch (Exception ex)
                    {
                        Log.Write(ex.Message, ErrorType.Zenith, "CreateBankTransactionToApi");
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
                        var postResponse = GetBankDepositCashPotTransactionResponse(item);
                        var statusResonse = GetStatusResponse(postResponse.REFERENCE_CODE);
                        item.Status = GetCashPotTransactionStatus(statusResonse.STATUS_CODE);


                        BankDepositResponseVm bankDepositResponse = new BankDepositResponseVm()
                        {
                            extraResult = postResponse.REFERENCE_CODE,
                            status = true,
                            response = (int)item.Status,
                            result = new BankDepositResponseResult()
                            {
                                transactionStatus = (int)item.Status,
                                transactionReference = postResponse.REFERENCE_CODE,
                                transactiondate = postResponse.DATE,
                                beneficiaryAccountName = item.ReceiverName,
                                beneficiaryBankCode = item.BankCode,
                                amountInBaseCurrency = item.SendingAmount,
                                targetAmount = item.ReceivingAmount,
                                partnerTransactionReference = item.ReceiptNo,
                            }
                        };
                        bankdepositTransactionResult = bankDepositResponse;
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
                        FlutterWaveVm vm = BindToFlutterCommonCustomerDetailsVm(item);
                        var postResponse = sSenderForAllTransfer.CreateFlutterWaveTransaction(vm);
                        if (postResponse != null)
                        {
                            var getTrasactionById = sSenderForAllTransfer.GetFlutterWaveTransactionById(postResponse.data.id);
                            //var statusResponse = sSenderForAllTransfer.GetFlutterWaveStatus(postResponse.data.txRef);
                            item.Status = GetBankStatusFromFWResponse(getTrasactionById.data.status);
                            BankDepositResponseVm bankDepositResponse = new BankDepositResponseVm()
                            {
                                extraResult = postResponse.data.id.ToString(),
                                status = true,
                                response = (int)item.Status,
                                result = new BankDepositResponseResult()
                                {
                                    transactionStatus = (int)item.Status,
                                    transactionReference = postResponse.data.id.ToString(),
                                    transactiondate = DateTime.Now.ToString(),
                                    beneficiaryAccountName = item.ReceiverName,
                                    beneficiaryBankCode = item.BankCode,
                                    amountInBaseCurrency = item.SendingAmount,
                                    targetAmount = item.ReceivingAmount,
                                    partnerTransactionReference = item.ReceiptNo,
                                }
                            };
                            bankdepositTransactionResult = bankDepositResponse;
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

            return new BankTransactionApiResponse()
            {

                BankAccountDeposit = item,
                BankDepositApiResponseVm = bankdepositTransactionResult,
            };
        }

        private BankDepositStatus GetBankStatusFromFWResponse(string status)
        {
            BankDepositStatus bankDepositStatus = BankDepositStatus.Incomplete;
            switch (status.ToLower())
            {
                case "successful":
                    bankDepositStatus = BankDepositStatus.Confirm;
                    break;
                case "pending":
                    bankDepositStatus = BankDepositStatus.Incomplete;
                    break;
                case "failed":
                    bankDepositStatus = BankDepositStatus.Failed;
                    break;
            }
            return bankDepositStatus;
        }

        private FlutterWaveVm BindToFlutterCommonCustomerDetailsVm(BankAccountDeposit item)
        {
            FlutterWaveApi webapi = new FlutterWaveApi();
            var recipient = db.Recipients.Where(x => x.Id == item.RecipientId).FirstOrDefault();
            var senderInfo = db.FaxerInformation.Where(x => x.Id == item.SenderId).FirstOrDefault();
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
                account_bank = item.BankCode,
                account_number = item.ReceiverAccountNo,
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


        private decimal GetRateCashPot(BankAccountDeposit item)
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
                TRANS_TYPE_ID = "2",
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

        private SendTransGenericResponseVm GetBankDepositCashPotTransactionResponse(BankAccountDeposit item)
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
                PAYER_ID = "10258",
                SEND_AMOUNT = item.SendingAmount.ToString(),
                SENDER_FIRST_NAME = senderInfo.FirstName,
                SENDER_LAST_NAME = senderInfo.MiddleName != null ? senderInfo.MiddleName + " " + senderInfo.LastName : senderInfo.LastName,
                SENDER_ADDRESS = senderInfo.Address1,
                SENDER_COUNTRY = senderInfo.Country,
                RECEIVER_FIRST_NAME = FirstName,
                RECEIVER_LAST_NAME = LastName,
                RECEIVER_MOBILE_NUMBER = item.ReceiverMobileNo,
                RECEIVING_AMOUNT = item.ReceivingAmount.ToString(),
                RECEIVER_ADDRESS = "",
                RECEIVER_CITY = "",
                RECEIVER_COUNTRY = item.ReceivingCountry,
                TRANSACTION_TYPE = "2",//Bank transfer transaction type = 2
                RECEIVER_BANK_NAME = db.Bank.Where(x => x.Id == item.BankId).Select(x => x.Name).FirstOrDefault(),
                RECEIVER_BANK_ACCOUNT_NO = item.ReceiverAccountNo,
                RECEIVER_BANK_CODE = item.BankCode,
                RECEIVER_BRANCH_CODE = item.BankCode,
                RECEIVER_BANK_IBAN = item.ReceiverAccountNo,
                RECEIVER_BANK_SWIFT = item.BankCode,
                RECEIVER_BANK_ACCOUNT_TITLE = item.ReceiverName,
                SECRET_ANSWER = item.ReceiptNo,
                SECRET_QUESTION = "What is your pickup reference number ?",
                SENDER_DOB = senderInfo.DateOfBirth.ToString(),
                SENDER_POST_CODE = senderInfo.PostalCode,
                SITE_LOCATION = ""

            };
            var senderDoument = db.SenderBusinessDocumentation.Where(x => x.SenderId == item.SenderId).FirstOrDefault();
            if (senderDoument != null)
            {
                sendTransGenericRequest.SENDER_ID_NUMBER = senderDoument.IdentityNumber;
                sendTransGenericRequest.SENDER_ID_ISSUE_DATE = "";
                sendTransGenericRequest.SENDER_ID_EXPIRY_DATE = senderDoument.ExpiryDate.ToString();
            }
            var response = cashPotApi.PostTransaction(sendTransGenericRequest);
            return response;
        }


        public BankDepositStatus GetCashPotTransactionStatus(string StatusCode)
        {
            BankDepositStatus status = BankDepositStatus.Incomplete;

            if (StatusCode == "1" || StatusCode == "2" || StatusCode == "3" ||
                StatusCode == "5" || StatusCode == "13" || StatusCode == "15" ||
                StatusCode == "16" || StatusCode == "17" || StatusCode == "18" ||
                StatusCode == "19" || StatusCode == "20")
            {
                status = BankDepositStatus.Incomplete;
            }
            else if (StatusCode == "6" || StatusCode == "7")
            {
                status = BankDepositStatus.Cancel;
            }
            else if (StatusCode == "10")
            {
                status = BankDepositStatus.Confirm;
            }
            else if (StatusCode == "14")
            {
                status = BankDepositStatus.FullRefund;
            }
            return status;
        }

        public ServiceResult<bool> IsValidBankAccount(SenderBankAccountDepositVm model, decimal SendingAmount, string SendingCountry,
            TransactionTransferType TransactionTransferType = TransactionTransferType.Online, int agentId = 0)
        {
            var result = new ServiceResult<bool>();
            result.Data = true;
            var Apiservice = Common.Common.GetApiservice(SendingCountry, model.CountryCode, SendingAmount,
                        TransactionTransferMethod.BankDeposit, TransactionTransferType, agentId);
            switch (Apiservice)
            {
                case DB.Apiservice.VGG:
                    // validate Bank Account
                    BankDepositApi api = new BankDepositApi();
                    var accessToken = api.Login<AccessTokenVM>();

                    Common.FaxerSession.BankAccessToken = accessToken.Result;
                    if (accessToken.Result == null)
                    {
                        result.Data = false;
                        result.Message = "Receiver's bank account number validation is taking longer than expected, please try again later!";
                        return result;
                    }
                    if (accessToken.Result == null && string.IsNullOrEmpty(accessToken.Result.AccessToken))
                    {

                        result.Data = false;
                        result.Message = "Receiver's bank account number validation is taking longer than expected, please try again later!";
                        return result;
                    }

                    var validateAccountNo = api.ValidateAccountNo<AccountNoLookUpResponse>(
                         model.BranchCode, model.AccountNumber, accessToken.Result);
                    var IsValidateAccountNo = validateAccountNo.Result.status;
                    if (!IsValidateAccountNo)
                    {
                        result.Data = false;
                        result.Message = "Enter valid account no";
                        return result;
                    }
                    break;
                case DB.Apiservice.TransferZero:
                    TransferZeroApi transferZeroApi = new TransferZeroApi();

                    if (model.CountryCode == "GH" && model.CountryCode == "NG")
                    {
                        AccountValidationRequest accountValidationRequest = new AccountValidationRequest(
                                        bankAccount: model.AccountNumber,
                                        bankCode: model.BranchCode,
                                        country: (AccountValidationRequest.CountryEnum)Common.Common.getAccountValidationCountryCodeForTZ(model.CountryCode),
                                        currency: (AccountValidationRequest.CurrencyEnum)Common.Common.getAccountValidationCountryCurrencyForTZ(model.CountryCode),
                                        method: AccountValidationRequest.MethodEnum.Bank
                         );
                        var transferZeroresult = transferZeroApi.ValidateAccountNo(accountValidationRequest);
                        IsValidateAccountNo = transferZeroresult.Meta == null ? true : false;
                        if (!IsValidateAccountNo)
                        {

                            result.Data = false;
                            result.Message = "Enter valid account no";
                            return result;

                        }
                    }
                    break;
                case DB.Apiservice.Zenith:

                    ZenithApi zenithApi = new ZenithApi();
                    var zenithApiResponse = zenithApi.IsValidAccount(new ZenithTransferVerifyAccountModel()
                    {
                        TargetAccountNo = model.AccountNumber,
                        DestSortCode = model.BranchCode
                    });
                    if (zenithApiResponse.Data == false)
                    {
                        result.Data = false;
                        result.Message = zenithApiResponse.Message;
                        return result;
                    }

                    break;
                default:
                    result.Data = false;
                    result.Message = "Service Not Avialable";
                    break;
            }
            return result;

        }



        #endregion

        #region email and sms 
        public void SendEmailAndSms(BankAccountDeposit item)
        {

            switch (item.Status)
            {
                case BankDepositStatus.Held:
                    SetTransactionEmailType(TransactionEmailType.TransactionInProgress);
                    TransactionInProgressEmail(item, true);
                    break;
                case BankDepositStatus.UnHold:
                    break;
                case BankDepositStatus.Cancel:

                    SetTransactionEmailType(TransactionEmailType.TransactionCancelled);
                    TransactionCancelled(item);
                    break;
                case BankDepositStatus.Confirm:

                    SetTransactionEmailType(TransactionEmailType.TransactionCompleted);
                    SendTransactionCompletedSms(item);
                    TransactionCompletedEmail(item);
                    break;
                case BankDepositStatus.Incomplete:

                    SetTransactionEmailType(TransactionEmailType.TransactionInProgress);
                    if (item.IsManualDeposit == true)
                    {
                        ManualBankDepositEmail(item);
                    }
                    else
                    {
                        SendTransactionInProgressSms(item);
                        TransactionInProgressEmail(item);
                    }
                    break;
                case BankDepositStatus.Failed:
                    break;
                case BankDepositStatus.PaymentPending:
                    break;
                case BankDepositStatus.IdCheckInProgress:

                    SetTransactionEmailType(TransactionEmailType.IDCheck);
                    IdCheckInProgress(item);
                    break;
                case BankDepositStatus.PendingBankdepositConfirmtaion:


                    SetTransactionEmailType(TransactionEmailType.TransactionInProgress); ;
                    //SendTransactionInProgressSms(item);
                    TransactionInProgressEmail(item);

                    break;
                case BankDepositStatus.ReInitialise:
                    break;
                case BankDepositStatus.FullRefund:


                    SetTransactionEmailType(TransactionEmailType.TransactionCompleted);
                    TransactionRefundEmail(item);
                    break;
                case BankDepositStatus.PartailRefund:

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

        //public void TransactionRefundEmail(BankAccountDeposit item)
        //{
        //    // Sending Email Code goes here 
        //    var senderInfo = Common.Common.GetSenderInfo(item.SenderId);
        //    string email = senderInfo.Email;
        //    MailCommon mail = new MailCommon();
        //    var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
        //    string body = "";
        //    string bankName = item.BankId == 0 ? item.BankName : GetBankName(item.BankId);
        //    string sendingCountryCurrency = Common.Common.GetCountryCurrency(item.SendingCountry);
        //    string receivingCountryCurrecy = Common.Common.GetCountryCurrency(item.ReceiverCountry);
        //    string ReceivingCountryName = Common.Common.GetCountryName(item.ReceiverCountry);
        //    string receiverFirstName = "";
        //    try
        //    {
        //        receiverFirstName = item.ReceiverName.Split(' ')[0];
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    var refundHistory = db.RefundHistory.Where(x => x.TransactionId == item.Id && x.TransactionServiceType == TransactionServiceType.BankDeposit).FirstOrDefault();
        //    if (refundHistory != null)
        //    {
        //        body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/RefundIssued/Index?" +
        //            "&SenderFristName=" + senderInfo.FirstName +
        //            "&SendingCurrency=" + sendingCountryCurrency +
        //            "&RefundAmount=" + refundHistory.RefundedAmount +
        //            "&TransactionNumber=" + item.ReceiptNo +
        //            "&SendingAmount=" + item.SendingAmount +
        //            "&SendingCountry=" + item.SendingCountry +
        //            "&Fee=" + item.Fee +
        //            "&ReceiverFirstName=" + receiverFirstName +
        //            "&ReceivingCurrency=" + receivingCountryCurrecy +
        //            "&ReceivingAmount=" + item.ReceivingAmount +
        //            "&BankName=" + bankName +
        //            "&BankAccount=" + item.ReceiverAccountNo +
        //            "&BranchCode=" + item.BankCode +
        //            "&transactionServiceType=" + TransactionServiceType.BankDeposit +
        //            "&WalletName=" + "" +
        //            "&MobileNo=" + item.ReceiverMobileNo);
        //        mail.SendMail(email, "Refund in progress", body);
        //    }
        //}

        public void TransactionRefundEmail(BankAccountDeposit item)
        {
            // Sending Email Code goes here 
            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);
            string email = senderInfo.Email;

            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string bankName = item.BankId == 0 ? item.BankName : GetBankName(item.BankId);

            string sendingCountryCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(item.SendingCurrency, item.SendingCountry);
            string receivingCountryCurrecy = Common.Common.GetCurrencyByCurrencyOrCountry(item.ReceivingCurrency, item.ReceiverCountry);


            string ReceivingCountryName = Common.Common.GetCountryName(item.ReceiverCountry);
            string receiverFirstName = "";
            try
            {
                receiverFirstName = item.ReceiverName.Split(' ')[0];
            }
            catch (Exception)
            {
            }
            var refundHistory = db.RefundHistory.Where(x => x.TransactionId == item.Id && x.TransactionServiceType == TransactionServiceType.BankDeposit).FirstOrDefault();

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
                    "&BankAccount=" + item.ReceiverAccountNo +
                    "&BranchCode=" + item.BankCode +
                    "&transactionServiceType=" + TransactionServiceType.BankDeposit +
                    "&WalletName=" + "" +
                    "&MobileNo=" + item.ReceiverMobileNo);
                mail.SendMail(email, "Refund Issued -" + item.ReceiptNo, body);
            }
        }
        public void TransactionCompletedEmail(BankAccountDeposit item)
        {

            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);
            string email = senderInfo.Email;

            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string bankName = item.BankId == 0 ? item.BankName : GetBankName(item.BankId);
            string sendingCountryCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(item.SendingCurrency, item.SendingCountry);
            string receivingCountryCurrecy = Common.Common.GetCurrencyByCurrencyOrCountry(item.ReceivingCurrency, item.ReceiverCountry);


            string ReceivingCountryName = Common.Common.GetCountryName(item.ReceiverCountry);
            string receiverFirstName = "";
            try
            {
                receiverFirstName = item.ReceiverName.Split(' ')[0];
            }
            catch (Exception)
            {
            }

            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/BankDepositEmail/Index?" +
                 "&fee=" + item.Fee + "&sendingAmount=" + item.SendingAmount +
                 "&sendingCurrency=" + sendingCountryCurrency + "&receiverAccountNo=" + item.ReceiverAccountNo
                 + "&ReceiveingCurrency=" + receivingCountryCurrecy + "&receivingAmount=" + item.ReceivingAmount +
                 "&receiverName=" + item.ReceiverName + "&receiptNo=" + item.ReceiptNo
                 + "&bankName=" + bankName + "&senderName=" + senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName +
                 "&bankCode=" + item.BankCode + "&receivingCountry=" + ReceivingCountryName
                 + "&status=" + item.Status);
            mail.SendMail(email, "Bank Account Deposit", body);

            Log.Write(email, ErrorType.UnSpecified, "Bank Deposit Confirmation");
        }
        public void TransactionInProgressEmail(BankAccountDeposit item, bool IstransactionHeld = false)
        {

            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);
            string email = senderInfo.Email;

            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string bankName = item.BankId == 0 ? item.BankName : GetBankName(item.BankId);
            string sendingCountryCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(item.SendingCurrency, item.SendingCountry);
            string receivingCountryCurrecy = Common.Common.GetCurrencyByCurrencyOrCountry(item.ReceivingCurrency, item.ReceiverCountry);
            string ReceivingCountryName = Common.Common.GetCountryName(item.ReceiverCountry);
            string receiverFirstName = "";
            try
            {
                receiverFirstName = item.ReceiverName.Split(' ')[0];

                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ManualBankDepositEmail?" +
                 "&fee=" + item.Fee + "&sendingAmount=" + item.SendingAmount + "&sendingCurrency=" + sendingCountryCurrency +
                 "&receiverAccountNo=" + item.ReceiverAccountNo
                 + "&ReceiveingCurrency=" + receivingCountryCurrecy + "&receivingAmount=" + item.ReceivingAmount +
                 "&receiverName=" + item.ReceiverName + "&receiptNo=" + item.ReceiptNo
                 + "&bankName=" + bankName + "&senderName=" + senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName +
                 "&bankCode=" + item.BankCode + "&receivingCountry=" + ReceivingCountryName +
                 "&receiverFirstName=" + receiverFirstName
                 + "&paymentReference=" + item.PaymentReference + "&SenderPaymentMode=" + item.SenderPaymentMode + "&IsTransactionHeld=" + IstransactionHeld);
                mail.SendMail(email, "Confirmation of transfer to" + " " + item.ReceiverName, body);
                Log.Write(email + " Bank Deposit In Progress email", ErrorType.UnSpecified, "Bank InProgress");
            }
            catch (Exception)
            {

                Log.Write(email + " Cannot Send Bank Deposit In Progress email", ErrorType.UnSpecified, "Bank InProgress");
            }


        }

        public void ManualBankDepositEmail(BankAccountDeposit item)
        {
            TransactionInProgressEmail(item);
            SendTransactionInProgressSms(item);
            SendManualBankDepositSmsToAgent(item);
        }
        public void IdCheckInProgress(BankAccountDeposit item)
        {

            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);

            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string bankName = item.BankId == 0 ? item.BankName : GetBankName(item.BankId);

            string sendingCountryCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(item.SendingCurrency, item.SendingCountry);
            string receivingCountryCurrecy = Common.Common.GetCurrencyByCurrencyOrCountry(item.ReceivingCurrency, item.ReceiverCountry);

            string ReceivingCountryName = Common.Common.GetCountryName(item.ReceiverCountry);
            string receiverFirstName = "";
            try
            {
                receiverFirstName = item.ReceiverName.Split(' ')[0];
            }
            catch (Exception)
            {
            }
            string WalletName = "";
            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/TransactionPaused?" +
                "&SenderFristName=" + senderInfo.FirstName + "&TransactionNumber=" + item.ReceiptNo
                + "&SendingAmount=" + item.SendingAmount + "&ReceivingAmount=" + item.ReceivingAmount +
                "&Receivingcountry=" + ReceivingCountryName + "&Fee=" + item.Fee
                + "&ReceiverFirstName=" + receiverFirstName + "&BankName=" + bankName +
                "&BankAccount=" + item.ReceiverAccountNo + "&BankCode=" + item.BankCode
                + "&TransactionServiceType=" + TransactionServiceType.BankDeposit + "&WalletName=" + WalletName + "&MFCN=" + "");


            mail.SendMail(senderInfo.Email, "Your transfer has been paused", body);
        }
        public void TransactionCancelled(BankAccountDeposit item)
        {

            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);

            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string bankName = item.BankId == 0 ? item.BankName : Common.Common.getBankName(item.BankId);
            string ReceiverCountryName = Common.Common.GetCountryName(item.ReceiverCountry);

            string SendingCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(item.SendingCurrency, item.SendingCountry);
            string ReceivingCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(item.ReceivingCurrency, item.ReceiverCountry);

            string WalletName = "";

            //body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/TransactionCancelled?" +
            //     "&SenderFristName=" + senderInfo.FirstName + "&TransactionNumber=" + item.ReceiptNo +
            //     "&SendingAmount=" + item.SendingAmount + "&Receivingcountry=" + ReceiverCountryName
            //     + "&Fee=" + item.Fee + "&ReceiverName=" + item.ReceiverName + "&BankName=" + bankName +
            //     "&BankAccount=" + item.ReceiverAccountNo
            //     + "&BankCode=" + item.BankCode + "&TransactionServiceType=" +
            //     TransactionServiceType.BankDeposit + "&WalletName=" + WalletName + "&MFCN=" + ""
            //     + "&SendingCurrency=" + SendingCurrency + "&ReceivingCurrency=" + ReceivingCurrency);

            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/TransferedCancelledByTZ?" +
                 "&SenderFristName=" + senderInfo.FirstName +
                 "&TransactionNumber=" + item.ReceiptNo +
                 "&RecipentName=" + item.ReceiverName +
                 "&BankName=" + bankName +
                 "&BankAccount=" + item.ReceiverAccountNo +
                 "&ReceiverCountry=" + ReceiverCountryName +
                 "&transferMethod=" + TransactionTransferMethod.BankDeposit +
                 "&WalletName=" + "" +
                 "&MobileNo=" + item.ReceiverMobileNo
                 );

            mail.SendMail(senderInfo.Email, "Money Transfer Cancelled" + " " + item.ReceiptNo, body);

        }


        public void TransactionCancelledByMoneyFex(BankAccountDeposit item)
        {

            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);

            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string bankName = item.BankId == 0 ? item.BankName : Common.Common.getBankName(item.BankId);
            string ReceiverCountryName = Common.Common.GetCountryName(item.ReceiverCountry);

            string SendingCurrency = Common.Common.GetCountryName(item.SendingCountry);
            string ReceivingCurrency = Common.Common.GetCountryName(item.ReceiverCountry);

            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/TransactionCancelledByMoneyFex?" +
                 "&SenderFristName=" + senderInfo.FirstName +
                 "&TransactionNumber=" + item.ReceiptNo +
                 "&RecipentName=" + item.ReceiverName +
                 "&BankName=" + bankName +
                 "&BankAccount=" + item.ReceiverAccountNo +
                 "&ReceiverCountry=" + ReceiverCountryName +
                 "&receivingCurrency=" + ReceivingCurrency +
                 "&sendingCurrency=" + SendingCurrency +
                 "&exchangeRate=" + item.ExchangeRate +
                 "&fee=" + item.Fee +
                 "&receivingAmount=" + item.ReceivingAmount +
                 "&sendingAmount=" + item.SendingAmount +
                 "&bankCode=" + item.BankCode +
                 "&transferMethod=" + TransactionTransferMethod.BankDeposit +
                 "&walletName=" + "" +
                 "&mobileNo=" + item.ReceiverMobileNo);

            mail.SendMail(senderInfo.Email, "Money Transfer Cancelled" + " " + item.ReceiptNo, body);

        }

        public void SendTransactionInProgressSms(BankAccountDeposit item)
        {
            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);
            string phoneNo = Common.Common.GetCountryPhoneCode(item.SendingCountry) + senderInfo.PhoneNumber;
            SmsApi smsApi = new SmsApi();

            string receivingCountryCurrecy = Common.Common.GetCurrencyByCurrencyOrCountry(item.ReceivingCurrency, item.ReceiverCountry);

            string msg = smsApi.GetManualBankDepositSenderFirstMessage(item.ReceiverName, receivingCountryCurrecy
                + " " + item.ReceivingAmount,
                Common.Common.GetCountryName(item.ReceivingCountry), item.ReceiptNo);
            smsApi.SendSMS(phoneNo, msg);
        }
        public void SendTransactionCompletedSms(BankAccountDeposit item)
        {
            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);
            string phoneNo = Common.Common.GetCountryPhoneCode(item.SendingCountry) + senderInfo.PhoneNumber;
            SmsApi smsApi = new SmsApi();
            string ReceivingCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(item.ReceivingCurrency, item.ReceiverCountry);

            string msg = smsApi.GetManualBankDepositSenderSecondMessage(item.ReceiverName, ReceivingCurrency
                + " " + item.ReceivingAmount, item.ReceiptNo);
            smsApi.SendSMS(phoneNo, msg);

            Log.Write(phoneNo + " Ref: " + item.ReceiptNo, ErrorType.UnSpecified, "Bank Sms");

        }


        public void SendManualBankDepositSmsToAgent(BankAccountDeposit item)
        {

            string bankName = item.BankId == 0 ? item.BankName : Common.Common.getBankName(item.BankId);
            string AgentPhoneNo = Common.Common.GetCountryPhoneCode(item.ReceiverCountry) +
                db.ManualDepositEnable.Where(x => x.PayingCountry == item.ReceivingCountry
            && x.IsEnabled == true).Select(x => x.MobileNo).FirstOrDefault();


            string receivingCountryCurrecy = Common.Common.GetCurrencyByCurrencyOrCountry(item.ReceivingCurrency, item.ReceiverCountry);

            SmsApi smsApi = new SmsApi();
            string msg = smsApi.GetManualBankDepositAgentMessage(item.ReceiverName, item.ReceiverName, item.BankName,
                item.ReceiverAccountNo, item.BankCode, receivingCountryCurrecy + " " + item.ReceivingAmount);
            smsApi.SendSMS(AgentPhoneNo, msg);
        }

        #endregion
        public void ScheduleUpdateTransactionStatus()
        {
            var transactions = List().Data.Where(x => x.Status == BankDepositStatus.Incomplete).ToList();
            foreach (var tran in transactions)
            {
            }
        }

        #region  for StaffAdmin
        public void SetStaffBankAccoutDepositEnterAmount(SenderBankAccoutDepositEnterAmountVm vm)
        {

            Common.AdminSession.SenderBankAccoutDepositEnterAmount = vm;



        }

        public SenderBankAccoutDepositEnterAmountVm GetStaffBankAccoutDepositEnterAmount()
        {

            SenderBankAccoutDepositEnterAmountVm vm = new SenderBankAccoutDepositEnterAmountVm();

            if (Common.AdminSession.SenderBankAccoutDepositEnterAmount != null)
            {

                vm = Common.AdminSession.SenderBankAccoutDepositEnterAmount;
            }
            return vm;
        }


        public void SetStaffBankAccountDeposit(SenderBankAccountDepositVm vm)
        {

            Common.AdminSession.SenderBankAccountDeposit = vm;

        }

        public SenderBankAccountDepositVm GetStaffBankAccountDeposit()
        {

            SenderBankAccountDepositVm vm = new SenderBankAccountDepositVm();

            if (Common.AdminSession.SenderBankAccountDeposit != null)
            {

                vm = Common.AdminSession.SenderBankAccountDeposit;
            }
            return vm;
        }


        #endregion
        //public DB.BankDepositStatus GetApiTransactionStatus(BankAccountDeposit item)
        //{

        //    switch (item.Apiservice)
        //    {
        //        case DB.Apiservice.VGG:


        //            BankDepositApi bankDepositApi = new BankDepositApi();


        //            var transcationStatus = ;
        //            if (transcationStatus == 1)
        //            {
        //                item.Status = BankDepositStatus.Incomplete;
        //            }
        //            else if (transcationStatus == 2)
        //            {
        //                item.Status = BankDepositStatus.Confirm;
        //            }
        //            else if (transcationStatus == 3)
        //            {
        //                item.Status = BankDepositStatus.Failed;
        //            }
        //            else if (transcationStatus == 0)
        //            {
        //                item.Status = BankDepositStatus.Incomplete;
        //            }
        //            break;
        //        case DB.Apiservice.TransferZero:


        //            break;

        //        case Apiservice.EmergentApi:
        //            var EmergentApiResponse = GetEmergentApiBankAccountDepositApiResponse(item);
        //            item.Status = BankDepositStatus.Incomplete;
        //            switch (EmergentApiResponse.status_code)
        //            {
        //                case 1: // Success
        //                    item.Status = BankDepositStatus.Confirm;
        //                    break;
        //                case 2: // Pending

        //                    item.Status = BankDepositStatus.Incomplete;
        //                    break;
        //                case 3: // Expired

        //                    item.Status = BankDepositStatus.Failed;
        //                    break;
        //                case 4: // Reversed
        //                    item.Status = BankDepositStatus.Cancel;
        //                    break;
        //                case 5: //In Clearing
        //                    item.Status = BankDepositStatus.Incomplete;
        //                    break;
        //                default:
        //                    break;
        //            }
        //            break;
        //        case Apiservice.Zenith:

        //            ZenithApi zenithApi = new ZenithApi();
        //            var zenithResponse = zenithApi.CreateBankDepositTransaction(item);


        //            item.Status = zenithResponse.Status;
        //            break;
        //        default:
        //            break;
        //    }
        //}

        public void SetTransactionSummary()
        {

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            SSenderForAllTransfer _senderForAllTransferServices = new SSenderForAllTransfer();

            var sendingAmountData = GetSenderBankAccoutDepositEnterAmount();
            var bankAccountDeposit = GetSenderBankAccountDeposit();
            //Completing Transaction
            var loggedUserData = GetLoggedUserData();
            var paymentMethod = GetPaymentMethod();

            TransactionSummaryVM transactionSummaryVm = new TransactionSummaryVM();
            transactionSummaryVm.BankAccountDeposit = bankAccountDeposit;
            int SenderWalletId = 0;
            var senderWalletInfo = senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id);
            if (senderWalletInfo != null)
            {

                SenderWalletId = senderWalletInfo.Id;
            }
            transactionSummaryVm.SenderAndReceiverDetail = new SenderAndReceiverDetialVM()
            {
                SenderId = loggedUserData.Id,
                SenderCountry = loggedUserData.CountryCode,
                ReceiverCountry = bankAccountDeposit.CountryCode,
                SenderWalletId = SenderWalletId
                //ReceiverId = bankAccountDeposit.RecentReceiverId == null ? 0 : (int)bankAccountDeposit.RecentReceiverId

            };

            //Set Sms Fee 
            transactionSummaryVm.KiiPayTransferPaymentSummary = new KiiPayTransferPaymentSummary()
            {
                ReceiverName = sendingAmountData.ReceiverName,
                SendingCurrency = sendingAmountData.SendingCurrencyCode,
                SendingAmount = sendingAmountData.SendingAmount,
                ReceivingAmount = sendingAmountData.ReceivingAmount,
                TotalAmount = sendingAmountData.TotalAmount,
                ExchangeRate = sendingAmountData.ExchangeRate,
                Fee = sendingAmountData.Fee,
                PaymentReference = "",
                ReceivingCurrency = sendingAmountData.ReceivingCurrencyCode,
                ReceivingCurrencySymbol = sendingAmountData.ReceivingCurrencySymbol,
                SendingCurrencySymbol = sendingAmountData.SendingCurrencySymbol,
                SendSMS = true,
                SMSFee = 0,

            };


            transactionSummaryVm.PaymentMethodAndAutoPaymentDetail = new PaymentMethodViewModel()
            {
                TotalAmount = sendingAmountData.TotalAmount,
                SendingCurrencySymbol = sendingAmountData.SendingCurrencySymbol,
                SenderPaymentMode = paymentMethod.SenderPaymentMode,
                EnableAutoPayment = false,
            };

            //For DebitCreditCardDetail


            var debitCreditCardDetail = GetDebitCreditCardDetail();

            transactionSummaryVm.CreditORDebitCardDetials = debitCreditCardDetail;
            transactionSummaryVm.CreditORDebitCardDetials.FaxingAmount = sendingAmountData.TotalAmount;


            var moneyFexBankAccountDepositData = GetMoneyFexBankAccountDeposit();
            transactionSummaryVm.MoneyFexBankDeposit = moneyFexBankAccountDepositData;

            transactionSummaryVm.TransferType = TransferType.BankDeposit;
            if (transactionSummaryVm.SenderAndReceiverDetail.SenderCountry == transactionSummaryVm.SenderAndReceiverDetail.ReceiverCountry)
            {

                transactionSummaryVm.IsLocalPayment = true;

            }
            else
            {
                transactionSummaryVm.IsLocalPayment = false;
            }
            _senderForAllTransferServices.SetTransactionSummary(transactionSummaryVm);

            _senderForAllTransferServices.GenerateReceiptNoForBankDepoist(transactionSummaryVm.BankAccountDeposit.IsManualDeposit);
            // return transactionSummaryVm;

        }

    }

    public class BankTransactionApiResponse
    {

        public BankAccountDeposit BankAccountDeposit { get; set; }
        public BankDepositResponseVm BankDepositApiResponseVm { get; set; }

    }
}