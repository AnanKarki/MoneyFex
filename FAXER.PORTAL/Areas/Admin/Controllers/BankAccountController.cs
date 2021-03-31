using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class BankAccountController : Controller
    {
        // GET: Admin/BankAccount
        BankAccountServices Service = new BankAccountServices();
        CommonServices CommonService = new CommonServices();
        public ActionResult Index(string message = "", string Country = "", string AccountNo = "", string LabelName = "", string LabelValue = "", int? page = null)
        {
            if (message == "addSuccess")
            {
                ViewBag.Message = "Bank Account Added Successfully !";
                ViewBag.ToastrVal = 4;
                message = "";
            }
            else if (message == "addFailure")
            {
                ViewBag.Message = "Something went wrong. Please try again !";
                ViewBag.ToastrVal = 0;
                message = "";
            }
            else if (message == "updateSuccess")
            {
                ViewBag.Message = "Bank Account updated successfully !";
                ViewBag.ToastrVal = 4;
                message = "";
            }
            else if (message == "updateFailure")
            {
                ViewBag.Message = "Something went wrong. Please contact Administrator !";
                ViewBag.ToastrVal = 0;
                message = "";
            }
            else if (message == "delSuccess")
            {
                ViewBag.Message = "Deleted Successfully !";
                ViewBag.ToastrVal = 4;
                message = "";

            }
            else if (message == "delFailure")
            {
                ViewBag.Message = "Something went wrong. Please try again!";
                ViewBag.ToastrVal = 0;
                message = "";

            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var Countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            ViewBag.AccountNo = AccountNo;
            ViewBag.LabelName = LabelName;
            ViewBag.LabelValue = LabelValue;
            IPagedList<BankAccountViewModel> vm = Service.getList(Country, AccountNo, LabelName, LabelValue).ToPagedList(pageNumber, pageSize);

            return View(vm);
        }


        [HttpGet]
        public ActionResult AddNewBankAccount()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult AddNewBankAccount([Bind(Include = AddNewBankAccountViewModel.BindProperty)]AddNewBankAccountViewModel model)
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            if (ModelState.IsValid)
            {
                if (model.TransferType == DB.TransferTypeForBankAccount.Select)
                {
                    ModelState.AddModelError("TransferType", "Select Transfer Type");
                    return View(model);
                }
                bool save = Service.saveData(model);
                if (save)
                {
                    return RedirectToAction("Index", "BankAccount", new { @message = "addSuccess" });
                }
                return RedirectToAction("Index", "BankAccount", new { @message = "addFailure" });
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult UpdateBankAccount(int id)
        {
            var vm = Service.getInfo(id);
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name", vm.Country);
            return View(vm);

        }

        [HttpPost]
        public ActionResult UpdateBankAccount([Bind(Include = AddNewBankAccountViewModel.BindProperty)]AddNewBankAccountViewModel model)
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");

            if (ModelState.IsValid)
            {
                if (model.TransferType == DB.TransferTypeForBankAccount.Select)
                {
                    ModelState.AddModelError("TransferType", "Select Transfer Type");
                    return View(model);
                }
                bool update = Service.updateData(model);
                if (update)
                {
                    return RedirectToAction("Index", "BankAccount", new { @message = "updateSuccess" });
                }
                return RedirectToAction("Index", "BankAccount", new { @message = "updateFailure" });
            }
            return View(model);
        }

        [HttpGet]
        public JsonResult DeleteBankAccount(int id)
        {
            if (id != 0)
            {
                bool delete = Service.deleteBankAccount(id);
                if (delete)
                {
                    return Json(new
                    {
                        Data = true,
                        Message = "Deleted Sucessfully"
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new
            {
                Data = false,
                Message = "Something went wrong. Please try again!"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}