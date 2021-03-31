using FAXER.PORTAL.Areas.KiiPayPersonal.Services;
using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SSenderWalletTransactionStatement
    {
        DB.FAXEREntities dbContext = null;
        public SSenderWalletTransactionStatement()
        {
            dbContext = new DB.FAXEREntities();
        }


        public List<WalletStatementList> GetWalletStatment(int WalletId , InOut? inout  , string Country=""  )

        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            var walletInfo = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == WalletId).FirstOrDefault();
         
            WalletStatementServices _kiiPayPersonalwalletStatementServices = new WalletStatementServices();
            var result = _kiiPayPersonalwalletStatementServices.WalletTransactionFormPersonal(WalletId).OrderByDescending(x => x.TransactionDate).ToList();
           
            var senderwalletInfo = dbContext.SenderKiiPayPersonalAccount.Where(x => x.KiiPayPersonalWalletId == WalletId).FirstOrDefault();
            if (senderwalletInfo != null )
            {

                var senderWalletTransaction = _kiiPayPersonalwalletStatementServices.WalletTransactionFormSender(Common.FaxerSession.LoggedUser.Id).OrderByDescending(x => x.TransactionDate).ToList();
                result = result.Concat(senderWalletTransaction).OrderByDescending(x => x.TransactionDate).ToList();

            }
            

            decimal balance = 0;
            string walletCurrencySymbol = Common.Common.GetCurrencySymbol(walletInfo.CardUserCountry);
            foreach (var item in result.OrderByDescending(x => x.TransactionDate ).ToList())
            {
                if (item.InOut == InOut.In)
                {
                    item.BalanceDecimal =   balance + item.NetDecimal;
                    item.Balance = walletCurrencySymbol +  item.BalanceDecimal.ToString();
                }
                else
                {
                    item.BalanceDecimal = balance - item.NetDecimal;
                    item.Balance = walletCurrencySymbol + item.BalanceDecimal.ToString();
                }
                balance = item.BalanceDecimal;

            }
            if (inout != null)
            {

                result = result.Where(x => x.InOut == inout).ToList();
            }


            //var result = (from c in data.ToList()
            //              select new SenderWalletTransactionStatementDetailVM()
            //              {

            //                  Id = c.Id,
            //                  Date = c.TransactionDate.Day.ToString() + "-" + Enum.GetName(typeof(Month), c.TransactionDate.Month).Substring(0, 4) + "-" + c.TransactionDate.Year.ToString(),
            //                  TransactionDate = c.TransactionDate,
            //                  Name =c.ReceiverName,
            //                  Type = Enum.GetName(typeof(KiiPayPersonalWalletPaymentType), c.WalletStatmentType),
            //                  AccountNumber = c.AccountNumber,
            //                  Reference = c.PaymentReference,
            //                  Gross =  c.Amount.ToString(),
            //                  Fee =  c.Fee.ToString(),
            //                  Net = (c.Amount + c.Fee).ToString(),
            //                  Balance = c.CurBal.ToString(),
            //                  PaymentType = KiiPayPersonalWalletPaymentType.PersonalToBusinessNational,
            //                  InOut = c.WalletStatmentStatus,
            //                  SendingCurrency = Common.Common.GetCurrencySymbol(c.SenderCountry)
            //              }).ToList();
            //return result;


            return result;
        }



    }
}