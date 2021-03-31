using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SSaveCreditDebitCardServices
    {
        private DB.FAXEREntities dbContext = null;
        public SSaveCreditDebitCardServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public bool SaveCreditDebitCard(AddMoneyToWalletEnterCardInfoVm vm)
        {

            DB.SavedCard savedCard = new DB.SavedCard()
            {
                CardName = "",
                ClientCode = vm.SecurityCode.Encrypt(),
                CreatedDate = DateTime.Now,
                EMonth = vm.ExpiringDateMonth.Encrypt(),
                EYear = vm.ExpiringDateYear.Encrypt(),
                IsDeleted = false,
                Module = DB.Module.KiiPayBusiness,
                Num = vm.CardNumber.Encrypt(),
                Remark = "",
                Type = vm.CardType,
                UserId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId,
            };

            var saveCreditDebitCard_result = SaveCreditDebitCardInformation(savedCard);
            return true;
        }

        public bool SaveSenderCreditDebitCard(SenderAddMoneyCardViewModel vm)
        {

            DB.SavedCard savedCard = new DB.SavedCard()
            {
                CardName = "",
                ClientCode = vm.SecurityCode.Encrypt(),
                CreatedDate = DateTime.Now,
                EMonth = vm.ExpiringDateMonth.Encrypt(),
                EYear = vm.ExpiringDateYear.Encrypt(),
                IsDeleted = false,
                Module = DB.Module.Faxer,
                Num = vm.CardNumber.Encrypt(),
                Remark = "",
                UserId = Common.FaxerSession.LoggedUser.Id,
            };

            var saveCreditDebitCard_result = SaveCreditDebitCardInformation(savedCard);
            return true;
        }

        public bool UpdateCreditDebitCard(AddMoneyToWalletEnterCardInfoVm vm)
        {
            var data = dbContext.SavedCard.Where(x => x.Id == vm.CardId).FirstOrDefault();
            data.ClientCode = vm.SecurityCode.Encrypt();
            data.EMonth = vm.ExpiringDateMonth.Encrypt();
            data.EYear = vm.ExpiringDateYear.Encrypt();
            data.Num = vm.CardNumber.Encrypt();
            data.Type = vm.CardType;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;
        }
        public bool SaveCreditDebitCard(AddMoneyToWalletEnterCardInfoVM vm)
        {

            DB.SavedCard savedCard = new DB.SavedCard()
            {
                CardName = "",
                ClientCode = vm.SecurityCode.Encrypt(),
                CreatedDate = DateTime.Now,
                EMonth = vm.ExpiringDateMonth.Encrypt(),
                EYear = vm.ExpiringDateYear.Encrypt(),
                IsDeleted = false,
                Module = DB.Module.KiiPayPersonal,
                Num = vm.CardNumber.Encrypt(),
                Remark = "",
                Type = vm.CardType,
                UserId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId,
            };

            var saveCreditDebitCard_result = SaveCreditDebitCardInformation(savedCard);
            return true;
        }

        public DB.SavedCard SaveCreditDebitCardInformation(DB.SavedCard model)
        {
            dbContext.SavedCard.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public DB.SavedCard GetCardInfo(int cardId)
        {

            var data = dbContext.SavedCard.Where(x => x.Id == cardId).FirstOrDefault();
            return data;

        }

        public List<DB.SavedCard> GetCardInfo()
        {

            var data = dbContext.SavedCard.ToList();
            return data;

        }
    }

}