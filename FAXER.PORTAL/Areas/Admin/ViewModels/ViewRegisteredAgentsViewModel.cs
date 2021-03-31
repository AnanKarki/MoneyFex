using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewRegisteredAgentsViewModel
    {
        public const string BindProperty = "Id , Name ,RegistrationNumber ,AgentBusinessLicenseNumber ,Email ,ContactPerson , Address1,Address2 , State,PostalCode ,City ,Country ,CountryCode" +
            ", PhoneNumber,CountryPhoneCode ,FaxNumber , Website , AgentStatus, AccountNo, LoginFailedCount ,ActivationCode , BusinessTypeEnum,     BusinessType , AgentType , AgentTypePrincipal " +
            ", AgentTypeLocal ,Checked , Logincode , AgentNoteList , CurrentCustomerDeposit,CurrentCustomerDepositFees ,CurrentBankDeposit ,TotalDeposit ,NameOfLatestDepositer " +
            ",LatestDepositedDateTime , AgentBankAccountReceipts";

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        //[Required]
        public string RegistrationNumber { get; set; }
        public string AgentBusinessLicenseNumber { get; set; }
        [Required]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter valid email")]
        public string Email { get; set; }
        public string ContactPerson { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string City { get; set; }
        public string Country { get; set; }

        public string CountryCode { get; set; }
        [Required]
        // [RegularExpression(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}", ErrorMessage ="Please enter a Valid Phone Number")]
        public string PhoneNumber { get; set; }
        public string CountryPhoneCode { get; set; }
        public string FaxNumber { get; set; }

        [RegularExpression(@"(http(s)?://)?([\w-]+\.)+[\w-]+(/[\w- ;,./?%&=]*)?", ErrorMessage = "Please enter valid URL")]
        public string Website { get; set; }
        public bool AgentStatus { get; set; }
        public string AccountNo { get; set; }
        


        public int LoginFailedCount { get; set; }


        public string ActivationCode { get; set; }


        public BusinessType BusinessTypeEnum { get; set; }
        public string BusinessType { get; set; }

        public string AgentType { get; set; }

   
        public bool AgentTypePrincipal { get; set; }

        public bool AgentTypeLocal { get; set; }


        [Range(typeof(bool), "true", "true", ErrorMessage = "Please Confirm before updating the data")]
        public bool Checked { get; set; }

        public string Logincode { get; set; }

        public List<AgentNoteViewModel> AgentNoteList { get; set; }

        #region Bank Account Statement details 
        public decimal CurrentCustomerDeposit { get; set; }

        public decimal CurrentCustomerDepositFees { get; set; }
        public decimal CurrentBankDeposit { get; set; }

        public decimal TotalDeposit { get; set; }

        public string NameOfLatestDepositer { get; set; }

        public string LatestDepositedDateTime { get; set; }
        public List<AgentBankAccountReceipt> AgentBankAccountReceipts { get; set; }
        #endregion


        
    }

    public class AgentNoteViewModel
    {

        public string Note { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }

        public string StaffName { get; set; }
    }

    public class AgentBankAccountReceipt {

        public int TransactionId { get; set; }
        public string Date { get; set; }
    }
}