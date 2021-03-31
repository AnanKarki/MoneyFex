using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class MFCareerViewModel {

        public const string BindProperty = "JobId,FirstName,LastName,Telephone,Email,Country,City,Position,CVURL,SupportingStatementURL,CareerList";
        public int JobId { get; set; }
        [Required( ErrorMessage ="Enter First Name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage ="Enter Last Name")]
        public string LastName { get; set; }
        [Required(ErrorMessage ="Enter Telephone")]

        public string Telephone { get; set; }
        [Required(ErrorMessage ="Enter Email")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter valid email")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Select Country")]
        public string Country { get; set; }
        [Required(ErrorMessage ="Enter City")]
        public string City { get; set; }
        [Required(ErrorMessage ="Enter Position")]
        public string Position { get; set; }

        public string CVURL { get; set; }

        public string SupportingStatementURL { get; set; }

        public List<MFCareerListViewModel> CareerList { get; set; }

    }
    public class MFCareerListViewModel
    {

        public int Id { get; set; }

        public string JobTitle { get; set; }

        public string JobDescription { get; set; }

        public string Location { get; set; }
        public string ContractType { get; set; }

        public string SalaryRange { get; set; }

        public string ClosingDate { get; set; }
    }
}