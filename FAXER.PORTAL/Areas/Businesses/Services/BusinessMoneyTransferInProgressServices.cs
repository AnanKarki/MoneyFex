using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.Services
{
    public class BusinessMoneyTransferInProgressServices
    {
        DB.FAXEREntities dbContext = null;
        int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant == null ? 0 : Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
        public BusinessMoneyTransferInProgressServices()
        {
            dbContext = new DB.FAXEREntities();
        }


        public DB.MerchantNonCardTransaction GetTransactionDetials(int Id)
        {

            var result = dbContext.MerchantNonCardTransaction.Where(x => x.Id == Id).FirstOrDefault();
            return result;

        }

        public DB.MerchantNonCardTransaction CancelTransaction(int Id)
        {

            var data = dbContext.MerchantNonCardTransaction.Where(x => x.Id == Id).FirstOrDefault();
            data.FaxingStatus = DB.FaxingStatus.Cancel;
            data.StatusChangedDate = DateTime.Now;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return data;
        }

        public DB.KiiPayBusinessWalletInformation ReDepositMFBCCard(int CardId, decimal Amount)
        {

            var result = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == CardId).FirstOrDefault();
            result.CurrentBalance += Amount;
            dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return result;

        }

        public List<ViewModels.BusinessMoneyTransferInProgressViewModel> GetTransferInProgressList(string searchText = "")
        {


            var data = new List<DB.MerchantNonCardTransaction>();
            if (!string.IsNullOrEmpty(searchText))
            {
                string text = Regex.Replace(searchText, @"\s+", "");
                data = (from c in dbContext.MerchantNonCardTransaction.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId && x.FaxingStatus == DB.FaxingStatus.NotReceived || x.FaxingStatus == DB.FaxingStatus.Hold
                           && (x.MFCN.ToLower() == searchText.ToLower() || (x.NonCardReciever.FirstName.ToLower() + x.NonCardReciever.MiddleName.ToLower()
                            + x.NonCardReciever.LastName.ToLower()) == text))
                        select c).ToList();
            }
            else
            {
                data = (from c in dbContext.MerchantNonCardTransaction.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId && x.FaxingStatus == DB.FaxingStatus.NotReceived || x.FaxingStatus == DB.FaxingStatus.Hold)
                        select c).ToList();
            }
            var result = (from c in data.ToList()
                          select new ViewModels.BusinessMoneyTransferInProgressViewModel()
                          {
                              TransactionId = c.Id,
                              ReceiverId = c.NonCardRecieverId,
                              ReceiverName = c.NonCardReciever.FirstName + " " + c.NonCardReciever.MiddleName + " " + c.NonCardReciever.LastName,
                              ReceiverCity = c.NonCardReciever.City,
                              ReceiverCountry = Common.Common.GetCountryName(c.NonCardReciever.Country),
                              MFCN = c.MFCN,
                              StatusOfTransfer = Enum.GetName(typeof(DB.FaxingStatus), c.FaxingStatus),
                              Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                              Time = c.TransactionDate.ToString("HH:mm"),
                              TransactionDateTime = c.TransactionDate,
                              TransferAmount = Common.Common.GetCurrencySymbol(c.NonCardReciever.Business.BusinessOperationCountryCode) + " " + c.FaxingAmount + " " + Common.Common.GetCountryCurrency(c.NonCardReciever.Business.BusinessOperationCountryCode)
                          }).OrderByDescending(x => x.TransactionDateTime).ToList();
            return result;

        }


    }
}