using FAXER.PORTAL.Areas.Agent.Controllers;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class SenderRegisterWalletsForFamilyandFriendsController : Controller
    {

        SRegisterKiiPayWallet _registerKiiPayWalletServices = null;
        SSenderRegisteredWallets _senderRegisteredWalletsServices = null;
        public SenderRegisterWalletsForFamilyandFriendsController()
        {
            _senderRegisteredWalletsServices = new SSenderRegisteredWallets();
            _registerKiiPayWalletServices = new SRegisterKiiPayWallet();
        }
        // GET: SenderRegisterWalletsForFamilyandFriends
        [HttpGet]
        public ActionResult SenderKiiPayWalletUserRegistration()
        {
            ViewBag.Countries = new SelectList(GetCountries(), "Code", "Name");
            return View();
        }
        [HttpPost]
        public ActionResult SenderKiiPayWalletUserRegistration([Bind(Include = SenderKiiPayWalletUserRegistrationViewModel.BindProperty)]SenderKiiPayWalletUserRegistrationViewModel Vm)
        {
            ViewBag.Countries = new SelectList(GetCountries(), "Code", "Name");
            var senderData=Common.FaxerSession.LoggedUser;
            if (ModelState.IsValid)
            {
                _registerKiiPayWalletServices.RegisterFamilyAndFriendsKiiPayWallet(Vm);
                string[] FullName = Vm.FullName.Split(' ');
                KiiPayPersonalWalletInformation KiiPayWalletUserRegistration = new KiiPayPersonalWalletInformation
                {
                    Id = Vm.Id,
                    CardUserCountry = Vm.Country,
                    MobileNo = Vm.MobileNo,
                    CardUserDOB = DateTime.Now,
                    CardUserEmail = Vm.Email,
                };
                if (FullName.Length < 3)
                {
                    KiiPayWalletUserRegistration.FirstName = FullName[0];
                    KiiPayWalletUserRegistration.LastName = FullName[FullName.Length -1];

                }
                else {
                    KiiPayWalletUserRegistration.FirstName = FullName[0];
                    KiiPayWalletUserRegistration.MiddleName = FullName[1];
                    KiiPayWalletUserRegistration.LastName = FullName[2];
                }
                
                var AddKiiPayWalletUserRegistration = _senderRegisteredWalletsServices.Add(KiiPayWalletUserRegistration);
                if (AddKiiPayWalletUserRegistration != null)
                {
                    SenderKiiPayPersonalAccount SenderKiiPayPersonalAccount = new SenderKiiPayPersonalAccount();
                    SenderKiiPayPersonalAccount.KiiPayPersonalWalletId = KiiPayWalletUserRegistration.Id;
                    SenderKiiPayPersonalAccount.SenderId = senderData.Id;
                    SenderKiiPayPersonalAccount.KiiPayAccountIsOF = KiiPayAccountIsOF.Family;
                    _senderRegisteredWalletsServices.AddPersonalAccount(SenderKiiPayPersonalAccount);
                }

                return RedirectToAction("SenderRegisterKiiPayEcardSuccess");
            }
            return View(Vm);
        }
        public List<CountryDropDownVm> GetCountries()
        {

            var result = (from c in Common.Common.GetCountries()
                          select new CountryDropDownVm()
                          {
                              Code = c.CountryCode,
                              Name = c.CountryName
                          }).ToList();
            return result;

        }
        [HttpGet]
        public ActionResult SenderRegisterKiiPayEcardSuccess([Bind(Include = SenderKiiPayWalletUserRegistrationViewModel.BindProperty)]SenderKiiPayWalletUserRegistrationViewModel Vm)
        {
        
            return View(Vm);
        }

        public JsonResult   GetCountryPhoneCode(string CountryCode) {

            return Json(new
            {
                CountryPhoneCode = Common.Common.GetCountryPhoneCode(CountryCode)

            }, JsonRequestBehavior.AllowGet);
        }

    }
}