using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class APIProviderSelectionViewModel
    {
        public const string BindProperty = " Id,SendingCountry ,ReceivingCountry , TransferType, TransferTypeName ,TransferMethod , TransferMethodName,FromRange ," +
                                           " ToRange, ApiProviderId, CreatedBy,CreatedDate,ApiProviderName, Range , Apiservice ,ApiserviceName,SendingCurrency,ReceivingCurrency , AgentId , AgentName";
        public int Id { get; set; }
        [Required(ErrorMessage = "Select Sending Country")]
        public string SendingCountry { get; set; }
        [Required(ErrorMessage = "Select Receiving Country")]
        public string ReceivingCountry { get; set; }
        public TransactionTransferType TransferType { get; set; }
        public string TransferTypeName { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public string TransferMethodName { get; set; }
        public decimal FromRange { get; set; }
        public decimal ToRange { get; set; }
        [Required(ErrorMessage = "Select Range")]
        public string Range { get; set; }
        public List<string> RangeList { get; set; }
        [Required(ErrorMessage = "Select Api Provider")]
        public int ApiProviderId { get; set; }
        public string ApiProviderName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Apiservice Apiservice { get; set; }
        public string ApiserviceName { get; set; }

        public string SendingCurrency { get; set; }

        public string ReceivingCurrency { get; set; }
        public int? AgentId { get; set; }
        public string AgentName { get; set; }

    }

}