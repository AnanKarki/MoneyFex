using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class PaySomeoneElseCardByAdminViewModel
    {
        public const string BindProperty = "SenderId  ,SenderAccountNo ,SenderFirstName ,SenderMiddleName , SenderLastName,SenderIdCardNumber , SenderIdCardType,SenderIdCardExpDate , SenderIdCardIssuingCountry," +
            " , SenderAddress,SenderCity , SenderCountry,SenderCountryCode , SenderCurrency,SenderCurrencySymbol ,SenderTelephone , SenderEmail,InvalidSenderDetails ,CardUserId ,CardUserFirstName ,CardUserMiddleName ,CardUserLastName" +
            " , CardUserAddress , CardUserCountry,CardUserCountryCode , CardUserCurrency, CardUserCurrencySymbol,CardUserCity ,CardUserEmail, ,CardUserPhotoURL , CardUserTelephone, , InvalidCardUserDetails,ErrorMessage , TopUpAmount,TopUpFee" +
            " , AmountIncludingFee, CurrentExchangeRate, ReceivingAmount, IncludingFee ,TopUpReference ,CardNumberDropDown ,IsCardAvailabled ,NameOnCard ,CardNumber ,CardEndMonth ,CardEndYear ,CardSecurityNo" +
            " , BillingAddress1 ,BillingAddress2 ,BillingCity ,BillingPostalCode , BillingCountry,AcceptTerms , PayingAdminName, bankToBankPayment";

        #region Sender Information 

        public int SenderId { get; set; }

        public string SenderAccountNo { get; set; }


        [Required]
        public string SenderFirstName { get; set; }
        public string SenderMiddleName { get; set; }

        [Required]
        public string SenderLastName { get; set; }

        public string SenderIdCardNumber { get; set; }

        public string SenderIdCardType { get; set; }

        public string SenderIdCardExpDate { get; set; }

        public string SenderIdCardIssuingCountry { get; set; }

        public string SenderAddress { get; set; }

        public string SenderCity { get; set; }

        public string SenderCountry { get; set; }

        public string SenderCountryCode { get; set; }

        public string SenderCurrency { get; set; }
        public string SenderCurrencySymbol { get; set; }

        public string SenderTelephone { get; set; }

        public string SenderEmail { get; set; }

        public bool InvalidSenderDetails { get; set; }

        #endregion

        #region MFTC Card User Details

        public int CardUserId { get; set; }

        public string CardUserFirstName { get; set; }

        public string CardUserMiddleName { get; set; }

        public string CardUserLastName { get; set; }

        public string CardUserAddress { get; set; }

        public string CardUserCountry { get; set; }


        public string CardUserCountryCode { get; set; }

        public string CardUserCurrency { get; set; }

        public string CardUserCurrencySymbol { get; set; }


        public string CardUserCity { get; set; }

        public string CardUserEmail { get; set; }


        public string CardUserPhotoURL { get; set; }

        public string CardUserTelephone { get; set; }
        public bool InvalidCardUserDetails { get; set; }

        public string ErrorMessage { get; set; }


        #endregion

        #region Transaction Details
        public decimal TopUpAmount { get; set; }
        public decimal TopUpFee { get; set; }
        public decimal AmountIncludingFee { get; set; }
        public decimal CurrentExchangeRate { get; set; }
        public decimal ReceivingAmount { get; set; }
        public bool IncludingFee { get; set; }

        public string TopUpReference { get; set; }



        #endregion


        #region Credit/Debit card details 
        public string CardNumberDropDown { get; set; }
        public bool IsCardAvailabled { get; set; }
        [Required]
        public string NameOnCard { get; set; }
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public string CardEndMonth { get; set; }
        [Required]
        public string CardEndYear { get; set; }
        [Required]
        public string CardSecurityNo { get; set; }

        [Required]
        public string BillingAddress1 { get; set; }

        public string BillingAddress2 { get; set; }

        [Required]
        public string BillingCity { get; set; }

        [Required]
        public string BillingPostalCode { get; set; }
        [Required]
        public string BillingCountry { get; set; }
        public bool AcceptTerms { get; set; }

        #endregion

        public string PayingAdminName { get; set; }
        public bool bankToBankPayment { get; set; }


    }
    public class SavedDropDownVM
    {



        public string CardName { get; set; }

        public string CardNumMasked { get; set; }

        public string CardNum { get; set; }



    }
}