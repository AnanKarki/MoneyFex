using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewMTCToMFTCTopUpServices
    {
        DB.FAXEREntities dbContext = null;
        public ViewMTCToMFTCTopUpServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<ViewMFTCToMFTCTopUpViewModel> GetMFTCToMFTCTopUpDetails(string CountryCode, string City)
        {

            var data = dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.ToList();

            if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {

                data = (from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.ToList()
                        join d in dbContext.KiiPayPersonalWalletInformation.Where(x => x.CardUserCountry.ToLower() == CountryCode.ToLower()
                        && x.CardUserCity.ToLower() == City.ToLower()) on c.SenderWalletId equals d.Id
                        select c).ToList();
            }
            else if (!string.IsNullOrEmpty(CountryCode))
            {


                data = (from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.ToList()
                        join d in dbContext.KiiPayPersonalWalletInformation.Where(x => x.CardUserCountry.ToLower() == CountryCode.ToLower()) on c.SenderWalletId equals d.Id
                        select c).ToList();

            }

            var result = (from c in data
                          join sender in dbContext.KiiPayPersonalWalletInformation on c.SenderWalletId equals sender.Id
                          select new ViewMFTCToMFTCTopUpViewModel()
                          {
                              SenderFullName = sender.FirstName + " " + sender.MiddleName + " " + sender.LastName,
                              SenderMFTCCardNumber = sender.MobileNo.Decrypt(),
                              SenderCity = sender.CardUserCity,
                              SenderCountry = Common.Common.GetCountryName(sender.CardUserCountry),
                              ReceiverFullName = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + " " + c.KiiPayPersonalWalletInformation.LastName,
                              ReceiverCity = c.KiiPayPersonalWalletInformation.CardUserCity,
                              ReceiverCountry = Common.Common.GetCountryName(c.KiiPayPersonalWalletInformation.CardUserCountry),
                              ReceiverEmail = c.KiiPayPersonalWalletInformation.CardUserEmail,
                              ReceiverMFTCCardNumber = c.KiiPayPersonalWalletInformation.MobileNo.Decrypt(),
                              TransactionId = c.Id,
                              TopUpAmount = c.FaxingAmount,
                              TopUpFee = c.FaxingFee,
                              TopUpType = Enum.GetName(typeof(DB.PaymentType), c.PaymentType),
                              TransactionDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                              TransactionTime = c.TransactionDate.ToString("HH:mm"),
                              TopUpTypeEnum = c.PaymentType

                          }).ToList();

            return result;


        }

        public DB.KiiPayPersonalWalletPaymentByKiiPayPersonal GetTopUpByCardUserDetails(int TransactionId)
        {

            var result = dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.Where(x => x.Id == TransactionId).FirstOrDefault();
            return result;

        }

        public DB.KiiPayPersonalWalletInformation GetSenderDetails(int Id) {

            var result = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == Id).FirstOrDefault();
            return result;
        }

        
    }
}