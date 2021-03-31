using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class RefundsFormRefundBalanceOnDeletedMFBCCardController : Controller
    {
        RefundsFormRefundBalanceOnDeletedMFBCCardServices Service = new RefundsFormRefundBalanceOnDeletedMFBCCardServices();
        CommonServices CommonService = new CommonServices();
        // GET: Admin/RefundsFormRefundBalanceOnDeletedMFBCCard
        public ActionResult Index(string MFBC="")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (MFBC == "success")
            {
                ViewBag.Message = "Your operation was successful !";
                MFBC = "";
            }
            if (string.IsNullOrEmpty(MFBC) == false)
            {
                var vm = Service.getList(MFBC);
                    return View(vm);
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index([Bind( Include = RefundsFormRefundOnDeletedMFBCCardViewModel.BindProperty )] RefundsFormRefundOnDeletedMFBCCardViewModel model)
        {
            if (model != null)
            {
                if (model.MFBCCardStatus == "IsRefunded")
                {
                    ModelState.AddModelError("EncryptedMFBCNumber", "This card is already refunded ! You can't proceed further.");
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.MFBCCardNumber))
                {
                    ModelState.AddModelError("EncryptedMFBCNumber", "Please enter the MFTC Number First !");
                    return View(model);
                }
                bool isDeleted = Service.isMFBCDeleted(model.EncryptedMFBCNumber);
                if (isDeleted == false)
                {
                    ModelState.AddModelError("MFBCCardNumber", "This card is not deleted ! It must be deleted first in order to proceed to refund !");
                    return View(model);
                }

                if (Common.StaffSession.LoggedStaff == null)
                {
                    
                    ModelState.AddModelError("AdminRefunderName", "You must be logged in as staff to make the changes !");
                    return View(model);
                }
                else if (string.IsNullOrEmpty(model.MFBCReasonForDeletion))
                {
                    ModelState.AddModelError("MFBCReasonForDeletion", "This field can't be blank !");
                    return View(model);
                }
                else if (model.ConfirmRefund == false)
                {
                    ModelState.AddModelError("ConfirmRefund", "Please Confirm the Refund Request before proceeding !");
                    return View(model);
                }
                decimal balance = Service.getCurrentBalance(model.EncryptedMFBCNumber);
                if (balance == 0)
                {
                    ModelState.AddModelError("MFBCAmountBeforeDeletion", "Sorry ! There's no amount to refund !");
                    return View(model);
                }
                bool result = Service.SaveDeletedMFBCRefund(model);
                if (result)
                {
                    return RedirectToAction("Index", new { @MFBC ="success"});
                }

            }
            return View(model);
        }
    }
}