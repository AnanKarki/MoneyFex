using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class MFBCCardUserDetailsViewModel
    {
        public const string BindProperty = "CardId ,FirstName ,MiddleName , LastName, Gender ,DateOfBirth ";
        public int CardId { get; set; }
        [Required]    
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }

        public Gender Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

    }
}