using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.Services
{
    public class MFTCCardPaymentByMerchantServices
    {
        DB.FAXEREntities dbContext = null;
        public MFTCCardPaymentByMerchantServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        
        public DB.KiiPayBusinessWalletInformation GetBusinessInformation()
        {

            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
            var result = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId && (x.CardStatus != DB.CardStatus.IsDeleted && x.CardStatus != DB.CardStatus.IsRefunded)).FirstOrDefault();


            return result;
        }


        public DB.KiiPayPersonalWalletPaymentByKiiPayBusiness SaveTransaction(DB.KiiPayPersonalWalletPaymentByKiiPayBusiness model)
        {

            dbContext.KiiPayPersonalWalletPaymentByKiiPayBusiness.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public DB.KiiPayPersonalWalletInformation GetCardInformationByCardNumber(string CardNumber)
        {


            string[] CardNo = CardNumber.Split('-');
            if (CardNo.Length > 1)
            {
                var MFTCCardNo = CardNumber.Encrypt();

                var result = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == MFTCCardNo).FirstOrDefault();


                return result;
            }
            else
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
}