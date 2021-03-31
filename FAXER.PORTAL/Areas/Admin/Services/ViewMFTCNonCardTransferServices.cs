using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewMFTCNonCardTransferServices
    {
        DB.FAXEREntities dbContext = null;
        public ViewMFTCNonCardTransferServices()
        {
            dbContext = new DB.FAXEREntities();
        }


        public List<ViewMFTCNonCardTransferViewModel> GetMFTCNonCardTransaction(string CountryCode , string City)
        {
            var data = dbContext.CardUserNonCardTransaction.ToList();
            if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City)) {


                data = dbContext.CardUserNonCardTransaction.Where(x => x.CardUserReceiverDetails.MFTCCardInformation.CardUserCountry.ToLower() == CountryCode
            && x.CardUserReceiverDetails.MFTCCardInformation.CardUserCity.ToLower() == City.ToLower()).ToList();

            }
            else if (!string.IsNullOrEmpty(CountryCode)) {
                data = dbContext.CardUserNonCardTransaction.Where(x => x.CardUserReceiverDetails.MFTCCardInformation.CardUserCountry.ToLower() == CountryCode).ToList();

            }
            else {
                data = dbContext.CardUserNonCardTransaction.ToList();
            }


            var result = (from c in data.OrderByDescending(x => x.TransactionDate)
                          select new ViewMFTCNonCardTransferViewModel()
                          {
                              CardUserName = c.CardUserReceiverDetails.MFTCCardInformation.FirstName + " " + c.CardUserReceiverDetails.MFTCCardInformation.MiddleName + " " + c.CardUserReceiverDetails.MFTCCardInformation.LastName,
                              CardUserCity = c.CardUserReceiverDetails.MFTCCardInformation.CardUserCity,
                              CardUserCountry = Common.Common.GetCountryName(c.CardUserReceiverDetails.MFTCCardInformation.CardUserCountry),
                              MFTCCardNumber = c.CardUserReceiverDetails.MFTCCardInformation.MobileNo.Decrypt(),
                              MFCN = c.MFCN,
                              Fee = c.FaxingFee,
                              TrasactionAmount = c.FaxingAmount,
                              TransactionDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                              TransactionTime = c.TransactionDate.ToString("HH:mm"),
                              ReceiverName = c.CardUserReceiverDetails.FirstName + " " + c.CardUserReceiverDetails.MiddleName + " " + c.CardUserReceiverDetails.LastName,
                              ReceiverCity = c.CardUserReceiverDetails.City,
                              ReceiverCountry = Common.Common.GetCountryName(c.CardUserReceiverDetails.Country),
                              ReceiverEmail = c.CardUserReceiverDetails.EmailAddress,
                              ReceiverTelephone = c.CardUserReceiverDetails.PhoneNumber,
                              TransactionId = c.Id,
                              Status = Common.Common.GetEnumDescription((DB.FaxingStatus)c.FaxingStatus),
                              FaxingStatus = c.FaxingStatus


                          }).ToList();

            return result;
        }

        internal bool HoldUnholdTransaction(CardUserNonCardTransaction model)
        {

            dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;
        }

        internal DB.CardUserNonCardTransaction GetTransactionDetails(int transactionId)
        {
            var data = dbContext.CardUserNonCardTransaction.Where(x => x.Id == transactionId).FirstOrDefault();
            return data;


        }

        public DB.CardUserNonCardTransaction GetCardUserNonCardTransaction(int TransactionID)
        {

            var result = dbContext.CardUserNonCardTransaction.Where(x => x.Id == TransactionID).FirstOrDefault();
            return result;

        }


    }
}