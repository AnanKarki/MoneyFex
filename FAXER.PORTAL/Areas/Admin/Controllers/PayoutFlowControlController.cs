using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Controllers.MyRegisteredKiiPayWallets;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class PayoutFlowControlController : Controller
    {
        PayoutFlowControlServices _service = null;
        CommonServices _CommonServices = null;
        public PayoutFlowControlController()
        {
            _service = new PayoutFlowControlServices();
            _CommonServices = new CommonServices();
        }
        // GET: Admin/PayoutFlowControl
        public ActionResult Index()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var vm = _service.GetPayoutFlowControlData();
            return View(vm);
        }


        public ActionResult SetPayoutFlowControl(int id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            PayoutFlowControlViewModel vm = new PayoutFlowControlViewModel();
            setCurrencyViewBag();
            ViewBag.Id = id;
            ViewBag.TransferMethod = TransactionTransferMethod.Select;
            if (id != 0)
            {
                vm = _service.payoutFlowControlByMasterId(id);

                ViewBag.TransferMethod = vm.Master.TransferMethod;
            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult SetPayoutFlowControl([Bind(Include = PayoutFlowControlViewModel.BindProperty)]PayoutFlowControlViewModel payoutFlowControlViewModel)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            setCurrencyViewBag();
            if (ModelState.IsValid)
            {
                if (payoutFlowControlViewModel.Master.TransferMethod == 0)
                {
                    ModelState.AddModelError("TransferMethod", " select Transfer Method");
                    return View(payoutFlowControlViewModel);
                }

                if (payoutFlowControlViewModel.Master.Id == 0)
                {
                    _service.AddPayoutFlowControl(payoutFlowControlViewModel);
                }
                else
                {
                    _service.UpdatePayoutFlowControl(payoutFlowControlViewModel);

                }
                return RedirectToAction("Index", "PayoutFlowControl");

            }
            return View();
        }
        [HttpGet]
        public JsonResult Delete(int id = 0)
        {
            if (id > 0)
            {
                _service.Remove(id);
                return Json(new
                {
                    Data = true,
                    Message = "Deleted Sucessfully"
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    Data = false,
                    Message = "Something went wrong. Please try again!"
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public void setCurrencyViewBag()
        {
            var currency = _CommonServices.GetCountryCurrencies();
            ViewBag.SendingCurrencies = new SelectList(currency, "Code", "Name");
            ViewBag.ReceivingCurrencies = new SelectList(currency, "Code", "Name");
        }
        [HttpGet]
        public JsonResult GetCurrency()
        {

            var result = _CommonServices.GetCountryCurrencies();
            return Json(new ServiceResult<List<Services.DropDownViewModel>>()
            {
                Data = result,
                Message = "",
                Status = ResultStatus.OK
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EnablePayout(int PayoutFlowControlId)
        {

            var result = _service.UpdateEnableAndDisablePayoutProvider(PayoutFlowControlId);

            return Json(new
            {
                Data = result,
            }, JsonRequestBehavior.AllowGet);


        }
        [HttpPost]
        public JsonResult SavePayoutFlow([Bind(Include = PayoutFlowControlViewModel.BindProperty)]PayoutFlowControlViewModel payoutFlowControlViewModel)
        {

            string message = "";
            if (payoutFlowControlViewModel.Master.Id == 0)
            {
                _service.AddPayoutFlowControl(payoutFlowControlViewModel);
                message = "Added Successfully";
            }
            else
            {
                _service.UpdatePayoutFlowControl(payoutFlowControlViewModel);
                message = "Updated Successfully";

            }


            return Json(new ServiceResult<PayoutFlowControlViewModel>()
            {

                Data = payoutFlowControlViewModel,
                Message = message,
                Status = ResultStatus.OK
            });
        }
        [HttpGet]
        public JsonResult GetDetails(int payoutFlowControlId)
        {

            var master = _service.GetPayoutProvideeMasterDetails(payoutFlowControlId);
            var details = _service.GetPayoutProvideDetails(payoutFlowControlId);

            var result = new PayoutFlowControlViewModel()
            {
                Master = master,
                Details = details
            };

            return Json(new ServiceResult<PayoutFlowControlViewModel>()
            {

                Data = result,
                Message = "",
                Status = ResultStatus.OK
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetApiProvider()
        {

            var apiProvider = _service.GetAPiProvider();
            return Json(new ServiceResult<List<Services.DropDownViewModel>>()
            {
                Data = apiProvider,
                Message = "",
                Status = ResultStatus.OK
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetBankOrWallet(TransactionTransferMethod transferMethod)
        {

            var bankOrWallet = _service.GetBankOrWallet(transferMethod);
            return Json(new ServiceResult<List<Services.DropDownViewModel>>()
            {
                Data = bankOrWallet,
                Message = "",
                Status = ResultStatus.OK
            }, JsonRequestBehavior.AllowGet);
        }
    }
}