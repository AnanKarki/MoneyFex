using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.Services
{
    public class MyMoneyFaxCardLimitServices
    {
        DB.FAXEREntities dbContext = null;
        public MyMoneyFaxCardLimitServices()
        {

            dbContext = new DB.FAXEREntities();

        }
        public ViewModels.MyMoneyFaxCardLimitViewModel GetCardLimitDetails()
        {

            int MFTCCardId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId;
            var result = (from c in dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardId).ToList()
                          join Currency in dbContext.Country on c.CardUserCountry equals  Currency.CountryCode
                          select new ViewModels.MyMoneyFaxCardLimitViewModel()
                          {
                              AmountOnCard = c.CurrentBalance.ToString() + " " + Currency.Currency,
                              CardWithdDrawlLimitAmount = c.CashWithdrawalLimit.ToString() + " " + Currency.Currency,
                              CardWithDrawlLimitType = Enum.GetName(typeof(DB.CardLimitType), c.CashLimitType),
                              CardPurchaseLimitAmount = c.GoodsPurchaseLimit.ToString() + " " + Currency.Currency,
                              CardPurchaseLimitType = Enum.GetName(typeof(DB.AutoPaymentFrequency) , c.GoodsLimitType)
                          }).FirstOrDefault();

            return result;
        }
    }
}