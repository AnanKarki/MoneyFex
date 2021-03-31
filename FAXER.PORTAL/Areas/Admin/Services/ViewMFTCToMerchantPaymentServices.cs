using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewMFTCToMerchantPaymentServices
    {

        DB.FAXEREntities dbContext = null;
        public ViewMFTCToMerchantPaymentServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<ViewMFTCToMerchantPaymentViewModel> GetMFTCToMerchantPaymentDetails(string CountryCode , string City) {


            var MerchantInternationalPayment = dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.ToList();

            var MerchantLocalPayment = dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.ToList();


            if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {

                MerchantInternationalPayment = dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.Where(x => x.KiiPayPersonalWalletInformation.CardUserCountry.ToLower() == CountryCode.ToLower() 
                                                    && x.KiiPayPersonalWalletInformation.CardUserCity.ToLower() == City.ToLower()).ToList();

                MerchantLocalPayment = dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Where(x => x.KiiPayPersonalWalletInformation.CardUserCountry.ToLower() == CountryCode.ToLower()
                                        && x.KiiPayPersonalWalletInformation.CardUserCity.ToLower() == City.ToLower()).ToList();

            }
            else if (!string.IsNullOrEmpty(CountryCode)) {
                MerchantInternationalPayment = dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.Where(x => x.KiiPayPersonalWalletInformation.CardUserCountry.ToLower() == CountryCode.ToLower()).ToList();

                MerchantLocalPayment = dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Where(x => x.KiiPayPersonalWalletInformation.CardUserCountry.ToLower() == CountryCode.ToLower()).ToList();


            }


            var InternatinalTransaction_result = (from c in MerchantInternationalPayment
                                                  select new ViewMFTCToMerchantPaymentViewModel()
                                                  {


                                                      CardUserName = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + " " + c.KiiPayPersonalWalletInformation.LastName,
                                                      CardUserMFTCCardNumber = c.KiiPayPersonalWalletInformation.MobileNo.Decrypt(),
                                                      CardUserCountry = Common.Common.GetCountryName(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                                      CardUserCity = c.KiiPayPersonalWalletInformation.CardUserCity,
                                                      MerchantName = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessName,
                                                      MerchantCity = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessOperationCity,
                                                      MerchantAccountNo = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo,
                                                      MerchantCountry = Common.Common.GetCountryName(c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessOperationCountryCode),
                                                      MerchantEmail = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.Email,
                                                      Fee = c.FaxingFee,
                                                      TransactionAmount = c.FaxingAmount,
                                                      PaymentType = "International",
                                                      TransationDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                                                      TransactionTime = c.TransactionDate.ToString("HH:mm"),
                                                      TransactionDateTime = c.TransactionDate,
                                                      TransactionID = c.Id


                                                  }).ToList();
            var NationalTransaction_result = (from c in MerchantLocalPayment
                                              select new ViewMFTCToMerchantPaymentViewModel()
                                              {
                                                  CardUserName = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + " " + c.KiiPayPersonalWalletInformation.LastName,
                                                  CardUserMFTCCardNumber = c.KiiPayPersonalWalletInformation.MobileNo.Decrypt(),
                                                  CardUserCountry = Common.Common.GetCountryName(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                                  CardUserCity = c.KiiPayPersonalWalletInformation.CardUserCity,
                                                  MerchantName = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessName,
                                                  MerchantAccountNo = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo,
                                                  MerchantCountry = Common.Common.GetCountryName(c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessOperationCountryCode),
                                                  MerchantCity = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessOperationCity,
                                                  MerchantEmail = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.Email,
                                                  Fee = 0,
                                                  TransactionAmount = c.AmountSent,
                                                  PaymentType = "National",
                                                  TransactionDateTime = c.TransactionDate,
                                                  TransationDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                                                  TransactionTime = c.TransactionDate.ToString("HH:mm"),
                                                  TransactionID = c.Id
                                              }).ToList();


            var result = new List<ViewMFTCToMerchantPaymentViewModel>();

            result = InternatinalTransaction_result.Concat(NationalTransaction_result).OrderByDescending(x => x.TransactionDateTime).ToList();

            return result;



        }

        public DB.KiiPayPersonalInternationalKiiPayBusinessPayment GetMerchantInternationalTransactionDetail(int TransactionId) {

            var result = dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.Where(x => x.Id == TransactionId).FirstOrDefault();
            return result;


        }



    }
}