using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.Services
{
    public class PayForGoodsServices
    {
        DB.FAXEREntities dbContext = null;
        public PayForGoodsServices()
        {

            dbContext = new DB.FAXEREntities();

        }
        public DB.KiiPayBusinessWalletInformation MFBCCardInformationByID()
        {
            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
            var result = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId && (x.CardStatus != DB.CardStatus.IsDeleted  && x.CardStatus != DB.CardStatus.IsRefunded)).FirstOrDefault();
            return result;

        }
        public decimal AmountOnCard()
        {
            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
            var data = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId && (x.CardStatus != DB.CardStatus.IsDeleted && x.CardStatus != DB.CardStatus.IsRefunded)).FirstOrDefault();

            return data.CurrentBalance;
        }

        public DB.KiiPayBusinessWalletInformation GetMFBCCardInformation(string MFBCCard)
        {
            var MFBC = MFBCCard.Encrypt();
            var result = dbContext.KiiPayBusinessWalletInformation.Where(x => x.MobileNo == MFBC ).FirstOrDefault();

            return result;

        }
        public DB.KiiPayBusinessWalletInformation GetMFBCCardByCardId(int CardID)
        {
            var result = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == CardID && x.CardStatus == DB.CardStatus.Active).FirstOrDefault();

            return result;

        }
        public DB.KiiPayBusinessWalletInformation GetCardInformationByCardNumber(string CardNumber)
        {

            var result = dbContext.KiiPayBusinessWalletInformation.ToList();

            for (int i = 0; i < result.Count; i++) {

                string[] tokens = result[i].MobileNo.Decrypt().Split('-');
                if (tokens[1] == CardNumber) {

                    var MFBCCard = result[i].MobileNo;
                    var model = dbContext.KiiPayBusinessWalletInformation.Where(x => x.MobileNo == MFBCCard && x.CardStatus == DB.CardStatus.Active).FirstOrDefault();
                    return model;
                }

            }
            return null;
        }
        public bool MFBCCardTransaction(DB.KiiPayBusinessLocalTransaction mFBCCardTransaction)
        {
            dbContext.KiiPayBusinessLocalTransaction.Add(mFBCCardTransaction);
            int result = dbContext.SaveChanges();
            if (result > 0)
            {

                var data = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == mFBCCardTransaction.PayedToKiiPayBusinessWalletInformationId).FirstOrDefault();
                data.CurrentBalance = data.CurrentBalance + mFBCCardTransaction.AmountSent;
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                int result1 = dbContext.SaveChanges();
                if (result1 > 0)
                {
                    int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
                    var data2 = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId && x.CardStatus == DB.CardStatus.Active).FirstOrDefault();
                    data2.CurrentBalance = data2.CurrentBalance - mFBCCardTransaction.AmountSent;
                    dbContext.Entry(data2).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
            }
            return false;
        }
        public string GetBusinessMerchantName(int KiiPayBusinessInformationId) {

            var result = dbContext.KiiPayBusinessInformation.Where(x => x.Id == KiiPayBusinessInformationId).Select(x => x.BusinessName).FirstOrDefault();
            return result;
        }

        public DB.KiiPayBusinessInformation GetBusinessInformation( int KiiPayBusinessInformationId) {

            var result = dbContext.KiiPayBusinessInformation.Where(x => x.Id == KiiPayBusinessInformationId).FirstOrDefault();
            return result;
        }
    }
}