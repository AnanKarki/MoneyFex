using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class PayoutProviderController : Controller
    {

        PayoutProviderServices _services = null;
        CommonServices _CommonServices = null;
        public PayoutProviderController()
        {
            _services = new PayoutProviderServices();
            _CommonServices = new CommonServices();
        }
        // GET: Admin/PayoutProvider
        public ActionResult Index(string SendingCountry = "", string ReceivingCountry = "", int TranferMethod = 0,
            string ProviderName = "", string Code = "", int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(Countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(Countries, "Code", "Name");
            ViewBag.TranferMethod = TranferMethod;
            ViewBag.ProviderName = ProviderName;
            ViewBag.Code = Code;
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            IPagedList<PayoutProviderViewModel> vm = _services.PayoutProviderList(SendingCountry, ReceivingCountry, TranferMethod).ToPagedList(pageNumber, pageSize);
            if (!string.IsNullOrEmpty(ProviderName))
            {
                ProviderName = ProviderName.Trim();
                vm = vm.Where(x => x.Master.Name.ToLower().Contains(ProviderName.ToLower())).ToPagedList(pageNumber, pageSize);

            }
            if (!string.IsNullOrEmpty(Code))
            {
                Code = Code.Trim();
                vm = vm.Where(x => x.Master.Code.ToLower().Contains(Code.ToLower())).ToPagedList(pageNumber, pageSize);

            }
            return View(vm);
        }
        [HttpGet]
        public JsonResult Delete(int id = 0)
        {
            
            if (id > 0)
            {
                _services.Delete(id);
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
        public ActionResult DeleteDetail(int DetailId, int PayoutProviderId)
        {
            _services.DeleteDetails(DetailId);
            return RedirectToAction("AddPayoutProviders", "PayoutProvider", new { @id = PayoutProviderId });
        }
        public ActionResult AddPayoutProviders(int id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(Countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(Countries, "Code", "Name");
            PayoutProviderViewModel vm = new PayoutProviderViewModel();
            vm.Details = new List<PayoutProviderDetailsViewModel>();
            ViewBag.Id = id;
            if (id > 0)
            {
                vm = _services.PayoutProvider(id);
                return View(vm);
            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult AddPayoutProviders([Bind(Include = PayoutProviderViewModel.BindProperty)]PayoutProviderViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(Countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(Countries, "Code", "Name");
            if (ModelState.IsValid)
            {
                if (vm.Master.TransferMethod == 0)
                {
                    ModelState.AddModelError("TransferMethod", " select Transfer Method");
                    return View(vm);
                }

                if (vm.Master.Id == 0)
                {
                    _services.AddPayoutProviders(vm);
                }
                else
                {
                    _services.UpdatePayoutProviders(vm);

                }
                return RedirectToAction("Index", "PayoutProvider");

            }
            return View(vm);
        }

        [HttpGet]
        public JsonResult GetDetails(int PayoutProviderId)
        {

            var master = _services.GetPayoutProvideeMasterDetails(PayoutProviderId);
            var details = _services.GetPayoutProvideDetails(PayoutProviderId);

            var result = new PayoutProviderViewModel()
            {
                Master = master,
                Details = details
            };

            return Json(new ServiceResult<PayoutProviderViewModel>()
            {

                Data = result,
                Message = "",
                Status = ResultStatus.OK
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveProviders([Bind(Include = PayoutProviderViewModel.BindProperty)]PayoutProviderViewModel vm)
        {

            string message = "";
            if (vm.Master.Id == 0)
            {
                _services.AddPayoutProviders(vm);
                message = "Added Successfully";
            }
            else
            {
                _services.UpdatePayoutProviders(vm);
                message = "Updated Successfully";

            }


            return Json(new ServiceResult<PayoutProviderViewModel>()
            {

                Data = vm,
                Message = message,
                Status = ResultStatus.OK
            });
        }


        [HttpGet]
        public JsonResult GetCountries()
        {

            var result = _CommonServices.GetCountries();
            return Json(new ServiceResult<List<Services.DropDownViewModel>>()
            {
                Data = result,
                Message = "",
                Status = ResultStatus.OK
            }, JsonRequestBehavior.AllowGet);
        }


    }
}