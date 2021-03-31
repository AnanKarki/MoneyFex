using System;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Common;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewMFTCCardToppingServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();

        public ViewMFTCCardToppingViewModel getMFTCCardTopUpList(string CountryCode = "", string City = "")
        {
            var data = new List<DB.SenderKiiPayPersonalWalletPayment>();
            //var data2 = new List<DB.TopUpSomeoneElseCardTransaction>();

            if (!string.IsNullOrEmpty(CountryCode) && string.IsNullOrEmpty(City))
            {
                data = dbContext.FaxingCardTransaction.Where(x => x.KiiPayPersonalWalletInformation.CardUserCountry == CountryCode).ToList();
                //data2 = dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.MFTCCardInformation.CardUserCountry == CountryCode).ToList();
            }
            else if (string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = dbContext.FaxingCardTransaction.Where(x => x.KiiPayPersonalWalletInformation.CardUserCity.ToLower() == City.ToLower()).ToList();
                //data2 = dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.MFTCCardInformation.CardUserCity.ToLower() == City.ToLower()).ToList();
            }
            else if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = dbContext.FaxingCardTransaction.Where(x => (x.KiiPayPersonalWalletInformation.CardUserCity.ToLower() == City.ToLower()) && (x.KiiPayPersonalWalletInformation.CardUserCountry == CountryCode)).ToList();
                //data2 = dbContext.TopUpSomeoneElseCardTransaction.Where(x => (x.MFTCCardInformation.CardUserCity.ToLower() == City.ToLower()) && (x.MFTCCardInformation.CardUserCountry == CountryCode)).ToList();
            }



            var result = new ViewMFTCCardToppingViewModel();

            var list1 = (from c in data.OrderByDescending(x => x.TransactionDate)
                         join e in dbContext.CardTopUpCreditDebitInformation on c.Id equals e.CardTransactionId into j
                         join d in dbContext.PaymentMethods on c.PaymentMethod equals d.PaymentMethodCode into k
                         from joined in k.DefaultIfEmpty()
                         from joinedJ in j.DefaultIfEmpty()
                         select new ViewMFTCCardTopUpViewModel()
                         {
                             Id = c.Id,
                             FaxerFirstName = "" , //c.KiiPayPersonalWalletInformation.FaxerInformation.FirstName,
                             FaxerMiddleName =  "" , //c.KiiPayPersonalWalletInformation.FaxerInformation.MiddleName,
                             FaxerLastName =   "" ,//c.KiiPayPersonalWalletInformation.FaxerInformation.LastName,
                             DateTimeForOrdering = c.TransactionDate,
                             CardUserFirstName = c.KiiPayPersonalWalletInformation.FirstName,
                             CardUserMiddleName = c.KiiPayPersonalWalletInformation.MiddleName,
                             CardUserLastName = c.KiiPayPersonalWalletInformation.LastName,
                             MFTCCardNumber = c.KiiPayPersonalWalletInformation.MobileNo.Contains("MFS") ? c.KiiPayPersonalWalletInformation.MobileNo : c.KiiPayPersonalWalletInformation.MobileNo.Decrypt().GetVirtualAccountNo(),
                             MFTCCardTopUPAmount = c.FaxingAmount,
                             Currency = CommonService.getCurrencyCodeFromCountry(c.KiiPayPersonalWalletInformation.CardUserCountry),
                             MFTCCardTopUPDate = c.TransactionDate.ToFormatedString(),
                             MFTCCardTopUPTime = c.TransactionDate.ToString("HH:mm"),
                             AmountOnMFTCCard = c.KiiPayPersonalWalletInformation.CurrentBalance,
                             MFTCCardTopUpMethod = c.OperatingUserType == OperatingUserType.Sender ? "Card-Registrar" : "",//   Enum.GetName(typeof(OperatingUserType), c.OperatingUserType),
                             PaymentMethod = joined == null ? "" : joined.PaymentMethodName,
                             MFTCCardTopUpper = c.OperatingUserName,
                             WithdrawalLimitAmount = c.KiiPayPersonalWalletInformation.CashWithdrawalLimit,
                             WithdrawalLimitType = Enum.GetName(typeof(AutoPaymentFrequency), c.KiiPayPersonalWalletInformation.CashLimitType),
                             PurchaseLimitAmount = c.KiiPayPersonalWalletInformation.GoodsPurchaseLimit,
                             PurchaseLimitType = Enum.GetName(typeof(AutoPaymentFrequency), c.KiiPayPersonalWalletInformation.GoodsLimitType),

                             //card details

                             CardName = joinedJ == null ? "" : joinedJ.NameOnCard,
                             CardNumber = joinedJ == null ? "" : joinedJ.CardNumber,
                             CardExpMonth = joinedJ == null ? "" : joinedJ.ExpiryDate.Substring(0, 2),
                             CardExpYear = joinedJ == null ? "" : joinedJ.ExpiryDate.Substring(3, 2),
                             IsCreditDebitSaved = joinedJ == null ? "" : joinedJ.IsSavedCard == true ? "Yes" : "No",
                             IsAutoRechargeActivated = joinedJ == null ? "" : joinedJ.AutoRecharged == true ? "Yes" : "No",
                             TopupedBy = TopupedBy.Sender

                         }).ToList().OrderByDescending(x => x.DateTimeForOrdering);

            //var list2 = (from c in data2.OrderByDescending(x => x.TransactionDate)
            //             join e in dbContext.CardTopUpCreditDebitInformation on c.Id equals e.CardTransactionId into j
            //             join d in dbContext.PaymentMethods on c.PaymentMethod equals d.PaymentMethodCode into k
            //             from joined in k.DefaultIfEmpty()
            //             from joinedJ in j.DefaultIfEmpty()
            //             select new ViewMFTCCardTopUpViewModel()
            //             {
            //                 Id = c.Id,
            //                 FaxerFirstName = c.MFTCCardInformation.FaxerInformation.FirstName,
            //                 FaxerMiddleName = c.MFTCCardInformation.FaxerInformation.MiddleName,
            //                 FaxerLastName = c.MFTCCardInformation.FaxerInformation.LastName,
            //                 CardUserFirstName = c.MFTCCardInformation.FirstName,
            //                 CardUserMiddleName = c.MFTCCardInformation.MiddleName,
            //                 CardUserLastName = c.MFTCCardInformation.LastName,
            //                 DateTimeForOrdering = c.TransactionDate,
            //                 MFTCCardNumber = c.MFTCCardInformation.MFTCCardNumber.Decrypt().FormatMFTCCard(),
            //                 MFTCCardTopUPAmount = c.FaxingAmount,
            //                 Currency = CommonService.getCurrencyCodeFromCountry(c.MFTCCardInformation.CardUserCountry),
            //                 MFTCCardTopUPDate = c.TransactionDate.ToFormatedString(),
            //                 MFTCCardTopUPTime = c.TransactionDate.ToString("HH:mm"),
            //                 AmountOnMFTCCard = c.MFTCCardInformation.CurrentBalance,
            //                 MFTCCardTopUpMethod = Enum.GetName(typeof(OperatingUserType), 0),
            //                 PaymentMethod = joined == null ? "" : joined.PaymentMethodName,
            //                 MFTCCardTopUpper = CommonService.getFaxerName(c.FaxerId),
            //                 WithdrawalLimitAmount = c.MFTCCardInformation.CashWithdrawalLimit,
            //                 WithdrawalLimitType = Enum.GetName(typeof(AutoPaymentFrequency), c.MFTCCardInformation.CashLimitType),
            //                 PurchaseLimitAmount = c.MFTCCardInformation.GoodsPurchaseLimit,
            //                 PurchaseLimitType = Enum.GetName(typeof(AutoPaymentFrequency), c.MFTCCardInformation.GoodsLimitType),
            //                 //card details

            //                 CardName = joinedJ == null ? "" : joinedJ.NameOnCard,
            //                 CardNumber = joinedJ == null ? "" : joinedJ.CardNumber,
            //                 CardExpMonth = joinedJ == null ? "" : joinedJ.ExpiryDate.Substring(0, 2),
            //                 CardExpYear = joinedJ == null ? "" : joinedJ.ExpiryDate.Substring(3, 2),
            //                 IsCreditDebitSaved = joinedJ == null ? "" : joinedJ.IsSavedCard == true ? "Yes" : "No",
            //                 IsAutoRechargeActivated = joinedJ == null ? "" : joinedJ.AutoRecharged == true ? "Yes" : "No",
            //                  TopupedBy = TopupedBy.SomeoneElse
            //             }).ToList();

            result.ViewMFTCCardTopUp = list1.ToList();

            return result;
        }

        public MFTCCardToppingReceiptViewModel getReceiptInfo(int id)
        {
            var data = (from c in dbContext.FaxingCardTransaction.Where(x => x.Id == id).ToList()
                        select new MFTCCardToppingReceiptViewModel()
                        {
                            Id = c.Id,
                            ReceiptNumber = c.ReceiptNumber,
                            Date = c.TransactionDate.ToFormatedString(),
                            Time = c.TransactionDate.ToString("HH:mm"),
                            FaxerFullName = "" ,  //c.KiiPayPersonalWalletInformation.FaxerInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.FaxerInformation.MiddleName + " " + c.KiiPayPersonalWalletInformation.FaxerInformation.LastName,
                            MFTCCardNumber = c.KiiPayPersonalWalletInformation.MobileNo.Contains("MF")  ? c.KiiPayPersonalWalletInformation.MobileNo : c.KiiPayPersonalWalletInformation.MobileNo.Decrypt().FormatMFTCCard(),
                            CardUserFullName = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + " " + c.KiiPayPersonalWalletInformation.LastName,
                            AmountTopUp = c.FaxingAmount.ToString(),
                            ExchangeRate = c.ExchangeRate.ToString(),
                            AmountInLocalCurrency = (c.FaxingAmount * c.ExchangeRate).ToString(),
                            Fee = c.FaxingFee.ToString(),
                            BalanceOnCard = c.KiiPayPersonalWalletInformation.CurrentBalance.ToString(),
                            StaffName = c.OperatingUserName,
                            StaffCode = c.OperatingStaffId != 0 ? CommonService.getStaffMFSCode(c.OperatingStaffId) : "",
                            SendingCurrency = CommonService.getCurrencyCodeFromCountry(c.KiiPayPersonalWalletInformation.CardUserCountry),
                            ReceivingCurrency = CommonService.getCurrencyCodeFromCountry(c.KiiPayPersonalWalletInformation.CardUserCountry),
                            StaffLoginCode = c.OperatingStaffId != 0 ? CommonService.getStaffLoginCode(c.OperatingStaffId) : "" ,
                            FaxerCountry = Common.Common.GetCountryName( c.KiiPayPersonalWalletInformation.CardUserCountry),
                            CardUserCity = c.KiiPayPersonalWalletInformation.CardUserCity,
                            CardUserCountry = Common.Common.GetCountryName(c.KiiPayPersonalWalletInformation.CardUserCountry)

                        }).FirstOrDefault();
            return data;
        }


    }
}