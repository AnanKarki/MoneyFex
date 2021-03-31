using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewMFTCCardPurchaseUsageServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();

        public List<ViewMFTCCardPurchaseUsageViewModel> getList(string CountryCode = "", string City = "")
        {
            var data = new List<DB.KiiPayPersonalNationalKiiPayBusinessPayment>();

            if (!string.IsNullOrEmpty(CountryCode) && string.IsNullOrEmpty(City))
            {
                data = dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Where(x=>x.KiiPayPersonalWalletInformation.CardUserCountry == CountryCode).ToList();
            }
            else if (string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Where(x => x.KiiPayPersonalWalletInformation.CardUserCity.ToLower() == City.ToLower()).ToList();
            }
            else if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Where(x => (x.KiiPayPersonalWalletInformation.CardUserCity.ToLower() == City.ToLower()) && (x.KiiPayPersonalWalletInformation.CardUserCountry == CountryCode)).ToList();
            }

            var result = (from c in data.OrderByDescending(x => x.TransactionDate)
                          join d in dbContext.SenderKiiPayPersonalAccount on c.KiiPayPersonalWalletInformationId equals d.KiiPayPersonalWalletId into SenderKiiPay
                          from d in SenderKiiPay.DefaultIfEmpty()
                          select new ViewMFTCCardPurchaseUsageViewModel()
                          {
                              FaxerFullName = d == null ? ""  :  d.SenderInformation.FirstName + " " + d.SenderInformation.MiddleName + " " + d.SenderInformation.LastName,
                              CardUserFullName = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + " " + c.KiiPayPersonalWalletInformation.LastName,
                              PurchaseAmount = c.AmountSent,
                              PurchaseDate = c.TransactionDate.ToFormatedString(),
                              Currency = CommonService.getCurrencyCodeFromCountry(c.KiiPayPersonalWalletInformation.CardUserCountry),
                              //BusinessMerchantVerifier = c.
                              BusinessMerchantName = CommonService.getBusinessName(c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.Id),
                              BusinessMerchantBMFSCode = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo
                          }).ToList();

            return result;
        }
    }
}