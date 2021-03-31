using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ReceiverServices
    {
        DB.FAXEREntities dbContext = null;
        CommonServices _commonServices = null;
        public ReceiverServices()
        {
            dbContext = new DB.FAXEREntities();
            _commonServices = new CommonServices();
        }

        public List<ReceiverTransactionStatement> GetTransactionStatement(int year = 0, int month = 0, int day = 0, int service = 0,
            string senderName = "")
        {
            IQueryable<FaxerInformation> senderInfo = dbContext.FaxerInformation;
            IQueryable<BankAccountDeposit> bankAccountDepositData = dbContext.BankAccountDeposit;
            IQueryable<FaxingNonCardTransaction> cashPickupData = dbContext.FaxingNonCardTransaction;
            IQueryable<MobileMoneyTransfer> mobileMoneyTransferData = dbContext.MobileMoneyTransfer;
            IQueryable<TopUpSomeoneElseCardTransaction> KiipayWalletData = dbContext.TopUpSomeoneElseCardTransaction;

            if (year > 0)
            {
                bankAccountDepositData = bankAccountDepositData.Where(x => x.TransactionDate.Year == year);
                cashPickupData = cashPickupData.Where(x => x.TransactionDate.Year == year);
                mobileMoneyTransferData = mobileMoneyTransferData.Where(x => x.TransactionDate.Year == year);
                KiipayWalletData = KiipayWalletData.Where(x => x.TransactionDate.Year == year);
            }
            if (month > 0)
            {
                bankAccountDepositData = bankAccountDepositData.Where(x => x.TransactionDate.Month == month);
                cashPickupData = cashPickupData.Where(x => x.TransactionDate.Month == month);
                mobileMoneyTransferData = mobileMoneyTransferData.Where(x => x.TransactionDate.Month == month);
                KiipayWalletData = KiipayWalletData.Where(x => x.TransactionDate.Month == month);
            }
            if (day > 0)
            {
                bankAccountDepositData = bankAccountDepositData.Where(x => x.TransactionDate.Day == day);
                cashPickupData = cashPickupData.Where(x => x.TransactionDate.Day == day);
                mobileMoneyTransferData = mobileMoneyTransferData.Where(x => x.TransactionDate.Day == day);
                KiipayWalletData = KiipayWalletData.Where(x => x.TransactionDate.Day == day);
            }
            if (!string.IsNullOrEmpty(senderName))
            {
                senderName = senderName.Trim();
                senderInfo = senderInfo.Where(x => (x.FirstName.ToLower().Contains(senderName.ToLower()) ||
                                                    x.MiddleName.ToLower().Contains(senderName.ToLower()) ||
                                                    x.LastName.ToLower().Contains(senderName.ToLower())));
            }

            var result = new List<ReceiverTransactionStatement>();

            switch (service)
            {
                case 0: //bank Account
                    result = GetBankStatement(senderInfo, bankAccountDepositData);
                    break;
                case 1: //MobileWallet

                    result = GetOtherWalletStatement(senderInfo, mobileMoneyTransferData);
                    break;
                case 2://CashPickUP
                    result = GetCashPickUpStatement(senderInfo, cashPickupData);

                    break;
                case 3://KiiPayWallet
                    result = GetKiiPayWalletStatement(senderInfo, KiipayWalletData);

                    break;
                case 4://Select
                    result = GetBankStatement(senderInfo, bankAccountDepositData).
                        Concat(GetOtherWalletStatement(senderInfo, mobileMoneyTransferData)).
                        Concat(GetCashPickUpStatement(senderInfo, cashPickupData)).
                        Concat(GetKiiPayWalletStatement(senderInfo, KiipayWalletData)).ToList();
                    break;
            }
            return result;

        }

        public List<ReceiverTransactionStatement> GetKiiPayWalletStatement(IQueryable<FaxerInformation> senderInfo, IQueryable<TopUpSomeoneElseCardTransaction> kiipayWalletData)
        {

            var KiiPay = (from c in dbContext.TopUpSomeoneElseCardTransaction
                          join d in senderInfo on c.FaxerId equals d.Id
                          join e in dbContext.Recipients on c.RecipientId equals e.Id
                          select new ReceiverTransactionStatement()
                          {
                              SenderId = c.FaxerId,
                              Amount = c.RecievingAmount,
                              Fee = c.FaxingFee,
                              Identifier = c.ReceiptNumber,
                              DateTime = c.TransactionDate,
                              Service = Service.KiiPayWallet,
                              ServiceType = "KiiPay Wallet",
                              TransactionId = c.Id,
                              SenderFirstName = d.FirstName,
                              SenderMiddleName = d.MiddleName,
                              SenderLastName = d.LastName,
                              ReceiverId = e.Id,
                              ReceiverName = e.ReceiverName,
                          }).ToList();
            return KiiPay;
        }

        public ReceiverTransactionDetailsViewModel GetTransactionDetailsOfReceiver(int transactionId, Service service)
        {
            ReceiverTransactionDetailsViewModel vm = new ReceiverTransactionDetailsViewModel();
            var senderInfo = dbContext.FaxerInformation;
            var sender = new FaxerInformation();

            switch (service)
            {
                case Service.BankAccount:
                    var bankAccountDeposit = dbContext.BankAccountDeposit.Where(x => x.Id == transactionId).FirstOrDefault();
                    sender = senderInfo.Where(x => x.Id == bankAccountDeposit.SenderId).FirstOrDefault();
                    vm = new ReceiverTransactionDetailsViewModel()
                    {
                        TransactionId = bankAccountDeposit.Id,
                        BankCode = bankAccountDeposit.BankCode,
                        BankName = dbContext.Bank.Where(x => x.Id == bankAccountDeposit.BankId).Select(x => x.Name).FirstOrDefault(),
                        ExchangeRate = bankAccountDeposit.ExchangeRate,
                        Fee = bankAccountDeposit.Fee,
                        FormattedDate = bankAccountDeposit.TransactionDate.ToString("yyyy-MMM-dd"),
                        TransactionDate = bankAccountDeposit.TransactionDate.ToString("dd/MM/yyyy"),
                        PaymentMethod = Common.Common.GetEnumDescription(bankAccountDeposit.SenderPaymentMode),
                        ReceiptNo = bankAccountDeposit.ReceiptNo,
                        ReceiverName = bankAccountDeposit.ReceiverName,
                        ReceiverFirstName = bankAccountDeposit.ReceiverName.Split(' ')[0],
                        ReceivingAmount = bankAccountDeposit.ReceivingAmount,
                        ReceiverAccountNo = bankAccountDeposit.ReceiverAccountNo,
                        ReceivingCountry = Common.Common.GetCountryName(bankAccountDeposit.ReceivingCountry),
                        ReceivingCountryCode = bankAccountDeposit.ReceivingCountry,
                        ReceivingCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(bankAccountDeposit.ReceivingCurrency, bankAccountDeposit.ReceivingCountry),
                        ReceivingCurrencySymbol = Common.Common.GetCurrencySymbolByCurrencyOrCountry(bankAccountDeposit.ReceivingCurrency, bankAccountDeposit.ReceivingCountry),
                        Service = Service.BankAccount,
                        ReceiverMobileNo = bankAccountDeposit.ReceiverMobileNo,
                        SendingCountry = Common.Common.GetCountryName(bankAccountDeposit.SendingCountry),
                        SendingCountryCode = bankAccountDeposit.ReceivingCountry,
                        SendingCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(bankAccountDeposit.SendingCurrency, bankAccountDeposit.SendingCountry),
                        SendingCurrencySymbol = Common.Common.GetCurrencySymbolByCurrencyOrCountry(bankAccountDeposit.SendingCurrency, bankAccountDeposit.SendingCountry),
                        TotalAmount = bankAccountDeposit.TotalAmount,
                        SendingAmount = bankAccountDeposit.SendingAmount,
                        ServiceName = Common.Common.GetEnumDescription(Service.BankAccount),
                        Status = Common.Common.GetEnumDescription(bankAccountDeposit.Status),
                        IsEuropeTransfer = bankAccountDeposit.IsEuropeTransfer,
                        SenderEmail = sender.Email,
                        SenderFirstName = sender.FirstName,
                        SenderName = sender.FirstName + " " + (string.IsNullOrEmpty(sender.MiddleName) == true ? "" : sender.MiddleName + " ") + sender.LastName,
                        SenderMobileNo = sender.PhoneNumber,

                    };
                    break;
                case Service.MobileWallet:
                    var mobileWallet = dbContext.MobileMoneyTransfer.Where(x => x.Id == transactionId).FirstOrDefault();
                    sender = senderInfo.Where(x => x.Id == mobileWallet.SenderId).FirstOrDefault();
                    vm = new ReceiverTransactionDetailsViewModel()
                    {
                        TransactionId = mobileWallet.Id,
                        ExchangeRate = mobileWallet.ExchangeRate,
                        Fee = mobileWallet.Fee,
                        FormattedDate = mobileWallet.TransactionDate.ToString("yyyy-MMM-dd"),
                        TransactionDate = mobileWallet.TransactionDate.ToString("dd/MM/yyyy"),
                        PaymentMethod = Common.Common.GetEnumDescription(mobileWallet.SenderPaymentMode),
                        ReceiptNo = mobileWallet.ReceiptNo,
                        ReceiverName = mobileWallet.ReceiverName,
                        ReceiverFirstName = mobileWallet.ReceiverName.Split(' ')[0],
                        ReceivingAmount = mobileWallet.ReceivingAmount,
                        ReceiverAccountNo = mobileWallet.PaidToMobileNo,
                        ReceivingCountry = Common.Common.GetCountryName(mobileWallet.ReceivingCountry),
                        ReceivingCountryCode = mobileWallet.ReceivingCountry,
                        ReceivingCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(mobileWallet.ReceivingCurrency, mobileWallet.ReceivingCountry),
                        ReceivingCurrencySymbol = Common.Common.GetCurrencySymbolByCurrencyOrCountry(mobileWallet.ReceivingCurrency, mobileWallet.ReceivingCountry),
                        Service = Service.MobileWallet,
                        ReceiverMobileNo = mobileWallet.PaidToMobileNo,
                        SendingCountry = Common.Common.GetCountryName(mobileWallet.SendingCountry),
                        SendingCountryCode = mobileWallet.ReceivingCountry,
                        SendingCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(mobileWallet.SendingCurrency, mobileWallet.SendingCountry),
                        SendingCurrencySymbol = Common.Common.GetCurrencySymbolByCurrencyOrCountry(mobileWallet.SendingCurrency, mobileWallet.SendingCountry),
                        TotalAmount = mobileWallet.TotalAmount,
                        SendingAmount = mobileWallet.SendingAmount,
                        ServiceName = Common.Common.GetEnumDescription(Service.MobileWallet),
                        Status = Common.Common.GetEnumDescription(mobileWallet.Status),
                        SenderEmail = sender.Email,
                        SenderFirstName = sender.FirstName,
                        SenderName = sender.FirstName + " " + (string.IsNullOrEmpty(sender.MiddleName) == true ? "" : sender.MiddleName + " ") + sender.LastName,
                        SenderMobileNo = sender.PhoneNumber,
                        MobileProvider = dbContext.MobileWalletOperator.Where(x => x.Id == mobileWallet.WalletOperatorId).Select(x => x.Name).FirstOrDefault()

                    };

                    break;
                case Service.CashPickUP:
                    var cashPickUp = dbContext.FaxingNonCardTransaction.Where(x => x.Id == transactionId).FirstOrDefault();
                    sender = senderInfo.Where(x => x.Id == cashPickUp.SenderId).FirstOrDefault();
                    var recipient = dbContext.Recipients.Where(x => x.Id == cashPickUp.RecipientId).FirstOrDefault();
                    vm = new ReceiverTransactionDetailsViewModel()
                    {
                        TransactionId = cashPickUp.Id,
                        ExchangeRate = cashPickUp.ExchangeRate,
                        Fee = cashPickUp.FaxingFee,
                        FormattedDate = cashPickUp.TransactionDate.ToString("yyyy-MMM-dd"),
                        TransactionDate = cashPickUp.TransactionDate.ToString("dd/MM/yyyy"),
                        PaymentMethod = Common.Common.GetEnumDescription(cashPickUp.SenderPaymentMode),
                        ReceiptNo = cashPickUp.ReceiptNumber,
                        ReceiverName = recipient.ReceiverName,
                        ReceiverFirstName = recipient.ReceiverName.Split(' ')[0],
                        ReceivingAmount = cashPickUp.ReceivingAmount,
                        ReceiverAccountNo = recipient.AccountNo,
                        ReceivingCountry = Common.Common.GetCountryName(cashPickUp.ReceivingCountry),
                        ReceivingCountryCode = cashPickUp.ReceivingCountry,
                        ReceivingCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(cashPickUp.ReceivingCurrency, cashPickUp.ReceivingCountry),
                        ReceivingCurrencySymbol = Common.Common.GetCurrencySymbolByCurrencyOrCountry(cashPickUp.ReceivingCurrency, cashPickUp.ReceivingCountry),
                        Service = Service.CashPickUP,
                        ReceiverMobileNo = recipient.MobileNo,
                        SendingCountry = Common.Common.GetCountryName(cashPickUp.SendingCountry),
                        SendingCountryCode = cashPickUp.ReceivingCountry,
                        SendingCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(cashPickUp.SendingCurrency, cashPickUp.SendingCountry),
                        SendingCurrencySymbol = Common.Common.GetCurrencySymbolByCurrencyOrCountry(cashPickUp.SendingCurrency, cashPickUp.SendingCountry),
                        TotalAmount = cashPickUp.TotalAmount,
                        SendingAmount = cashPickUp.FaxingAmount,
                        ServiceName = Common.Common.GetEnumDescription(Service.CashPickUP),
                        Status = Common.Common.GetEnumDescription(cashPickUp.FaxingStatus),
                        SenderEmail = sender.Email,
                        SenderFirstName = sender.FirstName,
                        SenderName = sender.FirstName + " " + (string.IsNullOrEmpty(sender.MiddleName) == true ? "" : sender.MiddleName + " ") + sender.LastName,
                        SenderMobileNo = sender.PhoneNumber,
                        MFCN = cashPickUp.MFCN
                    };


                    break;
            }

            return vm;
        }

        public List<ReceiverTransactionStatement> GetOtherWalletStatement(IQueryable<FaxerInformation> senderInfo, IQueryable<MobileMoneyTransfer> mobileMoneyTransferData)
        {


            var mobileWallet = (from c in dbContext.MobileMoneyTransfer
                                join d in senderInfo on c.SenderId equals d.Id
                                join e in dbContext.Recipients on c.RecipientId equals e.Id

                                select new ReceiverTransactionStatement()
                                {
                                    SenderId = c.SenderId,
                                    Amount = c.ReceivingAmount,
                                    Fee = c.Fee,
                                    Identifier = c.ReceiptNo,
                                    DateTime = c.TransactionDate,
                                    Service = Service.MobileWallet,
                                    ServiceType = "MobileWallet",
                                    TransactionId = c.Id,
                                    SenderFirstName = d.FirstName,
                                    SenderMiddleName = d.MiddleName,
                                    SenderLastName = d.LastName,
                                    ReceiverId = e.Id,
                                    ReceiverName = e.ReceiverName,
                                }).ToList();
            return mobileWallet;
        }
        public List<ReceiverTransactionStatement> GetCashPickUpStatement(IQueryable<FaxerInformation> senderInfo, IQueryable<FaxingNonCardTransaction> cashPickupData)
        {
            var cashPickUp = (from c in dbContext.FaxingNonCardTransaction
                              join d in senderInfo on c.SenderId equals d.Id
                              join e in dbContext.Recipients on c.RecipientId equals e.Id
                              select new ReceiverTransactionStatement()
                              {
                                  SenderId = c.SenderId,
                                  Amount = c.ReceivingAmount,
                                  Fee = c.FaxingFee,
                                  Identifier = c.ReceiptNumber,
                                  DateTime = c.TransactionDate,
                                  Service = Service.CashPickUP,
                                  ServiceType = "CashPickUP",
                                  TransactionId = c.Id,
                                  SenderFirstName = d.FirstName,
                                  SenderMiddleName = d.MiddleName,
                                  SenderLastName = d.LastName,
                                  ReceiverId = e.Id,
                                  ReceiverName = e.ReceiverName,
                              }).ToList();
            return cashPickUp;
        }
        public List<ReceiverTransactionStatement> GetBankStatement(IQueryable<FaxerInformation> senderInfo, IQueryable<BankAccountDeposit> bankAccountDepositData)
        {
            var bankAccountDeposit = (from c in bankAccountDepositData
                                      join d in senderInfo on c.SenderId equals d.Id
                                      join e in dbContext.Recipients on c.RecipientId equals e.Id
                                      select new ReceiverTransactionStatement()
                                      {
                                          Identifier = c.ReceiptNo,
                                          Amount = c.ReceivingAmount,
                                          Fee = c.Fee,
                                          DateTime = c.TransactionDate,
                                          TransactionId = c.Id,
                                          SenderId = c.SenderId,
                                          Service = Service.BankAccount,
                                          ServiceType = "Bank Account",
                                          SenderFirstName = d.FirstName,
                                          SenderMiddleName = d.MiddleName,
                                          SenderLastName = d.LastName,
                                          ReceiverName = e.ReceiverName,
                                          ReceiverId = e.Id,
                                      }).ToList();
            return bankAccountDeposit;
        }

        internal void UpdateRecepient(Recipients recepientDetails)
        {

            dbContext.Entry(recepientDetails).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

        public NewReceiverTransactionStatement GetNewReceiverTransactionStatement(int recipientId, int year, int pageSize = 0, int pageNumber = 0)
        {
            NewReceiverTransactionStatement transactionStatement = new NewReceiverTransactionStatement();
            var senderInfo = dbContext.FaxerInformation;
            IQueryable<BankAccountDeposit> bankAccountDepositData = dbContext.BankAccountDeposit.Where(x => x.RecipientId == recipientId);
            IQueryable<FaxingNonCardTransaction> cashPickupData = dbContext.FaxingNonCardTransaction.Where(x => x.RecipientId == recipientId);
            IQueryable<MobileMoneyTransfer> mobileMoneyTransferData = dbContext.MobileMoneyTransfer.Where(x => x.RecipientId == recipientId);
            IQueryable<TopUpSomeoneElseCardTransaction> KiipayWalletData = dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.RecipientId == recipientId);

            if (year > 0)
            {
                bankAccountDepositData = bankAccountDepositData.Where(x => x.TransactionDate.Year == year);
                cashPickupData = cashPickupData.Where(x => x.TransactionDate.Year == year);
                mobileMoneyTransferData = mobileMoneyTransferData.Where(x => x.TransactionDate.Year == year);
                KiipayWalletData = KiipayWalletData.Where(x => x.TransactionDate.Year == year);
            }

            var bankAccountDeposit = (from c in bankAccountDepositData
                                      join d in senderInfo on c.SenderId equals d.Id
                                      join e in dbContext.Recipients on c.RecipientId equals e.Id
                                      join f in dbContext.Bank on c.BankId equals f.Id
                                      select new ReceiverTransactionStatement()
                                      {
                                          Identifier = c.ReceiptNo,
                                          Amount = c.ReceivingAmount,
                                          Fee = c.Fee,
                                          DateTime = c.TransactionDate,
                                          TransactionId = c.Id,
                                          SenderId = c.SenderId,
                                          Service = Service.BankAccount,
                                          ServiceType = "Bank Account",
                                          BanKWalletName = f.Name,
                                          BanKWalletNumber = c.ReceiverAccountNo,
                                          SenderName = d.FirstName + " " + d.MiddleName + " " + d.LastName,
                                          ReceiverName = e.ReceiverName,
                                          Status = c.Status.ToString(),
                                          ReceiverCountry = c.ReceivingCountry,
                                      }).ToList();
            var cashPickUp = (from c in cashPickupData
                              join d in senderInfo on c.SenderId equals d.Id
                              join e in dbContext.Recipients on c.RecipientId equals e.Id
                              select new ReceiverTransactionStatement()
                              {
                                  SenderId = c.SenderId,
                                  Amount = c.ReceivingAmount,
                                  Fee = c.FaxingFee,
                                  Identifier = c.ReceiptNumber,
                                  DateTime = c.TransactionDate,
                                  Service = Service.CashPickUP,
                                  ServiceType = "CashPickUP",
                                  TransactionId = c.Id,
                                  Status = c.FaxingStatus.ToString(),
                                  SenderName = d.FirstName + " " + d.MiddleName + " " + d.LastName,
                              }).ToList();
            var MobileWallet = (from c in mobileMoneyTransferData
                                join d in senderInfo on c.SenderId equals d.Id
                                join e in dbContext.Recipients on c.RecipientId equals e.Id
                                join f in dbContext.MobileWalletOperator on c.WalletOperatorId equals f.Id
                                select new ReceiverTransactionStatement()
                                {
                                    SenderId = c.SenderId,
                                    Amount = c.ReceivingAmount,
                                    Fee = c.Fee,
                                    Identifier = c.ReceiptNo,
                                    DateTime = c.TransactionDate,
                                    Service = Service.MobileWallet,
                                    ServiceType = "MobileWallet",
                                    TransactionId = c.Id,
                                    Status = c.Status.ToString(),
                                    BanKWalletName = f.Name,
                                    BanKWalletNumber = c.PaidToMobileNo,
                                    SenderName = d.FirstName + " " + d.MiddleName + " " + d.LastName,
                                }).ToList();
            var KiiPay = (from c in dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.RecipientId == recipientId)
                          join d in senderInfo on c.FaxerId equals d.Id
                          join e in dbContext.Recipients on c.RecipientId equals e.Id

                          select new ReceiverTransactionStatement()
                          {
                              SenderId = c.FaxerId,
                              Amount = c.RecievingAmount,
                              Fee = c.FaxingFee,
                              Identifier = c.ReceiptNumber,
                              DateTime = c.TransactionDate,
                              Service = Service.KiiPayWallet,
                              ServiceType = "KiiPay Wallet",
                              TransactionId = c.Id,
                              SenderName = d.FirstName + " " + d.MiddleName + " " + d.LastName,

                          }).ToList();

            transactionStatement.TransactionList = bankAccountDeposit.Concat(cashPickUp).Concat(MobileWallet).ToPagedList(pageNumber, pageSize);



            transactionStatement.Monthly = GetReceiverMonthlyTransactionMeter(transactionStatement.TransactionList.ToList(), recipientId);
            return transactionStatement;
        }

        private MonthlyTransactionMeter GetReceiverMonthlyTransactionMeter(List<ReceiverTransactionStatement> transactions, int recipientId)
        {
            var currency = getReceiverCurencyByRecipientId(recipientId);
            transactions = transactions.Where(x => x.Status.ToLower() == "paid" ||
                                      x.Status.ToLower() == "completed" ||
                                      x.Status.ToLower() == "received" ||
                                      x.Status.ToLower() == "confirm").ToList(); ;
            MonthlyTransactionMeter monthlTransactionMeter = new MonthlyTransactionMeter()
            {
                Jan = GetMonthlySum(transactions, 1, currency),
                Feb = GetMonthlySum(transactions, 2, currency),
                March = GetMonthlySum(transactions, 3, currency),
                April = GetMonthlySum(transactions, 4, currency),
                May = GetMonthlySum(transactions, 5, currency),
                Jun = GetMonthlySum(transactions, 6, currency),
                July = GetMonthlySum(transactions, 7, currency),
                Aug = GetMonthlySum(transactions, 8, currency),
                Sep = GetMonthlySum(transactions, 9, currency),
                Oct = GetMonthlySum(transactions, 10, currency),
                Nov = GetMonthlySum(transactions, 11, currency),
                Dec = GetMonthlySum(transactions, 12, currency),
            };
            return monthlTransactionMeter;
        }
        public string getReceiverCurencyByRecipientId(int recipientId)
        {
            var recipientCountry = dbContext.Recipients.Where(x => x.Id == recipientId).Select(x => x.Country).FirstOrDefault();
            return Common.Common.GetCountryCurrency(recipientCountry);
        }

        public string GetMonthlySum(List<ReceiverTransactionStatement> result, int Month, string Currency = "")
        {
            var Amount = result.Where(x => x.DateTime.Month == Month).ToList().Sum(x => x.Amount);
            if (Amount > 0)
            {
                return Amount + " " + Currency;
            }
            return "";
        }

        public List<ReceiverDetailsInfoViewModel> GetRecipients(bool IsBanned, string receiverName = "", string Country = "")
        {

            IQueryable<Recipients> recipients = dbContext.Recipients;
            if (IsBanned)
            {
                recipients = recipients.Where(x => x.IsBanned == IsBanned);
            }
            if (!string.IsNullOrEmpty(receiverName))
            {
                receiverName = receiverName.Trim();
                recipients = recipients.Where(x => x.ReceiverName.ToLower().Contains(receiverName.ToLower()));
            }
            if (!string.IsNullOrEmpty(Country))
            {
                recipients = recipients.Where(x => x.Country == Country);
            }
            List<ReceiverDetailsInfoViewModel> vm = new List<ReceiverDetailsInfoViewModel>();
            vm = (from c in recipients
                  join country in dbContext.Country on c.Country equals country.CountryCode
                  join bank in dbContext.Bank on c.BankId equals bank.Id into b
                  from bank in b.DefaultIfEmpty()
                  join otherWallet in dbContext.MobileWalletOperator on c.MobileWalletProvider equals otherWallet.Id into o
                  from otherwallet in o.DefaultIfEmpty()
                  select new ReceiverDetailsInfoViewModel()
                  {
                      Id = c.Id,
                      ReceiverCountry = country.CountryName,
                      ReceiverAccountNo = c.AccountNo,
                      Service = c.Service,
                      ServiceName = c.Service.ToString(),
                      ReceiverName = c.ReceiverName,
                      //FirstLetter = c.ReceiverName == null ? "" : c.ReceiverName.Substring(0, 1).ToLower(),
                      ReceiverPhoneNo = c.MobileNo,
                      Blocked = c.IsBanned,
                      ReceiverCountryFlag = c.Country.ToLower(),
                      BankId = c.BankId,
                      BankCode = c.BranchCode,
                      MobileWalletProvider = c.MobileWalletProvider,
                      BankName = bank != null ? bank.Name : null,
                      WalletName = otherwallet != null ? otherwallet.Name : null
                  }).ToList();

            return vm;
        }
        private bool CheckIfBlocked(string phoneNo, TransactionTransferMethod transferMethod, string country)
        {
            var isBolcked = dbContext.BlacklistedReceiver.Where(x => x.ReceiverAccountNo == phoneNo && x.TransferMethod == transferMethod && x.ReceiverCountry == country).Select(x => x.IsBlocked).FirstOrDefault();
            if (isBolcked)
            {
                return true;
            }
            return false;
        }
        internal void AddBlackListedReceiver(Recipients recepientDetails)
        {
            TransactionTransferMethod TransferMethod = TransactionTransferMethod.Select;
            switch (recepientDetails.Service)
            {
                case Service.BankAccount:
                    TransferMethod = TransactionTransferMethod.BankDeposit;
                    break;
                case Service.KiiPayWallet:
                    TransferMethod = TransactionTransferMethod.BankDeposit;
                    break;

                case Service.CashPickUP:
                    TransferMethod = TransactionTransferMethod.CashPickUp;
                    break;
                case Service.MobileWallet:
                    TransferMethod = TransactionTransferMethod.OtherWallet;
                    break;
            }
            BlacklistedReceiver model = new BlacklistedReceiver()
            {
                TransferMethod = TransferMethod,
                ReceiverAccountNo = recepientDetails.AccountNo,
                ReceiverCountry = recepientDetails.Country,
                CareatedDate = DateTime.Now,
                CreatedByUserId = Common.StaffSession.LoggedStaff.StaffId,
                IsBlocked = true,
                ReceiverName = recepientDetails.ReceiverName,
                ReceiverTelephone = recepientDetails.MobileNo
            };

            dbContext.BlacklistedReceiver.Add(model);
            dbContext.SaveChanges();

        }
        public Recipients receiverInfoByReceiverId(int ReceiverId)
        {
            var data = dbContext.Recipients.Where(x => x.Id == ReceiverId).FirstOrDefault();
            return data;
        }

        public List<ReceiverDocumentationViewModel> GetReceiverDocumentation(string Country = "", string City = "")
        {
            var data = dbContext.ReceiverDocumentation.ToList();

            if (!string.IsNullOrEmpty(Country))
            {
                data = data.Where(x => x.Country == Country).ToList();
            }
            if (!string.IsNullOrEmpty(City))
            {
                data = data.Where(x => x.City == City).ToList();
            }

            var result = (from c in data
                          join ctry in dbContext.Country on c.Country equals ctry.CountryCode
                          join receiver in dbContext.Recipients on c.ReceiverId equals receiver.Id
                          join Creator in dbContext.StaffInformation on c.CreatedBy equals Creator.Id into joined
                          from Creator in joined.DefaultIfEmpty()
                          select new ReceiverDocumentationViewModel()
                          {
                              Country = ctry.CountryName,
                              CountryFlag = c.Country.ToLower(),
                              CreatedDate = c.CreatedDate,
                              ReceiverNumber = c.ReceiverNumber,
                              Id = c.Id,
                              City = c.City,
                              CreatedBy = c.CreatedBy,
                              DocumentExpires = c.DocumentExpires,
                              DocumentName = c.DocumentName,
                              CreatedByName = Creator == null ? "" : Creator.FirstName + " " + Creator.MiddleName + " " + Creator.LastName,
                              ExpiryDateString = c.ExpiryDate.HasValue ? c.ExpiryDate.Value.ToString("yyyy-MM-dd") : string.Empty,
                              DocumentPhotoUrl = c.DocumentPhotoUrl,
                              DocumentType = c.DocumentType,
                              ReceiverId = c.ReceiverId,
                              ReceiverName = receiver.ReceiverName,
                          }).ToList();

            return result;
        }
        internal ReceiverDocumentationViewModel GetReceiverDocumentatiobyId(int id)
        {
            var data = dbContext.ReceiverDocumentation.Where(x => x.Id == id).ToList();
            var result = (from c in data
                          select new ReceiverDocumentationViewModel()
                          {
                              Country = c.Country,
                              CreatedDate = c.CreatedDate,
                              ReceiverNumber = c.ReceiverNumber,
                              Id = c.Id,
                              ReceiverId = c.ReceiverId,
                              City = c.City,
                              CreatedBy = c.CreatedBy,
                              DocumentExpires = c.DocumentExpires,
                              DocumentName = c.DocumentName,
                              CreatedByName = _commonServices.getStaffName(c.CreatedBy),
                              ExpiryDate = c.ExpiryDate,
                              DocumentPhotoUrl = c.DocumentPhotoUrl,
                              DocumentType = c.DocumentType,
                          }).FirstOrDefault();
            return result;
        }


        internal void Delete(int id)
        {
            var data = dbContext.ReceiverDocumentation.Where(x => x.Id == id).FirstOrDefault();
            dbContext.ReceiverDocumentation.Remove(data);
            dbContext.SaveChanges();
        }
        internal void UploadDocument(ReceiverDocumentationViewModel vm)
        {
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;
            ReceiverDocumentation model = new ReceiverDocumentation()
            {
                ReceiverNumber = vm.ReceiverName,
                City = vm.City,
                Country = vm.Country,
                CreatedBy = StaffId,
                CreatedDate = DateTime.Now,
                DocumentExpires = vm.DocumentExpires,
                DocumentName = vm.DocumentName,
                DocumentPhotoUrl = vm.DocumentPhotoUrl,
                DocumentType = vm.DocumentType,
                ExpiryDate = vm.ExpiryDate,
                ReceiverId = vm.ReceiverId
            };
            dbContext.ReceiverDocumentation.Add(model);
            dbContext.SaveChanges();
        }

        internal void UpdateDocument(ReceiverDocumentationViewModel vm)
        {
            var data = dbContext.ReceiverDocumentation.Where(x => x.Id == vm.Id).FirstOrDefault();
            data.ExpiryDate = vm.ExpiryDate;
            data.DocumentType = vm.DocumentType;
            data.DocumentPhotoUrl = vm.DocumentPhotoUrl;
            data.DocumentName = vm.DocumentName;
            data.DocumentExpires = vm.DocumentExpires;
            data.Country = vm.Country;
            data.City = vm.City;
            data.ReceiverNumber = vm.ReceiverNumber;
            data.ReceiverId = vm.ReceiverId;
            dbContext.Entry<ReceiverDocumentation>(data).State = EntityState.Modified;
            dbContext.SaveChanges();
        }

    }
}