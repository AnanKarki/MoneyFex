using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SSenderAddMoneyToWalletService
    {
        DB.FAXEREntities dbContext = null;
        
        public SSenderAddMoneyToWalletService()
        {
            dbContext = new DB.FAXEREntities();
        }
        public List<SenderSavedDebitCreditCard> GetSavedDebitCreditCardDetails()
        {
            var SenderId = Common.FaxerSession.LoggedUser == null ? 0 : Common.FaxerSession.LoggedUser.Id;
            var result = (from c in dbContext.SavedCard.Where(x => x.Module == DB.Module.Faxer && x.UserId == SenderId).ToList()
                          select new SenderSavedDebitCreditCard()
                          {
                              
                              CardId = c.Id,
                              CardHolderName = c.CardName.Encrypt(),
                              CardNumber = c.Num.Decrypt().FormatSavedCardNumber(),
                              ExpiringDateMonth = c.EMonth,
                              ExpiringDateYear = c.EYear,
                          }).ToList();
            return result;

        }

        public SenderAddMoneyToWalletViewModel GetSenderAddMoneyToWalletEnterAmount()
        {
            SenderAddMoneyToWalletViewModel vm = new SenderAddMoneyToWalletViewModel();
            vm.CurrencyCode = Common.Common.GetCountryCurrency(Common.FaxerSession.LoggedUser.CountryCode);

            // if user try to go back get the session value of business PersonalDetail  
            if (Common.FaxerSession.SenderAddMoneyToWallet != null)
            {
                vm = Common.FaxerSession.SenderAddMoneyToWallet;
            }

            return vm;

        }

        public void SetSenderAddMoneyToWalletEnterAmount(SenderAddMoneyToWalletViewModel vm)
        {
            Common.FaxerSession.SenderAddMoneyToWallet = vm;
        }
        public List<SenderAddMoneyToWalletViewModel> GetAddedAmount()
        {

            List<SenderAddMoneyToWalletViewModel> vm = new List<SenderAddMoneyToWalletViewModel>();
            return vm;

        }

        public bool AddMoneyToSenderWallet(StripeCreateTransactionVM vm)
        {
            int LoggedUserId = Common.FaxerSession.LoggedUser.Id;


            CommonAllServices _SenderCommonServices = new CommonAllServices();
            DB.AddMoneyToKiiPayPersonalWallet addMoneyToSender = new DB.AddMoneyToKiiPayPersonalWallet()
            {
                Amount = vm.Amount,
                StripeTokenId = vm.StripeTokenId,
                NameOnCard = vm.NameOnCard,
                TransactionDate = DateTime.Now,
                KiipayPersonalWalletId = _SenderCommonServices.GetKiipayPersonalWalletInfoByKiiPayPersonalId(LoggedUserId).Id,
                CardNum = vm.CardNum
            };
            var result = SaveAddMoneyToSenderWallet(addMoneyToSender);
            _SenderCommonServices.BalanceIn(result.KiipayPersonalWalletId, result.Amount);

            return true;
        }

        public DB.AddMoneyToKiiPayPersonalWallet SaveAddMoneyToSenderWallet(DB.AddMoneyToKiiPayPersonalWallet model)
        {

            dbContext.AddMoneyToKiiPayPersonalWallet.Add(model);
            dbContext.SaveChanges();
            return model;

        }

        public SenderAddMoneyToWalletSuccessViewModel GetSenderAddMoneyToWalletSuccess()
        {
            SenderAddMoneyToWalletSuccessViewModel vm = new SenderAddMoneyToWalletSuccessViewModel();
            vm.CurrencyCode = Common.Common.GetCurrencySymbol(Common.FaxerSession.LoggedUser.CountryCode);
            
            // if user try to go back get the session value of business PersonalDetail  
            if (Common.FaxerSession.SenderAddMoneySuccess != null)
            {
                vm = Common.FaxerSession.SenderAddMoneySuccess;
            }

            return vm;

        }

        public SenderAddMoneyCardViewModel GetSenderAddMoneyCard()
        {
            SenderAddMoneyCardViewModel vm = new SenderAddMoneyCardViewModel();
            vm.SendingCurrency = Common.Common.GetCurrencySymbol(Common.FaxerSession.LoggedUser.CountryCode);
            return vm;
        }
        public ServiceResult<SavedCard> Add(SavedCard model)
        {
            dbContext.SavedCard.Add(model);
            dbContext.SaveChanges();
            return new ServiceResult<SavedCard>()
            {
                Data = model,
                Message = "Save",
                Status = ResultStatus.OK
            };
        }
        public int SetCardId(int cardId)
        {
            Common.FaxerSession.CardId = cardId;
            return cardId;
        }
        public int GetCardId()
        {
            int cardId= Common.FaxerSession.CardId;
            return cardId;
        }
        public string GetCardHolderName(int cardId)
        {
            string cardName = dbContext.SavedCard.Where(x => x.Id == cardId).Select(x => x.CardName).FirstOrDefault();
            return cardName;
        }

    }
}