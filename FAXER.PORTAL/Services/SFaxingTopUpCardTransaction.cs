using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SFaxingTopUpCardTransaction
    {
        DB.FAXEREntities db = new DB.FAXEREntities();
        public DB.SenderKiiPayPersonalWalletPayment SaveTransaction(DB.SenderKiiPayPersonalWalletPayment obj)
        {
            db.FaxingCardTransaction.Add(obj);
            db.SaveChanges();
            DB.KiiPayPersonalWalletInformation cardInfo = new DB.KiiPayPersonalWalletInformation();
            cardInfo = db.KiiPayPersonalWalletInformation.Find(obj.KiiPayPersonalWalletInformationId);
            cardInfo.CurrentBalance += obj.RecievingAmount;
            db.Entry<DB.KiiPayPersonalWalletInformation>(cardInfo).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return obj;
        }

        public string GetCardUserCountry(int CardId)
        {

            var result = db.KiiPayPersonalWalletInformation.Where(x => x.Id == CardId).Select(x => x.CardUserCountry).FirstOrDefault();
            return result;

        }

        public DB.KiiPayPersonalWalletInformation GetMFTCCardInformation(int CardId) {

            var result = db.KiiPayPersonalWalletInformation.Where(x => x.Id == CardId).FirstOrDefault();
            return result;
        }

        public List<DB.KiiPayPersonalWalletInformation> GetMFTCCardList() {

            List<KiiPayPersonalWalletInformation> list = (from c in db.KiiPayPersonalWalletInformation.Where(x =>  x.CardUserCountry == FaxerSession.TransferringTopUpCardRegisteredCountry && x.Id != FaxerSession.TransferCardId && x.IsDeleted == false)
                                                         join d in db.SenderKiiPayPersonalAccount.Where(x => x.SenderId == FaxerSession.LoggedUser.Id)  on c.Id equals d.KiiPayPersonalWalletId
                                                         select c).ToList();
            //foreach (var item in list.Where(x => x.IsDeleted == false))
            //{
            //    var MFTCcard = item.MFTCCardNumber.Decrypt();
            //    item.MFTCCardNumber = Common.Common.FormatMFTCCard(MFTCcard);
            //}
            foreach (var item in list)
            {
                var MFTCcard = item.MobileNo.Decrypt();
                item.MobileNo = Common.Common.FormatMFTCCard(MFTCcard);
            }
            return list;

        }
        internal string GetNewMFTCCardTopUpReceipt()
        {


            //this code should be unique and random with 8 digit length
            var val = "Os-Ctu-MF" + Common.Common.GenerateRandomDigit(5);

            while (db.FaxingCardTransaction.Where(x => x.ReceiptNumber == val).Count() > 0)
            {
                val = "Os-Ctu-MF" + Common.Common.GenerateRandomDigit(5);
            }
            return val;
        }
    }
}