using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class BankAccount
    {

        public int Id { get; set; }
        public string CountryCode { get; set; }
        public string AccountNo { get; set; }
        public string LabelName { get; set; }
        public string LabelValue { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public int? LastModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public TransferTypeForBankAccount TransferType { get; set; }
    }

    public enum TransferTypeForBankAccount
    {
        [Display(Name = "All")]
        [Description("All")]
        All = 0,
        [Display(Name = "Online")]
        [Description("Online")]
        Online = 1,
        [Display(Name = "Aux Agent")]
        [Description("Aux Agent")]
        AuxAgent = 2,
        [Display(Name = "Select Type")]
        [Description("Select Type")]
        Select = 3,
    }
}