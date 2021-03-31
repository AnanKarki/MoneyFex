using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class LogosUploadController : Controller
    {
        LogosUploadServices _logoUploadServices = null;
        CommonServices _CommonServices = null;
        public LogosUploadController()
        {
            _logoUploadServices = new LogosUploadServices();
            _CommonServices = new CommonServices();
        }
        // GET: Admin/LogosUpload
        public ActionResult Index(string Country = "", int TransferMethod = 0, string Title = "", int? page = null)
        {
            var countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<LogosUploadViewModel> vm = _logoUploadServices.List(Country, TransferMethod).ToPagedList(pageNumber, pageSize);
            if (!string.IsNullOrEmpty(Title))
            {
                Title = Title.Trim();
                vm = vm.Where(x => x.Title.ToLower().Contains(Title.ToLower())).ToPagedList(pageNumber, pageSize);

            }
            ViewBag.TransferMethod = TransferMethod;
            ViewBag.Title = Title;


            return View(vm);
        }
        public ActionResult UploadNewLogo(int id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            if (id != 0)
            {
                LogosUploadViewModel vm = _logoUploadServices.List().Where(x => x.Id == id).FirstOrDefault();
                return View(vm);
            }
            return View();

        }
        [HttpPost]
        public ActionResult UploadNewLogo([Bind(Include = LogosUploadViewModel.BindProperty)]LogosUploadViewModel vm)
        {
            var countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            if (ModelState.IsValid)
            {
                string identificationDocPath = "";

                var Logo = Request.Files["Logo"];

                if (Logo != null && Logo.ContentLength > 0)
                {
                    string[] logopath = Logo.FileName.Split('.');
                    identificationDocPath = Guid.NewGuid() + "." + Logo.FileName.Split('.')[logopath.Length - 1];
                    Logo.SaveAs(Server.MapPath("~/Documents") + "\\" + identificationDocPath);
                    vm.Logo = "/Documents/" + identificationDocPath;

                }
                if (vm.Id == 0)
                {
                    _logoUploadServices.Add(vm);

                }
                else
                {
                    _logoUploadServices.Update(vm);
                }

                return RedirectToAction("Index", "LogosUpload");
            }
            return View(vm);
        }
        public JsonResult Delete(int id)
        {
            if (id > 0)
            {
                var data = _logoUploadServices.LogosUploadData().Data.Where(x => x.Id == id).FirstOrDefault();
                _logoUploadServices.Remove(data);
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

    }
}