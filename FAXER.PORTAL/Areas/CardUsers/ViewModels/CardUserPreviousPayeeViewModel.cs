using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.ViewModels
{
    public class CardUserPreviousPayeeViewModel
    {
        public const string BindProperty = " BusinessMFCode , BusinessName ,Confirm";

        public string BusinessMFCode { get; set; }


        public string BusinessName { get; set; }
        public bool Confirm { get; set; }
    }
}