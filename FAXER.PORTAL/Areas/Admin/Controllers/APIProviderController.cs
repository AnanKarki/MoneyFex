using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class APIProviderController : Controller
    {
        APIProviderServices _services = null;
        CommonServices _CommonServices = null;
        public APIProviderController()
        {
            _services = new APIProviderServices();
            _CommonServices = new CommonServices();
        }
        // GET: Admin/APIProvider
        public ActionResult Index(string country = "", int TransferMethod = 0,
            string ContactPerson = "", string Telephone = "", string Email = "", int? page = null)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");

            ViewBag.TransferMethod = TransferMethod;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.ContactPerson = ContactPerson;
            ViewBag.Telephone = Telephone;
            ViewBag.Email = Email;
            IPagedList<APIProviderViewModel> vm = _services.GetAPIProviderList(country, TransferMethod).ToPagedList(pageNumber, pageSize);

            if (!string.IsNullOrEmpty(ContactPerson))
            {
                ContactPerson = ContactPerson.Trim();
                vm = vm.Where(x => x.ContactPerson.ToLower().Contains(ContactPerson.ToLower())).ToPagedList(pageNumber, pageSize);

            }
            if (!string.IsNullOrEmpty(Telephone))
            {
                Telephone = Telephone.Trim();
                vm = vm.Where(x => x.Telephone.ToLower().Contains(Telephone.ToLower())).ToPagedList(pageNumber, pageSize);

            }
            if (!string.IsNullOrEmpty(Email))
            {
                Email = Email.Trim();
                vm = vm.Where(x => x.Email.ToLower().Contains(Email.ToLower())).ToPagedList(pageNumber, pageSize);

            }
            return View(vm);
        }
        [HttpGet]
        public JsonResult Delete(int id)
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

        public ActionResult AddAPIProvider(int id = 0)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");

            if (id > 0)
            {
                APIProviderViewModel vm = _services.GetAPIProviderList().Where(x => x.Id == id).FirstOrDefault();
                return View(vm);
            }
            return View();

        }
        [HttpPost]
        public ActionResult AddAPIProvider([Bind(Include = APIProviderViewModel.BindProperty)]APIProviderViewModel vm)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            if (ModelState.IsValid)
            {
                if (vm.TransferMethod == 0)
                {
                    ModelState.AddModelError("TransferMethod", "Select Method");
                    return View(vm);
                }
                if (vm.Id == 0)
                {
                    _services.AddAPiProvider(vm);
                }
                else
                {
                    _services.UpdateAPiProvider(vm);
                }
                return RedirectToAction("Index", "APIProvider");
            }
            return View();

        }
    }
}