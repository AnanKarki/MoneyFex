using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Services
{
    public class KiiPayBusinessAddMoneyToWalletServices
    {

        DB.FAXEREntities dbContext = null;
        int KiiPayBusinessId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId;
        public KiiPayBusinessAddMoneyToWalletServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<ViewModels.KiiPayBusinessSavedDebitCreditCardVM> GetSavedDebitCreditCardDetails() {

            var BusinessId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo == null ? 0  : Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId;
            var result = (from c in dbContext.SavedCard.Where(x => x.Module == DB.Module.KiiPayBusiness && x.UserId == BusinessId).ToList()
                          select new ViewModels.KiiPayBusinessSavedDebitCreditCardVM()
                          {
                              CardId = c.Id,
                              Name = c.CardName.Decrypt(),
                              CardNumber = c.Num.Decrypt().FormatSavedCardNumber()

                          }).ToList();
            return result;

        }

        public AddMoneyToWalletEnterAmountVm GetKiiPayBusinessAddMoneyToWalletEnterAmount()
        {
            AddMoneyToWalletEnterAmountVm vm = new AddMoneyToWalletEnterAmountVm();
            vm.CurrencyCode = Common.Common.GetCountryCurrency(Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode);
            // if user try to go back get the session value of business PersonalDetail  
            if (Common.BusinessSession.KiiPayBusinessAddMoneyToWalletEnterAmount != null)
            {
                vm = Common.BusinessSession.KiiPayBusinessAddMoneyToWalletEnterAmount;
            }

            return vm;

        }
        public void SetKiiPayBusinessAddMoneyToWalletEnterAmount(AddMoneyToWalletEnterAmountVm vm)
        {

            Common.BusinessSession.KiiPayBusinessAddMoneyToWalletEnterAmount = vm;
        }
        public List<AddMoneyToWalletEnterAmountVm> GetAddedAmount()
        {

            List<AddMoneyToWalletEnterAmountVm> vm = new List<AddMoneyToWalletEnterAmountVm>();
            return vm;

        }

        public bool AddMoneyToKiiPayBusinessWallet(StripeCreateTransactionVM vm) {

            KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();
            DB.AddMoneyToKiiPayBusinessWallet addMoneyToKiiPayBusiness = new DB.AddMoneyToKiiPayBusinessWallet()
            {
                Amount  = vm.Amount,
                StripeTokenId = vm.StripeTokenId,
                NameOnCard = vm.NameOnCard,
                TransactionDate = DateTime.Now,
                KiipayBusinessWalletId = _kiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId(KiiPayBusinessId).Id,
                CardNum = vm.CardNum
            };
            var result = SaveAddMoneyToKiiPayBusinessWallet(addMoneyToKiiPayBusiness);
            _kiiPayBusinessCommonServices.BalanceIn(result.KiipayBusinessWalletId , result.Amount);


            #region Wallet Statement Add 

            KiiPayBusinessWalletStatementServices _kiiPayBusinessWalletStatementServices = new KiiPayBusinessWalletStatementServices();
            KiiPayBusinessWalletStatementVM KiiPayBusinessWalletStatement = new KiiPayBusinessWalletStatementVM()
            {
                SendingAmount = addMoneyToKiiPayBusiness.Amount,
                Fee = 0,
                ReceivingAmount = addMoneyToKiiPayBusiness.Amount,
                SenderCurBal = _kiiPayBusinessCommonServices.GetAccountBalanceByWalletId((int)result.KiipayBusinessWalletId),
                SenderCountry = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode,
                TransactionDate = result.TransactionDate,
                WalletStatmentStatus = DB.WalletStatmentStatus.InBound,
                TransactionId = result.Id,
                WalletStatmentType = DB.WalletStatmentType.CreditDebitCard,
            };

            _kiiPayBusinessWalletStatementServices.AddkiiPayBusinessWalletStatementofCreditDebitCard(KiiPayBusinessWalletStatement);

            #endregion


            return true;
        } 


        public DB.AddMoneyToKiiPayBusinessWallet SaveAddMoneyToKiiPayBusinessWallet(DB.AddMoneyToKiiPayBusinessWallet model) {

            dbContext.AddMoneyToKiiPayBusinessWallet.Add(model);
            dbContext.SaveChanges();
            return model;

        }

    
    }
}