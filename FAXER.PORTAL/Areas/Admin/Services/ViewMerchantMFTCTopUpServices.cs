using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewMerchantMFTCTopUpServices
    {
        DB.FAXEREntities dbContext = null;
        public ViewMerchantMFTCTopUpServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<ViewMerchantMFTCTopUpViewModel> GetMerchantMFTCTopUpDetails(string CountryCode, string City) {


            var data = dbContext.KiiPayPersonalWalletPaymentByKiiPayBusiness.ToList();
            if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {

                data = dbContext.KiiPayPersonalWalletPaymentByKiiPayBusiness.Where(x => x.KiiPayBusinessWalletInformation.Country.ToLower() == CountryCode.ToLower()
                                                                            && x.KiiPayBusinessWalletInformation.City.ToLower() == City.ToLower()).ToList();


            }
            else if (!string.IsNullOrEmpty(CountryCode)) {

                data = dbContext.KiiPayPersonalWalletPaymentByKiiPayBusiness.Where(x => x.KiiPayBusinessWalletInformation.Country.ToLower() == CountryCode.ToLower()).ToList();
            }

            var result = (from c in data.OrderByDescending(x => x.TransactionDate)
                          join business in dbContext.KiiPayBusinessInformation on c.KiiPayBusinessInformationId equals business.Id
                          select new ViewMerchantMFTCTopUpViewModel()
                          {
                              MerchantName = business.BusinessName,
                              MerchantAccountNo = business.BusinessMobileNo,
                              MerchantCity = business.BusinessOperationCity,
                              MerchantCountry = Common.Common.GetCountryName(business.BusinessOperationCountryCode),
                              CardUserName = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + " " + c.KiiPayPersonalWalletInformation.LastName,
                              CardUserCity = c.KiiPayPersonalWalletInformation.CardUserCity,
                              CardUserCountry = Common.Common.GetCountryName(c.KiiPayPersonalWalletInformation.CardUserCountry),
                              CardUserEmail = c.KiiPayPersonalWalletInformation.CardUserEmail,
                              CardUserMFTCCardNumber = c.KiiPayPersonalWalletInformation.MobileNo.Decrypt(),
                              TopUpAmount = c.PayingAmount,
                              TopUpFee = c.Fee,
                              TopUpType = Enum.GetName(typeof(DB.PaymentType) , c.PaymentType),
                              TopUpTypeEnum = c.PaymentType,
                              TransactionId = c.Id,
                              TransactionDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                              TransactionTime= c.TransactionDate.ToString("HH:mm")

                          }).ToList();

            return result;
        }

        internal KiiPayBusinessInformation GetSenderDetails(int KiiPayBusinessInformationId)
        {

            var result = dbContext.KiiPayBusinessInformation.Where(x => x.Id == KiiPayBusinessInformationId).FirstOrDefault();
            return result;
        }

        internal KiiPayPersonalWalletPaymentByKiiPayBusiness GetTopUpByMerchantDetails(int transactionId)
        {

            var result = dbContext.KiiPayPersonalWalletPaymentByKiiPayBusiness.Where(x => x.Id == transactionId).FirstOrDefault();
            return result;
        }
    }
}