using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ServiceSettingViewModel
    {
        public const string BindProperty = " Master ,  Details ,BankDetails";
        public TransferServiceMasterViewModel Master { get; set; }
        public List<TransferServiceDetailsViewModel> Details { get; set; }
        public List<BankViewModel> BankDetails { get; set; }

    }

    public class TransferServiceMasterViewModel
    {

        public const string BindProperty = " Id,  SendingCountry , SendingCountryName,ReceivingCountry" +
                                           ",ReceivingCountryName,CreatedById,CreatedDateTime,SendingCurrrency,ReceivingCurrency,ServiceType";
        public int Id { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string SendingCountry { get; set; }
        public string SendingCountryName { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string ReceivingCountry { get; set; }
        public string ReceivingCountryName { get; set; }
        public int CreatedById { get; set; }
        public DateTime CreatedDateTime { get; set; }

        [Required(ErrorMessage = "Select Currency")]
        public string SendingCurrrency { get; set; }
        [Required(ErrorMessage = "Select Currency")]

        public string ReceivingCurrency { get; set; }

        public int ServiceType { get; set; }
    }

    public class TransferServiceDetailsViewModel
    {
        public const string BindProperty = " Id,  TransferServiceMasterId ,ImageUrl,ServiceType,IsChecked";
        public int Id { get; set; }
        public int TransferServiceMasterId { get; set; }
        public string ImageUrl { get; set; }
        public TransferService ServiceType { get; set; }
        public bool IsChecked { get; set; }

    }


}