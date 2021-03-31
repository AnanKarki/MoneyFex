using FAXER.PORTAL.Areas.Admin.ViewModels.SearchParamViewModels;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class DailyTransactionStatementServices
    {
        AgentInformation agentInfo = null;
        FAXEREntities db = null;
        public DailyTransactionStatementServices()
        {
            agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            db = new FAXEREntities();
        }

        public List<DailyTransactionStatementListVm> getDailyTransactionStatementList(int AgentID)
        {

            var refundNonCardFaxMoneyByAgentList = (from c in db.RefundNonCardFaxMoneyByAgent.Where(x => x.Agent_id == AgentID).ToList()
                                                    select new DailyTransactionStatementListVm()
                                                    {
                                                        Id = c.Id,
                                                        TransactionType = TransactionType.Refund,
                                                        TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.Refund),
                                                        CurrencySymbol = Common.CountryUtility.GetCountryCurrencySymbol(c.AgentInformation.CountryCode),
                                                        Amount = c.FaxingNonCardTransaction.FaxingAmount,
                                                        Fee = 0,
                                                        TransactionIdentifier = c.FaxingNonCardTransaction.ReceiptNumber,
                                                        DateAndTime = c.RefundedDate,
                                                        StaffName = c.NameofRefunder,
                                                        Type = Models.Type.Received,
                                                        AgentCommission = 0,
                                                    }).ToList();

            var CashPickupTransfer = (from c in db.FaxingNonCardTransaction.Where(x => x.PayingStaffId == AgentID).ToList()
                                      select new DailyTransactionStatementListVm()
                                      {
                                          Id = c.Id,
                                          TransactionType = TransactionType.CashPickUp,
                                          TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.CashPickUp),
                                          CurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                                          Amount = c.FaxingAmount,
                                          Fee = c.FaxingFee,
                                          TransactionIdentifier = c.ReceiptNumber,
                                          DateAndTime = c.TransactionDate,
                                          StaffName = c.AgentStaffName,
                                          Type = Models.Type.Transfer,
                                          AgentCommission = c.AgentCommission,
                                      }).ToList();

            var CashPickupReceiver = (from c in db.ReceiverNonCardWithdrawl.Where(x => x.PayingAgentStaffId == AgentID).ToList()
                                      select new DailyTransactionStatementListVm()
                                      {
                                          Id = c.Id,
                                          TransactionType = TransactionType.CashPickUp,
                                          TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.CashPickUp),
                                          CurrencySymbol = Common.CountryUtility.GetCountryCurrencySymbol(c.IssuingCountryCode),
                                          Amount = c.ReceivingAmount,
                                          Fee = 0M,
                                          TransactionIdentifier = c.ReceiptNumber,
                                          DateAndTime = c.TransactionDate,
                                          StaffName = c.PayingAgentName,
                                          Type = Models.Type.Received,
                                          AgentCommission = c.AgentCommission,
                                      }).ToList();

            var KiiPayWalletTransfer = (from c in db.TopUpSomeoneElseCardTransaction.Where(x => x.PayingStaffId == AgentID).ToList()
                                        select new DailyTransactionStatementListVm()
                                        {
                                            Id = c.Id,
                                            TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.KiiPayWallet),
                                            TransactionType = TransactionType.KiiPayWallet,
                                            CurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                                            Amount = c.FaxingAmount,
                                            Fee = c.FaxingFee,
                                            TransactionIdentifier = c.ReceiptNumber,
                                            DateAndTime = c.TransactionDate,
                                            StaffName = c.PayingStaffName,
                                            Type = Models.Type.Transfer,
                                            AgentCommission = c.AgentCommission,
                                        }).ToList();

            var KiiPayPersonalWalletReceiver = (from c in db.UserCardWithdrawl.Where(x => x.PayingAgentStaffId == AgentID).ToList()
                                                select new DailyTransactionStatementListVm()
                                                {
                                                    Id = c.Id,
                                                    TransactionType = TransactionType.KiiPayWallet,
                                                    TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.KiiPayWallet),
                                                    CurrencySymbol = Common.Common.GetCurrencySymbol(c.IssuingCountryCode),
                                                    Amount = c.TransactionAmount,
                                                    Fee = 0M,
                                                    TransactionIdentifier = c.ReceiptNumber,
                                                    DateAndTime = c.TransactionDate,
                                                    StaffName = c.PayingAgentName,
                                                    AgentCommission = 0,
                                                    Type = Models.Type.Received,
                                                }).ToList();
            var KiiPayBusinessWalletReceiver = (from c in db.MFBCCardWithdrawls.Where(x => x.PayingAgentStaffId == AgentID).ToList()
                                                select new DailyTransactionStatementListVm()
                                                {
                                                    Id = c.Id,
                                                    TransactionType = TransactionType.KiiPayWallet,
                                                    TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.KiiPayWallet),
                                                    CurrencySymbol = Common.Common.GetCurrencySymbol(c.IssuingCountryCode),
                                                    Amount = c.TransactionAmount,
                                                    Fee = 0M,
                                                    TransactionIdentifier = c.ReceiptNumber,
                                                    DateAndTime = c.TransactionDate,
                                                    StaffName = c.PayingAgentName,
                                                    AgentCommission = 0,
                                                    Type = Models.Type.Received,
                                                }).ToList();
            var OtherMobilesWalletsTransfer = (from c in db.MobileMoneyTransfer.Where(x => x.PayingStaffId == AgentID).ToList()
                                               join d in db.FaxerInformation on c.SenderId equals d.Id into joined
                                               from senderInfo in joined.DefaultIfEmpty()
                                               select new DailyTransactionStatementListVm()
                                               {
                                                   Id = c.Id,
                                                   TransactionType = TransactionType.OtherWalletTransfer,
                                                   TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.OtherWalletTransfer),
                                                   CurrencySymbol = Common.CountryUtility.GetCountryCurrencySymbol(c.SendingCountry),
                                                   Amount = c.SendingAmount,
                                                   Fee = c.Fee,
                                                   TransactionIdentifier = c.ReceiptNo,
                                                   DateAndTime = c.TransactionDate,
                                                   StaffName = c.PayingStaffName,
                                                   AgentCommission = c.AgentCommission,
                                                   Type = Models.Type.Transfer,
                                               }).ToList();
            var BankAccountDepositTransfer = (from c in db.BankAccountDeposit.Where(x => x.PayingStaffId == AgentID).ToList()
                                              select new DailyTransactionStatementListVm()
                                              {
                                                  Id = c.Id,
                                                  TransactionType = TransactionType.BankAccountDeposit,
                                                  TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.BankAccountDeposit),
                                                  CurrencySymbol = Common.CountryUtility.GetCountryCurrencySymbol(c.SendingCountry),
                                                  Amount = c.SendingAmount,
                                                  Fee = c.Fee,
                                                  TransactionIdentifier = c.ReceiptNo,
                                                  DateAndTime = c.TransactionDate,
                                                  StaffName = c.PayingStaffName,
                                                  AgentCommission = c.AgentCommission,
                                                  Type = Models.Type.Transfer,
                                              }).ToList();
            var AgentPayBillsTopUp = (from c in db.TopUpToSupplier.Where(x => x.PayingStaffId == AgentID).ToList()
                                      select new DailyTransactionStatementListVm()
                                      {
                                          Id = c.Id,
                                          TransactionType = TransactionType.PayBillsTopUp,
                                          TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.PayBillsTopUp),
                                          CurrencySymbol = Common.CountryUtility.GetCountryCurrencySymbol(Common.Common.GetCountryCodeByCountryName(c.PayingCountry)),
                                          Amount = c.SendingAmount,
                                          Fee = c.Fee,
                                          TransactionIdentifier = c.ReceiptNo,
                                          DateAndTime = c.PaymentDate,
                                          StaffName = c.PayingStaffName,
                                          AgentCommission = c.AgentCommission,
                                          Type = Models.Type.BillPayment,
                                      }).ToList();
            var AgentPayBillsMonthly = (from c in db.PayBill.Where(x => x.PayingStaffId == AgentID).ToList()
                                        select new DailyTransactionStatementListVm()
                                        {
                                            Id = c.Id,
                                            TransactionType = TransactionType.PayBillsMonthly,
                                            TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.PayBillsMonthly),
                                            CurrencySymbol = Common.CountryUtility.GetCountryCurrencySymbol(Common.Common.GetCountryCodeByCountryName(c.PayerCountry)),
                                            Amount = c.SendingAmount,
                                            Fee = c.Fee,
                                            TransactionIdentifier = c.ReceiptNo,
                                            DateAndTime = c.PaymentDate,
                                            StaffName = c.PayingStaffName,
                                            AgentCommission = c.AgentCommission,
                                            Type = Models.Type.BillPayment,
                                        }).ToList();
            var finalList = refundNonCardFaxMoneyByAgentList.Concat(CashPickupReceiver)
                .Concat(CashPickupTransfer).Concat(KiiPayWalletTransfer).Concat(KiiPayPersonalWalletReceiver).Concat(KiiPayBusinessWalletReceiver)
                .Concat(OtherMobilesWalletsTransfer).Concat(BankAccountDepositTransfer).Concat(AgentPayBillsTopUp).Concat(AgentPayBillsMonthly).ToList();
            return finalList.OrderByDescending(x => x.DateAndTime).ToList();
        }
        public List<DailyTransactionStatementListVm> GetAuxAgentDailyTransactionStatement(int AgentID)
        {
            var CashPickupTransfer = (from c in db.FaxingNonCardTransaction.Where(x => x.PayingStaffId == AgentID).ToList()
                                      join d in db.FaxerInformation on c.SenderId equals d.Id into joined
                                      from senderInfo in joined.DefaultIfEmpty()
                                      select new DailyTransactionStatementListVm()
                                      {
                                          Id = c.Id,
                                          TransactionType = TransactionType.CashPickUp,
                                          TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.CashPickUp),
                                          CurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                                          Amount = c.FaxingAmount,
                                          Fee = c.FaxingFee,
                                          TransactionIdentifier = c.ReceiptNumber,
                                          DateAndTime = c.TransactionDate,
                                          StaffName = c.AgentStaffName,
                                          Type = Models.Type.Transfer,
                                          AgentCommission = c.AgentCommission,
                                          ReceivingCountry = c.ReceivingCountry,
                                          ReceivingCountryName = Common.Common.GetCountryName(c.ReceivingCountry),
                                          SenderId = c.SenderId,
                                          StatusName = Common.Common.GetEnumDescription(c.FaxingStatus),
                                          ReceiverName = c.NonCardReciever.FullName,
                                          SenderName = c.SenderId == 0 ? "" : senderInfo.FirstName + " " + (!string.IsNullOrEmpty(senderInfo.MiddleName) == true ? senderInfo.MiddleName + " " : "") + senderInfo.LastName,
                                          FormatedDate = c.TransactionDate.ToString("dd/MM/yyyy HH:mm")
                                      }).ToList();

            var KiiPayWalletTransfer = (from c in db.TopUpSomeoneElseCardTransaction.Where(x => x.PayingStaffId == AgentID).ToList()
                                        select new DailyTransactionStatementListVm()
                                        {
                                            Id = c.Id,
                                            TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.KiiPayWallet),
                                            TransactionType = TransactionType.KiiPayWallet,
                                            CurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                                            Amount = c.FaxingAmount,
                                            Fee = c.FaxingFee,
                                            TransactionIdentifier = c.ReceiptNumber,
                                            DateAndTime = c.TransactionDate,
                                            StaffName = c.PayingStaffName,
                                            Type = Models.Type.Transfer,
                                            AgentCommission = c.AgentCommission,
                                            ReceivingCountry = c.ReceivingCountry,
                                            ReceivingCountryName = Common.Common.GetCountryName(c.ReceivingCountry),
                                            FormatedDate = c.TransactionDate.ToString("dd/MM/yyyy HH:mm"),
                                            StatusName = ""

                                        }).ToList();

            var OtherMobilesWalletsTransfer = (from c in db.MobileMoneyTransfer.Where(x => x.PayingStaffId == AgentID).ToList()
                                               join d in db.FaxerInformation on c.SenderId equals d.Id into joined
                                               from senderInfo in joined.DefaultIfEmpty()
                                               select new DailyTransactionStatementListVm()
                                               {
                                                   Id = c.Id,
                                                   TransactionType = TransactionType.OtherWalletTransfer,
                                                   TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.OtherWalletTransfer),
                                                   CurrencySymbol = Common.CountryUtility.GetCountryCurrencySymbol(c.SendingCountry),
                                                   Amount = c.SendingAmount,
                                                   Fee = c.Fee,
                                                   TransactionIdentifier = c.ReceiptNo,
                                                   DateAndTime = c.TransactionDate,
                                                   StaffName = c.PayingStaffName,
                                                   AgentCommission = c.AgentCommission,
                                                   Type = Models.Type.Transfer,
                                                   ReceivingCountry = c.ReceivingCountry,
                                                   ReceivingCountryName = Common.Common.GetCountryName(c.ReceivingCountry),
                                                   SenderId = c.SenderId,
                                                   StatusName = Common.Common.GetEnumDescription(c.Status),
                                                   ReceiverName = c.ReceiverName,
                                                   SenderName = c.SenderId == null ? "" : senderInfo.FirstName + " " + (!string.IsNullOrEmpty(senderInfo.MiddleName) == true ? senderInfo.MiddleName +" " : "") + senderInfo.LastName,
                                                   FormatedDate = c.TransactionDate.ToString("dd/MM/yyyy HH:mm")


                                               }).ToList();
            var BankAccountDepositTransfer = (from c in db.BankAccountDeposit.Where(x => x.PayingStaffId == AgentID).ToList()
                                              join d in db.FaxerInformation on c.SenderId equals d.Id into joined
                                              from senderInfo in joined.DefaultIfEmpty()

                                              select new DailyTransactionStatementListVm()
                                              {
                                                  Id = c.Id,
                                                  TransactionType = TransactionType.BankAccountDeposit,
                                                  TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.BankAccountDeposit),
                                                  CurrencySymbol = Common.CountryUtility.GetCountryCurrencySymbol(c.SendingCountry),
                                                  Amount = c.SendingAmount,
                                                  Fee = c.Fee,
                                                  TransactionIdentifier = c.ReceiptNo,
                                                  DateAndTime = c.TransactionDate,
                                                  StaffName = c.PayingStaffName,
                                                  AgentCommission = c.AgentCommission,
                                                  Type = Models.Type.Transfer,
                                                  ReceivingCountry = c.ReceivingCountry,
                                                  ReceivingCountryName = Common.Common.GetCountryName(c.ReceivingCountry),
                                                  SenderId = c.SenderId,
                                                  StatusName = Common.Common.GetEnumDescription(c.Status),
                                                  ReceiverName = c.ReceiverName,
                                                  SenderName = c.SenderId == 0 ? "" : senderInfo.FirstName + " " + (!string.IsNullOrEmpty(senderInfo.MiddleName) == true ? senderInfo.MiddleName + " " : "") + senderInfo.LastName,
                                                  FormatedDate = c.TransactionDate.ToString("dd/MM/yyyy HH:mm")
                                              }).ToList();
            var finalList = CashPickupTransfer.Concat(KiiPayWalletTransfer).
                Concat(OtherMobilesWalletsTransfer).Concat(BankAccountDepositTransfer).ToList();
            return finalList.OrderByDescending(x => x.DateAndTime).ToList();
        }
        public List<DailyTransactionStatementListVm> GetAuxAgentDailyTransactionStatement(SenderTransactionSearchParamVm searchParamVm)
        {
            searchParamVm = GetTrimmedSearchparam(searchParamVm);
            var result = GetTransactionHistoryOfSender(searchParamVm);
            return result;
        }
        public List<DailyTransactionStatementListVm> GetTransactionHistoryOfSender(SenderTransactionSearchParamVm searchParam)
        {
            return db.Sp_GetTransactionStatementOfAgent(searchParam);
        }
        public SenderTransactionSearchParamVm GetTrimmedSearchparam(SenderTransactionSearchParamVm searchParamVm)
        {
            string FromDate = "";
            string ToDate = "";
            if (!string.IsNullOrEmpty(searchParamVm.DateRange))
            {
                var Date = searchParamVm.DateRange.Split('-');
                FromDate = Date[0].ToDateTime().ToString("yyyy/MM/dd");
                ToDate = Date[1].ToDateTime().ToString("yyyy/MM/dd");
            }
            SenderTransactionSearchParamVm vm = new SenderTransactionSearchParamVm()
            {
                PageSize = searchParamVm.PageSize,
                PageNum = searchParamVm.PageNum,
                DateRange = searchParamVm.DateRange != null ? searchParamVm.DateRange : "",
                FromDate = FromDate,
                ToDate = ToDate,
                ReceiverName = searchParamVm.ReceiverName != null ? searchParamVm.ReceiverName.Trim() : "",
                ReceivingCountry = searchParamVm.ReceivingCountry != null ? searchParamVm.ReceivingCountry.Trim() : "",
                searchString = searchParamVm.searchString != null ? searchParamVm.searchString.Trim() : "",
                senderId = searchParamVm.senderId,
                SenderName = searchParamVm.SenderName != null ? searchParamVm.SenderName.Trim() : "",
                SendingCountry = searchParamVm.SendingCountry != null ? searchParamVm.SendingCountry.Trim() : "",
                Status = searchParamVm.Status != null ? searchParamVm.Status.Trim() : "",
            };
            return vm;
        }
        public string generateReferenceNumber()
        {
            string firstThreenumbers = Common.AgentSession.AgentInformation.AccountNo.Substring(0, 3);
            var val = firstThreenumbers + "-MF" + Common.Common.GenerateRandomDigit(6);
            while (db.AgentDailyTransactionStatementLog.Where(x => x.ReferenceNumber.Trim() == val.Trim()).Count() > 0)
            {
                val = firstThreenumbers + "-MF" + Common.Common.GenerateRandomDigit(6);
            }

            return val;
        }

        public string generateReferenceNumber(string AccountNo)
        {
            string firstThreenumbers = AccountNo.Substring(0, 3);
            var val = firstThreenumbers + "-MF" + Common.Common.GenerateRandomDigit(6);
            while (db.AgentDailyTransactionStatementLog.Where(x => x.ReferenceNumber.Trim() == val.Trim()).Count() > 0)
            {
                val = firstThreenumbers + "-MF" + Common.Common.GenerateRandomDigit(6);
            }

            return val;
        }

        public decimal getAgentAccountBalance(int agentID, int payingStaffId)
        {
            var data = (from c in db.BaankAccountCreditUpdateByAgent.Where(x => x.AgentId == agentID)
                        select c).ToList();
            decimal CurrentBankDeposit = data.Select(x => (decimal?)x.BankDeposit).Sum() ?? 0;
            decimal CurrentCustomerDeposit = data.Select(x => (decimal?)x.CustomerDeposit).Sum() ?? 0;
            decimal CustomerDepositFee = data.Select(x => (decimal?)x.CustomerDepositFees).Sum() ?? 0;

            decimal BankAccountTransferAmount = db.BankAccountDeposit.Where(x => x.PayingStaffId == payingStaffId).Select(x => (decimal?)x.SendingAmount).Sum() ?? 0;
            decimal CashPickUpTransferAmount = db.FaxingNonCardTransaction.Where(x => x.PayingStaffId == payingStaffId).Select(x => (decimal?)x.FaxingAmount).Sum() ?? 0;
            decimal OtherwallerTransferAmount = db.MobileMoneyTransfer.Where(x => x.PayingStaffId == payingStaffId).Select(x => (decimal?)x.SendingAmount).Sum() ?? 0;

            decimal NonCardwithdrawalAmount = db.ReceiverNonCardWithdrawl.Where(x => x.AgentId == agentID).Select(x => (decimal?)x.TransactionAmount).Sum() ?? 0;
            decimal MFBCCardWithdrawalAmount = db.MFBCCardWithdrawls.Where(x => x.AgentInformationId == agentID).Select(x => (decimal?)x.TransactionAmount).Sum() ?? 0;
            decimal MFTCCardWithdrawalAmount = db.UserCardWithdrawl.Where(x => x.AgentInformationId == agentID).Select(x => (decimal?)x.TransactionAmount).Sum() ?? 0;
            decimal CardUserNonCardWithdrawalAmount = db.CardUserNonCardWithdrawal.Where(x => x.AgentId == agentID).Select(x => (decimal?)x.TransactionAmount).Sum() ?? 0;
            decimal MerchantNonCardwithdrawalAmount = db.MerchantNonCardWithdrawal.Where(x => x.AgentId == agentID).Select(x => (decimal?)x.TransactionAmount).Sum() ?? 0;

            var TotalAmountDeposited = ((CurrentBankDeposit + CurrentCustomerDeposit + CustomerDepositFee + BankAccountTransferAmount + CashPickUpTransferAmount + OtherwallerTransferAmount) -
                (NonCardwithdrawalAmount + MFBCCardWithdrawalAmount + MFTCCardWithdrawalAmount + CardUserNonCardWithdrawalAmount + MerchantNonCardwithdrawalAmount));
            return TotalAmountDeposited;
        }

        public DB.AgentInformation GetAgentInformation(int agentID)
        {

            var data = db.AgentInformation.Where(x => x.Id == agentID).FirstOrDefault();
            return data;

        }

        public void saveAgentDailyTransactionStatementDetails(int agentId, string referenceNumber, int day, int month, int year, TransactionType transactionType, decimal accountBalance)
        {
            AgentDailyTransactionStatementLog model = new AgentDailyTransactionStatementLog()
            {
                AgentId = agentId,
                ReferenceNumber = referenceNumber,
                StatementDate = DateTime.Now,
                Day = day,
                Month = month,
                Year = year,
                TransactionType = transactionType,
                AccountBalance = accountBalance
            };
            if (model != null)
            {
                db.AgentDailyTransactionStatementLog.Add(model);
                db.SaveChanges();
            }
        }

        public int GetPayingStaffId(int AgentId)
        {
            var data = db.AgentStaffInformation.Where(x => x.AgentId == AgentId).Select(x => x.Id).FirstOrDefault();
            return data;

        }




        public AgentTransactionHistoryViewModel GetTransactionHistories(TransactionType transactionService, int AgentId)
        {
            AgentTransactionHistoryViewModel transactionHistory = new AgentTransactionHistoryViewModel();
            transactionHistory.FilterKey = transactionService;
            transactionHistory.TransactionHistoryList = new List<AgentTransactionHistoryList>();
            switch (transactionService)
            {
                case TransactionType.KiiPayWallet:
                    transactionHistory.TransactionHistoryList = GetKiiPayWalletPayment(AgentId);
                    transactionHistory.TransactionHistoryList.AddRange(GetKiiPayWalletWithdrawal(AgentId));
                    break;
                case TransactionType.CashPickUp:
                    transactionHistory.TransactionHistoryList = GetCashPickUpPayment(AgentId);
                    transactionHistory.TransactionHistoryList.AddRange(GetCashPickUpWithdrawal(AgentId).ToList());

                    break;
                case TransactionType.OtherWalletTransfer:
                    transactionHistory.TransactionHistoryList = GetOtherMobileWallets(AgentId);
                    break;
                case TransactionType.BankAccountDeposit:
                    transactionHistory.TransactionHistoryList = GetBankAccountDeposit(AgentId);
                    break;
                case TransactionType.PayBillsMonthly:
                    transactionHistory.TransactionHistoryList = GetPayBillsMonthly(AgentId);
                    break;
                case TransactionType.PayBillsTopUp:
                    transactionHistory.TransactionHistoryList = GetPayBillsTopUp(AgentId);
                    break;
                default:

                    transactionHistory.TransactionHistoryList = GetKiiPayWalletPayment(AgentId).
                                                                 Concat(GetKiiPayWalletWithdrawal(AgentId)).Concat(GetCashPickUpPayment(AgentId)).Concat(GetCashPickUpWithdrawal(AgentId)).
                                                                 Concat(GetOtherMobileWallets(AgentId)).Concat(GetBankAccountDeposit(AgentId)).Concat(GetPayBillsMonthly(AgentId)).
                                                                  Concat(GetPayBillsTopUp(AgentId)).ToList();
                    break;
            }



            transactionHistory.TransactionHistoryList = transactionHistory.TransactionHistoryList.OrderByDescending(x => x.TransactionDate).ToList();
            return transactionHistory;
        }
        public List<AgentTransactionHistoryList> GetKiiPayWalletPayment(int agentId)
        {
            var result = (from c in db.TopUpSomeoneElseCardTransaction.Where(x => x.PayingStaffId == agentId).ToList()
                          join d in db.FaxerInformation on c.FaxerId equals d.Id
                          join e in db.AgentStaffInformation on c.PayingStaffId equals e.Id
                          select new AgentTransactionHistoryList()
                          {
                              Id = c.Id,
                              SenderName = d.FirstName + " " + d.MiddleName + " " + d.LastName,
                              SenderEmail = d.Email,
                              SenderNumber = d.PhoneNumber,
                              TransactionDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                              TransactionMonth = c.TransactionDate.Month,
                              Fee = c.FaxingFee,
                              AmountSent = c.FaxingAmount,
                              ReceiverName = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + " " + c.KiiPayPersonalWalletInformation.LastName,
                              ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(c.ReceivingCountry),
                              ReceivingCurrrency = Common.Common.GetCountryCurrency(c.ReceivingCountry),
                              SendingCurrency = Common.Common.GetCountryCurrency(c.SendingCountry),
                              SendingCurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                              ReceivingAmount = c.RecievingAmount,
                              ReceiverEmail = c.KiiPayPersonalWalletInformation.CardUserEmail,
                              ReceiverNumber = c.KiiPayPersonalWalletInformation.MobileNo,
                              AmountPaid = c.TotalAmount,
                              Type = Models.Type.Transfer,
                              ReceiverCity = c.KiiPayPersonalWalletInformation.CardUserCity,
                              ReceiverCountry = Common.Common.GetCountryName(c.KiiPayPersonalWalletInformation.CardUserCountry),
                              ExchangeRate = c.ExchangeRate,
                              ReceiptNumber = c.ReceiptNumber,
                              TransactionStaff = c.PayingStaffName,
                              AgentName = e.Agent.Name,
                              AgentNumber = e.Agent.AccountNo,
                              PaymentMethod = SenderPaymentMode.Cash.ToString(),
                              ReceiverDOB = c.KiiPayPersonalWalletInformation.CardUserDOB.ToString("MMM d, yyyy"),
                              CustomerType = SystemCustomerType.CustomerDeposit,
                              CustomerTypeName = Common.Common.GetEnumDescription(SystemCustomerType.CustomerDeposit),
                              AgentCommission = c.AgentCommission,
                              TransactionType = TransactionType.KiiPayWallet
                          }).ToList();

            return result;

        }

        public List<AgentTransactionHistoryList> GetKiiPayWalletWithdrawal(int agentId)
        {
            var KiipayPersonal = (from c in db.UserCardWithdrawl.Where(x => x.PayingAgentStaffId == agentId).ToList()
                                  join e in db.AgentStaffInformation on c.PayingAgentStaffId equals e.Id
                                  join d in db.AgentInformation on e.AgentId equals d.Id

                                  select new AgentTransactionHistoryList()
                                  {
                                      Id = c.Id,
                                      TransactionDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                                      TransactionMonth = c.TransactionDate.Month,
                                      ReceiverName = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + " " + c.KiiPayPersonalWalletInformation.LastName,
                                      ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                      AmountWithdrawal = c.TransactionAmount,
                                      Type = Models.Type.Withdrawal,
                                      ReceiverCity = c.KiiPayPersonalWalletInformation.CardUserCity,
                                      ReceiverCountry = Common.Common.GetCountryName(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                      ReceiptNumber = c.ReceiptNumber,
                                      TransactionStaff = c.PayingAgentName,
                                      AgentName = e.Agent.Name,
                                      ReceiverNumber = c.KiiPayPersonalWalletInformation.MobileNo,
                                      AgentNumber = e.Agent.AccountNo,
                                      PaymentMethod = SenderPaymentMode.Cash.ToString(),
                                      ReceiverEmail = c.KiiPayPersonalWalletInformation.CardUserEmail,
                                      ReceiverDOB = c.KiiPayPersonalWalletInformation.CardUserDOB.ToString("MMM d, yyyy"),
                                      CustomerType = SystemCustomerType.MOneyFexWithdrawal,
                                      CustomerTypeName = Common.Common.GetEnumDescription(SystemCustomerType.MOneyFexWithdrawal),
                                      AgentCommission = c.AgentCommission,
                                      TransactionType = TransactionType.KiiPayWallet,
                                      AgentAddress = d.Address1 + " " + d.Address2 + " " + d.City + " " + Common.Common.GetCountryName(d.CountryCode),
                                      AgentCurrency = Common.Common.GetCountryCurrency(d.CountryCode),
                                      PayingStaffAgentName = e.FirstName + " " + e.MiddleName + " " + e.LastName,
                                      PayingStaffAccount = e.AgentMFSCode,

                                  }).ToList();


            var kiipayBusiness = (from c in db.MFBCCardWithdrawls.Where(x => x.PayingAgentStaffId == agentId).ToList()
                                  join e in db.AgentStaffInformation on c.PayingAgentStaffId equals e.Id
                                  join d in db.AgentInformation on e.AgentId equals d.Id
                                  select new AgentTransactionHistoryList()
                                  {
                                      Id = c.Id,
                                      TransactionDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                                      TransactionMonth = c.TransactionDate.Month,
                                      ReceiverName = c.KiiPayBusinessWalletInformation.FirstName + " " + c.KiiPayBusinessWalletInformation.MiddleName + " " + c.KiiPayBusinessWalletInformation.LastName,
                                      ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(c.KiiPayBusinessWalletInformation.Country),
                                      AmountWithdrawal = c.TransactionAmount,
                                      Type = Models.Type.Withdrawal,
                                      ReceiverCity = c.KiiPayBusinessWalletInformation.City,
                                      ReceiverCountry = Common.Common.GetCountryName(c.KiiPayBusinessWalletInformation.Country),
                                      ReceiptNumber = c.ReceiptNumber,
                                      TransactionStaff = c.PayingAgentName,
                                      AgentName = e.Agent.Name,
                                      ReceiverNumber = c.KiiPayBusinessWalletInformation.MobileNo,
                                      AgentNumber = e.Agent.AccountNo,
                                      PaymentMethod = SenderPaymentMode.Cash.ToString(),
                                      ReceiverEmail = c.KiiPayBusinessWalletInformation.Email,
                                      ReceiverDOB = c.KiiPayBusinessWalletInformation.DOB.ToString("MMM d, yyyy"),
                                      CustomerType = SystemCustomerType.MOneyFexWithdrawal,
                                      CustomerTypeName = Common.Common.GetEnumDescription(SystemCustomerType.MOneyFexWithdrawal),
                                      TransactionType = TransactionType.KiiPayWallet,
                                      AgentCommission = c.AgentCommission,
                                      AgentAddress = d.Address1 + " " + d.Address2 + " " + d.City + " " + Common.Common.GetCountryName(d.CountryCode),
                                      AgentCurrency = Common.Common.GetCountryCurrency(d.CountryCode),
                                      PayingStaffAgentName = e.FirstName + " " + e.MiddleName + " " + e.LastName,
                                      PayingStaffAccount = e.AgentMFSCode,

                                  }).ToList();


            return KiipayPersonal.Concat(kiipayBusiness).ToList();

        }

        public List<AgentTransactionHistoryList> GetCashPickUpPayment(int agentId)
        {
            var result = (from c in db.FaxingNonCardTransaction.Where(x => x.PayingStaffId == agentId).ToList()
                          join d in db.FaxerInformation on c.NonCardReciever.FaxerID equals d.Id
                          join e in db.AgentStaffInformation on c.PayingStaffId equals e.Id
                          select new AgentTransactionHistoryList()
                          {
                              Id = c.Id,
                              SenderName = d.FirstName + " " + d.MiddleName + " " + d.LastName,
                              SenderEmail = d.Email,
                              SenderNumber = d.PhoneNumber,
                              TransactionDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                              TransactionMonth = c.TransactionDate.Month,
                              Fee = c.FaxingFee,
                              AmountSent = c.FaxingAmount,
                              ReceiverName = c.NonCardReciever.FirstName + " " + c.NonCardReciever.MiddleName + " " + c.NonCardReciever.LastName,
                              ReceivingCurrencySymbol = Common.Common.GetCurrencySymbolByCurrencyOrCountry(c.ReceivingCurrency, c.ReceivingCountry),
                              ReceivingCurrrency = Common.Common.GetCurrencyByCurrencyOrCountry(c.ReceivingCurrency, c.ReceivingCountry),
                              SendingCurrency = Common.Common.GetCountryCurrency(c.SendingCountry),
                              SendingCurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                              ReceivingAmount = c.ReceivingAmount,
                              ReceiverEmail = c.NonCardReciever.EmailAddress,
                              ReceiverNumber = c.NonCardReciever.PhoneNumber,
                              AmountPaid = c.TotalAmount,
                              Type = Models.Type.Transfer,
                              ReceiverCity = c.NonCardReciever.City,
                              ReceiverCountry = Common.Common.GetCountryName(c.NonCardReciever.Country),
                              ExchangeRate = c.ExchangeRate,
                              ReceiptNumber = c.ReceiptNumber,
                              TransactionStaff = c.AgentStaffName,
                              AgentName = e.Agent.Name,
                              AgentNumber = e.Agent.AccountNo,
                              PaymentMethod = c.PaymentMethod,
                              MFCN = c.MFCN,
                              Status = Common.Common.GetEnumDescription(c.FaxingStatus),
                              SenderDOB = d.DateOfBirth.ToFormatedString("MMM d, yyyy"),
                              CustomerType = SystemCustomerType.CustomerPayment,
                              CustomerTypeName = Common.Common.GetEnumDescription(SystemCustomerType.CustomerPayment),
                              AgentCommission = c.AgentCommission,
                              TransactionType = TransactionType.CashPickUp
                          }).ToList();

            return result;
        }

        public List<AgentTransactionHistoryList> GetCashPickUpWithdrawal(int agentId)
        {
            var result = (from c in db.ReceiverNonCardWithdrawl.Where(x => x.PayingAgentStaffId == agentId).ToList()
                          join e in db.AgentInformation on c.Agent.Id equals e.Id
                          join d in db.FaxerInformation on c.ReceiversDetails.FaxerID equals d.Id
                          join f in db.AgentStaffInformation on e.Id equals f.AgentId
                          join trans in db.FaxingNonCardTransaction on c.MFCN equals trans.MFCN
                          select new AgentTransactionHistoryList()
                          {
                              Id = c.Id,
                              TransactionDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                              TransactionMonth = c.TransactionDate.Month,
                              ReceiverName = c.ReceiversDetails.FirstName + " " + c.ReceiversDetails.MiddleName + " " + c.ReceiversDetails.LastName,
                              ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(c.ReceiversDetails.Country),
                              AmountWithdrawal = c.TransactionAmount,
                              Type = Models.Type.Received,
                              ReceiverCity = c.ReceiversDetails.City,
                              ReceiverCountry = Common.Common.GetCountryName(c.ReceiversDetails.Country),
                              ReceiptNumber = c.ReceiptNumber,
                              TransactionStaff = c.PayingAgentName,
                              AgentName = e.Name,
                              ReceiverNumber = c.ReceiversDetails.PhoneNumber,
                              AgentNumber = e.AccountNo,
                              PaymentMethod = SenderPaymentMode.Cash.ToString(),
                              ReceiverEmail = c.ReceiversDetails.EmailAddress,
                              SenderName = d.FirstName + " " + d.MiddleName + " " + d.LastName,
                              SenderEmail = d.Email,
                              SenderNumber = d.PhoneNumber,
                              Fee = trans.FaxingFee,
                              AmountSent = trans.FaxingAmount,
                              ReceivingCurrrency = Common.Common.GetCountryCurrency(c.ReceiversDetails.Country),
                              SendingCurrency = Common.Common.GetCountryCurrency(d.Country),
                              SendingCurrencySymbol = Common.Common.GetCurrencySymbol(d.Country),
                              ReceivingAmount = c.ReceivingAmount,
                              AmountPaid = trans.TotalAmount,
                              ExchangeRate = trans.ExchangeRate,
                              MFCN = c.MFCN,
                              SenderDOB = d.DateOfBirth.ToFormatedString("MMM d, yyyy"),
                              CustomerType = SystemCustomerType.MOneyFexWithdrawal,
                              CustomerTypeName = Common.Common.GetEnumDescription(SystemCustomerType.MOneyFexWithdrawal),
                              AgentCommission = c.AgentCommission,
                              TransactionType = TransactionType.CashPickUp,
                              AgentAddress = e.Address1 + " " + e.Address2 + " " + e.City + " " + Common.Common.GetCountryName(e.CountryCode),
                              AgentCurrency = Common.Common.GetCountryCurrency(e.CountryCode),
                              PayingStaffAgentName = f.FirstName + " " + f.MiddleName + " " + f.LastName,
                              PayingStaffAccount = f.AgentMFSCode,


                          }).ToList();
            return result;


        }

        public List<AgentTransactionHistoryList> GetOtherMobileWallets(int agentId)
        {
            var data = db.MobileMoneyTransfer.Where(x => x.PayingStaffId == agentId).ToList();

            var result = new List<AgentTransactionHistoryList>();
            try
            {
                result = (from c in db.MobileMoneyTransfer.Where(x => x.PayingStaffId == agentId).ToList()
                          join d in db.FaxerInformation on c.SenderId equals d.Id
                          join e in db.AgentStaffInformation on c.PayingStaffId equals e.Id
                          select new AgentTransactionHistoryList()
                          {
                              Id = c.Id,
                              SenderName = d.FirstName + " " + d.MiddleName + " " + d.LastName,
                              SenderEmail = d.Email,
                              SenderNumber = d.PhoneNumber,
                              TransactionDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                              TransactionMonth = c.TransactionDate.Month,
                              Fee = c.Fee,
                              AmountSent = c.SendingAmount,
                              ReceiverName = c.ReceiverName,
                              ReceivingCurrencySymbol = Common.Common.GetCurrencySymbolByCurrencyOrCountry(c.ReceivingCurrency, c.ReceivingCountry),
                              ReceivingCurrrency = Common.Common.GetCurrencyByCurrencyOrCountry(c.ReceivingCurrency, c.ReceivingCountry),

                              SendingCurrency = Common.Common.GetCountryCurrency(c.SendingCountry),
                              SendingCurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                              ReceivingAmount = c.ReceivingAmount,
                              ReceiverNumber = c.PaidToMobileNo,
                              AmountPaid = c.TotalAmount,
                              Type = Models.Type.Transfer,
                              ReceiverCity = c.ReceiverCity,
                              ReceiverCountry = Common.Common.GetCountryName(c.ReceivingCountry),
                              ExchangeRate = c.ExchangeRate,
                              ReceiptNumber = c.ReceiptNo,
                              TransactionStaff = c.PayingStaffName,
                              AgentName = e.Agent.Name,
                              AgentNumber = e.Agent.AccountNo,
                              SenderDOB = d.DateOfBirth == null ? "" : d.DateOfBirth.ToFormatedString("MMM d, yyyy"),
                              PaymentMethod = "Cash",
                              AccountNumber = c.PaidToMobileNo,
                              WalletName = c.WalletOperatorId > 0 ? Common.Common.GetMobileWalletInfo(c.WalletOperatorId).Name : "",
                              CustomerType = SystemCustomerType.CustomerDeposit,
                              CustomerTypeName = Common.Common.GetEnumDescription(SystemCustomerType.CustomerDeposit),
                              AgentCommission = c.AgentCommission,
                              TransactionType = TransactionType.OtherWalletTransfer
                          }).ToList();
            }
            catch (Exception ex)
            {

                throw;
            }


            return result;
        }

        public List<AgentTransactionHistoryList> GetBankAccountDeposit(int agentId)
        {
            var result = (from c in db.BankAccountDeposit.Where(x => x.PayingStaffId == agentId).ToList()
                          join d in db.FaxerInformation on c.SenderId equals d.Id
                          join e in db.AgentStaffInformation on c.PayingStaffId equals e.Id
                          select new AgentTransactionHistoryList()
                          {
                              Id = c.Id,
                              SenderName = d.FirstName + " " + d.MiddleName + " " + d.LastName,
                              SenderEmail = d.Email,
                              SenderNumber = d.PhoneNumber,
                              TransactionDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                              TransactionMonth = c.TransactionDate.Month,
                              Fee = c.Fee,
                              AmountSent = c.SendingAmount,
                              ReceiverName = c.ReceiverName,
                              ReceivingCurrencySymbol = Common.Common.GetCurrencySymbolByCurrencyOrCountry(c.ReceivingCurrency, c.ReceivingCountry),
                              ReceivingCurrrency = Common.Common.GetCurrencyByCurrencyOrCountry(c.ReceivingCurrency, c.ReceivingCountry),
                              SendingCurrency = Common.Common.GetCountryCurrency(c.SendingCountry),
                              SendingCurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                              ReceivingAmount = c.ReceivingAmount,
                              ReceiverNumber = c.ReceiverMobileNo,
                              BankBranch = c.BankCode,
                              BankName = db.Bank.Where(x => x.Id == c.BankId).Select(x => x.Name).FirstOrDefault(),
                              AmountPaid = c.TotalAmount,
                              Type = Models.Type.Transfer,
                              ReceiverCity = c.ReceiverCity,
                              SenderCountry = Common.Common.GetCountryName(c.SendingCountry),
                              ReceiverCountry = Common.Common.GetCountryName(c.ReceivingCountry),
                              ExchangeRate = c.ExchangeRate,
                              ReceiptNumber = c.ReceiptNo,
                              TransactionStaff = c.PayingStaffName,
                              AgentName = e.Agent.Name,
                              AgentNumber = e.Agent.AccountNo,
                              AccountNumber = c.ReceiverAccountNo,
                              SenderDOB = d.DateOfBirth.ToFormatedString("MMM d, yyyy"),
                              PaymentMethod = "Cash",
                              Status = Common.Common.GetEnumDescription(c.Status),
                              BankStatus = c.Status,
                              IsRetryableCountry = Common.Common.IsRetryAbleCountry(c.Id),
                              CustomerType = SystemCustomerType.CustomerDeposit,
                              CustomerTypeName = Common.Common.GetEnumDescription(SystemCustomerType.CustomerDeposit),
                              AgentCommission = c.AgentCommission,
                              TransactionType = TransactionType.BankAccountDeposit

                          }).ToList();

            return result;
        }
        public List<AgentTransactionHistoryList> GetManualBankAccountDepositTrasactionDetails(int Id)
        {
            var result = (from c in db.BankAccountDeposit.Where(x => x.Id == Id && x.IsManualDeposit == true).ToList()
                          join d in db.FaxerInformation on c.SenderId equals d.Id
                          select new AgentTransactionHistoryList()
                          {
                              Id = c.Id,
                              SenderName = d.FirstName + " " + d.MiddleName + " " + d.LastName,
                              SenderEmail = d.Email,
                              SenderNumber = d.PhoneNumber,
                              TransactionDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                              TransactionMonth = c.TransactionDate.Month,
                              Fee = c.Fee,
                              AmountSent = c.SendingAmount,
                              ReceiverName = c.ReceiverName,
                              ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(c.ReceivingCountry),
                              ReceivingCurrrency = Common.Common.GetCountryCurrency(c.ReceivingCountry),
                              SendingCurrency = Common.Common.GetCountryCurrency(c.SendingCountry),
                              SendingCurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                              ReceivingAmount = c.ReceivingAmount,
                              ReceiverNumber = c.ReceiverMobileNo,
                              BankBranch = c.BankCode,
                              BankName = db.Bank.Where(x => x.Id == c.BankId).Select(x => x.Name).FirstOrDefault(),
                              AmountPaid = c.TotalAmount,
                              Type = Models.Type.Transfer,
                              ReceiverCity = c.ReceiverCity,
                              ReceiverCountry = Common.Common.GetCountryName(c.ReceivingCountry),
                              ExchangeRate = c.ExchangeRate,
                              ReceiptNumber = c.ReceiptNo,
                              TransactionStaff = c.PayingStaffName,
                              SenderCountry = Common.Common.GetCountryName(c.SendingCountry),
                              BankCode = c.BankCode,
                              AccountNumber = c.ReceiverAccountNo,
                              SenderDOB = d.DateOfBirth.ToFormatedString("MMM d, yyyy"),
                              PaymentMethod = "Cash",
                              Status = Common.Common.GetEnumDescription(c.Status),
                              CustomerType = SystemCustomerType.CustomerDeposit,
                              CustomerTypeName = Common.Common.GetEnumDescription(SystemCustomerType.CustomerDeposit),
                              AgentCommission = c.AgentCommission,
                              TransactionType = TransactionType.BankAccountDeposit

                          }).ToList();

            return result;
        }


        public List<AgentTransactionHistoryList> GetPayBillsTopUp(int agentId)
        {
            var result = (from c in db.TopUpToSupplier.Where(x => x.PayingStaffId == agentId).ToList()
                          join e in db.AgentInformation on c.PayerId equals e.Id
                          select new AgentTransactionHistoryList()
                          {
                              Id = c.Id,
                              TransactionDate = c.PaymentDate.ToString("dd/MM/yyyy"),
                              TransactionMonth = c.PaymentDate.Month,
                              Fee = c.Fee,
                              AmountSent = c.SendingAmount,
                              ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(Common.Common.GetCountryCodeByCountryName(c.SupplierCountry)),
                              ReceivingCurrrency = Common.Common.GetCountryCurrency(Common.Common.GetCountryCodeByCountryName(c.SupplierCountry)),
                              SendingCurrency = Common.Common.GetCountryCurrency(Common.Common.GetCountryCodeByCountryName(c.PayingCountry)),
                              SendingCurrencySymbol = Common.Common.GetCurrencySymbol(Common.Common.GetCountryCodeByCountryName(c.PayingCountry)),
                              ReceivingAmount = c.ReceivingAmount,
                              SenderName = db.Suppliers.Where(x => x.Id == c.SuplierId).Select(x => x.KiiPayBusinessInformation.BusinessName).FirstOrDefault(),
                              ReceiverName = db.Suppliers.Where(x => x.Id == c.SuplierId).Select(x => x.KiiPayBusinessInformation.BusinessName).FirstOrDefault(),
                              AmountPaid = c.TotalAmount,
                              Type = Models.Type.Transfer,
                              PaymentMethod = "Cash",
                              ExchangeRate = c.EcxhangeRate,
                              ReceiptNumber = c.ReceiptNo,
                              TransactionStaff = c.PayingStaffName,
                              AgentName = e.Name,
                              AgentNumber = e.AccountNo,
                              AccountNumber = c.SupplierAccountNo,
                              ReceiverNumber = c.WalletNo,
                              CustomerType = SystemCustomerType.CustomerPayment,
                              CustomerTypeName = Common.Common.GetEnumDescription(SystemCustomerType.CustomerPayment),
                              AgentCommission = c.AgentCommission,
                              TransactionType = TransactionType.PayBillsTopUp


                          }).ToList();

            return result;
        }

        public List<AgentTransactionHistoryList> GetPayBillsMonthly(int agentId)
        {
            var result = (from c in db.PayBill.Where(x => x.PayingStaffId == agentId).ToList()
                          join e in db.AgentInformation on c.PayerId equals e.Id
                          select new AgentTransactionHistoryList()
                          {
                              Id = c.Id,
                              TransactionDate = c.PaymentDate.ToString("dd/MM/yyyy"),
                              TransactionMonth = c.PaymentDate.Month,
                              Fee = c.Fee,
                              AmountSent = c.SendingAmount,
                              ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(Common.Common.GetCountryCodeByCountryName(c.SupplierCountry)),
                              ReceivingCurrrency = Common.Common.GetCountryCurrency(Common.Common.GetCountryCodeByCountryName(c.SupplierCountry)),
                              SendingCurrency = Common.Common.GetCountryCurrency(Common.Common.GetCountryCodeByCountryName(c.PayerCountry)),
                              SendingCurrencySymbol = Common.Common.GetCurrencySymbol(Common.Common.GetCountryCodeByCountryName(c.PayerCountry)),
                              ReceivingAmount = c.Amount,
                              SenderName = db.Suppliers.Where(x => x.Id == c.SupplierId).Select(x => x.KiiPayBusinessInformation.BusinessName).FirstOrDefault(),
                              ReceiverName = db.Suppliers.Where(x => x.Id == c.SupplierId).Select(x => x.KiiPayBusinessInformation.BusinessName).FirstOrDefault(),
                              AmountPaid = c.Total,
                              Type = Models.Type.Transfer,
                              PaymentMethod = c.SenderPaymentMode.ToString(),
                              ExchangeRate = c.ExchangeRate,
                              ReceiptNumber = c.ReceiptNo,
                              TransactionStaff = c.PayingStaffName,
                              AgentName = e.Name,
                              AgentNumber = e.AccountNo,
                              AccountNumber = c.RefCode,
                              ReceiverNumber = c.Supplier.KiiPayBusinessInformation.BusinessMobileNo,
                              CustomerType = SystemCustomerType.CustomerPayment,
                              CustomerTypeName = Common.Common.GetEnumDescription(SystemCustomerType.CustomerPayment),
                              AgentCommission = c.AgentCommission,
                              TransactionType = TransactionType.PayBillsMonthly

                          }).ToList();

            return result;
        }
    }
}