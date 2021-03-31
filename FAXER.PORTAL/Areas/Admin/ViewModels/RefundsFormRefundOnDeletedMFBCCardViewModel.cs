using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class RefundsFormRefundOnDeletedMFBCCardViewModel
    {
        public const string BindProperty = "Id , KiiPayBusinessInformationId ,BusinessName , BusinessLicenseNumber,Address,City,Country ,Telephone ,CurrentDate ,CurrentTime , MFBCCardStatus,MFBCNameOnCard" +
            " , EncryptedMFBCNumber,MFBCCardNumber , MFBCCreditOnCard,MFBCAmountBeforeDeletion , MFBCCurrency, MFBCCurrencySymbol, MFBCDeleter,MFBCReasonForDeletion ,AdminRefunderName , AdminRefunderId,AdminRefRequestDate" +
            " , AdminRefRequestTime , ConfirmRefund";

        public int Id { get; set; }
        public int KiiPayBusinessInformationId { get; set; }
        public string BusinessName { get; set; }
        public string BusinessLicenseNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Telephone { get; set; }
        public DateTime CurrentDate { get; set; }
        public TimeSpan CurrentTime { get; set; }
        public string MFBCCardStatus { get; set; }
        public string MFBCNameOnCard { get; set; }
        public string EncryptedMFBCNumber { get; set; }
        public string MFBCCardNumber { get; set; }
        public decimal MFBCCreditOnCard { get; set; }
        public decimal MFBCAmountBeforeDeletion { get; set; }
        public string MFBCCurrency { get; set; }
        public string MFBCCurrencySymbol { get; set; }
        public string MFBCDeleter { get; set; }
        public string MFBCReasonForDeletion { get; set; }
        public string AdminRefunderName { get; set; }
        public int AdminRefunderId { get; set; }
        public DateTime AdminRefRequestDate { get; set; }
        public TimeSpan AdminRefRequestTime { get; set; }
        public bool ConfirmRefund { get; set; }
    }
}