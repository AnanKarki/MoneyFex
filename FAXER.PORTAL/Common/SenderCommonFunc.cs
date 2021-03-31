using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Common
{
    public class SenderCommonFunc
    {

        DB.FAXEREntities dbContext = null;
        public SenderCommonFunc()
        {
            dbContext = new DB.FAXEREntities();
        }

        public SenderCommonFunc(DB.FAXEREntities db)
        {
            dbContext = db;
        }

        public List<SenderRecentReceiverDropDownVM> GetRecenltyPaidReceivers(string country)
        {


            var result = new List<SenderRecentReceiverDropDownVM>();

            int senderId = FaxerSession.LoggedUser.Id;
            result = (from c in dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.FaxerId == senderId && x.ReceivingCountry == country).ToList()
                      select new SenderRecentReceiverDropDownVM()
                      {
                          Id = c.KiiPayPersonalWalletId,
                          ReceiverMobileNo = c.KiiPayPersonalWalletInformation.MobileNo,
                          ReceiverName = c.KiiPayPersonalWalletInformation.FirstName
                      }).GroupBy(x => x.Id).Select(x => x.FirstOrDefault()).ToList();

            int senderWalletId = 0;
            var senderWalletInfo = GetSenderKiiPayWalletInfo(senderId);
            if (senderWalletInfo != null)
            {

                senderWalletId = senderWalletInfo.Id;
            }
            var OtherData = (from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.Where(x => x.SenderWalletId == senderWalletId && x.ReceivingCountry == country).ToList()
                             select new SenderRecentReceiverDropDownVM()
                             {
                                 Id = c.ReceiverWalletId,
                                 ReceiverMobileNo = c.KiiPayPersonalWalletInformation.MobileNo,
                                 ReceiverName = c.KiiPayPersonalWalletInformation.FirstName
                             }).GroupBy(x => x.Id).Select(x => x.FirstOrDefault()).ToList();

            result.Concat(OtherData).ToList();
            return result;

        }

        internal int GetwalletCountOfSelf(int id)
        {

            var result = dbContext.SenderKiiPayPersonalAccount.Where(x => x.SenderId == id && x.KiiPayAccountIsOF == DB.KiiPayAccountIsOF.Sender).Count();

            return result;

        }

        public string GetSenderAddress()
        {

            var senderInfo = dbContext.FaxerInformation.Where(x => x.Id == FaxerSession.LoggedUser.Id).FirstOrDefault();
            string address = senderInfo.Address1 + " " + senderInfo.City + " " + senderInfo.PostalCode;
            if (senderInfo != null)
            {
                return address;
            }
            else
            {
                return null;
            }
        }
        public KiiPayPersonalWalletInformation GetSenderKiiPayWalletInfo(int senderId = 0)
        {
            var result = dbContext.SenderKiiPayPersonalAccount.Where(x => x.SenderId == senderId && x.KiiPayAccountIsOF == DB.KiiPayAccountIsOF.Sender).FirstOrDefault();

            if (result != null)
            {
                return result.KiiPayPersonalWalletInformation;
            }

            return null;

        }
        public KiiPayPersonalWalletInformation GetKiiPayWalletInfo(int senderId, int walletId)
        {



            var result = dbContext.SenderKiiPayPersonalAccount.Where(x => x.SenderId == senderId && x.KiiPayPersonalWalletId == walletId).FirstOrDefault();

            if (result == null)
            {

                return null;
            }
            return result.KiiPayPersonalWalletInformation;

        }


        internal int GetwalletCountOfSender(int id)
        {
            var result = dbContext.SenderKiiPayPersonalAccount.Where(x => x.SenderId == id && x.KiiPayAccountIsOF == DB.KiiPayAccountIsOF.Family).Count();

            return result;

        }
        internal int GetwalletCountOfSenderSelf(int id)
        {
            var result = dbContext.SenderKiiPayPersonalAccount.Where(x => x.SenderId == id && x.KiiPayAccountIsOF == DB.KiiPayAccountIsOF.Sender).Count();

            return result;

        }


        public decimal GetCurrentKiiPayWalletBal(int walletId)
        {

            var result = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == walletId).Select(x => x.CurrentBalance).FirstOrDefault();
            return result;
        }
        public List<SenderSavedDebitCreditCard> GetSavedDebitCreditCardDetails()
        {
            var SenderId = FaxerSession.LoggedUser == null ? 0 : FaxerSession.LoggedUser.Id;
            var result = (from c in dbContext.SavedCard.Where(x => x.Module == DB.Module.Faxer && x.UserId == SenderId).ToList()
                          select new SenderSavedDebitCreditCard()
                          {
                              CardId = c.Id,
                              CardHolderName = c.CardName.Decrypt(),
                              CardNumber = c.Num.Decrypt().FormatSavedCardNumber(),
                              ExpiringDateMonth = c.EMonth.Decrypt(),
                              ExpiringDateYear = c.EYear.Decrypt(),
                              CreditDebitCardType = c.Type,
                          }).ToList();
            return result;

        }

        public List<SenderSavedDebitCreditCard> GetSavedDebitCreditCardDetails(int SenderId, Module module = Module.Faxer)
        {

            var result = (from c in dbContext.SavedCard.Where(x => x.Module == module && x.UserId == SenderId).ToList()
                          select new SenderSavedDebitCreditCard()
                          {
                              CardId = c.Id,
                              CardHolderName = c.CardName.Decrypt(),
                              CardNumber = c.Num.Decrypt().FormatSavedCardNumber(),
                              DecryptedCardNumber = c.Num.Decrypt(),
                              ExpiringDateMonth = c.EMonth.Decrypt(),
                              ExpiringDateYear = c.EYear.Decrypt(),
                              CreditDebitCardType = c.Type,
                             SecurityCode = c.ClientCode.Decrypt()
                          }).ToList();
            return result;

        }


        public void GetAddressDropDown()
        {

            int SenderId = FaxerSession.LoggedUser.Id;
            var senderInfo = dbContext.FaxerInformation.Where(x => x.Id == SenderId).FirstOrDefault();

        }


        public bool SenderHasEnoughWalletBaltoTransfer(decimal Amount, int WalletId)
        {

            var senderWalletBalance = GetCurrentKiiPayWalletBal(WalletId);

            if (senderWalletBalance < Amount)
            {

                return false;
            }
            return true;

        }


        public decimal GetMonthlyTransactionMeter(int senderId = 0)
        {


            try
            {

                var Currentmonth = DateTime.Now.Month;

                var Meter1data = dbContext.FaxerMerchantPaymentTransaction.Where(x => x.SenderKiiPayBusinessPaymentInformation.SenderInformation.Id == senderId && x.PaymentDate.Month == Currentmonth).ToList();

                decimal Meter1 = 0M;
                if (Meter1data.Count > 0) {

                    Meter1 = Meter1data.Sum(x => x.TotalAmount);
                }
                decimal Meter3 = 0M;
                //var Meter2 = dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.Where(x => x.SenderId == senderId && x.TransactionDate.Month == Currentmonth).ToList().Sum(x => x.TotalAmount);  
                var Meter3data = dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.FaxerId == senderId && x.TransactionDate.Month == Currentmonth).ToList();
                if (Meter1data.Count > 0) {

                    Meter3 = Meter3data.Sum(x => x.TotalAmount);
                }

                decimal Meter4 = 0M;

                var Meter4data = dbContext.FaxingNonCardTransaction.Where(x => x.NonCardReciever.FaxerID == senderId && x.TransactionDate.Month == Currentmonth && x.FaxingStatus != FaxingStatus.Cancel).ToList();
                if (Meter4data.Count > 0) {

                    Meter4 = Meter4data.Sum(x => x.TotalAmount);
                }

                decimal Meter5 = 0M;
                var Meter5data = dbContext.MobileMoneyTransfer.Where(x => x.SenderId == senderId && x.TransactionDate.Month == Currentmonth && x.Status != MobileMoneyTransferStatus.Cancel).ToList();
                if (Meter5data.Count > 0) {

                    Meter5 = Meter5data.Sum(x => x.TotalAmount);

                }
                decimal Meter6 = 0M;
                var Meter6data = dbContext.BankAccountDeposit.Where(x => x.SenderId == senderId && x.TransactionDate.Month == Currentmonth && x.Status != BankDepositStatus.Cancel).ToList();
                if (Meter6data.Count > 0) {

                    Meter6 = Meter6data.Sum(x => x.TotalAmount);
                }

                return Meter1 + Meter3 + Meter4 + Meter5 + Meter6;

            }
            catch (Exception ex)
            {
                Log.Write("MobileError Api GetMonthlyTransactionMeter" + " " + ex.Message);
                return 0;
            }
            
        }

        public void ClearKiiPayTransferSession()
        {

            ClearSessionData();
            HttpContext.Current.Session.Remove("SearchKiiPayWalletVM");
            HttpContext.Current.Session.Remove("CommonEnterAmountViewModel");
            HttpContext.Current.Session.Remove("SearchKiiPayWalletVM");
            HttpContext.Current.Session.Remove("KiiPayTransferPaymentSummary");
            HttpContext.Current.Session.Remove("PaymentMethodViewModel");
            HttpContext.Current.Session.Remove("SenderAndReceiverDetials");
            HttpContext.Current.Session.Remove("TransactionSummary");

        }
        public void ClearTransferBankDepositSession()
        {

            ClearSessionData();
            HttpContext.Current.Session.Remove("SenderBankAccountDeposit");
            HttpContext.Current.Session.Remove("CommonEnterAmountViewModel");
            HttpContext.Current.Session.Remove("SenderBankAccoutDepositEnterAmount");
            HttpContext.Current.Session.Remove("PaymentMethodViewModel");
            HttpContext.Current.Session.Remove("TransactionSummary");
            HttpContext.Current.Session.Remove("CreditDebitDetails");
        }
        public void ClearCashPickUpSession()
        {

            ClearSessionData();
            HttpContext.Current.Session.Remove("SenderCashPickUp");
            HttpContext.Current.Session.Remove("CommonEnterAmountViewModel");
            HttpContext.Current.Session.Remove("SenderMobileEnrterAmount");
            HttpContext.Current.Session.Remove("TransactionSummary");
            HttpContext.Current.Session.Remove("SenderMoneyFexBankDeposit");
        }

        public void ClearFamilyAndFriendSession()
        {

            ClearSessionData();
            HttpContext.Current.Session.Remove("SenderAndReceiverDetials");
            HttpContext.Current.Session.Remove("KiiPayTransferPaymentSummary");
            HttpContext.Current.Session.Remove("SearchKiiPayWalletVM");
            HttpContext.Current.Session.Remove("SenderMobileEnrterAmount");
            HttpContext.Current.Session.Remove("PaymentMethodViewModel");
            HttpContext.Current.Session.Remove("CreditDebitDetails");
            HttpContext.Current.Session.Remove("SenderMoneyFexBankDeposit");
        }

        public void ClearPayBillsSession()
        {

            ClearSessionData();
            HttpContext.Current.Session.Remove("SenderPayMonthlyBillVM");
            HttpContext.Current.Session.Remove("PayBillSupplierCountryCode");
            HttpContext.Current.Session.Remove("PaymentReference");
            HttpContext.Current.Session.Remove("SenderPayingSupplierAbroadReferenceOne");
            HttpContext.Current.Session.Remove("SenderTopUpAnAccount");
            HttpContext.Current.Session.Remove("SenderTopUpAccountNumber");
            HttpContext.Current.Session.Remove("PaymentReference");
            HttpContext.Current.Session.Remove("SenderTopUpSupplierAbroadAbroadEnterAmont");
            HttpContext.Current.Session.Remove("SenderTopUpSupplierAbroadVm");
            HttpContext.Current.Session.Remove("SenderTopUpAnLocalAccount");
        }

        public void ClearPayForServiceSession()
        {

            ClearSessionData();
            HttpContext.Current.Session.Remove("SenderPayForGoodsAndServices");
            HttpContext.Current.Session.Remove("SenderMobileEnrterAmount");
            HttpContext.Current.Session.Remove("PaymentMethodViewModel");
            HttpContext.Current.Session.Remove("TransactionSummary");
            HttpContext.Current.Session.Remove("CreditDebitDetails");
            HttpContext.Current.Session.Remove("SenderMoneyFexBankDeposit");
            HttpContext.Current.Session.Remove("SenderLocalEnterAmount");
            HttpContext.Current.Session.Remove("SenderAccountPaymentSummary");


        }
        public void ClearMobileTransferSession()
        {

            ClearSessionData();
            HttpContext.Current.Session.Remove("SenderMobileMoneyTransfer");
            HttpContext.Current.Session.Remove("SenderMobileEnrterAmount");
            HttpContext.Current.Session.Remove("PaymentMethodViewModel");
            HttpContext.Current.Session.Remove("TransactionSummary");
            HttpContext.Current.Session.Remove("CommonEnterAmountViewModel");
            HttpContext.Current.Session.Remove("SenderMoneyFexBankDeposit");
            HttpContext.Current.Session.Remove("CreditDebitDetails");

        }
        public void ClearStaffMobileTransferSession()
        {
            ClearSessionData();
            HttpContext.Current.Session.Remove("CashPickupInformationViewModel");
            HttpContext.Current.Session.Remove("ReceiverDetailsInformation");
            HttpContext.Current.Session.Remove("PaymentMethodViewModel");
            HttpContext.Current.Session.Remove("TransactionSummary");
            HttpContext.Current.Session.Remove("MobileMoneyTransferEnterAmount");
            HttpContext.Current.Session.Remove("SenderMoneyFexBankDeposit");
            HttpContext.Current.Session.Remove("CreditDebitDetails");
        }

        public void ClearSessionData() {

            var id = HttpContext.Current.Session.SessionID;

            SSessionTransactionSummary sessionTransactionSummary = new SSessionTransactionSummary();
        sessionTransactionSummary.ClearSessionTransactionSummary(id);
        }
        internal bool IsEnabledMoneyFexbankAccount(string Country)
        {
            var data = dbContext.BankAccount.Where(x => x.CountryCode == Country).FirstOrDefault();

            if (data != null)
            {

                return true;
            }
            return false;

        }
    }
}