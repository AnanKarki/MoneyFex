using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.Services
{
    public class CardUser_TransferInProgressServices
    {

        DB.FAXEREntities dbContext = null;
        int MFTCCardId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId;
        public CardUser_TransferInProgressServices()
        {
            dbContext = new DB.FAXEREntities();
        }
        public DB.CardUserNonCardTransaction CancelTransaction(int Id)
        {

            var data = dbContext.CardUserNonCardTransaction.Where(x => x.Id == Id).FirstOrDefault();
            data.FaxingStatus = DB.FaxingStatus.Cancel;
            data.StatusChangedDate = DateTime.Now;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return data;
        }



        public List<ViewModels.CardUser_TransferInProgressViewModel> GetTransferInProgressList(string searchText = "")
        {


            var data = new List<DB.CardUserNonCardTransaction>();
            if (!string.IsNullOrEmpty(searchText))
            {
                string text = Regex.Replace(searchText, @"\s+", "");
                data = (from c in dbContext.CardUserNonCardTransaction.Where(x => x.MFTCCardId == MFTCCardId && x.FaxingStatus == DB.FaxingStatus.NotReceived || x.FaxingStatus == DB.FaxingStatus.Hold
                           && (x.MFCN.ToLower() == searchText.ToLower() || (x.CardUserReceiverDetails.FirstName.ToLower() + x.CardUserReceiverDetails.MiddleName.ToLower()
                            + x.CardUserReceiverDetails.LastName.ToLower()) == text))
                        select c).ToList();
            }
            else
            {
                data = (from c in dbContext.CardUserNonCardTransaction.Where(x => x.MFTCCardId == MFTCCardId && x.FaxingStatus == DB.FaxingStatus.NotReceived || x.FaxingStatus == DB.FaxingStatus.Hold)
                        select c).ToList();
            }
            var result = (from c in data.ToList()
                          select new ViewModels.CardUser_TransferInProgressViewModel()
                          {
                              TransactionId = c.Id,
                              ReceiverId = c.NonCardRecieverId,
                              ReceiverName = c.CardUserReceiverDetails.FirstName + " " + c.CardUserReceiverDetails.MiddleName + " " + c.CardUserReceiverDetails.LastName,
                              ReceiverCity = c.CardUserReceiverDetails.City,
                              ReceiverCountry = Common.Common.GetCountryName(c.CardUserReceiverDetails.Country),
                              MFCN = c.MFCN,
                              StatusOfTransfer = Common.Common.GetEnumDescription(c.FaxingStatus),
                              Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                              Time = c.TransactionDate.ToString("HH:mm"),
                              TransactionDateTime = c.TransactionDate,
                              TransferAmount = Common.Common.GetCurrencySymbol(c.CardUserReceiverDetails.MFTCCardInformation.CardUserCountry) + " " + c.FaxingAmount + " " + Common.Common.GetCountryCurrency(c.CardUserReceiverDetails.MFTCCardInformation.CardUserCountry)
                          }).OrderByDescending(x => x.TransactionDateTime).ToList();
            return result;

        }

        internal DB.CardUserNonCardTransaction getTransactionDetails(int id)
        {
            var result = dbContext.CardUserNonCardTransaction.Where(x => x.Id == id).FirstOrDefault();
            return result;
        }

        public DB.KiiPayPersonalWalletInformation ReDepositCard(int CardId, decimal Amount)
        {
            var data = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == CardId).FirstOrDefault();
            data.CurrentBalance += Amount;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return data;
        }
    }
}