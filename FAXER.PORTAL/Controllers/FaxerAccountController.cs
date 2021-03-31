using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Services;
using System.Web.Security;
using FAXER.PORTAL.DB;

namespace FAXER.PORTAL.Controllers
{
    public class FaxerAccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public ApplicationSignInManager SignInManager
        {

            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: FaxerAccount
        public ActionResult Register()
        {
            var country = Common.Common.GetCountries();
            ViewBag.Countries = new SelectList(country, "CountryCode", "CountryName");

            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        //[HandleError]
        public ActionResult Register([Bind(Include = RegisterViewModel.BindProperty)]RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool isValidAge = Common.DateUtilities.ValidateAge(model.DateOfBirth);
                bool isEmailExist = Common.OtherUtlities.IsEmailExist(model.Email);
                if (isEmailExist == false)
                {
                    ModelState.AddModelError("Email", "Email Already Exist");
                }
                if (isValidAge == false)
                {
                    ModelState.AddModelError("DateOfBirth", "You must be 18 years of age or above to sign up to our services");
                }
            }
            if (ModelState.IsValid)
            {
                if (Common.Common.ValidatePassword(model.Password))
                {
                    try
                    {
                        Common.FaxerSession.RegisterViewModel = model;
                        Session["FaxerDetails"] = model.FirstName;
                        Session["FaxerFirstName"] = model.FirstName;
                        Session["FaxerMiddleName"] = model.MiddleName;
                        Session["FaxerLastName"] = model.LastName;
                        Session["FaxerDateOfBirth"] = model.DateOfBirth;
                        Session["FaxerGender"] = model.GGender;
                        Session["FaxerUserName"] = model.Email;
                        Session["FaxerPassword"] = model.Password;

                        return RedirectToAction("Identification");
                    }
                    catch (Exception)
                    {


                    }
                }
                else
                {
                    ModelState.AddModelError("Password", "Password Should Contain Atleast 1 UpperCase, 1 number and 1 Special Character");
                }


            }
            return View(model);
        }


