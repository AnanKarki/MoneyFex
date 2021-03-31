using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewRegisteredAgentsController : Controller
    {
        Services.CommonServices common = new Services.CommonServices();
        Services.ViewRegisteredAgentsServies AgentInformation = new Services.ViewRegisteredAgentsServies();
        // GET: Admin/ViewRegisteredAgents

        public ActionResult index(string CountryCode = "", string City = "", string message = "", string searchAgent = "" , bool IsFromActivity = false, bool IsFromManualBankAccount = false)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (message == "mailSent")
            {
                ViewBag.Message = "Mails sent successfully !";
                message = "";
            }
            else if (message == "mailNotSent")
            {
                ViewBag.Message = "Mails not sent ! Please try again.";
                message = "";
            }
            var countries = common.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            SetViewBagForSCities(CountryCode);
            var viewmodel = new List<ViewModels.ViewRegisteredAgentsViewModel>();

            #region old

            //if (!string.IsNullOrEmpty(CountryCode) && string.IsNullOrEmpty(City))
            //{
            //    viewmodel = AgentInformation.getFilterAgentList(CountryCode, City);
            //    ViewBag.Country = CountryCode;

            //}
            //else if (string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            //{

            //    viewmodel = AgentInformation.getFilterAgentList(CountryCode, City);
            //    ViewBag.Country = CountryCode;
            //}
            //else if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            //{

            //    viewmodel = AgentInformation.getFilterAgentList(CountryCode, City);
            //    ViewBag.Country = CountryCode;
            //    ViewBag.City = City;
            //}
            //else
            //{
            //    viewmodel = AgentInformation.getFilterAgentList(CountryCode, City);
            //    //viewmodel = AgentInformation.getAgentInformationList();
            //    ViewBag.Country = "";
            //}

            #endregion

            // Get List of Agent Information 

            viewmodel = AgentInformation.getFilterAgentList(CountryCode, City, searchAgent);

            ViewBag.RegisterdAgents = AgentInformation.List().Count();
            ViewBag.ActiveAgents = AgentInformation.ListOfActiveAgents().Count();
            ViewBag.InActiveAgents = AgentInformation.ListOfInActiveAgents().Count();

            ViewBag.agentResult = new AgentResult();
            ViewBag.IsFromAgentActivity = IsFromActivity;
            ViewBag.IsFromManualBankAccount = IsFromManualBankAccount;
            return View(viewmodel);
        }
        //Change Agent Status active or deactive 
        public ActionResult AgentStatus(int id = 0)
        {
            if (id != 0)
            {
                AgentInformation.AgentStaus(id);
            }

            // Get List of Agent Information 
            return RedirectToAction("index");
        }


        // get Agent Information as per registration Number of Agent 
        [HttpGet]
        public ActionResult RegisterAnAgent(String RegistrationNumber = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var viewmodel = new ViewModels.ViewRegisteredAgentsViewModel();
            var countries = common.GetCountries();


            if (RegistrationNumber != "")
            {
                viewmodel = AgentInformation.getAgentInformation(RegistrationNumber);


            }
            if (viewmodel == null)
            {
                TempData["Message2"] = "Invaild Registration Code";
                return RedirectToAction("index");

            }
            ViewBag.countries = new SelectList(countries, "Code", "Name", viewmodel.CountryCode);
            ViewBag.AgentResult = new AgentResult();
            return View(viewmodel);


        }
        /// <summary>
        /// Add if Agent does not Exist otherwise Update Agent  
        /// </summary>
        /// <param name="Agent"></param>
        /// <returns>RegisterAgentView</returns>
        [HttpPost]
        public ActionResult RegisterAnAgent([Bind(Include = ViewRegisteredAgentsViewModel.BindProperty)]ViewRegisteredAgentsViewModel Agent)
        {
            AgentResult agentResult = new AgentResult();
            var countries = common.GetCountries();
            ViewBag.countries = new SelectList(countries, "Code", "Name", Agent.CountryCode);
            if (Agent.Id == 0)
            {

                bool checkExistingEmail = AgentInformation.checkExistingEmail(Agent.Email);
                if (checkExistingEmail == false)
                {
                    ModelState.AddModelError("Email", "Sorry ! An agent is already registered with this Email Address.");
                    return View(Agent);
                }
            }

            if (ModelState.IsValid)
            {
                // return true if action is successful

                var result = AgentInformation.AddOrUpdateAgentInformtaion(Agent);

                string msg = "";
                // If case is true
                if (result)
                {
                    string activationCode = Agent.ActivationCode;


                    MailCommon mail = new MailCommon();
                    var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                    string body = "";

                    var link = string.Format("{0}/Agent/AgentDetails/ValidateLink?activationCode={1}", baseUrl, activationCode);
                    try
                    {
                        body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/AgentRegistrationEmail?AgentName=" + Agent.Name + "&Link=" + link);


                        mail.SendMail(Agent.Email, "Agent Registration Information - Link", body);
                        //mail.SendMail("anankarki97@gmail.com", "Agent Registration Information - Link", body);
                        msg = "Registraion Completed";
                    }
                    catch (Exception)
                    {
                        msg = "Problem finding mail";
                    }
                }

                TempData["Message2"] = msg;
                return RedirectToAction("Index");
            }
            //  if case is false         
            return View();
        }

        public ActionResult getCountryCode(string countryCode)
        {
            var code = common.getPhoneCodeFromCountry(countryCode);
            return Json(new
            {
                CountryPhoneCode = code
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult UpdateAgent(string RegistrationNo)
        {
            if (string.IsNullOrEmpty(RegistrationNo) == false)
            {
                var vm = AgentInformation.getAgentInformation(RegistrationNo);
                return View(vm);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateAgent([Bind(Include = ViewRegisteredAgentsViewModel.BindProperty)]ViewRegisteredAgentsViewModel model)
        {
            if (model != null)
            {
                bool valid = true;
                if (string.IsNullOrEmpty(model.ContactPerson))
                {
                    ModelState.AddModelError("ContactPerson", "This field can't be empty !");
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
                    ModelState.AddModelError("PostalCode", "This field can't be empty !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.City))
                {
                    ModelState.AddModelError("City", "This field can't be empty !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.PhoneNumber))
                {
                    ModelState.AddModelError("PhoneNumber", "This field can't be empty !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.FaxNumber))
                {
                    ModelState.AddModelError("FaxNumber", "This field can't be empty !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.Website))
                {
                    ModelState.AddModelError("Website", "This field can't be empty !");
                    valid = false;
                }
                if (model.Checked == false)
                {
                    ModelState.AddModelError("Checked", "Please Confirm before updating the data !");
                    valid = false;
                }
                if (valid == true)
                {
                    bool result = AgentInformation.UpdateAgentInformation(model);
                    if (result)
                    {
                        return RedirectToAction("Index");
                    }

                }

            }
            return View(model);
        }



        public ActionResult ViewRegisteredAgentsMore(String RegistrationNumber)
        {
            var viewmodel = new ViewModels.ViewRegisteredAgentsViewModel();

            viewmodel = AgentInformation.ViewMoreAgentInformation(RegistrationNumber);

            var result = AgentInformation.GetAgentNotes(viewmodel.Id);
            viewmodel.AgentNoteList = result;

            return View(viewmodel);


        }

        public ActionResult AddNewAgentMote(string note, int AgentId, string RegistrationNo)
        {

            AgentNote agentNote = new AgentNote()
            {
                AgentId = AgentId,
                Note = note,
                CreatedByStaffName = Common.StaffSession.LoggedStaff.FirstName + " " + Common.StaffSession.LoggedStaff.MiddleName + " " + Common.StaffSession.LoggedStaff.LastName,
                CreatedByStaffId = Common.StaffSession.LoggedStaff.StaffId,
                CreatedDateTime = DateTime.Now

            };
            var result = AgentInformation.AddNewAgentNote(agentNote);

            //string registrationNo = AgentInformation;
            return RedirectToAction("ViewRegisteredAgentsMore", new { RegistrationNumber = RegistrationNo });

        }
        public ActionResult DeleteAgent(int id)
        {

            var result = AgentInformation.DeleteAgent(id);
            if (result)
            {
                return RedirectToAction("index");
            }
            return View();

        }

        public void SetViewBagForSCities(string Country = "")
        {
            var cities = SCity.GetCities(DB.Module.Agent, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
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