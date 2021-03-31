using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Services
{
    public class KiiPayBusinessPayAnInvoiceServices
    {
        KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = null;
        DB.FAXEREntities dbContext = null;
        public KiiPayBusinessPayAnInvoiceServices()
        {
            _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();
            dbContext = new DB.FAXEREntities();
        }

        public List<InvoiceMasterListvm> GetInvoiceList()
        {
            var BusinessMobile = Common.BusinessSession.LoggedKiiPayBusinessUserInfo == null ? "" : Common.BusinessSession.LoggedKiiPayBusinessUserInfo.BusinessMobileNo;
            KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();
            var data = dbContext.KiiPayBusinessInvoiceMaster.Where(x => x.ReceiverWalletNo == BusinessMobile && x.InvoiceStatus != DB.InvoiceStatus.Deleted).ToList();
            var result = (from c in data.ToList()
                          select new InvoiceMasterListvm()
                          {
                              Id = c.Id,
                              InvoiceStatus = Enum.GetName(typeof(DB.InvoiceStatus), c.InvoiceStatus),
                              InvoiceStatusEnum = c.InvoiceStatus,
                              TotalAmount = c.TotalAmount,
                              CurrencySymbol = getCurrencySymbol(c.ReceiverCountry),
                              InvoiceDate = c.InvoiceDate.Date.Day + "-" + Enum.GetName(typeof(Month), c.InvoiceDate.Month).Substring(0, 3) + "-" + c.InvoiceDate.Year,
                              InvoiceNo = c.InvoiceNo,
                              SenderName = GetBusinessFullName(c.SenderId),
                              SenderWalletNo = c.SenderWalletNo,
                              ReciverName = GetBusinessFullName(c.ReceiverId),
                              ReciverWalletNo = c.ReceiverWalletNo,
                          }).ToList();
            return result;

        }

        public string GetBusinessFullName(int Id) {

            var data = _kiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId(Id);
            return data == null ? "" : data.FirstName + " " + data.MiddleName + " " +  data.LastName;
        }

        public KiiPayBusinessInvoiceMaster PayInvoice(int id)
        {
            var data = dbContext.KiiPayBusinessInvoiceMaster.Where(x => x.Id == id).FirstOrDefault();

            var exchangeRate = SExchangeRate.GetExchangeRateValue(data.ReceiverCountry, data.SenderCountry);

            var paymentSummary = SEstimateFee.CalculateFaxingFee(data.TotalAmount, false, true, exchangeRate, SEstimateFee.GetFaxingCommision(data.ReceiverCountry));

            
            if (data != null)
            {
                data.AmountToBePaidByPayer = paymentSummary.ReceivingAmount;
                data.TotalAmountIncludingFee = paymentSummary.TotalAmount;
                data.InvoiceStatus = DB.InvoiceStatus.Paid;
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                // sender is the invoice requester 
                _kiiPayBusinessCommonServices.BalanceIn(data.SenderWalletId, data.AmountToBePaidByPayer);

                // sender is the invoice payer 
                _kiiPayBusinessCommonServices.BalanceOut(data.ReceiverWalletId, data.TotalAmountIncludingFee);



                #region Notification Section 

                DB.Notification notification = new DB.Notification()
                {
                    SenderId = data.ReceiverId,
                    ReceiverId = data.SenderId,
                    Amount = Common.Common.GetCountryCurrency(data.ReceiverCountry) + " " + data.TotalAmount,
                    CreationDate = DateTime.Now,
                    Title = DB.Title.InvoiceRequest,
                    Message = "Business Mobile No :" + Common.BusinessSession.LoggedKiiPayBusinessUserInfo.BusinessMobileNo,
                    NotificationReceiver = DB.NotificationFor.KiiPayBusiness,
                    NotificationSender = DB.NotificationFor.KiiPayBusiness,
                    Name = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.BusinessName,
                };

                _kiiPayBusinessCommonServices.SendNotification(notification);
                #endregion


                return data;
            }

            return null;
        }

        public KiiPayBusinessPayAnInvoiceSummaryvm GetPayingSummary(int id)
        {
            KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();
            var data = dbContext.KiiPayBusinessInvoiceMaster.Where(x => x.Id == id).FirstOrDefault();
            var exchangeRate = SExchangeRate.GetExchangeRateValue(data.ReceiverCountry, data.SenderCountry);

            var paymentSummary = SEstimateFee.CalculateFaxingFee(data.TotalAmount, false, true, exchangeRate, SEstimateFee.GetFaxingCommision(data.ReceiverCountry));


            var result =  new KiiPayBusinessPayAnInvoiceSummaryvm()
                          {
                              Id = data.Id,
                              BusinessName = _kiiPayBusinessCommonServices.GetKiipayBusinessWalletInfo(data.SenderWalletId).KiiPayBusinessInformation.BusinessName,
                              Amount = paymentSummary.FaxingAmount,
                              Fee = paymentSummary.FaxingFee,
                              InvoiceNo = data.InvoiceNo,
                              TotalAmount = paymentSummary.TotalAmount,
                          };
            return result;

        }

    

        public KiiPayBusinessSendAnInvoicevm GetInvoiceMasterDetails(int id)
        {


            KiiPayBusinessSendAnInvoicevm vm = new KiiPayBusinessSendAnInvoicevm();
            var master = dbContext.KiiPayBusinessInvoiceMaster.Where(x => x.Id == id && x.InvoiceStatus != DB.InvoiceStatus.Deleted).ToList();
            if (master.Count() > 0)
            {
                vm.InvoiceMaster = (from c in master.ToList()
                                    select new InvoiceMastervm()
                                    {
                                        AmountDue = 0,
                                        CountryCode = "",
                                        Discount = c.Discount,
                                        DiscountAmount = c.DiscountAmount,
                                        DiscountMethodId = c.DiscountMethod,
                                        InvoiceDate = c.InvoiceDate,
                                        InvoiceNo = c.InvoiceNo,
                                        NoteToReceipient = c.NoteToReceipent,
                                        Shipping = c.ShippingCost,
                                        Subtotal = c.SubTotal,
                                        ToCCInvoiceMobileNumber = c.CCWalletNo,
                                        ToInvoiceMobileNumber = c.ReceiverWalletNo,
                                        TotalAmount = c.TotalAmount,
                                        FromBusinessName = _kiiPayBusinessCommonServices.GetKiipayBusinessWalletInfo(c.SenderWalletId).KiiPayBusinessInformation.BusinessName,
                                        FromInvoiceMobileNumber = _kiiPayBusinessCommonServices.GetKiipayBusinessWalletInfo(c.SenderWalletId).KiiPayBusinessInformation.BusinessMobileNo,
                                        Id = c.Id,

                                    }).FirstOrDefault();


                vm.InvoiceDetails = (from c in dbContext.KiiPayBusinessInvoiceDetail.Where(x => x.KiiPayBusinessInvoiceMasterId == id).ToList()
                                     select new InvoiceDetailsvm()
                                     {
                                         Amount = c.Qty * c.Price,
                                         CurrencySymbol = "",
                                         InvoiceMasterId = c.KiiPayBusinessInvoiceMasterId,
                                         Price = c.Price,
                                         Quantity = c.Qty,
                                         ItemName = c.ItemName,
                                     }).ToList();
                return vm;


            }
            return null;

        }
        private string getCurrencySymbol(string receiverCountry)
        {
            string Symbol = Common.Common.GetCurrencySymbol(receiverCountry);
            return Symbol;
        }
    }
}