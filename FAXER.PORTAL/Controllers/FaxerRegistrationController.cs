using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class FaxerRegistrationController : Controller
    {
        // GET: FaxerRegestration
        FAXER.PORTAL.DB.FAXEREntities dbContext = new DB.FAXEREntities();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        //[HandleError]
        public ActionResult Index([Bind(Include = RegisterViewModel.BindProperty)]RegisterViewModel model)
        {         

            if (ModelState.IsValid)
            {
                bool isValidAge = Common.DateUtilities.ValidateAge(model.DateOfBirth);
                bool isEmailExist = Common.OtherUtlities.IsEmailExist(model.Email);
                if (isEmailExist==false)
                {
                    ModelState.AddModelError("Email", "Email Already Exist");
                }
                if (isValidAge == false)
                {
                    ModelState.AddModelError("DateOfBirth", "Age Must Be Greater Than 18");
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    Session["FaxerFirstName"] = model.FirstName;
                    Session["FaxerMiddleName"] = model.MiddleName;
                    Session["FaxerLastName"] = model.LastName;
                    Session["FaxerDateOfBirth"] = model.DateOfBirth;
                    Session["FaxerGender"] = model.GGender;
                    Session["FaxerUserName"] = model.Email;
                    Session["FaxerPassword"] = model.Password;

                    return RedirectToAction("Index", "FaxerIdentification");
                }
                catch (Exception)
                {


                }


            }
            return View();
        }
    }
}