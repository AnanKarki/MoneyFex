using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.Dashboard.LearnMore
{
    public class MFCareerController : Controller
    {
        Services.SMFCareer mFCareerServices = null;
        CommonServices CommonServices = new CommonServices();
        public MFCareerController()
        {
            mFCareerServices = new Services.SMFCareer();
        }
        // GET: Career
        public ActionResult Index()
        {
            var countries = CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");

            var model = mFCareerServices.GetMFCareerList();
            var vm = new Models.MFCareerViewModel();
            vm.CareerList = model;
            ViewBag.InvalidModel = "False";
            return View(vm);
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = MFCareerViewModel.BindProperty)]MFCareerViewModel vm)
        {
            var countries = CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            var model = mFCareerServices.GetMFCareerList();
            vm.CareerList = model;
            if (ModelState.IsValid)
            {

                var JObDetails = mFCareerServices.GetJob(vm.JobId);
              
                var attachmentPaths = new List<string>();
                if (Request.Files.Count > 0)
                {
                    string directory = Server.MapPath("/Documents");




                    var upload = Request.Files["CV"];
                    var Statement = Request.Files["Statement"];
                    string filename = "";
                    if (upload != null && upload.ContentLength > 0)
                    {
                        filename = Guid.NewGuid() + "." + upload.FileName.Split('.')[1]; //Path.GetFileName(upload.FileName);
                        upload.SaveAs(Server.MapPath("~/Documents") + "\\" + filename);
                        vm.CVURL = "/Documents/" + filename;
                        attachmentPaths.Add(HostingEnvironment.MapPath(@"/") + vm.CVURL);

                    }
                    if (Statement != null && Statement.ContentLength > 0)
                    {
                        filename = Guid.NewGuid() + "." + Statement.FileName.Split('.')[1]; //Path.GetFileName(upload.FileName);
                        upload.SaveAs(Server.MapPath("~/Documents") + "\\" + filename);
                        vm.SupportingStatementURL = "/Documents/" + filename;
                        attachmentPaths.Add(HostingEnvironment.MapPath(@"/") + vm.SupportingStatementURL);

                    }

                }
                if (vm.CVURL == null)
                {
                    ModelState.AddModelError("CVURL", "Please Upload Your CV");
                    ViewBag.InvalidModel = "True";

                    return View(vm);
                }
                if(vm.SupportingStatementURL == null)
                {

                    ModelState.AddModelError("SupportingStatementURL", "Please Upload the Supporting Statement");
                    ViewBag.InvalidModel = "True";

                    return View(vm);
                }

                DB.JobApplicant jobApplicant = new DB.JobApplicant()
                {
                    FirstName = vm.FirstName,
                    LastName = vm.LastName,
                    Country = vm.Country,
                    City = vm.City,
                    Email = vm.Email,
                    Telephone = vm.Telephone,
                    CvURL = vm.CVURL,
                    PositionAppliedFor = vm.Position,
                    SupportingStatementURL = vm.SupportingStatementURL,
                    JobId = vm.JobId,
                };

                var obj = mFCareerServices.SaveJobApplicant(jobApplicant);

                MailCommon mailCommon = new MailCommon();

                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

                string EmailToHR_body = "";
                string CountryCode = Common.Common.GetCountryName(obj.Country);

                var JobDetails = mFCareerServices.GetJob(obj.JobId);


                EmailToHR_body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/EmailToHRAdminAboutJobApplicant?CandidateName=" + obj.FirstName + " " + obj.LastName +
                    "&Telephone=" + obj.Telephone + "&Email=" + obj.Email + "&Country=" + CountryCode + "&City=" + obj.City
                    + "&JobTitle=" + JObDetails.JobTitle);


                mailCommon.SendMail_ApplyJob("Hrdepartment@moneyfex.com", "Job application -" + JObDetails.JobTitle, EmailToHR_body, attachmentPaths.ToArray());


                string EmailtoJobApplicant_body = "";
                EmailtoJobApplicant_body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/EmailToJobApplicant?JobApplicantFirstName=" + obj.FirstName + "&JobTitle=" + JObDetails.JobTitle);


                mailCommon.SendMail(jobApplicant.Email, "Job application - " + JObDetails.JobTitle, EmailtoJobApplicant_body);
            }

            else
            {
                ViewBag.InvalidModel = "True";

                return View(vm);


            }
            ViewBag.successful = "True";
            var viewmodel = new Models.MFCareerViewModel();
            viewmodel.CareerList = model;
            return View(viewmodel);
        }
    }
}