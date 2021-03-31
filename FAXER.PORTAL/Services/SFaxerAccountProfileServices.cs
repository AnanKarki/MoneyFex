using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SFaxerAccountProfileServices
    {
        DB.FAXEREntities dbContext = null;
        public SFaxerAccountProfileServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public SavedCard AddCard(SenderAddMoneyCardViewModel model)
        {


            //if (dbContext.SavedCard.Where(x => x.UserId == FaxerSession.LoggedUser.Id && x.IsDeleted == false).ToList().Count < 1)
            //{
            DB.SavedCard savedCard = new DB.SavedCard()
            {
                CardName = FaxerSession.LoggedUser.FullName.Encrypt(),
                EMonth = model.ExpiringDateMonth.Encrypt(),
                EYear = model.ExpiringDateYear.Encrypt(),
                CreatedDate = System.DateTime.Now,
                UserId = FaxerSession.LoggedUser.Id,
                Num = model.CardNumber.Encrypt(),
                ClientCode = model.SecurityCode.Encrypt(),
                Type = model.SelectCard

            };
            dbContext.SavedCard.Add(savedCard);
            dbContext.SaveChanges();
            return savedCard;
            //}
            //return null;
        }


        public List<SenderSavedCreditDebitCardViewModel> GetSavedCreditDebitCards()
        {


            var data = (from c in dbContext.SavedCard.Where(x => x.UserId == FaxerSession.LoggedUser.Id && x.IsDeleted == false).ToList()
                        select new Models.SenderSavedCreditDebitCardViewModel()
                        {
                            Id = c.Id,
                            CardNo = "XXXX-XXXX-XXXX-" + c.Num.Decrypt().Right(4),
                            CardNoWithoutFormat = c.Num.Decrypt(),
                            EndDate = c.EMonth.Decrypt() + "/" + c.EYear.Decrypt(),
                            EndMonth = c.EMonth.Decrypt(),
                            EndYear = c.EYear.Decrypt(),
                            SecurityCodeUnmask = c.ClientCode.Decrypt(),
                            SecurityCode = "...",
                            Status = GetCardStatus(c.EYear.Decrypt(), c.EMonth.Decrypt()),

                        }).ToList();
            return data;
        }
        public SenderSavedCreditDebitCardViewModel List(int cardId = 0)
        {


            var data = (from c in dbContext.SavedCard.Where(x => x.Id == cardId).ToList()
                        select new Models.SenderSavedCreditDebitCardViewModel()
                        {
                            Id = c.Id,
                            CardNo = "XXXX-XXXX-XXXX-" + c.Num.Decrypt().Right(4),
                            CardNoWithoutFormat = c.Num.Decrypt(),
                            EndDate = c.EMonth.Decrypt() + "/" + c.EYear.Decrypt(),
                            EndMonth = c.EMonth.Decrypt(),
                            EndYear = c.EYear.Decrypt(),
                            SecurityCodeUnmask = c.ClientCode.Decrypt(),
                            SecurityCode = "...",
                            Status = GetCardStatus(c.EYear.Decrypt(), c.EMonth.Decrypt()),
                            CardType = c.Type

                        }).FirstOrDefault();
            return data;
        }


        private string GetCardStatus(string eYear, string eMonth)
        {
            if (eYear.Length == 2) {
                eYear = "20" + eYear;
            }
            var expiryDate = new DateTime(Convert.ToInt32(eYear), Convert.ToInt32(eMonth), 1);
            if (DateTime.Now.Date > expiryDate)
            {
                return "Expired";
            }

            else
            {
                return "Active";
            }

        }

        public SavedCard UpdateCard(SenderAddMoneyCardViewModel vm, int cardId)
        {
            var data = dbContext.SavedCard.Where(x => x.Id == cardId).FirstOrDefault();
            data.Num = vm.CardNumber.Encrypt();
            data.EMonth = vm.ExpiringDateMonth.Encrypt();
            data.EYear = vm.ExpiringDateYear.Encrypt();
            data.ClientCode = vm.SecurityCode.Encrypt();

            dbContext.Entry(data).State = EntityState.Modified;
            dbContext.SaveChanges();
            return data;
        }

        public SavedCard RemoveCard(int CardId)
        {
            var card = dbContext.SavedCard.Find(CardId);
            dbContext.SavedCard.Remove(card);
            dbContext.SaveChanges();
            var MFTCAutoTopUp = (from c in dbContext.KiiPayPersonalWalletInformation
                                 join d in dbContext.SenderKiiPayPersonalAccount.Where(x => x.SenderId == Common.FaxerSession.LoggedUser.Id) on c.Id equals d.KiiPayPersonalWalletId
                                 select c).ToList();

            foreach (var AutoTopUp in MFTCAutoTopUp)
            {
                AutoTopUp.AutoTopUp = false;
                AutoTopUp.AutoTopUpAmount = 0;
                dbContext.Entry(AutoTopUp).State = EntityState.Modified;
                dbContext.SaveChanges();

            }

            var MFBCAutoPayment = dbContext.FaxerMerchantPaymentInformation.Where(x => x.SenderInformationId == Common.FaxerSession.LoggedUser.Id).ToList();

            foreach (var AutoPayment in MFBCAutoPayment)
            {

                AutoPayment.AutoPaymentFrequency = AutoPaymentFrequency.NoLimitSet;
                AutoPayment.AutoPaymentAmount = 0;
                AutoPayment.EnableAutoPayment = false;
                dbContext.Entry(AutoPayment).State = EntityState.Modified;
                dbContext.SaveChanges();
            }

            var otherMFTCPayment = dbContext.OtherMFTCCardAutoTopUpInformation.Where(x => x.FaxerId == Common.FaxerSession.LoggedUser.Id).ToList();

            foreach (var item in otherMFTCPayment)
            {

                item.AutoPaymentAmount = 0;
                item.AutoPaymentFrequency = AutoPaymentFrequency.NoLimitSet;
                item.EnableAutoPayment = false;
                dbContext.Entry(item).State = EntityState.Modified;
                dbContext.SaveChanges();
            }

            return card;
        }
    }
}