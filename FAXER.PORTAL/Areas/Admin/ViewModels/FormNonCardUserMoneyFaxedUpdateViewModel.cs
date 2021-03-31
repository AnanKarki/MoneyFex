using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class FormNonCardUserFaxerDetailsViewModel
    {
        public int Id { get; set; }
        public int FaxerId { get; set; }
        public string FaxerFirstName { get; set; }
        public string FaxerMiddleName { get; set; }
        public string FaxerLastName { get; set; }
        public string FaxerIDCardNumber { get; set; }
        public string FaxerIDCardType { get; set; }
        public string FaxerIDCardExpDate { get; set; }
        public string FaxerAddress { get; set; }
        public string FaxerCity { get; set; }
        public string FaxerCountryCode { get; set; }
        public string FaxerCountry { get; set; }
        public string FaxerEmailAddress { get; set; }
        public string FaxerTelephone { get; set; }
        public string FaxingStatus { get; set; }

        public FaxingStatus FaxingStatusEnum { get; set; }
    }

    public class FormNonCardUserFaxAmountViewModel
    {
        public int Id { get; set; }
        [Required]
        public decimal FaxingAmount { get; set; }
        public decimal FaxingFee { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal ReceivingAmount { get; set; }
        [Required]
        public string ReceivingCountry { get; set; }
        public string SendingCurrency { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public string ReceivingCurrency { get; set; }
        public string ReceivingCurrencySymbol { get; set; }

    }

    public class FormNonCardUserReceiverDetailsViewModel
    {
        public int Id { get; set; }
        [Required]
        public int ReceiverId { get; set; }

        [Required]
        public string ReceiverFirstName { get; set; }
        public string ReceiverMiddleName { get; set; }
        [Required]
        public string ReceiverLastName { get; set; }
        [Required]
        public string ReceiverAddress { get; set; }
        [Required]
        public string ReceiverCountry { get; set; }
        [Required]
        public string ReceiverCountryCode { get; set; }
        [Required]
        public string ReceiverCity { get; set; }
        [Required]
        public string ReceieverTelephone { get; set; }

    }

    public class FormNonCardUserAdminDetailsViewModel
    {
        public int Id { get; set; }
        [Required]
        public string AgencyName { get; set; }
        [Required]
        public string MFSCode { get; set; }
        [Required]
        public string NameOfUpdater { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }

    }

    public class FormNonCardUserMoneyFaxedUpdateViewModel
    {
        public const string BindProperty = "FormNonCardUserFaxerDetails , FormNonCardUserFaxAmount ,FormNonCardUserReceiverDetails , FormNonCardUserAdminDetails," +
            " CheckConfirmation,FaxingNonCardTransactionId";

        public FormNonCardUserMoneyFaxedUpdateViewModel()
        {
            FormNonCardUserFaxerDetails = new FormNonCardUserFaxerDetailsViewModel();
            FormNonCardUserFaxAmount = new FormNonCardUserFaxAmountViewModel();
            FormNonCardUserReceiverDetails = new FormNonCardUserReceiverDetailsViewModel();
            FormNonCardUserAdminDetails = new FormNonCardUserAdminDetailsViewModel();
        }

      
        public FormNonCardUserFaxerDetailsViewModel FormNonCardUserFaxerDetails { get; set; }
        public FormNonCardUserFaxAmountViewModel FormNonCardUserFaxAmount { get; set; }
        public FormNonCardUserReceiverDetailsViewModel FormNonCardUserReceiverDetails { get; set; }
        public FormNonCardUserAdminDetailsViewModel FormNonCardUserAdminDetails { get; set; }

        [Required]
        public bool CheckConfirmation { get; set; }
        public int FaxingNonCardTransactionId { get; set; }
    }
}