using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Services
{
    public class SavedCreditDebitCardServices
    {
        FAXEREntities dbContext = null;
        StripServices _stripeServices = null;
        public SavedCreditDebitCardServices()
        {
            _stripeServices = new StripServices();
            dbContext = new FAXEREntities();
        }


        public List<SavedCreditDebitCardViewModel> getSavedCardsList()
        {
            if(Common.CardUserSession.LoggedCardUserViewModel != null)
            {
                if(Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId != 0)
                {
                    var result = (from c in dbContext.SavedCard.Where(x => x.IsDeleted == false && x.Module == Module.KiiPayPersonal && x.UserId == Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId).ToList()
                                  select new SavedCreditDebitCardViewModel()
                                  {
                                      Id = c.Id,
                                      CardNumber = c.Num.Decrypt().FormatSavedCardNumber(),
                                      ExpiryDate = c.EMonth.Decrypt() + "/" + c.EYear.Decrypt(),
                                      Status = _stripeServices.isCardActiveExpiredStatus(c.EMonth, c.EYear)
                                  }).ToList();
                    return result;
                }

            }
            return null;
        }

        public bool addNewCard(AddNewCardViewModel model)
        {
            if(model != null)
            {
                DB.SavedCard data = new DB.SavedCard()
                {
                    Type = model.CardType,
                    Num = model.CardNumber.ToString().Encrypt(),
                    EYear = model.ExpYear.ToString().Encrypt(),
                    EMonth = model.ExpMonth.ToString().Encrypt(),
                    Remark = "",
                    ClientCode = model.SecurityCode.ToString().Encrypt(),
                    CardName = model.NameOnCard.Encrypt(),
                    IsDeleted = false,
                    UserId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId,
                    CreatedDate = DateTime.Now,
                    Module = Module.KiiPayPersonal
                };
                dbContext.SavedCard.Add(data);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool updateCard(AddNewCardViewModel model)
        {
            if(model != null)
            {
                var data = dbContext.SavedCard.Where(x => x.Id == model.Id).FirstOrDefault();
                if (data != null)
                {
                    data.Type = model.CardType;
                    data.Num = model.CardNumber.Encrypt();
                    data.EMonth = model.ExpMonth.ToString().Encrypt();
                    data.EYear = model.ExpYear.ToString().Encrypt();
                    data.ClientCode = model.SecurityCode.Encrypt();
                    data.CardName = model.NameOnCard.Encrypt();
                    dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public AddNewCardViewModel getCardInfo(int id)
        {
            if(id != 0)
            {
                var data = dbContext.SavedCard.Where(x => x.Id == id).FirstOrDefault();
                if(data != null)
                {
                    var result = new AddNewCardViewModel()
                    {
                        Id = data.Id,
                        CardNumber = data.Num.Decrypt(),
                        CardType = (CreditDebitCardType)0,
                        ExpMonth = data.EMonth.Decrypt().ToInt(),
                        ExpYear = data.EYear.Decrypt().ToInt(),
                        NameOnCard = data.CardName.Decrypt(),
                        SecurityCode = data.ClientCode.Decrypt()
                    };
                    return result;
                }
            }
            return null;
        }

        public bool deleteCard(int id)
        {
            if(id != 0)
            {
                var data = dbContext.SavedCard.Where(x => x.Id == id).FirstOrDefault();
                if(data != null)
                {
                    data.IsDeleted = true;
                    dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
            }
            return false;
        }

    }
}