using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    /// <summary>
    /// Virtual Account Registration Controller
    /// </summary>
    public class TopUpRegestrationController : Controller
    {
        DB.FAXEREntities dbContext = new DB.FAXEREntities();
        // GET: TopUpRegestration
        public ActionResult UserContactDetails()
        {
            if (Common.FaxerSession.LoggedUser == null)
            {
                var Url = Request.Url.ToString();
                string[] tokens = Url.Split('/');
                Common.FaxerSession.FromUrl = "/" + tokens[3] + "/" + tokens[4];
                return RedirectToAction("Login", "FaxerAccount");
            }
            TopUpCardUserDetailsModelView userModel = new TopUpCardUserDetailsModelView();
            if (Session["TopUpUserDetails"] != null)
            {
                userModel = (Models.TopUpCardUserDetailsModelView)Session["TopUpUserDetails"];
            }
            
            return View(userModel);
        }
        [HttpPost]
        public ActionResult UserContactDetails([Bind(Include = TopUpCardUserDetailsModelView.BindProperty)]TopUpCardUserDetailsModelView userModel)
        {

            //if (ModelState.IsValid)
            //{

            //    Session["TopUpUserDetails"] = vm;
            //    return RedirectToAction("UserAddressDetails");
            //}

            ViewBag.Country = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");

            if (ModelState.IsValid)
            {
                //var duplicate = dbContext.MFTCCardInformation.Where(x => x.FirstName.ToLower().Trim() == userModel.TopUpUserFirstName.ToLower().Trim() && x.FaxerId == FaxerSession.LoggedUser.Id).FirstOrDefault();
                //if (duplicate != null)
                //{
                //    ModelState.AddModelError("duplicateError", "This Name is already exist.");
                //    return View("UserContactDetails", userModel);
                //}

                //var emailAlreadyExist = dbContext.MFTCCardInformation.Where(x => x.CardUserEmail == )
                bool validAge = true;
                var agevalidation = GetAge(userModel.TopUpUserDateOfBirth);
                if (agevalidation < 18)
                {
                    validAge = false;
                }
                int GetAge(DateTime bornDate)
                {
                    DateTime today = DateTime.Today;
                    int age = today.Year - bornDate.Year;
                    if (bornDate > today.AddYears(-age))
                        age--;
                    return age;
                }
                if (validAge == false)
                {
                    ModelState.AddModelError("InvalidAge", "Date of birth should be 18 years above.");
                    return View(userModel);
                }
                //if (userModel.AgeVerification == false)
                //{
                //    ModelState.AddModelError("AgeVerification", "Please Confirm Before Continue.");
                //    return View("UserContactDetails", userModel);
                //}
                else
                {
                    HttpContext.Session["TopUpUserDetails"] = userModel;
                    return View("UserAddressDetails");
                }
            }

            return View(userModel);

        }
        [HttpGet]
        public ActionResult UserAddressDetails()
        {
            ViewBag.Country = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
            TopUpCardUserContactDetailsViewModel userContactModel = new TopUpCardUserContactDetailsViewModel();
            if (Session["TopUpUserContactDetails"] != null)
            {

                userContactModel = (Models.TopUpCardUserContactDetailsViewModel)Session["TopUpUserContactDetails"];
                return View(userContactModel);
            }
            return View();

        }
        [HttpPost]
        public ActionResult UserAddressDetails([Bind(Include = TopUpCardUserDetailsModelView.BindProperty)]TopUpCardUserDetailsModelView userModel)
        {
            ViewBag.Country = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");

            if (ModelState.IsValid)
            {
                //var duplicate = dbContext.MFTCCardInformation.Where(x => x.FirstName.ToLower().Trim() == userModel.TopUpUserFirstName.ToLower().Trim() && x.FaxerId == FaxerSession.LoggedUser.Id).FirstOrDefault();
                //if (duplicate != null)
                //{
                //    ModelState.AddModelError("duplicateError", "This Name is already exist.");
                //    return View("UserContactDetails", userModel);
                //}

                //var emailAlreadyExist = dbContext.MFTCCardInformation.Where(x => x.CardUserEmail == )
                bool validAge = true;
                var agevalidation = GetAge(userModel.TopUpUserDateOfBirth);
                if (agevalidation < 18)
                {
                    validAge = false;
                }
                int GetAge(DateTime bornDate)
                {
                    DateTime today = DateTime.Today;
                    int age = today.Year - bornDate.Year;
                    if (bornDate > today.AddYears(-age))
                        age--;
                    return age;
                }
                if (validAge == false)
                {
                    ModelState.AddModelError("InvalidAge", "Date of birth should be 18 years above.");
                    return View("UserContactDetails", userModel);
                }
                //if (userModel.AgeVerification == false)
                //{
                //    ModelState.AddModelError("AgeVerification", "Please Confirm Before Continue.");
                //    return View("UserContactDetails", userModel);
                //}
                else
                {
                    HttpContext.Session["TopUpUserDetails"] = userModel;
                    return View();
                }
            }
            return View("UserContactDetails", userModel);
        }

        [HttpPost]
        public ActionResult CardUserIdentification([Bind(Include = TopUpCardUserContactDetailsViewModel.BindProperty)]TopUpCardUserContactDetailsViewModel userContactModel)
        {
            if (ModelState.IsValid)
            {
                var EmailAlreadyExist = dbContext.KiiPayPersonalWalletInformation.Where(x => x.CardUserEmail == userContactModel.EmailAddress).FirstOrDefault();
                if (EmailAlreadyExist != null) {
                    ViewBag.Country = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
                    ModelState.AddModelError("EmailAddress", "Email Already Exist");
                    return View("UserAddressDetails", userContactModel);
                }
                HttpContext.Session["TopUpUserContactDetails"] = userContactModel;
                return View();
            }
            ViewBag.Country = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
            return View("UserAddressDetails", userContactModel);
        }

        // POST: TopUpRegestration/Create
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> CreateTopUpUser(HttpPostedFileBase upload, FormCollection formCollection)
        {
            try
            {
                Models.TopUpCardUserDetailsModelView cardUserDetails = (Models.TopUpCardUserDetailsModelView)Session["TopUpUserDetails"];
                Models.TopUpCardUserContactDetailsViewModel cardUserContactDetails = (Models.TopUpCardUserContactDetailsViewModel)Session["TopUpUserContactDetails"];


                
                Models.UserTopUpRegestrationModelViewModel topUpUserModel = new Models.UserTopUpRegestrationModelViewModel
                {
                    Address1 = cardUserContactDetails.Address1,
                    Address2 = cardUserContactDetails.Address2,
                    State = cardUserContactDetails.State,
                    PostalCode = cardUserContactDetails.PostalCode,
                    City = cardUserContactDetails.City,
                    Country = cardUserContactDetails.Country,
                    PhoneNumber = cardUserContactDetails.PhoneNumber.FormatPhoneNo(),
                    EmailAddress = cardUserContactDetails.EmailAddress,
                    TopUpUserFirstName = cardUserDetails.TopUpUserFirstName,
                    TopUpUserMiddleName = cardUserDetails.TopUpUserMiddleName,
                    TopUpUserLastName = cardUserDetails.TopUpUserLastName,
                    TopUpUserDateOfBirth = cardUserDetails.TopUpUserDateOfBirth
                };
                //if (upload == null)
                //{
                //    ModelState.AddModelError("File", "Please Upload A Passport Size Photo Of The Card User.");
                //    return View("CardUserIdentification");
                //}
                var fileName = "";
                var ImageFileURL = "";
                if (upload != null)
                {
                    string directory = Server.MapPath("~/TopUpUsers");
                    fileName = Guid.NewGuid() + "." + upload.FileName.Split('.')[1];
                    if (upload != null && upload.ContentLength > 0)
                    {
                        upload.SaveAs(Server.MapPath("~/TopUpUsers") + "\\" + fileName);
                        ImageFileURL = "/TopUpUsers/" + fileName;
                    }
                }
                    #region cardholder informaiton
                    SFaxingNonCardTransaction service = new SFaxingNonCardTransaction();
                    string MFTCCardNumber = service.GetNewMFTCCardNumber();
                    ///example MFS-123
                    string MFTCCard = MFTCCardNumber + "-" + topUpUserModel.TopUpUserFirstName.ToUpper();
                    DB.KiiPayPersonalWalletInformation cardRegestration = new DB.KiiPayPersonalWalletInformation
                    {
                        AutoTopUp = false,
                        AutoTopUpAmount = 0,
                        UserPhoto = ImageFileURL,
                        MFTCardPhoto = ImageFileURL,
                        CardStatus = CardStatus.Active,
                        CardUserCity = topUpUserModel.City,
                        CardUserCountry = topUpUserModel.Country,
                        CardUserDOB = topUpUserModel.TopUpUserDateOfBirth,
                        CardUserEmail = topUpUserModel.EmailAddress,
                        Address1 = topUpUserModel.Address1,
                        Address2 = topUpUserModel.Address2,
                        CardUserTel = topUpUserModel.PhoneNumber,
                        CashLimitType = 0,
                        CashWithdrawalLimit = 0,
                        CurrentBalance = 0,
                        //FaxerId = FaxerSession.LoggedUser.Id,
                        FirstName = topUpUserModel.TopUpUserFirstName,
                        GoodsLimitType = 0,
                        GoodsPurchaseLimit = 0,
                        LastName = topUpUserModel.TopUpUserLastName,
                        MobileNo = MFTCCard.Encrypt(),
                        MiddleName = topUpUserModel.TopUpUserMiddleName,
                        //TempSMS = false,
                        IsDeleted = false,
                        CardUserPostalCode = topUpUserModel.PostalCode,
                        CardUserState = topUpUserModel.State
                    };
                    var result = dbContext.KiiPayPersonalWalletInformation.Add(cardRegestration);
                    #endregion
                    dbContext.SaveChanges();
                    SCity.Save(topUpUserModel.City, topUpUserModel.Country, DB.Module.CardUser);
                    MailCommon mail = new MailCommon();
                    var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                    string FaxerName = Common.FaxerSession.LoggedUser.FullName;
                    string FaxerEmail = Common.FaxerSession.LoggedUser.UserName;
                    string body = "";

                    string TopUpMoneyfaxCard = baseUrl + "/StartFaxingMoney/Index?mFTCCardInformationId=" + cardRegestration.Id;
                    string CardUserCountry = Common.Common.GetCountryName(cardRegestration.CardUserCountry);

                    string FaxerCountry = Common.Common.GetCountryName(Common.FaxerSession.LoggedUser.CountryCode);
                    body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFTCCardRegistrationEmail?FaxerName=" + FaxerName + "&MFTCCardNumber=" + MFTCCard.GetVirtualAccountNo() +
                        "&CardUserName=" + cardRegestration.FirstName + " " + cardRegestration.MiddleName + " " + cardRegestration.LastName
                        + "&CardUserCountry=" + CardUserCountry + "&TopUpMoneyfaxCard=" + TopUpMoneyfaxCard);

                    mail.SendMail(FaxerEmail, "MoneyFex virtual account registration - confirmation ", body);


                    string body2 = "";

                    body2 = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFTCCardRegistrationEmailToCardUser?CardUserName=" + cardRegestration.FirstName + " " + cardRegestration.MiddleName + " " + cardRegestration.LastName +
                        "&MFTCCardNumber=" + cardRegestration.MobileNo.Decrypt().GetVirtualAccountNo() +
                        "&SenderName=" + FaxerName + "&SenderCountry=" + FaxerCountry + "&CardUserCountry=" + CardUserCountry);

                    mail.SendMail(cardRegestration.CardUserEmail, " MoneyFex virtual account registration - User Confirmation", body2);


                    FaxerSession.TopUpCardId = result.Id.ToString();
                
                return RedirectToAction("UserSuccess");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException.Message);
            }
            return RedirectToAction("UserSuccess");
        }

        public ActionResult getCountryPhoneCode(string country)
        {
            string code = dbContext.Country.Where(x => x.CountryCode == country).FirstOrDefault().CountryPhoneCode;
            return Json(new
            {
                CountryPhoneCode = code
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UserSuccess()
        {
            ViewBag.TopUPCardId = FaxerSession.TopUpCardId;
            return View();
        }
    }
}
