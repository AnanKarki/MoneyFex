using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class RefundsFormRefundBalanceOnDeletedMFBCCardServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();
        RefundsViewDeletedMFBCCardBalanceRefundServices ReceiptService = new RefundsViewDeletedMFBCCardBalanceRefundServices();

        public ViewModels.RefundsFormRefundOnDeletedMFBCCardViewModel getList(string MFBC)
        {
            MFBC = MFBC.Encrypt();
            var result = (from c in dbContext.KiiPayBusinessWalletInformation.Where(x => x.MobileNo == MFBC).ToList()
                          join d in dbContext.DeletedMFBCCards on c.MobileNo equals d.MFBCCardNumber
                          select new ViewModels.RefundsFormRefundOnDeletedMFBCCardViewModel()
                          {
                              KiiPayBusinessInformationId = c.KiiPayBusinessInformation.Id,
                              BusinessName = c.KiiPayBusinessInformation.BusinessName,
                              BusinessLicenseNumber = c.KiiPayBusinessInformation.BusinessLicenseNumber,
                              Address = c.KiiPayBusinessInformation.BusinessOperationAddress1,
                              City = c.KiiPayBusinessInformation.BusinessOperationCity,
                              Country = CommonService.getCountryNameFromCode(c.KiiPayBusinessInformation.BusinessOperationCountryCode),
                              Telephone = c.KiiPayBusinessInformation.PhoneNumber,
                              CurrentDate = DateTime.Now.Date,
                              CurrentTime = DateTime.Now.TimeOfDay,
                              MFBCCardStatus = Enum.GetName(typeof(CardStatus), c.CardStatus),
                              MFBCNameOnCard = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                              EncryptedMFBCNumber = c.MobileNo,
                              MFBCCardNumber = c.MobileNo.Decrypt(),
                              MFBCCreditOnCard = c.CurrentBalance,
                              MFBCAmountBeforeDeletion = c.CurrentBalance,
                              MFBCCurrency = CommonService.getCurrency(c.Country),
                              MFBCCurrencySymbol = CommonService.getCurrencySymbol(c.Country),
                              MFBCDeleter = CommonService.getStaffName(d.DeletedBy),
                              MFBCReasonForDeletion = "",
                              AdminRefunderName = CommonService.getStaffName(Common.StaffSession.LoggedStaff.StaffId),
                              AdminRefunderId = StaffSession.LoggedStaff.StaffId,
                              AdminRefRequestDate = DateTime.Now.Date,
                              AdminRefRequestTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute,0)
                          }).FirstOrDefault();
            return result;
        }

        public bool SaveDeletedMFBCRefund(ViewModels.RefundsFormRefundOnDeletedMFBCCardViewModel model)
        {
            if (model != null)
            {
                RefundOnDeletedMFBCCard data = new RefundOnDeletedMFBCCard()
                {
                    MFBCCardNumber = model.EncryptedMFBCNumber,
                    KiiPayBusinessInformationId = model.KiiPayBusinessInformationId,
                    Amount = model.MFBCAmountBeforeDeletion,
                    DeletionReason = model.MFBCReasonForDeletion,
                    RefundAdmin = Common.StaffSession.LoggedStaff.StaffId,
                    RefundRequestDate = DateTime.Now.Date,
                    RefundRequestTime = DateTime.Now.TimeOfDay,
                    ReceiptNo = ReceiptService.generateReceiptNo()
                };
                dbContext.RefundOnDeletedMFBCCard.Add(data);
                dbContext.SaveChanges();

                //removing amount from mfbccardinformation
                var mfbcinfo = dbContext.KiiPayBusinessWalletInformation.Where(x => x.MobileNo == model.EncryptedMFBCNumber).FirstOrDefault();
                mfbcinfo.CurrentBalance = 0;
                mfbcinfo.CardStatus = CardStatus.IsRefunded;
                dbContext.Entry(mfbcinfo).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();


                //sending mail and receipt
                MailCommon mail = new MailCommon();
                var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

                var deletedInfo = dbContext.DeletedMFBCCards.Where(x => x.MFBCCardNumber == model.EncryptedMFBCNumber).FirstOrDefault();
                string cardCurrency = CommonService.getCurrencyCodeFromCountry(mfbcinfo.Country);
                string cardUserName = mfbcinfo.FirstName + " " + mfbcinfo.MiddleName + " " + mfbcinfo.LastName;
                string refunderMFSCode = CommonService.getStaffMFSCode(model.AdminRefunderId);
                string merchantEmail = dbContext.KiiPayBusinessInformation.Where(x => x.Id == model.KiiPayBusinessInformationId).FirstOrDefault().Email;
                string body = "";
                string BusinessMobileNo = CommonService.getBusinessMobileNo(model.KiiPayBusinessInformationId);
                string mailSubject = "MFBC Deleted Card Balance Refund -"+model.MFBCCardNumber;


                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFBCDeletedCardBalanceRefundEmail?MerchantName=" + model.BusinessName 
                    + "&RefundAmount=" + model.MFBCAmountBeforeDeletion.ToString() + cardCurrency+ "&MerchantAccountNo=" + BusinessMobileNo + "&Deleter=" 
                    + CommonService.getStaffName(deletedInfo.DeletedBy) + "&MFBCNo=" + model.MFBCCardNumber + "&CardUserName=" + model.MFBCNameOnCard 
                    + "&CardUserDOB=" + mfbcinfo.DOB.ToFormatedString() + "&CardUserCountry=" 
                    + CommonService.getCountryNameFromCode(mfbcinfo.Country) + "&CardUserCity=" + mfbcinfo.City);

                string URL = baseUrl + "/EmailTemplate/AdminMFBCCardRefundReciept?ReceiptNumber=" + data.ReceiptNo + "&MerchantAccNo=" + BusinessMobileNo + "&Date=" 
                    + model.AdminRefRequestDate.ToFormatedString() + "&Time=" + model.AdminRefRequestTime + "&MerchantName=" + model.BusinessName + "&MFBCardNumber=" 
                    + model.MFBCCardNumber + "&MerchantCountry=" + model.Country + "&MerchantCity=" + model.City + "&MFBCCardUserName=" + cardUserName 
                    + "&RefundingStaffName=" + model.AdminRefunderName + "&RefundingStaffCode=" + refunderMFSCode + "&RefundedAmount=" + model.MFBCAmountBeforeDeletion.ToString() + " " +  cardCurrency
                    + "&RefundingType=" + Common.StaffSession.LoggedStaff.LoginCode;

                var output = Common.Common.GetPdf(URL);
                mail.SendMail(merchantEmail, mailSubject, body, output);


                return true;
            }
            return false;
        }





        public bool isMFBCDeleted(string MFBC)
        {
            var data = dbContext.KiiPayBusinessWalletInformation.Where(x => x.MobileNo == MFBC).FirstOrDefault();
            if (data.CardStatus == CardStatus.IsDeleted)
            {
                return true;
            }
            return false;
        }

        public decimal getCurrentBalance(string MFBC)
        {
            var data = dbContext.KiiPayBusinessWalletInformation.Where(x => x.MobileNo == MFBC).FirstOrDefault();
            return data.CurrentBalance;
        }
    }
}