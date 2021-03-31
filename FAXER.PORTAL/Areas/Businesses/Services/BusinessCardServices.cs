using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.Services
{
    public class BusinessCardServices
    {
        DB.FAXEREntities dbContext = null;
        int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant== null ? 0 : Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId ;
        public BusinessCardServices() {
            dbContext = new DB.FAXEREntities();

        }

        public DB.KiiPayBusinessWalletInformation GetMFBCCardInformation(int KiiPayBusinessInformationId)
        {

            var result = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId && (x.CardStatus != DB.CardStatus.IsDeleted && x.CardStatus != DB.CardStatus.IsRefunded )).FirstOrDefault();
            return result;
        }
        public bool EmailExist(string email) {

            var result = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Email == email).FirstOrDefault();
            if (result != null) {
                return true;
            }
            else
            {
                return false;
            }
        }
        public DB.KiiPayBusinessWalletInformation RegisterMFBCCard( DB.KiiPayBusinessWalletInformation mFBCCardInformation) {

            dbContext.KiiPayBusinessWalletInformation.Add(mFBCCardInformation);
            dbContext.SaveChanges();
            return mFBCCardInformation;

        }
        internal string GetNewMFBCCardNumber()
        {
            //this code should be unique and random with 8 digit length
            //var val = Common.Common.GenerateRandomDigit(8);

            //MFS-123-123-1234-Firstname
            string val = String.Format("{0:d10}", (DateTime.Now.Ticks / 10) % 1000000000);

            while (dbContext.KiiPayBusinessWalletInformation.Where(x => x.MobileNo == val).Count() > 0)
            {
                val = String.Format("{0:d10}", (DateTime.Now.Ticks / 10) % 1000000000);
            }
            return "MFBC-" + val;
        }

        public string getCardPhoto(int id)
        {
            var result = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == id && (x.CardStatus != DB.CardStatus.IsDeleted && x.CardStatus != DB.CardStatus.IsRefunded)).FirstOrDefault();
            if (result != null)
            {
                return result.KiiPayUserPhoto;
            }
            return "";
        }

        public decimal GetCreditOnCard() {

            var result = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId && (x.CardStatus != DB.CardStatus.IsDeleted && x.CardStatus != DB.CardStatus.IsRefunded)).FirstOrDefault();
            if (result != null)
            {
                return result.CurrentBalance;
            }
            return 0;
        }
    }
}