using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class RefundsFormRefundOnDeletedMFTCCardViewModel
    {
        public const string BindProperty = "Id ,FaxerId , FaxerFirstName,FaxerMiddleName ,FaxerLastName , FaxerAddress,FaxerCity, FaxerCountry,FaxerTelephone ,FaxerEmail , StatusOfCard, MFTCCardNumber" +
            ",EncryptedMFTCNumber ,MFTCAmountBeforeDeletion ,MFTCCurrency, MFTCCurrencySymbol, MFTCCardDeleter ,MFTCCardDeleterId , MFTCReasonForDeletion,AdminNameOfRefunder , AdminRefundRequestDate " +
            ", AdminRefundRequestTime,ConfirmRefundRequest ";
        public int Id { get; set; }
        public int FaxerId { get; set; }
        public string FaxerFirstName { get; set; }
        public string FaxerMiddleName { get; set; }
        public string FaxerLastName { get; set; }
        public string FaxerAddress { get; set; }
        public string FaxerCity { get; set; }
        public string FaxerCountry { get; set; }
        public string FaxerTelephone { get; set; }
        public string FaxerEmail { get; set; }
        public string StatusOfCard { get; set; }
        public string MFTCCardNumber { get; set; }
        public string EncryptedMFTCNumber { get; set; }
        public decimal MFTCAmountBeforeDeletion { get; set; }
        public string MFTCCurrency { get; set; }
        public string MFTCCurrencySymbol { get; set; }
        public string MFTCCardDeleter { get; set; }
        public int MFTCCardDeleterId { get; set; }
        public string MFTCReasonForDeletion { get; set; }
        public string AdminNameOfRefunder { get; set; }
        public DateTime AdminRefundRequestDate { get; set; }
        public TimeSpan AdminRefundRequestTime { get; set; }
        public bool ConfirmRefundRequest { get; set; }
    }
}