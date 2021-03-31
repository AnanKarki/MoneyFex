using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SSenderWalletStatementServices
    {
        DB.FAXEREntities dbContext = null;
        int LoggedId = Common.FaxerSession.LoggedUser == null ? 0 : Common.FaxerSession.LoggedUser.Id;
        CommonAllServices _SenderCommonServices = null;
        public SSenderWalletStatementServices()
        {
            dbContext = new DB.FAXEREntities();
            _SenderCommonServices = new CommonAllServices();
        }

    }
}