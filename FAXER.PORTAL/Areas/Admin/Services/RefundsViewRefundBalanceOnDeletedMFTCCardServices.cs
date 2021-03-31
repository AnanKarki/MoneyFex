using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class RefundsViewRefundBalanceOnDeletedMFTCCardServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();



        public List<RefundBalanceOnDeletedMFTCViewModel> getList(string CountryCode = "", string City = "")
        {
            var data = new List<DB.RefundOnDeletedMFTCCard>();

            if (!string.IsNullOrEmpty(CountryCode) && string.IsNullOrEmpty(City))
            {
                data = dbContext.RefundOnDeletedMFTCCard.Where(x => x.Faxer.Country == CountryCode).ToList();
            }
            else if (string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = dbContext.RefundOnDeletedMFTCCard.Where(x => x.Faxer.City.ToLower() == City.ToLower()).ToList();
            }
            else if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = dbContext.RefundOnDeletedMFTCCard.Where(x => (x.Faxer.City.ToLower() == City.ToLower()) && (x.Faxer.Country == CountryCode)).ToList();
            }


            var result = (from c in data.OrderByDescending(x => x.RefundRequestDate)
                          join d in dbContext.FaxerInformation on c.FaxerId equals d.Id
                          join e in dbContext.DeletedMFTCCards on c.MFTCCardNumber equals e.MoblieNumber
                          join f in dbContext.KiiPayPersonalWalletInformation on c.MFTCCardNumber equals f.MobileNo
                          select new RefundBalanceOnDeletedMFTCViewModel()
                          {
                              Id = c.Id,
                              FaxerId = c.FaxerId,
                              FaxerName = d.FirstName + " " + d.MiddleName + " " + d.LastName,
                              Address = d.Address1,
                              Country = CommonService.getCountryNameFromCode(d.Country),
                              City = d.City,
                              Telephone = CommonService.getPhoneCodeFromCountry(d.Country) + d.PhoneNumber,
                              Email = d.Email,
                              MFTCNumber = c.MFTCCardNumber.Decrypt(),
                              AmountOnMFTC = c.Amount,
                              Currency = CommonService.getCurrencyCodeFromCountry(f.CardUserCountry),
                              MFTCDeletionDate = e.Date.ToFormatedString(),
                              MFTCDeleter = getStaffName(e.DeletedBy),
                              AdminRefunderMFSCode = CommonService.getStaffMFSCode(c.RefundAdmin),
                              AdminRefunder = getStaffName(c.RefundAdmin),
                              RefundConfirm = "",
                              RefundDate = c.RefundRequestDate.ToFormatedString(),
                              RefundTime = c.RefundRequestTime.ToString(@"hh\:mm"),
                              ReasonForRefundRequest = c.DeletionReason,
                              StaffLoginCode = getStaffLoginCode(e.DeletedBy)
                          }).ToList();
            return result;
        }


        public string generateReceiptNo()
        {
            var val = "Ad-MFTC-R-MF" + Common.Common.GenerateRandomDigit(5);

            while (dbContext.RefundOnDeletedMFTCCard.Where(x => x.ReceiptNo == val).Count() > 0)
            {
                val = "Ad-MFTC-R-MF" + Common.Common.GenerateRandomDigit(5);
            }
            return val;
        }

        public string getLastFourDigitsFromSavedCards(int FaxerId)
        {
            var val = dbContext.SavedCard.Where(x => x.UserId == FaxerId).FirstOrDefault();
            if (val != null)
            {
                return val.Num.Decrypt().Right(4);
            }
            return "";
        }

        public bool insertReceiptNo(string receiptNo, int id)
        {
            var data = dbContext.RefundOnDeletedMFTCCard.Where(x => x.Id == id).FirstOrDefault();
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
            return dbContext.RefundOnDeletedMFTCCard.Where(x => x.Id == id).FirstOrDefault().ReceiptNo;
        }

        public string getCardUserName(int id)
        {
            var data = dbContext.RefundOnDeletedMFTCCard.Where(x => x.Id == id).FirstOrDefault();
            var val = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == data.MFTCCardNumber).FirstOrDefault();
            string name = val.FirstName + " " + val.MiddleName + " " + val.LastName;
            return name;
        }
        public string getStaffLoginCode(int id)
        {

            var result = dbContext.StaffLogin.Where(x => x.StaffId == id).Select(x => x.LoginCode).FirstOrDefault();
            return result;
        }



        public string getStaffName(int id)
        {

            var data = dbContext.StaffInformation.Find(id);
            string name = data.FirstName + " " + data.MiddleName + " " + data.LastName;
            return name;

        }
    }
}