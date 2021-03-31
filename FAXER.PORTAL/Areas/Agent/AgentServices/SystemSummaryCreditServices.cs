using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class SystemSummaryCreditServices
    {
        DB.FAXEREntities dbContext = null;
        DailyTransactionStatementServices _transactionSatement = null;
        public SystemSummaryCreditServices()
        {
            dbContext = new DB.FAXEREntities();
            _transactionSatement = new DailyTransactionStatementServices();
        }

        public SystemSummaryCreditViewModel GetSummaryDetails()
        {
            int AgentId = Common.AgentSession.AgentInformation.Id;
            int PayingAgentId = Common.AgentSession.LoggedUser.PayingAgentStaffId;
            var transactionhistory = _transactionSatement.GetTransactionHistories(TransactionType.All, PayingAgentId);
            var data = transactionhistory.TransactionHistoryList.ToList();
            SystemSummaryCreditViewModel vm = new SystemSummaryCreditViewModel();
            vm.TransactionHisotry = data;
            var currentMonth = DateTime.Now.Month;
            var CurrentMonthData = data.Where(x => x.TransactionMonth == currentMonth).ToList();
            decimal CustomerDeposit = CurrentMonthData.Where(x => x.CustomerType == SystemCustomerType.CustomerDeposit).Sum(x => x.AmountPaid);
            decimal CustomerPayment = CurrentMonthData.Where(x => x.CustomerType == SystemCustomerType.CustomerPayment).Sum(x => x.AmountPaid);

            CashWithdrawalServices _CashWithdrawalServices = new CashWithdrawalServices();
            var CashWithdrawaData = _CashWithdrawalServices.getCashWithdrawalList();
            List<AgentTransactionHistoryList> MOneyFexWithdrawalList = (from c in CashWithdrawaData.ToList()
                                                                        select new AgentTransactionHistoryList()
                                                                        {
                                                                            Id = c.Id,
                                                                            AmountPaid = c.Amount,
                                                                            TransactionMonth = c.DateAndTime.Month,
                                                                            AgentName = c.StaffName,
                                                                            TransactionDate = c.DateAndTime.ToShortDateString(),
                                                                            CustomerType = SystemCustomerType.MOneyFexWithdrawal,
                                                                            CustomerTypeName = Common.Common.GetEnumDescription(SystemCustomerType.MOneyFexWithdrawal),
                                                                            ReceiptNumber = c.ReceiptNo

                                                                        }).ToList();


            var prefundingData = dbContext.BaankAccountCreditUpdateByAgent.Where(x => x.AgentId == AgentId).ToList();

            var prefundingList = (from c in prefundingData
                                 select new AgentTransactionHistoryList()
                                  {
                                      Id = c.Id,
                                      AmountPaid = c.BankDeposit,
                                      ReceiptNumber = c.ReceiptNo,
                                      CustomerType = SystemCustomerType.Prefunding,
                                      CustomerTypeName = Common.Common.GetEnumDescription(SystemCustomerType.Prefunding),
                                      TransactionDate = c.CreatedDateTime.ToShortDateString(),
                                      TransactionTime = c.CreatedDateTime.ToShortTimeString(),
                                      TransactionMonth = c.CreatedDateTime.Month,
                                    

                                  }).ToList();

            vm.TransactionHisotry.AddRange(MOneyFexWithdrawalList);
            vm.TransactionHisotry.AddRange(prefundingList);

            decimal MoneyFexWithdrawal = _CashWithdrawalServices.getCashWithdrawalList().Where(x => x.TransactionMonth == currentMonth).Sum(x => x.Amount);
            decimal Fee = CurrentMonthData.Where(x => x.CustomerType == SystemCustomerType.CustomerDeposit).Sum(x => x.Fee);
            decimal Commission = CurrentMonthData.Sum(x => x.AgentCommission);
            decimal BankDeposit = _transactionSatement.GetBankAccountDeposit(PayingAgentId).Where(x => x.TransactionMonth == currentMonth).Sum(x => x.AmountPaid);

            decimal total = CustomerDeposit + CustomerPayment + MoneyFexWithdrawal + Fee + Commission + BankDeposit;
            MonthlyAccountSummary model = new MonthlyAccountSummary()
            {
                BankDeposit = BankDeposit,
                Commission = Commission,
                Fee = Fee,
                CustomerDeposit = CustomerDeposit,
                CustomerPayment = CustomerPayment,
                MoneyFexWithdrawal = MoneyFexWithdrawal,
                Total = total

            };
            vm.MonthlyAccountSummary = model;
            return vm;
        }

        internal void AddPreFund(SystemSummaryCreditViewModel vm)
        {
            BankAccountCreditUpdateServices bankAccountCreditUpdateServices = new BankAccountCreditUpdateServices();
            var AgentInfo = Common.AgentSession.AgentInformation;
            BaankAccountCreditUpdateByAgent model = new BaankAccountCreditUpdateByAgent()
            {
                AgentId = AgentInfo.Id,
                CreatedDateTime = DateTime.Now,
                ReceiptNo = bankAccountCreditUpdateServices.GetReceiptNoForBankAccountDeposit(),
                NameOfUpdater = AgentInfo.Name,
                BankDeposit = vm.PrefundingAmount
            };
            dbContext.BaankAccountCreditUpdateByAgent.Add(model);
            dbContext.SaveChanges();
        }

        public AgentTransactionHistoryList PrefundingDetails(int id, string ReeceiptNo)
        {
            var prefundingData = dbContext.BaankAccountCreditUpdateByAgent.Where(x => x.Id == id && x.ReceiptNo == ReeceiptNo).ToList();

            var prefundingList = (from c in prefundingData
                                  join d in dbContext.AgentInformation on c.AgentId equals d.Id
                                  join e in dbContext.AgentStaffInformation on c.AgentId equals e.AgentId
                                  select new AgentTransactionHistoryList()
                                  {
                                      Id = c.Id,
                                      AmountPaid = c.BankDeposit,
                                      ReceiptNumber = c.ReceiptNo,
                                      CustomerType = SystemCustomerType.Prefunding,
                                      CustomerTypeName = Common.Common.GetEnumDescription(SystemCustomerType.Prefunding),
                                      TransactionDate = c.CreatedDateTime.ToShortDateString(),
                                      TransactionTime = c.CreatedDateTime.ToShortTimeString(),
                                      TransactionMonth = c.CreatedDateTime.Month,
                                      AgentName = d.ContactPerson,
                                      AgentNumber = d.AccountNo,
                                      AgentAddress = d.Address1 + " " + d.Address2 + " " + d.City + " " + Common.Common.GetCountryName(d.CountryCode),
                                      AgentCurrency = Common.Common.GetCountryCurrency(d.CountryCode),
                                      PayingStaffAgentName = e.FirstName + " " + e.MiddleName + " " + e.LastName,
                                      PayingStaffAccount = e.AgentMFSCode,

                                  }).FirstOrDefault();
            return prefundingList;



        }
        public AgentTransactionHistoryList WithDrawalDetails(int id, string ReeceiptNo)
        {
            int PayingAgentId = Common.AgentSession.LoggedUser.PayingAgentStaffId;
            CashWithdrawalServices _CashWithdrawalServices = new CashWithdrawalServices();

            var CashWithdrawaData = _CashWithdrawalServices.getCashWithdrawalList().Where(x => x.Id == id && x.ReceiptNo == ReeceiptNo).ToList();
            var kiipaydetails = _transactionSatement.GetKiiPayWalletWithdrawal(PayingAgentId).Where(x => x.Id == id && x.ReceiptNumber == ReeceiptNo).FirstOrDefault();
            if (kiipaydetails != null)
            {
                return kiipaydetails;
            }
            var casPickUpDetails = _transactionSatement.GetCashPickUpWithdrawal(PayingAgentId).Where(x => x.Id == id && x.ReceiptNumber == ReeceiptNo).FirstOrDefault();
            if (casPickUpDetails != null)
            {
                return casPickUpDetails;
            }
            var result = (from c in CashWithdrawaData
                          join d in dbContext.AgentInformation on c.AgentId equals d.Id
                          join e in dbContext.AgentStaffInformation on c.AgentId equals e.AgentId
                          select new AgentTransactionHistoryList()
                          {
                              Id = c.Id,
                              AmountPaid = c.Amount,
                              TransactionMonth = c.DateAndTime.Month,
                              TransactionDate = c.DateAndTime.ToShortDateString(),
                              CustomerType = SystemCustomerType.MOneyFexWithdrawal,
                              CustomerTypeName = Common.Common.GetEnumDescription(SystemCustomerType.MOneyFexWithdrawal),
                              ReceiptNumber = c.ReceiptNo,
                              AgentName = d.ContactPerson,
                              AgentNumber = d.AccountNo,
                              AgentAddress = d.Address1 + " " + d.Address2 + " " + d.City + " " + Common.Common.GetCountryName(d.CountryCode),
                              AgentCurrency = Common.Common.GetCountryCurrency(d.CountryCode),
                              PayingStaffAgentName = e.FirstName + " " + e.MiddleName + " " + e.LastName,
                              PayingStaffAccount = e.AgentMFSCode,
                            
                          }).FirstOrDefault();
            return result;
        }
    }
}