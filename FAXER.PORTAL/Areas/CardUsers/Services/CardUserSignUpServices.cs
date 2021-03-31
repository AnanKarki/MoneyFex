using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.Services
{
    public class CardUserSignUpServices
    {
        DB.FAXEREntities dbContext = null;

        public CardUserSignUpServices()
        {
            dbContext = new DB.FAXEREntities();
        }
        public bool CardUserExist(string MFTCCardNumber) {

            string MFTC = MFTCCardNumber.Encrypt();
            var result = dbContext.KiiPayPersonalUserInformation.Where(x => x.MobileNo == MFTC).FirstOrDefault();
            if (result != null) {
                return true;
            }
            return false;
        }

        public bool CardUserEmailExist(string email) {

            var result = dbContext.KiiPayPersonalUserInformation.Where(x => x.EmailAddress == email).FirstOrDefault();

            if (result != null) {

                return true;
            }
            return false;
        }

        public DB.KiiPayPersonalWalletInformation GetMFTCCardInformation(string MFTCCardNumber)
        {


            string[] token = MFTCCardNumber.Split('-');
            if (token.Length < 2)
            {
                var data = dbContext.KiiPayPersonalWalletInformation.Where(x => x.IsDeleted == false).ToList();

                for (int i = 0; i < data.Count; i++)
                {

                    string[] tokens = data[i].MobileNo.Decrypt().Split('-');
                    if (tokens[1] == MFTCCardNumber)
                    {

                        var MFTCCard = data[i].MobileNo;
                        var model = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == MFTCCard && x.CardStatus == DB.CardStatus.Active).FirstOrDefault();
                        return model;
                    }

                }
                return null;
            }
            else {


                string MFTC = MFTCCardNumber.Encrypt();


                var result = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == MFTC).FirstOrDefault();



                return result;
            }

        }
        public DB.KiiPayPersonalUserInformation AddcardUserInformation(DB.KiiPayPersonalUserInformation cardUserInformation)
        {

            dbContext.KiiPayPersonalUserInformation.Add(cardUserInformation);
            dbContext.SaveChanges();
            return cardUserInformation;
        }

        public DB.KiiPayPersonalUserLogin AddCarduserLogin(DB.KiiPayPersonalUserLogin cardUserLogin)
        {

            dbContext.KiiPayPersonalUserLogin.Add(cardUserLogin);
            dbContext.SaveChanges();
            return cardUserLogin;
        }

        public bool CardAlreadyExist(string MFTCNumber) {

            string MFTC = MFTCNumber.Encrypt();
            var result = dbContext.KiiPayPersonalUserInformation.Where(x => x.MobileNo == MFTC).FirstOrDefault();
            if (result != null) {
                return true;
            }
            return false;

        }

        public DB.KiiPayPersonalWalletInformation GetCardUserDetials(string Email) {

            var result = dbContext.KiiPayPersonalWalletInformation.Where(x => x.CardUserEmail == Email).FirstOrDefault();
            return result;

        }
    }
}