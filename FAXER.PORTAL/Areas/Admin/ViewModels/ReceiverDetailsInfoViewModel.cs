using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ReceiverDetailsInfoViewModel
    {
        public const string BindProperty = "Id,BankId,MobileWalletProvider , FirstLetter,ReceiverName ,ReceiverAccountNo , ReceiverPhoneNo,ReceiverCountry ,ReceiverCountryFlag ,ReceiverAddress" +
            " , ReceiverEmail ,ReceivedAmount , Service,ServiceName,BankMobileName,BankCode, DateTime, Identifier ,Blocked , TransactionTransferMethod";
        public int Id { get; set; }

        public string FirstLetter { get; set; }
        [Required(ErrorMessage = "Enter Name")]
        public string ReceiverName { get; set; }
        [Required(ErrorMessage = "Enter Account no")]
        public string ReceiverAccountNo { get; set; }
        [Required(ErrorMessage = "Enter Phone no")]
        public string ReceiverPhoneNo { get; set; }
        public string ReceiverAccountOrPhoneNo
        {
            get
            {
                return (string.IsNullOrEmpty(ReceiverAccountNo) == true ? ReceiverPhoneNo : ReceiverAccountNo);
            }
        }

        [Required(ErrorMessage = "Select Country")]
        public string ReceiverCountry { get; set; }
        public string ReceiverCountryFlag { get; set; }
        public string ReceiverAddress { get; set; }
        public string ReceiverEmail { get; set; }
        public string ReceivedAmount { get; set; }
        public Service Service { get; set; }
        public string ServiceName { get; set; }
        public string BankMobileName { get; set; }
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public string WalletName { get; set; }
        public string BankOrWalletName
        {
            get
            {
                return (string.IsNullOrEmpty(WalletName) == true ? "" : WalletName) + (string.IsNullOrEmpty(BankName) == true ? "" : BankName);
            }
        }

        public DateTime DateTime { get; set; }
        public string Identifier { get; set; }
        public bool Blocked { get; set; }
        public int? BankId { get; set; }
        public int? MobileWalletProvider { get; set; }
        public TransactionTransferMethod TransactionTransferMethod { get; set; }
    }


}