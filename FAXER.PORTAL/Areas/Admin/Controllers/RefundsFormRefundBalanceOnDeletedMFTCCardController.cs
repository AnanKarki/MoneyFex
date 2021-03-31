using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class RefundsFormRefundBalanceOnDeletedMFTCCardController : Controller
    {
        RefundsFormRefundBalanceOnDeletedMFTCCardServices Service = new RefundsFormRefundBalanceOnDeletedMFTCCardServices();
        // GET: Admin/RefundsFormRefundBalanceOnDeletedMFTCCard
        public ActionResult Index(string MFTC="")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (MFTC == "success")
            {
                ViewBag.Message = "Your Operation was successful !";
                MFTC = "";
            }
            if (string.IsNullOrEmpty(MFTC) == false)
            {
                var vm = Service.getList(MFTC);
                return View(vm);
            }
            return View();
        }
        [HttpPost]
        public ActionResult Index ([Bind(Include = RefundsFormRefundOnDeletedMFTCCardViewModel.BindProperty)]RefundsFormRefundOnDeletedMFTCCardViewModel model)
        {
            if (model != null)
            {
                if (model.StatusOfCard == "IsRefunded")
                {
                    ModelState.AddModelError("EncryptedMFTCNumber", "This card is already refunded ! You can't proceed further.");
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.MFTCCardNumber))
                {
                    ModelState.AddModelError("EncryptedMFTCNumber", "Please enter the MFTC Number First !");
                    return View(model);
                }
                bool isDeleted = Service.isMFTCDeleted(model.EncryptedMFTCNumber);
                if (isDeleted == false)
                {
                    ModelState.AddModelError("MFTCCardNumber", "This card is not deleted ! It must be deleted first in order to proceed to refund !");
                    return View(model);
                }

                if (Common.StaffSession.LoggedStaff == null)
                {
                    ModelState.AddModelError("AdminNameOfRefunder", "You must be logged in as staff to make the changes !");
                    return View(model);
                }
                else if (string.IsNullOrEmpty(model.MFTCReasonForDeletion))
                {
                    ModelState.AddModelError("MFTCReasonForDeletion", "This field can't be blank !");
                    return View(model);
                }
                else if (model.ConfirmRefundRequest == false)
                {
                    ModelState.AddModelError("ConfirmRefundRequest", "Please Confirm the Refund Request before proceeding !");
                    return View(model);
                }
                decimal balance = Service.getCurrentBalance(model.EncryptedMFTCNumber);
                if (balance == 0)
                {
                    ModelState.AddModelError("MFTCAmountBeforeDeletion", "Sorry ! There's no amount to refund !");
                    return View(model);
                }
                bool result = Service.SaveDeletedMFTCRefund(model);
                if (result)
                {
                    return RedirectToAction("Index", new { @MFTC ="success"});
                }

            }
            return View(model);
        }
    }
}