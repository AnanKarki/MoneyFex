using FAXER.PORTAL.Areas.Admin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class AdminCommonController : Controller
    {
        CommonServices _commonServices = null;
        public AdminCommonController()
        {
            _commonServices = new CommonServices();
        }
        // GET: Admin/AdminCommon
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetCountryByCurrency(string currency)
        {
            var countries = _commonServices.GetCountries().ToList();
            if (!string.IsNullOrEmpty(currency))
            {
                countries = countries.Where(x => x.CountryCurrency == currency).ToList();
            }
            return Json(new
            {
                Data = countries
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSenders(SelectSearchParam param)
        {
            var senderInformations = new List<SenderListDropDown>();
            if (param.isBusiness)
            {
                senderInformations = _commonServices.GetBusinessSenderList();
            }
            else
            {
                senderInformations = _commonServices.GetSenderList().ToList();
            }
            if (param.country != "All" && !string.IsNullOrEmpty(param.country))
            {
                senderInformations = senderInformations.Where(x => x.Country == param.country.Trim()).ToList();
            }
            if (param.country == "All" && !string.IsNullOrEmpty(param.Currecny))
            {
                var country = _commonServices.getCountryCodeFromCurrency(param.Currecny);
                senderInformations = senderInformations.Where(x => x.Country == country).ToList();

            }
            if (param.query == null)
            {
                param.query = "";
            }
            var senders = (from c in senderInformations.Where(x => x.senderName.ToLower().Contains(param.query.ToLower()))
                           select new SelectDropDownVm()
                           {
                               Id = c.senderId,
                               text = c.senderName
                           }).ToList();
            senders.Add(new SelectDropDownVm()
            {
                Id = 0,
                text = "All"
            });
            var results = senders;
            return Json(new
            {
                results
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAuxSenders(SelectSearchParam param)
        {
            var senderInformations = _commonServices.GetSenderRegisteredByAuxAgent(param.country, param.City).ToList();
            if (param.country == "All" && !string.IsNullOrEmpty(param.Currecny))
            {
                var country = _commonServices.getCountryCodeFromCurrency(param.Currecny);
                senderInformations = senderInformations.Where(x => x.Country == country).ToList();
            }
            if (param.query == null)
            {
                param.query = "";
            }
            var senders = (from c in senderInformations.Where(x => x.senderName.ToLower().Contains(param.query.ToLower()))
                           select new SelectDropDownVm()
                           {
                               Id = c.senderId,
                               text = c.senderName
                           }).ToList();
            senders.Add(new SelectDropDownVm()
            {
                Id = 0,
                text = "All"
            });
            var results = senders;
            return Json(new
            {
                results
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAgents(SelectSearchParam param)
        {
            var agentInformation = _commonServices.GetAgent(param.country, param.isAuxAgent, param.City);
            if (!string.IsNullOrEmpty(param.Currecny))
            {
                var country = _commonServices.getCountryCodeFromCurrency(param.Currecny);
                agentInformation = agentInformation.Where(x => x.AgentCountry == country).ToList();
            }
            if (param.query == null)
            {
                param.query = "";
            }
            var agents = (from c in agentInformation.Where(x => x.AgentName.ToLower().Contains(param.query.ToLower()))
                          select new SelectDropDownVm()
                          {
                              Id = c.AgentId,
                              text = c.AgentName
                          }).ToList();
            agents.Add(new SelectDropDownVm()
            {
                Id = 0,
                text = "All"
            });

            var results = agents;
            return Json(new
            {
                results
            }, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetAgentAccountNumber(int agentId = 0)
        {
            string Accountnumber = _commonServices.GetAgentAccNo(agentId);
            return Json(new
            {
                Data = Accountnumber
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCountryPhoneCountry(string countrycode = "")
        {
            string phoneCode = Common.Common.GetCountryPhoneCode(countrycode);
            return Json(new
            {
                PhoneCode = phoneCode
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStaffAccToCountryAndCity(string countryCode = "", string city = "")
        {
            var staffs = _commonServices.GetFilteredStaffList(countryCode, city);
            return Json(new
            {
                Data = staffs
            }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetCityByCountry(string countryCode = "")
        {
            var cities = _commonServices.GetCities(countryCode);
            return Json(new
            {
                Data = cities
            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetPagignationButton(int TotalNumberOfpage, int CurrentpageCount, bool isGoToNextPage)
        {
            int count = 0;
            int PageNumber = 0;
            bool IsStartCount = false;
            bool IsEndCount = false;
            if (isGoToNextPage)
            {
                var difference = TotalNumberOfpage - CurrentpageCount;
                if (difference < 10)
                {
                    count = CurrentpageCount + difference;
                    IsEndCount = true;
                }
                else
                {
                    count = CurrentpageCount + 10;
                }
                PageNumber = CurrentpageCount + 1;

            }
            else
            {
                if (CurrentpageCount != 0)
                {
                    int remainder = (CurrentpageCount % 10);
                    if (remainder == 0)
                    {
                        count = CurrentpageCount - 10;
                        if (count == 0)
                        {
                            //count = 10;
                            PageNumber = count + 1;
                        }
                        PageNumber = count - 10 + 1;

                    }
                    else
                    {

                        count = CurrentpageCount - remainder;

                        PageNumber = count - 10 + 1;
                    }
                    CurrentpageCount = PageNumber - 1;


                    //if (count == 0)
                    //{
                    //    count = 10;
                    //    PageNumber = count + 1;
                    //}
                    //else
                    //{
                    //    PageNumber = CurrentpageCount - 20 + 1;

                    //}
                    if (PageNumber == 1)
                    {
                        IsStartCount = true;
                    }
                }
            }
            return Json(new
            {
                CurrentpageCount = CurrentpageCount,
                Count = count,
                PageNumber = PageNumber,
                IsStartCount = IsStartCount,
                IsEndCount = IsEndCount,
            }, JsonRequestBehavior.AllowGet);
        }

    }

}