        public ActionResult Identification()
        {
            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            var idCardTypes = dbContext.IdentityCardType.ToList();
            ViewBag.IdCardType = new SelectList(dbContext.IdentityCardType.OrderBy(x => x.CardType), "CardType", "CardType");
            ViewBag.IssuingCountry = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
            return View();

        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Identification([Bind(Include = FaxerIdentification.BindProperty)]FaxerIdentification model)
        {

            var fileName = "";

            if (Request.Files.Count > 0)
            {
                //check model validation
                if (ModelState.IsValid)
                {
                    var upload = Request.Files[0];
                    if (model.CheckAmount == true && upload == null)
                    {
                        ModelState.AddModelError("File", "Please Upload Identity Document");
                    }
                    Common.FaxerSession.FaxerIdentification = model;
                    Session["IdCardType"] = model.IdCardType;
                    Session["IdCardNumber"] = model.IdCardNumber;
                    Session["IdCardExpDate"] = model.IdCardExpiringDate;
                    Session["IdCardIssuingCountry"] = model.IssuingCountry;

                    string directory = Server.MapPath("/Documents");
                    fileName = "";
                    if (upload != null && upload.ContentLength > 0)
                    {
                        fileName = Guid.NewGuid() + "." + upload.FileName.Split('.')[1]; //Path.GetFileName(upload.FileName);
                        upload.SaveAs(Path.Combine(directory, fileName));
                    }
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        Common.FaxerSession.CardUrl = "/Documents/" + fileName;
                    }
                    bool isValidExpityDat = Common.DateUtilities.DateGreaterThanToday(model.IdCardExpiringDate);
                    if (!isValidExpityDat)
                    {
                        ModelState.AddModelError("IdCardExpiringDate", "Date Must Be Greater Than Today");
                        DB.FAXEREntities Context = new DB.FAXEREntities();
                        ViewBag.IdCardType = new SelectList(Context.IdentityCardType.OrderBy(x => x.CardType), "CardType", "CardType");
                        ViewBag.IssuingCountry = new SelectList(Context.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
                        return View(model);
                    }
                    if (model.CheckAmount == true)
                    {
                        if (string.IsNullOrEmpty(fileName))
                        {

                            ModelState.AddModelError("File", "Please Upload Identity Document");
                            DB.FAXEREntities Context = new DB.FAXEREntities();
                            ViewBag.IdCardType = new SelectList(Context.IdentityCardType.OrderBy(x => x.CardType), "CardType", "CardType");
                            ViewBag.IssuingCountry = new SelectList(Context.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
                            return View(model);
                        }
                    }
                    return RedirectToAction("ContactDetail");
                }
            }
            else
            {
                if (model.CheckAmount == true)
                {

                    if (string.IsNullOrEmpty(fileName))
                    {

                        ModelState.AddModelError("File", "Please Upload Identity Document");
                    }
                }
            }
            bool isValidExpityDate = Common.DateUtilities.DateGreaterThanToday(model.IdCardExpiringDate);
            if (!isValidExpityDate)
            {
                ModelState.AddModelError("IdCardExpiringDate", "Date Must Be Greater Than Today");
            }

            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            ViewBag.IdCardType = new SelectList(dbContext.IdentityCardType.OrderBy(x => x.CardType), "CardType", "CardType");
            ViewBag.IssuingCountry = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
            return View(model);
        }
        public ActionResult ContactDetail()
        {
            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            var countries = dbContext.Country.OrderBy(x => x.CountryName).Select(c => new SelectListItem()
            {
                Text = c.CountryName,
                Value = c.CountryCode
            }).ToList();

            ViewBag.Country = countries;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ContactDetail([Bind(Include = FaxerContactDetails.BindProperty)]FaxerContactDetails model)
        {
            FAXEREntities dbContext = new FAXEREntities();
            var countries = dbContext.Country.OrderBy(x => x.CountryName).Select(c => new SelectListItem()
            {
                Text = c.CountryName,
                Value = c.CountryCode
            }).ToList();

            ViewBag.Country = countries;
            if (ModelState.IsValid)
            {
                Common.FaxerSession.FaxerContactDetails = model;
            }

            var registerViewModel = Common.FaxerSession.RegisterViewModel;
            var faxerIdentification = Common.FaxerSession.FaxerIdentification;
            try
            {
                var user = new ApplicationUser
                {
                    UserName = registerViewModel.Email,
                    Email = registerViewModel.Email,
                    FirstName = registerViewModel.FirstName,
                    MiddleName = registerViewModel.MiddleName,
                    LastName = registerViewModel.LastName,
                    DOB = registerViewModel.DateOfBirth
                };
                var Password = registerViewModel.Password.ToHash();
                SFaxerSignUp faxerSignUpService = new SFaxerSignUp();
                string accountNo = faxerSignUpService.GetNewAccount(10);
                if (model.PhoneNumber != null)
                {

                    long phoneno = long.Parse(model.PhoneNumber);

                    model.PhoneNumber = phoneno.ToString();
                }
                DB.FaxerInformation faxerInformation = new DB.FaxerInformation()
                {
                    FirstName = registerViewModel.FirstName,
                    MiddleName = registerViewModel.MiddleName,
                    LastName = registerViewModel.LastName,
                    DateOfBirth = registerViewModel.DateOfBirth,
                    GGender = (int)registerViewModel.GGender,
                    Email = registerViewModel.Email,
                    AccountNo = accountNo,

                    IdCardType = faxerIdentification.IdCardType,
                    IdCardNumber = faxerIdentification.IdCardNumber,
                    IssuingCountry = faxerIdentification.IssuingCountry,
                    IdCardExpiringDate = faxerIdentification.IdCardExpiringDate,
                    CheckAmount = faxerIdentification.CheckAmount,
                    CardUrl = Common.FaxerSession.CardUrl,

                    Address1 = model.Address1,
                    Address2 = model.Address2,
                    City = model.City,
                    State = model.State,
                    PostalCode = model.PostalCode,
                    Country = model.Country,
                    PhoneNumber = model.PhoneNumber
                };
                faxerSignUpService.AddFaxerInformation(faxerInformation);

                SCity.Save(faxerInformation.City, faxerInformation.Country, DB.Module.Faxer);

                var guId = Guid.NewGuid().ToString();
                DB.FaxerLogin login = new DB.FaxerLogin()
                {
                    FaxerId = faxerInformation.Id,
                    UserName = faxerInformation.Email,
                    Password = Password,
                    ActivationCode = guId,
                    IsActive = false,
                    MobileNo = faxerInformation.PhoneNumber
                };
                faxerSignUpService.AddFaxerLogin(login);


                // Generate verification code for sender registration 

                string VerficationCode = Common.Common.GenerateVerificationCode(6);

                RegistrationCodeVerificationViewModel vm = new RegistrationCodeVerificationViewModel()
                {
                    PhoneNo = Common.Common.GetCountryPhoneCode(faxerInformation.Country) + " " + faxerInformation.PhoneNumber,
                    UserId = faxerInformation.Id,
                    VerificationCode = VerficationCode,
                    RegistrationOf = RegistrationOf.Sender,
                    Country = faxerInformation.Country
                };

                Common.FaxerSession.RegistrationCodeVerificationViewModel = vm;
                SRegistrationVerificationCode registrationVerificationCodeServices = new SRegistrationVerificationCode();
                registrationVerificationCodeServices.Add(vm);



                //send activation link to the user
                MailCommon mail = new MailCommon();
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                var link = string.Format("{0}/FaxerAccount/Activate/{1}", baseUrl, guId);

                string body = "";
                body = Common.Common.GetTemplate(baseUrl + "/emailtemplate/FaxerActivationEmail?guid=" + guId + "&faxername=" + faxerInformation.FirstName + " " + faxerInformation.LastName);

                mail.SendMail(faxerInformation.Email, "Welcome to MoneyFex", body);

                #region Registration Verification 

                // Sms Function Executed Here 

                Common.FaxerSession.RegistrationCodeVerificationViewModel = vm;

                SmsApi smsApiService = new SmsApi();


                string message = smsApiService.GetRegistrationMessage(vm.VerificationCode);
                smsApiService.SendSMS(vm.PhoneNo, message);
                // redirected to the verfication Code Screen


                return RedirectToAction("Index", "SenderRegistrationCodeVerification");


                #endregion



                return RedirectToAction("LoginMessage");
                //update reseller 
                //IsActivated True
                #region identity implementation 
                //var result = await UserManager.CreateAsync(user, registerViewModel.Password);
                //if (result.Succeeded)
                //{
                //    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                //    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                //    // Send an email with this link
                //    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                //    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code }, protocol: Request.Url.Scheme);
                //    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                //    ReceiversDetailsViewModel modelReceiver = (ReceiversDetailsViewModel)Session["ReceiverDetails"];

                //    DB.ReceiversDetail receiversDetails = new DB.ReceiversDetail
                //    {
                //        City = modelReceiver.ReceiverCity,
                //        Country = modelReceiver.ReceiverCountry,
                //        EmailAddress = modelReceiver.ReceiverEmailAddress,
                //        FirstName = modelReceiver.ReceiverFirstName,
                //        MiddleName = modelReceiver.ReceiverMiddleName,
                //        LastName = modelReceiver.ReceiverLastName,
                //        PhoneNumber = Convert.ToDecimal(modelReceiver.ReceiverPhoneNumber),
                //        CreatedDate = modelReceiver.CreatedDate,
                //        FaxerID = user.Id,
                //        IsDeleted = false
                //    };
                //    DB.FAXEREntities dbContext = new DB.FAXEREntities();
                //    dbContext.ReceiversDetails.Add(receiversDetails);
                //    await dbContext.SaveChangesAsync();

                //    return RedirectToAction("LoginMessage", "Account");
                //} 
                #endregion
                //AddErrors(result);
            }
            catch (Exception ex)
            {

            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, bool IsBusiness = false)
        {

            LoginViewModel model = new LoginViewModel();
            model.isBusiness = IsBusiness;
            HttpCookie aCookie = Request.Cookies["RememberMe"];

            if (aCookie != null)
            {
                model.Email = aCookie.Values["UserName"].Decrypt();
                model.Password = aCookie.Values["Password"].Decrypt();
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login([Bind(Include = LoginViewModel.BindProperty)] LoginViewModel model, string returnUrl)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check user Attempt
           // var userAttemptIsValid = LockUser();

            //if (userAttemptIsValid.Data == false)
            //{

            //    ModelState.AddModelError("", userAttemptIsValid.Message);
            //    return View(model);
            //}
            
            ViewBag.IsActive = true;

            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            Services.SFaxerSignUp FaxerSignUpService = new Services.SFaxerSignUp();


            model.Email = model.Email.Trim();
            model.Email = Common.Common.IgnoreZero(model.Email);

            SignInStatus SignInStatus = FaxerSignUpService.Login(model);
            switch (SignInStatus)
            {
                case SignInStatus.Success:
                    //var userList = await UserManager.FindByNameAsync(model.Email);
                    //if (!await UserManager.IsEmailConfirmedAsync(userList.Id))
                    //{
                    //    ViewBag.MessageDisplay = "You Must Have Confirmed Email To Log On";
                    //    return View("Error");
                    //}

                    FaxerSession.LoggedUserName = model.Email;
                    if (string.IsNullOrEmpty(Common.FaxerSession.FromUrl))
                    {
                        var faxerinfo = FaxerSignUpService.GetInformation(model.Email);
                        Common.FaxerSession.FaxerInformation = faxerinfo;

                        if (model.RememberMe)
                        {
                            // Clear any other tickets that are already in the response
                            Response.Cookies.Clear();

                            // Set the new expiry date - to thirty days from now
                            DateTime expiryDate = DateTime.Now.AddDays(1);

                            // Create a new forms auth ticket
                            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(2, model.Email, DateTime.Now, expiryDate, true, String.Empty);

                            // Encrypt the ticket
                            string encryptedTicket = FormsAuthentication.Encrypt(ticket);

                            // Create a new authentication cookie - and set its expiration date
                            HttpCookie authenticationCookie = new HttpCookie("RememberMe", encryptedTicket);
                            authenticationCookie.Expires = ticket.Expiration;
                            //if (model.Password.Contains("&")) {

                            //    model.Password.Replace("&", "&amp;");
                            //}

                            authenticationCookie.Values["UserName"] = model.Email.Encrypt();
                            authenticationCookie.Values["Password"] = model.Password.Encrypt();
                            // Add the cookie to the response.

                            Response.Cookies.Add(authenticationCookie);


                        }
                        HttpCookie authenticationCookies = new HttpCookie("ALoginAttempt");
                        Response.Cookies.Add(authenticationCookies);
                        HttpContext.Request.Cookies.Clear();

                        //return RedirectToAction("Index", "SenderDashBoard");
                        return RedirectToAction("Index", "SenderTransferMoneyNow");

                    }
                    else
                    {


                        // If user try to estimate the faxing amount summary of different sending country then his/her register country as a sender then
                        //user will not be able to transfer the amount
                        if (Common.FaxerSession.FromUrl == "/NonCardMoneyFax/NonCardReceiversDetails")
                        {

                            if (Common.FaxerSession.LoggedUser.CountryCode != Common.FaxerSession.FaxingCountry)
                            {

                                TempData["IsInValidSendingCountry"] = true;

                                return RedirectToAction("Index", "Home");
                            }
                        }
                        return Redirect(Common.FaxerSession.FromUrl);
                    }
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("LoginMessage", new { ReturnUrl = returnUrl, model.RememberMe });
                case SignInStatus.Failure:
                default:


                    var IsActive = FaxerSignUpService.IsActive(model);
                    //var LoginFailCount = FaxerSignUpService.LoginFailureCount(model.Email);
                    var senderInfo = FaxerSignUpService.SenderInfo(Common.Common.IgnoreZero(model.Email));

                    if (senderInfo == null)
                    {

                        ModelState.AddModelError("", "No account found, please sign up to access our services");
                    }
                    else if (senderInfo.Faxer.IsBusiness != model.isBusiness)
                    {
                        ModelState.AddModelError("", "No account found, please sign up to access our services.");
                    }
                    else if (IsActive != null)
                    {
                        if (IsActive == false)
                        {
                            ViewBag.IsActive = false;
                            //AccountReverification(model.Email);
                            ModelState.AddModelError("", "Account deactivated, Please contact customer service");


                            //AccountReverification(model.Email);


                            //ModelState.AddModelError("", "Your account is currently deactive please contact moneyfex suport team");

                            //return RedirectToAction("Index", "SenderRegistrationCodeVerification");
                            return View(model);
                        }
                    }

                    else
                    {
                        ModelState.AddModelError("", "Invalid Username or Password.");
                    }
                    return View(model);
            }



            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true



            #region sign In Old
            //var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            //switch (result)
            //{
            //    case SignInStatus.Success:
            //        var userList = await UserManager.FindByNameAsync(model.Email);
            //        if (!await UserManager.IsEmailConfirmedAsync(userList.Id))
            //        {
            //            ViewBag.MessageDisplay = "You Must Have Confirmed Email To Log On";
            //            return View("Error");
            //        }
            //        Session["LoggedUserName"] = model.Email;
            //        return RedirectToAction("Index", "Dashboard");
            //    case SignInStatus.LockedOut:
            //        return View("Lockout");
            //    case SignInStatus.RequiresVerification:
            //        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, model.RememberMe });
            //    case SignInStatus.Failure:
            //    default:
            //        ModelState.AddModelError("", "Invalid login attempt.");
            //        return View(model);
            //} 
            #endregion
            return View(model);
        }

        public void AccountReverification(string email)
        {

            Services.SFaxerSignUp _faxerSignUpService = new SFaxerSignUp();
            var faxerinfo = _faxerSignUpService.GetInformation(email);

            SRegistrationVerificationCode sRegistrationVerificationCode = new SRegistrationVerificationCode();

            RegistrationCodeVerificationViewModel vm = new RegistrationCodeVerificationViewModel();
            vm.Country = faxerinfo.Country;
            vm.PhoneNo = Common.Common.GetCountryPhoneCode(faxerinfo.Country) + " " + faxerinfo.PhoneNumber;
            vm.RegistrationOf = DB.RegistrationOf.Sender;
            vm.VerificationCode = sRegistrationVerificationCode.GetVerificationCode(vm.PhoneNo);
            vm.UserId = faxerinfo.Id;

            Common.FaxerSession.RegistrationCodeVerificationViewModel = vm;
        }
        public ServiceResult<bool> LockUser()
        {

            string ErrorMsg = "";
            bool IsValid = true;
            HttpCookie authenticationCookies = Request.Cookies["ALoginAttempt"];
            string endTime = "";
            try
            {
                endTime = authenticationCookies.Values["EndDate"] ?? "";
            }
            catch (Exception ex)
            {

            }
            FormsAuthenticationTicket ticket2 = new FormsAuthenticationTicket(2, endTime, DateTime.Now, DateTime.Now.AddMinutes(5), true, String.Empty);
            string encryptedTicket2 = FormsAuthentication.Encrypt(ticket2);

            if (authenticationCookies == null)
            {
                authenticationCookies = new HttpCookie("ALoginAttempt", encryptedTicket2);
            }
            int count = authenticationCookies.Values["Count"].ToInt();
            if (count > 5)
            {
                if (string.IsNullOrEmpty(authenticationCookies.Values["EndDate"]))
                {
                    authenticationCookies.Values["EndDate"] = DateTime.Now.AddMinutes(5).TimeOfDay.ToString();

                }

                var EndDate = TimeSpan.Parse(authenticationCookies.Values["EndDate"]);
                Response.Cookies.Add(authenticationCookies);
                if (DateTime.Now.TimeOfDay < EndDate)
                {
                    ErrorMsg = "Something went wrong. Please try again in 5 minitues ";
                    IsValid = false;
                }

            }
            count++;
            authenticationCookies.Values["Count"] = count.ToString();
            Response.Cookies.Add(authenticationCookies);
            HttpContext.Request.Cookies.Clear();

            return new ServiceResult<bool>()
            {
                Data = IsValid,
                Message = ErrorMsg,
                Status = ResultStatus.OK

            };

        }
        [HttpGet]
        public ActionResult Activate(string id)
        {
            Services.SFaxerSignUp signUp = new Services.SFaxerSignUp();
            var faxer = signUp.ActivateFaxerLogin(id);
            return RedirectToAction("Login", "faxerAccount");
        }
        [HttpGet]
        public ActionResult LoginMessage(string id)
        {
            return View();
        }
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {

            Services.SFaxerSignUp service = new Services.SFaxerSignUp();
            var EmailExist = service.EmailExist(email);
            if (EmailExist)
            {
                Log.Write("Hello");
                FaxerSession.ResetPassToken = Common.Common.GenerateRandomDigit(6);
                email = email.Trim();
                FaxerSession.ResetEmail = email;
                MailCommon mail = new MailCommon();
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

                string FaxerName = service.GetFaxerNameByEmail(email);

                Log.Write("Hello");
                string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/PasswordSecirityCodeEmail?UserName=" + FaxerName +
                    "&SecurityCode=" + FaxerSession.ResetPassToken);
                //var message = string.Format("Please User This Key # {1} # to reset your password", baseUrl, FaxerSession.ResetPassToken);

                Log.Write("Send email Start");
                mail.SendMail(email, "MoneyFex Password Reset Key", body);

                Log.Write("Send email End");
                SmsApi smsApiServices = new SmsApi();
                var message = smsApiServices.GetPasswordResetMessage(FaxerSession.ResetPassToken);

                var faxerInfo = service.GetInformation(email);
                string PhoneNo = Common.Common.GetCountryPhoneCode(faxerInfo.Country) + "" + faxerInfo.PhoneNumber;
                smsApiServices.SendSMS(PhoneNo, message);

                return RedirectToAction("SecurityCode");
            }
            ModelState.AddModelError("InvalidEmail", "Please enter a valid email");

            return View();


        }
        [HttpGet]
        public ActionResult SecurityCode()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SecurityCode(string code)
        {
            if (code == Common.FaxerSession.ResetPassToken)
            {
                FaxerSession.IsValidToResetPassword = true;
                return RedirectToAction("ResetPassword");
            }
            else
            {

                ModelState.AddModelError("Error", "security Code didn't match");
            }
            return View();
        }
        [HttpGet]
        public ActionResult ResetPassword()
        {
            if (!FaxerSession.IsValidToResetPassword)
                RedirectToAction("SecurityCode");

            if (string.IsNullOrEmpty(FaxerSession.ResetEmail))
                RedirectToAction("Login");
            FaxerPasswordResetViewModel model = new FaxerPasswordResetViewModel();
            model.Email = FaxerSession.ResetEmail;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword([Bind(Include = FaxerPasswordResetViewModel.BindProperty)]FaxerPasswordResetViewModel model)
        {
            if (!FaxerSession.IsValidToResetPassword)
                RedirectToAction("SecurityCode");

            if (ModelState.IsValid)
            {
                bool result = new Services.SFaxerSignUp().ResetPassword(model);
                MailCommon mail = new MailCommon();
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

                string body = "";
                try
                {
                    string FaxerName = new Services.SFaxerSignUp().GetFaxerNameByEmail(model.Email);
                    body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/FaxerAccountPasswordUpdateEmail?SenderName=" + FaxerName);

                    mail.SendMail(model.Email, "Password Update ", body);

                }
                catch (Exception e)
                {

                    string msg = "System Error";

                }

                return RedirectToAction("PasswordResetsuccessFull");
            }
            else
            {

            }
            return View(model);
        }

        public ActionResult PasswordResetsuccessFull()
        {

            return View();
        }
        [HttpGet]
        public ActionResult LogOut()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult getCountryPhoneCode(string country)
        {
            DB.FAXEREntities dbContext = new DB.FAXEREntities();

            string code = dbContext.Country.Where(x => x.CountryCode == country).Select(x => x.CountryPhoneCode).FirstOrDefault();
            return Json(new
            {
                CountryPhoneCode = code
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult FaxerRegisteredByAdmin(string Id)
        {


            return View();
        }

        public ActionResult RequestForReactivation(string Email)
        {


            Services.SFaxerSignUp servicesGetFaxerInfo = new SFaxerSignUp();
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            MailCommon mail = new MailCommon();

            string body = "";

            var FaxerInformation = servicesGetFaxerInfo.GetInformation(Email);
            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/FaxerAccountReactivationRequest/Index?FullName="
                + FaxerInformation.FirstName + " " + FaxerInformation.MiddleName + " " + FaxerInformation.LastName +
                "&Country=" + Common.Common.GetCountryName(FaxerInformation.Country) + "&City=" + FaxerInformation.City +
                "&Email=" + FaxerInformation.Email + "&MFNumber=" + FaxerInformation.AccountNo);

            mail.SendMail("reactivation@moneyfex.com", "Account Re-Activation Request.", body);

            //mail.SendMail("anankarki97@gmail.com", "Account Re-Activation Request.", body);
            TempData["RequestSuccessful"] = "Request has sent successfully";
            return RedirectToAction("Login");
        }
    }
}