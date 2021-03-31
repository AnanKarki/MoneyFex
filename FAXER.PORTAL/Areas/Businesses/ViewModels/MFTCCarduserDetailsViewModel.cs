using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class MFTCCarduserDetailsViewModel
    {
        public const string BindProperty = "Id ,  CardUserName , MFTCCardNo , CardUserCountry , CarduserCity ,  Phone , Email , Confirm";
        public int Id { get; set; }
        public string CardUserName { get; set; }
        public string  MFTCCardNo { get; set; }
        public string CardUserCountry { get; set; }
        public string CarduserCity { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool Confirm { get; set; }

    }
}