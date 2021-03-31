using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class IntroductoryFeeController : Controller
    {
        IntroductoryFeeServices _introductoryFee = null;
        STransferFeePercentageServices _transferFee = null;
        CommonServices _CommonServices = null;
        public IntroductoryFeeController()
        {
            _introductoryFee = new IntroductoryFeeServices();
            _transferFee = new STransferFeePercentageServices();
            _CommonServices = new CommonServices();
        }

        // GET: Admin/IntroductoryFee
        public ActionResult Index(string SendingCountry = "", string ReceivingCountry = "", int TransferType = 0, int TransferMethod = 0, int? page = null)

        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var sendingcountries = _transferFee.GetCountries();
            ViewBag.SendingCountries = new SelectList(sendingcountries, "Code", "Name");

            var receivingCountries = _transferFee.GetCountries();
            ViewBag.ReceivingCountries = new SelectList(receivingCountries, "Code", "Name");

            ViewBag.TransferType = TransferType;
            ViewBag.TransferMethod = TransferMethod;

            int StaffId = Common.StaffSession.LoggedStaff.StaffId;
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            IPagedList<IntroductoryFeeViewModel> vm = _introductoryFee.GetIntroductoryfee(SendingCountry, ReceivingCountry, TransferType, TransferMethod).Data.ToPagedList(pageNumber, pageSize);
            return View(vm);

        }


        [HttpGet]
        public ActionResult SetIntroductoryfee()
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var sendingcountries = _transferFee.GetCountries();
            ViewBag.SendingCountries = new SelectList(sendingcountries, "Code", "Name");

            var receivingCountries = _transferFee.GetCountries();
            ViewBag.ReceivingCountries = new SelectList(receivingCountries, "Code", "Name");

            var agents = _CommonServices.GetAgent();
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");

            IntroductoryFeeViewModel vm = new IntroductoryFeeViewModel();
            return View();

        }
        [HttpPost]
        public ActionResult SetIntroductoryfee([Bind(Include = IntroductoryFeeViewModel.BindProperty)]IntroductoryFeeViewModel vm)
        {
            var sendingcountries = _transferFee.GetCountries();
            ViewBag.SendingCountries = new SelectList(sendingcountries, "Code", "Name");

            var receivingCountries = _transferFee.GetCountries();
            ViewBag.ReceivingCountries = new SelectList(receivingCountries, "Code", "Name");

            var agents = _CommonServices.GetAgent();
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");

            if (ModelState.IsValid)
            {
                if (vm.TransferType == 0)
                {
                    ModelState.AddModelError("TransferType", "Select Transfer Type");
                    return View(vm);

                }
                if (vm.TransferMethod == 0)
                {
                    ModelState.AddModelError("TransferMethod", "Select Transfer Method");
                    return View(vm);

                }
                if (vm.Range == 0)
                {
                    ModelState.AddModelError("Range", "Select Range");
                    return View(vm);

                }
                if (vm.FeeType == 0)
                {
                    ModelState.AddModelError("FeeType", "Select Fee Type");
                    return View(vm);

                }
                _introductoryFee.CreateIntroductoryFee(vm);
                _introductoryFee.CreateIntroductoryFeeHistory(vm);
                return RedirectToAction("Index", "IntroductoryFee");
            }
            return View(vm);
        }


        [HttpGet]
        public ActionResult UpdateIntoductoryFee(int id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var sendingcountries = _transferFee.GetCountries();
            ViewBag.SendingCountries = new SelectList(sendingcountries, "Code", "Name");

            var receivingCountries = _transferFee.GetCountries();
            ViewBag.ReceivingCountries = new SelectList(receivingCountries, "Code", "Name");

            IntroductoryFeeViewModel vm = _introductoryFee.GetIntroductoryfeeById(id);
            var agents = _CommonServices.GetAgent(vm.SendingCountry);
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");
            return View(vm);
        }
        [HttpPost]
        public ActionResult UpdateIntoductoryFee([Bind(Include = IntroductoryFeeViewModel.BindProperty)]IntroductoryFeeViewModel vm)
        {

            var sendingcountries = _transferFee.GetCountries();
            ViewBag.SendingCountries = new SelectList(sendingcountries, "Code", "Name");

            var receivingCountries = _transferFee.GetCountries();
            ViewBag.ReceivingCountries = new SelectList(receivingCountries, "Code", "Name");
            var agents = _CommonServices.GetAgent(vm.SendingCountry);
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");
            if (ModelState.IsValid)
            {

                if (vm.TransferType == 0)
                {
                    ModelState.AddModelError("TransferType", "Select Transfer Type");
                    return View(vm);

                }
                if (vm.TransferMethod == 0)
                {
                    ModelState.AddModelError("TransferMethod", "Select Transfer Method");
                    return View(vm);

                }
                if (vm.Range == 0)
                {
                    ModelState.AddModelError("Range", "Select Range");
                    return View(vm);

                }
                if (vm.FeeType == 0)
                {
                    ModelState.AddModelError("FeeType", "Select Fee Type");
                    return View(vm);

                }
                _introductoryFee.UpdateIntroductoryFeePercentage(vm);
                _introductoryFee.CreateIntroductoryFeeHistory(vm);
                return RedirectToAction("Index", "IntroductoryFee");
            }
            return View(vm);
        }

        [HttpGet]
        public JsonResult Delete(int id)
        {
            if (id > 0)
            {

                var data = _introductoryFee.List().Data.Where(x => x.Id == id).FirstOrDefault();
                var result = _introductoryFee.Remove(data);
                return Json(new
                {
                    Data = true,
                    Message = "Deleted Sucessfully"
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    Data = false,
                    Message = "Something went wrong. Please try again!"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetIntroductoryfee(string sendingCountry = "", string receivingCounrty = "", int transferType = 0,
            int method = 0, int Range = 0, int FeeType = 0, string otherRange = "", int AgentId = 0)
        {
            var introductoryFee = _introductoryFee.GetFeeDetials(sendingCountry, receivingCounrty, transferType, method, Range, FeeType, AgentId);
            decimal IntroductoryFee = 0;
            if (introductoryFee != null)
            {
                IntroductoryFee = introductoryFee.Fee;
            }
            return Json(new
            {
                IntroductoryFee,

            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult IntroductoryFeeHistory(string SendingCountry = "", string ReceivingCountry = "", int TransferType = 0, int TransferMethod = 0, int Year = 0, int Month = 0, int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var sendingcountries = _transferFee.GetCountries(SendingCountry);
            ViewBag.SendingCountries = new SelectList(sendingcountries, "Code", "Name");

            var receivingCountries = _transferFee.GetCountries(ReceivingCountry);
            ViewBag.ReceivingCountries = new SelectList(receivingCountries, "Code", "Name");

            ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));
            ViewBag.TransferType = TransferType;
            ViewBag.Month = Month;
            ViewBag.TransferMethod = TransferMethod;

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            IPagedList<IntroductoryFeeHistoryViewModel> vm = _introductoryFee.GetIntroductoryfeeHistory(SendingCountry, ReceivingCountry, TransferType, TransferMethod, Year, Month).Data.ToPagedList(pageNumber, pageSize);

            return View(vm);
        }

    }
}