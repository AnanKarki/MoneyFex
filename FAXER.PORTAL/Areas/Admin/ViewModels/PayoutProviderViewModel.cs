using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class PayoutProviderViewModel
    {
        public const string BindProperty = "Master , Details ";
        public PayoutProviderMasterViewModel Master { get; set; }
        public List<PayoutProviderDetailsViewModel> Details { get; set; }
    }


    public class PayoutProviderMasterViewModel
    {
        public const string BindProperty = " Id,SendingCountry , ReceivingCountry, TransferMethod,TransferMethodName ,CreatedBy,CreatedDate";
        public int? Id { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string SendingCountry { get; set; }
        public string SendingCountryFlag { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string ReceivingCountry { get; set; }
        public string ReceivingCountryFlag { get; set; }
        [Required(ErrorMessage = "Select Transfer Method")]
        public TransactionTransferMethod TransferMethod { get; set; }
        public string TransferMethodName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        [Required(ErrorMessage = "Enter Name")]
        public string Name { get; set; }
        public string Code { get; set; }
    }
    public class PayoutProviderDetailsViewModel
    {
        public const string BindProperty = " Id ,Name , Code,BranchName ";
        public int Id { get; set; }
        public int PayoutProviderId { get; set; }
        [Required(ErrorMessage = "Enter Name")]
        public string Name { get; set; }
        public string Code { get; set; }
        public string BranchName { get; set; }
    }

}