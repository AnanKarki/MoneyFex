using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class AUXAgentDocumentationController : Controller
    {
        CommonServices _commonServices = null;
        AuxAgentDocumentationServices _services = null;
        public AUXAgentDocumentationController()
        {
            _commonServices = new CommonServices();
            _services = new AuxAgentDocumentationServices();
        }

        // GET: Admin/AUXAgentDocumentation
        public ActionResult Index(string SendingCountry = "", int AgentId = 0, int DocumentType = 0, string City = "",
            string AgentAccountNo = "", string DocumentName = "", string DateRange = "", string staff = "", int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var sendingcountries = _commonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(sendingcountries, "Code", "Name");
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var agents = _commonServices.GetAuxAgents();
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");
            ViewBag.City = City;
            ViewBag.AgentAccountNo = AgentAccountNo;
            ViewBag.DocumentName = DocumentName;
            ViewBag.Datetime = DateRange;
            ViewBag.staff = staff;
            IPagedList<BusinessDocumentationViewModel> vm = _services.GetAuxAgentDocumentation(SendingCountry, AgentId, DocumentType, City, AgentAccountNo, DocumentName, DateRange, staff).ToPagedList(pageNumber, pageSize);
            return View(vm);
        }

        public ActionResult UploadAuxAgentDocument(int id = 0)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _commonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            var IssuingCountries = _commonServices.GetCountries();
            ViewBag.IssuingCountries = new SelectList(IssuingCountries, "Code", "Name");

            var Cities = _commonServices.GetCitiesByName();
            ViewBag.Cities = new SelectList(Cities, "City", "City");

            var agent = _commonServices.GetAuxAgents();
            ViewBag.Agents = new SelectList(agent, "AgentId", "AgentName");

            BusinessDocumentationViewModel vm = new BusinessDocumentationViewModel(); ;
            if (id != 0)
            {
                vm = _services.GetUploadedDocumentInfo(id);
            }
            return View(vm);

        }

        [HttpPost]
        public ActionResult UploadAuxAgentDocument([Bind(Include = BusinessDocumentationViewModel.BindProperty)]BusinessDocumentationViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var IssuingCountries = _commonServices.GetCountries();
            ViewBag.IssuingCountries = new SelectList(IssuingCountries, "Code", "Name");

            var Countries = _commonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");

            var Cities = _commonServices.GetCitiesByName();
            ViewBag.Cities = new SelectList(Cities, "City", "City");

            var agent = _commonServices.GetAuxAgents();
            ViewBag.Agents = new SelectList(agent, "AgentId", "AgentName");
            if (ModelState.IsValid)
            {
                var CurrentDate = DateTime.Now;

                if (vm.DocumentType == DocumentType.Select)
                {
                    ModelState.AddModelError("DocumentType", "Select Document Type.");
                    return View(vm);
                }
                if (vm.DocumentExpires == DocumentExpires.Select)
                {
                    ModelState.AddModelError("DocumentExpires", "Select Document Expires.");
                    return View(vm);
                }
                if (vm.DocumentExpires == DocumentExpires.Yes && vm.ExpiryDate == null)
                {
                    ModelState.AddModelError("ExpiryDate", "Enter Expiry Date. ");
                    return View(vm);
                }

                if (vm.ExpiryDate < CurrentDate)
                {
                    ModelState.AddModelError("ExpiryDate", "Document has expired. ");
                    return View(vm);
                }

                if (Request.Files.Count < 1)
                {
                    var identificationdoc = Request.Files["DocumentPhotoUrl"];

                }
                string identificationDocPath = "";
                var IdentificationDoc = Request.Files["DocumentPhotoUrl"];
                if (IdentificationDoc != null && IdentificationDoc.ContentLength > 0)
                {
                    var allowedExtensions = new string[] { "gif", "jpg", "png", "jpeg", "pdf" };
                    var extension = IdentificationDoc.FileName.Split('.')[1];
                    extension = extension.ToLower();
                    identificationDocPath = Guid.NewGuid() + "." + IdentificationDoc.FileName.Split('.')[1];

                    if (allowedExtensions.Contains(extension))
                    {

                        try
                        {


                            IdentificationDoc.SaveAs(Server.MapPath("~/Documents") + "\\" + identificationDocPath);
                        }
                        catch (Exception ex)
                        {


                        }

                        vm.DocumentPhotoUrl = "/Documents/" + identificationDocPath;

                    }
                    else
                    {
                        ModelState.AddModelError("DocumentPhotoUrl", "File type not allowed to upload. ");
                        return View(vm);
                    }

                }
                if (vm.Id == 0)
                {
                    _services.UploadAuxAgentDocument(vm);
                }
                else
                {
                    _services.UpdateAuxAgentDocument(vm);
                }
                return RedirectToAction("Index", "AUXAgentDocumentation");
            }

            return View(vm);
        }
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
        public JsonResult GetAgentByCountry(string Country = "")
        {
            var data = _commonServices.GetAuxAgents().Where(x => x.Country == Country).ToList();
            return Json(new
            {
                Data = data
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAccountNumber(int agentId = 0)
        {
            string Accountnumber = _commonServices.GetAgentAccNo(agentId);
            return Json(new
            {
                Data = Accountnumber
            }, JsonRequestBehavior.AllowGet);
        }

    }
}