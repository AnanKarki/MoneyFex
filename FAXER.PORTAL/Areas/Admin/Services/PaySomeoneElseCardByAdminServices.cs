using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class PaySomeoneElseCardByAdminServices
    {

        DB.FAXEREntities dbContext = null;
        public PaySomeoneElseCardByAdminServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public DB.FaxerInformation GetSenderInformation(string AccountNo)
        {

            var result = dbContext.FaxerInformation.Where(x => x.AccountNo == AccountNo).FirstOrDefault();
            return result;
        }


        public DB.KiiPayPersonalWalletInformation GetMFTCCardInformation(string MFTCCardNo)
        {

            string[] token = MFTCCardNo.Split('-');


            if (token.Length > 2)
            {

                string MFTCCardEncrypted = MFTCCardNo.Encrypt();
                var result = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == MFTCCardEncrypted).FirstOrDefault();
                return result;


            }
            else
            {

                var data = dbContext.KiiPayPersonalWalletInformation.ToList();
                for (int i = 0; i < data.Count; i++)
                {
                    try
                    {

                        string[] tokens = data[i].MobileNo.Decrypt().Split('-');
                        if (tokens[1] == MFTCCardNo)
                        {

                            var MFTCCard = data[i].MobileNo;
                            var model = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == MFTCCard).FirstOrDefault();
                            return model;
                        }
                    }
                    catch (Exception ex)
                    {

                    }



                }



            }
            return null;
        }

        public DB.KiiPayPersonalWalletInformation GetMFTCCardInformationByID(int Id)
        {


            var result = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == Id).FirstOrDefault();
            return result;
        }


        public DB.TopUpSomeoneElseCardTransaction TopUpSomeoneElseCard(DB.TopUpSomeoneElseCardTransaction model)
        {

            dbContext.TopUpSomeoneElseCardTransaction.Add(model);
            dbContext.SaveChanges();
            return model;


        }
        internal string GetNewMFTCCardTopUpReceipt()
        {


            //this code should be unique and random with 8 digit length
            var val = "Os-Ctu-MF" + Common.Common.GenerateRandomDigit(5);

            while (dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.ReceiptNumber == val).Count() > 0)
            {
                val = "Os-Ctu-MF" + Common.Common.GenerateRandomDigit(5);
            }
            return val;
        }

        internal List<ViewModels.SavedDropDownVM> getsavedcardList(int id)
        {
            var data = dbContext.SavedCard.Where(x => x.UserId == id).ToList();

            var result = (from c in data
                          select new ViewModels.SavedDropDownVM()
                          {
                              CardName = c.CardName.Decrypt(),
                              CardNum = c.Num.Decrypt(),
                              CardNumMasked = Common.Common.GetCreditCardMasked(c.Num.Decrypt())
                          }).ToList();

            return result;
        }

        internal DB.SavedCard GetCreditCardDetails(string cardNum , int FaxerId)
        {


            string cardNumEncrypted = cardNum.Encrypt();



            var data = dbContext.SavedCard.Where(x => x.Num == cardNumEncrypted && x.UserId == FaxerId).FirstOrDefault();

            return data;
        }


        public DB.SavedCard GetSavedCardByFaxerId(int Id)
        {


            var result = dbContext.SavedCard.Where(x => x.UserId == Id).FirstOrDefault();
            return result;
        }

    }
}