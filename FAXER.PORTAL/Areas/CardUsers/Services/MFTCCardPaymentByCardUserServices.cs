using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.Services
{
    public class MFTCCardPaymentByCardUserServices
    {

        DB.FAXEREntities dbContext = null;
        public MFTCCardPaymentByCardUserServices()
        {
            dbContext = new DB.FAXEREntities();
        }


        public DB.KiiPayPersonalWalletInformation GetMFTCCardInformationByCardNumber(string MFTCCardNO)
        {


            var CardNumber = MFTCCardNO.Trim().Encrypt();
            var result = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == CardNumber).FirstOrDefault();
            return result;
        }

        public KiiPayPersonalWalletPaymentByKiiPayPersonal SaveTransaction(KiiPayPersonalWalletPaymentByKiiPayPersonal transaction)
        {

            dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.Add(transaction);
            dbContext.SaveChanges();
            
            Common.Common.IncreaseCreditOnMFTCCard(transaction.ReceiverWalletId , transaction.FaxingAmount);
            Common.Common.DeductCreditOnCard(transaction.SenderWalletId, transaction.FaxingAmount);

            return transaction;

        }

        public DB.KiiPayPersonalWalletInformation GetCardInformationByCardNumber(string CardNumber)
        {

            var result = dbContext.KiiPayPersonalWalletInformation.ToList();

            for (int i = 0; i < result.Count; i++)
            {
                if (!result[i].MobileNo.Contains("MF"))
                {
                    string[] tokens = result[i].MobileNo.Decrypt().Split('-');
                    if (tokens[1] == CardNumber)
                    {

                        var MFTCCard = result[i].MobileNo;
                        var model = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == MFTCCard && x.CardStatus == DB.CardStatus.Active).FirstOrDefault();
                        return model;
                    }
                }

            }
            return null;
        }
    }
}