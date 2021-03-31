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
    public class AgentDocumentationController : Controller
    {
        AgentDocumentationServices _services = null;
        CommonServices _CommonServices = null;
        public AgentDocumentationController()
        {
            _services = new AgentDocumentationServices();
            _CommonServices = new CommonServices();
        }
        // GET: Admin/AgentDocumentation
        public ActionResult Index(string SendingCountry = "", int AgentId = 0, int DocumentType = 0, string City = "", string StaffName = "", string AccountNo = "", int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var sendingcountries = _CommonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(sendingcountries, "Code", "Name");
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var agents = _CommonServices.GetAgent(SendingCountry);
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");
            ViewBag.City = City;
            ViewBag.StaffName = StaffName;
            ViewBag.AccountNo = AccountNo;
            IPagedList<BusinessDocumentationViewModel> vm = _services.GetAgentDocumentation(SendingCountry, AgentId, DocumentType, City, StaffName, AccountNo).ToPagedList(pageNumber, pageSize);
            return View(vm);
        }

        public ActionResult UploadAgentDocument(int id = 0)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");

            ViewBag.IssuingCountries = new SelectList(Countries, "Code", "Name");

            var Cities = _CommonServices.GetCitiesByName();
            ViewBag.Cities = new SelectList(Cities, "City", "City");

            var agent = _CommonServices.GetAgent();
            ViewBag.Agents = new SelectList(agent, "AgentId", "AgentName");

            BusinessDocumentationViewModel vm = new BusinessDocumentationViewModel(); ;
            if (id != 0)
            {
                vm = _services.GetUploadedDocumentInfo(id);
            }
            return View(vm);

        }

        [HttpPost]
        public ActionResult UploadAgentDocument([Bind(Include = BusinessDocumentationViewModel.BindProperty)]BusinessDocumentationViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var IssuingCountries = _CommonServices.GetCountries();
            ViewBag.IssuingCountries = new SelectList(IssuingCountries, "Code", "Name");

            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");

            var Cities = _CommonServices.GetCitiesByName();
            ViewBag.Cities = new SelectList(Cities, "City", "City");

            var agent = _CommonServices.GetAgent(vm.CountryCode);
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
                    _services.UploadAgentDocument(vm);
                }
                else
                {
                    _services.UpdateAgentDocument(vm);
                }
                return RedirectToAction("Index", "AgentDocumentation");
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
        public JsonResult GetAgentByCountry(string Country = "")
        {
            var data = _CommonServices.GetAgent(Country).ToList();
            return Json(new
            {
                Data = data
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAccountNumber(int agentId = 0)
        {
            string Accountnumber = _CommonServices.GetAgentAccNo(agentId);
            return Json(new
            {
                Data = Accountnumber
            }, JsonRequestBehavior.AllowGet);
        }
    }
}