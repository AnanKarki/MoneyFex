using FAXER.PORTAL.Areas.KiiPayBusiness.Services;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Controllers
{
    public class KiiPayBusinessPayAnInvoiceController : Controller
    {

        KiiPayBusinessPayAnInvoiceServices _kiiPayBusinessPayAnInvoiceService = null;
        public KiiPayBusinessPayAnInvoiceController()
        {
            _kiiPayBusinessPayAnInvoiceService = new KiiPayBusinessPayAnInvoiceServices();
        }
        // GET: KiiPayBusiness/KiiPayBusinessPayAnInvoice
        public ActionResult Index(InvoiceStatus? invoiceStatus)
        {
            List<InvoiceMasterListvm> List = new List<InvoiceMasterListvm>();
            var vm = _kiiPayBusinessPayAnInvoiceService.GetInvoiceList();
            if (invoiceStatus != null)
            {
                vm = vm.Where(x => x.InvoiceStatusEnum == invoiceStatus).ToList();
            }
            return View(vm);
        }

        public ActionResult SeeInvoice(int Id)
        {
            KiiPayBusinessSendAnInvoicevm vm = new KiiPayBusinessSendAnInvoicevm();
            vm = _kiiPayBusinessPayAnInvoiceService.GetInvoiceMasterDetails(Id);
            return View(vm);
        }
        public ActionResult PayInvoice(int Id)
        {
            var data = _kiiPayBusinessPayAnInvoiceService.PayInvoice(Id);
            if (data != null)
            {
                Services.KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();
                KiiPayBusinessWalletStatementServices _kiiPayBusinessWalletStatementServices = new KiiPayBusinessWalletStatementServices();
                KiiPayBusinessWalletStatementVM KiiPayBusinessWalletStatement = new KiiPayBusinessWalletStatementVM()
                {
                    SendingAmount = data.AmountToBePaidByPayer,
                    Fee = data.PayerFee,
                    ReceivingAmount = data.TotalAmount,
                    SenderCurBal = _kiiPayBusinessCommonServices.GetAccountBalanceByWalletId((int)data.SenderWalletId),
                    SenderCountry = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CountryCode,
                    TransactionDate = data.CreationDateTime,
                    WalletStatmentStatus = DB.WalletStatmentStatus.OutBound,
                    TransactionId = data.Id,
                    WalletStatmentType = DB.WalletStatmentType.Invoice,
                };

                _kiiPayBusinessWalletStatementServices.AddOutboundkiiPayBusinessWalletStatementofCreditDebitCard(KiiPayBusinessWalletStatement);

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        public ActionResult PayInvoiceSummary(int Id)
        {

            KiiPayBusinessPayAnInvoiceSummaryvm vm = new KiiPayBusinessPayAnInvoiceSummaryvm();
            vm = _kiiPayBusinessPayAnInvoiceService.GetPayingSummary(Id);
            return View(vm);
        }

    }
}