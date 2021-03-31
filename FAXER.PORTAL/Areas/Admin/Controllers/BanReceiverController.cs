using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using PagedList;
using System.Linq;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class BanReceiverController : Controller
    {
        BlackListedReceiverServcies _services = null;
        ReceiverServices _receiverServices = null;
        CommonServices _CommonServices = null;
        public BanReceiverController()
        {
            _services = new BlackListedReceiverServcies();
            _CommonServices = new CommonServices();
            _receiverServices = new ReceiverServices();
        }


        // GET: Admin/BanReceiver
        public ActionResult Index(string Country = "", int Service = 0, string ReceiverName = "",
            string BankOrProviderName = "", string AccountNumber = "", string MobileNo = "", int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            bool isBanned = true;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<ReceiverDetailsInfoViewModel> vm = _receiverServices.GetRecipients(isBanned).ToPagedList(pageNumber, pageSize);
            ViewBag.Service = Service;
            ViewBag.ReceiverName = ReceiverName;
            ViewBag.BankOrProviderName = BankOrProviderName;
            ViewBag.AccountNumber = AccountNumber;
            ViewBag.MobileNo = MobileNo;

            SetViewBagForCountriesBankAndWallet();
            if (Service != 0)
            {
                vm = vm.Where(x => x.Service == (Service)Service).ToPagedList(pageNumber, pageSize);
            }
            if (!string.IsNullOrEmpty(Country))
            {
                vm = vm.Where(x => x.ReceiverCountryFlag == Country.ToLower()).ToPagedList(pageNumber, pageSize);
            }
            if (!string.IsNullOrEmpty(ReceiverName))
            {
                ReceiverName = ReceiverName.Trim();
                vm = vm.Where(x => x.ReceiverName.ToLower().Contains(ReceiverName.ToLower())).ToPagedList(pageNumber, pageSize);
            }
            if (!string.IsNullOrEmpty(BankOrProviderName))
            {
                BankOrProviderName = BankOrProviderName.Trim();
                vm = vm.Where(x => x.BankMobileName.ToLower().Contains(BankOrProviderName.ToLower())).ToPagedList(pageNumber, pageSize);
            }
            if (!string.IsNullOrEmpty(AccountNumber))
            {
                AccountNumber = AccountNumber.Trim();
                vm = vm.Where(x => x.ReceiverAccountNo.ToLower().Contains(AccountNumber.ToLower())).ToPagedList(pageNumber, pageSize);
            }
            if (!string.IsNullOrEmpty(MobileNo))
            {
                MobileNo = MobileNo.Trim();
                vm = vm.Where(x => x.ReceiverPhoneNo.ToLower().Contains(MobileNo.ToLower())).ToPagedList(pageNumber, pageSize);
            }
            return View(vm);
        }
        public ActionResult BanAReceiver(int id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            SetViewBagForCountriesBankAndWallet();
            if (id != 0)
            {
                bool isBanned = true;
                ReceiverDetailsInfoViewModel vm = _receiverServices.GetRecipients(isBanned).Where(x => x.Id == id).FirstOrDefault();
                return View(vm);
            }
            return View();
        }
        [HttpPost]
        public ActionResult BanAReceiver([Bind(Include = ReceiverDetailsInfoViewModel.BindProperty)]ReceiverDetailsInfoViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetViewBagForCountriesBankAndWallet();
            if (ModelState.IsValid)
            {
                if (vm.Service == Service.BankAccount)
                {
                    if (vm.BankId == 0 || vm.BankId == null)
                    {
                        ModelState.AddModelError("BankId", "Select Bank");
                        return View(vm);
                    }
                    if (string.IsNullOrEmpty(vm.BankCode))
                    {
                        ModelState.AddModelError("BankCode", "Enter Bank Code");
                        return View(vm);
                    }

                }
                if (vm.Service == Service.MobileWallet)
                {
                    if (vm.MobileWalletProvider == 0 || vm.MobileWalletProvider == null)
                    {
                        ModelState.AddModelError("MobileWalletProvider", "Select Wallet Provider");
                        return View(vm);
                    }
                }
                if (vm.Id != 0)
                {
                    _services.Update(vm);
                }
                else
                {
                    _services.Add(vm);
                }
                return RedirectToAction("Index", "BanReceiver");
            }
            return View(vm);
        }
        public ActionResult Delete(int id)
        {
            _services.Delete(id);
            return RedirectToAction("Index", "BanReceiver");
        }
        public void SetViewBagForCountriesBankAndWallet()
        {
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");

            var Banks = _CommonServices.GetBanks();
            ViewBag.Banks = new SelectList(Banks, "Id", "Name");

            var MobileWallet = _CommonServices.GetWalletProvider();
            ViewBag.MobileWalletProvider = new SelectList(MobileWallet, "Id", "Name");
        }
    }
}