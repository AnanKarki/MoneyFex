using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewRegisteredFaxersViewModel
    {
        public const string BindProperty = "Id , FirstName ,MiddleName ,LastName,FullName , DateOfBirth,Gender , GenderName,UsernameEmail ,Password , IDCardImage,IDCardType ,IDCardNumber ,IDCardExpDate" +
            " ,IssuingCountry ,TransactionOver1000 ,Address1 ,Address2 ,City ,State , PostalCode,Country  ,Phone ,CountryCode ,AccountStatus ,MFAccountNo ,LoginFailedCount, Confirm,IsFromBusiness, FaxerNoteList ";

        public int Id { get; set; }
        [Required(ErrorMessage = "Enter First Name")]
        public string FirstName { get; set; }
        public string FirstLetterOfSender
        {
            get
            {
                return FirstName == null ? "" : FirstName.Substring(0, 1).ToLower();
            }
        }

        public string MiddleName { get; set; }
        [Required(ErrorMessage = "Enter Last Name")]
        public string LastName { get; set; }

        public string FullName
        {
            get
            {
                return FirstName + " " + (string.IsNullOrEmpty(MiddleName) == true ? "" : MiddleName + " ") + LastName;
            }
        }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public System.DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage = "Select Gender")]
        public Gender Gender { get; set; }
        public string GenderName { get; set; }
        public int GGender { get; set; }
        public string GGenderName
        {
            get
            {
                if (GGender == 0)
                {
                    return "Male";
                }
                else
                {
                    return "Female";
                }
            }
        }

        [Required(ErrorMessage = "Enter Email")]
        //[RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string UsernameEmail { get; set; }
        public string Password { get; set; }
        public string IDCardImage { get; set; }

        public string IDCardType { get; set; }

        public string IDCardNumber { get; set; }
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime IDCardExpDate { get; set; }

        public string IssuingCountry { get; set; }

        public bool TransactionOver1000 { get; set; }
        [Required(ErrorMessage = "Enter Address 1")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Required(ErrorMessage = "Enter City")]
        public string City { get; set; }
        //[Required]
        public string State { get; set; }
        [Required(ErrorMessage = "Enter Post Code")]
        public string PostalCode { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Enter Phone Number")]
        public string Phone { get; set; }
        public string CountryPhoneCode { get; set; }
        public string CountryCode { get; set; }
        public bool AccountStatus { get; set; }
        public string AccountStatusName { get; set; }
        public string MFAccountNo { get; set; }
        public int LoginFailedCount { get; set; }
        public bool Confirm { get; set; }
        public bool IsFromBusiness { get; set; }
        public int TotalCount { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<FaxerNoteViewModel> FaxerNoteList { get; set; }

    }



    public class FaxerNoteViewModel
    {

        public string Note { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }

        public string StaffName { get; set; }
    }
}