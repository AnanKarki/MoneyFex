using FAXER.PORTAL.Areas.Agent.Models;
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
    public class SenderPersonalKeyPayBankAccountController : Controller
    {
        SUserBankAccount _userBankAccountServices = null;
        
        public SenderPersonalKeyPayBankAccountController()
        {
            _userBankAccountServices = new SUserBankAccount();
        }

        // GET: SenderPersonalKeyPayBankAccount

       
        

        [HttpGet]
        public ActionResult Index()
        {
            List<SenderSavedBanklAccountVM> vm = new List<SenderSavedBanklAccountVM>();
            vm = (from c in _userBankAccountServices.List().Data.Where(x => x.UserId == Common.FaxerSession.LoggedUser.Id && x.UserType == DB.Module.Faxer).ToList()
                  select new SenderSavedBanklAccountVM
                  {
                      AccountNumber = c.AccountNumber,
                      BankName = c.BankName,
                      Id = c.Id,
                      FormattedAccNo = FormatAccNo(c.AccountNumber)

                  }).ToList();


            return View(vm);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = SenderSavedBanklAccountVM.BindProperty)] SenderSavedBanklAccountVM model)
        {
            List<SenderSavedBanklAccountVM> vm = new List<SenderSavedBanklAccountVM>();
            vm = (from c in _userBankAccountServices.List().Data.Where(x => x.UserId == Common.FaxerSession.LoggedUser.Id && x.UserType == DB.Module.Faxer).ToList()
                  select new SenderSavedBanklAccountVM
                  {
                      AccountNumber = c.AccountNumber,
                      BankName = c.BankName,
                      Id = c.Id,
                      FormattedAccNo = FormatAccNo(c.AccountNumber)

                  }).ToList();
          
            return View(vm);
        }

     
        public ActionResult Delete(int id)
        {
            var data = _userBankAccountServices.List().Data.Where(x => x.Id == id).FirstOrDefault();
            var result = _userBankAccountServices.Remove(data);
            return RedirectToAction("Index");
        }


        private string FormatAccNo(string accountno) {
            // 12356456789
            string formattedAccNO = accountno;
            try
            {
                formattedAccNO = accountno.Substring(accountno.Length - 5, 5);
            }
            catch (Exception)
            {
                
            }
           
            return "XXX-" + formattedAccNO;

        }

        public ActionResult AddNewBankAccount( string Country= "" )
        {
            var country = Common.Common.GetCountries();
            var Banks = GetBanks(Country);
            var Branches = GetBankBranches();
            ViewBag.Countries = new SelectList(country, "CountryCode", "CountryName" , Country);
            ViewBag.Banks = new SelectList(Banks, "BankId", "BankName");
            ViewBag.Branches = new SelectList(Branches, "BranchId", "BranchName");
            SenderAddNewBankVM vm = new SenderAddNewBankVM();
            ViewBag.IsPinCodeSend = 0;
            return View(vm);
        }


        [HttpPost]

        public ActionResult AddNewBankAccount([Bind(Include = SenderAddNewBankVM.BindProperty)]SenderAddNewBankVM model)
          
        {
            var country = Common.Common.GetCountries();
            var Banks = GetBanks(model.CountryCode);
            var Branches = GetBankBranches(model.BankId);
            ViewBag.Countries = new SelectList(country, "CountryCode", "CountryName");
            ViewBag.Banks = new SelectList(Banks, "BankId", "BankName");
            ViewBag.Branches = new SelectList(Branches, "BranchId", "BranchName");

            ViewBag.IsPinCodeSend = 0;
            if (ModelState.IsValid)
            {
                _userBankAccountServices.SetBankId(model.BankId);
                if (string.IsNullOrEmpty(model.UserEnterPinCode))
                {
                    model.PinCode = GetMobilePin();
                    ViewBag.IsPinCodeSend = 1;
                    return View(model);
                }
                else
                {
                    //  check if the pincode in session and model.enerpincode are equal if not then show error message in popup
                    string sentPinCode = _userBankAccountServices.GetMobilePinCode();

                    if(model.UserEnterPinCode != sentPinCode)
                    {
                        ViewBag.IsPinCodeSend = 1;
                        ModelState.AddModelError("UserEnterPinCode", " Invalid Pincode");
                        return View(model);
                    }
                }
                SavedBank SavedBank = new SavedBank
                {
                    AccountNumber = model.AccountNumber,
                    isDeleted = false,
                    Address = model.Address,
                    BankName = getbanknamebybankid(model.BankId),
                    //BankName = _userBankAccountServices.GetBankName(model.BankId),
                    BranchCode = model.BranchCode,
                    BranchName = getbranchnamebybranchid(model.BranchId),
                    Country = Common.Common.GetCountryName(model.CountryCode),
                    CreatedDate = DateTime.Now,
                    OwnerName = model.OwnerName,
                    UserId= Common.FaxerSession.LoggedUser.Id,
                    UserType = Module.Faxer,
                };
                var result = _userBankAccountServices.Add(SavedBank);
                return RedirectToAction("AddBAnkAccountSuccess");
            }
            return View(model);

            //return RedirectToAction("AddBAnkAccountSuccess", "SenderPersonalKeyPayBankAccount");
        }

        private string getbranchnamebybranchid(int branchId)
        {
            SBankAndBranch _bankAndBranchServices = new SBankAndBranch();
            var result = _bankAndBranchServices.GetBankBranches().Where(x => x.Id == branchId).Select(x => x.BranchName).FirstOrDefault();
            return result;
            
        }

        private string getbanknamebybankid(int bankId)
        {
            
            SBankAndBranch _bankAndBranchServices = new SBankAndBranch();
            var result = _bankAndBranchServices.GetBanks().Where(x => x.Id == bankId).Select(x => x.Name).FirstOrDefault();
            return result;
        }

        public ActionResult AddBAnkAccountSuccess()
        {
            Common.FaxerSession.SentMobilePinCode = null;
            return View();
        }



        public string GetMobilePin()
        {
            //if session null generate code and return  else return value in session

            string code = "";
            if (Common.FaxerSession.SentMobilePinCode == null || Common.FaxerSession.SentMobilePinCode == "")
            {
                code = Common.Common.GenerateRandomDigit(6);
                _userBankAccountServices.SetMobilePinCode(code);

                SmsApi smsService = new SmsApi();
                var msg = smsService.GetPinCodeMsg(code);
                var phone = Common.FaxerSession.LoggedUser.CountryPhoneCode + Common.FaxerSession.LoggedUser.PhoneNo;
                smsService.SendSMS(phone, msg);
            }

            else
            {
                code = Common.FaxerSession.SentMobilePinCode;
            }
            
            string mobilePinCode = code;
            return mobilePinCode;
        }
        public class BankDropDown
        {

            public int BankId { get; set; }
            public string BankName { get; set; }

        }
        public List<BankDropDown> GetBanks(string Country="")
        {

            SBankAndBranch _bankAndBranchServices = new SBankAndBranch();
            //var result = new List<BankDropDown>();
            //var bank1 = new BankDropDown()
            //{
            //    BankId = 1,
            //    BankName = "agaga"
            //};
            //var bank2 = new BankDropDown()
            //{
            //    BankId = 2,
            //    BankName = "fasfdsdf"
            //};
            //result.Add(bank1);
            //result.Add(bank2);

            var result = (from c in _bankAndBranchServices.GetBanks().Where(x => x.CountryCode == Country)
                          select new BankDropDown()
                          {
                              BankId = c.Id,
                              BankName = c.Name
                          }).ToList();
            return result;
        }

        public class BankBranchDropDown
        {
            public int BranchId { get; set; }
            public string BranchName { get; set; }
        }
        public List<BankBranchDropDown> GetBankBranches(int bankId = 0)
        {

            SBankAndBranch _bankAndBranchServices = new SBankAndBranch();
            var result = new List<BankBranchDropDown>();
            //var bank1 = new BankBranchDropDown()
            //{
            //    BranchId = 1,
            //    BranchName = "babasad"
            //};
            //var bank2 = new BankBranchDropDown()
            //{
            //    BranchId = 2,
            //    BranchName = "masd"
            //};
            //result.Add(bank1);
            //result.Add(bank2);

            result = (from c in _bankAndBranchServices.GetBankBranches().Where(x => x.BankId == bankId).ToList()
                      select new BankBranchDropDown()
                      {
                          BranchId = c.Id,
                          BranchName = c.BranchName
                      }).ToList();
            return result;
        }



        public JsonResult GetBranches(int bankId)
        {

            var result = GetBankBranches(bankId);
            return Json(new {

                Data = result

            }, JsonRequestBehavior.AllowGet); 
        }


        public JsonResult GetBankCode(int BankId)
        {

            var bankCode = _userBankAccountServices.GetBankCode(BankId);
            return Json(new
            {
                BranchCode = bankCode.Code
            }, JsonRequestBehavior.AllowGet);

        }
    }
}