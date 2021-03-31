using FAXER.PORTAL.Areas.KiiPayBusiness.Services;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Controllers
{
    public class KiiPayBusinessSendAnInvoiceController : Controller
    {
        KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = null;
        KiiPayBusinessSendAnInvoiceServices _kiiPayBusinessSendAnInvoiceServices = null;
        public KiiPayBusinessSendAnInvoiceController()
        {
            _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();
            _kiiPayBusinessSendAnInvoiceServices = new KiiPayBusinessSendAnInvoiceServices();
        }
        // GET: KiiPayBusiness/KiiPayBusinessSendAnInvoice
        public ActionResult Index(InvoiceStatus? invoiceStatus)
        {
            List<InvoiceMasterListvm> List = new List<InvoiceMasterListvm>();
            var vm = _kiiPayBusinessSendAnInvoiceServices.GetInvoiceList();
            if (invoiceStatus != null)
            {
                vm = vm.Where(x => x.InvoiceStatusEnum == invoiceStatus).ToList();
            }
            return View(vm);
        }
        public ActionResult Create(int Id = 0)
        {
            var BusinessInfo = _kiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId
                                (Common.BusinessSession.LoggedKiiPayBusinessUserInfo
                                .KiiPayBusinessInformationId);
            KiiPayBusinessSendAnInvoicevm vm = new KiiPayBusinessSendAnInvoicevm();
            vm.InvoiceMaster = new InvoiceMastervm();
            vm.InvoiceDetails = new List<InvoiceDetailsvm>();
            vm.InvoiceMaster.FromBusinessName = BusinessInfo.KiiPayBusinessInformation.BusinessName;
            vm.InvoiceMaster.FromInvoiceMobileNumber = BusinessInfo.KiiPayBusinessInformation.BusinessMobileNo;
            ViewBag.Id = Id;

            return View(vm);
        }



        public JsonResult IsValidMobileNo(string mobileNo)
        {

            var result = _kiiPayBusinessCommonServices.IsValidMobileNo(mobileNo);
            return Json(new
            {
                Data = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Remind(int Id = 0)
        {
            return RedirectToAction("Index");
        }
        public ActionResult Cancel(int Id = 0)
        {
            bool CancelSuccess = _kiiPayBusinessSendAnInvoiceServices.CancelInvoice(Id);
            if (CancelSuccess)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int Id = 0)
        {
            bool DeleteSuccess = _kiiPayBusinessSendAnInvoiceServices.DeleteInvoice(Id);
            if (DeleteSuccess)
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public JsonResult SendInvoice([Bind(Include = KiiPayBusinessSendAnInvoicevm.BindProperty)]KiiPayBusinessSendAnInvoicevm vm)
        {

            bool SaveSucess = _kiiPayBusinessSendAnInvoiceServices.SendInvoice(vm);

            return Json(new
            {
                Amount = vm.InvoiceMaster.TotalAmount,
                ReceiverName = _kiiPayBusinessCommonServices.GetBusinessInformationByMobileNo(
                                                       vm.InvoiceMaster.ToInvoiceMobileNumber).
                                                       KiiPayBusinessInformation.BusinessName


            });
            //return RedirectToAction("InvoiceCreateSuccess", new { Amount = vm.InvoiceMaster.TotalAmount,
            //    ReceiverName = _kiiPayBusinessCommonServices.GetBusinessInformationByMobileNo(
            //                                           vm.InvoiceMaster.ToInvoiceMobileNumber).
            //                                           KiiPayBusinessInformation.BusinessName});



        }
        [HttpPost]
        public JsonResult UpdateSendInvoice([Bind(Include = KiiPayBusinessSendAnInvoicevm.BindProperty)]KiiPayBusinessSendAnInvoicevm vm)
        {

            bool UpdateSucess = _kiiPayBusinessSendAnInvoiceServices.UpdateInvoice(vm);

            return Json(new
            {
                Amount = vm.InvoiceMaster.TotalAmount,
                ReceiverName = _kiiPayBusinessCommonServices.GetBusinessInformationByMobileNo(
                                                       vm.InvoiceMaster.ToInvoiceMobileNumber).
                                                       KiiPayBusinessInformation.BusinessName


            });
            //return RedirectToAction("InvoiceCreateSuccess", new { Amount = vm.InvoiceMaster.TotalAmount,
            //    ReceiverName = _kiiPayBusinessCommonServices.GetBusinessInformationByMobileNo(
            //                                           vm.InvoiceMaster.ToInvoiceMobileNumber).
            //                                           KiiPayBusinessInformation.BusinessName});



        }
        public ActionResult InvoiceCreateSuccess(decimal Amount, string ReceiverName)
        {
            KiiPayBusinessSendAnInvoiceSuccessvm vm = new KiiPayBusinessSendAnInvoiceSuccessvm();
            vm.Amount = Amount;
            vm.CurrencySymbol = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CurrencySymbol;
            vm.ReceiverName = ReceiverName;
            return View(vm);
        }

        public JsonResult GetDiscountMethod()
        {

            var data = new List<DiscountMethodVM>();

            data.Add(new DiscountMethodVM()
            {
                Id = 0,
                Name = "%"
            });
            data.Add(new DiscountMethodVM()
            {
                Id = 1,
                Name = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CurrencySymbol
            });
            return Json(new
            {
                Data = data
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SeeInvoice(int Id)
        {
            var BusinessInfo = _kiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId
                                (Common.BusinessSession.LoggedKiiPayBusinessUserInfo
                                .KiiPayBusinessInformationId);
            KiiPayBusinessSendAnInvoicevm vm = new KiiPayBusinessSendAnInvoicevm();
            vm = _kiiPayBusinessSendAnInvoiceServices.GetInvoiceMasterDetails(Id);
            vm.InvoiceMaster.FromBusinessName = BusinessInfo.KiiPayBusinessInformation.BusinessName;
            vm.InvoiceMaster.FromInvoiceMobileNumber = BusinessInfo.KiiPayBusinessInformation.BusinessMobileNo;
            return View(vm);
        }

        public JsonResult GetInvoiceInfo(int Id)
        {


            var BusinessInfo = _kiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId
                                (Common.BusinessSession.LoggedKiiPayBusinessUserInfo
                                .KiiPayBusinessInformationId);
            KiiPayBusinessSendAnInvoicevm vm = new KiiPayBusinessSendAnInvoicevm();
            vm = _kiiPayBusinessSendAnInvoiceServices.GetInvoiceMasterDetails(Id);
            vm.InvoiceMaster.FromBusinessName = BusinessInfo.KiiPayBusinessInformation.BusinessName;
            vm.InvoiceMaster.FromInvoiceMobileNumber = BusinessInfo.KiiPayBusinessInformation.BusinessMobileNo;
            return Json(new
            {
                Data = vm
            }, JsonRequestBehavior.AllowGet);
        }
    }

    public class DiscountMethodVM
    {

        public int Id { get; set; }
        public string Name { get; set; }

    }

}