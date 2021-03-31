using FAXER.PORTAL.Areas.KiiPayBusiness.Services;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Controllers
{
    public class KiiPayBusinessWalletStatementController : Controller
    {
        KiiPayBusinessWalletStatementServices _kiiPayBusinessWalletStatementServices = null;
        public KiiPayBusinessWalletStatementController()
        {
            _kiiPayBusinessWalletStatementServices = new KiiPayBusinessWalletStatementServices();
        }

        // GET: KiiPayBusiness/KiiPayBusinessWalletStatement
        public ActionResult Index(WalletStatementFilterType? WalletStatementFilterType)
        {

            var result = new List<WalletStatementVM>();
            if (WalletStatementFilterType != null) {

                result = _kiiPayBusinessWalletStatementServices.GetWalletStatement((WalletStatementFilterType)WalletStatementFilterType,Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId);
            }
            return View(result);
        }

        public ActionResult RefundTransaction(int Id , WalletStatementType walletStatementType) {


            switch (walletStatementType)
            {
                case WalletStatementType.KiiPayPersonalIn:
                    _kiiPayBusinessWalletStatementServices.KiiPayPersonalPaymentRefund(Id);
                    break;
                case WalletStatementType.KiiPayPersonalOut:
                    break;
                case WalletStatementType.BusinessPaymentNationalIn:

                    _kiiPayBusinessWalletStatementServices.RefundBusinessNationalPayment(Id);
                    break;
                case WalletStatementType.BusinessPaymentNationalOut:
                    break;
                case WalletStatementType.BusinessPaymentInternationalIn:

                    _kiiPayBusinessWalletStatementServices.RefundBusinessInternationalPayment(Id);
                    break;
                case WalletStatementType.BusinessPaymentInternationalOut:
                    break;
                default:
                    break;
            }
            TempData["Successful"] = 1;
            return View("Index");

        }
        
    }
}