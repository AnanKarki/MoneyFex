using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class CreditDebitCardViewModel
    {

        public CreditDebitCardViewModel()
        {
            try
            {
                if(Common.FaxerSession.LoggedUser!=null)
                SetFee(Common.FaxerSession.LoggedUser.CountryCode);

            }
            catch (Exception)
            {

            }
        }
        public CreditDebitCardViewModel(string CountryCode)
        {
            SetFee(CountryCode);
        }
        
        public const string BindProperty = "FaxingAmount,NameOnCard,ReceiverName,CardNumber,EndMM,EndYY" +
            " ,SecurityCode,AddressLineOne,AddressLineTwo, CityName , ZipCode ," +
            " SaveCard , AutoTopUp , AutoTopUpAmount ,PaymentFrequency, CountyName ,FaxingCurrency " +
            ", FaxingCurrencySymbol , PaymentDay, Confirm,StripeTokenID ,CreditDebitCardType,UserImage,ExpiryDate, ThreeDEnrolled  ";

        [Display(Name = "Faxing Amount Including Fee")]
        [Required(ErrorMessage = "Enter Faxing Amount")]
        public decimal FaxingAmount { get; set; }

        [Display(Name = "Name on Card")]

        [Required(ErrorMessage = "Enter Name On Card")]
        public string NameOnCard { get; set; }
        public string ReceiverName { get; set; }

        [Display(Name = "Card Number")]

        [Required(ErrorMessage = "Enter Card Number")]
        public string CardNumber { get; set; }

        [Display(Name = "MM")]

        [Required(ErrorMessage = "Enter Month")]
        public string EndMM { get; set; }
        [Display(Name = "YY")]

        [Required(ErrorMessage = "Enter Year")]
        public string EndYY { get; set; }

        [Display(Name = "Security Code")]

        [Required(ErrorMessage = "Enter Security Code")]
        public string SecurityCode { get; set; }

        [Display(Name = "Address line 1")]
        [Required(ErrorMessage = "Enter Address")]
        public string AddressLineOne { get; set; }


        [Display(Name = "Address Line 2(optional)")]
        public string AddressLineTwo { get; set; }


        [Display(Name = "City")]
        [Required(ErrorMessage = "Enter City")]
        public string CityName { get; set; }


        [Display(Name = "Post/Zip Code")]
        [Required(ErrorMessage = "Enter Zip Code")]
        public string ZipCode { get; set; }

        [Display(Name = "Save this Credit/Debit card for future use")]
        public bool SaveCard { get; set; }
        public bool AutoTopUp { get; set; }
        public decimal AutoTopUpAmount { get; set; }

        public DB.AutoPaymentFrequency PaymentFrequency { get; set; }


        [Display(Name = "Country")]
        [Required(ErrorMessage = "Enter Country")]
        public string CountyName { get; set; }

        [StringLength(200)]
        public string FaxingCurrency { get; set; }

        [StringLength(200)]
        public string FaxingCurrencySymbol { get; set; }

        [StringLength(200)]
        public string PaymentDay { get; set; }
        public bool Confirm { get; set; }

        #region Stripe Portion 

        [StringLength(200)]
        public string StripeTokenID { get; set; }

        #endregion



        [Range(0, int.MaxValue)]
        public CreditDebitCardType CreditDebitCardType { get; set; }


        [StringLength(200)]
        public string UserImage { get; set; }

        [DataType(DataType.Date)]
        public string ExpiryDate { get; set; }

        /// <summary>
        /// Y / N (Yes / No)
        /// </summary>
        public bool ThreeDEnrolled { get; set; }

        public decimal CreditDebitCardFee { get; set; }

        public void SetFee(string countryCode)
        {
            try
            {

                var CustomerPaymentFee = Common.Common.CustomerPaymentFee(countryCode);

                if (CustomerPaymentFee != null)
                {
                    //this.CreditDebitCardFee = 0.05M;
                    this.CreditDebitCardFee = CustomerPaymentFee.CreditCard;
                }
            }
            catch (Exception)
            {
                
            }
            
        }
        public string CardUsageMsg { get; set; }
        public bool IsCardUsageMsg { get; set; }

        public string ErrorMsg { get; set; }
        
        public int TransactionSummaryId { get; set; }



    }

    public enum CreditDebitCardType
    {

        [Display(Name = "Visa")]
        VisaCard,

        [Display(Name = "Mastercard")]
        MasterCard,

        //[Display(Name = "American Express")]
        //AmericanExpress,

        //[Display(Name = "Maestro")]
        //Maestro
    }


}