using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Services
{
    public class RequestAPaymentServices
    {
        FAXEREntities dbContext = null;
        KiiPayPersonalCommonServices _commonServices = null;
        public RequestAPaymentServices()
        {
            _commonServices = new KiiPayPersonalCommonServices();
            dbContext = new FAXEREntities();
        }



        public RequestHistoryViewModel getRequestHistory()
        {
            RequestHistoryViewModel vm = new RequestHistoryViewModel()
            {
                FilterKey = 0
            };
            int kiiPayWalletId = dbContext.KiiPayPersonalUserInformation.Where(x => x.Id == Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId).FirstOrDefault().KiiPayPersonalWalletInformationId;
            var requestsList = (from c in dbContext.KiiPayPersonalRequestForPayment.Where(x => x.RequestReceiverId == kiiPayWalletId && x.IsDeleted == false).ToList()
                                select new RequestHistoryList()
                                {
                                    Id = c.Id,
                                    Name = _commonServices.getKiiPayPersonalUserNameFromWalletId(c.RequestSenderId),
                                    WalletNumber = _commonServices.getKiiPayPersonalWalletNumberFromWalletId(c.RequestSenderId),
                                    Date = c.RequestedDate.Day.ToString() + "-" + Enum.GetName(typeof(Month), c.RequestedDate.Month).Substring(0, 4) + "-" + c.RequestedDate.Year.ToString(),
                                    Amount = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol + c.RequestSendingAmount.ToString(),
                                    Status = c.Status,
                                    isPaid = c.IsPaid
                                }).ToList();
            vm.RequestHistoryList = requestsList;
            return vm;

        }
        public SendRequestEnterAmountViewModel getEnterAmountLocalRequestVM(int id)
        {
            if (id != 0)
            {
                var requestData = dbContext.KiiPayPersonalRequestForPayment.Find(id);
                if (requestData != null)
                {
                    var receiverData = dbContext.KiiPayPersonalWalletInformation.Find(requestData.RequestSenderId);
                    if (receiverData != null)
                    {
                        var result = new SendRequestEnterAmountViewModel()
                        {
                            Id = requestData.Id,
                            PhotoUrl = receiverData.UserPhoto,
                            ReceiverName = receiverData.FirstName + " " + receiverData.MiddleName + " " + receiverData.LastName,
                            CurrencySymbol = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol,
                            CurrencyCode = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrency,
                            Amount = requestData.TotalAmount,
                            Note = requestData.RequestNote
                        };
                        return result;

                    }
                }
            }
            return null;
        }

        public SendRequesEnterAmountAbroadViewModel getEnterAmountInternationalRequestVM(int id)
        {
            if (id != 0)
            {
                var requestData = dbContext.KiiPayPersonalRequestForPayment.Find(id);
                if (requestData != null)
                {
                    var receiverData = dbContext.KiiPayPersonalWalletInformation.Find(requestData.RequestSenderId);
                    if (receiverData != null)
                    {
                        var result = new SendRequesEnterAmountAbroadViewModel()
                        {
                            Id = requestData.Id,
                            PhotoUrl = receiverData.UserPhoto,
                            ReceiversName = receiverData.FirstName + " " + receiverData.MiddleName + " " + receiverData.LastName,
                            LocalAmount = requestData.RequestSendingAmount,
                            ForeignAmount = requestData.RequestReceivingAmount,
                            ExchangeRate = _commonServices.calculateExchangeRate(Common.CardUserSession.LoggedCardUserViewModel.CountryCode, receiverData.CardUserCountry),
                            LocalCurrency = Common.Common.GetCountryCurrency(receiverData.CardUserCountry),
                            ForeignCurrency = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrency,
                            LocalCurrencySymbol = Common.Common.GetCurrencySymbol(receiverData.CardUserCountry),
                            ForeignCurrencySymbol = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol,
                            Note = requestData.RequestNote
                        };
                        return result;
                    }
                }
            }
            return null;
        }

        public bool updateLocalRequestData(SendRequestEnterAmountViewModel model)
        {
            if (model != null)
            {
                var data = dbContext.KiiPayPersonalRequestForPayment.Find(model.Id);
                if (data != null)
                {
                    data.RequestReceivingAmount = model.Amount;
                    data.RequestSendingAmount = model.Amount;
                    data.TotalAmount = model.Amount;
                    data.RequestNote = model.Note;
                    dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
            }
            return false;
        }


        public bool updateInternationalRequestData(SendRequesEnterAmountAbroadViewModel model)
        {
            if (model != null)
            {
                var data = dbContext.KiiPayPersonalRequestForPayment.Find(model.Id);
                if (data != null)
                {
                    data.RequestSendingAmount = model.LocalAmount;
                    data.RequestReceivingAmount = model.ForeignAmount;
                    data.TotalAmount = model.ForeignAmount;
                    data.RequestNote = model.Note;
                    dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;

                }
            }
            return false;
        }

        public void deleteRequest(int id)
        {
            if (id != 0)
            {
                var data = dbContext.KiiPayPersonalRequestForPayment.Find(id);
                if (data != null)
                {
                    data.IsDeleted = true;
                    data.Status = RequestPaymentStatus.Deleted;
                    dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
            }
        }

        public void cancelRequest(int id)
        {
            if (id != 0)
            {
                var data = dbContext.KiiPayPersonalRequestForPayment.Find(id);
                if (data != null)
                {
                    data.Status = RequestPaymentStatus.Cancelled;                    
                    dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                }
            }
        }

        public PaymentType? getRequestType(int id)
        {
            if (id != 0)
            {
                var data = dbContext.KiiPayPersonalRequestForPayment.Find(id);
                if (data != null)
                {
                    return data.RequestType;
                }
            }
            return null;
        }

        public bool isRequestAlreadyPaid(int id)
        {
            if(id != 0)
            {
                var data = dbContext.KiiPayPersonalRequestForPayment.Find(id);
                if(data != null)
                {
                    if(data.IsPaid == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}