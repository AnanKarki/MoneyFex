using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.MyRegisteredKiiPayWallets
{
    public class SenderRegisteredWalletsController : Controller
    {
        // GET: SenderRegisteredWallets
        SSenderRegisteredWallets _senderRegisteredWalletsServices = null;
        public SenderRegisteredWalletsController()
        {
            _senderRegisteredWalletsServices = new SSenderRegisteredWallets();
        }
        [HttpGet]
        public ActionResult SenderWalletProfile(int SenderWalletId = 0)
        {
            SenderWalletProfileVM showDetails = new SenderWalletProfileVM();
            ViewBag.Wallets = new SelectList(Wallets(), "Id", "FirstName");

            if (SenderWalletId == 0)
            {
                ViewBag.Wallets = new SelectList(Wallets(), "Id", "FirstName");
            }
            else
            {
                ViewBag.Wallets = new SelectList(Wallets(), "Id", "FirstName", SenderWalletId);
                var WalletInfo = _senderRegisteredWalletsServices.List().Data.Where(x => x.Id == SenderWalletId).FirstOrDefault();


                showDetails.Id = WalletInfo.Id;
                showDetails.City = WalletInfo.CardUserCity;
                showDetails.Name = WalletInfo.FirstName + " " + WalletInfo.MiddleName + "" + WalletInfo.LastName;
                showDetails.Country = Common.Common.GetCountryName(WalletInfo.CardUserCountry);
                showDetails.EmailAddress = WalletInfo.CardUserEmail;
                showDetails.WalletBallanceAvaiable = Common.Common.GetCurrencySymbol(WalletInfo.CardUserCountry) + " " + WalletInfo.CurrentBalance;
                showDetails.WalletStatus = Enum.GetName(typeof(CardStatus), WalletInfo.CardStatus);
                showDetails.MobileNo = WalletInfo.MobileNo;

                showDetails.WalletId = SenderWalletId;
            }

            Common.FaxerSession.BackButtonURL = Request.Url.ToString();
            return View(showDetails);

        }

        [HttpPost]
        public ActionResult SenderWalletProfile([Bind(Include = Models.SenderWalletProfileVM.BindProperty)] Models.SenderWalletProfileVM vm)
        {
            ViewBag.Wallets = new SelectList(Wallets(), "Id", "FirstName");
            if (ModelState.IsValid)
            {
                return RedirectToAction("SenderUpdateThisWallet");
            }
            return View(vm);
        }

        public ActionResult Delete(int id)
        {
            var data = _senderRegisteredWalletsServices.List().Data.Where(x => x.Id == id).FirstOrDefault();
            data.IsDeleted = true;
            _senderRegisteredWalletsServices.Update(data);
            return RedirectToAction("SenderWalletProfile");
        }

        public List<KiiPayPersonalWalletInformation> Wallets()
        {
            var SenderId = FaxerSession.LoggedUser.Id;
            var SendingWallets = (from c in _senderRegisteredWalletsServices.List().Data.ToList()
                                  join d in _senderRegisteredWalletsServices.ListofSender().Data.Where(x => x.SenderId == SenderId) on c.Id equals d.KiiPayPersonalWalletId
                                  select c).ToList();
            return SendingWallets;

        }

        [HttpGet]
        public ActionResult SenderUpdateThisWallet(int WalletId)
        {
            SenderWalletProfileVM vm = new SenderWalletProfileVM();
            if (WalletId == 0)
            {

                TempData["NoWalletSelected"] = "Please a select Wallet to update";
                return View(vm);
            }
            if (Request.UrlReferrer != null)
            {

                Common.FaxerSession.BackButtonURL = Request.UrlReferrer.ToString();
            }
            if (WalletId != 0)
            {
                var countries = Common.Common.GetCountries();

                var data = (from c in _senderRegisteredWalletsServices.List().Data.Where(x => x.Id == WalletId).ToList()
                            join d in countries on c.CardUserCountry equals d.CountryCode
                            select new SenderWalletProfileVM()
                            {
                                Id = c.Id,
                                Address = c.Address1,
                                City = c.CardUserCity,
                                MobileNo = c.MobileNo,
                                EmailAddress = c.CardUserEmail,

                            }).FirstOrDefault();


                ViewBag.WalletId = data.Id;
                return View(data);

            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult SenderUpdateThisWallet([Bind(Include = Models.SenderWalletProfileVM.BindProperty)]SenderWalletProfileVM vm)
        {


            ViewBag.IsPinCodeSend = 0;
            if (string.IsNullOrEmpty(vm.UserEnterPinCode))
            {
                vm.Code = GetMobilePin();
                ViewBag.IsPinCodeSend = 1;
                return View(vm);
            }
            else
            {
                //  check if the pincode in session and model.enerpincode are equal if not then show error message in popup
                string sentPinCode = _senderRegisteredWalletsServices.GetMobilePinCode();

                if (vm.UserEnterPinCode != sentPinCode)
                {
                    ViewBag.IsPinCodeSend = 1;
                    ModelState.AddModelError("UserEnterPinCode", " Invalid Pincode");
                    return View(vm);
                }
                else
                {
                    var data = _senderRegisteredWalletsServices.List().Data.Where(x => x.Id == vm.Id).FirstOrDefault();

                    data.CardUserCity = vm.City;
                    data.CardUserTel = vm.MobileNo;
                    data.CardUserEmail = vm.EmailAddress;
                    data.Address1 = vm.Address;

                    _senderRegisteredWalletsServices.Update(data);
                    return RedirectToAction("SenderWalletProfile", "SenderRegisteredWallets");
                }
            }





        }


        public string GetMobilePin()
        {
            //if session null generate code and return else return value in session

            string code = "";
            if (Common.FaxerSession.SentMobilePinCode == null || Common.FaxerSession.SentMobilePinCode == "")
            {
                code = Common.Common.GenerateRandomDigit(6);
                _senderRegisteredWalletsServices.SetMobilePinCode(code);
            }

            else
            {
                code = Common.FaxerSession.SentMobilePinCode;
            }

            string mobilePinCode = code;
            return mobilePinCode;
        }
    }
}