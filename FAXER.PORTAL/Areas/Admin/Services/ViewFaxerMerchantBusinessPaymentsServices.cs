using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewFaxerMerchantBusinessPaymentsServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();


        public List<ViewFaxerMerchantBusinessPaymentsViewModel> getDetails(string CountryCode = "", string City = "")
        {
            var data = new List<DB.SenderKiiPayBusinessPaymentTransaction>();

            if (!string.IsNullOrEmpty(CountryCode) && string.IsNullOrEmpty(City))
            {
                data = dbContext.FaxerMerchantPaymentTransaction.Where(x => x.SenderKiiPayBusinessPaymentInformation.SenderInformation.Country == CountryCode).ToList();
            }
            else if (string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = dbContext.FaxerMerchantPaymentTransaction.Where(x => x.SenderKiiPayBusinessPaymentInformation.SenderInformation.City.ToLower() == City.ToLower()).ToList();
            }
            else if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = dbContext.FaxerMerchantPaymentTransaction.Where(x => (x.SenderKiiPayBusinessPaymentInformation.SenderInformation.City.ToLower() == City.ToLower()) && (x.SenderKiiPayBusinessPaymentInformation.SenderInformation.Country == CountryCode)).ToList();
            }



            var result = (from c in data.OrderByDescending(x => x.PaymentDate)
                          join d in dbContext.FaxerMerchantPaymentInformation on c.SenderKiiPayBusinessPaymentInformationId equals d.Id into joinedT
                          from joined in joinedT.DefaultIfEmpty()
                          select new ViewFaxerMerchantBusinessPaymentsViewModel()
                          {
                              Id = c.Id,
                              FaxerName = c.SenderKiiPayBusinessPaymentInformation.SenderInformation.FirstName + " " + c.SenderKiiPayBusinessPaymentInformation.SenderInformation.MiddleName + " " + c.SenderKiiPayBusinessPaymentInformation.SenderInformation.LastName,
                              FaxerAddress = c.SenderKiiPayBusinessPaymentInformation.SenderInformation.Address1,
                              FaxerCountry = CommonService.getCountryNameFromCode(c.SenderKiiPayBusinessPaymentInformation.SenderInformation.Country),
                              FaxerCity = c.SenderKiiPayBusinessPaymentInformation.SenderInformation.City,
                              MerchantName = c.SenderKiiPayBusinessPaymentInformation.KiiPayBusinessInformation.BusinessName,
                              MerchantAccountNo = c.SenderKiiPayBusinessPaymentInformation.KiiPayBusinessInformation.BusinessMobileNo,
                              MerchantCountry = CommonService.getCountryNameFromCode(c.SenderKiiPayBusinessPaymentInformation.KiiPayBusinessInformation.BusinessOperationCountryCode),
                              MerchantCity = c.SenderKiiPayBusinessPaymentInformation.KiiPayBusinessInformation.BusinessOperationCity,
                              PaymentAmount = c.PaymentAmount,
                              PaymentCurrency = CommonService.getCurrencyCodeFromCountry(c.SenderKiiPayBusinessPaymentInformation.SenderInformation.Country),
                              PaymentReference = c.PaymentReference,
                              Date = c.PaymentDate.ToFormatedString(),
                              Time = c.PaymentDate.ToString("HH:mm"),
                              PaymentByAdmin = c.StaffId == null ? "No" : "Yes",
                              AdminPayer = c.StaffId == null ? "" : CommonService.getStaffName(c.StaffId ?? default(int)),
                              AutoPaymentEnable = c.SenderKiiPayBusinessPaymentInformation.EnableAutoPayment == true ? "Yes" : "No",
                              AutoPaymentAmount = c.SenderKiiPayBusinessPaymentInformation.AutoPaymentAmount,
                              FrequencyOfAutoPay = Enum.GetName(typeof(AutoPaymentFrequency), c.SenderKiiPayBusinessPaymentInformation.AutoPaymentFrequency)
                          }).ToList();
            return result;
        }

        public AdminPayGoodsAndServicesReceiptViewModel getReceiptInfo(int id)
        {
            var result = (from c in dbContext.FaxerMerchantPaymentTransaction.Where(x => x.Id == id).ToList()
                          join d in dbContext.FaxerMerchantPaymentInformation on c.SenderKiiPayBusinessPaymentInformationId equals d.Id into joinedT
                          from joined in joinedT.DefaultIfEmpty()
                          select new AdminPayGoodsAndServicesReceiptViewModel()
                          {
                              ReceiptNumber = c.ReceiptNumber,
                              Date = c.PaymentDate.ToFormatedString(),
                              Time = c.PaymentDate.ToString("HH:mm"),
                              FaxerFullName = c.SenderKiiPayBusinessPaymentInformation.SenderInformation.FirstName + c.SenderKiiPayBusinessPaymentInformation.SenderInformation.MiddleName + c.SenderKiiPayBusinessPaymentInformation.SenderInformation.LastName,
                              BusinessMerchantName = c.SenderKiiPayBusinessPaymentInformation.KiiPayBusinessInformation.BusinessName,
                              BusinessMFCode = c.SenderKiiPayBusinessPaymentInformation.KiiPayBusinessInformation.BusinessMobileNo,
                              StaffName = c.StaffId == null ? "" : CommonService.getStaffName(c.StaffId ?? default(int)),
                              StaffCode = c.StaffId == null ? "" : CommonService.getStaffMFSCode(c.StaffId ?? default(int)),
                              AmountPaid = c.PaymentAmount.ToString(),
                              ExchangeRate = c.ExchangeRate.ToString(),
                              AmountInLocalCurrency = c.ReceivingAmount.ToString(),
                              SendingCurrency = CommonService.getFaxerCurrencyFromId(joined.SenderInformationId),
                              ReceivingCurrency = CommonService.getBusinessCurrencyFromId(joined.KiiPayBusinessInformationId),
                              Fee = c.FaxingFee.ToString(),
                              StaffLoginCode = c.StaffId == null ? "" : CommonService.getStaffLoginCode(c.StaffId ?? default(int)),
                              FaxerCountry = Common.Common.GetCountryName(c.SenderKiiPayBusinessPaymentInformation.SenderInformation.Country),
                              BusinessCountry = Common.Common.GetCountryName(c.SenderKiiPayBusinessPaymentInformation.KiiPayBusinessInformation.BusinessOperationCountryCode),
                              BusinessCity = c.SenderKiiPayBusinessPaymentInformation.KiiPayBusinessInformation.BusinessOperationCity,
                          }).FirstOrDefault();
            return result;
        }
    }
}