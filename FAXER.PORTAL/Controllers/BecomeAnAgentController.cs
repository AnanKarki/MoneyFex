using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class BecomeAnAgentController : Controller
    {
        DB.FAXEREntities dbContext = new DB.FAXEREntities();
        // GET: BecomeAnAgent
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Message = "";
            ViewBag.Country = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
            ViewBag.AgentResult = new AgentResult();
            return View();
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = BecomeAnAgent.BindProperty)]BecomeAnAgent model)
        {
            model.AgentRegistrationCode = GetNewAccount(6);
            
            string msg = "";
            string activationCode = Guid.NewGuid().ToString();
            ViewBag.Country = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
            
            if (ModelState.IsValid)
            {
                   dbContext.BecomeAnAgent.Add(new DB.BecomeAnAgent() {
                    AgentRegistrationCode=model.AgentRegistrationCode,
                    BusinessEmailAddress=model.BusinessEmailAddress,
                    City=model.City,
                    CompanyBusinessName=model.CompanyBusinessName,
                    ContactName=model.ContactName,
                    ContactPhone=model.ContactPhone,
                    CountryCode=model.CountryCode,
                    FirstName=model.FirstName,
                    LastName=model.LastName,
                    PostZipCode=model.PostZipCode,
                    StateProvince=model.StateProvince,
                    Street=model.Street,
                    Address1=model.Address1,
                    Address2=model.Address2,
                    BusinessLicenseRegistrationNumber=model.BusinessLicenseRegistrationNumber,
                    FaxNo=model.FaxNo,
                    Website=model.Website,
                    ActivationCode= activationCode,
                    BusinessType = model.BusinessType
                   });
                var emailExist = dbContext.BecomeAnAgent.Where(x => x.BusinessEmailAddress == model.BusinessEmailAddress).FirstOrDefault();
                if (emailExist != null)
                {
                    ModelState.AddModelError("BusinessEmailAddress", "Email Already Exist");
                    ViewBag.Message = msg;
                    ViewBag.Country = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
                    return View(model);
                }
                else
                {
                    int result = dbContext.SaveChanges();
                  
                    if (result == 1)
                    {

                        MailCommon mail = new MailCommon();
                        var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                        //var link = string.Format("{0}/Agent/AgentDetails/ValidateLink?activationCode={1}", baseUrl, activationCode);
                        string body = "";
                        try
                        {
                            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/AgentRegistrationEmailToAdmin?AgentName=" + model.CompanyBusinessName +
                                "&AgentTelephone=" + model.ContactPhone
                                + "&AgentEmail=" + model.BusinessEmailAddress + "&RegistrationCode=" + model.AgentRegistrationCode);

                            string body2 = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/AgentInitialRegistrationEmail?ContactPerson=" + model.ContactName
                                + "&RegistrationCode=" + model.AgentRegistrationCode);

                            mail.SendMail("agentregistration@moneyfex.com", " Alert: New Agent Registration -" + model.CompanyBusinessName , body);
                            mail.SendMail(model.BusinessEmailAddress, "Initial Agent Registration Code", body2);
                            msg = "Thank you for showing the interest to join MoneyFex network of agents, a member of the agent registration team will contact you shortly to complete your application..";
                        }
                        catch (Exception)
                        {

                            msg = "Problem in sending mail";
                        }
                        TempData["Message"] = msg;
                        return RedirectToAction("SuccessFullyRegistered");

                    }
                }
            }
            ViewBag.Message = msg;
            return View(model);
        }


        public JsonResult GetCountryPhoneCode(string CountryCode) {

            var PhoneCode = Common.Common.GetCountryPhoneCode(CountryCode);
            return Json(new
            {
                PhoneCode = PhoneCode
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SuccessFullyRegistered() {

            return View();
        }
        [HttpGet]
        public ActionResult ActivateAgent(string ActivationCode)
        {
            ViewBag.ActivationCode = ActivationCode;
            return View();
        }

        public string GetNewAccount(int length)
        {
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < length; i++)
                s = String.Concat(s, random.Next(10).ToString());
            //return s;
            while (dbContext.BecomeAnAgent.Where(x => x.AgentRegistrationCode == s).Count() > 0)
            {
                for (int i = 0; i < length; i++)
                    s = String.Concat(s, random.Next(10).ToString());
            }
            return s;
        }



        [HttpPost]
        public ActionResult Activate(string ActivationCode)
        {
            var agentLogin = dbContext.AgentLogin.Where(x => x.ActivationCode == ActivationCode).FirstOrDefault();
            if (agentLogin!=null)
            {
                agentLogin.IsActive = true;
                dbContext.Entry(agentLogin).State = EntityState.Modified;
                dbContext.SaveChanges();
                MailCommon mail = new MailCommon();
                try
                {
                    string mesgBody = "The agent login code for" + agentLogin.AgentInformation.Name + "is "+agentLogin.LoginCode+". This code should be used together with your login details to gain access to the portal.";
                    mail.SendMail(agentLogin.Username, "Agent Registration Information.",mesgBody);
                }
                catch (Exception)
                {

                    throw;
                }
            }
            return RedirectToAction("ActivateAgent", "BecomeAnAgent", new { ActivationCode = ActivationCode });
        }
    }
}