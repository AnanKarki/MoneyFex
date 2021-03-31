using FAXER.PORTAL.Areas.KiiPayPersonal.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SSenderSendARequest
    {

        FAXER.PORTAL.DB.FAXEREntities db = null;
        KiiPayPersonalCommonServices _commonServices = null;
        public SSenderSendARequest()
        {
            db = new FAXEREntities();
            _commonServices = new KiiPayPersonalCommonServices();
        }

        #region Basic Operations On Entity SenderRequestAPayment

        public KiiPayPersonalRequestForPayment Add(KiiPayPersonalRequestForPayment model)
        {
            db.KiiPayPersonalRequestForPayment.Add(model);
            db.SaveChanges();
            return model;
        }

        public KiiPayPersonalRequestForPayment Update(KiiPayPersonalRequestForPayment model)
        {
            db.Entry<KiiPayPersonalRequestForPayment>(model).State = EntityState.Modified;
            db.SaveChanges();
            return model;
        }

        public KiiPayPersonalRequestForPayment Delete(KiiPayPersonalRequestForPayment model)
        {
            model.Status = RequestPaymentStatus.Deleted;
            db.Entry<KiiPayPersonalRequestForPayment>(model).State = EntityState.Modified;
            db.SaveChanges();
            return model;
        }

        public List<KiiPayPersonalRequestForPayment> List()
        {
            return db.KiiPayPersonalRequestForPayment.ToList();
        }
        #endregion

        public List<DropDownViewModel> GetRecentNumbersInternational()
        {

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            
            int senderWalletId = senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id).Id;
            var result = (from c in db.KiiPayPersonalRequestForPayment.Where(x => x.RequestSenderId == senderWalletId && x.RequestType == PaymentType.International)
                          join d in db.KiiPayPersonalWalletInformation on c.RequestReceiverId equals d.Id
                          select new DropDownViewModel()
                          {
                              Id = d.Id,
                              Code = d.MobileNo,
                              Name = d.MobileNo
                          }).GroupBy(x => x.Code).Select(x => x.FirstOrDefault()).ToList();
            return result;

        }
        public List<DropDownViewModel> GetRecentNumbers()
        {


            //var senderId = db.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).Select(x => x.Id).FirstOrDefault();
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            int senderWalletId = senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id).Id;
            var result = (from c in db.KiiPayPersonalRequestForPayment.Where(x => x.RequestSenderId == senderWalletId && x.RequestType == PaymentType.Local)
                          join d in db.KiiPayPersonalWalletInformation on c.RequestReceiverId equals d.Id
                          select new DropDownViewModel()
                          {
                              Id = d.Id,
                              Code = d.MobileNo,
                              Name = d.MobileNo
                          }).GroupBy(x => x.Code).Select(x => x.FirstOrDefault()).ToList();
            return result;
            //List<DropDownViewModel> list = new List<DropDownViewModel>();
            //list.Add(new DropDownViewModel()
            //{
            //    Id = 1,
            //    Name = "985106789543"
            //});
            //list.Add(new DropDownViewModel()
            //{
            //    Id = 2,
            //    Name = "9841568974"
            //});

            //return list;
        }

        public LoggedUser GetLoggedUserData()
        {

            LoggedUser vm = new LoggedUser();

            if (Common.FaxerSession.LoggedUser != null)
            {

                vm = Common.FaxerSession.LoggedUser;
            }
            return vm;
        }

        public void SetSendARequest(SenderSendARequestVM vm)
        {

            Common.FaxerSession.SenderSendARequest = vm;

        }

        public SenderSendARequestVM GetSendARequest()
        {

            SenderSendARequestVM vm = new SenderSendARequestVM();

            if (Common.FaxerSession.SenderSendARequest != null)
            {

                vm = Common.FaxerSession.SenderSendARequest;
            }
            return vm;
        }

        public void SetSendRequestEnterAmount(SenderMobileEnrterAmountVm vm)
        {

            Common.FaxerSession.SenderMobileEnrterAmount = vm;
        }

        public SenderMobileEnrterAmountVm GetSendRequestEnterAmount()
        {

            SenderMobileEnrterAmountVm vm = new SenderMobileEnrterAmountVm();

            if (Common.FaxerSession.SenderMobileEnrterAmount != null)
            {

                vm = Common.FaxerSession.SenderMobileEnrterAmount;
            }
            return vm;
        }

        public KiiPayPersonalWalletInformation GetReceiverInformationFromMobileNumber(string mobileNumber)
        {
            KiiPayPersonalWalletInformation data = new KiiPayPersonalWalletInformation();
            var receiverWalletData = db.KiiPayPersonalWalletInformation.Where(x => x.MobileNo.Trim() == mobileNumber.Trim()).FirstOrDefault();

            if (receiverWalletData != null)
            {
                data = receiverWalletData;
            }

            return data;

        }


        public void SetSendRequestEnterAmountLocal(SenderLocalEnterAmountVM vm)
        {

            Common.FaxerSession.SenderLocalEnterAmount = vm;

        }

        public SenderLocalEnterAmountVM GetSendRequestEnterAmountLocal()
        {

            SenderLocalEnterAmountVM vm = new SenderLocalEnterAmountVM();

            if (Common.FaxerSession.SenderLocalEnterAmount != null)
            {

                vm = Common.FaxerSession.SenderLocalEnterAmount;
            }
            return vm;
        }


        public bool MakePaymentRequest(PaymentType requestType)
        {
            var sendRequestData = GetSendARequest();
            //var senderWalletData = db.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            var senderWalletData = senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id);

            var receiverWalletData = db.KiiPayPersonalWalletInformation.Where(x => x.MobileNo.Trim() == sendRequestData.MobileNumber.Trim()).FirstOrDefault();

            if (requestType == PaymentType.International)
            {
                var requestingAmountData = GetSendRequestEnterAmount();

                if (senderWalletData != null && receiverWalletData != null)
                {

                    DB.KiiPayPersonalRequestForPayment data = new KiiPayPersonalRequestForPayment()
                    {
                        RequestSenderId = senderWalletData.Id,
                        RequestReceiverId = receiverWalletData.Id,
                        RequestSendingCountry = senderWalletData.CardUserCountry,
                        RequestReceivingCountry = receiverWalletData.CardUserCountry,
                        RequestSendingAmount = requestingAmountData.SendingAmount,
                        RequestReceivingAmount = requestingAmountData.SendingAmount,
                        ExchangeRate = requestingAmountData.ExchangeRate,
                        Fee = requestingAmountData.Fee,
                        TotalAmount = requestingAmountData.TotalAmount,
                        RequestNote = requestingAmountData.PaymentReference,
                        RequestType = requestType,
                        Status = RequestPaymentStatus.UnPaid,
                        RequestedDate = DateTime.Now

                    };
                    //DB.SenderRequestAPayment data = new SenderRequestAPayment()
                    //{
                    //    RequestSenderId = senderWalletData.Id,
                    //    RequestReceiverId = receiverWalletData.Id,
                    //    RequestSendingCountry = senderWalletData.Country,
                    //    RequestReceivingCountry = receiverWalletData.CardUserCountry,
                    //    RequestSendingAmount = requestingAmountData.SendingAmount,
                    //    RequestReceivingAmount = requestingAmountData.SendingAmount,
                    //    ExchangeRate = requestingAmountData.ExchangeRate,
                    //    Fee = requestingAmountData.Fee,
                    //    TotalAmount = requestingAmountData.TotalAmount,
                    //    RequestNote = requestingAmountData.PaymentReference,
                    //    RequestType = requestType,
                    //    Status = RequestPaymentStatus.UnPaid,
                    //    RequestedDate = DateTime.Now
                    //};
                    //db.SenderRequestAPayment.Add(data);
                    db.KiiPayPersonalRequestForPayment.Add(data);
                    db.SaveChanges();
                    return true;

                }

            }

            else
            {
                var requestingAmountData = GetSendRequestEnterAmountLocal();
                if (senderWalletData != null && receiverWalletData != null)
                {
                    //DB.SenderRequestAPayment data = new SenderRequestAPayment()
                    //{
                    //    RequestSenderId = senderWalletData.Id,
                    //    RequestReceiverId = receiverWalletData.Id,
                    //    RequestSendingCountry = senderWalletData.CardUserCountry,
                    //    RequestReceivingCountry = receiverWalletData.CardUserCountry,
                    //    RequestSendingAmount = requestingAmountData.Amount,
                    //    RequestReceivingAmount = requestingAmountData.Amount,
                    //    ExchangeRate = 1,
                    //    Fee = 0,
                    //    TotalAmount = requestingAmountData.Amount,
                    //    RequestNote = requestingAmountData.PaymentReference,
                    //    RequestType = requestType,
                    //    Status = RequestPaymentStatus.UnPaid,
                    //    RequestedDate = DateTime.Now
                    //};

                    DB.KiiPayPersonalRequestForPayment data = new KiiPayPersonalRequestForPayment()
                    {
                        RequestSenderId = senderWalletData.Id,
                        RequestReceiverId = receiverWalletData.Id,
                        RequestSendingCountry = senderWalletData.CardUserCountry,
                        RequestReceivingCountry = receiverWalletData.CardUserCountry,
                        RequestSendingAmount = requestingAmountData.Amount,
                        RequestReceivingAmount = requestingAmountData.Amount,
                        ExchangeRate = 1,
                        Fee = 0,
                        TotalAmount = requestingAmountData.Amount,
                        RequestNote = requestingAmountData.PaymentReference,
                        RequestType = requestType,
                        Status = RequestPaymentStatus.UnPaid,
                        RequestedDate = DateTime.Now,
                        RequestFrom = Module.Faxer,
                        RequestTo = Module.KiiPayPersonal

                    };

                    //db.SenderRequestAPayment.Add(data);
                    db.KiiPayPersonalRequestForPayment.Add(data);
                    db.SaveChanges();
                    return true;

                }
            }

            return false;
        }

        #region Request History

        public Dictionary<int, string> GetPaidStatusList()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            dict.Add(0, "All");
            dict.Add(1, "Paid");
            dict.Add(2, "Unpaid");
            dict.Add(3, "Cancelled");
            return dict;
        }

        public SenderRequestHistoryViewModel GetRequestHistory()
        {
            SenderRequestHistoryViewModel vm = new SenderRequestHistoryViewModel()
            {
                FilterKey = 0
            };
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            int SenderWalletId = senderCommonFunc.GetSenderKiiPayWalletInfo(Common.FaxerSession.LoggedUser.Id).Id;
            var requestsList = (from c in db.KiiPayPersonalRequestForPayment.Where(x => x.RequestSenderId == SenderWalletId && x.Status != RequestPaymentStatus.Deleted).ToList()
                                select new SenderRequestHistoryList()
                                {
                                    Id = c.Id,
                                    Name = _commonServices.getKiiPayPersonalUserNameFromWalletId(c.RequestReceiverId),
                                    WalletNumber = _commonServices.getKiiPayPersonalWalletNumberFromWalletId(c.RequestReceiverId),
                                    Date = c.RequestedDate.Day.ToString() + "-" + Enum.GetName(typeof(Month), c.RequestedDate.Month).Substring(0, 4) + "-" + c.RequestedDate.Year.ToString(),
                                    Amount = Common.Common.GetCurrencySymbol(c.RequestSendingCountry) + c.RequestSendingAmount,
                                    Status = c.Status,
                                }).ToList();
            vm.RequestHistoryList = requestsList;
            return vm;

        }


        public KiiPayPersonalRequestForPayment IsRequestAlreadyPaid(int id)
        {
            if (id != 0)
            {
                var data = db.KiiPayPersonalRequestForPayment.Where(x => x.Id == id).FirstOrDefault();
                if (data != null)
                {
                    if (data.Status != RequestPaymentStatus.Paid)
                    {
                        return data;
                    }
                }
            }
            return null; ;
        }

        public KiiPayPersonalRequestForPayment CancelRequest(KiiPayPersonalRequestForPayment model)
        {
            model.Status = RequestPaymentStatus.Cancelled;
            db.Entry<KiiPayPersonalRequestForPayment>(model).State = EntityState.Modified;
            db.SaveChanges();
            return model;
        }

        public SenderLocalEnterAmountVM GetSendRequestEnterAmountLocalForEdit(int id)
        {
            if (id != 0)
            {
                var requestData = db.KiiPayPersonalRequestForPayment.Find(id);
                if (requestData != null)
                {
                    var receiverData = db.KiiPayPersonalWalletInformation.Find(requestData.RequestReceiverId);
                    if (receiverData != null)
                    {
                        SenderLocalEnterAmountVM result = new SenderLocalEnterAmountVM()
                        {
                            Id = requestData.Id,
                            ReceiverImage = receiverData.UserPhoto,
                            ReceiverName = receiverData.FirstName + " " + receiverData.MiddleName + " " + receiverData.LastName,
                            CurrencySymbol = Common.Common.GetCurrencySymbol(requestData.RequestSendingCountry),
                            CurrencyCode = Common.Common.GetCountryCurrency(requestData.RequestSendingCountry),
                            Amount = requestData.RequestSendingAmount,
                            PaymentReference = requestData.RequestNote
                        };
                        return result;

                    }
                }

            }
            return null;
        }
        public SenderMobileEnrterAmountVm GetSendRequestEnterAmountInternationalForEdit(int id)
        {
            if (id != 0)
            {
                var requestData = db.KiiPayPersonalRequestForPayment.Find(id);
                if (requestData != null)
                {
                    var receiverData = db.KiiPayPersonalWalletInformation.Find(requestData.RequestReceiverId);
                    if (receiverData != null)
                    {
                        SenderMobileEnrterAmountVm result = new SenderMobileEnrterAmountVm()
                        {
                            Id = requestData.Id,
                            ImageUrl = receiverData.UserPhoto,
                            ReceiverName = receiverData.FirstName + " " + receiverData.MiddleName + " " + receiverData.LastName,
                            ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(requestData.RequestReceivingCountry),
                            ReceivingCurrencyCode = Common.Common.GetCountryCurrency(requestData.RequestReceivingCountry),
                            SendingAmount = requestData.RequestSendingAmount,
                            ReceivingAmount = requestData.RequestReceivingAmount,
                            PaymentReference = requestData.RequestNote,
                            TotalAmount = requestData.TotalAmount,
                            ExchangeRate = requestData.ExchangeRate,
                            ReceiverId = requestData.RequestReceiverId,
                            SendingCurrencyCode = Common.Common.GetCountryCurrency(requestData.RequestSendingCountry),
                            SendingCurrencySymbol = Common.Common.GetCurrencySymbol(requestData.RequestSendingCountry),
                            Fee = requestData.Fee,
                        };

                        SetSendRequestEnterAmount(result);
                        return result;

                    }
                }

            }
            return null;
        }
    }
    #endregion
}
