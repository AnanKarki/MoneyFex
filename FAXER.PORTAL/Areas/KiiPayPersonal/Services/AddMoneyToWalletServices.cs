using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Services
{
    public class AddMoneyToWalletServices
    {
        DB.FAXEREntities dbContext = null;
       
        public AddMoneyToWalletServices()
        {
            dbContext = new DB.FAXEREntities();
        }   

        public AddMoneyToWalletEnterAmountVM GetKiiPayPersonalAddMoneyToWalletEnterAmount()
        {
            AddMoneyToWalletEnterAmountVM vm = new AddMoneyToWalletEnterAmountVM();
            vm.CurrencyCode = Common.Common.GetCountryCurrency(Common.CardUserSession.LoggedCardUserViewModel.CountryCode);
            // if user try to go back get the session value of  PersonalDetail  
            if (Common.CardUserSession.KiiPayPersonalAddMoneyToWalletEnterAmount != null)
            {
                vm = Common.CardUserSession.KiiPayPersonalAddMoneyToWalletEnterAmount;
            }

            return vm;

        }

        public void SetKiiPayPersonalAddMoneyToWalletEnterAmount(AddMoneyToWalletEnterAmountVM vm)
        {

            Common.CardUserSession.KiiPayPersonalAddMoneyToWalletEnterAmount = vm;
        }

        public List<ViewModels.KiiPayPersonalSavedDebitCreditCardVM> GetSavedDebitCreditCardDetails()
        {

            var PersonalId = Common.CardUserSession.LoggedCardUserViewModel == null ? 0 : Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId;
            var result = (from c in dbContext.SavedCard.Where(x => x.Module == DB.Module.KiiPayPersonal && x.UserId == PersonalId).ToList()
                          select new ViewModels.KiiPayPersonalSavedDebitCreditCardVM()
                          {
                              CardId = c.Id,
                              Name = c.CardName.Decrypt(),
                              CardNumber = c.Num.Decrypt().FormatSavedCardNumber()

                          }).ToList();
            return result;

        }

        public bool AddMoneyToKiiPayBusinessWallet(StripeCreateTransactionVM vm , int KiiPayPersonalWalletId = 0)
        {

            KiiPayPersonalCommonServices _kiiPayPersonalCommonServices = new KiiPayPersonalCommonServices();
            DB.AddMoneyToKiiPayPersonalWallet addMoneyToKiiPayBusiness = new DB.AddMoneyToKiiPayPersonalWallet()
            {
                Amount = vm.Amount / 100,
                StripeTokenId = vm.StripeTokenId,
                NameOnCard = vm.NameOnCard,
                TransactionDate = DateTime.Now,
                KiipayPersonalWalletId = KiiPayPersonalWalletId,
                CardNum = vm.CardNum
            };
            var result = SaveAddMoneyToKiiPayPersonalWallet(addMoneyToKiiPayBusiness);
            _kiiPayPersonalCommonServices.BalanceIn(result.KiipayPersonalWalletId, result.Amount);



             #region Wallet Statement Add 

            KiiPayPersonalWalletStatementServices _kiiPayPersonalWalletStatementServices = new KiiPayPersonalWalletStatementServices();
            KiiPayPersonalWalletStatementVM KiiPayPersonalWalletStatement = new KiiPayPersonalWalletStatementVM()
            {
                SendingAmount = addMoneyToKiiPayBusiness.Amount,
                Fee = 0,
                ReceivingAmount = addMoneyToKiiPayBusiness.Amount,
                SenderCurBal = _kiiPayPersonalCommonServices.GetKiipayPersonalWalletInfo(result.KiipayPersonalWalletId).CurrentBalance,
                SenderCountry = _kiiPayPersonalCommonServices.GetKiipayPersonalWalletInfo(result.KiipayPersonalWalletId).CardUserCountry,
                TransactionDate = result.TransactionDate,
                WalletStatmentStatus = DB.WalletStatmentStatus.InBound,
                TransactionId = result.Id,  
                WalletStatmentType = DB.WalletStatmentType.CreditDebitCard,
                ReceiverCountry = vm.ReceivingCountry,
                
            };

            _kiiPayPersonalWalletStatementServices.AddkiiPayPersonalWalletStatementofCreditDebitCard(KiiPayPersonalWalletStatement);

            #endregion

            return true;
        }


        public DB.AddMoneyToKiiPayPersonalWallet SaveAddMoneyToKiiPayPersonalWallet(DB.AddMoneyToKiiPayPersonalWallet model)
        {

            dbContext.AddMoneyToKiiPayPersonalWallet.Add(model);
            dbContext.SaveChanges();
            return model;

        }
    }
}