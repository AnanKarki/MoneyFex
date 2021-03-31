using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Services
{
    public class KiiPayBusinessSendAnInvoiceServices
    {
        DB.FAXEREntities dbContext = null;

        public KiiPayBusinessSendAnInvoiceServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<InvoiceMasterListvm> GetInvoiceList()
        {
            var BusinessMobile = Common.BusinessSession.LoggedKiiPayBusinessUserInfo == null ? "" : Common.BusinessSession.LoggedKiiPayBusinessUserInfo.BusinessMobileNo;
            KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();
            var data = dbContext.KiiPayBusinessInvoiceMaster.Where(x => x.SenderWalletNo == BusinessMobile && x.InvoiceStatus != DB.InvoiceStatus.Deleted).ToList();
            var result = (from c in data
                          select new InvoiceMasterListvm()
                          {
                              Id = c.Id,
                              InvoiceStatus = Enum.GetName(typeof(DB.InvoiceStatus), c.InvoiceStatus),
                              InvoiceStatusEnum = c.InvoiceStatus,
                              TotalAmount = c.TotalAmount,
                              CurrencySymbol = getCurrencySymbol(c.ReceiverCountry),
                              InvoiceDate = c.InvoiceDate.Date.Day + "-" + Enum.GetName(typeof(Month), c.InvoiceDate.Month).Substring(0, 3) + "-" + c.InvoiceDate.Year,
                              InvoiceNo = c.InvoiceNo,
                              ReciverName = _kiiPayBusinessCommonServices.GetKiipayBusinessWalletInfo(c.ReceiverId).FirstName + " " + _kiiPayBusinessCommonServices.GetKiipayBusinessWalletInfo(c.ReceiverId).MiddleName + " " + _kiiPayBusinessCommonServices.GetKiipayBusinessWalletInfo(c.ReceiverId).LastName,
                              ReciverWalletNo = c.ReceiverWalletNo,
                          }).ToList();
            return result;

        }

        private string getCurrencySymbol(string receiverCountry)
        {
            string Symbol = Common.Common.GetCurrencySymbol(receiverCountry);
            return Symbol;
        }

        public bool CancelInvoice(int id)
        {
            var data = dbContext.KiiPayBusinessInvoiceMaster.Where(x => x.Id == id).FirstOrDefault();
            if (data != null)
            {
                data.InvoiceStatus = DB.InvoiceStatus.Cancelled;
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
           
            return false;
        }

        public bool DeleteInvoice(int id)
        {
            var data = dbContext.KiiPayBusinessInvoiceMaster.Where(x => x.Id == id).FirstOrDefault();
            if (data != null)
            {
                data.InvoiceStatus = DB.InvoiceStatus.Deleted;
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }

            return false;
        }

        public bool SendInvoice(KiiPayBusinessSendAnInvoicevm vm)
        {
            KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();


            if (vm.InvoiceMaster != null)
            {
                ///Add Master
                DB.KiiPayBusinessInvoiceMaster invoiceMaster = new DB.KiiPayBusinessInvoiceMaster()
                {
                    CCWalletId = _kiiPayBusinessCommonServices.GetBusinessInformationByMobileNo(vm.InvoiceMaster.ToCCInvoiceMobileNumber).Id,
                    CCWalletNo = vm.InvoiceMaster.ToCCInvoiceMobileNumber,
                    CreationDateTime = DateTime.Now,
                    Discount = vm.InvoiceMaster.Discount,
                    DiscountAmount = vm.InvoiceMaster.DiscountAmount,
                    DiscountMethod = vm.InvoiceMaster.DiscountMethodId,
                    InvoiceDate = vm.InvoiceMaster.InvoiceDate,
                    InvoiceNo = vm.InvoiceMaster.InvoiceNo,
                    InvoiceStatus = DB.InvoiceStatus.UnPaid,
                    NoteToReceipent = vm.InvoiceMaster.NoteToReceipient,
                    ReceiverCountry = _kiiPayBusinessCommonServices.GetBusinessInformationByMobileNo(vm.InvoiceMaster.ToInvoiceMobileNumber).Country,
                    ReceiverId = _kiiPayBusinessCommonServices.GetBusinessInformationByMobileNo(vm.InvoiceMaster.ToInvoiceMobileNumber).Id,
                    ReceiverWalletNo = vm.InvoiceMaster.ToInvoiceMobileNumber,
                    SenderCountry = _kiiPayBusinessCommonServices.GetBusinessInformationByMobileNo(Common.BusinessSession.LoggedKiiPayBusinessUserInfo.BusinessMobileNo).Country,
                    SenderId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId,
                    SenderWalletId = _kiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId(Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId).Id,
                    SenderWalletNo = _kiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId(Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId).KiiPayBusinessInformation.BusinessMobileNo,
                    ShippingCost = vm.InvoiceMaster.Shipping,
                    SubTotal = vm.InvoiceMaster.Subtotal,
                    TotalAmount = vm.InvoiceMaster.TotalAmount,
                    ReceiverWalletId = _kiiPayBusinessCommonServices.GetBusinessInformationByMobileNo(vm.InvoiceMaster.ToInvoiceMobileNumber).Id,
                };

             
                var resultMaster = SaveBusinessInvoiceMasterInformation(invoiceMaster);


                var invoiceDetails = (from c in vm.InvoiceDetails
                                      select new DB.KiiPayBusinessInvoiceDetail()
                                      {
                                          KiiPayBusinessInvoiceMasterId = resultMaster.Id,
                                          ItemName = c.ItemName,
                                          Price = c.Price,
                                          Qty = c.Quantity,
                                      }).ToList();

                var resultDetail = SaveBusinessInvoiceDetailsInformation(invoiceDetails);

                #region Notification Section 

                DB.Notification notification = new DB.Notification()
                {
                    SenderId = invoiceMaster.SenderId,
                    ReceiverId = invoiceMaster.ReceiverId,
                    Amount = Common.Common.GetCountryCurrency(invoiceMaster.ReceiverCountry) + " " + invoiceMaster.TotalAmount,
                    CreationDate = DateTime.Now,
                    Title = DB.Title.InvoiceRequest,
                    Message = "Business Mobile No :" + Common.BusinessSession.LoggedKiiPayBusinessUserInfo.BusinessMobileNo,
                    NotificationReceiver = DB.NotificationFor.KiiPayBusiness,
                    NotificationSender = DB.NotificationFor.KiiPayBusiness,
                    Name = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.BusinessName,
                };

                _kiiPayBusinessCommonServices.SendNotification(notification);
                #endregion

                return true;
            }


            return false;
        }
        public bool UpdateInvoice(KiiPayBusinessSendAnInvoicevm vm)
        {
            var model = dbContext.KiiPayBusinessInvoiceMaster.Where(x => x.Id == vm.InvoiceMaster.Id).FirstOrDefault();

            RemoveBusinessInvoiceMasterInformation(model);

            SendInvoice(vm);
            return true;
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
                                        InvoiceDateToString = c.InvoiceDate.ToString("yyyy-MM-dd")
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

        public DB.KiiPayBusinessInvoiceMaster SaveBusinessInvoiceMasterInformation(DB.KiiPayBusinessInvoiceMaster model)
        {
            dbContext.KiiPayBusinessInvoiceMaster.Add(model);
            dbContext.SaveChanges();
            return model;
        }
        public DB.KiiPayBusinessInvoiceMaster RemoveBusinessInvoiceMasterInformation(DB.KiiPayBusinessInvoiceMaster model)
        {
            dbContext.KiiPayBusinessInvoiceMaster.Remove(model);
            dbContext.SaveChanges();
            return model;
        }
        public List<DB.KiiPayBusinessInvoiceDetail> SaveBusinessInvoiceDetailsInformation(List<DB.KiiPayBusinessInvoiceDetail> model)
        {
            dbContext.KiiPayBusinessInvoiceDetail.AddRange(model);
            dbContext.SaveChanges();
            return model;
        }


        public void GetInvoicePaymentSummary( string CountryofPayer , string CountryOfPayee , decimal   Amount) {

            decimal ExchangeRate = SExchangeRate.GetExchangeRateValue(CountryofPayer, CountryOfPayee);
            var PaymentSummary = SEstimateFee.CalculateFaxingFee(Amount,
                                                               false, true, ExchangeRate,
                                                               SEstimateFee.GetFaxingCommision(CountryofPayer));
            
        }
    }
}