using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Permissions;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class TransactionAmountLimitViewModel
    {
        public const string BindProperty = "Id, SendingCurrency, ReceivingCurrency,SendingCountry,SendingCountryName , ReceivingCountry ,ReceivingCountryName ," +
            "SenderId ,SenderName, SenderAccountNo ,StaffId,StaffName, StaffAccountNo,ForModule ,Amount";
        public int Id { get; set; }
        [Required(ErrorMessage = "Select Currency")]
        public string SendingCurrency { get; set; }

        [Required(ErrorMessage = "Select Currency")]
        public string ReceivingCurrency { get; set; }

        [Required(ErrorMessage = "Select Country")]
        public string SendingCountry { get; set; }

        public string SendingCountryName { get; set; }

        [Required(ErrorMessage = "Select Country")]
        public string ReceivingCountry { get; set; }
        public string ReceivingCountryName { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderAccountNo { get; set; }

        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public string StaffAccountNo { get; set; }

        public Module ForModule { get; set; }

        [Required(ErrorMessage = "Enter Amount")]
        public decimal Amount { get; set; }
    }
}