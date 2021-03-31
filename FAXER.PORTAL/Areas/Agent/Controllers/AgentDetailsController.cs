using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class AgentDetailsController : Controller
    {
        DB.FAXEREntities dbContext = new DB.FAXEREntities();
        // GET: Agent/AgentDetails
        [HttpGet]
        public ActionResult Index(string agentRegistrationCode)
        {
            AgentsDetailsViewModel vm = new AgentsDetailsViewModel();
            //var becomeAnAgentTempData = dbContext.BecomeAnAgent.Where(x => x.AgentRegistrationCode == agentRegistrationCode).FirstOrDefault();
            var becomeAnAgentTempData = dbContext.AgentInformation.Where(x => x.RegistrationNumber == agentRegistrationCode).FirstOrDefault();
            
            if (becomeAnAgentTempData != null)
            {
                vm.AgentName = becomeAnAgentTempData.Name;
                vm.ContactPerson = becomeAnAgentTempData.ContactPerson;
                vm.EmailAddress = becomeAnAgentTempData.Email;
                vm.RegistrationNo = becomeAnAgentTempData.RegistrationNumber;
                vm.BusinessLicenseNo = becomeAnAgentTempData.LicenseNumber;
              // Common.AgentSession.BecomeAnAgent = becomeAnAgentTempData;
            }

            return RedirectToAction(  "Index" , "AgentRegistration", new { agentRegistrationCode });
            
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = AgentsDetailsViewModel.BindProperty)] AgentsDetailsViewModel vm)
        {
            if (ModelState.IsValid)
            {
                bool isValidPassword = Common.Common.ValidatePassword(vm.Password);
                if (!(isValidPassword))
                {
                    ModelState.AddModelError("Password", "Invalid Password");
                    return View();
                }
                else
                {
                    vm.Password = Common.Common.Encrypt(vm.Password);
                }
                Common.AgentSession.AgentDetails = vm;
                return RedirectToAction("ContactDetails", new { area = "Agent" });
            }
            else
            {
                return View();
            }
        }
        [HttpGet]
        public ActionResult ContactDetails()
        {
            AgentContactDetailsViewModel vm = new AgentContactDetailsViewModel();
           var becomeAnAgentTempData = Common.AgentSession.BecomeAnAgent;
            if (becomeAnAgentTempData != null)
            {
                vm.City = becomeAnAgentTempData.City;
                vm.Country = becomeAnAgentTempData.CountryCode;
                vm.PhoneNumber = becomeAnAgentTempData.ContactPhone;
                vm.StateProvinceRegion = becomeAnAgentTempData.StateProvince;
                vm.ZipPostalCode = becomeAnAgentTempData.PostZipCode;
                vm.Address1 = becomeAnAgentTempData.Address1;
                vm.Address2 = becomeAnAgentTempData.Address2;
                vm.FaxNo = becomeAnAgentTempData.FaxNo;
                vm.Website = becomeAnAgentTempData.Website;
                ViewBag.CountryList = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName",vm.Country);
            }
            else
            {
                ViewBag.CountryList = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
            }
            ViewBag.AgentResult = new AgentResult();
            return View(vm);
        }
        [HttpPost]
        public ActionResult ContactDetails([Bind(Include = AgentContactDetailsViewModel.BindProperty)] AgentContactDetailsViewModel vm)
        {
            AgentResult agentResult = new AgentResult();
            if (ModelState.IsValid)
            {
                if (!vm.Accept)
                {
                    ViewBag.CountryList = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
                    agentResult.Status = ResultStatus.Warning;
                    agentResult.Message= "Please check the box to accept to our terms and conditions";
                    ViewBag.AgentResult = agentResult;
                    return View();
                }
                var agentDetails = Common.AgentSession.AgentDetails;
                string accountNo = Common.Common.GenerateRandomDigit(10);
                var model = new AgentInformation()
                {
                    Address1 = vm.Address1,
                    Address2 = vm.Address2,
                    ContactPerson = agentDetails.ContactPerson,
                    CountryCode = vm.Country,
                    Email = agentDetails.EmailAddress,
                    FaxNumber = vm.FaxNo,
                    Name = agentDetails.AgentName,
                    Password = agentDetails.Password,
                    PhoneNumber = vm.PhoneNumber.FormatPhoneNo(),
                    PostalCode = vm.ZipPostalCode,
                    LicenseNumber = agentDetails.BusinessLicenseNo,
                    RegistrationNumber = agentDetails.RegistrationNo,
                    State = vm.StateProvinceRegion,
                    Website = vm.Website,
                    AccountNo = accountNo
                   
                };
                dbContext.AgentInformation.Add(model);
               int result= dbContext.SaveChanges();
                if (result==1)
                {
                    string loginCode = FAXER.PORTAL.Common.Common.GenerateRandomDigit(5);
                    string activationCode = Guid.NewGuid().ToString();
                    dbContext.AgentLogin.Add(new AgentLogin() {
                        ActivationCode=activationCode,
                        AgentId=model.Id,
                        IsActive=false,
                        LoginCode=loginCode,
                        LoginFailedCount=0,
                        Password=agentDetails.Password,
                        Username=model.Email
                    });
                    dbContext.SaveChanges();
                    MailCommon mail = new MailCommon();
                    //send mail to admin as activation code
                    var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                    var link = string.Format("{0}/BecomeAnAgent/ActivateAgent?ActivationCode={1}", baseUrl, activationCode);
                    var Activationlink = string.Format("{0}/Agent/AgentDetails/ActivateAgent?ActivationCode={1}", baseUrl, activationCode);

                    try
                    {
                        string mailMessage =string.Format("The following agent {0} with telephone {1} and email {2} has registered on MoneyFax Platform, please verify and activate account by clicking the link {3}",agentDetails.AgentName,model.PhoneNumber,model.Email,link);
                        mail.SendMail("moneyfaxer@gmail.com", "Agent Activation Information.", mailMessage);
                        // Email to Agent 
                        string body = "";                        
                        body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/AgentActivationEmail?NameOfContactPerson=" + model.ContactPerson + "&EmailAddress=" + model.Email + "&LoginCode=" + loginCode + "&Link=" + Activationlink);

                        mail.SendMail(agentDetails.EmailAddress, "Agent Registration Confirmation", body);
                        //end 
                        agentResult.Status = ResultStatus.OK;
                        agentResult.Message = "The Agent account has been successfully set up. Please go to the registered email to get your login detalis. You would only be able to login after the activation of your account by administrator of Moneyfax Service";
                        
                    }
                    catch (Exception)
                    {

                        agentResult.Status = ResultStatus.OK;
                        agentResult.Message = "Problem in sending mail";
                    }
                }
            }
            ViewBag.CountryList = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
            ViewBag.AgentResult = agentResult;
            Session.Clear();
            return View();
        }

        public ActionResult LoginMessage() {


            return View();
        }
        [HttpGet]
        public ActionResult ValidateLink(string activationCode)
        {
            var becomeAnAgentInfo = dbContext.BecomeAnAgent.Where(x => x.ActivationCode == activationCode).FirstOrDefault();
            MailCommon mail = new MailCommon();
            try
            {
                mail.SendMail(becomeAnAgentInfo.BusinessEmailAddress, "About Agent Registration Code.", "Your Registration Code is "+becomeAnAgentInfo.AgentRegistrationCode);
            }
            catch (Exception)
            {

            }
            ViewBag.ActivationCode = activationCode;
            return RedirectToAction("ValidateLinkTemp", new { @AgentRegistrationCode = becomeAnAgentInfo.AgentRegistrationCode, @ActivationCode = activationCode });
            //return PartialView();
        }

        public ActionResult ValidateLinkTemp(string AgentRegistrationCode, string ActivationCode)
        {
            var becomeAnAgentInfo = dbContext.BecomeAnAgent.Where(x => x.ActivationCode == ActivationCode).FirstOrDefault() ?? new BecomeAnAgent();
            if (becomeAnAgentInfo.AgentRegistrationCode == AgentRegistrationCode)
            {
                return RedirectToAction("Index", "AgentDetails", new { area = "agent", agentRegistrationCode = AgentRegistrationCode });
            }
            return RedirectToAction("ValidateLink", new { @activationCode= ActivationCode });
        }

        [HttpPost]
        public ActionResult ValidateLink(string AgentRegistrationCode,string ActivationCode)
        {
            var becomeAnAgentInfo = dbContext.BecomeAnAgent.Where(x => x.ActivationCode == ActivationCode).FirstOrDefault() ?? new BecomeAnAgent();
            if (becomeAnAgentInfo.AgentRegistrationCode==AgentRegistrationCode)
            {
                return RedirectToAction("Index", "AgentDetails", new { area="agent", agentRegistrationCode=AgentRegistrationCode});
            }
            ModelState.AddModelError("AgentRegistrationCode", "Please anter a valid registration code");
            ViewBag.ActivationCode = ActivationCode;
            return View();
        }

        public ActionResult ActivateAgent(string ActivationCode) {

            var result = dbContext.AgentLogin.Where(x => x.ActivationCode == ActivationCode).FirstOrDefault();

            result.IsActive = false;

            dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return RedirectToAction("Login", "AgentLogin");
        }


    }
}