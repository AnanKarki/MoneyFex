using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using Microsoft.Office.Interop.Excel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ServiceSettingsController : Controller
    {
        CommonServices _CommonServices = null;
        ServiceSettingsServices _serviceSetting = null;
        BankServices _bankServices = null;
        public ServiceSettingsController()
        {
            _CommonServices = new CommonServices();
            _serviceSetting = new ServiceSettingsServices();
            _bankServices = new BankServices();
        }
        // GET: Admin/ServiceSettings
        public ActionResult Index(string sendingCountry = "", string receivingCountry = "", int? page = null, int PageSize = 10)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var countries = _CommonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(countries, "Code", "Name");
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);
            IPagedList<ServiceSettingViewModel> model = _serviceSetting.GetCurrentSetting(sendingCountry, receivingCountry).ToPagedList(pageNumber, pageSize);

            return View(model);
        }
        public ActionResult ViewCurrentSettingByCurrency()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            List<ServiceSettingViewModel> model = _serviceSetting.GetCurrentSettingByCurrency();

            return View(model);
        }
        public ActionResult CurrentSetting(int Id = 0, string sendingCountry = "", string receivingCountry = "", string receivngCurrecny = "", string sendingCurrency = "", bool IsBankChecked = false)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var countries = _CommonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(countries, "Code", "Name", sendingCountry);
            ViewBag.ReceivingCountries = new SelectList(countries, "Code", "Name", receivingCountry);

            var Currency = _CommonServices.GetCountryCurrencies();
            ViewBag.SendingCurrencies = new SelectList(Currency, "Code", "Name", sendingCurrency);
            ViewBag.ReceivingCurrencies = new SelectList(Currency, "Code", "Name", receivngCurrecny);

            ServiceSettingViewModel model = new ServiceSettingViewModel();
            ViewBag.ServiceTypes = new SelectList(_serviceSetting.GetServiceType(), "Id", "Name");
            model.Master = new TransferServiceMasterViewModel()
            {
                ServiceType = (int)TransferService.BankDeposit,
                Id = Id,
                SendingCurrrency = sendingCurrency,
                ReceivingCurrency = receivngCurrecny,
                SendingCountry = sendingCountry,
                ReceivingCountry = receivingCountry,
            };
            model.Details = GetTransferServicesEnumInList();
            model.BankDetails = _serviceSetting.GetBanks(receivingCountry, receivngCurrecny);
            ViewBag.ShowBankList = false;
            if (IsBankChecked == true)
            {
                model.Details[0].IsChecked = true;
                if (model.BankDetails.Count != 0)
                {
                    ViewBag.ShowBankList = true;
                }
            }
            if (Id != 0 && IsBankChecked == false)
            {
                model.Master = _serviceSetting.GetCurrentSetting().Where(x => x.Master.Id == Id).Select(x => x.Master).FirstOrDefault();
                var data = _serviceSetting.GetTransferServiceDetailList().Data.Where(x => x.TransferMasterId == model.Master.Id).ToList();

                if (data.Count() != 0)
                {
                    model.Master.ServiceType = (int)data.FirstOrDefault().ServiceType;
                }
                model.Details = (from c in model.Details
                                 join d in data on c.ServiceType equals d.ServiceType into cd
                                 from d in cd.DefaultIfEmpty()
                                 select new TransferServiceDetailsViewModel()
                                 {
                                     ImageUrl = c.ImageUrl,
                                     IsChecked = d == null ? false : true,
                                     ServiceType = c.ServiceType,
                                     TransferServiceMasterId = c.TransferServiceMasterId
                                 }).ToList();
                if (model.Details.Where(x => x.ServiceType == TransferService.BankDeposit).Select(x => x.IsChecked).FirstOrDefault() == true)
                {
                    model.BankDetails = _serviceSetting.GetBankDetails(model.Master.Id, model.Master.ReceivingCountry);
                    if (model.BankDetails.Count != 0)
                    {
                        ViewBag.ShowBankList = true;
                    }
                }
            }

            return View(model);

        }
        [HttpPost]
        public ActionResult CurrentSetting([Bind(Include = ServiceSettingViewModel.BindProperty)] ServiceSettingViewModel model)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var countries = _CommonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(countries, "Code", "Name");
            var Currency = _CommonServices.GetCountryCurrencies();
            ViewBag.SendingCurrencies = new SelectList(Currency, "Code", "Name");
            ViewBag.ReceivingCurrencies = new SelectList(Currency, "Code", "Name");
            ViewBag.ServiceTypes = new SelectList(_serviceSetting.GetServiceType(), "Id", "Name");
            //model.Details = GetTransferServicesEnumInList();
            ViewBag.ShowBankList = false;

            if (string.IsNullOrEmpty(model.Master.SendingCurrrency) == true ||
                string.IsNullOrEmpty(model.Master.SendingCountry) == true ||
                string.IsNullOrEmpty(model.Master.ReceivingCountry) == true ||
                string.IsNullOrEmpty(model.Master.ReceivingCurrency) == true)
            {
                model.Details = GetTransferServicesEnumInList();
                model.BankDetails = _serviceSetting.GetBanks(model.Master.ReceivingCountry, model.Master.ReceivingCurrency);

                if (model.BankDetails.Count != 0)
                {
                    ViewBag.ShowBankList = true;
                }

                return View(model);
            }

            model.Details.Where(x => x.ServiceType == (TransferService)model.Master.ServiceType).FirstOrDefault().IsChecked = true;

            if (model.Master.Id == 0)
            {
                _serviceSetting.AddServiceSetting(model);

            }
            else
            {
                _serviceSetting.UpdateServiceSetting(model);

            }
            return RedirectToAction("Index", "ServiceSettings");


        }

        public ActionResult CurrentSettingByCurrencey(int Id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Currency = _CommonServices.GetCountryCurrencies();
            ViewBag.SendingCurrencies = new SelectList(Currency, "Code", "Name");
            ViewBag.ReceivingCurrencies = new SelectList(Currency, "Code", "Name");
            ServiceSettingViewModel model = new ServiceSettingViewModel();
            ViewBag.ServiceTypes = new SelectList(_serviceSetting.GetServiceType(), "Id", "Name");
            model.Master = new TransferServiceMasterViewModel()
            {
                ServiceType = (int)TransferService.BankDeposit,
            };

            model.Details = GetTransferServicesEnumInList();

            if (Id != 0)
            {


                model.Master = _serviceSetting.GetCurrentSettingByCurrency().Where(x => x.Master.Id == Id).Select(x => x.Master).FirstOrDefault();

                var data = _serviceSetting.GetTransferServiceByCurrencyDetailList().Data.Where(x => x.TransferServiceByCurrencyMasterId == model.Master.Id).ToList();
                if (data.Count() != 0)
                {
                    model.Master.ServiceType = (int)data.FirstOrDefault().ServiceType;
                }
                model.Details = (from c in model.Details
                                 join d in data on c.ServiceType equals d.ServiceType into cd
                                 from d in cd.DefaultIfEmpty()
                                 select new TransferServiceDetailsViewModel()
                                 {
                                     ImageUrl = c.ImageUrl,
                                     IsChecked = d == null ? false : true,
                                     ServiceType = c.ServiceType,
                                     TransferServiceMasterId = c.TransferServiceMasterId
                                 }).ToList();
            }

            return View(model);

        }

        private List<TransferServiceDetailsViewModel> GetTransferServicesEnumInList()
        {
            List<TransferServiceDetailsViewModel> details = new List<TransferServiceDetailsViewModel>();
            details.Add(new TransferServiceDetailsViewModel()
            {
                ImageUrl = "/Content/icon/svg/bank.svg",
                ServiceType = TransferService.BankDeposit
            });
            details.Add(new TransferServiceDetailsViewModel()
            {
                ImageUrl = "/Content/icon/svg/cash-pickup.svg",
                ServiceType = TransferService.CahPickUp
            });
            details.Add(new TransferServiceDetailsViewModel()
            {
                ImageUrl = "/Content/icon/svg/kiipay.svg",
                ServiceType = TransferService.KiiPayWallet
            });
            details.Add(new TransferServiceDetailsViewModel()
            {
                ImageUrl = "/Content/icon/mobile.png",
                ServiceType = TransferService.OtherWallet
            });
            details.Add(new TransferServiceDetailsViewModel()
            {
                ImageUrl = "/Content/icon/svg/paybills.svg",
                ServiceType = TransferService.PayBills
            });

            return details;
        }

        [HttpPost]
        public ActionResult CurrentSettingByCurrencey([Bind(Include = ServiceSettingViewModel.BindProperty)] ServiceSettingViewModel model)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Currency = _CommonServices.GetCountryCurrencies();
            ViewBag.SendingCurrencies = new SelectList(Currency, "Code", "Name");
            ViewBag.ReceivingCurrencies = new SelectList(Currency, "Code", "Name");

            ViewBag.ServiceTypes = new SelectList(_serviceSetting.GetServiceType(), "Id", "Name");
            if (string.IsNullOrEmpty(model.Master.SendingCurrrency) == true || string.IsNullOrEmpty(model.Master.ReceivingCurrency) == true)
            {
                model.Details = GetTransferServicesEnumInList();
                return View(model);
            }
            //if (model.Details.Where(x => x.IsChecked == true).Count() == 0)
            //{
            //    model.Details = GetTransferServicesEnumInList();
            //    ModelState.AddModelError("Invalid", "Select atleast one Transfer Service");
            //    return View(model);
            //}
            model.Details.Where(x => x.ServiceType == (TransferService)model.Master.ServiceType).FirstOrDefault().IsChecked = true;

            if (model.Master.Id == 0)
            {
                _serviceSetting.AddServiceSettingByCurrency(model);
            }
            else
            {
                _serviceSetting.UpdateServiceSettingByCurrency(model);
            }
            return RedirectToAction("ViewCurrentSettingByCurrency", "ServiceSettings");
        }
        [HttpGet]
        public JsonResult Delete(int id)
        {
            if (id > 0)
            {
                _serviceSetting.Remove(id);
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
        public JsonResult DeleteByCurrency(int id)
        {

            if (id > 0)
            {
                _serviceSetting.RemoveByCurrency(id);
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
        public JsonResult GetBanks(string CountryCode, string Currency)
        {
            string currencyOfCountryCode = Common.Common.GetCountryCurrency(CountryCode);
            if (Currency == currencyOfCountryCode)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            var banks = _bankServices.GetBankList(CountryCode);
            return Json(banks, JsonRequestBehavior.AllowGet);
        }
    }
}