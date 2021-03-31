using FAXER.PORTAL.Areas.Staff.Services;
using FAXER.PORTAL.Areas.Staff.ViewModels;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace FAXER.PORTAL.Areas.Staff.Controllers
{
    public class StaffSignUpController : Controller
    {
        CommonServices CommonService = new CommonServices();
        // GET: Staff/StaffSignUp
        [HttpGet]
        public ActionResult Index()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = StaffDetailsViewModel.BindProperty)]StaffDetailsViewModel model)
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");


            if (string.IsNullOrEmpty(model.StaffFirstName))
            {
                ModelState.AddModelError("StaffFirstName", "The Field is Required");
                return View(model);

            }
            if (string.IsNullOrEmpty(model.StaffLastName))
            {
                ModelState.AddModelError("StaffLastName", "The Field is Required");
                return View(model);

            }
            if (string.IsNullOrEmpty(model.StaffEmailAddress))
            {
                ModelState.AddModelError("StaffEmailAddress", "The Field is Required");
                return View(model);

            }
            if (string.IsNullOrEmpty(model.CofirmStaffEmailAddress))
            {
                ModelState.AddModelError("CofirmStaffEmailAddress", "The Field is Required");
                return View(model);

            }
            if (model.StaffDateOfBirth == default(DateTime))
            {
                ModelState.AddModelError("StaffDateOfBirth", "The Field is Required");
                return View(model);

            }
            if (string.IsNullOrEmpty(model.StaffBirthCountry))
            {
                ModelState.AddModelError("StaffBirthCountry", "The Field is Required");
                return View(model);

            }
            if (model.StaffEmailAddress != model.CofirmStaffEmailAddress)
            {
                ModelState.AddModelError("CofirmStaffEmailAddress", "Email Address Did Not Match");
                return View(model);
            }
            Services.StaffSignUpServices services = new StaffSignUpServices();
            var emailExist = services.GetStaffByEmail(model.StaffEmailAddress);
            bool isValidAge = Common.DateUtilities.ValidateAge(model.StaffDateOfBirth);
            if (emailExist != null)
            {

                ModelState.AddModelError("CofirmStaffEmailAddress", "Email Already Exist");
                return View(model);
            }
            if (isValidAge == false)
            {
                ModelState.AddModelError("StaffDateOfBirth", "Age must be Greater than 18");
                return View(model);
            }
            
                Common.StaffSession.StaffDetails = model;
                Session["StaffFirstName"] = model.StaffFirstName;
                Session["StaffMiddleName"] = model.StaffMiddleName;
                Session["StaffLastName"] = model.StaffLastName;
                Session["StaffDateOfBirth"] = model.StaffDateOfBirth;
                Session["StaffBirthCountry"] = model.StaffBirthCountry;
                Session["StaffGender"] = model.StaffGender;
                Session["StaffEmailAddress"] = model.StaffEmailAddress;

                return RedirectToAction("StaffContactDetails");
            

        }


        [HttpGet]
        public ActionResult StaffContactDetails()
        {

            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            if (Common.StaffSession.StaffDetails == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public ActionResult StaffContactDetails([Bind(Include = StaffContactDetailsViewModel.BindProperty)]StaffContactDetailsViewModel model)
        {

            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            bool isValid = true;
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.StaffAddress1))
                {
                    ModelState.AddModelError("StaffAddress1", "This field can't be blank.");
                    isValid = false;
                }
                //else if (string.IsNullOrEmpty(model.StaffAddress2))
                //{
                //    ModelState.AddModelError("StaffAddress2", "This field can't be blank.");
                //    isValid = false;
                //}
                else if (string.IsNullOrEmpty(model.StaffCity))
                {
                    ModelState.AddModelError("StaffCity", "This field can't be blank.");
                    isValid = false;
                }
                else if (string.IsNullOrEmpty(model.StaffPostalCode))
                {
                    ModelState.AddModelError("StaffPostalCode", "This field can't be blank.");
                    isValid = false;
                }
                else if (string.IsNullOrEmpty(model.StaffCountry))
                {
                    ModelState.AddModelError("StaffCountry", "This field can't be blank.");
                    isValid = false;
                }
                else if (string.IsNullOrEmpty(model.StaffPhoneNumber))
                {
                    ModelState.AddModelError("StaffPhoneNumber", "This field can't be blank.");
                    isValid = false;
                }
                else if (string.IsNullOrEmpty(model.BeenLivingSince.ToString()))
                {
                    ModelState.AddModelError("BeenLivingSince", "This field can't be blank.");
                    isValid = false;
                }
            }
            if (isValid == false)
            {
                return View(model);
            }
            if (isValid == true)
            {
                Common.StaffSession.StaffContactDetails = model;
                Session["StaffAddress1"] = model.StaffAddress1;
                Session["StaffAddress2"] = model.StaffAddress2;
                Session["StaffCity"] = model.StaffCity;
                Session["StaffCountry"] = model.StaffCountry;
                Session["StaffPhoneNumber"] = model.StaffPhoneNumber;
                Session["StaffPostalCode"] = model.StaffPostalCode;
                Session["StaffState"] = model.StaffState;
                Session["BeenLivingSince"] = model.BeenLivingSince;

                if (model.BeenLivingSince < BeenLivingSince.ThreeYear)
                {
                    return RedirectToAction("StaffContactDetails1");
                }
                else
                {
                    return RedirectToAction("StaffNextOfKin");
                }
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult StaffContactDetails1()
        {

            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            if (Common.StaffSession.StaffContactDetails == null)
            {
                return RedirectToAction("StaffContactDetails");
            }
            return View();
        }
        [HttpPost]
        public ActionResult StaffContactDetails1([Bind(Include = StaffContactDetails_1ViewModel.BindProperty)]ViewModels.StaffContactDetails_1ViewModel model)
        {
            var StaffContactDetails = Common.StaffSession.StaffContactDetails;
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            if (ModelState.IsValid)
            {

                if (Common.StaffSession.StaffContactDetails.StaffAddress1.ToLower() == model.StaffAddress1.ToLower())
                {

                    ModelState.AddModelError("StaffAddress1", "Address cannot be same to previous Address");
                    return View(model);
                }
                if (Common.StaffSession.StaffContactDetails.StaffCity.ToLower() == model.StaffCity.ToLower())
                {

                    ModelState.AddModelError("StaffCity", "City cannot be same to previous City");
                    return View(model);
                }
                if (model.BeenLivingSince == null)
                {
                    ModelState.AddModelError("BeenLivingSince", "The Field is required");
                    return View(model);
                }
                Common.StaffSession.StaffContactDetails1 = model;
                Session["StaffAddress1"] = model.StaffAddress1;
                Session["StaffAddress2"] = model.StaffAddress2;
                Session["StaffCity"] = model.StaffCity;
                Session["StaffCountry"] = model.StaffCountry;
                Session["StaffPhoneNumber"] = model.StaffPhoneNumber;
                Session["StaffPostalCode"] = model.StaffPostalCode;
                Session["StaffState"] = model.StaffState;
                Session["BeenLivingSince"] = model.BeenLivingSince;

                if (((int)model.BeenLivingSince + (int)StaffContactDetails.BeenLivingSince) < (int)BeenLivingSince.ThreeYear)
                {
                    return RedirectToAction("StaffContactDetails2");
                }
                else
                {
                    return RedirectToAction("StaffNextOfKin");
                }
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult StaffContactDetails2()
        {

            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            if (Common.StaffSession.StaffContactDetails1 == null)
            {
                return RedirectToAction("StaffContactDetails1");
            }
            return View();
        }
        [HttpPost]
        public ActionResult StaffContactDetails2([Bind(Include = StaffContactDetails_2ViewModel.BindProperty)]ViewModels.StaffContactDetails_2ViewModel model)
        {

            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            if (ModelState.IsValid)
            {

               
                if (Common.StaffSession.StaffContactDetails.StaffAddress1.ToLower() == model.StaffAddress1.ToLower() || Common.StaffSession.StaffContactDetails1.StaffAddress1.ToLower() == model.StaffAddress1.ToLower())
                {
                    ModelState.AddModelError("StaffAddress1", "Address cannot be same to Previous Addresses ");
                    return View(model);
                }

                if (Common.StaffSession.StaffContactDetails.StaffCity.ToLower() == model.StaffCity.ToLower() || Common.StaffSession.StaffContactDetails1.StaffCity.ToLower() == model.StaffCity.ToLower())
                {
                    ModelState.AddModelError("StaffCity", "City cannot be same to Previous City ");
                    return View(model);
                }
                if (model.BeenLivingSince == null)
                {
                    ModelState.AddModelError("BeenLivingSince", "The Field is required");
                    return View(model);
                }
                Common.StaffSession.StaffContactDetails2 = model;
                Session["StaffAddress1"] = model.StaffAddress1;
                Session["StaffAddress2"] = model.StaffAddress2;
                Session["StaffCity"] = model.StaffCity;
                Session["StaffCountry"] = model.StaffCountry;
                Session["StaffPhoneNumber"] = model.StaffPhoneNumber;
                Session["StaffPostalCode"] = model.StaffPostalCode;
                Session["StaffState"] = model.StaffState;
                Session["BeenLivingSince"] = model.BeenLivingSince;


                return RedirectToAction("StaffNextOfKin");


            }
            return View();
        }
        [HttpGet]
        public ActionResult StaffNextOfKin()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            if (Common.StaffSession.StaffContactDetails == null)
            {
                return RedirectToAction("StaffContactDetails");
            }
            if (Common.StaffSession.StaffContactDetails != null && Common.StaffSession.StaffContactDetails1 == null)
            {
                if (Common.StaffSession.StaffContactDetails.BeenLivingSince < BeenLivingSince.ThreeYear)
                {
                    return RedirectToAction("StaffContactDetails1");
                }



            }
            if (Common.StaffSession.StaffContactDetails1 != null && Common.StaffSession.StaffContactDetails2 == null)
            {
                if ((int)Common.StaffSession.StaffContactDetails1.BeenLivingSince + (int)Common.StaffSession.StaffContactDetails.BeenLivingSince < (int)BeenLivingSince.ThreeYear)
                {
                    return RedirectToAction("StaffContactDetails2");
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult StaffNextOfKin([Bind(Include = StaffNextOfKinViewModel.BindProperty)]ViewModels.StaffNextOfKinViewModel model)
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            if (Common.StaffSession.StaffDetails.StaffFirstName.ToLower() == model.NOKFirstName.ToLower())
            {
                ModelState.AddModelError("NOKFirstName", "Kin Name and Staff Name Cannot be same");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                Common.StaffSession.StaffNextOfKinDetails = model;
                Session["NOKAddress1"] = model.NOKAddress1;
                Session["NOKAddress2"] = model.NOKAddress2;
                Session["NOKCity"] = model.NOKCity;
                Session["NOKCountry"] = model.NOKCountry;
                Session["NOKFirstName"] = model.NOKFirstName;
                Session["NOKMiddleName"] = model.NOKMiddleName;
                Session["NOKLastName"] = model.NOKLastName;
                Session["NOKPhoneNumber"] = model.NOKPhoneNumber;
                Session["NOKPostalCode"] = model.NOKPostalCode;
                Session["NOKRelationship"] = model.NOKRelationship;
                Session["NOKState"] = model.NOKState;

                return RedirectToAction("StaffComplianceDocumentation");

            }
            return View(model);
        }
        [HttpGet]
        public ActionResult StaffComplianceDocumentation()
        {
            //if (Common.StaffSession.StaffNextOfKinDetails == null)
            //{
            //    return RedirectToAction("StaffNextOfKin");
            //}
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult StaffComplianceDocumentation([Bind(Include = StaffComplianceDocumentationViewModel.BindProperty)]ViewModels.StaffComplianceDocumentationViewModel model)
        {
            string[] fileName = new string[6];
            if (Request.Files.Count > 0)
            {
                string directory = Server.MapPath("/Documents");
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var residentPermit = Request.Files["ResidentPermit"];
                    var passport1 = Request.Files["Passport1"];
                    var passport2 = Request.Files["Passport2"];
                    var UtilityBill = Request.Files["UtilityBill"];
                    var CV = Request.Files["CV"];
                    var HighestQual = Request.Files["HighestQual"];
                    
                    if (string.IsNullOrEmpty(residentPermit.FileName))
                    {
                        ModelState.AddModelError("ResidencePermitUrl", "Please Choose All files");
                        return View();
                    }
                    if (string.IsNullOrEmpty(passport1.FileName))
                    {
                        ModelState.AddModelError("PassportSide1Url", "Please Choose All files");
                        return View();
                    }
                    if (string.IsNullOrEmpty(passport2.FileName))
                    {
                        ModelState.AddModelError("PassportSide2Url", "Please Choose All files");
                        return View();
                    }
                    if (string.IsNullOrEmpty(UtilityBill.FileName))
                    {
                        ModelState.AddModelError("UtilityBillUrl", "Please Choose All files");
                        return View();
                    }
                    if (string.IsNullOrEmpty(CV.FileName))
                    {
                        ModelState.AddModelError("CVUrl", "Please Choose All files");
                        return View();
                    }
                    if (string.IsNullOrEmpty(HighestQual.FileName))
                    {
                        ModelState.AddModelError("HighestQualificationUrl", "Please Choose All files");
                        return View();
                    }

                    var upload = Request.Files[i];
                    if (upload != null && upload.ContentLength > 0)
                    {
                        fileName[i] = Guid.NewGuid() + "." + upload.FileName.Split('.')[1]; //Path.GetFileName(upload.FileName);
                        upload.SaveAs(Server.MapPath("~/Documents") + "\\" + fileName[i]);

                    }
                }
                model.ResidencePermitUrl = "/Documents/" + fileName[0];
                model.PassportSide1Url = "/Documents/" + fileName[1];
                model.PassportSide2Url = "/Documents/" + fileName[2];
                model.UtilityBillUrl = "/Documents/" + fileName[3];
                model.CVUrl = "/Documents/" + fileName[4];
                model.HighestQualificationUrl = "/Documents/" + fileName[5];

            }

            if (ModelState.IsValid)
            {


                var StaffDetails = Common.StaffSession.StaffDetails;
                var StaffContactDetails = Common.StaffSession.StaffContactDetails;
                var StaffNextToKin = Common.StaffSession.StaffNextOfKinDetails;
                var StaffContactDetails1 = Common.StaffSession.StaffContactDetails1;
                var StaffContactDetails2 = Common.StaffSession.StaffContactDetails2;
                Services.StaffSignUpServices staffServices = new Services.StaffSignUpServices();
                string staffMFSCode = staffServices.GetNewAccount(7);
                DB.StaffInformation staffInformation = new DB.StaffInformation()
                {
                    FirstName = StaffDetails.StaffFirstName.Trim(),
                    MiddleName = StaffDetails.StaffMiddleName,
                    LastName = StaffDetails.StaffLastName.Trim(),
                    DateOfBirth = StaffDetails.StaffDateOfBirth,
                    BirthCountry = StaffDetails.StaffBirthCountry,
                    EmailAddress = StaffDetails.StaffEmailAddress,
                    Gender = (int)StaffDetails.StaffGender,
                    StaffMFSCode = staffMFSCode,
                    SytemLoginLevelOfStaff = DB.SystemLoginLevel.Level4,
                    Address1 = StaffContactDetails.StaffAddress1,
                    Address2 = StaffContactDetails.StaffAddress2,
                    City = StaffContactDetails.StaffCity,
                    Country = StaffContactDetails.StaffCountry,
                    State = StaffContactDetails.StaffState,
                    PostalCode = StaffContactDetails.StaffPostalCode,
                    PhoneNumber = StaffContactDetails.StaffPhoneNumber.FormatPhoneNo(),
                    BeenLivingSince = (BeenLivingSince)StaffContactDetails.BeenLivingSince,

                    NOKFirstName = StaffNextToKin.NOKFirstName,
                    NOKMiddleName = StaffNextToKin.NOKMiddleName,
                    NOKLastName = StaffNextToKin.NOKLastName,
                    NOKAddress1 = StaffNextToKin.NOKAddress1,
                    NOKAddress2 = StaffNextToKin.NOKAddress2,
                    NOKCity = StaffNextToKin.NOKCity,
                    NOKCountry = StaffNextToKin.NOKCountry,
                    NOKPhoneNumber = StaffNextToKin.NOKPhoneNumber.FormatPhoneNo(),
                    NOKPostalCode = StaffNextToKin.NOKPostalCode,
                    NOKRelationship = StaffNextToKin.NOKRelationship,
                    NOKState = StaffNextToKin.NOKState,

                    PassportSide1Url = model.PassportSide1Url,
                    PassportSide2Url = model.PassportSide2Url,
                    HighestQualificationUrl = model.HighestQualificationUrl,
                    UtilityBillUrl = model.UtilityBillUrl,
                    ResidencePermitUrl = model.ResidencePermitUrl,
                    CVUrl = model.CVUrl,



                };

                staffInformation = staffServices.SignUp(staffInformation);
                if (staffInformation != null)
                {
                    if (StaffContactDetails1 != null)
                    {
                        DB.StaffAddresses contactDetails1 = new DB.StaffAddresses()
                        {
                            StaffId = staffInformation.Id,
                            StaffAddress1 = StaffContactDetails1.StaffAddress1,
                            StaffAddress2 = StaffContactDetails1.StaffAddress2,
                            StaffCity = StaffContactDetails1.StaffCity,
                            StaffCountry = StaffContactDetails1.StaffCountry,
                            StaffPhoneNumber = StaffContactDetails1.StaffPhoneNumber.FormatPhoneNo(),
                            StaffPostalCode = StaffContactDetails1.StaffPostalCode,
                            StaffState = StaffContactDetails1.StaffState,
                            BeenLivingSince = (BeenLivingSince)StaffContactDetails1.BeenLivingSince
                        };
                        contactDetails1 = staffServices.AddStaffAddresses(contactDetails1);
                        if (contactDetails1 != null)
                        {
                            if (StaffContactDetails2 != null)
                            {
                                DB.StaffAddresses contactDetails2 = new DB.StaffAddresses()
                                {
                                    StaffId = staffInformation.Id,
                                    StaffAddress1 = StaffContactDetails2.StaffAddress1,
                                    StaffAddress2 = StaffContactDetails2.StaffAddress2,
                                    StaffCity = StaffContactDetails2.StaffCity,
                                    StaffCountry = StaffContactDetails2.StaffCountry,
                                    StaffPhoneNumber = StaffContactDetails2.StaffPhoneNumber.FormatPhoneNo(),
                                    StaffPostalCode = StaffContactDetails2.StaffPostalCode,
                                    StaffState = StaffContactDetails2.StaffState,
                                    BeenLivingSince = (BeenLivingSince)StaffContactDetails2.BeenLivingSince
                                };
                                contactDetails2 = staffServices.AddStaffAddresses(contactDetails2);
                            }
                        }

                    }
                    var guId = Guid.NewGuid().ToString();
                    var LoginCode = staffServices.GetNewLoginCode(6);
                    DB.StaffLogin login = new DB.StaffLogin()
                    {
                        StaffId = staffInformation.Id,
                        UserName = staffInformation.EmailAddress,
                        Password = null,
                        ActivationCode = guId,
                        LoginCode = LoginCode,
                        IsActive = false,
                        LoginStartTime = "09:00",
                        LoginEndTIme = "17:00",
                        LoginStartDay = DayOfWeek.Monday,
                        LoginEndDay = DayOfWeek.Friday
                    };
                    login = staffServices.AddStaffLogin(login);
                    //send activation link to the user
                    MailCommon mail = new MailCommon();
                    var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                    var link = string.Format("{0}/Staff/StaffSignUp/FirstLogin?activationcode={1}", baseUrl, login.ActivationCode);
                    try
                    {
                        //string mesgBody = "The staff MFSCode " + staffInformation.StaffMFSCode;
                        //mail.SendMail(staffInformation.EmailAddress, "New Staff Has Been registered", link);

                        // GET: EmailTemplate/StaffActivationEmail
                        //string NameOfContactPerson, string EmailAddress, string LoginCode, string Link
                            string body = "";
                        
                        body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/StaffActivationEmail?NameOfContactPerson=" + staffInformation.FirstName + " " + staffInformation.LastName 
                            + "&EmailAddress=" + staffInformation.EmailAddress + "&LoginCode=" + login.LoginCode + "&Link=" + link);

                        mail.SendMail("anege1234@gmail.com", "Staff Confirmation Message", body);
                        mail.SendMail(staffInformation.EmailAddress, "Staff Registration Confirmation ", body);
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    return RedirectToAction("LoginMessage");
                }
                else
                {
                    return View(model);
                }

            }

            return View(model);
        }

        [HttpGet]
        public ActionResult LoginMessage()
        {
            Session.Clear();
            Session.Abandon();
            return View();
        }
        [HttpGet]
        public ActionResult FirstLogin(string activationcode)
        {
            Services.StaffSignUpServices staffServices = new StaffSignUpServices();

            var LoginData = staffServices.GetStaffLoginDetails(activationcode);
            if (LoginData != null)
            {
                ViewModels.StaffLoginViewModel model = new StaffLoginViewModel()
                {
                    StaffEmail = LoginData.UserName,
                    LoginCode = LoginData.LoginCode,
                };
                return View(model);
            }
            return View();
        }
        [HttpPost]
        public ActionResult FirstLogin([Bind(Include = StaffLoginViewModel.BindProperty)]ViewModels.StaffLoginViewModel model)
        {
            Services.StaffSignUpServices staffServices = new StaffSignUpServices();
            if (ModelState.IsValid)
            {

                if (Common.Common.ValidatePassword(model.StaffPassword))
                {
                    if (model.StaffPassword != model.StaffConfirmPassword)
                    {
                        ModelState.AddModelError("StaffConfirmPassword", "Password Did Not Match");
                        return View(model);
                    }
                    var data = staffServices.UpdateStaffLogin(model);
                    if (data != null)
                    {
                        return RedirectToAction("StaffMainLogin", "StaffLogin");
                    }
                }
                else
                {
                    ModelState.AddModelError("StaffPassword", "Password Should Contain Atleast 1 UpperCase, 1 number and 1 Special Character");
                }
            }
            return View(model);
        }
    }
}