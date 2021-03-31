using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewMerchantNonCardTransferServices
    {

        DB.FAXEREntities dbContext = null;
        public ViewMerchantNonCardTransferServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<ViewMerchantNonCardTransferViewModel> GetMerchantNonCardTransferDetails(string CountryCode, string City)
        {

            var data = dbContext.MerchantNonCardTransaction.ToList();

            if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {

                data = dbContext.MerchantNonCardTransaction.Where(x => x.NonCardReciever.Business.BusinessOperationCountryCode.ToLower() == CountryCode.ToLower()
                                                                   && x.NonCardReciever.Business.BusinessOperationCity.ToLower() == City.ToLower()).ToList();
            }
            else if (!string.IsNullOrEmpty(CountryCode))
            {


                data = dbContext.MerchantNonCardTransaction.Where(x => x.NonCardReciever.Country.ToLower() == CountryCode.ToLower()).ToList();
            }

            var result = (from c in data.OrderByDescending(x => x.TransactionDate)
                          select new ViewMerchantNonCardTransferViewModel()
                          {
                              MerchantName = c.NonCardReciever.Business.BusinessName,
                              MerchantAccountNo = c.NonCardReciever.Business.BusinessMobileNo,
                              MerchantCountry = Common.Common.GetCountryName(c.NonCardReciever.Business.BusinessOperationCountryCode),
                              MerchantCity = c.NonCardReciever.Business.BusinessOperationCity,
                              ReceiverName = c.NonCardReciever.FirstName + " " + c.NonCardReciever.MiddleName + " " + c.NonCardReciever.LastName,
                              ReceiverEmail = c.NonCardReciever.EmailAddress,
                              ReceiverCountry = Common.Common.GetCountryName(c.NonCardReciever.Country),
                              ReceiverCity = c.NonCardReciever.City,
                              ReceiverTelephone = Common.Common.GetCountryPhoneCode(c.NonCardReciever.Country) + c.NonCardReciever.PhoneNumber,
                              Fee = c.FaxingFee,
                              MFCN = c.MFCN,
                              TransactionDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                              TransactionId = c.Id,
                              TransactionTime = c.TransactionDate.ToString("HH:mm"),
                              TransactionStatus = Common.Common.GetEnumDescription((DB.FaxingStatus)c.FaxingStatus),
                              TransferedAmount = c.FaxingAmount,
                              TransactionIsUpdated = "No",
                              FaxingStatus = c.FaxingStatus

                          }).ToList();

            return result;


        }

        public DB.MerchantNonCardTransaction GetTransactionDetail(int TransactionId)
        {

            var data = dbContext.MerchantNonCardTransaction.Where(x => x.Id == TransactionId).FirstOrDefault();

            return data;
        }

        public bool HoldUnHoldTransaction(DB.MerchantNonCardTransaction model)
        {

            dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;
        }

    }
}