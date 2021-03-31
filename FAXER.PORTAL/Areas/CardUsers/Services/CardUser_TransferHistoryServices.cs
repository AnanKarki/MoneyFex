using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.Services
{
    public class CardUser_TransferHistoryServices
    {
        DB.FAXEREntities dbContext = null;

        int MFTCCardId = Common.CardUserSession.LoggedCardUserViewModel == null ? 0 : Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId;
        public CardUser_TransferHistoryServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<ViewModels.CardUser_TransferHistoryViewModel> getTransactionhistoryList(string searchText = "")
        {

            var data = new List<DB.CardUserNonCardTransaction>();
            if (!string.IsNullOrEmpty(searchText))
            {
                string text = Regex.Replace(searchText, @"\s+", "");
                data = (from c in dbContext.CardUserNonCardTransaction.Where(x => x.MFTCCardId == MFTCCardId && x.FaxingStatus != DB.FaxingStatus.NotReceived
                           && x.MFCN.ToLower() == searchText.ToLower() || (x.CardUserReceiverDetails.FirstName.ToLower() + " " + x.CardUserReceiverDetails.MiddleName.ToLower() +
                           " " + x.CardUserReceiverDetails.LastName.ToLower()) == text)
                        select c).ToList();
            }
            else
            {
                data = (from c in dbContext.CardUserNonCardTransaction.Where(x => x.MFTCCardId == MFTCCardId && x.FaxingStatus != DB.FaxingStatus.NotReceived)
                        select c).ToList();
            }
            var result = (from c in data.ToList()
                          select new ViewModels.CardUser_TransferHistoryViewModel()
                          {
                              TransactionId = c.Id,
                              ReceiverId = c.NonCardRecieverId,
                              ReceiverName = c.CardUserReceiverDetails.FirstName + " " + c.CardUserReceiverDetails.MiddleName + " " + c.CardUserReceiverDetails.LastName,
                              ReceiverCity = c.CardUserReceiverDetails.City,
                              ReceiverCountry = Common.Common.GetCountryName(c.CardUserReceiverDetails.Country),
                              MFCN = c.MFCN,
                              StatusOfTransfer = Common.Common.GetEnumDescription( (DB.FaxingStatus)c.FaxingStatus) ,
                              Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                              Time = c.TransactionDate.ToString("HH:mm"),
                              TransactionDateTime = c.TransactionDate,
                              TransferAmount = Common.Common.GetCurrencySymbol(c.CardUserReceiverDetails.MFTCCardInformation.CardUserCountry) + "" + c.FaxingAmount + Common.Common.GetCountryCurrency(c.CardUserReceiverDetails.MFTCCardInformation.CardUserCountry)
                          }).OrderByDescending(x => x.TransactionDateTime).ToList();
            return result;
        }


        public DB.CardUserReceiverDetails GetReceiversDetails(int Id)
        {

            var result = dbContext.CardUserReceiverDetails.Where(x => x.Id == Id).FirstOrDefault();
            return result;
        }
    }
}