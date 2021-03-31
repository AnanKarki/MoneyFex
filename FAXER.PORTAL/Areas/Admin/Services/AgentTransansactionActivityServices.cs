using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class AgentTransansactionActivityServices
    {
        DB.FAXEREntities dbContext = null;
        public AgentTransansactionActivityServices()
        {
            dbContext = new DB.FAXEREntities();
        }


        public List<AgentTransctionActivityVm> GetAgentTransactionHistory(int AgentId, TransactionServiceType transactionServiceType, int year = 0, int month = 0, int Day = 0)
        {
            var activityList = new List<AgentTransctionActivityVm>();

            switch (transactionServiceType)
            {
                case TransactionServiceType.MobileWallet:
                    //transactionHistory.TransactionHistoryList = GetMobileTransferDetails(faxerId);
                    break;
                case TransactionServiceType.KiiPayWallet:
                    activityList.AddRange(GetKiiPayPersonalTransaction(AgentId, year, month, Day));
                    break;
                case TransactionServiceType.BillPayment:
                    //transactionHistory.TransactionHistoryList = GetBillPaymentDetails(faxerId);
                    break;
                case TransactionServiceType.ServicePayment:
                    activityList.AddRange(GetkiiPayBusinessTransaction(AgentId, year, month, Day));
                    //transactionHistory.TransactionHistoryList = GetServicePaymentDetails(faxerId);
                    break;
                case TransactionServiceType.CashPickUp:
                    activityList.AddRange(GetAgentCashPickUpTransaction(AgentId, year, month, Day));
                    //transactionHistory.TransactionHistoryList = GetCashPickUpDetails(faxerId);
                    break;
                case TransactionServiceType.BankDeposit:
                    activityList.AddRange(GetkiiPayBankDeposit(AgentId, year, month, Day));
                    //transactionHistory.TransactionHistoryList = GetBankDepositDetails(faxerId);
                    break;
                default:
                    activityList.AddRange(GetKiiPayPersonalTransaction(AgentId, year, month, Day));
                    activityList.AddRange(GetkiiPayBusinessTransaction(AgentId, year, month, Day));
                    activityList.AddRange(GetAgentCashPickUpTransaction(AgentId, year, month, Day));
                    activityList.AddRange(GetkiiPayBankDeposit(AgentId, year, month, Day));
                    break;
            }



            activityList.AddRange(GetCashwithdrawal(AgentId, year, month, Day));
            activityList.AddRange(GetkiipayPersonalWithdrawal(AgentId, year, month, Day));
            activityList.AddRange(GetkiipayBusinessWithdrawal(AgentId, year, month, Day));

            var TransactionType = Common.Common.GetEnumDescription(transactionServiceType);
            var TransactionData = activityList.Where(x => x.TransferMethod == TransactionType).ToList();


            if (transactionServiceType == TransactionServiceType.All)
            {
                TransactionData = activityList;

            }

            return TransactionData;
        }


        public AgentLogin getLoginInfo(int agentId)
        {
            var data = dbContext.AgentLogin.Where(x => x.AgentId == agentId).FirstOrDefault();
            return data;

        }

        public List<AgentTransctionActivityVm> GetAgentCashPickUpTransaction(int AgentId, int year = 0, int month = 0, int Day = 0)
        {
            var data = dbContext.AgentFaxMoneyInformation.Where(x => x.AgentId == AgentId).ToList();
            if (year != 0)
            {
                data = data.Where(x => x.NonCardTransaction.TransactionDate.Year == year).ToList();
            }
            if (month != 0)
            {
                data = data.Where(x => x.NonCardTransaction.TransactionDate.Month == month).ToList();
            }
            if (Day != 0)
            {
                data = data.Where(x => x.NonCardTransaction.TransactionDate.Day == Day).ToList();
            }

            var cashpickup = (from c in data
                              select new AgentTransctionActivityVm()
                              {
                                  Id = c.Id,
                                  Amount = Common.Common.GetCurrencySymbol(c.NonCardTransaction.SendingCountry) + c.NonCardTransaction.FaxingAmount,
                                  Fee = Common.Common.GetCurrencySymbol(c.NonCardTransaction.SendingCountry) + c.NonCardTransaction.FaxingFee,
                                  DateTime = c.NonCardTransaction.TransactionDate.ToString("dd/MM/yyyy"),
                                  identifier = c.NonCardTransaction.ReceiptNumber,
                                  SenderId = c.NonCardTransaction.NonCardReciever.FaxerID,
                                  TransferMethod = Common.Common.GetEnumDescription(TransactionServiceType.CashPickUp),
                                  TransferType = Common.Common.GetEnumDescription(AgentTransferType.Transfer),
                                  StaffName = c.NameOfPayingAgent
                              }).ToList();

            return cashpickup;
        }


        public List<AgentTransctionActivityVm> GetKiiPayPersonalTransaction(int AgentId, int year = 0, int month = 0, int Day = 0)
        {
            var data = dbContext.VirtualAccountDepositByAgent.Where(x => x.AgentId == AgentId).ToList();
            if (year != 0)
            {
                data = data.Where(x => x.TransactionDate.Year == year).ToList();
            }
            if (month != 0)
            {
                data = data.Where(x => x.TransactionDate.Month == month).ToList();
            }
            if (month != 0)
            {
                data = data.Where(x => x.TransactionDate.Day == Day).ToList();
            }

            var kiipayPersonalTransaction = (from c in data
                                             select new AgentTransctionActivityVm()
                                             {
                                                 Id = c.Id,
                                                 Amount = Common.Common.GetCurrencySymbol(c.Faxer.Country) + c.DepositAmount,
                                                 Fee = Common.Common.GetCurrencySymbol(c.Faxer.Country) + c.DepositFees,
                                                 DateTime = c.TransactionDate.ToString("dd/MM/yyyy"),
                                                 identifier = c.ReceiptNumber,
                                                 SenderId = c.FaxerId,
                                                 TransferMethod = Common.Common.GetEnumDescription(TransactionServiceType.KiiPayWallet),
                                                 TransferType = Common.Common.GetEnumDescription(AgentTransferType.Transfer),
                                                 StaffName = c.PayingStaffName
                                             }).ToList();
            return kiipayPersonalTransaction;
        }
        public List<AgentTransctionActivityVm> GetkiiPayBusinessTransaction(int AgentId, int year = 0, int month = 0, int Day = 0)
        {
            var data = dbContext.BusinessAccountDepositByAgent.Where(x => x.AgentId == AgentId).ToList();
            if (year != 0)
            {
                data = data.Where(x => x.TransactionDate.Year == year).ToList();
            }
            if (month != 0)
            {
                data = data.Where(x => x.TransactionDate.Month == month).ToList();
            }
            if (Day != 0)
            {
                data = data.Where(x => x.TransactionDate.Day == Day).ToList();
            }


            var kiiPayBusinessTransaction = (from c in data
                                             select new AgentTransctionActivityVm()
                                             {
                                                 Id = c.Id,
                                                 Amount = Common.Common.GetCurrencySymbol(c.Sender.Country) + c.DepositAmount,
                                                 DateTime = c.TransactionDate.ToString("dd/MM/yyyy"),
                                                 Fee = Common.Common.GetCurrencySymbol(c.Sender.Country) + c.DepositFees,
                                                 identifier = c.ReceiptNumber,
                                                 SenderId = c.SenderId,
                                                 TransferMethod = Common.Common.GetEnumDescription(TransactionServiceType.ServicePayment),
                                                 TransferType = Common.Common.GetEnumDescription(AgentTransferType.Transfer),
                                                 StaffName = c.PayingStaffName
                                             }).ToList();
            return kiiPayBusinessTransaction;

        }

        public List<AgentTransctionActivityVm> GetkiiPayBankDeposit(int AgentId, int year = 0, int month = 0, int Day = 0)
        {
            var data = dbContext.BankAccountDeposit.Where(x => x.PaidFromModule == DB.Module.Agent).ToList();
            if (year != 0)
            {
                data = data.Where(x => x.TransactionDate.Year == year).ToList();
            }
            if (month != 0)
            {
                data = data.Where(x => x.TransactionDate.Month == month).ToList();
            }
            if (Day != 0)
            {
                data = data.Where(x => x.TransactionDate.Day == Day).ToList();
            }

            var kiiPayBankDeposit = (from c in data
                                     join d in dbContext.AgentStaffInformation.ToList() on c.PayingStaffId equals d.Id
                                     where d.AgentId == AgentId
                                     select new AgentTransctionActivityVm()
                                     {
                                         Id = c.Id,
                                         Amount = Common.Common.GetCurrencySymbol(c.SendingCountry) + c.SendingAmount,
                                         DateTime = c.TransactionDate.ToString("dd/MM/yyyy"),
                                         Fee = Common.Common.GetCurrencySymbol(c.SendingCountry) + c.Fee,
                                         identifier = c.ReceiptNo,
                                         SenderId = c.SenderId,
                                         TransferMethod = Common.Common.GetEnumDescription(TransactionServiceType.BankDeposit),
                                         TransferType = Common.Common.GetEnumDescription(AgentTransferType.Transfer),
                                         StaffName = c.PayingStaffName
                                     }).ToList();
            return kiiPayBankDeposit;

        }
        public List<AgentTransctionActivityVm> GetCashwithdrawal(int AgentId, int year = 0, int month = 0, int Day = 0)
        {
            var data = dbContext.ReceiverNonCardWithdrawl.Where(x => x.AgentId == AgentId).ToList();
            if (year != 0)
            {
                data = data.Where(x => x.TransactionDate.Year == year).ToList();
            }
            if (month != 0)
            {
                data = data.Where(x => x.TransactionDate.Month == month).ToList();
            }
            if (Day != 0)
            {
                data = data.Where(x => x.TransactionDate.Day == Day).ToList();
            }


            var cashwithdrawal = (from c in data
                                  select new AgentTransctionActivityVm()
                                  {
                                      Id = c.Id,
                                      Amount = Common.Common.GetCurrencySymbol(c.ReceiversDetails.Country) + c.ReceivingAmount,
                                      Fee = "0",
                                      DateTime = c.TransactionDate.ToString("dd/MM/yyyy"),
                                      identifier = c.MFCN,
                                      SenderId = c.ReceiversDetails.FaxerID,
                                      TransferMethod = Common.Common.GetEnumDescription(TransactionServiceType.CashPickUp),
                                      TransferType = Common.Common.GetEnumDescription(AgentTransferType.Withdrawal),
                                      StaffName = c.Agent.Name
                                  }).ToList();
            return cashwithdrawal;

        }

        public List<AgentTransctionActivityVm> GetkiipayPersonalWithdrawal(int AgentId, int year = 0, int month = 0, int Day = 0)
        {

            var data = dbContext.UserCardWithdrawl.Where(x => x.AgentInformationId == AgentId).ToList();
            if (year != 0)
            {
                data = data.Where(x => x.TransactionDate.Year == year).ToList();
            }
            if (month != 0)
            {
                data = data.Where(x => x.TransactionDate.Month == month).ToList();
            }
            if (Day != 0)
            {
                data = data.Where(x => x.TransactionDate.Day == Day).ToList();
            }


            var kiipayPersonalWithdrawal = (from c in data
                                            select new AgentTransctionActivityVm()
                                            {
                                                Id = c.Id,
                                                Amount = Common.Common.GetCurrencySymbol(c.KiiPayPersonalWalletInformation.CardUserCountry) + c.TransactionAmount,
                                                Fee = "0",
                                                DateTime = c.TransactionDate.ToString("dd/MM/yyyy"),
                                                identifier = c.ReceiptNumber,
                                                SenderId = c.KiiPayPersonalWalletInformationId,
                                                TransferMethod = Common.Common.GetEnumDescription(TransactionServiceType.KiiPayWallet),
                                                TransferType = Common.Common.GetEnumDescription(AgentTransferType.Withdrawal),
                                                StaffName = c.PayingAgentName
                                            }).ToList();
            return kiipayPersonalWithdrawal;

        }
        public List<AgentTransctionActivityVm> GetkiipayBusinessWithdrawal(int AgentId, int year = 0, int month = 0, int day = 0)
        {
            var data = dbContext.MFBCCardWithdrawls.Where(x => x.AgentInformationId == AgentId).ToList();
            if (year != 0)
            {
                data = data.Where(x => x.TransactionDate.Year == year).ToList();
            }
            if (month != 0)
            {
                data = data.Where(x => x.TransactionDate.Month == month).ToList();
            }
            if (day != 0)
            {
                data = data.Where(x => x.TransactionDate.Day == day).ToList();
            }

            var kiipayBusinessWithdrawal = (from c in data
                                            select new AgentTransctionActivityVm()
                                            {
                                                Id = c.Id,
                                                Amount = Common.Common.GetCurrencySymbol(c.KiiPayBusinessWalletInformation.Country) + c.TransactionAmount,
                                                DateTime = c.TransactionDate.ToString("dd/MM/yyyy"),
                                                Fee = "0",
                                                identifier = c.ReceiptNumber,
                                                SenderId = c.KiiPayBusinessWalletInformationId,
                                                TransferMethod = Common.Common.GetEnumDescription(TransactionServiceType.KiiPayWallet),
                                                TransferType = Common.Common.GetEnumDescription(AgentTransferType.Withdrawal),
                                                StaffName = c.PayingAgentName

                                            }).ToList();
            return kiipayBusinessWithdrawal;

        }



        public List<DailyTransactionStatementListVm> getRecentTransactionStatementList()
        {

            var agentStaffInformation = dbContext.AgentStaffInformation.Where(x => x.Agent.IsAUXAgent == false);
            var CashPickupReceiver = (from c in dbContext.ReceiverNonCardWithdrawl.Where(x => x.PayingAgentStaffId != 0).ToList()
                                      join d in agentStaffInformation on c.PayingAgentStaffId equals d.Id
                                      select new DailyTransactionStatementListVm()
                                      {
                                          Id = c.Id,
                                          TransactionType = TransactionType.CashPickUp,
                                          TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.CashPickUp),
                                          CurrencySymbol = Common.CountryUtility.GetCountryCurrencySymbol(c.IssuingCountryCode),
                                          Currency = Common.Common.GetCountryCurrency(c.IssuingCountryCode),
                                          Amount = c.ReceivingAmount,
                                          Fee = 0M,
                                          TransactionIdentifier = c.ReceiptNumber,
                                          DateAndTime = c.TransactionDate,
                                          StaffName = c.PayingAgentName,
                                          //Type = Models.Type.Received,
                                          AgentCommission = c.AgentCommission,
                                          ReceivingCountry= Common.Common.GetCountryName(c.Agent.CountryCode),
                                          SendingCountry = Common.Common.GetCountryName(c.IssuingCountryCode),
                                          StatusName = "Completed",
                                          AgentId = d.AgentId,
                                          TransferMethod = "Cash Pickup",
                                      }).ToList();

            var KiiPayWalletTransfer = (from c in dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.PayingStaffId != null).ToList()
                                        join d in agentStaffInformation on c.PayingStaffId equals d.Id
                                        select new DailyTransactionStatementListVm()
                                        {
                                            Id = c.Id,
                                            TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.KiiPayWallet),
                                            TransactionType = TransactionType.KiiPayWallet,
                                            CurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                                            Currency = Common.Common.GetCountryCurrency(c.SendingCountry),
                                            Amount = c.FaxingAmount,
                                            Fee = c.FaxingFee,
                                            TransactionIdentifier = c.ReceiptNumber,
                                            DateAndTime = c.TransactionDate,
                                            StaffName = c.PayingStaffName,
                                            //Type = Models.Type.Transfer,
                                            AgentCommission = c.AgentCommission,
                                            ReceivingCountry = Common.Common.GetCountryName(c.ReceivingCountry),
                                            SendingCountry = Common.Common.GetCountryName(c.SendingCountry),
                                            StatusName = "Completed",
                                            AgentId = d.AgentId,
                                            TransferMethod = "KiiPay Wallet",

                                        }).ToList();

            var KiiPayPersonalWalletReceiver = (from c in dbContext.UserCardWithdrawl.Where(x => x.PayingAgentStaffId != 0).ToList()
                                                join d in agentStaffInformation on c.PayingAgentStaffId equals d.Id
                                                select new DailyTransactionStatementListVm()
                                                {
                                                    Id = c.Id,
                                                    TransactionType = TransactionType.KiiPayWallet,
                                                    TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.KiiPayWallet),
                                                    CurrencySymbol = Common.Common.GetCurrencySymbol(c.IssuingCountryCode),
                                                    Currency = Common.Common.GetCountryCurrency(c.IssuingCountryCode),
                                                    Amount = c.TransactionAmount,
                                                    Fee = 0M,
                                                    TransactionIdentifier = c.ReceiptNumber,
                                                    DateAndTime = c.TransactionDate,
                                                    StaffName = c.PayingAgentName,
                                                    AgentCommission = 0,
                                                    //Type = Models.Type.Received
                                                    ReceivingCountry = Common.Common.GetCountryName(c.AgentInformation.CountryCode),
                                                    SendingCountry = Common.Common.GetCountryName(c.AgentInformation.CountryCode),
                                                    StatusName = "Completed",
                                                    AgentId = d.AgentId,
                                                    TransferMethod = "KiiPay Wallet",
                                                }).ToList();
            var KiiPayBusinessWalletReceiver = (from c in dbContext.MFBCCardWithdrawls.Where(x => x.PayingAgentStaffId != 0).ToList()
                                                join d in agentStaffInformation on c.PayingAgentStaffId equals d.Id
                                                select new DailyTransactionStatementListVm()
                                                {
                                                    Id = c.Id,
                                                    TransactionType = TransactionType.KiiPayWallet,
                                                    TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.KiiPayWallet),
                                                    CurrencySymbol = Common.Common.GetCurrencySymbol(c.IssuingCountryCode),
                                                    Currency = Common.Common.GetCountryCurrency(c.IssuingCountryCode),
                                                    Amount = c.TransactionAmount,
                                                    Fee = 0M,
                                                    TransactionIdentifier = c.ReceiptNumber,
                                                    DateAndTime = c.TransactionDate,
                                                    StaffName = c.PayingAgentName,
                                                    AgentCommission = 0,
                                                    //Type = Models.Type.Received
                                                    ReceivingCountry = Common.Common.GetCountryName(c.AgentInformation.CountryCode),
                                                    SendingCountry = Common.Common.GetCountryName(c.AgentInformation.CountryCode),
                                                    StatusName = "Completed",

                                                    TransferMethod = "KiiPay Wallet",
                                                    AgentId = d.AgentId,
                                                }).ToList();
            var OtherMobilesWalletsTransfer = (from c in dbContext.MobileMoneyTransfer.Where(x => x.PaidFromModule == Module.Agent).ToList()
                                               join d in agentStaffInformation on c.PayingStaffId equals d.Id

                                               select new DailyTransactionStatementListVm()
                                               {
                                                   Id = c.Id,
                                                   TransactionType = TransactionType.OtherWalletTransfer,
                                                   TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.OtherWalletTransfer),
                                                   CurrencySymbol = Common.CountryUtility.GetCountryCurrencySymbol(c.SendingCountry),
                                                   Currency = Common.Common.GetCountryCurrency(c.SendingCountry),
                                                   Amount = c.SendingAmount,
                                                   Fee = c.Fee,
                                                   TransactionIdentifier = c.ReceiptNo,
                                                   DateAndTime = c.TransactionDate,
                                                   StaffName = c.PayingStaffName,
                                                   AgentCommission = c.AgentCommission,
                                                   //Type = Models.Type.Transfer
                                                   ReceivingCountry = Common.Common.GetCountryName(c.ReceivingCountry),
                                                   SendingCountry = Common.Common.GetCountryName(c.SendingCountry),
                                                   StatusName = c.Status.ToString(),
                                                   AgentId = d.AgentId,
                                                   TransferMethod = "Mobile Wallet",
                                               }).ToList();
            var BankAccountDepositTransfer = (from c in dbContext.BankAccountDeposit.Where(x => x.PaidFromModule == Module.Agent).ToList()
                                              join d in agentStaffInformation on c.PayingStaffId equals d.Id

                                              select new DailyTransactionStatementListVm()
                                              {
                                                  Id = c.Id,
                                                  TransactionType = TransactionType.BankAccountDeposit,
                                                  TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.BankAccountDeposit),
                                                  CurrencySymbol = Common.CountryUtility.GetCountryCurrencySymbol(c.SendingCountry),
                                                  Currency = Common.Common.GetCountryCurrency(c.SendingCountry),
                                                  Amount = c.SendingAmount,
                                                  Fee = c.Fee,
                                                  TransactionIdentifier = c.ReceiptNo,
                                                  DateAndTime = c.TransactionDate,
                                                  StaffName = c.PayingStaffName,
                                                  AgentCommission = c.AgentCommission,
                                                  //Type = Models.Type.Transfer,
                                                  ReceivingCountry = Common.Common.GetCountryName(c.ReceivingCountry),
                                                  SendingCountry = Common.Common.GetCountryName(c.SendingCountry),
                                                  StatusName = c.Status.ToString(),
                                                  TransferMethod = "Bank Deposit",
                                                  AgentId = d.AgentId,
                                              }).ToList();
            var AgentPayBillsTopUp = (from c in dbContext.TopUpToSupplier.Where(x => x.PayingStaffId != 0).ToList()
                                      join d in agentStaffInformation on c.PayingStaffId equals d.Id
                                      select new DailyTransactionStatementListVm()
                                      {
                                          Id = c.Id,
                                          TransactionType = TransactionType.PayBillsTopUp,
                                          TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.PayBillsTopUp),
                                          CurrencySymbol = Common.CountryUtility.GetCountryCurrencySymbol(Common.Common.GetCountryCodeByCountryName(c.PayingCountry)),
                                          Currency = Common.Common.GetCountryCurrency(c.PayingCountry),
                                          Amount = c.SendingAmount,
                                          Fee = c.Fee,
                                          TransactionIdentifier = c.ReceiptNo,
                                          DateAndTime = c.PaymentDate,
                                          StaffName = c.PayingStaffName,
                                          AgentCommission = c.AgentCommission,
                                          //Type = Models.Type.BillPayment,
                                          StatusName = "Completed",
                                          ReceivingCountry = Common.Common.GetCountryName(c.PayingCountry),
                                          SendingCountry = Common.Common.GetCountryName(c.SupplierCountry),
                                          AgentId = d.AgentId,
                                          TransferMethod = "Bill Payment",
                                      }).ToList();
            var AgentPayBillsMonthly = (from c in dbContext.PayBill.Where(x => x.PayingStaffId != 0).ToList()
                                        join d in agentStaffInformation on c.PayingStaffId equals d.Id
                                        select new DailyTransactionStatementListVm()
                                        {
                                            Id = c.Id,
                                            TransactionType = TransactionType.PayBillsMonthly,
                                            TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.PayBillsMonthly),
                                            CurrencySymbol = Common.CountryUtility.GetCountryCurrencySymbol(Common.Common.GetCountryCodeByCountryName(c.PayerCountry)),
                                            Currency = Common.Common.GetCountryCurrency(c.PayerCountry),
                                            Amount = c.SendingAmount,
                                            Fee = c.Fee,
                                            TransactionIdentifier = c.ReceiptNo,
                                            DateAndTime = c.PaymentDate,
                                            StaffName = c.PayingStaffName,
                                            AgentCommission = c.AgentCommission,
                                            //Type = Models.Type.BillPayment,
                                            ReceivingCountry = Common.Common.GetCountryName(c.SupplierCountry),
                                            SendingCountry = Common.Common.GetCountryName(c.PayerCountry),
                                            StatusName = "Completed",
                                            TransferMethod = "Service Payment",
                                            AgentId = d.AgentId,
                                        }).ToList();
            var CashPickupTransfer = (from c in dbContext.FaxingNonCardTransaction.Where(x => (x.PayingStaffId != 0 || x.PayingStaffId != null)).ToList()
                                      join d in agentStaffInformation on c.PayingStaffId equals d.Id
                                      select new DailyTransactionStatementListVm()
                                      {
                                          Id = c.Id,
                                          TransactionType = TransactionType.CashPickUp,
                                          TransactionTypeName = Common.Common.GetEnumDescription(TransactionType.CashPickUp),
                                          CurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                                          Currency = Common.Common.GetCountryCurrency(c.SendingCountry),
                                          Amount = c.FaxingAmount,
                                          Fee = c.FaxingFee,
                                          TransactionIdentifier = c.ReceiptNumber,
                                          DateAndTime = c.TransactionDate,
                                          StaffName = c.AgentStaffName,
                                          //Type = Models.Type.Transfer,
                                          AgentCommission = c.AgentCommission,
                                          ReceivingCountry = Common.Common.GetCountryName(c.ReceivingCountry),
                                          SendingCountry = Common.Common.GetCountryName(c.SendingCountry),
                                          AgentId = d.AgentId,
                                          StatusName = c.FaxingStatus.ToString(),
                                          TransferMethod = "Cash Pickup",
                                      }).ToList();
            var finalList = CashPickupReceiver.Concat(CashPickupTransfer).Concat(KiiPayWalletTransfer).Concat(KiiPayPersonalWalletReceiver).Concat(KiiPayBusinessWalletReceiver)
                .Concat(OtherMobilesWalletsTransfer).Concat(BankAccountDepositTransfer).Concat(AgentPayBillsTopUp).Concat(AgentPayBillsMonthly).ToList();
            return finalList.OrderByDescending(x => x.DateAndTime).ToList();
        }





    }

}