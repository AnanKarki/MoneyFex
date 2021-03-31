using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class STopUpSomeoneElseCard
    {

        DB.FAXEREntities dbContext = null;
        public STopUpSomeoneElseCard()
        {

            dbContext = new DB.FAXEREntities();
        }

        public DB.KiiPayPersonalWalletInformation GetCarduserInfo(string MFTCCardNo)
        {


            string MFTC = Common.Common.Encrypt(MFTCCardNo.Trim());
            var result = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == MFTC && x.IsDeleted == false).FirstOrDefault();

            return result;

        }

        public DB.KiiPayPersonalWalletInformation GetCardUserInfoByNumberOnly(string MFTCCardNo)
        {

            var result = dbContext.KiiPayPersonalWalletInformation.Where(x => x.IsDeleted == false).ToList();

            for (int i = 0; i < result.Count; i++)
            {

                string[] tokens = result[i].MobileNo.Decrypt().Split('-');
                if (tokens[1] == MFTCCardNo)
                {

                    var MFTCCard = result[i].MobileNo;
                    var model = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == MFTCCard && x.CardStatus == DB.CardStatus.Active).FirstOrDefault();
                    return model;
                }

            }
            return null;
        }


        public DB.TopUpSomeoneElseCardTransaction SaveTransaction(DB.TopUpSomeoneElseCardTransaction obj)
        {
            

                dbContext.TopUpSomeoneElseCardTransaction.Add(obj);
                int result = dbContext.SaveChanges();
            
            //if (result > 0)
            //{
            //    var data = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == obj.KiiPayPersonalWalletId).FirstOrDefault();

            //    data.CurrentBalance += obj.RecievingAmount;
            //    dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            //    dbContext.SaveChanges();
            //}
            return obj;
        }

        public List<Models.TopUpSomeoneElsePaymentDetialsViewModel> GetPaymentDetails(string FromDate, string ToDate)
        {
            int FaxerId = Common.FaxerSession.LoggedUser.Id;

            var data = new List<DB.TopUpSomeoneElseCardTransaction>();
            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                var fromDate = Convert.ToDateTime(FromDate);
                var toDate = Convert.ToDateTime(ToDate);
                data = dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.FaxerId == FaxerId && (DbFunctions.TruncateTime(x.TransactionDate) >= fromDate.Date
                          && DbFunctions.TruncateTime(x.TransactionDate) <= toDate.Date)).OrderByDescending(x => x.TransactionDate).ToList();


            }
            else
            {

                data = dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.FaxerId == FaxerId).OrderByDescending(x => x.TransactionDate).ToList();

            }



            var result = (from c in data
                          select new Models.TopUpSomeoneElsePaymentDetialsViewModel()
                          {
                              Id = c.Id,
                              MFTCCardId = c.KiiPayPersonalWalletId,
                              CardUserName = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + " " + c.KiiPayPersonalWalletInformation.LastName,
                              CardUserCity = c.KiiPayPersonalWalletInformation.CardUserCity,
                              CardUserCountry = Common.Common.GetCountryName(c.KiiPayPersonalWalletInformation.CardUserCountry),
                              Date = c.TransactionDate,
                              MFTCCardNumber = c.KiiPayPersonalWalletInformation.MobileNo.Decrypt(),
                              TopUpAmount = c.FaxingAmount.ToString(),
                              TopUpReference = c.TopUpReference,
                              Time = c.TransactionDate.ToString("HH:mm"),
                              PaymentMethod = "CardPayment"


                          }).ToList();


            //var result2 = (from c in dbContext.BanktoBankTransferTopUpSomeoneElseCard.Where(x => x.FaxerId == FaxerId).ToList().OrderByDescending(x => x.TransactionDate)
            //               join d in dbContext.MFTCCardInformation on c.MFTCCardId equals d.Id
            //               select new Models.TopUpSomeoneElsePaymentDetialsViewModel()
            //               {
            //                   Id = c.Id,
            //                   MFTCCardId = c.MFTCCardId,
            //                   CardUserName = d.FirstName + " " + d.MiddleName + " " + d.LastName,
            //                   CardUserCity = d.CardUserCity,
            //                   CardUserCountry = Common.Common.GetCountryName(d.CardUserCountry),
            //                   Date = c.TransactionDate,
            //                   MFTCCardNumber = d.MFTCCardNumber.Decrypt(),
            //                   TopUpAmount = c.TopUpAmount.ToString(),
            //                   TopUpReference = c.TopUpReference,
            //                   Time = c.TransactionDate.ToString("HH:mm"),
            //                   PaymentMethod = "BankToBankPayment",

            //               }).ToList();

            //List<Models.TopUpSomeoneElsePaymentDetialsViewModel> list = new List<Models.TopUpSomeoneElsePaymentDetialsViewModel>();

            //var model = list.Concat(result).Concat(result2).OrderByDescending(x => x.Date).ToList();

            return result;


        }


    }
}