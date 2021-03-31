using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Services
{
    public class KiiPayBusinessSavedCreditDebitCardServices
    {
        DB.FAXEREntities dbContext = null;
        public KiiPayBusinessSavedCreditDebitCardServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<ViewModels.KiiPayBusinessSavedCreditDebitCardListVM> GetSavedDebitCreditCardDetails()
        {

            var BusinessId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo == null ? 0 : Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId;
            var result = (from c in dbContext.SavedCard.Where(x => x.Module == DB.Module.KiiPayBusiness && x.UserId == BusinessId && x.IsDeleted == false).ToList()
                          select new ViewModels.KiiPayBusinessSavedCreditDebitCardListVM()
                          {
                              CardId = c.Id,
                              CardNo = c.Num.Decrypt().FormatSavedCardNumber(),
                              CardStatus = getStatusByExpDate(c.EMonth, c.EYear) == false ? "Expired" : "Active",
                              ExpMonth = c.EMonth.Decrypt(),
                              ExpYear = c.EYear.Decrypt()
                          }).ToList();
            return result;

        }


        public bool DeleteCard(int id)
        {

            var Cardtodelete = dbContext.SavedCard.Where(X => X.Id == id).FirstOrDefault();
            Cardtodelete.IsDeleted = true;
            dbContext.Entry(Cardtodelete).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;


        }

        public ViewModels.AddMoneyToWalletEnterCardInfoVm GetCardDetail(int id)
        {
            var data = dbContext.SavedCard.Where(x => x.Id == id).ToList();
            var result = (from c in data.ToList()
                          select new ViewModels.AddMoneyToWalletEnterCardInfoVm()
                          {
                              CardId = c.Id,
                              CardNumber = c.Num.Decrypt(),
                              CardType = c.Type,
                              ExpiringDateMonth = c.EMonth.Decrypt(),
                              ExpiringDateYear = c.EYear.Decrypt(),
                              SecurityCode = c.ClientCode.Decrypt(),
                          }).FirstOrDefault();
            return result;

        }
        internal bool UpdateCard(int id)
        {
            throw new NotImplementedException();
        }

        private bool getStatusByExpDate(string eMonth, string eYear)
        {
            var currentDate = DateTime.Now;
            eYear = currentDate.Year.ToString().Substring(0, 2) + "" + eYear.Decrypt();
            if (int.Parse(eYear) > currentDate.Year)
            {
                return true;
            }
            else if (int.Parse(eYear) == currentDate.Year && int.Parse(eMonth.Decrypt()) > currentDate.Month)
            {

                return true;
            }
            else
            {
                return false;

            }

        }
    }
}