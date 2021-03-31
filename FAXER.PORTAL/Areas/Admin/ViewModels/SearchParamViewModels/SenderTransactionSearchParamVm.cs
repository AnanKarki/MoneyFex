using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels.SearchParamViewModels
{
    public class SenderTransactionSearchParamVm
    {
        public int TransactionServiceType { get; set; }
        public int senderId { get; set; }
        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string DateRange { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string ReceiverName { get; set; }
        public string searchString { get; set; }
        public string Status { get; set; }
        public string PhoneNumber { get; set; }
        public string SendingCurrency { get; set; }
        public string TransactionWithAndWithoutFee { get; set; }
        public string ResponsiblePerson { get; set; }
        public string SearchByStatus { get; set; }
        public string MFCode { get; set; }
        public int PageNum { get; set; }
        public int PageSize { get; set; }
        public int CurrentpageCount { get; set; }
        public bool IsBusiness { get; set; }
    }
}