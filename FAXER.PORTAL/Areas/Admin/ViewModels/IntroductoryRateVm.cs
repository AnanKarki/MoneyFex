using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class IntroductoryRateVm
    {
        public const string BindProperty = "Id , SendingCountry ,ReceivingCountry ,TransactionTransferType , TransactionTransferMethod,AgentName ,AgentId , Range,FromRange" +
            " ,ToRange ,Rate , NoOfTransaction, CreatedDate ,CreatedBy";

        public int? Id { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string SendingCountry { get; set; }

        [Required(ErrorMessage = "Select Country")]
        public string ReceivingCountry { get; set; }
        public TransactionTransferType TransactionTransferType { get; set; }
        public TransactionTransferMethod TransactionTransferMethod { get; set; }

        public string AgentName { get; set; }

        public int? AgentId { get; set; }
        public string Range { get; set; }
        public decimal FromRange { get; set; }
        public decimal ToRange { get; set; }
        public decimal Rate { get; set; }

        /// <summary>
        /// No of Transaction for the rate
        /// </summary>
        public int NoOfTransaction { get; set; }

        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
    }

    public class IntroductoryRateListVm
    {

        public int Id { get; set; }
        public string AgentName { get; set; }
        public string SendingCountry { get; set; }
        public string SendingCountryFlag { get; set; }
        public string ReceivingCountry { get; set; }
        public string ReceivingCountryFlag { get; set; }
        public decimal Rate { get; set; }
        public string TransferMethod { get; set; }
        public string Range { get; set; }
        public string CreationDate { get; set; }
        public List<string> RangeList { get; set; }


    }
}