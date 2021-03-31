using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class FormNonCardUserMoneyFaxedUpdateController : Controller
    {
        Services.CommonServices CommonService = new Services.CommonServices();
        FormNonCardUserMoneyFaxedUpdateServices Service = new FormNonCardUserMoneyFaxedUpdateServices();
        // GET: Admin/FormNonCardUserMoneyFaxedUpdate
        [HttpGet]
        public ActionResult Index(string MFTC = "", string message="")
        {
            AdminResult adminResult = new AdminResult();
            ViewBag.AdminResult = adminResult;
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (message == "success")
            {
                ViewBag.Message = "Updated Successfully !";
                message = "";
            }
            if (MFTC != "")
            {
                SetViewBagForContries();

                var vm = Service.getFaxerInfo(MFTC);
                if (vm.FormNonCardUserFaxerDetails.FaxingStatusEnum == DB.FaxingStatus.Hold || vm.FormNonCardUserFaxerDetails.FaxingStatusEnum == DB.FaxingStatus.Refund
                    || vm.FormNonCardUserFaxerDetails.FaxingStatusEnum == DB.FaxingStatus.Received  || vm.FormNonCardUserFaxerDetails.FaxingStatusEnum ==  DB.FaxingStatus.Cancel )
                {

                    adminResult.Status = AdminResultStatus.Warning;
                    adminResult.Message = "Sorry! This Transaction can not be completed because it has been   "  + Common.Common.GetEnumDescription((DB.FaxingStatus)vm.FormNonCardUserFaxerDetails.FaxingStatusEnum) + "   . You cannot update the transaction information" ;
                    ViewBag.AdminResult = adminResult;
                    return View(new FormNonCardUserMoneyFaxedUpdateViewModel());
                }
                return View(vm);
            }
            SetViewBagForContries();
            return View(new FormNonCardUserMoneyFaxedUpdateViewModel());
        }

        private void SetViewBagForContries()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = FormNonCardUserMoneyFaxedUpdateViewModel.BindProperty)]FormNonCardUserMoneyFaxedUpdateViewModel model)
        {

            AdminResult adminResult = new AdminResult();
            ViewBag.AdminResult = adminResult;
            if (model != null)
            {
                bool valid = true;
                if (model.FormNonCardUserFaxerDetails.FaxerFirstName == null)
                {
                    ModelState.AddModelError("FaxerFirstName", "Please make Faxer's Details available !");
                    valid = false;
                }
                //else
                //if (model.FormNonCardUserFaxAmount.ReceivingCountry == null)
                //{
                //    ModelState.AddModelError("ReceivingCountry", "Please enter the Receiver's Country !");
                //    valid = false;
                //}
                //else
                //if (model.FormNonCardUserFaxAmount.FaxingAmount == 0)
                //{
                //    ModelState.AddModelError("FaxingAmount", "The Faxing Amount must be greater than 0 !");
                //    valid = false;
                //}
                else
                if (model.FormNonCardUserReceiverDetails.ReceiverFirstName == null)
                {
                    ModelState.AddModelError("ReceiverFirstName", "This field can't be blank !");
                    valid = false;
                }
                else
                if (model.FormNonCardUserReceiverDetails.ReceiverLastName == null)
                {
                    ModelState.AddModelError("ReceiverLastName", "This field can't be blank !");
                    valid = false;
                }
                else
               if (model.FormNonCardUserReceiverDetails.ReceiverAddress == null)
                {
                    ModelState.AddModelError("ReceiverAddress", "This field can't be blank !");
                    valid = false;
                }
                else
               if (model.FormNonCardUserReceiverDetails.ReceiverCity == null)
                {
                    ModelState.AddModelError("ReceiverCity", "This field can't be blank !");
                    valid = false;
                }
                //else
                //if (model.FormNonCardUserReceiverDetails.ReceiverCountry == null)
                //{
                //    ModelState.AddModelError("ReceiverCountry", "This field can't be blank !");
                //    valid = false;
                //}
                else
                if (model.FormNonCardUserReceiverDetails.ReceieverTelephone == null)
                {
                    ModelState.AddModelError("ReceieverTelephone", "This field can't be blank !");
                    valid = false;
                }
                //else
                //if (model.FormNonCardUserReceiverDetails.ReceiverCountry != model.FormNonCardUserFaxAmount.ReceivingCountry)
                //{
                //    ModelState.AddModelError("ReceiverCountry", "The Receiving Country Name on the Faxed Amount Section and Receiver's Detail section should match !");
                //    valid = false;
                //}
                //else
                //if (model.FormNonCardUserAdminDetails.AgencyName == null)
                //{
                //    ModelState.AddModelError("AgencyName", "This field can't be blank !");
                //    valid = false;
                //}
                //else
                //if (model.FormNonCardUserAdminDetails.MFSCode == null)
                //{
                //    ModelState.AddModelError("MFSCode", "This field can't be blank !");
                //    valid = false;
                //}
                //else
                //if (model.FormNonCardUserAdminDetails.NameOfUpdater == null)
                //{
                //    ModelState.AddModelError("NameOfUpdater", "This field can't be blank !");
                //    valid = false;
                //}
                else
                if (model.CheckConfirmation == false)
                {
                    ModelState.AddModelError("CheckConfirmation", "Please accept the Confirmation Text !");
                    valid = false;
                }
                if (valid == false)
                {
                    SetViewBagForContries();
                    return View(model);
                }
                if (valid == true)
                {
                    var result = Service.saveNonCardFaxingInfo(model);
                    if (result)
                    {
                        return RedirectToAction("Index", new { @message ="success"});
                    }
                }
            }
            
            SetViewBagForContries();
            return View(model);
        }


        public ActionResult getFaxingSummary(decimal faxingAmount = 0, string faxingCC = "", string receivingCC = "")
        {
            var faxingSummary = Service.getFaxingCalculationSummary(faxingAmount, faxingCC, receivingCC);
            return Json(new ViewModels.FormNonCardUserFaxAmountViewModel()
            {
                FaxingAmount = faxingSummary.TopUpAmount,
                FaxingFee = faxingSummary.TopUpFees,
                TotalAmount = faxingSummary.TotalAmountIncludingFees,
                ExchangeRate = faxingSummary.ExchangeRate,
                ReceivingAmount = faxingSummary.ReceivingAmount,
                ReceivingCountry = receivingCC,
            }, JsonRequestBehavior.AllowGet);
        }
    }
}