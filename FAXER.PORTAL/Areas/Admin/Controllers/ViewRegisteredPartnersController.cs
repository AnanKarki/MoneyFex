using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewRegisteredPartnersController : Controller
    {
        ViewRegisteredPartnersServices Service = new ViewRegisteredPartnersServices();
        CommonServices Common = new CommonServices();
        // GET: Admin/ViewRegisteredPartners
        public ActionResult Index(string Country = "", string City = "", string Search = "", string Message = "")
        {
            if (Message == "regSuccess")
            {
                ViewBag.Message = "Partner Registered Successfully";
                ViewBag.ToastrVal = 4;
                Message = "";
            }
            else if (Message == "regFail")
            {
                ViewBag.Message = "Something went wrong. Please try again !";
                ViewBag.ToastrVal = 0;
                Message = "";
            }
            else if (Message == "actSuccess")
            {
                ViewBag.Message = "Operation Successful !";
                ViewBag.ToastrVal = 4;
                Message = "";
            }
            else if (Message == "actFailure")
            {
                ViewBag.Message = "Something went wrong. Please try again !";
                ViewBag.ToastrVal = 0;
                Message = "";
            }
            else if (Message == "delSuccess")
            {
                ViewBag.Message = "Partner deleted successfully !";
                ViewBag.ToastrVal = 4;
                Message = "";
            }
            else if (Message == "delFailure")
            {
                ViewBag.Message = "Something went wrong. Please try again !";
                ViewBag.ToastrVal = 0;
                Message = "";
            }
            else if (Message == "UpdateNotFound")
            {
                ViewBag.Message = "Something went wrong. Please contact Admin !";
                ViewBag.ToastrVal = 0;
                Message = "";
            }
            else if (Message == "UpdateSuccess")
            {
                ViewBag.Message = "Partner Information Updated Successfully !";
                ViewBag.ToastrVal = 4;
                Message = "";

            }
            else if (Message == "UpdateFailure")
            {
                ViewBag.Message = "Something went wrong. Please try again !";
                ViewBag.ToastrVal = 0;
                Message = "";

            }

            SetViewBagForCountries();
            SetViewBagForSCities(Country);
            ViewBag.TotalRegisteredPartner = Service.List().Count;
            ViewBag.TotatActivePartner = Service.ListOfActivePartner().Count();
            ViewBag.TotalInActivePartner = Service.ListOfInActivePartner().Count;
            var vm = Service.getList(Country, City);
            if (!string.IsNullOrEmpty(Search))
            {
                vm = vm.Where(x => x.NameOfPartner.ToLower().Contains(Search.ToLower())).ToList();
            }

            string[] alpha = vm.GroupBy(x => x.FirstLetterOfPartner).Select(x => x.FirstOrDefault()).OrderBy(x => x.FirstLetterOfPartner).Select(x => x.FirstLetterOfPartner).ToArray();
            ViewBag.Alpha = alpha;
            ViewBag.Search = Search;
            return View(vm);
        }

        public ActionResult PartnerDashBoard(int PartnerId)
        {
            return View();
        }
        [HttpGet]
        public ActionResult RegisterAPartner()
        {
            SetViewBagForCountries();
            return View();
        }


        [HttpPost]
        public ActionResult RegisterAPartner([Bind(Include = RegisterAPartnerViewModel.BindProperty)]RegisterAPartnerViewModel model)
        {
            if (model != null)
            {

                bool valid = true;
                bool isEmailExist = Service.checkExistingPartnerEmail(model.EmailAddress);


                if (isEmailExist == false)
                {
                    valid = false;
                    ModelState.AddModelError("EmailAddress", "This email address has already been registered. Please try again!");
                }
                if (string.IsNullOrEmpty(model.Name))
                {
                    ModelState.AddModelError("Name", "Partner Name can't be empty !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.LicenseNo))
                {
                    ModelState.AddModelError("LicenseNo", "This field can't be empty !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.ContactPersonName))
                {
                    ModelState.AddModelError("ContactPersonName", "This field can't be empty !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.Address1))
                {
                    ModelState.AddModelError("Address1", "This field can't be empty !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.State))
                {
                    ModelState.AddModelError("State", "This field can't be empty !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.PostalCode))
                {
                    ModelState.AddModelError("PostalCode", "This field can't be blank !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.City))
                {
                    ModelState.AddModelError("City", "This field can't be empty !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.Country))
                {
                    ModelState.AddModelError("Country", "Country field can't be empty !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.PhoneNumber))
                {
                    ModelState.AddModelError("PhoneNumber", "This field can't be empty !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.EmailAddress))
                {
                    ModelState.AddModelError("EmailAddress", "This field can't be empty !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.Website))
                {
                    ModelState.AddModelError("Website", "This field can't be empty !");
                    valid = false;
                }
                if (valid == true)
                {
                    if (Request.Files.Count > 0)
                    {
                        string fileName = "";
                        string directory = Server.MapPath("/Documents");
                        var partnerLogo = Request.Files["partnerLogo"];

                        if (partnerLogo != null && partnerLogo.ContentLength > 0)
                        {
                            fileName = Guid.NewGuid() + "." + partnerLogo.FileName.Split('.')[1];
                            partnerLogo.SaveAs(Path.Combine(directory, fileName));
                        }
                        else
                        {
                            ModelState.AddModelError("PartnerLogoUrl", "Partner Logo is mandatory !");
                            valid = false;
                            SetViewBagForCountries();
                            return View(model);
                        }
                        model.PartnerLogoUrl = "/Documents/" + fileName;
                    }
                    bool saveData = Service.savePartner(model);
                    if (saveData)
                    {
                        return RedirectToAction("Index", "ViewRegisteredPartners", new { @message = "regSuccess" });
                    }
                    else
                    {
                        return RedirectToAction("Index", "ViewRegisteredPartners", new { @message = "regFail" });
                    }

                }

            }
            SetViewBagForCountries();
            return View(model);
        }

        [HttpGet]
        public ActionResult UpdatePartnerInfo(int id)
        {
            if (id != 0)
            {
                var data = Service.getInfo(id);
                return View(data);
            }
            return RedirectToAction("Index", "ViewRegisteredPartners", new { @Message = "UpdateNotFound" });
        }



        [HttpPost]
        public ActionResult UpdatePartnerInfo([Bind(Include = RegisterAPartnerViewModel.BindProperty)]RegisterAPartnerViewModel model)
        {
            if (model != null)
            {
                bool valid = true;
                if (string.IsNullOrEmpty(model.LicenseNo))
                {
                    ModelState.AddModelError("LicenseNo", "This field can't be blank !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.ContactPersonName))
                {
                    ModelState.AddModelError("ContactPersonName", "This field can't be blank !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.PartnerType))
                {
                    ModelState.AddModelError("PartnerType", "This field can't be blank !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.Address1))
                {
                    ModelState.AddModelError("Address1", "This field can't be blank !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.City))
                {
                    ModelState.AddModelError("City", "This field can't be blank !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.State))
                {
                    ModelState.AddModelError("State", "This field can't be blank !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.PostalCode))
                {
                    ModelState.AddModelError("PostalCode", "This field can't be blank !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.PhoneNumber))
                {
                    ModelState.AddModelError("PhoneNumber", "This field can't be blank !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.Website))
                {
                    ModelState.AddModelError("Website", "This field can't be blank !");
                    valid = false;
                }
                if (valid == true)
                {
                    if (Request.Files.Count > 0)
                    {
                        string fileName = "";
                        string directory = Server.MapPath("/Documents");
                        var partnerLogo = Request.Files["partnerLogo"];

                        if (partnerLogo != null && partnerLogo.ContentLength > 0)
                        {
                            fileName = Guid.NewGuid() + "." + partnerLogo.FileName.Split('.')[1];
                            partnerLogo.SaveAs(Path.Combine(directory, fileName));
                            model.PartnerLogoUrl = "/Documents/" + fileName;
                        }

                    }
                    bool saveData = Service.updatePartnerInfo(model);
                    if (saveData)
                    {
                        return RedirectToAction("Index", "ViewRegisteredPartners", new { @Message = "UpdateSuccess" });
                    }
                    return RedirectToAction("Index", "ViewRegisteredPartners", new { @Message = "UpdateFailure" });
                }
            }
            return View(model);

        }

        public ActionResult ActivateDeactivatePartner(int id)
        {
            if (id != 0)
            {
                bool result = Service.activateDeactivatePartner(id);
                if (result)
                {
                    return RedirectToAction("Index", "ViewRegisteredPartners", new { @Message = "actSuccess" });
                }
            }
            return RedirectToAction("Index", "ViewRegisteredPartners", new { @Message = "actFailure" });
        }

        public ActionResult DeletePartner(int id)
        {
            if (id != 0)
            {
                bool result = Service.deletePartner(id);
                if (result)
                {
                    return RedirectToAction("Index", "ViewRegisteredPartners", new { @Message = "delSuccess" });
                }
            }
            return RedirectToAction("Index", "ViewRegisteredPartners", new { @Message = "delFailure" });
        }


        public ActionResult getCountryCode(string countryCode)
        {
            var code = Common.getPhoneCodeFromCountry(countryCode);
            return Json(new
            {
                CountryCode = code
            }, JsonRequestBehavior.AllowGet);
        }

        private void SetViewBagForCountries()
        {
            var countries = Common.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }
        public void SetViewBagForSCities(string Country = "")
        {
            var cities = SCity.GetCities(DB.Module.Agent, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }

    }


}