
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class PersonalDetailsViewModel
    {
        public const string BindProperty = "Id ,PhotoUrl ,FirstName , MiddleName , LastName ,BirthDateDay , BirthDateMonth ,BirthDateYear , Gender";
        public int Id { get; set; }
        public string PhotoUrl { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public int BirthDateDay { get; set; }
        [Required]
        public Month BirthDateMonth { get; set; }
        [Required]
        public int BirthDateYear { get; set; }
        public Gender Gender { get; set; }
    }
}