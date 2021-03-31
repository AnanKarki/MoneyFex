using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace FAXER.PORTAL.Models
{
    public class RecentTranferAndRecipientViewModel
    {
        public List<SenderTransactionHistoryList> RecentTransfer { get; set; }
        public List<RecipientsViewModel> Recipients { get; set; }

        public SenderMonthlyTransactionMeterViewModel SenderMonthlyTransaction { get; set; }
    }

    public class RecipientsViewModel
    {
        public const String BindProperty = "Id , SenderId ,Service , ReceiverName, Country, MobileNo ,BankId ,BankName ,AccountNo , BranchCode ," +
            "Reason, MobileWalletProvider,MobileWalletProviderName, IBusiness,ReceiverPostalCode  ,ReceiverStreet ,ReceiverCity , ReceiverEmail ";
        public int Id { get; set; }
        public int SenderId { get; set; }
        public DB.Service Service { get; set; }
        public string ServiceName { get; set; }
        public string ReceiverName { get; set; }
        public string Country { get; set; }
        public string ReceiverCountryLower { get; set; }
        public string ReciverFirstLetter { get; set; }
        public string MobileNo { get; set; }
        public int BankId { get; set; }
        public string BankName { get; set; }
        public string AccountNo { get; set; }
        public string BranchCode { get; set; }
        public ReasonForTransfer Reason { get; set; }
        public int MobileWalletProvider { get; set; }
        public string MobileWalletProviderName { get; set; }
        public bool IBusiness { get; set; }
        public string ReceiverPostalCode { get; set; }
        public string ReceiverStreet { get; set; }
        public string ReceiverCity { get; set; }
        public string ReceiverEmail { get; set; }
        public int IdentityCardId { get; set; }
        public string IdentityCardNumber { get; set; }
    }
}