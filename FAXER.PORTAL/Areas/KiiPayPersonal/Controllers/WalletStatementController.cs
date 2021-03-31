using FAXER.PORTAL.Areas.KiiPayPersonal.Services;
using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Controllers
{
    public class WalletStatementController : Controller
    {
        WalletStatementServices _services = null;
        public WalletStatementController()
        {
            _services = new WalletStatementServices();
        }
        // GET: KiiPayPersonal/WalletStatement
        public ActionResult Index(int filterKey = 0)
        {
            var vm = _services.getWalletStatement();
            if(filterKey == 1)
            {
                vm.WalletStatementList = vm.WalletStatementList.Where(x => x.InOut == InOut.In).ToList();
            }
            if(filterKey == 2)
            {
                vm.WalletStatementList = vm.WalletStatementList.Where(x => x.InOut == InOut.Out).ToList();
            }
            vm.Filter = filterKey;
            return View(vm);
        }


        public ActionResult Refund(int id, KiiPayPersonalWalletPaymentType paymentType)
        {
            switch (paymentType)
            {
                case KiiPayPersonalWalletPaymentType.PersonalToPersonal:
                    _services.KiiPayPersonalPaymentRefund(id);
                    break;
                case KiiPayPersonalWalletPaymentType.BusinessToPersonal:
                    break;
                case KiiPayPersonalWalletPaymentType.PersonalToBusinessNational:

                    _services.RefundBusinessNationalPayment(id);
                    break;
                case KiiPayPersonalWalletPaymentType.PersonalToBusinessInternational:
                    break;
                default:
                    break;
            }
            TempData["Successful"] = 1;
            return View("Index");
        }
    }
}