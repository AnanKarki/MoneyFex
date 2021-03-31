using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class RefundsFormRefundBalanceOnDeletedMFTCCardServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();

        public ViewModels.RefundsFormRefundOnDeletedMFTCCardViewModel getList(string MFTC)
        {

            MFTC = MFTC.Encrypt();
            var result = (from c in dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == MFTC).ToList()
                          join d in dbContext.DeletedMFTCCards on c.MobileNo equals d.MoblieNumber
                          join e in dbContext.SenderKiiPayPersonalAccount on c.Id equals e.KiiPayPersonalWalletId into SenderKiipay
                          from e in SenderKiipay.DefaultIfEmpty()
                          select new ViewModels.RefundsFormRefundOnDeletedMFTCCardViewModel()
                          {
                              FaxerId = e.SenderId,
                              FaxerFirstName = e == null  ? "" :  e.SenderInformation.FirstName,
                              FaxerMiddleName = e == null ? "" : e.SenderInformation.MiddleName,
                              FaxerLastName = e == null ? "" : e.SenderInformation.LastName,
                              FaxerAddress = e == null ? "" : e.SenderInformation.Address1,
                              FaxerCity = e == null ? "" : e.SenderInformation.City,
                              FaxerCountry = CommonService.getCountryNameFromCode(e == null ? "" : e.SenderInformation.Country),
                              
                              FaxerTelephone = e == null ? "" : e.SenderInformation.PhoneNumber,
                              FaxerEmail = e == null ? "" : e.SenderInformation.Email,
                              StatusOfCard = Enum.GetName(typeof(CardStatus), c.CardStatus),
                              MFTCCardNumber = c.MobileNo.Decrypt().FormatMFTCCard(),
                              EncryptedMFTCNumber = c.MobileNo,
                              MFTCAmountBeforeDeletion = c.CurrentBalance,
                              MFTCCurrency = CommonService.getCurrency(c.CardUserCountry),
                              MFTCCurrencySymbol = CommonService.getCurrencySymbol(c.CardUserCountry),
                              MFTCCardDeleter = CommonService.getStaffName(d.DeletedBy),
                              MFTCReasonForDeletion = "",
                              AdminNameOfRefunder = getStaffName(Common.StaffSession.LoggedStaff.StaffId),
                              MFTCCardDeleterId = Common.StaffSession.LoggedStaff.StaffId,
                              AdminRefundRequestDate = DateTime.Now.Date,
                              //AdminRefundRequestTime = TimeSpan.Parse($"{DateTime.Now.TimeOfDay.Hours}:{DateTime.Now.TimeOfDay.Minutes}"),
                              AdminRefundRequestTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0)
        }).FirstOrDefault();
            //TimeSpan currentTime = new TimeSpan(15, 52, 0);
            //currentTime = DateTime.Now.TimeOfDay;
            //result.AdminRefundRequestTime = currentTime;
            return result;
        }

        public bool SaveDeletedMFTCRefund(ViewModels.RefundsFormRefundOnDeletedMFTCCardViewModel model)
        {
            if (model != null)
            {
                RefundOnDeletedMFTCCard data = new RefundOnDeletedMFTCCard()
                {
                    MFTCCardNumber = model.EncryptedMFTCNumber,
                    FaxerId = model.FaxerId,
                    Amount = model.MFTCAmountBeforeDeletion,
                    DeletionReason = model.MFTCReasonForDeletion,
                    RefundAdmin = Common.StaffSession.LoggedStaff.StaffId,
                    RefundRequestDate = DateTime.Now.Date,
                    RefundRequestTime = DateTime.Now.TimeOfDay,
                    ReceiptNo = generateReceiptNo()
                };
                dbContext.RefundOnDeletedMFTCCard.Add(data);
                dbContext.SaveChanges();

                //removing amount from mftccardinformation
                var mftcinfo = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == model.EncryptedMFTCNumber).FirstOrDefault();
                mftcinfo.CurrentBalance = 0;
                mftcinfo.CardStatus = CardStatus.IsRefunded;
                dbContext.Entry(mftcinfo).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                //sending mail and PDF Receipt
                MailCommon mail = new MailCommon();
                var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

                string body = "";
                string faxerName = model.FaxerFirstName + " " + model.FaxerMiddleName + " " + model.FaxerLastName;
                string cardUserName = mftcinfo.FirstName + " " + mftcinfo.MiddleName + " " + mftcinfo.LastName;
                string cardUserCountry = CommonService.getCountryNameFromCode(mftcinfo.CardUserCountry);
                string refundAdminCode = CommonService.getStaffMFSCode(model.MFTCCardDeleterId);
                string topUpMethod = "";
                string cardUserCurrency = CommonService.getCurrencyCodeFromCountry(mftcinfo.CardUserCountry);
                string faxerCurrency = CommonService.getCurrencyCodeFromCountry(mftcinfo.CardUserCountry);
                string mailSubject = "MFTC Deleted Card Balance Refund -" + mftcinfo.MobileNo.Decrypt();

                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFTCDeletedCardBalanceRefundEmail?FaxerName=" + faxerName 
                    + "&RefundAmount=" + model.MFTCAmountBeforeDeletion.ToString() + cardUserCurrency + "&FourDigitOfCard=" + model.EncryptedMFTCNumber.Decrypt().Right(4) 
                    + "&Deleter=" + model.MFTCCardDeleter + "&MFTCNo=" + model.EncryptedMFTCNumber.Decrypt() + "&CardUserName=" + cardUserName + "&CardUserDOB=" 
                    + mftcinfo.CardUserDOB.ToFormatedString() + "&CardUserCountry=" + cardUserCountry + "&CardUserCity=" + mftcinfo.CardUserCity + "&TopUpMethod=" 
                    + topUpMethod + "&OriginalTopUpAmountSendCurrency=" + faxerCurrency);

                string URL = baseUrl + "/EmailTemplate/AdminMFTCCardRefundRecieptController?ReceiptNumber=" + data.ReceiptNo + "&CardLastFourDigits=" 
                    +mftcinfo.MobileNo.Decrypt().Right(4) + "&Date=" + model.AdminRefundRequestDate.ToFormatedString() + "&Time=" + model.AdminRefundRequestTime 
                    + "&MFTCardRegistrar=" + faxerName + "&MFTCardNumber=" + mftcinfo.MobileNo.Decrypt() + "&MFTCardUserName=" + cardUserName 
                    + "&MFTCardUserCountry=" + cardUserCountry + "&MFTCardUserCity=" + mftcinfo.CardUserCity + "&RefundingStaffName=" 
                    + model.AdminNameOfRefunder + "&RefundingStaffCode=" + refundAdminCode + "&RefundedAmount=" + model.MFTCAmountBeforeDeletion + " " +  cardUserCurrency +
                   "&BalanceOnCard=" + model.MFTCAmountBeforeDeletion +  "&RefundingType=" + Common.StaffSession.LoggedStaff.LoginCode;

                
                var output = Common.Common.GetPdf(URL);
                mail.SendMail(model.FaxerEmail, mailSubject, body, output);





                return true;
            }
            return false;
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

        public string getStaffName(int id)
        {

            var data = dbContext.StaffInformation.Find(id);
            string name = data.FirstName + " " + data.MiddleName + " " + data.LastName;
            return name;

        }

        public decimal getCurrentBalance (string MFTC)
        {
            var data = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == MFTC).FirstOrDefault();
            return data.CurrentBalance;
        }

        public bool isMFTCDeleted(string MFTC)
        {
            var data = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == MFTC).FirstOrDefault();
            if (data.CardStatus == CardStatus.IsDeleted)
            {
                return true;
            }
            return false;
        }
    }
}