using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Services
{
    public class KiiPayBusinessWithdrawMoneyFromWalletServices
    {


        DB.FAXEREntities dbContext = null;
        public KiiPayBusinessWithdrawMoneyFromWalletServices()
        {
            dbContext = new DB.FAXEREntities();
        }



        public List<KiiPayBusinessUserBanckAccountsVM> GetSavedBankAccount()
        {
            var result = (from c in dbContext.BankAccount.ToList()
                          select new ViewModels.KiiPayBusinessUserBanckAccountsVM()
                          {
                              AccountNumber =c.AccountNo,
                              BankId = c.Id,
                              Name =c.LabelName

                          }).ToList();
            return result;
           

        }

        public WithdrawMoneyFromWalletSummaryVM GetWithdrawMoneySummary()
        {
            WithdrawMoneyFromWalletSummaryVM vm = new WithdrawMoneyFromWalletSummaryVM();
            return vm;


        }public WithdrawMoneyFromWalletSuccessVM GetWithdrawMoneySuccess()
        {
            WithdrawMoneyFromWalletSuccessVM vm = new WithdrawMoneyFromWalletSuccessVM();
            return vm;


        }
    }
}