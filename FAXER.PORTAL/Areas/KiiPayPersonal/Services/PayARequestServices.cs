using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Services
{
    public class PayARequestServices
    {
        FAXEREntities dbContext = null;
        KiiPayPersonalCommonServices _commonServices = null;
        public PayARequestServices()
        {
            _commonServices = new KiiPayPersonalCommonServices();
            dbContext = new FAXEREntities();
        }


        public PayARequestViewModel getPayRequestsPageViewModel()
        {
            PayARequestViewModel vm = new PayARequestViewModel()
            {
                StatusList = 0
            };
            int kiiPayWalletId = dbContext.KiiPayPersonalUserInformation.Where(x => x.Id == Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId).FirstOrDefault().KiiPayPersonalWalletInformationId;

            var requestsList = (from c in dbContext.KiiPayPersonalRequestForPayment.Where(x => x.RequestReceiverId == kiiPayWalletId && x.IsDeleted == false).ToList()
                                select new RequestsList()
                                {
                                    Id = c.Id,
                                    Name = _commonServices.getKiiPayPersonalUserNameFromWalletId(c.RequestSenderId),
                                    WalletNo = _commonServices.getKiiPayPersonalWalletNumberFromWalletId(c.RequestSenderId),
                                    Date = c.RequestedDate.Day.ToString() + "-" + Enum.GetName(typeof(Month), c.RequestedDate.Month).Substring(0, 4) + "-" + c.RequestedDate.Year.ToString(),
                                    Amount = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol + c.RequestSendingAmount.ToString(),
                                    PaymentType = c.RequestType,
                                    Status = c.Status,
                                    isPaid = c.IsPaid
                                }).ToList();
            vm.RequestsList = requestsList;
            return vm;
        }

        public PayAPaymentRequestViewModel getPaymentSummaryPageViewModel(int id)
        {
            if (id != 0)
            {
                var paymentRequestData = dbContext.KiiPayPersonalRequestForPayment.Find(id);
                if (paymentRequestData != null)
                {
                    var receiverWalletData = dbContext.KiiPayPersonalWalletInformation.Find(paymentRequestData.RequestSenderId);
                    if (receiverWalletData != null)
                    {
                        decimal exchangeRate = _commonServices.calculateExchangeRate(Common.CardUserSession.LoggedCardUserViewModel.CountryCode, receiverWalletData.CardUserCountry);
                        decimal faxingFeeRate = SEstimateFee.GetFaxingCommision(Common.CardUserSession.LoggedCardUserViewModel.CountryCode);
                        var getSummary = SEstimateFee.CalculateFaxingFee(paymentRequestData.RequestReceivingAmount, true, true, exchangeRate, faxingFeeRate);

                        var result = new PayAPaymentRequestViewModel()
                        {
                            Id = paymentRequestData.Id,
                            PhotoUrl = receiverWalletData.UserPhoto,
                            ReceiverName = receiverWalletData.FirstName + " " + receiverWalletData.MiddleName + " " + receiverWalletData.LastName,
                            ReceiverWalletId = receiverWalletData.Id,
                            Amount = getSummary.FaxingAmount,
                            Fee = getSummary.FaxingFee,
                            PayingAmount = getSummary.TotalAmount,
                            ReceivingAmount = getSummary.ReceivingAmount,
                            ExchangeRate = getSummary.ExchangeRate,
                            ReceiverCurrencySymbol = Common.Common.GetCurrencySymbol(receiverWalletData.CardUserCountry)
                        };
                        if (Common.CardUserSession.PayARequestSession != null)
                        {
                            Common.CardUserSession.PayARequestSession = null;
                        }
                        Common.CardUserSession.PayARequestSession = result;
                        return result;
                    }
                }
            }
            return null;
        }

        public bool isRequestAlreadyPaid(int id)
        {
            var data = dbContext.KiiPayPersonalRequestForPayment.Find(id);
            if(data != null)
            {
                if(data.IsPaid == true)
                {
                    return true;
                }
            }
            return false;
        }

        public bool isRequestCancelled(int id)
        {
            var data = dbContext.KiiPayPersonalRequestForPayment.Find(id);
            if(data != null)
            {
                if(data.Status == RequestPaymentStatus.Cancelled)
                {
                    return true;
                }
            }
            return false;
        }

        public bool payAPaymentRequest()
        {
            if (Common.CardUserSession.PayARequestSession != null)
            {
                var paymentSessionData = Common.CardUserSession.PayARequestSession;
                var senderUserData = dbContext.KiiPayPersonalUserInformation.Find(Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId);
                if (senderUserData != null)
                {
                    var senderWalletData = dbContext.KiiPayPersonalWalletInformation.Find(senderUserData.KiiPayPersonalWalletInformationId);
                    var receiverWalletData = dbContext.KiiPayPersonalWalletInformation.Find(paymentSessionData.ReceiverWalletId);
                    if (senderWalletData != null && receiverWalletData != null)
                    {
                        //deducting amount from sender's wallet
                        senderWalletData.CurrentBalance = senderWalletData.CurrentBalance - paymentSessionData.PayingAmount;
                        dbContext.Entry(senderWalletData).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                        //adding amount to receiver's wallet
                        receiverWalletData.CurrentBalance = receiverWalletData.CurrentBalance + paymentSessionData.ReceivingAmount;
                        dbContext.Entry(receiverWalletData).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                        //updating transaction table
                        var paymentRequestTableData = dbContext.KiiPayPersonalRequestForPayment.Find(paymentSessionData.Id);
                        if(paymentRequestTableData != null)
                        {
                            paymentRequestTableData.RequestSendingAmount = paymentSessionData.Amount;
                            paymentRequestTableData.RequestReceivingAmount = paymentSessionData.ReceivingAmount;
                            paymentRequestTableData.TotalAmount = paymentSessionData.PayingAmount;
                            paymentRequestTableData.Fee = paymentSessionData.Fee;
                            paymentRequestTableData.ExchangeRate = paymentSessionData.ExchangeRate;
                            paymentRequestTableData.IsPaid = true;
                            paymentRequestTableData.Status = RequestPaymentStatus.Paid;
                            paymentRequestTableData.PaymentDate = DateTime.Now;
                            dbContext.Entry(paymentRequestTableData).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();
                            Common.CardUserSession.LoggedCardUserViewModel.BalanceOnCard = _commonServices.getAvailableBalance();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public Dictionary<int, string> getPaidStatusList()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            dict.Add(0, "All");
            dict.Add(1, "Paid");
            dict.Add(2, "Unpaid");
            dict.Add(3, "Cancelled");
            return dict;
        }


    }
}