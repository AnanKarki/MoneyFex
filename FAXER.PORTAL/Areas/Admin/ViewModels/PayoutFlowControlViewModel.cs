using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class PayoutFlowControlViewModel
    {
        public const string BindProperty = "Master ,Details ";

        public PayoutFlowControlMasterViewModel Master { get; set; }
        public List<PayoutFlowControlDetailsViewModel> Details { get; set; }

    }

    public class PayoutFlowControlMasterViewModel
    {
        public const string BindProperty = "Id ,SendingCurrency, ReceivingCurrency,TransferMethod,PayoutApi , IsPayoutEnabled,CreatedDate , CreatedBy,CreatedByName  ";

        public int Id { get; set; }
        [Required(ErrorMessage = "Select Currency")]
        public string SendingCurrency { get; set; }
        [Required(ErrorMessage = "Select Currency")]
        public string ReceivingCurrency { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public Apiservice PayoutApi { get; set; }
        public bool IsPayoutEnabled { get; set; }

        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
    }
    public class PayoutFlowControlDetailsViewModel
    {
        public const string BindProperty = "Id ,PayoutProviderId, PayoutProviderName,PayoutFlowControlId";

        public int Id { get; set; }
        public int PayoutProviderId { get; set; }
        public string PayoutProviderName { get; set; }
        public int PayoutFlowControlId { get; set; }

    }
}