using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Services
{
    public class WalletStatementServices
    {
        FAXEREntities dbContext = null;
        KiiPayPersonalCommonServices _commonServices = null;

        int PersonalId = Common.CardUserSession.LoggedCardUserViewModel == null ? 0 : Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalWalletId;
        public WalletStatementServices()
        {
            _commonServices = new KiiPayPersonalCommonServices();
            dbContext = new FAXEREntities();
        }



        public WalletStatementViewModel getWalletStatement(int WalletId = 0)
        {
            var vm = new WalletStatementViewModel()
            {
                Filter = 0
            };

            string SendingCurrency = "";
            if (WalletId == 0)
            {

                WalletId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalWalletId;
                SendingCurrency = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol;
            }
            else {
                var walletInfo = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == WalletId).FirstOrDefault();
                SendingCurrency = Common.Common.GetCurrencySymbol(walletInfo.CardUserCountry);
            }

            //Calculate Outgoing Part
            //vm.WalletStatementList = (from c in dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Where(x => x.KiiPayPersonalWalletInformationId == Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalWalletId).ToList()
            //                          select new WalletStatementList()
            //                          {
            //                              Id = c.Id,
            //                              Date = c.TransactionDate.Day.ToString() + "-" + Enum.GetName(typeof(Month), c.TransactionDate.Month).Substring(0, 4) + "-" + c.TransactionDate.Year.ToString(),
            //                              TransactionDate = c.TransactionDate,
            //                              Name = c.KiiPayBusinessWalletInformation.FirstName + " " + c.KiiPayBusinessWalletInformation.MiddleName + " " + c.KiiPayBusinessWalletInformation.LastName,
            //                              Type = Enum.GetName(typeof(KiiPayPersonalWalletPaymentType), KiiPayPersonalWalletPaymentType.PersonalToBusinessNational),
            //                              AccountNumber = c.KiiPayBusinessWalletInformation.MobileNo,
            //                              Reference = c.PaymentReference,
            //                              Gross = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol + c.AmountSent.ToString(),
            //                              Fee = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol + "0",
            //                              Net = "-" + Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol + c.AmountSent.ToString(),
            //                              NetDecimal = c.AmountSent,
            //                              PaymentType = KiiPayPersonalWalletPaymentType.PersonalToBusinessNational,
            //                              InOut = InOut.Out
            //                          })
            //                      .Concat(from c in dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.Where(x => x.PayedFromKiiPayPersonalWalletId == Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalWalletId).ToList()
            //                              select new WalletStatementList()
            //                              {
            //                                  Id = c.Id,
            //                                  Date = c.TransactionDate.Day.ToString() + "-" + Enum.GetName(typeof(Month), c.TransactionDate.Month).Substring(0, 4) + "-" + c.TransactionDate.Year.ToString(),
            //                                  TransactionDate = c.TransactionDate,
            //                                  Name = c.KiiPayBusinessWalletInformation.FirstName + " " + c.KiiPayBusinessWalletInformation.MiddleName + " " + c.KiiPayBusinessWalletInformation.LastName,
            //                                  Type = Enum.GetName(typeof(KiiPayPersonalWalletPaymentType), KiiPayPersonalWalletPaymentType.PersonalToBusinessInternational),
            //                                  AccountNumber = c.KiiPayBusinessWalletInformation.MobileNo,
            //                                  Reference = c.PaymentReference,
            //                                  Gross = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol + c.FaxingAmount.ToString(),
            //                                  Fee = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol + c.FaxingFee.ToString(),
            //                                  Net = "-" + Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol + c.TotalAmount.ToString(),
            //                                  NetDecimal = c.TotalAmount,
            //                                  PaymentType = KiiPayPersonalWalletPaymentType.PersonalToBusinessInternational,
            //                                  InOut = InOut.Out
            //                              })
            //                              .Concat(from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.Where(x => x.SenderWalletId == Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalWalletId).ToList()
            //                                      select new WalletStatementList()
            //                                      {
            //                                          Id = c.Id,
            //                                          Date = c.TransactionDate.Day.ToString() + "-" + Enum.GetName(typeof(Month), c.TransactionDate.Month).Substring(0, 4) + "-" + c.TransactionDate.Year.ToString(),
            //                                          TransactionDate = c.TransactionDate,
            //                                          Name = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + " " + c.KiiPayPersonalWalletInformation.LastName,
            //                                          Type = Enum.GetName(typeof(KiiPayPersonalWalletPaymentType), KiiPayPersonalWalletPaymentType.PersonalToPersonal),
            //                                          AccountNumber = c.KiiPayPersonalWalletInformation.MobileNo,
            //                                          Reference = c.PaymentReference,
            //                                          Gross = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol + c.FaxingAmount.ToString(),
            //                                          Fee = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol + c.FaxingFee.ToString(),
            //                                          NetDecimal = c.TotalAmount,
            //                                          Net = "-" + Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol + c.TotalAmount.ToString(),
            //                                          PaymentType = KiiPayPersonalWalletPaymentType.PersonalToPersonal,
            //                                          InOut = InOut.Out
            //                                      })    //Calculate InComing Part
            //                                      .Concat(from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.Where(x => x.ReceiverWalletId == Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalWalletId
            //                                                                  && x.TransactionFromPortal == TransactionFrom.KiiPayPortal).ToList()
            //                                              select new WalletStatementList()
            //                                              {
            //                                                  Id = c.Id,
            //                                                  Date = c.TransactionDate.Day.ToString() + "-" + Enum.GetName(typeof(Month), c.TransactionDate.Month).Substring(0, 4) + "-" + c.TransactionDate.Year.ToString(),
            //                                                  TransactionDate = c.TransactionDate,
            //                                                  Name = _commonServices.getKiiPayPersonalUserNameFromWalletId(c.SenderWalletId),
            //                                                  Type = Enum.GetName(typeof(KiiPayPersonalWalletPaymentType), KiiPayPersonalWalletPaymentType.PersonalToPersonal),
            //                                                  AccountNumber = _commonServices.getKiiPayPersonalWalletNumberFromWalletId(c.SenderWalletId),
            //                                                  Reference = c.PaymentReference,
            //                                                  Gross = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol + c.RecievingAmount.ToString(),
            //                                                  Fee = "+" + _commonServices.KiiPayPersonalWalletUserCurrencySymbolFromWalletId(c.SenderWalletId) + c.FaxingFee.ToString(),
            //                                                  Net = _commonServices.KiiPayPersonalWalletUserCurrencySymbolFromWalletId(c.SenderWalletId) + c.TotalAmount.ToString(),
            //                                                  NetDecimal = c.TotalAmount,
            //                                                  PaymentType = KiiPayPersonalWalletPaymentType.PersonalToPersonal,
            //                                                  InOut = InOut.In,
            //                                                  IsRefunded = c.IsRefunded
            //                                              })
            //                                              .Concat(from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayBusiness.Where(x => x.KiiPayPersonalWalletInformationId == Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalWalletId).ToList()
            //                                                      select new WalletStatementList()
            //                                                      {
            //                                                          Id = c.Id,
            //                                                          Date = c.TransactionDate.Day.ToString() + "-" + Enum.GetName(typeof(Month), c.TransactionDate.Month).Substring(0, 4) + "-" + c.TransactionDate.Year.ToString(),
            //                                                          TransactionDate = c.TransactionDate,
            //                                                          Name = c.KiiPayBusinessWalletInformation.FirstName + " " + c.KiiPayBusinessWalletInformation.MiddleName + " " + c.KiiPayBusinessWalletInformation.LastName,
            //                                                          Type = Enum.GetName(typeof(KiiPayPersonalWalletPaymentType), KiiPayPersonalWalletPaymentType.BusinessToPersonal),
            //                                                          AccountNumber = c.KiiPayBusinessWalletInformation.MobileNo,
            //                                                          Reference = c.PaymentReference,
            //                                                          Gross = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol + c.RecievingAmount.ToString(),
            //                                                          Fee = _commonServices.KiiPayBusinessWalletUserCurrencySymbolFromWalletId(c.KiiPayBusinessWalletInformationId) + c.Fee.ToString(),
            //                                                          Net = "+" + _commonServices.KiiPayBusinessWalletUserCurrencySymbolFromWalletId(c.KiiPayBusinessWalletInformationId) + c.TotalAmount.ToString(),
            //                                                          NetDecimal = c.TotalAmount,
            //                                                          PaymentType = KiiPayPersonalWalletPaymentType.BusinessToPersonal,
            //                                                          InOut = InOut.In,
            //                                                          IsRefunded = c.IsRefunded
            //                                                      })
            //                                                      .Concat(from c in dbContext.AddMoneyToKiiPayPersonalWallet.Where(x => x.KiipayPersonalWalletId == Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalWalletId).ToList()
            //                                                              select new WalletStatementList()
            //                                                              {
            //                                                                  Id = c.Id,
            //                                                                  Date = c.TransactionDate.Day.ToString() + "-" + Enum.GetName(typeof(Month), c.TransactionDate.Month).Substring(0, 4) + "-" + c.TransactionDate.Year.ToString(),
            //                                                                  TransactionDate = c.TransactionDate,
            //                                                                  Name = c.NameOnCard,
            //                                                                  Type = Enum.GetName(typeof(KiiPayPersonalWalletPaymentType), KiiPayPersonalWalletPaymentType.DebitCreditCard),
            //                                                                  AccountNumber = Common.CardUserSession.LoggedCardUserViewModel.MobileNumber,
            //                                                                  Reference = Common.Common.FormatSavedCardNumber(c.CardNum),
            //                                                                  Gross = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol + c.Amount.ToString(),
            //                                                                  Fee = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol + "0",
            //                                                                  Net = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol + c.Amount.ToString(),
            //                                                                  NetDecimal = c.Amount,
            //                                                                  PaymentType = KiiPayPersonalWalletPaymentType.DebitCreditCard,
            //                                                                  InOut = InOut.In,
            //                                                                  IsRefunded = c.IsRefunded
            //                                                              }).OrderBy(x => x.TransactionDate).ToList();


            WalletTransactionFormPersonal(WalletId);
            decimal balance = 0;
            foreach (var item in vm.WalletStatementList)
            {
                if (item.InOut == InOut.In)
                {
                    item.BalanceDecimal = balance + item.NetDecimal;
                    item.Balance = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol + item.BalanceDecimal.ToString();
                }
                else
                {
                    item.BalanceDecimal = balance - item.NetDecimal;
                    item.Balance = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol + item.BalanceDecimal.ToString();
                }
                balance = item.BalanceDecimal;

            }


            return vm;
        }

        public List<WalletStatementList> WalletTransactionFormPersonal(int WalletId ) {

            string SendingCurrency = "";
            if (WalletId == 0)
            {

                WalletId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalWalletId;
                SendingCurrency = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol;
            }
            else
            {
                var walletInfo = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == WalletId).FirstOrDefault();
                SendingCurrency = Common.Common.GetCurrencySymbol(walletInfo.CardUserCountry);
            }
            var WalletStatementList = (from c in dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Where(x => x.KiiPayPersonalWalletInformationId == WalletId).ToList()
                                      select new WalletStatementList()
                                      {
                                          Id = c.Id,
                                          Date = c.TransactionDate.Day.ToString() + "-" + Enum.GetName(typeof(Month), c.TransactionDate.Month).Substring(0, 4) + "-" + c.TransactionDate.Year.ToString(),
                                          TransactionDate = c.TransactionDate,
                                          Name = c.KiiPayBusinessWalletInformation.FirstName + " " + c.KiiPayBusinessWalletInformation.MiddleName + " " + c.KiiPayBusinessWalletInformation.LastName,
                                          Type = Common.Common.GetEnumDescription(KiiPayPersonalWalletPaymentType.PersonalToBusinessNational),
                                          AccountNumber = c.KiiPayBusinessWalletInformation.MobileNo,
                                          Reference = c.PaymentReference,
                                          Gross = SendingCurrency + c.AmountSent.ToString(),
                                          Fee = SendingCurrency + "0",
                                          Net = "-" + SendingCurrency + c.AmountSent.ToString(),
                                          NetDecimal = c.AmountSent,
                                          PaymentType = KiiPayPersonalWalletPaymentType.PersonalToBusinessNational,
                                          InOut = InOut.Out
                                      })
                             .Concat(from c in dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.Where(x => x.PayedFromKiiPayPersonalWalletId == WalletId).ToList()
                                     select new WalletStatementList()
                                     {
                                         Id = c.Id,
                                         Date = c.TransactionDate.Day.ToString() + "-" + Enum.GetName(typeof(Month), c.TransactionDate.Month).Substring(0, 4) + "-" + c.TransactionDate.Year.ToString(),
                                         TransactionDate = c.TransactionDate,
                                         Name = c.KiiPayBusinessWalletInformation.FirstName + " " + c.KiiPayBusinessWalletInformation.MiddleName + " " + c.KiiPayBusinessWalletInformation.LastName,
                                         Type = Common.Common.GetEnumDescription(KiiPayPersonalWalletPaymentType.PersonalToBusinessInternational),
                                         AccountNumber = c.KiiPayBusinessWalletInformation.MobileNo,
                                         Reference = c.PaymentReference,
                                         Gross = SendingCurrency + c.FaxingAmount.ToString(),
                                         Fee = SendingCurrency + c.FaxingFee.ToString(),
                                         Net = "-" + SendingCurrency + c.TotalAmount.ToString(),
                                         NetDecimal = c.TotalAmount,
                                         PaymentType = KiiPayPersonalWalletPaymentType.PersonalToBusinessInternational,
                                         InOut = InOut.Out
                                     })
                                 .Concat(from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.Where(x => x.SenderWalletId == WalletId).ToList()
                                         select new WalletStatementList()
                                         {
                                             Id = c.Id,
                                             Date = c.TransactionDate.Day.ToString() + "-" + Enum.GetName(typeof(Month), c.TransactionDate.Month).Substring(0, 4) + "-" + c.TransactionDate.Year.ToString(),
                                             TransactionDate = c.TransactionDate,
                                             Name = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + " " + c.KiiPayPersonalWalletInformation.LastName,
                                             Type = Common.Common.GetEnumDescription(KiiPayPersonalWalletPaymentType.PersonalToPersonal),
                                             AccountNumber = c.KiiPayPersonalWalletInformation.MobileNo,
                                             Reference = c.PaymentReference,
                                             Gross = SendingCurrency + c.FaxingAmount.ToString(),
                                             Fee = SendingCurrency + c.FaxingFee.ToString(),
                                             NetDecimal = c.TotalAmount,
                                             Net = "-" + SendingCurrency + c.TotalAmount.ToString(),
                                             PaymentType = KiiPayPersonalWalletPaymentType.PersonalToPersonal,
                                             InOut = InOut.Out
                                         })    //Calculate InComing Part
                                       .Concat(from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.Where(x => x.ReceiverWalletId == WalletId
                                                                   && x.TransactionFromPortal == TransactionFrom.KiiPayPortal).ToList()
                                               select new WalletStatementList()
                                               {
                                                   Id = c.Id,
                                                   Date = c.TransactionDate.Day.ToString() + "-" + Enum.GetName(typeof(Month), c.TransactionDate.Month).Substring(0, 4) + "-" + c.TransactionDate.Year.ToString(),
                                                   TransactionDate = c.TransactionDate,
                                                   Name = _commonServices.getKiiPayPersonalUserNameFromWalletId(c.SenderWalletId),
                                                   Type = Common.Common.GetEnumDescription(KiiPayPersonalWalletPaymentType.PersonalToPersonal),
                                                   AccountNumber = _commonServices.getKiiPayPersonalWalletNumberFromWalletId(c.SenderWalletId),
                                                   Reference = c.PaymentReference,
                                                   Gross = SendingCurrency + c.RecievingAmount.ToString(),
                                                   Fee = "+" + _commonServices.KiiPayPersonalWalletUserCurrencySymbolFromWalletId(c.SenderWalletId) + c.FaxingFee.ToString(),
                                                   Net = _commonServices.KiiPayPersonalWalletUserCurrencySymbolFromWalletId(c.SenderWalletId) + c.TotalAmount.ToString(),
                                                   NetDecimal = c.TotalAmount,
                                                   PaymentType = KiiPayPersonalWalletPaymentType.PersonalToPersonal,
                                                   InOut = InOut.In,
                                                   IsRefunded = c.IsRefunded
                                               })
                                               .Concat(from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayBusiness.Where(x => x.KiiPayPersonalWalletInformationId == WalletId).ToList()
                                                       select new WalletStatementList()
                                                       {
                                                           Id = c.Id,
                                                           Date = c.TransactionDate.Day.ToString() + "-" + Enum.GetName(typeof(Month), c.TransactionDate.Month).Substring(0, 4) + "-" + c.TransactionDate.Year.ToString(),
                                                           TransactionDate = c.TransactionDate,
                                                           Name = c.KiiPayBusinessWalletInformation.FirstName + " " + c.KiiPayBusinessWalletInformation.MiddleName + " " + c.KiiPayBusinessWalletInformation.LastName,
                                                           Type = Common.Common.GetEnumDescription(KiiPayPersonalWalletPaymentType.BusinessToPersonal),
                                                           AccountNumber = c.KiiPayBusinessWalletInformation.MobileNo,
                                                           Reference = c.PaymentReference,
                                                           Gross = SendingCurrency + c.RecievingAmount.ToString(),
                                                           Fee = _commonServices.KiiPayBusinessWalletUserCurrencySymbolFromWalletId(c.KiiPayBusinessWalletInformationId) + c.Fee.ToString(),
                                                           Net = "+" + _commonServices.KiiPayBusinessWalletUserCurrencySymbolFromWalletId(c.KiiPayBusinessWalletInformationId) + c.TotalAmount.ToString(),
                                                           NetDecimal = c.TotalAmount,
                                                           PaymentType = KiiPayPersonalWalletPaymentType.BusinessToPersonal,
                                                           InOut = InOut.In,
                                                           IsRefunded = c.IsRefunded
                                                       })
                                                       .Concat(from c in dbContext.AddMoneyToKiiPayPersonalWallet.Where(x => x.KiipayPersonalWalletId == WalletId).ToList()
                                                               join d in dbContext.KiiPayPersonalWalletInformation on c.KiipayPersonalWalletId equals d.Id
                                                               select new WalletStatementList()
                                                               {
                                                                   Id = c.Id,
                                                                   Date = c.TransactionDate.Day.ToString() + "-" + Enum.GetName(typeof(Month), c.TransactionDate.Month).Substring(0, 4) + "-" + c.TransactionDate.Year.ToString(),
                                                                   TransactionDate = c.TransactionDate,
                                                                   Name = c.NameOnCard,
                                                                   Type = Common.Common.GetEnumDescription(KiiPayPersonalWalletPaymentType.DebitCreditCard),
                                                                   AccountNumber = d.MobileNo,
                                                                   Reference = Common.Common.FormatSavedCardNumber(c.CardNum),
                                                                   Gross = SendingCurrency + c.Amount.ToString(),
                                                                   Fee = SendingCurrency + "0",
                                                                   Net = SendingCurrency + c.Amount.ToString(),
                                                                   NetDecimal = c.Amount,
                                                                   PaymentType = KiiPayPersonalWalletPaymentType.DebitCreditCard,
                                                                   InOut = InOut.In,
                                                                   IsRefunded = c.IsRefunded
                                                               }).OrderBy(x => x.TransactionDate).ToList();

            return WalletStatementList;
        }
        public List<WalletStatementList> WalletTransactionFormSender(int SenderId)
        {

            var KiiPayWallettrans = (from c in dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.FaxerId == SenderId && x.SenderPaymentMode == SenderPaymentMode.KiiPayWallet).ToList()
                                     select new WalletStatementList()
                                     {
                                         Id = c.Id,
                                         Date = c.TransactionDate.Day.ToString() + "-" + Enum.GetName(typeof(Month), c.TransactionDate.Month).Substring(0, 4) + "-" + c.TransactionDate.Year.ToString(),
                                         TransactionDate = c.TransactionDate,
                                         Name = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + " " + c.KiiPayPersonalWalletInformation.LastName,
                                         Type = Common.Common.GetEnumDescription(KiiPayPersonalWalletPaymentType.PersonalToPersonal),
                                         AccountNumber = c.KiiPayPersonalWalletInformation.MobileNo,
                                         Reference = c.TopUpReference,
                                         Gross = Common.Common.GetCurrencySymbol(c.SendingCountry) + c.FaxingAmount.ToString(),
                                         Fee = Common.Common.GetCurrencySymbol(c.SendingCountry) + c.FaxingFee,
                                         Net = "-" + Common.Common.GetCurrencySymbol(c.SendingCountry) + c.TotalAmount.ToString(),
                                         NetDecimal = c.TotalAmount,
                                         PaymentType = KiiPayPersonalWalletPaymentType.PersonalToPersonal,
                                         InOut = InOut.Out,
                                     }).ToList();


            var CashPickUptrans = (from c in dbContext.FaxingNonCardTransaction.Where(x => x.NonCardReciever.FaxerID == SenderId && x.SenderPaymentMode == SenderPaymentMode.KiiPayWallet).ToList()
                                   select new WalletStatementList()
                                   {
                                       Id = c.Id,
                                       Date = c.TransactionDate.Day.ToString() + "-" + Enum.GetName(typeof(Month), c.TransactionDate.Month).Substring(0, 4) + "-" + c.TransactionDate.Year.ToString(),
                                       TransactionDate = c.TransactionDate,
                                       Name = c.NonCardReciever.FullName == null ? c.NonCardReciever.FullName : c.NonCardReciever.FirstName + " " + c.NonCardReciever.MiddleName + " " + c.NonCardReciever.LastName,
                                       Type = Common.Common.GetEnumDescription(KiiPayPersonalWalletPaymentType.CashPickUp),
                                       AccountNumber = c.MFCN,
                                       Reference = c.MFCN,
                                       Gross = Common.Common.GetCurrencySymbol(c.SendingCountry) + c.FaxingAmount.ToString(),
                                       Fee = Common.Common.GetCurrencySymbol(c.SendingCountry) + c.FaxingFee,
                                       Net = "-" + Common.Common.GetCurrencySymbol(c.SendingCountry) + c.TotalAmount.ToString(),
                                       NetDecimal = c.TotalAmount,
                                       PaymentType = KiiPayPersonalWalletPaymentType.CashPickUp,
                                       InOut = InOut.Out,
                                   }).ToList();

            var ServicePaymenttrans = (from c in dbContext.FaxerMerchantPaymentTransaction.Where(x => x.SenderKiiPayBusinessPaymentInformation.SenderInformationId == SenderId && x.SenderPaymentMode == SenderPaymentMode.KiiPayWallet).ToList()
                                       select new WalletStatementList()
                                       {
                                           Id = c.Id,
                                           Date = c.PaymentDate.Day.ToString() + "-" + Enum.GetName(typeof(Month), c.PaymentDate.Month).Substring(0, 4) + "-" + c.PaymentDate.Year.ToString(),
                                           TransactionDate = c.PaymentDate,
                                           Name = c.SenderKiiPayBusinessPaymentInformation.KiiPayBusinessInformation.BusinessName,
                                           Type =  Common.Common.GetEnumDescription(c.PaymentType == PaymentType.International ? KiiPayPersonalWalletPaymentType.PersonalToBusinessInternational : KiiPayPersonalWalletPaymentType.PersonalToBusinessNational),
                                           AccountNumber = c.SenderKiiPayBusinessPaymentInformation.KiiPayBusinessInformation.BusinessMobileNo,
                                           Reference = c.PaymentReference,
                                           Gross = Common.Common.GetCurrencySymbol(c.SendingCountry) + c.PaymentAmount.ToString(),
                                           Fee = Common.Common.GetCurrencySymbol(c.SendingCountry) + c.FaxingFee,
                                           Net = "-" + Common.Common.GetCurrencySymbol(c.SendingCountry) + c.TotalAmount.ToString(),
                                           NetDecimal = c.TotalAmount,
                                           PaymentType = c.PaymentType == PaymentType.International ? KiiPayPersonalWalletPaymentType.PersonalToBusinessInternational : KiiPayPersonalWalletPaymentType.PersonalToBusinessNational,
                                           InOut = InOut.Out,
                                       }).ToList();



            var BillPaymenttrans = (from c in dbContext.PayBill.Where(x => x.PayerId == SenderId && x.SenderPaymentMode == SenderPaymentMode.KiiPayWallet).ToList()
                                    select new WalletStatementList()
                                    {
                                        Id = c.Id,
                                        Date = c.PaymentDate.Day.ToString() + "-" + Enum.GetName(typeof(Month), c.PaymentDate.Month).Substring(0, 4) + "-" + c.PaymentDate.Year.ToString(),
                                        TransactionDate = c.PaymentDate,
                                        Name = c.Supplier.KiiPayBusinessInformation.BusinessName,
                                        Type = Common.Common.GetEnumDescription(KiiPayPersonalWalletPaymentType.BillPayment),
                                        AccountNumber = c.Supplier.KiiPayBusinessInformation.BusinessMobileNo,
                                        Reference = c.RefCode,
                                        Gross = Common.Common.GetCurrencySymbol(c.PayerCountry) + c.SendingAmount.ToString(),
                                        Fee = Common.Common.GetCurrencySymbol(c.PayerCountry) + c.Fee,
                                        Net = "-" + Common.Common.GetCurrencySymbol(c.PayerCountry) + c.Total.ToString(),
                                        NetDecimal = c.Total,
                                        PaymentType = KiiPayPersonalWalletPaymentType.BillPayment,
                                        InOut = InOut.Out,
                                    }).ToList();

            var topUpAccounttrans = (from c in dbContext.TopUpToSupplier.Where(x => x.PayerId == SenderId && x.SenderPaymentMode == SenderPaymentMode.KiiPayWallet).ToList()
                                     join d in dbContext.Suppliers on c.SuplierId equals d.Id
                                     select new WalletStatementList()
                                     {
                                         Id = c.Id,
                                         Date = c.PaymentDate.Day.ToString() + "-" + Enum.GetName(typeof(Month), c.PaymentDate.Month).Substring(0, 4) + "-" + c.PaymentDate.Year.ToString(),
                                         TransactionDate = c.PaymentDate,
                                         Name = d.KiiPayBusinessInformation.BusinessName,
                                         Type = Common.Common.GetEnumDescription(KiiPayPersonalWalletPaymentType.BillPayment),
                                         AccountNumber = d.KiiPayBusinessInformation.BusinessMobileNo,
                                         Reference = c.ReceiptNo,
                                         Gross = Common.Common.GetCurrencySymbol(c.PayingCountry) + c.SendingAmount.ToString(),
                                         Fee = Common.Common.GetCurrencySymbol(c.PayingCountry) + c.Fee,
                                         Net = "-" + Common.Common.GetCurrencySymbol(c.PayingCountry) + c.TotalAmount.ToString(),
                                         NetDecimal = c.TotalAmount,
                                         PaymentType = KiiPayPersonalWalletPaymentType.BillPayment,
                                         InOut = InOut.Out,
                                     }).ToList();


            var mobileMoneyTransfertrans = (from c in dbContext.MobileMoneyTransfer.Where(x => x.SenderId == SenderId && x.SenderPaymentMode == SenderPaymentMode.KiiPayWallet).ToList()

                                            select new WalletStatementList()
                                            {
                                                Id = c.Id,
                                                Date = c.TransactionDate.Day.ToString() + "-" + Enum.GetName(typeof(Month), c.TransactionDate.Month).Substring(0, 4) + "-" + c.TransactionDate.Year.ToString(),
                                                TransactionDate = c.TransactionDate,
                                                Name = c.ReceiverName,
                                                Type = Common.Common.GetEnumDescription(KiiPayPersonalWalletPaymentType.MobileTransfer),
                                                AccountNumber = c.PaidToMobileNo,
                                                Reference = c.ReceiptNo,
                                                Gross = Common.Common.GetCurrencySymbol(c.SendingCountry) + c.SendingAmount.ToString(),
                                                Fee = Common.Common.GetCurrencySymbol(c.SendingCountry) + c.Fee,
                                                Net = "-" + Common.Common.GetCurrencySymbol(c.SendingCountry) + c.TotalAmount.ToString(),
                                                NetDecimal = c.TotalAmount,
                                                PaymentType = KiiPayPersonalWalletPaymentType.MobileTransfer,
                                                InOut = InOut.Out,
                                            }).ToList();

            var bankAccountDeposittrans = (from c in dbContext.BankAccountDeposit.Where(x => x.SenderId == SenderId && x.SenderPaymentMode == SenderPaymentMode.KiiPayWallet).ToList()

                                           select new WalletStatementList()
                                           {
                                               Id = c.Id,
                                               Date = c.TransactionDate.Day.ToString() + "-" + Enum.GetName(typeof(Month), c.TransactionDate.Month).Substring(0, 4) + "-" + c.TransactionDate.Year.ToString(),
                                               TransactionDate = c.TransactionDate,
                                               Name = c.ReceiverName,
                                               Type = Common.Common.GetEnumDescription(KiiPayPersonalWalletPaymentType.BankDeposit),
                                               AccountNumber = c.ReceiverAccountNo,
                                               Reference = c.ReceiptNo,
                                               Gross = Common.Common.GetCurrencySymbol(c.SendingCountry) + c.SendingAmount.ToString(),
                                               Fee = Common.Common.GetCurrencySymbol(c.SendingCountry) + c.Fee,
                                               Net = "-" + Common.Common.GetCurrencySymbol(c.SendingCountry) + c.TotalAmount.ToString(),
                                               NetDecimal = c.TotalAmount,
                                               PaymentType = KiiPayPersonalWalletPaymentType.BankDeposit,
                                               InOut = InOut.Out,
                                           }).ToList();


            var result = KiiPayWallettrans.
                          Concat(CashPickUptrans).
                          Concat(ServicePaymenttrans).
                          Concat(BillPaymenttrans).
                          Concat(topUpAccounttrans).
                          Concat(mobileMoneyTransfertrans).
                          Concat(bankAccountDeposittrans).ToList();


            return result;
            //decimal balance = 0;
            //foreach (var item in vm.WalletStatementList)
            //{
            //    if (item.InOut == InOut.In)
            //    {
            //        item.BalanceDecimal = balance + item.NetDecimal;
            //        item.Balance = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol + item.BalanceDecimal.ToString();
            //    }
            //    else
            //    {
            //        item.BalanceDecimal = balance - item.NetDecimal;
            //        item.Balance = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol + item.BalanceDecimal.ToString();
            //    }
            //    balance = item.BalanceDecimal;

            //}

        }

        public List<WalletStatementList> GetKiiPayPersonalBusinessNationalPayment()
        {

            var result = (from c in dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Where(x => x.KiiPayPersonalWalletInformationId == PersonalId).ToList()
                          select new WalletStatementList()
                          {
                              Id = c.Id,
                              Date = c.TransactionDate.Day.ToString() + "-" + Enum.GetName(typeof(Month), c.TransactionDate.Month).Substring(0, 4) + "-" + c.TransactionDate.Year.ToString(),
                              TransactionDate = c.TransactionDate,
                              Name = c.KiiPayBusinessWalletInformation.FirstName + " " + c.KiiPayBusinessWalletInformation.MiddleName + " " + c.KiiPayBusinessWalletInformation.LastName,
                              Type = Enum.GetName(typeof(KiiPayPersonalWalletPaymentType), KiiPayPersonalWalletPaymentType.PersonalToBusinessNational),
                              AccountNumber = c.KiiPayBusinessWalletInformation.MobileNo,
                              Reference = c.PaymentReference,
                              Gross = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol + c.AmountSent.ToString(),
                              Fee = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol + "0",
                              Net = "-" + Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol + c.AmountSent.ToString(),
                              NetDecimal = c.AmountSent,
                              PaymentType = KiiPayPersonalWalletPaymentType.PersonalToBusinessNational,
                              InOut = InOut.Out
                          }).ToList();
            return result;

        }

        public bool KiiPayPersonalPaymentRefund(int Id)
        {


            var data = dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.Where(x => x.Id == Id).FirstOrDefault();
            data.IsRefunded = true;
            dbContext.Entry<KiiPayPersonalWalletPaymentByKiiPayPersonal>(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();


            SaveRefundedTransaction(data.FaxingAmount, data.Id, TypeOfKiiPayPersonalTransaction.KiiPayPersonalPayment);

            _commonServices.BalanceIn(data.SenderId, data.FaxingAmount);
            _commonServices.BalanceOut(data.ReceiverWalletId, data.FaxingAmount);
            return true;
        }

        public bool RefundBusinessNationalPayment(int Id)
        {

            var data = dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Where(x => x.Id == Id).FirstOrDefault();
            data.IsRefunded = true;
            dbContext.Entry<KiiPayPersonalNationalKiiPayBusinessPayment>(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            SaveRefundedTransaction(data.AmountSent, data.Id, TypeOfKiiPayPersonalTransaction.BusinessToPersonalPayment);


            _commonServices.BalanceIn(data.KiiPayPersonalWalletInformationId, data.AmountSent);
            _commonServices.BalanceOut(data.KiiPayBusinessWalletInformationId, data.AmountSent);
            return true;

        }
        public bool RefundBusinessInternationalPayment(int Id)
        {

            var data = dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.Where(x => x.Id == Id).FirstOrDefault();
            data.IsRefunded = true;
            dbContext.Entry<KiiPayPersonalInternationalKiiPayBusinessPayment>(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            SaveRefundedTransaction(data.FaxingAmount, data.Id, TypeOfKiiPayPersonalTransaction.BusinessToPersonalPayment);

            _commonServices.BalanceIn(data.PayedFromKiiPayPersonalWalletId, data.FaxingAmount);
            _commonServices.BalanceOut(data.PayedToKiiPayBusinessInformationId, data.FaxingAmount);
            return true;

        }

        public DB.KiiPayPersonalRefundedTransaction SaveRefundedTransaction(decimal Amount, int TransactionId, TypeOfKiiPayPersonalTransaction typeOfTransaction)
        {


            DB.KiiPayPersonalRefundedTransaction refundedTransaction = new KiiPayPersonalRefundedTransaction()
            {
                AmountRefunded = Amount,
                RefundedDate = DateTime.Now,
                TransactionId = TransactionId,
                TypeOfTransaction = typeOfTransaction
            };

            dbContext.KiiPayPersonalRefundedTransaction.Add(refundedTransaction);
            dbContext.SaveChanges();
            return refundedTransaction;


        }
        public bool refundTransaction(int id, KiiPayPersonalWalletPaymentType paymentType)
        {
            if (id != 0)
            {
                if (paymentType == KiiPayPersonalWalletPaymentType.PersonalToPersonal)
                {
                    var data = dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.Find(id);
                    if (data != null)
                    {
                        //create refund data
                        DB.KiiPayPersonalRefundedTransaction refundData = new KiiPayPersonalRefundedTransaction()
                        {
                            TransactionId = data.Id,
                            AmountRefunded = data.FaxingAmount,
                            TypeOfTransaction = TypeOfKiiPayPersonalTransaction.KiiPayPersonalPayment,
                            RefundedDate = DateTime.Now
                        };
                        var saveRefundData = dbContext.KiiPayPersonalRefundedTransaction.Add(refundData);
                        dbContext.SaveChanges();

                        //deduct from receiver's account
                        var receiverWalletData = dbContext.KiiPayPersonalWalletInformation.Find(data.ReceiverWalletId);
                        if (receiverWalletData != null)
                        {
                            receiverWalletData.CurrentBalance = receiverWalletData.CurrentBalance - data.RecievingAmount;
                            dbContext.Entry(receiverWalletData).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }


                        //add to sender's account
                        var senderWalletData = dbContext.KiiPayPersonalWalletInformation.Find(data.SenderWalletId);
                        if (senderWalletData != null)
                        {
                            senderWalletData.CurrentBalance = senderWalletData.CurrentBalance + data.FaxingAmount;
                            dbContext.Entry(senderWalletData).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }


                        //update main transaction table
                        data.IsRefunded = true;
                        data.KiiPayPersonalRefundedTransactionId = saveRefundData.Id;
                        dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        return true;
                    }
                }
                else if (paymentType == KiiPayPersonalWalletPaymentType.BusinessToPersonal)
                {
                    var data = dbContext.KiiPayPersonalWalletPaymentByKiiPayBusiness.Find(id);
                    if (data != null)
                    {
                        //create Refund data
                        DB.KiiPayPersonalRefundedTransaction refundData = new KiiPayPersonalRefundedTransaction()
                        {
                            TransactionId = data.Id,
                            AmountRefunded = data.PayingAmount,
                            TypeOfTransaction = TypeOfKiiPayPersonalTransaction.BusinessToPersonalPayment,
                            RefundedDate = DateTime.Now
                        };
                        var saveRefundData = dbContext.KiiPayPersonalRefundedTransaction.Add(refundData);
                        dbContext.SaveChanges();

                        //deduct from receiver's account
                        var receiverWalletData = dbContext.KiiPayPersonalWalletInformation.Find(data.KiiPayPersonalWalletInformationId);
                        if (receiverWalletData != null)
                        {
                            receiverWalletData.CurrentBalance = receiverWalletData.CurrentBalance - data.RecievingAmount;
                            dbContext.Entry(receiverWalletData).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }

                        //add to sender's account
                        var senderWalletData = dbContext.KiiPayBusinessWalletInformation.Find(data.KiiPayBusinessWalletInformationId);
                        if (senderWalletData != null)
                        {
                            senderWalletData.CurrentBalance = senderWalletData.CurrentBalance + data.PayingAmount;
                            dbContext.Entry(senderWalletData).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                        }

                        //update main transaction table
                        data.IsRefunded = true;
                        data.KiiPayPersonalRefundedTransactionId = saveRefundData.Id;
                        dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }
    }
}