using FAXER.PORTAL.Areas.Agent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class BankAccountCreditUpdateServices
    {


        DB.FAXEREntities dbContext = null;
        public BankAccountCreditUpdateServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public Models.BankAccountCreditViewModel GetCreditDetails()
        {
            var AgentDetails = Common.AgentSession.AgentInformation;

            var data = (from c in dbContext.BaankAccountCreditUpdateByAgent.Where(x => x.AgentId == AgentDetails.Id)
                        select c).ToList();

            var CustomerDepositFee = data.Select(x => (decimal?)x.CustomerDepositFees).Sum() ?? 0;// data.Select(x => x.CustomerDepositFees).Sum(); 
            var CurrentBankDeposit = data.Select(x => (decimal?)x.BankDeposit).Sum() ?? 0;


            decimal CardUserNonCardWithdrawalAmount = dbContext.CardUserNonCardWithdrawal.Where(x => x.AgentId == AgentDetails.Id).Select(x => (decimal?)x.TransactionAmount).Sum() ?? 0;
            decimal MerchantNonCardwithdrawalAmount = dbContext.MerchantNonCardWithdrawal.Where(x => x.AgentId == AgentDetails.Id).Select(x => (decimal?)x.TransactionAmount).Sum() ?? 0;

            decimal NonCardwithdrawalAmount = dbContext.ReceiverNonCardWithdrawl.Where(x => x.AgentId == AgentDetails.Id).Select(x => (decimal?)x.TransactionAmount).Sum() ?? 0;
            decimal MFBCCardWithdrawalAmount = dbContext.MFBCCardWithdrawls.Where(x => x.AgentInformationId == AgentDetails.Id).Select(x => (decimal?)x.TransactionAmount).Sum() ?? 0;
            decimal MFTCCardWithdrawalAmount = dbContext.UserCardWithdrawl.Where(x => x.AgentInformationId == AgentDetails.Id).Select(x => (decimal?)x.TransactionAmount).Sum() ?? 0;
            var CurrentCustomerDeposit = data.Select(x => (decimal?)x.CustomerDeposit).Sum() ?? 0;
            var vm = new BankAccountCreditViewModel()
            {

                AgentAccountNumber = AgentDetails.AccountNo,
                AgentName = AgentDetails.Name,
                AgentCountry =Common.Common.GetCountryName(AgentDetails.CountryCode),
                City = AgentDetails.City,
                CurrenctBankDeposit = CurrentBankDeposit,
                CurrentCustomerDeposit = CurrentCustomerDeposit,
                NameOfLatestUpdater = data.Select(x => x.NameOfUpdater).LastOrDefault(),
                TotalAmountDeposited = ((CurrentBankDeposit + CurrentCustomerDeposit + CustomerDepositFee) - (NonCardwithdrawalAmount + 
                MFBCCardWithdrawalAmount + MFTCCardWithdrawalAmount + CardUserNonCardWithdrawalAmount + MerchantNonCardwithdrawalAmount)).ToString(),
                LatestTransactionDateTime = data.Select(x => x.CreatedDateTime).ToList().LastOrDefault(),
                CurrentCustomerDepositFees = CustomerDepositFee,
                Currency = Common.Common.GetCountryCurrency(AgentDetails.CountryCode),
                CurrencySymbol = Common.Common.GetCurrencySymbol(AgentDetails.CountryCode),
                NameOfUpdater = Common.AgentSession.LoggedUser.PayingAgentStaffName
            };

            return vm;
        }


        public DB.BaankAccountCreditUpdateByAgent SaveNewAccountDeposit(DB.BaankAccountCreditUpdateByAgent model) {

            dbContext.BaankAccountCreditUpdateByAgent.Add(model);
            dbContext.SaveChanges();
            return model;

        }


    
        internal string GetReceiptNoForBankAccountDeposit()
        {
            string AgentCode = Common.AgentSession.AgentInformation.AccountNo;
            //this code should be unique and random with 8 digit length
            var val = "PF-" + Common.Common.GenerateRandomDigit(5);
            while (dbContext.BaankAccountCreditUpdateByAgent.Where(x => x.ReceiptNo == val).Count() > 0)
            {
                val = GetReceiptNoForBankAccountDeposit();
            }
            return val;
        }

    }
}