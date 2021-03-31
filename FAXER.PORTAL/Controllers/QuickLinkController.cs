using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
namespace FAXER.PORTAL.Controllers
{
    
    public class QuickLinkController : Controller
    {
        SMFCareer mFCareerServices = new SMFCareer();
        CommonServices CommonServices = new CommonServices();
        SNewsHomeServices newsService = new SNewsHomeServices();
        SSenderTrackTransferServices _tracktransferServices = new SSenderTrackTransferServices();

        // GET: QuickLink
        public ActionResult SenderAboutUs()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SenderCareer()
        {
            var model = mFCareerServices.GetMFCareerList();
            var vm = new MFCareerViewModel();
            vm.CareerList = model;
            return View(vm);
        }
        [HttpPost]
        public ActionResult SenderCareer([Bind(Include = MFCareerViewModel.BindProperty)]MFCareerViewModel vm)
        {
            var model = mFCareerServices.GetMFCareerList();
            vm.CareerList = model;
            int jobId = vm.CareerList.Select(x => x.Id).FirstOrDefault();
            if (model.Count > 0)
            {
                var JObDetails = mFCareerServices.GetJob(jobId);
                if (JObDetails != null)
                {
                    return RedirectToAction("SenderJobApplication", "QuickLink", new { @jobId = jobId });
                }
            }
            return View(vm);
        }
        [HttpGet]
        public ActionResult SenderJobApplication(int jobId)
        {

            MFCareerViewModel vm = new MFCareerViewModel();
            var model = mFCareerServices.GetMFCareerList();
            vm.CareerList = model;
            var countries = CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            vm.JobId = jobId;
            return View(vm);
        }
        [HttpPost]
        public ActionResult SenderJobApplication([Bind(Include = MFCareerViewModel.BindProperty)]MFCareerViewModel vm)
        {
            var countries = CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
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
                    return View(vm);
                }
                if (vm.SupportingStatementURL == null)
                {
                    ModelState.AddModelError("SupportingStatementURL", "Please Upload the Supporting Statement");

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
                return View(vm);

            }
            return View(vm);

        }
        [HttpGet]
        public ActionResult SenderContact()
        {

            ViewBag.Success = 0;
            return View();
        }

        [HttpPost]
        public ActionResult SenderContact(ContactUsVm model)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Success = 0;
                return View(model);
            }
            ViewBag.Success = 1;
            model = new ContactUsVm();
            ModelState.Clear();
            return View(model);
        }
        public void SendEmail(ContactUsVm model)
        {

        }
        public ActionResult SenderFAQ()
        {
            return View();
        }

        public ActionResult SenderReportAFraud()
        {
            return View();
        }

        public ActionResult SenderFindLocation()
        {
            return View();
        }
        public ActionResult SenderTrackATransfer()
        {
            return View();
        }
        [HttpGet]
        public ActionResult SenderTrackTransferDetails()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SenderTrackTransferDetails([Bind(Include = TrackFax.BindProperty)]TrackFax model)
        {
            TrackFaxDetails faxDetails = new TrackFaxDetails();
            if (ModelState.IsValid)
            {
                // Merchant Transfer 
                if (model.MoneyFaxControlNumber.Contains("SP"))
                {
                    faxDetails = _tracktransferServices.GetFaxDetails(model);
                }
                // CardUser Transfer
                else if (model.MoneyFaxControlNumber.Contains("CU"))
                {
                    faxDetails = _tracktransferServices.GetFaxCardDetails(model);
                }
                else
                {
                    faxDetails = _tracktransferServices.GetFaxNonCardDetails(model);
                }
                if (faxDetails != null)
                {
                    if (faxDetails.SenderSurName.ToLower() == model.FaxerSurNam.ToLower())
                    {
                        model.FaxingStatus = faxDetails.StatusOfFax;
                        FaxerSession.TrackAFaxFaxDetails = model;
                        return View(faxDetails);
                    }
                    else
                    {
                        TempData["ValidData"] = "Sender Lastname doesn't match";
                        return RedirectToAction("SenderTrackATransfer");
                    }
                }
                else
                {
                    TempData["ValidData"] = "Please enter a valid MFCN Number";
                }
            }
            else
            {
                TempData["ValidData"] = "Sender Lastname and MFCN is required";
                return RedirectToAction("SenderTrackATransfer");
            }
            return RedirectToAction("SenderTrackATransfer");
        }

        public ActionResult SenderHowToSendMoney()
        {
            return View();
        }
        public ActionResult SenderToPayForGoodsAndServices()
        {
            return View();
        }
        public ActionResult SenderSiteTermsOfUse()
        {
            return View();
        }
        public ActionResult SenderPrivacyPolicy()
        {
            return View();
        }
        public ActionResult SenderTermsAndConditions()
        {
            return View();
        }
        public ActionResult SenderCookiePolicy()
        {
            return View();
        }
    }


    public class ContactUsVm {

        [Required(ErrorMessage = "Enter name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Enter email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Enter telephone")]
        public string Telephone { get; set; }
        public string FaxNo { get; set; }
        [Required(ErrorMessage = "Enter city")]
        public string City { get; set; }
        public string Country { get; set; }
        [Required(ErrorMessage = "Enter subject")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "Enter message")]

        public string Message { get; set; }


    }
}