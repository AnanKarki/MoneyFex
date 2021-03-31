using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Staff.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewRegisteredStaffDetailsController : Controller
    {
        Services.CommonServices CommonService = new Services.CommonServices();
        Services.ViewRegisteredStaffDetailsServices Service = new Services.ViewRegisteredStaffDetailsServices();
        // GET: Admin/ViewRegisteredStaffDetails
        public ActionResult Index(string Country = "", string City = "", string Search = "",int? page=null)
        {
            #region Old
            //if (Common.StaffSession.LoggedStaff == null)
            //{
            //    return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            //}

            //if (message == "mailSent")
            //{
            //    ViewBag.Message = "Mails sent successfully !";
            //    message = "";
            //}
            //else if (message == "mailNotSent")
            //{
            //    ViewBag.Message = "Mails not sent ! Please try again.";
            //    message = "";
            //}

            //SetViewBagForCountries();
            //SetViewBagForSCities(CountryCode);
            //var vm = Service.getStaffInfoList(CountryCode, City);
            //if (vm != null)
            //{
            //    ViewBag.Country = CountryCode;
            //}
            //return View(vm);
            #endregion

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            SetViewBagForSCities(Country);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            

            ViewBag.TotalRegisteredStaff = Service.List().Count;
            ViewBag.TotalHolidayStaff = Service.ListOfHolidayStaff().Count();
            ViewBag.TotalInActiveStaff = Service.ListOfInActiveStaff().Count;

            IPagedList<ViewModels.ViewRegisteredStaffViewModel> vm = Service.getStaffInfoList(Country, City).ToPagedList(pageNumber,pageSize);
            if (!string.IsNullOrEmpty(Search))
            {
                vm = vm.Where(x => x.StaffFirstName.ToLower().Contains(Search.ToLower())).ToPagedList(pageNumber, pageSize);
            }


            string[] alpha = vm.GroupBy(x => x.FirstLetterOfStaff).Select(x => x.FirstOrDefault()).OrderBy(x => x.FirstLetterOfStaff).Select(x => x.FirstLetterOfStaff).ToArray();
            ViewBag.Alpha = alpha;
            ViewBag.Search = Search;
            return View(vm);
        }


        [HttpGet]
        public ActionResult StaffDashBoard(int StaffId)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            ViewBag.LoginCode = "";
            ViewBag.AccessCode = "";
            ViewBag.Currency = "";
            ViewBag.TotalNumberOfHolidays = 0; ViewBag.DayTaken = 0; ViewBag.DayLeft = 0; ViewBag.MonthlySalary = 0;
          StaffDashboardViewModel vm = new StaffDashboardViewModel();
            if (StaffId != 0)
            {
                StaffHolidaysServices _HoliDayServices = new StaffHolidaysServices();
                ViewStaffPayslipServices _paySlipServices = new ViewStaffPayslipServices();
                vm = Service.getStaffDashBoard(StaffId);
                ViewBag.LoginCode = CommonService.getStaffLoginCode(StaffId);
                ViewBag.AccessCode = CommonService.getStaffMFSCode(StaffId);
                ViewBag.TotalNumberOfHolidays = _HoliDayServices.GetEditHolidayList(StaffId) == null ? 0 
                    : _HoliDayServices.GetEditHolidayList(StaffId).NoOfDays;
                ViewBag.DayTaken = _HoliDayServices.GetEditHolidayList(StaffId).AlreadyTaken;
                ViewBag.DayLeft = _HoliDayServices.GetEditHolidayList(StaffId).NoLeft;
                ViewBag.MonthlySalary = 1000M;
                ViewBag.Currency = CommonService.getCurrency(vm.Staff.Country);

            }

            return View(vm);
        }

        [HttpGet]
        public ActionResult ViewRegisteredStaffDetailsMore(int staffId)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (staffId != 0)
            {
                string staffName = "";
                var vm = Service.getStaffDetailsMore(staffId, out staffName);

                ViewBag.staffName = staffName;
                return View(vm);
            }
            return View();
        }

        [HttpGet]
        public ActionResult ViewPreviousAddress(int staffId)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            string staffName = "";
            var vm = Service.getPreviousAddress(staffId, out staffName);

            ViewBag.StaffName = staffName;
            return View(vm);
        }


        [HttpGet]
        public ActionResult RegisterAStaff()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            setViewBagForCountries();
            return View();
        }

        [HttpPost]
        public ActionResult RegisterAStaff([Bind(Include = RegisterAStaffViewModel.BindProperty)]RegisterAStaffViewModel model)
        {
            bool valid = true;
            if (model != null)
            {
                bool checkExistingEmail = Service.checkExistingEmail(model.StaffEmailAddress);
                bool isvalidDOB = DateUtilities.ValidateAge(model.StaffDOB);
                if (isvalidDOB == false)
                {
                    ModelState.AddModelError("StaffDOB", "You must be 18 years of age or greater in order to get resistered !");
                    valid = false;
                }
                if (checkExistingEmail == false)
                {
                    ModelState.AddModelError("StaffEmailAddress", "Sorry ! An staff is already registered with this email address .");
                    valid = false;
                }

                if (string.IsNullOrEmpty(model.StaffFirstName))
                {
                    ModelState.AddModelError("StaffFirstName", "This field can't be blank.");
                    valid = false;
                }
                else if (string.IsNullOrEmpty(model.StaffLastName))
                {
                    ModelState.AddModelError("StaffLastName", "This field can't be blank.");
                    valid = false;
                }
                else if (string.IsNullOrEmpty(model.StaffDOB.ToString()))
                {
                    ModelState.AddModelError("StaffDOB", "This field can't be blank.");
                    valid = false;
                }
                else if (string.IsNullOrEmpty(model.StaffBirthCountry))
                {
                    ModelState.AddModelError("StaffBirthCountry", "This field can't be blank.");
                    valid = false;
                }
                else if (string.IsNullOrEmpty(model.StaffEmailAddress))
                {
                    ModelState.AddModelError("StaffEmailAddress", "This field can't be blank.");
                    valid = false;
                }
                else if (string.IsNullOrEmpty(model.ConfirmEmail))
                {
                    ModelState.AddModelError("ConfirmEmail", "This field can't be blank.");
                    valid = false;
                }
                else if (model.StaffEmailAddress.ToLower() != model.ConfirmEmail.ToLower())
                {
                    ModelState.AddModelError("ConfirmEmail", "Email Addresses don't match !");
                    valid = false;
                }
                if (ModelState.IsValid)
                {
                    if (valid == true)
                    {
                        Common.AdminSession.StaffBasicDetails = model;
                        Session["StaffFirstName"] = model.StaffFirstName;
                        Session["StaffMiddleName"] = model.StaffMiddleName;
                        Session["StaffLastName"] = model.StaffLastName;
                        Session["StaffDOB"] = model.StaffDOB;
                        Session["StaffBirthCountry"] = model.StaffBirthCountry;
                        Session["StaffGender"] = model.StaffGender;
                        Session["StaffEmailAddress"] = model.StaffEmailAddress;
                        return RedirectToAction("StaffContactDetails");
                    }
                }
            }



            setViewBagForCountries();
            return View(model);

        }

        public ActionResult getCountryCode(string countryCode)
        {
            var code = CommonService.getPhoneCodeFromCountry(countryCode);
            return Json(new
            {
                CountryCode = code
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult StaffContactDetails()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            setViewBagForCountries();
            return View();
        }


        [HttpPost]
        public ActionResult StaffContactDetails([Bind(Include = StaffContactDetailsViewModel.BindProperty)]StaffContactDetailsViewModel model)
        {
            bool isValid = true;
            if (model != null)
            {
                if (string.IsNullOrEmpty(model.Address1))
                {
                    ModelState.AddModelError("Address1", "This field can't be blank.");
                    isValid = false;
                }
                else if (string.IsNullOrEmpty(model.City))
                {
                    ModelState.AddModelError("City", "This field can't be blank.");
                    isValid = false;
                }
                else if (string.IsNullOrEmpty(model.State))
                {
                    ModelState.AddModelError("State", "This field can't be blank.");
                    isValid = false;
                }
                else if (string.IsNullOrEmpty(model.PostalCode))
                {
                    ModelState.AddModelError("PostalCode", "This field can't be blank.");
                    isValid = false;
                }
                else if (string.IsNullOrEmpty(model.Country))
                {
                    ModelState.AddModelError("Country", "This field can't be blank.");
                    isValid = false;
                }
                else if (string.IsNullOrEmpty(model.PhoneNumber))
                {
                    ModelState.AddModelError("PhoneNumber", "This field can't be blank.");
                    isValid = false;
                }
                else if (model.BeenLivingSince == 0)
                {
                    ModelState.AddModelError("BeenLivingSince", "This field can't be blank.");
                    isValid = false;
                }

                if (isValid == true)
                {
                    Common.AdminSession.StaffContactDetails = model;
                    Session["Address1"] = model.Address1;
                    Session["Address2"] = model.Address2;
                    Session["City"] = model.City;
                    Session["State"] = model.State;
                    Session["PostalCode"] = model.PostalCode;
                    Session["Country"] = model.Country;
                    Session["PhoneNumber"] = model.PhoneNumber;
                    Session["BeenLivingSince"] = model.BeenLivingSince;

                    if ((int)model.BeenLivingSince <= (int)Staff.ViewModels.BeenLivingSince.ThreeYear)
                    {
                        return RedirectToAction("StaffContactDetails2");
                    }
                    else
                    {
                        return RedirectToAction("StaffNextOfKin");
                    }

                }
            }
            setViewBagForCountries();
            return View(model);
        }

        [HttpGet]
        public ActionResult StaffContactDetails2()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            setViewBagForCountries();
            return View();
        }

        [HttpPost]
        public ActionResult StaffContactDetails2([Bind(Include = StaffContactDetails2ViewModel.BindProperty)]StaffContactDetails2ViewModel model)
        {
            bool isValid = true;
            if (model != null)
            {
                if (string.IsNullOrEmpty(model.Address1))
                {
                    ModelState.AddModelError("Address1", "This field can't be blank.");
                    isValid = false;
                }
                else if (string.IsNullOrEmpty(model.City))
                {
                    ModelState.AddModelError("City", "This field can't be blank.");
                    isValid = false;
                }
                else if (string.IsNullOrEmpty(model.State))
                {
                    ModelState.AddModelError("State", "This field can't be blank.");
                    isValid = false;
                }
                else if (string.IsNullOrEmpty(model.PostalCode))
                {
                    ModelState.AddModelError("PostalCode", "This field can't be blank.");
                    isValid = false;
                }
                else if (string.IsNullOrEmpty(model.Country))
                {
                    ModelState.AddModelError("Country", "This field can't be blank.");
                    isValid = false;
                }
                else if (string.IsNullOrEmpty(model.PhoneNumber))
                {
                    ModelState.AddModelError("PhoneNumber", "This field can't be blank.");
                    isValid = false;
                }
                else if (model.BeenLivingSince == 0)
                {
                    ModelState.AddModelError("BeenLivingSince", "This field can't be blank.");
                    isValid = false;
                }

                if (isValid == true)
                {
                    Common.AdminSession.StaffContactDetails2 = model;
                    Session["Address1"] = model.Address1;
                    Session["Address2"] = model.Address2;
                    Session["City"] = model.City;
                    Session["State"] = model.State;
                    Session["PostalCode"] = model.PostalCode;
                    Session["Country"] = model.Country;
                    Session["PhoneNumber"] = model.PhoneNumber;
                    Session["BeenLivingSince"] = model.BeenLivingSince;

                    if ((int)model.BeenLivingSince + (int)AdminSession.StaffContactDetails.BeenLivingSince <= (int)Staff.ViewModels.BeenLivingSince.ThreeYear)
                    {
                        return RedirectToAction("StaffContactDetails3");
                    }
                    else
                    {
                        return RedirectToAction("StaffNextOfKin");
                    }

                }
            }

            setViewBagForCountries();
            return View(model);
        }

        [HttpGet]
        public ActionResult StaffContactDetails3()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            setViewBagForCountries();
            return View();
        }


        [HttpPost]
        public ActionResult StaffContactDetails3([Bind(Include = StaffContactDetails3ViewModel.BindProperty)]StaffContactDetails3ViewModel model)
        {
            bool isValid = true;
            if (model != null)
            {
                if (string.IsNullOrEmpty(model.Address1))
                {
                    ModelState.AddModelError("Address1", "This field can't be blank.");
                    isValid = false;
                }
                else if (string.IsNullOrEmpty(model.City))
                {
                    ModelState.AddModelError("City", "This field can't be blank.");
                    isValid = false;
                }
                else if (string.IsNullOrEmpty(model.State))
                {
                    ModelState.AddModelError("State", "This field can't be blank.");
                    isValid = false;
                }
                else if (string.IsNullOrEmpty(model.PostalCode))
                {
                    ModelState.AddModelError("PostalCode", "This field can't be blank.");
                    isValid = false;
                }
                else if (string.IsNullOrEmpty(model.Country))
                {
                    ModelState.AddModelError("Country", "This field can't be blank.");
                    isValid = false;
                }
                else if (string.IsNullOrEmpty(model.PhoneNumber))
                {
                    ModelState.AddModelError("PhoneNumber", "This field can't be blank.");
                    isValid = false;
                }
                else if (model.BeenLivingSince == 0)
                {
                    ModelState.AddModelError("BeenLivingSince", "This field can't be blank.");
                    isValid = false;
                }

                if (isValid == true)
                {
                    Common.AdminSession.StaffContactDetails3 = model;
                    Session["Address1"] = model.Address1;
                    Session["Address2"] = model.Address2;
                    Session["City"] = model.City;
                    Session["State"] = model.State;
                    Session["PostalCode"] = model.PostalCode;
                    Session["Country"] = model.Country;
                    Session["PhoneNumber"] = model.PhoneNumber;
                    Session["BeenLivingSince"] = model.BeenLivingSince;

                    return RedirectToAction("StaffNextOfKin");
                }
            }

            setViewBagForCountries();
            return View(model);
        }



        [HttpGet]
        public ActionResult StaffNextOfKin()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            setViewBagForCountries();
            return View();
        }

        [HttpPost]
        public ActionResult StaffNextOfKin([Bind(Include = StaffNextOfKinViewModel.BindProperty)]StaffNextOfKinViewModel model)
        {
            bool valid = true;
            if (model != null)
            {
                if (string.IsNullOrEmpty(model.NOKFirstName))
                {
                    ModelState.AddModelError("NOKFirstName", "This field can't be blank !");
                    valid = false;
                }
                else if (string.IsNullOrEmpty(model.NOKLastName))
                {
                    ModelState.AddModelError("NOKLastName", "This field can't be blank !");
                    valid = false;
                }
                else if (string.IsNullOrEmpty(model.NOKAddress1))
                {
                    ModelState.AddModelError("NOKAddress1", "This field can't be blank !");
                    valid = false;
                }
                else if (string.IsNullOrEmpty(model.NOKCity))
                {
                    ModelState.AddModelError("NOKCity", "This field can't be blank !");
                    valid = false;
                }
                else if (string.IsNullOrEmpty(model.NOKState))
                {
                    ModelState.AddModelError("NOKState", "This field can't be blank !");
                    valid = false;
                }
                else if (string.IsNullOrEmpty(model.NOKPostalCode))
                {
                    ModelState.AddModelError("NOKPostalCode", "This field can't be blank !");
                    valid = false;
                }
                else if (string.IsNullOrEmpty(model.NOKCountry))
                {
                    ModelState.AddModelError("NOKCountry", "This field can't be blank !");
                    valid = false;
                }
                else if (string.IsNullOrEmpty(model.NOKPhoneNumber))
                {
                    ModelState.AddModelError("NOKPhoneNumber", "This field can't be blank !");
                    valid = false;
                }
                else if (string.IsNullOrEmpty(model.NOKRelationship))
                {
                    ModelState.AddModelError("NOKRelationship", "This field can't be blank !");
                    valid = false;
                }

                if (valid == true)
                {
                    Common.AdminSession.StaffNOKDetails = model;
                    Session["NOKFirstName"] = model.NOKFirstName;
                    Session["NOKMiddleName"] = model.NOKMiddleName;
                    Session["NOKLastName"] = model.NOKLastName;
                    Session["NOKAddress1"] = model.NOKAddress1;
                    Session["NOKAddress2"] = model.NOKAddress2;
                    Session["NOKCity"] = model.NOKCity;
                    Session["NOKState"] = model.NOKState;
                    Session["NOKPostalCode"] = model.NOKPostalCode;
                    Session["NOKCountry"] = model.NOKCountry;
                    Session["NOKPhoneNumber"] = model.NOKPhoneNumber;
                    Session["NOKRelationship"] = model.NOKRelationship;

                    return RedirectToAction("StaffComplianceDocumentation");
                }
            }

            setViewBagForCountries();
            return View(model);
        }

        [HttpGet]
        public ActionResult StaffComplianceDocumentation()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            return View();
        }

        [HttpPost]
        public ActionResult StaffComplianceDocumentation([Bind(Include = StaffComplianceDocumentationViewModel.BindProperty)]StaffComplianceDocumentationViewModel model)
        {
            if (model != null)
            {

                string[] fileName = new string[6];

                if (Request.Files.Count > 0)
                {
                    string directory = Server.MapPath("/Documents");
                    for (int i = 0; i < Request.Files.Count; i++)
                    {

                        var residentPermit = Request.Files["ResidencePermit"];
                        var passport1 = Request.Files["Passport1"];
                        var passport2 = Request.Files["Passport2"];
                        var UtilityBill = Request.Files["UtilityBill"];
                        var CV = Request.Files["CV"];
                        var HighestQual = Request.Files["Qualification"];

                        if (string.IsNullOrEmpty(residentPermit.FileName))
                        {
                            ModelState.AddModelError("ResidencePermitIDCardUrl", "Please choose a file.");
                            return View();
                        }
                        else if (string.IsNullOrEmpty(passport1.FileName))
                        {
                            ModelState.AddModelError("PassportSide1Url", "Please choose a file.");
                            return View();
                        }
                        else if (string.IsNullOrEmpty(passport2.FileName))
                        {
                            ModelState.AddModelError("PassportSide2Url", "Please choose a file.");
                            return View();
                        }
                        else if (string.IsNullOrEmpty(UtilityBill.FileName))
                        {
                            ModelState.AddModelError("UtilityBillUrl", "Please choose a file.");
                            return View();
                        }
                        else if (string.IsNullOrEmpty(CV.FileName))
                        {
                            ModelState.AddModelError("CVUrl", "Please choose a file.");
                            return View();
                        }
                        else if (string.IsNullOrEmpty(HighestQual.FileName))
                        {
                            ModelState.AddModelError("HighestQualificationUrl", "Please choose a file.");
                            return View();
                        }



                        var upload = Request.Files[i];
                        if (upload != null && upload.ContentLength > 0)
                        {
                            fileName[i] = Guid.NewGuid() + "." + upload.FileName.Split('.')[1];
                            upload.SaveAs(Path.Combine(directory, fileName[i]));
                        }
                    }
                    model.ResidencePermitIDCardUrl = "/Documents/" + fileName[0];
                    model.PassportSide1Url = "/Documents/" + fileName[1];
                    model.PassportSide2Url = "/Documents/" + fileName[2];
                    model.UtilityBillUrl = "/Documents/" + fileName[3];
                    model.CVUrl = "/Documents/" + fileName[4];
                    model.HighestQualificationUrl = "/Documents/" + fileName[5];

                    if (ModelState.IsValid)
                    {
                        Common.AdminSession.StaffDocumentsDetails = model;
                        Session["ResidencePermitIDCardUrl"] = model.ResidencePermitIDCardUrl;
                        Session["PassportSide1Url"] = model.PassportSide1Url;
                        Session["PassportSide2Url"] = model.PassportSide2Url;
                        Session["UtilityBillUrl"] = model.UtilityBillUrl;
                        Session["CVUrl"] = model.CVUrl;
                        Session["HighestQualificationUrl"] = model.HighestQualificationUrl;

                        return RedirectToAction("StaffEmployment");
                    }


                }


            }

            return View(model);
        }

        [HttpGet]
        public ActionResult StaffEmployment()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            return View();
        }

        [HttpPost]
        public ActionResult StaffEmployment([Bind(Include = StaffEmploymentViewModel.BindProperty)]StaffEmploymentViewModel model)
        {
            bool valid = true;
            if (model != null)
            {
                if (string.IsNullOrEmpty(model.Position))
                {
                    ModelState.AddModelError("Position", "This field can't be blank !");
                    valid = false;
                }
                else if (model.Salary == 0)
                {
                    ModelState.AddModelError("Salary", "This field can't be blank !");
                    valid = false;
                }
                else if (model.ModeOfJob == null)
                {
                    ModelState.AddModelError("ModeOfJob", "This field can't be blank !");
                    valid = false;
                }
                else if (string.IsNullOrEmpty(model.EmploymentDate.ToString()))
                {
                    ModelState.AddModelError("EmploymentDate", "This field can't be blank !");
                    valid = false;
                }
                else if (string.IsNullOrEmpty(model.DateOfLeaving.ToString()))
                {
                    ModelState.AddModelError("EmploymentDate", "This field can't be blank !");
                    valid = false;
                }
                if (valid == true)
                {
                    Common.AdminSession.StaffEmploymentDetails = model;
                    Session["Position"] = model.Position;
                    Session["Salary"] = model.Salary;
                    Session["ModeOfJob"] = model.ModeOfJob;
                    Session["EmploymentDate"] = model.EmploymentDate;
                    Session["DateOfLeaving"] = model.DateOfLeaving;



                    //getting MFS Code for staff
                    StaffSignUpServices staffServices = new StaffSignUpServices();
                    string staffMFSCode = staffServices.GetNewAccount(10);

                    var basicDetails = Common.AdminSession.StaffBasicDetails;
                    var contactDetails = Common.AdminSession.StaffContactDetails;
                    var contactDetails2 = Common.AdminSession.StaffContactDetails2;
                    var contactDetails3 = AdminSession.StaffContactDetails3;
                    var nokDetails = Common.AdminSession.StaffNOKDetails;
                    var documentDetails = Common.AdminSession.StaffDocumentsDetails;
                    var employmentDetails = Common.AdminSession.StaffEmploymentDetails;

                    //inserting into database
                    DB.StaffInformation staffInformation = new DB.StaffInformation()
                    {
                        FirstName = basicDetails.StaffFirstName,
                        MiddleName = basicDetails.StaffMiddleName,
                        LastName = basicDetails.StaffLastName,
                        DateOfBirth = basicDetails.StaffDOB,
                        BirthCountry = basicDetails.StaffBirthCountry,
                        Gender = (int)basicDetails.StaffGender,
                        EmailAddress = basicDetails.StaffEmailAddress,

                        Address1 = contactDetails.Address1,
                        Address2 = contactDetails.Address2,
                        City = contactDetails.City,
                        State = contactDetails.State,
                        PostalCode = contactDetails.PostalCode,
                        Country = contactDetails.Country,
                        PhoneNumber = contactDetails.PhoneNumber,
                        BeenLivingSince = contactDetails.BeenLivingSince,
                        StaffMFSCode = staffMFSCode,


                        NOKFirstName = nokDetails.NOKFirstName,
                        NOKMiddleName = nokDetails.NOKMiddleName,
                        NOKLastName = nokDetails.NOKLastName,
                        NOKAddress1 = nokDetails.NOKAddress1,
                        NOKAddress2 = nokDetails.NOKAddress2,
                        NOKCity = nokDetails.NOKCity,
                        NOKState = nokDetails.NOKState,
                        NOKPostalCode = nokDetails.NOKPostalCode,
                        NOKCountry = nokDetails.NOKCountry,
                        NOKPhoneNumber = nokDetails.NOKPhoneNumber,
                        NOKRelationship = nokDetails.NOKRelationship,

                        ResidencePermitUrl = documentDetails.ResidencePermitIDCardUrl,
                        PassportSide1Url = documentDetails.PassportSide1Url,
                        PassportSide2Url = documentDetails.PassportSide2Url,
                        UtilityBillUrl = documentDetails.UtilityBillUrl,
                        CVUrl = documentDetails.CVUrl,
                        HighestQualificationUrl = documentDetails.HighestQualificationUrl
                    };

                    staffInformation = Service.SaveStaffInformation(staffInformation);
                    if (staffInformation != null)
                    {
                        if (contactDetails2 != null)
                        {
                            DB.StaffAddresses contactDetails2Data = new DB.StaffAddresses()
                            {
                                StaffId = staffInformation.Id,
                                StaffAddress1 = contactDetails2.Address1,
                                StaffAddress2 = contactDetails2.Address2,
                                StaffCity = contactDetails2.City,
                                StaffCountry = contactDetails2.Country,
                                StaffPhoneNumber = contactDetails2.PhoneNumber,
                                StaffPostalCode = contactDetails2.PostalCode,
                                StaffState = contactDetails2.State,
                                BeenLivingSince = contactDetails2.BeenLivingSince
                            };
                            contactDetails2Data = Service.SaveStaffAddresses(contactDetails2Data);
                            if (contactDetails2Data != null)
                            {
                                if (contactDetails3 != null)
                                {
                                    DB.StaffAddresses contactDetails3Data = new DB.StaffAddresses()
                                    {
                                        StaffId = staffInformation.Id,
                                        StaffAddress1 = contactDetails3.Address1,
                                        StaffAddress2 = contactDetails3.Address2,
                                        StaffCity = contactDetails3.City,
                                        StaffState = contactDetails3.State,
                                        StaffPostalCode = contactDetails3.PostalCode,
                                        StaffCountry = contactDetails3.Country,
                                        StaffPhoneNumber = contactDetails3.PhoneNumber,
                                        BeenLivingSince = contactDetails3.BeenLivingSince
                                    };
                                    contactDetails3Data = Service.SaveStaffAddresses(contactDetails3Data);

                                }
                            }

                        }

                        //inserting into staffEmployment table
                        DB.StaffEmployment staffEmploymentData = new DB.StaffEmployment()
                        {
                            StaffId = staffInformation.Id,
                            Position = employmentDetails.Position,
                            Salary = employmentDetails.Salary,
                            Mode = (ModeOfJob)employmentDetails.ModeOfJob,
                            EmploymentDate = employmentDetails.EmploymentDate,
                            LeavingDate = employmentDetails.DateOfLeaving
                        };
                        staffEmploymentData = Service.saveEmploymentDetails(staffEmploymentData);

                        //inserting into staffLogin Table
                        var guId = Guid.NewGuid().ToString();
                        var LoginCode = Service.GetNewLoginCode(5);
                        DB.StaffLogin login = new DB.StaffLogin()
                        {
                            StaffId = staffInformation.Id,
                            UserName = staffInformation.EmailAddress,
                            Password = null,
                            ActivationCode = guId,
                            LoginCode = LoginCode,
                            IsActive = false
                        };
                        login = Service.AddStaffLogin(login);

                        //send activation link to the user
                        MailCommon mail = new MailCommon();
                        var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                        var link = string.Format("{0}/Staff/StaffSignUp/FirstLogin?activationcode={1}", baseUrl, login.ActivationCode);
                        try
                        {
                            string body = "";

                            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/StaffActivationEmail?NameOfContactPerson=" + staffInformation.FirstName + " " + staffInformation.LastName
                                + "&EmailAddress=" + staffInformation.EmailAddress + "&LoginCode=" + login.LoginCode + "&Link=" + link);


                            mail.SendMail("anankarki97@gmail.com", "Staff Registration Confirmation ", body);
                            //string msgBody = "";
                            //mail.SendMail(staffInformation.EmailAddress, "Congratulations ! You have been registered. Please click the below activation link for further processing.", link);

                        }
                        catch (Exception)
                        {
                            throw;
                        }


                    }

                    return RedirectToAction("Index");
                }

            }
            return View(model);

        }


        public ActionResult changeStatus(int staffId)
        {
            if (staffId != 0)
            {
                Service.ChangeStatus(staffId);
            }
            return RedirectToAction("Index");
        }

        public ActionResult deleteStaff(int staffId)
        {
            if (staffId != 0)
            {
                Service.deleteStaff(staffId);
            }
            return RedirectToAction("Index");
        }


        private void setViewBagForCountries()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }
        public JsonResult ChangeSysLoginLevel(int staffId, int sysValue)
        {
            var result = Service.changeSystemLoginLevel(staffId, sysValue);
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public JsonResult ChangeMode(int staffId, int mode)
        {
            var result = Service.ChangeMode(staffId, mode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeEmpDate(int staffId, DateTime date)
        {
            var result = Service.ChangeEmploymentDate(staffId, date);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeLeavingDate(int staffId, DateTime date)
        {
            var result = Service.ChangeLeavingDate(staffId, date);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeStartTime(int staffId, DateTime time)
        {
            var result = Service.ChangeStartTime(staffId, time);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeEndTime(int staffId, DateTime time)
        {
            var result = Service.ChangeEndTime(staffId, time);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeStartDay(int staffId, DayOfWeek day)
        {
            var result = Service.ChangeLoginStartDay(staffId, day);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeEndDay(int staffId, DayOfWeek day)
        {
            var result = Service.ChangeLoginEndDay(staffId, day);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeHolidaysEntitlement(int staffId, StaffHolidaysEntiltlement value)
        {
            var result = Service.ChangeHolidaysEntitlement(staffId, value);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeNoOfHolidays(int staffId, int noOfDays)
        {
            var result = Service.ChangeNoOfHolidays(staffId, noOfDays);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult changePermitType(int staffId, string type)
        {
            var result = Service.changePermitType(staffId, type);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ChangeExpiryDate(int staffId, DateTime date)
        {
            var result = Service.changeExpiryDate(staffId, date);
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public void SetViewBagForSCities(string Country = "")
        {
            var cities = SCity.GetCities(DB.Module.Staff, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }
        private void SetViewBagForCountries()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }

        public ActionResult sendEmails(string body = "", string subject = "", string emails = "")
        {
            if (!string.IsNullOrEmpty(body) && !string.IsNullOrEmpty(subject) && !(string.IsNullOrEmpty(emails)))
            {
                bool result = Common.Common.sendBulkMail(emails, subject, body);
                if (result)
                {
                    return RedirectToAction("Index", new { @message = "mailSent" });
                }

            }
            return RedirectToAction("Index", new { @message = "mailNotSent" });
        }

    }
}