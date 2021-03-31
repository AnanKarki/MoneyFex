using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class RefundsViewDeletedMFBCCardBalanceRefundServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();

        public List<RefundBalanceOnDeletedMFBCViewModel> getList(string CountryCode = "", string City = "")
        {
            var data = new List<DB.RefundOnDeletedMFBCCard>();
            if (!string.IsNullOrEmpty(CountryCode) && string.IsNullOrEmpty(City))
            {
                data = dbContext.RefundOnDeletedMFBCCard.Where(x=>x.Business.BusinessOperationCountryCode == CountryCode).ToList();
            }
            else if (string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = dbContext.RefundOnDeletedMFBCCard.Where(x => x.Business.BusinessOperationCity.ToLower() == City.ToLower()).ToList();
            }
            else if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = dbContext.RefundOnDeletedMFBCCard.Where(x => (x.Business.BusinessOperationCity.ToLower() == City.ToLower()) && (x.Business.BusinessOperationCountryCode == CountryCode)).ToList();
            }


            var result = (from c in data.OrderByDescending(x=>x.RefundRequestDate)
                          join d in dbContext.KiiPayBusinessInformation on c.KiiPayBusinessInformationId equals d.Id
                          join e in dbContext.DeletedMFBCCards on c.MFBCCardNumber equals e.MFBCCardNumber
                          join f in dbContext.KiiPayBusinessWalletInformation on c.MFBCCardNumber equals f.MobileNo
                          select new RefundBalanceOnDeletedMFBCViewModel()
                          {
                              Id = c.Id,
                              BusinessName = d.BusinessName,
                              BusinessLicenseNo = d.BusinessLicenseNumber,
                              Address = d.BusinessOperationAddress1,
                              Country = CommonService.getCountryNameFromCode(d.BusinessOperationCountryCode),
                              City = d.BusinessOperationCity,
                              Telephone = CommonService.getPhoneCodeFromCountry(d.BusinessOperationCountryCode) + d.PhoneNumber,
                              Email = d.Email,
                              MFBCNumber = c.MFBCCardNumber.Decrypt(),
                              CreditOnMFBC = f.CurrentBalance,
                              Currency = CommonService.getCurrencyCodeFromCountry(f.Country),
                              MFBCDeletionDate = e.Date.ToFormatedString(),
                              AdminRefunder = CommonService.getStaffName(c.RefundAdmin),
                              AdminRefunderMFSCode = CommonService.getStaffMFSCode(c.RefundAdmin),
                              RefundConfirm = "",
                              RefundDate = c.RefundRequestDate.ToFormatedString(),
                              RefundTime = c.RefundRequestTime.ToString(@"hh\:mm"),
                              ReasonForRefundRequest = c.DeletionReason,
                              StaffLoginCode = dbContext.StaffLogin.Where(x => x.StaffId == e.DeletedBy).Select(x => x.LoginCode).FirstOrDefault()
                          }).ToList();
            return result;
        }


        public string generateReceiptNo()
        {
            var val = "Ad-MFBC-R-MF" + Common.Common.GenerateRandomDigit(5);

            while (dbContext.RefundOnDeletedMFBCCard.Where(x => x.ReceiptNo == val).Count() > 0)
            {
                val = "Ad-MFBC-R-MF" + Common.Common.GenerateRandomDigit(5);
            }
            return val;
        }

        public bool insertReceiptNo(string receiptNo, int id)
        {
            var data = dbContext.RefundOnDeletedMFBCCard.Where(x => x.Id == id).FirstOrDefault();
            if (string.IsNullOrEmpty(data.ReceiptNo))
            {
                data.ReceiptNo = receiptNo;
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                return true;
            }
            return false;
        }

        public string getReceiptNo(int id)
        {
            return dbContext.RefundOnDeletedMFBCCard.Where(x => x.Id == id).FirstOrDefault().ReceiptNo;
        }

        public string getMFBCardUserName(int id)
        {
            var data = dbContext.RefundOnDeletedMFBCCard.Where(x => x.Id == id).FirstOrDefault();
            var val = dbContext.KiiPayBusinessWalletInformation.Where(x => x.MobileNo == data.MFBCCardNumber).FirstOrDefault();
            string name = val.FirstName + " " + val.MiddleName + " " + val.LastName;
            return name;
        }
    }
}