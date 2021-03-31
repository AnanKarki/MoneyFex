using FAXER.PORTAL.Areas.KiiPayPersonal.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SSenderPayARequest
    {
        FAXER.PORTAL.DB.FAXEREntities db = null;
        KiiPayPersonalCommonServices _commonServices = null;
        public SSenderPayARequest()
        {
            db = new DB.FAXEREntities();
            _commonServices = new KiiPayPersonalCommonServices();
        }
        public SenderPayARequestViewModel GetPayRequests()
        {
            SenderPayARequestViewModel vm = new SenderPayARequestViewModel()
            {
                StatusList = 0
            };
            //int loggedUserId = Common.FaxerSession.LoggedUser.Id;

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            int senderWalletId = senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id).Id;

            var requestsList = (from c in db.KiiPayPersonalRequestForPayment.Where(x => x.RequestReceiverId == senderWalletId && x.Status != RequestPaymentStatus.Deleted).ToList()
                                select new SenderRequestsList()
                                {
                                    Id = c.Id,
                                    Name = _commonServices.getKiiPayPersonalUserNameFromWalletId(c.RequestSenderId),
                                    WalletNo = _commonServices.getKiiPayPersonalWalletNumberFromWalletId(c.RequestSenderId),
                                    Date = c.RequestedDate.Day.ToString() + "-" + Enum.GetName(typeof(Month), c.RequestedDate.Month).Substring(0, 4) + "-" + c.RequestedDate.Year.ToString(),
                                    Amount = Common.Common.GetCurrencySymbol(c.RequestReceivingCountry) + c.RequestReceivingAmount.ToString(),
                                    PaymentType = c.RequestType,
                                    Status = c.Status,
                                }).ToList();
            vm.RequestsList = requestsList;
            return vm;
        }

        public SenderPayAPaymentRequestViewModel GetValidRequestReceived(int id)
        {
            var paymentRequestData = db.KiiPayPersonalRequestForPayment.Where(x => x.Id == id).FirstOrDefault();
            if (paymentRequestData != null)
            {
                if (paymentRequestData.Status == RequestPaymentStatus.UnPaid)
                {

                    var receiverWalletData = db.KiiPayPersonalWalletInformation.Find(paymentRequestData.RequestSenderId);
                    if (receiverWalletData != null)
                    {
                        decimal exchangeRate = _commonServices.calculateExchangeRate(Common.FaxerSession.LoggedUser.CountryCode, receiverWalletData.CardUserCountry);
                        decimal faxingFeeRate = SEstimateFee.GetFaxingCommision(Common.FaxerSession.LoggedUser.CountryCode);
                        var getSummary = SEstimateFee.CalculateFaxingFee(paymentRequestData.RequestReceivingAmount, true, true, exchangeRate, faxingFeeRate);

                        SenderPayAPaymentRequestViewModel senderPayAPaymentRequest = new SenderPayAPaymentRequestViewModel()
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
                            ReceiverCurrencySymbol = Common.Common.GetCurrencySymbol(receiverWalletData.CardUserCountry),
                            SendingCurrencySymbol = Common.Common.GetCurrencySymbol(Common.FaxerSession.LoggedUser.CountryCode),
                            AvailableBalance = GetAvailableBalance()
                        };

                        SetSenderPayARequestSession(senderPayAPaymentRequest);

                        return senderPayAPaymentRequest;
                    }
                }
               
            }
            return null;
        }

        public decimal GetAvailableBalance()
        {
            int senderWalletId = db.SenderKiiPayPersonalAccount.Where(x => x.SenderId == Common.FaxerSession.LoggedUser.Id && x.KiiPayAccountIsOF == KiiPayAccountIsOF.Sender).Select(x => x.KiiPayPersonalWalletId).FirstOrDefault();
            var userData = db.KiiPayPersonalWalletInformation.Where(x => x.Id == senderWalletId).FirstOrDefault();
            if (userData != null)
            {
                decimal balance = userData.CurrentBalance;
                return balance;
            }
            return 0;
        }
        public bool PayAPaymentRequest()
        {
            var paymentSessionData = GetSenderPayARequestSession();
            var senderUserData = db.FaxerInformation.Where(X => X.Id == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
            if (senderUserData != null)
            {
                int senderWalletId = db.SenderKiiPayPersonalAccount.Where(x => x.SenderId == senderUserData.Id && x.KiiPayAccountIsOF == KiiPayAccountIsOF.Sender).Select(x => x.KiiPayPersonalWalletId).FirstOrDefault();
                var senderWalletData = db.KiiPayPersonalWalletInformation.Where(x=> x.Id   == senderWalletId).FirstOrDefault();
                var receiverWalletData = db.KiiPayPersonalWalletInformation.Find(paymentSessionData.ReceiverWalletId);
                if (senderWalletData != null && receiverWalletData != null)
                {
                    //deducting amount from sender's wallet

                    senderWalletData.CurrentBalance = senderWalletData.CurrentBalance - paymentSessionData.PayingAmount;
                    db.Entry<KiiPayPersonalWalletInformation>(senderWalletData).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    //adding amount to receiver's wallet
                    receiverWalletData.CurrentBalance = receiverWalletData.CurrentBalance + paymentSessionData.ReceivingAmount;
                    db.Entry<KiiPayPersonalWalletInformation>(receiverWalletData).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();


                    //updating transaction table
                    var paymentRequestTableData = db.KiiPayPersonalRequestForPayment.Where(x=> x.Id ==  paymentSessionData.Id).FirstOrDefault();
                    if (paymentRequestTableData != null)
                    {
                        paymentRequestTableData.RequestSendingAmount = paymentSessionData.Amount;
                        paymentRequestTableData.RequestReceivingAmount = paymentSessionData.ReceivingAmount;
                        paymentRequestTableData.TotalAmount = paymentSessionData.PayingAmount;
                        paymentRequestTableData.Fee = paymentSessionData.Fee;
                        paymentRequestTableData.ExchangeRate = paymentSessionData.ExchangeRate;
                        paymentRequestTableData.Status = RequestPaymentStatus.Paid;
                        paymentRequestTableData.PaymentDate = DateTime.Now;
                        db.Entry<KiiPayPersonalRequestForPayment>(paymentRequestTableData).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        return true;
                    }
                }
            }

            return false;
        }


        public void SetSenderPayARequestSession(SenderPayAPaymentRequestViewModel model)
        {
            Common.FaxerSession.SenderPayARequest = model;
        }

        public SenderPayAPaymentRequestViewModel GetSenderPayARequestSession()
        {
            SenderPayAPaymentRequestViewModel vm = new SenderPayAPaymentRequestViewModel();
            if(Common.FaxerSession.SenderPayARequest != null)
            {
                vm = Common.FaxerSession.SenderPayARequest;
            }
            return vm;
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