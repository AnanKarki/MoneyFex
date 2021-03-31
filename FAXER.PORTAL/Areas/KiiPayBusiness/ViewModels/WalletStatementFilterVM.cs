using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class WalletStatementFilterVM
    {


        public WalletStatementFilterType WalletStatementFilterType { get; set; }

    }

    public enum WalletStatementFilterType {

        All ,
        In ,
        Out

    }
}