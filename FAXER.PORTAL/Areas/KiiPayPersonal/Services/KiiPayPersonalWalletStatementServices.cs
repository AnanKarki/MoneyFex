using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Services
{
    public class KiiPayPersonalWalletStatementServices
    {
        DB.FAXEREntities dbContext = null;
        public KiiPayPersonalWalletStatementServices()
        {
            dbContext = new DB.FAXEREntities();
        }


        public void AddkiiPayPersonalWalletStatement(KiiPayPersonalWalletStatementVM model)
        {

            #region Balance In Bound
            DB.KiiPayPersonalWalletStatement kiiPayPersonalWalletStatementOutBound = new DB.KiiPayPersonalWalletStatement()
            {
                Amount = model.SendingAmount,
                SenderCountry = model.SenderCountry,
                CurBal = model.SenderCurBal,
                Fee = model.Fee,
                ReceivingCountry = model.ReceiverCountry,
                TransactionDate = model.TransactionDate,
                TransactionId = model.TransactionId,
                WalletStatmentStatus = DB.WalletStatmentStatus.OutBound,
                WalletStatmentType = model.WalletStatmentType

            };
            #endregion

            #region Balance Out Bound
            DB.KiiPayPersonalWalletStatement kiiPayPersonalWalletStatementInBound = new DB.KiiPayPersonalWalletStatement()
            {
                Amount = model.ReceivingAmount,
                SenderCountry = model.SenderCountry,
                CurBal = model.SenderCurBal,
                Fee = model.Fee,
                ReceivingCountry = model.ReceiverCountry,
                TransactionDate = model.TransactionDate,
                TransactionId = model.TransactionId,
                WalletStatmentStatus = DB.WalletStatmentStatus.OutBound,
                WalletStatmentType = model.WalletStatmentType
            };

            #endregion
            dbContext.KiiPayPersonalWalletStatement.Add(kiiPayPersonalWalletStatementInBound);
            dbContext.KiiPayPersonalWalletStatement.Add(kiiPayPersonalWalletStatementOutBound);
            dbContext.SaveChanges();
        }
        public void AddkiiPayPersonalWalletStatementofCreditDebitCard(KiiPayPersonalWalletStatementVM model)
        {


            #region Balance In Bound
            DB.KiiPayPersonalWalletStatement kiiPayBusinessWalletStatementInBound = new DB.KiiPayPersonalWalletStatement()
            {
                Amount = model.SendingAmount,
                SenderCountry = model.SenderCountry,
                CurBal = model.SenderCurBal,
                Fee = model.Fee,
                ReceivingCountry = model.ReceiverCountry,
                TransactionDate = model.TransactionDate,
                TransactionId = model.TransactionId,
                WalletStatmentStatus = DB.WalletStatmentStatus.InBound,
                WalletStatmentType = model.WalletStatmentType

            };
            #endregion

            dbContext.KiiPayPersonalWalletStatement.Add(kiiPayBusinessWalletStatementInBound);
            dbContext.SaveChanges();
        }
    }
}